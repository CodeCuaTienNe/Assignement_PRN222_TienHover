using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NMS_BusinessObjects;
using NMS_Repositories;

namespace NMS_Blazor.Services
{
    public class AuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly IConfiguration _configuration;

        public AuthService(IAccountRepository accountRepository, 
                          ProtectedSessionStorage sessionStorage,
                          IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _sessionStorage = sessionStorage;
            _configuration = configuration;
        }

        public async Task<(bool Success, string? ErrorMessage, int? UserRole)> Login(string email, string password)
        {
            try
            {
                var user = _accountRepository.GetAccount(email, password);
                
                if (user == null)
                {
                    return (false, "Invalid email or password", null);
                }

                // Store user info in session
                await _sessionStorage.SetAsync("UserId", user.AccountId);
                await _sessionStorage.SetAsync("UserEmail", user.AccountEmail);
                await _sessionStorage.SetAsync("UserRole", user.AccountRole);
                
                return (true, null, user.AccountRole);
            }
            catch (Exception ex)
            {
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
    }
}
