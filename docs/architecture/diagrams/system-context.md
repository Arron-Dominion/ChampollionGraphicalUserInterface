# System Context Diagram

## Purpose

This diagram shows Champollion Graphical User Interface as a single system and the people, software, storage, and web locations with which it directly interacts. Internal UI, Application, and Domain components are intentionally outside the scope of this view.

```mermaid
flowchart LR
    user["User<br/>Configures and runs decompilation,<br/>reviews output and diagnostics"]

    app["Champollion Graphical User Interface<br/>Windows x64 Avalonia desktop application"]

    subgraph localMachine["Windows 10/11 x64 machine"]
        fileSystem[("Local fixed-drive file system<br/>PEX inputs, generated Papyrus and assembly,<br/>UserData settings and logs")]
        legacy["Legacy Champollion.exe<br/>External PEX decompiler<br/>with companion distribution"]
        current["Current Champollion.exe<br/>External PEX decompiler<br/>standalone distribution"]
        windowsServices["Windows desktop services<br/>File and folder pickers, File Explorer,<br/>clipboard, drive discovery"]
        webView2["Microsoft Edge WebView2 Runtime<br/>Hosts the embedded Help browser"]
    end

    subgraph nexusMods["Nexus Mods"]
        legacyPage["Legacy Champollion location<br/>Skyrim mod 35307"]
        currentPage["Current Champollion location<br/>Starfield mod 4528"]
    end

    user -->|"Selects operations, paths, games,<br/>editions, and options"| app
    app -->|"Shows status, captured process output,<br/>progress, and confirmations"| user

    app <-->|"Validates and searches paths;<br/>reads PEX files; writes settings and logs"| fileSystem
    legacy <-->|"Reads PEX input and writes requested output"| fileSystem
    current <-->|"Reads PEX input and writes requested output"| fileSystem

    app -->|"Starts the selected edition with<br/>structured command arguments"| legacy
    legacy -->|"Returns standard output, standard error,<br/>and process exit code"| app
    app -->|"Starts the selected edition with<br/>structured command arguments"| current
    current -->|"Returns standard output, standard error,<br/>and process exit code"| app

    app <-->|"Uses native desktop integration"| windowsServices
    app -->|"Embeds browser through<br/>Avalonia.Controls.WebView"| webView2
    webView2 <-->|"Loads download page over HTTPS"| legacyPage
    webView2 <-->|"Loads download page over HTTPS"| currentPage
```

## Boundary Notes

- The application does not contain or distribute either Champollion executable. The user obtains an edition separately and selects its local `Champollion.exe`, or asks the application to search eligible local fixed drives.
- Legacy and Current Champollion are separate external command-line tools. The application supplies structured arguments, captures both output streams, and treats the process exit code as the success signal.
- The local file system holds selected `.pex` inputs, generated Papyrus source and optional assembly, application-adjacent `UserData/settings.json`, and diagnostic logs. Network, mapped, and removable drives are outside the supported path boundary.
- The Help browser uses `Avalonia.Controls.WebView` backed by the Microsoft Edge WebView2 Runtime. It opens the Legacy page at `https://www.nexusmods.com/skyrim/mods/35307` and the Current page at `https://www.nexusmods.com/starfield/mods/4528`.
- Windows desktop services provide file and folder selection, clipboard access, File Explorer navigation, fixed-drive discovery, and the WebView2 host. The currently supported product boundary is Windows 10/11 x64.