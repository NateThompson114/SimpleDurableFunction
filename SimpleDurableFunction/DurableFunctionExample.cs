using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SimpleDurableFunction
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }

    public class StringValidator : AbstractValidator<string>
    {
        public StringValidator()
        {
            RuleFor(str => str).NotNull().NotEmpty().WithMessage("The string must not be empty");
        }
    }

    public static class DurableFunctionExample
    {
        [FunctionName("Orchestrator")]
        public static async Task<ValidationResult> Orchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation($"Starting Orchestrator.");

            var input = context.GetInput<string>();

            var result = await context.CallActivityAsync<ValidationResult>("Activity", input);

            return result;
        }

        [FunctionName("Activity")]
        public static ValidationResult Activity(
            [ActivityTrigger] string input,
            ILogger log)
        {
            log.LogInformation($"Activity triggered with input: {input}");

            var validator = new StringValidator();
            var validationResult = validator.Validate(input);

            return new ValidationResult
            {
                IsValid = validationResult.IsValid,
                Message = validationResult.IsValid ? $"Processed input: {input}" : string.Join(", ", validationResult.Errors)
            };
        }

        [FunctionName("HttpEndpoint")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableClient starter,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            var instanceId = await starter.StartNewAsync("Orchestrator", null);

            string name = req.Query["name"];

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}: {instanceId}")
                : new BadRequestObjectResult($"Please pass a name on the query string or in the request body: {instanceId}");
        }
    }
}
