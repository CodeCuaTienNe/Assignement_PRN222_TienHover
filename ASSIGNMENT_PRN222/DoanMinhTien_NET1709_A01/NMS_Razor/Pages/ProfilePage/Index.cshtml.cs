using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.ProfilePage
{
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;

        public IndexModel(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        [BindProperty]
        public ProfileViewModel ProfileVM { get; set; } = new ProfileViewModel();
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is logged in
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Get current user ID
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            // Get account details
            var account = _accountRepository.GetAccountById((short)accountId.Value);
            if (account == null)
            {
                return RedirectToPage("/Login");
            }

            // Map account data to view model
            ProfileVM.AccountName = account.AccountName;
            ProfileVM.AccountEmail = account.AccountEmail;
            
            string roleName = account.AccountRole switch
            {
                1 => "Staff",
                2 => "Lecturer",
                3 => "Admin",
                _ => "Unknown"
            };
            
            ProfileVM.RoleName = roleName;
            
            return Page();
        }

        public IActionResult OnPost()
        {
            // First check only the required name field and completely ignore password validation
            if (string.IsNullOrEmpty(ProfileVM.AccountName))
            {
                ModelState.AddModelError("ProfileVM.AccountName", "Name is required.");
                return Page();
            }

            // We'll manually handle the password match validation only
            if (!string.IsNullOrEmpty(ProfileVM.NewPassword) && ProfileVM.NewPassword != ProfileVM.ConfirmPassword)
            {
                ModelState.AddModelError("ProfileVM.ConfirmPassword", "The new password and confirmation password do not match.");
                return Page();
            }

            try
            {
                // Get current user ID
                var accountId = HttpContext.Session.GetInt32("AccountId");
                if (!accountId.HasValue)
                {
                    return RedirectToPage("/Login");
                }

                // Get account details
                var account = _accountRepository.GetAccountById((short)accountId.Value);
                if (account == null)
                {
                    return RedirectToPage("/Login");
                }

                // Update only allowed fields
                account.AccountName = ProfileVM.AccountName;
                
                // Only update password if provided
                if (!string.IsNullOrEmpty(ProfileVM.NewPassword))
                {
                    account.AccountPassword = ProfileVM.NewPassword;
                }

                // Save changes
                _accountRepository.UpdateAccount(account);
                
                // Update session data for name change
                HttpContext.Session.SetString("AccountName", account.AccountName);
                
                SuccessMessage = "Your profile has been updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating profile: {ex.Message}";
                return Page();
            }
        }
    }

    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string AccountName { get; set; }
        
        [Display(Name = "Email")]
        public string AccountEmail { get; set; }
        
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        
        // Remove all validation attributes
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        
        // Remove all validation attributes
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
