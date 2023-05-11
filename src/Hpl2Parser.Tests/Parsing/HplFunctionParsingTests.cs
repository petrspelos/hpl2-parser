using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Core.Parsing.Syntax;
using Hpl2Parser.Core.Tokenizing;
using System.Linq;
using Xunit;

namespace Hpl2Parser.Tests.Parsing;

public class HplFunctionParsingTests
{
    private readonly HplParser _parser;

    public HplFunctionParsingTests()
    {
        _parser = new(null);
    }


    [Fact]
    public void ValidFunctionDeclaration_ShouldBeRecognized()
    {
        _parser.SetTokens(new HplToken[]
        {
                new(HplTokenType.Identifier, "void"),
                new(HplTokenType.Identifier, "MyFunction"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.CloseParen),
                new(HplTokenType.OpenBracket),
                new(HplTokenType.CloseBracket),
                new(HplTokenType.EndOfFile)
        });

        HplSyntaxTree syntaxTree = _parser.Parse();

        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        Assert.Equal(HplSyntaxNodeType.FunctionDeclaration, syntaxTree.RootElements.First().NodeType);

        var functionNode = syntaxTree.RootElements.First() as HplFunctionDeclarationNode;
        Assert.Equal("MyFunction", functionNode.Identifier);
        Assert.Empty(functionNode.ParameterList);

        Assert.Empty(_parser.Diagnostics);
    }

