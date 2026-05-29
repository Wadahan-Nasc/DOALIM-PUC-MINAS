using System.Security.Claims;
using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class CarrinhoController : Controller
    {
        private readonly AppDbContext _context;

        private const int DuracaoCarrinhoMinutos = 30;

        public CarrinhoController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------------------
        // LIMPEZA DE ITENS EXPIRADOS
        // Chamado no início de toda action para garantir que itens
        // expirados não interfiram nas operações do carrinho.
        // -------------------------------------------------------------------------

        private async Task LimparItensExpiradosAsync(int IdBeneficiario)
        {
            var itensExpirados = await _context.CarrinhoItens
                .Where(c => c.IdBeneficiario == IdBeneficiario
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
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            await LimparItensExpiradosAsync(usuarioId.Value);

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
                    FotoProduto = c.Produto.FotoProduto == null ?
                                    "" : $"data:image/jpeg;base64,{Convert.ToBase64String(c.Produto.FotoProduto)}",
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
        // -------------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(int idProduto, int quantidade)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            await LimparItensExpiradosAsync(usuarioId.Value);

            // Verifica se o produto existe e está disponível
            var produto = await _context.Produtos
                .Include(p=>p.Lotes)
                .FirstOrDefaultAsync(p => p.IdProduto == idProduto
                                            && p.StatusProduto);
            // Trata o caso de produto inexistente ou indisponível
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado ou indisponível.";
                return RedirectToAction("Vitrine", "Produtos");
            }
            
            // Verifica se o produto já está no carrinho
            var itemExistente = await _context.CarrinhoItens
                .FirstOrDefaultAsync(c => c.IdBeneficiario == usuarioId
                                        && c.IdProduto == idProduto);

            // Se true, atualiza a quantidade do item existente
            if (itemExistente != null)
            {
                itemExistente.QuantidadeDesejada = quantidade;
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Quantidade atualizada no carrinho.";
                return RedirectToAction(nameof(Index));
            }

            // Verifica o limite de 15 produtos distintos no carrinho
            // Como todos os itens do carrinho de um usuário são agrupados por IdBeneficiario,
            // basta contar quantos IdBeneficiario são iguais ao do beneficiário logado;
            var totalItens = await _context.CarrinhoItens
                .CountAsync(c => c.IdBeneficiario == usuarioId);

            if(totalItens >= 15)
            {
                TempData["Erro"] = "Limite de 15 produtos distintos no carrinho atingido.";
                return RedirectToAction(nameof(Index));
            }

            // Determina a expiração - herda a do carrinho existente ou cria nova
            var itemExistenteQualquer = await _context.CarrinhoItens
                .Where(c => c.IdBeneficiario == usuarioId)
                .OrderBy(c => c.DataExpiracao) // Ordena por expiração
                .FirstOrDefaultAsync(c => c.IdBeneficiario == usuarioId); // Busca o primeiro item (com a data de expiração mais próxima)

            // Variável de controle do tempo de expiração
            var expiracao = itemExistenteQualquer != null? 
                itemExistenteQualquer.DataExpiracao    // Se já há itens no carrinho, herda a expiração mais antiga
                : DateTime.UtcNow.AddMinutes(DuracaoCarrinhoMinutos);   // Se é o primeiro item, cria nova expiração de 30 minutos

            // Cria um novo item no carrinho
            var novoItem = new CarrinhoItem
            {
                IdBeneficiario = usuarioId.Value,
                IdProduto = idProduto,
                QuantidadeDesejada = quantidade,
                DataExpiracao = expiracao,
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
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
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
        // Tenta reservar todos os itens do carrinho.
        // Bloqueia se houver itens indisponíveis.
        // -------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar ()
        {
            var usuarioId = ObterUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            await LimparItensExpiradosAsync(usuarioId.Value);

            var itensNoCarrinho = await _context.CarrinhoItens
                .Include(c => c.Produto)
                    .ThenInclude(p => p.Lotes)
                .Where(c => c.IdBeneficiario == usuarioId.Value)
                .ToListAsync();

            if (!itensNoCarrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction(nameof(Index));
            }

            var hoje = DateTime.Today;
            var resumoReservas = new List<ResumoReservaViewModel>();
            var itensIndisponiveis = new List<string>();

            // Valida disponibilidade de todos os itens antes de reservar qualquer um
            foreach (var item in itensNoCarrinho)
            {
                var loteMaisUrgente = item.Produto.Lotes
                    .Where(l => l.StatusLote == StatusLote.Disponivel
                             && l.DataValidade.Date >= hoje
                             && l.Quantidade > 0)
                    .OrderBy(l => l.DataValidade)
                    .FirstOrDefault();

                if (loteMaisUrgente == null || loteMaisUrgente.Quantidade < item.QuantidadeDesejada)
                {
                    itensIndisponiveis.Add(item.Produto.NomeProduto);
                }
            }

            // Caso houver itens indisponíveis, bloqueia a finalização e informa o usuário
            if (itensIndisponiveis.Any())
            {
                TempData["Erro"] = $"Os seguintes itens não estão disponíveis em quantidade suficiente: {string.Join(", ", itensIndisponiveis)}. Remova-os do carrinho para continuar.";
                return RedirectToAction(nameof(Index));
            }

            // Cria o Pedido que agrupa as reservas
            var pedido = new Pedido
            {
                IdBeneficiario = usuarioId.Value,
                DataPedido = DateTime.UtcNow,
                StatusPedido = StatusPedido.Pendente
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync(); // Salva para gerar o IdPedido necessário para as reservas

            // Cria reservas individualmente para cada item, atualiza o estoque do lote e remove o item do carrinho
            foreach (var item in itensNoCarrinho)
            {
                var loteMaisUrgente = item.Produto.Lotes
                    .Where(l => l.StatusLote == StatusLote.Disponivel
                             && l.DataValidade.Date >= hoje
                             && l.Quantidade > 0)
                    .OrderBy(l => l.DataValidade)
                    .FirstOrDefault()!;

                //Deduz quantidade do lote
                loteMaisUrgente.Quantidade -= item.QuantidadeDesejada;

                // Se o lote esgotar, marca como inativo
                if (loteMaisUrgente.Quantidade == 0)
                    loteMaisUrgente.StatusLote = StatusLote.Inativo;

                // Verifica se o produto ainda tem lotes ativos
                var aindaTemLotes = item.Produto.Lotes
                    .Any(l => l.StatusLote == StatusLote.Disponivel
                           && l.DataValidade.Date >= hoje
                           && l.Quantidade > 0
                           && l.IdLote != loteMaisUrgente.IdLote);

                // Caso não, marca o produto como indisponível
                if (!aindaTemLotes && loteMaisUrgente.Quantidade == 0)
                    item.Produto.StatusProduto = false;

                // Cria a reserva para o item
                var reserva = new Reserva
                {
                    IdLote = loteMaisUrgente.IdLote,
                    IdBeneficiario = usuarioId.Value,
                    IdPedido = pedido.IdPedido,
                    QuantidadeReservada = item.QuantidadeDesejada,
                    Status = StatusReserva.Pendente,
                    DataReserva = DateTime.UtcNow
                };

                _context.Reservas.Add(reserva);

                resumoReservas.Add(new ResumoReservaViewModel
                {
                    NomeProduto = item.Produto.NomeProduto,
                    MarcaProduto = item.Produto.MarcaProduto,
                    CategoriaProduto = item.Produto.CategoriaProduto,
                    UnidadeProduto = item.Produto.UnidadeMedida,
                    NumeroLote = loteMaisUrgente.NumeroLote,
                    DataValidadeLote = loteMaisUrgente.DataValidade,
                    QuantidadeDesejada = item.QuantidadeDesejada,
                    StatusReserva = "Pendente",
                    Sucesso = true
                });
            }

            // Remove todos os itens do carrinho do usuário após criar as reservas
            _context.CarrinhoItens.RemoveRange(itensNoCarrinho);
            await _context.SaveChangesAsync();

            // Prepara o ViewModel para a página de confirmação do pedido
            var viewModel = new PedidoConfirmadoViewModel
            {
                IdPedido = pedido.IdPedido,
                DataPedido = pedido.DataPedido,
                TotalReservas = resumoReservas.Count,
                Reservas = resumoReservas
            };

            return View("Confirmado", viewModel);

        }

        // -------------------------------------------------------------------------
        // GET: /Carrinho/VerificarExpiracao
        // É O ENDPOINT DE POLLING** — chamado pelo JavaScript a cada 300 segundos.
        // Retorna JSON com o tempo restante do carrinho.
        //
        // Como funciona o polling:
        // 1. A View do carrinho carrega com um timer em JavaScript
        // 2. A cada 300 segundos, o JS faz uma requisição GET para esta URL
        // 3. O controller consulta o banco e retorna o tempo restante em JSON
        // 4. O JS atualiza o contador na tela e exibe alertas se necessário
        // 5. Se expirado, o JS avisa o usuário e redireciona para a vitrine
        // -------------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> VerificarExpiracao()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                // Retorna JSON de erro — o JS tratará o redirecionamento
                return Json(new { expirado = true, segundosRestantes = 0 });

            await LimparItensExpiradosAsync(usuarioId.Value);

            // Busca o item com menor expiração (o mais urgente)
            var itemMaisUrgente = await _context.CarrinhoItens
                .Where(c => c.IdBeneficiario == usuarioId)
                .OrderBy(c => c.DataExpiracao)
                .FirstOrDefaultAsync();

            // Carrinho vazio ou expirado
            if (itemMaisUrgente == null)
                return Json(new { expirado = true, segundosRestantes = 0 });

            var segundosRestantes = (int)(itemMaisUrgente.DataExpiracao - DateTime.UtcNow).TotalSeconds;
            segundosRestantes = Math.Max(0, segundosRestantes);

            // Retorna JSON com informações de expiração para o JavaScript processar
            return Json(new
            {
                expirado = segundosRestantes == 0,
                segundosRestantes = segundosRestantes,
                minutosRestantes = segundosRestantes / 60,
                estaExpirando = segundosRestantes <= 300 // aviso quando <= 5 minutos
            });
        }


        // -------------------------------------------------------------------------
        // MÉTODO AUXILIAR: Obter ID do usuário logado
        // Lê o ID do usuário logado a partir dos Claims da autenticação.
        // Retorna null se não estiver autenticado.
        // -------------------------------------------------------------------------

        private int? ObterUsuarioId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}

/*
** POLLING:
É uma técnica onde o cliente pergunta ao servidor periodicamente se algo mudou;
Browser vai chamar uma URL do controller a cada 300 segundos via JavaScript;
Controller responde com o tempo restante em JSON;
*/
