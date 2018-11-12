namespace Shouldly.Refactorings.MSTest.Delegates
{
    using Microsoft.CodeAnalysis;

    public delegate TNew BuildNewExpressionDelegate<TOld, TNew>(TOld expression)
        where TOld : SyntaxNode
        where TNew : SyntaxNode;
}