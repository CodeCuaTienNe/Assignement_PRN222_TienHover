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
    }
}
