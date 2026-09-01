# Path Selection and Validation Component Diagram

## Purpose

This diagram shows how executable, PEX input, and output paths move from native Windows pickers through GUI state into Application-owned validation and normalization.

```mermaid
flowchart LR
    user["User"]

    subgraph ui["GUI project"]
        view["MainWindow<br/>Picker and open-folder event handlers"]
        viewModel["MainViewModel<br/>Path state, messages, and output resolution"]
    end

    subgraph application["Application project"]
        validator["LocalPathValidator<br/>Executable, input, and output validation"]
        outputPaths["ApplicationOutputPaths<br/>Default source and assembly folders"]
        validationResult["PathValidationResult<br/>Normalized path or error"]
    end

    pickers["Avalonia StorageProvider<br/>Windows file and folder pickers"]
    explorer["Windows File Explorer"]
    storage[("Local fixed-drive file system")]

    user -->|"Browses or enters a path"| view
    view -->|"Opens picker"| pickers
    pickers <-->|"Enumerates selectable files and folders"| storage
    pickers -->|"Returns local path"| view
    view -->|"Updates bound path"| viewModel
    viewModel -->|"Validates on executable or input change"| validator
    validator -->|"Returns"| validationResult
    validationResult -->|"Updates validation message"| viewModel
    validator <-->|"Checks existence, drive type,<br/>extension, and protected locations"| storage
    outputPaths -->|"Supplies startup and reset defaults"| viewModel
    viewModel -->|"Resolves effective output folder"| view
    view -->|"Opens existing folder"| explorer
```

## Key Relationships

- `MainWindow` owns platform picker and File Explorer interactions; it passes selected local paths to `MainViewModel`.
- `MainViewModel` validates executable and input paths as they change and displays the returned message.
- `LocalPathValidator` expands environment variables, normalizes full paths, requires local fixed drives, and applies executable, input, and output-specific rules.
- Output paths receive full validation again in `ChampollionRunner` before directories are created and execution begins.