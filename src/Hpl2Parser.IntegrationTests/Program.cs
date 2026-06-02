using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Core.Parsing.Syntax;
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
        PrintTree(syntaxTree, sampleFile);
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

static void PrintTree(HplSyntaxTree tree, string fileName = "HPL Syntax Tree")
{
    Console.WriteLine(fileName);
    for (var i = 0; i < tree.RootElements.Count; i++)
    {
        var treeSymbol = i == tree.RootElements.Count - 1 ? "└─" : "├─";
        var element = tree.RootElements.ElementAt(i);

        if (element is HplFunctionDeclarationNode funcDeclaration)
        {
            Console.WriteLine($"{treeSymbol} ƒ {funcDeclaration.Identifier}");
            for (var x = 0; x < funcDeclaration.ParameterList.Count; x++)
            {
                var prefix = "|    " + (x == funcDeclaration.ParameterList.Count - 1 ? "└" : "├");
                Console.WriteLine($"{prefix} 🇵 {StringifyParameterNode(funcDeclaration.ParameterList.ElementAt(x))}");
            }

            foreach (var funcExpression in funcDeclaration.BodyElements)
            {
                if (funcExpression is HplFunctionCallNode funcCall)
                {
                    Console.WriteLine($"- Function Call: {funcCall.Identifier}...");
                }
                else
                {
                    Console.WriteLine($"- Unvisualised node: {funcExpression.NodeType}");
                }
            }
        }
        else
        {
            Console.WriteLine($"{treeSymbol} Element (visualization not implemented): {element.NodeType}");
        }
    }
}

static string StringifyParameterNode(HplSyntaxNode node)
{
    if (node is not HplFunctionParameterNode param)
    {
        throw new InvalidOperationException("Cannot stringify a non-parameter node as a parameter.");
    }

    return $"{param.Identifier} (type: {param.Type})";
}
