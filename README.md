# Champollion Graphical User Interface

A Windows x64 desktop interface for external Champollion PEX-to-Papyrus command-line tools. It presents command parameters as approachable controls, validates local paths, keeps the interface responsive, and captures standard output and errors in the application.

This repository does not contain or distribute either Champollion executable. Download the edition you need separately and select its local `.exe` in the application.

## Features

- Legacy and Current Champollion profiles with separate saved executable paths.
- Skyrim, Skyrim Special Edition, Fallout 4, Fallout 76, and Starfield compatibility rules.
- Individual `.pex` file and directory input through browsing or pasted paths.
- All documented parameters except threaded mode, which remains hidden pending executable behavior research.
- Distinct Help, Version, Print Information, Print Compile Time, and Decompile operations.
- Structured process arguments, including safe handling of paths containing spaces.
- Responsive per-file execution that drains standard output and standard error before reporting completion.
- Failure isolation: one failed file does not stop remaining files, and failures are summarized and logged.
- Bounded, cancellable automatic search across local fixed drives, including Program Files, with protected system and known unrelated application paths excluded.
- Per-user executable and edition-plus-game option persistence. Input and output paths are not persisted.
- In-app Help, output copy, diagnostic log navigation, progress, and run confirmation.
- An embedded Help browser for the official Legacy and Current Nexus Mods download pages.

Automatic search looks for `Champollion.exe` throughout local fixed drives, including `Program Files` and `Program Files (x86)`, and classifies the supplied distributions conservatively. Legacy requires the complete companion layout (`Decompiler.dll`, `Pex.dll`, `vcredist_x64.exe`, and `doc/Readme.html`); Current is the standalone distribution. A partial Legacy layout is treated as ambiguous and rejected. File version `1.0.x` corroborates Legacy but is not used alone to accept an incomplete distribution. Protected Windows directories and known unrelated vendor or game-store trees remain excluded.

## Architecture

```text
src/
	ChampollionGraphicalUserInterface/              Avalonia views and view models
	ChampollionGraphicalUserInterface.Application/  Use cases, DTO input/output contracts, enums, validation, execution, search, settings
	ChampollionGraphicalUserInterface.Domain/       Enums and models
tests/
	ChampollionGraphicalUserInterface.Application.Tests/
	ChampollionGraphicalUserInterface.Domain.Tests/
	ChampollionGraphicalUserInterface.Tests/
```

Each test project mirrors the folder structure of the source project it covers. Every non-generated C# class file has a corresponding `*Tests.cs` file in the equivalent relative folder; assembly metadata files are excluded.

Application contracts sent from the UI are stored under `DTO/Input`; results and progress sent back to the UI are stored under `DTO/Output`. DTOs are data-only and contain no validation, transformation, lookup, or derived-value logic. Application-owned enums are stored under `Enum`. Each contract and enum has its own source file.

The solution targets .NET 10 and uses the `.slnx` format.

## Documentation

The [`docs`](docs) directory contains architecture references, durable engineering reports, and human-readable guides for the repository's GitHub Copilot customizations.

| Area | Contents |
| --- | --- |
| [Architecture](docs/architecture/README.md) | Internal design documentation covering implementation algorithms and the complete architecture diagram suite. |
| [Algorithms](docs/architecture/algorithms/README.md) | Detailed behavior such as concurrent local-drive discovery and conservative Legacy or Current executable classification. |
| [Architecture diagrams](docs/architecture/diagrams/README.md) | Source-grounded Mermaid views of system scope, runtime boundaries, static structure, behavior, data, security, types, and delivery. |
| [Evaluation reports](docs/reports/README.md) | Immutable timestamped engineering reports produced by repository custom agents, including .NET platform evaluations. |
| [Copilot skills](docs/tools/skills/README.md) | Contributor guides for using and maintaining the development, code-review, architecture-diagram, and root README skills. |
| [Copilot agents](docs/tools/agents/README.md) | Contributor guides for custom agents and their durable outputs, including the .NET Platform Evaluator. |

The architecture diagram collection includes:

- [System Context](docs/architecture/diagrams/system-context.md) and [Container](docs/architecture/diagrams/container-diagram.md) views for external relationships and runtime boundaries.
- [Focused component](docs/architecture/diagrams/components/README.md), [unified component](docs/architecture/diagrams/unified-component-diagram.md), [detailed package](docs/architecture/diagrams/gui-package-diagram.md), [summary package](docs/architecture/diagrams/summary-gui-package-diagram.md), and [layered package](docs/architecture/diagrams/uml-gui-package-diagram.md) views for static responsibilities and compile-time dependencies.
- [Sequence](docs/architecture/diagrams/sequences/README.md) and [Communication](docs/architecture/diagrams/communications/README.md) views for ordered interactions and numbered object collaborations.
- [Data Flow](docs/architecture/diagrams/data-flows/README.md) and [Security](docs/architecture/diagrams/security/README.md) views for data movement, trust boundaries, implemented controls, and residual risks.
- [UML Class](docs/architecture/diagrams/class-diagrams/README.md) views for per-project production types and source-visible relationships.
- [Deployment](docs/architecture/diagrams/deployments/README.md) views for GitHub Actions packaging, GitHub Releases, and optional manual distribution to mod websites.

## Copilot Customizations

The repository includes GitHub Copilot customizations for distinct engineering workflows:

