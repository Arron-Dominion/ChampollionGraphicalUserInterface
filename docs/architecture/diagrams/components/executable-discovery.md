# Executable Discovery Component Diagram

## Purpose

This diagram shows the cancellable GUI workflow that searches local fixed drives and accepts only a `Champollion.exe` matching the selected Legacy or Current edition.

```mermaid
flowchart LR
    user["User"]

    subgraph ui["GUI project"]
        view["MainWindow<br/>Auto search and cancel controls"]
        viewModel["MainViewModel<br/>Search state, cancellation, and status"]
    end

    subgraph application["Application project"]
        search["ExecutableSearchService<br/>Bounded concurrent traversal"]
        validator["LocalPathValidator<br/>Candidate path validation"]
        classifier["ChampollionExecutableClassifier<br/>Legacy, Current, or Unknown"]
        progress["SearchProgress<br/>Worker activity DTO"]
        settings["AppSettingsStore<br/>Persists accepted executable path"]
    end

    storage[("Ready local fixed drives<br/>Application, profile, Desktop,<br/>Documents, Downloads, and drive roots")]

    user -->|"Starts or cancels search"| view
    view -->|"Invokes generated commands"| viewModel
    viewModel -->|"Selected edition and cancellation token"| search
    search <-->|"Enumerates bounded directory trees"| storage
    search -->|"Validates each candidate"| validator
    validator <-->|"Checks file and fixed drive"| storage
    search -->|"Classifies valid candidate"| classifier
    classifier <-->|"Checks companion files and version metadata"| storage
    search -->|"Reports worker snapshots"| progress
    progress -->|"Updates status"| viewModel
    search -->|"Matching path, no result, or cancellation"| viewModel
    viewModel -->|"Saves successful path"| settings
    viewModel -->|"Displays result and restores prior path when needed"| view
```

## Key Relationships

- `MainViewModel` owns search cancellation and preserves the previous executable path when search is cancelled or exhausted.
- `ExecutableSearchService` coordinates the bounded worker swarm and excludes unsupported or intentionally skipped directory trees.
- `LocalPathValidator` establishes whether a candidate is an eligible local executable before classification.
- `ChampollionExecutableClassifier` requires the complete Legacy companion layout; standalone valid executables classify as Current, while partial Legacy layouts remain Unknown.