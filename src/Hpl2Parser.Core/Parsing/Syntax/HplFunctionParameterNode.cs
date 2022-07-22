namespace Hpl2Parser.Core.Parsing.Syntax;

public sealed class HplFunctionParameterNode : HplSyntaxNode
{
    public override HplSyntaxNodeType NodeType => HplSyntaxNodeType.FunctionParameter;

    public string Type { get; set; }
    
    public string Identifier { get; set; }
}
