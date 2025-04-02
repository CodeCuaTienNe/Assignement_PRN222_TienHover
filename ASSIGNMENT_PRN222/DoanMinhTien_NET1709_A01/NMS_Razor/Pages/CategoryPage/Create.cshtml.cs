using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_Razor.Hubs;
using NMS_Repositories;

namespace NMS_Razor.Pages.CategoryPage
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<SignalRHub> _hubContext;

        public CreateModel(ICategoryRepository categoryRepository, IConfiguration configuration, IHubContext<SignalRHub> hubContext)
        {
            _categoryRepository = categoryRepository;
            _configuration = configuration;
            _hubContext = hubContext;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        [BindProperty]
        public bool IsActive { get; set; } = true;

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

            // Only Staff can access this page
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Set default active status
            Category = new Category();
            IsActive = true;
            
            // Populate parent categories dropdown
            ViewData["ParentCategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
            return Page();
        }

        public IActionResult OnPost()
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

            // Only Staff can add categories
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Remove validation for ParentCategoryId to allow null values
            ModelState.Remove("Category.ParentCategoryId");

            if (!ModelState.IsValid)
            {
                ViewData["ParentCategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }

            try
            {
                // Handle empty parent category - convert empty string to null
                if (Category.ParentCategoryId == 0)
                {
                    Category.ParentCategoryId = null;
                }

                // Assign the IsActive value
                Category.IsActive = IsActive;
                
                _categoryRepository.AddCategory(Category);
                _hubContext.Clients.All.SendAsync("Change");
                SuccessMessage = "Category created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                ViewData["ParentCategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}
