using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class EnergyMonthUse : ViewModelBase
    {
        public long Id { get; set; }
        //public string Period { get; set; }
        public string Period { get; set; }
        public int IdDepartMade { get; set; }
        public int IdEnergyResource { get; set; }
        public int IdOrganization { get; set; }
        public int IdCostCenter { get; set; }
        public int IdProduct { get; set; }
        public double Fact { get; set; }
        public double Plan { get; set; }
        public double Fabricate { get; set; }

    }
}
