using System.Net;
using System.Net.Mail;

namespace Doalim_dev.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task EnviarEmailAsync(string para, string assunto, string corpo)
        {
            // Em desenvolvimento, apenas loga o e-mail no console
            // Substitua pela configuração SMTP real antes de publicar
            var ambiente = _config["Ambiente"] ?? "Development";

            if (ambiente == "Development")
            {
                _logger.LogInformation("=== E-MAIL (DEV - não enviado) ===");
                _logger.LogInformation("Para: {Para}", para);
                _logger.LogInformation("Assunto: {Assunto}", assunto);
                _logger.LogInformation("Corpo: {Corpo}", corpo);
                _logger.LogInformation("===================================");
                return;
            }

            // Configuração SMTP para produção (preencher no appsettings.json)
            var host = _config["Email:Host"] ?? throw new InvalidOperationException("Email:Host não configurado.");
            var port = int.Parse(_config["Email:Port"] ?? "587");
            var usuario = _config["Email:Usuario"] ?? throw new InvalidOperationException("Email:Usuario não configurado.");
            var senha = _config["Email:Senha"] ?? throw new InvalidOperationException("Email:Senha não configurado.");
            var remetente = _config["Email:Remetente"] ?? usuario;

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(usuario, senha),
                EnableSsl = true
            };

            var mensagem = new MailMessage(remetente, para, assunto, corpo)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mensagem);
        }
    }
}
