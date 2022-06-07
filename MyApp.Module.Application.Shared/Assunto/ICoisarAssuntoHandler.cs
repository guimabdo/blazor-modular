namespace MyApp.Module.Application.Shared.Assunto;
public interface ICoisarAssuntoHandler  // HTTP POST /CoisarAssuntoHandler
{
    Task<CoisarAssuntoResponse> Post(CoisarAssuntoRequest request);
}

public record CoisarAssuntoRequest(string Campo1, string Campo2)
{
    public string? Campo3 { get; init; }
};
public record CoisarAssuntoResponse(string Campo1, string Campo2);

//public class CoisarAssuntoValidator()
