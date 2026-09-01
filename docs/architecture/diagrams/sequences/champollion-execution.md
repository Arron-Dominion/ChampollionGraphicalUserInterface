# Champollion Execution and Diagnostics Sequence Diagram

## Purpose

This diagram follows a confirmed operation through settings persistence, request validation, per-input process execution, live stream capture, progress reporting, and optional diagnostic logging.

```mermaid
sequenceDiagram
    actor User
    participant Window as MainWindow
    participant VM as MainViewModel
    participant Store as AppSettingsStore
    participant Runner as ChampollionRunner
    participant Rules as CompatibilityRules
    participant Validator as LocalPathValidator
    participant Builder as ChampollionCommandBuilder
    participant FS as Local File System
    participant CLI as Selected Champollion.exe
    participant Log as DiagnosticLogWriter

    User->>Window: Select Run Champollion
    Window->>FS: Check configured output directories
    FS-->>Window: Existing and missing directories
    Window-->>User: Show confirmation and directories to create
    alt User cancels
        User-->>Window: Cancel
        Window-->>User: Return to workspace
    else User confirms
        Window->>VM: RunAsync()
        activate VM
        VM->>Store: SaveAsync(current profile)
        Store->>FS: Atomically replace settings.json
        VM->>VM: CreateRequest()
        VM->>Runner: RunAsync(request, progress, liveOutput)
        activate Runner
        Runner->>Rules: Validate(request)
        Runner->>Validator: Validate executable, input, and outputs
        Validator->>FS: Normalize paths and check drive, existence, and type
        FS-->>Validator: Path and drive metadata
        Runner->>FS: Enumerate matching PEX inputs
        FS-->>Runner: Resolved input paths
        alt Validation fails
            Runner-->>VM: ArgumentException
            VM-->>Window: Display correction status and errors
        else Request is valid
            Runner->>FS: Create approved output directories
            loop Each resolved PEX input or input-free operation
                Runner->>Builder: BuildArguments(request, input)
                Builder-->>Runner: Ordered argument list
                Runner->>CLI: Start process with redirected streams
                par Standard output
                    CLI-->>Runner: stdout chunks
                    Runner-->>VM: ExecutionOutput
                    VM-->>Window: Append and auto-scroll live output
                and Standard error
                    CLI-->>Runner: stderr chunks
                    Runner-->>VM: ExecutionOutput marked as error
                    VM-->>Window: Append prefixed stderr
                end
                CLI->>FS: Read PEX and write requested output
                CLI-->>Runner: Exit code
                Runner-->>VM: ExecutionProgress
                VM-->>Window: Update completed count and status
            end
            opt Failure or noteworthy stderr exists
                Runner->>Log: WriteAsync(request, results)
                Log->>FS: Create timestamped diagnostic log
                Log-->>Runner: Log path
            end
            Runner-->>VM: ExecutionSummary
            VM-->>Window: Append per-input summary and final status
        end
        deactivate Runner
        VM->>VM: Clear IsRunning
        deactivate VM
    end
```

## Notes

- The process working directory is the selected executable directory, and every argument is added through `ProcessStartInfo.ArgumentList`.
- Standard output, standard error, and process exit are all awaited before a file result is finalized.
- A failed input is recorded and summarized without preventing subsequent resolved inputs from being attempted.