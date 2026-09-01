# Architecture Diagrams

Visual descriptions of Champollion Graphical User Interface and its relationships.

- [System Context Diagram](system-context.md): Users, the application boundary, external executables, storage, web content, and Windows platform services.
- [Container Diagram](container-diagram.md): The desktop GUI process, external Champollion CLI process, local storage, and supporting platform integrations.
- [Unified Component Diagram](unified-component-diagram.md): All major GUI, Application, and Domain components and their external dependencies in one view.
- [Feature Component Diagrams](components/README.md): Focused component views for individual GUI workflows.
- [GUI Package Diagram](gui-package-diagram.md): Compile-time dependencies among GUI namespaces, referenced projects, and UI framework packages.
- [Summary GUI Package Diagram](summary-gui-package-diagram.md): Collapsed project packages and grouped direct external package dependencies.
- [Layered UML Package Diagram](uml-gui-package-diagram.md): Collapsed project packages separated into Presentation, Application, Domain, and External Dependency layers.
- [Deployment Diagrams](deployments/README.md): GitHub Actions packaging, GitHub Release publication, and manual distribution to mod websites.
- [Sequence Diagrams](sequences/README.md): Ordered interactions for startup, configuration, discovery, execution, desktop actions, and Help workflows.
- [Data Flow Diagrams](data-flows/README.md): Level 0, unified Level 1, and focused views of GUI data movement and persistence.
- [Security Diagrams](security/README.md): Trust boundaries, implemented controls, local data exposure, external integrations, and release provenance.
- [UML Class Diagrams](class-diagrams/README.md): Per-project classes, records, enums, inheritance, ownership, and typed dependencies.
- [Communication Diagrams](communications/README.md): Object collaborations and numbered messages for focused and unified GUI workflows.