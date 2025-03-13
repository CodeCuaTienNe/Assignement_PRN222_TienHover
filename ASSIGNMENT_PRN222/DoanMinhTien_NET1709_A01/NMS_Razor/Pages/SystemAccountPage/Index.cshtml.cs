using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NMS_BusinessObjects;
using NMS_DAOs;
using NMS_Repositories;

namespace NMS_Razor.Pages.SystemAccountPage
{
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;

        public IndexModel(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IList<SystemAccount> SystemAccount { get;set; } = default!;

        public void OnGet()
        {
            SystemAccount = _accountRepository.GetAccounts();
        }
    }
}
