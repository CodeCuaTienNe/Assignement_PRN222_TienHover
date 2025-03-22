using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage.TagPage
{
    public class IndexModel : PageModel
    {
        private readonly ITagRepository _tagRepository;
        private readonly IConfiguration _configuration;

        public IndexModel(ITagRepository tagRepository, IConfiguration configuration)
        {
            _tagRepository = tagRepository;
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public IList<Tag> Tags { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // Check login
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Check role - only Staff can manage tags
            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;

            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Get tags list
            Tags = _tagRepository.GetAllTags();

            // Apply search if provided
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Tags = Tags.Where(t => 
                    t.TagName != null && t.TagName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Note != null && t.Note.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Page();
        }
    }
}
