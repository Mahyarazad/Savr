using Savr.Domain.Primitives;

namespace Savr.Application.Abstractions
{
    public interface ISpecification<T>
    {
        List<WhereExpression<T>> WhereExpressions { get; }
        List<string> IncludeStrings { get; }

        // Include and paganitation comes here

    }

}
