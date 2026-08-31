---
name: champollion-development
description: 'Implement, modify, refactor, or fix code in ChampollionGraphicalUserInterface. Use for changes to its .NET 10 clean architecture, Avalonia UI, Domain or Application layers, DTOs, execution, search, settings, validation, tests, XML documentation, regions, packaging, or release behavior.'
argument-hint: 'Describe the Champollion GUI change to implement, refactor, or fix'
user-invocable: true
disable-model-invocation: false
---

# Champollion Development

Use this workflow for every implementation change in this repository. Preserve the current clean-architecture dependency direction, source organization, documentation pattern, and mirrored test structure.

Use `champollion-code-review` for defect-oriented reviews of pull requests, commits, branches, diffs, or changed files. This skill may inspect surrounding code while implementing a change, but it does not own the dedicated Copilot Review workflow.

## Establish Context

1. Start from the named file, symbol, behavior, failing test, or nearest implementation surface.
2. Read the owning source file and its mirrored test before editing.
3. Check nearby implementations for the established local pattern.
4. If the requested change alters an architectural boundary, verify the current project references and update this skill in the same change.
5. Make the smallest change that resolves the behavior at its owning layer.

## Architecture Boundaries

The dependency direction is:

```text
ChampollionGraphicalUserInterface (Avalonia UI)
    -> ChampollionGraphicalUserInterface.Application
    -> ChampollionGraphicalUserInterface.Domain

ChampollionGraphicalUserInterface.Application
    -> ChampollionGraphicalUserInterface.Domain

ChampollionGraphicalUserInterface.Domain
    -> no project dependencies
```

Never add a reverse dependency. Domain must not reference Application, Avalonia, persistence, processes, or operating-system APIs. Application must not reference the UI project or Avalonia.

### Domain

Place stable business vocabulary in `src/ChampollionGraphicalUserInterface.Domain`:

- `Enums/`: domain-wide choices such as edition, operation, and supported game.
- `Models/`: framework-independent requests, options, and business data.

Domain records contain domain data. Put compatibility validation, path handling, persistence, process execution, and presentation behavior outside Domain.

### Application

Place use-case behavior in `src/ChampollionGraphicalUserInterface.Application`:

- `CommandLine/`: translate validated domain requests into structured command arguments.
- `DTO/Input/`: data sent from UI to Application.
- `DTO/Output/`: results, progress, and validation data returned from Application to UI.
- `Enum/`: Application-owned enums, even when primarily associated with input or output. Preserve the established plural `.Enums` namespace despite the singular folder name.
- `Execution/`: process execution, output capture, result calculation, and diagnostic logging.
- `Search/`: executable discovery, traversal, and edition classification.
- `Settings/`: settings persistence, migration, profile lookup, and profile mutation.
- `Validation/`: compatibility and local-path rules.

DTOs are passive data contracts only. They may declare record/class properties but must not contain validation, factories, lookup, mutation helpers, transformations, computed getters, or derived-value calculations. Compute values in the owning Application service and supply them to the DTO.

Keep one public type per production file. Match the file name to the type name and retain the established namespace for that folder.

### UI

Place presentation behavior in `src/ChampollionGraphicalUserInterface`:

- `App.axaml.cs`: composition root and service construction.
- `Views/`: Avalonia controls, dialogs, browser/clipboard/file-picker behavior, and view-only event handling.
- `ViewModels/`: bindable state, commands, UI workflow orchestration, and calls into Application services.
- `Converters/`: presentation-only value conversion.

Do not move validation, persistence, search, command construction, or process execution into views or view models. Keep code-behind limited to view-specific platform interactions. Preserve CommunityToolkit source-generator attributes and keep each attribute adjacent to its declaration.

## Placement Decisions

Before adding a type or member, ask in order:

1. Is it stable business vocabulary with no framework or I/O dependency? Put it in Domain.
2. Is it use-case logic, validation, persistence, search, process execution, or command construction? Put it in Application under the owning feature folder.
3. Is it only a directional boundary contract? Put passive data in `DTO/Input` or `DTO/Output` according to data flow.
4. Is it Application-owned enumeration? Put it in `Application/Enum` and use the established `ChampollionGraphicalUserInterface.Application.Enums` namespace.
5. Is it bindable state or UI orchestration? Put it in a view model.
6. Is it an Avalonia/platform interaction? Put it in the view or composition root.

Do not create a new abstraction unless it removes meaningful duplication or matches an existing ownership boundary.

## Documentation Pattern

Apply this pattern to hand-written production C# files under `src`. Document every type and member, including private and internal fields, properties, constructors, methods, event handlers, and partial hooks. Do not add XML documentation or regions to test files unless the task explicitly requests it.

