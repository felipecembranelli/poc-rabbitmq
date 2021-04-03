using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            
            await CreateMessage("http://localhost:5000/", args[0], int.Parse(args[1]));    
        }

        private static async Task CreateMessage(string url, string queue, int numberRequests)
        {
            client.BaseAddress = new Uri(url);

            for (int i = 0; i < numberRequests; i++)
            {
                var message  = "{'Mensagem': 'teste " + i.ToString() + "'}";

                var response = await client.PostAsync("api/mensagens/" + queue, new StringContent(message, System.Text.Encoding.UTF8, "application/json"));
                if (response != null)
                {
                    Console.WriteLine(response.ToString());
                }
            }
            
        }
    }
}
