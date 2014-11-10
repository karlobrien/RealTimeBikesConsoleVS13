using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeBikesConsole
{
    public class ContractsDO
    {
        public string Name { get; set; }
        public string CommercialName { get; set; }
        public string CountryCode { get; set; }
        public List<string> Cities { get; set; } 
    }
}
