using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using StrongCrawler.Events;

namespace StrongCrawler
{
    public class StrongCrawler: ICrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;//爬虫启动事件

        public event EventHandler<OnCompletedEventArgs> OnCompleted;//爬虫完成事件

        public event EventHandler<OnErrorEventArgs> OnError;//爬虫出错事件

        public SemaphoreSlim Semaphore;
        //private int MaxThread = Environment.ProcessorCount + 1;

        public StrongCrawler(SemaphoreSlim semaphore)
        {
            Semaphore = semaphore;
        }

        /// <summary>
        /// 异步创建爬虫
        /// </summary>
        /// <param name="uri">爬虫URL地址</param>
        /// <param name="proxy">代理服务器</param>
        /// <returns>网页源代码</returns>
        public async Task StartAsync(Uri uri, Script script, Operation operation, CrawlerPartConfig cpc)
        {
            await Task.Run(() =>
            {
                try
                {
                    Semaphore.Wait();
                    OnStart?.Invoke(this, new OnStartEventArgs(uri));
                    var _service = PhantomJSDriverService.CreateDefaultService();
                    var _option = new PhantomJSOptions();
                    var driver = new PhantomJSDriver(_service, _option);

                    try
                    {
                        var watch = DateTime.Now;
                        driver.Navigate().GoToUrl(uri.ToString());
                        if (script != null)
                        {
                            driver.ExecuteScript(script.Code, script.Args);
                        }
                        if (operation != null)
                        {
                            ExecuteAction(operation, driver);
                        }

                        var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                        var milliseconds = DateTime.Now.Subtract(watch).Milliseconds;
                        News news = GetNews(ref driver, ref cpc, ref uri, ref threadId, ref milliseconds);
                        OnCompleted?.Invoke(this, new OnCompletedEventArgs(news));
                        //OnCompleted?.Invoke(this, new OnCompletedEventArgs(uri, threadId, milliseconds, pageSource, driver, cpc));
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(this, new OnErrorEventArgs(uri, ex));
                    }
                    finally
                    {
                        if (driver != null)
                        {
                            driver.Quit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new OnErrorEventArgs(uri, ex));
                }
                finally
                {
                    Semaphore.Release();
                }
            });
        }

        /// <summary>
        /// 创同步建爬虫
        /// </summary>
        /// <param name="uri">爬虫URL地址</param>
        /// <param name="proxy">代理服务器</param>
        /// <returns>网页源代码</returns>
        public PhantomJSDriver StartSync(Uri uri, Script script, Operation operation)
        {
            try
            {
                var _service = PhantomJSDriverService.CreateDefaultService();
                var _option = new PhantomJSOptions();
                var driver = new PhantomJSDriver(_service, _option);

                var watch = DateTime.Now;
                driver.Navigate().GoToUrl(uri.ToString());
                if (script != null)
                {
                    ExecuteScript(script, driver);
                }
                if (operation != null)
                {
                    ExecuteAction(operation, driver);
                }

                var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                var milliseconds = DateTime.Now.Subtract(watch).Milliseconds;
                //var pageSource = driver.PageSource;
                return driver;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ExecuteScript(Script script, PhantomJSDriver driver)
        {
            int times = 1;
            if (script.FetchModel == 5)
            {
                times = script.ActionTimes;
            }
            for (int i=0;i<times;i++)
            {
                driver.ExecuteScript(script.Code, script.Args);
                var driverWait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(script.Timeout));
                if (script.Condition != null)
                {
                    driverWait.Until(script.Condition);
                }
            }
        }

        private void ExecuteAction(Operation operation, PhantomJSDriver driver)
        {
            if (driver.FindElement(By.XPath(operation.ExtentionData2)).Displayed && !driver.FindElement(By.XPath(operation.ExtentionData3)).Displayed)
            {
                operation.Action?.Invoke(driver);
                var driverWait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(operation.Timeout));
                if (operation.Condition != null)
                {
                    driverWait.Until(operation.Condition);
                }
                ExecuteAction(operation, driver);
            }
            else if (driver.FindElement(By.XPath(operation.ExtentionData3)).Displayed)
            {
                return;
            }
        }

        private News GetNews(ref PhantomJSDriver driver, ref CrawlerPartConfig cpc, ref Uri uri, ref int threadId, ref int milliseconds)
        {
            string title = driver.FindElement(By.XPath(cpc.Title_Xpath)).Text;
            var contentcollection = driver.FindElements(By.XPath(cpc.Content_Xpath));
            string content = string.Empty;
            foreach (var item in contentcollection)
            {
                content += item.Text;
            }
            string source = string.Empty;
            string editor = string.Empty;
            DateTime? publishtime = null;
            if (!string.IsNullOrEmpty(cpc.Source_Xpath))
            {
                source = driver.FindElement(By.XPath(cpc.Source_Xpath)).Text;
            }
            if (!string.IsNullOrEmpty(cpc.Editor_Xpath))
            {
                editor = driver.FindElement(By.XPath(cpc.Editor_Xpath)).Text;
            }
            if (!string.IsNullOrEmpty(cpc.PublishTime_Xpath))
            {
                publishtime = Convert.ToDateTime(driver.FindElement(By.XPath(cpc.PublishTime_Xpath)).Text.TrimStart().Substring(0, 16));
            }

            News news = new News()
            {
                Id = Guid.NewGuid(),
                CrawlerConfigId = cpc.CrawlerConfigId,
                Title = title,
                Content = content,
                Editor = editor,
                Source = source,
                Address = uri.ToString(),
                SpendTime = milliseconds,
                ThreadId = threadId,
                PublishTime = publishtime
            };

            return news;
        }

        ///// <summary>
        ///// 异步创建爬虫
        ///// </summary>
        ///// <param name="uri">爬虫URL地址</param>
        ///// <param name="proxy">代理服务器</param>
        ///// <returns>网页源代码</returns>
        //public async Task StartAsync_New(Uri uri, Script script, Operation operation, CrawlerPartConfig cpc)
        //{
        //    await Task.Run(() =>
        //    {
        //        try
        //        {
        //            Semaphore.Wait();
        //            OnStart?.Invoke(this, new OnStartEventArgs(uri));
        //            var _service = PhantomJSDriverService.CreateDefaultService();
        //            var _option = new PhantomJSOptions();
        //            var driver = new PhantomJSDriver(_service, _option);

        //            try
        //            {
        //                var watch = DateTime.Now;
        //                driver.Navigate().GoToUrl(uri.ToString());
        //                if (script != null)
        //                {
        //                    driver.ExecuteScript(script.Code, script.Args);
        //                }
        //                driver.FindElement(By.XPath("//input[@id='pagenav']")).SendKeys("2");
        //                operation.Action?.Invoke(driver);
        //                var driverWait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(operation.Timeout));
        //                if (operation.Condition != null)
        //                {
        //                    driverWait.Until(operation.Condition);
        //                }

        //                var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        //                var milliseconds = DateTime.Now.Subtract(watch).Milliseconds;
        //                var pageSource = driver.PageSource;
        //                OnCompleted?.Invoke(this, new OnCompletedEventArgs(uri, threadId, milliseconds, pageSource, driver, cpc));
        //            }
        //            catch (Exception ex)
        //            {
        //                OnError?.Invoke(this, new OnErrorEventArgs(uri, ex));
        //            }
        //            finally
        //            {
        //                if (driver != null)
        //                {
        //                    driver.Close();
        //                    driver.Quit();
        //                    driver.Dispose();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            OnError?.Invoke(this, new OnErrorEventArgs(uri, ex));
        //        }
        //        finally
        //        {
        //            Semaphore.Release();
        //            //Console.WriteLine("释放信号量，信号量：" + Semaphore.CurrentCount);
        //        }
        //    });
        //}

        ///// <summary>
        ///// 创同步建爬虫
        ///// </summary>
        ///// <param name="uri">爬虫URL地址</param>
        ///// <param name="proxy">代理服务器</param>
        ///// <returns>网页源代码</returns>
        //public PhantomJSDriver StartSync_New(Uri uri, Script script, Operation operation)
        //{
        //    try
        //    {
        //        var _service = PhantomJSDriverService.CreateDefaultService();
        //        var _option = new PhantomJSOptions();
        //        var driver = new PhantomJSDriver(_service, _option);

        //        var watch = DateTime.Now;
        //        driver.Navigate().GoToUrl(uri.ToString());
        //        if (script != null)
        //        {
        //            driver.ExecuteScript(script.Code, script.Args);
        //        }
        //        driver.FindElement(By.XPath("//input[@id='pagenav']")).SendKeys("2");
        //        operation.Action?.Invoke(driver);
        //        var driverWait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(operation.Timeout));
        //        if (operation.Condition != null)
        //        {
        //            driverWait.Until(operation.Condition);
        //        }

        //        var threadId = Thread.CurrentThread.ManagedThreadId;
        //        var milliseconds = DateTime.Now.Subtract(watch).Milliseconds;
        //        var pageSource = driver.PageSource;
        //        return driver;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
