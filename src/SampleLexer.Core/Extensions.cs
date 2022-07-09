using System;

namespace SampleLexer.Core
{
    public static class Extensions
    {
        public static void MoveForwardBy(this ref ReadOnlySpan<char> span, int amount)
        {
            if (!span.CanMoveForward(amount))
                throw new InvalidOperationException($"Cannot move forward by {amount}, because the span is only {span.Length} long.");

            if (span.Length == amount)
            {
                span = new ReadOnlySpan<char>();
            }
            else
            {
                span = span[amount..];
            }
        }

        public static bool CanMoveForward(this ReadOnlySpan<char> span, int amount) => span.Length >= amount;

        public static bool IsEndOfLine(this char c) => c == '\r' || c == '\n';

        public static bool IsIdentifierCharacter(this char c) => char.IsLetterOrDigit(c) || c == '_';

        public static bool IsNumericLiteralCharacter(this char c) => char.IsDigit(c) || c == '.' || c == 'f';
    }
}
