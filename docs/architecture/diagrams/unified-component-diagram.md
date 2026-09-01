# Unified Component Diagram

## Purpose

This diagram combines the major components used by all GUI features. It expands the GUI container into its UI, Application, and Domain project boundaries while retaining local storage, Windows integrations, WebView2, Nexus Mods, and the external Champollion CLI at the system edge.

```mermaid
flowchart TB
    user["User"]

    subgraph guiProject["ChampollionGraphicalUserInterface - Avalonia GUI project"]
        app["App<br/>Composition root"]
        mainWindow["MainWindow<br/>XAML views, dialogs, pickers,<br/>browser, clipboard, and shell actions"]
        mainViewModel["MainViewModel<br/>Bindable state and workflow orchestration"]
        converter["EnumDisplayNameConverter<br/>Presentation formatting"]
    end

    subgraph applicationProject["ChampollionGraphicalUserInterface.Application"]
        compatibility["CompatibilityRules"]
        pathValidator["LocalPathValidator"]
        outputPaths["ApplicationOutputPaths"]
        settingsStore["AppSettingsStore"]
        searchService["ExecutableSearchService"]
        classifier["ChampollionExecutableClassifier"]
        runner["ChampollionRunner"]
        commandBuilder["ChampollionCommandBuilder"]
        logWriter["DiagnosticLogWriter"]
        contracts["Input and output DTOs<br/>Settings, validation, search,<br/>execution progress, and results"]
    end

    subgraph domainProject["ChampollionGraphicalUserInterface.Domain"]
        domainModels["ChampollionRequest and DecompilationOptions"]
        domainEnums["ChampollionEdition, ChampollionOperation,<br/>and SupportedGame"]
    end

    cli["Selected Legacy or Current<br/>Champollion.exe"]
    storage[("Local fixed-drive storage<br/>PEX, generated output, executable,<br/>settings, logs, and legal documents")]
    windows["Windows desktop services<br/>Pickers, File Explorer, clipboard,<br/>associated applications, and drives"]
    webView2["WebView2 Runtime"]
    nexus["Nexus Mods download pages"]

    user <-->|"Desktop interaction"| mainWindow
    app -->|"Constructs"| mainViewModel
    app -->|"Assigns DataContext"| mainWindow
    mainWindow <-->|"Compiled bindings and event calls"| mainViewModel
    mainWindow -->|"Formats game names"| converter

    mainViewModel --> compatibility
    mainViewModel --> pathValidator
    mainViewModel --> outputPaths
    mainViewModel --> settingsStore
    mainViewModel --> searchService
    mainViewModel --> runner
    mainViewModel <-->|"Creates and consumes"| contracts
    mainViewModel --> domainModels
    mainViewModel --> domainEnums

    searchService --> pathValidator
    searchService --> classifier
    searchService --> contracts
    runner --> compatibility
    runner --> pathValidator
    runner --> commandBuilder
    runner --> logWriter
    runner --> contracts
    commandBuilder --> domainModels
    compatibility --> domainModels
    compatibility --> domainEnums
    settingsStore --> contracts

    searchService <-->|"Traverses and classifies"| storage
    pathValidator <-->|"Validates local paths"| storage
    settingsStore <-->|"Loads, migrates, and saves"| storage
    logWriter -->|"Writes diagnostics"| storage
    runner -->|"Starts and captures"| cli
    cli <-->|"Reads PEX and writes output"| storage

    mainWindow <-->|"Native desktop integration"| windows
    mainViewModel -->|"Launches File Explorer"| windows
    mainWindow -->|"Hosts embedded browser"| webView2
    webView2 <-->|"HTTPS"| nexus
```

## Reading the Diagram

- Arrows represent runtime calls or data flow, not project-reference direction alone.
- `App`, `MainWindow`, and `MainViewModel` form the composition, presentation, and orchestration spine of the desktop process.
- Application services own compatibility, paths, persistence, search, command construction, execution, and diagnostics. DTOs cross between orchestration and those services without owning behavior.
- Domain types carry stable request, option, edition, operation, and game vocabulary without depending on Application or Avalonia.
- The focused [Feature Component Diagrams](components/README.md) expand each workflow with its user trigger, result flow, and relevant external dependencies.