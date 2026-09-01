---
name: champollion-diagrams
description: 'Create, update, review, or maintain architecture diagrams for ChampollionGraphicalUserInterface. Use for Mermaid system context, container, component, package, deployment, sequence, data-flow, security, UML class, or communication diagrams; focused and unified views; diagram indexes and notation; or synchronizing diagrams with source, workflows, packaging, and release behavior.'
argument-hint: 'Describe the Champollion architecture diagram to create, update, review, or synchronize.'
user-invocable: true
disable-model-invocation: false
---

# Champollion Diagrams

Create and maintain the architecture diagrams under `docs/architecture/diagrams`. Ground every node, relationship, message, data flow, control, and deployment step in current repository evidence. Diagrams document the implemented system unless the request explicitly asks for a clearly labeled proposed architecture.

Use `champollion-development` when implementing product behavior. Use this skill when the primary outcome is an architecture diagram, diagram review, notation change, or synchronization of diagrams after implementation changes.

Human maintainers should keep this skill synchronized with `docs/tools/skills/champollion-diagrams.md`.

## Repository Context

Verify these facts against current source before relying on them:

- `ChampollionGraphicalUserInterface` is the .NET 10 Avalonia presentation project and composition root.
- `ChampollionGraphicalUserInterface.Application` owns use cases, directional DTOs, validation, paths, settings, executable search and classification, command construction, process execution, and diagnostics.
- `ChampollionGraphicalUserInterface.Domain` owns framework-independent requests, options, editions, operations, and supported-game vocabulary.
- Compile-time dependency direction is GUI to Application and Domain, and Application to Domain. Domain has no project dependencies.
- The GUI, Application, and Domain code run in one desktop process. They are project and logical boundaries, not separately deployed containers.
- Legacy and Current `Champollion.exe` are external third-party command-line processes selected or discovered on local fixed drives. This repository does not ship them.
- Local storage includes PEX inputs, generated source and assembly, the selected Champollion distribution, application-adjacent `UserData/settings.json`, corrupt-settings backups, diagnostic logs, and packaged legal documents.
- Windows integrations include file and folder pickers, File Explorer, clipboard, associated applications, fixed-drive discovery, and Edge WebView2.
- WebView2 loads the fixed Legacy Skyrim and Current Starfield Nexus Mods pages for Help browsing.
- `.github/workflows/build-release.yml`, `scripts/package-windows.ps1`, and `packaging/windows` own the Windows build, package, artifact, and release paths.

## Establish Evidence

1. Start from the requested diagram, changed behavior, source file, workflow, package script, or nearest existing diagram.
2. Read the relevant diagram index and its notation before editing. Do not invent a second notation for the same diagram family.
3. Follow the owning implementation path rather than documenting only a registration or forwarding surface.
4. Check source projects, project files, tests, workflows, and packaging scripts as appropriate to the diagram type.
5. Distinguish repository-owned behavior from external software, platform services, user actions, and operational assumptions.
6. State only relationships visible in source or configuration. Do not infer a control, dependency, persisted field, return value, or deployment guarantee from naming alone.
7. Preserve unrelated user edits and keep changes scoped to the requested architecture surface.

## Choose the Diagram Type

Select the view by the question it must answer. Do not use one diagram type to answer a different architectural question.

