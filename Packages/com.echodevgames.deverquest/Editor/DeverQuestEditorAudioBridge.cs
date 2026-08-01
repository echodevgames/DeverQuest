//----- DeverQuestEditorAudioBridge.cs START -----

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestEditorAudioChannel
    {
        Music = 0,
        Ambience = 1
    }

    /// <summary>
    /// Provides two logical long-form audio channels over Unity's internal
    /// editor preview transport.
    ///
    /// Unity exposes global preview stop controls rather than dependable
    /// per-clip transport controls. DeverQuest therefore snapshots both
    /// logical channels, stops the native preview transport, and rebuilds
    /// only the channels that should remain active after each user action.
    /// This allows Music and Ambience to be controlled independently while
    /// also preventing abandoned preview clips from accumulating.
    /// </summary>
    [InitializeOnLoad]
    internal static class DeverQuestEditorAudioBridge
    {
        private sealed class ChannelState
        {
            public AudioClip clip;
            public bool loop;
            public bool paused;
            public int sample;
            public double startedAt;
            public float volume = 1f;
        }

        private const BindingFlags Flags =
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        private const double CueCompletionGraceSeconds = 0.15d;

        private static readonly Type AudioUtilType =
            typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");

        private static readonly MethodInfo PlayMethod =
            FindMethod(
                new[] { "PlayPreviewClip", "PlayClip" },
                method => method.GetParameters().Any(
                    parameter =>
                        parameter.ParameterType == typeof(AudioClip)));

        private static readonly MethodInfo StopAllMethod =
            FindMethod(
                new[] { "StopAllPreviewClips", "StopAllClips" },
                method => method.GetParameters().Length == 0);

        private static readonly MethodInfo SetVolumeMethod =
            FindMethod(
                new[] { "SetPreviewClipVolume", "SetClipVolume" },
                method => method.GetParameters().Any(
                    parameter => parameter.ParameterType == typeof(float)));

        private static readonly ChannelState MusicState =
            new ChannelState();

        private static readonly ChannelState AmbienceState =
            new ChannelState();

        private static AudioClip cueClip;
        private static bool cueActive;
        private static double cueExpectedEnd;

        static DeverQuestEditorAudioBridge()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            AssemblyReloadEvents.beforeAssemblyReload -= Shutdown;
            AssemblyReloadEvents.beforeAssemblyReload += Shutdown;

            EditorApplication.quitting -= Shutdown;
            EditorApplication.quitting += Shutdown;
        }

        public static bool IsAvailable =>
            AudioUtilType != null &&
            PlayMethod != null &&
            StopAllMethod != null;

        public static bool VolumeSupported =>
            PlayMethodAcceptsVolume || SetVolumeMethod != null;

        public static bool IndependentVolumeSupported =>
            PlayMethodAcceptsVolume;

        /// <summary>
        /// DeverQuest can determine logical playback completion from the
        /// clip duration even when Unity does not expose a reliable native
        /// per-clip status query.
        /// </summary>
        public static bool PlaybackStatusSupported => IsAvailable;

        public static bool Play(
            DeverQuestEditorAudioChannel channel,
            AudioClip clip,
            bool loop,
            float volume)
        {
            if (clip == null || !IsAvailable)
            {
                return false;
            }

            CapturePositions();
            ClearCue();

            ChannelState state = GetState(channel);
            state.clip = clip;
            state.loop = loop;
            state.paused = false;
            state.sample = 0;
            state.startedAt = EditorApplication.timeSinceStartup;
            state.volume = Mathf.Clamp01(volume);

            if (RebuildNativeChannels())
            {
                return true;
            }

            ClearState(state);
            RebuildNativeChannels();
            return false;
        }

        public static bool PlayCue(
            AudioClip clip,
            float volume)
        {
            if (clip == null || !IsAvailable)
            {
                return false;
            }

            // Rebuild the two long-form channels first. This removes an
            // earlier cue or abandoned native preview clip without changing
            // either channel's logical position.
            CapturePositions();
            ClearCue();

            if (!RebuildNativeChannels())
            {
                return false;
            }

            cueClip = clip;
            cueActive = true;
            cueExpectedEnd =
                EditorApplication.timeSinceStartup +
                Math.Max(0.05d, clip.length) +
                CueCompletionGraceSeconds;

            if (InvokePlay(clip, 0, false, volume))
            {
                return true;
            }

            ClearCue();
            return false;
        }

        public static void Pause(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null || state.paused)
            {
                return;
            }

            CapturePositions();
            state.paused = true;
            ClearCue();
            RebuildNativeChannels();
        }

        public static void Resume(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null || !state.paused)
            {
                return;
            }

            CapturePositions();
            state.paused = false;
            state.startedAt = EditorApplication.timeSinceStartup;
            ClearCue();
            RebuildNativeChannels();
        }

        public static bool Stop(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null)
            {
                return false;
            }

            CapturePositions();
            ClearState(state);
            ClearCue();
            RebuildNativeChannels();
            return true;
        }

        public static void StopAll()
        {
            StopAllNative();
            ClearState(MusicState);
            ClearState(AmbienceState);
            ClearCue();
        }

        public static bool IsPlaying(
            DeverQuestEditorAudioChannel channel)
        {
            ChannelState state = GetState(channel);
            if (state.clip == null || state.paused)
            {
                return false;
            }

            if (!state.loop && HasReachedEnd(state))
            {
                return false;
            }

            return true;
        }

        public static AudioClip GetClip(
            DeverQuestEditorAudioChannel channel)
        {
            return GetState(channel).clip;
        }

        public static void SetVolume(
            DeverQuestEditorAudioChannel channel,
            float volume)
        {
            ChannelState state = GetState(channel);
            state.volume = Mathf.Clamp01(volume);

            if (state.clip == null)
            {
                return;
            }

            CapturePositions();
            ClearCue();

            if (PlayMethodAcceptsVolume)
            {
                RebuildNativeChannels();
                return;
            }

            // Unity versions that expose only a global preview gain cannot
            // apply separate Music and Ambience levels. Apply it only when
            // one long-form channel is active so the slider remains useful
            // without pretending to be an independent mixer.
            if (CountPlayingChannels() == 1)
            {
                InvokeGlobalVolume(state.volume);
            }
        }

        private static bool RebuildNativeChannels()
        {
            if (!IsAvailable)
            {
                return false;
            }

            NormalizeCompletedChannels();
            StopAllNative();

            bool success = true;
            double now = EditorApplication.timeSinceStartup;

            success &= RestartChannel(AmbienceState, now);
            success &= RestartChannel(MusicState, now);

            if (!success)
            {
                StopAllNative();
                return false;
            }

            if (!PlayMethodAcceptsVolume &&
                CountPlayingChannels() == 1)
            {
                ChannelState active =
                    IsLogicallyPlaying(MusicState)
                        ? MusicState
                        : AmbienceState;
                InvokeGlobalVolume(active.volume);
            }

            return true;
        }

        private static bool RestartChannel(
            ChannelState state,
            double now)
        {
            if (state.clip == null || state.paused)
            {
                return true;
            }

            int startSample = ClampSample(
                state.clip,
                state.sample,
                state.loop);

            if (!InvokePlay(
                    state.clip,
                    startSample,
                    state.loop,
                    state.volume))
            {
                return false;
            }

            state.sample = startSample;
            state.startedAt = now;
            return true;
        }

        private static void CapturePositions()
        {
            double now = EditorApplication.timeSinceStartup;
            CapturePosition(MusicState, now);
            CapturePosition(AmbienceState, now);
        }

        private static void CapturePosition(
            ChannelState state,
            double now)
        {
            if (state.clip == null || state.paused)
            {
                return;
            }

            if (state.clip.samples <= 1 || state.clip.frequency <= 0)
            {
                state.sample = 0;
                state.startedAt = now;
                return;
            }

            double elapsed = Math.Max(0d, now - state.startedAt);
            long advanced = (long)Math.Floor(
                elapsed * state.clip.frequency);
            long nextSample = (long)state.sample + advanced;

            if (state.loop)
            {
                nextSample %= state.clip.samples;
            }
            else if (nextSample >= state.clip.samples)
            {
                ClearState(state);
                return;
            }

            state.sample = (int)Math.Max(0L, nextSample);
            state.startedAt = now;
        }

        private static void NormalizeCompletedChannels()
        {
            double now = EditorApplication.timeSinceStartup;
            NormalizeCompletedChannel(MusicState, now);
            NormalizeCompletedChannel(AmbienceState, now);
        }

        private static void NormalizeCompletedChannel(
            ChannelState state,
            double now)
        {
            if (state.clip == null || state.paused || state.loop)
            {
                return;
            }

            if (HasReachedEnd(state, now))
            {
                ClearState(state);
            }
        }

        private static bool HasReachedEnd(ChannelState state)
        {
            return HasReachedEnd(
                state,
                EditorApplication.timeSinceStartup);
        }

        private static bool HasReachedEnd(
            ChannelState state,
            double now)
        {
            if (state.clip == null || state.loop || state.paused)
            {
                return false;
            }

            if (state.clip.frequency <= 0)
            {
                return now - state.startedAt >= state.clip.length;
            }

            double elapsed = Math.Max(0d, now - state.startedAt);
            long advanced = (long)Math.Floor(
                elapsed * state.clip.frequency);
            return (long)state.sample + advanced >=
                   state.clip.samples;
        }

        private static bool IsLogicallyPlaying(ChannelState state)
        {
            return state.clip != null &&
                   !state.paused &&
                   (state.loop || !HasReachedEnd(state));
        }

        private static int CountPlayingChannels()
        {
            int count = 0;
            if (IsLogicallyPlaying(MusicState))
            {
                count++;
            }
            if (IsLogicallyPlaying(AmbienceState))
            {
                count++;
            }
            return count;
        }

        private static ChannelState GetState(
            DeverQuestEditorAudioChannel channel)
        {
            return channel == DeverQuestEditorAudioChannel.Ambience
                ? AmbienceState
                : MusicState;
        }

        private static void Update()
        {
            NormalizeCompletedChannels();

            if (cueActive &&
                EditorApplication.timeSinceStartup >= cueExpectedEnd)
            {
                ClearCue();
            }
        }

        private static bool InvokePlay(
            AudioClip clip,
            int startSample,
            bool loop,
            float volume)
        {
            if (PlayMethod == null)
            {
                return false;
            }

            try
            {
                PlayMethod.Invoke(
                    null,
                    BuildArguments(
                        PlayMethod,
                        clip,
                        startSample,
                        loop,
                        Mathf.Clamp01(volume)));
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest] Editor preview playback failed: " +
                    exception.GetBaseException().Message);
                return false;
            }
        }

        private static int ClampSample(
            AudioClip clip,
            int sample,
            bool loop)
        {
            if (clip == null || clip.samples <= 1)
            {
                return 0;
            }

            if (loop)
            {
                int positive = Math.Max(0, sample);
                return positive % clip.samples;
            }

            return Mathf.Clamp(sample, 0, clip.samples - 1);
        }

        private static bool PlayMethodAcceptsVolume =>
            PlayMethod != null &&
            PlayMethod.GetParameters().Any(
                parameter => parameter.ParameterType == typeof(float));

        private static void InvokeGlobalVolume(float volume)
        {
            if (SetVolumeMethod == null)
            {
                return;
            }

            try
            {
                SetVolumeMethod.Invoke(
                    null,
                    BuildArguments(
                        SetVolumeMethod,
                        null,
                        0,
                        false,
                        Mathf.Clamp01(volume)));
            }
            catch
            {
                // Preview gain is optional and differs by Unity version.
            }
        }

        private static void StopAllNative()
        {
            InvokeNoArgument(StopAllMethod);
        }

        private static void Shutdown()
        {
            StopAll();
        }

        private static void ClearState(ChannelState state)
        {
            state.clip = null;
            state.loop = false;
            state.paused = false;
            state.sample = 0;
            state.startedAt = 0d;
            state.volume = 1f;
        }

        private static void ClearCue()
        {
            cueClip = null;
            cueActive = false;
            cueExpectedEnd = 0d;
        }

        private static MethodInfo FindMethod(
            string[] names,
            Func<MethodInfo, bool> predicate)
        {
            if (AudioUtilType == null)
            {
                return null;
            }

            return AudioUtilType
                .GetMethods(Flags)
                .Where(method => names.Contains(method.Name))
                .FirstOrDefault(predicate);
        }

        private static object[] BuildArguments(
            MethodInfo method,
            AudioClip clip,
            int startSample,
            bool loop,
            float volume)
        {
            ParameterInfo[] parameters = method.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                ParameterInfo parameter = parameters[index];
                Type parameterType = parameter.ParameterType;
                if (parameterType == typeof(AudioClip))
                {
                    arguments[index] = clip;
                }
                else if (parameterType == typeof(bool))
                {
                    arguments[index] = loop;
                }
                else if (parameterType == typeof(int))
                {
                    arguments[index] = startSample;
                }
                else if (parameterType == typeof(float))
                {
                    arguments[index] = volume;
                }
                else if (parameter.HasDefaultValue)
                {
                    arguments[index] = parameter.DefaultValue;
                }
                else if (parameterType.IsValueType)
                {
                    arguments[index] =
                        Activator.CreateInstance(parameterType);
                }
                else
                {
                    arguments[index] = null;
                }
            }

            return arguments;
        }

        private static void InvokeNoArgument(MethodInfo method)
        {
            if (method == null)
            {
                return;
            }

            try
            {
                method.Invoke(null, Array.Empty<object>());
            }
            catch
            {
                // Internal preview method signatures vary by Unity version.
            }
        }
    }
}

//----- DeverQuestEditorAudioBridge.cs END -----
