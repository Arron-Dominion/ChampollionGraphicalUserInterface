# Configuration and Settings Component Diagram

## Purpose

This diagram shows application startup, saved configuration loading, edition and game profile management, and settings persistence.

```mermaid
flowchart LR
    user["User"]

    subgraph ui["GUI project"]
        app["App<br/>Composition root"]
        view["MainWindow<br/>Bound configuration controls"]
        viewModel["MainViewModel<br/>Configuration state and orchestration"]
    end

    subgraph application["Application project"]
        settingsStore["AppSettingsStore<br/>Load, save, migrate, and profile access"]
        compatibility["CompatibilityRules<br/>Edition, game, operation, and option rules"]
        outputPaths["ApplicationOutputPaths<br/>Application-owned output defaults"]
        settingsContracts["AppSettings and SavedOptions<br/>Persistence DTOs"]
    end

    domain["Domain models and enums<br/>Edition, game, operation,<br/>and decompilation options"]
    currentStorage[("Application/UserData/settings.json")]
    legacyStorage[("Legacy LocalAppData settings and logs")]

    app -->|"Constructs and initializes"| viewModel
    app -->|"Assigns DataContext"| view
    user <-->|"Selects edition, game,<br/>operation, and options"| view
    view <-->|"Compiled bindings"| viewModel

    viewModel -->|"Loads and saves"| settingsStore
    settingsStore <-->|"Serializes"| settingsContracts
    settingsStore <-->|"Reads and atomically replaces"| currentStorage
    settingsStore -->|"Copies then removes migrated data"| legacyStorage
    viewModel -->|"Gets supported combinations"| compatibility
    viewModel -->|"Resets transient output paths"| outputPaths
    viewModel -->|"Creates and applies selections"| domain
```

## Key Relationships

- `App` constructs one `MainViewModel` and starts `InitializeAsync` after assigning it to `MainWindow`.
- `MainViewModel` keeps executable paths separate by edition and option profiles separate by edition and game.
- `AppSettingsStore` owns JSON persistence and migration. Input and output paths remain transient and are not included in saved settings.
- `CompatibilityRules` controls available games and options; `ApplicationOutputPaths` restores the two application-adjacent output defaults.