| Diagram type | Purpose | Create or update when |
| --- | --- | --- |
| System Context | Shows the application as one system and every person, external system, executable, storage boundary, web location, and platform service with which it directly interacts. | Establishing system scope, adding or removing an external integration, changing supported platforms, or changing a direct system boundary. Do not expose internal projects or services here. |
| Container | Expands the system into independently running or persisted runtime containers such as the GUI process, external CLI process, and local storage. | Runtime/process boundaries, launch behavior, storage responsibilities, or platform integrations change. Do not model the three repository projects as separate containers. |
| Focused Component | Shows repository-owned implementation components and external collaborators participating in one GUI feature. | A feature gains, removes, or reallocates services, DTOs, views, view models, platform integrations, or external dependencies. Prefer one focused diagram when a unified view would obscure the feature. |
| Unified Component | Shows the major Presentation, Application, and Domain components and external dependencies in one static collaboration view. | A cross-feature component, composition root, project responsibility, or major external dependency changes. Keep detail below class-member level. |
| Detailed Package | Shows compile-time namespace/package dependencies and project/package responsibilities. | Namespaces, folders, project references, package references, or architectural ownership change. Arrows mean imports or references, not runtime calls. |
| Summary Package | Collapses each production project to one package and groups direct external dependencies by package name. | A concise project-reference or direct NuGet dependency overview is needed, or project/package references change. Omit internal namespaces and transitive dependencies. |
| Layered UML Package | Organizes collapsed project packages into Presentation, Application, Domain, and External Dependency layers. | Explaining clean-architecture direction or changing a project/layer boundary. Dependencies must point inward and Domain must remain independent. |
| Deployment | Shows build or release execution environments, artifact stores, package movement, publication destinations, and manual distribution steps. | GitHub Actions, runners, permissions, packaging, artifact names, checksums, releases, or mod-site distribution changes. Create separate automated GitHub-only and GitHub-plus-manual-distribution views when both paths matter. |
| Focused Sequence | Shows time-ordered calls, asynchronous boundaries, loops, callbacks, guards, and alternate outcomes for one workflow. | Call order, cancellation, concurrency, confirmation, retries, callbacks, or error sequencing matters. Use participants in execution order and represent `alt`, `opt`, `loop`, and async behavior explicitly. |
| Unified Sequence | Shows the principal end-to-end path across multiple workflows in temporal order. | A readable happy path is needed from startup through generated output, or cross-workflow ordering changes. Keep detailed failure alternatives in focused sequences. |
| Level 0 Data Flow | Treats the entire GUI as one process and shows data exchanged with external entities and stores. | Establishing the data boundary, adding a major external data exchange, or changing high-level inputs/outputs. Do not show call order. |
| Unified Level 1 Data Flow | Decomposes the GUI into its major data-transforming processes, stores, and external entities. | Data ownership or movement changes across multiple features, or a unified data view is requested. Balance coverage against readable flow density. |
| Focused Data Flow | Shows data entering, transforming, returning, and persisting for one feature. | DTOs, settings, migration, path metadata, CLI arguments, generated files, process streams, logs, clipboard data, or web content change. Label arrows with data, not method calls. |
| Security Context | Shows trust zones, attack surfaces, external parties, and system-wide trust-boundary crossings. | Platform, external executable, web, local storage, or distribution trust assumptions change. |
| Focused Security | Shows implemented controls and residual risks for local execution, filesystem, settings/logs, desktop/web integration, or supply chain. | A security-relevant validation, process launch, persistence, browser, shell, CI, packaging, checksum, signing, or provenance behavior changes. Never present a desired or absent control as implemented. |
| Unified Security | Summarizes principal runtime and delivery controls, boundaries, and residual risks in one view. | A cross-cutting security overview is needed or several focused security surfaces change together. Preserve the distinction between controls and assumptions. |
| Per-Project UML Class | Shows hand-written production classes, records, enums, inheritance, realization, ownership, and source-visible typed dependencies for one project. | Types, constructors, retained collaborators, properties, record fields, inheritance, implemented interfaces, or important method dependencies change. Omit generated members and assembly metadata. |
| Focused Communication | Shows runtime object instances and numbered messages for one collaboration without implying a vertical timeline. | The question is which objects exchange messages, rather than when lifelines activate. Use decisions for guarded branches and focused views for detailed alternatives. |
| Unified Communication | Combines the principal object collaborations into one canvas. | An end-to-end object collaboration is required. Use numbered workflow subgraphs and lane-local aliases when flows would otherwise compete for shared hub nodes. |

## Canonical Locations

Use the existing structure:

```text
docs/architecture/diagrams/
    README.md
    system-context.md
    container-diagram.md
    unified-component-diagram.md
    gui-package-diagram.md
    summary-gui-package-diagram.md
    uml-gui-package-diagram.md
    components/
    deployments/
    sequences/
    data-flows/
    security/
    class-diagrams/
    communications/
```

Place a focused diagram in its family directory. Update that directory's `README.md` and the root diagram index when adding, removing, or renaming a page. Use lowercase kebab-case filenames. Link focused and unified counterparts where that helps navigation.

## Mermaid And Page Conventions

- Use Mermaid fenced blocks in Markdown.
- Use `flowchart` for context, container, component, package, deployment, data-flow, security, and communication views.
- Use `sequenceDiagram` for sequence views and `classDiagram` for UML class views.
- Give each page one `#` title and a short `## Purpose` that defines scope and exclusions.
- Add notes or a compact responsibility table when relationships need interpretation that would make edge labels too long.
- Keep labels factual and concise. Use `<br/>` only to wrap meaningful phrases.
- Keep arrow meaning consistent within a page and document it when it differs from the family default.
- Prefer a few meaningful subgraphs for system, project, trust, execution-environment, or numbered-workflow boundaries.
- Avoid configuration CSS and hard-coded edge-label backgrounds. They rendered as artifacts in VS Code dark preview.
- Do not solve congestion only by increasing `nodeSpacing` or `rankSpacing`; first reduce duplicate routes, introduce branch nodes, group related components, or create focused views.
- Every edge must visibly terminate at a declared node. Avoid unexplained helper nodes or lines that appear to lead outside the diagram.
- Keep the diagram usable in VS Code Markdown preview at normal zoom. Long horizontal canvases and crossing labels are defects even when Mermaid parses.

