# Adventurer User Guide

## What DeverQuest is

DeverQuest is a Unity Editor companion for deliberate development work. You start a Quest, work toward a stated goal, classify interruptions honestly, add notes and evidence, and finalize a Chronicle. The fantasy layer gives the work texture. The time record remains the backbone.

A healthy DeverQuest session should help you answer four questions:

1. What did I intend to accomplish?
2. How much eligible Focus did I actually record?
3. What happened while I worked?
4. What should I do next?

## Open DeverQuest

Use **Tools > DeverQuest > Developer Companion**. Direct workspace shortcuts are available under **Tools > DeverQuest > Workspaces**.

The main workspaces are:

| Workspace | Primary purpose |
|---|---|
| Quest | Start, pause, meditate, resume, complete, abandon, or recover a Quest |
| Quest Log & Git | Notes, commit context, repository status, media, and voice evidence |
| Character | Adventurer sheet, inventory, equipment, abilities, Companions, and deterministic encounters |
| Guild Hall | Authentication, Contracts, campaign content, shared records, and authorized administration |
| Rewards & History | Chronicles, exports, corrections, reward records, compensation preview |
| Audio & Wellness | Playlist, ambience, warning cues, wellness reminders, Approved Breaks |
| Settings | Profile, timer, idle, reminder, storage, display, and integration settings |

## Sign in and select your Adventurer

A Guild account and Adventurer are related but not identical:

- The **Guild account** determines authentication, rank, permissions, and project assignment.
- The **Adventurer** holds the character identity, progression, wallet, inventory, equipment, Companions, and battle state.

Sign in only to your own account. Confirm the displayed account and Adventurer before starting a Quest, buying an item, accepting a trade, completing a Contract, or publishing a record.

## Start a Quest

Before pressing Start:

1. Write a concrete goal. Prefer “Implement and test save-slot deletion confirmation” over “work on game.”
2. Confirm the project and Department.
3. Select a Quest Profile when reusable settings or an encounter are appropriate.
4. Select a Quest Contract when the work was assigned or formally scoped.
5. Review Focus duration and any Focus Stages.
6. Confirm the correct Git repository if you intend to attach commit evidence.
7. Confirm there is no recovered or paused Quest that should be resumed instead.

Once started, the Quest becomes the active record. Avoid starting a second conceptual task inside it without noting the change or completing the first Quest.

## Understand time classifications

### Focus

Eligible deliberate work while the Quest is actively running. This is the principal time used for ordinary Quest rewards and Chronicle reporting.

### Paused

The Quest remains recoverable, but Focus does not advance. Use Pause for interruptions that should not be counted as work.

### Meditation

A separate intentional state. It is recorded distinctly and does not become Focus merely because it happened during a Quest.

### Approved Break

A configured wellness break started through a reminder or break control. A completed break may grant its configured wellness/character benefit, but it is not Focus. Resuming too early records an early ending and grants no completion benefit. Time beyond the approved duration becomes Idle/Unverified.

### Idle/Unverified

Time for which DeverQuest cannot verify active eligible work. It must not be relabeled as Focus merely to improve totals.

### External Activity

Evidence that a configured foreground creative tool was active while recent input continued. It can prevent a false Unity-focus idle pause on supported platforms, but it does not independently add Focus seconds.

## Work with the live Quest

### Pause and resume

Use Pause when leaving the work station, switching to unrelated activity, or handling an interruption. Resume the same Quest when returning. Repeated presses should not create duplicate sessions.

### Meditation

Use Meditation for deliberate thinking, reflection, or the package's intended non-Focus state. Confirm the timer classification before returning to ordinary work.

### Focus Stages

A Quest Profile may divide a Quest into ordered stages. Each stage can carry a name, duration or pace expectation, and tactical/reward context. Stage results are recorded in the session snapshot. Later edits to the source profile should not rewrite an already finalized Chronicle.

### Low-health safety pause

Some tactical Quests may pause for safety when the Adventurer reaches a configured low-health condition. Treat this as a state transition, not a reason to spam combat controls. Review the encounter state, use a supported recovery/exit action, then resume only when the interface permits it.

## Quest Log and evidence

Use the Quest Log for useful facts rather than a transcript of every keystroke:

- decisions and reasons;
- completed subgoals;
- blockers and attempted fixes;
- file or system areas changed;
- test results;
- next steps;
- commit references.

### Git

The Git panel observes the repository and can support staging/commit workflows. Always review changes before committing. DeverQuest does not replace Git discipline, pull requests, code review, or remote backups.

A Chronicle may record branch and commit context. A commit proves that a repository state existed; it does not prove the quality or authorship of the work by itself.

### Existing media attachment

Attach screenshots, reference audio, art, or other evidence only when it is appropriate to retain. DeverQuest copies the file into the dated Media folder so the Chronicle does not depend on the original path. Unlinking removes the Quest reference but intentionally does not erase the copied file.

### Voice memo

Choose a microphone, provide a useful memo name, record, and select **Stop and Attach**. Operating-system microphone permission applies. Script reload, abandonment, or editor shutdown should cancel recording safely. Review sensitive content before retaining or sharing the file.

## Complete a Quest

Before completion:

