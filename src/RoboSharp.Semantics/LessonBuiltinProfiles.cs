namespace RoboSharp.Semantics;

/// <summary>Named lesson profiles for kid-friendly progression — each is a subset of <see cref="BuiltinCatalog"/>.</summary>
public static class LessonBuiltinProfiles
{
    public const string FullId = "full";
    public const string BasicMovementId = "basic-movement";
    public const string MovementAndTurnsId = "movement-turns";
    public const string MovementAndPrintId = "movement-print";
    public const string WithSensingId = "with-sensing";

    private static readonly SelectingBuiltinProfileProvider Full = new(Enum.GetValues<BuiltinId>());
    private static readonly SelectingBuiltinProfileProvider BasicMovement = new([BuiltinId.Move, BuiltinId.TurnLeft]);
    private static readonly SelectingBuiltinProfileProvider MovementTurns = new([BuiltinId.Move, BuiltinId.TurnLeft, BuiltinId.TurnRight]);
    private static readonly SelectingBuiltinProfileProvider MovementPrint =
        new([BuiltinId.Move, BuiltinId.TurnLeft, BuiltinId.TurnRight, BuiltinId.Print]);
    private static readonly SelectingBuiltinProfileProvider WithSensing =
        new([
            BuiltinId.Move, BuiltinId.TurnLeft, BuiltinId.TurnRight, BuiltinId.Print,
            BuiltinId.FrontIsClear, BuiltinId.LeftIsClear, BuiltinId.RightIsClear,
        ]);

    private static readonly Dictionary<string, (IBuiltinProfileProvider Provider, string DisplayName, string Blurb)> Registry =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [FullId] = (Full, "Full toolbox", "Everything: move, turn, print, sensing, pick/drop, arrays."),
            [BasicMovementId] = (BasicMovement, "Starter: move & turn left", "Just move() and turnLeft() — smallest first step."),
            [MovementAndTurnsId] = (MovementTurns, "Movement + both turns", "move(), turnLeft(), turnRight() — practice steering."),
            [MovementAndPrintId] = (MovementPrint, "Movement + print", "Add print() to show numbers and messages."),
            [WithSensingId] = (WithSensing, "Sensing walls", "Includes frontIsClear(), leftIsClear(), rightIsClear() for decisions."),
        };

    /// <summary>Returns the provider for <paramref name="profileId"/> or the full profile if unknown.</summary>
    public static IBuiltinProfileProvider GetProvider(string profileId) =>
        Registry.TryGetValue(profileId, out var e) ? e.Provider : Full;

    public static string GetDisplayName(string profileId) =>
        Registry.TryGetValue(profileId, out var e) ? e.DisplayName : "Full toolbox";

    public static string GetBlurb(string profileId) =>
        Registry.TryGetValue(profileId, out var e) ? e.Blurb : Registry[FullId].Blurb;

    /// <summary>Stable ids for combo boxes (order = suggested teaching order).</summary>
    public static IReadOnlyList<string> OrderedProfileIds { get; } =
        [BasicMovementId, MovementAndTurnsId, MovementAndPrintId, WithSensingId, FullId];

    public static string DescribeBuiltinsForHelp(IBuiltinProfileProvider profile)
    {
        var names = BuiltinCatalog.AllBuiltinNames.Where(n => BuiltinCatalog.TryGet(n, out var sig) && profile.IsAvailable(sig.Id)).ToList();
        names.Sort(StringComparer.Ordinal);
        return string.Join("\r\n", names.Select(static n => $"  • {n}()"));
    }
}
