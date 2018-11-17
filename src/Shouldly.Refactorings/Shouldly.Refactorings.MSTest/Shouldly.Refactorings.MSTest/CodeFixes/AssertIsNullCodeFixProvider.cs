namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Shouldly.Refactorings.MSTest.ExpressionBuilders;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertIsNullCodeFixProvider)), Shared]
    public class AssertIsNullCodeFixProvider : BaseShouldlyCodeFixProvider
    {
        public AssertIsNullCodeFixProvider()
        {
            ExpressionBuilders = new[]
            {
                new InvocationExpressionBuilder(IsBasicOverload, BuildShouldBeNull),
                new InvocationExpressionBuilder(IsMessageOverload, BuildShouldBeNullWithMessage),
                new InvocationExpressionBuilder(IsMessageAndParametersOverload, BuildShouldBeNullWithMessageAndParameters)
            };
        }

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
        protected override string Title => GetLocalizedResourceString(nameof(Resources.AssertIsNullFixText)).ToString();

        /// <summary>
        /// Gets an optional <see cref="T:Microsoft.CodeAnalysis.CodeFixes.FixAllProvider" /> that can fix all/multiple occurrences of diagnostics fixed by this code fix provider.
        /// Return null if the provider doesn't support fix all/multiple occurrences.
        /// Otherwise, you can return any of the well known fix all providers from <see cref="T:Microsoft.CodeAnalysis.CodeFixes.WellKnownFixAllProviders" /> or implement your own fix all provider.
        /// </summary>
        /// <returns></returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        /// Computes one or more fixes for the specified <see cref="T:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext" />.
        /// </summary>
        /// <param name="context">A <see cref="T:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext" /> containing context information about the diagnostics to fix.
        /// The context must only contain diagnostics with a <see cref="P:Microsoft.CodeAnalysis.Diagnostic.Id" /> included in the <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider.FixableDiagnosticIds" /> for the current provider.</param>
        /// <returns></returns>
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var expression = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: cancellationToken => ConvertToShouldlyAssertion(context.Document, expression, cancellationToken),
                    equivalenceKey: Title),
                diagnostic);
        }

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

        private InvocationExpressionSyntax BuildShouldBeNull(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            var separatedArguments = SyntaxFactory.SeparatedList(new SyntaxNode[] { });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);
            var newInvocationExpression = SyntaxFactory
                                       .InvocationExpression(newMemberAccessExpression)
                                       .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());

            return newInvocationExpression;
        }

        private MemberAccessExpressionSyntax BuildMemberAccessExpression(InvocationExpressionSyntax invocationExpression)
        {
            var valueArgument = invocationExpression.ArgumentList.Arguments[0];

            var identifier = valueArgument.Expression as IdentifierNameSyntax;
            var shouldBeIdentifier = SyntaxFactory.IdentifierName("ShouldBeNull");

            var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, shouldBeIdentifier);

            return newExpression;
        }

        private InvocationExpressionSyntax BuildShouldBeNullWithMessage(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            var arguments = invocationExpression.ArgumentList.Arguments;
            var messageArgument = arguments[1];
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { messageArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var newInvocationExpression = SyntaxFactory
                                       .InvocationExpression(newMemberAccessExpression, newArgumentList)
                                       .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());

            return newInvocationExpression;
        }

        private InvocationExpressionSyntax BuildShouldBeNullWithMessageAndParameters(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            var arguments = invocationExpression.ArgumentList.Arguments;
            var valueArgument = arguments[0];
            var stringFormatArguments = arguments.Where(x => x != valueArgument);
            var stringFormatInvocationExpression = BuildStringFormatExpression(stringFormatArguments);
            var stringFormatInvocationArgument = SyntaxFactory.Argument(stringFormatInvocationExpression);
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { stringFormatInvocationArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var newInvocationExpression = SyntaxFactory
                                       .InvocationExpression(newMemberAccessExpression, newArgumentList)
                                       .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());

            return newInvocationExpression;
        }

        private bool IsBasicOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 1)
            {
                return false;
            }

            return parameters[0].Name == "value";
        }

        private bool IsMessageAndParametersOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 3)
            {
                return false;
            }

            return parameters[0].Name == "value" &&
                   parameters[1].Name == "message" &&
                   parameters[2].Name == "parameters" && parameters[2].IsParams;
        }

        private bool IsMessageOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 2)
            {
                return false;
            }

            return parameters[0].Name == "value" &&
                   parameters[1].Name == "message";
        }
    }
}