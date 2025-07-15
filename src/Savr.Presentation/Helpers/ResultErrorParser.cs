using FluentResults;

namespace Savr.Presentation.Helpers
{
    public static class ResultErrorParser
    {
        public static object ParseResultError(List<IError> errors)
        {
            return new
            {
                Errors = errors.Select(x => x.Message).ToList()
            };
        }
    }
}
