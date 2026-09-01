# Output and Desktop Actions Sequence Diagram

## Purpose

This diagram shows live-output auto-scroll, clipboard copy, and navigation to generated output, diagnostic logs, or application settings.

```mermaid
sequenceDiagram
    actor User
    participant Runner as ChampollionRunner
    participant VM as MainViewModel
    participant Window as MainWindow
    participant Dispatcher as Avalonia UI Dispatcher
    participant Clipboard as Windows Clipboard
    participant FS as Local File System
    participant Explorer as File Explorer
    participant Store as AppSettingsStore

    Runner-->>VM: ExecutionOutput chunk
    VM->>VM: Append OutputText
    VM-->>Window: OutputText property changed
    Window->>Dispatcher: Post ScrollToEnd()
    Dispatcher-->>Window: Scroll live output

    alt User selects Copy
        User->>Window: CopyOutput_Click
        Window->>VM: Read OutputText
        VM-->>Window: Complete displayed output
        Window->>Clipboard: SetValueAsync(text)
    else User opens source or assembly output
        User->>Window: Open output folder
        Window->>VM: ResolveOutputDirectory(path)
        VM-->>Window: Effective directory or null
        Window->>FS: Directory.Exists(directory)
        alt Directory exists
            Window->>Explorer: Start with directory argument
        else Directory is unavailable
            Window->>VM: Set status message
            VM-->>User: Report that a run is required
        end
    else User opens diagnostic log
        User->>VM: OpenLogFolderCommand.Execute()
        VM->>Explorer: Start with /select and LogPath
    else User opens settings folder
        User->>VM: OpenSettingsFolderCommand.Execute()
        VM->>Store: Read SettingsDirectory
        Store-->>VM: UserData directory
        VM->>Explorer: Start with settings directory
    end
```

## Notes

- Auto-scroll is posted at background dispatcher priority after each `OutputText` property change.
- Output-folder resolution uses the configured path or falls back to the selected CLI executable's working directory.
- Clipboard and output-folder handlers remain in the view; generated log and settings commands are exposed by the view model.