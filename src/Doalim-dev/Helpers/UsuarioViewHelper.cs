namespace Doalim_dev.Helpers
{
    /// <summary>
    /// Funções utilitárias compartilhadas entre as views de usuário.
    /// Centraliza Iniciais e MascaraEmail para evitar duplicação em
    /// Index, Details, Delete e outras views de Usuarios.
    /// </summary>
    public static class UsuarioViewHelper
    {
        /// <summary>
        /// Retorna as iniciais do nome (primeira letra do primeiro e do último nome).
        /// Ex.: "João Silva" → "JS"
        /// </summary>
        public static string Iniciais(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "US";

            var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var primeira = partes[0][0].ToString();
            var segunda  = partes.Length > 1 ? partes[^1][0].ToString() : string.Empty;
            return (primeira + segunda).ToUpperInvariant();
        }

        /// <summary>
        /// Mascara o e-mail para exibição pública.
        /// Ex.: "joao.silva@gmail.com" → "jo***@gmail.com"
        /// </summary>
        public static string MascaraEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return "Nao informado";

            var partes = email.Split('@', 2);
            var nome   = partes[0].Length <= 2
                ? partes[0][0] + "*"
                : partes[0][..2] + "***";

            return $"{nome}@{partes[1]}";
        }

        /// <summary>
        /// Detecta o MIME type de um arquivo de comprovação pelos magic bytes.
        /// Suporta PDF, PNG e JPEG. Fallback: application/octet-stream.
        /// </summary>
        public static string MimeDocumento(byte[]? arquivo)
        {
            if (arquivo == null || arquivo.Length < 4) return "application/octet-stream";

            // PDF: %PDF
            if (arquivo[0] == 0x25 && arquivo[1] == 0x50 &&
                arquivo[2] == 0x44 && arquivo[3] == 0x46)
                return "application/pdf";

            // PNG: \x89PNG
            if (arquivo[0] == 0x89 && arquivo[1] == 0x50 &&
                arquivo[2] == 0x4E && arquivo[3] == 0x47)
                return "image/png";

            // JPEG: FF D8
            if (arquivo[0] == 0xFF && arquivo[1] == 0xD8)
                return "image/jpeg";

            return "application/octet-stream";
        }
    }
}
