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
        public SystemAccount GetAccount(String email, String password);

    }
}
