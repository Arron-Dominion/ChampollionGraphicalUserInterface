# Copilot Agents

This directory contains human-readable documentation for the repository's GitHub Copilot custom agents. Active agent definitions remain under `.github/agents` so Visual Studio Code and GitHub Copilot can discover them.

## Available Agents

| Agent | Purpose | Documentation | Definition |
| --- | --- | --- | --- |
| `Champollion Architect` | Designs new features before implementation and produces private proposed diagrams, identified changes, rationale, and implementation slices under ignored `Feature/Design`. | [Usage and maintenance guide](champollion-architect.md) | [Agent definition](../../../.github/agents/champollion-architect.agent.md) |
| `Champollion .NET Platform Evaluator` | Evaluates .NET upgrades, current Windows x64 readiness, proposed target feasibility, platform-specific dependencies, replacement boundaries, publishing, packaging, and CI. | [Usage and maintenance guide](champollion-dotnet-platform-evaluator.md) | [Agent definition](../../../.github/agents/champollion-dotnet-platform-evaluator.agent.md) |

## Documentation Policy

Each repository agent should have a guide in this directory that explains:

- when and how people should use it;
- its supported scope and operational limits;
- which repository writes it may make;
- required reports or other durable outputs;
- which repository changes require maintenance;
- how to validate the agent after editing it.

Keep operational instructions required by Copilot in the corresponding `.agent.md` file. Keep these guides focused on contributor usage, maintenance, report interpretation, validation, and troubleshooting.
