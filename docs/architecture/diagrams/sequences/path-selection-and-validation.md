# Path Selection and Validation Sequence Diagram

## Purpose

This diagram shows native file or folder selection and the immediate validation performed when an executable or PEX input path changes.

```mermaid
sequenceDiagram
    actor User
    participant Window as MainWindow
    participant Picker as Avalonia StorageProvider
    participant FS as Local File System
    participant VM as MainViewModel
    participant Validator as LocalPathValidator

    User->>Window: Select Browse executable, file, or folder
    Window->>Picker: OpenFilePickerAsync() or OpenFolderPickerAsync()
    Picker->>FS: Enumerate eligible local entries
    FS-->>Picker: Files and folders
    Picker-->>User: Display native picker
    alt User cancels picker
        Picker-->>Window: Empty selection
        Window-->>User: Preserve current path
    else User selects executable or PEX input
        Picker-->>Window: Selected local path
        Window->>VM: Set ExecutablePath or InputPath
        VM->>Validator: ValidateExecutable() or ValidateInput()
        Validator->>FS: Normalize path and check drive, existence, and type
        FS-->>Validator: Path and drive metadata
        Validator-->>VM: PathValidationResult
        VM-->>Window: ExecutableValidation or InputValidation changed
        Window-->>User: Display validation result
    else User selects output folder
        Picker-->>Window: Selected local path
        Window->>VM: Set SourceOutputPath or AssemblyOutputPath
        VM-->>Window: Bound path changed
        Note over VM,Validator: Output validation is deferred until run validation.
    end
```

## Notes

- Executable and PEX input paths are validated immediately through generated property-change hooks.
- Output selection updates GUI state immediately, but `ChampollionRunner` performs authoritative output validation before creating directories or starting the CLI.
- Environment variables are expanded and paths are normalized by Application-owned validation rather than picker code.