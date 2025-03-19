using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage
{
    public class MyNewsModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public MyNewsModel(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        
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

            // Check role
            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;
            var currentUserId = HttpContext.Session.GetInt32("AccountId");

            // Only Staff can access this page
            if (role != staffRole || !currentUserId.HasValue)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Get articles based on search term or get all by author
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var allArticles = _newsArticleRepository.SearchNewsArticles(SearchTerm);
                NewsArticle = allArticles.Where(a => a.CreatedById == currentUserId).ToList();
            }
            else
            {
                // Fix the error by explicitly casting int? to short
                short authorId = (short)currentUserId.Value;
                NewsArticle = _newsArticleRepository.GetNewsArticlesByAuthor(authorId);
            }

            return Page();
        }
    }
}
