# Desktop, Help, and About Data Flow Diagram

## Purpose

This DFD shows data exchanged with Windows desktop services, the embedded browser, Nexus Mods, and packaged legal documents outside the core execution flow.

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
    desktop(["4.0 Present Output and Desktop Actions"])
    help(["5.0 Serve Help and About Content"])
    clipboard["Windows Clipboard"]
    explorer["Windows File Explorer and Shell"]
    webView["Edge WebView2 Runtime"]
    nexus["Nexus Mods"]

    settings[("D1 Application UserData<br/>settings.json")]
    outputs[("D5 Generated Papyrus and Assembly")]
    logs[("D6 UserData/Logs")]
    legal[("D7 Application Legal Documents<br/>LICENSE.txt and THIRD-PARTY-NOTICES.txt")]

    user -->|"Copy output command"| desktop
    desktop -->|"Complete displayed output text"| clipboard

    user -->|"Open output, log, or settings command"| desktop
    settings -->|"Settings directory path"| desktop
    outputs -->|"Existing output directory path"| desktop
    logs -->|"Diagnostic log path"| desktop
    desktop -->|"Directory or selected-file path"| explorer
    explorer -->|"Shell launch result"| desktop
    desktop -->|"Status or opened location"| user

    user -->|"Current or Legacy page selection<br/>and browser navigation command"| help
    help -->|"Fixed Nexus Mods URI or history command"| webView
    webView -->|"HTTPS request"| nexus
    nexus -->|"Download-page HTML and assets"| webView
    webView -->|"Rendered page and browser state"| help
    help -->|"Embedded browser content"| user

    user -->|"Open legal document command"| help
    legal -->|"Document path and existence"| help
    help -->|"Existing document path"| explorer
    help -->|"Document or missing-file status"| user
```

## Data Boundaries

- Clipboard receives the complete in-memory output text; the GUI does not read clipboard contents back.
- File Explorer and the Windows shell receive local paths only. They do not return file contents to the GUI.
- WebView2 exchanges selected URIs and browser state with the GUI and obtains page content from Nexus Mods over HTTPS.
- Legal documents are packaged application files. Missing files produce a GUI status message rather than execution failure.