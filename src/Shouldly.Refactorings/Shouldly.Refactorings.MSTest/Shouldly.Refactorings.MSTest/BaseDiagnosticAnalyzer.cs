namespace Shouldly.Refactorings.MSTest
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    public abstract class BaseDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The MSTest namespace
        /// </summary>
        private const string MsTestNamespace = "Microsoft.VisualStudio.TestTools.UnitTesting";

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected SyntaxNodeAnalysisContext Context { get; set; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// Builds the rule.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="titleKey">The title key.</param>
        /// <param name="formatKey">The format key.</param>
        /// <param name="categoryKey">The category key.</param>
        /// <param name="descriptionKey">The description key.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="enabledByDefault">if set to <c>true</c> [enabled by default].</param>
        /// <returns></returns>
        protected static DiagnosticDescriptor BuildRule(string id, string titleKey, string formatKey, string categoryKey, string descriptionKey, DiagnosticSeverity severity, bool enabledByDefault = true)
        {
            var title = GetLocalizedResourceString(titleKey);
            var messageFormat = GetLocalizedResourceString(formatKey);
            var category = GetLocalizedResourceString(categoryKey);
            var description = GetLocalizedResourceString(descriptionKey);

            return new DiagnosticDescriptor(
                id,
                title,
                messageFormat,
                category.ToString(),
                severity,
                isEnabledByDefault: enabledByDefault,
                description: description);
        }

        /// <summary>
        /// Analyzes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void Analyze(SyntaxNodeAnalysisContext context)
        {
            Context = context;

            if (!HasMsTestUsingDirective())
            {
                return;
            }

            AnalyzeInvocation();
        }

        /// <summary>
        /// Analyzes the invocation.
        /// </summary>
        protected abstract void AnalyzeInvocation();

        /// <summary>
        /// Raises the diagnostic.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="location">The location.</param>
        /// <param name="args">The arguments.</param>
        protected void RaiseDiagnostic(DiagnosticDescriptor rule, Location location, params object[] args)
        {
            var diagnostic = Diagnostic.Create(rule, location, args);
            Context.ReportDiagnostic(diagnostic);
        }

        /// <summary>
        /// Gets the localized resource string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The localized resource string</returns>
        private static LocalizableString GetLocalizedResourceString(string name)
        {
            return new LocalizableResourceString(name, Resources.ResourceManager, typeof(Resources));
        }

        /// <summary>
        /// Determines whether has the current compilation unit has a using directive referencing the MSTest namespace.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the current compilation unit has a using directive referencing the MSTest namespace; otherwise, <c>false</c>.
        /// </returns>
        private bool HasMsTestUsingDirective()
        {
            var root = Context.Node.SyntaxTree.GetRoot() as CompilationUnitSyntax;

            var usingDirectives = root
                                      .DescendantNodes()
                                      .Where(x => x.Kind() == SyntaxKind.UsingDirective)
                                      .Select(x => x as UsingDirectiveSyntax);

            return usingDirectives.Any(x =>
            {
                var usingNamespace = x.Name.ToString();
                return usingNamespace == MsTestNamespace;
            });
        }
    }
}