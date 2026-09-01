# Champollion README Skill

The `champollion-readme` skill teaches GitHub Copilot how to maintain the repository-root `README.md` as an accurate, concise entry point for users and contributors.

The active skill definition is [`.github/skills/champollion-readme/SKILL.md`](../../../.github/skills/champollion-readme/SKILL.md). This guide explains how contributors use and maintain it. The active skill remains Copilot's operational source of truth.

## Why This Is A Separate Skill

Root README maintenance is different from implementation, architecture diagram creation, and defect review:

- `champollion-development` owns code changes and implementation validation;
- `champollion-diagrams` owns architecture diagram selection, notation, and synchronization;
- `champollion-code-review` owns evidence-backed defect and consistency findings;
- `champollion-readme` owns root README scope, organization, evidence mapping, links, and maintenance decisions.

The development and review skills can delegate README assessment to this skill. Keeping the responsibility separate prevents implementation rules or detailed diagram conventions from bloating the repository entry point.

## When To Use It

Use the skill when:

- a product feature, limitation, workflow, or prerequisite changes;
- architecture, project layout, target framework, or test organization changes;
- documentation, Copilot skills, or custom agents are added, removed, or renamed;
- build, test, run, packaging, artifact, or release behavior changes;
- settings, paths, migration, logs, or installer permissions change;
- dependencies, credits, licenses, or distributed legal files change;
- the root README needs reorganization, link repair, or an evidence audit.

Do not update the README for private refactoring or implementation details that do not alter a root-level user or contributor claim.

## How To Invoke It

### Automatic Discovery

Copilot can select this skill when a request mentions the root README, repository overview, feature list, documentation links, build instructions, application usage, prerequisites, saved configuration, credits, licenses, packaging, artifacts, or release instructions.

Example requests:

```text
Update the root README for the new settings migration behavior.
```

```text
Add the new Copilot customization and its human guide to the README.
```

```text
Audit README feature and release claims against the current implementation.
```

### Manual Invocation

The skill is user-invocable as `/champollion-readme` in Copilot Chat.

```text
/champollion-readme Synchronize the Features and Using the Application sections with this branch.
```

```text
/champollion-readme Review all local links and documentation entry points in the root README.
```

```text
/champollion-readme Update Windows Release instructions from the current packaging script and workflow.
```

## README Section Ownership

Each section has a primary evidence source:

| Section | Evidence to inspect |
| --- | --- |
| Introduction and Features | UI, view models, Application services, Domain vocabulary, and tests |
| Architecture | Solution, project files, `src`, `tests`, project references, and architecture docs |
| Documentation | Indexes under `docs/architecture`, `docs/reports`, and `docs/tools` |
| Copilot Customizations | `.github/skills`, `.github/agents`, and human guides under `docs/tools` |
| Build and Test | `global.json`, solution/project files, test projects, and verified commands |
| Using the Application | Main window, view model, validation, execution, search, Help, and output behavior |
| Credits and Licenses | `LICENSE`, `THIRD-PARTY-NOTICES.txt`, package references, and packaging inputs |
| Saved Configuration | Settings store and DTOs, output paths, migration, installer permissions, and UI actions |
| Windows Release | Packaging script, Inno Setup files, publish configuration, and GitHub Actions workflow |

The README is not evidence for itself. If a claim differs from its owner, verify intended behavior and update the README accordingly.

## Expected Workflow

When the skill is active, Copilot should:

1. Identify the changed repository surface or README section.
2. Read the current section and owning evidence.
3. Check linked detailed documentation for consistency.
4. Decide whether a stable root-level claim changes.
5. Make the smallest update that restores accuracy.
6. Keep detailed content in its owning document and link to it.
7. Check neighboring sections for duplication or contradiction.
8. Validate local links, headings, commands, paths, and diagnostics.
9. Report updated sections or an explicit no-impact conclusion.

Development should perform this assessment after its first focused implementation validation succeeds. A code change may require README updates, diagram updates, both, or neither.

## Writing Principles

- Write for users and contributors arriving at the repository.
- Keep the root README scannable and link to detailed maintenance or architecture pages.
- Prefer durable behavior over implementation trivia or transient local state.
- Use exact names, paths, commands, versions, runtime identifiers, artifacts, and settings keys.
- Keep commands executable from the repository root unless stated otherwise.
- Avoid roadmap promises and unsupported platform or validation claims.
- Preserve the established structure unless reorganization improves navigation or removes duplication.
- Verify every relative link.

