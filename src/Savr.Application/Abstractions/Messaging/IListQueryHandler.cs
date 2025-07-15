using MediatR;

namespace Savr.Application.Abstractions.Messaging
{
    public interface IListQueryHandler<TQuery, TItem> : IRequestHandler<TQuery, TItem>
           where TQuery : IListQuery<TItem>
    {
    }
}

