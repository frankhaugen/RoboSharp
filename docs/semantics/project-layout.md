# Suggested layout — `RoboSharp.Semantics`

```text
RoboSharp.Semantics/
  Symbols/
    Symbol.cs
    TypeSymbol.cs
    VariableSymbol.cs
    ParameterSymbol.cs
    FunctionSymbol.cs

  Binding/
    Scope.cs
    Binder.cs
    BoundNode.cs
    BoundStatement.cs
    BoundExpression.cs
    ...

  Diagnostics/
    Diagnostic.cs
    DiagnosticBag.cs

  Builtins/
    BuiltinId.cs
    BuiltinFunctionDefinition.cs
    BuiltinFunctionProfile.cs
```

Language layout: [../language/project-layout.md](../language/project-layout.md).
