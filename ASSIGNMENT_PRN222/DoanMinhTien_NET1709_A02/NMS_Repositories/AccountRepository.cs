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
        public List<SystemAccount> GetAccounts()
        {
            return AccountDAO.Instance.GetAccounts();
        }

        public SystemAccount GetAccountById(short id)
        {
            return AccountDAO.Instance.GetAccountById(id);
        }

        public SystemAccount GetAccount(string email, string password)
        {
            return AccountDAO.Instance.GetAccount(email, password);
        }

        public void AddAccount(SystemAccount account)
        {
            AccountDAO.Instance.AddAccount(account);
        }

        public void UpdateAccount(SystemAccount account)
        {
            AccountDAO.Instance.UpdateAccount(account);
        }

        public void DeleteAccount(short id)
        {
            AccountDAO.Instance.DeleteAccount(id);
        }

        public bool IsEmailExists(string email)
        {
            return AccountDAO.Instance.IsEmailExists(email);
        }

        public bool IsAccountInUse(short id)
        {
            return AccountDAO.Instance.IsAccountInUse(id);
        }
    }
}
