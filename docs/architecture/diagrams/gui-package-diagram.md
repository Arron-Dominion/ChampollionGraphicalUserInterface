# GUI Package Diagram

## Purpose

This diagram shows the compile-time package and namespace dependencies of the `ChampollionGraphicalUserInterface` desktop project. Unlike the component diagrams, its arrows mean "imports or references" rather than runtime control or data flow.

See the [Summary GUI Package Diagram](summary-gui-package-diagram.md) for the same project boundary with internal packages collapsed.

```mermaid
flowchart LR
    subgraph guiProject["ChampollionGraphicalUserInterface project"]
        root["Root package<br/>Program, App, and ViewLocator<br/><br/>Desktop startup, composition,<br/>application resources, and view resolution"]
        views["Views package<br/>MainWindow XAML and code-behind<br/><br/>Controls, dialogs, pickers, clipboard,<br/>File Explorer, WebView, and legal documents"]
        viewModels["ViewModels package<br/>MainViewModel and ViewModelBase<br/><br/>Bindable state, commands,<br/>and GUI workflow orchestration"]
        converters["Converters package<br/>EnumDisplayNameConverter<br/><br/>Presentation-only value conversion"]
        assets["Assets and packaged resources<br/>Fonts, license, notices,<br/>manifest, and theme resources"]
    end

    subgraph applicationProject["ChampollionGraphicalUserInterface.Application project"]
        appExecution["Execution and CommandLine<br/>Runner, logging, and argument construction"]
        appSearch["Search<br/>Executable traversal and classification"]
        appSettings["Settings<br/>Configuration persistence and migration"]
        appValidation["Validation<br/>Compatibility and local-path rules"]
        appPaths["Paths<br/>Application-owned output defaults"]
        appContracts["DTO and Enum<br/>Directional contracts and Application enums"]
    end

    domain["ChampollionGraphicalUserInterface.Domain<br/>Models and Enums<br/><br/>Requests, options, editions,<br/>operations, and supported games"]

    subgraph externalPackages["NuGet and framework packages"]
        avalonia["Avalonia and Avalonia.Desktop<br/>Application lifetime, controls,<br/>storage, clipboard, XAML, and themes"]
        webView["Avalonia.Controls.WebView<br/>NativeWebView abstraction"]
        toolkit["CommunityToolkit.Mvvm<br/>Observable state and relay commands"]
        diagnostics["AvaloniaUI.DiagnosticsSupport<br/>Debug developer tools"]
    end

    root -->|"Composes and assigns"| views
    root -->|"Constructs"| viewModels
    root -->|"Creates services"| appExecution
    root -->|"Creates services"| appSearch
    root -->|"Creates services"| appSettings
    root -->|"Creates services"| appValidation
    root -->|"Gets output directories"| appPaths
    root -->|"Uses desktop lifetime and XAML"| avalonia
    root -->|"Enables in Debug"| diagnostics

    views -->|"Binds and delegates"| viewModels
    views -->|"Uses XAML converter"| converters
    views -->|"Uses controls and platform APIs"| avalonia
    views -->|"Hosts embedded browser"| webView
    views -->|"Loads packaged content"| assets

    viewModels -->|"Runs workflows"| appExecution
    viewModels -->|"Starts and cancels discovery"| appSearch
    viewModels -->|"Loads and saves profiles"| appSettings
    viewModels -->|"Evaluates paths and compatibility"| appValidation
    viewModels -->|"Resets output defaults"| appPaths
    viewModels -->|"Creates and consumes contracts"| appContracts
    viewModels -->|"Uses business vocabulary"| domain
    viewModels -->|"Uses generated observable members"| toolkit

    converters -->|"Implements IValueConverter"| avalonia

    appExecution --> appValidation
    appExecution --> appContracts
    appExecution --> domain
    appSearch --> appValidation
    appSearch --> appContracts
    appSearch --> domain
    appSettings --> appContracts
    appSettings --> domain
    appValidation --> appContracts
    appValidation --> domain
```

## Package Responsibilities

| Package | Responsibility |
| --- | --- |
| Root | Starts Avalonia, constructs Application services and `MainViewModel`, creates `MainWindow`, registers application resources, and resolves view models to views. |
| `Views` | Owns Avalonia controls and platform-specific presentation interactions. It delegates workflow state and Application calls through `MainViewModel`. |
| `ViewModels` | Owns bindable GUI state, commands, selection transitions, and workflow orchestration. It is the primary GUI consumer of Application and Domain. |
| `Converters` | Contains presentation-only conversion used by XAML bindings. |
| Assets and packaged resources | Supplies static application resources and files copied to build or publish output; it is not a C# namespace. |

## Dependency Rules

- The GUI project references both Application and Domain. Application references Domain; Domain has no project dependencies.
- `Views` depends on `ViewModels`, but view models do not depend on views or Avalonia controls.
- Platform-specific pickers, clipboard, WebView, dialogs, and shell behavior remain in `Views` or the GUI root package.
- Validation, persistence, executable search, command construction, process execution, and diagnostic logging remain in Application packages.
- The physical `Models` folder currently declared by the GUI project is empty, so it is not shown as a logical package.