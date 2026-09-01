# Domain Project UML Class Diagram

## Purpose

This diagram contains every production type in `ChampollionGraphicalUserInterface.Domain` and shows how one invocation owns its options and references the stable domain vocabulary.

## Diagram

```mermaid
classDiagram
    direction LR

    class ChampollionRequest {
        <<record>>
        +ChampollionEdition Edition
        +SupportedGame Game
        +ChampollionOperation Operation
        +string ExecutablePath
        +string? InputPath
        +DecompilationOptions Options
    }

    class DecompilationOptions {
        <<record>>
        +bool GenerateAssembly
        +string? AssemblyOutputPath
        +string? SourceOutputPath
        +bool GenerateComments
        +bool Recursive
        +bool RecreateSubdirectories
        +bool WriteHeader
        +bool Trace
        +bool NoDumpTree
        +bool DebugFunctions
        +bool NoDebugLineNumbers
        +bool Verbose
    }

    class ChampollionEdition {
        <<enumeration>>
        Legacy
        Current
    }

    class ChampollionOperation {
        <<enumeration>>
        Decompile
        Help
        Version
        PrintInformation
        PrintCompileTime
    }

    class SupportedGame {
        <<enumeration>>
        Skyrim
        SkyrimSpecialEdition
        Fallout4
        Fallout76
        Starfield
    }

    ChampollionRequest *-- "1" DecompilationOptions : contains
    ChampollionRequest --> "1" ChampollionEdition : selects
    ChampollionRequest --> "1" ChampollionOperation : requests
    ChampollionRequest --> "1" SupportedGame : targets
```

## Relationships

- `ChampollionRequest` is the aggregate request passed from GUI orchestration into Application execution.
- Each request contains exactly one `DecompilationOptions` value and one value from each domain enum.
- Domain types do not depend on Application, Avalonia, operating-system APIs, or external packages.