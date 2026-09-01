# Champollion Execution and Diagnostics Communication Diagram

## Purpose

This diagram shows the collaborating objects for run confirmation, request validation, structured external-process execution, callbacks, aggregation, and conditional diagnostics.

## Notation

```mermaid
flowchart LR
    actor(["Actor"])
    sender["sender : Type"]
    receiver["receiver : Type"]
    decision{"Decision?"}

    actor -->|"1: initiating message"| sender
    sender -->|"1.1: nestedCall()"| receiver
    receiver -.->|"1.2: return value"| sender
    sender -->|"2 *[each item]: repeatedMessage()"| receiver
    receiver --> decision
    decision -->|"[yes]"| sender
    decision -->|"[no]"| actor
```

## Diagram

```mermaid
flowchart TB
    user(["User"])
    window["window : MainWindow"]
    viewModel["viewModel : MainViewModel"]
    store["store : AppSettingsStore"]
    fileSystem["fileSystem : Local File System"]
    runner["runner : ChampollionRunner"]
    rules["rules : CompatibilityRules"]
    validator["validator : LocalPathValidator"]
    builder["builder : ChampollionCommandBuilder"]
    cli["process : Selected Champollion.exe"]
    logWriter["logWriter : DiagnosticLogWriter"]
    confirmationDecision{"Run confirmed?"}
    validationDecision{"Request valid?"}
    diagnosticsDecision{"Failure or stderr?"}

    user -->|"1: select Run Champollion"| window
    window -->|"1.1: inspect configured output directories"| fileSystem
    fileSystem -.->|"1.2: existing and missing directories"| window
    window -.->|"1.3: show run confirmation"| user
    user -->|"1.4: confirmation response"| confirmationDecision
    confirmationDecision -.->|"1.4a [cancelled]: return to workspace"| user
    confirmationDecision -->|"1.4b [confirmed]: approve run"| window

    window -->|"1.4b.1: RunAsync()"| viewModel
    viewModel -->|"1.4b.1.1: SaveAsync(current profile)"| store
    store -->|"1.4b.1.1.1: replace settings.json"| fileSystem
    viewModel -->|"1.4b.1.2: create ChampollionRequest"| viewModel
    viewModel -->|"1.4b.1.3: RunAsync(request,<br/>progress, liveOutput)"| runner

    runner -->|"1.4b.1.3.1: Validate(request)"| rules
    runner -->|"1.4b.1.3.2: validate executable,<br/>input, and outputs"| validator
    validator -->|"1.4b.1.3.2.1: normalize and inspect paths"| fileSystem
    fileSystem -.->|"1.4b.1.3.2.2: path and drive metadata"| validator
    validator -.->|"1.4b.1.3.2.3: validation results"| runner
    runner --> validationDecision
    validationDecision -.->|"1.4b.1.3.5a [invalid]: ArgumentException"| viewModel
    viewModel -.->|"1.4b.1.3.5a.1: correction status"| window
    validationDecision -->|"1.4b.1.3.3, 1.4b.1.3.5b [valid]:<br/>enumerate inputs and create output directories"| fileSystem
    fileSystem -.->|"1.4b.1.3.4: resolved input paths"| runner

    runner -->|"1.4b.1.3.6 *[each input]:<br/>BuildArguments(request, input)"| builder
    builder -.->|"1.4b.1.3.6.1: ordered argument list"| runner
    runner -->|"1.4b.1.3.6.2: start with redirected streams"| cli
    cli -->|"1.4b.1.3.6.3: read PEX and write output"| fileSystem
    cli -.->|"1.4b.1.3.6.4, 1.4b.1.3.6.7:<br/>stream chunks and exit code"| runner
    runner -.->|"1.4b.1.3.6.5, 1.4b.1.3.6.8:<br/>output and progress callbacks"| viewModel
    viewModel -.->|"1.4b.1.3.6.6: append and auto-scroll"| window

    runner --> diagnosticsDecision
    diagnosticsDecision -->|"1.4b.1.3.7 [yes]:<br/>WriteAsync(request, results)"| logWriter
    logWriter -->|"1.4b.1.3.7.1: create timestamped log"| fileSystem
    logWriter -.->|"1.4b.1.3.7.2: log path"| runner
    diagnosticsDecision -->|"[no log required]"| runner
    runner -.->|"1.4b.1.3.8: ExecutionSummary"| viewModel
    viewModel -.->|"1.4b.1.4: final results and clear IsRunning"| window
```

## Collaboration Notes

- `MainWindow` owns the directory-creation confirmation; `ChampollionRunner` remains the authoritative validator before any process starts.
- One process is created for each resolved PEX input, or once for an input-free operation. Structured `ArgumentList` values are returned by `ChampollionCommandBuilder`.
- Stream callbacks may interleave. A file result is finalized only after stdout, stderr, and process exit complete.
- A failed input is retained in the aggregate result without preventing later inputs, and logs are written only for failure or nonempty standard error.