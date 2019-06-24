using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class ViewLossesFact : ViewModelBase
    {
        public int Id { get; set; }
        public string DatePeriod { get; set; }
        public string Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int IdOrganization { get; set; }
        public int IdCostCenter { get; set; }
        public int IdEnergyResource { get; set; }
        public string ERName { get; set; }
        public string UnitName { get; set; }
        public double FactLoss { get; set; }
        public double FactLossCost { get; set; }
        public double FactUse { get; set; }
        public double FactTotal { get; set; }
        public double FactTotalCost { get; set; }
        public double Tariff { get; set; }
        public int IdDepartMade { get; set; }
        public double ProcLoss { get; set; }
    }
}
