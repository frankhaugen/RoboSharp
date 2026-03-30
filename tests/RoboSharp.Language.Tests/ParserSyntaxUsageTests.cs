using RoboSharp.Language.Syntax;

namespace RoboSharp.Language.Tests;

/// <summary>Realistic source shapes learners write: functions, control flow, calls, arrays, operators.</summary>
public class ParserSyntaxUsageTests
{
    private static SyntaxTree P(string text) => SyntaxTree.Parse(SourceText.From(text));

    [Test]
    public async Task Void_Main_With_Print_Parse_Clean()
    {
        const string src = """
            void main()
            {
                print(1);
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        await Assert.That(tree.Root.Members).HasCount(1);
        var fn = (FunctionDeclarationSyntax)tree.Root.Members[0];
        await Assert.That(fn.Identifier.Text).IsEqualTo("main");
        await Assert.That(fn.ReturnType).IsAssignableTo(typeof(PrimitiveTypeSyntax));
        var ret = (PrimitiveTypeSyntax)fn.ReturnType;
        await Assert.That(ret.Keyword.Kind).IsEqualTo(SyntaxKind.VoidKeyword);
    }

    [Test]
    public async Task Multiple_Functions_And_Global_Statements()
    {
        const string src = """
            void helper() { }

            void main()
            {
                print(0);
            }

            integer g = 1;
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        await Assert.That(tree.Root.Members).HasCount(3);
    }

