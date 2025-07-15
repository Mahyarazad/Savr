using FluentValidation.Results;

namespace Savr.Application.Helpers
{
    public static class ResultErrorParser
    {
        public static IEnumerable<string> GetErrorsFromValidator(ValidationResult validationResult)
        {
            return validationResult.Errors.Select(x => x.ErrorMessage).ToList();
        }
    }
}
