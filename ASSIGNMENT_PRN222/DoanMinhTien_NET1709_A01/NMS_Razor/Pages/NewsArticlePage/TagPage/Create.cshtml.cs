using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage.Tags
{
    public class CreateModel : PageModel
    {
        private readonly ITagRepository _tagRepository;

        public CreateModel(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public IActionResult OnGet()
        {
            // Check login
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Check role
            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;

            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            return Page();
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            // Check login
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Check role
            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;

            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Ensure tag name is not empty
            if (string.IsNullOrWhiteSpace(Tag.TagName))
            {
                ModelState.AddModelError("Tag.TagName", "Tag name cannot be empty.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _tagRepository.AddTag(Tag);
                SuccessMessage = "Tag created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
