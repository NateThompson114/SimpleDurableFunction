using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SimpleDurableFunction
{
    public static class DurableFunctionExample
    {
        [FunctionName("Orchestrator")]
        public static async Task<string> Orchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation($"Starting Orchestrator.");

            var input = context.GetInput<string>();

            var result = await context.CallActivityAsync<string>("Activity", input);

            return result;
        }

        [FunctionName("Activity")]
        public static string Activity(
            [ActivityTrigger] string input,
            ILogger log)
        {
            log.LogInformation($"Activity triggered with input: {input}");

            return $"Processed input: {input}";
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
