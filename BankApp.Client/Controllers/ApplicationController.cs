using BankApp.Client.Dto;
using BankApp.Client.HttpClients;
using BankApp.Client.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Client.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IGenericHttpClient _httpClient;

        public ApplicationController(IGenericHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.FullName), "FullName");
                formData.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), "DateOfBirth");
                formData.Add(new StringContent(model.Gender), "Gender");
                formData.Add(new StringContent(model.Occupation), "Occupation");
                formData.Add(new StringContent(model.MobileNumber), "MobileNumber");
                formData.Add(new StringContent(model.AadharNumber), "AadharNumber");
                formData.Add(new StringContent(model.PAN), "PAN");
                formData.Add(new StringContent(model.AccountTypeID.ToString()), "AccountTypeID");
                formData.Add(new StringContent("1"), "Status");
                formData.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), "ApplicationDate");

                if (model.ImageFile != null)
                {
                    var fileContent = new StreamContent(model.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
                    formData.Add(fileContent, "imageFile", model.ImageFile.FileName);
                }

                var result = await _httpClient.PostFormDataAsync<Result<ApplicationDto>>(ApiConstant.CreateApplication, formData);

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    return View(model);
                }

                TempData["SuccessMessage"] = "Application submitted successfully! Please wait for approval.";
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while submitting the application.");
                return View(model);
            }
        }

        public IActionResult Success()
        {
            return View();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.GetApplicationById, id);
                var result = await _httpClient.GetAsync<Result<ApplicationDto>>(url);

                if (result.IsError)
                {
                    TempData["ErrorMessage"] = "Application not found.";
                    return RedirectToAction("Dashboard", "Manager");
                }

                return View(result.Response);
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while fetching application details.";
                return RedirectToAction("Dashboard", "Manager");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.GetApplicationById, id);
                var result = await _httpClient.GetAsync<Result<ApplicationDto>>(url);

                if (result.IsError || result.Response == null)
                {
                    TempData["ErrorMessage"] = "Application not found.";
                    return RedirectToAction("Dashboard", "Manager");
                }

                var viewModel = new ApplicationViewModel
                {
                    FullName = result.Response.FullName,
                    DateOfBirth = result.Response.DateOfBirth,
                    Gender = result.Response.Gender,
                    Occupation = result.Response.Occupation,
                    MobileNumber = result.Response.MobileNumber,
                    AadharNumber = result.Response.AadharNumber,
                    PAN = result.Response.PAN,
                    AccountTypeID = result.Response.AccountTypeID
                };

                ViewBag.ApplicationId = id;
                return View(viewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while fetching application details.";
                return RedirectToAction("Dashboard", "Manager");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ApplicationId = id;
                return View(model);
            }

            try
            {
                var applicationDto = new ApplicationDto
                {
                    ApplicationID = id,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Occupation = model.Occupation,
                    MobileNumber = model.MobileNumber,
                    AadharNumber = model.AadharNumber,
                    PAN = model.PAN,
                    AccountTypeID = model.AccountTypeID,
                    Status = 1,
                    ApplicationDate = DateTime.Now
                };

                var url = string.Format(ApiConstant.UpdateApplication, id);
                var result = await _httpClient.PutAsync<Result<bool>>(url, applicationDto);

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    ViewBag.ApplicationId = id;
                    return View(model);
                }

                TempData["SuccessMessage"] = "Application updated successfully.";
                return RedirectToAction("Dashboard", "Manager");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the application.");
                ViewBag.ApplicationId = id;
                return View(model);
            }
        }
    }

}
