using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace SlybroadcastCampaign
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            if (args.Any() && args[0] == "1")
            {
                GetSlyReposne();
            }
            else
                RunCampaigns();
        }
        private static void GetSlyReposne()
        {
            LogHelper.Log("-------------------------------------------------------------");
            LogHelper.Log("SLY response Batch Job started");
            try
            {
                var dbHelper = new DBHelper();
                var campaigns = dbHelper.GetSlyCampaigns();
                if (campaigns != null && campaigns.Any())
                {
                    foreach (var camp in campaigns)
                    {
                        var data = new Dictionary<string, string>();
                        data.Add("c_uid", ConfigurationManager.AppSettings["c_uid"]);
                        data.Add("c_password", ConfigurationManager.AppSettings["c_password"]);
                        data.Add("c_option", "campaign_result");
                        data.Add("session_id", camp);
                        var resp = SlyAPITrigger(data).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(resp))
                        {
                            dbHelper.AddLog(null, null, null, camp, resp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Error in SLY batch job");
            }
            LogHelper.Log("-------------------------------------------------------------");
            LogHelper.Log("SLY Batch Job ended");
        }
        private static void RunCampaigns()
        {
            LogHelper.Log("-------------------------------------------------------------");
            LogHelper.Log("Campaign Batch Job started");
            var dbHelper = new DBHelper();
            try
            {
                var customer = dbHelper.GetCustomers();
                if (customer != null && customer.Any())
                {
                    var campaigns = customer.GroupBy(i => i.Message);
                    var excludeList = ConfigurationManager.AppSettings["exclude_list"].Split(',');
                    foreach (var camp in campaigns)
                    {
                        var data = new Dictionary<string, string>();
                        data.Add("c_method", "new_campaign");
                        data.Add("c_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz"));
                        data.Add("c_record_audio", camp.Key);
                        data.Add("c_phone", string.Join(",", camp.Select(i => i.MOBILE).Where(i => !excludeList.Contains(i) && !string.IsNullOrEmpty(i))));
                        data.Add("c_callerID", ConfigurationManager.AppSettings["c_callerID"]);
                        data.Add("c_uid", ConfigurationManager.AppSettings["c_uid"]);
                        data.Add("c_password", ConfigurationManager.AppSettings["c_password"]);
                        data.Add("mobile_only", ConfigurationManager.AppSettings["mobile_only"]);
                        data.Add("c_title", camp.FirstOrDefault().Campaign_Name);

                        var resp = SlyAPITrigger(data).GetAwaiter().GetResult();
                        if (!string.IsNullOrEmpty(resp))
                        {
                            dbHelper.AddLog(resp, camp.FirstOrDefault().Campaign_Name, camp.Key, null, null);
                        }
                    }
                }
                LogHelper.Log("-------------------------------------------------------------");
                LogHelper.Log("Campaign Batch Job ended");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SLY Campaign Batch Error");
            }
        }
        private static async Task<string> SlyAPITrigger(Dictionary<string, string> data)
        {
            try
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
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Error in SLY api call");
            }
            return null;
        }
    }
}
