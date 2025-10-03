using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using SkillSwapApp.ViewModels;
using System.Linq;
using System.Security.Claims;
using System;

namespace SkillSwapApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public AccountRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = new PasswordHasher<ApplicationUser>();
        }

        public bool Register(RegisterViewModel model)
        {
            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
                return false;

            // Create new user and securely hash the password
            var user = new ApplicationUser
            {
                Email = model.Email,
                NormalizedEmail = model.Email?.ToUpperInvariant(),
                UserName = model.Email, // use email as username
                NormalizedUserName = model.Email?.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString(),
                FullName = model.FullName,
                OfferedSkill = model.OfferedSkill,
                NeededSkill = model.NeededSkill
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public bool Login(LoginViewModel model)
        {
            // Validate user
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email);

            if (user == null) return false;

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                return false;
            }

            // Create claims (UserId + Email)   secure cookie to the browser.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email)
            };

//These claims are put into a ClaimsIdentity and then wrapped into a ClaimsPrincipal.
//ASP.NET Core calls SignInAsync, which issues a secure cookie to the browser.

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in user with cookie
            _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            ).Wait();

            return true;
        }

//watch 
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

        // Maintenance helper: remove all users from AspNetUsers
        public void DeleteAllUsers()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }
    }
}