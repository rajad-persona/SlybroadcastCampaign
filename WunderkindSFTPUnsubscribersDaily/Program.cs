using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WunderkindSFTPUnsubscribersDaily
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbHelper = new DBHelper();
            var filedate = DateTime.Now.ToString("yyyyMMdd");
            //var dataset = dbHelper.GetDataSet(Constants.Persona_Unsub_Daily_Job, false);
            var brazeData = new BrazeUser
            {
                Attributes = new List<Utilities.Attribute>()
            };
            //if (dataset.Tables[0].Rows.Count > 0)
            //{
            //    SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Persona_CancelledCustomers_{filedate}.csv");
            //    brazeData.Attributes = SFTPHelper.GetBrazeAttributes(dataset.Tables[0], "pn");
            //}

           var dataset = dbHelper.GetDataSet(Constants.MarketPlace_Unsub_Daily_Job, false);
            if (dataset.Tables[0].Rows.Count > 0)
            {
                SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Marketing_UnSubscribers_{filedate}.csv");
                brazeData.Attributes.AddRange(SFTPHelper.GetBrazeAttributes(dataset.Tables[0], "pn", false));
            }

            if (brazeData != null && brazeData.Attributes.Any())
            {
                SFTPHelper.AddUserstoBraze(brazeData).GetAwaiter().GetResult();
            }
        }




    }
}
