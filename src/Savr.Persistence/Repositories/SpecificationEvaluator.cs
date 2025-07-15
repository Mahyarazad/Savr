using Savr.Domain.Abstractions;
using Savr.Domain.Primitives;

namespace Savr.Persistence.Repositories
{
    public  class SpecificationEvaluator<T>
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, Domain.Abstractions.ISpecification<T> specification)
        {
            IQueryable<T> query = inputQuery;

            foreach(WhereExpression<T> expression in specification.WhereExpressions)
            {
                query = query.Where(expression.Criteria);
            }


            return query;
        }
    }
}
