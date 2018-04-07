using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerService
{
    public partial class ClubCrawlerService : ServiceBase
    {
        private bool isStart = false;
        public ClubCrawlerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartCrawler();
        }

        protected override void OnStop()
        {

        }

        private void StartCrawler()
        {
            System.Timers.Timer timer = new System.Timers.Timer(30000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Execute);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        private void Execute(object source, System.Timers.ElapsedEventArgs e)
        {
            string dtNow = DateTime.Now.ToString("HH:mm");
            GetConfig gc = new GetConfig();
            Dictionary<string, string> dic = gc.GetSysConfig();
            string StartTime = dic["StartTime"];
            if (dtNow.Substring(3, 2) == StartTime.Substring(3,2) && !isStart)
            {
                isStart = true;
                try
                {
                    NewsCrawler nc = new NewsCrawler();
                    nc.MissionStart();
                }
                catch(Exception ex)
                {
                    WriteLog.InsertLogs("", ex.Message);
                }
                finally
                {
                    isStart = false;
                }
            }
        }
    }
}
