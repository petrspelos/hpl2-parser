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
        _parser.SetTokens(new TokenBuilder()
            .FunctionDeclaration(expectedReturnType, "MyFunction")
            .EndOfFile()
            .Build()
        );

        HplSyntaxTree syntaxTree = _parser.Parse();

        var functionNode = syntaxTree.AssertRoot<HplFunctionDeclarationNode>();

        functionNode.Identifier.Should().Be("MyFunction");
        functionNode.AssertReturnType(expectedReturnType);
        _parser.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void ValidFunctionDeclaration_ShouldRecognizeMultipleParametersWithParameterIntention()
    {
        _parser.SetTokens(new TokenBuilder()
            .FunctionDeclaration("void", "MyFunction", paramBuilder =>
            {
                paramBuilder.Parameter("string", "myArg", HplFunctionParameterIntention.In);
                paramBuilder.Comma();
                paramBuilder.Parameter("int", "myOtherArg", HplFunctionParameterIntention.Out);
                paramBuilder.Comma();
                paramBuilder.Parameter("bool", "&referenceArgument");
            })
            .EndOfFile()
            .Build()
        );

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
        _parser.SetTokens(new TokenBuilder()
            .FunctionDeclaration("void", "MyFunction", paramBuilder =>
            {
                paramBuilder.Parameter("string", "myArg", HplFunctionParameterIntention.In);
            }, bodyBuilder =>
            {
                bodyBuilder.FunctionCall("FunctionToCall");
            })
            .EndOfFile()
            .Build()
        );

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
        _parser.SetTokens(new TokenBuilder()
            .FunctionDeclaration("void", "MyFunction", null, bodyBuilder =>
            {
                bodyBuilder.FunctionCall("Print", argsBuilder =>
                {
                    argsBuilder.StringLiteral("Hello, World!");
                });
            })
            .EndOfFile()
            .Build()
        );

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
        _parser.SetTokens(new TokenBuilder()
            .FunctionDeclaration("void", "MyFunction", null, bodyBuilder =>
            {
                bodyBuilder.FunctionCall("CreateParticleSystemAtEntity", argsBuilder =>
                {
                    argsBuilder.StringLiteral("confetti");
                    argsBuilder.Comma();
                    argsBuilder.StringLiteral("confetti.ps");
                    argsBuilder.Comma();
                    argsBuilder.StringLiteral("CeilingArea");
                    argsBuilder.Comma();
                    argsBuilder.Identifier("true");
                });
            })
            .EndOfFile()
            .Build()
        );

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
