using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Cnpj,Cpf,Nome,Email,Telefone,Endereco,FotoPerfil,Arquivocomprovacao,TipoUsuario,SenhaHash")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Cnpj,Cpf,Nome,Email,Telefone,Endereco,FotoPerfil,Arquivocomprovacao,TipoUsuario,SenhaHash")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        // GET: /Usuarios/MeuPerfil
        [AllowAnonymous]
        public async Task<IActionResult> MeuPerfil()
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.FindFirstValue(ClaimTypes.Email)
                     ?? User.Identity.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: /Usuarios/MeuPerfil
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MeuPerfil(Usuario model, IFormFile? arquivoComprovacao, IFormFile? arquivoFotoPerfil)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            ModelState.Remove("SenhaHash");
            ModelState.Remove("Arquivocomprovacao");
            ModelState.Remove("FotoPerfil");
            ModelState.Remove("TermosAceitados");

            if (!ModelState.IsValid) return View(model);

            var email = User.FindFirstValue(ClaimTypes.Email)
                     ?? User.Identity.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null) return NotFound();

            // Upload da foto de perfil
            if (arquivoFotoPerfil != null && arquivoFotoPerfil.Length > 0)
            {
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

                var pastaUpload = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "fotos-perfil");
                Directory.CreateDirectory(pastaUpload);

                var nomeArquivo = $"{usuario.IdUsuario}_{DateTime.Now:yyyyMMddHHmmss}{extensao}";
                var caminhoCompleto = Path.Combine(pastaUpload, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivoFotoPerfil.CopyToAsync(stream);
                }

                usuario.FotoPerfil = $"/uploads/fotos-perfil/{nomeArquivo}";
            }

            // Upload do arquivo de comprovação
            if (arquivoComprovacao != null && arquivoComprovacao.Length > 0)
            {
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

                var pastaUpload = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "comprovacoes");
                Directory.CreateDirectory(pastaUpload);

                var nomeArquivo = $"{usuario.IdUsuario}_{DateTime.Now:yyyyMMddHHmmss}{extensao}";
                var caminhoCompleto = Path.Combine(pastaUpload, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivoComprovacao.CopyToAsync(stream);
                }

                usuario.Arquivocomprovacao = $"/uploads/comprovacoes/{nomeArquivo}";
            }

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.Telefone = model.Telefone;
            usuario.Endereco = model.Endereco;
            // FotoPerfil, Cpf e Cnpj são tratados separadamente acima

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("MeuPerfil");
        }
    }
}