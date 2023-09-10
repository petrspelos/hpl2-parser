namespace Hpl2Parser.Core.Parsing.Syntax;

public class HplFunctionArgumentNode : HplSyntaxNode
{
    public override HplSyntaxNodeType NodeType => HplSyntaxNodeType.FunctionCallArgument;

    public HplFunctionCallArgumentType ArgumentType { get; set; }

    public string ArgumentValue { get; set; }
}
