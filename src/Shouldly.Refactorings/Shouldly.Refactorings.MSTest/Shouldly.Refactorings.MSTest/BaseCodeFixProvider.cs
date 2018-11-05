namespace Shouldly.Refactorings.MSTest
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Linq;

    public abstract class BaseCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The shouldly namespace
        /// </summary>
        private const string SHOULDLY_NAMESPACE = "Shouldly";

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
    }
}