namespace WebAppSystemsObra.Models.Dto
{
    public class ResumoEtapaDto
    {
        public int NumeroEtapa { get; set; }
        public DateTime DataReferencia { get; set; }
        public decimal PercentualExecutadoNaEtapa { get; set; }
        public decimal PercentualAcumulado { get; set; }
    }

}
