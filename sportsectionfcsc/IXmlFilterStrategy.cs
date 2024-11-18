using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportsectionfcsc
{
    public interface IXmlFilterStrategy
    {
        void FilterXml(Filters filters, string inputFilePath, string outputFilePath);
    }

}
