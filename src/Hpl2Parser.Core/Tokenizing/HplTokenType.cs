namespace Hpl2Parser.Core.Tokenizing;

public enum HplTokenType
{
    Unknown,
    InlineComment,
    StringLiteral,
    InvalidToken,
    MultilineComment,
    OpenParen,
    CloseParen,
    Comma,
    Ampersand,
    Identifier,
    OpenBracket,
    CloseBracket,
    EqualSign,
    Number,
    Semicolon,
    EndOfFile,
    Assignment,
    Colon,
    ExclamationPoint,
    LessThanSign,
    MoreThanSign,
    PercentageSign,
    BooleanLessThan,
    BooleanMoreThan,
    NotEqualSign,
    BooleanAndSign,
    BooleanOrSign
}
