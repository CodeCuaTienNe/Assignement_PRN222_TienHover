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
using NMS_DAOs;
using NMS_Razor.Hubs;
using NMS_Repositories;

namespace NMS_Razor.Pages.NewsArticlePage
{
    public class CreateModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<SignalRHub> _hubContext;

        public CreateModel(INewsArticleRepository newsArticleRepository, ICategoryRepository categoryRepository, 
                          ITagRepository tagRepository, IConfiguration configuration, IHubContext<SignalRHub> hubContext)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _configuration = configuration;
            _hubContext = hubContext;   
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
            
            // Get tags for checkboxes
            AvailableTags = _tagRepository.GetAllTags();
            
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
        
        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        
        public List<Tag> AvailableTags { get; set; } = new List<Tag>();
        
        [TempData]
        public string SuccessMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

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

            // Remove validation of NewsArticleId since it will be auto-generated
            ModelState.Remove("NewsArticle.NewsArticleId");
            
            if (!ModelState.IsValid)
            {
                // Re-populate dropdown lists if validation fails
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                AvailableTags = _tagRepository.GetAllTags();
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
            
            // Ensure status is active
            NewsArticle.NewsStatus = true;

            try
            {
                // ID will be auto-generated in the DAO
                _newsArticleRepository.AddNewsArticle(NewsArticle);
                
                // Add tags if any are selected
                if (SelectedTagIds != null && SelectedTagIds.Any())
                {
                    _newsArticleRepository.AddTagsToArticle(NewsArticle.NewsArticleId, SelectedTagIds);
                }
                _hubContext.Clients.All.SendAsync("Change");
                SuccessMessage = "Article created successfully!";
             
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error creating article: " + ex.Message;
                ModelState.AddModelError("", ErrorMessage);
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetAllCategories(), "CategoryId", "CategoryName");
                AvailableTags = _tagRepository.GetAllTags();
                return Page();
            }
        }
    }
}
