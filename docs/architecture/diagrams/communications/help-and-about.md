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
    user(["User"])
    window["window : MainWindow"]
    webView["browser : NativeWebView"]
    runtime["runtime : Edge WebView2 Runtime"]
    nexus["site : Nexus Mods"]
    applicationDirectory["files : Application Directory"]
    shell["shell : Windows Associated Application"]
    viewModel["viewModel : MainViewModel"]
    documentDecision{"Packaged document exists?"}

    user -->|"1a: select Current or Legacy page"| window
    window -->|"1a.1: Navigate(fixed Nexus Mods URI)"| webView
    webView -->|"1a.1.1: request navigation"| runtime
    runtime -->|"1a.1.1.1: HTTPS request"| nexus
    nexus -.->|"1a.1.1.2: download page"| runtime
    runtime -.->|"1a.1.2: rendered page"| webView
    webView -.->|"1a.2: embedded content"| user

    user -->|"1b: select Back, Forward, or Refresh"| window
    window -->|"1b.1: GoBack(), GoForward(),<br/>or Refresh()"| webView
    webView -->|"1b.1.1: perform browser action"| runtime
    runtime -.->|"1b.1.2: updated page"| webView

    user -->|"1c: open license or notices"| window
    window -->|"1c.1: check fixed packaged filename"| applicationDirectory
    applicationDirectory -.->|"1c.2: existence result"| window
    window --> documentDecision
    documentDecision -->|"1c.3a [yes]: start document<br/>with UseShellExecute"| shell
    shell -.->|"1c.3a.1: display document"| user
    documentDecision -->|"1c.3b [no]: set status"| viewModel
    viewModel -.->|"1c.3b.1: display missing-file status"| user
```

## Collaboration Notes

- Edition-page actions supply fixed HTTPS locations; browser history actions operate on the page currently held by `NativeWebView`.
- WebView2 and Nexus Mods participate only in Help browsing and do not collaborate with local Champollion execution.
- Legal-document actions use fixed filenames beside the application, check existence, and delegate display to the Windows-associated application.