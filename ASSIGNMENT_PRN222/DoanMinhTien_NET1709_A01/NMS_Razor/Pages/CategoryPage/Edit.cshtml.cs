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
    public class EditModel : PageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<SignalRHub> _hubContext;

        public EditModel(ICategoryRepository categoryRepository, IConfiguration configuration, IHubContext<SignalRHub> hubContext)
        {
            _categoryRepository = categoryRepository;
            _configuration = configuration;
            _hubContext = hubContext;   
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        
        [BindProperty]
        public bool IsActive { get; set; }

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

            // Only Staff can edit categories
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
            // Set IsActive from the category
            IsActive = category.IsActive == true;
            
            // Make sure category cannot be its own parent
            var categories = _categoryRepository.GetAllCategories()
                .Where(c => c.CategoryId != id.Value)
                .ToList();
                
            ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
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

            // Only Staff can edit categories
            if (role != staffRole)
            {
                return RedirectToPage("/AccessDenied");
            }
            
            // Prevent cyclic reference - category cannot be its own parent
            if (Category.ParentCategoryId == Category.CategoryId)
            {
                ModelState.AddModelError("Category.ParentCategoryId", "A category cannot be its own parent.");
            }

            if (!ModelState.IsValid)
            {
                var categories = _categoryRepository.GetAllCategories()
                    .Where(c => c.CategoryId != Category.CategoryId)
                    .ToList();
                
                ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                return Page();
            }

            try
            {
                // Assign the IsActive value
                Category.IsActive = IsActive;
                
                _categoryRepository.UpdateCategory(Category);
                _hubContext.Clients.All.SendAsync("Change");
                SuccessMessage = "Category updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var categories = _categoryRepository.GetAllCategories()
                    .Where(c => c.CategoryId != Category.CategoryId)
                    .ToList();
                
                ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}
