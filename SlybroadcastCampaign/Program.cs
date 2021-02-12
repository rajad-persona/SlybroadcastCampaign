using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SlybroadcastCampaign
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var dbHelper = new DBHelper();
            var customer = dbHelper.GetCustomers();
            if (customer != null && customer.Any())
            {
                var campaigns = customer.GroupBy(i => i.Message);
                foreach (var camp in campaigns)
                {
                    var data = new Dictionary<string, string>();
                    data.Add("c_method", "new_campaign");
                    data.Add("c_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz"));
                    data.Add("c_record_audio", camp.Key);
                    data.Add("c_phone", string.Join(",", camp.Select(i => i.MOBILE).Where(i => !string.IsNullOrEmpty(i))));
                    data.Add("c_callerID", "4252000970");
                    data.Add("c_uid", "BreanneS@PersonaNutrition.com");
                    data.Add("c_password", "Persona1!");
                    data.Add("mobile_only", "0");
                    data.Add("c_title", "campaign from API batchjob");

                    var resp = CreateCampaign(data).GetAwaiter().GetResult();
                }
            }
        }

        private static async Task<string> CreateCampaign(Dictionary<string, string> data)
        {
            if (ConfigurationManager.AppSettings["createCampaigns"] == "true")
            {
                using (HttpContent formContent = new FormUrlEncodedContent(data))
                {
                    using (HttpResponseMessage response = await client.PostAsync("https://www.mobile-sphere.com/gateway/vmb.php", formContent).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }
            }
            return null;
        }
    }
}
