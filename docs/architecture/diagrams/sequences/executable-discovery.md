# Executable Discovery Sequence Diagram

## Purpose

This diagram shows the cancellable automatic search for a Legacy or Current `Champollion.exe`, including progress updates and alternate completion paths.

```mermaid
sequenceDiagram
    actor User
    participant Window as MainWindow
    participant VM as MainViewModel
    participant Search as ExecutableSearchService
    participant FS as Local Fixed Drives
    participant Validator as LocalPathValidator
    participant Classifier as ChampollionExecutableClassifier
    participant Store as AppSettingsStore

    User->>Window: Select Auto search
    Window->>VM: SearchCommand.Execute()
    activate VM
    VM->>VM: Preserve path and create CancellationTokenSource
    VM->>Search: FindAsync(selectedEdition, progress, token)
    activate Search
    Search->>FS: Collect application, profile, known-folder, and drive roots
    loop Bounded workers process queued directories
        Search->>VM: Report SearchProgress
        VM-->>Window: Update worker and directory status
        Search->>FS: Check directory/Champollion.exe
        Search->>Validator: ValidateExecutable(candidate)
        Validator->>FS: Check fixed drive and file metadata
        Validator-->>Search: PathValidationResult
        opt Candidate path is valid
            Search->>Classifier: Classify(candidate)
            Classifier->>FS: Check Legacy markers and version metadata
            Classifier-->>Search: Legacy, Current, or Unknown
        end
    end
    alt Matching edition found
        Search->>Search: Publish result and cancel remaining workers
        Search-->>VM: Matching executable path
        VM->>Store: SaveAsync(updated settings)
        Store->>FS: Atomically replace UserData/settings.json
        VM-->>Window: Display found-and-saved status
    else Search exhausted
        Search-->>VM: null
        VM->>VM: Restore previous path
        VM-->>Window: Display no-result status
    else User selects Cancel search
        User->>Window: Select Cancel search
        Window->>VM: CancelSearchCommand.Execute()
        VM->>VM: Cancel token
        Search->>Search: Stop and await every worker
        Search-->>VM: OperationCanceledException
        VM->>VM: Restore previous path
        VM-->>Window: Display cancelled status
    end
    deactivate Search
    VM->>VM: Dispose cancellation source and clear IsSearching
    deactivate VM
```

## Notes

- Search completion, exhaustion, and caller cancellation all stop and await the worker swarm before `FindAsync` finishes.
- A successful path is persisted for the selected edition; unsuccessful or cancelled search preserves the prior configuration.
- The first concurrently validated and edition-matching candidate wins, so multiple valid installations do not have a deterministic path order.