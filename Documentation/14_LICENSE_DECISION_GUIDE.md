# License Decision Guide

## Why this document exists

A repository is not safely reusable merely because its source can be viewed. Before public or commercial distribution, the owner must choose and include an actual license file. This guide is not legal advice.

## Questions to decide

1. Should other people be allowed to use the package commercially?
2. May they modify and redistribute it?
3. Must derivative source remain open?
4. Must attribution be preserved?
5. Is patent language important?
6. Are trademarks, branding, starter lore, art, audio, and documentation licensed separately?
7. Will there be dual licensing or paid commercial terms?
8. Are third-party dependencies/assets included, and are their notices compatible?

## Common directions

| Direction | Typical effect |
|---|---|
| MIT | Permissive use/modification/redistribution with copyright and license notice |
| Apache-2.0 | Permissive plus explicit patent grant and notice requirements |
| GPL family | Copyleft obligations for distributed derivatives; version choice matters |
| Proprietary/EULA | Rights limited to the terms written by the owner |
| Dual license | Open/community terms plus separate commercial terms |

Have qualified counsel review the chosen terms for the intended business model.

## Separate asset rights

Code licensing does not automatically grant rights to:

- Unity or third-party assets;
- fonts;
- music and sound effects;
- logos and trademarks;
- copied names or lore;
- user-generated Chronicles or media.

The package's original generated starter content should still be listed explicitly in the chosen license scope.

## Release checklist

- [ ] Add a top-level `LICENSE` file.
- [ ] Add copyright holder and year.
- [ ] State whether documentation and generated starter content share the code license.
- [ ] Add third-party notices.
- [ ] Remove unlicensed content.
- [ ] Add license metadata to `package.json` if appropriate.
- [ ] Update README and distribution page.
- [ ] Preserve required notices in tarballs and source distributions.

## Temporary status

Until an explicit license is selected, distribution recipients should not assume permission to copy, modify, redistribute, or commercialize the package beyond the owner's direct authorization.

Treat the missing license as a release blocker, not a decorative placeholder. The release owner should complete and preserve the following decision record before publishing the package outside an approved private test group.

| Decision field | Final value |
|---|---|
| Chosen software license | |
| Copyright owner and year | |
| Commercial use permitted | |
| Modification and redistribution permitted | |
| Source disclosure requirement | |
| Documentation license | |
| Generated starter-content license | |
| Excluded third-party assets | |
| Required attribution/notices | |
| License/support contact | |
| Approval date and approver | |

After the decision is approved, add the actual license text at the package root, update `package.json` where appropriate, list asset exceptions, update the README and support policy, and rerun the documentation and distribution gates in Quest 1.
