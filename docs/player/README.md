# RoboSharp.Player

`RoboSharp.Player` is the **compiled-artifact runtime host**: a thin entry point over the same application/runtime concepts as other hosts. Policy and layering: [`AGENTS.md`](../../AGENTS.md).

## Lesson mode (direction)

In addition to running a bare `.roboexe`, the Player should be able to run in a **lesson context**:

| Mode | Behavior |
| ---- | -------- |
| **Free run** | Execute a `.roboexe` (or equivalent) without lesson metadata |
| **Lesson run** | Load lesson definition (profile, world, goals), run the program, evaluate goals, report pass/fail |

That supports classroom demos, headless checks, and parity with Studio’s teaching loop.

Details of goals, packs, and JSON shape: [../lessons/README.md](../lessons/README.md).
