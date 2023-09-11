namespace SimpleDurableFunction.Validators;

public class RealStringValidator : IStringValidator
{
    public ValidationResult Validate(string input)
    {
        // Real validation logic here
        return new ValidationResult() { IsValid = true, Message = "Is Valid" };
    }
}