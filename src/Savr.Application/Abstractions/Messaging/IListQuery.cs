using MediatR;

namespace Savr.Application.Abstractions.Messaging
{
    public interface IListQuery<TItem> : IRequest<TItem>
    {

    }
}