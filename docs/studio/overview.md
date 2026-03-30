# RoboSharp Studio — purpose

`RoboSharpStudio` is the desktop IDE for the RoboSharp ecosystem.

It exists to let a learner or teacher:

- open a RoboSharp workspace/project
- edit `.robo` source files
- inspect tokens, syntax tree, semantic model, IL, runtime state, stdout, stderr, and world state
- compile to `.roboexe`
- debug via a deterministic step debugger
- author or preview lesson/world content
- do all of that without coupling the application to one rendering technology or one storage mode

The Studio is not “the runtime with panels.” It is a host shell over:

- workspace services
- compiler services
- runtime/debugger services
- rendering projection services
- lesson/profile services
- IDE state services

That separation is non-negotiable.

## Summary position

The intended Studio architecture is:

- **Avalonia shell**
- **built-in .NET DI** via Generic Host / `ServiceCollection`
- **code-first UI**
- **workspace over filesystem abstractions**
- **compiler/runtime/debugger as separate libraries**
- **snapshot-based debug visualization**
- **layered world rendering**
- **stdout/stderr separation**
- **lesson/profile-aware editing experience**

That matches the language philosophy: explicit, inspectable, deterministic, modular, teachable.

See also: [technology-stack.md](technology-stack.md), [composition-and-domain.md](composition-and-domain.md), [scope-mvp-and-non-goals.md](scope-mvp-and-non-goals.md).
