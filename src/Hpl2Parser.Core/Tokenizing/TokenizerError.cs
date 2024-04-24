using FluentResults;

namespace Hpl2Parser.Core.Tokenizing;

public enum TokenizerErrorType
{
    UnknownError,
    UnknownToken
}

public sealed class TokenizerError : Error
{
    public static string InternalErrorType = "Tokenizer Failed";

    public TokenizerErrorType ErrorType => (TokenizerErrorType)Metadata["ErrorType"];

    public TokenizerError(TokenizerErrorType errorType) : base(InternalErrorType)
    {
        Metadata.Add("ErrorType", errorType);
    }
}
