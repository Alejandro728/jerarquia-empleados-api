namespace JerarquiaEmpleadosAPI.Data
{
    using Microsoft.Data.SqlClient;

    namespace JerarquiaEmpleados.API.Data
    {
        public class SqlConnectionFactory
        {
            private readonly IConfiguration _config;

            public SqlConnectionFactory(IConfiguration config)
            {
                _config = config;
            }

            public SqlConnection CreateConnection()
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
    }

}
