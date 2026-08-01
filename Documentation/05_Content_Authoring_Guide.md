# Content Authoring Guide

## Purpose

DeverQuest uses ScriptableObject assets to keep authored identity, quest, shop, audio, tactical, and campaign content separate from local session state. This allows designers to create reusable content while finalized Quests preserve historical snapshots.

## Authoring principles

1. Use stable, unique IDs. A display-name change must not create a new identity accidentally.
2. Keep demo, template, test, and production content in separate folders.
3. Treat generated assets as starting points, not untouchable package code.
4. Never author production assets inside `Packages/com.echodevgames.deverquest`.
5. Preserve backwards compatibility for IDs referenced by Adventurers, inventory, Contracts, or Chronicles.
6. Validate rights to names, lore, artwork, fonts, and audio.
7. Test content through deterministic encounters and a disposable Quest before publication.

## Recommended project structure

```text
Assets/
  DeverQuest/
    Catalogs/
      Identity/
      Combat/
      Companions/
    Campaigns/
      Production/
      DemoCampaign/
    Contracts/
    QuestProfiles/
    Encounters/
    Monsters/
    Abilities/
    Equipment/
    Shops/
    Audio/
    Loadouts/
    Templates/
    QA/
```

Use **Guild Hall > Campaign Content Scaffolding > Create Empty Studio Structure** to create the supported structure. Confirm exact generated paths in the project before standardizing team conventions.

## Safe generator behavior

The built-in generators are intended to upsert known starter assets without replacing authored content blindly. Test every generator twice in a disposable project:

- first run creates missing assets;
- second run preserves stable assets and references;
- neither run writes into package folders;
- no unexpected duplicates appear.

Commit generated assets and `.meta` files together.

## Identity assets

### Ancestry

Use an Ancestry asset for playable/sapient eligibility, traits, languages, attribute context, typed affinities, and other ancestry-linked rules. Avoid encoding a mutable display name as the only identity.

### Class Definition

Use a Class Definition for class eligibility, starting HP/Mana, starting attributes or bonuses, Department context, Ability Profile relationship, companion tradition, and starting loadout hooks.

### Faith

Use a Faith asset for optional belief/lore identity and eligibility restrictions. Do not infer real-world protected personal characteristics from a user's selection.

### Identity Catalog

The Identity Catalog groups available Ancestries, Classes, Faiths, alignments, and related creation choices. Validate that every listed asset exists and that default selections are playable.

## Quest Profiles

A Quest Profile is a reusable session template. Depending on configured content, it can define:

- title and description;
- intended duration;
- project/Department defaults;
- Focus Stages;
- tactical encounter or Survival configuration;
- pace/reward context;
- audio or ambience relationships;
- party context.

Use Profiles for repeatable shapes of work, not one-off task truth. The actual goal and final notes belong to the Quest/Contract record.

### Focus Stages

Stage names should describe observable milestones. Keep durations realistic and avoid reward structures that encourage unsafe work. Test ahead-of-pace, on-pace, behind-pace, skipped, and interrupted outcomes.

## Quest Contracts

A Contract represents assigned or formal work. Include:

- stable ID;
- title;
- actionable objective;
- project/Department;
- issuer and assignee rules;
- lifecycle state;
- completion criteria;
- reward;
- linked Profile or encounter where useful.

Do not use a Contract asset as an editable substitute for a finalized record. Finalized sessions should preserve a snapshot.

## Combat Codex

The generated Guild Combat Codex defines original creature families and damage types. Typed combat supports vulnerability, resistance, immunity, and absorption.

Authoring rules:

- every attack, Spell, or technique should use a valid damage-type ID;
- every creature-type ID should be stable;
- do not duplicate the same defense source to create accidental stacking;
- create controlled QA targets for each affinity combination;
- document the intended raw and final damage for regression.

## Equipment

Equipment can define category, rarity, weight, binding/trade policy, typed damage, defenses, and character effects. Distinguish reusable definition data from per-owner inventory identity.

