using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hpl2Parser.Core.Parsing.Syntax;
using Xunit;

namespace Hpl2Parser.Tests.Parsing;

internal static class AssertExtesions
{
    internal static TRootNodeType AssertRoot<TRootNodeType>(this HplSyntaxTree syntaxTree)
    {
        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        var rootElement = syntaxTree.RootElements.ElementAtOrDefault(0);
        return Assert.IsType<TRootNodeType>(rootElement);
    }

    internal static void AssertReturnType(this HplFunctionDeclarationNode functionNode, string returnTypeValue)
    {
        Assert.Equal(returnTypeValue, functionNode.ReturnType.TextValue);
    }

    internal static IEnumerable<HplFunctionParameterNode> AssertParameterListCount(this HplFunctionDeclarationNode functionNode, int count)
    {
        Assert.Equal(count, functionNode.ParameterList.Count);
        Assert.True(functionNode.ParameterList.All(n => n is HplFunctionParameterNode));
        return functionNode.ParameterList.Cast<HplFunctionParameterNode>();
    }
}
