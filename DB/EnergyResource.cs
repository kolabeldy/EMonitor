using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class EnergyResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IsMain { get; set; }
        public int IsPrime { get; set; }
        public int IsActual { get; set; }
        public int IdAlter { get; set; }
        public int Unit { get; set; }
    }
}
