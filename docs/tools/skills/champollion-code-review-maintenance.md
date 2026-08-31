# Champollion Code Review Skill Maintenance

## Purpose

The `champollion-code-review` skill gives GitHub Copilot repository-specific instructions for reviewing pull requests, commits, branches, diffs, and changed files in this project.

It is defect-oriented. It prioritizes correctness, regressions, clean-architecture violations with concrete impact, accessibility, Windows compatibility, packaging, release integrity, and meaningful missing tests. It should not manufacture findings for optional style preferences.

The active definition is:

```text
.github/skills/champollion-code-review/SKILL.md
```

This guide is for maintainers. The skill file remains Copilot's source of truth.

## When To Use It

Use the skill to review:

- pull requests, branch changes, commits, and uncommitted diffs;
- C#, Avalonia XAML, project, and test changes;
- Domain/Application/UI boundary changes;
- DTO direction and behavior placement;
- external process execution and command construction;
- executable search and Legacy/Current classification;
- settings migration and path validation;
- WebView2, file picker, clipboard, and Windows shell behavior;
- Windows publish, Inno Setup, versioning, artifacts, and GitHub releases.

Use `champollion-development` for implementation and bug-fixing work. The dedicated review skill owns Copilot Review and defect-finding requests.

## How To Invoke It

### Automatic Discovery

Copilot can select the skill when a request mentions reviewing a pull request, commit, branch, diff, changed file, packaging change, release workflow, or another trigger in the frontmatter description.

### Manual Invocation

```text
/champollion-code-review Review the current changes.
```

```text
/champollion-code-review Review this branch against main, focusing on clean architecture and DTO behavior.
```

```text
/champollion-code-review Review the Windows packaging and release workflow changes.
```

```text
/champollion-code-review Review MainWindow.axaml for Avalonia binding, accessibility, and layout regressions.
```

The reviewer should choose focused checks based on credible defect hypotheses. Not every review requires every build or packaging command.

## What The Skill Does

The workflow requires Copilot to:

1. Establish the review target and comparison base.
2. Inspect the complete relevant diff and owning implementation.
3. Trace changed symbols to consumers, bindings, persistence, packaging, and release steps.
4. Form falsifiable defect hypotheses.
5. Run cheap focused checks where possible.
6. Check mirrored tests and changed behavioral boundaries.
7. Re-read each changed location before reporting it.
8. Report actionable findings in severity order, or clearly state that none remain.

The evidence-first sequence prevents framework assumptions and repository-wide observations from becoming unsupported review comments.

## Review Domains

### Clean Architecture And DTOs

The skill protects this dependency direction:

```text
Avalonia UI -> Application -> Domain
Avalonia UI ----------------> Domain
Domain -> no project dependencies
```

It checks that DTOs remain passive directional contracts, Application behavior stays out of DTOs and Domain, and Avalonia/platform behavior stays out of Application and Domain.

### Source Documentation And Tests

Production C# under `src` follows full XML documentation and selective ordered regions. Tests mirror production type paths. The skill checks these conventions when changed, but documentation style should not distract from behavioral defects.

### Execution And Search

The skill checks structured `ArgumentList` use, asynchronous output draining, per-file failure isolation, diagnostic logging, bounded cancellation-aware search, and conservative executable classification.

Legacy classification requires all companion files plus `doc/Readme.html`. A partial Legacy layout remains ambiguous. Current uses the standalone layout.

### Avalonia And Windows Integration

The skill checks compiled bindings, CommunityToolkit-generated members, accessibility, focus, contrast, text/layout behavior, WebView2, clipboard, file pickers, and shell integration. A successful XAML build does not prove visual or interactive behavior.

Windows 10/11 x64 is the current supported target. Missing Linux support is not a review defect unless a change claims or introduces it.

### Packaging And Release

The skill protects Inno Setup identity, version propagation, x64 self-contained publishing, artifact/checksum selection, the prohibition on bundling `Champollion.exe`, and GitHub Actions release semantics.

The release job intentionally runs without checkout and supplies `GH_REPO`. Artifact upload/download behavior must be assessed against the exact action versions and wildcard paths.

