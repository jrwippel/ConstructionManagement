using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebAppSystemsObra.Models.Dto;

namespace WebAppSystemsObra.Services
{

    public class PdfService
    {
        public byte[] GerarRelatorioPLS(RelatorioObraViewModel dados)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Relatório PLS da Obra - Etapa {dados.NumeroEtapa}")
                                 .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text($"Obra: {dados.DescricaoObra}");
                        col.Item().Text($"Proponente: {dados.NomeProponente}");
                        col.Item().Text($"Endereço: {dados.Endereco}");

                        col.Item().PaddingVertical(10).Element(container =>
                        {
                            container.LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                        col.Item().Text("Evolução da Etapa").Bold().FontSize(14);

                        col.Item().PaddingTop(10).Element(e =>
                        {
                            e.DefaultTextStyle(x => x.FontSize(10)).Table(table =>  // 👈 aqui aplicamos o estilo de texto local
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(90);
                                    columns.ConstantColumn(100);
                                });

                                // Cabeçalho
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellHeader).Text("Serviço").FontColor(Colors.White).SemiBold();
                                    header.Cell().Element(CellHeader).AlignCenter().Text("Incidência (%)").FontColor(Colors.White).SemiBold();
                                    header.Cell().Element(CellHeader).AlignCenter().Text("Executado Item (%)").FontColor(Colors.White).SemiBold();
                                    header.Cell().Element(CellHeader).AlignCenter().Text("Execução na Obra (%)").FontColor(Colors.White).SemiBold();
                                });

                                // Conteúdo
                                foreach (var s in dados.Servicos)
                                {
                                    var pesoFinal = (s.PercentualIncidencia * s.PercentualExecucao) / 100;

                                    table.Cell().Element(CellContent).Text(s.Descricao);
                                    table.Cell().Element(CellContent).AlignCenter().Text($"{s.PercentualIncidencia:0.##}%");
                                    table.Cell().Element(CellContent).AlignCenter().Text($"{s.PercentualExecucao:0.##}%");
                                    table.Cell().Element(CellContent).AlignCenter().Text($"{pesoFinal:0.##}%");
                                }

                                static IContainer CellHeader(IContainer container) =>
                                    container
                                        .Background("#003366")
                                        .Border(0.5f)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .PaddingVertical(4)
                                        .PaddingHorizontal(3)
                                        .ShowOnce();

