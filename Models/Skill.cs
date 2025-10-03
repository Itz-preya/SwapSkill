using System.ComponentModel.DataAnnotations;

namespace SkillSwapApp.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }  

        

       
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
