using Newtonsoft.Json;
using SlybroadcastCampaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AttentiveAPI
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var dbHelper = new DBHelper();
            var customer = dbHelper.GetAttentiveCustomers();
            //var req = new List<string>();
            //req.Add("3608402536");
            //req.Add("5096702234");
            //var resp = SendMessage(req).GetAwaiter().GetResult();
        }

        private static async Task<string> SendMessage(List<string> data)
        {
            var req = new Message
            {
                body = "This is a test message from Attentive API Batch JOb",
                to = string.Join(";", data),
                type = "TRANSACTIONAL"
            };
            var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer EhwTWneyXpU5-PvdToEFtgqPyCC22uyLKeRhA1-7uDg");
            using (var response = await client.PostAsync("https://api.attentivemobile.com/1/messages", content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var repsponse= await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return repsponse;
            }
            return null;
        }

    }
    public class Message
    {
        public string to { get; set; }
        public string body { get; set; }
        public string type { get; set; }
    }

}
