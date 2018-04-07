using System;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.PhantomJS;
using StrongCrawler.Events;

namespace CrawlerService
{
    public interface ICrawler
    {
        event EventHandler<OnStartEventArgs> OnStart;//爬虫启动事件

        event EventHandler<OnCompletedEventArgs> OnCompleted;//爬虫完成事件

        event EventHandler<OnErrorEventArgs> OnError;//爬虫出错事件

        Task StartAsync(Uri uri, Script script, Operation operation, CrawlerPartConfig cpc); //异步爬虫

        PhantomJSDriver StartSync(Uri uri, Script script, Operation operation);//同步爬虫

        //Task StartAsync_New(Uri uri, Script script, Operation operation, CrawlerPartConfig cpc);

        //PhantomJSDriver StartSync_New(Uri uri, Script script, Operation operation);
    }
}
