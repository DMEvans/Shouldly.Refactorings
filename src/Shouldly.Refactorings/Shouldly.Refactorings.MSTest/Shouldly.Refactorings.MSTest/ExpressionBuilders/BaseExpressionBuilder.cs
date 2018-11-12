namespace Shouldly.Refactorings.MSTest.Selectors
{
    using Microsoft.CodeAnalysis;
    using Shouldly.Refactorings.MSTest.Delegates;
    using Shouldly.Refactorings.MSTest.ExpressionBuilders;

    public abstract class BaseExpressionBuilder<TSymbol, TOldSyntax, TNewSyntax>
        : IExpressionBuilder<TSymbol, TOldSyntax, TNewSyntax>
        where TSymbol : ISymbol
        where TOldSyntax : SyntaxNode
        where TNewSyntax : SyntaxNode
    {
        public BaseExpressionBuilder(OverloadMatchDelegate<TSymbol> match, BuildNewExpressionDelegate<TOldSyntax, TNewSyntax> build)
        {
            Match = match;
            Build = build;
        }

        public BuildNewExpressionDelegate<TOldSyntax, TNewSyntax> Build { get; private set; }
        public OverloadMatchDelegate<TSymbol> Match { get; private set; }
    }
}