## Family-Specific Notation

### Context, Container, Component, Package, And Deployment

- Context nodes describe people and external systems around one application boundary.
- Container nodes describe executable processes, persisted stores, platform runtimes, or deployment destinations.
- Component nodes use implementation type names for repository-owned services and clear role names for grouped contracts.
- Package arrows represent compile-time references only. Verify direct references in project files and source imports.
- Deployment nodes may use UML-style stereotypes such as `<<device>>`, `<<execution environment>>`, `<<artifact store>>`, and `<<deployment destination>>` encoded as text.
- Show artifact movement and conditions such as tag-only publication. Do not imply that `Champollion.exe` is packaged.

### Data Flow

Use the notation defined in `docs/architecture/diagrams/data-flows/README.md`:

- rectangle: external entity;
- rounded rectangle: GUI or Application process that transforms data;
- cylinder: persistent, file-backed, or explicitly modeled in-memory store;
- labeled arrow: transferred data.

Name data flows as nouns such as `ChampollionRequest`, path metadata, structured arguments, stdout/stderr, settings JSON, or generated files. Do not label DFD arrows with UI commands unless the command itself is the modeled data.

Every individual DFD must include the compact visual legend used by the family. Keep Level 0 balanced with Level 1: external inputs and outputs must not disappear merely because the GUI is decomposed.

### Security

Use the notation and class roles defined in `docs/architecture/diagrams/security/README.md`:

- amber rectangle: external party, runtime, process, or trust assumption;
- blue rounded node: component inside the GUI boundary;
- green hexagon: implemented security control;
- cylinder: persistent or file-backed local data;
- red rectangle: residual risk, missing control, or operational assumption;
- dashed subgraph: trust boundary.

Every individual security diagram must include visual notation. Ground controls in exact implementation or workflow evidence. Examples include local fixed-drive validation, structured `ArgumentList`, disabled shell execution for the CLI, redirected streams, package rejection of bundled Champollion, and SHA-256 generation. Keep residual trust explicit, including unsigned external executables, plaintext settings/logs, browser navigation assumptions, or unsigned release provenance when still applicable.

### UML Class

- Create one class diagram per production project.
- Inventory hand-written production types before drawing and reconcile the count after edits.
- Omit CommunityToolkit-generated members, XAML-generated partial members, compiler-generated record members, and assembly metadata.
- Use `<|--` for inheritance, `<|..` for interface realization, `*--` for composition, `o--` for retained shared collaborators, `..>` for dependencies, and `-->` for typed properties or record fields.
- Use aggregation only for a collaborator actually retained by the owner. Use dependency for calls, creation, parameters, or return types that are not retained.
- Do not draw an association to an enum or model unless a source-visible member is typed by it.
- Keep members selective for behavior-heavy types and sufficient to identify passive data contracts.

### Sequence

- Order participants by interaction responsibility and keep external actors or processes at the edges when practical.
- Show awaited work, background callbacks, loops over inputs, cancellation, and guarded alternatives where source behavior requires them.
- Use focused sequences for complete alternatives and the unified sequence for the principal happy path.
- Do not turn static dependencies into messages. Every message must correspond to a call, event, callback, process exchange, or platform interaction.

### Communication

Mermaid has no native communication-diagram grammar. Use a flowchart with UML-style object labels such as `viewModel : MainViewModel`.

- Rounded nodes represent human or platform actors.
- Rectangles represent runtime object instances.
- Solid arrows represent calls, commands, events, or one-way messages.
- Dashed arrows represent returns, results, or callbacks.
- Use nested numbering such as `1`, `1.1`, and `1.1.1`.
- Put guards in brackets and loops in forms such as `*[each input]`.
- Use a diamond immediately after the object receives a result when guarded alternatives would otherwise create several competing edges from that object.
- A decision must have at least two outgoing guarded branches. Do not add decorative diamonds to a linear happy path.
- Consolidate repeated callbacks between the same participants onto one connector when the full numbers and meanings remain explicit.
- For dense unified diagrams, keep one Mermaid canvas but organize independent numbered workflows into labeled subgraphs. Use `flowchart TB` for the canvas and `direction TB` inside each lane when vertical stacking is clearer.
- Lane-local aliases may repeat the same displayed runtime object label. Explain that repeated labels represent the same collaborator and exist only to prevent unrelated workflow routes from sharing a visual hub.
- Keep focused communication diagrams as one notation block and one main diagram block. Do not replace message arrows with detached ledgers or split one requested unified collaboration into separate Mermaid canvases.
- Every communication page using decisions must show the diamond in its visual notation. The family index remains the canonical meaning table.

