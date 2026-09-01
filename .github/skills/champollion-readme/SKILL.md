---
name: champollion-readme
description: 'Create, update, review, audit, or maintain the root README.md for ChampollionGraphicalUserInterface. Use when product features, architecture summaries, documentation links, Copilot customizations, build and test commands, application usage, prerequisites, credits, licenses, saved configuration, paths, Windows packaging, artifacts, or release behavior change.'
argument-hint: 'Describe the root README section or repository change to document, synchronize, or audit.'
user-invocable: true
disable-model-invocation: false
---

# Champollion README

Maintain the repository-root `README.md` as the concise entry point for users and contributors. Keep every claim synchronized with current source, project files, tests, documentation indexes, Copilot customizations, packaging, and release behavior.

Human maintainers should keep this skill synchronized with `docs/tools/skills/champollion-readme.md`.

Use `champollion-development` for implementation, `champollion-diagrams` for architecture diagram creation and notation, and `champollion-code-review` for defect-oriented reviews. This skill owns root README scope, evidence mapping, organization, links, and maintenance decisions.

## README Role

The root README should answer:

- What is the application and what does it support?
- Which user-visible capabilities and limitations matter?
- How is the repository organized at a high level?
- Where can readers find detailed architecture, reports, and tooling documentation?
- How do contributors build, test, and run the application?
- How do users configure and operate it?
- Which external tools, runtimes, credits, and licenses apply?
- Where is configuration stored and how is it migrated or protected?
- How are Windows release packages produced and what do they contain?

Keep detailed implementation, diagram notation, skill maintenance procedures, algorithm explanations, and immutable reports in their owning documents. Link them from the root README instead of duplicating them.

## Source Of Truth By Section

Verify README claims against these owners:

| README section | Primary evidence |
| --- | --- |
| Introduction and Features | UI behavior, view models, Application services, Domain vocabulary, tests, and supported executable behavior |
| Architecture | `.slnx`, project files, `src`, `tests`, project references, DTO folders, and architecture documentation |
| Documentation | `docs/architecture`, `docs/reports`, `docs/tools`, and their current indexes |
| Copilot Customizations | `.github/skills`, `.github/agents`, and corresponding human guides under `docs/tools` |
| Build and Test | `global.json`, solution and project files, test projects, and verified commands |
| Using the Application | `MainWindow`, `MainViewModel`, validation, execution, search, Help, and output workflows |
| Credits and Licenses | `LICENSE`, `THIRD-PARTY-NOTICES.txt`, project package references, packaging inputs, and external-tool attribution |
| Saved Configuration | `AppSettingsStore`, settings DTOs, application output paths, migration behavior, installer permissions, and UI settings actions |
| Windows Release | `scripts/package-windows.ps1`, `packaging/windows`, project publish settings, and `.github/workflows/build-release.yml` |

Do not treat the README as evidence for itself. Resolve discrepancies using owning implementation or configuration and update the README only after the intended behavior is clear.

## When To Update

Assess root README impact when any of these change:

- application purpose, supported operating system, architecture, runtime, or external Champollion relationship;
- user-visible feature, limitation, operation, option, search behavior, validation rule, output, or error handling;
- project layout, dependency direction, DTO ownership, test layout, target framework, or solution format;
- architecture, reports, skill, agent, or other documentation entry points;
- build, test, run, packaging, or release commands;
- user workflow, prerequisite, WebView2 behavior, local-path requirement, or output-directory behavior;
- package dependency, third-party component, attribution, license, or distributed legal file;
- settings location, schema, persisted fields, migration, corruption handling, logs, or installer permissions;
- runtime identifier, package type, artifact name, checksum, installer availability, or bundled-file policy.

Do not update the README for private refactoring, test-only implementation detail, internal method naming, or another fact that does not change a current root-level claim. Record that README impact was assessed with no update required when working through a development or review workflow.

## Required Workflow

