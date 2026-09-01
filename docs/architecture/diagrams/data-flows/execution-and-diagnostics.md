# Execution and Diagnostics Data Flow Diagram

## Purpose

This DFD shows how transient GUI selections become a validated request, how each PEX input crosses the external CLI boundary, and how output, progress, summaries, generated files, and diagnostics return.

## Legend

```mermaid
flowchart LR
    external["External<br/>Outside the GUI"]
    process(["Process<br/>Transforms data"])
    store[("Data Store<br/>Holds data")]

    external -->|"Data Flow<br/>Transfers data"| process
    process -->|"Data Flow<br/>Transfers data"| store
```

## Diagram

```mermaid
flowchart LR
    user["User"]
    execute(["3.0 Validate and Execute Request"])
    build(["3.1 Build Structured Arguments"])
    present(["4.0 Present Progress and Results"])
    cli["Legacy or Current Champollion.exe"]

    settings[("D1 Application UserData<br/>settings.json")]
    inputs[("D4 PEX Inputs")]
    installation[("D3 Champollion Distribution")]
    outputs[("D5 Generated Papyrus and Assembly")]
    logs[("D6 UserData/Logs")]

    user -->|"Confirmed edition, game, operation,<br/>paths, and decompilation options"| execute
    execute -->|"Executable path and active profile"| settings
    inputs -->|"PEX paths and directory enumeration"| execute
    installation -->|"Executable path and working directory"| execute

    execute -->|"Normalized ChampollionRequest<br/>and resolved input path"| build
    build -->|"Ordered ArgumentList values"| execute
    execute -->|"Arguments and working directory"| cli
    inputs -->|"PEX file bytes"| cli
    cli -->|"Generated source and optional assembly"| outputs
    cli -->|"stdout chunks, stderr chunks, and exit code"| execute

    execute -->|"ExecutionOutput and ExecutionProgress"| present
    execute -->|"Request details and noteworthy results"| logs
    execute -->|"ExecutionSummary and log path"| present
    present -->|"Live output, progress, per-input status,<br/>success and failure counts, and errors"| user
```

## Data Contracts

- `ChampollionRequest` carries edition, game, operation, executable path, optional input path, and `DecompilationOptions`.
- `ExecutionOutput` carries input path, stream classification, and a text chunk.
- `ExecutionProgress` carries completed count, total count, and current input.
- `FileExecutionResult` retains input path, exit code, complete standard output, complete standard error, and success.
- `ExecutionSummary` returns all file results, optional log path, and successful and failed counts.
- Diagnostic logs persist edition, game, operation, each input, exit code, and complete process streams.