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
flowchart TB
    gui(["0.0 Champollion Graphical User Interface"])
    subgraph userFlow["User boundary"]
        direction TB
        user["User"]
        userInput["Edition, game, operation, options,<br/>paths, commands, and confirmations"]
        userOutput["Validation, status, progress,<br/>process output, summaries, and help"]
        user -->|"IN"| userInput
        userOutput -->|"OUT"| user
    end

    subgraph cliFlow["CLI execution boundary"]
        direction TB
        cli["Legacy or Current Champollion.exe"]
        cliInput["Structured arguments<br/>and working directory"]
        cliOutput["Standard output, standard error,<br/>and exit code"]
        cliInput -->|"ARG"| cli
        cli -->|"IO"| cliOutput
    end

    subgraph localDataFlow["Local data boundary"]
        direction TB
        localData[("Local Fixed-Drive Data")]
        localDataInput["Settings, logs, output-directory creation,<br/>and file metadata queries"]
        localDataOutput["Saved profiles, paths, PEX metadata,<br/>executables, logs, and legal documents"]
        pexInput["PEX input"]
        generatedOutput["Generated Papyrus source and assembly"]
        localDataInput -->|"FS"| localData
        localData -->|"DATA"| localDataOutput
        localData -->|"PEX"| pexInput
        cli -->|"GEN"| generatedOutput
    end

    subgraph windowsFlow["Windows desktop boundary"]
        direction TB
        windows["Windows Desktop Services"]
        windowsInput["Picker requests, clipboard text,<br/>and shell paths"]
        windowsOutput["Selected paths and platform results"]
        windowsInput -->|"REQ"| windows
        windows -->|"RESULT"| windowsOutput
    end

    subgraph browserFlow["Web content boundary"]
        direction TB
        webView["WebView2 Runtime"]
        nexus["Nexus Mods"]
        webViewInput["Selected download-page URI"]
        webRequest["HTTPS request"]
        webContent["Rendered web content"]
        webViewOutput["Browser state and page content"]
        webViewInput -->|"URI"| webView
        webView -->|"HTTPS"| webRequest
        webRequest -->|"REQUEST"| nexus
        nexus -->|"HTML"| webContent
        webContent -->|"RENDER"| webView
        webView -->|"PAGE"| webViewOutput
    end

    subgraph profileFlow["Browser profile boundary"]
        direction TB
        webViewData[("Per-user Local AppData WebView2 profile")]
        profileInput["WebView2 profile configuration"]
        profileOutput["Cookies and browser state"]
        profileInput -->|"CFG"| webViewData
        webViewData -->|"STATE"| profileOutput
    end

    userInput -->|"IN"| gui
    gui -->|"OUT"| userOutput
    gui -->|"ARG"| cliInput
    cliOutput -->|"IO"| gui
    gui -->|"FS"| localDataInput
    localDataOutput -->|"DATA"| gui
    pexInput -->|"PEX"| cli
    generatedOutput -->|"GEN"| localData
    gui -->|"REQ"| windowsInput
    windowsOutput -->|"RESULT"| gui
    gui -->|"URI"| webViewInput
    webViewOutput -->|"PAGE"| gui
    gui -->|"CFG"| profileInput
    profileOutput -->|"STATE"| gui
```

## Flow Key

| Code | Data exchanged |
| --- | --- |
| `IN` / `OUT` | User selections and confirmations, or GUI validation, status, progress, output, summaries, and help |
| `ARG` / `IO` | Structured CLI arguments and working directory, or standard output, standard error, and exit code |
| `FS` / `DATA` | Local settings/log/output-directory operations, or saved profiles, paths, metadata, executables, logs, and legal documents |
| `PEX` / `GEN` | PEX input, or generated Papyrus source and assembly |
| `REQ` / `RESULT` | Windows picker, clipboard, and shell requests, or selected paths and platform results |
| `URI` / `HTTPS` / `REQUEST` | Selected download-page URI, HTTPS request, or request sent to Nexus Mods |
| `HTML` / `RENDER` / `PAGE` | Rendered web content, rendering exchange, or browser state and page content |
| `CFG` / `STATE` | WebView2 profile configuration, or cookies and browser state |

## Boundary Rules

- Input and output paths remain transient GUI data and are not written to settings.
- The external CLI reads PEX files and writes generated output directly on the local file system.
- The GUI captures CLI streams and the exit code, then persists diagnostic data only for failures or noteworthy standard error.
- Web content is isolated to the embedded Help browser and does not enter the local execution request.
- WebView2 browser state is stored under per-user Local AppData and is removed by Windows uninstall.