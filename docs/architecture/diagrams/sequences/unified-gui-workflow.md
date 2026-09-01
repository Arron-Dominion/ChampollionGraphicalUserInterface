# Unified GUI Workflow Sequence Diagram

## Purpose

This diagram combines the primary happy path from application launch through configuration, confirmed decompilation, live output, and opening generated files.

```mermaid
sequenceDiagram
    actor User
    participant App
    participant Window as MainWindow
    participant VM as MainViewModel
    participant Store as AppSettingsStore
    participant Validator as LocalPathValidator
    participant Runner as ChampollionRunner
    participant Builder as ChampollionCommandBuilder
    participant CLI as Champollion.exe
    participant FS as Local File System
    participant Explorer as File Explorer

    User->>App: Launch application
    App->>VM: Construct with Application services
    App->>Window: Assign VM as DataContext
    App->>VM: InitializeAsync()
    VM->>Store: LoadAsync()
    Store->>FS: Migrate and read UserData/settings.json
    Store-->>VM: Saved configuration
    VM-->>Window: Restore edition, game, executable, and options

    User->>Window: Select PEX input and output folders
    Window->>VM: Update bound paths
    VM->>Validator: Validate changed executable and input paths
    Validator->>FS: Check local fixed-drive paths
    Validator-->>VM: Validation results
    VM-->>Window: Display path status

    User->>Window: Select Run Champollion
    Window->>FS: Identify output directories to create
    Window-->>User: Request confirmation
    User-->>Window: Confirm
    Window->>VM: RunAsync()
    VM->>Store: SaveAsync(current profile)
    Store->>FS: Replace settings.json
    VM->>Runner: RunAsync(request, progress, output)
    Runner->>Validator: Validate request paths
    Validator->>FS: Check local path and drive metadata
    FS-->>Validator: Validation metadata
    Runner->>FS: Enumerate resolved PEX inputs
    Runner->>FS: Create approved output directories

    loop Each resolved PEX input
        Runner->>Builder: BuildArguments(request, input)
        Builder-->>Runner: Ordered argument list
        Runner->>CLI: Start redirected process
        CLI->>FS: Read PEX and write Papyrus or assembly
        CLI-->>Runner: stdout, stderr, and exit code
        Runner-->>VM: Live output and progress
        VM-->>Window: Update output, progress, and status
    end

    Runner-->>VM: ExecutionSummary
    VM-->>Window: Display final per-input results
    User->>Window: Open generated output
    Window->>VM: ResolveOutputDirectory()
    VM-->>Window: Existing output path
    Window->>Explorer: Open directory
    Explorer-->>User: Display generated files
```

## Notes

- This unified view follows the successful decompilation path. Focused sequence diagrams document cancellation, invalid paths, search exhaustion, process failures, diagnostic logging, and browser-only interactions.
- The GUI remains responsive because settings, search, process stream capture, and execution completion use asynchronous workflows.