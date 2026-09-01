# Champollion Diagrams Skill

The `champollion-diagrams` skill teaches GitHub Copilot how to create and maintain the repository's Mermaid architecture diagrams without confusing architectural levels, inventing implementation details, or producing unreadable unified views.

The active skill definition is [`.github/skills/champollion-diagrams/SKILL.md`](../../../.github/skills/champollion-diagrams/SKILL.md). This guide explains how contributors use and maintain it. The active skill remains Copilot's operational source of truth.

## When To Use It

Use this skill when the primary outcome is:

- a new architecture diagram;
- an update to an existing diagram after source, workflow, or packaging changes;
- a focused and unified view of the same architecture;
- a review of diagram accuracy, notation, links, or readability;
- a diagram-family index or visual legend;
- synchronization between diagrams and current repository behavior.

Use `champollion-development` when the primary task is implementing or fixing product behavior. Use `champollion-code-review` for defect-oriented reviews of code, packaging, workflows, or documentation changes.

## How To Invoke It

### Automatic Discovery

Copilot can select the skill when a request mentions Champollion architecture diagrams, Mermaid, system context, containers, components, packages, deployment, sequences, data flows, security, UML classes, communications, visual notation, or diagram maintenance.

Example requests:

```text
Update the execution data-flow diagram for the new progress result.
```

```text
Create a focused component diagram for the new GUI workflow and update the unified component view.
```

```text
Check whether the deployment and supply-chain security diagrams still match the release workflow.
```

### Manual Invocation

The skill is user-invocable as `/champollion-diagrams` in Copilot Chat.

```text
/champollion-diagrams Create a sequence diagram for settings import, including cancellation and invalid data.
```

```text
/champollion-diagrams Synchronize the UML class diagrams with the current production types.
```

```text
/champollion-diagrams Review all communication diagrams for overlapping messages and unclear branches.
```

Manual invocation is useful when a request could otherwise be mistaken for implementation work or when the desired architectural viewpoint is the main concern.

## Diagram Selection Guide

Choose the diagram by the question it answers. Several diagrams can describe the same feature from different viewpoints.

| Question | Diagram to use |
| --- | --- |
| Who and what interacts directly with the application? | System Context |
| Which processes and persistent stores exist at runtime? | Container |
| Which implementation components collaborate for one feature? | Focused Component |
| How do major components collaborate across the application? | Unified Component |
| Which namespaces, projects, and packages reference each other? | Detailed Package |
| What are the direct project and NuGet dependencies at a glance? | Summary Package |
| How do project packages align with Presentation, Application, and Domain layers? | Layered UML Package |
| Where are builds executed and how do artifacts reach users or mod sites? | Deployment |
| In what order do calls, callbacks, loops, and alternatives occur? | Sequence |
| What data enters, moves through, leaves, or persists around the GUI? | Data Flow |
| Where are trust boundaries, implemented controls, and residual risks? | Security |
| Which production types and typed relationships exist in one project? | UML Class |
| Which runtime objects exchange numbered messages? | Communication |

### Focused And Unified Views

Use a focused view for complete feature detail, guarded alternatives, and feature-specific external dependencies. Use a unified view for system-wide orientation or a principal happy path.

A unified diagram remains one conceptual Mermaid canvas. It may use labeled subgraphs to prevent independent workflows from competing for shared visual hubs. Do not force every focused failure path into a unified view when that makes the result unreadable.

## Expected Workflow

When the skill is active, Copilot should:

1. Start from the requested diagram, changed behavior, source file, workflow, or packaging script.
2. Read the existing family index, notation, and focused or unified counterpart.
3. Follow the owning source or configuration path and identify repository-owned and external responsibilities.
4. State what the diagram includes, excludes, and what its arrows mean.
5. Make the smallest source-grounded diagram change.
6. Update visual notation when a new node or relationship type is introduced.
7. Update family and root indexes when pages are added, removed, or renamed.
8. Compare focused and unified views for consistent names, boundaries, and principal behavior.
9. Validate Mermaid structure, links, source claims, and normal-zoom preview readability.

The skill should not infer a control, persisted value, dependency, callback, or deployment guarantee from a type or file name alone.

## Repository Architecture Summary

The production dependency direction is:

```text
ChampollionGraphicalUserInterface (Avalonia UI)
    -> ChampollionGraphicalUserInterface.Application
    -> ChampollionGraphicalUserInterface.Domain

ChampollionGraphicalUserInterface.Application
    -> ChampollionGraphicalUserInterface.Domain

ChampollionGraphicalUserInterface.Domain
    -> no project dependencies
```

