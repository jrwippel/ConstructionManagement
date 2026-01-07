using WebAppSystems.Models;

public interface IImagemEtapaRepository
{
    void Adicionar(ImagemEtapa imagem);
    ImagemEtapa BuscarPorId(int id);
    IEnumerable<ImagemEtapa> ListarTodas();
    void Remover(int id);
    IEnumerable<ImagemEtapa> ListarPorEtapa(int etapaId);
    IQueryable<ImagemEtapa> ListarPorObra(int obraId); // 🔁 Alterado para `IQueryable<T>`
}
