using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.Models
{
    public class ItemType : ITableSQLite
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }
        [NotNull]
        public MapItemType MapItemType { get; set; }
        public PinType PinType { get; set; }
        public string StrokeColor { get; set; }
        public float StrokeWidth { get; set; }
        public string FillColor { get; set;}
        public double Radius { get; set; }

    }
}
