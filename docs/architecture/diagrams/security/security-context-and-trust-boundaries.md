# Security Context and Trust Boundaries

## Purpose

This diagram identifies the GUI's principal attack surfaces, trust zones, implemented boundary controls, and dependencies that remain outside the application's assurance boundary.

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
flowchart LR
    user["Local user"]:::external

    subgraph appBoundary["GUI process trust boundary"]
        ui(["Avalonia UI and MainViewModel"]):::component
        validation{{"Compatibility and local-path validation"}}:::control
        runner(["ChampollionRunner"]):::component
        settings(["AppSettingsStore and diagnostics"]):::component
        help(["Help and desktop handlers"]):::component
    end

    subgraph localBoundary["Current Windows user and local machine"]
        cli["Selected Champollion.exe"]:::external
        storage[("PEX, generated output,<br/>settings, and logs")]
        desktop["Windows shell, clipboard,<br/>pickers, and File Explorer"]:::external
        webView["Edge WebView2 Runtime"]:::external
    end

    subgraph internetBoundary["Internet and publisher boundary"]
        nexus["Nexus Mods and linked web content"]:::external
        releases["GitHub and mod-site release assets"]:::external
    end

    user -->|"Paths, options, confirmation,<br/>and browser actions"| ui
    releases -->|"GUI package and separately<br/>obtained Champollion distribution"| user
    ui --> validation --> runner
    ui --> settings
    ui --> help
    runner -->|"Structured arguments"| cli
    cli <-->|"PEX reads and generated writes"| storage
    settings <-->|"Plaintext configuration and diagnostics"| storage
    help -->|"Shell paths and clipboard text"| desktop
    help -->|"Fixed initial HTTPS URI<br/>and browser commands"| webView
    webView <-->|"Web requests and content"| nexus

    cliRisk["No executable signature or hash verification;<br/>CLI runs with the GUI user's permissions"]:::risk
    dataRisk["Writable local data has no application-level<br/>encryption or access-control hardening"]:::risk
    webRisk["No navigation allowlist after the initial<br/>fixed Nexus Mods locations"]:::risk

    cli -.-> cliRisk
    storage -.-> dataRisk
    webView -.-> webRisk

    classDef external fill:#fff4ce,stroke:#8a6d1d,color:#241f12
    classDef component fill:#d9eaf7,stroke:#286182,color:#102630
    classDef control fill:#dff3df,stroke:#39733d,color:#173319
    classDef risk fill:#f8d7da,stroke:#9b3a42,color:#3d1519
    style appBoundary fill:#f5f9fc,stroke:#286182,stroke-width:2px,stroke-dasharray:5 5
    style localBoundary fill:#fafafa,stroke:#666,stroke-width:2px,stroke-dasharray:5 5
    style internetBoundary fill:#fffaf0,stroke:#8a6d1d,stroke-width:2px,stroke-dasharray:5 5
```

## Security Posture

- The application has no account, authentication, authorization, server API, or inbound network listener.
- It executes locally with the current Windows user's authority; the external CLI is not sandboxed or privilege-separated.
- Validation reduces accidental and malformed path use but does not establish the identity or trustworthiness of a selected executable or PEX input.
- WebView2 is isolated from the decompilation request path at the application level, but rendered web content remains an external trust dependency.