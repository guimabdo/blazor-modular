using MyApp.Module.Application.Shared.OutroAssunto.Criar;

namespace MyApp.Module.Application.OutroAssunto.Criar;

public class CriarCoisaOutroAssuntoHandler : HandlerBase, ICriarOutroAssuntoHandler
{
    public async Task Post(CoisarOutroAssuntoRequest criarOutroAssuntoRequest)
    {
        await Task.CompletedTask;
    }
}