using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrongCrawler
{
    public class Target
    {
        public string Name { get => _name; set => _name = value; }
        private string _name;
        public string Uri { get => _uri; set => _uri = value; }
        public Operation Operation { get => _operation; set => _operation = value; }
        private string _uri;
        private Operation _operation;

        private CrawlerPartConfig _cpc;
        public CrawlerPartConfig CPC { get => _cpc; set => _cpc = value; }
    }
}
