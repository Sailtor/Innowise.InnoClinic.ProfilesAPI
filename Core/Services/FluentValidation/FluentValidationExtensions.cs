using FluentValidation;

namespace Services.FluentValidation
{
    internal static class FluentValidationExtensions
    {
        public static void ValidateAndThrowCustom<T>(this IValidator<T> validator, T instance)
        {
            var res = validator.Validate(instance);

            if (!res.IsValid)
            {
                throw new ValidationException(res.Errors);
            }
        }
    }
}