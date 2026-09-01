# Help Browser and About Documents Sequence Diagram

## Purpose

This diagram shows the Help tab's WebView2-backed Nexus Mods navigation and the About tab's packaged legal-document actions.

```mermaid
sequenceDiagram
    actor User
    participant Window as MainWindow
    participant WebView as NativeWebView
    participant Runtime as Edge WebView2 Runtime
    participant Nexus as Nexus Mods
    participant FS as Application Directory
    participant Shell as Windows Associated Application
    participant VM as MainViewModel

    alt User opens Current or Legacy download page
        User->>Window: Select Current or Legacy
        Window->>WebView: Navigate(fixed Nexus Mods URI)
        WebView->>Runtime: Request navigation
        Runtime->>Nexus: HTTPS request
        Nexus-->>Runtime: Download page
        Runtime-->>WebView: Render page
        WebView-->>User: Display embedded content
    else User navigates browser history
        User->>Window: Select Back, Forward, or Refresh
        Window->>WebView: GoBack(), GoForward(), or Refresh()
        WebView->>Runtime: Perform browser action
        Runtime-->>WebView: Updated page
    else User opens legal document
        User->>Window: Select license or third-party notices
        Window->>FS: Check packaged document
        alt Document exists
            Window->>Shell: Start document with UseShellExecute
            Shell-->>User: Display document
        else Document is missing
            Window->>VM: Set missing-file status
            VM-->>User: Display status in workspace
        end
    end
```

## Notes

- Current navigates to Starfield mod `4528`; Legacy navigates to Skyrim mod `35307`.
- WebView2 availability affects only the embedded download browser, not locally configured CLI execution.
- Legal documents are expected beside the running GUI executable because the project copies them to build and publish output.