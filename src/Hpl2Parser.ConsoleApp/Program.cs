using System;
using System.IO;
using Hpl2Parser.Core;

namespace Hpl2Parser.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HplParser(new HplTokenizer());
            var fileText = File.ReadAllText("/home/peter/source/repos/petrspelos/hpl2-parser/src/Hpl2Parser.ConsoleApp/sample.hps");
            parser.Tokenize(fileText);
            parser.Parse();
        }
    }
}