## Focused Versus Unified Views

Create both views when readers need local detail and system-wide orientation:

- Focused views include complete feature collaborators, guards, error paths, and feature-specific external dependencies.
- Unified views show the principal architecture or happy path and link to focused pages for alternatives.
- A unified diagram means one conceptual and Mermaid canvas. Internal subgraphs and repeated lane-local aliases are allowed for readability.
- Do not force all focused detail into a unified view. If labels overlap, edges cross repeatedly, or the canvas clips at normal preview scale, summarize repeated interactions and point to focused diagrams.
- Keep vocabulary and boundary placement consistent across focused and unified views.

## Maintenance Triggers

When source or configuration changes, inspect the corresponding diagrams:

| Changed surface | Diagrams to inspect |
| --- | --- |
| Composition root, project responsibilities, or service wiring | Container, component, package, class, communication |
| Project references, namespaces, folders, or NuGet dependencies | Detailed, summary, and layered package; component; class |
| View, view-model, command, picker, clipboard, shell, or WebView workflow | Focused component, sequence, communication, DFD, security; unified views if principal flow changes |
| DTO, request, result, progress, or callback contract | Class, DFD, sequence, communication, component |
| Settings schema, profiles, migration, paths, backups, or logs | Component, sequence, DFD, security, communication, class |
| Validation or compatibility rules | Component, sequence, DFD, security, communication, class |
| Executable discovery, cancellation, or classification | Component, sequence, DFD, security, communication |
| Command arguments, process launch, stream capture, results, or diagnostics | Container, component, sequence, DFD, security, communication, class |
| External system, executable, website, runtime, or supported platform | System context, container, component, security, deployment as applicable |
| GitHub Actions, packaging scripts, installer, artifacts, checksums, or publication | Deployment and supply-chain security; context/container only if runtime delivery boundaries change |
| Production type added, removed, renamed, or retyped | Owning project class diagram; package/component diagrams when responsibility changes |

Do not update every listed diagram mechanically. Read each candidate and change it only when its abstraction level exposes the modified fact.

## Required Workflow

1. Identify whether the request is creation, maintenance, review, or proposed architecture.
2. Select the diagram family using the question-to-view matrix above.
3. Read the current family index, focused counterpart, unified counterpart, and directly owning source/configuration.
4. Form one falsifiable scope statement: what the diagram includes, excludes, and what each arrow means.
5. Make the smallest diagram change that represents the verified behavior.
6. Add or update visual notation when introducing a new node or relationship type.
7. Update local and root indexes for added, removed, or renamed diagrams.
8. Check focused and unified counterparts for vocabulary, boundary, and behavior consistency.
9. Run focused validation before widening the edit.

## Validation

After the first substantive edit, check the edited Markdown with editor diagnostics. Before completion verify:

- Mermaid fences are balanced and each diagram uses the intended grammar.
- Every node ID is declared and every edge visibly terminates.
- Local Markdown links resolve and renamed pages have no stale references.
- No trailing whitespace was introduced.
- Titles, Purpose sections, notation, and notes remain consistent with neighboring pages.
- Node and relationship counts agree with source inventories where completeness is claimed.
- Focused and unified views agree on system names, project boundaries, external actors, storage, and principal flows.
- Communication decisions have at least two branches; numbered lanes remain inside one unified canvas; repeated aliases use identical displayed object labels.
- DFD Level 0 and Level 1 external flows remain balanced.
- Security controls and residual risks remain visually and semantically distinct.
- Class relationships are supported by exact source types, members, inheritance, interfaces, construction, or calls.
- Deployment steps and artifact names agree with current workflow and packaging files.
- Diagram previews are readable at normal VS Code zoom without clipped nodes, overlapping text, unexplained crossings, or lines that appear to lead nowhere.

If no Mermaid renderer is installed, report that visual validation is limited to VS Code diagnostics, structural checks, and user preview. Do not claim a visual render was tested.

## Completion Report

Summarize which diagram views changed, the source evidence used, and the validation performed. Call out intentional exclusions, such as a success-only unified flow or focused alternatives. State any visual-preview limitation explicitly.
