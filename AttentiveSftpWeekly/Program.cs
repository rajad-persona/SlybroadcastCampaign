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
                    var filedate = DateTime.Now.ToString("yyyyMMdd");
                    var brazeData = new BrazeUser
                    {
                        Attributes = new List<Utilities.Attribute>()
                    };
                    var dataset = dbHelper.GetDataSet(Constants.Persona_Purchase_Subscribers, false);
                   
                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Persona_Purchasers_{filedate}.csv");
                        brazeData.Attributes.AddRange(SFTPHelper.GetBrazeAttributes(dataset.Tables[0], "pn"));
                    }
                    dataset = dbHelper.GetDataSet(Constants.Persona_NonPurchase_Subscribers, false);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Persona_NonPurchasers_{filedate}.csv");
                        brazeData.Attributes.AddRange(SFTPHelper.GetBrazeAttributes(dataset.Tables[0], "pn"));
                    }

                    dataset = dbHelper.GetDataSet(Constants.Persona_Paused_Subscribers, false);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Persona_Paused_{filedate}.csv");
                        brazeData.Attributes.AddRange(SFTPHelper.GetBrazeAttributes(dataset.Tables[0], "pn"));
                    }
                    if (ConfigurationManager.AppSettings["BrazeApiKey"] != null)
                    {
                        var wunderKindHistory = dbHelper.GetDataSet(Constants.WunderKind_get_History, false).Tables[0].AsEnumerable()
                                   .Select(r => r.Field<string>("FileName"))
                                   .ToList();

                        var processedFiles = new List<string>();
                        var emails = SFTPHelper.GetWunderKindSFTPFiles(wunderKindHistory, out processedFiles);

                        dbHelper.Insert(processedFiles.ConvertListToDataTable("FileName"), "TBL_WUNDERKIND_SUBSCRIBERS_BATCHJOB_LOG");
                        dataset = dbHelper.GetDataSet(string.Format(Constants.WunderKind_bulk_insert, string.Join("|", emails)), false);

                        if (dataset.Tables[0].Rows.Count > 0)
                        {
                            brazeData.Attributes.AddRange(SFTPHelper.GetBrazeAttributes(dataset.Tables[0], "wk"));
                        }

                        if (brazeData != null && brazeData.Attributes.Any())
                            SFTPHelper.AddUserstoBraze(brazeData).GetAwaiter().GetResult();
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex, "Error in SFTP batch job");
            }
            LogHelper.Log("SFTP Batch Ended");
        }



    }
}
