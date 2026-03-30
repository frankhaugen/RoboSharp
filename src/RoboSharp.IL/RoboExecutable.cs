namespace RoboSharp.IL;

/// <summary>
/// Versioned wrapper for a compiled fake executable (in-memory). On-disk <c>.roboexe</c> v1 uses JSON; see <c>RoboSharp.Toolchain</c>.
/// </summary>
public sealed class RoboExecutable
{
    public const int CurrentFormatVersion = 1;

    public required int FormatVersion { get; init; }

    public required RoboProgram Program { get; init; }

    public static RoboExecutable FromProgram(RoboProgram program, int formatVersion = CurrentFormatVersion) =>
        new()
        {
            FormatVersion = formatVersion,
            Program = program,
        };
}
