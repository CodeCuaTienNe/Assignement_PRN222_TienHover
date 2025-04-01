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
    public class DetailsModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public DetailsModel(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public NewsArticle NewsArticle { get; set; } = default!;
        public List<Tag> ArticleTags { get; set; } = new List<Tag>();

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var newsArticle = _newsArticleRepository.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập
            var role = HttpContext.Session.GetInt32("AccountRole");
            
            // Nếu không đăng nhập hoặc là Lecturer, chỉ cho phép xem bài viết active
            if (role == null || role == 2) // 2 là Lecturer
            {
                if (newsArticle.NewsStatus != true)
                {
                    return RedirectToPage("/AccessDenied");
                }
            }
            
            NewsArticle = newsArticle;
            
            // Load tags for this article
            ArticleTags = _newsArticleRepository.GetTagsForArticle(id);
            
            return Page();
        }
    }
}
