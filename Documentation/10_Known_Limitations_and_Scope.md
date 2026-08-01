# Known Limitations and Scope

## Release-candidate scope

Version 0.30.2 is a stabilization release. Its supported product story is:

- Unity Editor focus/session companion;
- recoverable Quest lifecycle;
- classified time and wellness breaks;
- local Chronicles, integrity, history, and exports;
- Guild accounts and project authority;
- ScriptableObject content scaffolding;
- Git/external activity/media evidence;
- rewards, inventory, shop, trading, and redemption ledger;
- character, equipment, typed combat, Companions, encounters, Survival, and encumbrance;
- optional shared Guild publication and healthy ranking;
- optional compensation estimate;
- single-owner Editor audio.

## Deferred major systems

The following are explicitly outside the 0.30 release candidate:

- crafting recipes and production chains;
- banking and account-like storage;
- housing/property systems;
- biome simulation and gathering ecology;
- broad tradeskill progression;
- broad weapon-skill progression;
- server-hosted authentication and immutable records;
- automatic external reward fulfillment;
- payroll execution;
- runtime/in-game DeverQuest UI.

## Architectural limitations

### EditorPrefs

Much local state is stored in Unity Editor preferences. It is convenient but not encrypted, project-isolated, transactional, or enterprise-grade. Machine/profile access can alter it.

### Folder-backed shared authority

Shared records are tamper-evident only within the limits of external permissions. A user with total rewrite access can replace content and hashes.

### Editor audio

Unity's shared preview transport allows one dependable owner. Playlist and ambience cannot be mixed. Internal AudioUtil behavior can change between Unity versions.

### Platform-specific external activity

Foreground-process support is principally designed around Windows behavior. Other platforms require explicit implementation and testing.

### IMGUI

The main interface uses Editor IMGUI. Accessibility, responsive layout, automated UI testing, and complex state separation are more limited than with a fully redesigned UI architecture.

### Local integration

Git, microphone, filesystem, and process monitoring depend on the host environment. The package cannot guarantee external tool availability or permissions.

## Policy limitations

- Wellness prompts are not medical advice.
- Compensation Preview is not payroll or legal compliance.
- Hall rankings are motivational and should not be used as a sole performance-management measure.
- Git references do not prove quality or authorship.
- Local Guild authentication is not an internet identity system.
- Redemptions record manual fulfillment; they do not deliver external benefits.

## Known high-risk areas to retest every patch

1. Active Quest recovery and finalization.
2. Reward/purchase/trade/redemption idempotency.
3. Chronicle write and shared publication retry.
4. Account/project authority denial.
5. Audio rapid controls and cue restoration.
6. Migration of EditorPrefs-backed schemas and stable asset IDs.
7. Typed damage and Companion persistence.
8. Compensation eligibility and disclaimers.

## Feature acceptance rule

A new major subsystem should not enter the release branch unless:

- Quest 1 currently passes;
- the subsystem has a storage and migration design;
- it cannot create false Focus or duplicate rewards;
- authority and privacy are defined;
- failure and recovery are testable;
- documentation and regression tests arrive with the code.
