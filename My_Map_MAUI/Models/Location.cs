using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.Models
{
    [Table(nameof(Location))]
    public class Location : ITableSQLite
    {
        [PrimaryKey, AutoIncrement, Indexed] 
        public int Id { get; set; }
        [NotNull]
        public int ItemId { get; set; }
        [NotNull]
        public double Latitude { get; set; }
        [NotNull]
        public double Longitude { get; set; }
        
    }
}
