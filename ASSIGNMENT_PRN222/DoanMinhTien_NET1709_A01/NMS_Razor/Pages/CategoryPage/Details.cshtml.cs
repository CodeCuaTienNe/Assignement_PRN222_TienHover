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
    public class DetailsModel : PageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;

        public DetailsModel(ICategoryRepository categoryRepository, IConfiguration configuration)
        {
            _categoryRepository = categoryRepository;
            _configuration = configuration;
        }

        public Category Category { get; set; } = default!;

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

            // Only Staff can view category details
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
            return Page();
        }
    }
}
