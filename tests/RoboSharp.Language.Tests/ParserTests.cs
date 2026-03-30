using RoboSharp.Language.Syntax;

namespace RoboSharp.Language.Tests;

public class ParserTests
{
    private static SyntaxTree Parse(string text) => SyntaxTree.Parse(SourceText.From(text));

    [Test]
    public async Task Empty_Source_Yields_Empty_Compilation_Unit()
    {
        var tree = Parse("");
        await Assert.That(tree.Root.Members).IsEmpty();
        await Assert.That(tree.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task Function_And_Global_Variable_Parse()
    {
        const string src = """
            integer add(integer a, integer b)
            {
                return a + b;
            }

            integer x = add(1, 2);
            """;

        var tree = Parse(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        await Assert.That(tree.Root.Members).HasCount(2);
        await Assert.That(tree.Root.Members[0]).IsAssignableTo(typeof(FunctionDeclarationSyntax));
        await Assert.That(tree.Root.Members[1]).IsAssignableTo(typeof(GlobalStatementSyntax));

        var fn = (FunctionDeclarationSyntax)tree.Root.Members[0];
        await Assert.That(fn.Identifier.Text).IsEqualTo("add");
        await Assert.That(fn.Parameters.Parameters).HasCount(2);

        var global = (GlobalStatementSyntax)tree.Root.Members[1];
        await Assert.That(global.Statement).IsAssignableTo(typeof(VariableDeclarationStatementSyntax));
    }

    [Test]
    public async Task If_Else_And_While_Parse()
    {
        const string src = """
            if (true) { }
            else { }

            while (false) { return; }
            """;

        var tree = Parse(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        await Assert.That(tree.Root.Members).HasCount(2);

        var ifMem = (GlobalStatementSyntax)tree.Root.Members[0];
        var ifStmt = (IfStatementSyntax)ifMem.Statement;
        await Assert.That(ifStmt.ElseClause).IsNotNull();

        var whileMem = (GlobalStatementSyntax)tree.Root.Members[1];
        await Assert.That(whileMem.Statement).IsAssignableTo(typeof(WhileStatementSyntax));
    }

    [Test]
    public async Task Array_Type_And_Literal_And_Index()
    {
        const string src = """
            integer[] xs = [1, 2];
            integer y = xs[0];
            """;

        var tree = Parse(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var m0 = (GlobalStatementSyntax)tree.Root.Members[0];
        var decl = (VariableDeclarationStatementSyntax)m0.Statement;
        await Assert.That(decl.Type).IsAssignableTo(typeof(ArrayTypeSyntax));

        var m1 = (GlobalStatementSyntax)tree.Root.Members[1];
        var decl2 = (VariableDeclarationStatementSyntax)m1.Statement;
        await Assert.That(decl2.Initializer).IsAssignableTo(typeof(IndexExpressionSyntax));
    }

    [Test]
    public async Task Binary_Precedence_Multiplicative_Before_Additive()
    {
        const string src = "integer x = 1 + 2 * 3;";
        var tree = Parse(src);
        await Assert.That(tree.Diagnostics).IsEmpty();
        var stmt = (VariableDeclarationStatementSyntax)((GlobalStatementSyntax)tree.Root.Members[0]).Statement;
        var bin = (BinaryExpressionSyntax)stmt.Initializer;
        await Assert.That(bin.OperatorToken.Kind).IsEqualTo(SyntaxKind.PlusToken);
        await Assert.That(bin.Right).IsAssignableTo(typeof(BinaryExpressionSyntax));
        var right = (BinaryExpressionSyntax)bin.Right;
        await Assert.That(right.OperatorToken.Kind).IsEqualTo(SyntaxKind.StarToken);
    }

    [Test]
    public async Task Assignment_Vs_Call_Expression_Statement()
    {
        var assign = Parse("integer x = 1; x = 2;");
        await Assert.That(assign.Diagnostics).IsEmpty();
        var s1 = ((GlobalStatementSyntax)assign.Root.Members[1]).Statement;
        await Assert.That(s1).IsAssignableTo(typeof(AssignmentStatementSyntax));

        var call = Parse("integer x = 1; foo(1);");
        await Assert.That(call.Diagnostics).IsEmpty();
        var s2 = ((GlobalStatementSyntax)call.Root.Members[1]).Statement;
        var es = (ExpressionStatementSyntax)s2;
        await Assert.That(es.Expression).IsAssignableTo(typeof(CallExpressionSyntax));
    }

    [Test]
    public async Task Serializer_Does_Not_Throw_On_Function()
    {
        const string src = "integer id(integer n) { return n; }";
        var tree = Parse(src);
        var serializer = new SyntaxTreeSerializer();
        var text = serializer.Serialize(tree.Root);
        await Assert.That(text).Contains("FunctionDeclarationSyntax");
        await Assert.That(text).Contains("id");
    }
}
