using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace CrawlerService
{
    public class Script
    {
        private string code;
        private object[] args;
        public string Code { get => code; set => code = value; }
        public object[] Args { get => args; set => args = value; }

        private Func<IWebDriver, bool> condition;
        private double timeout;
        private int fetchmodel;
        private int actiontimes;

        public double Timeout { get => timeout; set => timeout = value; }
        public Func<IWebDriver, bool> Condition { get => condition; set => condition = value; }
        public int FetchModel { get => fetchmodel; set => fetchmodel = value; }
        public int ActionTimes { get => actiontimes; set => actiontimes = value; }
    }
}
