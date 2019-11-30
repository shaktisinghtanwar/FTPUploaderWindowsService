using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FtpUploaderService
{
    public partial class FtpUploaderService : ServiceBase
    {
        Timer timer;
        public FtpUploaderService()
        {
           // InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer(new TimerCallback(RunService), null, 0, 1 * 1000 * 60);
        }
     
        private void RunService(object state)
        {
            ReadUsernameAndPassword();

            try
            {
                var userName = GetWindowsUserAccountName();
                var path = string.Format("C:\\Users\\{0}\\AppData\\Local\\R", userName);
                var date = DateTime.Now;
                int year = date.Year;
                int month = date.Month;
                int day = date.Day;
                if (!FtpHelper.DoesFtpDirectoryExist(year.ToString()))
                {
                    FtpHelper.MakeDirectoryFTP(year.ToString());
                }
                if (!FtpHelper.DoesFtpDirectoryExist(year + "/" + month.ToString()))
                {
                    FtpHelper.MakeDirectoryFTP(year + "/" + month.ToString());
                }
                if (!FtpHelper.DoesFtpDirectoryExist(year + "/" + month.ToString() + "/" + day.ToString()))
                {
                    FtpHelper.MakeDirectoryFTP(year + "/" + month.ToString() + "/" + day.ToString());
                }
                var files = Directory.GetFiles(path,"*.mp3");
                var filesToDelete = new List<string>();
                foreach (var file in files)
                {
                    try
                    {
                       var result =  FtpHelper.FileUploadFTP(file, year + "/" + month.ToString() + "/" + day.ToString());
                       if (result)
                          filesToDelete.Add(file);
                    }
                    catch (Exception ex)
                    {

                        Console.Write(ex.Message);
                    }
                }

                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ReadUsernameAndPassword()
        {
            string sPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Test.txt");
            var data = File.ReadAllText(sPath);
            FtpHelper.UserName = data.Split(',')[0].Split(':').Last();
            FtpHelper.Password = data.Split(',')[1].Split(':').Last();
        }

        public static string GetWindowsUserAccountName()
        {
            string userName = string.Empty;
            ManagementScope ms = new ManagementScope("\\\\.\\root\\cimv2");
            ObjectQuery query = new ObjectQuery("select * from win32_computersystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, query);

            foreach (ManagementObject mo in searcher?.Get())
            {
                userName = mo["username"]?.ToString();
            }
            userName = userName?.Substring(userName.IndexOf(@"\") + 1);

            return userName;
        }
        protected override void OnStop()
        {
        }
    }
}
