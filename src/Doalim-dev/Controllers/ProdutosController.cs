using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class ProdutosController : Controller
    {

        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string busca, string categoria, string marca, string numeroLote, DateTime? dataInicio, DateTime? dataFim, bool apenasAlertas, string statusFiltro = "ativos")
        {
            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            var ehDoador = User.IsInRole(TipoUsuario.DoadorPF.ToString()) || User.IsInRole(TipoUsuario.DoadorPJ.ToString());
            if (!ehDoador)
            {
                TempData["ErroSeguranca"] = "Acesso negado: Apenas Doadores possuem um painel de gerenciamento.";
                return RedirectToAction("Index", "Home");
            }

            // Auto-Inativação
            var produtosDoDoador = await _context.Produtos.Include(p => p.Lotes).Where(p => p.IdDoador == usuarioId).ToListAsync();
            bool precisaSalvarDB = false;

            foreach (var p in produtosDoDoador)
            {
                bool produtoTemLoteValidoEAtivo = false;

                if (p.Lotes != null && p.Lotes.Any())
                {
                    foreach (var lote in p.Lotes)
                    {
                        if (lote.DataValidade.Date < DateTime.Today && lote.StatusLote == StatusLote.Disponivel)
                        {
                            lote.StatusLote = StatusLote.Inativo;
                            precisaSalvarDB = true;
                        }

                        if (lote.StatusLote == StatusLote.Disponivel && lote.DataValidade.Date >= DateTime.Today)
                        {
                            produtoTemLoteValidoEAtivo = true;
                        }
                    }

                    if (!produtoTemLoteValidoEAtivo && p.StatusProduto == true)
                    {
                        p.StatusProduto = false;
                        precisaSalvarDB = true;
                    }
                }
            }

            if (precisaSalvarDB) await _context.SaveChangesAsync();

            var query = _context.Produtos.Include(p => p.Lotes).Where(p => p.IdDoador == usuarioId).AsQueryable();

            // Filtros
            if (statusFiltro == "ativos") query = query.Where(p => p.StatusProduto == true);
            else if (statusFiltro == "inativos") query = query.Where(p => p.StatusProduto == false);

            if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(p => p.NomeProduto.Contains(busca));
            if (!string.IsNullOrWhiteSpace(categoria)) query = query.Where(p => p.CategoriaProduto.Contains(categoria));
            if (!string.IsNullOrWhiteSpace(marca)) query = query.Where(p => p.MarcaProduto.Contains(marca));

            if (!string.IsNullOrWhiteSpace(numeroLote)) query = query.Where(p => p.Lotes.Any(l => l.NumeroLote.Contains(numeroLote)));
            if (dataInicio.HasValue) query = query.Where(p => p.Lotes.Any(l => l.DataValidade >= dataInicio.Value));
            if (dataFim.HasValue) query = query.Where(p => p.Lotes.Any(l => l.DataValidade <= dataFim.Value));

            if (apenasAlertas)
            {
                var limiteAlerta = DateTime.Today.AddDays(1);
                query = query.Where(p => p.Lotes.Any(l => l.StatusLote == StatusLote.Disponivel && l.DataValidade.Date <= limiteAlerta));
            }

            // Ordenação: ativos para cima e inativos para baixo e depois organiza em ordem alfabética
            query = query.OrderByDescending(p => p.StatusProduto).ThenBy(p => p.NomeProduto);

            var produtos = await query.ToListAsync();

            ViewBag.Busca = busca;
            ViewBag.Categoria = categoria;
            ViewBag.Marca = marca;
            ViewBag.NumeroLote = numeroLote;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
            ViewBag.ApenasAlertas = apenasAlertas;
            ViewBag.StatusFiltro = statusFiltro;

            return View(produtos);
        }


        // GET
        public IActionResult Create()
        {
            var ehDoador = User.IsInRole(TipoUsuario.DoadorPF.ToString()) || User.IsInRole(TipoUsuario.DoadorPJ.ToString());
            if (!ehDoador)
            {
                TempData["ErroSeguranca"] = "Acesso negado: Apenas Doadores podem cadastrar produtos.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto, IFormFile arquivoFoto, string[] NumeroLote, DateTime[] DataValidadeLote, int[] QuantidadeLote, bool[] statusLoteForm)
        {

            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            produto.IdDoador = usuarioId;

            if (!string.IsNullOrWhiteSpace(produto.CodigoBarras))
            {
                bool codigoJaExiste = await _context.Produtos.AnyAsync(p => p.IdDoador == usuarioId && p.CodigoBarras == produto.CodigoBarras);
                if (codigoJaExiste)
                {
                    ModelState.AddModelError("CodigoBarras", "Você já possui um produto com este Código de Barras no seu estoque.");
                }
            }

            ModelState.Remove(nameof(Produto.IdDoador));
            ModelState.Remove(nameof(Produto.Doador));

            // Imagem byte[] = varbinary(MAX)
            if (arquivoFoto != null && arquivoFoto.Length > 0)
            {
                using var ms = new MemoryStream();
                await arquivoFoto.CopyToAsync(ms);
                produto.FotoProduto = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                bool produtoTemLoteValido = false;

                _context.Add(produto);
                await _context.SaveChangesAsync();

                if (NumeroLote != null && NumeroLote.Length > 0)
                {
                    for (int i = 0; i < NumeroLote.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(NumeroLote[i]))
                        {
                            bool ativo = (statusLoteForm != null && statusLoteForm.Length > i) ? statusLoteForm[i] : true;

                            if (DataValidadeLote[i].Date < DateTime.Today) ativo = false;

                            if (ativo) produtoTemLoteValido = true;

                            var lote = new Lote
                            {
                                NumeroLote = NumeroLote[i],
                                DataValidade = DataValidadeLote[i],
                                Quantidade = QuantidadeLote[i],
                                StatusLote = ativo ? StatusLote.Disponivel : StatusLote.Inativo,
                                IdProduto = produto.IdProduto
                            };
                            _context.Add(lote);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Regra de négocio
                if (!produtoTemLoteValido && produto.StatusProduto)
                {
                    produto.StatusProduto = false;
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }


        // Redirecionamento feito
        public IActionResult Details(int? id) => RedirectToAction(nameof(Edit), new { id = id });
        public IActionResult Delete(int? id) => RedirectToAction(nameof(Edit), new { id = id });

        [AllowAnonymous]
        public async Task<IActionResult> Vitrine(VitrineFiltroViewModel filtros)
        {
            var usuarioLogado = User.Identity?.IsAuthenticated == true;
            var usuarioBeneficiario = UsuarioEhBeneficiario();
            var usuarioAprovado = false;

            if (usuarioLogado)
            {
                var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(usuarioIdClaim, out var usuarioId))
                {
                    usuarioAprovado = await _context.Usuarios
                        .AsNoTracking()
                        .AnyAsync(u => u.IdUsuario == usuarioId && u.StatusVerificacao == StatusVerificacao.Aprovado);
                }
            }

            ViewBag.UsuarioLogado = usuarioLogado;
            ViewBag.UsuarioBeneficiario = usuarioBeneficiario;
            ViewBag.UsuarioAprovado = usuarioAprovado;
            ViewBag.PodeReservar = usuarioLogado && usuarioBeneficiario && usuarioAprovado;

            var hoje = DateTime.Today;
            var query = _context.Produtos
                .Include(p => p.Lotes)
                .Where(p => p.StatusProduto
                    && p.Lotes.Any(l => l.StatusLote == StatusLote.Disponivel && l.DataValidade.Date >= hoje && l.Quantidade > 0));

            if (!string.IsNullOrWhiteSpace(filtros.NomeBusca))
                query = query.Where(p => p.NomeProduto.Contains(filtros.NomeBusca));

            var produtos = await query.ToListAsync();
            var idsDoadores = produtos.Select(p => p.IdDoador).Distinct().ToList();
            var nomesDoadores = await _context.Usuarios
                .Where(u => idsDoadores.Contains(u.IdUsuario))
                .Select(u => new { u.IdUsuario, u.Nome })
                .ToDictionaryAsync(u => u.IdUsuario, u => u.Nome);

            var produtosViewModel = produtos
                .Select(p =>
                {
                    nomesDoadores.TryGetValue(p.IdDoador, out var nomeDoador);

                    var lotesAtivos = p.Lotes
                        .Where(l => l.StatusLote == StatusLote.Disponivel && l.DataValidade.Date >= hoje && l.Quantidade > 0)
                        .OrderBy(l => l.DataValidade)
                        .ToList();

                    return new VitrineDoacoesViewModel
                    {
                        IdProduto = p.IdProduto,
                        Nome = p.NomeProduto,
                        DataValidade = lotesAtivos.First().DataValidade,
                        Categoria = p.CategoriaProduto ?? "",
                        MarcaProduto = p.MarcaProduto ?? "",
                        TipoArmazenamento = p.TipoArmazenamento ?? "",
                        FotoProduto = ObterFotoProdutoDataUrl(p.FotoProduto),
                        QuantidadeDisponivel = lotesAtivos.Sum(l => l.Quantidade),
                        NomeDoador = nomeDoador ?? "Doador"
                    };
                })
                .Where(p => !filtros.QuantidadeMinima.HasValue || p.QuantidadeDisponivel >= filtros.QuantidadeMinima.Value);

            produtosViewModel = filtros.OrdemValidade == "desc"
                ? produtosViewModel.OrderByDescending(p => p.DataValidade)
                : produtosViewModel.OrderBy(p => p.DataValidade);

            var viewModel = new VitrineCompletaViewModel
            {
                Filtros = filtros,
                Produtos = produtosViewModel.ToList()
            };

            return View(viewModel);
        }

        // GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            int usuarioId = ObterIdUsuarioLogado();

            var produto = await _context.Produtos.Include(p => p.Lotes).FirstOrDefaultAsync(p => p.IdProduto == id);
            if (produto == null) return NotFound();

            if (produto.IdDoador != usuarioId)
            {
                TempData["ErroSeguranca"] = "Acesso Negado: Este produto não pertence à sua conta.";
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto, IFormFile arquivoFoto,
            int[] IdLote, string[] NumeroLote, DateTime[] DataValidade, int[] Quantidade, bool[] statusLoteForm, int[] LotesExcluidos)
        {
            if (id != produto.IdProduto) return NotFound();

            int usuarioId = ObterIdUsuarioLogado();

            var produtoOriginal = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProduto == id);
            if (produtoOriginal == null || produtoOriginal.IdDoador != usuarioId)
            {
                TempData["ErroSeguranca"] = "Tentativa de Fraude identificada. Operação cancelada.";
                return RedirectToAction(nameof(Index));
            }

            produto.IdDoador = usuarioId;

            ModelState.Remove(nameof(Produto.Doador));
            ModelState.Remove(nameof(Produto.IdDoador));


            if (!string.IsNullOrWhiteSpace(produto.CodigoBarras))
            {
                bool codigoJaExiste = await _context.Produtos.AnyAsync(p => p.IdDoador == usuarioId && p.CodigoBarras == produto.CodigoBarras && p.IdProduto != id);
                if (codigoJaExiste) ModelState.AddModelError("CodigoBarras", "Você já possui OUTRO produto usando este mesmo Código.");
            }

            var idsLotesRecebidos = (IdLote ?? Array.Empty<int>())
                .Concat(LotesExcluidos ?? Array.Empty<int>())
                .Where(loteId => loteId > 0)
                .Distinct()
                .ToList();

            if (idsLotesRecebidos.Any())
            {
                var lotesDoProduto = await _context.Lotes
                    .CountAsync(l => idsLotesRecebidos.Contains(l.IdLote) && l.IdProduto == id);

                if (lotesDoProduto != idsLotesRecebidos.Count)
                {
                    TempData["ErroSeguranca"] = "Tentativa de alterar lote de outro produto identificada. Operação cancelada.";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (arquivoFoto != null && arquivoFoto.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await arquivoFoto.CopyToAsync(ms);
                        produto.FotoProduto = ms.ToArray();
                    }
                    else
                    {
                        produto.FotoProduto = produtoOriginal.FotoProduto;
                    }

                    _context.Update(produto);
                    await _context.SaveChangesAsync();


                    if (LotesExcluidos != null && LotesExcluidos.Length > 0)
                    {
                        foreach (var loteId in LotesExcluidos)
                        {
                            var lRemover = await _context.Lotes.FirstOrDefaultAsync(l => l.IdLote == loteId && l.IdProduto == produto.IdProduto);
                            if (lRemover != null) _context.Lotes.Remove(lRemover);
                        }
                    }

                    bool produtoTemLoteValido = false;


                    if (NumeroLote != null)
                    {
                        for (int i = 0; i < NumeroLote.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(NumeroLote[i])) continue;

                            bool ativo = (statusLoteForm != null && statusLoteForm.Length > i) ? statusLoteForm[i] : true;
                            if (DataValidade[i].Date < DateTime.Today) ativo = false;

                            if (ativo) produtoTemLoteValido = true;

                            if (IdLote != null && i < IdLote.Length && IdLote[i] > 0)
                            {
                                var loteExist = await _context.Lotes.FirstOrDefaultAsync(l => l.IdLote == IdLote[i] && l.IdProduto == produto.IdProduto);
                                if (loteExist != null)
                                {
                                    loteExist.NumeroLote = NumeroLote[i];
                                    loteExist.DataValidade = DataValidade[i];
                                    loteExist.Quantidade = Quantidade[i];
                                    loteExist.StatusLote = ativo ? StatusLote.Disponivel : StatusLote.Inativo;
                                    _context.Update(loteExist);
                                }
                            }

                            else
                            {
                                var novoLote = new Lote
                                {
                                    NumeroLote = NumeroLote[i],
                                    DataValidade = DataValidade[i],
                                    Quantidade = Quantidade[i],
                                    StatusLote = ativo ? StatusLote.Disponivel : StatusLote.Inativo,
                                    IdProduto = produto.IdProduto
                                };
                                _context.Add(novoLote);
                            }
                        }
                    }

                    if (!produtoTemLoteValido && produto.StatusProduto)
                    {
                        produto.StatusProduto = false;
                        _context.Update(produto);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.IdProduto)) return NotFound();
                    throw;
                }
            }

            produto.Lotes = await _context.Lotes.Where(l => l.IdProduto == id).ToListAsync();
            return View(produto);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.Include(p => p.Lotes).FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto != null && produto.IdDoador == ObterIdUsuarioLogado())
            {

                if (produto.Lotes != null && produto.Lotes.Any()) _context.Lotes.RemoveRange(produto.Lotes);

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id) => _context.Produtos.Any(e => e.IdProduto == id);

        private bool UsuarioEhBeneficiario()
        {
            return User.IsInRole(TipoUsuario.BeneficiarioPF.ToString())
                || User.IsInRole(TipoUsuario.BeneficiarioPJ.ToString());
        }

        private static string ObterFotoProdutoDataUrl(byte[]? fotoProduto)
        {
            if (fotoProduto == null || fotoProduto.Length == 0)
                return string.Empty;

            var mimeType = "image/jpeg";

            if (fotoProduto.Length >= 8
                && fotoProduto[0] == 0x89
                && fotoProduto[1] == 0x50
                && fotoProduto[2] == 0x4E
                && fotoProduto[3] == 0x47)
            {
                mimeType = "image/png";
            }
            else if (fotoProduto.Length >= 4
                && fotoProduto[0] == 0x3C
                && (fotoProduto[1] == 0x73 || fotoProduto[1] == 0x53 || fotoProduto[1] == 0x3F))
            {
                mimeType = "image/svg+xml";
            }

            return $"data:{mimeType};base64,{Convert.ToBase64String(fotoProduto)}";
        }

        // =======================================================================================
        // GET: /Produtos/GerenciarReservas
        // Exibe todas as reservas pendentes e confirmadas dos produtos do doador logado.
        // =======================================================================================
        [Authorize]
        public async Task<IActionResult> GerenciarReservas()
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            var reservas = await _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                        .ThenInclude(p => p.Doador)
                .Include(r => r.Beneficiario)
                    .ThenInclude(b => b.Usuario)
                .Where(r => r.Lote.Produto.IdDoador == usuarioId
                            && (r.Status == StatusReserva.Pendente
                                   || r.Status == StatusReserva.Confirmada))
                .OrderByDescending(r => r.DataReserva)
                .Select(r => new GerenciarReservaDoadorViewModel
                {
                    IdReserva = r.IdReserva,
                    IdPedido = r.IdPedido ?? 0,
                    DataReserva = r.DataReserva,
                    StatusReserva = r.Status.ToString(),
                    QuantidadeReservada = r.QuantidadeReservada,
                    NumeroLote = r.Lote.NumeroLote,
                    DataValidadeLote = r.Lote.DataValidade,
                    IdProduto = r.Lote.Produto.IdProduto,
                    NomeProduto = r.Lote.Produto.NomeProduto,
                    MarcaProduto = r.Lote.Produto.MarcaProduto,
                    CategoriaProduto = r.Lote.Produto.CategoriaProduto,
                    UnidadeMedidaProduto = r.Lote.Produto.UnidadeMedida,
                    FotoProduto = r.Lote.Produto.FotoProduto == null
                        ? null
                        : $"data:image/jpeg;base64,{Convert.ToBase64String(r.Lote.Produto.FotoProduto)}",
                    NomeBeneficiario = r.Beneficiario.Usuario.Nome,
                    TelefoneBeneficiario = r.Beneficiario.Usuario.Telefone,
                    EhOng = r.Beneficiario.Eong,
                    DataRetiradaInicio = r.DataRetiradaInicio,
                    DataRetiradaFim = r.DataRetiradaFim,
                    TokenConfirmacao = r.TokenConfirmacao
                })
                .ToListAsync();

            return View(reservas);
        }

        // =========================================================================================
        // POST: /Produtos/AprovarReserva
        // Doador aprova a reserva, define o intervalo de retirada e gera o token de confirmação.
        // =========================================================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarReserva(AprovarReservaViewModel viewModel)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Informe o intervalo de retirada corretamente.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            var reserva = await _context.Reservas
                   .Include(r => r.Lote)
                       .ThenInclude(l => l.Produto)
                   .FirstOrDefaultAsync(r => r.IdReserva == viewModel.IdReserva
                                          && r.Lote.Produto.IdDoador == usuarioId);

            if (reserva == null)
                return NotFound();

            // Valida se status da resevra é diferente de pendente
            if (reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Esta reserva não pode ser aprovada.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            // Valida se o intervalo de retirada é anterior ao vencimento do lote
            if (viewModel.DataRetiradaFim.Date > reserva.Lote.DataValidade.Date)
            {
                TempData["Erro"] = $"A data fim de retirada deve ser anterior ao " +
                                   $"vencimento do lote ({reserva.Lote.DataValidade:dd/MM/yyyy}).";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            // Valida que a data início é anterior ou igual à data fim
            if (viewModel.DataRetiradaInicio.Date > viewModel.DataRetiradaFim.Date)
            {
                TempData["Erro"] = "A data de início deve ser anterior à data fim.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            // Gera o token de confirmação — 8 caracteres alfanuméricos em maiúsculo
            reserva.TokenConfirmacao = Guid.NewGuid().ToString("N")[..8].ToUpper();
            reserva.DataRetiradaInicio = viewModel.DataRetiradaInicio;
            reserva.DataRetiradaFim = viewModel.DataRetiradaFim;
            reserva.Status = StatusReserva.Confirmada;

            // Marca o lote como Reservado — sai da vitrine
            reserva.Lote.StatusLote = StatusLote.Reservado;

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(reserva.IdPedido);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Reserva #{reserva.IdReserva} aprovada com sucesso!";
            return RedirectToAction(nameof(GerenciarReservas));
        }

        // =====================================================================
        // POST: /Produtos/RejeitarReserva
        // Doador rejeita a reserva. O lote volta para a vitrine
        // se ainda estiver dentro da validade.
        // =====================================================================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarReserva(int idReserva)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            var reserva = await _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva
                                       && r.Lote.Produto.IdDoador == usuarioId);

            if (reserva == null)
                return NotFound();

            if (reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Esta reserva não pode ser rejeitada.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            // Rejeita de fato a reserva
            reserva.Status = StatusReserva.Rejeitada;
            reserva.DataEncerramento = DateTime.UtcNow;

            // Devolve o lote para a vitrine se ainda estiver dentro da validade
            if (reserva.Lote.DataValidade.Date >= DateTime.Today)
            {
                reserva.Lote.Quantidade += reserva.QuantidadeReservada;
                reserva.Lote.StatusLote = StatusLote.Disponivel;
                reserva.Lote.Produto.StatusProduto = true;
            }

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(reserva.IdPedido);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Reserva #{reserva.IdReserva} rejeitada.";
            return RedirectToAction(nameof(GerenciarReservas));
        }

        // =====================================================================
        // POST: /Produtos/ConfirmarEntrega
        // Doador insere o token informado pelo beneficiário.
        // Se válido, marca a reserva como Entregue e o lote como Entregue.
        // =====================================================================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEntrega(int idReserva, string tokenInformado)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            var reserva = await _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva
                                       && r.Lote.Produto.IdDoador == usuarioId);

            if (reserva == null)
                return NotFound();

            // Valida se o status da reserva encontra-se como confirmada
            if (reserva.Status != StatusReserva.Confirmada)
            {
                TempData["Erro"] = "Esta reserva não pode ser confirmada como entregue.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            // Valida o token — comparação case-insensitive
            if (!string.Equals(reserva.TokenConfirmacao, tokenInformado?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["Erro"] = "Token inválido. Verifique o código informado pelo beneficiário.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            // Marca a reserva como retirada
            reserva.Status = StatusReserva.Retirada;
            reserva.DataEncerramento = DateTime.UtcNow;

            // Marca o lote como entregue — sai da vitrine e vai para o histórico
            reserva.Lote.StatusLote = StatusLote.Entregue;

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(reserva.IdPedido);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Entrega da reserva #{reserva.IdReserva} confirmada com sucesso!";
            return RedirectToAction(nameof(GerenciarReservas));
        }


        // =====================================================================
        // MÉTODO AUXILIAR
        // Lê o ID do usuário logado a partir dos Claims.
        // =====================================================================

        private int ObterIdUsuarioLogado()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(claim, out var id) ? id : 0;
        }

        // =====================================================================
        // MÉTODO AUXILIAR
        // Recalcula e atualiza o status do Pedido com base
        // no estado atual de todas as suas reservas filhas.
        // =====================================================================

        private async Task AtualizarStatusPedidoAsync(int? idPedido)
        {
            if (idPedido == null) return;

            var pedido = await _context.Pedidos
                .Include(p => p.Reservas)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

            if (pedido == null) return;

            var statusReservas = pedido.Reservas.Select(r => r.Status).ToList();

            pedido.StatusPedido = statusReservas.All(s => s == StatusReserva.Retirada)
                      ? StatusPedido.Retirado
                      : statusReservas.All(s => s == StatusReserva.Cancelada || s == StatusReserva.Rejeitada)
                                 ? StatusPedido.Cancelado
                                 : statusReservas.Any(s => s == StatusReserva.Confirmada)
                                            ? StatusPedido.Confirmado
                                            : StatusPedido.Pendente;
        }

    }
}
