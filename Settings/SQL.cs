using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyEPOS.Settings
{
    class SQL
    {
        public static string IPForConnection { get; set; } = "NEBULA\\SQLEXPRESS";
        public static string PortForConnection { get; set; } = "";
        public static string ConnectionString { get; set; } = $"Data Source={IPForConnection};Initial Catalog={Utility.SQL.DatabaseToExecuteAgainst};Integrated Security=True; User ID=SA; Password=123456";
    }
}
