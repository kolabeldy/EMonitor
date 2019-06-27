using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EMonitor.DB;
namespace EMonitor.Frames
{
    class ReferencesModel : ViewModelBase
    {
        private MyDBContext db;
        private ObservableCollection<EnergyResource> _erList;
        public ObservableCollection<EnergyResource> ERList
        {
            get
            {
                return _erList;
            }
            set
            {
                _erList = value;
                RaisePropertyChanged(() => ERList);
            }
        }
        public List<Unit> UnitList { get; set; }
        public List<ViewTariffFull> TariffList { get; set; }

        public EnergyResource SelectedER { get; set; }

        public ReferencesModel()
        {
            ERList = new ObservableCollection<EnergyResource>();
            UnitList = new List<Unit>();
            UnitListFill();
            TariffList = new List<ViewTariffFull>();
            TariffListFill();
            ERListFill();
        }

        private void UnitListFill()
        {
            using (db = new MyDBContext())
            {
                db.Units.Load();
                UnitList = db.Units.Local.ToList();
            }
        }
        private void TariffListFill()
        {
            using (db = new MyDBContext())
            {
                db.ViewTariffFulls.Load();
                TariffList = db.ViewTariffFulls.Local.ToList();
            }
        }

        private void ERListFill()
        {
            using (db = new MyDBContext())
            {
                db.EnergyResources.Load();

                var qrySource = from o in db.EnergyResources.ToList()
                                select new EnergyResource
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Unit = o.Unit,
                                    IsPrime = o.IsPrime,
                                    IsMain = o.IsMain,
                                    IsActual = o.IsActual
                                };
                ERList.Clear();
                ERList = Global.ObservableCollection<EnergyResource>(qrySource);
            }
        }

        public ICommand SaveCommand { get { return new RelayCommand<int>(OnSave); } }
        private void OnSave(int numToEdit = 0)
        {
            using (db = new MyDBContext())
            {
                EnergyResource newER;
                foreach (EnergyResource er in ERList)
                {
                    newER = db.EnergyResources.Find(er.Id);
                    newER.Name = er.Name;
                    newER.Unit = er.Unit;
                    newER.IsMain = er.IsMain;
                }
                db.SaveChanges();
            }
        }


    }
}
