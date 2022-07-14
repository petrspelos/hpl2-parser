using System;
using System.Collections.Generic;
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

            _tokens = tokens;
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

            throw new NotImplementedException();
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
                throw new NotImplementedException("Complex functions are not yet implemented");
            _tokenEnumerator.Next();

            funcNode.ParameterList = Array.Empty<HplSyntaxNode>();

            if (_tokenEnumerator.Peek().Type != HplTokenType.CloseParen)
                throw new NotImplementedException("Complex functions are not yet implemented");
            _tokenEnumerator.Next();

            // '{' and '}'
            _tokenEnumerator.Next(2);

            return funcNode;
        }
    }
}
