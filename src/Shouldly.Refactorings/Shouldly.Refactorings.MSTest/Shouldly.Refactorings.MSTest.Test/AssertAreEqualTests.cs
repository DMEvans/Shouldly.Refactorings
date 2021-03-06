﻿namespace Shouldly.Refactorings.MSTest.Test
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
    [TestCategory("MSTest - Assert.AreEqual")]
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
        public void WithAssertAreEqual_Basic_DiagnosticsRaised()
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            Assert.AreEqual(testValue1, testValue2);")
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            testValue2.ShouldBe(testValue1);")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertAreEqual,
                Message = "Assert.AreEqual(testValue1, testValue2) should be replaced with testValue1.ShouldBe(testValue2)",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithAssertAreEqual_WithMessage_DiagnosticsRaised()
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            Assert.AreEqual(testValue1, testValue2, \"The values do not match\");")
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            testValue2.ShouldBe(testValue1, \"The values do not match\");")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertAreEqual,
                Message = "Assert.AreEqual(testValue1, testValue2) should be replaced with testValue1.ShouldBe(testValue2)",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WithAssertAreEqual_WithMessageAndParameters_DiagnosticsRaised()
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            Assert.AreEqual(testValue1, testValue2, \"The values do not match: '{0}' and '{1}'\", testValue1, testValue2);")
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
                .AppendLine("            string testValue1 = \"TEST 1\";")
                .AppendLine("            string testValue2 = \"TEST 2\";")
                .AppendLine("            testValue2.ShouldBe(testValue1, string.Format(\"The values do not match: '{0}' and '{1}'\", testValue1, testValue2));")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}");

            var fixedCode = testCodeBuilder.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.AssertAreEqual,
                Message = "Assert.AreEqual(testValue1, testValue2) should be replaced with testValue1.ShouldBe(testValue2)",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AssertAreEqualCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AssertAreEqualAnalyzer();
        }
    }
}