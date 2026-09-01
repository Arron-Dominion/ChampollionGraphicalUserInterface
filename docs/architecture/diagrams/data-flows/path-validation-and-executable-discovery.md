# Path Validation and Executable Discovery Data Flow Diagram

## Purpose

This DFD shows how entered or selected paths are normalized and validated, and how automatic discovery converts filesystem metadata into a matching executable path and progress data.

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
flowchart LR
    user["User"]
    windows["Windows File and Folder Pickers"]
    pathFlow(["2.0 Capture and Validate Paths"])
    search(["2.1 Discover Compatible Executable"])
    classify(["2.2 Classify Executable Edition"])
    fixedDrives[("D2 Local Fixed Drives<br/>files, directories, and drive metadata")]
    distribution[("D3 Champollion Distribution<br/>EXE, version, and companion files")]
    settings[("D1 Application UserData<br/>settings.json")]

    user -->|"Browse request or entered path"| pathFlow
    pathFlow -->|"Picker options"| windows
    windows -->|"Selected local path"| pathFlow
    pathFlow -->|"Existence, extension, parent,<br/>drive-type, and protection queries"| fixedDrives
    fixedDrives -->|"File, directory, and drive metadata"| pathFlow
    pathFlow -->|"Normalized path or validation error"| user

    user -->|"Selected edition and search command"| search
    fixedDrives -->|"Starting roots and child directories"| search
    search -->|"Candidate Champollion.exe path"| pathFlow
    pathFlow -->|"Validated candidate path"| search
    search -->|"Candidate path"| classify
    distribution -->|"Companion-file presence and version metadata"| classify
    classify -->|"Legacy, Current, or Unknown"| search
    search -->|"Worker counts and directories searched"| user
    search -->|"Matching executable path"| settings
    search -->|"Match, no result, or cancellation status"| user
```

## Data Rules

- Validation returns `PathValidationResult`: validity, normalized absolute path, and an error when invalid.
- Search progress contains directories searched, active workers, and configured worker count.
- Search reads only ready local fixed-drive trees and skips unsupported or excluded locations.
- Classification consumes distribution metadata, not executable contents. A complete companion layout identifies Legacy; a valid standalone distribution identifies Current; ambiguous partial layouts remain Unknown.
- Only a successful matching executable path is saved. Cancellation or exhaustion restores the previous path.