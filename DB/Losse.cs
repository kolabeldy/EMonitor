using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class Losse : ViewModelBase
    {
        public int Id { get; set; }
        public string DatePeriod { get; set; }
        public int IdOrganization { get; set; }
        public int IdCostCenter { get; set; }
        public double Fact { get; set; }
        public int IdEnergyResource { get; set; }
        public int IdDepartMade { get; set; }

    }
}
