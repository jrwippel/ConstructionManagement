namespace WebAppSystemsObra.Models.Dto
{
    public class RelatorioObraViewModel
    {
        public string DescricaoObra { get; set; }
        public string NomeProponente { get; set; }
        public string Endereco { get; set; }
        public int NumeroEtapa { get; set; }
        public decimal MensuradoAnterior { get; set; }

        public List<RelatorioServico> Servicos { get; set; }
        public List<RelatorioFoto> Fotos { get; set; }

        public List<ResumoEtapaDto> ResumoPorEtapa { get; set; } = new();

        public byte[] GraficoEvolucaoEtapas { get; set; }


    }

    public class RelatorioServico
    {
        public string Descricao { get; set; }
        public decimal PercentualIncidencia { get; set; }
        public decimal PercentualExecucao { get; set; }
    }

    public class RelatorioFoto
    {
        public byte[] Bytes { get; set; }
        public string Descricao { get; set; }
    }

}
