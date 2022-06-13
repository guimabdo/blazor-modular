namespace MyApp.Module.Application.Shared.Assunto.Excluir;

public interface IExcluirAssuntoHandler
{
    Task<ExcluirAssuntoResponse> Delete(ExcluirAssuntoRequest request);
}

public record ExcluirAssuntoRequest(
    Guid Id);  

public class ExcluirAssuntoResponse
{
    public bool FoiExcluido { get; init; }
}