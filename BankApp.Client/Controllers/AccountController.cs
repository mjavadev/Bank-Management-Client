using BankApp.Client.Dto;
using BankApp.Client.HttpClients;
using BankApp.Client.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BankApp.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IGenericHttpClient _httpClient;

        public AccountController(IGenericHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userRequest = new UserRequest
                {
                    UserName = model.UserName,
                    Password = model.Password
                };

                var result = await _httpClient.PostAsync<Result<UserResponse>>(ApiConstant.Authenticate, userRequest);

                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to connect to authentication service");
                    return View(model);
                }

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    return View(model);
                }

                var userResponse = result.Response;
                if (userResponse == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid response from authentication service");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userResponse.UserName),
                    new Claim(ClaimTypes.NameIdentifier, userResponse.Id),
                    new Claim("UserId", userResponse.Id),
                    new Claim("FullName", userResponse.FullName),
                    new Claim("jwttoken", userResponse.Token ?? "")  // ⭐ Store JWT token in claims
                };

             //   var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{model.UserName}:{model.Password}"));

                if (userResponse.Roles != null && userResponse.Roles.Any())
                {
                    foreach (var role in userResponse.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Check if password change is required
                if (userResponse.MustChangePassword)
                {
                    TempData["InfoMessage"] = "You must change your password before continuing.";
                    return RedirectToAction("ChangePassword");
                }

                return RedirectToDashboard();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred during login: {ex.Message}");
                return View(model);
            }
        }

        private IActionResult RedirectToDashboard()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Dashboard", "Admin");
            else if (User.IsInRole("Manager"))
                return RedirectToAction("Dashboard", "Manager");
            else
                return RedirectToAction("Dashboard", "Customer");
        }


        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new ChangePasswordRequest
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword
                };

                var result = await _httpClient.PostAsync<Result<bool>>(ApiConstant.ChangePassword, request);

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    return View(model);
                }

                TempData["SuccessMessage"] = "Password changed successfully. Please login again.";
                return RedirectToAction("Logout");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"API Error: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while changing password.");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}