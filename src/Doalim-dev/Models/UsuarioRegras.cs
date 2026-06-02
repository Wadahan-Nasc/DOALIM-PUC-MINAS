namespace Doalim_dev.Models
{
    public static class UsuarioRegras
    {
        public static bool TemComprovacaoAprovada(Usuario usuario)
        {
            if (usuario.TipoUsuario == TipoUsuario.Admin)
                return usuario.Ativo;

            return usuario.StatusVerificacao == StatusVerificacao.Aprovado
                && usuario.Arquivocomprovacao != null
                && usuario.Arquivocomprovacao.Length > 0
                && usuario.Ativo;
        }

        public static bool PrecisaComprovacao(Usuario usuario)
        {
            return usuario.TipoUsuario != TipoUsuario.Admin;
        }

        public static bool EhPessoaJuridica(Usuario usuario)
        {
            return usuario.TipoUsuario == TipoUsuario.DoadorPJ
                || usuario.TipoUsuario == TipoUsuario.BeneficiarioPJ
                || !string.IsNullOrWhiteSpace(usuario.Cnpj);
        }

        public static TipoUsuario TipoDoadorCorrespondente(Usuario usuario)
        {
            return EhPessoaJuridica(usuario) ? TipoUsuario.DoadorPJ : TipoUsuario.DoadorPF;
        }

        public static TipoUsuario TipoBeneficiarioCorrespondente(Usuario usuario)
        {
            return EhPessoaJuridica(usuario) ? TipoUsuario.BeneficiarioPJ : TipoUsuario.BeneficiarioPF;
        }

        public static string NormalizarDigitos(string? valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}