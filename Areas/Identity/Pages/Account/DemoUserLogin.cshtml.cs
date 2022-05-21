using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BugTracker.Areas.Identity.Pages.Account
{
    public class DemoUserLoginModel : PageModel
    {

        private readonly SignInManager<BTUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IConfiguration _configuration;

        public DemoUserLoginModel(SignInManager<BTUser> signInManager, UserManager<BTUser> userManager, ILogger<LoginModel> logger, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }


        public async Task<IActionResult> OnPostAsync(string userRole)
        {
            string returnUrl = "~/Home/Dashboard";

            string userEmail = "";
            string userPassword = "";

            switch (userRole)
            {
                case "admin":
                    userEmail = _configuration["AdminEmail"];
                    userPassword = _configuration["DemoPassword"];
                    break;

                case "pm":
                    userEmail = _configuration["PMEmail"];
                    userPassword = _configuration["DemoPassword"];
                    break;

                case "developer":
                    userEmail = _configuration["DeveloperEmail"];
                    userPassword = _configuration["DemoPassword"];
                    break;

                case "submitter":
                    userEmail = _configuration["SubmitterEmail"];
                    userPassword = _configuration["DemoPassword"];
                    break;
            }

            var result = await _signInManager.PasswordSignInAsync(userEmail, userPassword, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = false });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}
