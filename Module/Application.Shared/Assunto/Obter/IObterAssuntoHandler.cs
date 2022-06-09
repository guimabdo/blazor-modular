namespace MyApp.Module.Application.Shared.Assunto.Obter;

public interface IObterAssuntoHandler
{
    Task<ObterAssuntoResponse> Get(ObterAssuntoRequest request);
}

public record ObterAssuntoRequest(string Filtro);  
public class ObterAssuntoResponse
{
    public string? Campo1 { get; init; }
}