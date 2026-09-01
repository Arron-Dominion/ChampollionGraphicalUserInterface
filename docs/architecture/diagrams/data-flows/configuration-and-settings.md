# Configuration and Settings Data Flow Diagram

## Purpose

This DFD shows how GUI selections become saved edition and game profiles, how settings return at startup, and which paths remain transient.

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
    configure(["1.0 Manage GUI State and Settings"])
    rules(["1.1 Apply Compatibility and Profile Rules"])
    legacy[("D0 Legacy LocalAppData<br/>settings.json and Logs")]
    settings[("D1 Application UserData<br/>settings.json")]
    logs[("D6 Application UserData/Logs")]
    memory[("M1 Transient GUI State<br/>input and output paths, status,<br/>progress, and live output")]

    user -->|"Edition, game, operation,<br/>executable path, and options"| configure
    configure -->|"Current selections"| rules
    rules -->|"Supported games, enabled options,<br/>profile key, and cleared incompatible values"| configure

    legacy -->|"Legacy settings and logs"| configure
    configure -->|"Migrated settings"| settings
    configure -->|"Migrated diagnostic logs"| logs
    configure -->|"Executable paths, remembered games,<br/>and edition-game option profiles"| settings
    settings -->|"AppSettings JSON"| configure

    configure -->|"Current in-memory selections and messages"| memory
    memory -->|"Bound values"| configure
    configure -->|"Visible controls and restored selections"| user
```

## Persisted Data

- `LegacyExecutablePath` and `CurrentExecutablePath`.
- `LastLegacyGame` and `LastCurrentGame`.
- `EditionGameOptions`, keyed by explicit edition and game combinations.
- Saved options include assembly, comments, recursion, recreated subdirectories, header, trace, tree suppression, debug functions, debug-line suppression, and verbose output.

## Transient Data

Input path, source-output path, assembly-output path, current status, search state, run progress, live process output, and the latest log path remain in memory and are not serialized into `settings.json`.