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
    [TestCategory("MSTest - Assert.IsNull")]
    public class AssertIsNullTests : CodeFixVerifier
    {
        [TestMethod]
        public void WithAssertIsNull_NoMessage_ChangesToShouldlyAssert()
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
                .AppendLine("            Assert.IsNull(testValue);")
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
                .AppendLine("            testValue.ShouldBeNull();")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertIsNull,
                Message = "Assert.IsNull(testValue) should be replaced with testValue.ShouldBeNull()",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithAssertIsNull_WithFormattedMessage_ChangesToShouldlyAssertWithFormattedMessage()
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
                .AppendLine("            Assert.IsNull(testValue, \"The object '{0}' was not null\", nameof(testValue));")
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
                .AppendLine("            testValue.ShouldBeNull(string.Format(\"The object '{0}' was not null\", nameof(testValue)));")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertIsNull,
                Message = "Assert.IsNull(testValue) should be replaced with testValue.ShouldBeNull()",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithAssertIsNull_WithMessage_ChangesToShouldlyAssertWithMessage()
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
                .AppendLine("            Assert.IsNull(testValue, \"The object was not null\");")
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
                .AppendLine("            testValue.ShouldBeNull(\"The object was not null\");")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertIsNull,
                Message = "Assert.IsNull(testValue) should be replaced with testValue.ShouldBeNull()",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithoutAssertIsNull_NoDiagnosticCreated()
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
            return new AssertIsNullCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AssertIsNullAnalyzer();
        }
    }
}