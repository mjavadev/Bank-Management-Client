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

        [HttpPost]
        public async Task<IActionResult> RejectApplication(int id, string reason)
        {
            try
            {
                var url = string.Format(ApiConstant.RejectApplication, id);
                var rejectRequest = new RejectRequest { Reason = reason };
                var result = await _httpClient.PostAsync<Result<bool>>(url, rejectRequest);

                if (result.IsError)
                {
                    return Json(new { success = false, message = "Failed to reject application" });
                }

                return Json(new { success = true, message = "Application rejected successfully" });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred" });
            }
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
    }

}
