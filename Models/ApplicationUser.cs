using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SkillSwapApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extra fields for your project
        [Required]
        public string FullName { get; set; }
        [Required]
        public string OfferedSkill { get; set; }
        [Required]
        public string NeededSkill { get; set; }
        [Required]
        [EmailAddress]  // data annotations for email format validation
        public override string Email { get; set; }
    }
}
