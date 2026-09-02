# Unified Level 1 GUI Data Flow Diagram

## Purpose

This Level 1 DFD decomposes the GUI into its five major data-processing responsibilities and shows their shared stores and external boundaries.

## Legend

```mermaid
flowchart LR
    external["External<br/>Outside the GUI"]
    process(["Process<br/>Transforms data"])
    store[("Data Store<br/>Holds data")]

    external -->|"Data Flow<br/>Transfers data"| process
    process -->|"Data Flow<br/>Transfers data"| store
```

## Diagram

```mermaid
flowchart TB
    e1["E1 User"]
    e2["E2 Champollion CLI"]
    e3["E3 Windows"]
    e4["E4 WebView2"]
    e5["E5 Nexus"]

    p1(["P1 GUI State"])
    p2(["P2 Path/CLI Discovery"])
    p3(["P3 Execute Request"])
    p4(["P4 Output/Desktop"])
    p5(["P5 Help/About"])

    d0[("D0 Legacy AppData")]
    d1[("D1 Settings")]
    d2[("D2 Drive Metadata")]
    d3[("D3 Champollion Distribution")]
    d4[("D4 PEX Inputs")]
    d5[("D5 Generated Output")]
    d6[("D6 Logs")]
    d7[("D7 Legal Documents")]
    d8[("D8 WebView2 Profile")]

    e1 -->|"SEL"| p1
    d0 -->|"LEG"| p1
    p1 -->|"CFG"| d1
    p1 -->|"MIG"| d6
    d1 -->|"SET"| p1
    p1 -->|"REQ"| p2
    p1 -->|"CFG"| p3
    p1 -->|"CTL"| e1

    e1 -->|"PATH"| p2
    d2 -->|"META"| p2
    d3 -->|"EXE/META"| p2
    p2 -->|"VAL"| e1
    p2 -->|"EXE"| p1
    p2 -->|"PATH"| p3

    e1 -->|"RUN"| p3
    d4 -->|"PEX"| p3
    d3 -->|"EXE"| p3
    p3 -->|"ARG"| e2
    d4 -->|"BYTES"| e2
    e2 -->|"GEN"| d5
    e2 -->|"IO"| p3
    p3 -->|"LOG"| d6
    p3 -->|"RES"| p4

    p4 -->|"OUT"| e1
    e1 -->|"CMD"| p4
    d1 -->|"SET"| p4
    d5 -->|"OUT"| p4
    d6 -->|"LOG"| p4
    p4 -->|"SHELL"| e3

    e1 -->|"HELP"| p5
    d7 -->|"DOC"| p5
    p5 -->|"DOC"| e3
    p5 -->|"URI"| e4
    e4 -->|"HTTPS"| e5
    e5 -->|"HTML"| e4
    e4 -->|"PAGE"| p5
    p5 -->|"CFG"| d8
    d8 -->|"STATE"| p5
    p5 -->|"VIEW"| e1
```

## Unified Flow Key

| Code | Data exchanged |
| --- | --- |
| `SEL` | Edition, game, operation, options, paths, commands, and confirmations |
| `CFG` | Current profiles, configured request data, or WebView2 profile configuration |
| `REQ` / `PATH` | Transient request selections or validated path data |
| `EXE` / `ARG` | Executable paths or structured CLI arguments |
| `PEX` / `BYTES` / `GEN` | Resolved PEX paths, PEX bytes, or generated files |
| `IO` / `RES` / `LOG` | CLI streams and exit code, execution results, or diagnostic data |
| `OUT` / `CMD` / `SHELL` | Presented output, desktop commands, or Windows shell paths |
| `HELP` / `DOC` / `VIEW` | Help or legal commands, document paths, or Help content/status |
| `URI` / `HTTPS` / `HTML` / `PAGE` | Download URI, request, page content, or rendered content |
| `STATE` | Cookies and browser state |

## Process Traceability

| Unified ID | Detailed diagram | Detailed subprocesses |
| --- | --- | --- |
| `P1` | [configuration-and-settings.md](configuration-and-settings.md) | `1.0`, `1.1` |
| `P2` | [path-validation-and-executable-discovery.md](path-validation-and-executable-discovery.md) | `2.0`, `2.1`, `2.2` |
| `P3` | [execution-and-diagnostics.md](execution-and-diagnostics.md) | `3.0`, `3.1` |
| `P4` | [output-and-desktop-actions.md](output-and-desktop-actions.md) | `4.0` |
| `P5` | [help-and-about.md](help-and-about.md) | `5.0` |

## Level 1 Scope

- Processes `1.0` through `5.0` are logical responsibilities inside the single GUI executable, not separately deployed services.
- `D0` is migration input only. Current settings and logs live beside the application under `UserData`.
- `D2` represents metadata read during validation and traversal; `D3` represents the selected third-party executable distribution.
- `D4` and `D5` remain separate because PEX inputs are read while Papyrus source and optional assembly are generated.
- Transient paths, status, progress, output text, and the latest log path move between GUI processes in memory and are not persistent stores.