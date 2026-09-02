# WebView2 and Desktop Integration Security

## Purpose

This diagram shows the security boundaries crossed by embedded web browsing, clipboard writes, file and folder selection, File Explorer, and associated-application launches.

## Notation

```mermaid
flowchart LR
    subgraph boundary["Trust boundary"]
        external["External<br/>Outside the GUI"]:::external
        component(["GUI component<br/>Inside the application"]):::component
        control{{"Implemented<br/>security control"}}:::control
        data[("Local<br/>data")]
    end

    risk["Residual risk<br/>or assumption"]:::risk
    external --> component --> control --> data
    data -.-> risk

    classDef external fill:#fff4ce,stroke:#8a6d1d,color:#241f12
    classDef component fill:#d9eaf7,stroke:#286182,color:#102630
    classDef control fill:#dff3df,stroke:#39733d,color:#173319
    classDef risk fill:#f8d7da,stroke:#9b3a42,color:#3d1519
    style boundary fill:#fafafa,stroke:#666,stroke-width:2px,stroke-dasharray:5 5
```

See the [security diagram notation table](README.md#notation) for additional detail.

## Diagram

```mermaid
flowchart TB
    user["Local user"]:::external

    subgraph guiBoundary["GUI process trust boundary"]
        help(["Help browser handlers"]):::component
        desktop(["Desktop action handlers"]):::component
        fixedUris{{"Hard-coded HTTPS Nexus Mods<br/>initial and edition-page URIs"}}:::control
        fixedDocs{{"Fixed legal filenames plus<br/>existence check"}}:::control
        existingFolder{{"Resolve and require an existing<br/>output directory before opening"}}:::control
        structuredShell{{"ArgumentList for explorer.exe<br/>directory handoff"}}:::control
    end

    subgraph osBoundary["Windows platform boundary"]
        webView["Edge WebView2 Runtime"]:::external
        clipboard["Windows Clipboard"]:::external
        explorer["File Explorer"]:::external
        associatedApp["Associated text-file application"]:::external
        pickers["Native file and folder pickers"]:::external
    end

    subgraph internetBoundary["Internet content boundary"]
        nexus["Nexus Mods pages"]:::external
        linkedSites["Page-selected linked content"]:::external
    end

    legal[("Packaged LICENSE.txt and<br/>THIRD-PARTY-NOTICES.txt")]
    output[("In-memory process output")]
    webViewData[("Per-user Local AppData<br/>WebView2 profile data")]

    user -->|"Edition page, Back, Forward,<br/>or Refresh"| help
    help --> fixedUris --> webView
    help -->|"UserDataFolder configuration"| webViewData
    webView <-->|"HTTPS content"| nexus
    nexus <-->|"User-followed page links"| linkedSites
    help -->|"Browser history commands"| webView

    user -->|"Copy output"| desktop
    output --> desktop -->|"Complete displayed text"| clipboard
    user -->|"Browse for path"| desktop
    desktop <-->|"Picker constraints and selected path"| pickers
    desktop --> existingFolder --> structuredShell --> explorer
    legal --> fixedDocs --> associatedApp

    navigationRisk["No application navigation allowlist,<br/>request filter, or origin enforcement"]:::risk
    clipboardRisk["Clipboard contents are available to the<br/>desktop environment and other same-user software"]:::risk
    handlerRisk["Explorer, WebView2, and file handlers are<br/>external processes trusted at invocation"]:::risk

    webView -.-> navigationRisk
    clipboard -.-> clipboardRisk
    explorer -.-> handlerRisk
    associatedApp -.-> handlerRisk

    classDef external fill:#fff4ce,stroke:#8a6d1d,color:#241f12
    classDef component fill:#d9eaf7,stroke:#286182,color:#102630
    classDef control fill:#dff3df,stroke:#39733d,color:#173319
    classDef risk fill:#f8d7da,stroke:#9b3a42,color:#3d1519
    style guiBoundary fill:#f5f9fc,stroke:#286182,stroke-width:2px,stroke-dasharray:5 5
    style osBoundary fill:#fafafa,stroke:#666,stroke-width:2px,stroke-dasharray:5 5
    style internetBoundary fill:#fffaf0,stroke:#8a6d1d,stroke-width:2px,stroke-dasharray:5 5
```

## Implemented Controls

- The initial Help URI and the two edition buttons use fixed HTTPS Nexus Mods locations.
- WebView2 user data is assigned to a per-user Local AppData directory instead of the installed application directory.
- Windows uninstall removes the application-specific WebView2 profile, including its cookies and cached login state.
- Legal-document actions combine the application directory with one of two fixed filenames and check existence before shell launch.
- Output-folder actions require a resolved existing directory and pass it to `explorer.exe` as one structured argument.
- Executable and PEX pickers expose extension filters, with authoritative path validation performed after selection.
- Clipboard integration writes the displayed output and does not read arbitrary clipboard content.

## Residual Trust

- The GUI does not subscribe to a navigation event to restrict origins or schemes after WebView2 displays the initial page. Page links and browser history can therefore leave the fixed locations.
- Web content, WebView2, File Explorer, clipboard consumers, and associated file handlers are outside the GUI process boundary.
- Copying output may disclose paths or tool output through clipboard history, synchronization, or other same-user software.