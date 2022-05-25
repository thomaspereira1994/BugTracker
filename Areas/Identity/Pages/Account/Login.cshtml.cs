using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BugTracker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<BTUser> _signInManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IConfiguration _configuration;

        public LoginModel(SignInManager<BTUser> signInManager,
                          UserManager<BTUser> userManager,
                          ILogger<LoginModel> logger,
                          IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? demoUserRole, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Home/Dashboard");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!string.IsNullOrEmpty(demoUserRole))
            {
                switch (demoUserRole)
                {
                    case "admin":
                        Input.Email = _configuration["AdminEmail"];
                        Input.Password = _configuration["DemoPassword"];
                        Input.RememberMe = false;
                        break;

                    case "pm":
                        Input.Email = _configuration["PMEmail"];
                        Input.Password = _configuration["DemoPassword"];
                        Input.RememberMe = false;
                        break;

                    case "developer":
                        Input.Email = _configuration["DeveloperEmail"];
                        Input.Password = _configuration["DemoPassword"];
                        Input.RememberMe = false;
                        break;


                    case "submitter":
                        Input.Email = _configuration["SubmitterEmail"];
                        Input.Password = _configuration["DemoPassword"];
                        Input.RememberMe = false;
                        break;
                }

            }

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
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

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //    public async Task<IActionResult> OnPostAsync(string userRole, string? notNeeded)
        //    {
        //        string returnUrl = "~/Home/Dashboard";

        //        string userEmail = "";
        //        string userPassword = "";

        //        switch (userRole)
        //        {
        //            case "admin":
        //                userEmail = _configuration["AdminUsername"];
        //                userPassword = _configuration["AdminPassword"];
        //                break;

        //            case "pm":
        //                userEmail = _configuration["PMUsername"];
        //                userPassword = _configuration["PMPassword"];
        //                break;

        //            case "developer":
        //                userEmail = _configuration["DeveloperUsername"];
        //                userPassword = _configuration["DeveloperPassword"];
        //                break;

        //            case "submitter":
        //                userEmail = _configuration["SubmitterUsername"];
        //                userPassword = _configuration["AdminPassword"];
        //                break;
        //        }

        //        var result = await _signInManager.PasswordSignInAsync(userEmail, userPassword, false, lockoutOnFailure: false);

        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User logged in.");
        //            return LocalRedirect(returnUrl);
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning("User account locked out.");
        //            return RedirectToPage("./Lockout");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return Page();
        //        }
        //    }
        //
    }
}
