using System;

namespace Hpl2Parser.Core.Parsing.Syntax;

public sealed class HplTypeReferenceNode : HplSyntaxNode
{
    public override HplSyntaxNodeType NodeType => HplSyntaxNodeType.TypeReference;

    public string TextValue { get; set; } = string.Empty;
}
