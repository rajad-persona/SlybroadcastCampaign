using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttentiveSftpWeekly
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = new System.Data.DataTable();
            dt.Clear();
            dt.Columns.Add("Name");
            dt.Columns.Add("age");
            dt.Columns.Add("testcolumn1");
            dt.Columns.Add("testcolumn2");
            dt.Columns.Add("testcolumn3");
            dt.Columns.Add("testcolumn4");
            dt.Columns.Add("testcolumn5");
            dt.Columns.Add("testcolumn6");
            dt.Columns.Add("testcolumn7");
            dt.Columns.Add("testcolumn8");
            dt.Columns.Add("testcolumn9");
            dt.Columns.Add("testcolumn10");
            dt.Columns.Add("testcolumn11");
            dt.Columns.Add("testcolumn12");
            dt.Columns.Add("testcolumn13");
            dt.Columns.Add("testcolumn14");
            dt.Columns.Add("testcolumn15");
            dt.Columns.Add("testcolumn16");
            dt.Columns.Add("testcolumn17");
            dt.Columns.Add("testcolumn18");
           
            for (int i = 0; i <= 10000; i++)
            {
                System.Data.DataRow _ravi = dt.NewRow();
                _ravi["Name"] = "testdata";
                _ravi["age"] = "26";
                _ravi["testcolumn1"] = "testdata"+i.ToString();
                _ravi["testcolumn2"] = "testdata"+i.ToString();
                _ravi["testcolumn3"] = "testdata"+i.ToString();
                _ravi["testcolumn4"] = "testdata"+i.ToString();
                _ravi["testcolumn5"] = "testdata"+i.ToString();
                _ravi["testcolumn6"] = "testdata"+i.ToString();
                _ravi["testcolumn7"] = "testdata"+i.ToString();
                _ravi["testcolumn8"] = "testdata"+i.ToString();
                _ravi["testcolumn9"] = "testdata" + i.ToString();
                _ravi["testcolumn10"] = "testdata" + i.ToString();
                _ravi["testcolumn11"] = "testdata" + i.ToString();
                _ravi["testcolumn12"] = "testdata" + i.ToString();
                _ravi["testcolumn13"] = "testdata" + i.ToString();
                _ravi["testcolumn14"] = "testdata" + i.ToString();
                _ravi["testcolumn15"] = "testdata" + i.ToString();
                _ravi["testcolumn16"] = "testdata" + i.ToString();
                _ravi["testcolumn17"] = "testdata" + i.ToString();
                _ravi["testcolumn18"] = "testdata" + i.ToString();
                dt.Rows.Add(_ravi);
            }

            Utilities.Helper.FileUploadSFTP(dt, "testFile.csv");
        }
    }
}
