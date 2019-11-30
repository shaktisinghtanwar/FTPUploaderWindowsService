using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;

namespace FtpUploaderService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }

        private void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(serviceInstaller1.ServiceName))
            {
                sc.Start();
            }
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            FtpHelper.UserName = Context.Parameters["FtpUserName"];
            FtpHelper.Password = Context.Parameters["FtpPassword"];
            string sPath = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) ,"Test.txt");

            if (File.Exists(sPath))

                File.Delete(sPath);


            string data = string.Format("Username:{0},Password:{1}", Context.Parameters["FtpUserName"], Context.Parameters["FtpPassword"]);

            
            File.WriteAllText(sPath, data);
        }
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);   
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);          
        }
    }
}
