using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportsectionfcsc
{
    public struct Filters
    {
        public string Name { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public string Gender { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public List<string> Days { get; set; }

        public Filters()
        {
            Name = "";
            AgeFrom = null;
            AgeTo = null;
            Gender = "";
            TimeFrom = "";
            TimeTo = "";
            Days = new List<string>();
        }
    }
}
        

    
