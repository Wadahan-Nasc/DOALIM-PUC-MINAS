namespace Doalim_dev.Models
{
    public enum TipoUsuario
    {
        DoadorPF = 0,
        DoadorPJ = 1,
        BeneficiarioPF = 2,
        BeneficiarioPJ = 3,
        Admin = 4
    }

    public enum StatusVerificacao
    {
        NaoAplicavel = 0,
        Pendente = 1,
        Aprovado = 2,
        Rejeitado = 3
    }

    public enum StatusDoacao
    {
        Disponivel = 1,
        Reservado = 2,
        Entregue = 3,
        Cancelado = 4
    }
}
