using FluentValidation;

namespace Savr.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(128); ;
            RuleFor(p=> p.ManufacturePhone).MaximumLength(128); ;
        }
    }
}