- [Champollion development skill](docs/tools/skills/champollion-development.md) for implementation, refactoring, and fixes.
- [Champollion code review skill](docs/tools/skills/champollion-code-review-maintenance.md) for defect-oriented change review and source-grounded architecture-diagram and root README consistency audits.
- [Champollion diagrams skill](docs/tools/skills/champollion-diagrams.md) for creating, maintaining, and auditing source-grounded architecture diagrams.
- [Champollion README skill](docs/tools/skills/champollion-readme.md) for keeping the repository entry point concise and synchronized with product, contributor, documentation, customization, configuration, and release changes.
- [Champollion Architect](docs/tools/agents/champollion-architect.md) for reviewing new-feature code paths, patterns, best practices, changes, rationale, and proposed diagrams in a private Git-ignored design workspace before implementation.
- [Champollion .NET Platform Evaluator](docs/tools/agents/champollion-dotnet-platform-evaluator.md) for timestamped .NET, Windows x64, Avalonia/WebView2, proposed-platform, and release-readiness reports.

## Build and Test

Install the .NET SDK selected by `global.json`, then run:

```powershell
dotnet restore .\ChampollionGraphicalUserInterface.slnx
dotnet test .\ChampollionGraphicalUserInterface.slnx -c Release
dotnet run --project .\src\ChampollionGraphicalUserInterface\ChampollionGraphicalUserInterface.csproj
```

These commands cover normal build, test, and run workflows. Before submitting production code changes, follow the stricter [Copilot skill validation checklist](docs/tools/skills/champollion-development.md#validation-checklist), which also checks XML documentation and compiler warnings in each source project.

## Using the Application

1. Select Legacy or Current Champollion and the target game.
2. Browse to the intended executable, select the application-directory option, or start automatic search.
3. Choose an operation and select or paste the input path when required.
4. Select compatible output and advanced options.
5. Confirm the run, then use **Open folder** beside the source or assembly output to find generated files.
6. Review and copy the complete process output in the workspace.

Paths must resolve to local fixed drives. UNC paths, mapped network locations, removable drives, and protected output locations are rejected. Missing eligible output directories are shown in the confirmation and created only after approval.

At startup, the Papyrus source output path is populated with `<application directory>\ChampollionGraphicalUserInterfaceOutput`, and the assembly output path is populated with `<application directory>\ChampollionGraphicalUserInterfaceAssembly`. The **Open folder** buttons use the paths displayed in those fields. The status area reports when a resolved folder does not exist yet.

The Help tab includes an embedded browser with selectors for the Legacy Skyrim page and Current Starfield page. The browser uses Microsoft Edge WebView2. WebView2 is included with Windows 11; Windows 10 systems without it must install the Microsoft Edge WebView2 Runtime before using the embedded download pages.

## Credits and Licenses

Champollion Graphical User Interface is Copyright © 2026 Arron-Dominion and is distributed under the [MIT License](LICENSE).

The external Legacy Champollion page credits **li1nx** as its creator. The external Current Champollion page credits **Nikitalita** as its creator and names **Orvid King**, **Nikita Lita**, and **Paul-Henry Perrin** in its file credits. Neither executable is included in this repository or its release packages; users obtain and license those tools separately.

The application uses Avalonia UI, Avalonia.Controls.WebView, CommunityToolkit.Mvvm, Microsoft .NET, and the Inter typeface. Inno Setup is an optional installer build tool and Microsoft Edge WebView2 is a separately supplied Windows runtime prerequisite. Copyright, license summaries, complete MIT and SIL Open Font License texts, upstream links, and external-tool credits are recorded in [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt). `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt` are included in portable and installed application directories.

## Saved Configuration

Configuration is stored in a clearly named folder beside the running executable:

```text
<application directory>\UserData\settings.json
```

Diagnostic logs are stored in `<application directory>\UserData\Logs`. On first launch, settings and logs from the previous `%LOCALAPPDATA%\ChampollionGraphicalUserInterface` location are copied into `UserData` and removed from the old location after each successful copy.

Settings are saved through a temporary file and atomically replaced to reduce partial-write corruption. If `settings.json` cannot be parsed, it is preserved in `UserData` as a timestamped `settings.corrupt-*.json` file and the application starts with default settings.

The Help tab displays the resolved settings path and provides an **Open settings folder** action. The JSON uses `LegacyExecutablePath` and `CurrentExecutablePath` for the two external executables. Parameter profiles are stored in `EditionGameOptions` under explicit keys such as `Legacy:Skyrim` and `Current:Skyrim`; this prevents the same game's Legacy and Current selections from sharing values. Input, Papyrus source output, and assembly output paths are never written to settings. Changing the edition or game clears the input path and restores both application output defaults.

The Windows installer creates the adjacent `UserData`, `ChampollionGraphicalUserInterfaceOutput`, and `ChampollionGraphicalUserInterfaceAssembly` folders with standard-user modify access while leaving the application binaries protected. This also preserves upgrade compatibility with existing installations. For portable use, extract the application to a user-writable directory and keep the application files and data folders together.

## Windows Release

Create the portable ZIP and installer with:

```powershell
.\scripts\package-windows.ps1 -Version 2.0.0
```

The script always creates a self-contained `win-x64` portable ZIP and SHA-256 checksum. If Inno Setup 6 or 7 is installed, it also creates an installer and checksum. Outputs are written to `artifacts/packages`.

The packaging script aborts if a third-party `Champollion.exe` is found in publish output. Release packages contain only this application's files and its required .NET/Avalonia runtime dependencies.
