using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.PhantomJS;

namespace StrongCrawler.Events
{
    /// <summary>
    /// 爬虫完成事件
    /// </summary>
    public class OnCompletedEventArgs : EventArgs
    {
        //public Uri Uri { get; private set; }// 爬虫URL地址
        //public int ThreadId { get; private set; }// 任务线程ID
        //public string PageSource { get; private set; }// 页面源代码
        //public long Milliseconds { get; private set; }// 爬虫请求执行事件
        //public PhantomJSDriver Driver { get; private set; }// PhantomJS WebDriver
        //public CrawlerPartConfig CPC { get; private set; }// XPath设置

        public News News { get; private set; }

        public OnCompletedEventArgs(News news)
        {
            this.News = news;
        }

        //public OnCompletedEventArgs(Uri uri, int threadId, long milliseconds, string pageSource, PhantomJSDriver driver, CrawlerPartConfig cpc)
        //{
        //    this.Uri = uri;
        //    this.ThreadId = threadId;
        //    this.Milliseconds = milliseconds;
        //    this.PageSource = pageSource;
        //    this.Driver = driver;
        //    this.CPC = cpc;
        //}
    }
}
