using System;

namespace Hpl2Parser.Core;

public interface IHplTokenizer
{
    public HplToken GetToken(ref ReadOnlySpan<char> spanWindow);
}
