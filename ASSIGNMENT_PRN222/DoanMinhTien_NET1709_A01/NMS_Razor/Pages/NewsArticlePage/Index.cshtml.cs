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

namespace NMS_Razor.Pages.NewsArticlePage
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public IndexModel(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }
            NewsArticle = _newsArticleRepository.GetNewsArticles();
            return Page();
        }
    }
}
