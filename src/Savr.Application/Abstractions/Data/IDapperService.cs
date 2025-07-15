using System.Data;

namespace Savr.Application.Abstractions.Data
{
    public interface IDapperService
    {
        IDbConnection CreateConnection();
    }
}
