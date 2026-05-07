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