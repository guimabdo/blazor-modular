using Microsoft.AspNetCore.Mvc;
using MyApp.Module.Application.Shared.Assunto;
namespace MyApp.Module.Application.Assunto;
public class CoisarAssuntoHandler: HandlerBase, ICoisarAssuntoHandler
{
    public async Task<CoisarAssuntoResponse> Post([FromBody]CoisarAssuntoRequest request)
    {
        //return await Task.FromResult(new CoisarAssuntoResponse(model.Campo1 + "a", model.Campo2 + "b"));
        return await Task.FromResult(new CoisarAssuntoResponse($"{DateTime.Now}", $"{Guid.NewGuid()}"));
    }
}
