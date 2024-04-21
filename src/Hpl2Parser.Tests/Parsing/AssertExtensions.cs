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

    internal static HplFunctionCallNode AssertSingleFunctionCall(this HplFunctionDeclarationNode functionNode)
    {
        Assert.Single(functionNode.BodyElements);
        return functionNode.BodyElements.Cast<HplFunctionCallNode>().First();
    }

    internal static void AssertArgumentCount(this HplFunctionCallNode functionCallNode, int expectedCount)
    {
        Assert.Equal(expectedCount, functionCallNode.Arguments.Count);
    }

    internal static void AssertSingleArgument(this HplFunctionCallNode functionCallNode, HplFunctionCallArgumentType argumentType, object argumentValue)
    {
        Assert.Single(functionCallNode.Arguments);
        var argument = functionCallNode.Arguments.First();
        Assert.Equal(argumentType, argument.ArgumentType);
        Assert.Equal(argumentValue, argument.ArgumentValue);
    }

    internal static void AssertArguments(this HplFunctionCallNode functionCallNode, params (HplFunctionCallArgumentType ArgumentType, object ArgumentValue)[] expectedArguments)
    {
        AssertArgumentCount(functionCallNode, expectedArguments.Length);
        for (int i = 0; i < expectedArguments.Length; i++)
        {
            var actual = functionCallNode.Arguments.ElementAt(i);
            var expected = expectedArguments[i];
            Assert.Equal(expected.ArgumentType, actual.ArgumentType);
            Assert.Equal(expected.ArgumentValue, actual.ArgumentValue);
        }
    }
}
