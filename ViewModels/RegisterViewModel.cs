using System.ComponentModel.DataAnnotations;

namespace SkillSwapApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")] 
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please select your offered skill")] 
        public string OfferedSkill { get; set; }

        [Required(ErrorMessage = "Please select the skill you need")] 
        public string NeededSkill { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}