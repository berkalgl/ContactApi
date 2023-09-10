using FluentValidation;

namespace ContactApi.V1.Models.Requests.Validators
{
    public class CreateContactRequestModelValidator : AbstractValidator<CreateContactRequestModel>
    {
        public CreateContactRequestModelValidator() 
        {
            RuleFor(x => x.Salutation)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .Must(x => int.TryParse(x, out var val) && val > 0)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}