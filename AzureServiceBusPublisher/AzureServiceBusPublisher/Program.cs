using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBusPublisher.Model;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AzureServiceBusPublisher
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://poc-edi-wtw.servicebus.windows.net/;SharedAccessKeyName=accesspolicy;SharedAccessKey=nNI8WjnvOQc7ICNuZ8t0ZjNkjX7Z6lYoqrHo6dGMcZ4=;";
        const string TopicName = "testtopic";
        static ITopicClient topicClient;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Sending Message to the Topic...");

            const int numberOfMessages = 100;
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Send messages.
            await SendMessagesAsync(numberOfMessages);

            Console.WriteLine("Sending Message completed.");

            Console.ReadKey();

            await topicClient.CloseAsync();

        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    
                    var sovMessage = new SOVMessage()
                    {
                        SOVId = 2345,
                        FacilityNumber = 13,
                        RiskScore = null,
                        Longitude = null,
                        Latitude = null,
                        FacilityName = "Hospital A",
                        TotalInsuredValue = 25000.45
                    };

                    string jsonMessage = JsonConvert.SerializeObject(sovMessage);

                    var message = new Message(Encoding.UTF8.GetBytes(jsonMessage));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {jsonMessage}");

                    // Send the message to the topic.
                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
