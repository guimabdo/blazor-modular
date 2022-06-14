using Microsoft.CodeAnalysis;
using MyApp.SourceGenerators.Extensions;
using MyApp.SourceGenerators.Finders;
using MyApp.SourceGenerators.Enums;

namespace MyApp.SourceGenerators
{
    [Generator]
    public class HandlerClientGenerators : ISourceGenerator
    {
        private const string DefaultNamespace = "MyApp.Module.Application.Shared";

        public void Execute(GeneratorExecutionContext context)
        {
            var handlers = ((HandlerFinder) context.SyntaxReceiver)?.Handlers;
            if(handlers == null) return;
            
            foreach (var handler in handlers)
            {
                var method = handler.GetHttpActionMethod();
                if(method is null) continue;
                
                var interfaceNamespace = handler.GetNamespace();
                var className = handler.Identifier.ValueText.Substring(1) + "Client";

                var returnDeclaration = method.GetReturnDeclaration();
                //TODO: Ainda trabalhando para tratar multiplos parametros de entrada
                var parameterDeclaration = method.ParameterList.Parameters.FirstOrDefault().GetParameterDeclaration();
                var hasAsync = returnDeclaration.HasAsync ? "async" : "";
                var httpMethod = method.IdentifyHttpMethodType();
                
                var methodBody = CreateHttpMethodBody(
                    httpMethod,
                    returnDeclaration, 
                    parameterDeclaration, 
                    handler.Identifier.ValueText.Substring(1));

                var source = $@"using {interfaceNamespace};
using System.Net.Http.Json;

namespace {DefaultNamespace};

public class {className} : {handler.Identifier.ValueText}
{{
    private readonly HttpClient _httpClient;
    public {className}(HttpClient httpClient)
    {{ 
        _httpClient = httpClient;
    }}

    public {hasAsync} {returnDeclaration.MethodDeclaration} {method.Identifier.ValueText}({parameterDeclaration.MethodDeclaration})
    {{
        {methodBody}
    }}
}}";

                context.AddSource($"{className}.g.cs", source);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new HandlerFinder());
        }

        private static string CreateHttpMethodBody(
            HttpMethodType httpMethod,
            ReturnDeclaration returnDeclaration,
            ParameterDeclaration parameterDeclaration, 
            string endPoint)
        {
            switch (httpMethod)
            {
                case HttpMethodType.Post : return CreateHttpPostMethodBody(returnDeclaration, parameterDeclaration, endPoint);
                case HttpMethodType.Get: return  CreateHttpGetMethodBody(returnDeclaration, parameterDeclaration, endPoint);
                case HttpMethodType.Delete: return CreateHttpDeleteMethodBody(returnDeclaration, parameterDeclaration, endPoint);
                case HttpMethodType.Unknown: return "//[Unknown] Http Method not Identified";
                default: return "//[NotFound] Http Method not Identified";
            }
        }
        
        private static string CreateHttpPostMethodBody(
            ReturnDeclaration returnDeclaration, 
            ParameterDeclaration parameterDeclaration, 
            string endPoint)
        {
            var returnLine = returnDeclaration.HasVoid ? 
                ""  : 
                $@"return await responseMessage.Content.ReadFromJsonAsync<{returnDeclaration.ResponseDeclaration}>();";
            
            return $@"
        var responseMessage = await _httpClient.PostAsJsonAsync(""{endPoint}"", {parameterDeclaration.Name});
        {returnLine}";
        }

        private static string CreateHttpDeleteMethodBody(
            ReturnDeclaration returnDeclaration,
            ParameterDeclaration parameterDeclaration,
            string endPoint)
        {
            
            var returnLine = returnDeclaration.HasVoid ?
                "" :
                $@"return await responseMessage.Content.ReadFromJsonAsync<{returnDeclaration.ResponseDeclaration}>();";        
            
            return $@"
        var queryString = QueryStringHelper.ToQueryString(request);
        var responseMessage =  await _httpClient.DeleteAsync($""{endPoint}?{{queryString}}"");
        {returnLine}";
        }

        private static string CreateHttpGetMethodBody(
            ReturnDeclaration returnDeclaration, 
            ParameterDeclaration parameterDeclaration, 
            string endPoint)
        {
            var methodSource = $@"
        var queryString = QueryStringHelper.ToQueryString({parameterDeclaration.Name});
        return await _httpClient.GetFromJsonAsync<{returnDeclaration.ResponseDeclaration}>($""{endPoint}?{{queryString}}"");";
            
            return methodSource;
        }
    }
}
