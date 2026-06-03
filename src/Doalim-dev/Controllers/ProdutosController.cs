using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class ProdutosController : BaseController
    {
        public ProdutosController(AppDbContext context) : base(context) { }

        public async Task<IActionResult> Index(string busca, string categoria, string marca, string numeroLote, DateTime? dataInicio, DateTime? dataFim, bool apenasAlertas, string statusFiltro = "ativos")
        {
            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            if (!await UsuarioPodeDoarAsync(usuarioId))
            {
                TempData["Erro"] = "Para doar, envie o arquivo de comprovação no seu perfil e aguarde a aprovação do administrador.";
                return RedirectToAction("MeuPerfil", "Usuarios");
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

            // Categorias disponíveis para o filtro — carregadas do domínio extensível
            ViewBag.CategoriasDisponiveis = await ObterCategoriasVitrineAsync();

            return View(produtos);
        }


        // GET
        public async Task<IActionResult> Create()
        {
            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            if (!await UsuarioPodeDoarAsync(usuarioId))
            {
                TempData["Erro"] = "Para cadastrar doações, envie o arquivo de comprovação no seu perfil e aguarde a aprovação do administrador.";
                return RedirectToAction("MeuPerfil", "Usuarios");
            }
            await CarregarLookupsAsync();
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto, IFormFile arquivoFoto, string[] NumeroLote, DateTime[] DataValidadeLote, int[] QuantidadeLote, bool[] statusLoteForm)
        {

            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            if (!await UsuarioPodeDoarAsync(usuarioId))
            {
                TempData["Erro"] = "Para doar, envie o arquivo de comprovação no seu perfil e aguarde a aprovação do administrador.";
                return RedirectToAction("MeuPerfil", "Usuarios");
            }

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

                // Regra de negócio
                if (!produtoTemLoteValido && produto.StatusProduto)
                {
                    produto.StatusProduto = false;
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            await CarregarLookupsAsync();
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
            int usuarioIdLogado = 0;

            if (usuarioLogado)
            {
                var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(usuarioIdClaim, out usuarioIdLogado))
                    usuarioAprovado = await UsuarioComprovadoAsync(usuarioIdLogado);
            }

            // Cidade do beneficiário logado (para filtro padrão por localidade)
            string? cidadeBeneficiario = null;
            if (usuarioBeneficiario && usuarioIdLogado > 0)
            {
                cidadeBeneficiario = await _context.Enderecos
                    .Where(e => e.IdUsuario == usuarioIdLogado)
                    .Select(e => e.Cidade)
                    .FirstOrDefaultAsync();
            }

            ViewBag.UsuarioLogado       = usuarioLogado;
            ViewBag.UsuarioBeneficiario = usuarioBeneficiario;
            ViewBag.UsuarioAprovado     = usuarioAprovado;
            ViewBag.PodeReservar        = usuarioLogado && usuarioBeneficiario && usuarioAprovado;
            ViewBag.EhBeneficiarioPF    = User.IsInRole("BeneficiarioPF");
            ViewBag.CidadeBeneficiario  = cidadeBeneficiario;

            var hoje = DateTime.Today;
            var query = _context.Produtos
                .Include(p => p.Lotes)
                .Where(p => p.StatusProduto
                    && p.Lotes.Any(l => l.StatusLote == StatusLote.Disponivel && l.DataValidade.Date >= hoje && l.Quantidade > 0));

            if (!string.IsNullOrWhiteSpace(filtros.NomeBusca))
                query = query.Where(p => p.NomeProduto.Contains(filtros.NomeBusca));

            if (!string.IsNullOrWhiteSpace(filtros.Categoria))
                query = query.Where(p => p.CategoriaProduto == filtros.Categoria);

            // Filtro de localidade — aplicado apenas para beneficiários
            if (usuarioBeneficiario)
            {
                if (filtros.FiltrarPorCidade && !string.IsNullOrWhiteSpace(cidadeBeneficiario))
                {
                    // Filtra pelos doadores da mesma cidade do beneficiário
                    var idsDoadoresCidade = await _context.Enderecos
                        .Where(e => e.Cidade == cidadeBeneficiario)
                        .Select(e => e.IdUsuario)
                        .ToListAsync();
                    query = query.Where(p => idsDoadoresCidade.Contains(p.IdDoador));
                }
                else if (!filtros.FiltrarPorCidade && !string.IsNullOrWhiteSpace(filtros.Cidade))
                {
                    // Beneficiário digitou uma cidade específica
                    var idsDoadoresCidade = await _context.Enderecos
                        .Where(e => e.Cidade.Contains(filtros.Cidade))
                        .Select(e => e.IdUsuario)
                        .ToListAsync();
                    query = query.Where(p => idsDoadoresCidade.Contains(p.IdDoador));
                }

                if (!string.IsNullOrWhiteSpace(filtros.Bairro))
                {
                    var idsDoadoresBairro = await _context.Enderecos
                        .Where(e => e.Bairro.Contains(filtros.Bairro))
                        .Select(e => e.IdUsuario)
                        .ToListAsync();
                    query = query.Where(p => idsDoadoresBairro.Contains(p.IdDoador));
                }
            }

            // Categorias disponíveis para o select do filtro
            ViewBag.CategoriasVitrine = await ObterCategoriasVitrineAsync();

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
                        NomeDoador = nomeDoador ?? "Doador",
                        LimitePF = p.QuantidadePessoaFisica,
                        LimitePJ = p.QuantidadePessoaJuridica
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

            if (!await UsuarioPodeDoarAsync(usuarioId))
            {
                TempData["Erro"] = "Para gerenciar doações, envie o arquivo de comprovação no seu perfil e aguarde a aprovação do administrador.";
                return RedirectToAction("MeuPerfil", "Usuarios");
            }

            if (produto.IdDoador != usuarioId)
            {
                TempData["Erro"] = "Acesso Negado: Este produto não pertence à sua conta.";
                return RedirectToAction(nameof(Index));
            }

            await CarregarLookupsAsync();
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

            if (!await UsuarioPodeDoarAsync(usuarioId))
            {
                TempData["Erro"] = "Para alterar doações, envie o arquivo de comprovação no seu perfil e aguarde a aprovação do administrador.";
                return RedirectToAction("MeuPerfil", "Usuarios");
            }

            var produtoOriginal = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProduto == id);
            if (produtoOriginal == null || produtoOriginal.IdDoador != usuarioId)
            {
                TempData["Erro"] = "Tentativa de Fraude identificada. Operação cancelada.";
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
                    TempData["Erro"] = "Tentativa de alterar lote de outro produto identificada. Operação cancelada.";
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
            await CarregarLookupsAsync();
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

        // =======================================================================================
        // GET: /Produtos/HistoricoDoacoes
        // Relatório de doações concluídas (Retirada) e rejeitadas do doador logado.
        // =======================================================================================
        [Authorize(Roles = "DoadorPF,DoadorPJ")]
        public async Task<IActionResult> HistoricoDoacoes(HistoricoDoadorFiltroViewModel filtros)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            var query = _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                .Include(r => r.Beneficiario)
                    .ThenInclude(b => b.Usuario)
                .Where(r => r.Lote.Produto.IdDoador == usuarioId
                         && (r.Status == StatusReserva.Retirada || r.Status == StatusReserva.Rejeitada))
                .AsQueryable();

            // Filtro por status
            if (!string.IsNullOrWhiteSpace(filtros.Status) &&
                Enum.TryParse<StatusReserva>(filtros.Status, out var statusEnum))
                query = query.Where(r => r.Status == statusEnum);

            // Filtro por categoria
            if (!string.IsNullOrWhiteSpace(filtros.Categoria))
                query = query.Where(r => r.Lote.Produto.CategoriaProduto == filtros.Categoria);

            // Filtro por nome do produto
            if (!string.IsNullOrWhiteSpace(filtros.NomeProduto))
                query = query.Where(r => r.Lote.Produto.NomeProduto.Contains(filtros.NomeProduto));

            // Filtro por nome do beneficiário
            if (!string.IsNullOrWhiteSpace(filtros.NomeBeneficiario))
                query = query.Where(r => r.Beneficiario.Usuario.Nome.Contains(filtros.NomeBeneficiario));

            // Filtro por validade do lote
            if (filtros.ValidadeInicio.HasValue)
            {
                var inicio = filtros.ValidadeInicio.Value.Date;
                query = query.Where(r => r.Lote.DataValidade >= inicio);
            }
            if (filtros.ValidadeFim.HasValue)
            {
                var fim = filtros.ValidadeFim.Value.Date.AddDays(1);
                query = query.Where(r => r.Lote.DataValidade < fim);
            }

            // Filtro por data de reserva
            if (filtros.DataReservaInicio.HasValue)
            {
                var inicio = filtros.DataReservaInicio.Value.Date;
                query = query.Where(r => r.DataReserva >= inicio);
            }
            if (filtros.DataReservaFim.HasValue)
            {
                var fim = filtros.DataReservaFim.Value.Date.AddDays(1);
                query = query.Where(r => r.DataReserva < fim);
            }

            // Categorias disponíveis para o select do filtro
            ViewBag.CategoriasDisponiveis = await ObterCategoriasVitrineAsync();

            // Carrega para memória antes de projetar (FotoProduto usa Convert.ToBase64String)
            var reservasDb = await query
                .OrderByDescending(r => r.DataEncerramento ?? r.DataReserva)
                .ToListAsync();

            var itens = reservasDb.Select(r => new HistoricoDoadorViewModel
            {
                IdReserva = r.IdReserva,
                IdPedido = r.IdPedido ?? 0,
                NomeProduto = r.Lote.Produto.NomeProduto,
                MarcaProduto = r.Lote.Produto.MarcaProduto ?? "",
                CategoriaProduto = r.Lote.Produto.CategoriaProduto ?? "",
                UnidadeMedidaProduto = r.Lote.Produto.UnidadeMedida ?? "",
                FotoProduto = ObterFotoProdutoDataUrl(r.Lote.Produto.FotoProduto),
                NumeroLote = r.Lote.NumeroLote,
                DataValidadeLote = r.Lote.DataValidade,
                QuantidadeReservada = r.QuantidadeReservada,
                StatusReserva = r.Status.ToString(),
                DataReserva = r.DataReserva,
                DataEncerramento = r.DataEncerramento,
                MotivoRejeicao = r.MotivoRejeicao,
                NomeBeneficiario = r.Beneficiario.Usuario.Nome,
                EhOng = r.Beneficiario.Eong
            }).ToList();

            var viewModel = new HistoricoDoadorPageViewModel
            {
                Filtros = filtros,
                Itens = itens
            };

            return View(viewModel);
        }

        /// <summary>
        /// Carrega os valores ativos de domínio (Categoria, TipoArmazenamento, UnidadeMedida)
        /// e os disponibiliza via ViewBag para os formulários de Create e Edit.
        /// </summary>
        private async Task CarregarLookupsAsync()
        {
            var todos = await ObterLookupsSegurosAsync();

            if (todos.Any())
            {
                ViewBag.Categorias = todos.Where(v => v.Tipo == TipoLookup.Categoria).Select(v => v.Nome).ToList();
                ViewBag.TiposArmazenamento = todos.Where(v => v.Tipo == TipoLookup.TipoArmazenamento).Select(v => v.Nome).ToList();
                ViewBag.UnidadesMedida = todos.Where(v => v.Tipo == TipoLookup.UnidadeMedida).Select(v => v.Nome).ToList();
                return;
            }

            ViewBag.Categorias = await ObterCategoriasVitrineAsync();
            ViewBag.TiposArmazenamento = new List<string> { "Ambiente", "Congelado", "Local fechado" };
            ViewBag.UnidadesMedida = new List<string> { "Kg", "mg", "L", "ml" };
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

        private async Task<List<string>> ObterCategoriasVitrineAsync()
        {
            try
            {
                return await _context.ValoresLookup
                    .Where(v => v.Tipo == TipoLookup.Categoria && v.Ativo)
                    .OrderBy(v => v.Nome)
                    .Select(v => v.Nome)
                    .ToListAsync();
            }
            catch (SqlException ex) when (ex.Number == 208)
            {
                return await _context.Produtos
                    .Where(p => p.CategoriaProduto != null && p.CategoriaProduto != "")
                    .Select(p => p.CategoriaProduto)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
        }

        private async Task<List<ValorLookup>> ObterLookupsSegurosAsync()
        {
            try
            {
                return await _context.ValoresLookup
                    .Where(v => v.Ativo)
                    .OrderBy(v => v.Nome)
                    .ToListAsync();
            }
            catch (SqlException ex) when (ex.Number == 208)
            {
                return new List<ValorLookup>();
            }
        }

        // =======================================================================================
        // GET: /Produtos/GerenciarReservas
        // Exibe reservas do doador logado com filtros opcionais.
        // =======================================================================================
        [Authorize(Roles = "DoadorPF,DoadorPJ")]
        public async Task<IActionResult> GerenciarReservas(
            string? nomeBeneficiario = null,
            DateTime? dataInicio     = null,
            DateTime? dataFim        = null,
            string?   status         = null)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            // Query base — carrega todos os status relevantes de uma só vez
            var queryBase = _context.Reservas
                .Include(r => r.Lote).ThenInclude(l => l.Produto)
                .Include(r => r.Beneficiario).ThenInclude(b => b.Usuario)
                .Where(r => r.Lote.Produto.IdDoador == usuarioId
                         && (r.Status == StatusReserva.Pendente
                          || r.Status == StatusReserva.Confirmada
                          || r.Status == StatusReserva.Retirada));

            // Filtro por nome do beneficiário
            if (!string.IsNullOrWhiteSpace(nomeBeneficiario))
                queryBase = queryBase.Where(r =>
                    r.Beneficiario.Usuario.Nome.Contains(nomeBeneficiario));

            // Filtro por período de reserva
            if (dataInicio.HasValue)
                queryBase = queryBase.Where(r => r.DataReserva >= dataInicio.Value.Date);
            if (dataFim.HasValue)
                queryBase = queryBase.Where(r => r.DataReserva < dataFim.Value.Date.AddDays(1));

            var todas = await queryBase
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            // Divide por status (o filtro de status esconde seções inteiras)
            bool mostrarPendente   = string.IsNullOrEmpty(status) || status == "Pendente";
            bool mostrarConfirmada = string.IsNullOrEmpty(status) || status == "Confirmada";
            bool mostrarRetirada   = string.IsNullOrEmpty(status) || status == "Retirada";

            var reservasPendentes   = mostrarPendente   ? todas.Where(r => r.Status == StatusReserva.Pendente).ToList()   : new();
            var reservasConfirmadas = mostrarConfirmada ? todas.Where(r => r.Status == StatusReserva.Confirmada).ToList() : new();
            var reservasRetiradas   = mostrarRetirada   ? todas.Where(r => r.Status == StatusReserva.Retirada).ToList()   : new();

            // Avaliações já realizadas pelo doador para as reservas Retiradas
            var idsRetiradas = reservasRetiradas.Select(r => r.IdReserva).ToList();
            Dictionary<int, int> avaliacoesDoador = new();
            if (idsRetiradas.Any())
            {
                avaliacoesDoador = await _context.Avaliacoes
                    .Where(a => a.IdAvaliador == usuarioId
                             && a.IdReserva != null
                             && idsRetiradas.Contains(a.IdReserva.Value))
                    .ToDictionaryAsync(a => a.IdReserva!.Value, a => a.Nota);
            }

            // Pedidos em que ao menos uma avaliação existe — grupo todo considerado avaliado
            var pedidosJaAvaliados = reservasRetiradas
                .Where(r => avaliacoesDoador.ContainsKey(r.IdReserva))
                .Select(r => r.IdPedido)
                .Distinct()
                .ToHashSet();

            // Função local de projeção
            GerenciarReservaDoadorViewModel MapReserva(Reserva r, bool ehRetirada = false) => new()
            {
                IdReserva             = r.IdReserva,
                IdPedido              = r.IdPedido ?? 0,
                DataReserva           = r.DataReserva,
                StatusReserva         = r.Status.ToString(),
                QuantidadeReservada   = r.QuantidadeReservada,
                NumeroLote            = r.Lote.NumeroLote,
                DataValidadeLote      = r.Lote.DataValidade,
                IdProduto             = r.Lote.Produto.IdProduto,
                NomeProduto           = r.Lote.Produto.NomeProduto,
                MarcaProduto          = r.Lote.Produto.MarcaProduto,
                CategoriaProduto      = r.Lote.Produto.CategoriaProduto,
                UnidadeMedidaProduto  = r.Lote.Produto.UnidadeMedida,
                FotoProduto           = r.Lote.Produto.FotoProduto == null
                                          ? null
                                          : ObterFotoProdutoDataUrl(r.Lote.Produto.FotoProduto),
                IdUsuarioBeneficiario = r.IdBeneficiario,
                NomeBeneficiario      = r.Beneficiario.Usuario.Nome,
                TelefoneBeneficiario  = r.Beneficiario.Usuario.Telefone,
                EhOng                 = r.Beneficiario.Eong,
                DataRetiradaInicio    = r.DataRetiradaInicio,
                DataRetiradaFim       = r.DataRetiradaFim,
                TokenConfirmacao      = r.TokenConfirmacao,
                PodeAvaliar           = ehRetirada,
                JaAvaliou             = ehRetirada && avaliacoesDoador.ContainsKey(r.IdReserva),
                NotaAvaliacao         = (ehRetirada && avaliacoesDoador.TryGetValue(r.IdReserva, out var n))
                                          ? n : (int?)null
            };

            var viewModel = new GerenciarReservasPageViewModel
            {
                Pendentes   = reservasPendentes.Select(r => MapReserva(r)).ToList(),
                Confirmadas = reservasConfirmadas.Select(r => MapReserva(r)).ToList(),

                // Retiradas: TODOS os itens de pedidos ainda não avaliados
                // (agrupados por pedido na view para mostrar todos os produtos do card)
                Retiradas = reservasRetiradas
                    .Where(r => !pedidosJaAvaliados.Contains(r.IdPedido))
                    .Select(r => MapReserva(r, ehRetirada: true))
                    .ToList(),

                // Filtros ativos — para manter os valores no formulário
                FiltroNomeBeneficiario = nomeBeneficiario,
                FiltroDataInicio       = dataInicio,
                FiltroDataFim          = dataFim,
                FiltroStatus           = status
            };

            return View(viewModel);
        }

        // =========================================================================================
        // POST: /Produtos/AprovarReserva
        // Doador aprova a reserva, define o intervalo de retirada e gera o token de confirmação.
        // A quantidade já foi deduzida do lote em Finalizar — não alteramos StatusLote aqui.
        // =========================================================================================

        [HttpPost]
        [Authorize(Roles = "DoadorPF,DoadorPJ")]
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

            // Valida se status da reserva é diferente de pendente
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
            var tokenGerado = Guid.NewGuid().ToString("N")[..8].ToUpper();
            reserva.TokenConfirmacao = tokenGerado;
            reserva.DataRetiradaInicio = viewModel.DataRetiradaInicio;
            reserva.DataRetiradaFim = viewModel.DataRetiradaFim;
            reserva.Status = StatusReserva.Confirmada;

            // Aprova todas as reservas irmãs do mesmo pedido e mesmo doador com o mesmo token
            var irmasParaAprovar = await _context.Reservas
                .Include(r => r.Lote).ThenInclude(l => l.Produto)
                .Where(r => r.IdPedido == reserva.IdPedido
                         && r.IdReserva != reserva.IdReserva
                         && r.Status == StatusReserva.Pendente
                         && r.Lote.Produto.IdDoador == usuarioId)
                .ToListAsync();

            foreach (var irma in irmasParaAprovar)
            {
                irma.TokenConfirmacao = tokenGerado;
                irma.DataRetiradaInicio = viewModel.DataRetiradaInicio;
                irma.DataRetiradaFim = viewModel.DataRetiradaFim;
                irma.Status = StatusReserva.Confirmada;
            }

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(reserva.IdPedido);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Reserva #{reserva.IdReserva} aprovada com sucesso!";
            return RedirectToAction(nameof(GerenciarReservas));
        }

        // =====================================================================
        // POST: /Produtos/RejeitarItens
        // Doador rejeita uma seleção de reservas dentro de um pedido.
        // Quando todos os itens do pedido são enviados, equivale à rejeição total.
        // =====================================================================
        [HttpPost]
        [Authorize(Roles = "DoadorPF,DoadorPJ")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarItens(int idPedido, List<int> idsReservas, string? motivoRejeicao)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(motivoRejeicao))
            {
                TempData["Erro"] = "Informe o motivo da rejeição.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            if (idsReservas == null || !idsReservas.Any())
            {
                TempData["Erro"] = "Selecione ao menos um item para rejeitar.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            var reservas = await _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                .Where(r => idsReservas.Contains(r.IdReserva)
                         && r.IdPedido == idPedido
                         && r.Lote.Produto.IdDoador == usuarioId
                         && r.Status == StatusReserva.Pendente)
                .ToListAsync();

            if (!reservas.Any())
            {
                TempData["Erro"] = "Nenhuma reserva válida encontrada para rejeitar.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

            foreach (var reserva in reservas)
            {
                reserva.Status = StatusReserva.Rejeitada;
                reserva.MotivoRejeicao = motivoRejeicao.Trim();
                reserva.DataEncerramento = DateTime.UtcNow;

                // Devolve o lote para a vitrine se ainda estiver dentro da validade
                if (reserva.Lote.DataValidade.Date >= DateTime.Today)
                {
                    reserva.Lote.Quantidade += reserva.QuantidadeReservada;

                    if (reserva.Lote.StatusLote == StatusLote.Inativo)
                        reserva.Lote.StatusLote = StatusLote.Disponivel;

                    reserva.Lote.Produto.StatusProduto = true;
                }
            }

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(idPedido);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = reservas.Count == 1
                ? $"1 item rejeitado com sucesso."
                : $"{reservas.Count} itens rejeitados com sucesso.";
            return RedirectToAction(nameof(GerenciarReservas));
        }

        // POST: /Produtos/RejeitarReserva
        // Doador rejeita a reserva. O lote volta para a vitrine
        // se ainda estiver dentro da validade.
        // =====================================================================
        [HttpPost]
        [Authorize(Roles = "DoadorPF,DoadorPJ")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejeitarReserva(int idReserva, string? motivoRejeicao)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(motivoRejeicao))
            {
                TempData["Erro"] = "Informe o motivo da rejeição.";
                return RedirectToAction(nameof(GerenciarReservas));
            }

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

            // Rejeita a reserva com o motivo informado
            reserva.Status = StatusReserva.Rejeitada;
            reserva.MotivoRejeicao = motivoRejeicao.Trim();
            reserva.DataEncerramento = DateTime.UtcNow;

            // Devolve o lote para a vitrine se ainda estiver dentro da validade
            if (reserva.Lote.DataValidade.Date >= DateTime.Today)
            {
                reserva.Lote.Quantidade += reserva.QuantidadeReservada;

                // Garante que o lote volte a Disponivel (pode estar Inativo se foi zerado)
                if (reserva.Lote.StatusLote == StatusLote.Inativo)
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
        // Se válido, marca a reserva como Retirada.
        // O lote só é marcado como Entregue se a quantidade for zero.
        // =====================================================================
        [HttpPost]
        [Authorize(Roles = "DoadorPF,DoadorPJ")]
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

            if (reserva.Lote.Quantidade == 0)
                reserva.Lote.StatusLote = StatusLote.Entregue;

            // Confirma todas as reservas irmãs do mesmo pedido e mesmo doador
            var irmasParaConfirmar = await _context.Reservas
                .Include(r => r.Lote).ThenInclude(l => l.Produto)
                .Where(r => r.IdPedido == reserva.IdPedido
                         && r.IdReserva != reserva.IdReserva
                         && r.Status == StatusReserva.Confirmada
                         && r.Lote.Produto.IdDoador == usuarioId)
                .ToListAsync();

            foreach (var irma in irmasParaConfirmar)
            {
                irma.Status = StatusReserva.Retirada;
                irma.DataEncerramento = DateTime.UtcNow;
                if (irma.Lote.Quantidade == 0)
                    irma.Lote.StatusLote = StatusLote.Entregue;
            }

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(reserva.IdPedido);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Entrega da reserva #{reserva.IdReserva} confirmada! Avalie o beneficiario na secao abaixo.";
            return RedirectToAction(nameof(GerenciarReservas));
        }
    }
}
