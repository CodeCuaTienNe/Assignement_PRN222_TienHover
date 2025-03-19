using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMS_BusinessObjects;
using NMS_DAOs;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage
{
    public class EditModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ICategoryRepository _categoryRepository;

        public EditModel(INewsArticleRepository newsArticleRepository, ICategoryRepository categoryRepository)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet(string id)
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
            var currentUserId = HttpContext.Session.GetInt32("AccountId");

            // Only Staff can edit
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var newsArticle = _newsArticleRepository.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            // Check if the user is the creator of this article
            if (newsArticle.CreatedById != currentUserId)
            {
                return RedirectToPage("/AccessDenied");
            }

            NewsArticle = newsArticle;
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
            return Page();
        }

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
            var currentUserId = HttpContext.Session.GetInt32("AccountId");

            // Only Staff can edit
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }

            // Check if the user is the creator of this article
            var existingArticle = _newsArticleRepository.GetNewsArticleById(NewsArticle.NewsArticleId);
            if (existingArticle == null)
            {
                return NotFound();
            }
            
            if (existingArticle.CreatedById != currentUserId)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Update fields we want to keep from the existing record
            NewsArticle.CreatedById = existingArticle.CreatedById;
            NewsArticle.CreatedDate = existingArticle.CreatedDate;
            
            // Set updated info
            NewsArticle.UpdatedById = (short?)currentUserId;
            NewsArticle.ModifiedDate = DateTime.Now;

            try
            {
                _newsArticleRepository.UpdateNewsArticle(NewsArticle);
                SuccessMessage = "Article updated successfully!";
                
                // If we came from MyNews page (via query parameter), return there
                if (Request.Query.ContainsKey("returnToMyNews"))
                {
                    return RedirectToPage("./MyNews");
                }
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update article: {ex.Message}";
                ModelState.AddModelError("", ErrorMessage);
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}
