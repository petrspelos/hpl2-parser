using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Core.Tokenizing;

var sampleFiles = Directory.GetFiles("samples", "*.hps");
var success = 0;
var error = 0;

var parser = new HplParser(new HplTokenizer());
foreach (var sampleFile in sampleFiles)
{
    Console.WriteLine($"TESTING FILE: {sampleFile}");
    var fileText = File.ReadAllText(sampleFile);

    try
    {
        parser.Tokenize(fileText);
        var syntaxTree = parser.Parse();
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[FAILED] {e.Message}");
        Console.ResetColor();
        error++;
        continue;
    }

    success++;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("[SUCCESS]");
    Console.ResetColor();
}

Console.WriteLine("PARSER TEST FINISHED");
Console.WriteLine($"{error} FAILED - {success} SUCCEEDED");
