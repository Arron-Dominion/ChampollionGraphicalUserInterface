---
name: champollion-code-review
description: 'Review pull requests, commits, diffs, branches, changed files, architecture-diagram consistency, and root README accuracy in ChampollionGraphicalUserInterface. Use for GitHub Copilot code review and source-to-documentation audits of .NET 10, C#, Avalonia XAML, clean architecture, DTOs, tests, WebView2, process execution, executable search, settings, Mermaid diagrams, README claims, GitHub Actions, Windows Inno Setup packaging, release artifacts, versioning, and documentation.'
argument-hint: 'Review the current changes or a specified pull request, commit, diff, branch, or file.'
user-invocable: true
disable-model-invocation: false
---

# Champollion Code Review

Review changes for defects, regressions, architectural violations, accessibility failures, release risks, and missing tests. Produce findings that are actionable and supported by changed code or a directly affected call path.

Human maintainers should keep this skill synchronized with `docs/tools/skills/champollion-code-review-maintenance.md`.

Use `champollion-diagrams` as the authority for diagram purpose, abstraction level, source grounding, notation, focused/unified consistency, maintenance triggers, and validation. This skill owns the audit and finding decision; `champollion-diagrams` defines what each architecture view is expected to represent.

Use `champollion-readme` as the authority for root README scope, section ownership, evidence mapping, intentional summarization, maintenance triggers, links, and validation. This skill owns the audit and finding decision; `champollion-readme` defines what the repository entry point is expected to represent.

## Review Standard

- Review the change, not the repository in the abstract. Establish the comparison base and inspect the complete relevant diff.
- For an explicit architecture-diagram audit, treat the requested diagram family or repository architecture surface as the review target and report confirmed pre-existing mismatches. For an ordinary code review, report diagram drift only when the change introduces it, worsens it, or leaves a directly affected diagram stale.
- For an explicit root README audit, treat the requested README sections or repository documentation surface as the review target and report confirmed pre-existing mismatches. For an ordinary code review, report README drift only when the change introduces it, worsens it, or leaves a directly affected root-level claim stale.
- Prioritize correctness, behavior, accessibility, security, compatibility, packaging, and release integrity over style preferences.
- Do not report pre-existing issues unless the change introduces them, materially worsens them, or makes them newly reachable.
- Trace each candidate issue from the changed line to the code that computes, consumes, persists, packages, or presents the affected behavior.
- State the triggering condition and observable impact.
- Form a falsifiable hypothesis and run the cheapest available check that could disprove it.
- Treat missing tests as a finding only when changed behavior could regress without practical coverage.
- Do not claim a compile, XAML, test, packaging, or metadata failure when a focused command can verify it.
- Distinguish confirmed defects from checks requiring Windows UI interaction, a clean machine, Inno Setup, WebView2 runtime state, or external Champollion executables.
- Prefer one precise finding for a root cause over overlapping comments.

## Repository Orientation

Verify relevant facts against current files rather than treating this section as permanent truth.

- `ChampollionGraphicalUserInterface.Domain` owns framework-independent enums and models and has no project dependencies.
- `ChampollionGraphicalUserInterface.Application` owns command construction, directional DTOs, execution, search, settings, and validation. It depends only on Domain.
- `ChampollionGraphicalUserInterface` is the .NET 10 Avalonia Windows x64 UI and composition root. It depends on Application and Domain.
- The UI uses `Avalonia.Controls.WebView`, backed by Microsoft Edge WebView2 on Windows.
- The solution entry point is `ChampollionGraphicalUserInterface.slnx`.
- The three test projects mirror their corresponding production projects and relative source folders.
- Windows x64 is the current supported product target. Do not report missing Linux or macOS support as a defect unless the change claims or introduces that support.

## Required Workflow

