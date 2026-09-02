# Output and Desktop Actions Data Flow Diagram

## Purpose

This DFD details process `4.0 Present Output and Desktop Actions`, including output presentation, copy-to-clipboard, and opening generated output, logs, and settings through Windows desktop services.

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
    user["E1 User"]
    input["P3 ExecutionOutput,<br/>ExecutionProgress, and summary"]
    command["Copy output or open<br/>output, log, or settings command"]
    output(["4.0 Present Output<br/>and Desktop Actions"])
    settings[("D1 UserData/settings.json")]
    outputs[("D5 Generated Papyrus<br/>and Assembly")]
    logs[("D6 UserData/Logs")]
    clipboard["Windows Clipboard"]
    explorer["Windows File Explorer<br/>and Shell"]

    input -->|"progress, output chunks,<br/>summary, and paths"| output
    user --> command
    command --> output
    settings -->|"settings location"| output
    outputs -->|"output locations"| output
    logs -->|"log location"| output
    output -->|"visible output and status"| user
    output -->|"complete output text"| clipboard
    output -->|"directory or selected-file path"| explorer
    explorer -.->|"shell result"| output
```

## Data Boundaries

- Process `4.0` receives transient execution progress, output chunks, summaries, and paths from process `3.0`.
- The clipboard receives complete in-memory output text; the GUI does not read clipboard contents back.
- File Explorer and the Windows shell receive local paths only and do not return file contents to the GUI.
- Settings, generated output, and diagnostic logs provide locations for desktop actions.
