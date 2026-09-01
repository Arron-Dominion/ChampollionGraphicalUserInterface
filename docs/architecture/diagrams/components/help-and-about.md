# Help Browser and About Documents Component Diagram

## Purpose

This diagram shows the Help tab's embedded download browser and the About tab's packaged legal-document actions. These features do not participate in local Champollion execution.

```mermaid
flowchart LR
    user["User"]

    subgraph ui["GUI project"]
        xaml["MainWindow.axaml<br/>Help and About controls"]
        codeBehind["MainWindow code-behind<br/>Navigation and document handlers"]
        browser["NativeWebView<br/>Embedded browser control"]
        viewModel["MainViewModel<br/>Status message for missing documents"]
    end

    webView2["Microsoft Edge WebView2 Runtime"]
    legacy["Nexus Mods<br/>Legacy Skyrim page 35307"]
    current["Nexus Mods<br/>Current Starfield page 4528"]
    storage[("Application directory<br/>LICENSE.txt and THIRD-PARTY-NOTICES.txt")]
    shell["Windows associated application"]

    user -->|"Selects edition page, Back,<br/>Forward, or Refresh"| xaml
    xaml -->|"Raises event"| codeBehind
    codeBehind -->|"Navigate, GoBack, GoForward, or Refresh"| browser
    browser -->|"Uses native host"| webView2
    webView2 <-->|"HTTPS content"| legacy
    webView2 <-->|"HTTPS content"| current

    user -->|"Opens license or notices"| xaml
    xaml -->|"Raises event"| codeBehind
    codeBehind -->|"Checks packaged file"| storage
    codeBehind -->|"Opens existing document"| shell
    codeBehind -->|"Reports missing file"| viewModel
    viewModel -->|"Updates visible status"| xaml
```

## Key Relationships

- `MainWindow.axaml` owns the `NativeWebView`; code-behind supplies the fixed Legacy and Current Nexus Mods URIs and navigation actions.
- `Avalonia.Controls.WebView` uses the separately supplied WebView2 Runtime on Windows.
- The About actions resolve legal documents beside the running application and ask Windows to open them with the associated application.
- Browser availability and legal-document actions are independent of the local `Champollion.exe` execution workflow.