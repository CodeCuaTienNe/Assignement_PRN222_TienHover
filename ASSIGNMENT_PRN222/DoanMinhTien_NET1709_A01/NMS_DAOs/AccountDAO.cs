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
    }

}
