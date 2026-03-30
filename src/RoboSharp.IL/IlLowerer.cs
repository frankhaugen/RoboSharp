using RoboSharp.Language;
using RoboSharp.Semantics;

namespace RoboSharp.IL;

public sealed class IlLowerer
{
    public RoboProgram Lower(BoundCompilationUnit unit)
    {
        var strings = new List<string>();
        var stringIndex = new Dictionary<string, int>(StringComparer.Ordinal);
        var numbers = new List<double>();
        var numberBitsToIndex = new Dictionary<long, int>();
        var functions = new List<CompiledFunction>();
        var fnIndex = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var i = 0; i < unit.Functions.Count; i++)
            fnIndex[unit.Functions[i].Symbol.Name] = i;

        foreach (var fn in unit.Functions)
        {
            var emitter = new IlEmitter(strings, stringIndex, numbers, numberBitsToIndex, fnIndex);
            var locals = CountLocalSlots(fn);
            emitter.EmitStatement(fn.Body);
            if (fn.Symbol.ReturnType is PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Void })
                emitter.Emit(new Instruction(RoboOpcode.Return));

            var returnsVoid = fn.Symbol.ReturnType is PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Void };
            functions.Add(new CompiledFunction
            {
                Name = fn.Symbol.Name,
                ParameterCount = fn.Symbol.Parameters.Count,
                LocalSlotCount = locals,
                ReturnsVoid = returnsVoid,
                Instructions = emitter.ToInstructions(),
            });
        }

        var entry = unit.EntryPoint is null
            ? 0
            : fnIndex.GetValueOrDefault(unit.EntryPoint.Name, 0);

        return new RoboProgram
        {
            StringTable = strings,
            NumberTable = numbers,
            Functions = functions,
            EntryFunctionIndex = entry,
        };
    }

    private static int CountLocalSlots(BoundFunctionDeclaration fn)
    {
        var max = -1;
        foreach (var p in fn.Symbol.Parameters)
            max = Math.Max(max, p.SlotIndex);

        Visit(fn.Body);
        return max + 1;

        void Visit(BoundNode node)
        {
            switch (node)
            {
                case BoundVariableDeclarationStatement v:
                    max = Math.Max(max, v.Symbol.SlotIndex);
                    Visit(v.Initializer);
                    break;
                case BoundAssignmentStatement a:
                    Visit(a.Expression);
                    break;
                case BoundExpressionStatement e:
                    Visit(e.Expression);
                    break;
                case BoundIfStatement i:
                    Visit(i.Condition);
                    Visit(i.ThenStatement);
                    if (i.ElseStatement is not null)
                        Visit(i.ElseStatement);
                    break;
                case BoundWhileStatement w:
                    Visit(w.Condition);
                    Visit(w.Body);
                    break;
                case BoundReturnStatement r:
                    if (r.Expression is not null)
                        Visit(r.Expression);
                    break;
                case BoundBlockStatement b:
                    foreach (var s in b.Statements)
                        Visit(s);
                    break;
                case BoundBinaryExpression x:
                    Visit(x.Left);
                    Visit(x.Right);
                    break;
                case BoundUnaryExpression u:
                    Visit(u.Operand);
                    break;
                case BoundCallExpression c:
                    foreach (var a in c.Arguments)
                        Visit(a);
                    break;
                case BoundBuiltinCallExpression b:
                    foreach (var a in b.Arguments)
                        Visit(a);
                    break;
                case BoundArrayCreationExpression a:
                    foreach (var e in a.Elements)
                        Visit(e);
                    break;
                case BoundIndexExpression i:
                    Visit(i.Target);
                    Visit(i.Index);
                    break;
                case BoundConversionExpression c:
                    Visit(c.Operand);
                    break;
                case BoundLiteralExpression:
                case BoundVariableExpression:
                    break;
            }
        }
    }

    private sealed class IlEmitter
    {
        private readonly List<Instruction> _code = new();
        private readonly List<string> _stringTable;
        private readonly Dictionary<string, int> _stringIndex;
        private readonly List<double> _numberTable;
        private readonly Dictionary<long, int> _numberBitsToIndex;
        private readonly Dictionary<string, int> _functionIndex;

        public IlEmitter(
            List<string> stringTable,
            Dictionary<string, int> stringIndex,
            List<double> numberTable,
            Dictionary<long, int> numberBitsToIndex,
            Dictionary<string, int> functionIndex
        )
        {
            _stringTable = stringTable;
            _stringIndex = stringIndex;
            _numberTable = numberTable;
            _numberBitsToIndex = numberBitsToIndex;
            _functionIndex = functionIndex;
        }

        public IReadOnlyList<Instruction> ToInstructions() => _code;

        public void Emit(Instruction i) => _code.Add(i);

        public int Ip => _code.Count;

        public int EmitJumpPlaceholder(RoboOpcode op) =>
            EmitAndReturnIndex(new Instruction(op, 0));

        public void PatchJump(int instructionIndex, int targetIp) =>
            _code[instructionIndex] = _code[instructionIndex] with { A = targetIp };

        private int EmitAndReturnIndex(Instruction i)
        {
            _code.Add(i);
            return _code.Count - 1;
        }

        private int GetOrAddString(string s)
        {
            if (_stringIndex.TryGetValue(s, out var idx))
                return idx;

            idx = _stringTable.Count;
            _stringTable.Add(s);
            _stringIndex[s] = idx;
            return idx;
        }

        public void EmitStatement(BoundStatement stmt)
        {
            switch (stmt)
            {
                case BoundBlockStatement b:
                    foreach (var s in b.Statements)
                        EmitStatement(s);
                    break;
                case BoundVariableDeclarationStatement v:
                    EmitExpression(v.Initializer);
                    Emit(new Instruction(RoboOpcode.StoreLocal, v.Symbol.SlotIndex));
                    break;
                case BoundAssignmentStatement a:
                    EmitExpression(a.Expression);
                    Emit(new Instruction(RoboOpcode.StoreLocal, a.Symbol.SlotIndex));
                    break;
                case BoundExpressionStatement e:
                    EmitExpression(e.Expression);
                    if (e.Expression.Type is not PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Void })
                        Emit(new Instruction(RoboOpcode.Pop));
                    break;
                case BoundIfStatement i:
                    EmitExpression(i.Condition);
                    var jmpFalse = EmitJumpPlaceholder(RoboOpcode.JumpIfFalse);
                    EmitStatement(i.ThenStatement);
                    if (i.ElseStatement is null)
                    {
                        PatchJump(jmpFalse, Ip);
                    }
                    else
                    {
                        var jmpEnd = EmitJumpPlaceholder(RoboOpcode.Jump);
                        PatchJump(jmpFalse, Ip);
                        EmitStatement(i.ElseStatement);
                        PatchJump(jmpEnd, Ip);
                    }

                    break;
                case BoundWhileStatement w:
                    var head = Ip;
                    EmitExpression(w.Condition);
                    var jf = EmitJumpPlaceholder(RoboOpcode.JumpIfFalse);
                    EmitStatement(w.Body);
                    Emit(new Instruction(RoboOpcode.Jump, head));
                    PatchJump(jf, Ip);
                    break;
                case BoundReturnStatement r:
                    if (r.Expression is not null)
                        EmitExpression(r.Expression);
                    Emit(new Instruction(RoboOpcode.Return));
                    break;
            }
        }

        public void EmitExpression(BoundExpression expr)
        {
            switch (expr)
            {
                case BoundLiteralExpression l:
                    EmitLiteral(l);
                    break;
                case BoundVariableExpression v:
                    Emit(new Instruction(RoboOpcode.LoadLocal, v.Symbol.SlotIndex));
                    break;
                case BoundConversionExpression c:
                    EmitExpression(c.Operand);
                    Emit(new Instruction(RoboOpcode.ConvNumberToInt));
                    break;
                case BoundUnaryExpression u:
                    EmitExpression(u.Operand);
                    if (u.Syntax.OperatorToken.Kind == Language.SyntaxKind.MinusToken)
                        Emit(new Instruction(RoboOpcode.Neg));
                    else if (u.Syntax.OperatorToken.Kind == Language.SyntaxKind.BangToken)
                        Emit(new Instruction(RoboOpcode.Not));
                    else if (u.Syntax.OperatorToken.Kind == Language.SyntaxKind.PlusToken)
                    {
                        /* no-op */
                    }

                    break;
                case BoundBinaryExpression b:
                    if (b.Syntax.OperatorToken.Kind is Language.SyntaxKind.AmpersandAmpersandToken)
                    {
                        EmitExpression(b.Left);
                        var jf = EmitJumpPlaceholder(RoboOpcode.JumpIfFalse);
                        EmitExpression(b.Right);
                        var jEnd = EmitJumpPlaceholder(RoboOpcode.Jump);
                        PatchJump(jf, Ip);
                        Emit(new Instruction(RoboOpcode.PushBool, 0));
                        PatchJump(jEnd, Ip);
                    }
                    else if (b.Syntax.OperatorToken.Kind is Language.SyntaxKind.PipePipeToken)
                    {
                        EmitExpression(b.Left);
                        var jt = EmitJumpPlaceholder(RoboOpcode.JumpIfTrue);
                        EmitExpression(b.Right);
                        var jEnd = EmitJumpPlaceholder(RoboOpcode.Jump);
                        PatchJump(jt, Ip);
                        Emit(new Instruction(RoboOpcode.PushBool, 1));
                        PatchJump(jEnd, Ip);
                    }
                    else
                    {
                        EmitExpression(b.Left);
                        EmitExpression(b.Right);
                        Emit(b.Syntax.OperatorToken.Kind switch
                        {
                            Language.SyntaxKind.PlusToken => new Instruction(RoboOpcode.Add),
                            Language.SyntaxKind.MinusToken => new Instruction(RoboOpcode.Sub),
                            Language.SyntaxKind.StarToken => new Instruction(RoboOpcode.Mul),
                            Language.SyntaxKind.SlashToken => new Instruction(RoboOpcode.Div),
                            Language.SyntaxKind.EqualsEqualsToken => new Instruction(RoboOpcode.EqInt),
                            Language.SyntaxKind.BangEqualsToken => new Instruction(RoboOpcode.NeInt),
                            Language.SyntaxKind.LessToken => new Instruction(RoboOpcode.Lt),
                            Language.SyntaxKind.LessOrEqualsToken => new Instruction(RoboOpcode.Le),
                            Language.SyntaxKind.GreaterToken => new Instruction(RoboOpcode.Gt),
                            Language.SyntaxKind.GreaterOrEqualsToken => new Instruction(RoboOpcode.Ge),
                            _ => new Instruction(RoboOpcode.Nop),
                        });
                    }

                    break;
                case BoundCallExpression c:
                    foreach (var a in c.Arguments)
                        EmitExpression(a);

                    var idx = _functionIndex[c.Function.Name];
                    Emit(new Instruction(RoboOpcode.Call, idx, c.Arguments.Count));
                    break;
                case BoundBuiltinCallExpression b:
                    foreach (var a in b.Arguments)
                        EmitExpression(a);

                    Emit(new Instruction(RoboOpcode.CallBuiltin, (int)b.Builtin, b.Arguments.Count));
                    break;
                case BoundArrayCreationExpression a:
                    Emit(new Instruction(RoboOpcode.PushInt, a.Elements.Count));
                    Emit(new Instruction(RoboOpcode.NewIntArray));
                    for (var i = 0; i < a.Elements.Count; i++)
                    {
                        Emit(new Instruction(RoboOpcode.Dup));
                        Emit(new Instruction(RoboOpcode.PushInt, i));
                        EmitExpression(a.Elements[i]);
                        Emit(new Instruction(RoboOpcode.ArraySet));
                    }

                    break;
                case BoundIndexExpression i:
                    EmitExpression(i.Target);
                    EmitExpression(i.Index);
                    Emit(new Instruction(RoboOpcode.ArrayGet));
                    break;
            }
        }

        private void EmitLiteral(BoundLiteralExpression l)
        {
            switch (l.Type)
            {
                case PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Int }:
                    Emit(new Instruction(RoboOpcode.PushInt, Convert.ToInt32(l.Value)));
                    break;
                case PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Bool }:
                    Emit(new Instruction(RoboOpcode.PushBool, l.Value is true ? 1 : 0));
                    break;
                case PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Number }:
                    var d = Convert.ToDouble(l.Value);
                    var bits = BitConverter.DoubleToInt64Bits(d);
                    if (!_numberBitsToIndex.TryGetValue(bits, out var ni))
                    {
                        ni = _numberTable.Count;
                        _numberTable.Add(d);
                        _numberBitsToIndex[bits] = ni;
                    }

                    Emit(new Instruction(RoboOpcode.PushNumber, ni));
                    break;
                case PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.String }:
                    var si = GetOrAddString((string)l.Value);
                    Emit(new Instruction(RoboOpcode.PushString, si));
                    break;
                default:
                    Emit(new Instruction(RoboOpcode.PushInt, 0));
                    break;
            }
        }
    }
}
