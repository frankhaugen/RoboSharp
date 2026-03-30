# Syntax kind model and syntax facts

## `SyntaxKind`

One canonical enum for:

- tokens
- trivia
- node kinds
- operators
- punctuation
- keywords

Recommended shape:

```csharp
public enum SyntaxKind
{
    // Trivia
    WhitespaceTrivia,
    EndOfLineTrivia,
    CommentTrivia,

    // Tokens
    BadToken,
    EndOfFileToken,
    IdentifierToken,
    IntegerLiteralToken,
    NumberLiteralToken,
    StringLiteralToken,

    // Keywords
    IfKeyword,
    ElseKeyword,
    WhileKeyword,
    ReturnKeyword,
    IntegerKeyword,
    NumberKeyword,
    StringKeyword,
    BoolKeyword,
    TrueKeyword,
    FalseKeyword,

    // Operators / punctuation
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    BangToken,
    EqualsToken,
    EqualsEqualsToken,
    BangEqualsToken,
    LessToken,
    LessOrEqualsToken,
    GreaterToken,
    GreaterOrEqualsToken,
    AmpersandAmpersandToken,
    PipePipeToken,
    OpenParenToken,
    CloseParenToken,
    OpenBraceToken,
    CloseBraceToken,
    OpenBracketToken,
    CloseBracketToken,
    CommaToken,
    SemicolonToken,

    // Nodes
    CompilationUnit,
    GlobalStatement,
    FunctionDeclaration,
    Parameter,
    ParameterList,
    PrimitiveType,
    ArrayType,
    BlockStatement,
    VariableDeclarationStatement,
    AssignmentStatement,
    ExpressionStatement,
    IfStatement,
    ElseClause,
    WhileStatement,
    ReturnStatement,
    LiteralExpression,
    NameExpression,
    UnaryExpression,
    BinaryExpression,
    ParenthesizedExpression,
    CallExpression,
    ArrayLiteralExpression,
    IndexExpression
}
```

One enum is simpler than splitting token-kind and node-kind too early.

## `SyntaxFacts`

Single static place for language facts:

```csharp
public static class SyntaxFacts
{
    public static string? GetText(SyntaxKind kind);
    public static SyntaxKind GetKeywordKind(string text);
    public static int GetUnaryOperatorPrecedence(SyntaxKind kind);
    public static int GetBinaryOperatorPrecedence(SyntaxKind kind);
    public static bool IsTypeKeyword(SyntaxKind kind);
}
```

This keeps keyword/operator logic from leaking everywhere.
