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
            
            await CreateMessage("http://localhost:5000", 10);    
        }

        private static async Task CreateMessage(string url, int numberRequests)
        {
            // client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(
            //     new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            // client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            // var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

            // var msg = await stringTask;
            // Console.Write(msg);

            //HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            for (int i = 0; i < numberRequests; i++)
            {
                var message  = "{'Mensagem': 'teste " + i.ToString() + "'}";

                var response = await client.PostAsync("api/mensagens", new StringContent(message, System.Text.Encoding.UTF8, "application/json"));
                if (response != null)
                {
                    Console.WriteLine(response.ToString());
                }
            }
            
        }
    }
}
