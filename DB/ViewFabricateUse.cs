using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class ViewFabricateUse
    {
        public long Id { get; set; }
        public string Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int IdProduct { get; set; }
        public string ERName { get; set; }
        public string UnitName { get; set; }
        public double Fabricate { get; set; }
        public double Fact1 { get; set; }
        public double Fact0 { get; set; }
        public double Loss { get; set; }

    }
}
