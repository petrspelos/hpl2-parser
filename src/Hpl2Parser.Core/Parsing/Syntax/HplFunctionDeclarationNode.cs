using System.Collections.Generic;

namespace Hpl2Parser.Core.Parsing.Syntax;

public sealed class HplFunctionDeclarationNode : HplSyntaxNode
{
    public override HplSyntaxNodeType NodeType => HplSyntaxNodeType.FunctionDeclaration;

    public ICollection<HplSyntaxNode> ParameterList { get; set; }

    public HplTypeReferenceNode ReturnType { get; set; }
    
    public string Identifier { get; set; }
}
