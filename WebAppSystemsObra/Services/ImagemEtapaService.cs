using WebAppSystems.Models;
using WebAppSystemsObra.Repository;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WebAppSystemsObra.Services
{
    public class ImagemEtapaService
    {
        private readonly IImagemEtapaRepository _imagemEtapaRepository;
        private readonly BlobStorageService _blobStorageService; // 🔗 Adicionando o serviço de Azure Blob Storage

        public ImagemEtapaService(IImagemEtapaRepository imagemEtapaRepository, BlobStorageService blobStorageService)
        {
            _imagemEtapaRepository = imagemEtapaRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task AdicionarImagem(int etapaId, int numeroEtapaId, string fileName, string urlImagem, string descricao)
        {
            var imagem = new ImagemEtapa
            {
                EtapaId = etapaId, // 🔗 Agora recebe o ID correto da etapa!
                NumeroEtapaId = numeroEtapaId, // 🔗 Apenas referência numérica
                NomeArquivo = fileName,
                UrlImagem = urlImagem,
                Descricao = descricao
            };

            _imagemEtapaRepository.Adicionar(imagem);
        }


        public async Task RemoverImagem(int imagemId)
        {
            var imagem = _imagemEtapaRepository.BuscarPorId(imagemId);
            if (imagem == null)
                throw new Exception("Imagem não encontrada.");

            // 🗑️ Remove o arquivo físico do disco
            var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "etapas", imagem.NomeArquivo);
            if (System.IO.File.Exists(caminhoImagem))
            {
                System.IO.File.Delete(caminhoImagem);
            }

            // 🧹 Remove o registro do banco de dados
            _imagemEtapaRepository.Remover(imagemId);
        }


        /*
        public async Task RemoverImagem(int imagemId)
        {
            var imagem = _imagemEtapaRepository.BuscarPorId(imagemId);
            if (imagem == null)
                throw new Exception("Imagem não encontrada.");

            // 🔗 Primeiro, remove do Azure Blob Storage
            await _blobStorageService.DeleteFileAsync(imagem.NomeArquivo);

            // 🔗 Depois, remove do banco de dados
            _imagemEtapaRepository.Remover(imagemId);
        }

        */

        public IEnumerable<ImagemEtapa> ListarPorEtapa(int etapaId)
        {
            return _imagemEtapaRepository.ListarPorEtapa(etapaId);
        }

        public IEnumerable<ImagemEtapa> ListarPorObra(int obraId)
        {
            return _imagemEtapaRepository.ListarPorObra(obraId);
        }
    }
}
