using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage.TagPage
{
    public class DeleteModel : PageModel
    {
        private readonly ITagRepository _tagRepository;

        public DeleteModel(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;
        
        public bool IsInUse { get; set; }
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

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

        public IActionResult OnPost(int? id)
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

            try
            {
                _tagRepository.DeleteTag(id.Value);
                SuccessMessage = "Tag deleted successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                // Re-fetch the tag data
                Tag = _tagRepository.GetTagById(id.Value);
                IsInUse = _tagRepository.IsTagInUse(id.Value);
                return Page();
            }
        }
    }
}
