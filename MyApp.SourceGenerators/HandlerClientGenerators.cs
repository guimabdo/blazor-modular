using Microsoft.CodeAnalysis;
using MyApp.SourceGenerators.Extensions;
using MyApp.SourceGenerators.Finders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.SourceGenerators
{
    [Generator]
    public class HandlerClientGenerators : ISourceGenerator
    {
        private const string DEFAULT_NAMESPACE = "MyApp.Module.Application.Shared";

        public void Execute(GeneratorExecutionContext context)
        {
            var handlers = ((HandlerFinder)context.SyntaxReceiver)?.Handlers;
            if (handlers == null) return;

            foreach (var handler in handlers)
            {
                var interfaceNamespace = handler.GetNamespace();
                var className = handler.Identifier.ValueText.Substring(1) + "Client";
                var method = handler.GetHttpActionMethod();
                var returnDeclaration = method.GetReturnDeclaration();
                var hasAsync = returnDeclaration.HasAsync ? "async " : "";
                var request = method.GetRequestType();
                var methodBody = CreateHttpPostMethodBody(returnDeclaration, handler.Identifier.ValueText.Substring(1));

                var source = $@"using {interfaceNamespace};
using System.Net.Http.Json;

namespace {DEFAULT_NAMESPACE};

public class {className} : {handler.Identifier.ValueText}
{{
    private readonly HttpClient _httpClient;
    public {className}(HttpClient httpClient)
    {{ 
        _httpClient = httpClient;
    }}

    public {hasAsync} {returnDeclaration.MethodDeclaration} {method.Identifier.ValueText}({request})
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

        private static string CreateHttpPostMethodBody(ReturnDeclaration returnDeclaration, string endPoint)
        {
            return $@"
        var responseMessage = await _httpClient.PostAsJsonAsync(""{endPoint}"", request);
        return await responseMessage.Content.ReadFromJsonAsync<{returnDeclaration.ResponseDeclaration}>() ?? default!;";
        }
    }
}