    [Theory]
    [InlineData("string")]
    [InlineData("void")]
    [InlineData("bool")]
    [InlineData("int8")]
    [InlineData("int16")]
    [InlineData("int")]
    [InlineData("int64")]
    [InlineData("uint8")]
    [InlineData("uint16")]
    [InlineData("uint")]
    [InlineData("uint64")]
    [InlineData("float")]
    public void ValidFunctionDeclaration_ShouldRecognizeReturnType(string expectedIdentifier)
    {
        _parser.SetTokens(new HplToken[]
        {
                new(HplTokenType.Identifier, expectedIdentifier),
                new(HplTokenType.Identifier, "MyFunction"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.CloseParen),
                new(HplTokenType.OpenBracket),
                new(HplTokenType.CloseBracket),
                new(HplTokenType.EndOfFile)
        });

        HplSyntaxTree syntaxTree = _parser.Parse();

        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        Assert.Equal(HplSyntaxNodeType.FunctionDeclaration, syntaxTree.RootElements.First().NodeType);

        var functionNode = syntaxTree.RootElements.First() as HplFunctionDeclarationNode;
        Assert.Equal("MyFunction", functionNode.Identifier);
        Assert.Equal(expectedIdentifier, functionNode.ReturnType.TextValue);

        Assert.Empty(_parser.Diagnostics);
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeSingleParameter()
    {
        _parser.SetTokens(new HplToken[]
        {
                new(HplTokenType.Identifier, "void"),
                new(HplTokenType.Identifier, "MyFunction"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.Identifier, "string"),
                new(HplTokenType.Identifier, "myArg"),
                new(HplTokenType.CloseParen),
                new(HplTokenType.OpenBracket),
                new(HplTokenType.CloseBracket),
                new(HplTokenType.EndOfFile)
        });

        HplSyntaxTree syntaxTree = _parser.Parse();

        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        Assert.Equal(HplSyntaxNodeType.FunctionDeclaration, syntaxTree.RootElements.First().NodeType);

        var functionNode = syntaxTree.RootElements.First() as HplFunctionDeclarationNode;
        Assert.Equal("MyFunction", functionNode.Identifier);
        Assert.Equal("void", functionNode.ReturnType.TextValue);

        Assert.Single(functionNode.ParameterList);
        Assert.Equal(HplSyntaxNodeType.FunctionParameter, functionNode.ParameterList.First().NodeType);
        var param1 = functionNode.ParameterList.First() as HplFunctionParameterNode;
        Assert.NotNull(param1);
        Assert.Equal("string", param1.Type);
        Assert.Equal("myArg", param1.Identifier);

        Assert.Empty(_parser.Diagnostics);
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeMultipleParameters()
    {
        _parser.SetTokens(new HplToken[]
        {
                new(HplTokenType.Identifier, "void"),
                new(HplTokenType.Identifier, "MyFunction"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.Identifier, "string"),
                new(HplTokenType.Identifier, "myArg"),
                new(HplTokenType.Comma),
                new(HplTokenType.Identifier, "int"),
                new(HplTokenType.Identifier, "myOtherArg"),
                new(HplTokenType.Comma),
                new(HplTokenType.Identifier, "bool"),
                new(HplTokenType.Identifier, "lastArg"),
                new(HplTokenType.CloseParen),
                new(HplTokenType.OpenBracket),
                new(HplTokenType.CloseBracket),
                new(HplTokenType.EndOfFile)
        });

        HplSyntaxTree syntaxTree = _parser.Parse();

        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        Assert.Equal(HplSyntaxNodeType.FunctionDeclaration, syntaxTree.RootElements.First().NodeType);

        var functionNode = syntaxTree.RootElements.First() as HplFunctionDeclarationNode;
        Assert.Equal("MyFunction", functionNode.Identifier);
        Assert.Equal("void", functionNode.ReturnType.TextValue);

        Assert.Equal(3, functionNode.ParameterList.Count);
        Assert.Equal(HplSyntaxNodeType.FunctionParameter, functionNode.ParameterList.First().NodeType);

        var param1 = functionNode.ParameterList.ElementAt(0) as HplFunctionParameterNode;
        Assert.NotNull(param1);
        Assert.Equal("string", param1.Type);
        Assert.Equal("myArg", param1.Identifier);

        var param2 = functionNode.ParameterList.ElementAt(1) as HplFunctionParameterNode;
        Assert.NotNull(param2);
        Assert.Equal("int", param2.Type);
        Assert.Equal("myOtherArg", param2.Identifier);

        var param3 = functionNode.ParameterList.ElementAt(2) as HplFunctionParameterNode;
        Assert.NotNull(param3);
        Assert.Equal("bool", param3.Type);
        Assert.Equal("lastArg", param3.Identifier);

        Assert.Empty(_parser.Diagnostics);
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeMultipleParametersWithParameterIntention()
    {
        _parser.SetTokens(new HplToken[]
        {
                new(HplTokenType.Identifier, "void"),
                new(HplTokenType.Identifier, "MyFunction"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.Identifier, "string"),
                new(HplTokenType.Identifier, "&in"),
                new(HplTokenType.Identifier, "myArg"),
                new(HplTokenType.Comma),
                new(HplTokenType.Identifier, "int"),
                new(HplTokenType.Identifier, "&out"),
                new(HplTokenType.Identifier, "myOtherArg"),
                new(HplTokenType.Comma),
                new(HplTokenType.Identifier, "bool"),
                new(HplTokenType.Identifier, "&referenceArgument"),
                new(HplTokenType.CloseParen),
                new(HplTokenType.OpenBracket),
                new(HplTokenType.CloseBracket),
                new(HplTokenType.EndOfFile)
        });

        HplSyntaxTree syntaxTree = _parser.Parse();

        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        Assert.Equal(HplSyntaxNodeType.FunctionDeclaration, syntaxTree.RootElements.First().NodeType);

        var functionNode = syntaxTree.RootElements.First() as HplFunctionDeclarationNode;
        Assert.Equal("MyFunction", functionNode.Identifier);
        Assert.Equal("void", functionNode.ReturnType.TextValue);

        Assert.Equal(3, functionNode.ParameterList.Count);
        Assert.Equal(HplSyntaxNodeType.FunctionParameter, functionNode.ParameterList.First().NodeType);

        var param1 = functionNode.ParameterList.ElementAt(0) as HplFunctionParameterNode;
        Assert.NotNull(param1);
        Assert.Equal("string", param1.Type);
        Assert.Equal(HplFunctionParameterIntention.In, param1.Intention);
        Assert.Equal("myArg", param1.Identifier);

        var param2 = functionNode.ParameterList.ElementAt(1) as HplFunctionParameterNode;
        Assert.NotNull(param2);
        Assert.Equal("int", param2.Type);
        Assert.Equal(HplFunctionParameterIntention.Out, param2.Intention);
        Assert.Equal("myOtherArg", param2.Identifier);

        var param3 = functionNode.ParameterList.ElementAt(2) as HplFunctionParameterNode;
        Assert.NotNull(param3);
        Assert.Equal("bool", param3.Type);
        Assert.Equal(HplFunctionParameterIntention.None, param3.Intention);
        Assert.Equal("&referenceArgument", param3.Identifier);

        Assert.Empty(_parser.Diagnostics);
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeFunctionCall()
    {
        _parser.SetTokens(new HplToken[]
        {
                new(HplTokenType.Identifier, "void"),
                new(HplTokenType.Identifier, "MyFunction"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.Identifier, "string"),
                new(HplTokenType.Identifier, "&in"),
                new(HplTokenType.Identifier, "myArg"),
                new(HplTokenType.CloseParen),
                new(HplTokenType.OpenBracket),
                new(HplTokenType.Identifier, "FunctionToCall"),
                new(HplTokenType.OpenParen),
                new(HplTokenType.CloseParen),
                new(HplTokenType.CloseBracket),
                new(HplTokenType.EndOfFile)
        });

        HplSyntaxTree syntaxTree = _parser.Parse();

        Assert.NotNull(syntaxTree);
        Assert.Single(syntaxTree.RootElements);
        Assert.Equal(HplSyntaxNodeType.FunctionDeclaration, syntaxTree.RootElements.First().NodeType);

        var functionNode = syntaxTree.RootElements.First() as HplFunctionDeclarationNode;
        Assert.Single(functionNode.BodyElements);
        var functionCallNode = functionNode.BodyElements.First() as HplFunctionCallNode;
        Assert.NotNull(functionCallNode);
        Assert.Equal("FunctionToCall", functionCallNode.Identifier);

        Assert.Empty(_parser.Diagnostics);
    }
}
