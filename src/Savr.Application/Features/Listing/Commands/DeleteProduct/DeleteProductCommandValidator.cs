using FluentValidation;

namespace Savr.Application.Features.Products.Commands.DeleteProduct
{
    internal class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("Id is required");
        }
    }
}
