using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.ViewModels
{
    public partial class ScheduleViewModels : DataOfMapViewModels
    {
        public ScheduleViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "Schedule";
            IsBusy = false;
            IsFirstTimes = true;
        }
    }
}
