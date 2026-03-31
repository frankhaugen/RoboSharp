namespace RoboSharp.Language;

public readonly record struct TextSpan(int Start, int Length)
{
    public static TextSpan Invalid { get; } = new(-1, 0);

    public int End => Start + Length;

    /// <summary>Usable for source stepping / diagnostics (empty but real spans use Start ≥ 0).</summary>
    public bool IsValid => Start >= 0;
}
