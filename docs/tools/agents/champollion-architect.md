# Champollion Architect Guide

## Purpose

`Champollion Architect` is a repository-specific GitHub Copilot custom agent for designing new features before implementation. It reviews current application code, architecture, design patterns, framework guidance, quality attributes, tests, documentation, packaging, and release implications.

The active agent definition is [`.github/agents/champollion-architect.agent.md`](../../../.github/agents/champollion-architect.agent.md).

Every completed architecture task creates a private design package under:

```text
Feature/Design/<feature-slug>/
```

The root `.gitignore` excludes `Feature/Design`, so exploratory designs and proposed diagrams remain local and do not enter Git history.

## When To Use It

Use the Architect before implementation when:

- adding a user-visible feature or workflow;
- introducing a service, contract, integration, persistence model, or cross-layer behavior;
- evaluating multiple design patterns or implementation boundaries;
- changing asynchronous execution, cancellation, search, settings, validation, or diagnostics;
- adding platform, WebView, desktop, packaging, or release behavior;
- a feature needs proposed diagrams, a change inventory, and documented rationale;
- requirements are sufficiently complex that implementation should be split into independently verifiable slices.

For a narrow defect with an obvious owner and no design choice, use [`champollion-development`](../skills/champollion-development.md) directly. Use [`champollion-code-review`](../skills/champollion-code-review-maintenance.md) to review implementation rather than design it.

## How To Run The Agent

1. Open GitHub Copilot Chat.
2. Select `Champollion Architect` from the agent selector.
3. Describe the feature goal, users, constraints, compatibility requirements, and known acceptance criteria.
4. Answer only clarifying questions that materially affect the design.
5. Review the generated package under `Feature/Design` before authorizing implementation.

Example requests:

```text
Design a feature that lets users cancel automatic executable discovery and preserve partial progress.
```

```text
Design support for a new Current-edition operation from Domain through the Avalonia UI.
```

```text
Evaluate designs for per-game output profiles. Compare persistence and migration approaches before recommending one.
```

```text
Design a release-channel selector and identify UI, settings, validation, packaging, documentation, and security impacts.
```

## Output Contract

Each feature uses a filesystem-safe lowercase kebab-case directory:

```text
Feature/Design/<feature-slug>/
    README.md
    proposed-changes.md
    design-rationale.md
    outstanding-questions.md
    diagrams/
        proposed-design.md
```

If a slug belongs to a different request, the agent appends a UTC timestamp rather than overwriting it. If it is the same design, the agent reads and updates the package coherently.

### Design Overview

`README.md` records:

- feature summary and user value;
- status, scope, and non-goals;
- current-state repository evidence;
- assumptions, constraints, risks, and unresolved questions;
- recommended design summary;
- links to changes, rationale, and diagrams.

Valid statuses are `Proposed`, `Needs decision`, and `Ready for implementation`.

When a blocking question prevents a clear or safe design path, both the overview and proposed design must place this exact statement immediately after their titles:

```text
DESIGN-STATUS: BLOCKED
ABORT IMPLEMENTATION: Do not implement or continue this proposed design until every blocking question in outstanding-questions.md is resolved and the design is explicitly marked Ready for implementation.
```

Future Copilot calls must treat this statement as a hard stop. They should report the unresolved question identifiers and must not edit implementation files.

### Proposed Changes

`proposed-changes.md` inventories every affected layer and repository surface. Each change identifies its owner, reason, dependencies, expected behavior, and validation.

The inventory covers applicable production code, DTOs, UI, composition, persistence, migration, concurrency, errors, security, accessibility, tests, projects, dependencies, workflows, packaging, release behavior, canonical diagrams, and root README sections. It finishes with ordered implementation slices and focused validation gates.

### Design Rationale

`design-rationale.md` assigns stable identifiers such as `DR-001` to material decisions. Each decision records:

- context and requirement;
- viable alternatives;
- selected option;
- repository and external evidence;
- relevant pattern or guidance;
- trade-offs and rejected alternatives;
- consequences, risks, reversibility, and assumptions.

The agent does not label a choice as a best practice without explaining the concrete benefit. Current framework, package, security, platform, and lifecycle claims cite authoritative sources with access dates.

### Outstanding Questions

`outstanding-questions.md` always exists. It states `No outstanding questions.` when the design is settled. Otherwise, every item has a stable identifier, explains what is unclear and why it changes the design, identifies affected decisions and diagrams, lists known options and consequences, names the resolver, and is classified as `Blocking` or `Non-blocking`.

Any blocking item forces `Needs decision` status and the exact abort statement in both `README.md` and `diagrams/proposed-design.md`. A non-blocking item may remain only when the design records a bounded assumption that cannot invalidate the implementation path.

### Proposed Diagrams

`diagrams/proposed-design.md` contains at least one Mermaid diagram selected for the feature's principal architecture question. Diagrams follow [`champollion-diagrams`](../skills/champollion-diagrams.md) conventions but remain private proposals rather than updates to canonical architecture documentation.

Each diagram includes a proposed title, purpose and exclusions, notation when needed, visible distinction between retained and changed elements, explicit assumptions for unimplemented behavior, and links to supporting decisions and changes.

## Responsibility Boundaries

The Architect uses repository skills as procedural authorities:

| Skill | Responsibility |
| --- | --- |
| [`champollion-development`](../skills/champollion-development.md) | Implementation ownership, source conventions, mirrored tests, validation, and maintenance gates. |
| [`champollion-diagrams`](../skills/champollion-diagrams.md) | Diagram selection, Mermaid notation, proposal clarity, and later canonical synchronization. |
| [`champollion-readme`](../skills/champollion-readme.md) | Root README impact assessment after implementation. |
| [`champollion-code-review`](../skills/champollion-code-review-maintenance.md) | Independent post-implementation defect and documentation consistency review. |

