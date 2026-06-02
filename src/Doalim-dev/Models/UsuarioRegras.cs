namespace Doalim_dev.Models
{
    public static class UsuarioRegras
    {
        public static bool TemComprovacaoAprovada(Usuario usuario)
        {
            if (usuario.TipoUsuario == TipoUsuario.Admin)
                return usuario.Ativo;

            // Basta o admin ter marcado como Aprovado — não exige arquivo em disco
            return usuario.StatusVerificacao == StatusVerificacao.Aprovado
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

        /// <summary>
        /// Valida os dígitos verificadores do CPF (algoritmo Receita Federal).
        /// Retorna false para CPFs com todos os dígitos iguais (ex: 111.111.111-11).
        /// </summary>
        public static bool CpfValido(string? cpf)
        {
            var d = NormalizarDigitos(cpf);
            if (d.Length != 11 || d.Distinct().Count() == 1) return false;

            int Soma(int multiplicadorInicial)
            {
                int s = 0;
                for (int i = 0; i < multiplicadorInicial - 1; i++)
                    s += int.Parse(d[i].ToString()) * (multiplicadorInicial - i);
                return s;
            }

            int Resto(int soma)
            {
                int r = (soma * 10) % 11;
                return (r == 10 || r == 11) ? 0 : r;
            }

            return Resto(Soma(10)) == int.Parse(d[9].ToString())
                && Resto(Soma(11)) == int.Parse(d[10].ToString());
        }

        /// <summary>
        /// Valida os dígitos verificadores do CNPJ (algoritmo Receita Federal).
        /// </summary>
        public static bool CnpjValido(string? cnpj)
        {
            var d = NormalizarDigitos(cnpj);
            if (d.Length != 14 || d.Distinct().Count() == 1) return false;

            int Calc(int[] pesos)
            {
                int soma = pesos.Select((p, i) => p * int.Parse(d[i].ToString())).Sum();
                int r = soma % 11;
                return r < 2 ? 0 : 11 - r;
            }

            return Calc(new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }) == int.Parse(d[12].ToString())
                && Calc(new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }) == int.Parse(d[13].ToString());
        }
    }
}