namespace MyApp.Module.Application.Shared.OutroAssunto.Criar;

public interface ICriarOutroAssuntoHandler
{
    public Task Post(CoisarOutroAssuntoRequest criarOutroAssuntoRequest);
}

public record CoisarOutroAssuntoRequest(string Campo1, string Campo2)
{
    public string? Campo3 { get; init; }
};