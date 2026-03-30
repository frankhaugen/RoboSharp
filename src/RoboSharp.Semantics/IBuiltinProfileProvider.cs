namespace RoboSharp.Semantics;

public interface IBuiltinProfileProvider
{
    bool IsAvailable(BuiltinId id);
}
