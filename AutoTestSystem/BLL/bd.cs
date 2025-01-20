using AutoTestSystem.Model;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using KAutoHelper;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using PDUSPAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace AutoTestSystem.BLL
{
    public static class Bd
    {
        public static ConcurrentQueue<string> SummStr = new ConcurrentQueue<string>();
        public static ILog logger = Log4NetHelper.GetLogger(typeof(Bd));

        public static string TempCellLogPath = "\\Temp.txt";

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, IntPtr lpString, int nMaxCount);



        /// <summary>
        ///     创建初始文件夹,如果不存在.
        /// </summary>
        public static void CheckFolder(string path)
        {
            try
            {
                if (Directory.Exists(path)) return;
                Directory.CreateDirectory(path);
                loggerInfo($"Create Directory {path} succeed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///     创建初始文件夹,如果不存在.
        /// </summary>
        public static bool CreateFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.Create(path);
                loggerInfo($"Create file {path} succeed.");
                return true;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///     修改ToolStripItem:调用方法：
        ///     myToolStripLabel.InvokeOnToolStripItem(label => label.Text = "Updated!");
        ///     myToolStripProgressBar.InvokeOnToolStripItem(bar => bar.PerformStep());
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="action"></param>
        public static void InvokeOnToolStripItem<T>(this T item, Action<T> action)
            where T : ToolStripItem
        {
            var parent = item.GetCurrentParent();
            if (parent.InvokeRequired)
                parent.Invoke(action, item);
            else
                action(item);
        }

        public static bool CheckStr(this string str, string substr)
        {
            if (str.Contains(substr))
            {
                if (!string.IsNullOrEmpty(substr))
                {
                    loggerInfo($"Check Contain substr:{substr} ,pass.");
                }
                return true;
            }
            else
            {
                loggerError($"Check Contain substr:{substr} ,fail..");
                return false;
            }
        }
        public static Dictionary<string, object> JsonToDictionary(string jsonData)
        {
            //实例化JavaScriptSerializer类的新实例
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象
                return jss.Deserialize<Dictionary<string, object>>(jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void loggerDebug(string txt)
        {

            if (Global.SUPERSAVETIME == "1")
            {
                string newText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [DEBUG]" + " - " + txt;
                SummStr.Enqueue(newText);
            }
            else
            {
                logger.Debug(txt);
            }

            SaveLog(txt);


        }
        public static void loggerInfo(string txt)
        {
            if (Global.SUPERSAVETIME == "1")
            {
                string newText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [INFO]" + " - " + txt;
                SummStr.Enqueue(newText);
            }
            else
            {
                loggerInfo(txt);

            }
            SaveLog(txt);

        }
        public static void loggerWarn(string txt)
        {
            if (Global.SUPERSAVETIME == "1")
            {
                string newText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [WARN]" + " - " + txt;
                SummStr.Enqueue(newText);
            }
            else
            {
                logger.Warn(txt);
            }
            SaveLog(txt);

        }
        public static void loggerError(string txt)
        {
            if (Global.SUPERSAVETIME == "1")
            {
                string newText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [ERROR]" + " - " + txt;
                SummStr.Enqueue(newText);
            }
            else
            {
                logger.Error(txt);
            }
            SaveLog(txt);

        }
        public static void loggerFatal(string txt)
        {
            if (Global.SUPERSAVETIME == "1")
            {
                string newText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [FATAL]" + " - " + txt;
                SummStr.Enqueue(newText);
            }
            else
            {
                logger.Fatal(txt);
            }

            SaveLog(txt);

        }


        public static void MonitorClickZhuBo(string processName)
        {

            string windows_title = null;
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                {
                    IntPtr mainWindowHandle = process.MainWindowHandle;
                    if (mainWindowHandle != IntPtr.Zero)
                    {
                        const int nChars = 256;
                        IntPtr buffer = Marshal.AllocHGlobal(nChars * 2);
                        GetWindowText(mainWindowHandle, buffer, nChars);
                        windows_title = Marshal.PtrToStringUni(buffer);
                        Marshal.FreeHGlobal(buffer);
                    }
                }
                else
                {
                    //loggerDebug("can not find " + processName);
                }
            }

            IntPtr hWnd = IntPtr.Zero;
            hWnd = AutoControl.FindWindowHandle(null, windows_title);
            if (hWnd != null && hWnd != IntPtr.Zero)
            {
                var childhWnd = AutoControl.FindHandle(hWnd, null, "Stop");
                if (childhWnd != null && childhWnd != IntPtr.Zero)
                {
                    loggerDebug($"模拟{processName} 点击Stop");

                    AutoControl.SendClickOnControlByHandle(childhWnd);
                }
            }
            else
            {

                loggerDebug($"未找到{processName} 句柄");
            }



        }

        public static void DisableUSB()
        {

            //禁用USB
            const string keyName = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\USBSTOR";
            const string valueName = "Start";


            try
            {
                // 设置注册表键值为 4，禁用 USB 存储设备
                Registry.SetValue(keyName, valueName, 4, RegistryValueKind.DWord);
                loggerInfo("usb disabled ok");

            }
            catch (Exception ex)
            {
                loggerInfo("usb disabled Error:" + ex.Message);

            }


        }

        public static void SaveLog(string log, int type = 1)
        {

            if (Global.STATIONNAME != "BURNIN")
            {
                return;
            }

            try
            {

                using (StreamWriter sw = new StreamWriter(Global.LogPath + "\\" + TempCellLogPath, true, Encoding.Default))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {log}");
                }

            }
            catch (Exception)
            {
                //throw ex;
            }
        }



        public static bool NoCheckStr(this string str, string substr)
        {
            if (string.IsNullOrEmpty(substr))
                return true;
            else
            {
                if (str.Contains(substr))
                {
                    loggerInfo($"Check NoContain substr:{substr} ,fail...");
                    return false;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// 替换字符串中字符cellLogPath.Replacea(new char[] { '\\', '/', ':', '*', '?', '<', '>', '|', '"' });
        /// </summary>
        /// <param name="str"></param>
        /// <param name="oldStr"></param>
        /// <param name="newStr"></param>
        /// <returns></returns>
        public static string ReplaceStr(this string str, string[] oldStr, string newStr = " ")
        {
            foreach (var item in oldStr)
            {
                str = str.Replace(item, newStr);
            }
            return str;
        }
        public static string ReplaceStr(this string str, char[] oldStr, char newStr = ' ')
        {
            foreach (var item in oldStr)
            {
                str = str.Replace(item, newStr);
            }
            return str;
        }
        /// <summary>
        ///     压缩文件
        /// </summary>
        /// <param name="filePath">需要压缩的文件夹全路径</param>
        /// <param name="zipPath">压缩后文件的全路径</param>
        /// <param name="deletedAfterCompress">压缩后是否删除原文件</param>
        /// <returns></returns>
        public static bool CompressFile(string filePath, string zipPath, bool deletedAfterCompress = false)
        {
            loggerDebug($"zipPath:{zipPath}");
            var rReturn = false;
            var compCommand = $@"{System.Environment.CurrentDirectory}\7z.exe a -tzip {zipPath} {filePath}"; //压缩DOS指令
            if (RunDosCmd(compCommand).Contains("Everything is Ok"))
            {
                rReturn = true;
                if (deletedAfterCompress) Directory.Delete(filePath);
            }
            return rReturn;
        }



        static string GetNetName(string ip)
        {
            var str = RunDosCmd("ipconfig/all");

            var arr = Regex.Split(str, "\r\n");
            var ret = "";
            for (var i = 0; i < arr.Length; i++)
            {
                var ss = arr[i];
                if (ss.Contains("IPv4 地址") && ss.Contains(ip))
                { //找到了
                    loggerDebug(">>>>>>find NetInterface:" + ss);
                    while (i >= 0)
                    {
                        i--;
                        var temp = arr[i];
                        if (!temp.StartsWith(" ") && temp.Length > 0)
                        {
                            temp = Regex.Replace(temp, "以太网适配器 ", "");
                            temp = Regex.Replace(temp, ":", "");
                            temp = Regex.Replace(temp, " ", "\" \"");
                            ret = temp;
                            break;
                        }
                    }

                    break;
                }

            }

            if (ret == "")
            {
                //loggerInfo("ip地址:" + ip + " 没有找到,开始固定网口名称查找");
                ret = "0.10";
            }
            return ret;
        }
        public static void NetshInterfaceEnable(string netName)
        {
            //var name = Global.NetInterfaceName;
            var name = netName;
            RunDosCmd("netsh interface set interface " + name + " enabled");
        }
        public static void NetshInterfaceDisEnable(string netName)
        {
            //var name = GetNetName(ip);
            //Global.NetInterfaceName = name;
            var name = netName;
            RunDosCmd("netsh interface set interface " + name + " disabled");
        }


        /// <summary>
        ///     写SN到文件并复制到ATE目标路径
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="sourcefilePath">源文件绝对路径</param>
        /// <param name="destfilePath">目标文件绝对路径</param>
        /// <returns></returns>
        public static bool WriteSNandMoveFile(string sn, string sourcefilePath, string destfilePath)
        {
            var rReturn = false;
            try
            {
                if (File.Exists(sourcefilePath)) File.Delete(sourcefilePath);
                using (var sw = new StreamWriter(sourcefilePath, false, Encoding.Default))
                {
                    sw.WriteLine(sn);
                }
                var passToPath = destfilePath;
                if (File.Exists(passToPath)) File.Delete(passToPath);
                File.Move(sourcefilePath, passToPath);
                rReturn = true;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                rReturn = false;
            }
            return rReturn;
        }

        ///// <summary>
        /////     等待并读取csv文件
        ///// </summary>
        ///// <param name="timeOut">超时时间</param>
        ///// <param name="filepath">文件绝对路径</param>
        ///// <param name="csvLines">csv文件内容</param>
        ///// <returns></returns>
        //public static bool WaitingCSVlog(string timeOut, string filepath, string str, out string[] csvLines)
        //{
        //    //if (Global.STATIONNAME == "SRF")
        //    //{
        //    //    Sleep(120000);
        //    //}
        //    //else {
        //    //    Sleep(3000);
        //    //}

        //   // 
        //    var rReturn = false;
        //    var lngStart = DateTime.Now.AddSeconds(int.Parse(timeOut)).Ticks;
        //    var dir = new DirectoryInfo(filepath);
        //    while (DateTime.Now.Ticks <= lngStart)
        //    {
        //        // 返回目录中所有文件和子目录
        //        if (dir.GetFileSystemInfos().Length > 0)
        //        {
        //            loggerDebug($"find log number:{dir.GetFileSystemInfos().Length}.");
        //            var files = Directory.GetFileSystemEntries(filepath);
        //            foreach (var file in files)
        //            {
        //                if (file.Contains(str) && file.EndsWith(".csv"))
        //                {
        //                    loggerDebug($"find csv File:{file}");
        //                    //Thread.Sleep(3000);
        //                    using (var sr = new StreamReader(file))
        //                    {
        //                        csvLines = sr.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //                    }

        //                    return true;
        //                }
        //            }
        //        }
        //        Thread.Sleep(1000);
        //    }
        //    loggerError($"Waiting csv log timeout{timeOut}.FAIL!!! ");
        //    csvLines = null;
        //    return rReturn;
        //}



        public static bool WaitingCSVlog(string timeOut, string filepath, string str, string exeName, out string[] csvLines)
        {
            var rReturn = false;
            var lngStart = DateTime.Now.AddSeconds(int.Parse(timeOut)).Ticks;
            var dir = new DirectoryInfo(filepath);
            int checkInterval = 10000; // 10 seconds in milliseconds
            var lastCheckTime = DateTime.Now;



            while (DateTime.Now.Ticks <= lngStart)
            {

                if (exeName != "")
                {
                    // 每10秒检查一次 QCATestSuite.exe 是否存在
                    if ((DateTime.Now - lastCheckTime).TotalMilliseconds >= checkInterval)
                    {
                        lastCheckTime = DateTime.Now;

                        // 使用 Process 启动 cmd，并执行 tasklist | findstr QCATestSuite
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/c tasklist | findstr {exeName}",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();

                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        // 如果没有找到 QCATestSuite.exe 进程，则返回 false
                        if (string.IsNullOrEmpty(output))
                        {
                            loggerError($"{exeName}.exe process not found. Exiting...");
                            csvLines = null;
                            return false;
                        }
                    }

                }

                // 返回目录中所有文件和子目录
                if (dir.GetFileSystemInfos().Length > 0)
                {
                    loggerDebug($"find log number:{dir.GetFileSystemInfos().Length}.");
                    var files = Directory.GetFileSystemEntries(filepath);
                    foreach (var file in files)
                    {
                        if (file.Contains(str) && file.EndsWith(".csv"))
                        {
                            loggerDebug($"find csv File:{file}");
                            using (var sr = new StreamReader(file))
                            {
                                csvLines = sr.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            }
                            return true;
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            loggerError($"Waiting csv log timeout {timeOut}. FAIL!!! ");
            csvLines = null;
            return rReturn;
        }









        public static bool WaitingCSVlog2(string timeOut, string filepath, string str, out string[] csvLines)
        {

            //  Sleep(3000);
            var rReturn = false;
            var lngStart = DateTime.Now.AddSeconds(int.Parse(timeOut)).Ticks;
            var dir = new DirectoryInfo(filepath);
            while (DateTime.Now.Ticks <= lngStart)
            {
                // 返回目录中所有文件和子目录
                if (dir.GetFileSystemInfos().Length > 0)
                {
                    loggerDebug($"find log number:{dir.GetFileSystemInfos().Length}.");
                    var files = Directory.GetFileSystemEntries(filepath);
                    foreach (var file in files)
                    {
                        var name = Path.GetFileName(file);

                        if (name.Contains(str) && name.EndsWith(".csv"))
                        {
                            loggerDebug($"find csv File:{file}");
                            //Thread.Sleep(3000);
                            using (var sr = new StreamReader(file))
                            {
                                csvLines = sr.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            }

                            return true;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
            loggerError($"Waiting csv log timeout{timeOut}.FAIL!!! ");
            csvLines = null;
            return rReturn;
        }

        #region 蓝牙

        /// <summary>
        ///     搜索蓝牙并配对
        /// </summary>
        /// <param name="devName"></param>
        /// <param name="devAddress"></param>
        /// <param name="searchRetry">搜索蓝牙重试次数</param>
        /// <returns></returns>
        public static bool BTConnection(string devName, string devAddress, int searchRetry)
        {
            var rReturn = false;
            var retry = 0;
            try
            {
                loggerDebug("Searching Blue-tooth......");
                var bluetoothClient = new BluetoothClient();
                BluetoothRadio.PrimaryRadio.Mode = RadioMode.Discoverable;
                var Inquiry_Time = TimeSpan.FromSeconds(5);
                bluetoothClient.InquiryLength = Inquiry_Time;
                while (retry <= searchRetry)
                {
                    retry++;
                    //BluetoothDeviceInfo[] bluetoothDeviceInfo = bluetoothClient.DiscoverDevices(15, false, false, false, true);
                    var bluetoothDeviceInfo = bluetoothClient.DiscoverDevices(15);

                    loggerDebug(
                        $"Find {bluetoothDeviceInfo.Length} Bluetooth.Target Bluetooth is:{devName}, Bt_mac:{devAddress}.");
                    for (var i = 0; i < bluetoothDeviceInfo.Length; i++)
                    {
                        loggerDebug($"this name of BT{i}: {bluetoothDeviceInfo[i].DeviceName}");
                        if (bluetoothDeviceInfo[i].DeviceName == devName
                            && bluetoothDeviceInfo[i].DeviceAddress == BluetoothAddress.Parse(devAddress))
                        {
                            loggerDebug($"Target Bluetooth is founded: {devName},Ready to match.");
                            var bdi = new BluetoothDeviceInfo(bluetoothDeviceInfo[i].DeviceAddress);
                            if (!bdi.Authenticated)
                            {
                                //string pair = rd.Pin; /* PIN for your dongle */
                                rReturn = BluetoothSecurity.PairRequest(bdi.DeviceAddress, "0000");
                                loggerDebug($"Bluetooth pair {(rReturn ? "succeed" : "failed")}！");
                                if (rReturn) return true;
                            }
                        }
                    }

                    Thread.Sleep(10);
                }

                loggerError($"Target Bluetooth not been founded：{devName}！");
            }
            catch (Exception ex)
            {
                loggerFatal("Search Bluetooth Exception:" + ex);
                //throw;
            }
            finally
            {
                if (rReturn)
                {
                    if (BluetoothSecurity.RemoveDevice(BluetoothAddress.Parse(devAddress)))
                        loggerInfo("BT Remove Device Success !!");
                    else
                        loggerError("BT Remove Device Fail !!");
                }
                //else
                //{
                //    using (Comport DutCom = new Comport(DUTCOMinfo, ""))
                //    {//配对失败后，重启蓝牙，retry。
                //        string recvStr = "";
                //        DutCom.SendCommand2("3 0", ref recvStr, "", 1);
                //        DutCom.SendCommand2("4 0", ref recvStr, "", 1);
                //        Thread.Sleep(1000);
                //        DutCom.SendCommand2("2 0", ref recvStr, "Open connection", 1);
                //    }
                //}
            }

            return rReturn;
        }

        #endregion 蓝牙

        /// <summary>
        ///     对象序列化成Json文件并保存
        /// </summary>
        /// <param name="station">序列化对象</param>
        /// <param name="writeJsonPath">保存成Json文件的路径</param>
        /// <returns></returns>
        public static bool JsonSerializer(object station, out string JsonStr, string writeJsonPath = "", bool printJson = true)
        {
            var jsonClintContent = "";
            var result = false;
            try
            {
                // 序列化Json 配置，忽略值为null字段
                var setting = new JsonSerializerSettings();
                JsonConvert.DefaultSettings = () =>
                {
                    setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    setting.DateFormatString = "yyyy-MM-dd HH:mm:ss"; //!日期类型默认格式化处理
                    setting.NullValueHandling = NullValueHandling.Ignore; //!空值处理,忽略空值
                    // setting.DefaultValueHandling = DefaultValueHandling.Ignore;
                    setting.Formatting = Formatting.Indented;
                    return setting;
                };
                jsonClintContent =
                   JsonConvert.SerializeObject(station, setting)
                   .Replace("\"units\": \"\"", "\"units\": null");


                //替换不对的时间格式
                var newstr = new StringBuilder();

                var content = jsonClintContent;

                var contentArr = Regex.Split(content, "\r\n");

                for (var i = 0; i < contentArr.Length; i++)
                {
                    var line = contentArr[i];
                    if (line.Contains("0001-01-01 00:00:00") && line.Contains("start_time"))
                    {
                        var nextLine = contentArr[i + 1];
                        if (nextLine.Contains("finish_time"))
                        {

                            line = Regex.Replace(nextLine, "finish_time", "start_time");
                        }

                    }

                    if (i != contentArr.Length - 1)
                    {
                        newstr.AppendLine(line + "\r\n");
                    }
                    else
                    {
                        newstr.AppendLine(line + "");
                    }
                }

                jsonClintContent = newstr.ToString();

                if (printJson)
                {
                    loggerDebug(jsonClintContent);
                }
                if (!string.IsNullOrEmpty(writeJsonPath))
                {
                    File.WriteAllText(writeJsonPath, jsonClintContent);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                loggerFatal(ex.ToString());
            }
            JsonStr = jsonClintContent;
            return result;
        }

        /// <summary>
        ///     使用本地映射盘的方式操作共享文件夹,并复制文件到共享文件夹
        /// </summary>
        /// <param name="mapDrive">y映射的本地盘符名Z</param>
        /// <param name="netWorkUser"></param>
        /// <param name="netWorkPwd"></param>
        /// <param name="netWorkPath"></param>
        /// <param name="strFromPath">要上传的文件路径</param>
        /// <param name="strToPath"></param>
        public static void CopyLogToServer(string mapDrive, string netWorkUser, string netWorkPwd, string netWorkPath,
            string strFromPath, string strToPath)
        {
            var comLine =
                $@"net use {mapDrive}: /del /y&net use {mapDrive}: {netWorkPath} {netWorkPwd} /USER:{netWorkUser}";
            try
            {
                //net use Z: /del&net use Z: \\10.177.4.201\NOKIA_LOG Luxshare /USER:Luxshare\nokia_test
                if (RunDosCmd(comLine).Contains("命令成功完成"))
                {
                    if (!Directory.Exists(strToPath)) Directory.CreateDirectory(strToPath);
                    File.Copy(strFromPath, strToPath, true);
                    loggerInfo("Upload test log to logServer success.");
                }
            }
            catch (Exception ex)
            {
                loggerFatal("Upload test log to logServer fail:" + ex);
            }
            finally
            {
                RunDosCmd($"net use {mapDrive}: /del /y"); // 删除盘符
            }
        }






















        public static bool PowerCycleOutlet(int index)
        {
            bool rReturn = false;

            loggerInfo("Start Power OFF");
            bool rReturnoff = PoeConfigSetting(index.ToString(), "disable");
            Thread.Sleep(3000);


            loggerInfo("Start Power ON");
            bool rReturnon = PoeConfigSetting(index.ToString(), "poeDot3af");


            if (!PingIP(Global.DUTIP, 2))
            {

                loggerInfo("Cannot Ping Successfully");
                loggerInfo(">>>>>>>>>> PowerOFF OK");
                //  Sleep(10000);     

                rReturn = rReturnoff && rReturnon;
                return rReturn;
            }
            else
            {
                loggerError("断电后再上电，不应该能马上ping通，所以失败!!!");
                return rReturn = false;
            }

        }

        public static void WriteToCsvForALKLeak(string filename, List<string> headers, List<string> data)
        {
            // 读取现有的文件
            var lines = File.Exists(filename) ? File.ReadAllLines(filename).ToList() : new List<string>();

            // 如果文件是空的或者表头需要更新，那么更新表头
            if (lines.Count == 0 || lines[0].Split(',').Length < headers.Count)
            {
                if (lines.Count > 0)
                {
                    lines.RemoveAt(0);  // 删除旧的表头
                }
                lines.Insert(0, string.Join(",", headers));  // 插入新的表头
            }

            // 添加数据
            lines.Add(string.Join(",", data));

            // 写入文件
            File.WriteAllLines(filename, lines);
        }

        public static string GetCombinedMd5(params string[] md5Strings)
        {
            // 将所有MD5字符串连接起来
            StringBuilder combinedBuilder = new StringBuilder();
            foreach (string md5 in md5Strings)
            {
                combinedBuilder.Append(md5);
            }
            string combined = combinedBuilder.ToString();

            // 计算连接后的字符串的MD5值
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(combined));

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }


        public static string GetMD5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static bool PoeConfigSetting(string peo_Port, string poeType)
        {

            bool re = false;
            try
            {
                var cookies = new CookieContainer();
                var client = GetClient(cookies, "admin", "admin123");
                string url = $@"http://169.254.100.101/api/v1/service";
                string data = "";


                loggerDebug(">>>>>>>>>>poeType:" + poeType);


                if (poeType == "poeDot3af") //poe
                {
                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"2.5G 1/{peo_Port}\",{{\"Mode\":\"{"poeDot3af"}\",\"Priority\":\"low\",\"Lldp\":\"enable\",\"MaxPower\":15,\"Structure\":\"2Pair\"}}],\"id\":164}}";

                }
                else if (poeType == "disable")
                {

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"2.5G 1/{peo_Port}\",{{\"Mode\":\"{"disable"}\",\"Priority\":\"low\",\"Lldp\":\"enable\",\"MaxPower\":30,\"Structure\":\"2Pair\"}}],\"id\":164}}";

                }
                else if (poeType == "poePlusDot3bt")
                { //poe+

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"2.5G 1/{peo_Port}\",{{\"Mode\":\"{"poePlusDot3bt"}\",\"Priority\":\"low\",\"Lldp\":\"enable\",\"MaxPower\":30,\"Structure\":\"2Pair\"}}],\"id\":942}}";
                }
                else if (poeType == "poePlusPlusDot3bt")//poe++
                {

                    if (peo_Port == "19")
                    {
                        peo_Port = "3";
                    }
                    else if (peo_Port == "20")
                    {
                        peo_Port = "4";
                    }

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"10G 1/{peo_Port}\",{{\"Mode\":\"{"poePlusDot3bt"}\",\"Priority\":\"critical\",\"Lldp\":\"enable\",\"MaxPower\":60,\"Structure\":\"4Pair\"}}],\"id\":942}}";
                }



                loggerDebug("请求参数:" + data);

                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;
                var response_content = result.Content.ReadAsByteArrayAsync().Result;
                var responseStr = System.Text.Encoding.UTF8.GetString(response_content);
                loggerDebug(result.StatusCode + ":" + responseStr);
                if (result.IsSuccessStatusCode && responseStr.Contains("\"error\":null"))
                    re = true;
            }
            catch (Exception ex)
            {
                loggerError(ex.Message);
                re = false;
            }
            return re;


        }



        public static bool PoeConfigSetting2(string peo_Port, string poeType)
        {

            bool re = false;
            try
            {
                if (peo_Port == "19")
                {
                    peo_Port = "3";
                }
                else if (peo_Port == "20")
                {
                    peo_Port = "4";
                }



                var cookies = new CookieContainer();
                var client = GetClient(cookies, "admin", "admin123");
                string url = $@"http://169.254.100.101/api/v1/service";
                string data = "";


                loggerDebug(">>>>>>>>>>poeType:" + poeType);

                if (poeType == "poeDot3af") //poe
                {

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"10G 1/{peo_Port}\",{{\"Mode\":\"{"poeDot3af"}\",\"Priority\":\"critical\",\"Lldp\":\"enable\",\"MaxPower\":15,\"Structure\":\"2Pair\"}}],\"id\":942}}";
                }
                else if (poeType == "poePlusDot3at") //poe+
                {

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"10G 1/{peo_Port}\",{{\"Mode\":\"{"poePlusDot3at"}\",\"Priority\":\"critical\",\"Lldp\":\"enable\",\"MaxPower\":30,\"Structure\":\"2Pair\"}}],\"id\":942}}";
                }

                else if (poeType == "disable")
                {

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"10G 1/{peo_Port}\",{{\"Mode\":\"{"disable"}\",\"Priority\":\"critical\",\"Lldp\":\"enable\",\"MaxPower\":15,\"Structure\":\"4Pair\"}}],\"id\":942}}";

                }

                else if (poeType == "poePlusDot3bt")//poe++
                { //poe+

                    data = $"{{\"method\":\"poe.config.interface.set\",\"params\":[\"10G 1/{peo_Port}\",{{\"Mode\":\"{"poePlusDot3bt"}\",\"Priority\":\"critical\",\"Lldp\":\"enable\",\"MaxPower\":90,\"Structure\":\"4Pair\"}}],\"id\":942}}";
                }




                loggerDebug("请求参数:" + data);

                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;
                var response_content = result.Content.ReadAsByteArrayAsync().Result;
                var responseStr = System.Text.Encoding.UTF8.GetString(response_content);
                loggerDebug(result.StatusCode + ":" + responseStr);
                if (result.IsSuccessStatusCode && responseStr.Contains("\"error\":null"))
                    re = true;
            }
            catch (Exception ex)
            {
                loggerError(ex.Message);
                re = false;
            }
            return re;


        }


        #region 测试功能函数

        /// <summary>
        ///     结束进程
        /// </summary>
        /// <param name="processName"></param>
        public static bool KillProcess(string processName, string fileName = "")
        {
            try
            {
                //Process[] localAll = Process.GetProcesses();
                var myProc = Process.GetProcessesByName(processName); //获取所有进程
                //if (myProc.Length == 0) return true;
                foreach (var thisProc in myProc)
                {
                    if (thisProc.CloseMainWindow())
                    {
                        continue;
                    }
                    //  loggerDebug($"准备杀死程序 :{processName}...");
                    thisProc.Kill();
                    loggerDebug($"Kill process: {processName}...");

                    if (fileName != "")
                    {
                        loggerDebug($"another way kill again");
                        RunDosCmd($"taskkill /IM {Path.GetFileName(fileName)} /F", 0, false);
                    }


                    if (processName == "userspace_speedtest")
                    {
                        loggerDebug($"dos kill agagin");
                        RunDosCmd("taskkill /IM userspace_speedtest.exe /F", 0, false);
                    }

                }

                Thread.Sleep(500);
                var myProc2 = Process.GetProcessesByName(processName); //获取所有进程
                                                                       // loggerInfo("进程:" + processName + " 没有了");
                return myProc2.Length == 0;
                //return true;
            }
            catch (Exception ex)
            {
                // loggerError("杀死程序出现异常");
                loggerFatal(ex.ToString());
                // return true;
                throw;
            }
        }


        public static void KillProcessNoResForce(string processName)
        {


            RunDosCmd("taskkill /IM " + processName + ".exe" + " /F", 0, false);
            RunDosCmd("taskkill /IM " + processName + ".exe" + " /F", 0, false);
            RunDosCmd("taskkill /IM " + processName + ".exe" + " /F", 0, false);




        }



        public static bool KillProcessNoRes(string processName)
        {
            loggerInfo("ready to kill:" + processName);
            try
            {

                //if (processName == "BTTestSuiteRD" || processName == "ATSuite" || processName == "QCATestSuite" || processName == "BTTestSuite") {
                //    MonitorClickZhuBo(processName);
                //    Thread.Sleep(1000);
                //}


                //Process[] localAll = Process.GetProcesses();
                var myProc = Process.GetProcessesByName(processName); //获取所有进程
                //if (myProc.Length == 0) return true;
                foreach (var thisProc in myProc)
                {
                    if (thisProc.CloseMainWindow())
                    {
                        continue;
                    }
                    //  loggerDebug($"准备杀死程序 :{processName}...");
                    thisProc.Kill();
                    loggerDebug($"Kill process: {processName}...");

                    if (processName == "userspace_speedtest")
                    {
                        loggerDebug($"dos kill again");
                        RunDosCmd("taskkill /IM userspace_speedtest.exe /F", 0, false);
                    }
                    if (processName == "QCATestSuite")
                    {
                        RunDosCmd("taskkill /IM QCATestSuite.exe /F", 0, false);
                    }



                    if (Global.STATIONNAME == "SRF" || Global.STATIONNAME == "MBFT")
                    {
                        loggerDebug($"dos kill again");
                        RunDosCmd("taskkill /IM " + processName + ".exe" + " /F", 0, false);
                    }


                }

                Thread.Sleep(500);
                var myProc2 = Process.GetProcessesByName(processName); //获取所有进程
                                                                       // loggerInfo("进程:" + processName + " 没有了");
                                                                       // return myProc2.Length == 0;
                return true;
            }
            catch (Exception ex)
            {
                // loggerError("杀死程序出现异常");
                loggerFatal(ex.ToString());
                return true;
                // throw;
            }
        }

        public static bool KillProcessHaveRes(string processName)
        {
            loggerInfo("ready to kill:" + processName);
            try
            {
                //Process[] localAll = Process.GetProcesses();
                var myProc = Process.GetProcessesByName(processName); //获取所有进程
                //if (myProc.Length == 0) return true;
                foreach (var thisProc in myProc)
                {
                    if (thisProc.CloseMainWindow())
                    {
                        continue;
                    }
                    //  loggerDebug($"准备杀死程序 :{processName}...");
                    thisProc.Kill();
                    loggerDebug($"Kill process: {processName}...");

                    if (processName == "userspace_speedtest")
                    {
                        loggerDebug($"dos kill again");
                        RunDosCmd("taskkill /IM userspace_speedtest.exe /F", 0, false);
                    }

                    if (Global.STATIONNAME == "SRF" || Global.STATIONNAME == "MBFT")
                    {
                        loggerDebug($"dos kill again");
                        RunDosCmd("taskkill /IM " + processName + ".exe" + " /F", 0, false);
                    }


                }

                Thread.Sleep(500);
                var myProc2 = Process.GetProcessesByName(processName); //获取所有进程
                return myProc2.Length == 0;


            }
            catch (Exception ex)
            {

                loggerFatal(ex.ToString());
                throw;
            }
        }




        /// <summary>
        ///     启动进程
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool StartProcess(string processName, string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var myProc = Process.GetProcessesByName(processName); //获取所有进程
                if (myProc.Length > 0) return true;

                if (fileInfo.Directory != null)
                {
                    var p = new Process { StartInfo = { WorkingDirectory = fileInfo.Directory.ToString(), FileName = fileInfo.Name } };
                    loggerDebug(p.StartInfo.WorkingDirectory + "  " + p.StartInfo.FileName);
                    p.Start();
                }

                Thread.Sleep(2000);
                var myProc2 = Process.GetProcessesByName(processName); //获取所有进程
                return myProc2.Length > 0;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                throw;
            }
        }









        /// <summary>
        ///     重启进程
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool RestartProcess(string processName, string fileName)
        {

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (KillProcess(processName, fileName))
                {
                    loggerDebug("...start ready to start " + processName);
                    if (fileInfo.Directory != null)
                    {
                        var p = new Process { StartInfo = { WorkingDirectory = fileInfo.Directory.ToString(), FileName = fileInfo.Name } };
                        p.Start();
                    }

                    Thread.Sleep(1000);
                    var myProc = Process.GetProcessesByName(processName); //获取所有进程
                    if (myProc.Length > 0)
                    {
                        loggerDebug("find " + processName);
                    }
                    else
                    {
                        loggerDebug("not find " + processName);
                    }

                    return myProc.Length > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                // throw;
                return false;
            }
        }


        public static bool RestartProcess(string processName, string fileName, out Process rProcess)
        {

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (KillProcess(processName))
                {
                    Process p = null;
                    if (fileInfo.Directory != null)
                    {
                        p = new Process { StartInfo = { WorkingDirectory = fileInfo.Directory.ToString(), FileName = fileInfo.Name } };

                        p.Start();
                    }

                    Thread.Sleep(1000);
                    var myProc = Process.GetProcessesByName(processName); //获取所有进程
                    rProcess = p;
                    return myProc.Length > 0;
                }
                rProcess = null;
                return false;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                rProcess = null;
                return false;
            }
        }







        public static void Sleep(int millisecondsTimeout)
        {
            loggerDebug($"Waiting {millisecondsTimeout}ms.....");
            Thread.Sleep(millisecondsTimeout);
        }

        public static void Sleep(string secondsTimeout)
        {
            loggerDebug($"Waiting {secondsTimeout}s.....");
            Thread.Sleep(TimeSpan.FromSeconds(int.Parse(secondsTimeout)));
        }

        public static bool Copyfile(string srcPath, string dstDir, bool overwrite = true)
        {
            bool rReturn;
            if (Directory.Exists(srcPath))
            {
                var dir = new DirectoryInfo(srcPath);
                var fileInfo = dir.GetFileSystemInfos(); //返回目录中所有文件和子目录
                foreach (var i in fileInfo)
                    if (i is DirectoryInfo) //判断是否文件夹
                    {
                        Directory.CreateDirectory($@"{dstDir}\{i.Name}");
                        Copyfile(i.FullName, $@"{dstDir}\{i.Name}", overwrite);
                    }
                    else
                    {
                        File.Copy(i.FullName, $@"{dstDir}\{i.Name}", overwrite);
                    }
                rReturn = true;
            }
            else if (File.Exists(srcPath))
            {
                //File.Delete(srcPath); //删除指定文件
                File.Copy(srcPath, $@"{dstDir}\{Path.GetFileName(srcPath)}", overwrite);
                rReturn = true;
            }
            else
            {
                loggerWarn("path not exist!");
                rReturn = false;
            }
            return rReturn;
        }


        /// <summary>
        ///     清空目录下所有文件
        /// </summary>
        public static bool ClearDirectoryOrDeleteFile(string filePath)
        {
            var rReturn = false;
            if (Directory.Exists(filePath))
            {
                var dir = new DirectoryInfo(filePath);
                var fileInfo = dir.GetFileSystemInfos(); //返回目录中所有文件和子目录
                foreach (var i in fileInfo)
                    if (i is DirectoryInfo) //判断是否文件夹
                    {
                        var subDir = new DirectoryInfo(i.FullName)
                        {
                            Attributes = FileAttributes.Normal & FileAttributes.Directory
                        };
                        //去除文件夹只读属性
                        subDir.Delete(true); //删除子目录和文件
                    }
                    else
                    {
                        File.SetAttributes(i.FullName, FileAttributes.Normal);
                        File.Delete(i.FullName); //删除指定文件
                    }
                rReturn = true;
            }
            else if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath); //删除指定文件
                rReturn = true;
            }
            else
            {
                loggerWarn("path not exist!");
                rReturn = true;
            }
            return rReturn;
        }

        /// <summary>
        ///     清空目录下指定类型的文件
        /// </summary>
        /// <param name="dirPath">目录路径</param>
        /// <param name="extension">扩展名，后缀.txt</param>
        /// <returns></returns>
        public static bool ClearDirectory(string dirPath, string extension)
        {
            var rReturn = false;
            if (Directory.Exists(dirPath))
            {
                var dir = new DirectoryInfo(dirPath);
                var fileInfo = dir.GetFileSystemInfos(); //返回目录中所有文件和子目录
                foreach (var i in fileInfo)
                {
                    if (i.Extension != extension) continue;
                    File.SetAttributes(i.FullName, FileAttributes.Normal); // 去除文件只读属性
                    File.Delete(i.FullName); //删除指定文件
                }

                rReturn = true;
            }
            else
            {
                rReturn = true;
            }

            return rReturn;
        }

        /// <summary>
        ///     用反射的方法设置对象变量的值
        /// </summary>
        /// <param name="_object">对象实例</param>
        /// <param name="varName">变量名</param>
        /// <param name="varNewValue">新的变量值</param>
        // ReSharper disable once InconsistentNaming
        public static void SetVerReflection(object _object, string varName, string varNewValue)
        {
            try
            {
                if (string.IsNullOrEmpty(varName) || varNewValue == null) return;
                var myFieldInfo = _object.GetType().GetField(varName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (myFieldInfo != null)
                {
                    myFieldInfo.SetValue(_object, varNewValue);
                    loggerInfo($"Set variable:{varName}={varNewValue}");
                }
                else
                {
                    logger.Equals($"Set variable Fail:{varName} doesn't exist!");
                }
            }
            catch (Exception ex)
            {
                loggerDebug(ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///     用反射的方法获取对象变量的值
        /// </summary>
        /// <param name="_object">对象,实例</param>
        /// <param name="varName">变量名</param>
        /// <returns></returns>
        public static string GetVerReflection(object _object, string varName)
        {
            string varValue = null;
            try
            {
                if (!string.IsNullOrEmpty(varName))
                {
                    var myFieldInfo = _object.GetType().GetField(varName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (myFieldInfo != null)
                    {
                        if (myFieldInfo.GetValue(_object) != null) varValue = myFieldInfo.GetValue(_object).ToString();
                        loggerDebug($"Get Reflect variable:{varName}={varValue}");
                    }
                    else
                    {
                        loggerDebug($"Get Reflect variable value Fail:{varName} does't exist!");
                    }
                }
            }
            catch (Exception ex)
            {
                loggerDebug(ex.ToString());
                //throw;
            }

            return varValue;
        }
        public static Dictionary<string, string> ConvertToDictionary(object myClass)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var property in myClass.GetType().GetFields())
            {
                var value = property.GetValue(myClass) as string;

                if (property.Name == "error_code" || property.Name == "error_details")
                {
                    dictionary.Add(property.Name, value);
                }


                else if (value != "null" && value != "" && value != null)
                {
                    dictionary.Add(property.Name, value);
                }

            }
            return dictionary;
        }
        /// <summary>
        /// C#反射遍历对象字段，去掉值为fieldValue的字段
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="model">对象</param>
        public static T ForeachClassFields<T>(T model, string fieldValue)
        {
            var instance = Activator.CreateInstance<T>();
            //PropertyInfo[] PropertyList = t.GetProperties();
            FieldInfo[] FieldList = model.GetType().GetFields();
            foreach (FieldInfo item in FieldList)
            {
                string name = item.Name;
                var myFieldInfo = instance.GetType().GetField(name);
                var Value = item.GetValue(model);
                if (Value != null)
                {
                    if (Value.ToString() == fieldValue)
                    {
                        myFieldInfo.SetValue(instance, null);
                    }
                    else
                    {
                        myFieldInfo.SetValue(instance, Value);
                    }
                }
            }
            return instance;
        }
        public static void StartSFTPSever(string ip = "", string port = "8090")
        {
            loggerInfo($">>>>>>>>>>start: iperf3 -B {ip} -s -p{port}");
            Process SFTPSeverProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Verb = "runas";

            if (ip == "")
            {
                startInfo.Arguments = $"/C .\\iperfserver\\iperf3 -B 192.168.1.20 -s -p{port}";
            }
            else
            {
                startInfo.Arguments = $"/C .\\iperfserver\\iperf3 -B {ip} -s -p{port}";
            }

            SFTPSeverProcess.StartInfo = startInfo;
            SFTPSeverProcess.Start();
        }

        public static string RunDosCmd(string command, int timeout = 0, bool needPrint = true)
        {
            loggerDebug($"DosSendComd-->{command}");
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
                if (needPrint == true)
                {
                    loggerDebug(output + error);
                }

                return output + error;
            }
        }
        public static string RunDosCmd2(string command, int timeout = 0)
        {
            loggerDebug($"DosSendComd-->{command}");
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
                return "";
            }
        }
        // 发送命令后不管
        public static void SendDosCmd(string cmd)
        {
            loggerDebug($"DOSCommand-->{cmd}");
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + cmd,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
        }

        /// <summary>
        ///     运行DOS命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="errors">命令错误信息</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static string RunDosCmd(string cmd, out string errors, int timeout = 3000) //, string directoryPath = @"C:\Windows\System32")
        {
            loggerDebug($"DOSCommand-->{cmd}");
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                //process.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                process.StartInfo.Arguments = "/c " + cmd;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true; //不显示程序窗口
                var output = new StringBuilder();
                var error = new StringBuilder();

                var outputWaitHandle = new AutoResetEvent(false);
                var errorWaitHandle = new AutoResetEvent(false);
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                            outputWaitHandle.Set();
                        else
                            output.AppendLine(e.Data);
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                            errorWaitHandle.Set();
                        else
                            error.AppendLine(e.Data);
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) &&
                        errorWaitHandle.WaitOne(timeout))
                        // Process completed. Check process.ExitCode here.
                        loggerDebug("Process completed");
                    else
                        // Timed out.
                        loggerDebug("RunDOSCmd output--> Timeout..." + error);
                    loggerDebug("RunDOSCmd output-->" + output);
                    errors = error.ToString();
                    return output.ToString();
                }
            }
        }


        /// <summary>
        ///     确认窗口
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="bts">默认0:只有OK button</param>
        /// <returns></returns>
        public static bool ConfirmMessageBox(string text, string title, MessageBoxButtons bts = 0)
        {
            if (string.IsNullOrEmpty(title)) title = "please confirm(Xác nhận)";
            var dr = MessageBox.Show(text, title, bts, MessageBoxIcon.Exclamation);
            return dr == DialogResult.Yes || dr == DialogResult.OK;
        }

        /// <summary>
        ///     统计子字符串的总数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strSub"></param>
        /// <returns></returns>
        private static int GetTotalSubstr(string str, string strSub)
        {
            if (str.Contains(strSub))
                return str.Split(new[] { strSub }, StringSplitOptions.None).Length - 1;
            return 0;
        }

        //[DllImport("kernel32.dll")]
        //public static extern uint WinExec(string path, uint uCmdShow);

        ///// <summary>
        /////     每秒Ping IP地址一次，ping通立即返回true，超过times后返回失败
        ///// </summary>
        //public static bool PingIP(string address, int times)
        //{
        //    var rResult = false;
        //    //WinExec("arp -d", 0);
        //    //RunDosCmd("arp -d & exit");
        //    for (var i = times; i > 0; i--)
        //    {
        //        //if (i % 4 == 0 && Global.STATIONNAME != "RTT")
        //        //    RunDosCmd("arp -d & exit");
        //        if (i % 4 == 0) {
        //            RunDosCmd("arp -d & exit");
        //        }

        //        var pingReply = Ping(address);
        //        if (pingReply.Status == 0)
        //        {
        //            loggerDebug(
        //                $"Reply from {pingReply.Address}：bytes={pingReply.Buffer.Length} time={pingReply.RoundtripTime} TTL={pingReply.Options.Ttl} {pingReply.Status}");
        //            rResult = true;
        //            break;
        //        }

        //        loggerDebug($"ping {address} ：{pingReply.Status}");
        //        if (i == 1)
        //        {
        //            loggerDebug($"ping {address} ：Fail！！！！！");
        //            //RunDosCmd("ping 192.168.1.101 -S 192.168.1.100");
        //            rResult = false;
        //        }

        //        Thread.Sleep(1000);
        //    }

        //    return rResult;
        //}
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
                loggerError($"ping {address} ：Fail！！！！！");
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

                if (Global.STATIONNAME == "BURNIN")
                {

                    if (i % 4 == 0)
                        RunDosCmd("arp -d & exit");
                }
                if (Global.STATIONNAME == "RTT")
                {

                    if (i % 4 == 0)
                        RunDosCmd("arp -d & exit");
                }

                var pingReply = Ping(address);
                if (pingReply != null && pingReply.Status == 0)
                {
                    loggerInfo($"Reply from {pingReply.Address}：bytes={pingReply.Buffer.Length} time={pingReply.RoundtripTime} TTL={pingReply.Options.Ttl} {pingReply.Status}");
                    rResult = true;
                    break;
                }
                else
                {

                    var rr = pingReply != null ? pingReply.Status : IPStatus.Unknown;

                    loggerInfo($"ping {address} ：{rr}");
                    if (i == 1)
                    {
                        loggerError($"ping {address} ：Fail！！！！！");
                        rResult = false;
                    }
                }
                Thread.Sleep(1000);
            }
            return rResult;
        }





        //public static PingReply Ping(string address)
        //{
        //    Ping ping = null;
        //    try
        //    {
        //        ping = new Ping();
        //        return ping.Send(address, 2000);
        //    }
        //    catch (PingException ex)
        //    {
        //        if (ex.InnerException != null)
        //        {
        //            loggerDebug(ex.InnerException.ToString());
        //        }
        //        return Ping(address);
        //    }
        //    finally
        //    {
        //        if (ping != null)
        //        {
        //            // 2.0 下ping 的一个bug，需要显示转型后释放
        //            IDisposable disposable = ping;
        //            disposable.Dispose();
        //            ping.Dispose();
        //        }
        //    }
        //}

        /// <summary>
        ///     截取两个子字符中间的字符
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startStr"></param>
        /// <param name="endStr"></param>
        /// <returns></returns>
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
                loggerFatal(ex.ToString());
                //throw ex;
            }
            return result;
        }

        /// <summary>
        ///     截取sub1和sub2中间字符串
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

        /// <summary>
        ///     截取sub1和sub2第n次出现的中间字符串
        /// </summary>
        /// <param name="souce"></param>
        /// <param name="sub1"></param>
        /// <param name="sub2"></param>
        /// <param name="no">第几次出现</param>
        /// <returns></returns>
        public static string GetSubStringOfMid(string souce, string sub1, string sub2, int no)
        {
            var p1 = GetPosition(souce, sub1, no);
            var p2 = GetPosition(souce, sub2, no);
            if (p1 == 0 || p2 == 0)
                return "";

            var star = p1 + sub1.Length;
            if (p2 - star < 0)
                return "";
            return souce.Substring(star, p2 - star).Trim();
        }

        /// <summary>
        ///     获取当前字符在字符串中第no次出现的位置
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="key">字符</param>
        /// <param name="no">第几次出现</param>
        /// <returns>返回位置</returns>
        public static int GetPosition(string souces, string key, int no)
        {
            var pos = 0; //!出现的次数,每出现一次+1
            if (no == 0) //!如果是查找第0次出现则直接返回
                return 0;

            for (var i = 0; i < souces.Length; i++) //!遍历
                if (souces.IndexOf(key, i) > -1) //!查找到关键字
                {
                    i = souces.IndexOf(key, i);
                    pos++; //!出现次数+1
                    if (pos >= no) //!如果是需要的出现次数,则返回当前位置
                        return i;
                }

            return 0;
        }

        /// <summary>
        ///     截取字符串，获取测试值
        /// </summary>
        /// <param name="revStr"></param>
        /// <param name="SubStr1"></param>
        /// <param name="SubStr2"></param>
        /// <returns></returns>
        public static string GetValue(string revStr, string SubStr1, string SubStr2)
        {
            var TestValue = "";
            if (!string.IsNullOrEmpty(SubStr1) || !string.IsNullOrEmpty(SubStr2)) //需要提取TestValue
            {
                TestValue = GetSubStringOfMid(revStr, SubStr1, SubStr2);
                if (string.IsNullOrEmpty(TestValue)) {
                    
                        loggerDebug("Error! Get TestValue IsNullOrEmpty.");
                    
                }
                 
                else {
                    
                        loggerDebug($"GetTestValue:{TestValue}");
                     
                 }
                   
            }

            return TestValue;
        }

        /// <summary>
        ///     截取字符串，获取测试值
        /// </summary>
        /// <param name="revStr"></param>
        /// <param name="SubStr1"></param>
        /// <param name="SubStr2"></param>
        /// <returns></returns>
        public static string GetValueSpecial(string revStr, string SubStr1, string SubStr2)
        {
            var TestValue = "";
            if (!string.IsNullOrEmpty(SubStr1) || !string.IsNullOrEmpty(SubStr2)) //需要提取TestValue
            {
                TestValue = GetSubStringOfMid(revStr, SubStr1, SubStr2);
                if (string.IsNullOrEmpty(TestValue))
                {

                   // loggerDebug("Error! Get TestValue IsNullOrEmpty.");

                }

                else
                {

                  //  loggerDebug($"GetTestValue:{TestValue}");

                }

            }

            return TestValue;
        }

        /// <summary>
        ///     截取字符串，获取测试值
        /// </summary>
        /// <param name="revStr"></param>
        /// <param name="SubStr1"></param>
        /// <param name="SubStr2"></param>
        /// <param name="Round">如果是数字字符串，是否取整</param>
        /// <returns></returns>
        public static string
            GetValue(string revStr, string SubStr1, string SubStr2, bool Round = false) //, out string TestValue)
        {
            string testValue = null;
            double? temp = null;
            if (!string.IsNullOrEmpty(SubStr1) || !string.IsNullOrEmpty(SubStr2)) //需要提取TestValue
            {
                testValue = GetSubStringOfMid(revStr, SubStr1, SubStr2);
                if (string.IsNullOrEmpty(testValue))
                {
                    loggerDebug("Error! Get TestValue IsNullOrEmpty.");
                }
                else
                {
                    loggerDebug($"GetTestValue: {testValue}{(Round ? ",need integer" : "")}");
                    temp = Round ? Math.Round(double.Parse(testValue)) : double.Parse(testValue);
                }
            }

            testValue = temp.ToString();
            return testValue;
        }

        /// <summary>
        ///     比较规格上下限
        /// </summary>
        /// <param name="limitMin">下限值</param>
        /// <param name="limitMax">上限值</param>
        /// <param name="value">测试值</param>
        /// <param name="info"></param>
        /// <param name="round">取整=true，默认false不取整</param>
        /// <returns></returns>
        public static bool CompareLimit(string limitMin, string limitMax, string value, out string info,
            bool round = false)
        {
            var infoTemp = "";
            var rReturn = false;


            if (value == "NA")
            {
                info = "";
                loggerError("value is NA,no need compare,false");
                return false;
            }

            if (string.IsNullOrEmpty(limitMin) && string.IsNullOrEmpty(limitMax))
            {
                // 不需比较最大值和最小值直接返回true
                info = "";
                return true;
            }

            if (string.IsNullOrEmpty(value))
            {
                info = "";
                return false;
            }

            var temp = round ? Math.Round(double.Parse(value)) : double.Parse(value);

            if (string.IsNullOrEmpty(limitMin) && !string.IsNullOrEmpty(limitMax)) //只需比较最大值
            {
                rReturn = temp <= double.Parse(limitMax);
                loggerDebug("Compare Limit_max...");
            }

            if (!string.IsNullOrEmpty(limitMin) && string.IsNullOrEmpty(limitMax)) //只需比较最小值
            {
                rReturn = temp >= double.Parse(limitMin);
                loggerDebug("Compare Limit_min...");
            }

            if (!string.IsNullOrEmpty(limitMin) && !string.IsNullOrEmpty(limitMax)) //需要比较最小值和最大值
            {
                loggerDebug("Compare Limit_min and Limit_max...");
                var rReturn1 = temp >= double.Parse(limitMin);
                // loggerInfo("return1:" + rReturn1);
                var rReturn2 = temp <= double.Parse(limitMax);
                //  loggerInfo("return2:" + rReturn2);
                rReturn = rReturn1 & rReturn2;
                // loggerInfo("总结过:" + rReturn);
                if (!rReturn) infoTemp = temp < double.Parse(limitMin) ? "TooLow" : "TooHigh";
            }

            info = infoTemp;
            // loggerInfo("总结过2:" + info);
            return rReturn;
        }

        /// <summary>
        ///     检查Spce值
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="testValue"></param>
        /// <returns></returns>
        public static bool CheckSpec(string spec, string testValue)
        {
            if (string.IsNullOrEmpty(testValue)) return false;

            var rReturn = false;
            if (!string.IsNullOrEmpty(spec))
            {
                // Spec值有多种情况，属于包含关系
                if (spec.Contains("|") && spec.Contains(testValue))
                {
                    loggerDebug("check Spec contain pass");
                    rReturn = true;
                }
                else if (testValue == spec)
                {
                    // Spec值只有一种，检查==
                    loggerDebug("check Spec == pass");
                    rReturn = true;
                }
            }
            else
            {
                rReturn = true;
            }
            return rReturn;
        }

        #endregion 测试功能函数

        #region CSV文件操作

        public static void CreatCSVFile(string csvFilePath, string[] colHeader)
        {
            if (!File.Exists(csvFilePath))
            {
                File.Create(csvFilePath).Close();
                File.SetAttributes(csvFilePath, FileAttributes.Normal);
                Thread.Sleep(500);
                if (File.Exists(csvFilePath))
                {
                    var rowList = new List<string[]>();
                    rowList.Add(colHeader);
                    WriteCSV(csvFilePath, false, rowList);
                }
            }
        }

        public static void CreatCSVFile(string csvFilePath, string columnFilePath, bool updateColumn = false)
        {
            string[] colHeader;
            if (!File.Exists(csvFilePath))
            {
                File.Create(csvFilePath).Close();
                File.SetAttributes(csvFilePath, FileAttributes.Normal);
                Thread.Sleep(500);
                if (File.Exists(csvFilePath))
                {
                    var rowList = new List<string[]>();
                    using (var sr = new StreamReader(columnFilePath))
                    {
                        colHeader = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    rowList.Add(colHeader);
                    WriteCSV(csvFilePath, false, rowList);
                }
            }
            else
            {
                if (updateColumn)
                {
                    using (var sr = new StreamReader(columnFilePath))
                    {
                        colHeader = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    var ls = ReadCSV(csvFilePath);
                    if (!Enumerable.SequenceEqual(ls[0], colHeader))
                    {
                        ls[0] = colHeader;
                        WriteCSV(csvFilePath, false, ls);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }


        public static void CreatCSVFile(string csvFilePath, string columnFilePath)
        {
            string[] colHeader;
            if (!File.Exists(csvFilePath))
            {
                File.Create(csvFilePath).Close();
                File.SetAttributes(csvFilePath, FileAttributes.Normal);
                Thread.Sleep(500);
                if (File.Exists(csvFilePath))
                {
                    var rowList = new List<string[]>();
                    using (var sr = new StreamReader(columnFilePath))
                    {
                        colHeader = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    rowList.Add(colHeader);
                    WriteCSV(csvFilePath, false, rowList);
                }
            }
            else
            {
                //loggerDebug($"csv file have been created:{csvFilePath}");
            }
        }

        public static void WriteCSV(string filePathName, bool append, List<string[]> ls)
        {
#if DEBUG
#else
            KillProcess("Excel");
#endif



            if (File.Exists(filePathName))
            {

                try
                {
                    var fileWriter = new StreamWriter(filePathName, append, Encoding.GetEncoding(-0));
                    foreach (var strArr in ls) fileWriter.WriteLine(string.Join(",", strArr));
                    fileWriter.Flush();
                    fileWriter.Close();
                }
                catch (Exception ex)
                {
                    ReadyToTest usbDialog = new ReadyToTest();

                    usbDialog.StartPosition = FormStartPosition.CenterScreen;
                    usbDialog.ShowTip("请检查CSV是否打开，如果打开请关闭");



                    usbDialog.TopMost = true;
                    //// 设置窗体为无边框样式
                    usbDialog.FormBorderStyle = FormBorderStyle.None;
                    // 最大化窗体
                    usbDialog.WindowState = FormWindowState.Maximized;


                    usbDialog.ShowDialog();

                    return;

                }



            }
            else
            {
                loggerDebug($"the File:{filePathName} does not exist！");
            }
        }

        public static List<string[]> ReadCSV(string filePathName)
        {
            var ls = new List<string[]>();
            var fileReader = new StreamReader(filePathName);
            var strLine = "";
            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                    ls.Add(strLine.Split(','));
                else
                    Thread.Sleep(1);
            }

            fileReader.Close();
            return ls;
        }

        public static void DataGridViewToCSV(DataGridView dataGridView, bool append, string csvPath)
        {
            var sw = new StreamWriter(csvPath, append, Encoding.GetEncoding(-0));
            var strLine = "";
            try
            {
                //表头
                //for (int i = 0; i < dataGridView.ColumnCount; i++)
                //{
                //    if (i > 0)
                //        strLine += ",";
                //    strLine += dataGridView.Columns[i].HeaderText;
                //}
                //strLine.Remove(strLine.Length - 1);
                //sw.WriteLine(strLine);
                //strLine = "";
                //表的内容
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                {
                    strLine = "";
                    var colCount = dataGridView.Columns.Count;
                    for (var k = 0; k < colCount; k++)
                    {
                        if (k > 0 && k < colCount)
                            strLine += ",";
                        if (dataGridView.Rows[j].Cells[k].Value == null)
                        {
                            strLine += "";
                        }
                        else
                        {
                            var cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                            //防止里面含有特殊符号
                            cell = cell.Replace("\"", "\"\"");
                            cell = "\"" + cell + "\"";
                            strLine += cell;
                        }
                    }

                    sw.Flush();
                    sw.WriteLine(strLine);
                }

                sw.Close();
                loggerDebug($"Export test result to {csvPath} succeed");
            }
            catch (Exception ex)
            {
                loggerDebug($"Export test result to{csvPath} error!:{ex.Message} ");
                if (ReadCSV(csvPath).Count >= 65535)
                {
                    var renamePath = csvPath.Insert(csvPath.LastIndexOf("."), DateTime.Now.ToString("yyyy-MM-dd-HHmm"));
                    // 重命名
                    var fi = new FileInfo(csvPath); //result.csv
                    fi.MoveTo(renamePath); //result2020121115.csv
                }
            }
        }

        #endregion CSV文件操作

        /// <summary>
        /// 取取本机指定网段的IPV4地址
        /// </summary>
        /// <param name="networkSegment">如10.90.</param>
        /// <returns></returns>
        public static string GetAllIpv4Address(string networkSegment)
        {
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name); // 获取本机所有IPV4地址
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork && ipa.ToString().StartsWith(networkSegment))
                    return ipa.ToString();
            }
            return "";
        }

        /// <summary>
        /// 关闭指定的excel文件,需要在非管理员模式下运行才可以
        /// </summary>
        /// <param name="workbookName">excel文件名</param>
        /// <param name="isSave">关闭时是否保存</param>
        public static void CloseExcelWorkbook(string workbookName, bool isSave = false)
        {
            try
            {
                Process[] plist = Process.GetProcessesByName("Excel", ".");
                if (plist.Length > 1)
                    throw new Exception("More than one Excel process running.");
                else if (plist.Length == 0)
                    return;

                Object obj = Marshal.GetActiveObject("Excel.Application");
                Microsoft.Office.Interop.Excel.Application excelAppl = (Excel.Application)obj;
                Excel.Workbooks workbooks = excelAppl.Workbooks;
                foreach (Excel.Workbook wkbk in workbooks)
                {
                    if (wkbk.Name == workbookName)
                        wkbk.Close(isSave);
                }
                //dispose
                //workbooks.Close(); //this would close all workbooks
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (workbooks != null)
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbooks);
                //excelAppl.Quit(); //would close the excel application
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelAppl);
                GC.Collect();
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        public static string GetSHA256(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"{filePath} isn't exist");
            }
            byte[] by;
            SHA256Managed Sha256 = new SHA256Managed();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                by = Sha256.ComputeHash(stream);
            }
            return BitConverter.ToString(by).Replace("-", "").ToUpper();
            //return Convert.ToBase64String(by);      
        }

        public static byte[] GetSHA256Byte(string filePath)
        {
            byte[] by;
            SHA256Managed Sha256 = new SHA256Managed();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                by = Sha256.ComputeHash(stream);
            }
            return by;
        }

        public static void BinaryWrite(byte[] bytes, string writePath)
        {
            if (File.Exists(writePath))
            {
                // 去除文件只读属性
                File.SetAttributes(writePath, FileAttributes.Normal);
                File.Delete(writePath);
            }
            using (var stream = new FileStream(writePath, FileMode.CreateNew))
            {
                using (var bw = new BinaryWriter(stream))
                {
                    bw.Write(bytes);
                }
            }
        }

        public static string BinaryRead(string readPath)
        {
            if (!File.Exists(readPath))
            {
                MessageBox.Show($"{readPath} isn't exist");
            }
            byte[] reads;
            using (var stream = new FileStream(readPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var br = new BinaryReader(stream))
                {
                    reads = br.ReadBytes(100);
                }
            }
            return BitConverter.ToString(reads).Replace("-", "").ToUpper();
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

        public static HttpClient GetClient2(CookieContainer cookies, string username, string password)
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
            // client.DefaultRequestHeaders.Add("Authorization", $"Basic {strLoginPasswordEncoded}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJQcml2aWxlZ2UiOjE1LCJTY2hlbWUiOiJIVFRQIiwiVXNlck5hbWUiOiJhZG1pbiIsImlhdCI6MTYwNzkxMTY2NX0.GLqBxa2Y0MpiZoMjqe-mrBkktQWyA0mjbrUijkUMi_6kuZXp6vTixVuAi69wufs_8adwb9hr0cv3PKeeBqtHW_DBK0waSzPTldB4v3KgBgc-bnZjO1Em8A0EW1AqqrX5gpFyGMkEYuC0o4Agya_wGBeGjwjqqNTXTMKc5_yJWZsKxf4kBGryxb3fh0GNUhGgHirz9oKukkPV4xTqwK3xtNljfAQhhz5MXfuHZI1lbfSRUuQkbh0R_A3IPU-23xfFUixxrJiWsU8ZB363D09oNJ9ndF9anUUBVtQxVskCgPTdtEjy-zjIkFVqPoHR4X0omhBvflIJEbtz0F5mBz08TA");


            return client;
        }

        public static string HttpGet(string url)
        {
            var client = new HttpClient();
            var bytes = client.GetByteArrayAsync(url).GetAwaiter().GetResult();
            var result = System.Text.Encoding.UTF8.GetString(bytes);
            return result;
        }

        public static Dictionary<string, object> HttpGets(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Perform the GET request synchronously
                    var response = client.GetAsync(url).GetAwaiter().GetResult();

                    // Create a dictionary to store the response
                    var result = new Dictionary<string, object>
            {
                { "StatusCode", (int)response.StatusCode },
                { "Content", response.Content.ReadAsStringAsync().GetAwaiter().GetResult() }
            };

                    return result;
                }
                catch (HttpRequestException ex)
                {
                    // Log error or handle appropriately
                    Console.WriteLine($"HttpRequestException occurred: {ex.Message}");
                    return new Dictionary<string, object>
            {
                { "StatusCode", 500 },
                { "Content", "HttpRequestException occurred" }
            };
                }
                catch (Exception ex)
                {
                    // Log any other exceptions
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    return new Dictionary<string, object>
            {
                { "StatusCode", 500 },
                { "Content", "An unexpected error occurred" }
            };
                }
            }
        }


        public static string HttpPost(string url, HttpContent content, out string statusCode)
        {
            var client = new HttpClient();
            var result = client.PostAsync(url, content).GetAwaiter().GetResult();
            var bytes = result.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            var responseBody = System.Text.Encoding.UTF8.GetString(bytes);
            //if (!result.IsSuccessStatusCode)
            //{
            //    throw new Exception($"Failed to post requests.Response code: {result.StatusCode}");
            //}
            loggerDebug(result.StatusCode + ":" + responseBody);
            statusCode = result.StatusCode.ToString();
            return responseBody;
        }

    }
}