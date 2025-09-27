using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using SkillSwapApp.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace SkillSwapApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Register(RegisterViewModel model)
        {
            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
                return false;

            // Create new user (⚠️ password stored as plain text for demo only)
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                OfferedSkill = model.OfferedSkill,
                NeededSkill = model.NeededSkill,
                PasswordHash = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public bool Login(LoginViewModel model)
        {
            // Validate user
            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (user == null) return false;

            // ✅ Create claims (UserId + Email)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ✅ Sign in user with cookie
            _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            ).Wait();

            return true;
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            ).Wait();
        }
    }
}