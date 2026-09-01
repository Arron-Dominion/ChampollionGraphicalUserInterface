# Champollion Development Skill

The `champollion-development` skill teaches GitHub Copilot how to implement, refactor, and fix code in this repository without eroding its clean architecture, directional contracts, documentation style, or test layout.

The active skill definition is [`.github/skills/champollion-development/SKILL.md`](../../../.github/skills/champollion-development/SKILL.md). This guide explains how contributors use and maintain it.

## What the Skill Covers

The skill records the repository's current engineering conventions:

- Domain, Application, and Avalonia UI dependency boundaries;
- ownership rules for models, enums, DTOs, services, view models, and views;
- passive `DTO/Input` and `DTO/Output` contracts;
- one public production type per source file;
- mirrored source and test folder structures;
- XML documentation requirements for production C#;
- mandatory applicable `Constants`, `Variables`, `Properties`, `Constructors`, and `Methods` regions for production classes and structs;
- process, executable-search, settings, packaging, and platform guardrails;
- architecture-diagram impact assessment through `champollion-diagrams` after implementation changes;
- root README impact assessment through `champollion-readme` after implementation and repository changes;
- pre-implementation design handoff from `Champollion Architect` for materially ambiguous or cross-layer new features;
- focused validation followed by full solution tests.

It is intended to guide future implementation, refactoring, bug fixes, packaging changes, and documentation updates. It accepts pre-implementation designs from [`Champollion Architect`](../agents/champollion-architect.md), delegates architecture-diagram selection and maintenance to [`champollion-diagrams`](champollion-diagrams.md), and delegates root README scope and synchronization to [`champollion-readme`](champollion-readme.md). Use the dedicated [`champollion-code-review`](champollion-code-review-maintenance.md) skill for defect-oriented reviews of pull requests, commits, branches, diffs, and changed files.

## Using the Skill

### Automatic Use

The skill allows automatic model invocation. Copilot should load it when a request concerns this repository's source code, architecture, tests, packaging, or release behavior.

Examples:

- `Add validation for a new output option.`
- `Create a progress result returned from Application to the UI.`
- `Refactor executable classification without changing behavior.`
- `Add a new settings field and persist it.`

A contributor does not normally need to name the skill in these requests.

### Manual Use

The skill is user-invocable and can be selected explicitly as `/champollion-development` in Copilot Chat. Manual invocation is useful when a task is broad, when architectural placement is the main concern, or when confirming that the repository conventions are being applied.

Example prompts:

```text
/champollion-development Add a new Current-only decompilation option from Domain through the UI.
```

```text
/champollion-development Move this path rule to its correct clean-architecture layer and update tests.
```

## Expected Workflow

When the skill is active, Copilot should:

1. Start from the requested behavior, source file, symbol, or failing test.
2. Use `Champollion Architect` before implementation when a new feature crosses layers, introduces material design choices, or lacks an approved implementation shape.
3. Read any existing private design package, including `outstanding-questions.md`, and verify proposed current-state claims against source.
4. Abort before editing implementation when the package contains `DESIGN-STATUS: BLOCKED` or a blocking outstanding question; report the unresolved identifiers and required decisions.
5. Read the owning source file and mirrored test only after the design is unblocked.
6. Choose the owning layer before adding a type or method.
7. Make a focused implementation and add or update the mirrored test.
8. Preserve the production XML documentation and region pattern.
9. Run the narrowest affected test or build immediately after the first edit.
10. Use `champollion-diagrams` to assess whether changed types, dependencies, workflows, data, security boundaries, containers, or deployment behavior require diagram updates.
11. Update and validate only the affected focused or unified diagrams and indexes.
12. Use `champollion-readme` to assess whether stable product, contributor, documentation, customization, configuration, packaging, or release claims require root README updates.
13. Update and validate only the affected root README sections and links.
14. Run source documentation builds and the full solution tests before completion when applicable.
15. Report actual validation results, updated diagrams and README sections or their no-impact conclusions, and anything that could not run.

The skill is guidance, not a replacement for engineering review. Contributors should still inspect architectural decisions, generated diffs, tests, and user-visible behavior. Use `/champollion-code-review` when the requested task is review rather than implementation.

## Architecture Summary

The current dependency direction is:

```text
Avalonia UI -> Application -> Domain
Avalonia UI ----------------> Domain
Domain -> no project dependencies
```

The main placement rules are:

