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

    public enum StatusLote
    {
        Disponivel = 0,
        Reservado = 1,
        Entregue = 2,
        Inativo = 3
    }

    public enum StatusPedido
    {
        Pendente = 0,
        Confirmado = 1,
        Retirado = 2,
        Cancelado = 3
    }

    public enum StatusReserva
    {
        Pendente = 0,
        Confirmada = 1,
        Retirada = 2,
        Cancelada = 3,
        Rejeitada = 4
    }

    public enum TipoNotificacao
    {
        ReservaPendente  = 0,   // Doador: beneficiário fez uma reserva
        ReservaAprovada  = 1,   // Beneficiário: doador aprovou
        ReservaRejeitada = 2,   // Beneficiário: doador rejeitou
        LembreteRetirada = 3,   // Beneficiário: retirada próxima do vencimento
        DoacaoExpirada   = 4    // Doador: lote vencido com reserva associada
    }
}