1. Identify the review target, comparison base, and changed files.
2. Read the complete relevant diff and enough surrounding code to identify ownership and behavior.
3. Classify changes into Domain, Application, UI, tests, CI, Windows packaging, or documentation.
4. Follow changed symbols to callers, bindings, DTO consumers, persistence, generated members, scripts, and release steps.
5. Use `champollion-diagrams` maintenance triggers to identify architecture views that expose each changed or explicitly audited fact.
6. Compare those diagrams with the owning code, project files, workflows, and packaging scripts; check focused and unified counterparts when both expose the fact.
7. Use `champollion-readme` to identify root README sections that expose each changed or explicitly audited fact and compare their claims with owning evidence.
8. Check the mirrored test file and behavioral branches introduced by the change.
9. Run focused validation for each credible defect, diagram-mismatch, or README-mismatch hypothesis.
10. Re-read each exact changed location and affected diagram or README location before reporting it.
11. Report findings in descending severity. If no actionable defect, diagram mismatch, or README mismatch remains, say so and name material validation limitations.

## Validation Commands

Choose commands based on the changed area. Do not report a check as passed unless it completed successfully.

```powershell
dotnet restore .\ChampollionGraphicalUserInterface.slnx
dotnet build .\src\ChampollionGraphicalUserInterface\ChampollionGraphicalUserInterface.csproj -c Release --no-restore
dotnet test .\ChampollionGraphicalUserInterface.slnx -c Release --no-restore
git diff --check
```

For documentation-pattern changes, validate each affected source project with XML documentation enabled:

```powershell
dotnet build .\src\ChampollionGraphicalUserInterface.Domain\ChampollionGraphicalUserInterface.Domain.csproj -c Debug -p:GenerateDocumentationFile=true -p:TreatWarningsAsErrors=true
dotnet build .\src\ChampollionGraphicalUserInterface.Application\ChampollionGraphicalUserInterface.Application.csproj -c Debug -p:GenerateDocumentationFile=true -p:TreatWarningsAsErrors=true
dotnet build .\src\ChampollionGraphicalUserInterface\ChampollionGraphicalUserInterface.csproj -c Debug -p:GenerateDocumentationFile=true -p:TreatWarningsAsErrors=true
```

Force XAML recompilation when incremental output could hide a binding or control error:

```powershell
dotnet build .\src\ChampollionGraphicalUserInterface\ChampollionGraphicalUserInterface.csproj -c Release --no-restore -t:Rebuild
```

Run Windows packaging when packaging, version flow, publish settings, or artifact selection changes:

```powershell
.\scripts\package-windows.ps1 -Version 2.0.0-review
```

Absence of Inno Setup is a limitation: the script can still create the portable ZIP but cannot validate installer compilation.

## Architecture And Contract Checks

- Preserve dependency direction: UI may depend on Application and Domain; Application may depend on Domain; Domain must remain independent.
- Keep stable business vocabulary in Domain and I/O, validation, persistence, search, process, and command behavior in Application.
- Keep Avalonia and operating-system presentation interactions in the UI project.
- `DTO/Input` carries UI-to-Application data. `DTO/Output` carries Application-to-UI data.
- DTOs must remain passive. Flag validation, factories, mutation helpers, lookups, transformations, computed getters, and derived-value calculations placed in DTOs.
- Application-owned enums use the physical `Application/Enum` folder and established plural `.Application.Enums` namespace.
- Keep one public production type per file.
- Every new or renamed non-generated production type needs `TypeNameTests.cs` in the equivalent relative test folder. Exclude attribute-only `Properties/AssemblyInfo.cs`.
- DTO tests verify construction, defaults, and property transport. Owning service tests verify behavior.

## Source Documentation And Region Checks

Apply these checks to hand-written production C# under `src`, not generated files or tests unless explicitly changed for that purpose.

- Types and members require factual XML summaries or accurate inherited documentation.
- Parameters require matching `<param>` elements; non-void methods require `<returns>`; properties use `<value>` where it clarifies meaning.
- Positional records use type-level parameter documentation. Enum types and values are documented.
- Regions, when useful, use `Variables`, `Properties`, `Constructors`, and `Methods` in that order.
- Do not require empty or artificial regions for tiny positional records, enums, empty types, or primary-constructor-only types.
- Keep `[ObservableProperty]`, `[RelayCommand]`, `[GeneratedRegex]`, and similar attributes attached to declarations.
- Do not report documentation style alone above Low severity unless it causes failed strict documentation validation or materially obscures a public contract.

