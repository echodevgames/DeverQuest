//----- DeverQuestGitService.cs START -----

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestGitStatus
    {
        public bool GitAvailable;
        public bool IsRepository;
        public string RepositoryRoot = string.Empty;
        public string Branch = string.Empty;
        public string HeadHash = string.Empty;
        public string ShortHash = string.Empty;
        public string HeadSubject = string.Empty;
        public int StagedCount;
        public int UnstagedCount;
        public int UntrackedCount;
        public string Error = string.Empty;

        public bool HasStagedChanges => StagedCount > 0;
        public bool IsClean =>
            StagedCount == 0 &&
            UnstagedCount == 0 &&
            UntrackedCount == 0;
    }

    internal sealed class DeverQuestGitResult
    {
        public bool Succeeded;
        public string Output = string.Empty;
        public string Error = string.Empty;
    }

    internal static class DeverQuestGitService
    {
        private const int CommandTimeoutMilliseconds = 30000;

        public static DeverQuestGitStatus Refresh()
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(Application.dataPath, ".."));

            DeverQuestGitResult rootResult = Run(
                projectRoot,
                "rev-parse",
                "--show-toplevel");

            if (!rootResult.Succeeded)
            {
                return new DeverQuestGitStatus
                {
                    GitAvailable =
                        rootResult.Error.IndexOf(
                            "could not start",
                            StringComparison.OrdinalIgnoreCase) < 0,
                    IsRepository = false,
                    Error = rootResult.Error
                };
            }

            string repositoryRoot = rootResult.Output.Trim();
            DeverQuestGitStatus status = new DeverQuestGitStatus
            {
                GitAvailable = true,
                IsRepository = true,
                RepositoryRoot = repositoryRoot
            };

            DeverQuestGitResult branch = Run(
                repositoryRoot,
                "branch",
                "--show-current");
            DeverQuestGitResult hash = Run(
                repositoryRoot,
                "rev-parse",
                "HEAD");
            DeverQuestGitResult shortHash = Run(
                repositoryRoot,
                "rev-parse",
                "--short",
                "HEAD");
            DeverQuestGitResult subject = Run(
                repositoryRoot,
                "log",
                "-1",
                "--pretty=%s");
            DeverQuestGitResult changes = Run(
                repositoryRoot,
                "status",
                "--porcelain");

            status.Branch = branch.Succeeded
                ? branch.Output.Trim()
                : string.Empty;
            status.HeadHash = hash.Succeeded
                ? hash.Output.Trim()
                : string.Empty;
            status.ShortHash = shortHash.Succeeded
                ? shortHash.Output.Trim()
                : string.Empty;
            status.HeadSubject = subject.Succeeded
                ? subject.Output.Trim()
                : string.Empty;

            if (!changes.Succeeded)
            {
                status.Error = changes.Error;
                return status;
            }

            ParseStatus(changes.Output, status);
            return status;
        }

        public static DeverQuestGitResult CommitStaged(
            string repositoryRoot,
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return new DeverQuestGitResult
                {
                    Error = "Enter a commit message first."
                };
            }

            return Run(
                repositoryRoot,
                "commit",
                "-m",
                message.Trim());
        }

        public static DeverQuestGitResult StageAll(
            string repositoryRoot)
        {
            return Run(repositoryRoot, "add", "-A");
        }

        private static void ParseStatus(
            string output,
            DeverQuestGitStatus status)
        {
            using (StringReader reader = new StringReader(output))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length < 2)
                    {
                        continue;
                    }

                    char indexState = line[0];
                    char workTreeState = line[1];

                    if (indexState == '?' && workTreeState == '?')
                    {
                        status.UntrackedCount++;
                        continue;
                    }

                    if (indexState != ' ')
                    {
                        status.StagedCount++;
                    }

                    if (workTreeState != ' ')
                    {
                        status.UnstagedCount++;
                    }
                }
            }
        }

        private static DeverQuestGitResult Run(
            string workingDirectory,
            params string[] arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = BuildArguments(arguments),
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using (Process process = new Process
                       {
                           StartInfo = startInfo
                       })
                {
                    if (!process.Start())
                    {
                        return new DeverQuestGitResult
                        {
                            Error = "Git could not start."
                        };
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!process.WaitForExit(
                            CommandTimeoutMilliseconds))
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                            // The process may have exited between checks.
                        }

                        return new DeverQuestGitResult
                        {
                            Error = "Git timed out."
                        };
                    }

                    return new DeverQuestGitResult
                    {
                        Succeeded = process.ExitCode == 0,
                        Output = output.Trim(),
                        Error = process.ExitCode == 0
                            ? string.Empty
                            : string.IsNullOrWhiteSpace(error)
                                ? output.Trim()
                                : error.Trim()
                    };
                }
            }
            catch (Exception exception)
            {
                return new DeverQuestGitResult
                {
                    Error =
                        $"Git could not start: {exception.Message}"
                };
            }
        }

        private static string BuildArguments(
            IReadOnlyList<string> arguments)
        {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < arguments.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(QuoteArgument(arguments[index]));
            }

            return builder.ToString();
        }

        private static string QuoteArgument(string value)
        {
            value = value ?? string.Empty;
            return "\"" +
                   value.Replace("\"", "\\\"") +
                   "\"";
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestGitMonitor
    {
        private const string RepositoryKey =
            "EchoDevGames.DeverQuest.Git.ObservedRepository";
        private const string HeadKey =
            "EchoDevGames.DeverQuest.Git.ObservedHead";

        private static double nextCheckTime;

        static DeverQuestGitMonitor()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static void MarkObserved(DeverQuestGitStatus status)
        {
            if (status == null || !status.IsRepository)
            {
                return;
            }

            EditorPrefs.SetString(
                RepositoryKey,
                status.RepositoryRoot);
            EditorPrefs.SetString(HeadKey, status.HeadHash);
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup < nextCheckTime)
            {
                return;
            }

            nextCheckTime =
                EditorApplication.timeSinceStartup + 5d;

            if (!DeverQuestSessionStore.HasActiveSession)
            {
                return;
            }

            DeverQuestGitStatus status =
                DeverQuestGitService.Refresh();
            if (!status.IsRepository ||
                string.IsNullOrWhiteSpace(status.HeadHash))
            {
                return;
            }

            string observedRepository =
                EditorPrefs.GetString(RepositoryKey, string.Empty);
            string observedHead =
                EditorPrefs.GetString(HeadKey, string.Empty);

            if (observedRepository != status.RepositoryRoot ||
                string.IsNullOrWhiteSpace(observedHead))
            {
                MarkObserved(status);
                return;
            }

            if (observedHead == status.HeadHash)
            {
                return;
            }

            MarkObserved(status);

            if (DeverQuestSessionStore.HasActiveSession)
            {
                string subject = string.IsNullOrWhiteSpace(
                    status.HeadSubject)
                    ? "Git commit detected"
                    : status.HeadSubject;

                DeverQuestSessionStore.AddCommitEntry(
                    subject,
                    status.Branch,
                    status.ShortHash);
            }
        }
    }
}

//----- DeverQuestGitService.cs END -----
