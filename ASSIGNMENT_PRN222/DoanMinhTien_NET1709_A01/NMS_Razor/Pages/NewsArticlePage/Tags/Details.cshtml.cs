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
    public class DetailsModel : PageModel
    {
        private readonly ITagRepository _tagRepository;

        public DetailsModel(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag Tag { get; set; }
        public bool IsInUse { get; set; }

        public IActionResult OnGet(int? id)
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

            if (id == null)
            {
                return NotFound();
            }

            var tag = _tagRepository.GetTagById(id.Value);
            if (tag == null)
            {
                return NotFound();
            }

            Tag = tag;
            IsInUse = _tagRepository.IsTagInUse(id.Value);
            
            return Page();
        }
    }
}
