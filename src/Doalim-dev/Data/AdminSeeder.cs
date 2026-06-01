using Doalim_dev.Models;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("AdminSeeder");

            var email = configuration["AdminSeed:Email"] ?? "admin@doalim.com";
            var senha = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(senha))
            {
                logger.LogWarning("AdminSeed:Password nao foi configurado. Seed do administrador ignorado.");
                return;
            }

            try
            {
                await context.Database.MigrateAsync();

                var usuario = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null)
                {
                    usuario = new Usuario
                    {
                        Nome = configuration["AdminSeed:Nome"] ?? "Administrador Doalim",
                        Email = email,
                        SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                        Telefone = configuration["AdminSeed:Telefone"] ?? "(31) 99999-0000",
                        //Endereco = configuration["AdminSeed:Endereco"] ?? "Sede administrativa Doalim",
                        TipoUsuario = TipoUsuario.Admin,
                        StatusVerificacao = StatusVerificacao.Pendente,
                        Ativo = true,
                        DataCadastro = DateTime.UtcNow
                    };

                    context.Usuarios.Add(usuario);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Usuario administrador criado com o e-mail {Email}.", email);
                }
                else
                {
                    usuario.TipoUsuario = TipoUsuario.Admin;
                    usuario.Ativo = true;
                    if (usuario.StatusVerificacao == StatusVerificacao.NaoAplicavel)
                        usuario.StatusVerificacao = StatusVerificacao.Pendente;

                    if (string.IsNullOrWhiteSpace(usuario.Telefone))
                        usuario.Telefone = configuration["AdminSeed:Telefone"] ?? "(31) 99999-0000";

                    //if (string.IsNullOrWhiteSpace(usuario.Endereco))
                        //usuario.Endereco = configuration["AdminSeed:Endereco"] ?? "Sede administrativa Doalim";

                    if (configuration.GetValue<bool>("AdminSeed:ResetPassword"))
                        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

                    await context.SaveChangesAsync();
                }

                var administradorExiste = await context.Administradores
                    .AnyAsync(a => a.IdUsuario == usuario.IdUsuario);

                if (!administradorExiste)
                {
                    context.Administradores.Add(new Administrador
                    {
                        IdUsuario = usuario.IdUsuario
                    });

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Nao foi possivel criar ou atualizar o usuario administrador.");
            }
        }
    }
}
