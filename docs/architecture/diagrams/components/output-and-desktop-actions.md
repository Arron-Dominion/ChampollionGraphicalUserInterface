# Output and Desktop Actions Component Diagram

## Purpose

This diagram shows how execution output reaches the workspace and how users copy text or navigate to output, log, and settings locations through Windows desktop services.

```mermaid
flowchart LR
    user["User"]

    subgraph ui["GUI project"]
        view["MainWindow<br/>Live output view and desktop event handlers"]
        viewModel["MainViewModel<br/>Output text, status, paths, and commands"]
    end

    subgraph application["Application project"]
        runner["ChampollionRunner<br/>Execution output source"]
        settings["AppSettingsStore<br/>Settings directory location"]
    end

    clipboard["Windows Clipboard"]
    explorer["Windows File Explorer"]
    storage[("Output folders, UserData/Logs,<br/>and UserData/settings.json")]

    runner -->|"ExecutionOutput chunks and summary"| viewModel
    viewModel -->|"Property change notifications"| view
    view -->|"Auto-scrolls and renders complete output"| user
    user -->|"Selects Copy"| view
    view -->|"Writes OutputText"| clipboard
    user -->|"Opens generated output"| view
    view -->|"Requests effective directory"| viewModel
    viewModel -->|"Returns configured path or CLI working directory"| view
    view -->|"Launches folder"| explorer
    user -->|"Opens diagnostic log"| viewModel
    viewModel -->|"Launches explorer.exe with selected log"| explorer
    user -->|"Opens settings folder"| viewModel
    viewModel -->|"Gets directory"| settings
    viewModel -->|"Launches explorer.exe"| explorer
    explorer <-->|"Displays local files and folders"| storage
```

## Key Relationships

- `MainViewModel` accumulates complete standard output and prefixed standard-error chunks while retaining progress and final per-input statuses.
- `MainWindow` listens for output property changes and posts automatic scrolling to the Avalonia UI thread.
- Clipboard and output-folder actions remain view-specific platform interactions.
- Generated `OpenLogFolderCommand` and `OpenSettingsFolderCommand` launch File Explorer from `MainViewModel` using locations supplied by execution results and `AppSettingsStore`.