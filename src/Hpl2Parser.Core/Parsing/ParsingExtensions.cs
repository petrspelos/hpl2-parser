using System.Linq;

namespace Hpl2Parser.Core.Parsing;

internal static class ParsingExtensions
{
    //  TODO(Peter): write and use extensions
    private static string[] ReturnTypeNames = new[] {
        "void",
        "string",
        "bool",
        "int8",
        "int16",
        "int",
        "int64",
        "uint8",
        "uint16",
        "uint",
        "uint64",
        "float"
    };

    internal static bool IsReturnType(this string text) => ReturnTypeNames.Contains(text);
}
