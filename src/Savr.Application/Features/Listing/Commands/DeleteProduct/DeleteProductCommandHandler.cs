using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Domain.Abstractions.Persistence.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
using System.Net;

namespace Savr.Application.Features.Products.Commands.DeleteProduct
{
    internal class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Result>
    {
        private readonly IListingRepository _productRepository;
        private readonly IValidator<DeleteProductCommand> _validator;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IListingRepository productRepository, IValidator<DeleteProductCommand> validator, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _validator = validator;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Fail(HttpStatusCode.BadRequest.ToString());
            }

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

            var deleted = await _productRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

    }
}
