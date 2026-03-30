namespace RoboSharp.Runtime;

internal enum EvalKind
{
    Int,
    Bool,
    Double,
    String,
    Array,
}

internal readonly record struct EvalSlot(
    EvalKind Kind,
    int IntValue = 0,
    double DoubleValue = 0,
    string? Text = null,
    int ArrayId = 0)
{
    public static EvalSlot Int(int v) => new(EvalKind.Int, v);
    public static EvalSlot Bool(bool v) => new(EvalKind.Bool, v ? 1 : 0);
    public static EvalSlot Double(double v) => new(EvalKind.Double, DoubleValue: v);
    public static EvalSlot String(string? v) => new(EvalKind.String, Text: v);
    public static EvalSlot Array(int id) => new(EvalKind.Array, ArrayId: id);
}
