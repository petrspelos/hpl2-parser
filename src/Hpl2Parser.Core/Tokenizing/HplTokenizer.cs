using System;
using System.Text;

namespace Hpl2Parser.Core.Tokenizing;

public sealed class HplTokenizer : IHplTokenizer
{
    public HplToken GetToken(ref ReadOnlySpan<char> spanWindow)
    {
        var textBuilder = new StringBuilder();
        EatAllWhiteSpace(ref spanWindow);

        if (spanWindow.IsEmpty)
            return new(HplTokenType.EndOfFile);

        if (spanWindow[0] == '/' && spanWindow[1] == '/')
        {
            textBuilder.Append(spanWindow[0]);
            textBuilder.Append(spanWindow[1]);
            spanWindow.MoveForwardBy(2);

            while (!spanWindow.IsEmpty && !spanWindow[0].IsEndOfLine())
            {
                textBuilder.Append(spanWindow[0]);
                spanWindow.MoveForwardBy(1);
            }

            return new(HplTokenType.InlineComment, textBuilder.ToString());
        }

        if (spanWindow[0] == '/' && spanWindow[1] == '*')
        {
            textBuilder.Append(spanWindow[0]);
            textBuilder.Append(spanWindow[1]);
            spanWindow.MoveForwardBy(2);

            while (spanWindow[0] != '*' || spanWindow[1] != '/')
            {
                textBuilder.Append(spanWindow[0]);
                spanWindow.MoveForwardBy(1);

                if (spanWindow.IsEmpty)
                {
                    // NOTE(Peter): Expected "/*" but found the end of file instead
                    return new(HplTokenType.InvalidToken);
                }
            }

            textBuilder.Append(spanWindow[0]);
            textBuilder.Append(spanWindow[1]);
            spanWindow.MoveForwardBy(2);

            return new(HplTokenType.MultilineComment, textBuilder.ToString());
        }

        if (spanWindow[0] == '"')
        {
            textBuilder.Append(spanWindow[0]);
            spanWindow.MoveForwardBy(1);

            while (spanWindow[0] != '"')
            {
                if (spanWindow[0].IsEndOfLine())
                {
                    // NOTE(Peter): string literal's cannot contain unescaped newlines
                    return new(HplTokenType.InvalidToken);
                }

                if (spanWindow[0] == '\\' && spanWindow[1] == '"')
                {
                    textBuilder.Append(spanWindow[0]);
                    textBuilder.Append(spanWindow[1]);
                    spanWindow.MoveForwardBy(2);
                }
                else
                {
                    textBuilder.Append(spanWindow[0]);
                    spanWindow.MoveForwardBy(1);
                }

                if (spanWindow.IsEmpty)
                {
                    // NOTE(Peter): Expected closing " but found the end of file instead
                    return new(HplTokenType.InvalidToken);
                }
            }

            textBuilder.Append(spanWindow[0]);
            spanWindow.MoveForwardBy(1);

            return new(HplTokenType.StringLiteral, textBuilder.ToString());
        }

        if (spanWindow.Length >= 2 && spanWindow[0] == '<' && spanWindow[1] == '=')
        {
            spanWindow.MoveForwardBy(2);
            return new(HplTokenType.BooleanLessThan);
        }
        
        if (spanWindow.Length >= 2 && spanWindow[0] == '>' && spanWindow[1] == '=')
        {
            spanWindow.MoveForwardBy(2);
            return new(HplTokenType.BooleanMoreThan);
        }
        
        if (spanWindow.Length >= 2 && spanWindow[0] == '!' && spanWindow[1] == '=')
        {
            spanWindow.MoveForwardBy(2);
            return new(HplTokenType.NotEqualSign);
        }
        
        if (spanWindow.Length >= 2 && spanWindow[0] == '|' && spanWindow[1] == '|')
        {
            spanWindow.MoveForwardBy(2);
            return new(HplTokenType.BooleanOrSign);
        }
        
        if (spanWindow.Length >= 2 && spanWindow[0] == '&' && spanWindow[1] == '&')
        {
            spanWindow.MoveForwardBy(2);
            return new(HplTokenType.BooleanAndSign);
        }
        
        if (spanWindow[0] == '(')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.OpenParen);
        }

        if (spanWindow[0] == ')')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.CloseParen);
        }

        if (spanWindow[0] == ',')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.Comma);
        }

        if (spanWindow[0] == '&' && char.IsWhiteSpace(spanWindow[1]))
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.Ampersand);
        }

        if (spanWindow[0] == '{')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.OpenBracket);
        }

        if (spanWindow[0] == '}')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.CloseBracket);
        }

        if (spanWindow[0] == ';')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.Semicolon);
        }
        
        if (spanWindow[0] == ':')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.Colon);
        }
        
        if (spanWindow[0] == '!')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.ExclamationPoint);
        }
        
        if (spanWindow[0] == '<')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.LessThanSign);
        }
        
        if (spanWindow[0] == '>')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.MoreThanSign);
        }
        
        if (spanWindow[0] == '%')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.PercentageSign);
        }
        
        if (spanWindow[0] == '=' && spanWindow[1] == '=')
        {
            spanWindow.MoveForwardBy(2);
            return new(HplTokenType.EqualSign);
        }

        if (spanWindow[0] == '=')
        {
            spanWindow.MoveForwardBy(1);
            return new(HplTokenType.Assignment);
        }
        
        if (char.IsDigit(spanWindow[0]) || spanWindow[0] == '.' || spanWindow[0] == '-')
        {
            if (spanWindow[0] == '.')
            {
                if (spanWindow.Length == 1)
                    return new(HplTokenType.Unknown);

                if (!(spanWindow[1] == 'f') && !char.IsDigit(spanWindow[1]))
                    return new(HplTokenType.Unknown);
            }

            textBuilder.Append(spanWindow[0]);
            spanWindow.MoveForwardBy(1);

            while (!spanWindow.IsEmpty && spanWindow[0].IsNumericLiteralCharacter())
            {
                textBuilder.Append(spanWindow[0]);
                spanWindow.MoveForwardBy(1);
            }

            return new(HplTokenType.Number, textBuilder.ToString());
        }

        if (spanWindow[0].IsIdentifierCharacter() || spanWindow[0] == '&')
        {
            while (!spanWindow.IsEmpty && (spanWindow[0].IsIdentifierCharacter() || spanWindow[0] == '&'))
            {
                if (spanWindow[0] == '&' && textBuilder.Length != 0)
                    throw new InvalidOperationException("& symbol is only allowed at a beginning of an identifier");

                textBuilder.Append(spanWindow[0]);
                spanWindow.MoveForwardBy(1);
            }

            return new(HplTokenType.Identifier, textBuilder.ToString());
        }

        textBuilder.Append(spanWindow[0]);
        spanWindow.MoveForwardBy(1);
        return new(HplTokenType.Unknown, textBuilder.ToString());
    }

    private void EatAllWhiteSpace(ref ReadOnlySpan<char> spanWindow)
    {
        while (!spanWindow.IsEmpty && char.IsWhiteSpace(spanWindow[0]))
            spanWindow.MoveForwardBy(1);
    }
}
