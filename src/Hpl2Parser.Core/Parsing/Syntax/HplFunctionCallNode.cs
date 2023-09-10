using System;
using System.Collections;
using System.Collections.Generic;

namespace Hpl2Parser.Core.Parsing.Syntax;

public sealed class HplFunctionCallNode : HplSyntaxNode
{
    public override HplSyntaxNodeType NodeType => HplSyntaxNodeType.FunctionCall;

    public string Identifier { get; set; }

    public ICollection<HplFunctionArgumentNode> Arguments { get; set; } = new List<HplFunctionArgumentNode>();
}
