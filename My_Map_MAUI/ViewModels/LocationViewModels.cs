using My_Map_MAUI.Serviecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.ViewModels
{
    public partial class LocationViewModels : DataOfMapViewModels
    {
        public LocationViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "Location";
            IsBusy = false;
        }
    }
}
