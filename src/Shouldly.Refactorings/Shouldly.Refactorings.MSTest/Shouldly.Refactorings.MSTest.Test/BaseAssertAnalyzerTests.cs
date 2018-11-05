namespace Shouldly.Refactorings.MSTest.Test
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shouldly.Refactorings.MSTest.Analyzers;
    using Shouldly.Refactorings.MSTest.CodeFixes;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TestHelper;

    [TestClass]
    [TestCategory("MSTest - Base Assert Code Fix")]
    public class BaseAssertAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void UsingsOutsideNamespace_ShouldlyDoesNotExist_AddsUsingOutsideNamespace()
        {
            var testCodeBuilder = new StringBuilder();

            testCodeBuilder
                .AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("")
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
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
                .AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("using Shouldly;")
                .AppendLine("")
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
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
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UsingsOutsideNamespace_ShouldlyExists_NoUsingChanges()
        {
            var testCodeBuilder = new StringBuilder();

            testCodeBuilder
                .AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("using Shouldly;")
                .AppendLine("")
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
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
                .AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;")
                .AppendLine("using Shouldly;")
                .AppendLine("")
                .AppendLine("namespace ConsoleApplication1")
                .AppendLine("{")
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
        public void UsingsInsideNamespace_ShouldlyDoesNotExist_AddsUsingInsideNamespace()
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
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UsingsInsideNamespace_ShouldlyDoesNotExist_UpdatesCodeWithShouldlyAssert()
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
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 13) }
            };

            VerifyCSharpDiagnostic(startCode, expectedDiagnostic);
            VerifyCSharpFix(startCode, fixedCode, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void UsingsInsideNamespace_ShouldlyExists_NoUsingChanges()
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

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AssertIsNullCodeFix();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AssertIsNullAnalyzer();
        }
    }
}
