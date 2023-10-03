using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using SimpleDurableFunction.Validators;

namespace SimpleDurableFunction
{
    public class DurableFunctionExample
    {
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;
        private readonly IStringValidator _stringValidator;
        private readonly IConfigurationRefresher _configurationRefresher;
        

        public DurableFunctionExample(IFeatureManagerSnapshot featureManagerSnapshot, IConfigurationRefresherProvider refresherProvider, IStringValidator stringValidator)
        {
            _featureManagerSnapshot = featureManagerSnapshot;
            _stringValidator = stringValidator;
            _configurationRefresher = refresherProvider.Refreshers.First();
        }

        [FunctionName("Orchestrator")]
        public async Task<string> Orchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation($"Starting Orchestrator.");

            var input = context.GetInput<string>();

            // Signal to refresh the feature flags from Azure App Configuration.
            // This will be a no-op if the cache expiration time window is not reached.
            // Remove the 'await' operator if it's preferred to refresh without blocking.
            await _configurationRefresher.TryRefreshAsync();

            var featureName = "TestFeatureFlag";
            var featureEnabled = await _featureManagerSnapshot.IsEnabledAsync(featureName);

            if (featureEnabled)
            {
                log.LogInformation("The feature flag was active");
                //MyService1()
            }
            else
            {
                log.LogInformation("The feature flag was NOT active");
                //MyService2()
            }

            var result = await context.CallActivityAsync<string>("Activity", input);

            return result;
        }

        [FunctionName("Activity")]
        public string Activity(
            [ActivityTrigger] string input,
            ILogger log)
        {
            log.LogInformation($"Activity triggered with input: {input}");

            return $"Processed input: {input}";
        }

        [FunctionName("HttpEndpoint")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableClient starter,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            var instanceId = await starter.StartNewAsync("Orchestrator", null);

            string name = req.Query["name"];

            _stringValidator.Validate(name);

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}: {instanceId}")
                : new BadRequestObjectResult($"Please pass a name on the query string or in the request body: {instanceId}");
        }
    }
}
