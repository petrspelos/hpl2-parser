using Hpl2Parser.Core.Parsing.Syntax;
using Hpl2Parser.Core.Tokenizing;
using System;
using System.Collections.Generic;

namespace Hpl2Parser.Core;

public class TokenBuilder
{
    private readonly IList<HplToken> _tokens = [];
    public TokenBuilder FunctionDeclaration(string returnType, string functionName, Action<TokenBuilder> parameterBuilder = null, Action<TokenBuilder> bodyBuilder = null)
    {
        Identifier(returnType);
        Identifier(functionName);
        OpenParen();
        parameterBuilder?.Invoke(this);
        CloseParen();
        OpenBracket();
        bodyBuilder?.Invoke(this);
        CloseBracket();
        return this;
    }

    public TokenBuilder OpenParen()
    {
        _tokens.Add(new HplToken(HplTokenType.OpenParen));
        return this;
    }

    public TokenBuilder CloseParen()
    {
        _tokens.Add(new HplToken(HplTokenType.CloseParen));
        return this;
    }

    public TokenBuilder OpenBracket()
    {
        _tokens.Add(new HplToken(HplTokenType.OpenBracket));
        return this;
    }

    public TokenBuilder CloseBracket()
    {
        _tokens.Add(new HplToken(HplTokenType.CloseBracket));
        return this;
    }

    public TokenBuilder Identifier(string text)
    {
        _tokens.Add(new HplToken(HplTokenType.Identifier, text));
        return this;
    }

    public TokenBuilder Semicolon()
    {
        _tokens.Add(new HplToken(HplTokenType.Semicolon));
        return this;
    }

    public TokenBuilder EndOfFile()
    {
        _tokens.Add(new HplToken(HplTokenType.EndOfFile));
        return this;
    }

    public TokenBuilder Parameter(string type, string name, HplFunctionParameterIntention intention = HplFunctionParameterIntention.None)
    {
        Identifier(type);
        if (intention != HplFunctionParameterIntention.None)
        {
            _tokens.Add(new HplToken(
                    HplTokenType.Identifier,
                    intention == HplFunctionParameterIntention.In ? "&in" : "&out"));
        }
        Identifier(name);
        return this;
    }

    public TokenBuilder Comma()
    {
        _tokens.Add(new HplToken(HplTokenType.Comma));
        return this;
    }

    public TokenBuilder StringLiteral(string value)
    {
        _tokens.Add(new HplToken(HplTokenType.StringLiteral, value));
        return this;
    }

    public TokenBuilder NumberLiteral(string value)
    {
        _tokens.Add(new HplToken(HplTokenType.Number, value));
        return this;
    }

    public TokenBuilder FunctionCall(string functionName, Action<TokenBuilder> argsBuilder = null)
    {
        Identifier(functionName);
        OpenParen();
        argsBuilder?.Invoke(this);
        CloseParen();
        Semicolon();
        return this;
    }

    public IEnumerable<HplToken> Build() => _tokens;
}
