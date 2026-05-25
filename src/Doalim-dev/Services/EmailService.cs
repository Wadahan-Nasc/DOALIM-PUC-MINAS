using SendGrid;
using SendGrid.Helpers.Mail;

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
            var apiKey = _config["SendGrid:ApiKey"];
            var fromEmail = _config["SendGrid:FromEmail"] ?? "noreply@doalim.com";
            var fromName = _config["SendGrid:FromName"] ?? "Doalim";

            // Se não tiver chave configurada, apenas loga (ambiente de dev sem chave)
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogInformation("=== E-MAIL (SendGrid não configurado) ===");
                _logger.LogInformation("Para: {Para}", para);
                _logger.LogInformation("Assunto: {Assunto}", assunto);
                _logger.LogInformation("Corpo: {Corpo}", corpo);
                _logger.LogInformation("=========================================");
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(para);
            var message = MailHelper.CreateSingleEmail(from, to, assunto, null, corpo);

            var response = await client.SendEmailAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Body.ReadAsStringAsync();
                _logger.LogError("Falha ao enviar e-mail via SendGrid: {Status} - {Erro}",
                    response.StatusCode, erro);

                throw new Exception($"Falha ao enviar e-mail: {response.StatusCode}");
            }

            _logger.LogInformation("E-mail enviado via SendGrid para {Para}", para);
        }
    }
}