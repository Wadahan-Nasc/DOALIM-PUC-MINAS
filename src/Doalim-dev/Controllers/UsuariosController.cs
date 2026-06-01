using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doalim_dev.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.StatusVerificacao == StatusVerificacao.Aprovado)
                .ThenBy(u => u.Nome)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.Trim();
                usuarios = usuarios
                    .Where(u => u.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)
                             || u.Email.Contains(termo, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            await PopularPerfisViewBagAsync();
            ViewBag.Busca = busca;

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

            await PopularPerfisViewBagAsync();
            return View(usuario);
        }

        public IActionResult Create(TipoUsuario? tipoUsuario = null)
        {
            return View(new Usuario
            {
                TipoUsuario = tipoUsuario ?? TipoUsuario.Admin,
                Ativo = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Cnpj,Cpf,Nome,Email,Telefone,Endereco,FotoPerfil,Arquivocomprovacao,TipoUsuario,SenhaHash,Ativo")] Usuario usuario)
        {
            ModelState.Remove(nameof(usuario.TermosAceitados));

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                ModelState.AddModelError(nameof(usuario.Email), "Este e-mail já está cadastrado.");

            await ValidarIdentificadoresUnicosAsync(usuario);

            if (ModelState.IsValid)
            {
                usuario.Ativo = true;
                usuario.DataCadastro = DateTime.UtcNow;
                usuario.StatusVerificacao = usuario.TipoUsuario == TipoUsuario.Admin
                    ? StatusVerificacao.NaoAplicavel
                    : StatusVerificacao.Pendente;
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                await CriarPerfilComplementarAsync(usuario);

                TempData["Sucesso"] = usuario.TipoUsuario == TipoUsuario.Admin
                    ? "Administrador cadastrado com sucesso."
                    : "Usuário cadastrado com sucesso.";

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            TempData["Erro"] = "Dados pessoais só podem ser alterados pelo próprio usuário em Meu Perfil.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            TempData["Erro"] = "Dados pessoais só podem ser alterados pelo próprio usuário em Meu Perfil.";
            return await Task.FromResult(RedirectToAction(nameof(Details), new { id }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatusVerificacao(int id, StatusVerificacao status)
        {
            if (status != StatusVerificacao.Aprovado && status != StatusVerificacao.Rejeitado && status != StatusVerificacao.Pendente)
                return BadRequest();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                usuario.StatusVerificacao = StatusVerificacao.NaoAplicavel;
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Administrador não precisa de arquivo de comprovação.";
                return RedirectToAction(nameof(Index));
            }

            if (status == StatusVerificacao.Aprovado && string.IsNullOrWhiteSpace(usuario.Arquivocomprovacao))
            {
                TempData["Erro"] = "Não é possível aprovar um usuário sem arquivo de comprovação.";
                return RedirectToAction(nameof(Details), new { id });
            }

            usuario.StatusVerificacao = status;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Documentação de {usuario.Nome} atualizada para {status}.";
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

            ModelState.Remove("SenhaHash");
            ModelState.Remove("Arquivocomprovacao");
            ModelState.Remove("FotoPerfil");
            ModelState.Remove("TermosAceitados");

            await ValidarIdentificadoresUnicosAsync(model, usuario.IdUsuario);

            if (!ModelState.IsValid)
            {
                await PopularMeuPerfilViewBagAsync(usuario.IdUsuario);
                model.IdUsuario = usuario.IdUsuario;
                model.TipoUsuario = usuario.TipoUsuario;
                model.StatusVerificacao = usuario.StatusVerificacao;
                model.Ativo = usuario.Ativo;
                model.Arquivocomprovacao = usuario.Arquivocomprovacao;
                model.FotoPerfil = usuario.FotoPerfil;
                return View(model);
            }

            if (arquivoFotoPerfil != null && arquivoFotoPerfil.Length > 0)
            {
                var extensoesPermitidas = new[] { ".png", ".jpg", ".jpeg" };
                var extensao = Path.GetExtension(arquivoFotoPerfil.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    TempData["Erro"] = "Formato de foto inválido. Use PNG, JPG ou JPEG.";
                    return RedirectToAction(nameof(MeuPerfil));
                }

                const long tamanhoMaximo = 2 * 1024 * 1024;
                if (arquivoFotoPerfil.Length > tamanhoMaximo)
                {
                    TempData["Erro"] = "A foto excede o tamanho máximo permitido de 2MB.";
                    return RedirectToAction(nameof(MeuPerfil));
                }

                var pastaUpload = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "fotos-perfil");
                Directory.CreateDirectory(pastaUpload);

                var nomeArquivo = $"{usuario.IdUsuario}_{DateTime.Now:yyyyMMddHHmmss}{extensao}";
                var caminhoCompleto = Path.Combine(pastaUpload, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    await arquivoFotoPerfil.CopyToAsync(stream);

                usuario.FotoPerfil = $"/uploads/fotos-perfil/{nomeArquivo}";
            }

            if (arquivoComprovacao != null && arquivoComprovacao.Length > 0)
            {
                var extensoesPermitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
                var extensao = Path.GetExtension(arquivoComprovacao.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    TempData["Erro"] = "Formato de arquivo inválido. Use PNG, JPG, JPEG ou PDF.";
                    return RedirectToAction(nameof(MeuPerfil));
                }

                const long tamanhoMaximo = 5 * 1024 * 1024;
                if (arquivoComprovacao.Length > tamanhoMaximo)
                {
                    TempData["Erro"] = "O arquivo excede o tamanho máximo permitido de 5MB.";
                    return RedirectToAction(nameof(MeuPerfil));
                }

                var pastaUpload = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "comprovacoes");
                Directory.CreateDirectory(pastaUpload);

                var nomeArquivo = $"{usuario.IdUsuario}_{DateTime.Now:yyyyMMddHHmmss}{extensao}";
                var caminhoCompleto = Path.Combine(pastaUpload, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    await arquivoComprovacao.CopyToAsync(stream);

                usuario.Arquivocomprovacao = $"/uploads/comprovacoes/{nomeArquivo}";
                usuario.StatusVerificacao = StatusVerificacao.Pendente;
            }

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.Telefone = model.Telefone;
            usuario.Endereco = model.Endereco;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Perfil atualizado com sucesso.";
            return RedirectToAction(nameof(MeuPerfil));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TornarDoador()
        {
            return await AdicionarPerfilAsync("doador");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TornarBeneficiario()
        {
            return await AdicionarPerfilAsync("beneficiario");
        }

        private async Task<IActionResult> AdicionarPerfilAsync(string perfil)
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Auth");

            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario == null)
                return NotFound();

            if (usuario.TipoUsuario == TipoUsuario.Admin)
            {
                TempData["Erro"] = "Administradores não podem solicitar perfil operacional.";
                return RedirectToAction(nameof(MeuPerfil));
            }

            if (perfil == "doador")
            {
                if (!await _context.Doadores.AnyAsync(d => d.IdUsuario == usuario.IdUsuario))
                    _context.Doadores.Add(new Doador { IdUsuario = usuario.IdUsuario, QtdAlimentosDoados = "0" });

                if (usuario.TipoUsuario == TipoUsuario.BeneficiarioPF || usuario.TipoUsuario == TipoUsuario.BeneficiarioPJ)
                    usuario.TipoUsuario = UsuarioRegras.TipoDoadorCorrespondente(usuario);
            }
            else
            {
                if (!await _context.Beneficiarios.AnyAsync(b => b.IdUsuario == usuario.IdUsuario))
                    _context.Beneficiarios.Add(new Beneficiario { IdUsuario = usuario.IdUsuario });

                if (usuario.TipoUsuario == TipoUsuario.DoadorPF || usuario.TipoUsuario == TipoUsuario.DoadorPJ)
                    usuario.TipoUsuario = UsuarioRegras.TipoBeneficiarioCorrespondente(usuario);
            }

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Perfil complementar ativado. As ações continuam liberadas apenas após comprovação aprovada.";
            return RedirectToAction(nameof(MeuPerfil));
        }

        private async Task<Usuario?> ObterUsuarioLogadoAsync()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(usuarioId, out var id))
                return await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        private async Task ValidarIdentificadoresUnicosAsync(Usuario usuario, int? ignorarId = null)
        {
            var cpf = UsuarioRegras.NormalizarDigitos(usuario.Cpf);
            var cnpj = UsuarioRegras.NormalizarDigitos(usuario.Cnpj);
            var telefone = UsuarioRegras.NormalizarDigitos(usuario.Telefone);

            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Where(u => ignorarId == null || u.IdUsuario != ignorarId.Value)
                .Select(u => new { u.Cpf, u.Cnpj, u.Telefone })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(cpf) && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Cpf) == cpf))
                ModelState.AddModelError(nameof(usuario.Cpf), "Este CPF já está cadastrado.");

            if (!string.IsNullOrWhiteSpace(cnpj) && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Cnpj) == cnpj))
                ModelState.AddModelError(nameof(usuario.Cnpj), "Este CNPJ já está cadastrado.");

            if (!string.IsNullOrWhiteSpace(telefone) && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Telefone) == telefone))
                ModelState.AddModelError(nameof(usuario.Telefone), "Este telefone já está cadastrado.");
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
                    _context.Doadores.Add(new Doador { IdUsuario = usuario.IdUsuario, QtdAlimentosDoados = "0" });
            }
            else if (usuario.TipoUsuario == TipoUsuario.BeneficiarioPF || usuario.TipoUsuario == TipoUsuario.BeneficiarioPJ)
            {
                if (!await _context.Beneficiarios.AnyAsync(b => b.IdUsuario == usuario.IdUsuario))
                    _context.Beneficiarios.Add(new Beneficiario { IdUsuario = usuario.IdUsuario });
            }

            await _context.SaveChangesAsync();
        }

        private async Task PopularPerfisViewBagAsync()
        {
            ViewBag.Doadores = (await _context.Doadores.AsNoTracking().Select(d => d.IdUsuario).ToListAsync()).ToHashSet();
            ViewBag.Beneficiarios = (await _context.Beneficiarios.AsNoTracking().Select(b => b.IdUsuario).ToListAsync()).ToHashSet();
        }

        private async Task PopularMeuPerfilViewBagAsync(int usuarioId)
        {
            ViewBag.TemDoador = await _context.Doadores.AsNoTracking().AnyAsync(d => d.IdUsuario == usuarioId);
            ViewBag.TemBeneficiario = await _context.Beneficiarios.AsNoTracking().AnyAsync(b => b.IdUsuario == usuarioId);
        }
    }
}