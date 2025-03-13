using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NMS_BusinessObjects;
namespace NMS_DAOs
{
    public class AccountDAO
    {
        private FunewsManagementContext _context;
        private static AccountDAO instance;

        public AccountDAO()
        {
            _context = new FunewsManagementContext();   
        }

        public static AccountDAO Instance() 
        {
            if (instance == null)
            {
                instance = new AccountDAO();
            }
            return instance;
        }
        
        public SystemAccount GetAccount(String email, String password)
        {
            return _context.SystemAccounts.FirstOrDefault(m => m.AccountEmail.Equals(email) && m.AccountPassword.Equals(password));
        }

        public List<SystemAccount> GetAccounts()
        {
            return _context.SystemAccounts.ToList();
        }

        public SystemAccount GetAccountById(short id)
        {
            return _context.SystemAccounts.FirstOrDefault(m => m.AccountId == id);
        }

        public SystemAccount GetAccountByEmail(string email)
        {
            return _context.SystemAccounts.FirstOrDefault(a => a.AccountEmail.Equals(email));
        }

        public void AddAccount(SystemAccount account)
        {
            try
            {
                var existingAccount = GetAccountByEmail(account.AccountEmail);
                if (existingAccount != null)
                {
                    throw new Exception($"Account with email '{account.AccountEmail}' is already exist");
                }

                _context.SystemAccounts.Add(account);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to create account: " + ex.Message);
            }
        }

        public void UpdateAccount(SystemAccount account)
        {
            try
            {
                var existingAccount = GetAccountById(account.AccountId);
                if (existingAccount == null)
                {
                    throw new Exception($"No ID for account {account.AccountId}");
                }

                if (account.AccountEmail != existingAccount.AccountEmail)
                {
                    var emailExists = GetAccountByEmail(account.AccountEmail);
                    if (emailExists != null)
                    {
                        throw new Exception($"Account with email '{account.AccountEmail}' is already exist");
                    }
                }

                existingAccount.AccountName = account.AccountName;
                existingAccount.AccountEmail = account.AccountEmail;
                existingAccount.AccountRole = account.AccountRole;

                if (!string.IsNullOrEmpty(account.AccountPassword))
                {
                    existingAccount.AccountPassword = account.AccountPassword;
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to update account: " + ex.Message);
            }
        }

        public void DeleteAccount(short id)
        {
            try
            {
                var account = GetAccountById(id);
                if (account == null)
                {
                    throw new Exception($"Không tìm thấy tài khoản với ID {id}");
                }

                // Kiểm tra xem tài khoản có liên quan đến bài viết tin tức không
                var hasRelatedNews = _context.NewsArticles.Any(n => n.CreatedById == id || n.UpdatedById == id);
                if (hasRelatedNews)
                {
                    throw new Exception("Không thể xóa tài khoản vì nó có liên quan đến các bài viết tin tức");
                }

                _context.SystemAccounts.Remove(account);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa tài khoản: " + ex.Message);
            }
        }
    }
}
