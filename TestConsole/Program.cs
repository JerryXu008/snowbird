﻿using AutoTestSystem.DAL;
using Checkroute;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO.Ports;

namespace TestConsole
{
    internal class Program
    {
        public static Mescheckroute mescheckroute;

        public static List<string> GetColumnData(string filePath, int columnIndex)
        {
            var list = new  List<string>();
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // 创建工作簿实例
                IWorkbook workbook = new XSSFWorkbook(file);

                // 获取第一个工作表
                ISheet sheet = workbook.GetSheetAt(0);

                // 获取要读取的列的索引（例如，第一列为0）
             
                // 遍历工作表中的所有行
                for (int rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    // 获取当前行
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null)
                    {
                        continue;
                    }
                    // 获取当前单元格
                    ICell cell = row.GetCell(columnIndex);

                    if (cell == null) {
                        continue;
                    }

                    // 读取单元格内容并输出
                    string cellValue = cell.ToString();
                   // System.Console.WriteLine(cellValue);

                    if (cellValue != null && cellValue != "") {
                        if (cellValue.StartsWith("WIFI") || cellValue.StartsWith("BLE") || cellValue.StartsWith("ZB")) { 
                        
                            list.Add(cellValue);
                        
                        }
                    
                    }

                    int ii = 0;


                }
            }

