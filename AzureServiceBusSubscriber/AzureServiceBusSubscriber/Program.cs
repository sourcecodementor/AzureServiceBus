﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBusSubscriber.Model;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AzureServiceBusSubscriber
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://poc-edi-wtw.servicebus.windows.net/;SharedAccessKeyName=accesspolicy;SharedAccessKey=nNI8WjnvOQc7ICNuZ8t0ZjNkjX7Z6lYoqrHo6dGMcZ4=;";
        const string TopicName = "testtopic";
        const string SubscriptionName = "sub1";
        static ISubscriptionClient subscriptionClient;

        public static async Task Main(string[] args)
        {
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

       

            // Register subscription message handler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.

            SOVMessage sovMessage = JsonConvert.DeserializeObject<SOVMessage>(Encoding.UTF8.GetString(message.Body));

            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} SOVId:{sovMessage.SOVId}; FacilityName: {sovMessage.FacilityName}");

            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the subscriptionClient has already been closed.
            // If subscriptionClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        // Use this handler to examine the exceptions received on the message pump.
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
