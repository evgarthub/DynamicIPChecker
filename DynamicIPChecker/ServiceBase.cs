using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace DynamicIPChecker
{
    public partial class ServiceBase : System.ServiceProcess.ServiceBase
    {
        public string MyExternalIP { get; set; }
        public string MachineName { get; set; }

        public ServiceBase()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";

            this.MachineName = Environment.MachineName;
        }

        protected override void OnStart(string[] args)
        {

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["checkInterval"]);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(CheckExternalIP);
            timer.Start();

            eventLog1.WriteEntry("Dynamic IP Checker has been started");
        }
        
        protected override void OnStop()
        {
            eventLog1.WriteEntry("Dynamic IP Checker has been stopped");
        }

        protected override void OnShutdown()
        {
            eventLog1.WriteEntry("Computer is about to be turned off");
        }

        public void CheckExternalIP(object sender, System.Timers.ElapsedEventArgs args)
        {
            string currentIP = GetExternalIp();

            eventLog1.WriteEntry("Your current IP is: " + currentIP);

            if (this.MyExternalIP != currentIP)
            {
                SendResetEmail(currentIP);
                this.MyExternalIP = currentIP;
            }
        }

        public string GetExternalIp()
        {
            try
            {
                var externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                                    .Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch {
                eventLog1.WriteEntry("GetExternalIp(): Something went wrong while getting new IP address");
                return null;
            }

        }

        public void SendResetEmail(string ipAddress)
        {
            try
            {
                
                var email = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["emailFrom"])
                };

                email.To.Add(new MailAddress(ConfigurationManager.AppSettings["emailTo"]));
                email.Subject = ConfigurationManager.AppSettings["emailSubject"];
                email.IsBodyHtml = true;
                email.Body = String.Format(ConfigurationManager.AppSettings["emailBodyTemplate"], this.MachineName, ipAddress);
                var smtpClient = new SmtpClient
                {
                    EnableSsl = true
                };

                smtpClient.Send(email);
            }
            catch
            {
                eventLog1.WriteEntry("SendResetEmail(): Something went wrong while sending email");
            }
        }
    }
}
