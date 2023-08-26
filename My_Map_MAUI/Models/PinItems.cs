using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.Models
{
    public class PinItems : Items
    {
        public Microsoft.Maui.Devices.Sensors.Location Location { get; set; }
    }
}
