using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.Models
{
    [Table(nameof(Detail))]
    public class Detail :ITableSQLite
    {
        [PrimaryKey,Indexed]
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AvgPrice { get; set; }
        public float Rating { get; set; }
        public string OpeningTimes { get; set; }
        public string ClosingTimes { get; set; }

    }
}
