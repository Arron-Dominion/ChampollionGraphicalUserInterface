# Summary GUI Package Diagram

## Purpose

This diagram summarizes compile-time dependencies with each repository project collapsed to one package. External dependencies are grouped together and identified by package name only.

```mermaid
flowchart LR
    subgraph repository["GUI solution packages"]
        gui["ChampollionGraphicalUserInterface"]
        application["ChampollionGraphicalUserInterface.Application"]
        domain["ChampollionGraphicalUserInterface.Domain"]
    end

    subgraph external["External dependencies"]
        avalonia["Avalonia"]
        avaloniaDesktop["Avalonia.Desktop"]
        avaloniaWebView["Avalonia.Controls.WebView"]
        avaloniaTheme["Avalonia.Themes.Fluent"]
        avaloniaFonts["Avalonia.Fonts.Inter"]
        avaloniaDiagnostics["AvaloniaUI.DiagnosticsSupport"]
        communityToolkit["CommunityToolkit.Mvvm"]
    end

    gui --> application
    gui --> domain
    application --> domain

    gui --> avalonia
    gui --> avaloniaDesktop
    gui --> avaloniaWebView
    gui --> avaloniaTheme
    gui --> avaloniaFonts
    gui --> avaloniaDiagnostics
    gui --> communityToolkit
```

## Scope

- Each repository package represents one production project; internal namespaces and implementation types are intentionally omitted.
- Arrows represent direct project or NuGet package references declared by the production project files.
- Only direct external dependencies of the GUI project are included. Framework-provided assemblies and transitive packages are outside this summary.
- See the [GUI Package Diagram](gui-package-diagram.md) for internal GUI and Application package details.
- See the [Layered UML Package Diagram](uml-gui-package-diagram.md) for the same collapsed scope organized by architectural layer.