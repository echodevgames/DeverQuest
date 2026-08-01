# Security

## Security model

DeverQuest 0.30.2 is a local Unity Editor tool with optional folder-backed sharing. It is not a hardened network service. Security depends on the local operating-system account, Unity project access, filesystem permissions, Git security, backup policy, and any sync/server controls around shared records.

## Protect these assets

- Guild account and audit EditorPrefs state;
- active and completed Quest state;
- Chronicles, integrity journals, and corrections;
- voice memos and attached media;
- Shared Guild records and Adventurer snapshots;
- compensation policy/rates and exports;
- real-reward redemption records;
- Git repositories and credentials;
- package signing/checksum/release archives.

## Do not store

Do not place these in DeverQuest notes, goals, credentials, or demo content:

- API keys, tokens, private keys, passwords, recovery codes;
- customer secrets or regulated data;
- medical details unrelated to the tool's operation;
- confidential compensation data in shared Chronicles;
- copyrighted or licensed media without permission.

## Credential warning

Local Guild credentials are not an enterprise authentication system or encrypted password vault. Use unique low-sensitivity credentials and protect the operating-system account. Do not reuse work, email, banking, or identity-provider passwords.

## Permission model

Verify CEO, Boss, Project Leader, and Member permissions after every authority-related code change. Critical operations must validate service-level permission and target-project scope. UI hiding alone is insufficient.

## Shared repository hardening

- Host it in a studio-controlled location.
- Restrict ordinary Members from rewriting all records.
- Use versioned or immutable backups.
- Monitor mass replacement and sync conflicts.
- Separate QA and production repositories.
- Require administrator review for corrections and deletions.
- Do not expose the folder publicly.

An unkeyed hash detects content change but cannot stop an authorized filesystem writer from replacing both file and hash.

## Media privacy

Microphone and file attachments can contain personal, proprietary, or confidential content. Establish consent, access, retention, deletion, and incident procedures. Avoid automatic broad sharing of Media folders.

## Git execution

Review repository paths, command arguments, and staged changes. DeverQuest should not interpolate untrusted text into shell commands without safe argument handling. Normal Git credential and remote protections remain external.

## Package supply chain

For every release:

1. build from a clean tagged source state;
2. inspect tarball contents;
3. calculate and publish SHA-256;
4. archive the exact tarball and documentation;
5. test installation from that archive;
6. review dependency and Unity-version changes.

## Vulnerability reporting template

Until a private security contact is selected, do not publish detailed exploitation steps in a public issue when the defect could expose credentials, private media, unauthorized records, or destructive access. Record:

- package and Unity versions;
- affected data and authority level;
- reproduction prerequisites;
- minimal proof without real secrets;
- mitigation/workaround;
- whether production data may be affected.

## Security release blockers

- unauthorized Guild mutation;
- credential or private-media disclosure;
- arbitrary command/path injection;
- silent Chronicle replacement represented as valid;
- cross-account inventory/character data bleed;
- destructive reset without clear scope;
- package containing secrets or production records.
