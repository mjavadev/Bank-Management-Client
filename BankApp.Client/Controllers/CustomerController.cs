using BankApp.Client.Dto;
using BankApp.Client.HttpClients;
using BankApp.Client.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Client.Controllers
{
    [Authorize(Roles = "Customer")]

    public class CustomerController : Controller
    {
        private readonly IGenericHttpClient _httpClient;

        public CustomerController(IGenericHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;

                // Get customer details
                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult.IsError || customerResult.Response == null)
                {
                    TempData["ErrorMessage"] = "Unable to fetch customer details.";
                    return View(new DashboardViewModel());
                }

                var customer = customerResult.Response;

                // Get accounts
                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customer.CustomerID);
                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                var accounts = accountsResult.IsError ? new List<AccountDto>() : accountsResult.Response;

                // Get recent transactions from the first account
                var recentTransactions = new List<TransactionDto>();
                if (accounts.Any())
                {
                    var firstAccountId = accounts.First().AccountID;
                    var transactionsUrl = string.Format(ApiConstant.GetTransactionsByAccountId, firstAccountId);
                    var transactionsResult = await _httpClient.GetAsync<Result<List<TransactionDto>>>(transactionsUrl);

                    if (!transactionsResult.IsError && transactionsResult.Response != null)
                    {
                        recentTransactions = transactionsResult.Response
                            .OrderByDescending(t => t.TransactionDate)
                            .Take(10)
                            .ToList();
                    }
                }

                var viewModel = new DashboardViewModel
                {
                    Customer = customer,
                    Accounts = accounts,
                    RecentTransactions = recentTransactions,
                    TotalBalance = accounts.Sum(a => a.Balance),
                    PendingTransactionsCount = recentTransactions.Count(t => t.Status == 1)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading dashboard.";
                return View(new DashboardViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Transactions(int accountId)
        {
            try
            {
                var url = string.Format(ApiConstant.GetTransactionsByAccountId, accountId);
                var result = await _httpClient.GetAsync<Result<List<TransactionDto>>>(url);

                if (result.IsError)
                {
                    TempData["ErrorMessage"] = "Unable to fetch transactions.";
                    return View(new List<TransactionDto>());
                }

                ViewBag.AccountId = accountId;
                return View(result.Response ?? new List<TransactionDto>());
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while fetching transactions.";
                return View(new List<TransactionDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateTransaction()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult.IsError)
                {
                    TempData["ErrorMessage"] = "Unable to fetch customer details.";
                    return RedirectToAction("Dashboard");
                }

                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);
                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                ViewBag.Accounts = accountsResult.IsError ? new List<AccountDto>() : accountsResult.Response;
                return View();
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateTransaction(TransactionViewModel model)

        {

            //  Custom validation for Transfer transactions

            if (model.TransactionType == 3 && string.IsNullOrWhiteSpace(model.RecipientAccountNumber))

            {

                ModelState.AddModelError("RecipientAccountNumber", "Recipient account number is required for transfers");

            }

            if (!ModelState.IsValid)

            {

                var userId = User.FindFirst("UserId")?.Value;

                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);

                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);

                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                ViewBag.Accounts = accountsResult.IsError ? new List<AccountDto>() : accountsResult.Response;

                return View(model);

            }

            try

            {

                var transactionDto = new TransactionDto

                {

                    AccountID = model.AccountID,

                    AccountNumber = model.AccountNumber,

                    TransactionType = model.TransactionType,

                    Amount = model.Amount,

                    TransactionDate = DateTime.Now,

                    Description = model.Description,

                    Status = 1 // Pending

                };

                // ⭐ Only set recipient for transfer transactions

                if (model.TransactionType == 3 && !string.IsNullOrEmpty(model.RecipientAccountNumber))

                {

                    transactionDto.RecipientAccountNumber = model.RecipientAccountNumber;

                }

                var result = await _httpClient.PostAsync<Result<TransactionDto>>(ApiConstant.CreateTransaction, transactionDto);

                if (result.IsError)

                {

                    foreach (var error in result.Errors)

                    {

                        ModelState.AddModelError(string.Empty, error.ErrorMessage);

                    }

                    var userId = User.FindFirst("UserId")?.Value;

                    var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);

                    var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                    var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);

                    var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                    ViewBag.Accounts = accountsResult.IsError ? new List<AccountDto>() : accountsResult.Response;

                    return View(model);

                }

                TempData["SuccessMessage"] = "Transaction submitted successfully! Waiting for manager approval.";

                return RedirectToAction("Dashboard");

            }

            catch (Exception ex)

            {

                ModelState.AddModelError(string.Empty, "An error occurred while creating the transaction.");

                // ⭐ Reload accounts on error

                var userId = User.FindFirst("UserId")?.Value;

                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);

                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);

                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                ViewBag.Accounts = accountsResult.IsError ? new List<AccountDto>() : accountsResult.Response;

                return View(model);

            }

        }



        [HttpGet]
        public async Task<JsonResult> GetAccountDetails(int accountId)
        {
            try
            {
                var url = string.Format(ApiConstant.GetAccountById, accountId);
                var result = await _httpClient.GetAsync<Result<AccountDto>>(url);

                if (result.IsError || result.Response == null)
                {
                    return Json(new { success = false, message = "Account not found" });
                }

                return Json(new { success = true, account = result.Response });
            }
            catch
            {
                return Json(new { success = false, message = "Error fetching account details" });
            }
        }
    }

    }


