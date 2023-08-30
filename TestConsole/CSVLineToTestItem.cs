using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class CSVLineToTestItem
    {

        //GGC3510233370103_P1_Cal_ver_sweep_Test_Report
        static String[] csvLines;
        static void LineToTestItem() {

            using (var sr = new StreamReader("./GGC3510233370103_P1_Cal_ver_sweep_Test_Report.csv"))
            {
                csvLines = sr.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                //开始组装wifi 测试项






            }

        }
    }
}