## Maintaining The Skill

Review and update the skill when any of these change:

- solution or project names and dependency boundaries;
- target framework, Avalonia, CommunityToolkit, or WebView package versions;
- supported operating systems or architectures;
- DTO ownership, source folders, namespaces, documentation, or region patterns;
- execution, success, output, logging, or cancellation behavior;
- executable layout, filename, search roots, exclusions, or classifier evidence;
- settings location, migration, profile keys, or persisted fields;
- Windows runtime identifier, publish profile, installer identity, or artifact names;
- GitHub Actions versions, wildcard paths, permissions, or release commands;
- test projects or standard validation commands.

When a repository fact changes:

1. Update implementation and tests or packaging validation.
2. Update user and maintainer documentation.
3. Update the corresponding review invariant.
4. Update this guide when the maintenance procedure or rationale changes.
5. Run the revised skill against a representative diff.

## Adding A Review Rule

Add a repository-specific rule only for a recurring or high-impact failure mode.

1. Identify the concrete failure, trigger, impact, and affected files.
2. Put the rule in the owning subsystem section.
3. Phrase it as an observable relationship or check.
4. Include a cheap disconfirming validation where possible.
5. Avoid volatile values that the reviewer can read directly.
6. Add frontmatter discovery terms only if a new review category would otherwise be missed.
7. Test against both valid and deliberately broken examples when practical.

Remove or revise a rule when its premise becomes false, a structured tool enforces it more accurately, or it repeatedly creates false positives.

## Validation After Skill Changes

1. Confirm the file is `.github/skills/champollion-code-review/SKILL.md`.
2. Confirm the folder and frontmatter names are `champollion-code-review`.
3. Confirm frontmatter begins and ends with `---`.
4. Keep the quoted description under 1,024 characters and rich in review trigger terms.
5. Preserve `user-invocable: true` and `disable-model-invocation: false` unless invocation policy changes.
6. Keep the skill below 500 lines.
7. Check Markdown/YAML diagnostics and local links.
8. Run `git diff --check` when Git tracks the files.
9. Confirm `/champollion-code-review` appears after reloading the workspace if necessary.
10. Invoke it against a small representative diff and verify findings include changed locations, triggers, impacts, evidence, and bounded remediation.

Useful regression scenarios:

| Scenario | Expected review behavior |
| --- | --- |
| Logic added to an output DTO | Report behavior in a passive contract and point to the owning service. |
| Partial Legacy layout accepted as Current | Report edition misclassification. |
| Process completion reported before both streams drain | Report incomplete or lost output. |
| Command paths concatenated into one argument string | Report quoting/injection and path-with-spaces risk. |
| `Champollion.exe` enters publish output | Report prohibited third-party binary packaging. |
| Version changes only in About text | Report inconsistent binary/installer/artifact version flow. |
| Current artifact wildcard with merged download | Do not invent nested `artifacts/packages` paths. |
| Windows-only code remains in a Windows-only product | Do not report missing Linux support without a changed claim. |

## Troubleshooting

### Skill Is Not Discovered

- Confirm the exact path and matching frontmatter name.
- Confirm the request uses terms present in the description.
- Confirm automatic invocation is enabled.
- Reload VS Code after customization changes.
- Check YAML and Markdown diagnostics.

### Reviews Contain Speculative Findings

Strengthen the affected rule with a concrete trigger, impact, and disconfirming check. Prefer focused validation over adding an always-assumed conclusion.

### Reviews Miss A Recurring Defect

Add the owning file relationships and failure condition, not just the latest symptom. Prefer deterministic tests or CI enforcement when possible.

### Development And Review Skills Both Load

Ensure `champollion-development` does not advertise pull requests, commits, diffs, branches, or code review. Keep those trigger terms only in `champollion-code-review`.

## Review Ownership

Treat skill changes as engineering changes. A maintainer should verify that each rule reflects current repository behavior, requests available validation or states its limitations, avoids universal claims based on local behavior, and improves review signal rather than comment volume.