            return list;
        }


        /// <summary>
        /// Merci MBFT 生成
        /// </summary>
       static void CreateCSVDataMerciAndSnowBirdMBFT() {

            var list = GetColumnData("./test.xlsx", 0);
            var finalCSV = "./testFinal.csv";
            var sb = new StringBuilder();
            for (var i = 0; i < list.Count; i++)
            {

                //处理wifi
                var name = list[i];
                if (name.StartsWith("WIFI"))
                {

                    var arr = name.Split('_');
                    //WIFI_TX_FREQERR_F5180_HE_BW20_MCS0_C0
                    //WIFI_RX_POWER_F5290_HE_BW80_MCS0_P-82-100_C0
                    if (arr.Length != 8 && arr.Length != 9 && arr.Length != 10)
                    {
                        Console.WriteLine($"{name} 长度不对");


                        continue;
                    }


                    // ,WIFI_TX_POWER_F5955_HE_BW20_MCS0_C1,WIFI_TX_POWER_F5955_HE_BW20_MCS0_C1,y,6G_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,Tx,1,,,,5955, HE_SU-MCS0,,,,,,,,,,,,


                    if (name.Contains("_TEMP_"))
                    {

                        //WIFI_TX_CPU_TEMP_F2472_HE_BW20_MCS0_C0
                        var flag = name.Contains("C0") ? "0" : "1";
                        var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                        var f = arr[4].Replace('F', ' ').Trim();
                        var type = arr[5] + "_" + (arr[5] == "HE" ? "SU" : "MU") + "-" + arr[7];

                        string pattern = "-P-\\d+";
                        string replacement = "";

                        type = Regex.Replace(type, pattern, replacement);




                        var g5org2 = "";
                        if (f.StartsWith("2"))
                        {
                            g5org2 = "2G";
                        }
                        else
                        {
                            g5org2 = "5G";
                        }

                        var line = $",{name},{name},y,{g5org2}_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

                        sb.AppendLine(line);








                       
                    }
                    else if (name.Contains("USER1") || name.Contains("USER2")) {

                        var flag = name.Contains("C0") ? "0" : "1";
                        var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                        var f = arr[4].Replace('F', ' ').Trim();
                        //var type = arr[5] + "_" + (arr[5] == "HE" ? "SU" : "MU") + "-" + arr[8];


                        var g5org2 = "";
                        if (f.StartsWith("2"))
                        {
                            g5org2 = "2G";
                        }
                        else
                        {
                            g5org2 = "5G";
                        }

                        var type = "NA";

                        var line = $",{name},{name},y,{g5org2}_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

                        sb.AppendLine(line);
                    }
                    else
                    {
                        var flag = name.Contains("C0") ? "0" : "1";
                        var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                        var f = arr[3].Replace('F', ' ').Trim();
                        var type = arr[4] + "_" + (arr[4] == "HE" ? "SU" : "MU") + "-" + arr[6];

                        string pattern = "-P-\\d+";
                        string replacement = "";

                        type = Regex.Replace(type, pattern, replacement);
                        var g5org2 = "";
                        if (f.StartsWith("2"))
                        {
                            g5org2 = "2G";
                        }
                        else
                        {
                            g5org2 = "5G";
                        }





                        var line = $",{name},{name},y,{g5org2}_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

                        sb.AppendLine(line);

                    }


                }

                else if (name.StartsWith("BLE") || name.StartsWith("ZB"))
                {
                    var arr = name.Split('_');
                    //,BLE_RX_PER_F2402_P-91,BLE_RX_PER_F2402_P-91,y,BLE,1.4.1.1: Equipment.DUT.Initiate,0,3,Rx,,,,,2402,,,,,,,,,,,,,
                    //,ZB_TX_POWER_F2480,ZB_TX_POWER_F2480,y,Zigbee,1.4.1.1: Equipment.DUT.Initiate,0,3,Tx,,,,,2480,,,,,,,,,ZB_TX_POWER_F2405,,,,

                    if (arr.Length != 4 && arr.Length != 5)
                    {
                        Console.WriteLine($"{name} 长度不对");


                        continue;
                    }
                    var t = name.StartsWith("BLE") ? "BLE" : "Zigbee";
                    var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                    var f = arr[3].Replace('F', ' ').Trim();
                    var line = $",{name},{name},y,{t},1.4.1.1: Equipment.DUT.Initiate,0,3,{TxOrRx},,,,,{f},,,,,,,,,,,,,";
                    sb.AppendLine(line);
                }
                else
                {
                    Console.WriteLine($"{name} 格式不对");
                }

            }
            File.WriteAllText(finalCSV, sb.ToString());
        }

        //static void CreateCSVDataSnowbirdMBFT()
        //{

        //    var list = GetColumnData("./test.xlsx", 0);
        //    var finalCSV = "./testFinal.csv";
        //    var sb = new StringBuilder();
        //    for (var i = 0; i < list.Count; i++)
        //    {

        //        //处理wifi
        //        var name = list[i];
        //        if (name.StartsWith("WIFI"))
        //        {

        //            var arr = name.Split('_');

        //            if (arr.Length != 7 && arr.Length != 8 && arr.Length != 9)
        //            {
        //                Console.WriteLine($"{name} 长度不对");


        //                continue;
        //            }


        //            // ,WIFI_TX_POWER_F5955_BW20_MCS0_C1,WIFI_TX_POWER_F5955_BW20_MCS0_C1,y,6G_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,Tx,1,,,,5955, HE_SU-MCS0,,,,,,,,,,,,


        //            if (!name.Contains("_TEMP_"))
        //            {
        //                var flag = name.Contains("C0") ? "0" : "1";
        //                var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
        //                var f = arr[3].Replace('F', ' ').Trim();
        //                var type = "HE" + "_" +  "SU" + "-" + arr[5];
        //                var line = $",{name},{name},y,5G_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

        //                sb.AppendLine(line);
        //            }
        //            else
        //            {
        //                //WIFI_TX_CPU_TEMP_F2472_BW20_MCS0_C0
        //                var flag = name.Contains("C0") ? "0" : "1";
        //                var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
        //                var f = arr[3].Replace('F', ' ').Trim();
        //                var type = "HE" + "_" +  "SU"   + "-" + arr[5];
        //                var line = $",{name},{name},y,5G_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

        //                sb.AppendLine(line);

        //            }


        //        }

        //        else if (name.StartsWith("BLE") || name.StartsWith("ZB"))
        //        {
        //            var arr = name.Split('_');
        //            //,BLE_RX_PER_F2402_P-91,BLE_RX_PER_F2402_P-91,y,BLE,1.4.1.1: Equipment.DUT.Initiate,0,3,Rx,,,,,2402,,,,,,,,,,,,,
        //            //,ZB_TX_POWER_F2480,ZB_TX_POWER_F2480,y,Zigbee,1.4.1.1: Equipment.DUT.Initiate,0,3,Tx,,,,,2480,,,,,,,,,ZB_TX_POWER_F2405,,,,

        //            if (arr.Length != 4 && arr.Length != 5)
        //            {
        //                Console.WriteLine($"{name} 长度不对");


        //                continue;
        //            }
        //            var t = name.StartsWith("BLE") ? "BLE" : "Zigbee";
        //            var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
        //            var f = arr[3].Replace('F', ' ').Trim();
        //            var line = $",{name},{name},y,{t},1.4.1.1: Equipment.DUT.Initiate,0,3,{TxOrRx},,,,,{f},,,,,,,,,,,,,";
        //            sb.AppendLine(line);
        //        }
        //        else
        //        {
        //            Console.WriteLine($"{name} 格式不对");
        //        }

        //    }
        //    File.WriteAllText(finalCSV, sb.ToString());
        //}


        static void CreateCSVDataSnowbirdSRF()
        {

            var list = GetColumnData("./test.xlsx", 0);
            var finalCSV = "./testFinal.csv";
            var sb = new StringBuilder();
            for (var i = 0; i < list.Count; i++)
            {

                //处理wifi
                var name = list[i];
                if (name.StartsWith("WIFI"))
                {

                    var arr = name.Split('_');

                    if (arr.Length != 7 && arr.Length != 8)
                    {
                        Console.WriteLine($"{name} 长度不对");


                        continue;
                    }


                    // ,WIFI_TX_POWER_F5955_BW20_MCS0_C1,WIFI_TX_POWER_F5955_BW20_MCS0_C1,y,6G_RadioValidation,1.4.1.1: Equipment.DUT.Initiate,0,3,Tx,1,,,,5955, HE_SU-MCS0,,,,,,,,,,,,


                    if (!name.Contains("_TEMP_"))
                    {
                        var flag = name.Contains("C0") ? "0" : "1";
                        var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                        var f = arr[3].Replace('F', ' ').Trim();
                        //var type = "HE" + "_" + "SU" + "-" + arr[5];
                        var type = "";
                        var g5org2 = f.StartsWith("2") ? "2G" : "5G";
                        var line = $",{name},{name},y,{g5org2},1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

                        sb.AppendLine(line);
                    }
                    else
                    {
                        //WIFI_TX_CPU_TEMP_F2472_BW20_MCS0_C0
                        var flag = name.Contains("C0") ? "0" : "1";
                        var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                        var f = arr[3].Replace('F', ' ').Trim();
                        //var type = "HE" + "_" + "SU" + "-" + arr[6];
                        var type = "";
                        var g5org2 = f.StartsWith("2") ? "2G" : "5G";
                        var line = $",{name},{name},y,{g5org2},1.4.1.1: Equipment.DUT.Initiate,0,3,{ TxOrRx },{flag},,,,{f}, {type},,,,,,,,,,,,";

                        sb.AppendLine(line);

                    }


                }

                else if (name.StartsWith("BLE") || name.StartsWith("ZB"))
                {
                    var arr = name.Split('_');
                    //,BLE_RX_PER_F2402_P-91,BLE_RX_PER_F2402_P-91,y,BLE,1.4.1.1: Equipment.DUT.Initiate,0,3,Rx,,,,,2402,,,,,,,,,,,,,
                    //,ZB_TX_POWER_F2480,ZB_TX_POWER_F2480,y,Zigbee,1.4.1.1: Equipment.DUT.Initiate,0,3,Tx,,,,,2480,,,,,,,,,ZB_TX_POWER_F2405,,,,

                    if (arr.Length != 4 && arr.Length != 5)
                    {
                        Console.WriteLine($"{name} 长度不对");


                        continue;
                    }
                    var t = name.StartsWith("BLE") ? "BLE" : "Zigbee";
                    var TxOrRx = name.Contains("TX") ? "Tx" : "Rx";
                    var f = arr[3].Replace('F', ' ').Trim();
                    var line = $",{name},{name},y,{t},1.4.1.1: Equipment.DUT.Initiate,0,3,{TxOrRx},,,,,{f},,,,,,,,,,,,,";
                    sb.AppendLine(line);
                }
                else
                {
                    Console.WriteLine($"{name} 格式不对");
                }

            }
            File.WriteAllText(finalCSV, sb.ToString());
        }



        static void TestOthers() {
            //try {

            //    mescheckroute = new Mescheckroute("10.90.122.68");

            //    if (mescheckroute.CheckEeroTest("GGC1UCD133030G1G", "production", out string mesMsg) && mesMsg.Contains("OK"))
            //{
            //    Console.WriteLine(mesMsg);
            //}
            //else
            //{
            //    Console.WriteLine(mesMsg);

            //}
            //}
            //catch(Exception ex) {
            //    Console.WriteLine(ex.ToString());
            //}

            // TestPort();
            //var rec = "";
            //var ff = SendCommand("iperf3 -c 192.168.0.20 -i1 -t10 -O2 -P8 -R", ref rec, "iperf Done", 20000);
            //int ii = 0;
        }

      static  void POESwtichPortSpeed() {
            var cookies = new CookieContainer();
            var client = GetClient(cookies, "admin", "admin123");
            string url = $@"http://169.254.100.101/api/v1/service";
            //string data = $"{{\"method\":\"poe.status.powerin.get\",\"params\":[],\"id\":80}}";

            //string data = $"{{\"method\":\"poe.status.interface.get\",\"params\":[],\"id\":138}}";

            var dict = new Dictionary<string, object>();
            /*
             
             {
  "method": "port.config.set",
  "params": [
    "2.5G 1/1",
    {
      "AdvertiseDisabled": 977,
      "ExcessiveRestart": false,
      "FC": "off",
      "FrameLengthCheck": false,
      "GeneratePause": false,
      "MTU": 9018,
      "MediaType": "rj45",
      "ObeyPause": false,
      "PFC": 0,
      "Shutdown": false,
      "Speed": "force1GModeFdx"
    }
  ],
  "id": 29
}
             
             */

            var type = "force1GModeFdx"; // "force2G5ModeFdx"   "force1GModeFdx"  "force100ModeFdx" "force100ModeHdx"

            dict.Add("method", "port.config.set");
            dict.Add("id",240);
            List<object> list = new List<object>();
            dict.Add("params", list);
            list.Add("2.5G 1/9");
            var dict2 = new Dictionary<string, object>();
            list.Add(dict2);
            
            dict2.Add("ExcessiveRestart", false);
            dict2.Add("FC", "off");
            dict2.Add("FrameLengthCheck", false);
            dict2.Add("GeneratePause", false);
            dict2.Add("MTU", 9018);
            dict2.Add("MediaType", "rj45");
            dict2.Add("ObeyPause", false);
            dict2.Add("PFC", 0);
            dict2.Add("Shutdown", false);
            dict2.Add("Speed", type);
            if (type == "force2G5ModeFdx") {
                dict2.Add("AdvertiseDisabled", 969);
               
            }
            else if (type == "force1GModeFdx") {
                dict2.Add("AdvertiseDisabled", 977);
            }
            else if (type == "force100ModeFdx")
            {
                dict2.Add("AdvertiseDisabled", 969);
            }
            else if (type == "force100ModeHdx")
            {
                dict2.Add("AdvertiseDisabled", 921);
            }

          

                string data = JsonConvert.SerializeObject(dict);





            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;
            var response_content = result.Content.ReadAsByteArrayAsync().Result;
            var responseStr = System.Text.Encoding.UTF8.GetString(response_content);

            if (result.IsSuccessStatusCode && responseStr.Contains("\"error\":null"))
            {
                Console.WriteLine("set success:" + responseStr);
            }
            else { 
              Console.WriteLine("set fail:" + responseStr);
            }

            Console.WriteLine("");
        }
        public static HttpClient GetClient(CookieContainer cookies, string username, string password)
        {
            var handler = new HttpClientHandler() { CookieContainer = cookies, UseCookies = false };
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("User-Agent", "GetLimit");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            //client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            string strLoginPassword = string.Format("{0}:{1}", username, password);
            byte[] bytLoginPassword = System.Text.Encoding.UTF8.GetBytes(strLoginPassword);
            string strLoginPasswordEncoded = Convert.ToBase64String(bytLoginPassword);
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {strLoginPasswordEncoded}");
            return client;
        }
       static void ControlPOE() {
            var cookies = new CookieContainer();
            var client = GetClient(cookies, "admin", "admin123");
            string url = $@"http://169.254.100.101/api/v1/service";
            //string data = $"{{\"method\":\"poe.status.powerin.get\",\"params\":[],\"id\":80}}";

            string data = $"{{\"method\":\"poe.status.interface.get\",\"params\":[],\"id\":138}}";
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;
            var response_content = result.Content.ReadAsByteArrayAsync().Result;
            var responseStr = System.Text.Encoding.UTF8.GetString(response_content);

            Root root = JsonConvert.DeserializeObject<Root>(responseStr);

            foreach (var result2 in root.result)
            {
                Console.Write(result2.val.CurrentConsumption + "   ");
            }

            Console.WriteLine("");

           // Console.WriteLine(responseStr);
        }

        static void EnablePOE()
        {
            var cookies = new CookieContainer();
            var client = GetClient(cookies, "admin", "admin123");
            string url = $@"http://169.254.100.101/api/v1/service";
            //string data = $"{{\"method\":\"poe.status.powerin.get\",\"params\":[],\"id\":80}}";

            

            string data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"2.5G 1/{8}\",{{\"Mode\":\"{"poeDot3af"}\",\"Priority\":\"low\",\"Lldp\":\"enable\",\"MaxPower\":15,\"Structure\":\"2Pair\"}}],\"id\":164}}";



            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;
            var response_content = result.Content.ReadAsByteArrayAsync().Result;
            var responseStr = System.Text.Encoding.UTF8.GetString(response_content);

           

            Console.WriteLine("");

            // Console.WriteLine(responseStr);
        }

       static void TestDiGui(ref int ii) {
            try
            {


                if (ii == 5000)
                {
                    return;
                }
                ii++;
                //  Console.WriteLine("当前：" + ii);
                TestDiGui(ref ii);
            }
            catch (Exception e) { 
            
            }
        }

       static void TestTailDiGui(int n) {
            Console.WriteLine(n);
            if (n == 50000) {
                return;
               
            }
            TestTailDiGui(++n);
        }
        public static bool NoCheckStr ( string str, string substr)
        {
            if (string.IsNullOrEmpty(substr))
                return true;
            else
            {
                if (str.Contains(substr))
                {
                     
                    return false;
                }
                else
                    return true;
            }
        }

        public static string RunDosCmd(string command, int timeout = 0)
        {
            Console.WriteLine($"DosSendComd-->{command}");
            using (var p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c " + command;    //命令运行之后窗口关闭
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                var error = "";
                p.ErrorDataReceived += (sender, e) => { error += e.Data; };
                p.Start();
                p.BeginErrorReadLine();                     //获取cmd窗口的输出信息
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(timeout * 1000);
                p.Close();
                Console.WriteLine(output + error);
                return output + error;
            }
        }
        public static string GetMidStr(string sourse, string startStr, string endStr)
        {
            var result = string.Empty;
            try
            {
                if (startStr != null)
                {
                    var startIndex = sourse.IndexOf(startStr, StringComparison.Ordinal);
                    if (startIndex == -1)
                        return result;
                    var tmpStr = sourse.Substring(startIndex + startStr.Length);
                    if (endStr != null)
                    {
                        var endIndex = tmpStr.IndexOf(endStr, StringComparison.Ordinal);
                        if (endIndex == -1)
                            return result;
                        result = tmpStr.Remove(endIndex);
                    }
                    else
                    {
                        return tmpStr;
                    }
                }
                else
                {
                    var tmpStr = sourse;
                    if (endStr != null)
                    {
                        var endIndex = tmpStr.IndexOf(endStr, StringComparison.Ordinal);
                        if (endIndex == -1)
                            return result;
                        result = tmpStr.Remove(endIndex);
                    }
                }
            }
            catch (Exception ex)
            {
              
                //throw ex;
            }
            return result;
        }

        static void TestLoopLeakValue()
        {


            DateTime startTime = DateTime.Now;
            var rReturn = false;
            while ((DateTime.Now - startTime).TotalSeconds < 60)
            {

                var cmd = @"python ./testZaoSheng.py -p COM8 -cmd 0103000C00014409";
                string re = RunDosCmd(cmd, 2);

                var data = GetMidStr(re, "[", "]");
                if (data.Length >= 14)
                {
                    //data = data.Substring(6, 4);
                    //int int_value = Convert.ToInt32(data, 16);

                    //var value = System.Convert.ToString(int_value, 10);
                    //var value1 = (double.Parse(value) / 10).ToString();

                    if (data.Contains("46") || data.Contains("47"))
                    {

                        rReturn = true;
                         break;
                    }

                    Thread.Sleep(1000);
                }
            }

            if (rReturn)
            {
                Console.WriteLine("治具测试完成，开始读值..");

                var cmd = @"python ./testZaoSheng.py -p COM8 -cmd 010316dc000201b9";
                string re = RunDosCmd(cmd, 2);
                Console.WriteLine("气压值L" + re);
                var data = GetMidStr(re, "[", "]");
                if (data.Length >= 18)
                {




                    string hexNumber = data.Substring(6, 8);  // 提取 "05aa0000"
                    Console.WriteLine("提取出数据:" + hexNumber);


                     var   subString = hexNumber.Substring(4, 4) + hexNumber.Substring(0, 4);

                    // subString = "F6D0FFFF";

                    int decimalNumber = Convert.ToInt32(subString, 16);  // 将十六进制字符串转换为十进制数

                    Console.WriteLine("转换后数据:" + decimalNumber);
                }
                else {
                    Console.WriteLine("提取数据:" + data + " 有问题");
                }


            }
            else
            {
                Console.WriteLine("测试超时");
            }
        }



            // Console.WriteLine("python return:" + re);



            //var data =GetMidStr(re, "[", "]");
            //if (data.Length >= 18)
            //{
            //    data = data.Substring(6, 4);
            //    int int_value = Convert.ToInt32(data, 16);

            //    var value = System.Convert.ToString(int_value, 10);
            //    var value1 = (double.Parse(value) / 10).ToString();
            //    EnvTemp = value1;
            
            //    rReturn = true;





            //}






            static void Main(string[] args)
        {




           //var  subString = "00000600";

           //int decimalNumber = Convert.ToInt32(subString, 16);

           //int ff = 0;




            //Task.Run(() => {

            //    TestLoopLeakValue();

            //});

           

            // ControlPOE();

            //Console.WriteLine("sleep 10s");
            //Thread.Sleep(10000);

            //ControlPOE();

            //Console.WriteLine("sleep 10s");
            //Thread.Sleep(10000);


            //ControlPOE();



            
              CreateCSVDataMerciAndSnowBirdMBFT();

            // CreateCSVDataSnowbirdSRF();
            // POESwtichPortSpeed();

            //  TestPort();




            Console.WriteLine("生成完毕!!!");




            while (1 == 1)
            {

            }
        }




        public static PingReply Ping(string address, int retryCount = 50)
        {
            Ping ping = null;
            try
            {
                ping = new Ping();
                return ping.Send(address, 2000);
            }
            catch (PingException ex)
            {
                Console.WriteLine($"ping {address} ：Fail！！！！！");
                Thread.Sleep(3000);

                if (retryCount <= 1)
                {
                    return null;
                }

                return Ping(address, retryCount - 1);
            }
            finally
            {
                if (ping != null)
                {
                    IDisposable disposable = ping;
                    disposable.Dispose();
                    ping.Dispose();
                }
            }
        }

        /// <summary>
        /// 每秒Ping IP地址一次，ping通立即返回true，超过times后返回失败
        /// </summary>
        public static bool PingIP(string address, int times, bool flag = false)
        {
            bool rResult = false;
            for (int i = times; i > 0; i--)
            {
                //if (i % 4 == 0 && flag)
                //    RunDosCmd("arp -d & exit");
                var pingReply = Ping(address);
                if (pingReply != null && pingReply.Status == 0)
                {
                    Console.WriteLine($"Reply from {pingReply.Address}：bytes={pingReply.Buffer.Length} time={pingReply.RoundtripTime} TTL={pingReply.Options.Ttl} {pingReply.Status}");
                    rResult = true;
                    break;
                }
                else
                {

                    var rr = pingReply != null ? pingReply.Status : IPStatus.Unknown;

                    Console.WriteLine($"ping {address} ：{rr}");
                    if (i == 1)
                    {
                        Console.WriteLine($"ping {address} ：Fail！！！！！");
                        rResult = false;
                    }
                }
                Thread.Sleep(1000);
            }
            return rResult;
        }













        public static   bool SendCommand(string command, ref string strRecAll, string DataToWaitFor, int timeout = 3000)
        {
            bool rResult = false;
            
            //说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (var p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c " + command;
                p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //p.StartInfo.WorkingDirectory = directoryPath;
                var error = "";
                p.ErrorDataReceived += (sender, e) => { error += e.Data; };
                p.Start();
                //p.StandardInput.WriteLine(command);
                //p.StandardInput.AutoFlush = true;
                //p.StandardInput.Close();
                p.BeginErrorReadLine(); //获取cmd窗口的输出信息
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(timeout * 1000);
                p.Close();
                 
                if (output.Contains(DataToWaitFor))
                {
                    
                    rResult = true;
                }
                else
                {
                    // Timed out.
                     
                }
                strRecAll = output;
            }
            return rResult;
        }




        static void TelnetTest() {
            //测试telnet
            var telnetInfo = new TelnetInfo { _Address = "192.168.1.1" };
            var DUTCOMM = new Telnet(telnetInfo);
            DUTCOMM.Open("root@OpenWrt:/#");
            var revStr = "";
            DUTCOMM.SendCommand("xxxxx", ref revStr, "luxshare SW Version :", 120);

           
        }


        static void TestPort() {

             //  var  DUTCOMinfo = new SerialConnetInfo { PortName = "COM1", BaudRate = 115200 };
              
             //  var DUTCOMM = new Comport(DUTCOMinfo);
             //  DUTCOMM.Open();

             //  var revStr = "";
             //DUTCOMM.SendCommand("AT+CC_DET%", ref revStr, "%END", 120);
             //Console.WriteLine(">>>>>>>>>>>>>>>:"+revStr);

            //DUTCOMM.SendCommand("dmesg | grep 'mmcblk0' | head -1 | awk '{print $5}'", ref revStr, "root@OpenWrt:/#", 120);
            //var Value =  GetValue(revStr, "'{print $5}'", "root");

             var revStr = @"TP56=0.095
%END";

            Console.WriteLine("最终取值:" + GetValue(revStr, "T56=", "%END"));





            int ii = 0;
        }
        public static string GetValue(string revStr, string SubStr1, string SubStr2)
        {
            var TestValue = "";
            if (!string.IsNullOrEmpty(SubStr1) || !string.IsNullOrEmpty(SubStr2)) //需要提取TestValue
            {
                TestValue = GetSubStringOfMid(revStr, SubStr1, SubStr2);
                if (string.IsNullOrEmpty(TestValue))
                   Console.WriteLine("Error! Get TestValue IsNullOrEmpty.");
                else
                    Console.WriteLine($"GetTestValue:{TestValue}");
            }

            return TestValue;
        }

        public static string GetSubStringOfMid(string souce, string sub1, string sub2)
        {
            int p1 = -1, p2, start = -1, length = 0;

            if (!string.IsNullOrEmpty(sub1))
            {
                p1 = souce.IndexOf(sub1);
                start = p1 + sub1.Length;
            }
            else
            {
                start = 0;
                p1 = 0; //从字符串的0位置开始截取
            }

            if (!string.IsNullOrEmpty(sub2))
                p2 = souce.IndexOf(sub2, start); //从sub1位置开始查找sub2
            else
                return souce.Substring(start).Replace("\n", "").Replace("\t", "").Replace("\r", "")
                .Replace("\r\n", "").Trim(); //一直截取到字符串末尾

            if (p1 == -1 || p2 == -1) //找不到字符串返回空
                return null;

            length = p2 - start;
            return souce.Substring(start, length).Replace("\n", "").Replace("\t", "").Replace("\r", "")
                .Replace("\r\n", "").Trim();
        }


    }
}
