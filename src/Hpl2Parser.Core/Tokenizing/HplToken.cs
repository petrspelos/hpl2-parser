namespace Hpl2Parser.Core.Tokenizing;

public struct HplToken
{
    public HplToken()
    {
    }

    public HplToken(HplTokenType type)
    {
        Type = type;
    }

    public HplToken(HplTokenType type, string text) : this(type)
    {
        Text = text;
    }

    public HplTokenType Type { get; set; } = HplTokenType.Unknown;

    public string Text { get; set; } = string.Empty;
}
