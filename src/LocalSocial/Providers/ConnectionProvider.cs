using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Providers
{
    public class ConnectionProvider
    {
        public string GetConnectionString()
        {
            return "Server=tcp:poznan.database.windows.net,1433;Initial Catalog=LocalSocial;Persist Security Info=False;User ID=poznan;Password=Kaczka1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }
    }
}
