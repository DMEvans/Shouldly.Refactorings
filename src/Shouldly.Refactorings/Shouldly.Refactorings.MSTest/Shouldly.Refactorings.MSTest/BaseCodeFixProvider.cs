namespace Shouldly.Refactorings.MSTest
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class BaseCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The shouldly namespace
        /// </summary>
        private const string SHOULDLY_NAMESPACE = "Shouldly";

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        protected abstract string Title { get; }

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
        /// Converts to shouldly assertion.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated document</returns>
        protected abstract Task<Document> ConvertToShouldlyAssertion(Document document, SyntaxNode expression, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the localized resource string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected static LocalizableString GetLocalizedResourceString(string name)
        {
            return new LocalizableResourceString(name, Resources.ResourceManager, typeof(Resources));
        }

        /// <summary>
        /// Updateshouldies the using directive.
        /// </summary>
        /// <param name="compilationUnit">The compilation unit.</param>
        /// <returns></returns>
        protected CompilationUnitSyntax UpdateShouldyUsingDirective(CompilationUnitSyntax compilationUnit)
        {
            var usingDirectiveAlreadyExists = compilationUnit.DescendantNodes().Any(x =>
            {
                if (x.Kind() != SyntaxKind.UsingDirective)
                {
                    return false;
                }

                return (x as UsingDirectiveSyntax).Name.GetText().ToString() == SHOULDLY_NAMESPACE;
            });

            if (usingDirectiveAlreadyExists)
            {
                return compilationUnit;
            }

            var namespaceDeclaration = compilationUnit.ChildNodes().FirstOrDefault(x => x.Kind() == SyntaxKind.NamespaceDeclaration) as NamespaceDeclarationSyntax;
            var addUsingToNamespace = false;

            if (!(namespaceDeclaration is null))
            {
                addUsingToNamespace = namespaceDeclaration.Usings.Any();
            }

            var shouldlyIdentifier = SyntaxFactory.IdentifierName("Shouldly");
            var usingShouldly = SyntaxFactory.UsingDirective(shouldlyIdentifier);

            if (addUsingToNamespace)
            {
                var newNamespaceDeclaration = namespaceDeclaration.AddUsings(usingShouldly);
                return compilationUnit.ReplaceNode(namespaceDeclaration, newNamespaceDeclaration);
            }
            else
            {
                return compilationUnit.AddUsings(usingShouldly);
            }
        }

        protected SemanticModel ObtainSemanticModel(SyntaxTree syntaxTree)
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
                                    syntaxTrees: new[] { syntaxTree },
                                    references: references,
                                    options: compilationOptions);

            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            return semanticModel;
        }
    }
}