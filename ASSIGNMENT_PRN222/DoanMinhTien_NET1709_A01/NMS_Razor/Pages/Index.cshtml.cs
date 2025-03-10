using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NMS_Razor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRole { get; set; }
        public bool IsLoggedIn { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Get user information from session
            UserName = HttpContext.Session.GetString("AccountName");
            UserEmail = HttpContext.Session.GetString("AccountEmail");
            UserRole = HttpContext.Session.GetString("RoleName");
            IsLoggedIn = !string.IsNullOrEmpty(UserEmail);
        }
    }
}
