using BankApp.Client.Dto;
using BankApp.Client.HttpClients;
using BankApp.Client.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Client.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly IGenericHttpClient _httpClient;

        public AdminController(IGenericHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var employeesResult = await _httpClient.GetAsync<Result<List<EmployeeDto>>>(ApiConstant.GetAllEmployees);
                var customersResult = await _httpClient.GetAsync<Result<List<CustomerDto>>>(ApiConstant.GetAllCustomers);
                var applicationsResult = await _httpClient.GetAsync<Result<List<ApplicationDto>>>(ApiConstant.GetPendingApplications);

                ViewBag.TotalEmployees = employeesResult.IsError ? 0 : employeesResult.Response?.Count ?? 0;
                ViewBag.TotalCustomers = customersResult.IsError ? 0 : customersResult.Response?.Count ?? 0;
                ViewBag.PendingApplications = applicationsResult.IsError ? 0 : applicationsResult.Response?.Count ?? 0;

                return View();
            }
            catch
            {
                ViewBag.TotalEmployees = 0;
                ViewBag.TotalCustomers = 0;
                ViewBag.PendingApplications = 0;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Employees()
        {
            try
            {
                var result = await _httpClient.GetAsync<Result<List<EmployeeDto>>>(ApiConstant.GetAllEmployees);
                return View(result.IsError ? new List<EmployeeDto>() : result.Response);
            }
            catch
            {
                return View(new List<EmployeeDto>());
            }
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var employeeDto = new EmployeeDto
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Password = model.Password,
                    StaffCode = model.StaffCode,
                    JobTitle = model.JobTitle,
                    DateHired = model.DateHired
                };

                var result = await _httpClient.PostAsync<Result<EmployeeDto>>(ApiConstant.CreateEmployee, employeeDto);

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    return View(model);
                }

                TempData["SuccessMessage"] = "Employee created successfully.";
                return RedirectToAction("Employees");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the employee.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.GetEmployeeById, id);
                var result = await _httpClient.GetAsync<Result<EmployeeDto>>(url);

                if (result.IsError || result.Response == null)
                {
                    TempData["ErrorMessage"] = "Employee not found.";
                    return RedirectToAction("Employees");
                }

                var viewModel = new EmployeeViewModel
                {
                    UserName = result.Response.UserName,
                    FullName = result.Response.FullName,
                    StaffCode = result.Response.StaffCode,
                    JobTitle = result.Response.JobTitle,
                    DateHired = result.Response.DateHired
                };

                ViewBag.EmployeeId = id;
                return View(viewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred.";
                return RedirectToAction("Employees");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EmployeeId = id;
                return View(model);
            }

            try
            {
                var employeeDto = new EmployeeDto
                {
                    EmployeeID = id,
                    UserName = model.UserName,
                    FullName = model.FullName,
                    StaffCode = model.StaffCode,
                    JobTitle = model.JobTitle,
                    DateHired = model.DateHired
                };

                var url = string.Format(ApiConstant.UpdateEmployee, id);
                var result = await _httpClient.PutAsync<Result<bool>>(url, employeeDto);

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                    ViewBag.EmployeeId = id;
                    return View(model);
                }

                TempData["SuccessMessage"] = "Employee updated successfully.";
                return RedirectToAction("Employees");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the employee.");
                ViewBag.EmployeeId = id;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.DeleteEmployee, id);
                var result = await _httpClient.DeleteAsync<Result<bool>>(url);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Failed to delete employee" });
                }

                return Json(new { success = true, message = "Employee deleted successfully" });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }
    }

}

