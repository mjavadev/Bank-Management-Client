using BankApp.Client.Dto;
using BankApp.Client.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Client.Controllers
{
    [Authorize(Roles = "Manager,Admin")]

    public class ManagerController : Controller
    {
        private readonly IGenericHttpClient _httpClient;

        public ManagerController(IGenericHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var applicationsResult = await _httpClient.GetAsync<Result<List<ApplicationDto>>>(ApiConstant.GetPendingApplications);
                var transactionsResult = await _httpClient.GetAsync<Result<List<TransactionDto>>>(ApiConstant.GetPendingTransactions);

                ViewBag.PendingApplications = applicationsResult.IsError ? 0 : applicationsResult.Response?.Count ?? 0;
                ViewBag.PendingTransactions = transactionsResult.IsError ? 0 : transactionsResult.Response?.Count ?? 0;

                return View();
            }
            catch
            {
                ViewBag.PendingApplications = 0;
                ViewBag.PendingTransactions = 0;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            try
            {
                var result = await _httpClient.GetAsync<Result<List<ApplicationDto>>>(ApiConstant.GetAllApplications);
                return View(result.IsError ? new List<ApplicationDto>() : result.Response);
            }
            catch
            {
                return View(new List<ApplicationDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingApplications()
        {
            try
            {
                var result = await _httpClient.GetAsync<Result<List<ApplicationDto>>>(ApiConstant.GetPendingApplications);
                return View(result.IsError ? new List<ApplicationDto>() : result.Response);
            }
            catch
            {
                return View(new List<ApplicationDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.ApproveApplication, id);
                var result = await _httpClient.PostAsync<Result<UserResponse>>(url);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Failed to approve application" });
                }

                var userResponse = result.Response;
                return Json(new
                {
                    success = true,
                    userName = userResponse.UserName,
                    password = userResponse.TemporaryPassword,
                    fullName = userResponse.FullName,
                    message = "Application approved successfully"
                });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetApplicationDetails(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.GetApplicationById, id);
                var result = await _httpClient.GetAsync<Result<ApplicationDto>>(url);

                if (result.IsError || result.Response == null)
                {
                    return Json(new { success = false, message = "Application not found" });
                }

                return Json(new { success = true, application = result.Response });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> RejectApplication(int id, [FromBody] RejectRequest reasonModel)
        {
            try
            {
                // Defensive null/empty check for the Reason property
                if (reasonModel == null || string.IsNullOrWhiteSpace(reasonModel.Reason))
                {
                    return Json(new { success = false, message = "Rejection reason is required" });
                }

                var url = string.Format(ApiConstant.RejectApplication, id);
                var result = await _httpClient.PostAsync<Result<bool>>(url, reasonModel);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Failed to reject application" });
                }

                return Json(new { success = true, message = "Application rejected successfully" });
            }
            catch (Exception ex)
            {
                // Optional: log ex.Message or ex.ToString() here
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public class RejectRequest
        {
            public string Reason { get; set; }
        }


        [HttpGet]
        public async Task<IActionResult> Transactions()
        {
            try
            {
                var result = await _httpClient.GetAsync<Result<List<TransactionDto>>>(ApiConstant.GetAllTransactions);
                return View(result.IsError ? new List<TransactionDto>() : result.Response);
            }
            catch
            {
                return View(new List<TransactionDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingTransactions()
        {
            try
            {
                var result = await _httpClient.GetAsync<Result<List<TransactionDto>>>(ApiConstant.GetPendingTransactions);
                return View(result.IsError ? new List<TransactionDto>() : result.Response);
            }
            catch
            {
                return View(new List<TransactionDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> TransactionDetails(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.GetTransactionById, id);
                var result = await _httpClient.GetAsync<Result<TransactionDto>>(url);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Transaction not found" });
                }

                return Json(new { success = true, transaction = result.Response });
            }
            catch
            {
                return Json(new { success = false, message = "Error fetching transaction details" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTransaction(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.ApproveTransaction, id);
                var result = await _httpClient.PostAsync<Result<bool>>(url);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Failed to approve transaction" });
                }

                return Json(new { success = true, message = "Transaction approved successfully" });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectTransaction(int id)
        {
            try
            {
                var url = string.Format(ApiConstant.RejectTransaction, id);
                var result = await _httpClient.PostAsync<Result<bool>>(url);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Failed to reject transaction" });
                }

                return Json(new { success = true, message = "Transaction rejected successfully" });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            try
            {
                var result = await _httpClient.GetAsync<Result<List<CustomerDto>>>(ApiConstant.GetAllCustomers);
                return View(result.IsError ? new List<CustomerDto>() : result.Response);
            }
            catch
            {
                return View(new List<CustomerDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CustomerAccounts(int customerId)
        {
            try
            {
                // Get customer details
                var customerUrl = string.Format(ApiConstant.GetCustomerById, customerId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult.IsError || customerResult.Response == null)
                {
                    TempData["ErrorMessage"] = "Unable to fetch customer details.";
                    return RedirectToAction("Index"); // Redirect back to customers list
                }

                var customer = customerResult.Response;

                // Get accounts for this customer
                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerId);
                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                if (accountsResult.IsError || accountsResult.Response == null)
                {
                    TempData["ErrorMessage"] = "Unable to fetch accounts for this customer.";
                    return RedirectToAction("Index");
                }

                var accounts = accountsResult.Response;

                // Pass customer info via ViewBag
                ViewBag.CustomerName = customer.FullName;
                ViewBag.CustomerID = customer.CustomerID;
                ViewBag.CustomerUsername = customer.UserName;

                return View(accounts);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching customer accounts.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAccountStatus(int accountId, int customerId)
        {
            try
            {
                var url = string.Format(ApiConstant.ToggleAccountStatus, accountId);
                var result = await _httpClient.PutAsync<Result<bool>>(url, null);

                if (result.IsError)
                {
                    TempData["ErrorMessage"] = "Failed to toggle account status.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Account status updated successfully!";
                }

                return RedirectToAction("CustomerAccounts", new { customerId = customerId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating account status.";
                return RedirectToAction("CustomerAccounts", new { customerId = customerId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var result = await _httpClient.GetAsync<Result<CustomerDto>>(string.Format(ApiConstant.GetCustomerById, id));
            if (result.IsError || result.Response == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Customers");
            }

            var dto = new EditCustomerDto
            {
                CustomerID = result.Response.CustomerID,
                FullName = result.Response.FullName,
                DateOfBirth = result.Response.DateOfBirth,
                Gender = result.Response.Gender,
                Occupation = result.Response.Occupation,
                MobileNumber = result.Response.MobileNumber,
                AadharNumber = result.Response.AadharNumber,
                PAN = result.Response.PAN,
                UserName = result.Response.UserName,
                ApplicationUserID = result.Response.ApplicationUserID
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(EditCustomerDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _httpClient.PutAsync<Result<bool>>(string.Format(ApiConstant.UpdateCustomer, model.CustomerID), model);
            if (result.IsError || result.Response == false)
            {
                ModelState.AddModelError(string.Empty, "Failed to update customer details.");
                return View(model);
            }
            TempData["SuccessMessage"] = "Customer details updated successfully.";
            return RedirectToAction("Customers");
        }


    }

}
