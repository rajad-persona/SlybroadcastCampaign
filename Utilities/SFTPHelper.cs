using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
    }
}
