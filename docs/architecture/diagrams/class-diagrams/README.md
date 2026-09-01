# UML Class Diagrams

These diagrams show the hand-written production types owned by each project and the relationships visible in source code. Generated CommunityToolkit members, XAML-generated partial members, compiler-generated record members, and assembly metadata are omitted.

## Relationship Notation

| UML relationship | Mermaid form | Meaning in these diagrams |
| --- | --- | --- |
| Generalization | `<|--` | A class inherits from a base class. |
| Realization | `<|..` | A class implements an interface. |
| Composition | `*--` | The owner contains data whose lifetime is part of the owner model. |
| Aggregation | `o--` | A constructor-injected or retained collaborator is shared independently. |
| Dependency | `..>` | A type calls, creates, accepts, returns, or otherwise uses another type. |
| Association | `-->` | A property or record field is typed by another model or enum. |

Members shown are intentionally selective for behavior-heavy classes and complete enough to identify data contracts. Relationship labels describe the evidence for each connection.

## Projects

- [GUI Project Class Diagram](gui-project-class-diagram.md): Avalonia startup, views, view models, converter, view locator, and their Application/Domain collaborators.
- [Application Project Class Diagram](application-project-class-diagram.md): Validation, execution, command construction, search, settings, paths, DTOs, and the Application enum.
- [Domain Project Class Diagram](domain-project-class-diagram.md): Request and option records plus edition, operation, and game enums.

The project-level dependency direction remains GUI to Application and Domain, and Application to Domain. See the [Layered UML Package Diagram](../uml-gui-package-diagram.md) for that collapsed view.