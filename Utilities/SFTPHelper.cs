using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class SFTPHelper
    {
        private static string host = ConfigurationManager.AppSettings["sftphost"];
        private static int port = 22;
        private static string username = ConfigurationManager.AppSettings["sftpusername"];
        private static string password = ConfigurationManager.AppSettings["sftppassword"];
        public static void FileUploadSFTP(System.Data.DataTable data, string fileName)
        {


            byte[] csvFile = data.ToCSV(); // Function returns byte[] csv file

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    using (var ms = new System.IO.MemoryStream(csvFile))
                    {
                        if (ConfigurationManager.AppSettings["sftpFolder"] == null)
                            client.UploadFile(ms, $"{fileName}");
                        else
                            client.UploadFile(ms, $"/{ConfigurationManager.AppSettings["sftpFolder"]}/{fileName}");
                    }
                }
                else
                {
                    Console.WriteLine("SFTP couldn't connect");
                }
            }
        }
        public static List<Attribute> GetBrazeAttributes(DataTable dataset, string type, bool isSubsribe = true)
        {
            var brazeAttributes = dataset.ConvertToList<Utilities.Attribute>();
            foreach (var item in brazeAttributes)
            {
                item.ExternalId = type + item.ID.ToString();
                if (string.IsNullOrEmpty(item.customer_source))
                {
                    item.customer_source = type.ToUpper();
                }
                item.EmailSubscribe = item.PushSubscribe = isSubsribe ? "subscribed" : "unsubscribed";
            }
            return brazeAttributes;
        }
        public static List<string> GetWunderKindSFTPFiles(List<string> wunderKindHistory, out List<string> files)
        {
            var resp = new List<string>();
            files = new List<string>();
            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    var folders = client.ListDirectory("");
                    files = folders.Where(n => !wunderKindHistory.Contains(n.Name) && (n.Name?.ToLower()?.Contains("wunderkind_subscribers_") ?? false)).Select(n => n.Name).ToList();
                    resp = getEmailsfromExcel(files, client);
                }
                else
                {
                    Console.WriteLine("SFTP couldn't connect");
                }
            }
            return resp;
        }

        public static List<string> getEmailsfromExcel(List<string> filePaths, SftpClient sftp)
        {
            var resp = new List<string>();
            foreach (var filePath in filePaths)
            {
                using (var remoteFileStream = sftp.OpenRead(filePath))
                {
                    using (var reader = new StreamReader(remoteFileStream))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("@"))
                                resp.Add(line.Replace("\"", string.Empty));
                        }
                    }
                }
            }
            return resp;
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize = 49)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }


        public static async Task<string> AddUserstoBraze(BrazeUser data)
        {
            LogHelper.Log("AddUserstoBraze started");
            try
            {
                var splitArray = data.Attributes.SplitList();
                foreach (var item in splitArray)
                {
                    var apireq = new BrazeUser { Attributes = item };
                    var client = new HttpClient();
                    var content = new StringContent(JsonConvert.SerializeObject(apireq), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["BrazeApiKey"]}");
                    using (var response = await client.PostAsync("https://rest.iad-02.braze.com/users/track", content).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        var repsponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Error AddUserstoBraze ended");

            }
            LogHelper.Log("AddUserstoBraze ended");
            return null;
        }
    }
}
