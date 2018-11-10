namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Reflection;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertIsNullCodeFixProvider)), Shared]

    public class AssertAreEqualCodeFixProvider : BaseCodeFixProvider
    {
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
        /// Converts to shouldly assertion.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated document</returns>
        protected override async Task<Document> ConvertToShouldlyAssertion(Document document, SyntaxNode expression, CancellationToken cancellationToken)
        {
            var invocationExpression = expression as InvocationExpressionSyntax;
            var newInvocationExpression = BuildNewInvocationExpression(invocationExpression);
            
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot as CompilationUnitSyntax;
            newRoot = newRoot.ReplaceNode(invocationExpression, newInvocationExpression);
            newRoot = UpdateShouldyUsingDirective(newRoot);

            return document.WithSyntaxRoot(newRoot);
        }

        private InvocationExpressionSyntax BuildNewInvocationExpression(InvocationExpressionSyntax invocationExpression)
        {
            var argumentList = invocationExpression.ArgumentList;
            var argumentsArray = argumentList.Arguments.ToArray();
            var expectedArgument = argumentsArray[0];
            var actualArgument = argumentsArray[1];
            var otherArguments = argumentList.Arguments.Except(new[] { expectedArgument, actualArgument });

            var actualIdentifier = actualArgument.Expression as IdentifierNameSyntax;
            var shouldBeIdentifier = SyntaxFactory.IdentifierName("ShouldBe");

            var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, actualIdentifier, shouldBeIdentifier);

            InvocationExpressionSyntax newInvocationExpression;

            if (otherArguments.Count() == 0)
            {
                var separatedArguments = SyntaxFactory.SeparatedList(new[] { expectedArgument });
                var newArgumentList = SyntaxFactory.ArgumentList(separatedArguments);

                newInvocationExpression = SyntaxFactory
                                            .InvocationExpression(newExpression, newArgumentList)
                                            .WithLeadingTrivia(invocationExpression.GetLeadingTrivia());
            }
            else
            {
                var semanticModel = ObtainSemanticModel(invocationExpression);

                var symbolInfo = semanticModel.GetSymbolInfo(invocationExpression);

                newInvocationExpression = invocationExpression;
            }

            return newInvocationExpression;
        }

        private SemanticModel ObtainSemanticModel(SyntaxNode node)
        {
            var typesForReferences = new[]
            {
                typeof(object),
                typeof(Assert),
                typeof(Should)
            };
            
            var references = typesForReferences.Select(x => MetadataReference.CreateFromFile(x.Assembly.Location)).ToList();

            var compilationOptions = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    allowUnsafe: true);

            var compilation = CSharpCompilation.Create(
                                    "Test",
                                    syntaxTrees: new[] { node.SyntaxTree },
                                    references: references,
                                    options: compilationOptions);

            var semanticModel = compilation.GetSemanticModel(node.SyntaxTree, true);

            return semanticModel;
        }
    }
}
