# Champollion .NET Platform Evaluator Guide

## Purpose

`Champollion .NET Platform Evaluator` is a repository-specific GitHub Copilot custom agent for assessing .NET upgrades, current Windows x64 compatibility, and the feasibility of proposed platform or architecture targets.

It evaluates target framework lifecycle, Avalonia, WebView2, external Champollion execution, clean-architecture replacement boundaries, tests, publishing, Windows packaging, and CI.

The agent definition is:

```text
.github/agents/champollion-dotnet-platform-evaluator.agent.md
```

Every completed evaluation creates a new immutable report under:

```text
docs/reports/champollion-dotnet-platform-evaluation-<UTC_TIMESTAMP>.md
```

## Current Product Baseline

Windows 10/11 x64 is the current supported product target. Linux, macOS, and ARM64 are proposed targets only when explicitly requested or introduced by repository changes.

This differs from repositories where Windows and Linux are already first-class targets. The evaluator must not infer product support from Avalonia or portable Domain/Application projects alone. It separately assesses:

- desktop UI and embedded browser support;
- external Champollion executable availability;
- path, drive, shell, and process assumptions;
- publish and package formats;
- clean-target execution evidence.

## When To Use It

Run the evaluator when:

- planning or verifying a .NET upgrade;
- checking whether the current .NET release is still supported;
- upgrading Avalonia, WebView, or another platform-sensitive package;
- changing Windows minimum versions or x64 support;
- proposing Linux, macOS, or ARM64 support;
- introducing native or platform-specific dependencies;
- changing runtime identifiers, publish settings, installer behavior, or CI;
- preparing a major release or assessing platform coupling.

A useful cadence is before and after major .NET/UI migrations and before major releases.

## How To Run The Agent

1. Open GitHub Copilot Chat.
2. Select `Champollion .NET Platform Evaluator` from the agent selector.
3. Enter the target .NET release and platform matrix when known.
4. Allow read-only inspection, non-destructive diagnostics, web research, and report creation.
5. Open the report linked in the final response.

Examples:

```text
Evaluate upgrading this solution to .NET 11 while preserving Windows 10/11 x64 support.
```

```text
Evaluate whether Linux x64 support is feasible. Identify replacements needed for WebView2, external Champollion execution, path validation, shell integration, and packaging.
```

```text
Audit the current .NET, Avalonia, and WebView dependencies for Windows x64 release readiness.
```

```text
Evaluate adding Windows ARM64 without changing the currently supported x64 release.
```

## What The Agent Does

For each run, the evaluator:

1. Identifies the .NET target and requested platform matrix.
2. Generates a collision-resistant UTC report timestamp.
3. Inspects projects, resolved packages, source boundaries, UI/platform integrations, tests, publishing, packaging, docs, and CI.
4. Records available SDKs and workloads.
5. Runs focused non-destructive build, test, and publish diagnostics.
6. Separates compile, publish, package, and clean-target execution claims.
7. Verifies support claims against authoritative current sources.
8. Classifies findings by severity and confidence.
9. Assigns `Retain`, `Upgrade in place`, `Isolate`, or `Replace` decisions.
10. Writes and links the mandatory report.

The evaluator does not implement migrations or edit product code. Its expected repository write is the new report only.

## Repository Skill Integration

The evaluator understands the repository skills as follow-up workflow authorities:

| Skill | Evaluator use |
| --- | --- |
| [`champollion-development`](../skills/champollion-development.md) | Structure recommended implementation and validation work. |
| [`champollion-diagrams`](../skills/champollion-diagrams.md) | Identify architecture views that implementation must assess after platform, dependency, runtime, packaging, or deployment changes. |
| [`champollion-readme`](../skills/champollion-readme.md) | Identify root README claims that implementation must assess after support, prerequisite, build, configuration, packaging, or release changes. |
| [`champollion-code-review`](../skills/champollion-code-review-maintenance.md) | Recommend independent post-implementation defect and documentation consistency review when warranted. |

Skills describe procedures, not platform facts. The evaluator must still verify conclusions against current source, project files, workflows, packaging, diagnostics, and authoritative external documentation.

This integration affects report recommendations only. The evaluator does not invoke development or review workflows and must not modify product code, tests, diagrams, the root README, skills, or agent guides. Its only repository write remains the new immutable evaluation report.

## Reports

### Filename And Immutability

Reports use UTC timestamps with milliseconds:

```text
champollion-dotnet-platform-evaluation-YYYYMMDDTHHmmssfffZ.md
```

Never edit an old report to represent a later repository or support-policy state. Run the evaluator again and create a new report.

### Required Distinctions

Reports must distinguish:

- current Windows x64 support from proposed targets;
- framework documentation from verified application execution;
- compile, publish, package, install, and clean-machine launch results;
- Avalonia portability from WebView2 and other platform dependencies;
- GUI availability from external Champollion executable availability;
- confirmed facts, conclusions, assumptions, and limitations.

