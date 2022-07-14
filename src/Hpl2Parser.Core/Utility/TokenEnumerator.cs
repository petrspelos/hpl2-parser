using Hpl2Parser.Core.Tokenizing;
using System.Collections.Generic;
using System.Linq;

namespace Hpl2Parser.Core.Utility;

internal sealed class TokenEnumerator
{
    private readonly List<HplToken> _tokens;

    private int _index;

    public TokenEnumerator(IEnumerable<HplToken> tokens)
    {
        _tokens = tokens.ToList();
    }

    public void Next(int step = 1) => _index = _index + step;

    public HplToken Peek(int forward = 0)
    {
        var peekIndex = _index + forward;

        if (peekIndex >= _tokens.Count || peekIndex < 0)
            return new HplToken(HplTokenType.EndOfFile);

        return _tokens[peekIndex];
    }
}
