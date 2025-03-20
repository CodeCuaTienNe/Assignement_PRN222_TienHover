using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public interface IAccountRepository
    {
        SystemAccount GetAccount(string email, string password);
        SystemAccount GetAccountById(short id);
        List<SystemAccount> GetAccounts();
        void AddAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(short id);
        bool IsEmailExists(string email);
    }
}
