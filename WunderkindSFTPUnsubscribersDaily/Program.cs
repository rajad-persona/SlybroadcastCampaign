using System;
using System.Collections.Generic;
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
            var dataset = dbHelper.GetDataSet(Constants.WunderKind_Unsub_Daily_Job, false);
            if (dataset.Tables[0].Rows.Count > 0)
                SFTPHelper.FileUploadSFTP(dataset.Tables[0], $"Persona_UnSubscribers_{filedate}.csv");
        }
    }
}
