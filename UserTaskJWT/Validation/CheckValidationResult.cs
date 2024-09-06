using FluentValidation;
using FluentValidation.Results;

namespace UserTaskJWT.Web.Api.Validation
{
    public static class CheckValidationResult
    {
        public static void IsValidationResultValid(ValidationResult validationResult)
        {
            ArgumentNullException.ThrowIfNull(validationResult);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