1. Confirm the Quest goal and active project are correct.
2. Add Commit Details if the work was committed.
3. Write a Final Quest Log Entry that states the outcome and next action.
4. Review Focus, Meditation, Approved Break, and Idle/Unverified totals.
5. Review battle, reward, attachment, and Contract information where applicable.
6. Select **Complete Quest** once.

After completion:

- confirm the Quest disappears from active state;
- inspect the newest Chronicle in Rewards & History;
- confirm Markdown and machine-readable records agree;
- confirm reward and progression changes occurred once;
- correct errors through the authorized correction workflow rather than editing source records casually.

## Abandon versus complete

Use **Abandon Quest** when the session should not be represented as successful completion. Record a clear reason. Abandonment should remain distinguishable from completion and should not receive completion-only rewards.

## Recovery after reload or restart

DeverQuest preserves active session state locally. On assembly reload or editor shutdown, it should leave the Quest in a safe recoverable condition rather than continuing invisible Focus.

When a recovered Quest appears:

1. Confirm the goal, start time, and project.
2. Confirm it is paused or otherwise safe.
3. Resume only if the same work is continuing.
4. Complete or abandon it explicitly.
5. Report duplicate or unexplained recovered sessions immediately.

## Wellness reminders

A reminder provides three distinct choices:

- **Acknowledge Only:** records that you saw it; no break or benefit is claimed.
- **Snooze:** delays it; no completed action is recorded.
- **Take Approved Break:** pauses the Quest and begins a configured break classification.

Use the truthful choice. The wellness system is not medical advice, diagnosis, or an emergency service.

## Audio

### Playlist

A Playlist asset can contain multiple clips, weights, shuffle, and repeat behavior. Controls include Play, Stop, Previous, and Next.

### Ambience

An Ambience Profile provides environmental loops and Next Ambience behavior. Playlist music and ambience are intentionally mutually exclusive because Unity exposes one shared Editor preview transport.

### Warning cues

Warning, victory, level-up, combat, and other cues may temporarily interrupt long-form audio. The music should resume near its prior position. Rapid cues should replace one another, not stack.

If multiple tracks are ever audible at once, stop audio, capture exact reproduction steps, and report a Critical defect.

## Rewards and economy

Eligible finalized work may grant XP and coin according to profile policy. RPG actions alone do not create Focus time.

Inventory items can have type, rarity, weight, binding, trade eligibility, provenance, and ownership identity. Some items are ordinary stackable resources; equipment, redemptions, and rare items may require unique ownership records.

### Shop

Review the item, price, type, binding, and trade policy before Purchase. A successful purchase should charge once and add the correct ownership record once.

### Trading Post

An offered item enters escrow. The recipient can Accept or Reject. The sender can Cancel an open offer or Reclaim a rejected one. Bound, non-tradeable, and redemption records cannot be traded.

### Real-world redemption

A Redemption is a request and administrative ledger. It requires leadership approval and a later manual delivery confirmation. The software does not deliver gift cards, merchandise, Discord benefits, money, or any other external item automatically.

## Compensation Preview

Compensation Preview estimates eligible finalized time according to a locally configured policy. It excludes Meditation and Idle/Unverified time, excludes active Quests, and can exclude modified or legacy records. It is not payroll, a wage statement, a promise of payment, tax advice, or authorization to pay. Report discrepancies to the Guild administrator and preserve the affected Chronicles.

## Character and tactical systems

### Identity

Ancestry, Class, Faith, and Identity Catalog assets drive character creation. Eligibility rules, stable IDs, starting values, traits, languages, and Department context can come from these assets.

### Equipment and encumbrance

Equipment can alter character state and typed defenses. Physical coin and weighted items can contribute to encumbrance. Denomination exchange should preserve total coin value exactly.

### Spells, techniques, and abilities

Spells and Attack Techniques define tactical actions. Ability Profiles can connect class identity to tactical options. These systems affect deterministic RPG results, not professional Focus time.

### Companions

A persistent roster can include pets, familiars, minions, spirits, constructs, and mercenaries. One active Companion participates in deterministic encounters. Role behavior may include damage, interception, restoration, or control. HP, loyalty, battles, victories, XP, and levels persist per Adventurer.

### Encounters and Survival

Deterministic encounters use authored monster, encounter, damage, ability, and equipment data. Survival Quests run multiple waves and may grant par or loot rewards. The Battle Chronicle should preserve raw and final typed damage, important actions, outcome, and rewards.

Typed defenses follow the package rules:

- resistance reduces damage;
- vulnerability increases damage;
- immunity prevents damage;
- absorption converts qualifying damage to healing;
- resistance and vulnerability together cancel to normal;
- duplicate defenses should not stack multiple times.

## History and exports

Rewards & History provides Chronicle review and filtered CSV/JSON export. An export is a convenience copy, not the only backup. Preserve the original Chronicle root and shared repository according to policy.

## Daily closeout

Before ending work:

1. Complete, abandon, or safely pause the active Quest.
2. Confirm the latest Chronicle was written.
3. Confirm important work was committed and pushed through your normal workflow.
4. Review any failed publication, attachment, or timecard write.
5. Stop audio if leaving Unity open unattended.
6. Back up or synchronize the approved Chronicle location.
7. Leave a final note for the next Quest.
