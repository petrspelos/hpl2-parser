using System;
using System.Text;

namespace Hpl2Parser.Core
{
    public class HplTokenizer : IHplTokenizer
    {
        public HplToken GetToken(ref ReadOnlySpan<char> spanWindow)
        {
            var textBuilder = new StringBuilder();
            EatAllWhiteSpace(ref spanWindow);

            if (spanWindow.IsEmpty)
            {
                return new()
                {
                    Type = HplTokenType.EndOfFile
                };
            }

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

                return new()
                {
                    Type = HplTokenType.InlineComment,
                    Text = textBuilder.ToString()
                };
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
                        return new()
                        {
                            Type = HplTokenType.InvalidToken
                        };
                    }
                }

                textBuilder.Append(spanWindow[0]);
                textBuilder.Append(spanWindow[1]);
                spanWindow.MoveForwardBy(2);

                return new()
                {
                    Type = HplTokenType.MultilineComment,
                    Text = textBuilder.ToString()
                };
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
                        return new()
                        {
                            Type = HplTokenType.InvalidToken
                        };
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
                        return new()
                        {
                            Type = HplTokenType.InvalidToken
                        };
                    }
                }

                textBuilder.Append(spanWindow[0]);
                spanWindow.MoveForwardBy(1);

                return new()
                {
                    Type = HplTokenType.StringLiteral,
                    Text = textBuilder.ToString()
                };
            }

            if (spanWindow.Length >= 2 && spanWindow[0] == '<' && spanWindow[1] == '=')
            {
                spanWindow.MoveForwardBy(2);
                return new()
                {
                    Type = HplTokenType.BooleanLessThan
                };
            }
            
            if (spanWindow.Length >= 2 && spanWindow[0] == '>' && spanWindow[1] == '=')
            {
                spanWindow.MoveForwardBy(2);
                return new()
                {
                    Type = HplTokenType.BooleanMoreThan
                };
            }
            
            if (spanWindow.Length >= 2 && spanWindow[0] == '!' && spanWindow[1] == '=')
            {
                spanWindow.MoveForwardBy(2);
                return new()
                {
                    Type = HplTokenType.NotEqualSign
                };
            }
            
            if (spanWindow.Length >= 2 && spanWindow[0] == '|' && spanWindow[1] == '|')
            {
                spanWindow.MoveForwardBy(2);
                return new()
                {
                    Type = HplTokenType.BooleanOrSign
                };
            }
            
            if (spanWindow.Length >= 2 && spanWindow[0] == '&' && spanWindow[1] == '&')
            {
                spanWindow.MoveForwardBy(2);
                return new()
                {
                    Type = HplTokenType.BooleanAndSign
                };
            }
            
            if (spanWindow[0] == '(')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.OpenParen
                };
            }

            if (spanWindow[0] == ')')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.CloseParen
                };
            }

            if (spanWindow[0] == ',')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.Comma
                };
            }

            if (spanWindow[0] == '&')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.Ampersand
                };
            }

            if (spanWindow[0] == '{')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.OpenBracket
                };
            }

            if (spanWindow[0] == '}')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.CloseBracket
                };
            }

            if (spanWindow[0] == ';')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.Semicolon
                };
            }
            
            if (spanWindow[0] == ':')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.Colon
                };
            }
            
            if (spanWindow[0] == '!')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.ExclamationPoint
                };
            }
            
            if (spanWindow[0] == '<')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.LessThanSign
                };
            }
            
            if (spanWindow[0] == '>')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.MoreThanSign
                };
            }
            
            if (spanWindow[0] == '%')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.PercentageSign
                };
            }
            
            if (spanWindow[0] == '=' && spanWindow[1] == '=')
            {
                spanWindow.MoveForwardBy(2);
                return new()
                {
                    Type = HplTokenType.EqualSign
                };
            }

            if (spanWindow[0] == '=')
            {
                spanWindow.MoveForwardBy(1);
                return new()
                {
                    Type = HplTokenType.Assignment
                };
            }
            
            if (char.IsDigit(spanWindow[0]) || spanWindow[0] == '.' || spanWindow[0] == '-')
            {
                if (spanWindow[0] == '.')
                {
                    if (spanWindow.Length == 1)
                        return new()
                        {
                            Type = HplTokenType.Unknown
                        };

                    if (!(spanWindow[1] == 'f') && !char.IsDigit(spanWindow[1]))
                        return new()
                        {
                            Type = HplTokenType.Unknown
                        };
                }

                textBuilder.Append(spanWindow[0]);
                spanWindow.MoveForwardBy(1);

                while (!spanWindow.IsEmpty && spanWindow[0].IsNumericLiteralCharacter())
                {
                    textBuilder.Append(spanWindow[0]);
                    spanWindow.MoveForwardBy(1);
                }

                return new()
                {
                    Type = HplTokenType.Number,
                    Text = textBuilder.ToString()
                };
            }

            if (spanWindow[0].IsIdentifierCharacter())
            {
                while (!spanWindow.IsEmpty && spanWindow[0].IsIdentifierCharacter())
                {
                    textBuilder.Append(spanWindow[0]);
                    spanWindow.MoveForwardBy(1);
                }

                return new()
                {
                    Type = HplTokenType.Identifier,
                    Text = textBuilder.ToString()
                };
            }

            textBuilder.Append(spanWindow[0]);
            spanWindow.MoveForwardBy(1);
            return new()
            {
                Type = HplTokenType.Unknown,
                Text = textBuilder.ToString()
            };
        }

        private void EatAllWhiteSpace(ref ReadOnlySpan<char> spanWindow)
        {
            while (!spanWindow.IsEmpty && char.IsWhiteSpace(spanWindow[0]))
                spanWindow.MoveForwardBy(1);
        }
    }
}
