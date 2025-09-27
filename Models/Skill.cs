using System.ComponentModel.DataAnnotations;

namespace SkillSwapApp.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }  // e.g. "C#", "Cooking"

        public string Level { get; set; } // Beginner, Intermediate, Expert

        // Relation to User
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
