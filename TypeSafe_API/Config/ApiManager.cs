using BilakLk_API.Models;
using System.Data.SqlClient;

namespace BilakLk_API.Config
{
    internal class ApiManager
    {
        private readonly string Environment = BilkEnvironment.Development.ToString();

        private ApiManager() { }
        private static ApiManager instance = null;
        internal static ApiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiManager();
                }
                return instance;
            }
        }

        internal SqlConnectionStringBuilder GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            if (Environment == BilkEnvironment.Development.ToString())
            {
                builder.DataSource = "localhost";
                builder.InitialCatalog = "TypeSafe";
                builder.IntegratedSecurity = true; // This enables Windows Authentication
            }
            else if (Environment == BilkEnvironment.UAT.ToString())
            {
                builder.DataSource = "";
                builder.InitialCatalog = "";
                builder.UserID = "";
                builder.Password = "";
            }
            else if (Environment == BilkEnvironment.Live.ToString())
            {
                builder.DataSource = "";
                builder.InitialCatalog = "";
                builder.UserID = "";
                builder.Password = "";
            }
            return builder;
        }

        internal string GetEnvironment()
        { return Environment; }
    }
}
