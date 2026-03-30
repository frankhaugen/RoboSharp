# RoboSharp documentation

This folder is the home for human-oriented documentation: how the repo is organized, how to build and test it, and how the teaching pipeline fits together.

The **authoritative** rules for architecture, dependencies, and agent behavior live in the repository root as [`AGENTS.md`](../AGENTS.md). When docs and `AGENTS.md` disagree, trust `AGENTS.md`.

## Contents

| Document | Purpose |
| -------- | ------- |
| [Build and test](build.md) | SDK, restore, tests, regenerating `RoboSharp.slnx`, git hooks |
| [Repository layout](repository-layout.md) | Solution structure, projects, and dependency direction |
| [NuGet and packages](nuget.md) | Central package management, allowed packages, feed policy |
| [Architecture overview](architecture.md) | High-level pipeline and layer responsibilities (summary only) |

## Contributing to docs

Prefer short, accurate pages over long essays. Link to `AGENTS.md` for policy instead of duplicating it. Use relative links between docs files.
