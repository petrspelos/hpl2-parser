using FluentAssertions;
using Hpl2Parser.Core;
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
        _parser.SetTokens(new TokenBuilder()
            .FunctionDeclaration("void", "MyFunction")
            .EndOfFile()
            .Build()
        );

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();

        functionNode.Identifier.Should().Be("MyFunction");
        functionNode.ParameterList.Should().BeEmpty();
        _parser.Diagnostics.Should().BeEmpty();
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
    public void ValidFunctionDeclaration_ShouldRecognizeReturnType(string expectedReturnType)
    {
        _parser.SetTokens(
        [
            new(HplTokenType.Identifier, expectedReturnType),
            new(HplTokenType.Identifier, "MyFunction"),
            new(HplTokenType.OpenParen),
            new(HplTokenType.CloseParen),
            new(HplTokenType.OpenBracket),
            new(HplTokenType.CloseBracket),
            new(HplTokenType.EndOfFile)
        ]);

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();

        functionNode.Identifier.Should().Be("MyFunction");
        functionNode.AssertReturnType(expectedReturnType);
        _parser.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeMultipleParametersWithParameterIntention()
    {
        _parser.SetTokens(
        [
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
        ]);

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();
        functionNode.Identifier.Should().Be("MyFunction");
        functionNode.AssertReturnType("void");

        var parameterNodes = functionNode.AssertParameterListCount(3);
        var param1 = parameterNodes.ElementAt(0);
        param1.Type.Should().Be("string");
        param1.Identifier.Should().Be("myArg");
        param1.Intention.Should().Be(HplFunctionParameterIntention.In);

        var param2 = parameterNodes.ElementAt(1);
        param2.Type.Should().Be("int");
        param2.Identifier.Should().Be("myOtherArg");
        param2.Intention.Should().Be(HplFunctionParameterIntention.Out);

        var param3 = parameterNodes.ElementAt(2);
        param3.Type.Should().Be("bool");
        param3.Identifier.Should().Be("&referenceArgument");
        param3.Intention.Should().Be(HplFunctionParameterIntention.None);

        _parser.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeFunctionCall()
    {
        _parser.SetTokens(
        [
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
            new(HplTokenType.Semicolon),
            new(HplTokenType.CloseBracket),
            new(HplTokenType.EndOfFile)
        ]);

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();
        functionNode.Identifier.Should().Be("MyFunction");
        functionNode.AssertReturnType("void");
        functionNode.AssertParameterListCount(1);

        var functionCallNode = functionNode.AssertSingleFunctionCall();
        functionCallNode.Identifier.Should().Be("FunctionToCall");
        functionCallNode.Arguments.Should().HaveCount(0);

        _parser.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void ValidFunctionWithFunctionCallLiteralParameters_ShouldRecognizeFunctionCall()
    {
        _parser.SetTokens(
        [
            new(HplTokenType.Identifier, "void"),
            new(HplTokenType.Identifier, "MyFunction"),
            new(HplTokenType.OpenParen),
            new(HplTokenType.CloseParen),
            new(HplTokenType.OpenBracket),
            new(HplTokenType.Identifier, "Print"),
            new(HplTokenType.OpenParen),
            new(HplTokenType.StringLiteral, "Hello, World!"),
            new(HplTokenType.CloseParen),
            new(HplTokenType.Semicolon),
            new(HplTokenType.CloseBracket),
            new(HplTokenType.EndOfFile)
        ]);

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();
        var functionCallNode = functionNode.AssertSingleFunctionCall();
        functionCallNode.Identifier.Should().Be("Print");
        functionCallNode.AssertSingleArgument(HplFunctionCallArgumentType.StringLiteral, "Hello, World!");

        _parser.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void ValidFunctionWithFunctionCallMultipleLiteralParameters_ShouldRecognizeFunctionCall()
    {
        _parser.SetTokens(
        [
            new(HplTokenType.Identifier, "void"),
            new(HplTokenType.Identifier, "MyFunction"),
            new(HplTokenType.OpenParen),
            new(HplTokenType.CloseParen),
            new(HplTokenType.OpenBracket),
            new(HplTokenType.Identifier, "CreateParticleSystemAtEntity"),
            new(HplTokenType.OpenParen),
            new(HplTokenType.StringLiteral, "confetti"),
            new(HplTokenType.Comma),
            new(HplTokenType.StringLiteral, "confetti.ps"),
            new(HplTokenType.Comma),
            new(HplTokenType.StringLiteral, "CeilingArea"),
            new(HplTokenType.Comma),
            new(HplTokenType.Identifier, "true"),
            new(HplTokenType.CloseParen),
            new(HplTokenType.Semicolon),
            new(HplTokenType.CloseBracket),
            new(HplTokenType.EndOfFile)
        ]);

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();
        var functionCallNode = functionNode.AssertSingleFunctionCall();
        functionCallNode.Identifier.Should().Be("CreateParticleSystemAtEntity");
        functionCallNode.AssertArguments(
            new(HplFunctionCallArgumentType.StringLiteral, "confetti"),
            new(HplFunctionCallArgumentType.StringLiteral, "confetti.ps"),
            new(HplFunctionCallArgumentType.StringLiteral, "CeilingArea"),
            new(HplFunctionCallArgumentType.BooleanLiteral, "true")
        );

        _parser.Diagnostics.Should().BeEmpty();
    }
}