The three projects compile into one GUI process. They are not separate runtime containers. Legacy and Current `Champollion.exe` are external third-party processes selected by the user or discovered on local fixed drives and must never be shown as bundled application components.

Important external boundaries include local fixed-drive storage, Windows desktop services, Edge WebView2, Nexus Mods, GitHub Actions, GitHub Releases, and optional manual uploads to mod websites.

## Diagram Family Conventions

### Context And Container

System Context treats the GUI as one system and shows only direct people, systems, executables, storage, web locations, and platform services. Container expands the runtime into the GUI process, external CLI process, persistent storage, and supporting runtimes or services.

Do not expose internal services in System Context. Do not turn the Domain, Application, and GUI projects into separate runtime containers.

### Component And Package

Component diagrams show static implementation collaborators. Focused component diagrams cover one GUI feature; the unified component diagram retains major Application, Domain, storage, platform, browser, and CLI dependencies.

Package diagrams describe compile-time references, not runtime calls:

- the detailed package view expands logical namespaces and responsibilities;
- the summary view collapses each production project and lists direct external packages;
- the layered UML view places collapsed packages in Presentation, Application, Domain, and External Dependency layers.

Verify project and package relationships against current project files and source. Do not include transitive packages in a direct-dependency summary.

### Deployment

Deployment diagrams show execution environments, artifact stores, package movement, release destinations, and manual distribution steps. Keep GitHub-only automation separate from the GitHub-plus-mod-site path when both need explanation.

Verify runner operating systems, action versions, artifact names, tag conditions, checksums, GitHub CLI behavior, and packaging inputs against the current workflow and scripts. Never show third-party `Champollion.exe` inside release artifacts.

### Sequence And Communication

Sequence diagrams answer when messages happen. They show temporal order, asynchronous boundaries, loops, callbacks, cancellation, and alternatives.

Communication diagrams answer which runtime objects collaborate. They use numbered messages but do not imply a vertical timeline. Mermaid has no native communication grammar, so these diagrams use flowcharts with:

- rounded actor nodes;
- rectangular `instance : Type` object nodes;
- solid calls and dashed returns or callbacks;
- nested numbering such as `1`, `1.1`, and `1.1.1`;
- bracketed guards and `*[each item]` loops;
- decision diamonds for genuine guarded branches.

Every decision must have at least two guarded outgoing branches. Put the diamond after the object receives the value being tested. Avoid decorative decisions in linear happy paths.

For dense unified communication views, use one Mermaid canvas with labeled numbered-workflow subgraphs. Lane-local aliases may repeat the same displayed object name so unrelated workflows do not share a routing hub. Use top-to-bottom lanes when vertical stacking makes messages easier to trace, and explain that repeated labels represent the same runtime collaborator.

Do not replace communication arrows with detached message ledgers. Do not split a requested unified communication diagram into separate Mermaid canvases. Avoid custom edge-label CSS because it produced rendering artifacts in VS Code dark preview.

### Data Flow

Data Flow Diagrams model transferred data rather than method-call order:

- rectangles are external entities;
- rounded rectangles are GUI or Application processes that transform data;
- cylinders are persistent, file-backed, or explicitly modeled in-memory stores;
- arrow labels name transferred data.

Keep Level 0 external exchanges balanced with the unified Level 1 decomposition. Individual DFD pages include the family visual legend. Use nouns such as `ChampollionRequest`, path metadata, settings JSON, CLI arguments, stdout/stderr, or generated files rather than command names.

### Security

Security diagrams distinguish:

- external parties and trust assumptions;
- GUI components inside the application boundary;
- implemented controls;
- persistent local data;
- residual risks or missing controls;
- trust-boundary subgraphs.

Every individual security page includes visual notation. Confirm each implemented control in source, workflow, or packaging evidence. Keep absent controls and operational assumptions visibly separate. For example, structured arguments and local fixed-drive validation may be implemented while executable signature verification or signed release provenance remains absent.

### UML Class

Create one class diagram per production project. Inventory all hand-written production types before editing and reconcile the count afterward.

Omit generated CommunityToolkit members, XAML-generated members, compiler-generated record members, and assembly metadata. Relationships must be visible in source:

| Relationship | Mermaid | Evidence |
| --- | --- | --- |
| Generalization | `<|--` | Class inheritance |
| Realization | `<|..` | Implemented interface |
| Composition | `*--` | Owned data with owner lifetime |
| Aggregation | `o--` | Retained independently shared collaborator |
| Dependency | `..>` | Call, creation, parameter, or return use |
| Association | `-->` | Typed property or record field |

Do not draw an association merely because two types are conceptually related.

## Maintaining The Skill

Update both the active skill and this guide when repository behavior or diagram conventions make the current guidance false or incomplete.

