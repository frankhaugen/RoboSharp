# Technology stack (Studio host)

Concrete choices for **RoboSharpStudio**: **Avalonia**, **Microsoft.Extensions.DependencyInjection**, and a **code-first** UI mindset. Avalonia supports `Microsoft.Extensions.DependencyInjection` and can be built entirely in C# without XAML, though XAML remains available. See [Implementing dependency injection | Avalonia Docs](https://docs.avaloniaui.net/docs/app-development/dependency-injection).

## UI framework

Use **Avalonia** as the desktop UI shell.

**AvaloniaEdit** (the main code buffer) needs its **Fluent** theme merged into `Application.Styles` — e.g. `StyleInclude` for `avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml` alongside `FluentTheme`. Without it, the editor can appear empty. This is wired in [`StudioApp.cs`](../../src/RoboSharp.Studio/Shell/StudioApp.cs).

Reasons:

- better fit for code-first UI than WPF
- cross-platform option preserved
- works naturally with `Microsoft.Extensions.DependencyInjection` patterns in official guidance

## DI container

Use only:

- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Hosting`
- `Microsoft.Extensions.Logging`
- optionally `Microsoft.Extensions.Options`

No Autofac, DryIoc, Prism, ReactiveUI container, or framework-owned service locator.

## UI declaration rule

- **code-first views by default**
- **no required XAML**
- XAML may be tolerated in isolated cases only if later proven materially better

Spec the first version as entirely C#-built UI.

See [composition-and-domain.md](composition-and-domain.md) for registration patterns.