| Concern | Owner |
| --- | --- |
| Stable business vocabulary | `ChampollionGraphicalUserInterface.Domain` |
| Use cases, validation, persistence, search, execution, and command construction | `ChampollionGraphicalUserInterface.Application` |
| UI-to-Application data | `Application/DTO/Input` |
| Application-to-UI data | `Application/DTO/Output` |
| Application-owned enums | `Application/Enum`, using the established `.Application.Enums` namespace |
| Bindable state and UI workflow | UI `ViewModels` |
| Avalonia and operating-system interactions | UI `Views` or `App.axaml.cs` composition root |

DTOs carry data only. Validation, factories, mutation helpers, lookups, transformations, computed getters, and derived calculations belong to the owning Application service.

## Architecture Diagram Synchronization

Every production-code, project, workflow, packaging, or release change receives a diagram-impact assessment after its first focused implementation check succeeds. The development skill uses `champollion-diagrams` rather than maintaining a second set of diagram rules.

Typical mappings are:

| Development change | Diagram views to inspect |
| --- | --- |
| Production type added, removed, renamed, or retyped | Owning UML class diagram |
| Service wiring, ownership, project reference, namespace, or package dependency | Component and package diagrams |
| Calls, callbacks, guards, cancellation, or UI workflow | Sequence and communication diagrams |
| DTO, settings, paths, process streams, generated output, or persistence | Data Flow Diagrams |
| Validation, external process launch, WebView, desktop integration, or local-data exposure | Security diagrams |
| Runtime process, external system, supported platform, CI, packaging, artifact, or release path | Context, container, deployment, and supply-chain security diagrams as applicable |

These are candidate views, not a requirement to edit every listed file. A diagram changes only when its abstraction level exposes the modified fact. Focused and unified counterparts should use consistent names, boundaries, and principal flows.

When no existing diagram represents the changed detail, leave the architecture suite unchanged and report that diagram impact was assessed with no update required. This avoids diagram churn while making omission an explicit engineering decision.

## Root README Synchronization

Every production, project, workflow, packaging, release, documentation-index, skill, agent, prerequisite, configuration, or license change receives a root README impact assessment after its first focused implementation check succeeds. The development skill uses `champollion-readme` rather than maintaining a second set of README section and writing rules.

Update only sections whose root-level abstraction exposes the changed fact. Keep detailed implementation, algorithms, diagram notation, skill maintenance, and reports in their owning documents and link to them from the root README. Validate affected links, headings, commands, paths, claims, and diagnostics.

README and diagram impacts are independent. A change may require one, both, or neither. When internal-only work does not alter a current root-level user or contributor claim, leave the README unchanged and report that its impact was assessed with no update required.

## Code Documentation Summary

The convention applies to hand-written production C# under `src`, not generated output or test files unless a task explicitly requests test documentation.

- Every type and member receives a factual XML `<summary>` or appropriate `<inheritdoc/>`.
- Every parameter receives a matching `<param>` element.
- Every non-void method receives a `<returns>` element.
- Private helpers and expression-bodied methods receive the same complete summary, parameter, and return documentation as public methods.
- Properties use `<value>` when it clarifies nullability, meaning, or boolean behavior.
- Positional records document constructor properties with type-level `<param>` elements.
- Enum types and every enum value are documented.
- Attributes remain adjacent to declarations.

Every hand-written production class or struct uses each applicable region, in this order:

```text
Constants
Variables
Properties
Constructors
Methods
```

Compile-time `const` members belong in `Constants`; static-readonly and instance fields belong in `Variables`. Empty regions are not added. Positional records, enums, empty types, interfaces without grouped implementations, and primary-constructor-only types may omit inapplicable regions, but a file being short or newly created is not an exception.

## Maintaining the Skill

Update both the active `SKILL.md` and this guide when a change affects any convention the skill describes.

Common update triggers include:

- adding, removing, or renaming a source or test project;
- changing project references or dependency direction;
- introducing a new Application feature folder or contract direction;
- changing DTO ownership or allowing a new contract shape;
- changing source documentation or region conventions;
- changing target framework, runtime, operating-system support, or UI framework;
- changing test frameworks, test layout, build commands, packaging, or release validation;
- changing the requirement to assess or synchronize architecture diagrams during development;
- changing the responsibility boundary between `champollion-development` and `champollion-diagrams`;
- changing the requirement to assess or synchronize the root README during development;
- changing the responsibility boundary between `champollion-development` and `champollion-readme`;
- changing the Architect design handoff or the responsibility boundary between `champollion-development` and `Champollion Architect`;
- replacing a repository guardrail with a verified new behavior.

