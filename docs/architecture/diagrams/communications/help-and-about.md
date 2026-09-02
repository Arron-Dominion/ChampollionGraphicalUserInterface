# Help Browser and About Documents Communication Diagram

## Purpose

This diagram shows the object collaborations used for embedded download-page navigation, browser history, and packaged legal-document actions.

## Notation

```mermaid
flowchart LR
    actor(["Actor"])
    sender["sender : Type"]
    receiver["receiver : Type"]
    decision{"Decision?"}

    actor -->|"1: initiating message"| sender
    sender -->|"1.1: nestedCall()"| receiver
    receiver -.->|"1.2: return value"| sender
    sender -->|"2 *[each item]: repeatedMessage()"| receiver
    receiver --> decision
    decision -->|"[yes]"| sender
    decision -->|"[no]"| actor
```

## Diagram

```mermaid
flowchart TB
    subgraph browserWorkflow["1. Help browser collaboration"]
        direction TB
        browserUser(["user : User"])
        browserWindow["window : MainWindow"]
        browser["browser : NativeWebView"]
        runtime["runtime : Edge WebView2 Runtime"]
        profile["profile : WebView2<br/>Local AppData"]
        nexus["site : Nexus Mods"]

        browser -->|"1.0: EnvironmentRequested<br/>on attach"| browserWindow
        browserWindow -->|"1.0.1: set UserDataFolder"| profile
        browserUser -->|"1.1: select page"| browserWindow
        browserWindow -->|"1.2: Navigate(fixed URI)"| browser
        browser -->|"1.3: request navigation"| runtime
        runtime -->|"1.4: HTTPS request"| nexus
        nexus -.->|"1.5: page"| runtime
        runtime -.->|"1.6: rendered content"| browser
        browser -.->|"1.7: embedded content"| browserUser
    end

    subgraph historyWorkflow["2. Browser history collaboration"]
        direction TB
        historyUser(["user : User"])
        historyWindow["window : MainWindow"]
        historyBrowser["browser : NativeWebView"]
        historyRuntime["runtime : Edge WebView2 Runtime"]

        historyUser -->|"2.1: select Back, Forward,<br/>or Refresh"| historyWindow
        historyWindow -->|"2.2: browser history command"| historyBrowser
        historyBrowser -->|"2.3: perform action"| historyRuntime
        historyRuntime -.->|"2.4: updated page"| historyBrowser
    end

    subgraph legalWorkflow["3. About document collaboration"]
        direction TB
        legalUser(["user : User"])
        legalWindow["window : MainWindow"]
        applicationDirectory["files : Application Directory"]
        documentDecision{"Document exists?"}
        shell["shell : Windows Associated Application"]
        viewModel["viewModel : MainViewModel"]

        legalUser -->|"3.1: open license or notices"| legalWindow
        legalWindow -->|"3.2: check fixed filename"| applicationDirectory
        applicationDirectory -.->|"3.3: existence result"| legalWindow
        legalWindow --> documentDecision
        documentDecision -->|"3.4a [yes]: open document"| shell
        shell -.->|"3.5a: display document"| legalUser
        documentDecision -->|"3.4b [no]: set status"| viewModel
        viewModel -.->|"3.5b: display status"| legalUser
    end
```

## Collaboration Notes

- Edition-page actions supply fixed HTTPS locations; browser history actions operate on the page currently held by `NativeWebView`.
- WebView2 and Nexus Mods participate only in Help browsing and do not collaborate with local Champollion execution.
- The browser profile is stored in per-user Local AppData and is removed by the Windows installer during uninstall.
- Legal-document actions use fixed filenames beside the application, check existence, and delegate display to the Windows-associated application.