### Status And Decisions

Platform cells use `Supported`, `Unsupported`, `Conditional`, or `Not verified`.

Components receive one decision:

- `Retain`
- `Upgrade in place`
- `Isolate`
- `Replace`

At minimum, assess Domain, Application, UI, embedded browser, external process integration, tests, packaging, and CI.

## Platform-Specific Evaluation Areas

### Avalonia And WebView2

Avalonia alone does not prove the application is portable. The embedded Help browser uses `Avalonia.Controls.WebView` and Microsoft Edge WebView2 on Windows. Any proposed non-Windows target needs an officially supported browser path or an explicit replacement decision.

### External Champollion

The product invokes an external `Champollion.exe`. For each proposed target, verify that a compatible command-line executable exists and that its arguments, output, exit codes, and filesystem expectations are supported. Without that evidence, delivery may be blocked even when the GUI compiles.

### Windows Coupling

Current code intentionally assumes Windows in several areas, including `.exe` selection, fixed-drive search, protected Windows paths, `explorer.exe`, WebView2, and Inno Setup. These are baseline facts for Windows, not defects. A proposed target must decide whether to isolate or replace each coupling.

### Clean Architecture

Domain has no project dependencies, and Application depends only on Domain. Use these boundaries to identify the smallest replacement surface. A platform proposal may retain Domain and substantial Application logic while replacing or abstracting UI/platform services, but the report must verify actual API use rather than assume portability.

## Maintaining The Agent

Update the agent and this guide when any of these change:

- current supported operating systems or architectures;
- .NET target, SDK policy, Avalonia, or WebView technology;
- external Champollion executable support or invocation contract;
- project boundaries or platform abstractions;
- native dependencies and Windows-only API usage;
- runtime identifiers, publish profiles, installer/archive formats;
- CI target matrices and release artifacts;
- development, diagram, root README, or review workflow responsibilities referenced by evaluation reports;
- report filename, required sections, decisions, or severity definitions.

When changing scope, update the frontmatter description, baseline, scope, workflow, evaluation rules, report template, completion criteria, and this guide together.

## Frontmatter And Tools

Preserve these discovery and execution capabilities unless the agent contract changes:

| Field | Purpose |
| --- | --- |
| `name` | Display name in the agent selector. |
| `description` | Automatic discovery terms for upgrades and platform analysis. |
| `tools: [read, search, web, execute, edit]` | Repository inspection, current support research, diagnostics, and mandatory report creation. |
| `user-invocable: true` | Shows the agent in the selector. |
| `disable-model-invocation: false` | Allows delegation by another agent. |
| `argument-hint` | Shows the expected evaluation request. |

Do not remove `edit` while report creation remains mandatory. The body restricts writes even though the alias is broader.

## Validation After Agent Changes

1. Confirm the file path and `.agent.md` suffix.
2. Confirm valid YAML delimiters and a clear quoted description.
3. Confirm required tools and invocation settings remain present.
4. Confirm Windows x64 remains the baseline unless product support changed.
5. Confirm every requested non-Windows target is treated separately.
6. Run a focused evaluation request.
7. Confirm a unique report appears under `docs/reports`.
8. Confirm all required report headings are present.
9. Confirm UI/WebView and external Champollion receive explicit decisions.
10. Confirm commands distinguish passed, failed, and not run.
11. Confirm authoritative sources include titles, URLs, and access dates.
12. Confirm the recommended implementation sequence includes applicable development, diagram, README, and review gates without performing those workflows.
13. Check `git status --short`; the report should be the only unexpected write.

A useful validation request is:

```text
Evaluate the current .NET and Avalonia stack for Windows x64. Keep diagnostics focused but produce the complete required report.
```

## Troubleshooting

### Agent Does Not Appear

Confirm the file is under `.github/agents`, has the `.agent.md` suffix, valid frontmatter, `user-invocable: true`, and no diagnostics. Reload VS Code after changes.

### No Report Is Created

Confirm `edit` remains available and `docs/reports` is writable. A chat-only answer is incomplete; run the evaluation again rather than converting an incomplete response into an official report.

### Existing Report Is Overwritten

Restore the original report and strengthen timestamp collision handling. Reports are immutable engineering records.

### Linux Or macOS Is Marked Supported From A Build

The report must assess native UI/browser behavior, external Champollion availability, filesystem/shell assumptions, packaging, and clean-target execution. Without those checks, use `Conditional` or `Not verified`.

### Product Code Is Changed

Stop and inspect the working tree. The evaluator is not a migration agent. Revert only writes from that evaluation and preserve unrelated work.

## Review And Retention

Retain reports that informed shipped releases, platform commitments, or major architecture decisions. Review cited policies against their access date and require target-machine validation before public support claims. Remove only accidental, empty, or clearly incomplete reports that were never used for a decision.
