using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.Models
{
    internal class Schedule
    {
        public string Name { get; set; }
        public ObservableCollection<DateTime> DateTimes { get; set; }
        public ObservableCollection<TimeSpan> TimeSpans { get; set; }
        public ObservableCollection<PinItems> PinItems { get; set; }

    }
}
