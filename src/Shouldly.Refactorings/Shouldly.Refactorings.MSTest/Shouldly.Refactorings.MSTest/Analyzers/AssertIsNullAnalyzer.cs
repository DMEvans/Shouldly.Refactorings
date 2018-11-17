namespace Shouldly.Refactorings.MSTest.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;

    /// <summary>
    /// Analyzer to find and report invocations of MSTest Assert.IsNull
    /// </summary>
    /// <seealso cref="Shouldly.Refactorings.MSTest.BaseDiagnosticAnalyzer" />
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssertIsNullAnalyzer : BaseDiagnosticAnalyzer
    {
        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(AssertIsNullRule); } }

        /// <summary>
        /// Gets the assert is null rule.
        /// </summary>
        /// <value>
        /// The assert is null rule.
        /// </value>
        internal static DiagnosticDescriptor AssertIsNullRule => BuildRule();

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

            if (memberAccessExpression.Name.Identifier.Text != "IsNull")
            {
                return;
            }

            var argumentList = invocationExpression.ArgumentList;

            if (!argumentList.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentList.Arguments.First();

            RaiseDiagnostic(AssertIsNullRule, invocationExpression.GetLocation(), firstArgument.ToString());
        }

        /// <summary>
        /// Builds the diagnostic descriptor rule.
        /// </summary>
        /// <returns>The diagnostic descriptor rule.</returns>
        private static DiagnosticDescriptor BuildRule()
        {
            return BuildRule(
               DiagnosticIds.AssertIsNull,
               nameof(Resources.AssertIsNullTitle),
               nameof(Resources.AssertIsNullMessageFormat),
               nameof(Resources.AssertIsNullCategory),
               nameof(Resources.AssertIsNullDescription),
               DiagnosticSeverity.Info);
        }
    }
}