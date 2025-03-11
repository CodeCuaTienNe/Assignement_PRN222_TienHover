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
    public class DeleteModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public DeleteModel(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
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

            // Chỉ Staff mới có quyền xóa
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
            return Page();
        }

        public IActionResult OnPost(string id)
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

            // Chỉ Staff mới có quyền xóa
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

            try
            {
                _newsArticleRepository.DeleteNewsArticle(id);
                SuccessMessage = "Bài viết đã được xóa thành công!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi khi xóa bài viết: " + ex.Message;
                ModelState.AddModelError("", ErrorMessage);
                NewsArticle = newsArticle;
                return Page();
            }
        }
    }
}
