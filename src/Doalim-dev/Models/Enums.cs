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

    public enum StatusLote
    {
        Disponivel = 0,
        Reservado = 1,
        Entregue = 2,
        Inativo = 3
    }

    public enum StatusPedido
    {
        Pendente = 0,       // Todas pendentes
        Confirmado = 1,     // Alguma confirmada, mas não todas
        Retirado = 2,       // Todas retiradas
        Cancelado = 3       // Todas canceladas
    }
    public enum StatusReserva
    {
        Pendente = 0,
        Confirmada = 1,
        Retirada = 2,
        Cancelada = 3,
        Rejeitada = 4 // Essencial para diferenciar reservas canceladas pelo beneficiário e rejeitadas pelo doador;
    }

}