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
            var usuarios = await _context.Usuarios
                .OrderByDescending(u => u.StatusVerificacao == StatusVerificacao.Pendente)
                .ThenBy(u => u.Nome)
                .ToListAsync();

            return View(usuarios);
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
        public IActionResult Create(TipoUsuario? tipoUsuario = null)
        {
            return View(new Usuario
            {
                TipoUsuario = tipoUsuario ?? TipoUsuario.DoadorPF,
                Ativo = true
            });
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Cnpj,Cpf,Nome,Email,Telefone,Endereco,FotoPerfil,Arquivocomprovacao,TipoUsuario,SenhaHash,Ativo")] Usuario usuario)
        {
            ModelState.Remove(nameof(usuario.TermosAceitados));

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                ModelState.AddModelError(nameof(usuario.Email), "Este e-mail já está cadastrado.");

            if (ModelState.IsValid)
            {
                usuario.Ativo = true;
                usuario.DataCadastro = DateTime.UtcNow;
                usuario.StatusVerificacao = StatusInicialPorTipo(usuario.TipoUsuario);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Cnpj,Cpf,Nome,Email,Telefone,Endereco,FotoPerfil,Arquivocomprovacao,TipoUsuario,SenhaHash,StatusVerificacao,Ativo")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(usuario.TermosAceitados));

            var usuarioAtual = await _context.Usuarios.FindAsync(id);
            if (usuarioAtual == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
                ModelState.Remove(nameof(usuario.SenhaHash));

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email && u.IdUsuario != id))
                ModelState.AddModelError(nameof(usuario.Email), "Este e-mail já está cadastrado.");

            if (ModelState.IsValid)
            {
                try
                {
                    usuarioAtual.Nome = usuario.Nome;
                    usuarioAtual.Cpf = usuario.Cpf;
                    usuarioAtual.Cnpj = usuario.Cnpj;
                    usuarioAtual.Email = usuario.Email;
                    usuarioAtual.Telefone = usuario.Telefone;
                    usuarioAtual.Endereco = usuario.Endereco;
                    usuarioAtual.FotoPerfil = usuario.FotoPerfil;
                    usuarioAtual.Arquivocomprovacao = usuario.Arquivocomprovacao;
                    usuarioAtual.TipoUsuario = usuario.TipoUsuario;
                    usuarioAtual.Ativo = usuario.Ativo;
                    usuarioAtual.StatusVerificacao = usuario.TipoUsuario == TipoUsuario.Admin
                        ? StatusVerificacao.NaoAplicavel
                        : usuario.StatusVerificacao;

                    if (!string.IsNullOrWhiteSpace(usuario.SenhaHash))
                        usuarioAtual.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);

                    await _context.SaveChangesAsync();
                    await CriarPerfilComplementarAsync(usuarioAtual);

                    TempData["Sucesso"] = "Usuário atualizado com sucesso.";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatusVerificacao(int id, StatusVerificacao status)
        {
            if (status != StatusVerificacao.Aprovado && status != StatusVerificacao.Rejeitado && status != StatusVerificacao.Pendente)
                return BadRequest();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.StatusVerificacao = status;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Documentação de {usuario.Nome} atualizada para {status}.";
            return RedirectToAction(nameof(Index));
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

        private static StatusVerificacao StatusInicialPorTipo(TipoUsuario tipoUsuario)
        {
            return tipoUsuario == TipoUsuario.DoadorPJ || tipoUsuario == TipoUsuario.BeneficiarioPJ
                ? StatusVerificacao.Pendente
                : StatusVerificacao.NaoAplicavel;
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
        public async Task<IActionResult> MeuPerfil(Usuario model)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            ModelState.Remove("SenhaHash");
            ModelState.Remove("Arquivocomprovacao");
            ModelState.Remove("TermosAceitados");

            if (!ModelState.IsValid) return View(model);

            var email = User.FindFirstValue(ClaimTypes.Email)
                     ?? User.Identity.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null) return NotFound();

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.Telefone = model.Telefone;
            usuario.Endereco = model.Endereco;
            usuario.FotoPerfil = model.FotoPerfil;
            // Cpf e Cnpj não são atualizados — campos somente leitura

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("MeuPerfil");
        }
    }
}
