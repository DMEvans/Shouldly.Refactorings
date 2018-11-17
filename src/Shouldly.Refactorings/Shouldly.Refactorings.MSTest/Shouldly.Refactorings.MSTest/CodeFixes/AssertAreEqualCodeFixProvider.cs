namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Shouldly.Refactorings.MSTest.ExpressionBuilders;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertIsNullCodeFixProvider)), Shared]
    public class AssertAreEqualCodeFixProvider : BaseShouldlyCodeFixProvider
    {
        public AssertAreEqualCodeFixProvider()
        {
            ExpressionBuilders = new[]
            {
                new InvocationExpressionBuilder(IsBasicOverload, BuildShouldBeBasic),
                new InvocationExpressionBuilder(IsBasicWithMessageOverload, BuildShouldBeBasicWithMessage),
                new InvocationExpressionBuilder(IsBasicWithMessageAndParametersOverload, BuildShouldBeBasicWithMessageAndParameters)
            };
        }

        /// <summary>
        /// A list of diagnostic IDs that this provider can provider fixes for.
        /// </summary>
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.AssertAreEqual);

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        protected override string Title => GetLocalizedResourceString(nameof(Resources.AssertAreEqualFixText)).ToString();

        private MemberAccessExpressionSyntax BuildMemberAccessExpression(InvocationExpressionSyntax invocationExpression)
        {
            var actualArgument = invocationExpression.ArgumentList.Arguments[1];

            var identifier = actualArgument.Expression as IdentifierNameSyntax;
            var shouldBeIdentifier = SyntaxFactory.IdentifierName("ShouldBe");

            var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, shouldBeIdentifier);

            return newExpression;
        }

        private InvocationExpressionSyntax BuildShouldBeBasic(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            var expectedArgument = invocationExpression.ArgumentList.Arguments[0];
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { expectedArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var newInvocationExpression = SyntaxFactory
                                        .InvocationExpression(newMemberAccessExpression, newArgumentList)
                                        .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());

            return newInvocationExpression;
        }

        private InvocationExpressionSyntax BuildShouldBeBasicWithMessage(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            var arguments = invocationExpression.ArgumentList.Arguments;
            var expectedArgument = arguments[0];
            var messageArgument = arguments[2];
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { expectedArgument, messageArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var newInvocationExpression = SyntaxFactory
                                       .InvocationExpression(newMemberAccessExpression, newArgumentList)
                                       .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());

            return newInvocationExpression;
        }

        private InvocationExpressionSyntax BuildShouldBeBasicWithMessageAndParameters(InvocationExpressionSyntax invocationExpression)
        {
            var newMemberAccessExpression = BuildMemberAccessExpression(invocationExpression);

            var arguments = invocationExpression.ArgumentList.Arguments;
            var expectedArgument = arguments[0];

            var stringFormatArguments = arguments.Where(x => arguments.IndexOf(x) >= 2);
            var stringFormatInvocationExpression = BuildStringFormatExpression(stringFormatArguments);
            var stringFormatInvocationArgument = SyntaxFactory.Argument(stringFormatInvocationExpression);
            var separatedArguments = SyntaxFactory.SeparatedList(new[] { expectedArgument, stringFormatInvocationArgument });
            var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

            var newInvocationExpression = SyntaxFactory
                                       .InvocationExpression(newMemberAccessExpression, newArgumentList)
                                       .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());

            return newInvocationExpression;
        }

        private bool IsBasicOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 2)
            {
                return false;
            }

            return parameters[0].Name == "expected" &&
                   parameters[1].Name == "actual";
        }

        private bool IsBasicWithMessageAndParametersOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 4)
            {
                return false;
            }

            return parameters[0].Name == "expected" &&
                   parameters[1].Name == "actual" &&
                   parameters[2].Name == "message" &&
                   parameters[3].Name == "parameters" && parameters[3].IsParams;
        }

        private bool IsBasicWithMessageOverload(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Count() != 3)
            {
                return false;
            }

            return parameters[0].Name == "expected" &&
                   parameters[1].Name == "actual" &&
                   parameters[2].Name == "message";
        }
    }
}