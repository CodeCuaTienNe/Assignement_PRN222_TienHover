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
                
                // Check if it's the admin account from appsettings.json
                string adminEmail = _configuration["AdminAccount:Email"];
                string adminPassword = _configuration["AdminAccount:Password"];
                string adminName = _configuration["AdminAccount:Name"];
                int adminRole = int.Parse(_configuration["AdminRole"] ?? "3");
                
                if (email == adminEmail && password == adminPassword)
                {
                    // Store admin information in Session
                    HttpContext.Session.SetInt32("AccountId", 0); // Admin ID is set to 0
                    HttpContext.Session.SetString("AccountName", adminName ?? "Administrator");
                    HttpContext.Session.SetString("AccountEmail", adminEmail);
                    HttpContext.Session.SetInt32("AccountRole", adminRole);
                    HttpContext.Session.SetString("RoleName", "Administrator");
                    
                    // Redirect admin to SystemAccountPage
                    return RedirectToPage("/SystemAccountPage/Index");
                }
                
                // Check regular accounts from database
                var account = _accountRepository.GetAccount(email, password);
                if (account != null)
                {
                    // Store user information in Session
                    HttpContext.Session.SetInt32("AccountId", account.AccountId);
                    HttpContext.Session.SetString("AccountName", account.AccountName ?? string.Empty);
                    HttpContext.Session.SetString("AccountEmail", account.AccountEmail ?? string.Empty);
                    HttpContext.Session.SetInt32("AccountRole", account.AccountRole ?? 0);
                    
                    // Check role
                    int staffRole = 1;
                    int lecturerRole = 2;
                    
                    if (account.AccountRole == adminRole)
                    {
                        HttpContext.Session.SetString("RoleName", "Administrator");
                        return RedirectToPage("/SystemAccountPage/Index");
                    }
                    else if (account.AccountRole == staffRole)
                    {
                        HttpContext.Session.SetString("RoleName", "Staff");
                        return RedirectToPage("/NewsArticlePage/Index");
                    }
                    else if (account.AccountRole == lecturerRole)
                    {
                        HttpContext.Session.SetString("RoleName", "Lecturer");
                        return RedirectToPage("/NewsArticlePage/Index");
                    }
                    
                    return RedirectToPage("/NewsArticlePage/Index");
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