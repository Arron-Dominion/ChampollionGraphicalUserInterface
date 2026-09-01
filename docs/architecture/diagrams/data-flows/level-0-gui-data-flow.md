# Level 0 GUI Data Flow Diagram

## Purpose

This context-level DFD treats Champollion Graphical User Interface as one process and shows the data crossing its boundary.

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
    gui(["0.0 Champollion Graphical User Interface"])
    cli["Legacy or Current Champollion.exe"]
    windows["Windows Desktop Services"]
    webView["WebView2 Runtime"]
    nexus["Nexus Mods"]
    localData[("Local Fixed-Drive Data")]

    user -->|"Edition, game, operation, options,<br/>paths, commands, and confirmations"| gui
    gui -->|"Validation, status, progress,<br/>process output, summaries, and help"| user

    gui -->|"Structured arguments and working directory"| cli
    cli -->|"Standard output, standard error,<br/>and exit code"| gui

    gui -->|"Settings, logs, output-directory creation,<br/>and file metadata queries"| localData
    localData -->|"Saved profiles, paths, PEX metadata,<br/>executables, logs, and legal documents"| gui
    localData -->|"PEX input"| cli
    cli -->|"Generated Papyrus source and assembly"| localData

    gui -->|"Picker requests, clipboard text,<br/>and shell paths"| windows
    windows -->|"Selected paths and platform results"| gui
    gui -->|"Selected download-page URI"| webView
    webView -->|"HTTPS request"| nexus
    nexus -->|"Rendered web content"| webView
    webView -->|"Browser state and page content"| gui
```

## Boundary Rules

- Input and output paths remain transient GUI data and are not written to settings.
- The external CLI reads PEX files and writes generated output directly on the local file system.
- The GUI captures CLI streams and the exit code, then persists diagnostic data only for failures or noteworthy standard error.
- Web content is isolated to the embedded Help browser and does not enter the local execution request.