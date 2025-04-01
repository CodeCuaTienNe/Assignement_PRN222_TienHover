using NMS_BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_DAOs
{
    public class AccountDAO
    {
        private FunewsManagementContext _context;
        private static AccountDAO _instance;

        public AccountDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static AccountDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AccountDAO();
                }
                return _instance;
            }
        }

        public List<SystemAccount> GetAccounts()
        {
            return _context.SystemAccounts.ToList();
        }

        public SystemAccount GetAccountById(short id)
        {
            return _context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
        }

        public SystemAccount GetAccount(string email, string password)
        {
            return _context.SystemAccounts.FirstOrDefault(a => 
                a.AccountEmail.ToLower() == email.ToLower() && 
                a.AccountPassword == password);
        }

        public void AddAccount(SystemAccount account)
        {
            try
            {
                // Check if email already exists
                if (IsEmailExists(account.AccountEmail))
                {
                    throw new Exception($"Account with email '{account.AccountEmail}' already exists");
                }

                // Auto-generate ID by finding the max ID and adding 1
                short maxId = 0;
                if (_context.SystemAccounts.Any())
                {
                    maxId = _context.SystemAccounts.Max(a => a.AccountId);
                }
                account.AccountId = (short)(maxId + 1);

                _context.SystemAccounts.Add(account);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding account: " + ex.Message);
            }
        }

        public void UpdateAccount(SystemAccount account)
        {
            try
            {
                // Check if email is duplicated with another account (if email changed)
                if (!string.IsNullOrEmpty(account.AccountEmail))
                {
                    string emailLower = account.AccountEmail.ToLower();
                    var duplicateAccount = _context.SystemAccounts
                        .Where(a => a.AccountId != account.AccountId && a.AccountEmail.ToLower() == emailLower)
                        .FirstOrDefault();
                    
                    if (duplicateAccount != null)
                    {
                        throw new Exception($"Account with email '{account.AccountEmail}' already exists");
                    }
                }
                
                var existingAccount = _context.SystemAccounts.Find(account.AccountId);
                if (existingAccount != null)
                {
                    _context.Entry(existingAccount).CurrentValues.SetValues(account);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Account not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating account: " + ex.Message);
            }
        }

        public bool IsEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
                
            string emailLower = email.ToLower();
            return _context.SystemAccounts.Any(a => a.AccountEmail.ToLower() == emailLower);
        }

        public bool IsAccountInUse(short id)
        {
            // Check if account is used as author of any news articles
            bool hasArticles = _context.NewsArticles.Any(a => a.CreatedById == id || a.UpdatedById == id);
            
            return hasArticles;
        }

        public void DeleteAccount(short id)
        {
            try
            {
                // Check if the account is in use
                if (IsAccountInUse(id))
                {
                    throw new Exception("Cannot delete this account because it is associated with news articles");
                }

                var account = _context.SystemAccounts.Find(id);
                if (account != null)
                {
                    _context.SystemAccounts.Remove(account);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Account not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting account: " + ex.Message);
            }
        }
    }
}
