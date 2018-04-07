using System;

namespace CrawlerService
{
    public class CrawlerPartConfig
    {
        public Guid Id;
        public Guid CrawlerConfigId;
        public int FetchModel;
        public string IndexLink_XPath;
        public string LoadMoreButton_XPath;
        public string PageNumInput_XPath;
        public string GoToButton_XPath;
        public int? FirstPage;
        public int? LastPage;
        public string ExtentionData1;
        public string ExtentionData2;
        public string ExtentionData3;
        public string ExtentionData4;
        public string ExtentionData5;
        public string Title_Xpath;
        public string Content_Xpath;
        public string Editor_Xpath;
        public string Source_Xpath;
        public string PublishTime_Xpath;
    }
}
