namespace Shouldly.Refactorings.MSTest.CodeFixes
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class AssertIsNotNullCodeFixProvider : AssertIsNullCodeFixProvider
    {
        protected override string METHOD_NAME => "ShouldNotBeNull";
    }
}
