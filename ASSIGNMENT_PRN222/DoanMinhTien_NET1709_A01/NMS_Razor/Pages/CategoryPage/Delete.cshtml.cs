using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.CategoryPage
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;

        public DeleteModel(ICategoryRepository categoryRepository, IConfiguration configuration)
        {
            _categoryRepository = categoryRepository;
            _configuration = configuration;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        
        public bool IsInUse { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet(short? id)
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

            // Only Staff can delete categories
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategoryById(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            
            Category = category;
            IsInUse = _categoryRepository.IsCategoryInUse(id.Value);
            
            return Page();
        }

        public IActionResult OnPost(short? id)
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

            // Only Staff can delete categories
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
                _categoryRepository.DeleteCategory(id.Value);
                SuccessMessage = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            
            return RedirectToPage("./Index");
        }
    }
}
