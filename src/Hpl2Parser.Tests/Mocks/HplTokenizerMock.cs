using System;
using System.Collections.Generic;
using Hpl2Parser.Core.Tokenizing;

namespace Hpl2Parser.Tests.Mocks;

public class HplTokenizerMock : IHplTokenizer
{
    public Queue<HplToken> TokensQueue { get; set; } = new();
    
    public HplToken GetToken(ref ReadOnlySpan<char> spanWindow)
    {
        return TokensQueue.Dequeue();
    }
}
