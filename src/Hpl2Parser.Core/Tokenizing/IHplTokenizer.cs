using System;

namespace Hpl2Parser.Core.Tokenizing;

public interface IHplTokenizer
{
    public HplToken GetToken(ref ReadOnlySpan<char> spanWindow);
}
