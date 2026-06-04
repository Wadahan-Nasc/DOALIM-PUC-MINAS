using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    // Apenas beneficiários (PF ou PJ) têm acesso ao carrinho
    [Authorize(Roles = "BeneficiarioPF,BeneficiarioPJ")]
    public class CarrinhoController : BaseController
    {
        private const int DuracaoCarrinhoMinutos = 30;

        public CarrinhoController(AppDbContext context) : base(context) { }

        // -------------------------------------------------------------------------
        // LIMPEZA DE ITENS EXPIRADOS
        // Chamado no início de toda action para garantir que itens
        // expirados não interfiram nas operações do carrinho.
        // -------------------------------------------------------------------------

        private async Task LimparItensExpiradosAsync(int idBeneficiario)
        {
            var itensExpirados = await _context.CarrinhoItens
                .Where(c => c.IdBeneficiario == idBeneficiario
                         && c.DataExpiracao < DateTime.UtcNow)
                .ToListAsync();

            if (itensExpirados.Any())
            {
                _context.CarrinhoItens.RemoveRange(itensExpirados);
                await _context.SaveChangesAsync();
            }
        }

        // -------------------------------------------------------------------------
        // GET: /Carrinho
        // Exibe os itens do carrinho do beneficiário logado.
        // -------------------------------------------------------------------------

        public async Task<IActionResult> Index()
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            await LimparItensExpiradosAsync(usuarioId);

            var itens = await _context.CarrinhoItens
                .Include(c => c.Produto)
                    .ThenInclude(p => p.Doador)
                        .ThenInclude(d => d.Usuario)
                .Include(c => c.Produto)
                    .ThenInclude(p => p.Lotes)
                .Where(c => c.IdBeneficiario == usuarioId)
                .ToListAsync();

            var hoje = DateTime.Today;

            var itensViewModel = itens.Select(c =>
            {
                var loteMaisUrgente = c.Produto.Lotes
                    .Where(l => l.DataValidade > hoje
                             && l.StatusLote == StatusLote.Disponivel
                             && l.Quantidade > 0)
                    .OrderBy(l => l.DataValidade)
                    .FirstOrDefault();

                return new CarrinhoItemViewModel
                {
                    IdCarrinho = c.IdCarrinhoItem,
                    IdProduto = c.IdProduto,
                    NomeProduto = c.Produto.NomeProduto,
                    MarcaProduto = c.Produto.MarcaProduto,
                    CategoriaProduto = c.Produto.CategoriaProduto,
                    UnidadeMedidaProduto = c.Produto.UnidadeMedida,
                    FotoProduto = ObterFotoProdutoDataUrl(c.Produto.FotoProduto),
                    NomeDoador = c.Produto.Doador.Usuario.Nome,
                    QuantidadeDesejada = c.QuantidadeDesejada,
                    DataExpiracao = c.DataExpiracao,
                    // Dados do lote mais urgente — null se não houver lote disponível
                    NumeroLote = loteMaisUrgente?.NumeroLote ?? "Sem lote disponível",
                    DataValidadeLote = loteMaisUrgente?.DataValidade ?? DateTime.MinValue,
                    QuantidadeDisponivelLote = loteMaisUrgente?.Quantidade ?? 0,
                    // Sinaliza se o lote comporta quantidade desejada
                    LoteDisponivel = loteMaisUrgente != null && loteMaisUrgente.Quantidade >= c.QuantidadeDesejada,
                    AvisoDisponibilidade = loteMaisUrgente == null ? "Produto indisponível no momento"
                                            : (loteMaisUrgente.Quantidade < c.QuantidadeDesejada ?
                                            $"Quantidade indisponível. Máximo: {loteMaisUrgente.Quantidade}." : null)
                };
            }).ToList();

            var viewModel = new CarrinhoViewModel
            {
                Itens = itensViewModel,
            };

            return View(viewModel);
        }

        // -------------------------------------------------------------------------
        // POST: /Carrinho/Adicionar
        // Adiciona um produto ao carrinho. Se já existir, atualiza a quantidade.
        // Se o carrinho já tiver 15 produtos distintos, bloqueia a adição.
        // Valida aprovação do beneficiário e limites de quantidade por tipo (PF/PJ).
        // -------------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(int idProduto, int quantidade)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            // Garante que apenas beneficiários aprovados podem adicionar ao carrinho
            var aprovado = await UsuarioPodeReservarAsync(usuarioId);

            if (!aprovado)
            {
                TempData["Erro"] = "Para reservar, envie o arquivo de comprovação no seu perfil e aguarde a aprovação do administrador.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            await LimparItensExpiradosAsync(usuarioId);

            // Verifica se o produto existe e está disponível
            var produto = await _context.Produtos
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.IdProduto == idProduto && p.StatusProduto);

            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado ou indisponível.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            // Valida limites de quantidade por tipo de beneficiário
            if (User.IsInRole("BeneficiarioPF") && produto.QuantidadePessoaFisica > 0
                && quantidade > produto.QuantidadePessoaFisica)
            {
                TempData["Erro"] = $"Limite máximo para pessoa física: {produto.QuantidadePessoaFisica} unidade(s) de {produto.NomeProduto}.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            if (User.IsInRole("BeneficiarioPJ") && produto.QuantidadePessoaJuridica > 0
                && quantidade > produto.QuantidadePessoaJuridica)
            {
                TempData["Erro"] = $"Limite máximo para pessoa jurídica: {produto.QuantidadePessoaJuridica} unidade(s) de {produto.NomeProduto}.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            // Toda adição (novo item ou atualização de quantidade) reseta o timer
            // do carrinho inteiro para garantir 30 minutos a partir deste momento.
            var novaExpiracao = DateTime.UtcNow.AddMinutes(DuracaoCarrinhoMinutos);

            var itensExistentes = await _context.CarrinhoItens
                .Where(c => c.IdBeneficiario == usuarioId)
                .ToListAsync();

            foreach (var item in itensExistentes)
                item.DataExpiracao = novaExpiracao;

            // Verifica se o produto já está no carrinho
            var itemExistente = itensExistentes.FirstOrDefault(c => c.IdProduto == idProduto);

            // Se já existe, apenas atualiza a quantidade (expiração já foi resetada acima)
            if (itemExistente != null)
            {
                itemExistente.QuantidadeDesejada = quantidade;
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Quantidade atualizada no carrinho.";
                return RedirectToAction(nameof(Index));
            }

            // Verifica o limite de 15 produtos distintos no carrinho
            if (itensExistentes.Count >= 15)
            {
                TempData["Erro"] = "Limite de 15 produtos distintos no carrinho atingido.";
                return RedirectToAction(nameof(Index));
            }

            // Cria um novo item no carrinho com a expiração resetada
            var novoItem = new CarrinhoItem
            {
                IdBeneficiario = usuarioId,
                IdProduto = idProduto,
                QuantidadeDesejada = quantidade,
                DataExpiracao = novaExpiracao,
                DataAdicao = DateTime.UtcNow
            };

            _context.CarrinhoItens.Add(novoItem);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"{produto.NomeProduto} adicionado ao carrinho!";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------------
        // POST: /Carrinho/Remover
        // Remove um item específico do carrinho.
        // -------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int idCarrinhoItem)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            var itemASerRemovido = await _context.CarrinhoItens
                .FirstOrDefaultAsync(c => c.IdCarrinhoItem == idCarrinhoItem
                                       && c.IdBeneficiario == usuarioId);

            if (itemASerRemovido != null)
            {
                _context.CarrinhoItens.Remove(itemASerRemovido);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Item removido do carrinho.";
            }

            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------------
        // POST: /Carrinho/Finalizar
        // Tenta reservar todos os itens do carrinho dentro de uma transação de banco.
        // Os lotes são relidos dentro da transação para evitar race condition:
        // dois beneficiários tentando finalizar simultaneamente com o mesmo produto
        // não conseguem ambos passar pela validação antes de qualquer dedução ocorrer.
        // -------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar()
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            await LimparItensExpiradosAsync(usuarioId);

            // Carrega itens do carrinho (sem lotes — relidos dentro da transação)
            var itensNoCarrinho = await _context.CarrinhoItens
                .Include(c => c.Produto)
                    .ThenInclude(p => p.Doador)
                        .ThenInclude(d => d.Usuario)
                .Where(c => c.IdBeneficiario == usuarioId)
                .ToListAsync();

            if (!itensNoCarrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction(nameof(Index));
            }

            var hoje = DateTime.Today;
            var resumoReservas = new List<ResumoReservaViewModel>();
            PedidoConfirmadoViewModel? viewModel = null;

            // Envolve toda a operação de escrita em uma transação para garantir atomicidade.
            // Os lotes são carregados frescos dentro da transação, evitando que dois
            // usuários simultâneos passem a validação com o mesmo estoque.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var idsProdutos = itensNoCarrinho.Select(i => i.IdProduto).ToList();

                // Releitura dos lotes dentro da transação — dados sempre atualizados
                var lotesFrescos = await _context.Lotes
                    .Where(l => idsProdutos.Contains(l.IdProduto)
                             && l.StatusLote == StatusLote.Disponivel
                             && l.DataValidade.Date >= hoje
                             && l.Quantidade > 0)
                    .OrderBy(l => l.DataValidade)
                    .ToListAsync();

                // Valida e seleciona o lote FIFO para cada item dentro da transação
                var itensIndisponiveis = new List<string>();
                var lotesSelecionados = new Dictionary<int, Lote>(); // IdProduto → lote

                foreach (var item in itensNoCarrinho)
                {
                    var lote = lotesFrescos
                        .FirstOrDefault(l => l.IdProduto == item.IdProduto);

                    if (lote == null || lote.Quantidade < item.QuantidadeDesejada)
                        itensIndisponiveis.Add(item.Produto.NomeProduto);
                    else
                        lotesSelecionados[item.IdProduto] = lote;
                }

                if (itensIndisponiveis.Any())
                {
                    await transaction.RollbackAsync();
                    TempData["Erro"] = $"Os seguintes itens não estão disponíveis em quantidade suficiente: {string.Join(", ", itensIndisponiveis)}. Remova-os do carrinho para continuar.";
                    return RedirectToAction(nameof(Index));
                }

                // Cria o Pedido que agrupa as reservas
                var pedido = new Pedido
                {
                    IdBeneficiario = usuarioId,
                    DataPedido = DateTime.UtcNow,
                    StatusPedido = StatusPedido.Pendente
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync(); // Gera IdPedido para as reservas

                // Deduz estoque e cria reservas para cada item.
                // Mantemos pares (reservaObj, vmObj) para preencher o IdReserva gerado
                // pelo banco após o SaveChangesAsync (antes do save o Id ainda é 0).
                var pares = new List<(Reserva reserva, ResumoReservaViewModel vm)>();

                foreach (var item in itensNoCarrinho)
                {
                    var lote = lotesSelecionados[item.IdProduto];

                    lote.Quantidade -= item.QuantidadeDesejada;

                    // Lote esgotado → marca como inativo
                    if (lote.Quantidade == 0)
                        lote.StatusLote = StatusLote.Inativo;

                    // Se não há outro lote disponível, inativa o produto
                    var aindaTemOutrosLotes = lotesFrescos.Any(l =>
                        l.IdProduto == item.IdProduto
                        && l.IdLote != lote.IdLote
                        && l.Quantidade > 0);

                    if (!aindaTemOutrosLotes && lote.Quantidade == 0)
                        item.Produto.StatusProduto = false;

                    var reservaObj = new Reserva
                    {
                        IdLote = lote.IdLote,
                        IdBeneficiario = usuarioId,
                        IdPedido = pedido.IdPedido,
                        QuantidadeReservada = item.QuantidadeDesejada,
                        Status = StatusReserva.Pendente,
                        DataReserva = DateTime.UtcNow
                    };
                    _context.Reservas.Add(reservaObj);

                    var vmObj = new ResumoReservaViewModel
                    {
                        NomeProduto = item.Produto.NomeProduto,
                        MarcaProduto = item.Produto.MarcaProduto,
                        CategoriaProduto = item.Produto.CategoriaProduto,
                        UnidadeProduto = item.Produto.UnidadeMedida,
                        NumeroLote = lote.NumeroLote,
                        DataValidadeLote = lote.DataValidade,
                        QuantidadeDesejada = item.QuantidadeDesejada,
                        NomeDoador = item.Produto.Doador?.Usuario?.Nome ?? "Doador",
                        FotoProduto = ObterFotoProdutoDataUrl(item.Produto.FotoProduto),
                        StatusReserva = "Pendente",
                        Sucesso = true
                    };
                    resumoReservas.Add(vmObj);
                    pares.Add((reservaObj, vmObj));
                }

                // Remove todos os itens do carrinho e confirma a transação
                _context.CarrinhoItens.RemoveRange(itensNoCarrinho);
                await _context.SaveChangesAsync();

                // Notifica cada doador único sobre a nova reserva pendente
                var doadoresNotificados = new HashSet<int>();
                foreach (var item in itensNoCarrinho)
                {
                    var idDoador = item.Produto?.IdDoador ?? 0;
                    if (idDoador > 0 && doadoresNotificados.Add(idDoador))
                    {
                        await CriarNotificacaoAsync(
                            idUsuario  : idDoador,
                            titulo     : "Nova reserva pendente 🛒",
                            mensagem   : $"Você tem uma nova reserva pendente no pedido #{pedido.IdPedido}. Acesse Gerenciar Reservas para aprovar ou rejeitar.",
                            tipo       : TipoNotificacao.ReservaPendente,
                            url        : "/Produtos/GerenciarReservas");
                    }
                }
                await _context.SaveChangesAsync();

                // Após o save, os IdReserva gerados pelo banco já estão nos objetos EF
                foreach (var (r, v) in pares)
                    v.IdReserva = r.IdReserva;

                await transaction.CommitAsync();

                viewModel = new PedidoConfirmadoViewModel
                {
                    IdPedido = pedido.IdPedido,
                    DataPedido = pedido.DataPedido,
                    TotalReservas = resumoReservas.Count,
                    Reservas = resumoReservas
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Erro"] = "Ocorreu um erro ao finalizar o pedido. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }

            return View("Confirmado", viewModel);
        }

        // -------------------------------------------------------------------------
        // GET: /Carrinho/VerificarExpiracao
        // Endpoint de polling — chamado pelo JavaScript a cada 300 segundos.
        // Retorna JSON com o tempo restante do carrinho.
        // -------------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> VerificarExpiracao()
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return Json(new { expirado = true, segundosRestantes = 0 });

            await LimparItensExpiradosAsync(usuarioId);

            // Busca o item com menor expiração (o mais urgente)
            var itemMaisUrgente = await _context.CarrinhoItens
                .Where(c => c.IdBeneficiario == usuarioId)
                .OrderBy(c => c.DataExpiracao)
                .FirstOrDefaultAsync();

            if (itemMaisUrgente == null)
                return Json(new { expirado = true, segundosRestantes = 0 });

            var segundosRestantes = (int)(itemMaisUrgente.DataExpiracao - DateTime.UtcNow).TotalSeconds;
            segundosRestantes = Math.Max(0, segundosRestantes);

            return Json(new
            {
                expirado = segundosRestantes == 0,
                segundosRestantes = segundosRestantes,
                minutosRestantes = segundosRestantes / 60,
                estaExpirando = segundosRestantes <= 300
            });
        }
    }
}

/*
** POLLING:
É uma técnica onde o cliente pergunta ao servidor periodicamente se algo mudou;
Browser vai chamar uma URL do controller a cada 300 segundos via JavaScript;
Controller responde com o tempo restante em JSON;
*/
