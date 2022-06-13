using MyApp.Module.Application.Shared.Assunto.Excluir;

namespace MyApp.Module.Application.Assunto.Excluir;

public class ExcluirAssuntoHandler : HandlerBase, IExcluirAssuntoHandler
{
    public async Task<ExcluirAssuntoResponse> Delete(ExcluirAssuntoRequest request)
    {
        return await Task.FromResult(new ExcluirAssuntoResponse {FoiExcluido = true});
        //await Task.CompletedTask;
    }
}