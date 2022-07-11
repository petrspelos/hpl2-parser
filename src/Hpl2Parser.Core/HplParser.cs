using System;
using System.Collections.Generic;

namespace Hpl2Parser.Core
{
    public class HplParser
    {
        private readonly IHplTokenizer _hplTokenizer;

        private IEnumerable<HplToken> _tokens = Array.Empty<HplToken>();

        public HplParser(IHplTokenizer hplTokenizer)
        {
            _hplTokenizer = hplTokenizer;
        }

        public void Tokenize(string fileText)
        {
            var spanWindow = new ReadOnlySpan<char>(fileText.ToCharArray());
            var textLength = fileText.Length;

            var tokens = new List<HplToken>();
            while (!spanWindow.IsEmpty)
            {
                var token = _hplTokenizer.GetToken(ref spanWindow);
                //Console.WriteLine($"{textLength - spanWindow.Length} - {token.Type}: {token.Text}");
                tokens.Add(token);
            }

            _tokens = tokens;
        }

        public void Parse()
        {
            
        }
    }
}
