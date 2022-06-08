using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyApp.SourceGenerators.Enums;
using System;
using System.Linq;

namespace MyApp.SourceGenerators.Extensions
{
    public static class MethodDeclarationSyntaxExtensions
    {
        public static HttpMethodType IdentifyHttpMethodType(this MethodDeclarationSyntax method)
        {
            switch (method.Identifier.ValueText)
            {
                case "Post": return HttpMethodType.Post;
            }

            return HttpMethodType.Unknown;
        }


        public static ReturnDeclaration GetReturnDeclaration(this MethodDeclarationSyntax method)
        {
            var nodes = method.ReturnType.DescendantNodesAndSelf().ToList();
            var result = new ReturnDeclaration();
            nodes.ForEach(node => result.ResolverAndProcessSyntaxNode(node));
            return result;
        }

        private static void ResolverAndProcessSyntaxNode(this ReturnDeclaration declaration, SyntaxNode node)
        {
            switch (node)
            {
                case IdentifierNameSyntax syntax:
                    declaration.ProcessIdentifierNameSyntax(syntax);
                    break;
                case PredefinedTypeSyntax syntax:
                    declaration.ProcessPredefinedTypeSyntax(syntax);
                    break;
                case GenericNameSyntax syntax:
                    declaration.ProcessGenericNameSyntax(syntax);
                    break;
            }
        }

        private static void ProcessIdentifierNameSyntax(this ReturnDeclaration declaration, IdentifierNameSyntax identifierName)
        {
            declaration.TypeName = identifierName.Identifier.ValueText;

            switch (declaration.TypeName)
            {
                case "Task":
                    declaration.HasAsync = true;
                    declaration.HasVoid = true;
                    declaration.MethodDeclaration = declaration.TypeName;
                    break;
                case "ValueTask":
                    declaration.HasAsync = true;
                    declaration.HasVoid = true;
                    declaration.MethodDeclaration = declaration.TypeName;
                    break;
            }

            if (!declaration.ResponseDeclaration.Contains(declaration.TypeName))
            {
                declaration.ResponseDeclaration = declaration.TypeName;
            }

            if (!declaration.MethodDeclaration.Contains(declaration.TypeName))
            {
                declaration.MethodDeclaration = declaration.TypeName;
            }
        }

        private static void ProcessPredefinedTypeSyntax(this ReturnDeclaration declaration, PredefinedTypeSyntax syntax)
        {
            declaration.HasAsync = false;
            declaration.HasVoid = true;
            declaration.MethodDeclaration = "void";
            declaration.ResponseDeclaration = string.Empty;
            declaration.TypeName = syntax.ToFullString();
        }

        private static void ProcessGenericNameSyntax(this ReturnDeclaration declaration, GenericNameSyntax syntax)
        {
            var fullName = syntax.ToFullString().Trim();

            if (!declaration.MethodDeclaration.Contains(fullName))
            {
                declaration.MethodDeclaration = fullName;

                if (!declaration.MethodDeclaration.Contains("Task") &&
                    !declaration.MethodDeclaration.Contains("ValueTask")) return;

                declaration.HasAsync = true;
                declaration.HasVoid = false;
            }
            else if (!declaration.ResponseDeclaration.Contains(fullName))
            {
                declaration.ResponseDeclaration = fullName;
            }
        }


    }
}
