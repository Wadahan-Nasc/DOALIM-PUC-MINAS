using Doalim_dev.Models;
using Doalim_dev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Doalim_dev.Models.ViewModels;

namespace Doalim_dev.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? busca = null)
        {
            var query = _context.Usuarios
                .AsNoTracking()
                .OrderByDescending(u => u.TipoUsuario != TipoUsuario.Admin
                    && u.StatusVerificacao == StatusVerificacao.Pendente
                    && u.Arquivocomprovacao != null
                    && u.Arquivocomprovacao.Length > 0)
                .ThenBy(u => u.Nome)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.Trim();
                query = query.Where(u =>
                    u.Nome.Contains(termo)
                    || u.Email.Contains(termo));
            }

            ViewBag.Busca = busca;
            return View(await query.ToListAsync());
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
                usuario.Arquivocomprovacao = null;
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
            usuario.Arquivocomprovacao = null;

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
                usuario.Arquivocomprovacao = null;
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Administradores nao precisam de arquivo de comprovacao.";
                return RedirectToAction(nameof(Index));
            }

            if (status == StatusVerificacao.Aprovado
                && (usuario.Arquivocomprovacao == null || usuario.Arquivocomprovacao.Length == 0))
            {
                TempData["Erro"] = "Nao e possivel aprovar um usuario sem arquivo de comprovacao.";
                return RedirectToAction(nameof(Details), new { id });
            }

            usuario.StatusVerificacao = status;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Comprovacao de {usuario.Nome} atualizada para {status}.";
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> MeuPerfil(Usuario model, IFormFile? arquivoComprovacao, IFormFile? arquivoFotoPerfil)
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

            if (UsuarioRegras.PrecisaComprovacao(usuario) && comprovacaoValida.conteudo != null)
            {
                usuario.Arquivocomprovacao = comprovacaoValida.conteudo;
                usuario.StatusVerificacao = StatusVerificacao.Pendente;
            }

            if (!UsuarioRegras.PrecisaComprovacao(usuario))
            {
                usuario.Arquivocomprovacao = null;
                usuario.StatusVerificacao = StatusVerificacao.NaoAplicavel;
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["NovaSenha"]))
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(Request.Form["NovaSenha"].ToString());

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

            TempData["Sucesso"] = "Perfil de doador ativado. Entre novamente para atualizar o menu de acesso.";
            return RedirectToAction(nameof(MeuPerfil));
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

            TempData["Sucesso"] = "Perfil de beneficiario ativado. Entre novamente para atualizar o menu de acesso.";
            return RedirectToAction(nameof(MeuPerfil));
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

            var vm = new PerfilPublicoViewModel
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                FotoPerfil = usuario.FotoPerfil,
                Bio = usuario.Bio,
                TipoUsuario = usuario.TipoUsuario,
                Verificado = UsuarioRegras.TemComprovacaoAprovada(usuario),
                MembroDesde = usuario.DataCadastro,
                NotaMedia = null,
                TotalAvaliacoes = 0
            };

            return View(vm);
        }

        private async Task<Usuario?> ObterUsuarioLogadoAsync()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out var id))
            {
                return await _context.Usuarios
                    .Include(u => u.Endereco)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);
                var extensoesPermitidas = new[] { ".png", ".jpg", ".jpeg" };
                var extensao = Path.GetExtension(arquivoFotoPerfil.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    TempData["Erro"] = "Formato de foto inválido. Use PNG, JPG ou JPEG.";
                    return View(model);
                }

                const long tamanhoMaximo = 2 * 1024 * 1024; // 2MB
                if (arquivoFotoPerfil.Length > tamanhoMaximo)
                {
                    TempData["Erro"] = "A foto excede o tamanho máximo permitido de 2MB.";
                    return View(model);
                }

                using var msFoto = new MemoryStream();
                await arquivoFotoPerfil.CopyToAsync(msFoto);
                usuario.FotoPerfil = msFoto.ToArray();
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
            }
            else
            {
                usuario.Cnpj = null;
                if (string.IsNullOrWhiteSpace(UsuarioRegras.NormalizarDigitos(usuario.Cpf)))
                    ModelState.AddModelError(nameof(usuario.Cpf), "Informe o CPF.");
                var extensoesPermitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
                var extensao = Path.GetExtension(arquivoComprovacao.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    TempData["Erro"] = "Formato de arquivo inválido. Use PNG, JPG, JPEG ou PDF.";
                    return View(model);
                }

                const long tamanhoMaximo = 5 * 1024 * 1024; // 5MB
                if (arquivoComprovacao.Length > tamanhoMaximo)
                {
                    TempData["Erro"] = "O arquivo excede o tamanho máximo permitido de 5MB.";
                    return View(model);
                }

                using var msComp = new MemoryStream();
                await arquivoComprovacao.CopyToAsync(msComp);
                usuario.Arquivocomprovacao = msComp.ToArray();
            }
        }

        private void LimparModelStateDeCamposGerenciados()
        {
            ModelState.Remove(nameof(Usuario.TermosAceitados));
            ModelState.Remove(nameof(Usuario.FotoPerfil));
            ModelState.Remove(nameof(Usuario.Arquivocomprovacao));
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
            usuario.Nome = model.Nome;
            // Atualiza senha apenas se preenchida
            if (!string.IsNullOrWhiteSpace(Request.Form["NovaSenha"]))
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(Request.Form["NovaSenha"]!);
            usuario.Email = model.Email;
            usuario.Telefone = model.Telefone;
            usuario.Bio = model.Bio;

            // FotoPerfil, Cpf e Cnpj são tratados separadamente acima

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
            TempData["Sucesso"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("MeuPerfil");                      
            }
        
    // GET: /Usuarios/PerfilPublico/5
            [AllowAnonymous]
        public async Task<IActionResult> PerfilPublico(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.Ativo);

            if (usuario == null)
                return NotFound();

            // Dados públicos — nunca expor Email, Cpf, Cnpj, Telefone, Endereco, SenhaHash
            var vm = new PerfilPublicoViewModel
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                FotoPerfil = usuario.FotoPerfil,
                Bio = usuario.Bio,
                TipoUsuario = usuario.TipoUsuario,
                Verificado = usuario.StatusVerificacao == StatusVerificacao.Aprovado,
                MembroDesde = usuario.DataCadastro,
                // Avaliações ficam para quando RF-014 estiver pronto
                NotaMedia = null,
                TotalAvaliacoes = 0
            };

            return View(vm);
        }
    }
}
