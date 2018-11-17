namespace Shouldly.Refactorings.MSTest.ExpressionBuilders
{
    using Microsoft.CodeAnalysis;
    using Shouldly.Refactorings.MSTest.Delegates;
    using System;

    public interface IExpressionBuilder<TSymbol, TOldSyntax, TNewSyntax>
        where TSymbol : ISymbol
        where TOldSyntax : SyntaxNode
        where TNewSyntax : SyntaxNode
    {
        BuildNewExpressionDelegate<TOldSyntax, TNewSyntax> Build { get; }
        OverloadMatchDelegate<TSymbol> Match { get; }
    }
}