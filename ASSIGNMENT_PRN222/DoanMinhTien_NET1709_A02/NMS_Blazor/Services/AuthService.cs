using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NMS_BusinessObjects;
using NMS_Repositories;
using System.Text.Json;

namespace NMS_Blazor.Services
{
    public class AuthService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        
        // Session storage keys
        private const string USER_ID_KEY = "UserId";
        private const string USER_NAME_KEY = "UserName";
        private const string USER_ROLE_KEY = "UserRole";
        private const string IS_LOGGED_IN_KEY = "IsLoggedIn";
        
        // Event to notify components when authentication state changes
        public event EventHandler AuthenticationStateChanged;

        public AuthService(
            ProtectedSessionStorage sessionStorage,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _sessionStorage = sessionStorage;
            _accountRepository = accountRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResult> Login(string email, string password)
        {
            _logger.LogInformation($"Login attempt for: {email}");

            try
            {
                // Check for admin account credentials from appsettings.json
                string adminEmail = _configuration["AdminAccount:Email"];
                string adminPassword = _configuration["AdminAccount:Password"];
                string adminName = _configuration["AdminAccount:Name"] ?? "Administrator";
                
                // Parse admin role properly - could be string "3" or int 3
                int adminRole;
                if (!int.TryParse(_configuration["AdminRole"], out adminRole))
                {
                    adminRole = 3; // Default if parsing fails
                }

                _logger.LogDebug($"Admin config: Email={adminEmail}, Role={adminRole}");

                // Case-insensitive comparison for admin email
                if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword) &&
                    email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) &&
                    password.Equals(adminPassword))
                {
                    // Admin login successful
                    _logger.LogInformation($"Admin login successful, role: {adminRole}, name: {adminName}");
                    
                    // For admin user, there's no accountId in database
                    await StoreUserSession(null, adminName, adminRole);
                    
                    return new LoginResult
                    {
                        Success = true,
                        UserName = adminName,
                        UserRole = adminRole
                    };
                }

                // If not admin, check database
                var account = _accountRepository.GetAccount(email, password);
                
                if (account != null)
                {
                    _logger.LogInformation($"Database login successful for {email}, role: {account.AccountRole}");
                    await StoreUserSession(account.AccountId, account.AccountName, account.AccountRole.Value);
                    
                    return new LoginResult
                    {
                        Success = true,
                        UserName = account.AccountName,
                        UserRole = account.AccountRole.Value
                    };
                }

                _logger.LogWarning($"Login failed for {email}: Invalid credentials");
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = $"Login error: {ex.Message}"
                };
            }
        }

        private async Task StoreUserSession(short? userId, string userName, int userRole)
        {
            await _sessionStorage.SetAsync(IS_LOGGED_IN_KEY, true);
            
            if (userId.HasValue)
            {
                await _sessionStorage.SetAsync(USER_ID_KEY, userId.Value);
            }
            
            await _sessionStorage.SetAsync(USER_NAME_KEY, userName);
            await _sessionStorage.SetAsync(USER_ROLE_KEY, userRole);
            
            _logger.LogInformation($"User session stored - Name: {userName}, Role: {userRole}");
            
            // Notify components that authentication state has changed
            OnAuthenticationStateChanged();
        }

        public async Task Logout()
        {
            try
            {
                // Clear all session data
                await _sessionStorage.DeleteAsync(USER_ID_KEY);
                await _sessionStorage.DeleteAsync(USER_NAME_KEY);
                await _sessionStorage.DeleteAsync(USER_ROLE_KEY);
                await _sessionStorage.DeleteAsync(IS_LOGGED_IN_KEY);
                
                // Notify components about the change
                OnAuthenticationStateChanged();
                
                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }

        public async Task<UserInfo> GetCurrentUser()
        {
            try
            {
                var userInfo = new UserInfo();
                
                // Read login info from browser storage
                var isLoggedInResult = await _sessionStorage.GetAsync<bool>(IS_LOGGED_IN_KEY);
                userInfo.IsLoggedIn = isLoggedInResult.Success ? isLoggedInResult.Value : false;
                
                if (userInfo.IsLoggedIn)
                {
                    // Only get these values if user is logged in
                    var userIdResult = await _sessionStorage.GetAsync<short>(USER_ID_KEY);
                    var userNameResult = await _sessionStorage.GetAsync<string>(USER_NAME_KEY);
                    var userRoleResult = await _sessionStorage.GetAsync<int>(USER_ROLE_KEY);
                    
                    if (userIdResult.Success) userInfo.UserId = userIdResult.Value;
                    if (userNameResult.Success) userInfo.UserName = userNameResult.Value;
                    if (userRoleResult.Success) userInfo.UserRole = userRoleResult.Value;
                    
                    _logger.LogDebug($"Retrieved user info - Name: {userInfo.UserName}, Role: {userInfo.UserRole}");
                }
                
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user");
                // Return not logged in if there's an error
                return new UserInfo();
            }
        }

        public async Task RefreshUserSession()
        {
            var user = await GetCurrentUser();
            
            if (user.IsLoggedIn && user.UserId.HasValue)
            {
                var account = _accountRepository.GetAccountById(user.UserId.Value);
                if (account != null)
                {
                    // Update session data
                    await _sessionStorage.SetAsync("userId", account.AccountId);
                    await _sessionStorage.SetAsync("userName", account.AccountName);
                    await _sessionStorage.SetAsync("userEmail", account.AccountEmail);
                    await _sessionStorage.SetAsync("userRole", account.AccountRole);
                    
                    // Notify subscribers that authentication state has changed
                    OnAuthenticationStateChanged();
                }
            }
        }

        protected virtual void OnAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class UserInfo
    {
        public bool IsLoggedIn { get; set; }
        public short? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? UserRole { get; set; }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserRole { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