Repository source and configuration remain the evidence for current behavior. Skills define workflow rather than replacing source evidence.

The Architect may write only under `Feature/Design`. It does not implement code, update tests, modify projects, change canonical diagrams, edit the root README, or alter tracked documentation and customization files during a design run.

## Reviewing A Design Package

Before approving implementation, confirm:

1. Scope and acceptance criteria describe the intended user outcome.
2. Current-state claims link to the actual owning source.
3. The selected architecture preserves clean dependency direction.
4. Existing services and patterns were considered before new abstractions.
5. Alternatives are genuinely viable and rejected for explicit reasons.
6. Proposed changes account for tests, accessibility, security, migration, errors, and compatibility where applicable.
7. Diagrams label unimplemented behavior as proposed.
8. Every material change maps to a rationale decision and validation step.
9. Blocking assumptions and unresolved decisions are visible.
10. Implementation can proceed in small independently verifiable slices.

A package marked `Needs decision`, containing a blocking question, or containing `DESIGN-STATUS: BLOCKED` must not be handed to implementation. Future Copilot calls must abort implementation, report the unresolved identifiers, and wait for the required decisions. Once resolved, update the question dispositions, rationale, changes, diagrams, and overview; remove the marker only when the design is explicitly `Ready for implementation`.

## Git Ignore Behavior

The repository uses this root rule:

```gitignore
/Feature/Design/
```

Verify a package is ignored with:

```powershell
git check-ignore -v --no-index Feature/Design/<feature-slug>/README.md
git status --short
```

The first command should identify the root rule. The second command must not list design-package files.

The folder is local by design. It will not appear in another clone and is not a durable team artifact unless a maintainer deliberately moves an approved design into tracked documentation.

## Maintaining The Agent

Update the active agent and this guide when any of these change:

- design package location, file structure, or Git policy;
- clean-architecture ownership or test conventions;
- required design concerns, statuses, rationale fields, or diagram rules;
- outstanding-question fields, blocking criteria, or the exact short-circuit statement;
- responsibility boundaries among architecture, development, diagrams, README maintenance, and review;
- supported platform, framework, packaging, or release baselines;
- validation requirements or allowed repository writes.

When the agent changes:

1. Verify current repository structure and skill responsibilities.
2. Update [the active definition](../../../.github/agents/champollion-architect.agent.md).
3. Synchronize this guide and the [agents index](README.md).
4. Update the root README when the customization summary changes.
5. Confirm frontmatter, tools, and invocation settings.
6. Run a representative feature-design request.
7. Verify the complete package and rationale traceability.
8. Verify Git ignores every generated design artifact.
9. Confirm the agent changed no file outside `Feature/Design` during the design run.

## Validation Checklist

- [ ] The definition exists at `.github/agents/champollion-architect.agent.md`.
- [ ] YAML frontmatter is valid and the agent is discoverable and user-invocable.
- [ ] `read`, `search`, `web`, `execute`, and `edit` remain available for evidence, diagnostics, research, and local design creation.
- [ ] The root `.gitignore` excludes `/Feature/Design/`.
- [ ] The local `Feature/Design` directory exists.
- [ ] A representative run creates all five required deliverables.
- [ ] At least one proposed Mermaid diagram is present.
- [ ] Every material recommendation has alternatives and rationale.
- [ ] Outstanding questions are classified, and a settled design explicitly states that none remain.
- [ ] A blocked design contains the exact abort statement in both its overview and proposed design.
- [ ] A ready design contains no blocking question or blocked-status marker.
- [ ] Changes identify owners, tests, validation, diagram impact, and README impact.
- [ ] Internal links resolve and Markdown diagnostics are clear.
- [ ] `git status --short` does not list generated design artifacts.
- [ ] No tracked file is changed by a normal design run.

## Troubleshooting

### Agent Does Not Appear

Confirm the definition is under `.github/agents`, has the `.agent.md` suffix, valid frontmatter, and `user-invocable: true`. Reload VS Code after customization changes.

### Design Files Appear In Git Status

Run `git check-ignore -v --no-index` for the exact file. Confirm the root `.gitignore` contains `/Feature/Design/` with matching capitalization and that the package is beneath the repository-root `Feature/Design` directory.

### The Design Is Too Abstract

Require each proposed change to name its owning project, expected code surface, behavior, dependencies, and validation. Add focused sequence, class, component, or data-flow diagrams where prose does not make interactions testable.

### The Design Is Over-Engineered

Check each proposed pattern against a concrete requirement and existing repository convention. Remove abstractions that do not reduce meaningful complexity, preserve a boundary, or enable required testing.

### Proposed Behavior Looks Implemented

Strengthen proposed labels, distinguish retained and changed elements, and cite actual current-state source separately. Never copy a proposal into canonical architecture documentation before implementation.

### A Future Copilot Call Proceeds With A Blocked Design

Confirm the exact short-circuit statement appears immediately after the title in both required files and that each unresolved blocker is classified in `outstanding-questions.md`. Strengthen the consuming development workflow if it does not check the marker before its first edit.

## Related Documentation

- [Copilot agents index](README.md)
- [Active Architect definition](../../../.github/agents/champollion-architect.agent.md)
- [Copilot skills index](../skills/README.md)
- [Architecture documentation](../../architecture/README.md)
- [Root README](../../../README.md)
