using System;
using System.IO;
using Hpl2Parser.Core.Parsing;
using Hpl2Parser.Core.Parsing.Syntax;
using Hpl2Parser.Core.Tokenizing;

namespace Hpl2Parser.ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("USAGE: hpl2-parser path/to/script.hps");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Could not find the following script file: {args[0]}");
                return;
            }
            
            var parser = new HplParser(new HplTokenizer());
            var fileText = File.ReadAllText(args[0]);
            parser.Tokenize(fileText);
            var syntaxTree = parser.Parse();

            foreach(var node in syntaxTree.RootElements)
            {
                PrintNode(node);
            }
        }

        private static void PrintNode(HplSyntaxNode node)
        {
            if (node.NodeType == HplSyntaxNodeType.FunctionDeclaration)
            {
                var func = node as HplFunctionDeclarationNode;
                Console.WriteLine($"Function Declaration: {func.ReturnType} {func.Identifier}");
                Console.WriteLine("== Parameters:");
                foreach (var param in func.ParameterList)
                {
                    PrintNode(param);
                }

                Console.WriteLine("== Statements:");
                foreach (var statement in func.BodyElements)
                {
                    PrintNode(statement);
                }
            }
            else if (node.NodeType == HplSyntaxNodeType.TypeReference)
            {
                var tRef = node as HplTypeReferenceNode;
                Console.WriteLine($"Type Reference: {tRef.NodeType} - {tRef.TextValue}");
            }
            else if (node.NodeType == HplSyntaxNodeType.FunctionParameter)
            {
                var fParam = node as HplFunctionParameterNode;
                Console.WriteLine($"Function Param: {fParam.Type} {fParam.Intention} {fParam.Identifier}");
            }
            else if (node.NodeType == HplSyntaxNodeType.FunctionCall)
            {
                var fCall = node as HplFunctionCallNode;
                Console.WriteLine($"Call to {fCall.Identifier}");
                Console.WriteLine("== Function Call Arguments:");
                foreach (var arg in fCall.Arguments)
                {
                    PrintNode(arg);
                }
            }
            else if (node.NodeType == HplSyntaxNodeType.FunctionCallArgument)
            {
                var arg = node as HplFunctionArgumentNode;
                Console.WriteLine($"Argument: {arg.ArgumentType} - {arg.ArgumentValue}");
            }
            else
            {
                Console.WriteLine($"Unresolvable node of type {node.NodeType}");
            }
        }
    }
}
