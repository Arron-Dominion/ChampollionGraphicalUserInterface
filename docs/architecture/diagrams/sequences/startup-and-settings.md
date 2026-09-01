# Startup and Settings Sequence Diagram

## Purpose

This diagram shows desktop startup, service composition, legacy-data migration, settings loading, and initial view-model state restoration.

```mermaid
sequenceDiagram
    actor User
    participant Avalonia as Avalonia Desktop Lifetime
    participant App
    participant VM as MainViewModel
    participant Store as AppSettingsStore
    participant Legacy as Legacy LocalAppData
    participant Data as Application UserData
    participant Window as MainWindow

    User->>Avalonia: Launch application
    Avalonia->>App: OnFrameworkInitializationCompleted()
    App->>App: Construct validator, runner, search service, and settings store
    App->>VM: new MainViewModel(...)
    VM->>VM: ResetTransientPaths()
    VM->>VM: RefreshGames()
    App->>Window: new MainWindow(DataContext = VM)
    App->>VM: InitializeAsync()
    activate VM
    VM->>Store: LoadAsync()
    activate Store
    Store->>Legacy: Check settings.json and Logs
    opt Legacy data exists
        Store->>Data: Copy missing settings and log files
        Store->>Legacy: Delete each successfully migrated source
    end
    Store->>Data: Create UserData directory
    alt No settings file
        Store-->>VM: Default AppSettings
    else Valid settings file
        Store->>Data: Read settings.json
        Data-->>Store: JSON content
        Store-->>VM: Deserialized AppSettings
    else Malformed settings file
        Store->>Data: Rename as settings.corrupt-*.json
        Store-->>VM: Default AppSettings
    end
    deactivate Store
    VM->>VM: Restore game, executable path, and saved options
    VM-->>Window: Property change notifications update bindings
    deactivate VM
    Window-->>User: Display initialized workspace
```

## Notes

- `App` starts `InitializeAsync` without blocking framework initialization; settings I/O continues asynchronously after the window is assigned.
- Input and output paths are initialized from application defaults, not loaded from settings.
- Temporary read failures return default settings without treating the file as malformed.