# Copilot Skills

This directory contains human-readable documentation for the repository's GitHub Copilot skills. The executable skill definitions remain under `.github/skills` so Visual Studio Code and GitHub Copilot can discover them.

## Available Skills

| Skill | Purpose | Documentation | Definition |
| --- | --- | --- | --- |
| `champollion-development` | Guides implementation, refactoring, and fixes using the repository's architecture, documentation, testing, and validation conventions. | [Usage and maintenance guide](champollion-development.md) | [SKILL.md](../../../.github/skills/champollion-development/SKILL.md) |
| `champollion-code-review` | Reviews code and configuration changes for actionable defects and audits architecture diagrams and root README claims against repository evidence. | [Usage and maintenance guide](champollion-code-review-maintenance.md) | [SKILL.md](../../../.github/skills/champollion-code-review/SKILL.md) |
| `champollion-diagrams` | Creates and maintains source-grounded architecture diagrams, notation, focused views, unified views, and diagram indexes. | [Usage and maintenance guide](champollion-diagrams.md) | [SKILL.md](../../../.github/skills/champollion-diagrams/SKILL.md) |
| `champollion-readme` | Maintains the root README as a concise, source-grounded entry point for users and contributors. | [Usage and maintenance guide](champollion-readme.md) | [SKILL.md](../../../.github/skills/champollion-readme/SKILL.md) |

## Documentation Policy

Each repository skill should have a guide in this directory that explains:

- when and how people should use it;
- what behavior and repository conventions it protects;
- where its source definition lives;
- which repository changes require maintenance;
- how to validate the skill after editing it.

Keep these guides readable for contributors. Put concise operational instructions used by Copilot in the corresponding `SKILL.md` file, and keep the human guide focused on intent, examples, maintenance, and troubleshooting.
