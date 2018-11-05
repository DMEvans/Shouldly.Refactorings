namespace Shouldly.Refactorings.MSTest.Test
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shouldly.Refactorings.MSTest.Analyzers;
    using System.Text;
    using TestHelper;

    [TestClass]
    [TestCategory("MSTest - Assert.AreEqual(expected, actual)")]
    public class AssertAreEqualTests : CodeFixVerifier
    {
        [TestMethod]
        public void NoAssertAreEqual_NoDiagnosticsRaised()
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var startCode = testCodeBuilder.ToString();

            VerifyCSharpDiagnostic(startCode);
        }

        [TestMethod]
        public void WithAssertAreEqual_DiagnosticsRaised()
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            Assert.AreEqual(testValue1, testValue2);")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var startCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertAreEqual,
                Message = "Assert.AreEqual(testValue1, testValue2) should be replaced with testValue1.ShouldBeNull(testValue2)",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return base.GetCSharpCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AssertAreEqualAnalyzer();
        }
    }
}