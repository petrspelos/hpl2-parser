using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Core.Parsing.Syntax;
using Hpl2Parser.Core.Tokenizing;
using System;
using System.Linq;
using Xunit;

namespace Hpl2Parser.Tests
{
    public class HplParserTests
    {
        private readonly HplParser _parser;

        public HplParserTests()
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
    }
}
