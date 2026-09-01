# Path Selection and Validation Communication Diagram

## Purpose

This diagram shows the object collaborations used to browse for executable, input, and output paths and to validate changed executable or PEX input paths.

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
flowchart LR
    user(["User"])
    window["window : MainWindow"]
    picker["picker : Avalonia StorageProvider"]
    fileSystem["fileSystem : Local File System"]
    viewModel["viewModel : MainViewModel"]
    validator["validator : LocalPathValidator"]
    selectionDecision{"Path selected?"}
    pathTypeDecision{"Selected path type?"}

    user -->|"1: select Browse executable,<br/>PEX file, or folder"| window
    window -->|"1.1: OpenFilePickerAsync()<br/>or OpenFolderPickerAsync()"| picker
    picker -->|"1.1.1: enumerate eligible entries"| fileSystem
    fileSystem -.->|"1.1.2: files and folders"| picker
    picker -.->|"1.1.3: selected local path<br/>or empty selection"| window

    window --> selectionDecision
    selectionDecision -.->|"1.2a [no]: preserve current path"| user
    selectionDecision -->|"[yes]"| pathTypeDecision
    pathTypeDecision -->|"1.2b [executable or input]:<br/>set bound path"| viewModel
    viewModel -->|"1.2b.1: ValidateExecutable()<br/>or ValidateInput()"| validator
    validator -->|"1.2b.1.1: normalize and inspect drive,<br/>existence, extension, and type"| fileSystem
    fileSystem -.->|"1.2b.1.2: path and drive metadata"| validator
    validator -.->|"1.2b.2: PathValidationResult"| viewModel
    viewModel -.->|"1.2b.3: validation property changed"| window
    window -.->|"1.2b.4: display validation result"| user

    pathTypeDecision -->|"1.2c [output folder]: set source<br/>or assembly output path"| viewModel
    viewModel -.->|"1.2c.1: bound path changed"| window
```

## Collaboration Notes

- Picker filters support selection, but `LocalPathValidator` performs authoritative executable and PEX input validation after the bound value changes.
- Output-folder selection updates `MainViewModel` without immediate validation; `ChampollionRunner` validates outputs before creating directories or launching the CLI.
- Cancellation returns no selected object and leaves the existing bound path unchanged.