Before release, test:

- acquisition and provenance;
- unique ownership for equipment and rare items;
- equip/unequip idempotence;
- stat recalculation without duplicate bonuses;
- encumbrance changes;
- binding and trade rejection;
- migration from older inventory records.

## Spells and Attack Techniques

Spells and techniques should have stable IDs, display data, resource or eligibility rules, typed damage/effects, and deterministic outcome data. Avoid hidden randomness that cannot be reproduced from the Chronicle unless the random seed and inputs are recorded.

## Ability Profiles

An Ability Profile groups class-linked tactical options. Validate class eligibility and missing references. Existing Adventurers should not lose known abilities merely because a catalog is reordered.

## Monsters and Encounter Profiles

A Monster Profile defines the opponent's identity, creature type, HP, attack data, affinities, rewards, and loot context. An Encounter Profile connects one or more opponents with difficulty, par, reward, and scene/lore presentation.

Author at least one QA encounter for:

- ordinary typed damage;
- resistance;
- vulnerability;
- immunity;
- absorption;
- resistance plus vulnerability cancellation;
- Companion behavior;
- flee/recovery paths;
- defeat with no duplicate rewards.

## Survival content

A Survival Quest uses multiple waves and should define clear termination, progression, par, and weighted loot rules. Test short QA variants before long production versions. Confirm exit paths do not leave a battle or Quest permanently locked.

## Companion Profiles and Catalogs

A Companion Profile can represent a pet, familiar, minion, spirit, construct, mercenary, or other original companion. Define:

- stable ID and original identity;
- eligibility and Class relationship;
- role: striker, guardian, support, controller, or supported extension;
- creature and damage types;
- affinities;
- recruitment cost;
- starting and recovery state;
- progression rules.

The Companion Catalog lists recruitable profiles. Test recruitment, duplicate prevention, active selection, persistence, recovery, and eligibility.

## Shop assets

### Shop Profile

Groups available Shop Items and presentation/policy context.

### Shop Item

Define:

- stable product/item ID;
- item type;
- price;
- rarity;
- weight where relevant;
- stackability or unique ownership;
- binding;
- trade eligibility;
- Redemption behavior for real-world requests.

Test insufficient funds, repeat click, disabled/bound state, inventory capacity/encumbrance, and ownership migration.

## Starter Loadouts

A Starter Loadout connects identity/class creation to initial equipment, abilities, items, coin, or Companions. Applying it must be idempotent. Character migration or reopening creation should not repeatedly grant the same loadout.

## Audio assets

### Playlist

Add licensed AudioClips, weights, repeat mode, and shuffle behavior. Use visibly distinct clips for QA. Very short clips are useful for edge testing but can make preview-transport timing difficult to interpret.

### Warning Profile

Assign optional clips for idle, wellness, stages, attacks, low health, victory, defeat, Quest completion, purchase, and level-up events. Empty slots should degrade safely to supported fallback behavior.

### Ambience Profile

Use environmental loops suitable for single-channel Editor playback. Ambience and Playlist music cannot be layered by the package.

## External Activity Profile

Define each provider using a display name, executable process name without `.exe`, optional foreground window-title match, and recent-input freshness. Verify process naming on the target operating system. Do not use broad rules that make unrelated applications count as project activity.

## Content review checklist

- [ ] Stable unique IDs.
- [ ] Original or licensed names and media.
- [ ] No broken references.
- [ ] Demo/test assets separated from production.
- [ ] Generator rerun tested.
- [ ] Eligibility rules tested positively and negatively.
- [ ] Reward values reviewed for duplication and abuse.
- [ ] Typed combat has expected-value tests.
- [ ] Ownership, binding, trade, and encumbrance tested.
- [ ] Finalized Chronicle snapshots remain stable after asset edits.
- [ ] Content committed with `.meta` files.
- [ ] Migration path documented before changing referenced IDs.
