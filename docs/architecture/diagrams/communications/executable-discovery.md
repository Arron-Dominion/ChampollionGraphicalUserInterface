# Executable Discovery Communication Diagram

## Purpose

This diagram shows the collaborating objects used by automatic executable discovery, including progress callbacks and guarded success, exhaustion, and cancellation paths.

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
flowchart TB
    user(["User"])
    window["window : MainWindow"]
    viewModel["viewModel : MainViewModel"]
    search["search : ExecutableSearchService"]
    fileSystem["fileSystem : Local Fixed Drives"]
    validator["validator : LocalPathValidator"]
    classifier["classifier : ChampollionExecutableClassifier"]
    store["store : AppSettingsStore"]
    pathDecision{"Candidate path valid?"}
    editionDecision{"Requested edition?"}
    outcomeDecision{"Search outcome?"}

    user -->|"1: select Auto search"| window
    window -->|"1.1: SearchCommand.Execute()"| viewModel
    viewModel -->|"1.1.1: preserve path and create<br/>CancellationTokenSource"| viewModel
    viewModel -->|"1.1.2: FindAsync(edition, progress, token)"| search
    search -->|"1.1.2.1: collect search roots"| fileSystem
    search -->|"1.1.2.2 *[queued directory]:<br/>check Champollion.exe candidate"| fileSystem
    search -.->|"1.1.2.3 *[worker update]: SearchProgress"| viewModel
    viewModel -.->|"1.1.2.4: worker and directory status"| window

    search -->|"1.1.2.5 *[candidate]:<br/>ValidateExecutable(candidate)"| validator
    validator -->|"1.1.2.5.1: inspect drive and file metadata"| fileSystem
    validator -.->|"1.1.2.5.2: PathValidationResult"| pathDecision
    pathDecision -->|"[invalid]: inspect next candidate"| search
    pathDecision -->|"[valid] 1.1.2.6: Classify()"| classifier
    classifier -->|"1.1.2.6.1: inspect markers and version"| fileSystem
    classifier -.->|"1.1.2.6.2: classification"| editionDecision
    editionDecision -->|"[wrong or unknown]: inspect next candidate"| search
    editionDecision -->|"[matching]: first path"| outcomeDecision
    search -->|"[roots exhausted]: null"| outcomeDecision

    outcomeDecision -.->|"1.1.3a [matching]:<br/>cancel and await other workers"| viewModel
    viewModel -->|"1.1.3a.1: SaveAsync(updated settings)"| store
    store -->|"1.1.3a.1.1: replace settings.json"| fileSystem
    viewModel -.->|"1.1.3a.2: found-and-saved status"| window
    outcomeDecision -.->|"1.1.3b [exhausted]: null"| viewModel
    viewModel -->|"1.1.3b.1: restore previous path"| viewModel
    viewModel -.->|"1.1.3b.2: no-result status"| window

    user -->|"2 [search active]: select Cancel search"| window
    window -->|"2.1: CancelSearchCommand.Execute()"| viewModel
    viewModel -->|"2.1.1: cancel token"| search
    search -.->|"2.1.2: OperationCanceledException"| viewModel
    viewModel -->|"2.1.3: restore path, dispose token,<br/>and clear IsSearching"| viewModel
    viewModel -.->|"2.1.4: cancelled status"| window
```

## Collaboration Notes

- `ExecutableSearchService` owns the bounded worker coordination even though the worker tasks are not drawn as separate public objects.
- The first candidate that is both path-valid and classified for the requested edition wins; other workers are cancelled and awaited.
- Only a successful result is saved. Exhaustion and cancellation restore the previously configured executable path.