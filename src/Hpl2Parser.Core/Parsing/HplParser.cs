﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hpl2Parser.Core.Parsing.Syntax;
using Hpl2Parser.Core.Tokenizing;
using Hpl2Parser.Core.Utility;

namespace Hpl2Parser.Core.Parsing
{
    public class HplParser
    {
        private readonly IHplTokenizer _hplTokenizer;

        private IEnumerable<HplToken> _tokens = Array.Empty<HplToken>();

        private TokenEnumerator _tokenEnumerator;

        private IEnumerable<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics.ToList();

        public HplParser(IHplTokenizer hplTokenizer)
        {
            _hplTokenizer = hplTokenizer;
        }

        public void SetTokens(IEnumerable<HplToken> tokens)
        {
            _tokens = tokens;
            _tokenEnumerator = new(tokens);
        }

        public void Tokenize(string fileText)
        {
            var spanWindow = new ReadOnlySpan<char>(fileText.ToCharArray());

            var tokens = new List<HplToken>();
            while (!spanWindow.IsEmpty)
            {
                var token = _hplTokenizer.GetToken(ref spanWindow);
                tokens.Add(token);
            }

            SetTokens(tokens);
        }

        public HplSyntaxTree Parse()
        {
            var tree =  new HplSyntaxTree();

            while(_tokenEnumerator.Peek().Type != HplTokenType.EndOfFile)
                tree.RootElements.Add(ResolveTokenExpression());

            return tree;
        }

        private HplSyntaxNode ResolveTokenExpression()
        {
            if (!_tokens.Any())
                throw new InvalidOperationException("Resolving token expression cannot continue because the token list is empty");

            if (_tokenEnumerator is null)
                throw new InvalidOperationException("Resolving token expression cannot continue because the token enumerator is null");

            var token = _tokenEnumerator.Peek();

            if (MatchesFunctionDeclaration(token))
                return ParseAsFunctionDeclaration();

            throw new NotImplementedException("Encountered a token expression that was not a function declaraion - Not Implemented");
        }

        private bool MatchesFunctionDeclaration(HplToken token) => token.Type == HplTokenType.Identifier && token.Text.IsReturnType() && _tokenEnumerator.Peek(1).Type == HplTokenType.Identifier && _tokenEnumerator.Peek(2).Type == HplTokenType.OpenParen;

