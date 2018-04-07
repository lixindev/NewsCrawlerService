using System;

namespace StrongCrawler
{
    public class News
    {
        public Guid Id;
        public string Title;
        public string Content;
        public string Editor;
        public string Address;
        public string Source;
        public Guid CrawlerConfigId;
        public long SpendTime;
        public int ThreadId;
        public DateTime? PublishTime;
    }
}
