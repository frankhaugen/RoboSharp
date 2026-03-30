namespace RoboSharp.IL;

/// <summary>Teaching IL opcodes executed by <c>RoboSharp.Runtime</c>.</summary>
public enum RoboOpcode : byte
{
    Nop,
    Pop,
    Dup,

    PushInt,
    PushBool,
    PushNumber,
    PushString,

    LoadLocal,
    StoreLocal,

    Add,
    Sub,
    Mul,
    Div,
    Neg,

    EqInt,
    NeInt,
    Lt,
    Le,
    Gt,
    Ge,

    And,
    Or,
    Not,

    Jump,
    JumpIfFalse,
    JumpIfTrue,

    Call,
    CallBuiltin,
    Return,

    NewIntArray,
    ArrayLen,
    ArrayGet,
    ArraySet,

    ConvNumberToInt,
}
