using FluentValidation;

namespace SimpleDurableFunction.Validators;

public class StringValidator : AbstractValidator<string>
{
    public StringValidator()
    {
        RuleFor(str => str).NotNull().NotEmpty().WithMessage("The string must not be empty");
    }
}