Common triggers include:

- adding, removing, or renaming a diagram family or canonical page;
- changing the diagram directory structure or index policy;
- adopting a new Mermaid grammar or visual notation;
- changing focused/unified scope or readability conventions;
- adding, removing, or renaming production projects or layers;
- changing supported platforms, external executables, storage, WebView, or desktop integrations;
- changing workflow runners, packaging, artifacts, checksums, releases, or distribution channels;
- changing the class-relationship evidence standard;
- discovering a recurring rendering problem or validation gap.

Do not add a permanent skill rule for a one-off visual preference unless future diagram work should consistently follow it.

### Maintenance Steps

1. Verify the current convention in representative diagram pages and repository source.
2. Edit [the active skill definition](../../../.github/skills/champollion-diagrams/SKILL.md).
3. Update this guide when purpose, invocation, diagram selection, conventions, or maintenance changes.
4. Update the [skills index](README.md) if the skill name or guide path changes.
5. Keep the folder and frontmatter name identical: `champollion-diagrams`.
6. Keep the description rich in diagram-family and maintenance trigger terms.
7. Preserve `user-invocable: true` and `disable-model-invocation: false` unless invocation policy changes.
8. Keep `SKILL.md` below 500 lines and move extended human explanation into this guide.
9. Run the revised skill against a representative creation and maintenance request.

When a repository implementation change affects diagrams, update the implementation and tests first, then synchronize only the diagram views whose abstraction level exposes the changed fact.

## Validation Checklist

After changing the skill or this guide, verify:

- [ ] The active definition exists at `.github/skills/champollion-diagrams/SKILL.md`.
- [ ] The folder and YAML frontmatter names are both `champollion-diagrams`.
- [ ] Frontmatter begins and ends with `---`.
- [ ] The description still names all tasks and diagram families that should trigger the skill.
- [ ] Manual and automatic invocation settings match the intended behavior.
- [ ] The skill and this human guide describe the same canonical paths and conventions.
- [ ] Diagram types have accurate purpose and creation criteria.
- [ ] Repository architecture and external-boundary facts remain current.
- [ ] Focused/unified and communication-layout guidance matches representative pages.
- [ ] `SKILL.md` remains below 500 lines.
- [ ] Markdown and YAML diagnostics are clear.
- [ ] Local links resolve.
- [ ] No trailing whitespace is present.

For a representative diagram change, also verify:

- Mermaid fences and subgraphs are balanced;
- edges terminate at declared nodes;
- decision nodes have at least two branches;
- indexes link newly added or renamed pages;
- focused and unified terminology agrees;
- class relationships and security controls have source evidence;
- deployment artifacts agree with workflows and packaging scripts;
- the VS Code Markdown preview is readable at normal zoom.

If a Mermaid renderer is unavailable, report that visual validation is limited to editor diagnostics, structural checks, and manual preview. Do not claim a visual render was tested.

## Troubleshooting

### The Skill Is Not Discovered

1. Confirm the file is `.github/skills/champollion-diagrams/SKILL.md`.
2. Confirm the frontmatter name exactly matches the folder.
3. Confirm the prompt uses terms present in the description.
4. Confirm `disable-model-invocation` is `false`.
5. Check YAML and Markdown diagnostics.
6. Reload the VS Code window or start a new Copilot Chat request after metadata changes.

### The Slash Command Is Missing

Confirm `user-invocable: true`, then reopen Copilot Chat or reload VS Code so the skill list refreshes.

### Copilot Chooses The Wrong Diagram Type

State the architectural question rather than only asking for a diagram. For example, ask for data movement rather than runtime order, or compile-time dependencies rather than component collaboration. If confusion recurs, clarify the selection matrix in both the skill and this guide.

### A Diagram Parses But Is Hard To Read

Treat readability as a defect. Check for shared hub nodes, duplicate routes, long edge labels, horizontal clipping, unexplained crossings, or text overlapping lines. Introduce a genuine decision node, consolidate repeated same-pair messages, group independent workflows into labeled subgraphs, or create a focused counterpart. Do not rely only on larger spacing values.

### The Skill Gives Outdated Architecture

Treat project files, source, workflows, packaging scripts, and implemented behavior as authoritative. Update the skill and this guide together instead of modifying production code to satisfy stale diagram guidance.

## Related Documentation

- [Copilot skills index](README.md)
- [Active skill definition](../../../.github/skills/champollion-diagrams/SKILL.md)
- [Architecture diagram index](../../architecture/diagrams/README.md)
- [Champollion development skill](champollion-development.md)
- [Champollion code review skill](champollion-code-review-maintenance.md)
- [Repository README](../../../README.md)