## Execution And Command Checks

- Build process arguments through `ProcessStartInfo.ArgumentList`; do not concatenate user-controlled paths into a shell command.
- Preserve `UseShellExecute = false`, redirected output/error, and no-console-window behavior for captured execution.
- Drain standard output and standard error and await process exit before reporting completion.
- A failed input must not prevent remaining resolved inputs from being attempted.
- Process exit code remains the success signal unless verified executable behavior justifies another rule.
- Error or noteworthy stderr results should continue to produce a diagnostic log.
- Do not expose active execution cancellation unless both supported Champollion editions have verified safe cancellation behavior.
- Check standalone operations, input requirements, option eligibility, output directories, environment expansion, and paths containing spaces.

## Search And Classification Checks

- Search must remain bounded, cancellable, concurrent, and limited to ready local fixed drives.
- Preserve exclusion of Windows system and unrelated application directories unless verified installation evidence changes the rule.
- The first valid edition-matching candidate wins; all workers must be cancelled and awaited.
- Legacy classification requires the complete layout: `Decompiler.dll`, `Pex.dll`, `vcredist_x64.exe`, and `doc/Readme.html`.
- A partial Legacy layout is ambiguous and must not be accepted as Legacy or Current.
- A standalone executable is the Current layout. File version `1.0.x` may corroborate Legacy but must not accept an incomplete Legacy distribution.
- Do not report filename-only matching as sufficient edition validation.

## Settings And Path Checks

- Settings live in `<application directory>\UserData\settings.json`; logs live in `UserData\Logs`.
- Preserve migration from `%LOCALAPPDATA%\ChampollionGraphicalUserInterface`, deleting a legacy item only after successful copy or when the destination already exists.
- Keep Legacy and Current executable paths separate.
- Keep option profiles isolated by edition and game.
- Do not persist input, source-output, or assembly-output paths.
- Reject UNC, mapped network, removable-drive, and protected Windows output locations.
- Expand supported environment variables before validation and execution.

## Avalonia And Accessibility Checks

- The UI enables compiled bindings. Check binding paths, `x:DataType`, item-template types, converters, and generated CommunityToolkit properties against actual types.
- Validate Avalonia API claims against the referenced package version or a forced XAML build; do not import WPF assumptions.
- Check keyboard focus, tab order, automation names, control labels, command enablement, contrast, text wrapping, clipping, and responsive layout for changed UI.
- A successful XAML build proves type/property resolution, not visual layout, focus behavior, WebView navigation, or clipboard/file-picker behavior.
- WebView2 is a Windows runtime prerequisite. Do not assume the embedded Help browser proves cross-platform Avalonia support.
- Preserve live-output autoscroll, stable readable colors, complete output copying, and non-overlapping controls when related views change.

## Windows Packaging And Version Checks

- Treat the Inno Setup `AppId` as permanent upgrade identity.
- Keep product name, executable name, x64 architecture, publish directory, installer source, shortcuts, run entry, and artifact patterns aligned.
- The packaging `-Version` value must reach `dotnet publish`, generated assembly/file/informational metadata, installer metadata, artifact names, and checksums.
- Command-line `-p:Version` overrides the project default. Do not flag a differing local fallback without checking evaluated or emitted metadata.
- The installer and portable ZIP intentionally consume the same self-contained `win-x64` publish directory.
- Packaging must fail if `Champollion.exe` is present. This repository must never distribute third-party Champollion source or binaries.
- Do not claim installer, upgrade, launch, or uninstall validation when those actions were not exercised on Windows.

## GitHub Actions And Release Checks

- Check action versions, tag normalization, prerelease-compatible versions, permissions, job dependencies, artifact names, checksums, and release globs.
- With the current upload wildcard, `artifacts/packages` is not retained as a nested prefix inside the uploaded artifact.
- With `download-artifact` and `merge-multiple: true`, downloaded files are merged into the configured `artifacts` directory.
- The release job has no checkout and therefore requires `GH_REPO` or `--repo` for GitHub CLI repository context.
- Support both tag paths: workflow-created releases and releases already created through the GitHub UI.
- A failed `gh release create` should fall back to upload only when an existing release is confirmed; unrelated creation failures must remain visible.
- Ensure missing required artifacts fail visibly rather than publishing an incomplete release.

