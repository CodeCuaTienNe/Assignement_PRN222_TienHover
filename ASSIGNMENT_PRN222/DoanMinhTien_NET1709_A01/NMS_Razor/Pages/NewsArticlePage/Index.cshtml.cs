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
using Microsoft.Extensions.Configuration;

namespace NMS_Razor.Pages.NewsArticlePage
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly IConfiguration _configuration;

        public IndexModel(INewsArticleRepository newsArticleRepository, IConfiguration configuration)
        {
            _newsArticleRepository = newsArticleRepository;
            _configuration = configuration;
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        // Flag to indicate if we should show staff's own articles only
        [BindProperty(SupportsGet = true)]
        public bool MyArticlesOnly { get; set; }

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
            var adminRole = int.Parse(_configuration["AdminRole"] ?? "3");
            var staffRole = 1;
            var lecturerRole = 2;
            var currentUserId = HttpContext.Session.GetInt32("AccountId");

            // Only Admin, Staff, and Lecturer can access this page
            if (role != adminRole && role != staffRole && role != lecturerRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            List<NewsArticle> articles;

            // First get all articles or search results
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                articles = _newsArticleRepository.SearchNewsArticles(SearchTerm);
            }
            else
            {
                articles = _newsArticleRepository.GetAllNewsArticles();
            }

            // Then apply filters based on role and options
            if (role == lecturerRole)
            {
                // Lecturers can only see active articles
                NewsArticle = articles.Where(n => n.NewsStatus == true).ToList();
            }
            else if (role == staffRole && MyArticlesOnly)
            {
                // If staff selected "My Articles Only"
                NewsArticle = articles.Where(n => n.CreatedById == currentUserId).ToList();
            }
            else
            {
                // Admin or Staff viewing all articles
                NewsArticle = articles;
            }

            return Page();
        }
    }
}