## Root-Level Guardrails

The README currently communicates several important product boundaries:

- Windows x64 and .NET 10 are the supported product and framework targets.
- Legacy and Current `Champollion.exe` are external and are not distributed by this repository.
- Application paths are restricted to supported local fixed-drive locations.
- Executable paths and edition-plus-game settings are stored separately.
- Input and output paths are not persisted.
- Settings and logs use application-adjacent `UserData`, including legacy migration and corrupt-settings preservation.
- Release packages are self-contained `win-x64` and reject bundled third-party Champollion binaries.
- WebView2 is a separate prerequisite for the embedded Help browser.

These are reminders, not permanent assumptions. Verify their owning implementation and policy before maintaining related text.

## Maintaining The Skill

Update both the active skill and this guide when any of these change:

- root README purpose, audience, sections, or ordering;
- source-of-truth ownership for a README section;
- automatic update triggers or no-impact rules;
- product, architecture, documentation, build, usage, license, settings, or release coverage;
- responsibility boundaries among development, diagrams, review, and README skills;
- link, command, or validation conventions.

Do not add a permanent rule for a one-off wording preference unless future README work should consistently follow it.

### Maintenance Steps

1. Verify current README structure and representative source owners.
2. Edit [the active skill definition](../../../.github/skills/champollion-readme/SKILL.md).
3. Update this guide when usage, ownership, workflow, or maintenance guidance changes.
4. Update the [skills index](README.md) if the name or guide path changes.
5. Keep the folder and frontmatter name identical: `champollion-readme`.
6. Keep the description rich in root README section and maintenance trigger terms.
7. Preserve `user-invocable: true` and `disable-model-invocation: false` unless invocation policy changes.
8. Keep `SKILL.md` below 500 lines and move extended human explanation here.
9. Test the skill against one README update and one no-impact development change.

## Validation Checklist

After changing the skill or guide, verify:

- [ ] The definition exists at `.github/skills/champollion-readme/SKILL.md`.
- [ ] YAML frontmatter begins and ends with `---`.
- [ ] The folder and frontmatter names match.
- [ ] Discovery terms cover root README sections and maintenance tasks.
- [ ] Automatic and manual invocation settings remain intentional.
- [ ] The skill and guide describe the same responsibilities and evidence owners.
- [ ] Development and review integration remains accurate.
- [ ] The skill remains below 500 lines.
- [ ] Markdown diagnostics are clear.
- [ ] Local links resolve.
- [ ] No trailing whitespace is present.

For a representative README edit, also verify:

- headings remain coherent and unique;
- every relative link resolves from the repository root;
- commands and paths match current repository files;
- feature, prerequisite, configuration, architecture, and release claims match owning evidence;
- detailed content remains in linked documentation rather than being duplicated;
- the completion report names updated sections or gives a justified no-impact result.

A documentation-only organization or link update does not require a full application build. Run focused product checks when they are needed to verify a technical README claim or when the README changes alongside implementation.

## Troubleshooting

### The Skill Is Not Discovered

1. Confirm `.github/skills/champollion-readme/SKILL.md` exists.
2. Confirm the folder and frontmatter names are `champollion-readme`.
3. Confirm the request uses terms in the frontmatter description.
4. Confirm `disable-model-invocation` is `false`.
5. Check YAML and Markdown diagnostics.
6. Reload VS Code or start a new Copilot Chat request after metadata changes.

### The Slash Command Is Missing

Confirm `user-invocable: true`, then reopen Copilot Chat or reload VS Code.

### README Updates Become Too Detailed

Move implementation, algorithms, diagram notation, skill maintenance, or report detail into the owning documentation and keep a concise summary plus link in the root README.

### Development Changes Do Not Update The README

Confirm `champollion-development` requires a post-validation README impact assessment and names `champollion-readme` as the authority. Ensure its completion report requires either updated sections or a no-impact conclusion.

### Reviews Miss Stale README Claims

Confirm `champollion-code-review` uses this skill to map README sections to owning evidence. Require mismatch findings to cite both the evidence and README location while respecting intentional summary.

## Related Documentation

- [Copilot skills index](README.md)
- [Active README skill definition](../../../.github/skills/champollion-readme/SKILL.md)
- [Champollion development skill](champollion-development.md)
- [Champollion diagrams skill](champollion-diagrams.md)
- [Champollion code review skill](champollion-code-review-maintenance.md)
- [Root README](../../../README.md)
