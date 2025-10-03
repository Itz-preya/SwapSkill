using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSwapApp.Models;

namespace SkillSwapApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Skill> Skills { get; set; }
        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}