                                static IContainer CellContent(IContainer container) =>
                                    container
                                        .Border(0.5f)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .PaddingVertical(3)
                                        .PaddingHorizontal(3);
                            });
                        });

                        //Resumo
                        col.Item().PaddingTop(20).Element(e =>
                        {
                            e.DefaultTextStyle(x => x.FontSize(10)).Table(tabela =>
                            {
                                tabela.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();     // Indicador
                                    c.ConstantColumn(110);  // Valor (%)
                                });

                                tabela.Header(header =>
                                {
                                    header.Cell().Element(CellHeader).Text("Indicador").FontColor(Colors.White).SemiBold();
                                    header.Cell().Element(CellHeader).AlignRight().Text("Valor").FontColor(Colors.White).SemiBold();
                                });

                                void LinhaResumo(string titulo, decimal valor, string cor)
                                {
                                    tabela.Cell().Element(CellContent).Text(titulo);
                                    tabela.Cell().Element(CellContent).AlignRight().Text($"{valor:0.##}%").FontColor(cor);
                                }

                                var totalAtual = dados.Servicos.Sum(s => s.PercentualIncidencia * s.PercentualExecucao / 100);
                                var totalAnterior = dados.MensuradoAnterior;
                                var diferenca = totalAtual - totalAnterior;

                                LinhaResumo("Mensurado Acumulado Atual", totalAtual, Colors.Green.Darken2);
                                LinhaResumo("Mensurado Acumulado Mês Anterior", totalAnterior, Colors.Blue.Medium);
                                LinhaResumo("% Executado nesta Etapa", diferenca, diferenca >= 0 ? Colors.Green.Darken1 : Colors.Red.Darken2);

                                static IContainer CellHeader(IContainer container) =>
                                    container.Background("#003366")
                                             .Border(0.5f)
                                             .BorderColor(Colors.Grey.Lighten1)
                                             .PaddingVertical(4)
                                             .PaddingHorizontal(5)
                                             .ShowOnce();

                                static IContainer CellContent(IContainer container) =>
                                    container.Border(0.5f)
                                             .BorderColor(Colors.Grey.Lighten2)
                                             .PaddingVertical(4)
                                             .PaddingHorizontal(5);
                            });
                        });

                        col.Item().PageBreak();
                        col.Item().PaddingTop(20).Text("Histórico de Execução por Etapa").Bold().FontSize(13);

                        col.Item().Table(tabela =>
                        {
                            tabela.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(70);   // Etapa
                                c.ConstantColumn(120);  // Data
                                c.ConstantColumn(130);  // % Executado
                                c.ConstantColumn(130);  // % Acumulado
                            });

                            tabela.Header(header =>
                            {
                                header.Cell().Element(CellHeader).Text("Etapa").FontColor(Colors.White).SemiBold();
                                header.Cell().Element(CellHeader).Text("Data de Referência").FontColor(Colors.White).SemiBold();
                                header.Cell().Element(CellHeader).AlignCenter().Text("% Executado").FontColor(Colors.White).SemiBold();
                                header.Cell().Element(CellHeader).AlignCenter().Text("% Acumulado").FontColor(Colors.White).SemiBold();
                            });

                            foreach (var etapa in dados.ResumoPorEtapa)
                            {

                                var dataTexto = etapa.DataReferencia > DateTime.MinValue
                                ? etapa.DataReferencia.ToString("dd/MM/yyyy"): "—";

                                tabela.Cell().Element(CellContent).Text($"Etapa {etapa.NumeroEtapa}");
                                tabela.Cell().Element(CellContent).Text(etapa.DataReferencia.ToString("dd/MM/yyyy"));
                                tabela.Cell().Element(CellContent).AlignCenter().Text($"{etapa.PercentualExecutadoNaEtapa:0.##}%");
                                tabela.Cell().Element(CellContent).AlignCenter().Text($"{etapa.PercentualAcumulado:0.##}%");
                            }

                            static IContainer CellHeader(IContainer c) =>
                                c.Background("#003366")
                                 .Border(0.5f)
                                 .BorderColor(Colors.Grey.Lighten1)
                                 .PaddingVertical(4)
                                 .PaddingHorizontal(4)
                                 .ShowOnce();

                            static IContainer CellContent(IContainer c) =>
                                c.Border(0.5f)
                                 .BorderColor(Colors.Grey.Lighten2)
                                 .PaddingVertical(4)
                                 .PaddingHorizontal(4);
                        });                       


                        // Gráfico de evolução
                        if (dados.GraficoEvolucaoEtapas?.Length > 0)
                        {
                            col.Item().PageBreak(); // opcional: forçar nova página
                            col.Item().Text("Gráfico de Evolução por Etapa").Bold().FontSize(13);
                            col.Item().Image(dados.GraficoEvolucaoEtapas).FitWidth();
                        }

                        // Espaço após o gráfico
                        col.Item().PaddingTop(30); // 🔹 2 linhas de respiro (~20px)

                        col.Item().Text("Relatório Fotográfico").Bold().FontSize(13);

                        // Espaço após o gráfico
                        col.Item().PaddingTop(20); // 🔹 2 linhas de respiro (~20px)

                        // Galeria de fotos (2 por linha com espaçamento entre linhas)
                        for (int i = 0; i < dados.Fotos.Count; i += 2)
                        {
                            col.Item().PaddingBottom(10).Row(row => // 🔹 espaço de 1 linha entre linhas de fotos
                            {
                                // Primeira imagem
                                row.RelativeItem().Column(colFoto =>
                                {
                                    var foto = dados.Fotos[i];

                                    if (foto.Bytes?.Length > 0)
                                    {
                                        colFoto.Item().Element(c =>
                                            c.Width(140).Height(100).Image(foto.Bytes).FitArea()
                                        );
                                    }
                                    else
                                    {
                                        colFoto.Item().Text("[Imagem indisponível]")
                                            .FontColor(Colors.Grey.Darken1).Italic().FontSize(9);
                                    }

                                    colFoto.Item().Text(foto.Descricao ?? "")
                                        .FontSize(9).Italic();
                                });

                                // Segunda imagem (se houver)
                                if (i + 1 < dados.Fotos.Count)
                                {
                                    var foto2 = dados.Fotos[i + 1];

                                    row.RelativeItem().Column(colFoto =>
                                    {
                                        colFoto.Item().PaddingLeft(10); // 🔹 espaço entre colunas

                                        if (foto2.Bytes?.Length > 0)
                                        {
                                            colFoto.Item().Element(c =>
                                                c.Width(140).Height(100).Image(foto2.Bytes).FitArea()
                                            );
                                        }
                                        else
                                        {
                                            colFoto.Item().Text("[Imagem indisponível]")
                                                .FontColor(Colors.Grey.Darken1).Italic().FontSize(9);
                                        }

                                        colFoto.Item().Text(foto2.Descricao ?? "")
                                            .FontSize(9).Italic();
                                    });
                                }
                                else
                                {
                                    row.RelativeItem(); // 🔹 mantém alinhamento em caso ímpar
                                }
                            });
                        }




                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Gerado em ");
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    });
                });
            });

            return pdf.GeneratePdf();
        }


    }
}
