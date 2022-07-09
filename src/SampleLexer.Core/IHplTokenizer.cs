using System;

namespace SampleLexer.Core;

public interface IHplTokenizer
{
    public HplToken GetToken(ref ReadOnlySpan<char> spanWindow);
}
