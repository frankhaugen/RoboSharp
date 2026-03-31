namespace RoboSharp.Semantics;

public sealed class FullBuiltinProfileProvider : IBuiltinProfileProvider
{
    public bool IsAvailable(BuiltinId id) => true;
}