using RoboSharp.IL;
using RoboSharp.Semantics;
using RoboSharp.World;

namespace RoboSharp.Runtime;

/// <summary>Shared execution engine for <see cref="RoboInterpreter"/> and <see cref="RoboInterpreterSession"/>.</summary>
internal sealed class RoboInterpreterEngine
{
    private readonly List<EvalSlot> _stack = new();
    private readonly List<Frame> _frames = new();
    private readonly Dictionary<int, List<int>> _arrays = new();
    private int _nextArrayId = 1;
    private TextWriter _stdout = TextWriter.Null;
    private TextWriter _stderr = TextWriter.Null;
    private RobotWorld? _world;

    public int? CurrentFunctionIndex => _frames.Count > 0 ? _frames[^1].FunctionIndex : null;

    public int? CurrentInstructionPointer => _frames.Count > 0 ? _frames[^1].Ip : null;

    public bool HasActiveFrames => _frames.Count > 0;

    public ExecutionResult? Initialize(RoboProgram program, RobotWorld world, TextWriter stdout, TextWriter stderr)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(world);
        _stdout = stdout;
        _stderr = stderr;
        _world = world;
        _stack.Clear();
        _frames.Clear();
        _arrays.Clear();
        _nextArrayId = 1;

        if (program.Functions.Count == 0)
            return ExecutionResult.Failed(new RuntimeFault("Program has no functions.", -1, -1));

        var entry = program.EntryFunctionIndex;
        if (entry < 0 || entry >= program.Functions.Count)
            return ExecutionResult.Failed(new RuntimeFault("Invalid entry function index.", -1, -1));

