using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace MyApp.SourceGenerators.Extensions
{
    public static class InterfaceDeclarationSyntaxExtensions
    {
        public static MethodDeclarationSyntax GetHttpActionMethod(this InterfaceDeclarationSyntax interfaceHandler)
        {
            return interfaceHandler.Members
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m =>
                    m.Identifier.ValueText.Equals("Post", StringComparison.InvariantCultureIgnoreCase) ||
                    m.Identifier.ValueText.Equals("Delete", StringComparison.InvariantCultureIgnoreCase) ||
                    m.Identifier.ValueText.Equals("Get", StringComparison.InvariantCultureIgnoreCase)
                );
        }
    }
}
