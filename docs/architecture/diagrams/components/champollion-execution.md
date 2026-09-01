# Champollion Execution and Diagnostics Component Diagram

## Purpose

This diagram follows a confirmed run from GUI selections through request validation and structured command construction to external process output, progress, summaries, and diagnostic logging.

```mermaid
flowchart LR
    user["User"]

    subgraph ui["GUI project"]
        view["MainWindow<br/>Run handler and confirmation dialog"]
        viewModel["MainViewModel<br/>Request creation, progress,<br/>live output, and summary state"]
    end

    subgraph domain["Domain project"]
        request["ChampollionRequest"]
        options["DecompilationOptions"]
    end

    subgraph application["Application project"]
        runner["ChampollionRunner<br/>Validation and process orchestration"]
        compatibility["CompatibilityRules<br/>Request compatibility"]
        validator["LocalPathValidator<br/>Input and output path rules"]
        commandBuilder["ChampollionCommandBuilder<br/>Ordered argument list"]
        progress["ExecutionProgress and ExecutionOutput<br/>Directional result DTOs"]
        summary["ExecutionSummary and FileExecutionResult"]
        logWriter["DiagnosticLogWriter<br/>Failure and stderr logs"]
        settings["AppSettingsStore<br/>Pre-run configuration save"]
    end

    cli["Selected Champollion.exe<br/>External CLI process"]
    storage[("Local PEX inputs, output folders,<br/>settings, and UserData/Logs")]

    user -->|"Selects Run and confirms"| view
    view -->|"Calls RunAsync"| viewModel
    viewModel -->|"Persists current profile"| settings
    settings -->|"Writes"| storage
    viewModel -->|"Builds"| request
    options -->|"Carries selected parameters"| request
    viewModel -->|"Submits request with progress receivers"| runner
    runner --> compatibility
    runner --> validator
    validator <-->|"Validates and resolves paths"| storage
    runner -->|"Builds arguments for each input"| commandBuilder
    commandBuilder -->|"Ordered ArgumentList values"| runner
    runner -->|"Starts process per resolved input"| cli
    cli <-->|"Reads PEX and writes generated files"| storage
    cli -->|"stdout, stderr, and exit code"| runner
    runner -->|"Streams updates"| progress
    progress -->|"Updates status and live output"| viewModel
    runner -->|"Writes noteworthy results"| logWriter
    logWriter -->|"Creates timestamped log"| storage
    runner -->|"Returns aggregate result"| summary
    summary -->|"Updates final status and log path"| viewModel
    viewModel -->|"Displays results"| view
```

## Key Relationships

- `MainWindow` identifies missing output directories and requires confirmation before calling `MainViewModel.RunAsync`.
- `MainViewModel` persists the active profile, creates the domain request, and translates progress DTOs into bindable status and output text.
- `ChampollionRunner` resolves inputs, validates outputs, creates approved directories, executes every resolved input independently, and drains both redirected streams.
- `DiagnosticLogWriter` writes a log when a process fails or emits noteworthy standard error; one failed input does not prevent remaining inputs from running.