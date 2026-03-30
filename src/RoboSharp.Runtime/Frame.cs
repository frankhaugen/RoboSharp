namespace RoboSharp.Runtime;

internal sealed class Frame
{
    public Frame(int functionIndex, int returnIp, EvalSlot[] locals)
    {
        FunctionIndex = functionIndex;
        ReturnIp = returnIp;
        Locals = locals;
    }

    public int FunctionIndex { get; }
    public int ReturnIp { get; }
    public EvalSlot[] Locals { get; }
    public int Ip { get; set; }
}
