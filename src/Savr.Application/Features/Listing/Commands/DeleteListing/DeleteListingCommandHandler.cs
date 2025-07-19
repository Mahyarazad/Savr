using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;

using System.Net;

namespace Savr.Application.Features.Listings.Commands
{
    internal class DeleteListingCommandHandler : ICommandHandler<DeleteListingCommand, Result>
    {
        private readonly IListingRepository _productRepository;
        
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteListingCommandHandler(IListingRepository productRepository, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteListingCommand request, CancellationToken cancellationToken)
        {
            

            if (!await _productRepository.AnyAsync(request.Id, cancellationToken))
            {
                return Result.Fail(HttpStatusCode.NotFound.ToString());
            }

            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null || !Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail(HttpStatusCode.Unauthorized.ToString());
            }

            var ownsProduct = await _productRepository.DoesUserOwnThisListingAsync(request.Id, userId, cancellationToken);
            if (!ownsProduct)
            {
                return Result.Fail(HttpStatusCode.Forbidden.ToString());
            }

            await _productRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

    }
}
