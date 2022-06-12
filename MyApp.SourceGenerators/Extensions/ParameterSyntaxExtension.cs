using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyApp.SourceGenerators.Enums;

namespace MyApp.SourceGenerators.Extensions
{
    public static class ParameterSyntaxExtension
    {
        public static IEnumerable<ParameterDeclaration> GetParametersDeclaration(this IEnumerable<ParameterSyntax> parameters)
        {
            return parameters.Select(parameter => parameter.GetParameterDeclaration()).ToList();
        }
        
        public static ParameterDeclaration GetParameterDeclaration(this ParameterSyntax parameter)
        {
            return new ParameterDeclaration {
                Name = parameter.Identifier.Text,
                Type = parameter.Type?.ToFullString(),
                From = parameter.GetParameterFromType(),
                MethodDeclaration = parameter.ToFullString()
            };
        }
        
        private static ParameterFromType GetParameterFromType(this ParameterSyntax parameter)
        {
            if (parameter.AttributeLists.Count <= 0) return ParameterFromType.None;
    
            var attribute = parameter.AttributeLists
                .FirstOrDefault(atl => atl.ToFullString().Contains("From"))
                ?.ToFullString()
                .Trim();
    
            if (attribute is null) return ParameterFromType.None;

            switch (attribute)
            {
                case "[FromQuery]": return ParameterFromType.FromQuery;
                case "[FromBody]": return ParameterFromType.FromBody;
                default: return ParameterFromType.None;
            }
        }
    }
}