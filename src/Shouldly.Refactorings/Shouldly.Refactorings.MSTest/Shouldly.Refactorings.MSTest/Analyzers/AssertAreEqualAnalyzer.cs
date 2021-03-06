﻿namespace Shouldly.Refactorings.MSTest.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// Analyzer to find and report invocations of MSTest Assert.AreEqual
    /// </summary>
    /// <seealso cref="Shouldly.Refactorings.MSTest.BaseDiagnosticAnalyzer" />
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssertAreEqualAnalyzer : BaseDiagnosticAnalyzer
    {
        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(AssertAreEqualRule); } }

        /// <summary>
        /// Gets the assert is null rule.
        /// </summary>
        /// <value>
        /// The assert is null rule.
        /// </value>
        internal static DiagnosticDescriptor AssertAreEqualRule => BuildRule();

        /// <summary>
        /// Analyzes the invocation.
        /// </summary>
        protected override void AnalyzeInvocation()
        {
            var invocationExpression = Context.Node as InvocationExpressionSyntax;

            var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;

            if (memberAccessExpression is null)
            {
                return;
            }

            var identifierNameExpression = memberAccessExpression.Expression as IdentifierNameSyntax;

            if (identifierNameExpression?.Identifier.Text != "Assert")
            {
                return;
            }

            if (memberAccessExpression.Name.Identifier.Text != "AreEqual")
            {
                return;
            }

            var argumentList = invocationExpression.ArgumentList;

            if (!argumentList.Arguments.Any())
            {
                return;
            }

            var argumentsArray = argumentList.Arguments.ToArray();

            if (argumentsArray.Length < 2)
            {
                return;
            }

            var expectedArgument = argumentsArray[0];
            var actualArgument = argumentsArray[1];

            RaiseDiagnostic(
                AssertAreEqualRule, 
                invocationExpression.GetLocation(), 
                expectedArgument.ToString(), 
                actualArgument.ToString());
        }

        /// <summary>
        /// Builds the diagnostic descriptor rule.
        /// </summary>
        /// <returns>The diagnostic descriptor rule.</returns>
        private static DiagnosticDescriptor BuildRule()
        {
            return BuildRule(
               DiagnosticIds.AssertAreEqual,
               nameof(Resources.AssertAreEqualTitle),
               nameof(Resources.AssertAreEqualMessageFormat),
               nameof(Resources.AssertAreEqualCategory),
               nameof(Resources.AssertAreEqualDescription),
               DiagnosticSeverity.Info);
        }
    }
}