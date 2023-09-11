using System;

namespace SimpleDurableFunction.Validators;

public class MockStringValidator : IStringValidator
{
    public ValidationResult Validate(string input)
    {
        throw new NotImplementedException();
    }
}