Do not update the skill for a one-off implementation detail unless future changes should consistently follow it. Keep rules durable, specific, and supported by the repository.

### Maintenance Steps

1. Verify the current behavior in project files, source code, tests, and contributor documentation.
2. Edit [the skill definition](../../../.github/skills/champollion-development/SKILL.md).
3. Update this guide if the human workflow, architecture summary, examples, or maintenance instructions changed.
4. Keep the skill folder name and frontmatter `name` identical: `champollion-development`.
5. Keep the frontmatter `description` rich in concrete trigger terms so automatic discovery continues to work.
6. Keep `disable-model-invocation: false` for automatic use and `user-invocable: true` for manual use unless the intended behavior changes.
7. Keep `SKILL.md` concise and under 500 lines. Move lengthy human explanations here instead.
8. Review the skill against the live repository rather than relying only on this document.

## Validation Checklist

The source-project documentation builds enable compiler analysis of XML comments and treat warnings as errors. This catches missing summaries, mismatched parameter names, malformed XML, and incomplete return documentation that a normal build may not report.

After changing the skill, verify:

- [ ] The definition exists at `.github/skills/champollion-development/SKILL.md`.
- [ ] YAML frontmatter begins and ends with `---`.
- [ ] The frontmatter name matches the folder name.
- [ ] The description still names the tasks that should trigger the skill.
- [ ] Automatic and manual invocation settings match the intended behavior.
- [ ] Architecture paths, namespaces, project references, and commands match the repository.
- [ ] Documentation and region guidance matches representative production files.
- [ ] Constants use `Constants` regions and are not grouped into `Variables`.
- [ ] Private production methods include summaries, matching parameter elements, and return elements when non-void.
- [ ] Test guidance matches the current test projects and mirrored layout.
- [ ] Diagram-impact assessment and delegation to `champollion-diagrams` match the current development workflow.
- [ ] Completion guidance requires updated diagram names or an explicit no-impact conclusion.
- [ ] Root README impact assessment and delegation to `champollion-readme` match the current development workflow.
- [ ] Completion guidance requires updated README sections or an explicit no-impact conclusion.
- [ ] Architect handoff guidance matches the current private design-package contract.
- [ ] Development checks `outstanding-questions.md` and aborts before editing when the exact blocked-design marker or a blocking question is present.
- [ ] The skill remains under 500 lines.
- [ ] Markdown diagnostics are clear.

When the skill changes alongside production code, also run the normal project validation described in the skill. A documentation-only edit does not require rebuilding the application unless it changes or questions a technical claim.

## Troubleshooting

### The Skill Does Not Load Automatically

Check the following:

1. The file is named `SKILL.md`.
2. It is inside `.github/skills/champollion-development/`.
3. The frontmatter `name` is exactly `champollion-development`.
4. The description includes the terminology used by the request.
5. `disable-model-invocation` remains `false`.
6. The YAML uses spaces and quotes values that contain punctuation such as colons.

After correcting discovery metadata, start a new Copilot Chat request so skill selection is evaluated again.

### The Slash Command Is Missing

Confirm that `user-invocable` is `true`, then reopen Copilot Chat or reload the VS Code window if the skill list has not refreshed.

### The Skill Gives Outdated Guidance

Treat project files and implemented code as the source of truth. Confirm the intended architecture with maintainers, update `SKILL.md`, update this guide, and validate both together. Do not work around a stale rule in production code while leaving the skill incorrect.

## Related Documentation

- [Copilot skills index](README.md)
- [Active skill definition](../../../.github/skills/champollion-development/SKILL.md)
- [Champollion code review skill](champollion-code-review-maintenance.md)
- [Champollion diagrams skill](champollion-diagrams.md)
- [Active diagrams skill definition](../../../.github/skills/champollion-diagrams/SKILL.md)
- [Champollion README skill](champollion-readme.md)
- [Active README skill definition](../../../.github/skills/champollion-readme/SKILL.md)
- [Champollion Architect agent](../agents/champollion-architect.md)
- [Active Architect definition](../../../.github/agents/champollion-architect.agent.md)
- [Repository architecture and build instructions](../../../README.md)
