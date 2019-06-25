using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EMonitor.DB
{
    class MyDBContext : DbContext
    {
        public MyDBContext() : base("DefaultConnection")
        {
            Database.SetInitializer<MyDBContext>(null);
        }
        public virtual DbSet<EnergyMonthUse> EnergyMonthUses { get; set; }
        public virtual DbSet<CostCenter> CostCenters { get; set; }
        public virtual DbSet<ViewResult> ViewResults { get; set; }
        public virtual DbSet<ViewYear> ViewYears { get; set; }
        public virtual DbSet<EnergyResource> EnergyResources { get; set; }
        public virtual DbSet<ViewFabricateUse> ViewFabricateUses { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<ViewLossesFact> ViewLossesFacts { get; set; }
        public virtual DbSet<ViewTariff> ViewTariffs { get; set; }
        public virtual DbSet<ViewERPrice> ViewERPrices { get; set; }
        public virtual DbSet<Losse> Losses { get; set; }
    }
}
