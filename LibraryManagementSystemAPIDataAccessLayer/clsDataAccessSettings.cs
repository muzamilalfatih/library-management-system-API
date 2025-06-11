using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    internal class clsDataAccessSettings
    {
        static public string ConnectionString
        {
            get
            {
                return "Server=localhost;Database=LibraryManagementSystem;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";
            }
        }
    }
}
