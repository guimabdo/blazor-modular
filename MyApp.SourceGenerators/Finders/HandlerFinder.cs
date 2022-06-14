using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace MyApp.SourceGenerators.Finders
{
    public class HandlerFinder : ISyntaxReceiver
    {
        public HandlerFinder()
        {
            Handlers = new List<InterfaceDeclarationSyntax>();
        }

        public List<InterfaceDeclarationSyntax> Handlers { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is InterfaceDeclarationSyntax handler)) return;
            if (handler.Identifier.ValueText.EndsWith("Handler"))
            { Handlers.Add(handler); }
        }
    }
}
