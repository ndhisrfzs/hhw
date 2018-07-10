using System.Data;
using System.Data.SqlClient;

namespace GN
{
    public class DBComponent : Component
    {
        public IDbConnection dbConnection;

        public void Awake(string connectionString)
        {
            dbConnection = new SqlConnection(connectionString);
        }
    }
}
