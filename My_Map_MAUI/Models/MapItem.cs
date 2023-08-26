using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = SQLite.TableAttribute;

namespace My_Map_MAUI.Models
{
    [Table(nameof(MapItem))]
    public class MapItem: ITableSQLite
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set ; }
        [NotNull]
        public string Email { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public MapItiemState State { get; set; }
    }
}
