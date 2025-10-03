using Microsoft.AspNetCore.Mvc;
using SkillSwapApp.Repositories;
using SkillSwapApp.ViewModels;

namespace SkillSwapApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;  //inject dependency
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool result = _accountRepository.Register(model);
                if (result)
                    return RedirectToAction("Index", "Dashboard");

                ModelState.AddModelError("", "Registration failed.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1?? Check if user exists
                if (!_accountRepository.UserExists(model.Email))
                {
                    TempData["ErrorMessage"] = "No account found with this email. Please register first.";
                    return RedirectToAction("Register", "Account");
                }

                // 2?? Try to login if user exists
                bool result = _accountRepository.Login(model);
                if (result)
                    return RedirectToAction("Index", "Dashboard");


                // 3?? Wrong password
                ModelState.AddModelError("", "Invalid password. Please try again.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            _accountRepository.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
