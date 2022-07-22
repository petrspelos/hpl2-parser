using Hpl2Parser.Core.Parsing.Syntax;
using System.Collections.Generic;
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

    private static Dictionary<string, HplFunctionParameterIntention> Intentions = new() {
        { "&in", HplFunctionParameterIntention.In },
        { "&out", HplFunctionParameterIntention.Out }
    };

    internal static bool IsIntentionMark(this string text) => Intentions.ContainsKey(text);

    internal static HplFunctionParameterIntention ToIntention(this string text)
    {
        if (Intentions.ContainsKey(text))
            return Intentions[text];

        return HplFunctionParameterIntention.None;
    }
}
