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
            try
            {
                var userId = await GetUserId();
                if (!userId.HasValue)
                {
                    return; // Not logged in
                }

                // Fetch the current user data from the repository
                var account = _accountRepository.GetAccountById(userId.Value);
                if (account == null)
                {
                    return; // Account not found
                }

                // Update session using the same keys that are already defined in this service
                await _sessionStorage.SetAsync(USER_ID_KEY, account.AccountId);
                await _sessionStorage.SetAsync(USER_NAME_KEY, account.AccountName);
                await _sessionStorage.SetAsync(USER_ROLE_KEY, account.AccountRole);

                // Notify UI components that auth state has changed
                OnAuthenticationStateChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing user session");
            }
        }

        private async Task<short?> GetUserId()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<short>(USER_ID_KEY);
                if (result.Success)
                {
                    return result.Value;
                }
                return null;
            }
            catch
            {
                return null;
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
