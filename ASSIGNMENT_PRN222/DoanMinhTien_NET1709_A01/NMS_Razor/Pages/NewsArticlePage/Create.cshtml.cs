using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_DAOs;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage
{
    public class CreateModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;

        public CreateModel(INewsArticleRepository newsArticleRepository, ICategoryRepository categoryRepository, IConfiguration configuration)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
            _configuration = configuration;
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

            // Only Staff can create news
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Get categories for dropdown
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
            
            // Get current user ID from session
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId.HasValue)
            {
                // Set default values
                NewsArticle = new NewsArticle
                {
                    CreatedDate = DateTime.Now,
                    CreatedById = (short)accountId.Value,  // Explicit cast to short
                    NewsStatus = true  // Luôn đặt trạng thái là active
                };
            }

            return Page();
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost()
        {
            // Check login and role again
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;

            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Kiểm tra NewsArticleId đã được nhập
            if (string.IsNullOrEmpty(NewsArticle.NewsArticleId))
            {
                ModelState.AddModelError("NewsArticle.NewsArticleId", "Mã bài viết không được để trống");
            }

            if (!ModelState.IsValid)
            {
                // Re-populate dropdown lists if validation fails
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }

            // Set creation date and user if not already set
            if (NewsArticle.CreatedDate == DateTime.MinValue)
            {
                NewsArticle.CreatedDate = DateTime.Now;
            }

            if (NewsArticle.CreatedById == 0)
            {
                var accountId = HttpContext.Session.GetInt32("AccountId");
                if (accountId.HasValue)
                {
                    NewsArticle.CreatedById = (short)accountId.Value;  // Explicit cast to short
                }
            }
            
            // Đảm bảo trạng thái luôn là active
            NewsArticle.NewsStatus = true;

            try
            {
                _newsArticleRepository.AddNewsArticle(NewsArticle);
                SuccessMessage = "Bài viết đã được tạo thành công!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi khi tạo bài viết: " + ex.Message;
                ModelState.AddModelError("", ErrorMessage);
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}
