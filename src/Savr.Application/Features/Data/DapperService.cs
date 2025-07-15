using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Savr.Application.Abstractions.Data;
using System.Data;

namespace Savr.Application.Feature.Data
{
    public class DapperService : IDapperService
    {
        private readonly IConfiguration _configurtion;

        public DapperService(IConfiguration configurtion)
        {
            _configurtion = configurtion;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_configurtion.GetConnectionString("MSSqlServer"));
        }
    }
}
