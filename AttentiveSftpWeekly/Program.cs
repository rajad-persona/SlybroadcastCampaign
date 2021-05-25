using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace SFTPBatchJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.Log("SFTP Batch Started");
            try
            {
                var dbHelper = new DBHelper();
                if (ConfigurationManager.AppSettings["sftphost"]?.ToLower()?.Contains("attentive") ?? false)
                {
                    var dataset = dbHelper.GetDataSet(Constants.Attentive_weekly_Job);
                    var filedate = DateTime.Now.ToString("ddMMyyyy");
                    if (dataset.Tables[0].Rows.Count > 0)
                        SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"PurchaserCustomers_{filedate}.csv");
                    if (dataset.Tables[1].Rows.Count > 0)
                        SFTPHelper.FileUploadSFTP(dataset.Tables[1], $"CancelledCustomers_{filedate}.csv");
                }
                else if (ConfigurationManager.AppSettings["sftphost"]?.ToLower()?.Contains("40.86.85.18") ?? false)
                {
                    var filedate = DateTime.Now.ToString("yyyyMMddHH");
                    var dataset = dbHelper.GetDataSet(Constants.WunderKind_Hourly_Job, false);
                    if (dataset.Tables[0].Rows.Count > 0)
                        SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Persona_Subscribers_{filedate}.csv");

                    if (ConfigurationManager.AppSettings["BrazeApiKey"] != null)
                    {
                        var wunderKindHistory = dbHelper.GetDataSet(Constants.WunderKind_get_History, false).Tables[0].AsEnumerable()
                                   .Select(r => r.Field<string>("FileName"))
                                   .ToList();

                        var processedFiles = new List<string>();
                        var emails = SFTPHelper.GetWunderKindSFTPFiles(wunderKindHistory, out processedFiles);

                        dbHelper.Insert(processedFiles.ConvertListToDataTable("FileName"), "TBL_WUNDERKIND_SUBSCRIBERS_BATCHJOB_LOG");
                        var brazeData = new BrazeUser
                        {
                            Attributes = dbHelper.GetDataSet(string.Format(Constants.WunderKind_bulk_insert, string.Join("|", emails)), false).Tables[0].ConvertToList<Utilities.Attribute>()
                        };
                        if (brazeData != null && brazeData.Attributes.Any())
                            AddUserstoBraze(brazeData).GetAwaiter().GetResult();
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex, "Error in SFTP batch job");
            }
            LogHelper.Log("SFTP Batch Ended");
        }




        private static async Task<string> AddUserstoBraze(BrazeUser data)
        {
            LogHelper.Log("AddUserstoBraze started");
            try
            {
                var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["BrazeApiKey"]}");
                using (var response = await client.PostAsync("https://rest.iad-02.braze.com/users/track", content).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    var repsponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return repsponse;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex,"Error AddUserstoBraze ended");

            }
            LogHelper.Log("AddUserstoBraze ended");
            return null;
        }
    }
}
