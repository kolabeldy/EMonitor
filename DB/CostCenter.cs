using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMonitor.DB
{
    public class CostCenter : ViewModelBase
    {
        public int Id { get; set; }
        public int IdDepart { get; set; }
        public int IsMain { get; set; }

    }
}
