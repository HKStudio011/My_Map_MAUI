using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.Models
{
    [Table(nameof(Image))]
    public class Image : ITableSQLite
    {
        [PrimaryKey,AutoIncrement, Indexed]
        public int Id { get; set; }
        [NotNull]
        public int ItemId { get; set; }
        public string CurrentPath { get; set; }
        public string OriginPath { get; set; }
    }
}
