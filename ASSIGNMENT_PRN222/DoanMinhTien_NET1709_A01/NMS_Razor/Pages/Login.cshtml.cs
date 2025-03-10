using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NMS_Repositories;

namespace NMS_Razor.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string email { get; set; }
        [BindProperty]
        public string password { get; set; }

        private readonly IAccountRepository _accountRepository;
        public LoginModel(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }


        public IActionResult OnPost()
        {
            try
            {
                var account = _accountRepository.GetAccount(email, password);
                if (account != null)
                {
                    return RedirectToPage("/Index");
                }
                ModelState.AddModelError("", "Loi roi ko cho vo");
                return Page();
            }
            catch (Exception ex)
            {
                return Page();
            }
            

        }
    }
}