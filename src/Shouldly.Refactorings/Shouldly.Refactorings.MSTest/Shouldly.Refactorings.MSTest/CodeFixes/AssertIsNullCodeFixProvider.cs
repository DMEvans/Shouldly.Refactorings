namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertIsNullCodeFixProvider)), Shared]
    public class AssertIsNullCodeFixProvider : BaseCodeFixProvider
    {
        /// <summary>
        /// A list of diagnostic IDs that this provider can provider fixes for.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.AssertIsNull);

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        private string Title => GetLocalizedResourceString(nameof(Resources.AssertIsNullFixText)).ToString();

        /// <summary>
        /// Builds the new invocation expression.
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        /// <returns></returns>
        private InvocationExpressionSyntax BuildNewInvocationExpression(InvocationExpressionSyntax invocationExpression)
        {
            var argumentList = invocationExpression.ArgumentList;
            var firstArgument = argumentList.Arguments.First();
            var otherArguments = argumentList.Arguments.Where(x => x != firstArgument);

            var identifier = firstArgument.Expression as IdentifierNameSyntax;
            var shouldBeNullIdentifier = SyntaxFactory.IdentifierName("ShouldBeNull");

            var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, shouldBeNullIdentifier);

            InvocationExpressionSyntax newInvocationExpression;

            if (otherArguments.Count() == 0)
            {
                newInvocationExpression = SyntaxFactory
                                            .InvocationExpression(newExpression)
                                            .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
            }
            else if (otherArguments.Count() == 1)
            {
                var separatedArguments = SyntaxFactory.SeparatedList(otherArguments);
                var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

                newInvocationExpression = SyntaxFactory
                                            .InvocationExpression(newExpression, newArgumentList)
                                            .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
            }
            else
            {
                var stringFormatInvocationExpression = BuildStringFormatExpression(otherArguments);
                var argument = SyntaxFactory.Argument(stringFormatInvocationExpression);
                var separatedArguments = SyntaxFactory.SeparatedList(new[] { argument });
                var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

                newInvocationExpression = SyntaxFactory
                                            .InvocationExpression(newExpression, newArgumentList)
                                            .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
            }

            return newInvocationExpression;
        }

        /// <summary>
        /// Builds the string format expression.
        /// </summary>
        /// <param name="otherArguments">The other arguments.</param>
        /// <returns></returns>
        private InvocationExpressionSyntax BuildStringFormatExpression(IEnumerable<ArgumentSyntax> otherArguments)
        {
            var stringIdentifier = SyntaxFactory.IdentifierName("string");
            var formatIdentifier = SyntaxFactory.IdentifierName("Format");
            var separatedArguments = SyntaxFactory.SeparatedList(otherArguments);
            var argumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var stringFormatExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, stringIdentifier, formatIdentifier);
            var stringFormatInvocationExpression = SyntaxFactory.InvocationExpression(stringFormatExpression, argumentList);

            return stringFormatInvocationExpression;
        }

        /// <summary>
        /// Converts to Shouldly assertion.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated document</returns>
        private async Task<Document> ConvertToShouldlyAssertion(Document document, SyntaxNode expression, CancellationToken cancellationToken)
        {
            var invocationExpression = expression as InvocationExpressionSyntax;
            var newInvocationExpression = BuildNewInvocationExpression(invocationExpression);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot as CompilationUnitSyntax;
            newRoot = newRoot.ReplaceNode(invocationExpression, newInvocationExpression);
            newRoot = UpdateShouldyUsingDirective(newRoot);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}