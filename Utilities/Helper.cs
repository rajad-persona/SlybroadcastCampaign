using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Helper
    {
        public static void FileUploadSFTP(System.Data.DataTable data, string fileName)
        {
            var host = ConfigurationManager.AppSettings["sftphost"];
            var port = 22;
            var username = ConfigurationManager.AppSettings["sftpusername"];
            var password = ConfigurationManager.AppSettings["sftppassword"];

            byte[] csvFile = data.ToCSV(); // Function returns byte[] csv file

            using (var client = new Renci.SshNet.SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {

                    using (var ms = new System.IO.MemoryStream(csvFile))
                    {
                        //client.BufferSize = (uint)ms.Length; // bypass Payload error large files
                        if (ConfigurationManager.AppSettings["sftpFolder"] == null)
                            client.UploadFile(ms, $"{fileName}");
                        else
                            client.UploadFile(ms, $"/{ConfigurationManager.AppSettings["sftpFolder"]}/{fileName}");
                    }
                }
                else
                {
                    Console.WriteLine("I couldn't connect");
                }
            }
        }

    }
}