    [Test]
    public async Task Nested_If_With_Else_Block()
    {
        const string src = """
            void main()
            {
                if (true)
                {
                    if (false) { }
                    else { print(1); }
                }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task While_With_Single_Line_Body()
    {
        const string src = """
            void main()
            {
                while (false)
                    print(1);
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Return_With_And_Without_Expression()
    {
        const string src = """
            void main()
            {
                return;
            }

            integer id(integer x)
            {
                return x;
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Call_With_Multiple_Arguments()
    {
        const string src = """
            void main()
            {
                print(1, 2);
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Chained_Index_And_Call_On_Name()
    {
        const string src = """
            void main()
            {
                integer x = foo(1)[0];
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((FunctionDeclarationSyntax)tree.Root.Members[0]).Body.Statements[0];
        await Assert.That(stmt.Initializer).IsAssignableTo(typeof(IndexExpressionSyntax));
        var idx = (IndexExpressionSyntax)stmt.Initializer!;
        await Assert.That(idx.Target).IsAssignableTo(typeof(CallExpressionSyntax));
    }

    [Test]
    public async Task Parentheses_Override_Add_Multiply_Precedence()
    {
        const string src = "integer x = (1 + 2) * 3;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        var bin = (BinaryExpressionSyntax)stmt.Initializer!;
        await Assert.That(bin.OperatorToken.Kind).IsEqualTo(SyntaxKind.StarToken);
        await Assert.That(bin.Left).IsAssignableTo(typeof(ParenthesizedExpressionSyntax));
    }

    [Test]
    public async Task Logical_Or_Binds_Looser_Than_Logical_And()
    {
        const string src = "bool x = true || false && false;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        var root = (BinaryExpressionSyntax)stmt.Initializer!;
        await Assert.That(root.OperatorToken.Kind).IsEqualTo(SyntaxKind.PipePipeToken);
        await Assert.That(root.Right).IsAssignableTo(typeof(BinaryExpressionSyntax));
        var right = (BinaryExpressionSyntax)root.Right;
        await Assert.That(right.OperatorToken.Kind).IsEqualTo(SyntaxKind.AmpersandAmpersandToken);
    }

    [Test]
    public async Task Unary_Minus_And_Bang_On_Literals()
    {
        const string src = """
            void main()
            {
                integer a = -42;
                bool b = !false;
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Comparison_Operators_In_Condition()
    {
        const string src = """
            void main()
            {
                if (1 < 2 && 2 <= 2 && 3 > 1 && 3 >= 3 && 1 == 1 && 1 != 2)
                { }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Empty_Array_Literal()
    {
        const string src = "integer[] xs = [];";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        await Assert.That(stmt.Initializer).IsAssignableTo(typeof(ArrayLiteralExpressionSyntax));
        var lit = (ArrayLiteralExpressionSyntax)stmt.Initializer!;
        await Assert.That(lit.Elements).IsEmpty();
    }

    [Test]
    public async Task Multi_Dimensional_Array_Type()
    {
        const string src = "integer[][] grid = [[]];";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        await Assert.That(stmt.Type).IsAssignableTo(typeof(ArrayTypeSyntax));
    }

    [Test]
    public async Task Block_Inside_Then_And_Else()
    {
        const string src = """
            void main()
            {
                if (true) { print(1); }
                else { print(2); }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Expression_Statement_Is_Call()
    {
        const string src = """
            void main()
            {
                move();
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = ((FunctionDeclarationSyntax)tree.Root.Members[0]).Body.Statements[0];
        var es = (ExpressionStatementSyntax)stmt;
        await Assert.That(es.Expression).IsAssignableTo(typeof(CallExpressionSyntax));
    }

    [Test]
    public async Task Assignment_To_Local_After_Declaration()
    {
        const string src = """
            void main()
            {
                integer x = 0;
                x = 1;
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var assign = (AssignmentStatementSyntax)((FunctionDeclarationSyntax)tree.Root.Members[0]).Body.Statements[1];
        await Assert.That(assign.Identifier.Text).IsEqualTo("x");
    }

    [Test]
    public async Task String_Literal_In_Call()
    {
        const string src = """
            void main()
            {
                print("hi");
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Number_Literal_In_Expression()
    {
        const string src = "number x = 0.5;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Left_Associative_Addition_Chain()
    {
        const string src = "integer x = 1 + 2 + 3;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        var top = (BinaryExpressionSyntax)stmt.Initializer!;
        await Assert.That(top.OperatorToken.Kind).IsEqualTo(SyntaxKind.PlusToken);
        await Assert.That(top.Left).IsAssignableTo(typeof(BinaryExpressionSyntax));
    }

    [Test]
    public async Task Double_Unary_Bang_Parse()
    {
        const string src = "bool x = !!true;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        var outer = (UnaryExpressionSyntax)stmt.Initializer!;
        await Assert.That(outer.OperatorToken.Kind).IsEqualTo(SyntaxKind.BangToken);
        await Assert.That(outer.Operand).IsAssignableTo(typeof(UnaryExpressionSyntax));
    }

    [Test]
    public async Task Global_Integer_Declaration_Parse()
    {
        const string src = "integer counter = 0;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var g = (GlobalStatementSyntax)tree.Root.Members[0];
        var decl = (VariableDeclarationStatementSyntax)g.Statement;
        await Assert.That(decl.Identifier.Text).IsEqualTo("counter");
    }

    [Test]
    public async Task Type_Keyword_After_Function_Without_Separator_Produces_Diagnostic()
    {
        const string src = """
            void main() { }
            integer
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Missing_Semicolon_After_Global_Decl_Produces_Diagnostic()
    {
        const string src = "integer x = 1";
        var tree = P(src);
        await Assert.That(tree.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Bad_Token_In_Expression_Produces_Diagnostic()
    {
        const string src = "integer x = @;";
        var tree = P(src);
        await Assert.That(tree.Diagnostics.Any(d => d.Message.Contains("Unexpected", StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task Integer_Type_Without_Init_After_Name_Produces_Diagnostic()
    {
        const string src = "integer x";
        var tree = P(src);
        await Assert.That(tree.Diagnostics.Any(d => d.Message.Contains("Expected", StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task Serializer_Round_Trip_Prints_Compilation_Unit()
    {
        const string src = """
            void main()
            {
                print(7);
            }
            """;

        var tree = P(src);
        var serializer = new SyntaxTreeSerializer();
        var dump = serializer.Serialize(tree.Root);
        await Assert.That(dump).Contains("CompilationUnitSyntax");
        await Assert.That(dump).Contains("FunctionDeclarationSyntax");
        await Assert.That(dump).Contains("main");
    }

    [Test]
    public async Task Function_With_Two_Parameters()
    {
        const string src = """
            void swap(integer a, integer b)
            {
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var fn = (FunctionDeclarationSyntax)tree.Root.Members[0];
        await Assert.That(fn.Parameters.Parameters).HasCount(2);
        await Assert.That(fn.Parameters.Commas).HasCount(1);
    }

    [Test]
    public async Task Nested_Empty_Blocks()
    {
        const string src = """
            void main()
            {
                { { } }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Chained_Index_Expressions()
    {
        const string src = "integer x = m[i][j];";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        var outer = (IndexExpressionSyntax)stmt.Initializer!;
        await Assert.That(outer.Target).IsAssignableTo(typeof(IndexExpressionSyntax));
    }

    [Test]
    public async Task If_Without_Else_On_Block()
    {
        const string src = """
            void main()
            {
                if (false) { }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var ifStmt = (IfStatementSyntax)((FunctionDeclarationSyntax)tree.Root.Members[0]).Body.Statements[0];
        await Assert.That(ifStmt.ElseClause).IsNull();
    }

    [Test]
    public async Task While_Condition_With_Or_And()
    {
        const string src = """
            void main()
            {
                while (true || false && true) { }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Call_With_No_Arguments()
    {
        const string src = """
            void main()
            {
                seed();
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var es = (ExpressionStatementSyntax)((FunctionDeclarationSyntax)tree.Root.Members[0]).Body.Statements[0];
        var call = (CallExpressionSyntax)es.Expression;
        await Assert.That(call.Arguments).IsEmpty();
    }

    [Test]
    public async Task Deeply_Parenthesized_Literal()
    {
        const string src = "integer x = ((((1))));";
        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Mixed_Arithmetic_And_Comparison_In_While()
    {
        const string src = """
            void main()
            {
                while (1 + 1 < 3) { return; }
            }
            """;

        var tree = P(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
    }
}
