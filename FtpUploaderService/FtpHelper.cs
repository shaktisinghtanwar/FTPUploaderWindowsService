using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FtpUploaderService
{
    class FtpHelper
    {
        static string _userName;
        public static string UserName
        {
            get
            {
                return _userName.Replace("\\\\","\\");
            }
            internal set
            {
                _userName = value;

            }
        }
        public static string Password { get; internal set; }

        public static bool FileUploadFTP(string filepath, string ftpFolderName)
        {
            try
            {
                string ftpUrl = ConfigurationManager.AppSettings["ftpUrl"];
                var host = ftpUrl + ftpFolderName + "/" + Path.GetFileName(filepath);
             
                var uploadFile = filepath;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host);
                request.Credentials = new NetworkCredential(UserName, Password);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Timeout = int.MaxValue;
                request.ReadWriteTimeout = int.MaxValue;
                Console.WriteLine("Wait....");
                using (Stream fileStream = File.OpenRead(uploadFile))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine("Upload File Complete, status {response.StatusDescription}");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static void MakeDirectoryFTP(string directoryName)
        {

            try
            {
                string ftpUrl = ConfigurationManager.AppSettings["ftpUrl"];

                WebRequest request = WebRequest.Create(ftpUrl + directoryName);
               
                request.Credentials = new NetworkCredential(UserName, Password);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    request.Abort();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static bool DoesFtpDirectoryExist(string directoryName)
        {
            bool res = false;
            try
            {
                string ftpUrl = ConfigurationManager.AppSettings["ftpUrl"];

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl + directoryName + "/");
               
                request.Credentials = new NetworkCredential(UserName, Password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                request.Abort();
                Console.WriteLine(response.StatusCode);
                Console.ReadLine();
                res = true;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}
