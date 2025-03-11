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

            // Only Admin, Staff, and Lecturer can access this page
            if (role != adminRole && role != staffRole && role != lecturerRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // For Lecturer, only show active news
            if (role == lecturerRole)
            {
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    var allNews = _newsArticleRepository.SearchNewsArticles(SearchTerm);
                    NewsArticle = allNews.Where(n => n.NewsStatus == true).ToList();
                }
                else
                {
                    var allNews = _newsArticleRepository.GetAllNewsArticles();
                    NewsArticle = allNews.Where(n => n.NewsStatus == true).ToList();
                }
            }
            else
            {
                // For Admin and Staff, show all news
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    NewsArticle = _newsArticleRepository.SearchNewsArticles(SearchTerm);
                }
                else
                {
                    NewsArticle = _newsArticleRepository.GetAllNewsArticles();
                }
            }

            return Page();
        }
    }
}
