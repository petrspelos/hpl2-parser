using System;
using Hpl2Parser.Core;
using Xunit;

namespace Hpl2Parser.Tests
{
    public class HplTokenizerTests
    {
        private readonly HplTokenizer _tokenizer;

        public HplTokenizerTests()
        {
            _tokenizer = new HplTokenizer();
        }

        [Theory]
        [InlineData("// my comment\n\rAAA", "\n\rAAA")]
        [InlineData("   \t\t\r\n//my comment\n\rAAA", "\n\rAAA")]
        [InlineData("//This is a pretty cool comment_!", "")]
        [InlineData("// NOTE(Peter): Casual comment\n=)", "\n=)")]
        [InlineData("// NOTE(Peter): Casual comment\n\r", "\n\r")]
        [InlineData("////////////////////////////PlayMusic(\"*musicfile.ogg*\", false, 3, 1, 0, true);/////\n", "\n")]
        public void GetToken_ShouldReturnComment(string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(HplTokenType.InlineComment, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData("/* void test const { } @ \n\r\n\nvoid \"sdofisdfio\"\n*\n/\\*\n/\n*/123", "123")]
        public void GetToken_ShouldReturnMultilineComment(string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());
            
            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(HplTokenType.MultilineComment, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData("/* unfinished comment", "")]
        public void GetToken_ShouldReturnInvalidToken_WhenUnfinishedMultilineComment(string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(HplTokenType.InvalidToken, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData("\"Hello, I'm a string literal!\"123", "123")]
        [InlineData("\"Hello, I escape \\\"  !\"123", "123")]
        public void GetToken_ShouldReturnStringLiteral(string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(HplTokenType.StringLiteral, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.InvalidToken, "\"Unfinished string literal\n...\"", "\n...\"")]
        [InlineData(HplTokenType.InvalidToken, "\"Unfinished string literal", "")]
        public void GetToken_ShouldReturnInvalidToken_ForUnfinishedString(HplTokenType expectedType, string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(expectedType, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.OpenParen, "(somecode", "somecode")]
        [InlineData(HplTokenType.CloseParen, ")somecode", "somecode")]
        [InlineData(HplTokenType.Comma, ", somecode", " somecode")]
        [InlineData(HplTokenType.Ampersand, "& asSome", " asSome")]
        [InlineData(HplTokenType.CloseBracket, "}\n", "\n")]
        [InlineData(HplTokenType.OpenBracket, "{code", "code")]
        [InlineData(HplTokenType.Semicolon, ";\ncode", "\ncode")]
        [InlineData(HplTokenType.Assignment, "= code", " code")]
        [InlineData(HplTokenType.Colon, ": code", " code")]
        [InlineData(HplTokenType.ExclamationPoint, "! code", " code")]
        [InlineData(HplTokenType.LessThanSign, "< code", " code")]
        [InlineData(HplTokenType.MoreThanSign, "> code", " code")]
        [InlineData(HplTokenType.PercentageSign, "% code", " code")]
        public void GetToken_ShouldRecognizeOneCharacterSymbols(HplTokenType expectedType, string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(expectedType, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.Identifier, "HelloIdentifier();", "();")]
        [InlineData(HplTokenType.Identifier, "string", "")]
        [InlineData(HplTokenType.Identifier, "Foo_bar();", "();")]
        [InlineData(HplTokenType.Identifier, "Foo bar();", " bar();")]
        [InlineData(HplTokenType.Identifier, "Foo\nbar();", "\nbar();")]
        public void GetToken_ShouldRecognizeAnIdentifier(HplTokenType expectedType, string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(expectedType, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.EqualSign, "== abc", " abc")]
        [InlineData(HplTokenType.EqualSign, "==", "")]
        [InlineData(HplTokenType.BooleanLessThan, "<=", "")]
        [InlineData(HplTokenType.BooleanMoreThan, ">=", "")]
        [InlineData(HplTokenType.NotEqualSign, "!=", "")]
        [InlineData(HplTokenType.BooleanAndSign, "&&", "")]
        [InlineData(HplTokenType.BooleanOrSign, "||", "")]
        public void GetToken_ShouldRecognizeBooleanSigns(HplTokenType expectedType, string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(expectedType, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.Number, "12", "")]
        [InlineData(HplTokenType.Number, "1", "")]
        [InlineData(HplTokenType.Number, "1.02", "")]
        [InlineData(HplTokenType.Number, ".02", "")]
        [InlineData(HplTokenType.Number, "3.993f", "")]
        [InlineData(HplTokenType.Number, "-3.993f", "")]
        public void GetToken_ShouldRecognizeNumber(HplTokenType expectedType, string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(expectedType, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.EndOfFile, "", "")]
        [InlineData(HplTokenType.EndOfFile, "\n\n   \t \r\n", "")]
        public void GetToken_ShouldRecognizeEndOfFile(HplTokenType expectedType, string hplCode, string expectedCodeLeft)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualTokenType = _tokenizer.GetToken(ref spanWindow).Type;

            Assert.Equal(expectedType, actualTokenType);
            Assert.Equal(expectedCodeLeft.Length, spanWindow.Length);
            Assert.Equal(expectedCodeLeft, spanWindow.ToString());
        }

        [Theory]
        [InlineData(HplTokenType.Number, ".02CODE", ".02")]
        [InlineData(HplTokenType.Number, "3.993f!", "3.993f")]
        [InlineData(HplTokenType.Number, "-3.993f\r\n", "-3.993f")]
        [InlineData(HplTokenType.Identifier, "Foo_bar();", "Foo_bar")]
        [InlineData(HplTokenType.Identifier, "Foo bar();", "Foo")]
        [InlineData(HplTokenType.Identifier, "Foo\nbar();", "Foo")]
        [InlineData(HplTokenType.StringLiteral, "\"Hello, I'm a string literal!\"123", "\"Hello, I'm a string literal!\"")]
        [InlineData(HplTokenType.StringLiteral, "\"Hello, I escape \\\"  !\"123", "\"Hello, I escape \\\"  !\"")]
        [InlineData(HplTokenType.InlineComment, "// NOTE(Peter): Casual comment\n=)", "// NOTE(Peter): Casual comment")]
        [InlineData(HplTokenType.InlineComment, "// NOTE(Peter): Casual comment\n\r", "// NOTE(Peter): Casual comment")]
        [InlineData(HplTokenType.MultilineComment, "/* void test const { } @ \n\r\n\nvoid \"sdofisdfio\"\n*\n/\\*\n/\n*/123", "/* void test const { } @ \n\r\n\nvoid \"sdofisdfio\"\n*\n/\\*\n/\n*/")]
        public void GetToken_ShouldCaptureText(HplTokenType expectedType, string hplCode, string expectedText)
        {
            var spanWindow = new ReadOnlySpan<char>(hplCode.ToCharArray());

            var actualToken = _tokenizer.GetToken(ref spanWindow);

            Assert.Equal(expectedType, actualToken.Type);
            Assert.Equal(expectedText, actualToken.Text);
        }
    }
}
