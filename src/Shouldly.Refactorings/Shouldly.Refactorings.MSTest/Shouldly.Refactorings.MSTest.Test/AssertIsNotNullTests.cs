namespace Shouldly.Refactorings.MSTest.Test
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shouldly.Refactorings.MSTest.Analyzers;
    using Shouldly.Refactorings.MSTest.CodeFixes;
    using System.Text;
    using TestHelper;

    [TestClass]
    [TestCategory("MSTest - Assert.IsNotNull")]
    public class AssertIsNotNullTests : CodeFixVerifier
    {
        [TestMethod]
        public void WithAssertIsNotNull_NoMessage_ChangesToShouldlyAssert()
        {
            var testCodeBuilder = new StringBuilder();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("    using Shouldly;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("            Assert.IsNotNull(testValue);")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var startCode = testCodeBuilder.ToString();
            testCodeBuilder.Clear();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("    using Shouldly;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("            testValue.ShouldNotBeNull();")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertIsNotNull,
                Message = "Assert.IsNotNull(testValue) should be replaced with testValue.ShouldNotBeNull()",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithAssertIsNotNull_WithFormattedMessage_ChangesToShouldlyAssertWithFormattedMessage()
        {
            var testCodeBuilder = new StringBuilder();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("    using Shouldly;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("            Assert.IsNotNull(testValue, \"The object '{0}' was null\", nameof(testValue));")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var startCode = testCodeBuilder.ToString();
            testCodeBuilder.Clear();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("    using Shouldly;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("            testValue.ShouldNotBeNull(string.Format(\"The object '{0}' was null\", nameof(testValue)));")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertIsNotNull,
                Message = "Assert.IsNotNull(testValue) should be replaced with testValue.ShouldNotBeNull()",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithAssertIsNotNull_WithMessage_ChangesToShouldlyAssertWithMessage()
        {
            var testCodeBuilder = new StringBuilder();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("    using Shouldly;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("            Assert.IsNotNull(testValue, \"The object was null\");")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var startCode = testCodeBuilder.ToString();
            testCodeBuilder.Clear();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("    using Shouldly;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("            testValue.ShouldNotBeNull(\"The object was null\");")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertIsNotNull,
                Message = "Assert.IsNotNull(testValue) should be replaced with testValue.ShouldNotBeNull()",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithoutAssertIsNotNull_NoDiagnosticCreated()
        {
            var testCodeBuilder = new StringBuilder();

            testCodeBuilder
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
                .AppendLine("    using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("")
                .AppendLine("    [TestClass]")
                .AppendLine("    public class TypeName")
                .AppendLine("    {")
                .AppendLine("        [TestMethod]")
                .AppendLine("        public void DoSomething()")
                .AppendLine("        {")
                .AppendLine("            string testValue = null;")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var startCode = testCodeBuilder.ToString();

            VerifyCSharpDiagnostic(startCode);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AssertIsNotNullCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AssertIsNotNullAnalyzer();
        }
    }
}
