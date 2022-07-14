using System.Collections.Generic;

namespace Hpl2Parser.Core.Parsing.Syntax;

public sealed class HplSyntaxTree
{
    public ICollection<HplSyntaxNode> RootElements { get; init; } = new List<HplSyntaxNode>();
}
