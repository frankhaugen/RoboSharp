namespace RoboSharp.IL;

public readonly record struct Instruction(RoboOpcode Op, int A = 0, int B = 0, int C = 0);
