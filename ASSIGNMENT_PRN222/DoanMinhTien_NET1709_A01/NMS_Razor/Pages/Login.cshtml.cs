using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string? email { get; set; }
        [BindProperty]
        public string? password { get; set; }
        
        public string? ErrorMessage { get; set; }

        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        
        public LoginModel(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        public IActionResult OnPost()
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ErrorMessage = "Email and password are required!";
                    return Page();
                }
                
                var account = _accountRepository.GetAccount(email, password);
                if (account != null)
                {
                    // Store user information in Session
                    HttpContext.Session.SetInt32("AccountId", account.AccountId);
                    HttpContext.Session.SetString("AccountName", account.AccountName ?? string.Empty);
                    HttpContext.Session.SetString("AccountEmail", account.AccountEmail ?? string.Empty);
                    HttpContext.Session.SetInt32("AccountRole", account.AccountRole ?? 0);
                    
                    // Check role
                    int adminRole = int.Parse(_configuration["AdminRole"] ?? "0");
                    int staffRole = 1;
                    int lecturerRole = 2;
                    
                    if (account.AccountRole == adminRole)
                    {
                        HttpContext.Session.SetString("RoleName", "Administrator");
                    }
                    else if (account.AccountRole == staffRole)
                    {
                        HttpContext.Session.SetString("RoleName", "Staff");
                    }
                    else if (account.AccountRole == lecturerRole)
                    {
                        HttpContext.Session.SetString("RoleName", "Lecturer");
                    }
                    
                    return RedirectToPage("/Index");
                }
                
                ErrorMessage = "Invalid email or password!";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred: " + ex.Message;
                return Page();
            }
        }
    }
}