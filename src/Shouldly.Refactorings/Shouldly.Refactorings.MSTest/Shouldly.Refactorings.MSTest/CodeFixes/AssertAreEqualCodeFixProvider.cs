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

    /// <summary>
    /// Code fix provider to convert MSTest Assert.AreEqual invocations to Shouldly ShouldBe invocations
    /// </summary>
    /// <seealso cref="Shouldly.Refactorings.MSTest.CodeFixes.BaseShouldlyCodeFixProvider" />
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertIsNullCodeFixProvider)), Shared]
    public class AssertAreEqualCodeFixProvider : BaseShouldlyCodeFixProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertAreEqualCodeFixProvider"/> class.
        /// </summary>
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

        /// <summary>
        /// Builds the member access expression.
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        /// <returns>The member access expression</returns>
        private MemberAccessExpressionSyntax BuildMemberAccessExpression(InvocationExpressionSyntax invocationExpression)
        {
            var actualArgument = invocationExpression.ArgumentList.Arguments[1];

            var identifier = actualArgument.Expression as IdentifierNameSyntax;
            var shouldBeIdentifier = SyntaxFactory.IdentifierName("ShouldBe");

            var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, identifier, shouldBeIdentifier);

            return newExpression;
        }

        /// <summary>
        /// Builds the invocation expression for actual.ShouldBe(expected).
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        /// <returns>The new invocation expression</returns>
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

        /// <summary>
        /// Builds the invocation expression for actual.ShouldBe(expected, message).
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        /// <returns>The new invocation expression</returns>
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

        /// <summary>
        /// Builds the invocation expression for actual.ShouldBe(expected, string.Format(message, parameters)).
        /// </summary>
        /// <param name="invocationExpression">The invocation expression.</param>
        /// <returns>The new invocation expression</returns>
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

        /// <summary>
        /// Determines whether the invocation is Assert.AreEqual(expected, actual).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>
        ///   <c>true</c> if the invocation is Assert.AreEqual(expected, actual).
        /// </returns>
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

        /// <summary>
        /// Determines whether the invocation is Assert.AreEqual(expected, actual, message).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>
        ///   <c>true</c> if the invocation is Assert.AreEqual(expected, actual, message).
        /// </returns>
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

        /// <summary>
        /// Determines whether the invocation is Assert.AreEqual(expected, actual, message, parameters).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>
        ///   <c>true</c> if the invocation is Assert.AreEqual(expected, actual, message, parameters).
        /// </returns>
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