namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Shouldly.Refactorings.MSTest.ExpressionBuilders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class BaseShouldlyCodeFixProvider : BaseCodeFixProvider
    {
        protected IEnumerable<InvocationExpressionBuilder> ExpressionBuilders { get; set; }

        /// <summary>
        /// Converts to Shouldly assertion.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="syntaxNode">The expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated document</returns>
        protected override async Task<Document> ConvertToShouldlyAssertion(Document document, SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            var newSyntaxNode = BuildNewSyntaxNode(syntaxNode);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot as CompilationUnitSyntax;
            newRoot = newRoot.ReplaceNode(syntaxNode, newSyntaxNode);
            newRoot = UpdateShouldyUsingDirective(newRoot);

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Builds the new invocation expression.
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        /// <returns></returns>
        protected virtual SyntaxNode BuildNewSyntaxNode(SyntaxNode syntaxNode)
        {
            var invocationExpression = syntaxNode as InvocationExpressionSyntax;
            var semanticModel = ObtainSemanticModel(invocationExpression.SyntaxTree);
            var symbolInfo = semanticModel.GetSymbolInfo(invocationExpression);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
            var expressionBuilder = ExpressionBuilders.FirstOrDefault(x => x.Match(methodSymbol));

            if (expressionBuilder is null)
            {
                var message = string.Format("No code fix has been implemented for this overload: {0}", methodSymbol);
                throw new NotImplementedException(message);
            }

            var newInvocationExpression = expressionBuilder.Build(invocationExpression);

            return newInvocationExpression;
        }

        /// <summary>
        /// Builds the string format expression.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        protected virtual InvocationExpressionSyntax BuildStringFormatExpression(IEnumerable<ArgumentSyntax> arguments)
        {
            var stringIdentifier = SyntaxFactory.IdentifierName("string");
            var formatIdentifier = SyntaxFactory.IdentifierName("Format");
            var separatedArguments = SyntaxFactory.SeparatedList(arguments);
            var argumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var stringFormatExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, stringIdentifier, formatIdentifier);
            var stringFormatInvocationExpression = SyntaxFactory.InvocationExpression(stringFormatExpression, argumentList);

            return stringFormatInvocationExpression;
        }

    }
}
