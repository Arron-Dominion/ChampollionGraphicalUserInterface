# Settings, Logs, and Local Data Security

## Purpose

This diagram shows local configuration and diagnostic data handling, resilience controls, and confidentiality and integrity limits of application-writable storage.

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
    user["Local user and same-user processes"]:::external

    subgraph guiBoundary["GUI process trust boundary"]
        settings(["AppSettingsStore"]):::component
        logging(["DiagnosticLogWriter"]):::component
        atomic{{"Serialize to unique temporary file,<br/>then replace settings.json"}}:::control
        recovery{{"Preserve malformed JSON as<br/>settings.corrupt timestamped backup"}}:::control
        migration{{"Copy missing legacy files,<br/>then delete migrated sources"}}:::control
        minimization{{"Do not persist input paths,<br/>output paths, live output, or status"}}:::control
        conditionalLog{{"Write diagnostics only for failure<br/>or nonempty standard error"}}:::control
    end

    subgraph writableBoundary["Application-writable UserData boundary"]
        settingsFile[("settings.json<br/>Executable paths, games, option profiles")]
        corrupt[("settings.corrupt-*.json")]
        logs[("Logs/Champollion-*.log<br/>Paths, exit codes, stdout, stderr")]
    end

    legacy[("Legacy LocalAppData<br/>settings.json and Logs")]

    user <-->|"Read, modify, delete, or copy<br/>same-user local files"| settingsFile
    legacy --> migration
    migration --> settingsFile
    migration --> logs
    settings --> atomic --> settingsFile
    settingsFile --> settings
    settings --> recovery --> corrupt
    minimization --> settings
    logging --> conditionalLog --> logs

    tamperRisk["No application-level integrity check;<br/>a modified executable path may later be launched"]:::risk
    disclosureRisk["No encryption or redaction;<br/>logs may expose local paths and process output"]:::risk
    retentionRisk["No automatic log or corrupt-backup<br/>retention and deletion policy"]:::risk

    settingsFile -.-> tamperRisk
    logs -.-> disclosureRisk
    corrupt -.-> retentionRisk

    classDef external fill:#fff4ce,stroke:#8a6d1d,color:#241f12
    classDef component fill:#d9eaf7,stroke:#286182,color:#102630
    classDef control fill:#dff3df,stroke:#39733d,color:#173319
    classDef risk fill:#f8d7da,stroke:#9b3a42,color:#3d1519
    style guiBoundary fill:#f5f9fc,stroke:#286182,stroke-width:2px,stroke-dasharray:5 5
    style writableBoundary fill:#fafafa,stroke:#666,stroke-width:2px,stroke-dasharray:5 5
```

## Implemented Controls

- Settings replacement uses a unique temporary file and cleans it in a `finally` block, reducing partial-write exposure.
- Malformed JSON is moved aside and defaults are used; temporary I/O or access failures return defaults without misclassifying the file as corrupt.
- Migration copies settings and log files only when the destination is absent, then removes successfully migrated sources.
- Persisted settings contain executable paths, remembered games, and option profiles. PEX input and output paths remain transient.
- Diagnostic filenames are unique and logs are created only for failed processes or standard-error output.

## Data Protection Limits

- The application does not encrypt, sign, authenticate, redact, or assign narrower ACLs to settings, logs, or corrupt backups.
- The Windows installer deliberately grants ordinary users modify access to `UserData` and the application-owned output directories.
- Diagnostic logs retain complete input paths, standard output, and standard error until a user or external maintenance process removes them.