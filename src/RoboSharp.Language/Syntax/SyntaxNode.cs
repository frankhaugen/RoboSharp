namespace RoboSharp.Language.Syntax;

/// <summary>Base type for syntax tree nodes (tokens are not nodes).</summary>
public abstract record SyntaxNode;

public abstract record MemberSyntax : SyntaxNode;

public abstract record StatementSyntax : SyntaxNode;

public abstract record ExpressionSyntax : SyntaxNode;

public abstract record TypeSyntax : SyntaxNode;
