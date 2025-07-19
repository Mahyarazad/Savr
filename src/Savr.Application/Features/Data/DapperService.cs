using Microsoft.Extensions.Configuration;
using Npgsql;
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
            return new NpgsqlConnection(_configurtion.GetConnectionString("Postgres"));
        }
    }
}
