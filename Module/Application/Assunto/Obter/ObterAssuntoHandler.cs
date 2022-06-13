using MyApp.Module.Application.Shared.Assunto.Obter;

namespace MyApp.Module.Application.Assunto.Obter;

public class ObterAssuntoHandler : HandlerBase, IObterAssuntoHandler
{
    public Task<ObterAssuntoResponse> Get(ObterAssuntoRequest request)
    {
        return Task.FromResult(new ObterAssuntoResponse
        {
            Campo1 = $"Resultado [{request.Filtro} | {string.Join(" - ", request.LanguagesAvailable)} | {request.StandardLanguage}] para {request.Filtro} em [{DateTime.Now}]"
        });
    }
}