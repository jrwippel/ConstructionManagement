using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using WebAppSystems.Data;
using WebAppSystems.Helper;
using WebAppSystems.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using WebAppSystems.Services.Exceptions;
using WebAppSystemsObra.Services;
using WebAppSystemsObra.Repository;
using QuestPDF.Infrastructure;

namespace WebAppSystems
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Define a cultura padrão para "pt-BR"
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            // Configurar o contexto do banco de dados
            builder.Services.AddDbContext<WebAppSystemsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebAppSystemsContext")
                ?? throw new InvalidOperationException("Connection string 'WebAppSystemsContext' not found.")));

            // Injetar repositórios
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IObraRepository, ObraRepository>();
            builder.Services.AddScoped<IEtapaRepository, EtapaRepository>();
            builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
            builder.Services.AddScoped<IExecucaoRepository, ExecucaoRepository>();

            builder.Services.AddScoped<IImagemEtapaRepository, ImagemEtapaRepository>();
            builder.Services.AddScoped<ImagemEtapaService>();         




            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<SeedingService>();
            builder.Services.AddScoped<BasicAuthenticationFilterAttribute, BasicAuthenticationFilterAttribute>();
            builder.Services.AddScoped<AttorneyService>();
            builder.Services.AddScoped<ISessao, Sessao>();
            builder.Services.AddScoped<IEmail, Email>();
            builder.Services.AddScoped<ParametroService>();
            builder.Services.AddScoped<ClienteService>();
            builder.Services.AddScoped<ObraService>();
            builder.Services.AddScoped<EtapaService>();
            builder.Services.AddScoped<ServicoService>();
            builder.Services.AddScoped<ExecucaoService>();
            builder.Services.AddScoped<BlobStorageService>(); // 🔁 Adicionando o serviço de Azure Blob Storage
            builder.Services.AddScoped<RelatorioService>();
            builder.Services.AddScoped<PdfService>();
         


            builder.Services.AddHttpClient();

            builder.Services.AddSession(o =>
            {
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
                o.IdleTimeout = TimeSpan.FromMinutes(240);
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();

            // Configurar autenticação JWT
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Registra o HttpClient
            builder.Services.AddHttpClient();

            // Registra o serviço KeepAlive
            builder.Services.AddHostedService<KeepAliveService>();

            var app = builder.Build();

            var ptBR = new CultureInfo("pt-BR");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ptBR),
                SupportedCultures = new List<CultureInfo> { ptBR },
                SupportedUICultures = new List<CultureInfo> { ptBR }
            };

            app.UseRequestLocalization(localizationOptions);

            // Aplicar migrações pendentes no banco de dados
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var myDbContext = services.GetRequiredService<WebAppSystemsContext>();
                    myDbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            // Seed data
            app.Services.CreateScope().ServiceProvider.GetRequiredService<SeedingService>().Seed();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Adicionar middlewares de autenticação e autorização
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllers(); // Adicione suporte ao roteamento da API

            app.MapControllerRoute(
                name: "about",
                pattern: "about",
                defaults: new { controller = "Home", action = "About" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
