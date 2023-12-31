using System;
using Azure.Identity;
using Castle.Core.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using SimpleDurableFunction;
using SimpleDurableFunction.Validators;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SimpleDurableFunction
{
    internal class Startup : FunctionsStartup
    {
        public const string SentinelKey = "TestApp:Settings:Sentinel";
        private IConfigurationRoot _configuration;

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var userSecretConfig = new ConfigurationBuilder();
            userSecretConfig.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true);
            var azAppConfigConnection = userSecretConfig.Build()["AppConfig"];

            if (!string.IsNullOrEmpty(azAppConfigConnection))
            {
                // Use the connection string if it is available.
                _configuration = builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(azAppConfigConnection);
                    // Load all keys that start with 'TestApp:' and have no label
                    options.Select("TestApp:*");
                    // Configure to reload configuration if the registered key 'TestApp:Settings:Sentinel' is modified.
                    // Use the default cache expiration of 30 seconds. It can be overriden via AzureAppConfigurationRefreshOptions.SetCacheExpiration.
                    options.ConfigureRefresh(refresh =>
                    {
                        refresh.Register(SentinelKey, refreshAll: true);
                    });
                    // Load all feature flags with no label. To load specific feature flags and labels, set via FeatureFlagOptions.Select.
                    // Use the default cache expiration of 30 seconds. It can be overriden via FeatureFlagOptions.CacheExpirationInterval.
                    options.UseFeatureFlags();
                }).Build();
            }
            else
            {
                // Use Azure Active Directory authentication.
                // The identity of this app should be assigned 'App Configuration Data Reader' or 'App Configuration Data Owner' role in App Configuration.
                // For more information, please visit https://aka.ms/vs/azure-app-configuration/concept-enable-rbac
                _configuration = builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri("https://ningli-appconfig.azconfig.io"), new DefaultAzureCredential());
                    // Load all keys that start with 'TestApp:' and have no label
                    options.Select("TestApp:*");
                    // Configure to reload configuration if the registered key 'TestApp:Settings:Sentinel' is modified.
                    // Use the default cache expiration of 30 seconds. It can be overriden via AzureAppConfigurationRefreshOptions.SetCacheExpiration.
                    options.ConfigureRefresh(refresh =>
                    {
                        refresh.Register(SentinelKey, refreshAll: true);
                    });
                    // Load all feature flags with no label. To load specific feature flags and labels, set via FeatureFlagOptions.Select.
                    // Use the default cache expiration of 30 seconds. It can be overriden via FeatureFlagOptions.CacheExpirationInterval.
                    options.UseFeatureFlags();
                }).Build();
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var phase2 = _configuration.GetValue<bool>("FeatureManagement:MockUnfinishedFeature");
            // Make Azure App Configuration services and feature manager available through dependency injection

            if (phase2)
            {
                builder.Services.AddTransient<IStringValidator, MockStringValidator>();
            }
            else
            {
                builder.Services.AddTransient<IStringValidator, RealStringValidator>();
            }

            builder.Services.AddAzureAppConfiguration();
            builder.Services.AddFeatureManagement();
            builder.Services.AddTransient<StringValidator>();
        }
    }
}
