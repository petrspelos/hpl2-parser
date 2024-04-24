using System;
using System.Collections.Generic;
using FluentResults;

namespace Hpl2Parser.Core.Tokenizing;

public interface IHplTokenizer
{
    public Result<IReadOnlyCollection<HplToken>> Tokenize(string code);

    public HplToken GetToken(ref ReadOnlySpan<char> spanWindow);
}
