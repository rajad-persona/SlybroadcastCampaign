using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Extensions
    {
        public static byte[] ToCSV(this System.Data.DataTable table)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

            foreach (System.Data.DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }
            return Encoding.GetEncoding("iso-8859-1").GetBytes(result.ToString());
        }


      public  static DataTable ConvertListToDataTable(this List<string> data,string columnName)
        {
            DataTable dtTable = new DataTable();

            dtTable.Columns.Add(columnName, typeof(string));

            foreach (var row in data)
            {
                dtTable.Rows.Add(row);
            }

            return dtTable;
        }

    }
}
