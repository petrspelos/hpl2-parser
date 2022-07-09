namespace SampleLexer.Core;

public struct HplToken
{
    public HplToken()
    {
    }

    public HplTokenType Type { get; set; } = HplTokenType.Unknown;

    public string Text { get; set; } = string.Empty;
}
