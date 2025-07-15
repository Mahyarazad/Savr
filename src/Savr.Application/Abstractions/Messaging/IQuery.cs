using FluentResults;
using MediatR;

namespace Savr.Application.Abstractions.Messaging
{
    internal interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}
