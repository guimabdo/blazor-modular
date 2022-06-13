namespace MyApp.Module.Application.Shared.Assunto.Obter;

public interface IObterAssuntoHandler
{
    Task<ObterAssuntoResponse> Get(ObterAssuntoRequest request);
}

public record ObterAssuntoRequest(
    string Filtro,
    LanguageType StandardLanguage,
    IEnumerable<LanguageType> LanguagesAvailable);  

public class ObterAssuntoResponse
{
    public string? Campo1 { get; init; }
}

public enum LanguageType
{
    CSharp,
    FSharp,
    TypeScript,
    CPlusPlus,
    C,
    Java,
    JavaScript,
    Python
}