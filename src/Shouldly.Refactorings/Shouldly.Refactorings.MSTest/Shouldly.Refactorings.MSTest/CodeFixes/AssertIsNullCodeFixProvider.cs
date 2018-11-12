namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Shouldly.Refactorings.MSTest.ExpressionBuilders;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertIsNullCodeFixProvider)), Shared]
    public class AssertIsNullCodeFixProvider : BaseShouldlyCodeFixProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertIsNullCodeFixProvider"/> class.
        /// </summary>
        public AssertIsNullCodeFixProvider()
        {
            ExpressionBuilders = new[]
            {
                new InvocationExpressionBuilder(IsValueOverload, BuildShouldBeNull),
                new InvocationExpressionBuilder(IsValueAndMessageOverload, BuildShouldBeNullWithMessage),
                new InvocationExpressionBuilder(IsValueMessageAndParametersOverload, BuildShouldBeNullWithMessageAndParameters)
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

        private MemberAccessExpressionSyntax BuildMemberAccessExpression(InvocationExpressionSyntax invocationExpression)
        {
            var argumentList = invocationExpression.ArgumentList;
            var firstArgument = argumentList.Arguments.First();

            var identifier = firstArgument.Expression as IdentifierNameSyntax;
            var shouldBeNullIdentifier = SyntaxFactory.IdentifierName("ShouldBeNull");

            var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, shouldBeNullIdentifier);

            return newExpression;
        }

        private InvocationExpressionSyntax BuildShouldBeNull(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            return SyntaxFactory
                        .InvocationExpression(newMemberAccessExpression)
                        .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
        }

        private InvocationExpressionSyntax BuildShouldBeNullWithMessage(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);
            var messageArgument = invocationExpression.ArgumentList.Arguments[1];
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { messageArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            return SyntaxFactory
                        .InvocationExpression(newMemberAccessExpression, newArgumentList)
                        .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
        }

        private InvocationExpressionSyntax BuildShouldBeNullWithMessageAndParameters(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);
            var messageArgument = invocationExpression.ArgumentList.Arguments[1];
            var parametersArgument = invocationExpression.ArgumentList.Arguments[2];
            var stringFormatArguments = new[] { messageArgument, parametersArgument };
            var stringFormatInvocationExpression = BuildStringFormatExpression(stringFormatArguments);
            var stringFormatInvocationArgument = SyntaxFactory.Argument(stringFormatInvocationExpression);
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { stringFormatInvocationArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            return SyntaxFactory
                        .InvocationExpression(newMemberAccessExpression, newArgumentList)
                        .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
        }

        private bool IsValueAndMessageOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 2)
            {
                return false;
            }

            return parameters[0].Name == "value" &&
                   parameters[1].Name == "message";
        }

        private bool IsValueMessageAndParametersOverload(IMethodSymbol symbol)
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

        private bool IsValueOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 1)
            {
                return false;
            }

            return parameters[0].Name == "value";
        }
    }
}