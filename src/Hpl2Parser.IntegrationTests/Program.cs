using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Core.Tokenizing;
using System.IO.Compression;

//const string SourceDirectory = "D:\\Amnesia Custom Stories";
//var zipFiles = Directory.GetFiles(SourceDirectory, "*.zip");

//Directory.CreateDirectory("samples");

//foreach (var zipFile in zipFiles)
//{
//    ExtractHpsFilesWithUniquePrefix(zipFile, "samples/");
//}

//Console.WriteLine("Done");

//static void ExtractHpsFilesWithUniquePrefix(string zipPath, string outputPath)
//{
//    Console.WriteLine($"Extracting {zipPath}...");
//    using (ZipArchive archive = ZipFile.OpenRead(zipPath))
//    {
//        foreach (ZipArchiveEntry entry in archive.Entries)
//        {
//            if (entry.FullName.EndsWith(".hps", StringComparison.OrdinalIgnoreCase))
//            {
//                Console.WriteLine($"Extracting {entry.Name}");
//                string uniquePrefix = Guid.NewGuid().ToString()[..4];
//                string destinationPath = Path.Combine(outputPath, uniquePrefix + "_" + entry.Name);
//                entry.ExtractToFile(destinationPath, true);
//            }
//        }
//    }
//}

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
