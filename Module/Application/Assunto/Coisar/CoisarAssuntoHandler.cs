using MyApp.Module.Application.Shared.Assunto;
using MyApp.Module.Application.Shared.Assunto.Coisar;

namespace MyApp.Module.Application.Assunto.Coisar;
public class CoisarAssuntoHandler: HandlerBase, ICoisarAssuntoHandler
{
    public async Task<CoisarAssuntoResponse> Post(CoisarAssuntoRequest request)
    {
        return await Task.FromResult(new CoisarAssuntoResponse($"{DateTime.Now}", $"{Guid.NewGuid()}"));
    }
}
