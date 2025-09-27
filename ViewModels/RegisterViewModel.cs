namespace SkillSwapApp.ViewModels
{
    public class RegisterViewModel
    {
        public string FullName { get; set; }
        public string OfferedSkill { get; set; }
        public string NeededSkill { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}