        PushFrame(program, entry, returnIp: -1, argumentCount: 0);
        return null;
    }

    /// <summary>Execute one instruction. Returns <see langword="null"/> if execution should continue, <see cref="ExecutionResult"/> if faulted, or <see cref="ExecutionResult.Completed"/> when the call stack is empty.</summary>
    public ExecutionResult? ExecuteNext(RoboProgram program)
    {
        if (_frames.Count == 0)
            return ExecutionResult.Completed;

        var frame = _frames[^1];
        var fn = program.Functions[frame.FunctionIndex];
        if (frame.Ip < 0 || frame.Ip >= fn.Instructions.Count)
            return ExecutionResult.Failed(new RuntimeFault("Instruction pointer out of range (missing return?).", frame.FunctionIndex, frame.Ip));

        var insn = fn.Instructions[frame.Ip];
        frame.Ip++;

        try
        {
            ExecuteInstruction(program, fn, insn);
        }
        catch (InvalidOperationException ex)
        {
            return ExecutionResult.Failed(new RuntimeFault(ex.Message, frame.FunctionIndex, frame.Ip - 1));
        }

        return _frames.Count == 0 ? ExecutionResult.Completed : null;
    }

    private void ExecuteInstruction(RoboProgram program, CompiledFunction fn, Instruction insn)
    {
        var frame = _frames[^1];

        switch (insn.Op)
        {
            case RoboOpcode.Nop:
                break;
            case RoboOpcode.Pop:
                Pop();
                break;
            case RoboOpcode.Dup:
                Push(Peek());
                break;
            case RoboOpcode.PushInt:
                Push(EvalSlot.Int(insn.A));
                break;
            case RoboOpcode.PushBool:
                Push(EvalSlot.Bool(insn.A != 0));
                break;
            case RoboOpcode.PushNumber:
                Push(EvalSlot.Double(program.NumberTable[insn.A]));
                break;
            case RoboOpcode.PushString:
                Push(EvalSlot.String(program.StringTable[insn.A]));
                break;
            case RoboOpcode.LoadLocal:
                Push(frame.Locals[insn.A]);
                break;
            case RoboOpcode.StoreLocal:
                frame.Locals[insn.A] = Pop();
                break;
            case RoboOpcode.Add:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Int(a + b));
                break;
            }
            case RoboOpcode.Sub:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Int(a - b));
                break;
            }
            case RoboOpcode.Mul:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Int(a * b));
                break;
            }
            case RoboOpcode.Div:
            {
                var b = PopInt();
                var a = PopInt();
                if (b == 0)
                    throw new InvalidOperationException("Division by zero.");
                Push(EvalSlot.Int(a / b));
                break;
            }
            case RoboOpcode.Neg:
                Push(EvalSlot.Int(-PopInt()));
                break;
            case RoboOpcode.EqInt:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Bool(a == b));
                break;
            }
            case RoboOpcode.NeInt:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Bool(a != b));
                break;
            }
            case RoboOpcode.Lt:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Bool(a < b));
                break;
            }
            case RoboOpcode.Le:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Bool(a <= b));
                break;
            }
            case RoboOpcode.Gt:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Bool(a > b));
                break;
            }
            case RoboOpcode.Ge:
            {
                var b = PopInt();
                var a = PopInt();
                Push(EvalSlot.Bool(a >= b));
                break;
            }
            case RoboOpcode.And:
            {
                var b = PopBool();
                var a = PopBool();
                Push(EvalSlot.Bool(a & b));
                break;
            }
            case RoboOpcode.Or:
            {
                var b = PopBool();
                var a = PopBool();
                Push(EvalSlot.Bool(a | b));
                break;
            }
            case RoboOpcode.Not:
                Push(EvalSlot.Bool(!PopBool()));
                break;
            case RoboOpcode.Jump:
                frame.Ip = insn.A;
                break;
            case RoboOpcode.JumpIfFalse:
            {
                var cond = PopBool();
                if (!cond)
                    frame.Ip = insn.A;
                break;
            }
            case RoboOpcode.JumpIfTrue:
            {
                var cond = PopBool();
                if (cond)
                    frame.Ip = insn.A;
                break;
            }
            case RoboOpcode.Call:
                PushFrame(program, insn.A, frame.Ip, insn.B);
                break;
            case RoboOpcode.CallBuiltin:
                InvokeBuiltin((BuiltinId)insn.A, insn.B);
                break;
            case RoboOpcode.Return:
                Return(program, fn);
                break;
            case RoboOpcode.NewIntArray:
            {
                var n = PopInt();
                if (n < 0)
                    throw new InvalidOperationException("Negative array size.");

                var id = _nextArrayId++;
                var list = new List<int>(Enumerable.Repeat(0, n));
                _arrays[id] = list;
                Push(EvalSlot.Array(id));
                break;
            }
            case RoboOpcode.ArrayLen:
            {
                var id = PopArray();
                Push(EvalSlot.Int(_arrays[id].Count));
                break;
            }
            case RoboOpcode.ArrayGet:
            {
                var idx = PopInt();
                var id = PopArray();
                var list = _arrays[id];
                if ((uint)idx >= (uint)list.Count)
                    throw new InvalidOperationException("Array index out of range.");

                Push(EvalSlot.Int(list[idx]));
                break;
            }
            case RoboOpcode.ArraySet:
            {
                var val = PopInt();
                var idx = PopInt();
                var id = PopArray();
                var list = _arrays[id];
                if ((uint)idx >= (uint)list.Count)
                    throw new InvalidOperationException("Array index out of range.");

                list[idx] = val;
                break;
            }
            case RoboOpcode.ConvNumberToInt:
            {
                var d = PopDouble();
                Push(EvalSlot.Int((int)d));
                break;
            }
            default:
                throw new InvalidOperationException($"Unknown opcode {insn.Op}.");
        }
    }

    private void InvokeBuiltin(BuiltinId id, int argCount)
    {
        var world = _world ?? throw new InvalidOperationException("World not set.");
        if (!RobotWorldCommands.TryGetPrimaryActor(world, out var actor))
            throw new InvalidOperationException("No primary actor.");

        switch (id)
        {
            case BuiltinId.Move:
                EnsureArity(argCount, 0);
                RobotWorldCommands.TryMoveForward(world, actor);
                break;
            case BuiltinId.TurnLeft:
                EnsureArity(argCount, 0);
                RobotWorldCommands.TurnLeft(world, actor);
                break;
            case BuiltinId.TurnRight:
                EnsureArity(argCount, 0);
                RobotWorldCommands.TurnRight(world, actor);
                break;
            case BuiltinId.Pick:
                EnsureArity(argCount, 0);
                RobotWorldCommands.TryPick(world, actor);
                break;
            case BuiltinId.Drop:
                EnsureArity(argCount, 0);
                RobotWorldCommands.TryDrop(world, actor);
                break;
            case BuiltinId.FrontIsClear:
                EnsureArity(argCount, 0);
                Push(EvalSlot.Bool(RobotWorldCommands.FrontIsClear(world, actor)));
                break;
            case BuiltinId.LeftIsClear:
                EnsureArity(argCount, 0);
                Push(EvalSlot.Bool(RobotWorldCommands.LeftIsClear(world, actor)));
                break;
            case BuiltinId.RightIsClear:
                EnsureArity(argCount, 0);
                Push(EvalSlot.Bool(RobotWorldCommands.RightIsClear(world, actor)));
                break;
            case BuiltinId.Print:
                EnsureArity(argCount, 1);
                PrintOne(Pop());
                break;
            case BuiltinId.Count:
                EnsureArity(argCount, 1);
            {
                var arrId = PopArray();
                Push(EvalSlot.Int(_arrays[arrId].Count));
                break;
            }
            case BuiltinId.Add:
                EnsureArity(argCount, 2);
            {
                var val = PopInt();
                var arrId = PopArray();
                _arrays[arrId].Add(val);
                break;
            }
            case BuiltinId.GetLast:
                EnsureArity(argCount, 1);
            {
                var arrId = PopArray();
                var list = _arrays[arrId];
                if (list.Count == 0)
                    throw new InvalidOperationException("getLast on empty array.");

                Push(EvalSlot.Int(list[^1]));
                break;
            }
            case BuiltinId.TakeLast:
                EnsureArity(argCount, 1);
            {
                var arrId = PopArray();
                var list = _arrays[arrId];
                if (list.Count == 0)
                    throw new InvalidOperationException("takeLast on empty array.");

                var v = list[^1];
                list.RemoveAt(list.Count - 1);
                Push(EvalSlot.Int(v));
                break;
            }
            default:
                throw new InvalidOperationException($"Unhandled built-in {id}.");
        }
    }

    private static void EnsureArity(int actual, int expected)
    {
        if (actual != expected)
            throw new InvalidOperationException($"Built-in arity mismatch (expected {expected}, got {actual}).");
    }

    private void PrintOne(EvalSlot slot)
    {
        switch (slot.Kind)
        {
            case EvalKind.Int:
                _stdout.WriteLine(slot.IntValue);
                break;
            case EvalKind.Bool:
                _stdout.WriteLine(slot.IntValue != 0 ? "true" : "false");
                break;
            case EvalKind.Double:
                _stdout.WriteLine(slot.DoubleValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
                break;
            case EvalKind.String:
                _stdout.WriteLine(slot.Text ?? "");
                break;
            default:
                _stderr.WriteLine("print: unsupported value.");
                break;
        }
    }

    private void Return(RoboProgram program, CompiledFunction fn)
    {
        var retIp = _frames[^1].ReturnIp;
        EvalSlot? ret = null;
        if (!fn.ReturnsVoid)
            ret = Pop();

        _frames.RemoveAt(_frames.Count - 1);
        if (_frames.Count == 0)
            return;

        _frames[^1].Ip = retIp;
        if (ret is not null)
            Push(ret.Value);
    }

    private void PushFrame(RoboProgram program, int functionIndex, int returnIp, int argumentCount)
    {
        var fn = program.Functions[functionIndex];
        if (_stack.Count < argumentCount)
            throw new InvalidOperationException("Stack underflow (arguments).");

        var locals = new EvalSlot[fn.LocalSlotCount];
        for (var i = argumentCount - 1; i >= 0; i--)
            locals[i] = Pop();

        _frames.Add(new Frame(functionIndex, returnIp, locals));
    }

    private void Push(EvalSlot v) => _stack.Add(v);

    private EvalSlot Pop()
    {
        if (_stack.Count == 0)
            throw new InvalidOperationException("Stack underflow.");
        var i = _stack.Count - 1;
        var v = _stack[i];
        _stack.RemoveAt(i);
        return v;
    }

    private EvalSlot Peek()
    {
        if (_stack.Count == 0)
            throw new InvalidOperationException("Stack underflow.");
        return _stack[^1];
    }

    private int PopInt()
    {
        var s = Pop();
        if (s.Kind != EvalKind.Int)
            throw new InvalidOperationException("Expected int on stack.");
        return s.IntValue;
    }

    private double PopDouble()
    {
        var s = Pop();
        if (s.Kind != EvalKind.Double)
            throw new InvalidOperationException("Expected number on stack.");
        return s.DoubleValue;
    }

    private bool PopBool()
    {
        var s = Pop();
        return s.Kind switch
        {
            EvalKind.Bool => s.IntValue != 0,
            EvalKind.Int => s.IntValue != 0,
            _ => throw new InvalidOperationException("Expected bool on stack."),
        };
    }

    private int PopArray()
    {
        var s = Pop();
        if (s.Kind != EvalKind.Array)
            throw new InvalidOperationException("Expected array on stack.");
        return s.ArrayId;
    }
}
