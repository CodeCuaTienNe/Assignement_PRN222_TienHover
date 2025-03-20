using Microsoft.Identity.Client;
using NMS_BusinessObjects;
using NMS_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public SystemAccount GetAccount(string email, string password)
            => AccountDAO.Instance().GetAccount(email, password);
            
        public SystemAccount GetAccountById(short id)
            => AccountDAO.Instance().GetAccountById(id);
            
        public List<SystemAccount> GetAccounts()
            => AccountDAO.Instance().GetAccounts();
            
        public void AddAccount(SystemAccount account)
            => AccountDAO.Instance().AddAccount(account);
            
        public void UpdateAccount(SystemAccount account)
            => AccountDAO.Instance().UpdateAccount(account);
            
        public void DeleteAccount(short id)
            => AccountDAO.Instance().DeleteAccount(id);
            
        public bool IsEmailExists(string email)
            => AccountDAO.Instance().GetAccountByEmail(email) != null;
    }
}
