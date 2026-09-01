# Unified Security Controls

## Purpose

This diagram summarizes the primary runtime and delivery controls and the residual risks that cross the GUI's trust boundaries.

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
    maintainer["Maintainer and GitHub"]:::external

    subgraph deliveryBoundary["Delivery trust boundary"]
        ci(["Build and release workflow"]):::component
        tests{{"Release tests"}}:::control
        exclude{{"Reject bundled Champollion.exe"}}:::control
        checksum{{"SHA-256 package checksums"}}:::control
        package[("GUI ZIP or installer")]
    end

    subgraph guiBoundary["GUI process trust boundary"]
        ui(["Avalonia GUI and orchestration"]):::component
        validate{{"Compatibility plus absolute,<br/>local fixed-drive path validation"}}:::control
        arguments{{"Structured ArgumentList and<br/>shell-free CLI launch"}}:::control
        persistence{{"Atomic settings replacement,<br/>corrupt backup, transient-path minimization"}}:::control
        fixedContent{{"Fixed initial HTTPS and<br/>packaged-document locations"}}:::control
    end

    subgraph externalBoundary["Current-user, platform, and Internet boundaries"]
        cli["Selected external Champollion.exe"]:::external
        localData[("PEX, output, settings, and logs")]
        windows["Windows shell, clipboard,<br/>pickers, and WebView2"]:::external
        web["Nexus Mods and linked content"]:::external
    end

    maintainer --> ci --> tests --> exclude --> checksum --> package
    package -->|"Install or extract"| user
    user -->|"Selections and confirmation"| ui
    ui --> validate --> arguments --> cli
    ui --> persistence --> localData
    cli <-->|"Read PEX, write generated files,<br/>return process streams"| localData
    ui --> fixedContent --> windows
    windows <-->|"HTTPS requests and content"| web

    executableRisk["CLI authenticity is not verified<br/>and execution is not sandboxed"]:::risk
    dataRisk["Writable plaintext settings and logs<br/>have no integrity or confidentiality control"]:::risk
    browserRisk["WebView navigation is not restricted<br/>after the fixed initial location"]:::risk
    supplyRisk["No signed binaries, attestations, or SBOM;<br/>some build dependencies are not immutably pinned"]:::risk

    cli -.-> executableRisk
    localData -.-> dataRisk
    windows -.-> browserRisk
    package -.-> supplyRisk

    classDef external fill:#fff4ce,stroke:#8a6d1d,color:#241f12
    classDef component fill:#d9eaf7,stroke:#286182,color:#102630
    classDef control fill:#dff3df,stroke:#39733d,color:#173319
    classDef risk fill:#f8d7da,stroke:#9b3a42,color:#3d1519
    style deliveryBoundary fill:#fffaf0,stroke:#8a6d1d,stroke-width:2px,stroke-dasharray:5 5
    style guiBoundary fill:#f5f9fc,stroke:#286182,stroke-width:2px,stroke-dasharray:5 5
    style externalBoundary fill:#fafafa,stroke:#666,stroke-width:2px,stroke-dasharray:5 5
```

## Control Summary

- Runtime controls validate compatibility and local paths, constrain protected output locations, build structured arguments, avoid shell-based CLI launch, and capture process results.
- Local-data controls reduce partial writes, preserve malformed settings, minimize persisted path data, and create diagnostics only for noteworthy runs.
- Integration controls start from fixed HTTPS download pages, fixed packaged legal filenames, and existing directories passed as structured Explorer arguments.
- Delivery controls run release tests, reject bundled third-party executables, restrict workflow permissions by job, and produce SHA-256 checksum files.

## Priority Residual Risks

- A selected or settings-supplied executable is not authenticated before it runs with current-user authority.
- Settings and diagnostic logs are writable plaintext files; logs may retain local paths and complete external-process output.
- Embedded browsing can navigate beyond the fixed starting origins, and external desktop handlers remain trusted dependencies.
- Release artifacts are neither code-signed nor attested, and selected build dependencies are referenced by mutable versions.