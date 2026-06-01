using System.Security.Claims;
using Doalim_dev.Models;
using Doalim_dev.Services;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

       // REGISTRO — RF-001 + RF-002
       
        [HttpGet]
        public IActionResult Registro()
        {
            return View(new RegistroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel vm)
        {
            // Validações contextuais (dependem do TipoUsuario)
            bool ehDoador = vm.TipoUsuario == TipoUsuario.DoadorPF || vm.TipoUsuario == TipoUsuario.DoadorPJ;
            bool ehPJ = vm.TipoUsuario == TipoUsuario.DoadorPJ || vm.TipoUsuario == TipoUsuario.BeneficiarioPJ;
            bool ehPF = vm.TipoUsuario == TipoUsuario.DoadorPF || vm.TipoUsuario == TipoUsuario.BeneficiarioPF;

            if (vm.TipoUsuario == TipoUsuario.Admin)
                ModelState.AddModelError(nameof(vm.TipoUsuario),
                    "Cadastro de administrador deve ser feito apenas pelo seed do sistema ou por outro administrador.");

            // RF-002: Doador precisa aceitar o Termo
            if (ehDoador && !vm.AceitouTermo)
                ModelState.AddModelError(nameof(vm.AceitouTermo),
                    "O aceite do Termo de Responsabilidade é obrigatório para doadores.");

            // CPF obrigatório para PF
            if (ehPF && string.IsNullOrWhiteSpace(vm.Cpf))
                ModelState.AddModelError(nameof(vm.Cpf), "CPF é obrigatório para pessoa física.");

            // CNPJ obrigatório para PJ
            if (ehPJ && string.IsNullOrWhiteSpace(vm.Cnpj))
                ModelState.AddModelError(nameof(vm.Cnpj), "CNPJ é obrigatório para pessoa jurídica.");

            if (!ModelState.IsValid)
                return View(vm);

            // Verifica se e-mail já está cadastrado
            bool emailJaExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == vm.Email);

            if (emailJaExiste)
            {
                ModelState.AddModelError(nameof(vm.Email), "Este e-mail já está cadastrado.");
                return View(vm);
            }

            // Cria o usuário com hash BCrypt da senha
            await ValidarIdentificadoresUnicosAsync(vm);
            if (!ModelState.IsValid)
                return View(vm);
            var usuario = new Usuario
            {
                Nome               = vm.Nome,
                Email              = vm.Email,
                SenhaHash          = BCrypt.Net.BCrypt.HashPassword(vm.Senha),
                TipoUsuario        = vm.TipoUsuario,
                Cpf                = vm.Cpf,
                Cnpj               = vm.Cnpj,
                Telefone           = vm.Telefone,
                Endereco           = vm.Endereco,
                Ativo              = true,
                DataCadastro       = DateTime.UtcNow,
                // Todo usu�rio come�a pendente at� o administrador validar a documenta��o.
                StatusVerificacao  = StatusVerificacao.Pendente
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); // Gera o IdUsuario

            if (ehDoador)
            {
                _context.Doadores.Add(new Doador
                {
                    IdUsuario = usuario.IdUsuario,
                    QtdAlimentosDoados = "0"
                });
                await _context.SaveChangesAsync();
            }
            else if (vm.TipoUsuario == TipoUsuario.BeneficiarioPF || vm.TipoUsuario == TipoUsuario.BeneficiarioPJ)
            {
                _context.Beneficiarios.Add(new Beneficiario
                {
                    IdUsuario = usuario.IdUsuario
                });
                await _context.SaveChangesAsync();
            }
            else if (vm.TipoUsuario == TipoUsuario.Admin)
            {
                _context.Administradores.Add(new Administrador
                {
                    IdUsuario = usuario.IdUsuario
                });
                await _context.SaveChangesAsync();
            }

            // RF-002: Registra o aceite do termo para doadores
            if (ehDoador && vm.AceitouTermo)
            {
                var ipOrigem = HttpContext.Connection.RemoteIpAddress?.ToString();
                var termo = new TermoAceitacao
                {
                    UsuarioId    = usuario.IdUsuario,
                    DataAceite   = DateTime.UtcNow,
                    VersaoTermo  = "v1.0-2026",
                    IpOrigem     = ipOrigem
                };
                _context.TermosAceitacao.Add(termo);
                await _context.SaveChangesAsync();
            }

            // Faz login automático após o cadastro
            await RealizarLoginAsync(usuario, isPersistent: false);

            TempData["Sucesso"] = $"Bem-vindo(a), {usuario.Nome}! Cadastro realizado com sucesso.";
            return RedirectToAction("Index", "Home");
        }

        // LOGIN — RF-001     

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == vm.Email);

            // Verifica existência e senha sem revelar qual está errado (segurança)
            if (usuario == null || !SenhaConfere(vm.Senha, usuario.SenhaHash))
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
                return View(vm);
            }

            if (!usuario.Ativo)
            {
                ModelState.AddModelError(string.Empty,
                    "Sua conta está suspensa. Entre em contato com o suporte.");
                return View(vm);
            }

            await RealizarLoginAsync(usuario, vm.LembrarMe);

            // Redireciona para a URL de origem ou Dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (usuario.TipoUsuario == TipoUsuario.Admin)
                return RedirectToAction("Index", "Usuarios");

            return RedirectToAction("Index", "Home");
        }

        // LOGOUT — RF-001
        
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // RECUPERAÇÃO DE SENHA — RF-001
        
        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View(new RecuperarSenhaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarSenha(RecuperarSenhaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == vm.Email);

            // Sempre exibe a mesma mensagem, independente de o e-mail existir ou não
            // Isso evita enumeration attack
            if (usuario != null && usuario.Ativo)
            {
                usuario.TokenRecuperacao = Guid.NewGuid().ToString("N");
                usuario.TokenExpiracao   = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                var link = Url.Action(
                    nameof(ResetSenha), "Auth",
                    new { token = usuario.TokenRecuperacao, email = usuario.Email },
                    Request.Scheme);

                var corpo = $@"
                    <p>Olá, <strong>{usuario.Nome}</strong>!</p>
                    <p>Recebemos uma solicitação para redefinir a senha da sua conta no Doalim.</p>
                    <p>Clique no botão abaixo para criar uma nova senha. 
                       Este link é válido por <strong>1 hora</strong>.</p>
                    <p style='margin: 24px 0;'>
                        <a href='{link}' 
                           style='background:#1D9E75;color:#fff;padding:12px 24px;
                                  border-radius:8px;text-decoration:none;font-weight:bold;'>
                            Redefinir Senha
                        </a>
                    </p>
                    <p style='color:#888;font-size:12px;'>
                        Se você não solicitou a redefinição, ignore este e-mail.
                        Sua senha permanece a mesma.
                    </p>";

                await _emailService.EnviarEmailAsync(
                    usuario.Email,
                    "Doalim — Redefinição de Senha",
                    corpo);
            }

            TempData["Sucesso"] =
                "Se o e-mail estiver cadastrado, você receberá as instruções em breve.";

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ResetSenha(string token, string email)
        {
            // Valida o token antes de exibir o formulário
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.TokenRecuperacao == token &&
                    u.TokenExpiracao > DateTime.UtcNow);

            if (usuario == null)
            {
                TempData["Erro"] =
                    "Link inválido ou expirado. Solicite um novo link de recuperação.";
                return RedirectToAction(nameof(RecuperarSenha));
            }

            return View(new ResetSenhaViewModel { Token = token, Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetSenha(ResetSenhaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Email == vm.Email &&
                    u.TokenRecuperacao == vm.Token &&
                    u.TokenExpiracao > DateTime.UtcNow);

            if (usuario == null)
            {
                TempData["Erro"] =
                    "Link inválido ou expirado. Solicite um novo link de recuperação.";
                return RedirectToAction(nameof(RecuperarSenha));
            }

            // Atualiza a senha e invalida o token
            usuario.SenhaHash        = BCrypt.Net.BCrypt.HashPassword(vm.NovaSenha);
            usuario.TokenRecuperacao = null;
            usuario.TokenExpiracao   = null;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Senha redefinida com sucesso! Faça login com sua nova senha.";

            return RedirectToAction(nameof(Login));
        }

        // TERMO DE RESPONSABILIDADE — RF-002
        
        [HttpGet]
        public IActionResult Termo()
        {
            return View();
        }

        // MÉTODO PRIVADO: centraliza a criação do cookie
        
        private async Task RealizarLoginAsync(Usuario usuario, bool isPersistent)
        {
            var roles = new HashSet<string> { usuario.TipoUsuario.ToString() };

            if (await _context.Doadores.AnyAsync(d => d.IdUsuario == usuario.IdUsuario))
                roles.Add(UsuarioRegras.TipoDoadorCorrespondente(usuario).ToString());

            if (await _context.Beneficiarios.AnyAsync(b => b.IdUsuario == usuario.IdUsuario))
                roles.Add(UsuarioRegras.TipoBeneficiarioCorrespondente(usuario).ToString());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name,           usuario.Nome),
                new Claim(ClaimTypes.Email,          usuario.Email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var identidade  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal   = new ClaimsPrincipal(identidade);
            var authProps   = new AuthenticationProperties { IsPersistent = isPersistent };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProps);
        }
        private static bool SenhaConfere(string senha, string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(senhaHash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
            }
            catch
            {
                return false;
            }
        }
        private async Task ValidarIdentificadoresUnicosAsync(RegistroViewModel vm)
        {
            var cpf = UsuarioRegras.NormalizarDigitos(vm.Cpf);
            var cnpj = UsuarioRegras.NormalizarDigitos(vm.Cnpj);
            var telefone = UsuarioRegras.NormalizarDigitos(vm.Telefone);

            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new { u.Cpf, u.Cnpj, u.Telefone })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(cpf) && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Cpf) == cpf))
                ModelState.AddModelError(nameof(vm.Cpf), "Este CPF j� est� cadastrado.");

            if (!string.IsNullOrWhiteSpace(cnpj) && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Cnpj) == cnpj))
                ModelState.AddModelError(nameof(vm.Cnpj), "Este CNPJ j� est� cadastrado.");

            if (!string.IsNullOrWhiteSpace(telefone) && usuarios.Any(u => UsuarioRegras.NormalizarDigitos(u.Telefone) == telefone))
                ModelState.AddModelError(nameof(vm.Telefone), "Este telefone j� est� cadastrado.");
        }
    }
}
