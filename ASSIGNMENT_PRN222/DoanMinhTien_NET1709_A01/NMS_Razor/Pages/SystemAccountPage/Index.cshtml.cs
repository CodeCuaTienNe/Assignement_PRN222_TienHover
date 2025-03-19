using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_DAOs;
using NMS_Repositories;

namespace NMS_Razor.Pages.SystemAccountPage
{
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public IndexModel(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        public IList<SystemAccount> SystemAccount { get;set; } = default!;
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is logged in
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Check if user has admin role
            var role = HttpContext.Session.GetInt32("AccountRole");
            var adminRole = int.Parse(_configuration["AdminRole"] ?? "3");

            if (role != adminRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            SystemAccount = _accountRepository.GetAccounts();
            return Page();
        }
    }
}
