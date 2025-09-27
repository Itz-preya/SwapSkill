using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSwapApp.Models; // Use the correct namespace for ApplicationUser

namespace SkillSwapApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}