        private HplSyntaxNode ParseAsFunctionDeclaration()
        {
            var funcNode = new HplFunctionDeclarationNode();

            funcNode.ReturnType = new()
            {
                TextValue = _tokenEnumerator.Peek().Text
            };
            _tokenEnumerator.Next();

            funcNode.Identifier = _tokenEnumerator.Peek().Text;
            _tokenEnumerator.Next();

            if (_tokenEnumerator.Peek().Type != HplTokenType.OpenParen)
                throw new InvalidOperationException($"Expected a token of type OpenParen but found {_tokenEnumerator.Peek().Type}");
            _tokenEnumerator.Next();

            funcNode.ParameterList = new List<HplSyntaxNode>();

            while (_tokenEnumerator.Peek().Type != HplTokenType.CloseParen)
            {
                if (_tokenEnumerator.Peek().Type == HplTokenType.Comma)
                    _tokenEnumerator.Next();

                if (_tokenEnumerator.Peek().Type == HplTokenType.EndOfFile)
                    throw new InvalidOperationException("Expected a token of type CloseParen but found End of file");

                var paramNode = new HplFunctionParameterNode();

                if (_tokenEnumerator.Peek().Type == HplTokenType.Identifier
                    && _tokenEnumerator.Peek(1).Type == HplTokenType.Identifier
                    && _tokenEnumerator.Peek(2).Type == HplTokenType.Identifier)
                {
                    // NOTE(Peter): This argument specifies intention
                    if (!_tokenEnumerator.Peek(1).Text.IsIntentionMark())
                        throw new InvalidOperationException($"Expected an intention identifier but found {_tokenEnumerator.Peek(1).Text}");

                    paramNode.Type = _tokenEnumerator.Peek().Text;
                    paramNode.Intention = _tokenEnumerator.Peek(1).Text.ToIntention();
                    paramNode.Identifier = _tokenEnumerator.Peek(2).Text;
                    
                    _tokenEnumerator.Next(3);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(_tokenEnumerator.Peek().Type == HplTokenType.Identifier);
                    System.Diagnostics.Debug.Assert(_tokenEnumerator.Peek(1).Type == HplTokenType.Identifier);

                    paramNode.Type = _tokenEnumerator.Peek().Text;
                    paramNode.Identifier = _tokenEnumerator.Peek(1).Text;
                    
                    _tokenEnumerator.Next(2);
                }

                funcNode.ParameterList.Add(paramNode);
            }

            // for parameterless functions
            if (_tokenEnumerator.Peek().Type == HplTokenType.CloseParen)
                _tokenEnumerator.Next();

            if (_tokenEnumerator.Peek().Type != HplTokenType.OpenBracket)
                throw new InvalidOperationException($"Expected {{ but found {_tokenEnumerator.Peek().Type}");

            // '{'
            _tokenEnumerator.Next();

            funcNode.BodyElements = new List<HplSyntaxNode>();
            while (_tokenEnumerator.Peek().Type != HplTokenType.CloseBracket)
            {
                // Function Call
                if (_tokenEnumerator.Peek().Type == HplTokenType.Identifier && _tokenEnumerator.Peek(1).Type == HplTokenType.OpenParen)
                {
                    var functionCall = new HplFunctionCallNode()
                    {
                        Identifier = _tokenEnumerator.Peek().Text
                    };
                    _tokenEnumerator.Next(2);

                    if (_tokenEnumerator.Peek().Type == HplTokenType.CloseParen)
                    {
                        _tokenEnumerator.Next();
                    }
                    else
                    {
                        while (_tokenEnumerator.Peek().Type != HplTokenType.CloseParen)
                        {
                            var argument = ParseFunctionCallArgument();

                            if (argument is null)
                                throw new NotImplementedException("There was no implementation for parsing the specified function call argument");

                            functionCall.Arguments.Add(argument);

                            if (_tokenEnumerator.Peek().Type == HplTokenType.Comma)
                                _tokenEnumerator.Next(); // TODO: Missing commas should be reported in diagnostics
                        }
                        _tokenEnumerator.Next(); // CloseParen - end of arguments
                    }

                    if (_tokenEnumerator.Peek().Type != HplTokenType.Semicolon)
                        throw new NotImplementedException("Complex function calls are not supported at the moment... function calls must be followed by a semicolon");
                    else
                        _tokenEnumerator.Next();

                    funcNode.BodyElements.Add(functionCall);
                }
                else if (_tokenEnumerator.Peek().Type == HplTokenType.InlineComment || _tokenEnumerator.Peek().Type == HplTokenType.MultilineComment)
                {
                    _tokenEnumerator.Next();
                }
                else
                {
                    // TODO: Infinite loop prevention, not implemented
                    throw new NotImplementedException($"{_tokenEnumerator.Peek().Type} found inside body - but we expected a function call (not implemented)");
                }
            }

            _tokenEnumerator.Next(1);

            return funcNode;
        }

        private HplFunctionArgumentNode ParseFunctionCallArgument()
        {
            if (_tokenEnumerator.Peek().Type == HplTokenType.StringLiteral) {
                var arg = new HplFunctionArgumentNode
                {
                    ArgumentType = HplFunctionCallArgumentType.StringLiteral,
                    ArgumentValue = _tokenEnumerator.Peek().Text
                };

                _tokenEnumerator.Next();
                return arg;
            }

            if (_tokenEnumerator.Peek().Type == HplTokenType.Number)
            {
                // TODO: Support more number types
                var arg = new HplFunctionArgumentNode
                {
                    ArgumentType = HplFunctionCallArgumentType.FloatLiteral,
                    ArgumentValue = _tokenEnumerator.Peek().Text
                };

                _tokenEnumerator.Next();
                return arg;
            }

            if (_tokenEnumerator.Peek().Type == HplTokenType.Identifier) {
                return ParseFunctionCallIdentifierArgument();
            }

            throw new NotImplementedException($"{_tokenEnumerator.Peek().Type} is not implemented as a function call argument at this time...");
        }

        private HplFunctionArgumentNode ParseFunctionCallIdentifierArgument()
        {
            var identifier = _tokenEnumerator.Peek().Text;
            if (identifier == "true" || identifier == "false") { // boolean literal
                _tokenEnumerator.Next();
                return new HplFunctionArgumentNode
                {
                    ArgumentType = HplFunctionCallArgumentType.BooleanLiteral,
                    ArgumentValue = identifier
                };
            }

            _tokenEnumerator.Next();
            return new HplFunctionArgumentNode
            {
                ArgumentType = HplFunctionCallArgumentType.VariableReference,
                ArgumentValue = identifier
            };
        }
    }
}
