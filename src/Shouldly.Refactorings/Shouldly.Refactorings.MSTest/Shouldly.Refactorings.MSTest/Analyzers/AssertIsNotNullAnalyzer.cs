namespace Shouldly.Refactorings.MSTest.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssertIsNotNullAnalyzer : BaseDiagnosticAnalyzer
    {
        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AssertIsNotNullRule);

        /// <summary>
        /// Gets the assert is null rule.
        /// </summary>
        /// <value>
        /// The assert is null rule.
        /// </value>
        internal static DiagnosticDescriptor AssertIsNotNullRule => BuildRule();

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

            if (memberAccessExpression.Name.Identifier.Text != "IsNotNull")
            {
                return;
            }

            var argumentList = invocationExpression.ArgumentList;

            if (!argumentList.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentList.Arguments.First();

            RaiseDiagnostic(AssertIsNotNullRule, invocationExpression.GetLocation(), firstArgument.ToString());
        }


        /// <summary>
        /// Builds the diagnostic descriptor rule.
        /// </summary>
        /// <returns>The diagnostic descriptor rule.</returns>
        private static DiagnosticDescriptor BuildRule()
        {
            return BuildRule(
               DiagnosticIds.AssertIsNotNull,
               nameof(Resources.AssertIsNotNullTitle),
               nameof(Resources.AssertIsNotNullMessageFormat),
               nameof(Resources.AssertIsNotNullCategory),
               nameof(Resources.AssertIsNotNullDescription),
               DiagnosticSeverity.Info);
        }
    }
}