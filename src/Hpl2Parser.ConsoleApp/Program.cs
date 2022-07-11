using System;
using System.IO;
using Hpl2Parser.Core;

namespace Hpl2Parser.ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: hpl2-parser path/to/script.hps");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Could not find the following script file: {args[0]}");
                return;
            }
            
            var parser = new HplParser(new HplTokenizer());
            var fileText = File.ReadAllText(args[0]);
            parser.Tokenize(fileText);
            parser.Parse();
        }
    }
}
