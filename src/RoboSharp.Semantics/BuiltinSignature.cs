namespace RoboSharp.Semantics;

public sealed record BuiltinSignature(BuiltinId Id, TypeSymbol ReturnType, IReadOnlyList<TypeSymbol> ParameterTypes);