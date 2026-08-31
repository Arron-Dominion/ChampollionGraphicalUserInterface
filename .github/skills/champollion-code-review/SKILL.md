---
name: champollion-code-review
description: 'Review pull requests, commits, diffs, branches, and changed files in ChampollionGraphicalUserInterface. Use for GitHub Copilot code review of .NET 10, C#, Avalonia XAML, clean architecture, DTOs, tests, WebView2, process execution, executable search, settings, GitHub Actions, Windows Inno Setup packaging, release artifacts, versioning, and documentation.'
argument-hint: 'Review the current changes or a specified pull request, commit, diff, branch, or file.'
user-invocable: true
disable-model-invocation: false
---

# Champollion Code Review

Review changes for defects, regressions, architectural violations, accessibility failures, release risks, and missing tests. Produce findings that are actionable and supported by changed code or a directly affected call path.

Human maintainers should keep this skill synchronized with `docs/tools/skills/champollion-code-review-maintenance.md`.

## Review Standard

- Review the change, not the repository in the abstract. Establish the comparison base and inspect the complete relevant diff.
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
5. Check the mirrored test file and behavioral branches introduced by the change.
6. Run focused validation for each credible defect hypothesis.
7. Re-read each exact changed location before reporting it.
8. Report findings in descending severity. If no actionable defect remains, say so and name material validation limitations.

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

- Require documentation updates when commands, prerequisites, supported targets, settings behavior, artifact names, install paths, versioning, or maintenance procedures change.
- Do not request documentation churn when a change restores already documented behavior.
- Keep `.github` customization definitions synchronized with their human guides under `docs/tools`.

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
- trigger and observable impact;
- evidence or focused failed validation;
- a bounded remediation direction.

Findings come first. Keep summaries secondary. Do not invent findings to fill severity categories. If no findings remain, state that clearly and list only material residual risks or checks that could not run.
