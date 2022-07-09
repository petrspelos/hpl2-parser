using SampleLexer.Core;
using System;
using System.IO;

namespace SampleLexer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HplParser(new HplTokenizer());
            var fileText = File.ReadAllText("/home/peter/source/repos/petrspelos/sample-lexer/src/SampleLexer.ConsoleApp/sample.hps");
            parser.Tokenize(fileText);
            parser.Parse();
        }
    }
}
