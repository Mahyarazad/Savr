using Savr.Domain.Entities;
using System.Linq.Expressions;

namespace Savr.Application.Abstractions
{
    public static class FilterExtensions
    {
        public static IQueryable<Listing> ApplyFilter(IQueryable<Listing> query, SqlFilter filter)
        {
            var parameter = Expression.Parameter(typeof(Listing), "x");
            var property = Expression.PropertyOrField(parameter, filter.Field);

            Expression constant;

            if (property.Type == typeof(string))
                constant = Expression.Constant(filter.Value);
            else if (property.Type == typeof(Guid))
                constant = Expression.Constant(Guid.Parse(filter.Value));
            else if (property.Type == typeof(long))
                constant = Expression.Constant(long.Parse(filter.Value));
            else if (property.Type == typeof(decimal))
                constant = Expression.Constant(decimal.Parse(filter.Value));
            else if (property.Type == typeof(int))
                constant = Expression.Constant(int.Parse(filter.Value));
            else if (property.Type == typeof(bool))
                constant = Expression.Constant(bool.Parse(filter.Value));
            else
                throw new NotSupportedException($"Unsupported property type: {property.Type}");

            Expression comparison;

            switch (filter.Operator.ToUpperInvariant())
            {
                case "=":
                    comparison = Expression.Equal(property, constant);
                    break;
                case "!=":
                case "<>":
                    comparison = Expression.NotEqual(property, constant);
                    break;
                case ">":
                    comparison = Expression.GreaterThan(property, constant);
                    break;
                case "<":
                    comparison = Expression.LessThan(property, constant);
                    break;
                case ">=":
                    comparison = Expression.GreaterThanOrEqual(property, constant);
                    break;
                case "<=":
                    comparison = Expression.LessThanOrEqual(property, constant);
                    break;
                case "LIKE":
                case "ILIKE":
                    comparison = Expression.Call(
                        property,
                        nameof(string.Contains),
                        Type.EmptyTypes,
                        constant
                    );
                    break;
                default:
                    throw new NotSupportedException($"Operator '{filter.Operator}' is not supported");
            }

            var lambda = Expression.Lambda<Func<Listing, bool>>(comparison, parameter);
            return query.Where(lambda);
        }
    }

}
