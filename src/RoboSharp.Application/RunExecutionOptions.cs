namespace RoboSharp.Application;

/// <summary>Optional limits when running lowered IL (Player, tests, hosts).</summary>
public sealed record RunExecutionOptions
{
    /// <summary>When set (positive), run at most this many instructions (session <c>RunToEnd</c>); otherwise run to completion with the standard interpreter.</summary>
    public int? MaxInstructions { get; init; }
}
