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
flowchart TB
    subgraph desktopFlow["1. Output and desktop actions"]
        direction TB
        desktopUser["user : User"]
        desktopCommand["Copy output command<br/>or open output, log,<br/>or settings command"]
        desktop["4.0 Present Output<br/>and Desktop Actions"]
        settings[("D1 UserData/<br/>settings.json")]
        outputs[("D5 Generated<br/>Papyrus and Assembly")]
        logs[("D6 UserData/Logs")]
        clipboard["Windows Clipboard"]
        explorer["Windows File Explorer<br/>and Shell"]

        desktopUser -->|"desktop action command"| desktopCommand
        desktopCommand --> desktop
        settings -->|"settings<br/>directory path"| desktop
        outputs -->|"existing output<br/>directory path"| desktop
        logs -->|"diagnostic<br/>log path"| desktop
        desktop -->|"complete displayed<br/>output text"| clipboard
        desktop -->|"directory or<br/>selected-file path"| explorer
        explorer -.->|"shell result"| desktop
        desktop -.->|"status or<br/>opened location"| desktopUser
    end

    subgraph browserFlow["2. Help browser data flow"]
        direction TB
        browserUser["user : User"]
        help["5.0 Serve Help<br/>and About Content"]
        webView["Edge WebView2<br/>Runtime"]
        nexus["Nexus Mods"]
        webViewProfile[("Per-user Local AppData<br/>WebView2 profile")]

        browserUser -->|"Current or Legacy page<br/>or browser navigation"| help
        help -->|"fixed Nexus Mods URI<br/>or history command"| webView
        webView -->|"HTTPS request"| nexus
        nexus -.->|"download-page HTML<br/>and assets"| webView
        webView -.->|"rendered page and<br/>browser state"| help
        help -->|"profile configuration"| webViewProfile
        help -.->|"embedded browser<br/>content"| browserUser
    end

    subgraph legalFlow["3. About document data flow"]
        direction TB
        legalUser["user : User"]
        legalHelp["5.0 Serve Help<br/>and About Content"]
        legal[("D7 Packaged Legal Documents<br/>LICENSE.txt and THIRD-PARTY-NOTICES.txt")]
        legalExplorer["Windows File Explorer<br/>and Shell"]
        legalStatus["GUI status"]
        legalDecision{"Document exists?"}

        legalUser -->|"open license or notices"| legalHelp
        legal -->|"document path and existence"| legalHelp
        legalHelp --> legalDecision
        legalDecision -->|"[yes]:<br/>existing document path"| legalExplorer
        legalDecision -->|"[no]:<br/>missing-file status"| legalStatus
        legalExplorer -.->|"display document"| legalUser
        legalStatus -.->|"status"| legalUser
    end
```

## Data Boundaries

- Clipboard receives the complete in-memory output text; the GUI does not read clipboard contents back.
- File Explorer and the Windows shell receive local paths only. They do not return file contents to the GUI.
- WebView2 exchanges selected URIs and browser state with the GUI and obtains page content from Nexus Mods over HTTPS.
- WebView2 persists browser state in the current user's Local AppData; Windows uninstall removes this application-specific profile.
- Legal documents are packaged application files. Missing files produce a GUI status message rather than execution failure.