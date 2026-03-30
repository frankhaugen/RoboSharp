namespace RoboSharp.Runtime;

public sealed class RuntimeFault
{
    public RuntimeFault(string message, int functionIndex, int instructionPointer)
    {
        Message = message;
        FunctionIndex = functionIndex;
        InstructionPointer = instructionPointer;
    }

    public string Message { get; }
    public int FunctionIndex { get; }
    public int InstructionPointer { get; }
}
