using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace CrawlerService
{
    public class NewsCrawler
    {
        StrongCrawler sc;
        public SemaphoreSlim Semaphore;
        //private int MaxThread = Environment.ProcessorCount + 1;

        public void MissionStart()
        {
            GetConfig gc = new GetConfig();
            List<CrawlerConfig> List_cc = gc.GetCrawlerConfig();
            Dictionary<string, string> dic = gc.GetSysConfig();
            int MaxThread = Convert.ToInt32(dic["ThreadNum"]);
            Semaphore = new SemaphoreSlim(MaxThread, MaxThread);
            foreach (CrawlerConfig cc in List_cc)
            {
                GetCrawlerPartConfig(cc.CrawlerConfigId, cc.Address);
            }
            Semaphore.Dispose();
            ClearOldNews();
        }

        private void GetCrawlerPartConfig(Guid CrawlerConfigId, string Address)
        {
            GetConfig gc = new GetConfig();
            sc = new StrongCrawler(Semaphore);
            sc.OnError += (s, e) =>
            {
                WriteLog.InsertLogs(e.Uri.ToString(), e.Exception.Message);
            };
            sc.OnCompleted += (s, e) =>
            {
                NewsOperator newsOpera = new NewsOperator();
                newsOpera.InsertNews(e.News);
            };
            List<CrawlerPartConfig> List_cpc = gc.GetCrawlerPartConfig(CrawlerConfigId);
            List<Target> Targets = new List<Target>();
            foreach (CrawlerPartConfig cpc in List_cpc)
            {
                StartSync(cpc, Address,ref Targets);
            }
            FetchDataAsync(Targets);

        }

        private void StartSync(CrawlerPartConfig cpc, string Address,ref List<Target> List_tar)
        {
            try
            {
                Uri uri = new Uri(Address);
                if (cpc.FetchModel == 1)
                {
                    PhantomJSDriver driver = sc.StartSync(uri, null, null);
                    var titlecollection = driver.FindElements(By.XPath(cpc.IndexLink_XPath));
                    foreach (var item in titlecollection)
                    {
                        if (!string.IsNullOrEmpty(item.Text))
                        {
                            Target t = new Target
                            {
                                Name = item.Text,
                                Uri = item.GetAttribute("href"),
                                Operation = null,
                                CPC = cpc
                            };
                            List_tar.Add(t);
                        }
                    }
                    driver.Quit();
                }
                else if (cpc.FetchModel == 3)
                {
                    Operation opera = new Operation
                    {
                        Action = (x) =>
                        {
                            x.FindElement(By.XPath(cpc.LoadMoreButton_XPath)).Click();
                        },
                        Condition = (x) =>
                        {
                            return !x.FindElement(By.XPath(cpc.ExtentionData1)).Displayed;
                        },
                        Timeout = 10000,
                        ExtentionData1 = cpc.ExtentionData1,
                        ExtentionData2 = cpc.ExtentionData2,
                        ExtentionData3 = cpc.ExtentionData3
                    };
                    PhantomJSDriver driver = sc.StartSync(uri, null, opera);
                    var titlecollection = driver.FindElements(By.XPath(cpc.IndexLink_XPath));
                    foreach (var item in titlecollection)
                    {
                        if (!string.IsNullOrEmpty(item.Text))
                        {
                            Target t = new Target
                            {
                                Name = item.Text,
                                Uri = item.GetAttribute("href"),
                                Operation = null,
                                CPC = cpc
                            };
                            List_tar.Add(t);
                        }
                    }
                    driver.Quit();
                }
                else if (cpc.FetchModel == 5)
                {
                    Script script = new Script()
                    {
                        Code = "document.body.scrollTop=10000",
                        Condition = (x) =>
                        {
                            return !x.FindElement(By.XPath(cpc.ExtentionData1)).Displayed;
                        },
                        ActionTimes = Convert.ToInt32(cpc.ExtentionData4),
                        FetchModel = 5,
                        Timeout = 10000
                    };
                    PhantomJSDriver driver = sc.StartSync(uri, script, null);
                    var titlecollection = driver.FindElements(By.XPath(cpc.IndexLink_XPath));
                    foreach (var item in titlecollection)
                    {
                        if (!string.IsNullOrEmpty(item.Text))
                        {
                            Target t = new Target
                            {
                                Name = item.Text,
                                Uri = item.GetAttribute("href"),
                                Operation = null,
                                CPC = cpc
                            };
                            List_tar.Add(t);
                        }
                    }
                    driver.Quit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FetchDataAsync(List<Target> tarlist)
        {
            List<Task> _tasks = new List<Task>();
            foreach (Target tar in tarlist)
            {  
                GetConfig gc = new GetConfig();
                var newsUrl = tar.Uri.Substring(0, tar.Uri.IndexOf(tar.Uri)+1);//定义爬虫入口URL
                if (!gc.CheckIsExists(newsUrl))
                {
                    Task _task = sc.StartAsync(new Uri(newsUrl), null, tar.Operation, tar.CPC);
                    _tasks.Add(_task);
                }
            }
            Task.WaitAll(_tasks.ToArray());
        }

        private void ClearOldNews()
        {
            GetConfig gc = new GetConfig();
            NewsOperator newsOpera = new NewsOperator();
            int Period = Convert.ToInt32(gc.GetSysConfig()["Period"]);
            DateTime DeadLine = Convert.ToDateTime(DateTime.Now.AddDays(Period * -1).ToString("yyyy-MM-dd"));
            newsOpera.DeleteNews(DeadLine);
        }
    }
}
