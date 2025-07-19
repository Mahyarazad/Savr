using Savr.Domain.Primitives;
using System.Linq.Expressions;

namespace Savr.Application.Abstractions
{
    public class WhereExpression<T>
    {
        public Expression<Func<T, bool>> Criteria { get;set; }
    }
}