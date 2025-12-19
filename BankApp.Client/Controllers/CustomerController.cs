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

                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult.IsError || customerResult.Response == null)
                {
                    TempData["ErrorMessage"] = "Unable to fetch customer details.";
                    return View(new DashboardViewModel());
                }

                var customer = customerResult.Response;

                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customer.CustomerID);
                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                var accounts = accountsResult.IsError ? new List<AccountDto>() : accountsResult.Response;

                var recentTransactions = new List<TransactionDto>();
                if (accounts.Any())
                {
                    // Get ALL transactions across ALL customer accounts
                    var allTransactions = new List<TransactionDto>();

                    foreach (var account in accounts)
                    {
                        var transactionsUrl = string.Format(ApiConstant.GetTransactionsByAccountId, account.AccountID);
                        var transactionsResult = await _httpClient.GetAsync<Result<List<TransactionDto>>>(transactionsUrl);

                        if (!transactionsResult.IsError && transactionsResult.Response != null)
                        {
                            allTransactions.AddRange(transactionsResult.Response);
                        }
                    }

                    // Take top 10 most recent from ALL accounts in last 30 days
                    recentTransactions = allTransactions
                        .Where(t => t.TransactionDate >= DateTime.Now.AddDays(-30))  // Last 30 days
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(10)
                        .ToList();
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
            catch (Exception)
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

                    Status = 1

                };

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

            catch (Exception)

            {

                ModelState.AddModelError(string.Empty, "An error occurred while creating the transaction.");

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
        public async Task<IActionResult> CreateAccount()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult == null || customerResult.IsError || customerResult.Response == null)
                {
                    TempData["ErrorMessage"] = "Failed to load customer data.";
                    return RedirectToAction("Dashboard");
                }

                var customer = customerResult.Response;

                var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customer.CustomerID);
                var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                var existingAccounts = accountsResult != null && !accountsResult.IsError && accountsResult.Response != null
                    ? accountsResult.Response
                    : new List<AccountDto>();

                var existingAccountTypeIds = existingAccounts.Select(a => a.AccountTypeID).ToList();

                var allAccountTypes = await _httpClient.GetAsync<List<AccountTypeDto>>(ApiConstant.GetAccountTypes) ?? new List<AccountTypeDto>();

                var availableAccountTypes = allAccountTypes
                    .Where(at => !existingAccountTypeIds.Contains(at.AccountTypeID))
                    .ToList();

                if (!availableAccountTypes.Any())
                {
                    TempData["InfoMessage"] = "You already have all available account types (Savings, Current, and Fixed Deposit).";
                    return RedirectToAction("Dashboard");
                }

                var viewModel = new CreateAccountViewModel
                {
                    CustomerID = customer.CustomerID,
                    AvailableAccountTypes = availableAccountTypes
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirst("UserId")?.Value;
                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult != null && !customerResult.IsError && customerResult.Response != null)
                {
                    var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);
                    var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                    var existingAccounts = accountsResult?.Response ?? new List<AccountDto>();
                    var existingAccountTypeIds = existingAccounts.Select(a => a.AccountTypeID).ToList();

                    var allAccountTypes = await _httpClient.GetAsync<List<AccountTypeDto>>(ApiConstant.GetAccountTypes) ?? new List<AccountTypeDto>();


                    model.AvailableAccountTypes = allAccountTypes
                        .Where(at => !existingAccountTypeIds.Contains(at.AccountTypeID))
                        .ToList();
                }

                return View(model);
            }

            try
            {
                var accountDto = new CreateAccountDto
                {
                    CustomerID = model.CustomerID,
                    AccountTypeID = model.AccountTypeID
                };

                var result = await _httpClient.PostAsync<Result<AccountDto>>(ApiConstant.CreateAccount, accountDto);

                if (result.IsError)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }

                    var userId = User.FindFirst("UserId")?.Value;
                    var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                    var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                    if (customerResult != null && !customerResult.IsError && customerResult.Response != null)
                    {
                        var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);
                        var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                        var existingAccounts = accountsResult?.Response ?? new List<AccountDto>();
                        var existingAccountTypeIds = existingAccounts.Select(a => a.AccountTypeID).ToList();

                        var allAccountTypes = await _httpClient.GetAsync<List<AccountTypeDto>>(ApiConstant.GetAccountTypes) ?? new List<AccountTypeDto>();


                        model.AvailableAccountTypes = allAccountTypes
                            .Where(at => !existingAccountTypeIds.Contains(at.AccountTypeID))
                            .ToList();
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] = $"Account created successfully! Account Number: {result.Response.AccountNumber}";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");

                var userId = User.FindFirst("UserId")?.Value;
                var customerUrl = string.Format(ApiConstant.GetCustomerByUserId, userId);
                var customerResult = await _httpClient.GetAsync<Result<CustomerDto>>(customerUrl);

                if (customerResult != null && !customerResult.IsError && customerResult.Response != null)
                {
                    var accountsUrl = string.Format(ApiConstant.GetAccountsByCustomerId, customerResult.Response.CustomerID);
                    var accountsResult = await _httpClient.GetAsync<Result<List<AccountDto>>>(accountsUrl);

                    var existingAccounts = accountsResult?.Response ?? new List<AccountDto>();
                    var existingAccountTypeIds = existingAccounts.Select(a => a.AccountTypeID).ToList();

                    var allAccountTypes = await _httpClient.GetAsync<List<AccountTypeDto>>(ApiConstant.GetAccountTypes) ?? new List<AccountTypeDto>();


                    model.AvailableAccountTypes = allAccountTypes
                        .Where(at => !existingAccountTypeIds.Contains(at.AccountTypeID))
                        .ToList();
                }

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