1. Identify the repository change or README section to maintain.
2. Read the current root section and its owning evidence from the table above.
3. Check linked detailed documentation and avoid contradicting or duplicating it.
4. Decide whether a root-level user or contributor claim changes.
5. Make the smallest update that restores accuracy and keeps the README scannable.
6. Add, remove, or rename links when documentation or customization entry points change.
7. Re-read neighboring sections for duplicate, conflicting, or misplaced information.
8. Validate all local links, Markdown diagnostics, headings, commands, and whitespace.
9. Report the sections updated or state that README impact was assessed with no change required.

## Content And Style

- Write for users and contributors arriving at the repository, not only maintainers already familiar with the code.
- Lead with stable capabilities and operational facts. Avoid roadmap promises and speculative behavior.
- Keep the root overview concise; use links for detailed algorithms, diagrams, skill maintenance, reports, installation, and release internals.
- Preserve established section names and order unless a structural change improves navigation or removes duplication.
- Use exact repository names, paths, commands, runtime identifiers, artifact types, and configuration keys.
- Use relative Markdown links for repository content and verify every target.
- Use tables for compact indexes or ownership summaries and bullets for scan-friendly capabilities.
- Keep code blocks executable from the repository root unless the surrounding text states another working directory.
- Do not list transient build output, local machine state, or generated report filenames as current universal facts.
- Do not claim a package, installer, platform, runtime, control, or workflow was validated when it was not.

## Repository Guardrails

Preserve these root-level truths unless verified implementation and policy changes replace them:

- The product is a Windows x64 Avalonia desktop interface targeting .NET 10.
- This repository does not contain or distribute Legacy or Current `Champollion.exe`.
- Supported application paths resolve to local fixed drives; unsupported network, mapped, removable, and protected output locations are rejected.
- Legacy and Current executable paths and edition-plus-game options are stored separately.
- Input and output paths are not persisted.
- Settings and logs live under application-adjacent `UserData`, with legacy `%LOCALAPPDATA%` migration and corrupt-settings preservation.
- Windows packages are self-contained `win-x64`; packaging rejects a bundled third-party `Champollion.exe`.
- WebView2 is required only for the embedded Help browser and is a separately supplied Windows runtime prerequisite.

Verify these facts before editing related text. Do not preserve a stale README statement merely because it appears in this list.

## Integration With Other Skills

### Development

`champollion-development` should use this skill after focused implementation validation when code, projects, workflows, packaging, documentation, or release behavior could change a root README claim. README assessment is separate from architecture-diagram assessment; a change may require one, both, or neither.

### Diagrams

`champollion-diagrams` owns diagram content and indexes. Use this skill when adding, removing, or renaming diagram entry points requires the root Documentation section to change. Do not copy diagram-family maintenance rules into the root README.

### Code Review

`champollion-code-review` should use this skill to audit directly affected README claims during normal diff review and to audit the requested README surface during an explicit documentation audit. A mismatch finding must cite both the owning evidence and README location, while respecting intentional root-level summarization.

## Validation

After the first README edit, check editor diagnostics. Before completion verify:

- exactly one root `README.md` is being maintained;
- heading hierarchy and section order remain coherent;
- every relative link resolves from the repository root;
- commands, paths, project names, framework versions, runtime identifiers, artifact names, and configuration keys match owning evidence;
- feature and usage claims match implemented behavior and limitations;
- architecture and documentation summaries match current indexes;
- Copilot customization lists match `.github/skills`, `.github/agents`, and human guides;
- credits and license statements match distributed files and dependency evidence;
- no duplicated or contradictory claims exist across sections;
- no trailing whitespace is introduced;
- Markdown diagnostics are clear.

Run product builds or tests only when needed to verify a technical claim or when the README change accompanies implementation. A documentation-only link or organization update does not require a full application build.

## Completion Report

Name the root README sections changed, summarize the evidence used, and list link and diagnostic validation. If no update is required, state which changed surface was assessed and why it does not affect a root-level claim.
