using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrawlerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerService.Tests
{
    [TestClass()]
    public class NewsCrawlerTests
    {
        [TestMethod()]
        public void MissionStartTest()
        {
            NewsCrawler nc = new NewsCrawler();
            nc.MissionStart();
        }
    }
}