using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : BaseController
    {
        public UsuariosController(AppDbContext context) : base(context) { }

        private const int ItensPorPagina = 15;

        public async Task<IActionResult> Index(string? busca = null, int pagina = 1,
                                                string? status = null, string? tipo = null)
        {
            var query = _context.Usuarios
                .AsNoTracking()
                .OrderByDescending(u => u.TipoUsuario != TipoUsuario.Admin
                    && u.StatusVerificacao == StatusVerificacao.Pendente
                    && u.ArquivoComprovacao != null)
                .ThenBy(u => u.Nome)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.Trim();
                query = query.Where(u =>
                    u.Nome.Contains(termo)
                    || u.Email.Contains(termo));
            }

            // Filtro por tipo de perfil
            if (!string.IsNullOrWhiteSpace(tipo) && Enum.TryParse<TipoUsuario>(tipo, out var tipoEnum))
                query = query.Where(u => u.TipoUsuario == tipoEnum);

            // Filtro por status de verificação
            if (status == "SemArquivo")
                query = query.Where(u => u.TipoUsuario != TipoUsuario.Admin
                                      && (u.ArquivoComprovacao == null || u.ArquivoComprovacao.Length == 0));
            else if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StatusVerificacao>(status, out var statusEnum))
                query = query.Where(u => u.StatusVerificacao == statusEnum);

            var totalRegistros = await query.CountAsync();
            var totalPaginas   = (int)Math.Ceiling(totalRegistros / (double)ItensPorPagina);
            pagina = Math.Clamp(pagina, 1, Math.Max(1, totalPaginas));

            var usuarios = await query
                .Skip((pagina - 1) * ItensPorPagina)
                .Take(ItensPorPagina)
                .ToListAsync();

            ViewBag.Busca        = busca;
            ViewBag.Status       = status;
            ViewBag.Tipo         = tipo;
            ViewBag.PaginaAtual  = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(usuarios);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // Serve o arquivo de comprovação com headers HTTP corretos para abrir inline no browser
        // Acessível pelo próprio usuário (dono do arquivo) ou pelo administrador.
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerComprovacao(int id)
        {
            // Verifica permissão: dono do arquivo ou admin
            var idLogado = ObterIdUsuarioLogado();
            var ehAdmin  = User.IsInRole("Admin");
            if (!ehAdmin && idLogado != id)
                return User.Identity?.IsAuthenticated == true ? Forbid() : RedirectToAction("Login", "Auth");

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new { u.IdUsuario, u.ArquivoComprovacao })
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null || usuario.ArquivoComprovacao == null || usuario.ArquivoComprovacao.Length == 0)
                return NotFound();

            var arquivo = usuario.ArquivoComprovacao;

            // Detecta MIME pelos magic bytes
            string mime;
            if (arquivo.Length >= 4
                && arquivo[0] == 0x25 && arquivo[1] == 0x50
                && arquivo[2] == 0x44 && arquivo[3] == 0x46)
            {
                mime = "application/pdf";   // %PDF
            }
            else if (arquivo.Length >= 4
                && arquivo[0] == 0x89 && arquivo[1] == 0x50
                && arquivo[2] == 0x4E && arquivo[3] == 0x47)
            {
                mime = "image/png";          // PNG
            }
            else
            {
                mime = "image/jpeg";         // fallback JPEG
            }

            return File(arquivo, mime);
        }

        public IActionResult Create(TipoUsuario? tipoUsuario = null)
        {
            return View(new Usuario
            {
                TipoUsuario = tipoUsuario ?? TipoUsuario.DoadorPF,
                Ativo = true,
                StatusVerificacao = tipoUsuario == TipoUsuario.Admin
                    ? StatusVerificacao.NaoAplicavel
                    : StatusVerificacao.Pendente
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            LimparModelStateDeCamposGerenciados();

            usuario.Email = usuario.Email?.Trim() ?? string.Empty;
            usuario.Telefone = usuario.Telefone?.Trim() ?? string.Empty;
            usuario.Cpf = string.IsNullOrWhiteSpace(usuario.Cpf) ? null : usuario.Cpf.Trim();
            usuario.Cnpj = string.IsNullOrWhiteSpace(usuario.Cnpj) ? null : usuario.Cnpj.Trim();

            await ValidarIdentificadoresUnicosAsync(usuario);

            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                usuario.Cpf = null;
                usuario.Cnpj = null;
                usuario.ArquivoComprovacao = null;
                usuario.StatusVerificacao = StatusVerificacao.NaoAplicavel;
            }
            else
            {
                ValidarDocumentoObrigatorio(usuario);
                usuario.StatusVerificacao = StatusVerificacao.Pendente;
            }

            if (!ModelState.IsValid)
                return View(usuario);

            usuario.Ativo = true;
            usuario.DataCadastro = DateTime.UtcNow;
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
            usuario.FotoPerfil = null;
            usuario.ArquivoComprovacao = null;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await CriarPerfilComplementarAsync(usuario);

            TempData["Sucesso"] = usuario.TipoUsuario == TipoUsuario.Admin
                ? "Administrador cadastrado com sucesso."
                : "Usuario cadastrado. Ele precisara enviar a comprovacao pelo proprio perfil.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Endereco)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
                return NotFound();

            TempData["Erro"] = "O admin nao pode alterar dados pessoais de usuarios. Use esta tela apenas para consultar e aprovar comprovacoes.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id)
        {
            TempData["Erro"] = "O admin nao pode alterar dados pessoais de usuarios.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatusVerificacao(int id, StatusVerificacao status)
        {
            if (status != StatusVerificacao.Aprovado
                && status != StatusVerificacao.Rejeitado
                && status != StatusVerificacao.Pendente)
            {
                return BadRequest();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                usuario.StatusVerificacao = StatusVerificacao.NaoAplicavel;
                usuario.ArquivoComprovacao = null;
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Administradores nao precisam de arquivo de comprovacao.";
                return RedirectToAction(nameof(Index));
            }

            usuario.StatusVerificacao = status;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Comprovação de {usuario.Nome} atualizada para {status}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
                _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> MeuPerfil()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Auth");

            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario == null)
                return NotFound();

            await PopularMeuPerfilViewBagAsync(usuario.IdUsuario);
            return View(usuario);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MeuPerfil(Usuario model, IFormFile? arquivoComprovacao, IFormFile? arquivoFotoPerfil, bool removerComprovacao = false)
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Auth");

            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario == null)
                return NotFound();

            LimparModelStateDeCamposGerenciados();
            ModelState.Remove(nameof(Usuario.SenhaHash));
            ModelState.Remove(nameof(Usuario.TipoUsuario));
            ModelState.Remove(nameof(Usuario.StatusVerificacao));
            ModelState.Remove(nameof(Usuario.Cpf));
            ModelState.Remove(nameof(Usuario.Cnpj));

            model.IdUsuario = usuario.IdUsuario;
            model.TipoUsuario = usuario.TipoUsuario;
            model.Cpf = usuario.Cpf;
            model.Cnpj = usuario.Cnpj;
            model.SenhaHash = usuario.SenhaHash;
            model.StatusVerificacao = usuario.StatusVerificacao;
            model.Ativo = usuario.Ativo;
            model.DataCadastro = usuario.DataCadastro;

            await ValidarIdentificadoresUnicosAsync(model, usuario.IdUsuario);

            if (!ModelState.IsValid)
            {
                await PopularMeuPerfilViewBagAsync(usuario.IdUsuario);
                usuario.Nome = model.Nome;
                usuario.Email = model.Email;
                usuario.Telefone = model.Telefone;
                usuario.Bio = model.Bio;
                usuario.Endereco = model.Endereco;
                return View(usuario);
            }

            var fotoValida = await LerUploadAsync(arquivoFotoPerfil, 2 * 1024 * 1024, new[] { ".png", ".jpg", ".jpeg" }, "foto");
            if (fotoValida.erro != null)
            {
                TempData["Erro"] = fotoValida.erro;
                await PopularMeuPerfilViewBagAsync(usuario.IdUsuario);
                return View(usuario);
            }

            var comprovacaoValida = await LerUploadAsync(arquivoComprovacao, 5 * 1024 * 1024, new[] { ".png", ".jpg", ".jpeg", ".pdf" }, "comprovacao");
            if (comprovacaoValida.erro != null)
            {
                TempData["Erro"] = comprovacaoValida.erro;
                await PopularMeuPerfilViewBagAsync(usuario.IdUsuario);
                return View(usuario);
            }

            usuario.Nome = model.Nome?.Trim() ?? usuario.Nome;
            usuario.Email = model.Email?.Trim() ?? usuario.Email;
            usuario.Telefone = model.Telefone?.Trim() ?? usuario.Telefone;
            usuario.Bio = string.IsNullOrWhiteSpace(model.Bio) ? null : model.Bio.Trim();

            if (fotoValida.conteudo != null)
                usuario.FotoPerfil = fotoValida.conteudo;

            if (UsuarioRegras.PrecisaComprovacao(usuario))
            {
                if (removerComprovacao)
                {
                    usuario.ArquivoComprovacao = null;
                    usuario.StatusVerificacao = StatusVerificacao.Pendente;
                }
                else if (comprovacaoValida.conteudo != null)
                {
                    usuario.ArquivoComprovacao = comprovacaoValida.conteudo;
                    usuario.StatusVerificacao = StatusVerificacao.Pendente;
                }
            }
            else
            {
                usuario.ArquivoComprovacao = null;
                usuario.StatusVerificacao = StatusVerificacao.NaoAplicavel;
            }

            var novaSenha       = Request.Form["NovaSenha"].ToString();
            var confirmaSenha   = Request.Form["ConfirmaNovaSenha"].ToString();

            if (!string.IsNullOrWhiteSpace(novaSenha))
            {
                if (novaSenha != confirmaSenha)
                {
                    TempData["Erro"] = "A nova senha e a confirmação não conferem. Nenhuma alteração foi salva.";
                    await PopularMeuPerfilViewBagAsync(usuario.IdUsuario);
                    return View(usuario);
                }
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            }

            AtualizarEndereco(usuario, model.Endereco);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Perfil atualizado com sucesso.";
            return RedirectToAction(nameof(MeuPerfil));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TornarDoador()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Auth");

            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario == null)
                return NotFound();

            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                TempData["Erro"] = "Administradores nao usam perfis de doador ou beneficiario.";
                return RedirectToAction(nameof(MeuPerfil));
            }

            if (!await _context.Doadores.AnyAsync(d => d.IdUsuario == usuario.IdUsuario))
                _context.Doadores.Add(new Doador { IdUsuario = usuario.IdUsuario, QtdAlimentosDoados = 0 });

            usuario.TipoUsuario = UsuarioRegras.TipoDoadorCorrespondente(usuario);
            await _context.SaveChangesAsync();

            // Força o relogin para que o cookie de autenticação reflita as novas roles
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Sucesso"] = "Perfil de doador ativado com sucesso! Faça login novamente para acessar as novas funcionalidades.";
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TornarBeneficiario()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Auth");

            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario == null)
                return NotFound();

            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                TempData["Erro"] = "Administradores nao usam perfis de doador ou beneficiario.";
                return RedirectToAction(nameof(MeuPerfil));
            }

            if (!await _context.Beneficiarios.AnyAsync(b => b.IdUsuario == usuario.IdUsuario))
                _context.Beneficiarios.Add(new Beneficiario { IdUsuario = usuario.IdUsuario });

            usuario.TipoUsuario = UsuarioRegras.TipoBeneficiarioCorrespondente(usuario);
            await _context.SaveChangesAsync();

            // Força o relogin para que o cookie de autenticação reflita as novas roles
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Sucesso"] = "Perfil de beneficiário ativado com sucesso! Faça login novamente para acessar as novas funcionalidades.";
            return RedirectToAction("Login", "Auth");
        }

        [AllowAnonymous]
        public async Task<IActionResult> PerfilPublico(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.Ativo);

            if (usuario == null)
                return NotFound();

            // Estatísticas de avaliações
            var stats = await _context.Avaliacoes
                .Where(a => a.IdAvaliado == id.Value)
                .GroupBy(a => a.IdAvaliado)
                .Select(g => new { NotaMedia = g.Average(a => (double)a.Nota), Total = g.Count() })
                .FirstOrDefaultAsync();

            // Avaliacao agora e feita diretamente nos cards de reserva (por reserva).
            // O perfil publico exibe apenas a media — sem formulario de avaliacao.
            var vm = new PerfilPublicoViewModel
            {
                IdUsuario       = usuario.IdUsuario,
                Nome            = usuario.Nome,
                FotoPerfil      = usuario.FotoPerfil,
                Bio             = usuario.Bio,
                TipoUsuario     = usuario.TipoUsuario,
                Verificado      = UsuarioRegras.TemComprovacaoAprovada(usuario),
                MembroDesde     = usuario.DataCadastro,
                NotaMedia       = stats?.NotaMedia,
                TotalAvaliacoes = stats?.Total ?? 0,
                NotaDoLogado    = null,
                PodeAvaliar     = false
            };

            return View(vm);
        }

        // -----------------------------------------------------------------------------------------
        // GET: /Usuarios/PerfilPublicoJson?id=X
        // Retorna os dados públicos de um usuário em JSON — usado pelo modal de perfil.
        // -----------------------------------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PerfilPublicoJson(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.Ativo);

            if (usuario == null)
                return NotFound();

            var stats = await _context.Avaliacoes
                .Where(a => a.IdAvaliado == id)
                .GroupBy(a => a.IdAvaliado)
                .Select(g => new { NotaMedia = g.Average(a => (double)a.Nota), Total = g.Count() })
                .FirstOrDefaultAsync();

            var tipoLabel = usuario.TipoUsuario switch
            {
                TipoUsuario.DoadorPF       => "Doador PF",
                TipoUsuario.DoadorPJ       => "Doador PJ",
                TipoUsuario.BeneficiarioPF => "Beneficiário PF",
                TipoUsuario.BeneficiarioPJ => "Beneficiário PJ",
                _                          => "Usuário"
            };

            var iniciais = string.Concat(
                usuario.Nome.Split(' ')
                    .Where(p => p.Length > 0)
                    .Take(2)
                    .Select(p => p[0].ToString().ToUpper()));

            return Json(new
            {
                idUsuario       = usuario.IdUsuario,
                nome            = usuario.Nome,
                iniciais        = iniciais,
                bio             = usuario.Bio,
                tipoLabel       = tipoLabel,
                verificado      = UsuarioRegras.TemComprovacaoAprovada(usuario),
                membroDesde     = usuario.DataCadastro.ToString("MMMM 'de' yyyy",
                                      new System.Globalization.CultureInfo("pt-BR")),
                notaMedia       = stats?.NotaMedia,
                totalAvaliacoes = stats?.Total ?? 0,
                temFoto         = usuario.FotoPerfil != null && usuario.FotoPerfil.Length > 0,
                fotoUrl         = usuario.FotoPerfil != null && usuario.FotoPerfil.Length > 0
                                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(usuario.FotoPerfil)}"
                                    : null
            });
        }

        // -----------------------------------------------------------------------------------------
        // POST: /Usuarios/AvaliarUsuario
        // Registra ou atualiza a avaliacao do usuario logado para a reserva informada.
        // O "avaliado" e determinado automaticamente a partir da reserva (nao precisa ser enviado
        // pelo formulario — evita adulteracao de dados).
        // -----------------------------------------------------------------------------------------
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AvaliarUsuario(int idReserva, int nota)
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Auth");

            var idLogado = ObterIdUsuarioLogado();
            if (idLogado == 0) return RedirectToAction("Login", "Auth");

            if (nota < 1 || nota > 5)
            {
                TempData["Erro"] = "Nota invalida. Escolha entre 1 e 5 estrelas.";
                return RedirectToAction("MinhasReservas", "Reservas");
            }

            // Carrega a reserva para determinar quem e o "avaliado"
            var reserva = await _context.Reservas
                .Include(r => r.Lote).ThenInclude(l => l.Produto)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva);

            if (reserva == null) return NotFound();
            if (reserva.Status != StatusReserva.Retirada) return BadRequest();

            // Determina o avaliado e a pagina de retorno
            int idAvaliado;
            bool ehBeneficiario = reserva.IdBeneficiario == idLogado;
            bool ehDoador       = reserva.Lote.Produto.IdDoador == idLogado;

            if (ehBeneficiario)
                idAvaliado = reserva.Lote.Produto.IdDoador;
            else if (ehDoador)
                idAvaliado = reserva.IdBeneficiario;
            else
                return Forbid();

            // Bloqueia avaliacao duplicada — nao permite alterar nota ja enviada
            var avaliacaoExistente = await _context.Avaliacoes
                .AnyAsync(a => a.IdAvaliador == idLogado && a.IdReserva == idReserva);

            if (avaliacaoExistente)
            {
                TempData["Erro"] = "Voce ja avaliou esta reserva. A avaliacao nao pode ser alterada.";
                return ehBeneficiario
                    ? RedirectToAction("MinhasReservas", "Reservas")
                    : RedirectToAction("GerenciarReservas", "Produtos");
            }

            _context.Avaliacoes.Add(new Avaliacao
            {
                IdAvaliador   = idLogado,
                IdAvaliado    = idAvaliado,
                IdReserva     = idReserva,
                Nota          = nota,
                DataAvaliacao = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Avaliacao enviada! Obrigado pelo feedback.";

            return ehBeneficiario
                ? RedirectToAction("MinhasReservas", "Reservas")
                : RedirectToAction("GerenciarReservas", "Produtos");
        }

        private async Task<Usuario?> ObterUsuarioLogadoAsync()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out var id))
            {
                return await _context.Usuarios
                    .Include(u => u.Endereco)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            return string.IsNullOrWhiteSpace(email)
                ? null
                : await _context.Usuarios.Include(u => u.Endereco).FirstOrDefaultAsync(u => u.Email == email);
        }

        private async Task ValidarIdentificadoresUnicosAsync(Usuario usuario, int? ignorarId = null)
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Where(u => !ignorarId.HasValue || u.IdUsuario != ignorarId.Value)
                .Select(u => new { u.IdUsuario, u.Email, u.Cpf, u.Cnpj, u.Telefone })
                .ToListAsync();

            if (usuarios.Any(u => string.Equals(u.Email, usuario.Email, StringComparison.OrdinalIgnoreCase)))
                ModelState.AddModelError(nameof(usuario.Email), "Este e-mail ja esta cadastrado.");

            var cpf = UsuarioRegras.NormalizarDigitos(usuario.Cpf);
            if (!string.IsNullOrWhiteSpace(cpf)
                && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Cpf) == cpf))
                ModelState.AddModelError(nameof(usuario.Cpf), "Este CPF ja esta cadastrado.");

            var cnpj = UsuarioRegras.NormalizarDigitos(usuario.Cnpj);
            if (!string.IsNullOrWhiteSpace(cnpj)
                && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Cnpj) == cnpj))
                ModelState.AddModelError(nameof(usuario.Cnpj), "Este CNPJ ja esta cadastrado.");

            var telefone = UsuarioRegras.NormalizarDigitos(usuario.Telefone);
            if (!string.IsNullOrWhiteSpace(telefone)
                && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Telefone) == telefone))
                ModelState.AddModelError(nameof(usuario.Telefone), "Este numero de contato ja esta cadastrado.");
        }

        private void ValidarDocumentoObrigatorio(Usuario usuario)
        {
            if (UsuarioRegras.EhPessoaJuridica(usuario))
            {
                usuario.Cpf = null;
                if (string.IsNullOrWhiteSpace(UsuarioRegras.NormalizarDigitos(usuario.Cnpj)))
                    ModelState.AddModelError(nameof(usuario.Cnpj), "Informe o CNPJ.");
                else if (!UsuarioRegras.CnpjValido(usuario.Cnpj))
                    ModelState.AddModelError(nameof(usuario.Cnpj), "CNPJ inválido. Verifique os dígitos informados.");
            }
            else
            {
                usuario.Cnpj = null;
                if (string.IsNullOrWhiteSpace(UsuarioRegras.NormalizarDigitos(usuario.Cpf)))
                    ModelState.AddModelError(nameof(usuario.Cpf), "Informe o CPF.");
                else if (!UsuarioRegras.CpfValido(usuario.Cpf))
                    ModelState.AddModelError(nameof(usuario.Cpf), "CPF inválido. Verifique os dígitos informados.");
            }
        }

        private void LimparModelStateDeCamposGerenciados()
        {
            ModelState.Remove(nameof(Usuario.TermosAceitados));
            ModelState.Remove(nameof(Usuario.FotoPerfil));
            ModelState.Remove(nameof(Usuario.ArquivoComprovacao));
            ModelState.Remove(nameof(Usuario.Endereco));
            ModelState.Remove("Endereco.Usuario");
        }

        private async Task CriarPerfilComplementarAsync(Usuario usuario)
        {
            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                if (!await _context.Administradores.AnyAsync(a => a.IdUsuario == usuario.IdUsuario))
                    _context.Administradores.Add(new Administrador { IdUsuario = usuario.IdUsuario });
            }
            else if (usuario.TipoUsuario == TipoUsuario.DoadorPF || usuario.TipoUsuario == TipoUsuario.DoadorPJ)
            {
                if (!await _context.Doadores.AnyAsync(d => d.IdUsuario == usuario.IdUsuario))
                    _context.Doadores.Add(new Doador { IdUsuario = usuario.IdUsuario, QtdAlimentosDoados = 0 });
            }
            else if (usuario.TipoUsuario == TipoUsuario.BeneficiarioPF || usuario.TipoUsuario == TipoUsuario.BeneficiarioPJ)
            {
                if (!await _context.Beneficiarios.AnyAsync(b => b.IdUsuario == usuario.IdUsuario))
                    _context.Beneficiarios.Add(new Beneficiario { IdUsuario = usuario.IdUsuario });
            }

            await _context.SaveChangesAsync();
        }

        private void AtualizarEndereco(Usuario usuario, Endereco? endereco)
        {
            if (endereco == null
                || string.IsNullOrWhiteSpace(endereco.Cep)
                || string.IsNullOrWhiteSpace(endereco.Logradouro)
                || string.IsNullOrWhiteSpace(endereco.Numero)
                || string.IsNullOrWhiteSpace(endereco.Bairro)
                || string.IsNullOrWhiteSpace(endereco.Cidade)
                || string.IsNullOrWhiteSpace(endereco.Estado))
            {
                return;
            }

            if (usuario.Endereco == null)
            {
                usuario.Endereco = new Endereco { IdUsuario = usuario.IdUsuario };
                _context.Enderecos.Add(usuario.Endereco);
            }

            usuario.Endereco.Cep = endereco.Cep.Trim();
            usuario.Endereco.Logradouro = endereco.Logradouro.Trim();
            usuario.Endereco.Numero = endereco.Numero.Trim();
            usuario.Endereco.Complemento = string.IsNullOrWhiteSpace(endereco.Complemento) ? null : endereco.Complemento.Trim();
            usuario.Endereco.Bairro = endereco.Bairro.Trim();
            usuario.Endereco.Cidade = endereco.Cidade.Trim();
            usuario.Endereco.Estado = endereco.Estado.Trim().ToUpperInvariant();
        }

        private static async Task<(byte[]? conteudo, string? erro)> LerUploadAsync(
            IFormFile? arquivo,
            long tamanhoMaximo,
            string[] extensoesPermitidas,
            string nomeCampo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return (null, null);

            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!extensoesPermitidas.Contains(extensao))
                return (null, $"Formato de {nomeCampo} invalido.");

            if (arquivo.Length > tamanhoMaximo)
                return (null, $"O arquivo de {nomeCampo} excede o tamanho permitido.");

            using var ms = new MemoryStream();
            await arquivo.CopyToAsync(ms);
            return (ms.ToArray(), null);
        }

        private async Task PopularMeuPerfilViewBagAsync(int usuarioId)
        {
            ViewBag.EhDoador = await _context.Doadores.AsNoTracking().AnyAsync(d => d.IdUsuario == usuarioId);
            ViewBag.EhBeneficiario = await _context.Beneficiarios.AsNoTracking().AnyAsync(b => b.IdUsuario == usuarioId);

            var stats = await _context.Avaliacoes
                .Where(a => a.IdAvaliado == usuarioId)
                .GroupBy(a => a.IdAvaliado)
                .Select(g => new { NotaMedia = g.Average(a => (double)a.Nota), Total = g.Count() })
                .FirstOrDefaultAsync();

            ViewBag.NotaMedia       = stats?.NotaMedia;
            ViewBag.TotalAvaliacoes = stats?.Total ?? 0;
        }
    }
}
