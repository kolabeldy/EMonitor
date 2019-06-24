using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class ViewResult : ViewModelBase
    {
        public int Id { get; set; }
        public string Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int IdCostCenter { get; set; }
        public int IdDepart { get; set; }
        public int IdCategory { get; set; }
        public int IsAnalysis { get; set; }
        public int IdEnergyResource { get; set; }
        public int IsMain { get; set; }
        public string DepartCategory { get; set; }
        public string ResourceName { get; set; }
        public string UnitName { get; set; }
        public double Plan { get; set; }
        public double Fact { get; set; }
        public double Difference { get; set; }
        public double PlanCost { get; set; }
        public double FactCost { get; set; }
        public double DifferenceCost { get; set; }
        public int IdAlter { get; set; }
        public int IsFlag { get; set; }

    }
}
