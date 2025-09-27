using SkillSwapApp.ViewModels;

namespace SkillSwapApp.Repositories
{
    public interface IAccountRepository
    {
        bool Register(RegisterViewModel model);
        bool Login(LoginViewModel model);
        void Logout();
        bool UserExists(string email);
    }
}