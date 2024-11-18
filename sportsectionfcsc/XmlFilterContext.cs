using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportsectionfcsc
{
    public class XmlFilterContext
    {
        private IXmlFilterStrategy _strategy;

        public XmlFilterContext(IXmlFilterStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IXmlFilterStrategy strategy)
        {
            _strategy = strategy;
        }

        public void FilterXml(Filters filters, string inputFilePath, string outputFilePath)
        {
            _strategy.FilterXml(filters, inputFilePath, outputFilePath);
        }
    }

}
