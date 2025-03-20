using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Razor.Pages.ReportPage
{
    public class IndexModel : PageModel
    {
        private readonly IReportRepository _reportRepository;
        private readonly IConfiguration _configuration;

        public IndexModel(IReportRepository reportRepository, IConfiguration configuration)
        {
            _reportRepository = reportRepository;
            _configuration = configuration;
        }

        [BindProperty]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);

        [BindProperty]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public List<NewsArticle> Articles { get; set; }
        public string CategoryChartData { get; set; }
        public string AuthorChartData { get; set; }
        public string StatusChartData { get; set; }
        public string DailyChartData { get; set; }
        
        public int TotalArticles { get; set; }
        public bool IsPostback { get; set; } = false;

        public IActionResult OnGet()
        {
            // Check login
            var email = HttpContext.Session.GetString("AccountEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Check role - only Admin can view reports
            var role = HttpContext.Session.GetInt32("AccountRole");
            var adminRole = int.Parse(_configuration["AdminRole"] ?? "3");

            if (role != adminRole)
            {
                return RedirectToPage("/AccessDenied");
            }
            
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

            // Check role - only Admin can view reports
            var role = HttpContext.Session.GetInt32("AccountRole");
            var adminRole = int.Parse(_configuration["AdminRole"] ?? "3");

            if (role != adminRole)
            {
                return RedirectToPage("/AccessDenied");
            }

            // Validate dates
            if (EndDate < StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
                return Page();
            }

            // Mark as postback to show the report section in the view
            IsPostback = true;

            // Get report data for the date range
            Articles = _reportRepository.GetArticlesByDateRange(StartDate, EndDate);
            TotalArticles = Articles.Count;

            // Generate chart data
            var categoryData = _reportRepository.GetArticleCountByCategory(StartDate, EndDate);
            var authorData = _reportRepository.GetArticleCountByAuthor(StartDate, EndDate);
            var statusData = _reportRepository.GetArticleCountByStatus(StartDate, EndDate);
            var dailyData = _reportRepository.GetArticleCountByDay(StartDate, EndDate);

            // Convert the data to JSON for Chart.js using built-in System.Text.Json
            CategoryChartData = JsonSerializer.Serialize(categoryData);
            AuthorChartData = JsonSerializer.Serialize(authorData);
            StatusChartData = JsonSerializer.Serialize(statusData);
            DailyChartData = JsonSerializer.Serialize(dailyData);
            
            return Page();
        }
    }
}
