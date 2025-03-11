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
            // Kiểm tra đăng nhập
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Kiểm tra quyền
            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;
            var currentUserId = HttpContext.Session.GetInt32("AccountId");

            // Chỉ Staff mới có quyền chỉnh sửa
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

            // Kiểm tra xem người dùng hiện tại có phải là người tạo bài viết không
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
            // Kiểm tra đăng nhập
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Kiểm tra quyền
            var role = HttpContext.Session.GetInt32("AccountRole");
            var staffRole = 1;
            var currentUserId = HttpContext.Session.GetInt32("AccountId");

            // Chỉ Staff mới có quyền chỉnh sửa
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }

            // Kiểm tra xem người dùng hiện tại có phải là người tạo bài viết không
            var existingArticle = _newsArticleRepository.GetNewsArticleById(NewsArticle.NewsArticleId);
            if (existingArticle == null || existingArticle.CreatedById != currentUserId)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Cập nhật thông tin người chỉnh sửa và thời gian
            NewsArticle.UpdatedById = (short?)currentUserId;
            NewsArticle.ModifiedDate = DateTime.Now;

            try
            {
                _newsArticleRepository.UpdateNewsArticle(NewsArticle);
                SuccessMessage = "Bài viết đã được cập nhật thành công!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi khi cập nhật bài viết: " + ex.Message;
                ModelState.AddModelError("", ErrorMessage);
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}
