using System.Collections.Immutable;

namespace RoboSharp.Semantics;

/// <summary>Profile that exposes only a fixed set of <see cref="BuiltinId"/> values (lesson gating).</summary>
public sealed class SelectingBuiltinProfileProvider : IBuiltinProfileProvider
{
    private readonly ImmutableHashSet<BuiltinId> _allowed;

    public SelectingBuiltinProfileProvider(IEnumerable<BuiltinId> allowed) =>
        _allowed = allowed.ToImmutableHashSet();

    public bool IsAvailable(BuiltinId id) => _allowed.Contains(id);
}
