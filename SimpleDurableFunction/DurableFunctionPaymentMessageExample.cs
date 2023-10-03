using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SimpleDurableFunction.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SimpleDurableFunction
{
    public static class PaymentOrchestratorFunction
    {
        [FunctionName("PaymentOrchestratorFunction_HttpStart")]
        public static async Task HttpStart(
            [HttpTrigger("post")] HttpRequest req,

            [DurableClient] IDurableClient client)
        {
            var request = await req.ReadAsStringAsync();
            var paymentMessage = JsonConvert.DeserializeObject<PaymentMessage>(request);
            var instanceId = paymentMessage.Header.BatchId.ToString();

            await client.StartNewAsync("PaymentOrchestratorFunction", instanceId, paymentMessage);
        }

        [FunctionName("PaymentOrchestratorFunction")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var paymentMessage = await context.CallActivityAsync<List<PaymentMessage>>("UpdateInstanceIdActivity", context.GetInput<PaymentMessage>());

            await WriteToFile(paymentMessage);
        }

        [FunctionName("UpdateInstanceIdActivity")]
        public static List<PaymentMessage> Run(
            [ActivityTrigger] PaymentMessage paymentMessage)
        {
            var message = SplitPayments(paymentMessage);

            return message;
        }

        public static List<PaymentMessage> SplitPayments(PaymentMessage incoming)
        {
            var list = new List<PaymentMessage>();

            foreach (var payment in incoming.PaymentBatch.Payments.ToList())
            {
                var newMessage = new PaymentMessage
                {
                    Header = new PaymentMessageHeader()
                    {
                        BatchId = incoming.Header.BatchId,
                        ClientBatchID = incoming.Header.ClientBatchID,
                        OrganizationId = incoming.Header.OrganizationId,
                        PaymentCount = incoming.Header.PaymentCount,
                        UserId = incoming.Header.UserId,
                        MessageId = Guid.NewGuid()
                    },
                    PaymentBatch = new PaymentBatch()
                    {
                        ExternalId = incoming.PaymentBatch.ExternalId,
                        Id = incoming.PaymentBatch.Id,
                        Payments = new Collection<Payment>()
                    }
                };

                newMessage.PaymentBatch.Payments.Clear();
                newMessage.PaymentBatch.Payments.Add(payment);
                list.Add(newMessage);
            }
            return list;
        }

        public static async Task WriteToFile(List<PaymentMessage> messages)
        {
            const string blobSaveLocation = @"C:\Users\nthompson2\OneDrive - AvidXchange, Inc\Documents\TestPaymentIngestion";
            var filename = $"{messages.First().Header.BatchId}-{DateTime.UtcNow:yyyy-MM-ddThh-mm-ss}.json";
            var filePath = Path.Combine(blobSaveLocation, filename);
            var paymentMessageBlob = JsonConvert.SerializeObject(messages);

            await File.WriteAllTextAsync(filePath, paymentMessageBlob);
        }
    }
}
