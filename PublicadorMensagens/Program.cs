using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;


namespace PublicadorMensagens
{
    class Program
    {
        static void Main(string[] args)
        {
            //string url = string.Format("{0}/name?PrimaryName={1}", System.Configuration.ConfigurationManager.AppSettings["URLREST"], "yournmae");
            //string url = string.Format("{0}/name?PrimaryName={1}", "localhost:5001", "yournmae");
            //string details = CallRestMethod(url);

            CallPOSTWebAPIAsync("localhost:5001");
        }

        public static string CallGETRestMethod(string url)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            webrequest.Headers.Add("Username", "xyz");
            webrequest.Headers.Add("Password", "abc");
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            result = responseStream.ReadToEnd();            
            webresponse.Close();
            return result;
        }

        public static async Task CallPOSTWebAPIAsync(string url)
        {
            var message  = "{'Mensagem': 'Primeiro teste'}";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var response = await client.PostAsync("api/mensagens", new StringContent(student, Encoding.UTF8, "application/json"));
            if (response != null)
            {
                Console.WriteLine(response.ToString());
            }
        }
    }
}