## Documentation Checks

- Require documentation updates when commands, prerequisites, supported targets, settings behavior, artifact names, install paths, versioning, maintenance procedures, or another documented contract changes.
- Do not request documentation churn when a change restores already documented behavior.
- Keep `.github` customization definitions synchronized with their human guides under `docs/tools`.

Use `champollion-readme` for root README checks. During ordinary change review, inspect only directly affected root-level claims. During an explicit README audit, inspect the requested sections and report verified pre-existing discrepancies.

A root README mismatch must cite both the owning code, project, workflow, packaging, legal, customization, or detailed documentation evidence and the affected README location. Do not report intentional root-level summarization, omitted implementation detail, or an internal-only change that does not alter a current user or contributor claim. Missing README maintenance is generally Low severity; use Medium only when false prerequisite, compatibility, configuration, packaging, security, or release guidance creates a concrete user or release risk.

## Architecture Diagram Consistency Checks

Use `champollion-diagrams` to select candidate views and interpret their intended scope. Architecture diagrams live under `docs/architecture/diagrams` and include context, container, component, package, deployment, sequence, data-flow, security, UML class, and communication views.

For each changed or explicitly audited fact:

1. Identify the owning source, project file, workflow, or packaging script.
2. Select only diagram views whose documented abstraction level exposes that fact.
3. Compare names, boundaries, ownership, dependencies, collaborators, messages, data, controls, risks, runtime nodes, and deployment steps as applicable.
4. Check focused and unified counterparts for contradictory representations.
5. Distinguish an intentional omission from stale, missing, unsupported, or contradictory architecture.
6. Validate any changed Mermaid structure, local links, notation, and normal-zoom readability when diagrams are part of the review target.

Treat these as mismatch candidates:

- a production type is added, removed, renamed, moved, or retyped but a class diagram claiming project coverage remains stale;
- project references, namespaces, package dependencies, service wiring, or ownership differ from package or component diagrams;
- implemented calls, callbacks, guards, cancellation, or workflow outcomes contradict sequence or communication diagrams;
- DTOs, settings, paths, process streams, generated files, or persistence differ from data-flow diagrams;
- an implemented security control is missing or misrepresented, or an absent control is shown as implemented;
- runtime processes, external systems, supported platforms, storage, WebView, or desktop boundaries contradict context or container diagrams;
- CI runners, packaging steps, artifact names, checksums, release behavior, or distribution paths contradict deployment or supply-chain security diagrams;
- focused and unified views use incompatible names, boundaries, dependencies, or principal flows for the same architecture.

Do not report a mismatch merely because a diagram intentionally summarizes detail outside its scope. Examples include generated members omitted from class diagrams, failure alternatives omitted from a documented happy-path unified view, transitive packages omitted from the direct-dependency summary, or internal services omitted from System Context.

A diagram mismatch finding must cite both sides of the contradiction: the source or configuration evidence and the affected diagram location. State the exact stale or false representation and the maintenance impact. Missing diagram updates are generally Low severity; use Medium only when misleading security, deployment, compatibility, or operational guidance creates a concrete user or release risk.

## Severity And Finding Format

Use these severities:

- **Critical**: Data loss, credential exposure, arbitrary code execution, or compromised release artifacts.
- **High**: Build, startup, execution, installation, upgrade, or core workflow failure for Windows x64.
- **Medium**: User-visible incorrect behavior, accessibility failure, incomplete release, architectural violation with concrete impact, or likely regression.
- **Low**: Narrow correctness or maintainability defect with limited impact. Do not use Low for optional polish.

For each finding provide:

- an imperative title;
- severity and confidence;
- a precise changed-file location;
- both owning evidence and affected documentation locations for architecture-diagram or root README mismatch findings;
- trigger and observable impact;
- evidence or focused failed validation;
- a bounded remediation direction.

Findings come first. Keep summaries secondary. Do not invent findings to fill severity categories. If no findings remain, state that clearly and list only material residual risks or checks that could not run.
