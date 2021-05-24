using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace SFTPBatchJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbHelper = new DBHelper();
            if (ConfigurationManager.AppSettings["sftphost"]?.ToLower()?.Contains("attentive") ?? false)
            {
                var dataset = dbHelper.GetDataSet(Constants.Attentive_weekly_Job);
                var filedate = DateTime.Now.ToString("ddMMyyyy");
                if (dataset.Tables[0].Rows.Count > 0)
                    Utilities.Helper.FileUploadSFTP(dataset.Tables[0], $"PurchaserCustomers_{filedate}.csv");
                if (dataset.Tables[1].Rows.Count > 0)
                    Utilities.Helper.FileUploadSFTP(dataset.Tables[1], $"CancelledCustomers_{filedate}.csv");
            }
            else if (ConfigurationManager.AppSettings["sftphost"]?.ToLower()?.Contains("40.86.85.18") ?? false)
            {
                var filedate = DateTime.Now.ToString("yyyyMMddHH");
                var dataset = dbHelper.GetDataSet(Constants.WunderKind_Hourly_Job,false);
                if (dataset.Tables[0].Rows.Count > 0)
                    Utilities.Helper.FileUploadSFTP(dataset.Tables[0], $"Persona_Subscribers_{filedate}.csv");
            }
        }
    }
}
