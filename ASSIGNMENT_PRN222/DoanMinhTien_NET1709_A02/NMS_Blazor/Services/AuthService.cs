using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NMS_BusinessObjects;
using NMS_Repositories;
using System.Diagnostics;

namespace NMS_Blazor.Services
{
    public class AuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAccountRepository accountRepository, 
            ProtectedSessionStorage sessionStorage,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _accountRepository = accountRepository;
            _sessionStorage = sessionStorage;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool Success, string? ErrorMessage, int? UserRole)> Login(string email, string password)
        {
            try
            {
                // Check if it's the admin account from appsettings.json
                var adminEmail = _configuration.GetValue<string>("AdminAccount:Email");
                var adminPassword = _configuration.GetValue<string>("AdminAccount:Password");
                var adminRole = _configuration.GetValue<int>("AdminRole", 3);

                _logger.LogInformation($"Attempting login for: {email}");
                _logger.LogDebug($"Admin email from config: {adminEmail}");

                // Check if this is the admin account from configuration
                if (email == adminEmail && password == adminPassword)
                {
                    _logger.LogInformation("Admin credentials matched from configuration");
                    
                    // Store admin info in session
                    await _sessionStorage.SetAsync("UserId", (short)1); // Default admin ID
                    await _sessionStorage.SetAsync("UserEmail", adminEmail);
                    await _sessionStorage.SetAsync("UserRole", adminRole);
                    await _sessionStorage.SetAsync("UserName", _configuration.GetValue<string>("AdminAccount:Name", "Administrator"));
                    
                    return (true, null, adminRole);
                }

                // Otherwise check against database
                var user = _accountRepository.GetAccount(email, password);
                
                if (user == null)
                {
                    _logger.LogWarning($"Invalid login attempt for {email} - user not found in database");
                    return (false, "Invalid email or password", null);
                }

                _logger.LogInformation($"User found in database: {user.AccountName}, Role: {user.AccountRole}");

                // Store user info in session
                await _sessionStorage.SetAsync("UserId", user.AccountId);
                await _sessionStorage.SetAsync("UserEmail", user.AccountEmail);
                await _sessionStorage.SetAsync("UserRole", user.AccountRole);
                await _sessionStorage.SetAsync("UserName", user.AccountName);
                
                return (true, null, user.AccountRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed with exception");
                return (false, $"Login failed: {ex.Message}", null);
            }
        }

        public async Task Logout()
        {
            await _sessionStorage.DeleteAsync("UserId");
            await _sessionStorage.DeleteAsync("UserEmail");
            await _sessionStorage.DeleteAsync("UserRole");
        }

        public async Task<bool> IsUserAuthenticated()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<short>("UserId");
                return result.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int?> GetUserRole()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<int?>("UserRole");
                return result.Success ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool IsLoggedIn, short? UserId, string UserName, int? UserRole)> GetCurrentUser()
        {
            try
            {
                var userIdResult = await _sessionStorage.GetAsync<short>("UserId");
                var userNameResult = await _sessionStorage.GetAsync<string>("UserName");
                var userRoleResult = await _sessionStorage.GetAsync<int>("UserRole");
                
                bool isLoggedIn = userIdResult.Success;
                short? userId = userIdResult.Success ? userIdResult.Value : null;
                string userName = userNameResult.Success ? userNameResult.Value : string.Empty;
                int? userRole = userRoleResult.Success ? userRoleResult.Value : null;
                
                return (isLoggedIn, userId, userName, userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user information from session");
                return (false, null, string.Empty, null);
            }
        }
    }
}
