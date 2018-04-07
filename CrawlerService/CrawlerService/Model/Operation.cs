using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace CrawlerService
{
    public class Operation
    {
        public delegate void DriverAction<T>(T arg);
        private DriverAction<IWebDriver> action;
        private Func<IWebDriver, bool> condition;
        private double timeout;
        private string extentiondata1;
        private string extentiondata2;
        private string extentiondata3;

        public double Timeout { get => timeout; set => timeout = value; }
        public Func<IWebDriver, bool> Condition { get => condition; set => condition = value; }
        public DriverAction<IWebDriver> Action { get => action; set => action = value; }
        public string ExtentionData1 { get => extentiondata1; set => extentiondata1 = value; }
        public string ExtentionData2 { get => extentiondata2; set => extentiondata2 = value; }
        public string ExtentionData3 { get => extentiondata3; set => extentiondata3 = value; }
    }
}
