# Evaluation Reports

This directory stores immutable, timestamped engineering reports produced by repository custom agents.

## .NET Platform Evaluations

`Champollion .NET Platform Evaluator` writes reports named:

```text
champollion-dotnet-platform-evaluation-YYYYMMDDTHHmmssfffZ.md
```

Each report captures the repository state, local diagnostic environment, external support policies, findings, component decisions, and limitations at a point in time.

Do not edit an existing report to describe a later state. Run the evaluator again and create a new timestamped report. Retain reports that informed a shipped release, support commitment, framework migration, or major architecture decision.

See the [evaluator guide](../tools/agents/champollion-dotnet-platform-evaluator.md) for usage and maintenance instructions.