- Add `/// <summary>` describing responsibility or behavior.
- Add one matching `/// <param>` for every method, constructor, primary-constructor, and positional-record parameter.
- Add `/// <returns>` to every non-void method, including methods returning `Task` or `Task<T>`.
- Add `/// <value>` to properties when it clarifies the returned value, nullability, or boolean meaning.
- Use `/// <inheritdoc/>` for overrides when inherited documentation is accurate; add parameter documentation where the local pattern does so.
- Document enum types and every enum value.
- For positional records, document the record with a summary and type-level parameter elements. Do not add artificial regions.
- For property-based records, document the type and every property.
- Keep comments factual and specific. Do not narrate syntax or duplicate the member name without explaining its role.

Keep XML documentation aligned with the declaration and place it before declaration attributes. After documentation edits, compile source projects with XML documentation enabled and warnings treated as errors.

## Region Pattern

Use regions only when they improve navigation. Use these exact names and this order when applicable:

```text
#region Variables
#region Properties
#region Constructors
#region Methods
```

- `Variables`: constants and fields, including `[ObservableProperty]` backing fields.
- `Properties`: explicit, computed, and collection properties.
- `Constructors`: explicit constructors.
- `Methods`: public, internal, protected, and private methods, event handlers, command methods, and partial hooks.

Do not add empty or artificial regions. Tiny positional records, enums, empty types, and primary-constructor-only types may omit inapplicable regions. Keep `[ObservableProperty]`, `[RelayCommand]`, `[GeneratedRegex]`, and similar attributes attached to their declarations when moving members.

## Tests

Tests live under `tests` and mirror the production project and relative source folder:

- `ChampollionGraphicalUserInterface.Domain.Tests` covers Domain.
- `ChampollionGraphicalUserInterface.Application.Tests` covers Application.
- `ChampollionGraphicalUserInterface.Tests` covers the Avalonia UI project.

For every new or renamed non-generated production C# type file, add or rename the corresponding `TypeNameTests.cs` in the equivalent relative test folder. Exclude attribute-only assembly metadata such as `Properties/AssemblyInfo.cs`.

Test behavior at its owning layer:

- DTO tests verify construction, defaults, and property transport only.
- Service tests verify validation, calculations, transformations, persistence, search, and execution behavior.
- Domain tests verify enum/model contracts and immutable record behavior.
- UI tests verify view-model state transitions and view/application contracts.

Keep tests focused on the changed behavior. Preserve the one-production-file-to-one-test-file mirror even when related behavior is tested elsewhere.

## Repository Guardrails

- Target .NET 10 and Windows x64 unless an explicit platform decision changes the project.
- Use `ProcessStartInfo.ArgumentList`; never concatenate untrusted paths into a command string.
- Keep process execution asynchronous and drain standard output and standard error before completion.
- Keep executable search bounded, cancellable, edition-aware, and limited to local fixed drives.
- Keep user settings and logs under the application-adjacent `UserData` location and preserve legacy migration behavior.
- Never add or package third-party Champollion source or binaries.
- Never edit generated files or build output under `bin` or `obj`.
- Preserve unrelated user changes and avoid broad refactors during focused work.

## Validation Workflow

After the first substantive edit, run the narrowest affected test or project build. Repair and rerun that check before widening scope.

Before completion, run all applicable checks from the repository root:

```powershell
dotnet build .\src\ChampollionGraphicalUserInterface.Domain\ChampollionGraphicalUserInterface.Domain.csproj -c Debug -p:GenerateDocumentationFile=true -p:TreatWarningsAsErrors=true
dotnet build .\src\ChampollionGraphicalUserInterface.Application\ChampollionGraphicalUserInterface.Application.csproj -c Debug -p:GenerateDocumentationFile=true -p:TreatWarningsAsErrors=true
dotnet build .\src\ChampollionGraphicalUserInterface\ChampollionGraphicalUserInterface.csproj -c Debug -p:GenerateDocumentationFile=true -p:TreatWarningsAsErrors=true
dotnet test .\ChampollionGraphicalUserInterface.slnx -c Debug
```

Also verify:

- No new editor diagnostics exist.
- Regions are balanced and members are in the correct category.
- XML parameter names exactly match declarations.
- DTO files remain data-only.
- Project references preserve the dependency direction.
- Every changed/new production type has its mirrored test file.
- Packaging and documentation are updated when user-visible behavior, prerequisites, or versions change.

## Completion Report

Summarize the behavior and ownership changes, list validation performed and actual test totals, and disclose any check that could not run. Do not claim release artifacts were regenerated unless packaging was executed.
