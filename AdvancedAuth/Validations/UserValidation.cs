using AdvancedAuth.Model;
using FluentValidation;

namespace AdvancedAuth.Validations
{
    public class UserValidation : AbstractValidator<User>
    {
        public UserValidation()
        {
            RuleFor(x => x.FirstName).NotNull().WithMessage("Name cannot be empty")
                                     .MaximumLength(20).WithMessage("Name must be 20 characters");

            RuleFor(x => x.LastName).NotNull().WithMessage("Last name cannot be empty")
                                    .MaximumLength(30).WithMessage("Last name must be 30 characters");

            RuleFor(x => x.Email).NotNull().WithMessage("Email cannot be empty").EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.BirthDate).NotNull().WithMessage("Birth date cannot be empty");
        }
    }
}
