using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppSystems.Models;



namespace WebAppSystems.Data
{
    public class WebAppSystemsContext : DbContext
    {
        public WebAppSystemsContext (DbContextOptions<WebAppSystemsContext> options)
            : base(options)
        {
        }        
        public DbSet<WebAppSystems.Models.Attorney> Attorney { get; set; } = default!;
        public DbSet<WebAppSystems.Models.Cliente> Clientes { get; set; } = default!;
        public DbSet<WebAppSystems.Models.Obra> Obras { get; set; } = default!;
        public DbSet<WebAppSystems.Models.Etapa> Etapas { get; set; } = default!;

        public DbSet<WebAppSystems.Models.Servico> Servicos { get; set; } = default!;

        public DbSet<WebAppSystems.Models.Execucao> Execucaos { get; set; } = default!;

        public DbSet<WebAppSystems.Models.ImagemEtapa> ImagensEtapa { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Execucao>()
                .HasOne(e => e.Servico)
                .WithMany(s => s.Execucao)
                .HasForeignKey(e => e.ServicoId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 volta pra Restrict
        }

        public DbSet<WebAppSystems.Models.Parametros>? Parametros { get; set; }
       



    }
}


