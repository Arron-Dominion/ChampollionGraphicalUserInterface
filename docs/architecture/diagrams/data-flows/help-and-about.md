# Help and About Data Flow Diagram

## Purpose

This DFD details process `5.0 Serve Help and About Content`, including embedded Help browsing, Nexus Mods content, the per-user WebView2 profile, and packaged legal documents.

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
    help(["5.0 Serve Help<br/>and About Content"])
    legal[("D7 Packaged Legal Documents<br/>LICENSE.txt and THIRD-PARTY-NOTICES.txt")]
    explorer["Windows File Explorer<br/>and Shell"]
    status["GUI status"]
    decision{"Document exists?"}
    webView["E4 Edge WebView2 Runtime"]
    nexus["E5 Nexus Mods"]
    profile[("D8 Per-user Local AppData<br/>WebView2 profile")]

    user -->|"CMD"| help
    legal -->|"DOC"| help
    help --> decision
    decision -->|"YES"| explorer
    decision -->|"NO"| status
    explorer -.->|"DISPLAY"| user
    status -.->|"STATUS"| user

    help -->|"NAV"| webView
    webView -->|"HTTPS"| nexus
    nexus -.->|"HTML"| webView
    webView -.->|"PAGE"| help
    help -->|"CFG"| profile
    profile -.->|"STATE"| help
    help -.->|"VIEW"| user
```

## Flow Key

| Code | Data exchanged |
| --- | --- |
| `CMD` | Help navigation, browser navigation, or legal-document command |
| `DOC` | Document path and existence information |
| `YES` / `NO` | Existing document path or missing-file status branch |
| `DISPLAY` / `STATUS` | Displayed legal document or GUI status |
| `NAV` | Current or Legacy page selection or browser navigation |
| `HTTPS` / `HTML` / `PAGE` | HTTPS request, download-page HTML/assets, or rendered page content |
| `CFG` / `STATE` | WebView2 profile configuration or cookies and browser state |
| `VIEW` | Embedded browser content returned to the user |

## Data Boundaries

- Process `5.0` handles Help navigation and About/legal document commands.
- Legal documents are packaged application files. Missing files produce a GUI status message rather than execution failure.
- WebView2 exchanges selected page navigation and browser state with the GUI and obtains page content from Nexus Mods over HTTPS.
- The browser profile is stored in per-user Local AppData; Windows uninstall removes the application-specific profile.
