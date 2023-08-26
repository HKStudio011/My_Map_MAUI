using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxLengthAttribute = SQLite.MaxLengthAttribute;

namespace My_Map_MAUI.Models
{
    [Table(nameof(User))]
    public class User:ITableSQLite
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        [Unique]
        public string Email { get; set; }
        [NotNull]
        public string Password { get; set; }
        [NotNull]
        public UserState State { get; set; } // 0 chua kh,1 kh,2 kh co google driver
        public string Cloud { get; set; }

    }
}
