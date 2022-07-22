using System.Collections.Generic;

namespace Hpl2Parser.Core.Parsing.Syntax;

public sealed class HplFunctionParameterNode : HplSyntaxNode
{
    public override HplSyntaxNodeType NodeType => HplSyntaxNodeType.FunctionParameter;

    public string Type { get; set; }
    
    public string Identifier { get; set; }
    
    public HplFunctionParameterIntention Intention { get; set; } = HplFunctionParameterIntention.None;
}
