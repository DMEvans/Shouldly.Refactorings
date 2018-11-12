namespace Shouldly.Refactorings.MSTest.Delegates
{
    using Microsoft.CodeAnalysis;

    public delegate bool OverloadMatchDelegate<T>(T symbol) where T : ISymbol;
}