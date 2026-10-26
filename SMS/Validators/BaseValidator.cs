using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class BaseValidator<TRequest> : AbstractValidator<TRequest> where TRequest : BaseDto
    {
        public BaseValidator()
        {
        }
    }
}
