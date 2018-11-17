namespace Shouldly.Refactorings.MSTest.ExpressionBuilders
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Shouldly.Refactorings.MSTest.Delegates;
    using Shouldly.Refactorings.MSTest.Selectors;

    public class InvocationExpressionBuilder :
        BaseExpressionBuilder<IMethodSymbol, InvocationExpressionSyntax, InvocationExpressionSyntax>
    {
        public InvocationExpressionBuilder(
            OverloadMatchDelegate<IMethodSymbol> match, 
            BuildNewExpressionDelegate<InvocationExpressionSyntax, InvocationExpressionSyntax> build) 
            : base(match, build)
        {
        }
    }
}
