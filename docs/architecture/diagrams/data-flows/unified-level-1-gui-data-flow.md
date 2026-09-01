# Unified Level 1 GUI Data Flow Diagram

## Purpose

This Level 1 DFD decomposes the GUI into its five major data-processing responsibilities and shows their shared stores and external boundaries.

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
flowchart TB
    user["User"]
    cli["Legacy or Current Champollion.exe"]
    windows["Windows Desktop Services"]
    webView["WebView2 Runtime"]
    nexus["Nexus Mods"]

    settingsFlow(["1.0 Manage GUI State and Settings"])
    pathFlow(["2.0 Validate Paths and Discover CLI"])
    executeFlow(["3.0 Validate and Execute Request"])
    outputFlow(["4.0 Present Output and Desktop Actions"])
    helpFlow(["5.0 Serve Help and About Content"])

    legacy[("D0 Legacy LocalAppData")]
    settings[("D1 UserData/settings.json")]
    fixedDrives[("D2 Local Fixed-Drive Metadata")]
    installation[("D3 Champollion Distribution")]
    inputs[("D4 PEX Inputs")]
    outputs[("D5 Generated Papyrus and Assembly")]
    logs[("D6 UserData/Logs")]
    legal[("D7 Packaged Legal Documents")]

    user -->|"Selections and options"| settingsFlow
    legacy -->|"Prior settings and logs"| settingsFlow
    settingsFlow -->|"Migrated settings and current profiles"| settings
    settingsFlow -->|"Migrated diagnostic logs"| logs
    settings -->|"Saved executable paths,<br/>games, and option profiles"| settingsFlow
    settingsFlow -->|"Transient request selections"| pathFlow
    settingsFlow -->|"Configured request data"| executeFlow
    settingsFlow -->|"Restored controls"| user

    user -->|"Entered paths, browse,<br/>search, and cancellation"| pathFlow
    fixedDrives -->|"Drive, directory, and file metadata"| pathFlow
    installation -->|"EXE, companion files,<br/>and version metadata"| pathFlow
    pathFlow -->|"Normalized paths,<br/>validation, and search progress"| user
    pathFlow -->|"Matching executable path"| settingsFlow
    pathFlow -->|"Validated path data"| executeFlow

    user -->|"Run confirmation"| executeFlow
    inputs -->|"Resolved PEX paths"| executeFlow
    installation -->|"Executable path"| executeFlow
    executeFlow -->|"Structured arguments"| cli
    inputs -->|"PEX bytes"| cli
    cli -->|"Generated files"| outputs
    cli -->|"stdout, stderr, and exit code"| executeFlow
    executeFlow -->|"Noteworthy request and result data"| logs
    executeFlow -->|"Progress, output chunks,<br/>summary, and paths"| outputFlow

    outputFlow -->|"Visible output and status"| user
    user -->|"Copy or open command"| outputFlow
    settings -->|"Settings location"| outputFlow
    outputs -->|"Output locations"| outputFlow
    logs -->|"Log location"| outputFlow
    outputFlow -->|"Clipboard text or shell path"| windows

    user -->|"Help navigation or legal command"| helpFlow
    legal -->|"Document path"| helpFlow
    helpFlow -->|"Document shell path"| windows
    helpFlow -->|"Download URI"| webView
    webView -->|"HTTPS request"| nexus
    nexus -->|"Page content"| webView
    webView -->|"Rendered content"| helpFlow
    helpFlow -->|"Help content or status"| user
```

## Level 1 Scope

- Processes `1.0` through `5.0` are logical responsibilities inside the single GUI executable, not separately deployed services.
- `D0` is migration input only. Current settings and logs live beside the application under `UserData`.
- `D2` represents metadata read during validation and traversal; `D3` represents the selected third-party executable distribution.
- `D4` and `D5` remain separate because PEX inputs are read while Papyrus source and optional assembly are generated.
- Transient paths, status, progress, output text, and the latest log path move between GUI processes in memory and are not persistent stores.