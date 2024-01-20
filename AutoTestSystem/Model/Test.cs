using AutoTestSystem.BLL;
using AutoTestSystem.DAL;
using Newtonsoft.Json;
using PDUSPAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutoTestSystem.BLL.Bd;
using static AutoTestSystem.MainForm;
using static System.String;

namespace AutoTestSystem.Model
{
    public class Test
    {
        /// 扫描SN
        public string SN; //{ get; private set; }
        public static Thread thread = null;
        /// 机种
        public string DutMode;// { get; private set; }

        /// DUT IP address
        public string DUTIP;// { get; private set; }

        public Test()
        {
        }

        public Test(string _SN, string _DutMode)
        {
            SN = _SN;
            DutMode = _DutMode;
        }

        public Test(string _SN, string _DutMode, string _DUTIP)
        {
            SN = _SN;
            DutMode = _DutMode;
            DUTIP = _DUTIP;
        }


        





        public bool StepTest(test_phases testPhase, Items item, int retryTimes, phase_items phaseItem, ref int retry)
        {

            if (DateTime.Now.ToLongDateString().Contains("2021"))
            { //时间不对
                loggerInfo("time error ,update time>>");
                RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC
                var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                loggerInfo("get updated time>>>:" + dateTime.ToLongTimeString());
                SyDateTimeHelper.SetLocalDateTime2(dateTime);

            }
            if (item.ItemName == "Checkroute")
            {
                var curTime = DateTime.Now;
                
                var dateTimeNow = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                bool same = false;
                same = (curTime.ToShortDateString() == dateTimeNow.ToShortDateString());

                
                if (same == false)
                { //时间不对
                    loggerInfo("!!!startTime error,update time");

                    RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC

                    var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                    startTime = dateTime;
                    loggerInfo("!!!get updated time:" + dateTime.ToLongTimeString());
                    SyDateTimeHelper.SetLocalDateTime(dateTime);

                }

            }
            
            bool specFlag = false; //spec中是否有变量
            string info = "";
            error_code = "";
            error_details = "";
            bool rReturn = false;
            // 如果有多行errorcode
            string[] ErrorList = item.ErrorCode.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            // debug模式下要skip的测试步骤
            if ((Global.TESTMODE == "debug" || Global.TESTMODE == "fa" || IsDebug) &&
                (item.TestKeyword.Contains("GetIpaddrEnv")
                || item.TestKeyword.Contains("CheckEeroTest") || item.TestKeyword.Contains("Checkroute")
                || item.TestKeyword.Contains("CheckEeroABA")
                || item.TestKeyword.Contains("GetWorkOrder")
                 || item.TestKeyword.Contains("SetDHCP")
                 || item.TestKeyword.Contains("CKCustom")
                
                ))
            {
                loggerWarn("This is debug mode.Skip this step.");
                testPhase.phase_details = "This is debug mode.";
                return rReturn = true;
            }


            

            // 发送的命令中有变量
            while (!String.IsNullOrEmpty(item.ComdOrParam) && item.ComdOrParam.Contains("<") && item.ComdOrParam.Contains(">"))
            {
                string verName = GetMidStr(item.ComdOrParam, "<", ">");
                item.ComdOrParam = GetMidStr(item.ComdOrParam, null, "<") + GetVerReflection(f1, verName) + GetMidStr(item.ComdOrParam, ">", null);
                //retry = 0;  //！有变量不允许retry
                //retryTimes = 0;
            }
            // Spec值中含有变量
            while (!string.IsNullOrEmpty(item.Spec) && item.Spec.Contains("<") && item.Spec.Contains(">"))
            {
                if (item.EeroName == "TEST_IMAGE_VERSION" &&  (Global.STATIONNAME.ToUpper() == "MBLT" || Global.STATIONNAME.ToUpper() == "SFT"))
                    specFlag = false;
                else
                    specFlag = true;
#if DEBUG
                //MesMac = "9c:a5:70:00:39:60";
                MesMac = "11:22:33:44:55:66";
#endif

#if DEBUG
                mes_qsdk_version = Global.QSDKVER;
#else
                if (item.TestKeyword == "Veritfy_QSDK_Version")
                {
                    mes_qsdk_version = mescheckroute.getFirmwareFW(SN);
                    loggerDebug($"mescheckroute.getFirmwareFW:{mes_qsdk_version}");
                }
#endif
               
                
                
                
                string verName = GetMidStr(item.Spec, "<", ">");
                item.Spec = GetMidStr(item.Spec, null, "<") + GetVerReflection(f1, verName) + GetMidStr(item.Spec, ">", null);
                if (string.IsNullOrEmpty(item.Spec))
                {
                    loggerError($"Parsing item.Spec failed, IsNullOrEmpty!!! test FAIL.");
                    retry = 0;  //！有变量不允许retry
                    retryTimes = 0;
                    error_code = ErrorList[0].Split(':')[0].Trim();
                    error_details = ErrorList[0].Split(':')[1].Trim();
                    testPhase.phase_details = error_details;
                    testPhase.error_code = error_code;
                    return rReturn = false;
                }
                loggerDebug($"item.Spec:{item.Spec}");
            }

            //added 20230516  SpecStatic,为了解决specFlag默认等于false的bug，导致jsonupload fail
            if (!IsNullOrEmpty(item.SpecStatic) && item.SpecStatic.Contains("<") && item.SpecStatic.Contains(">"))
            {
                if (item.EeroName == "TEST_IMAGE_VERSION" && (Global.STATIONNAME.ToUpper() == "MBLT" || Global.STATIONNAME.ToUpper() == "SFT"))  
                {
                    loggerInfo($"MBLT Test_Image_Version特殊处理,设置specFlag=false");
                    specFlag = false;
                }

                else
                {
                    loggerInfo($"item.SpecStatic:{item.SpecStatic},设置specFlag=true");
                    specFlag = true; //这样spec的值就不会作为minlimit传给服务器

                }




            }

            //get limit
            {
               
                if (!IsNullOrEmpty(item.Json) && Global.OnlineLimit=="1" && Global.STATIONNAME!= "BURNIN" && Global.STATIONNAME != "SetDHCP" && Global.STATIONNAME != "Revert")
                {
                    if (Online_Limit == null)
                    {
                        item.Limit_min = null;
                        item.Limit_max = null;
                    }
                    else
                    {
                        bool findFlag = false;
                        foreach (var lim in Online_Limit.limits)
                        {
                            if (lim.test_name == item.EeroName)
                            {
                                if (lim.model.ToLower() == DutMode.ToLower() && lim.station_type.ToLower() == Global.STATIONNAME.ToLower())
                                {
                                    if (lim.limit_type.ToLower() == "limit")
                                    {
                                        item.Limit_min = lim.lower_limit;
                                        item.Limit_max = lim.upper_limit;
                                    }
                                    else if (lim.limit_type.ToLower() == "match")
                                    {
                                        item.Limit_min = null;
                                        item.Limit_max = null;
                                        item.Spec = lim.lower_limit;
                                    }
                                    else if (lim.limit_type.ToLower() == "none" || lim.limit_type.ToLower() == "bool")
                                    {
                                        item.Limit_min = null;
                                        item.Limit_max = null;
                                    }
                                    else
                                    {
                                        loggerFatal($"limit_type {lim.limit_type.ToLower()} is unknow");
                                        throw new Exception($"limit_type {lim.limit_type.ToLower()} is unknow");
                                    }
                                    loggerInfo($"get online limit:{item.EeroName} Spec:{item.Spec},Min:{item.Limit_min},Max:{item.Limit_max}");
                                    findFlag = true;
                                    break;
                                }
                                else
                                    loggerInfo($"{item.EeroName} found, but {lim.model},{lim.station_type} is match.");
                            }
                        }
                        if (!findFlag)
                            loggerInfo($"{item.EeroName} not found in online limit");
                    }
                }
            }







            if (item.ComdOrParam == "quit" || item.ComdOrParam == "0x03")
            {
                byte[] quit = { 0x03 };
                item.ComdOrParam = Encoding.ASCII.GetString(quit).ToUpper();
            }

            try
            {
                switch (item.TestKeyword)
                {


                    case "skip":
                        {
                            rReturn = true;
                            loggerDebug("当前测试项跳过");
                        }
                        break;
                    case "KillProcess":
                        rReturn = KillProcess(item.ComdOrParam);
                        break;
                    case "KillProcessNoRes":
                        rReturn = KillProcessNoRes(item.ComdOrParam);
                        break;

                    case "StartProcess":
                        {
                            if (Global.STATIONNAME != "SRF") {
                                RunDosCmd("taskkill /IM " + item.ExpectStr + ".exe" + " /F");
                                RunDosCmd("taskkill /IM " + item.ExpectStr + ".exe" + " /F");
                            }
                          

                            rReturn = StartProcess(item.ExpectStr, item.ComdOrParam);
                            Sleep("1");
                            Program.SwitchToThisWindow(Global.processInfo.MainWindowHandle, true);
                            break;
                        }
                       

                    case "CopyFileToDir":
                        rReturn = Copyfile(item.ComdOrParam, item.ExpectStr);
                        break;

                    case "CopyCalBin":
                        {
                            var calbinpath = $@"C:\tftp-3\{SN}\{item.ComdOrParam}";
                            rReturn = Copyfile(calbinpath, item.ExpectStr);
                            break;
                        }


                    case "StartRecordTime": {

                            PowerToTelnetStartTime = DateTime.Now;
                            rReturn = true;

                            break;
                        }

                    case "EndRecordTime":
                        {

                            if (PowerToTelnetStartTime != null) {

                                PowerToTelnetEndTime = DateTime.Now;

                               var  seconds = (PowerToTelnetEndTime - PowerToTelnetStartTime).TotalSeconds;
                               item.testValue = seconds.ToString();

                            }
                            rReturn = true;
                            break;
                        }




                    case "CopyMBFTConfig":

                        return true;

                        File.Delete(item.ExpectStr + $@"\dutSetup_2G.txt");
                        File.Delete(item.ExpectStr + $@"\dutSetup_5G.txt");
                        File.Delete(item.ExpectStr + $@"\dutSetup_6G.txt");
                        if (!IsNullOrEmpty(SN) && (item.CheckStr1.Contains(SN) || Global.GoldenSN.Contains(SN)))
                        {
                            rReturn = Copyfile(item.ComdOrParam + $@"\Golden", item.ExpectStr);
                            loggerWarn("run Gloden ATE config.");
                        }
                        else
                        {
                            rReturn = Copyfile(item.ComdOrParam + $@"\NoGolden", item.ExpectStr);
                            loggerWarn("run no Gloden ATE config.");
                        }
                        break;

                    case "RestartProcess":
                        rReturn = RestartProcess(item.ExpectStr, item.ComdOrParam);
                        Sleep("2");
                        Program.SwitchToThisWindow(Global.processInfo.MainWindowHandle, true);
                        break;

                    case "Wait":
                    case "ThreadSleep":
                        if (string.IsNullOrEmpty(item.ComdOrParam))
                            Sleep(item.TimeOut);
                        else
                            Sleep(item.ComdOrParam);
                        rReturn = true;
                        break;

                    case "ThreadSleep2":
                        Sleep( int.Parse(item.ComdOrParam));
                        rReturn = true;
                        break;

                    case "MessageBoxShow":
                        rReturn = ConfirmMessageBox(item.ComdOrParam, item.ExpectStr, item.TimeOut == "0" ? MessageBoxButtons.OK : MessageBoxButtons.YesNo);
                        break;
                    case "CKCustom":
                        {

                            var url = $"http://172.23.241.211:9000/stationprocess/passstationcheck/{SN}";
                            loggerDebug(url);

                            var ret = "";
                            try
                            {
                                ret = HttpGet(url);

                                var dict = JsonToDictionary(ret);
                                loggerInfo(">>>>:" + ret);
                                if (dict != null && dict["isexist"].ToString() == "1")
                                {
                                    rReturn = false;
                                    loggerInfo(">>>>:" + $"SN:{SN} 在Burnin站已存在pass记录");
                                }
                                else
                                {
                                    rReturn = true;
                                }

                            }
                            catch
                            {

                                loggerInfo("异常:>>>>:" + ret);
                                rReturn = false;
                            }




                        }
                        break;

                    case "GetCsnErroMessage_CCT":
                        {

                            if (Global.TESTMODE == "debug") {
                                loggerDebug("debug,skip");
                                return rReturn = true;
                            }


                           var url = $"http://10.90.116.132:8086/api/CHKRoute/serial/{SN}/station/CCT-1630";
                            loggerDebug(url);

                            var ret = "";
                            try
                            {
                                // ret = HttpGet(url);
                                ret = HttpPost(url, null, out _);


                                loggerInfo(">>>>:" + ret);
                                if (ret.Contains("OK"))
                                {
                                    rReturn = true;
                                }
                                else
                                {

                                    MainForm.f1.ShowLbl_FAIL_TEXT(ret);
                                    rReturn = false;
                                }
                            }
                            catch(Exception ex)
                            {

                                loggerInfo(">>>>:" + ex.Message);
                                rReturn = false;
                            }




                        }
                        break;

               
                    case "PowerPingDUT":
 
                    case "PingDUT":
                        try
                        {
                           


                            if (Global.STATIONNAME == "BURNIN") {
                                if (item.TestKeyword == "PingDUT") {
                                    Regex regex = new Regex(@"_(\d+)$");
                                    Match match = regex.Match(item.ItemName);
                                    if (match.Success)
                                    {
                                        var loopNum = match.Groups[1].Value;
                                        loggerInfo($"--------------------------------------LoopIndex:{loopNum}-------------------------------------");
                                    }
                                }
                               
                            }

                         


                            rReturn = PingIP(!IsNullOrEmpty(item.ComdOrParam) ? item.ComdOrParam : DUTIP, int.Parse(item.TimeOut));
                            if (retryTimes > 0)
                            {
                                HandleSpecialMethed(item, rReturn, "");
                            }
                        }
                        catch (Exception ex) {
                            rReturn = false;
                            loggerError(ex.ToString());
                        }
                       
                        break;

                    case "PingDUT2":
                        try
                        {
                            rReturn = PingIP(!IsNullOrEmpty(item.ComdOrParam) ? item.ComdOrParam : DUTIP, int.Parse(item.TimeOut));
                            
                        }
                        catch (Exception ex)
                        {
                            rReturn = false;
                            loggerError(ex.ToString());
                        }

                        break;

                    case "PingDUT_CHECK":
                        {
                            rReturn = PingIP(!IsNullOrEmpty(item.ComdOrParam) ? item.ComdOrParam : DUTIP, int.Parse(item.TimeOut));
                            if (item.SubStr1 == "!")
                                rReturn = !rReturn;
                        }
                        break;

                    case "SampleTelnetLogin":
                        {
                            SampleComm = new Telnet(new TelnetInfo { _Address = item.ComdOrParam });
                            rReturn = SampleComm.Open(item.ExpectStr);
                        }
                        break;

                    case "WaitForTelnet":
                        {
                            if (DUTCOMM == null || DUTCOMM.GetType() != typeof(Telnet))
                            {

                                if (DUTCOMM != null) { 
                                    DUTCOMM.Close();
                                }

                                if (!IsNullOrEmpty(item.ComdOrParam))
                                    telnetInfo = new TelnetInfo { _Address = item.ComdOrParam };
                                
                                
                                DUTCOMM = new Telnet(telnetInfo);
                            }
                            rReturn = DUTCOMM.Open(Global.PROMPT);
                            //if (DUTCOMM == null || DUTCOMM.GetType() != typeof(Telnet))
                            //{
                            //    DUTCOMM = new Telnet(telnetInfo);
                            //}
                            //if (!IsNullOrEmpty(item.ComdOrParam))
                            //{
                            //    DUTCOMM = new Telnet(new TelnetInfo { _Address = item.ComdOrParam });
                            //}
                            //rReturn = DUTCOMM.Open(Global.PROMPT);
                        }
                        break;
                    case "WaitForTelnet2":
                        {
                            if (DUTCOMM2 == null)
                            {
                                DUTCOMM2 = new Telnet(telnetInfo);
                            }
                            else
                            {
                                DUTCOMM2.Close();
                            }
                            rReturn = DUTCOMM2.Open(Global.PROMPT);
                        }
                        break;

                    case "TestLoadWifi2":
                        {

                            var revStr = "";
                            inPutValue = "";

                            if (DUTCOMM2.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                rReturn = true;
                                // 需要提取测试值
                                if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                                    item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);
                                else
                                    return rReturn = true;

                                if (item.TestKeyword.Contains("TempSensorTest_After"))
                                {
                                    if (string.IsNullOrEmpty(item.testValue))
                                    {
                                        item.testValue = "0";
                                        return rReturn = false;
                                    }
                                    else
                                        item.testValue = Math.Round(double.Parse(item.testValue)).ToString(); //取整
                                }
                                // 需要比较Spec
                                if (!string.IsNullOrEmpty(item.Spec) && string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                    return rReturn = CheckSpec(item.Spec, item.testValue);

                                // 需要比较Limit
                                if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max))
                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);

                              
                            }
                            else
                            {
                                HandleSpecialMethed(item, rReturn, revStr);
                            }

                           
                        }
                        break;


                    case "TelnetAndSendCmd":
                        {
                            STAComm = new Telnet(new TelnetInfo { _Address = item.ComdOrParam });
                            string revStr = "";
                            if (STAComm.Open(item.ExpectStr) && STAComm.SendCommand(item.CheckStr2, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1))
                            {
                                rReturn = true;
                            }
                            Sleep("2");
                        }
                        break;
                    case "COMPortOpen":
                    case "SerialPortOpen":
                        {
                            if (DUTCOMM == null || DUTCOMM.GetType() == typeof(Telnet))
                            {
                                if (DUTCOMM != null) { 
                                    DUTCOMM.Close();
                                }

                                if (!string.IsNullOrEmpty(item.ComdOrParam))
                                {
                                    DUTCOMinfo = new SerialConnetInfo { PortName = item.ComdOrParam, BaudRate = int.Parse(item.ExpectStr) };
                                }
                                DUTCOMM = new Comport(DUTCOMinfo);
                            }

                            DUTCOMM.Close();
                            
                            rReturn = DUTCOMM.Open();
                        }
                        break;

                    case "CloseDUTCOMM":
                        if (DUTCOMM != null)
                        {
                            DUTCOMM.Close();
                            rReturn = true;
                        }
                        break;

                    case "ClearDirectory":
                        rReturn = ClearDirectoryOrDeleteFile(item.ComdOrParam);
                        break;

                    case "PassSN":
                        rReturn = WriteSNandMoveFile(SN, $@"{System.Environment.CurrentDirectory}\{item.ComdOrParam}", item.ExpectStr);
                        break;

                    case "Waitingcsvlog":
                        csvLines = null;
                        rReturn = WaitingCSVlog(item.TimeOut, item.ComdOrParam, SN, out csvLines);
                        break;
                    case "WaitingcsvlogBT":
                        {
                            csvLines = null;
                            var lngStart = DateTime.Now.AddSeconds(int.Parse(item.TimeOut)).Ticks;
                            while (DateTime.Now.Ticks <= lngStart)
                            {
                                var files = Directory.GetFileSystemEntries(item.ComdOrParam);
                                if (files.Length != 0)
                                {
                                    foreach (var file in files)
                                    {
                                        if (file.Contains(SN) && Directory.Exists(file))
                                        {
                                            return rReturn = WaitingCSVlog2("2", file, SN, out csvLines);
                                        }
                                    }
                                }
                                else
                                    Thread.Sleep(1000);
                            }
                        }
                        break;

                    case "CreateZipFile":
                        {
                            string zipPath = $@"{System.Environment.CurrentDirectory}\{item.ComdOrParam}";
                            if (Directory.Exists($@"D:\litepoint"))
                            {
                                Directory.Delete($@"D:\litepoint");
                            }
                            if (File.Exists(zipPath))
                            {
                                File.Delete(zipPath);
                            }
                            Directory.CreateDirectory($@"D:\litepoint");
                            rReturn = CompressFile($@"D:\litepoint", zipPath);
                        }
                        break;

                    case "CompressFile":
                        {
                            string zipPath = item.ExpectStr.Replace($@"./", $@"{System.Environment.CurrentDirectory}\").Replace("SN", SN).Replace("DateTime.Now:yyyy-MM-dd_hh-mm-ss", $"{startTime:yyyy-MM-dd_HH-mm-ss}");
                            
                            loggerInfo("压缩后的文件路径:"+zipPath);
                                

                            var files = Directory.GetFileSystemEntries(item.ComdOrParam);
                            if (files.Length != 0)
                            {
                                string zipPathPass = zipPath.Replace("SaveData", $@"SaveData\{Global.TESTMODE}\pass");
                                string zipPathFail = zipPath.Replace("SaveData", $@"SaveData\{Global.TESTMODE}\fail");
                                foreach (var file in files)
                                {
                                    if (File.Exists(zipPathPass))//说明wifi测试pass了，接着要看当前的bt结果了
                                    {
                                        loggerInfo("存在了pass路径:" + zipPathPass);
                                        loggerInfo("看看file=" + file);

                                        if (file.ToLower().EndsWith("pass"))//bt 测试pass，接着向之前的pass路径压缩
                                        {
                                            zipPath = zipPathPass;
                                            break;
                                        } 
                                        else if (file.ToLower().EndsWith("fail")) //bt测试fail了，把之前的pass路径移动到fail路径
                                        {
                                            zipPath = zipPathFail;
                                            if (!Directory.Exists(Path.GetDirectoryName(zipPathFail))) {
                                                Directory.CreateDirectory(Path.GetDirectoryName(zipPathFail));
                                            }

                                            loggerInfo("移动路径:from:" + zipPathPass + " to:" + zipPathFail);
                                            File.Move(zipPathPass, zipPathFail);
                                            break;
                                        }
                                        else {
                                        
                                        
                                        }
                                    }
                                    else if (File.Exists(zipPathFail))//说明wififail了，bt继续压缩到这个fail路径
                                    {
                                        zipPath = zipPathFail;
                                        break;
                                    }
                                    else //wifi首次测试，此时没有fail或者pass文件夹
                                    {

                                        loggerInfo("xxxxxxxxxxx 当前路径:" + file);
                                        if (file.ToLower().EndsWith("pass"))
                                        {
                                            zipPath = zipPathPass;
                                            break;
                                        }
                                        else if (file.ToLower().EndsWith("fail"))
                                        {
                                            zipPath = zipPathFail;
                                            break;
                                        }
                                        else {
                                            //bt 会在对应软件log下面生成pass或者fail文件夹，本次snowbird wifi没有生成pass/fai文件夹
                                            loggerInfo("没有pass或者fail结尾,当做pass");
                                            zipPath = zipPathPass;

                                        }
                                    }
                                }

                                loggerInfo("CompressFile的最终路径：" + zipPath);

                                foreach (var file in files)
                                {
                                    if (file.Contains(SN))
                                        return rReturn = CompressFile(file, zipPath);
                                }
                                loggerDebug($"No {SN} file found!");
                            }
                            else
                            {
                                loggerDebug("Directory is empty!");
                            }
                        }
                        break;

                    case "CheckEeroTest":
                        {
                            try
                            {
                                
                                if (mescheckroute.CheckEeroTest(SN, Global.TESTMODE, out string mesMsg) && mesMsg.Contains("OK"))
                                {
                                   
                                    rReturn = true;
                                }
                                else
                                {
                                   
                                    loggerError("mesMsg:" + mesMsg);
                                    MainForm.f1.ShowLbl_FAIL_TEXT(mesMsg);
                                }
                            }
                            catch (Exception ex) {
                                MessageBox.Show(ex.Message);
                                rReturn = false;
                            }
                        }
                        break;

                    case "Checkroute":
                        {
                             
                              

                            if (mescheckroute.checkroute(SN, Global.FIXTURENAME, out string mesMsg) && mesMsg.Contains("OK"))
                            {
                                rReturn = true;
                            }
                            else
                            {
                                loggerError("mesMsg:" + mesMsg);
                                MainForm.f1.ShowLbl_FAIL_TEXT(mesMsg);
                               
                                
                                if (mesMsg.Contains("recheck"))
                                {
                                    error_code = ErrorList[0].Split(':')[0].Trim();
                                    error_details = ErrorList[0].Split(':')[1].Trim();
                                    testPhase.phase_details = error_details;
                                    testPhase.error_code = error_code;
                                }
                                else if (mesMsg.Contains("失败"))
                                {
                                    error_code = ErrorList[1].Split(':')[0].Trim();
                                    error_details = ErrorList[1].Split(':')[1].Trim();
                                    testPhase.phase_details = error_details;
                                    testPhase.error_code = error_code;
                                }
                                else
                                {
                                    error_code = ErrorList[2].Split(':')[0].Trim();
                                    error_details = ErrorList[2].Split(':')[1].Trim();
                                    testPhase.phase_details = error_details;
                                    testPhase.error_code = error_code;
                                }
                                loggerError($"test fail,set error_code:{error_code},error_details:{error_details}");
                            }
                        }
                        break;

                    case "ShopFloorCheck":
                        {
                            string url = $@"http://{Global.MescheckrouteIP}:8092/api/CHKRoute/serial/{SN}/station/{Global.STATIONNO}";
                            //StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                            string response = HttpPost(url, null, out string statusCode);
                            if (response.Contains(item.ExpectStr))
                            {
                                rReturn = true;
                            }
                            else
                            {
                                loggerError("mesMsg:" + response);
                                if (response.Contains("recheck"))
                                {
                                    error_code = ErrorList[0].Split(':')[0].Trim();
                                    error_details = ErrorList[0].Split(':')[1].Trim();
                                    testPhase.phase_details = error_details;
                                    testPhase.error_code = error_code;
                                }
                                else if (response.Contains("失败"))
                                {
                                    error_code = ErrorList[1].Split(':')[0].Trim();
                                    error_details = ErrorList[1].Split(':')[1].Trim();
                                    testPhase.phase_details = error_details;
                                    testPhase.error_code = error_code;
                                }
                                else
                                {
                                    error_code = ErrorList[2].Split(':')[0].Trim();
                                    error_details = ErrorList[2].Split(':')[1].Trim();
                                    testPhase.phase_details = error_details;
                                    testPhase.error_code = error_code;
                                }
                                loggerError($"test fail,set error_code:{error_code},error_details:{error_details}");
                            }
                        }
                        break;

                    case "GetWorkOrder":
                        {
                            string getUrl = "";
                            if (string.IsNullOrEmpty(item.ComdOrParam))
                                getUrl = $@"http://{Global.MESIP}:{Global.MESPORT}/api/1/serial/{SN}";
                            else
                                getUrl = item.ComdOrParam;
                            loggerDebug($"Start get WorkOrder from MES, http url:{getUrl}");
                            string response = HttpGet(getUrl);
                            response = Regex.Replace(response, "\"", "");
                            response = response.Trim();
                            loggerDebug($"Get workOrder from Mes:{response}.");
                            if (!response.ToLower().Contains("fail") || !response.ToLower().Contains("error"))
                            {
                                WorkOrder = response;
                                rReturn = true;
                            }
                            else
                            {
                                WorkOrder = "0";
                                rReturn = false;
                            }
                        }
                        break;

                    case "POEconfig":
                        {
                            
                            rReturn = PoeConfigSetting(Global.POE_PORT, item.ComdOrParam);
                        }
                        break;
                    case "POEReadConsumption":
                        {
                            //Random r = new Random();
                            //int num = r.Next(1, 101);
                            var cookies = new CookieContainer();
                            var client = GetClient(cookies, "admin", "admin123");
                            string url = $@"http://169.254.100.101/api/v1/service";
                            string data = $"{{\"method\":\"poe.status.interface.get\",\"params\":[],\"id\":138}}";
                            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                            var result = client.PostAsync(url, content).Result;
                            var response_content = result.Content.ReadAsByteArrayAsync().Result;
                            var responseStr = System.Text.Encoding.UTF8.GetString(response_content);
                            logger.Debug(result.StatusCode + ":" + responseStr);

                            Root root = JsonConvert.DeserializeObject<Root>(responseStr);

                            if (root != null && root.result != null && root.result.Count > 0)
                            {
                                logger.Debug("~~~~~~~~~~~~~~~~~~:" + Global.POE_PORT);
                                logger.Info(">>>>>>>>>>>Consumption=" + root.result[int.Parse(Global.POE_PORT) - 1].val.PowerConsumption);

                                double number = (double)(root.result[int.Parse(Global.POE_PORT) - 1].val.PowerConsumption) / 10;
                                item.testValue = number.ToString("F2");
                                rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);

                            }
                            else
                            {
                                logger.Error("POE信息解析失败!!!");
                            }

                        }
                        break;

                    case "ChagePOERate":
                        {
                            var cookies = new CookieContainer();
                            var client = GetClient(cookies, "admin", "admin123");
                            string url = $@"http://169.254.100.101/api/v1/service";


                            var dict = new Dictionary<string, object>();


                            //var type = "force2G5ModeFdx"; // "force2G5ModeFdx"   "force1GModeFdx"  "force100ModeFdx" "force100ModeHdx"

                            var type = item.ComdOrParam;


                            dict.Add("method", "port.config.set");
                            dict.Add("id", 240);
                            List<object> list = new List<object>();
                            dict.Add("params", list);
                            list.Add($"2.5G 1/{Global.POE_PORT}");
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
                            if (type == "force2G5ModeFdx")
                            {
                                dict2.Add("AdvertiseDisabled", 969);

                            }
                            else if (type == "force1GModeFdx")
                            {
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

                            logger.Info(data);

                            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                            var result = client.PostAsync(url, content).Result;
                            var response_content = result.Content.ReadAsByteArrayAsync().Result;
                            var responseStr = System.Text.Encoding.UTF8.GetString(response_content);

                            if (result.IsSuccessStatusCode && responseStr.Contains("\"error\":null"))
                            {
                                logger.Info("set success:" + responseStr);
                                rReturn = true;
                            }
                            else
                            {
                                logger.Error("set fail:" + responseStr);
                            }
                           

                        }
                        break;

                    case "HardwareReset": {

                            loggerInfo($"-----HardWareReboot------");
                            rReturn = PowerCycleOutlet(int.Parse(Global.POE_PORT));
                        }
                        break;


                    case "HardwareResetWPS":
                        {
                            //#if DEBUG
                            //                            return rReturn = true;
                            //#endif
                            Sleep(1000);

                            if (DUTCOMM != null && DUTCOMM.GetType() == typeof(Telnet))
                            {
                                DUTCOMM.Close();
                            }


                            PowerCycleOutletWPS(int.Parse(Global.WPSPortNum));

                            Sleep(3000);
                            loggerInfo("开始pingdutip:" + Global.WPSDUTIP);
                            if (!PingIP(Global.WPSDUTIP, 2))
                            {
                                Sleep(item.TimeOut);
                                return rReturn = true;
                            }

                            if (!rReturn)
                            {
                                loggerInfo("不成功重新断电上电");
                                Power_OnOff_WPS(int.Parse(Global.WPSPortNum), false);
                                Sleep(3000);
                                Power_OnOff_WPS(int.Parse(Global.WPSPortNum), true);
                                if (!PingIP(Global.WPSDUTIP, 2))
                                {
                                    Sleep(item.TimeOut);
                                    return rReturn = true;
                                }
                            }
                        }
                        break;









                    case "SoftwareReset":
                        {  

                            loggerInfo($"-----SoftWareReboot------");
                            var recvStr = "";
                            DUTCOMM.SendCommand("reboot", ref recvStr, "", 10);

                            bool notShutDown = true;
                            for (var i = 0; i < 10; i++) {
                                bool r = PingIP(Global.DUTIP, 1);
                                if (r == false) {
                                    notShutDown = false;
                                    break;
                                }
                                Thread.Sleep(1000);
                            }

                            rReturn = !notShutDown;
                                            

                        }
                        break;


                    case "PowerOFFTest":
                        {



                            Regex regex = new Regex(@"_(\d+)$");
                            Match match = regex.Match(item.ItemName);
                            if (match.Success)
                            {
                                var loopNum = match.Groups[1].Value;
                                loggerInfo($"-------------------{loopNum } Power cycle test -----------------");
                            }


                            bool powercycle = PowerCycleOutlet(int.Parse(Global.POE_PORT));

                            rReturn = powercycle;
                          
                         
                        }
                        break;

                    case "PowerCycleTest":
                        {
               
                               
                                bool powercycle_ALL = true;
                                for (int i = 0; i < int.Parse(item.ComdOrParam); i++)
                                {
                                    loggerInfo($"-------------------{i+1} Power cycle test -----------------");
                                   
                                    bool powercycle = PowerCycleOutletWPS(int.Parse(Global.POE_PORT));
                                                               
                                   //Thread.Sleep(1000);
                                    powercycle_ALL &= powercycle;
                                    if (!powercycle)
                                    {
                                        break;
                                    }
                                }
                                if (powercycle_ALL)
                                {
                                    rReturn = true;
                                }
                            
                        }
                        break;

                      


                    case "CheckEeroABA":
                        {
                            if (mescheckroute.checkEeroABA(SN, Global.FIXTURENAME, Global.STATIONNAME, out string mesMsg) && mesMsg.Contains("OK"))
                            {
                                rReturn = true;
                            }
                            else
                            {
                                loggerError("mesMsg:" + mesMsg);
                                MainForm.f1.ShowLbl_FAIL_TEXT(mesMsg);
                            } 
                        }
                        break;

                    case "GetPcbaErroMessage":
                        {

                            //  rReturn = true;



                           // return rReturn;




                            MesMac = "";
                            // loggerInfo("MesMac 置空");
                            if (mescheckroute.GetPcbaErroMessage(SN, out CSN, out MesMac, out string mesMsg))
                            {
                                loggerDebug("CUSTOMER_SN:" + CSN + ", GetMesMac:" + MesMac);
                                rReturn = true;
                            }
                            loggerDebug("mesMsg:" + mesMsg);

                        }
                        break;

                    case "GetCsnErroMessage":
                        {
                            MesMac = "";
                           // loggerInfo("MesMac 置空");
                            if (mescheckroute.GetCsnErroMessage(SN, out CSN, out string IPSN, out MesMac, out string mesMsg))
                                rReturn = true;
                            loggerDebug("mesMsg:" + mesMsg);
                            loggerDebug("Get MesMac:" + MesMac + ", sn:" + CSN + ", IPSN:" + IPSN);
                            
                        }
                        break;

                    //case "ReportChildBoard":
                    //    {
                    //        if (mescheckroute.GetCsnErroMessage(SN, out string serialNum, out string ItemPartSN, out string MesMac, out string mesMsg)
                    //            && mesMsg.Contains("OK"))
                    //        {
                    //            item.testValue = ItemPartSN.Trim();
                    //            mesPhases.ChildBoardSN = ItemPartSN.Trim();
                    //            rReturn = true;
                    //        }
                    //    }
                    //    break;

                    case "GetMesIP":
                        {
                            if (mescheckroute.GETIP(SN, out DUTMesIP, out string mesMsg) && mesMsg.Contains("OK"))
                            {
                                rReturn = true;
                                loggerDebug("mesMsg:" + mesMsg);
                                loggerDebug("sn:" + SN + ", MesIP:" + DUTMesIP);
                            }
                            else {
                                loggerDebug("fail sn:" + SN);
                                loggerDebug("mesMsg:" + mesMsg);
                               
                            }
                            
                        }
                        break;

                    case "FIXCOMSend_Custom":
                        {
                            if (Global.FIXTUREFLAG == "1")
                            {

                                if ( 
                                       ((Global.STATIONNAME=="MBFT" || Global.STATIONNAME=="SRF") && SRF_POP_RETRY == 1)
                                        ||
                                        ((Global.STATIONNAME == "RTT") && RTT_PING_RETRY == 1)

                                    )
                                {
                                    FixSerialPort.OpenCOM();
                                    var revStr = "";
                                    inPutValue = "";
                                    if (FixSerialPort.SendCommandToFix(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                         && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                                    {
                                        rReturn = true;
                                        // 需要提取测试值
                                        if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                                        {
                                            item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);
                                            if (item.TestKeyword == "CurrentTest") //治具返回的电流是mA，需要转成A
                                            {
                                                item.testValue = (double.Parse(item.testValue) / 1000).ToString();
                                            }
                                        }
                                        else
                                        {
                                            return rReturn = true;
                                        }

                                        // 需要比较Spec
                                        if (!string.IsNullOrEmpty(item.Spec) && string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                        {
                                            return rReturn = CheckSpec(item.Spec, item.testValue);
                                        }

                                        // 需要比较Limit
                                        if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max))
                                        {
                                            rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                        }
                                        if (item.TestKeyword == "CurrentTest")
                                        {
                                            HandleSpecialMethed(item, rReturn, revStr);
                                        }
                                    }
                                    else
                                    {
                                        if (item.TestKeyword == "CurrentTest")
                                        {
                                            HandleSpecialMethed(item, rReturn, revStr);
                                        }
                                    }

                                }
                                else {

                                    return rReturn = true;


                                }


                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.CheckStr2))
                                {
                                    // 不使用治具，messageboxshow提示作业员操作，然后确认。
                                    rReturn = ConfirmMessageBox(item.CheckStr2, item.ItemName);
                                }
                                else
                                {
                                    loggerWarn($"Attention! FIXTUREFLAG == {Global.FIXTUREFLAG},and no ConfirmMessageBox. ");
                                    rReturn = true;
                                }
                            }
                        }
                        break;

                    case "CurrentTest":
                    case "FIXCOMSend":
                        {
                            if (Global.FIXTUREFLAG == "1")
                            {
                                //using (var FIXCOMM = new Comport(FixCOMinfo))
                                //{


                                if (FixSerialPort == null) {

                                    return rReturn = false;
                                }


                                FixSerialPort.OpenCOM();
                                var revStr = "";
                                inPutValue = "";
                                if (FixSerialPort.SendCommandToFix(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                     && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                                {
                                    rReturn = true;
                                    // 需要提取测试值
                                    if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                                    {
                                        item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);
                                        if (item.TestKeyword == "CurrentTest") //治具返回的电流是mA，需要转成A
                                        {
                                            item.testValue = (double.Parse(item.testValue) / 1000).ToString();
                                        }
                                    }
                                    else
                                    {
                                        return rReturn = true;
                                    }

                                    // 需要比较Spec
                                    if (!string.IsNullOrEmpty(item.Spec) && string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                    {
                                        return rReturn = CheckSpec(item.Spec, item.testValue);
                                    }

                                    // 需要比较Limit
                                    if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max))
                                    {
                                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                    }
                                    if (item.TestKeyword == "CurrentTest")
                                    {
                                        HandleSpecialMethed(item, rReturn, revStr);
                                    }
                                }
                                else {
                                    if (item.TestKeyword == "CurrentTest")
                                    {
                                        HandleSpecialMethed(item, rReturn, revStr);
                                    }
                                }
                                //    FIXCOMM.Close(); FIXCOMM.Dispose();
                                //}
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.CheckStr2))
                                {
                                    // 不使用治具，messageboxshow提示作业员操作，然后确认。
                                    rReturn = ConfirmMessageBox(item.CheckStr2, item.ItemName);
                                }
                                else
                                {
                                    loggerWarn($"Attention! FIXTUREFLAG == {Global.FIXTUREFLAG},and no ConfirmMessageBox. ");
                                    rReturn = true;
                                }
                            }
                        }
                        break;

                    case "GROUND_POINT_MEAS": {
                            InutMEASPOP usbDialog = new InutMEASPOP();
                            double value=-1;
                            usbDialog.TextHandler = (str) => {

                                value = double.Parse(str);
                            };
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.TopMost = true;
                            usbDialog.ShowTip();
                            usbDialog.ShowDialog();

                            loggerInfo("输入的值:" + value);
                            item.testValue = value.ToString();
                            rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);

                            int ii = 0;
                        }
                        break;







                    case "CheckLED_Manual":
                        {
                            LEDSHow usbDialog = new LEDSHow();
                            var value ="";
                            usbDialog.TextHandler = (str) => {

                                value = str;
                            };
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.TopMost = true;
                            usbDialog.ShowTip(item.ComdOrParam);
                            usbDialog.ShowDialog();

                            loggerInfo("点击:" + value);
                            item.testValue = value.ToString();
                            if (value == "yes")
                            {
                                rReturn = true;
                            }
                            else { 
                            
                            }
                        }
                        break;
                        


                    case "ShowFixtureTip": {
                          
                            
                            
                            //ShowFixtureTip usbDialog = new ShowFixtureTip();
                            //usbDialog.TextHandler = (str) => { };
                            //usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            //usbDialog.ShowTip();
                            //usbDialog.ShowDialog();
                           
                            
                            
                            rReturn = true;
                        }
                        break;


                    case "PressButtonShow": {
                            ButtonShow usbDialog = new ButtonShow();
                            usbDialog.TextHandler = (str) => { };
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.TopMost = true;
                            usbDialog.ShowPressTip();
                            usbDialog.ShowDialog();
                            rReturn = true;
                        }
                        break;

                    case "ReleaseButtonShow":
                        {
                            ButtonShow usbDialog = new ButtonShow();
                            usbDialog.TextHandler = (str) => { };
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.TopMost = true;
                            usbDialog.ShowReleaseTip();
                            usbDialog.ShowDialog();
                            rReturn = true;
                        }
                        break;

                    case "ReadyToTest":
                        {





                            ReadyToTest usbDialog = new ReadyToTest();
                          
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.ShowTip();



                            usbDialog.TopMost = true;
                            //// 设置窗体为无边框样式
                            usbDialog.FormBorderStyle = FormBorderStyle.None;
                            // 最大化窗体
                            usbDialog.WindowState = FormWindowState.Maximized;


                            usbDialog.ShowDialog();

                            rReturn=true;



                        }
                        break;

                    case "PressOrReleaseButtonShowNoRes":
                        {


                           
                            int timeout = int.Parse(item.TimeOut);
                            var lngStart = DateTime.Now.AddSeconds(timeout).Ticks;
                            var lngCurTime = DateTime.Now.Ticks;

                            ButtonShow usbDialog = new ButtonShow();
                            usbDialog.TextHandler = (str) => { };
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.ShowTip(item.CheckStr2);
                            usbDialog.TopMost = true;

                            Task.Factory.StartNew(() =>
                            {

                                FixSerialPort.OpenCOM();
                                var revStr = "";
                                inPutValue = "";


                                while (lngCurTime < lngStart)
                                {

                                    lngCurTime = DateTime.Now.Ticks;


                                    var res = DUTCOMM.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, 5);

                                    loggerDebug(res.ToString());

                                    var checkStr = item.CheckStr2 == "1" ? "OK!10" : "OK!11";

                                    if (revStr.CheckStr(checkStr))
                                    {
                                        rReturn = true;

                                        break;
                                    }

                                    Thread.Sleep(200);

                                }

                                usbDialog.Invoke((MethodInvoker)delegate
                                {
                                    usbDialog.CloseDia();
                                    usbDialog.Focus();

                                });


                            });

                            // 设置窗体为无边框样式
                            usbDialog.FormBorderStyle = FormBorderStyle.None;
                           // 最大化窗体
                           usbDialog.WindowState = FormWindowState.Maximized;


                           usbDialog.ShowDialog();



                         

                        }
                        break;

                    case "ReleaseButtonShowNoRes":
                        {
                            ButtonShow usbDialog = new ButtonShow();
                            usbDialog.TextHandler = (str) => { };
                            usbDialog.StartPosition = FormStartPosition.CenterScreen;
                            usbDialog.TopMost = true;
                            usbDialog.ShowReleaseTip();
                            usbDialog.ShowDialog();
                            rReturn = true;
                        }
                        break;


                    case "WhiteLEDTest":
                        {

                            FixSerialPort.OpenCOM();
                            Thread.Sleep(500);
                            var revStr = "";
                            if (FixSerialPort.SendCommandToFix(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                 && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                rReturn = true;

                            }
                            else
                            {

                                rReturn = false;
                            }



                            if (rReturn)
                            {

                                // 需要提取测试值
                                if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                                {
                                    item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);

                                    // 需要比较Spec
                                    if (!string.IsNullOrEmpty(item.Spec) && string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                    {
                                        rReturn = CheckSpec(item.Spec, item.testValue);
                                    }
                                    // 需要比较Limit
                                    if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max))
                                    {
                                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                    }

                                }
                                else
                                {
                                    rReturn = true;
                                }


                            }
                            

                            if (rReturn == false)
                            {
                                loggerWarn("WhiteLEDTest retry=" + retryTimes.ToString());
                                if (retryTimes == 6 || retryTimes == 3)
                                {

                                    var recvStr = "";

                                    FixSerialPort.SendCommandToFix("AT+PORTEJECT%", ref recvStr, "OK", 10);

                                    Sleep(500);

                                    FixSerialPort.SendCommandToFix("AT+PORTINSERT%", ref recvStr, "OK", 10);



                                }

                            }

                        }


                        break;


                    case "ClearInput":
                        {

                            inPutValue = "";
                            loggerInfo("inPutValu 清空");
                            rReturn = true;
                        }
                        break;



                    case "VoltageTest":
                    case "LEDTest":
                        if (inPutValue == "")
                        {
                            FixSerialPort.OpenCOM();
                            Thread.Sleep(500);
                            var revStr = "";
                            if (FixSerialPort.SendCommandToFix(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                 && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                rReturn = true;
                                inPutValue = revStr;
                            }
                            else
                            {
                                inPutValue = "";
                                return rReturn = false;
                            }
                        }
                        {
                            var revStr = inPutValue;
                            // 需要提取测试值
                            if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                            {
                                item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2).Replace(";", "");
                            }
                            else
                            {
                                return rReturn = true;
                            }
                            // 需要比较Spec
                            if (!string.IsNullOrEmpty(item.Spec) && string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                            {
                                return rReturn = CheckSpec(item.Spec, item.testValue);
                            }
                            // 需要比较Limit
                            if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max))
                            {
                                rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                            }
                        }
                        if (!rReturn)
                        {
                            inPutValue = "";
                        }
                        if (!rReturn) {
                           // HandleSpecialMethed(item, rReturn, "");
                        }

                        break;

                    case "BTPairTest":
                        rReturn = BTConnection(item.ComdOrParam, BtDevAddress, int.Parse(item.RetryTimes));
                        //rReturn = ConfrimMessageBox($"Pls connect Qorvo Inc.bt and confirm succed than click ok.", item.ItemName, MessageBoxButtons.YesNo);
                        break;



                    //case "BLE":
                    //case "Zigbee": //SRF
                    //    {
                    //        if (csvLines == null)
                    //        {
                    //            return rReturn = false;
                    //        }
                    //        for (int i = 0; i < csvLines.Length; i++)
                    //        {
                    //            string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);
                    //            if (temp[0] == item.TestKeyword && temp[1] == item.SubStr1 && temp[3] == item.ComdOrParam)
                    //            {
                    //                loggerInfo($"find test result in csv line{i + 1}.testResult={temp[57]}");
                    //                if (item.ItemName.ToLower().Contains("power"))
                    //                {
                    //                    item.testValue = temp[33];
                    //                    item.Unit = temp[35];
                    //                    item.Limit_min = temp[36];
                    //                    item.Limit_max = temp[37];
                    //                }

                    //                if (item.ItemName.ToLower().Contains("rx_per"))
                    //                {
                    //                    item.testValue = temp[49];
                    //                    item.Unit = temp[50];
                    //                    item.Limit_min = temp[51];
                    //                    item.Limit_max = temp[52];
                    //                }

                    //                if (item.ItemName.ToLower().Contains("pathloss"))
                    //                {
                    //                    item.testValue = temp[46];
                    //                    item.Unit = "dB";
                    //                }

                    //                if (item.ItemName == "BLE_RX_PER_F2450_P-90-97" || item.ItemName == "ZB_RX_PER_F2450_P-95-102")
                    //                {
                    //                    item.testValue = temp[47];
                    //                    item.Unit = "dB";
                    //                    item.Limit_min = "";
                    //                    item.Limit_max = "";
                    //                }


                    //                if (string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                    //                    rReturn = temp[57].ToLower() == "pass" ? true : false;
                    //                else
                    //                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);

                    //                if (!rReturn)
                    //                {
                    //                    ErrorList = temp[2].Trim().Split(new string[] { "\n" }, 0);

                    //                    //if (temp[2] == "" || temp[2] == "NA") {
                    //                    //    ErrorList = item.ErrorCode.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    //                    //}


                    //                }

                    //                return rReturn;
                    //            }
                    //        }
                    //        loggerError($"Don't find test result in csv,test fail!");
                    //    }
                    //    break;





                    case "BLE":
                    case "Zigbee": 
                        {
                            if (csvLines == null) { 
                               return rReturn = false;
                            }
                            for (int i = 0; i < csvLines.Length; i++)
                            {
                                string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);


                                if (temp[0] == item.TestKeyword && temp[1] == item.SubStr1 && temp[3] == item.ComdOrParam)
                                {
                                    loggerInfo($"find test result in csv line{i + 1}.testResult={temp[57]}");
                                    if (item.ItemName.ToLower().Contains("power"))
                                    {
                                        item.testValue = temp[33];


                                        if (temp[36] != "" && temp[36] != "NA" && temp[36] != "N/A" && temp[36] != "null" && temp[36] != "None" && temp[36] != "NULL")
                                        {
                                            item.Limit_min = temp[36];
                                        }

                                        if (temp[37] != "" && temp[37] != "NA" && temp[37] != "N/A" && temp[37] != "null" && temp[37] != "None" && temp[37] != "NULL")
                                        {
                                            item.Limit_max = temp[37];
                                        }

                                        if (temp[35] != "" && temp[35] != "NA" && temp[35] != "N/A" && temp[35] != "null" && temp[35] != "None" && temp[35] != "NULL")
                                        {
                                            item.Unit = temp[35];
                                        }

                                        //item.Unit = temp[35];
                                        //item.Limit_min = temp[36];
                                        //item.Limit_max = temp[37];
                                    }

                                    if (item.ItemName.ToLower().Contains("rx_per"))
                                    {
                                       
                                        
                                        item.testValue = temp[49];


                                        if (temp[51] != "" && temp[51] != "NA" && temp[51] != "N/A" && temp[51] != "null" && temp[51] != "None" && temp[51] != "NULL")
                                        {
                                            item.Limit_min = temp[51];
                                        }

                                        if (temp[52] != "" && temp[52] != "NA" && temp[52] != "N/A" && temp[52] != "null" && temp[52] != "None" && temp[52] != "NULL")
                                        {
                                            item.Limit_max = temp[52];
                                        }

                                        if (temp[50] != "" && temp[50] != "NA" && temp[50] != "N/A" && temp[50] != "null" && temp[50] != "None" && temp[50] != "NULL")
                                        {
                                            item.Unit = temp[50];
                                        }

                                        //item.Unit = temp[50];
                                        //item.Limit_min = temp[51];
                                        //item.Limit_max = temp[52];
                                    }






                                    if (item.ItemName.ToLower().Contains("pathloss"))
                                    {
                                        item.testValue = temp[46];
                                        item.Unit = "dB";
                                    }
                                    
                                    //if (item.ItemName== "BLE_RX_PER_F2450_P-90-97" || item.ItemName== "ZB_RX_PER_F2450_P-95-102")
                                    //{
                                    //    item.testValue = temp[47];
                                    //    item.Unit = "dB";
                                    //    item.Limit_min = "";
                                    //    item.Limit_max = "";
                                    //}
 

                                    if (string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                        rReturn = temp[57].ToLower() == "pass" ? true : false;
                                    else
                                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);



                                    if (temp[57] == "FAIL")
                                    {
                                        loggerError("zhubo csv 返回fail，所以为fail");
                                        rReturn = false;

                                    }



                                    if (!rReturn) {
                                        ErrorList = temp[2].Trim().Split(new string[] { "\n" }, 0);

                                        //if (temp[2] == "" || temp[2] == "NA") {
                                        //    ErrorList = item.ErrorCode.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                        //}
                                        
                                       
                                    }

                                    return rReturn;
                                }
                            }
                            loggerError($"Don't find test result in csv,test fail!");
                        }
                        break;

                    case "2G":
                    case "5G"://SRF
                        {
                            for (int i = 0; i < csvLines.Length; i++)
                            {
                                string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);

                                if (temp[0] == item.TestKeyword && temp[1] == item.SubStr1 && temp[4] == item.SubStr2 && temp[3] == item.ComdOrParam)
                                {
                                    loggerInfo($"find test result in csv line{i + 1}.testResult={temp[15]}");
                                    rReturn = temp[15].ToLower() == "pass" ? true : false;
                                    if (!rReturn)
                                    {
                                        try
                                        {
                                            ErrorList = temp[2].Trim().Split(new string[] { "\n" }, 0);
                                            return rReturn;
                                        }
                                        catch (Exception ex)
                                        {
                                            loggerError(ex.Message);
                                            ErrorList = item.error_code.Trim().Split(new string[] { "\n" }, 0);
                                            return rReturn;
                                        }

                                    }


                                    if (item.ItemName.ToLower().Contains("pathloss"))
                                    {
                                        item.testValue = temp[11];
                                        item.Unit = "dB";
                                    }
                                    else if (item.ItemName.ToLower().Contains("rx_per"))
                                    {
                                        //item.testValue = temp[13];
                                        //item.Unit = temp[14];
                                        //item.Limit_min = "0";
                                        //item.Limit_max = "10.0";

                                       // item.testValue = temp[12];//Rx_Level power
                                      
                                        
                                        item.testValue = temp[13];//PER
                                        item.Unit = temp[14];
                                     
                                        
                                        
                                        item.Limit_min = temp[9];
                                        item.Limit_max = temp[10];
                                        //item.Unit = "%";
                                    }
                                    else if (item.ItemName.ToLower().Contains("tx_power"))
                                    { 
                                        item.testValue = temp[7];
                                        item.Unit = temp[8];
                                        item.Limit_min = temp[9];
                                        item.Limit_max = temp[10];
                                    }

 
                                    if (string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                        rReturn = temp[15].ToLower() == "pass" ? true : false;
                                    else
                                    {
                                        loggerInfo("MinVlaue:" + item.Limit_min + " MaxValue:" + item.Limit_max + " value:" + item.testValue);
                                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);

                                    }

                                    if (!rReturn) {
                                        
                                        ErrorList = temp[2].Trim().Split(new string[] { "\n" }, 0);
 
                                    }
                                       

                                    return rReturn;
                                }
                            }
                            loggerError($"Don't find test result in csv,test fail!");
                        }
                        break;

                    case "6G_RadioValidation":
                    case "5G_RadioValidation":
                    case "2G_RadioValidation":
                        {
                            if (!IsNullOrEmpty(item.CheckStr1))
                            {
                                //这里是加上powerSpec
                                item.ItemName = item.ItemName + "-" + item.CheckStr1;
                            }



                            /// MBFT
                            for (int i = 1; i < csvLines.Length; i++)
                            {
                                string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);

                                 if (item.ItemName== "WIFI_TX_POWER_F5180_HE_BW20_MCS0_C0")
                                {
                                    int ii = 0;
                                }

                                    //5G ,  TX   , 0/1 ,   5775 ,    HE_SU-MCS13/NA ,  
                                    if ((temp[0] == item.TestKeyword.Substring(0, 2) && temp[1] == item.SubStr1 && temp[4] == item.SubStr2 && temp[5].Trim() == item.ComdOrParam && temp[6].Trim() == item.ExpectStr)
                                    && (temp[21].Trim() == item.CheckStr1 || IsNullOrEmpty(item.CheckStr1)))
                                {
                                    loggerInfo($"find test result in csv line{i + 1}.testResult={temp[46]}");
                                    rReturn = temp[46].ToLower() == "pass" ? true : false;
                                    if (!rReturn)
                                    {

                                        try
                                        {
                                            ErrorList = temp[3].Trim().Split(new string[] { "\n" }, 0);
                                            ValidationFail = true;
                                            // return rReturn;
                                        }
                                        catch (Exception ex)
                                        {
                                            loggerError("~~~~~~~~~" + ex.ToString());
                                            ValidationFail = true;
                                            ErrorList = item.error_code.Trim().Split(new string[] { "\n" }, 0);
                                            // return rReturn;
                                        }

                                    }




                                    if (
                                        item.ItemName.ToLower().Contains("rx_per")
                                        || 
                                        item.ItemName.ToLower().Contains("rx_power") // 非Sweep版本的时候使用
                                        
                                        )
                                    {

                                        try
                                        {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_max = "0.0";
                                        }


                                        item.testValue = temp[37];


                                        if (temp[39] != "" && temp[39] != "NA" && temp[39] != "N/A" && temp[39] != "null" && temp[39] != "None" && temp[39] != "NULL")
                                        {
                                            item.Limit_min = temp[39];
                                        }

                                        if (temp[40] != "" && temp[40] != "NA" && temp[40] != "N/A" && temp[40] != "null" && temp[40] != "None" && temp[40] != "NULL")
                                        {
                                            item.Limit_max = temp[40];
                                        }

                                        if (temp[38] != "" && temp[38] != "NA" && temp[38] != "N/A" && temp[38] != "null" && temp[38] != "None" && temp[38] != "NULL")
                                        {
                                            item.Unit = temp[38];
                                        }




                                    }

                                    //else if (item.ItemName.ToLower().Contains("rx_power"))//这是sweep版本的时候取值，非sweep版不用
                                    //{


                                    //    item.testValue = temp[42];
                                    //    item.Unit = temp[43];
                                    //    item.Limit_min = temp[44];
                                    //    item.Limit_max = temp[45];

                                    //}






                                    else if (item.ItemName.ToLower().Contains("tx_power"))
                                    {
                                        if (item.ItemName.ToLower().Contains("user1"))
                                        {
                                            item.testValue = temp[66];
                                            item.Unit = temp[67];
                                            item.Limit_min = temp[68];
                                            item.Limit_max = temp[69];
                                        }
                                        else if (item.ItemName.ToLower().Contains("user2"))
                                        {
                                            item.testValue = temp[70];
                                            item.Unit = temp[71];
                                            item.Limit_min = temp[72];
                                            item.Limit_max = temp[73];
                                        }
                                        else
                                        {

                                            try
                                            {
                                                item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                            }
                                            catch
                                            {
                                                item.Limit_min = "0.0";
                                            }

                                            try
                                            {
                                                item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                            }
                                            catch
                                            {
                                                item.Limit_max = "0.0";
                                            }



                                            item.testValue = temp[20];

                                            if (temp[23] != "" && temp[23] != "NA" && temp[23] != "N/A" && temp[23] != "null" && temp[23] != "None" && temp[23] != "NULL")
                                            {
                                                item.Limit_min = temp[23];
                                            }

                                            if (temp[24] != "" && temp[24] != "NA" && temp[24] != "N/A" && temp[24] != "null" && temp[24] != "None" && temp[24] != "NULL")
                                            {
                                                item.Limit_max = temp[24];
                                            }

                                            if (temp[22] != "" && temp[22] != "NA" && temp[22] != "N/A" && temp[22] != "null" && temp[22] != "None" && temp[22] != "NULL")
                                            {
                                                item.Unit = temp[22];
                                            }
 
                                        }
                                    }
                                    else if (item.ItemName.ToLower().Contains("tx_evm"))
                                    {
                                        try
                                        {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_max = "0.0";
                                        }


                                        if (item.ItemName.ToLower().Contains("user1"))
                                        {
                                            item.testValue = temp[98];
                                            item.Unit = temp[99];
                                            item.Limit_min = temp[100];
                                            item.Limit_max = temp[101];
                                        }
                                        else if (item.ItemName.ToLower().Contains("user2"))
                                        {
                                            item.testValue = temp[102];
                                            item.Unit = temp[103];
                                            item.Limit_min = temp[104];
                                            item.Limit_max = temp[105];
                                        }
                                        else
                                        {
                                            item.testValue = temp[8];
                                            item.Unit = temp[9];
                                            item.Limit_min = temp[10];
                                            item.Limit_max = temp[11];
                                        }
                                    }
                                   
                                    else if (item.ItemName.ToLower().Contains("tx_loleak"))
                                    {
                                        try {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch {
                                            item.Limit_max = "0.0";
                                        }

                                            


                                        item.testValue = temp[25];
                                        if (temp[27] != "" && temp[27] != "NA" && temp[27] != "N/A" && temp[27] != "null" && temp[27] != "None" && temp[27] != "NULL")
                                        {
                                            item.Limit_min = temp[27];
                                        }

                                        if (temp[28] != "" && temp[28] != "NA" && temp[28] != "N/A" && temp[28] != "null" && temp[28] != "None" && temp[28] != "NULL")
                                        {
                                            item.Limit_max = temp[28];
                                        }

                                        if (temp[26] != "" && temp[26] != "NA" && temp[26] != "N/A" && temp[26] != "null" && temp[26] != "None" && temp[26] != "NULL")
                                        {
                                            item.Unit = temp[26];
                                        }
 

                                    }



                                    else if (item.ItemName.ToLower().Contains("tx_freqerr"))
                                    {


                                        try
                                        {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_max = "0.0";
                                        }


                                        item.testValue = temp[12];



                                        if (temp[14] != "" && temp[14] != "NA" && temp[14] != "N/A" && temp[14] != "null" && temp[14] != "None" && temp[14] != "NULL")
                                        {
                                            item.Limit_min = temp[14];
                                        }

                                        if (temp[15] != "" && temp[15] != "NA" && temp[15] != "N/A" && temp[15] != "null" && temp[15] != "None" && temp[15] != "NULL")
                                        {
                                            item.Limit_max = temp[15];
                                        }

                                        if (temp[13] != "" && temp[13] != "NA" && temp[13] != "N/A" && temp[13] != "null" && temp[13] != "None" && temp[13] != "NULL")
                                        {
                                            item.Unit = temp[13];
                                        }
 

                                    }
                                    else if (item.ItemName.ToLower().Contains("tx_sysclkerr"))
                                    {

                                        try
                                        {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_max = "0.0";
                                        }


                                        item.testValue = temp[16];


                                        if (temp[18] != "" && temp[18] != "NA" && temp[18] != "N/A" && temp[18] != "null" && temp[18] != "None" && temp[18] != "NULL")
                                        {
                                            item.Limit_min = temp[18];
                                        }

                                        if (temp[19] != "" && temp[19] != "NA" && temp[19] != "N/A" && temp[19] != "null" && temp[19] != "None" && temp[19] != "NULL")
                                        {
                                            item.Limit_max = temp[19];
                                        }

                                        if (temp[17] != "" && temp[17] != "NA" && temp[17] != "N/A" && temp[17] != "null" && temp[17] != "None" && temp[17] != "NULL")
                                        {
                                            item.Unit = temp[17];
                                        }

 

                                    }
                                    else if (item.ItemName.ToLower().Contains("tx_specmsk"))
                                    {
                                        try
                                        {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_max = "0.0";
                                        }


                                        item.testValue = temp[33];


                                        if (temp[35] != "" && temp[35] != "NA" && temp[35] != "N/A" && temp[35] != "null" && temp[35] != "None" && temp[35] != "NULL")
                                        {
                                            item.Limit_min = temp[35];
                                        }

                                        if (temp[36] != "" && temp[36] != "NA" && temp[36] != "N/A" && temp[36] != "null" && temp[36] != "None" && temp[36] != "NULL")
                                        {
                                            item.Limit_max = temp[36];
                                        }

                                        if (temp[34] != "" && temp[34] != "NA" && temp[34] != "N/A" && temp[34] != "null" && temp[34] != "None" && temp[34] != "NULL")
                                        {
                                            item.Unit = temp[34];
                                        }


 

                                    }
                                    else if (item.ItemName.ToLower().Contains("tx_specflat"))
                                    {
                                        try
                                        {
                                            item.Limit_min = double.Parse(item.Limit_min).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_min = "0.0";
                                        }

                                        try
                                        {
                                            item.Limit_max = double.Parse(item.Limit_max).ToString("0.0");
                                        }
                                        catch
                                        {
                                            item.Limit_max = "0.0";
                                        }


                                        item.testValue = temp[29];



                                        if (temp[31] != "" && temp[31] != "NA" && temp[31] != "N/A" && temp[31] != "null" && temp[31] != "None" && temp[31] != "NULL")
                                        {
                                            item.Limit_min = temp[31];
                                        }

                                        if (temp[32] != "" && temp[32] != "NA" && temp[32] != "N/A" && temp[32] != "null" && temp[32] != "None" && temp[32] != "NULL")
                                        {
                                            item.Limit_max = temp[32];
                                        }

                                        if (temp[30] != "" && temp[30] != "NA" && temp[30] != "N/A" && temp[30] != "null" && temp[30] != "None" && temp[30] != "NULL")
                                        {
                                            item.Unit = temp[30];
                                        }

 
                                    }



                                    else if (item.ItemName.ToLower().Contains("tx_radio_temp"))
                                    {


                                        item.testValue = temp[130];
                                        item.Unit = "C";

                                    }



                                    if (string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                    {
                                        rReturn = temp[46].ToLower() == "pass" ? true : false;
                                    }

                                    else
                                    {

                                        //判断当前是否是GoldenSN
                                        if (Global.GoldenSN.Contains(SN) && Global.STATIONNAME == "MBFT")
                                        {
                                            if (item.ItemName.ToLower().Contains("tx_power")) //只处理tx_power,和台湾数据作对比
                                            {

                                                if (csvGoldenLines != null && csvGoldenLines.Length > 0)
                                                {
                                                    bool foundTempInfo = false;
                                                    for (int i1 = 1; i1 < csvGoldenLines.Length; i1++)
                                                    {
                                                        string[] goldenTemp = csvGoldenLines[i1].Split(new char[] { ',' }, StringSplitOptions.None);



                                                        if ((temp[0] == goldenTemp[0] && temp[1] == goldenTemp[1] && temp[4] == goldenTemp[4] && temp[5].Trim() == goldenTemp[5].Trim() && temp[6].Trim() == goldenTemp[6].Trim() && temp[21].Trim() == goldenTemp[21].Trim()))

                                                        {
                                                            foundTempInfo = true;
                                                            loggerInfo("找到了GoldenSN 对应的数据");
                                                            loggerInfo($"goldenSN 信息：{ goldenTemp[0]},{ goldenTemp[1]},{goldenTemp[4]},{goldenTemp[5].Trim()},{goldenTemp[6].Trim()},{goldenTemp[21]}");

                                                            var goldenValue = "";
                                                            if (item.ItemName.ToLower().Contains("user1"))
                                                            {
                                                                goldenValue = goldenTemp[66];

                                                            }
                                                            else if (item.ItemName.ToLower().Contains("user2"))
                                                            {
                                                                goldenValue = goldenTemp[70];

                                                            }
                                                            else
                                                            {
                                                                goldenValue = goldenTemp[20];

                                                            }
                                                            var goldenValueDouble = double.Parse(goldenValue);
                                                            var itemValueDouble = double.Parse(item.testValue);
                                                            double sub = goldenValueDouble - itemValueDouble;
                                                            loggerInfo($"goldValue:{goldenValueDouble}, itemValue:{itemValueDouble}, Sub:{sub}");
                                                            double limit = double.Parse(Global.TXPowerLimit);
                                                            if ((sub <= limit) && (sub >= -limit))
                                                            {
                                                                rReturn = true;
                                                            }
                                                            else
                                                            {

                                                                rReturn = false;

                                                                ErrorList = new string[] { "1.4.1.1:Equipment.DUT.Initiate" };
                                                            }

                                                            break;

                                                        }

                                                    }
                                                    if (foundTempInfo == false)
                                                    {
                                                        loggerInfo("未找到GoldenSN 对应的数据，按照比对limit的方式进行");
                                                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                                    }


                                                }
                                                else
                                                {
                                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                                }


                                            }
                                            else
                                            {
                                                rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                            }
                                        }
                                        else
                                        {
                                            rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                        }


                                    }


                                    if (!rReturn)
                                    {
                                        ErrorList = temp[3].Trim().Split(new string[] { "\n" }, 0);

                                        ValidationFail = true;
                                    }

                                    if (item.ItemName.ToLower().Contains("tx_temp"))
                                    {
                                        item.testValue = temp[130];
                                        item.Unit = "C";
                                        rReturn = true;
                                    }

                                    return rReturn;
                                }
                            }
                            loggerError($"Don't find test result in csv,test fail!");
                        }
                        break;





                    case "6G_Calibration":
                    case "5G_Calibration":
                    case "2G_Calibration":
                        {
                            /// MBFT
                            for (int i = 0; i < csvLines.Length; i++)
                            {
                                string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);


                                if (temp[0] == item.TestKeyword.Substring(0, 2) && temp[1] == item.SubStr1 && temp[4] == item.SubStr2 && temp[5] == item.ComdOrParam)
                                {
                                    loggerInfo($"find test result in csv line{i + 1}.testResult={temp[46]}");


                                    rReturn = temp[46].ToLower() == "pass" ? true : false;
                                    if (!rReturn)
                                    {
                                        try
                                        {
                                            ErrorList = temp[3].Trim().Split(new string[] { "\n" }, 0);
                                            CalibrationFail = true;
                                            return rReturn;
                                        }
                                        catch (Exception ex)
                                        {
                                            loggerError("~~~~~~~~~" + ex.ToString());
                                            ErrorList = item.error_code.Trim().Split(new string[] { "\n" }, 0);
                                            CalibrationFail = true;
                                            return rReturn;
                                        }
                                    }


                                    return rReturn;
                                }
                            }
                            loggerError($"Don't find test result in csv,test fail!");
                        }
                        break;












                    //case "6G_RadioValidation":
                    //case "5G_RadioValidation":
                    //case "2G_RadioValidation":
                    //    {
                    //        if (!IsNullOrEmpty(item.CheckStr1))
                    //        {
                    //            //这里是加上powerSpec
                    //            item.ItemName = item.ItemName + "-" + item.CheckStr1;
                    //        }



                    //        /// MBFT
                    //        for (int i = 1; i < csvLines.Length; i++)
                    //        {
                    //            string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);



                    //            //5G ,  TX   , 0/1 ,   5775 ,    HE_SU-MCS13/NA ,  
                    //            if ((temp[0] == item.TestKeyword.Substring(0, 2) && temp[1] == item.SubStr1 && temp[4] == item.SubStr2 && temp[5].Trim() == item.ComdOrParam && temp[6].Trim() == item.ExpectStr)
                    //                && (temp[21].Trim() == item.CheckStr1 || IsNullOrEmpty(item.CheckStr1)))
                    //            {
                    //                loggerInfo($"find test result in csv line{i + 1}.testResult={temp[46]}");
                    //                rReturn = temp[46].ToLower() == "pass" ? true : false;
                    //                if (!rReturn)
                    //                {

                    //                    try
                    //                    {
                    //                        ErrorList = temp[3].Trim().Split(new string[] { "\n" }, 0);
                    //                        ValidationFail = true;
                    //                        return rReturn;
                    //                    }
                    //                    catch (Exception ex)
                    //                    {
                    //                        loggerError("~~~~~~~~~" + ex.ToString());
                    //                        ValidationFail = true;
                    //                        ErrorList = item.error_code.Trim().Split(new string[] { "\n" }, 0);
                    //                        return rReturn;
                    //                    }

                    //                }




                    //                if (item.ItemName.ToLower().Contains("rx_per") || item.ItemName.ToLower().Contains("rx_power"))
                    //                {
                    //                    item.testValue = temp[37];
                    //                    item.Unit = temp[38];
                    //                    item.Limit_min = temp[39];
                    //                    item.Limit_max = temp[40];
                    //                }
                    //                else if (item.ItemName.ToLower().Contains("tx_power"))
                    //                {
                    //                    if (item.ItemName.ToLower().Contains("user1"))
                    //                    {
                    //                        item.testValue = temp[66];
                    //                        item.Unit = temp[67];
                    //                        item.Limit_min = temp[68];
                    //                        item.Limit_max = temp[69];
                    //                    }
                    //                    else if (item.ItemName.ToLower().Contains("user2"))
                    //                    {
                    //                        item.testValue = temp[70];
                    //                        item.Unit = temp[71];
                    //                        item.Limit_min = temp[72];
                    //                        item.Limit_max = temp[73];
                    //                    }
                    //                    else
                    //                    {
                    //                        item.testValue = temp[20];
                    //                        item.Unit = temp[22];
                    //                        item.Limit_min = temp[23];
                    //                        item.Limit_max = temp[24];
                    //                    }
                    //                }
                    //                else if (item.ItemName.ToLower().Contains("tx_evm"))
                    //                {
                    //                    if (item.ItemName.ToLower().Contains("user1"))
                    //                    {
                    //                        item.testValue = temp[98];
                    //                        item.Unit = temp[99];
                    //                        item.Limit_min = temp[100];
                    //                        item.Limit_max = temp[101];
                    //                    }
                    //                    else if (item.ItemName.ToLower().Contains("user2"))
                    //                    {
                    //                        item.testValue = temp[102];
                    //                        item.Unit = temp[103];
                    //                        item.Limit_min = temp[104];
                    //                        item.Limit_max = temp[105];
                    //                    }
                    //                    else
                    //                    {
                    //                        item.testValue = temp[8];
                    //                        item.Unit = temp[9];
                    //                        item.Limit_min = temp[10];
                    //                        item.Limit_max = temp[11];
                    //                    }
                    //                }
                    //                //else if (item.ItemName.ToLower().Contains("rx_power"))//这是sweep版本的时候取值，非sweep版不用
                    //                //{


                    //                //    item.testValue = temp[42];
                    //                //    item.Unit = temp[43];
                    //                //    item.Limit_min = temp[44];
                    //                //    item.Limit_max = temp[45];

                    //                //}
                    //                else if (item.ItemName.ToLower().Contains("tx_loleak"))
                    //                {


                    //                    item.testValue = temp[25];
                    //                    item.Unit = temp[26];
                    //                    item.Limit_min = temp[27];
                    //                    item.Limit_max = temp[28];

                    //                }



                    //                else if (item.ItemName.ToLower().Contains("tx_freqerr"))
                    //                {


                    //                    item.testValue = temp[12];
                    //                    item.Unit = temp[13];
                    //                    item.Limit_min = temp[14];
                    //                    item.Limit_max = temp[15];

                    //                }
                    //                else if (item.ItemName.ToLower().Contains("tx_sysclkerr"))
                    //                {

                    //                    item.testValue = temp[16];
                    //                    item.Unit = temp[17];
                    //                    item.Limit_min = temp[18];
                    //                    item.Limit_max = temp[19];

                    //                }
                    //                else if (item.ItemName.ToLower().Contains("tx_specmsk"))
                    //                {


                    //                    item.testValue = temp[33];
                    //                    item.Unit = temp[34];
                    //                    item.Limit_min = temp[35];
                    //                    item.Limit_max = temp[36];

                    //                }
                    //                else if (item.ItemName.ToLower().Contains("tx_specflat"))
                    //                {


                    //                    item.testValue = temp[29];
                    //                    item.Unit = temp[30];
                    //                    item.Limit_min = temp[31];
                    //                    item.Limit_max = temp[32];

                    //                }



                    //                else if (item.ItemName.ToLower().Contains("tx_radio_temp"))
                    //                {


                    //                    item.testValue = temp[130];
                    //                    item.Unit = "C";

                    //                }











                    //                if (string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                    //                {
                    //                    rReturn = temp[46].ToLower() == "pass" ? true : false;
                    //                }

                    //                else
                    //                {

                    //                    //判断当前是否是GoldenSN
                    //                    if (Global.GoldenSN.Contains(SN) && Global.STATIONNAME == "MBFT")
                    //                    {
                    //                        if (item.ItemName.ToLower().Contains("tx_power")) //只处理tx_power,和台湾数据作对比
                    //                        {

                    //                            if (csvGoldenLines != null && csvGoldenLines.Length > 0)
                    //                            {
                    //                                bool foundTempInfo = false;
                    //                                for (int i1 = 1; i1 < csvGoldenLines.Length; i1++)
                    //                                {
                    //                                    string[] goldenTemp = csvGoldenLines[i1].Split(new char[] { ',' }, StringSplitOptions.None);



                    //                                    if ((temp[0] == goldenTemp[0] && temp[1] == goldenTemp[1] && temp[4] == goldenTemp[4] && temp[5].Trim() == goldenTemp[5].Trim() && temp[6].Trim() == goldenTemp[6].Trim() && temp[21].Trim() == goldenTemp[21].Trim()))

                    //                                    {
                    //                                        foundTempInfo = true;
                    //                                        loggerInfo("找到了GoldenSN 对应的数据");
                    //                                        loggerInfo($"goldenSN 信息：{ goldenTemp[0]},{ goldenTemp[1]},{goldenTemp[4]},{goldenTemp[5].Trim()},{goldenTemp[6].Trim()},{goldenTemp[21]}");

                    //                                        var goldenValue = "";
                    //                                        if (item.ItemName.ToLower().Contains("user1"))
                    //                                        {
                    //                                            goldenValue = goldenTemp[66];

                    //                                        }
                    //                                        else if (item.ItemName.ToLower().Contains("user2"))
                    //                                        {
                    //                                            goldenValue = goldenTemp[70];

                    //                                        }
                    //                                        else
                    //                                        {
                    //                                            goldenValue = goldenTemp[20];

                    //                                        }
                    //                                        var goldenValueDouble = double.Parse(goldenValue);
                    //                                        var itemValueDouble = double.Parse(item.testValue);
                    //                                        double sub = goldenValueDouble - itemValueDouble;
                    //                                        loggerInfo($"goldValue:{goldenValueDouble}, itemValue:{itemValueDouble}, Sub:{sub}");
                    //                                        double limit = double.Parse(Global.TXPowerLimit);
                    //                                        if ((sub <= limit) && (sub >= -limit))
                    //                                        {
                    //                                            rReturn = true;
                    //                                        }
                    //                                        else
                    //                                        {

                    //                                            rReturn = false;

                    //                                            ErrorList = new string[] { "1.4.1.1:Equipment.DUT.Initiate" };
                    //                                        }

                    //                                        break;

                    //                                    }

                    //                                }
                    //                                if (foundTempInfo == false)
                    //                                {
                    //                                    loggerInfo("未找到GoldenSN 对应的数据，按照比对limit的方式进行");
                    //                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                    //                                }


                    //                            }
                    //                            else
                    //                            {
                    //                                rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                    //                            }


                    //                        }
                    //                        else
                    //                        {
                    //                            rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                    //                    }


                    //                }


                    //                if (!rReturn)
                    //                {
                    //                    ErrorList = temp[3].Trim().Split(new string[] { "\n" }, 0);

                    //                    ValidationFail = true;
                    //                }

                    //                if (item.ItemName.ToLower().Contains("tx_temp"))
                    //                {
                    //                    item.testValue = temp[130];
                    //                    item.Unit = "C";
                    //                    rReturn = true;
                    //                }

                    //                return rReturn;
                    //            }
                    //        }
                    //        loggerError($"Don't find test result in csv,test fail!");
                    //    }
                    //    break;





                

                    case "GetIQXInfo":
                        {
                            for (int i = 1; i < csvLines.Length; i++)
                            {
                                var temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);
                                if (temp.Length == 2 && string.Equals(temp[0], item.ComdOrParam, StringComparison.OrdinalIgnoreCase))
                                    item.testValue = temp[1];
                            }
                            rReturn = true;
                        }
                        break;

                    case "RunDosCmdParallel":
                        {
                            dosCmd.SendCommand3(item.ComdOrParam);
                            return rReturn = true;
                        }

                    case "RunDosCmdThread":
                        {
                            KillProcess("cmd");
                            KillProcess("userspace_speedtest");

                            RunDosCmd("taskkill /IM userspace_speedtest.exe /F");

                            thread = new Thread(() => dosCmd.SendCommand3(item.ComdOrParam));
                            thread.IsBackground = true;
                            if (thread.IsAlive)
                            {
                                thread.Abort();
                            }
                            thread.Start();
                            return rReturn = true;
                        }

                    case "ETH_SPEED_TX":
                        {
                            string revStr = "";
                            try
                            {
                                string logFile = $@"{Environment.CurrentDirectory}\{item.ComdOrParam}";
                                using (var read = new StreamReader(logFile))
                                {
                                    revStr = read.ReadToEnd();
                                    loggerDebug("logPath：" + logFile);
                                    loggerDebug(revStr);
                                }

                                if (revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                                {
                                    item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);
                                    item.testValue = (Math.Round(double.Parse(item.testValue) / 1000000)).ToString(); //取整
                                }
                                else
                                    return rReturn = false;
                            }
                            catch (Exception ex)
                            {
                                loggerFatal(ex.Message);
                                //客户嫌等待5秒太长，改为等待2秒
                                //Sleep(item.TimeOut);
                                //Sleep(2);
                                Sleep(item.TimeOut);
                                return rReturn = false;
                            }
                            // 需要比较Limit
                            if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max))
                                rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                            break;
                        }

                    case "IperfWiFiSpeedTest":
                        {
                            string revStr = "";
                            inPutValue = "";
                            if (dosCmd.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                string[] lines = revStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var line in lines)
                                {
                                    if (line.Contains(item.CheckStr1) && line.Contains(item.CheckStr2))
                                    {
                                        item.testValue = GetValue(line, item.SubStr1, item.SubStr2);

                                        if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("G"))
                                        {
                                            string tempValue = item.testValue.Replace("G", "").Trim();
                                            item.testValue = ((double.Parse(tempValue)) * 1000).ToString();
                                        }
                                        else if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("M"))
                                            item.testValue = (double.Parse(item.testValue.Replace("M", "").Trim())).ToString();
                                        else
                                            loggerDebug("Get Speed error!");

                                        rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case "RedirectedTextCheck":  //IperfWiFiSpeedTest重定向文本结果检查
                        {
                            string revStr = "";
                            try
                            {
                                using (var read = new StreamReader($@"{System.Environment.CurrentDirectory}\{item.ComdOrParam}"))
                                {
                                    revStr = read.ReadToEnd();
                                    loggerDebug("RedirectedTextCheck Received:" + revStr);
                                }
                            }
                            catch (Exception ex)
                            {
                                loggerFatal(ex.Message);
                                
                                Sleep(item.TimeOut);
                                
                               
                                return rReturn = false;
                            }
                             
                            string[] lines = revStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                if (line.Contains(item.CheckStr1) && line.Contains(item.CheckStr2))
                                {
                                    item.testValue = GetValue(line, item.SubStr1, item.SubStr2);

                                    if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("G"))
                                    {
                                        string tempValue = item.testValue.Replace("G", "").Trim();
                                        item.testValue = ((double.Parse(tempValue)) * 1000).ToString();
                                    }
                                    else if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("M"))
                                        item.testValue = (double.Parse(item.testValue.Replace("M", "").Trim())).ToString();
                                    else
                                        loggerDebug("Get Speed error!");

                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                    break;
                                }
                            }
                            //RTT 的6.1.0.2.1: OTA.Throughput.Radio1.5GHz.Tx 和 6.1.0.2.1: OTA.Throughput.Radio0.2GHz.Tx 特殊处理
                          
                            //if ( Global.STATIONNAME=="RTT" && !rReturn && retry == 2) {
                            //    //删除log 并 重新启动iperf3
                            //    try
                            //    {
                            //        loggerInfo("stationName=" + "RTT" + ", retry=2, restart iperf3");
                            //        loggerDebug("start delete file " + $@"{System.Environment.CurrentDirectory}\{item.ComdOrParam}");
                            //        File.Delete($@"{System.Environment.CurrentDirectory}\{item.ComdOrParam}");
                            //        loggerDebug("restart iperf3");
                            //        if (item.ComdOrParam == "WIFI2G_THROUGHPUT_parallel_Tx.txt")
                            //        {
                            //            dosCmd.SendCommand3("start /B iperf3 -c 192.168.1.2 -i1 -O2 -P10 -t10 -B 192.168.1.12 > WIFI2G_THROUGHPUT_parallel_Tx.txt  & exit");
                            //        }
                            //        else if (item.ComdOrParam == "WIFI5G_THROUGHPUT_parallel_Tx1.txt") {
                            //            dosCmd.SendCommand3("start /B iperf3 -c 192.168.1.50 -i1 -O2 -P30 -t10 -B 192.168.1.15 > WIFI5G_THROUGHPUT_parallel_Tx1.txt  & exit");
                            //        }
                                    
                            //    }
                            //    catch(Exception ex) {
                            //        loggerDebug("Delete file action error:" + ex.Message.ToString());
                            //        retry = 0;//直接retry=0，不需要下次retry了
                            //    }
                                
                            //}
                        }
                        break;

                    case "RedirectedTextCheck5G":  //IperfWiFiSpeedTest重定向文本结果检查
                        {
                            string[] files = item.ComdOrParam.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            double sum = 0;
                            foreach (var file in files)
                            {
                                string revStr = "";
                                try
                                {
                                    using (var read = new StreamReader($@"{System.Environment.CurrentDirectory}\{file}"))
                                    {
                                        revStr = read.ReadToEnd();
                                        loggerDebug(revStr);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    loggerFatal(ex.Message);
                                    Sleep(item.TimeOut);
                                    return rReturn = false;
                                }

                                string[] lines = revStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var line in lines)
                                {
                                    if (line.Contains(item.CheckStr1) && line.Contains(item.CheckStr2))
                                    {
                                        item.testValue = GetValue(line, item.SubStr1, item.SubStr2);

                                        if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("G"))
                                        {
                                            string tempValue = item.testValue.Replace("G", "").Trim();
                                            sum += (double.Parse(tempValue)) * 1000;
                                        }
                                        else if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("M"))
                                            sum += (double.Parse(item.testValue.Replace("M", "").Trim()));
                                        else
                                            loggerDebug("Get Speed error!");
                                    }
                                }
                            }
                            loggerDebug($"Get 5G total:{sum}M!");
                            item.testValue = sum.ToString();
                            rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                        }
                        break;

                    case "IperfThroughput":
                        if (inPutValue == "")
                        {
                            var revStr = "";
                            if (dosCmd.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                rReturn = true;
                                inPutValue = revStr;
                            }
                            else
                            {
                                inPutValue = "";
                                return rReturn = false;
                            }
                        }
                        {
                            var revStr = inPutValue;
                            string[] lines = revStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                if (line.Contains(item.CheckStr1) && line.Contains(item.CheckStr2))
                                {
                                    item.testValue = GetValue(line, item.SubStr1, item.SubStr2);

                                    if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("G"))
                                    {
                                        string tempValue = item.testValue.Replace("G", "").Trim();
                                        item.testValue = ((double.Parse(tempValue)) * 1000).ToString();
                                    }
                                    else if (!string.IsNullOrEmpty(item.testValue) && item.testValue.EndsWith("M"))
                                        item.testValue = (double.Parse(item.testValue.Replace("M", "").Trim())).ToString();
                                    else
                                        loggerDebug("Get Speed error!");

                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                    break;
                                }
                            }

                            //清空一下上次的结果，因为下次要重新执行cmd了


                            if (Global.STATIONNAME == "MBLT" && (item.ItemName == "ETH1_SPEED_TX" || item.ItemName == "ETH1_SPEED_RX")) {

                                inPutValue = "";


                            }

                        }
                        break;


                     




                    case "TempSensorTest_AfterWiFiTXPowerTest":
                        for (int i = 0; i < csvLines.Length; i++)
                        {
                            string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);
                            if (temp[0].Contains(item.TestKeyword.Substring(item.TestKeyword.IndexOf("_"))))
                            {
                                item.testValue = Math.Round(double.Parse(temp[1])).ToString();
                                
                                
                                
                                //item.TestValue = temp[1];
                                loggerDebug($"Get tempSensorTest:{temp[0]},{temp[1]}");
                                // 需要比较Limit
                                if (!String.IsNullOrEmpty(item.Limit_min) || !String.IsNullOrEmpty(item.Limit_max))
                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                            }
                        }
                        break;

                    case "TempSensorTest_AfterDesenseWiFiTest":
                        for (int i = 0; i < csvLines.Length; i++)
                        {
                            string[] temp = csvLines[i].Split(new char[] { ',' }, StringSplitOptions.None);
                            if (temp[0].Contains("AfterDesense"))
                            {
                                item.testValue = Math.Round(double.Parse(temp[1])).ToString();
                                //item.TestValue = temp[1];
                                loggerDebug($"Get tempSensorTest:{temp[0]},{temp[1]}");
                                // 需要比较Limit
                                if (!String.IsNullOrEmpty(item.Limit_min) || !String.IsNullOrEmpty(item.Limit_max))
                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                            }
                        }
                        break;
                    case "SampleTelnetCmd":
                        {
                            string revStr = "";
                            if (SampleComm.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                rReturn = true;
                            }
                        }
                        break;

                    case "GetMD52G":
                    case "GetMD55G":
                    case "RunDosCmd":
                        {
                            var revStr = "";
                            if (dosCmd.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, short.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2))
                            {
                                rReturn = true;
                                // 需要提取测试值
                                if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                                    item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);
                                else
                                    return rReturn = true;
                                // 需要比较Spec
                                if (!String.IsNullOrEmpty(item.Spec))
                                    rReturn = CheckSpec(item.Spec, item.testValue);
                                // 需要比较Limit
                                if (!String.IsNullOrEmpty(item.Limit_min) || !String.IsNullOrEmpty(item.Limit_max))
                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                            }
                            if (item.TestKeyword == "GetMD52G")
                            {
                                MD52G = item.testValue;
                            }
                            else if (item.TestKeyword == "GetMD55G")
                            {
                                MD55G = item.testValue;
                            }
                        }
                        break;
                    case "DisableNetInterface": {
                            string name = item.ComdOrParam;
                            NetshInterfaceDisEnable(name);
                            rReturn = true;
                        }
                        break;
                    case "EnableNetInterface":
                        {
                            string name = item.ComdOrParam;
                            NetshInterfaceEnable(name);
                            rReturn = true;
                        }
                        break;

                    case "DoPushPopRetry": {
                            //关闭Com7
                            //delay 10ms
                            //打开Com7
                            //delay 10ms
                            //关闭com1
                            //打开com1

                            DUTCOMM.Close();
                            Thread.Sleep(10);
                            DUTCOMM.Open();

                            FixSerialPort.Close();
                            Thread.Sleep(10);
                            FixSerialPort.OpenCOM();

                            string recvStr = "";
                            FixSerialPort.SendCommandToFix("AT+PORTEJECT%", ref recvStr, "OK", 10);

                            Sleep(500);
 
                            FixSerialPort.SendCommandToFix("AT+PORTINSERT%", ref recvStr, "OK", 10);
                           
                            Sleep(500);

                            //if (item.ComdOrParam != "1") { //需要等待开机重启
                            //    string revStr = "";

                            //    if (DUTCOMM.SendCommand("", ref revStr, "luxshare SW Version :", 120)){
                            //        rReturn = true;
                            //    }
                            //}
                            //else {
                            //    rReturn = true;
                            //}


                            rReturn = true;



                        }
                        break;
                    case "RTT_5G_CloseAndOpen": {
                          
                        }
                        break;

                    case "test1":
                        {
                            rReturn = true;
                        }
                        break;

                    case "IdentifyWPS":
                        {
                            try
                            {
                                if (!Power_OnOff_WPS(int.Parse(Global.WPSPortNum), false))
                                    Global.WPS = "0";
                            }
                            catch (Exception)
                            {
                                Global.WPS = "0";
                            }
                            loggerInfo($"Identify WPS type is:{(Global.WPS == "0" ? "old" : "new")}");
                            rReturn = true;
                        }
                        break;

                    case "PowerONTest":
                    default:
                        {

                        
                            if (DUTCOMM == null) {

                                loggerError("DUTCOM 为空");
                               return rReturn = false;
                            }

                            //loggerWarn($"Warning!!!,this is default DUT test-method, ErrorList.Length is {ErrorList.Length.ToString()}");
                            var revStr = "";
                            inPutValue = "";
                            
                            if ((Global.TESTMODE == "debug" || Global.TESTMODE == "fa" || IsDebug) && Global.STATIONNAME == "SRF" && item.TestKeyword.Contains("SetIpaddrEnv"))
                            {
                                rReturn = true;
                            }

                            else if ((Global.TESTMODE == "debug" || Global.TESTMODE == "fa" || IsDebug)  && item.TestKeyword.Contains("SetDHCP"))
                            {
                                rReturn = true;
                            }

                            else if ((Global.TESTMODE == "debug" || Global.TESTMODE == "fa" || IsDebug) && item.EeroName.Trim()== "CHECK_ART_PARTITION")
                            {
                                rReturn = true;
                                loggerDebug("Debug Mode CHECK_ART_PARTITION SKip ,do not run");
                            }


                            else if (DUTCOMM.SendCommand(item.ComdOrParam, ref revStr, item.ExpectStr, double.Parse(item.TimeOut))
                                && revStr.CheckStr(item.CheckStr1) && revStr.CheckStr(item.CheckStr2)
                                
                                
                                
                                
                                )
                            {

                                var itemNoContain = item.NoContain;
                                if (item.ItemName == "CheckCalData1" || item.ItemName == "CheckCalData2") {

                                  //  loggerDebug("去掉各种空格换行");

                                     revStr = revStr.Replace(" ", "");
                                     revStr = revStr.Replace("\r", "");
                                     revStr = revStr.Replace("\n", "");
                                     revStr = revStr.Replace("\t", "");

                                    loggerInfo("revStr=" + revStr);

                                    itemNoContain = itemNoContain.Replace(" ", "");
                                    itemNoContain = itemNoContain.Replace("\r", "");
                                    itemNoContain = itemNoContain.Replace("\n", "");
                                    itemNoContain = itemNoContain.Replace("\t", "");

                                    loggerInfo("itemNoContain=" + itemNoContain);

                                }
                                
                                if (revStr.NoCheckStr(itemNoContain) == false) {

                                    loggerError($"{revStr} contains {item.NoContain}, fail");
                                      return rReturn = false;
                                }




                                


                                rReturn = true;
                             
                                // 需要提取测试值
                                if (!string.IsNullOrEmpty(item.SubStr1) || !string.IsNullOrEmpty(item.SubStr2))
                                {
                                    item.testValue = GetValue(revStr, item.SubStr1, item.SubStr2);

                                    if (item.testValue == "" || item.testValue==null) { 
                                        rReturn = false;

                                      
                                    }

                                    if (item.ItemName == "Read_MAC_DHCP") {

                                        loggerDebug("读取到板子mac地址:" + item.testValue);

                                        MesMac = item.testValue;
                                    }
                                   

                                        
                                }
                                else
                                {
                                    // return rReturn = true;
                                    rReturn = true;
                                    
                                }

                                if (item.ItemName == "LED_MODEL")
                                {

                                    if (item.testValue.Contains("cat: can't open")) {

                                        item.testValue = "";
                                        return rReturn = false;
                                    }
                                
                                
                                }



                                if (item.TestKeyword.Contains("TempSensorTest_After") || item.TestKeyword.Contains("TempSensorTest_Before"))
                                {
                                    if (string.IsNullOrEmpty(item.testValue))
                                    {
                                        item.testValue = "0";
                                        return rReturn = false;
                                    }
                                    else {
                                        //item.testValue = Math.Round(double.Parse(item.testValue)).ToString(); //取整
                                        item.testValue = Math.Round(double.Parse(item.testValue)/1000).ToString();
                                    }
                                        
                                }

                                if (item.TestKeyword.Contains("ETH_SPEED_RX"))
                                {
                                    item.testValue = (Math.Round(double.Parse(item.testValue) / 1000000)).ToString(); //取整
                                }

                                if (item.TestKeyword.Contains("LED_CURRENT"))
                                {
                                    loggerInfo("step .................... testValue=" + item.testValue);
                                    item.testValue = (Math.Round(double.Parse(item.testValue) / 10)).ToString(); //取整

                                    loggerInfo("step .................... testValue="+item.testValue);
                                }


                                if (item.TestKeyword == "TEST_IMAGE_VERSION")
                                {
                                    ImageVer = item.Spec;
                                    logger.Info("<<<<<<<<<<读取到Spec:" + ImageVer);
                                }
                              
                                // 需要比较Spec
                                if (!string.IsNullOrEmpty(item.Spec) && string.IsNullOrEmpty(item.Limit_min) && string.IsNullOrEmpty(item.Limit_max))
                                {
                                    loggerInfo("比较spec开始: Spec=" + item.Spec + " Value=" + item.testValue);

                                    rReturn = CheckSpec(item.Spec, item.testValue);
                                    loggerDebug(">>>>>>>>:" + rReturn);


                                    //特殊处理MBLT MMC_MODEL，因为有时候会返回一些干扰信息，rd认为是对的
                                    if (!rReturn && Global.STATIONNAME == "MBLT" && item.ItemName == "MMC_MODEL") {
                                        // 8GTF4R | DG4008 | S40004
                                        var arr = item.Spec.Split('|');
                                        foreach (var str in arr) {
                                            if (item.testValue.Contains(str.Trim())) { 
                                                rReturn = true;
                                                break;
                                            }
                                        }

                                    }


                                }

                               
                                loggerDebug("min:"+ item.Limit_min);
                                loggerDebug("max:"+ item.Limit_max);
                                loggerDebug("Spec:" + item.Spec);
                                loggerDebug("Value:" + item.testValue);

                                // 需要比较Limit
                                if (!string.IsNullOrEmpty(item.Limit_min) || !string.IsNullOrEmpty(item.Limit_max)) {
                                    
                                    rReturn = CompareLimit(item.Limit_min, item.Limit_max, item.testValue, out info);
                                    loggerDebug("返回结果:" + (rReturn ? "True" : "False"));
                                }
                               

                                if (!rReturn)
                                {
                                    HandleSpecialMethed(item, rReturn, revStr);
                                }


                            }
                            else {  //


                               


                                //****************************P1阶段先屏蔽掉**********************************//


                                //if (Global.STATIONNAME == "SFT" || Global.STATIONNAME == "MBFT"
                                //     || Global.STATIONNAME == "RTT" || Global.STATIONNAME == "SRF"){


                                //    if (DUTCOMM != null && DUTCOMM.GetType() == typeof(Telnet))
                                //    {
                                //       loggerInfo(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                                //       loggerError("connet again");
                                //       telnetInfo = new TelnetInfo { _Address = DUTCOMM.HostIP };
                                //       DUTCOMM.Close();
                                //       DUTCOMM = new Telnet(telnetInfo);
                                //       DUTCOMM.Open(Global.PROMPT);
                                //    }


                                //}

                                HandleSpecialMethed(item, rReturn, revStr);



                                loggerInfo("最终测试结果:" + rReturn);

                            }


                            if (item.TestKeyword == "QorvoBLEPeripheral")
                                BtDevAddress = item.testValue.Trim();


                            if (item.TestKeyword == "PowerONTest") {
                                if (rReturn == true)
                                {
                                    loggerInfo(">>>>>>>>>>PowerON OK");

                                }
                                else {
                                    loggerInfo(">>>>>>>>>>PowerON Fail");
                                }
                               
                            }

                        }
                        break;
                }
            }
            catch (ThreadAbortException ex)
            {
                //abort线程忽略报错
                loggerWarn(ex.Message);
                rReturn = false;
                HandleSpecialMethed(item, rReturn, "");
                return rReturn;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                rReturn = false;
                HandleSpecialMethed(item, rReturn, "");
                return rReturn;
            }
            finally
            {
                // finally请不要给rReturn赋值,不生效！！，return true/false会提前返回，不执行最后的return rReturn;。

                // 设置错误码
                if ((retryTimes == 0 && !rReturn) && (IsNullOrEmpty(error_code) && IsNullOrEmpty(error_details)))
                {
                   
                    loggerDebug($"ErrorList.length {ErrorList.Length}.");
                    if (ErrorList.Length == 1) {
                        loggerDebug("ErrorList = " + ErrorList[0]);
                    }
                    if (ErrorList.Length > 1 && info == "TooHigh") // TooHigh
                    {
                        error_code = ErrorList[1].Split(':')[0].Trim();
                        error_details = ErrorList[1].Split(':')[1].Trim();
                    }
                    else if (ErrorList.Length == 0)
                    {
                        error_code = "0.0.0.0";
                        error_details = item.ItemName;
                    }
                    else //added by jonathan
                    {
                        if (ErrorList[0].Split(':').Length > 0) {
                            error_code = ErrorList[0].Split(':')[0].Trim();
                        }
                        if (ErrorList[0].Split(':').Length > 1)
                        {
                            error_details = ErrorList[0].Split(':')[1].Trim();
                        }

                        if (error_code == "" || error_details == "") {
                            loggerError("errorcode 异常:," + "error_code=" + error_code + ", errordetails=" + error_details);
                        }

                        
                    }

                    //防止rf测试项 errocode 不对的问题
                    if ((error_details.Trim() == "Equipment.DUT.Initiate" && error_code.Trim() == "1.4.1.1") || error_code.Trim() == "" || error_details.Trim() == "")
                    {

                        loggerInfo("errocde=1.4.1.1或者空,特殊处理错误类型");

                        error_code = item.EeroName;
                        error_details = item.EeroName;
                    }




                    testPhase.phase_details = error_details;
                    testPhase.error_code = error_code;
                }

                if (retryTimes == 0 || rReturn)
                {
                    item.ElapsedTime = $"{Convert.ToDouble((DateTime.Now - item.start_time).TotalSeconds),0:F1}";
                    if (item.TestKeyword != "Wait" && item.TestKeyword != "ThreadSleep")
                    {
                        if (rReturn)
                            loggerInfo($"{item.ItemName} {(rReturn ? "PASS" : "FAIL")}!! ElapsedTime:{item.ElapsedTime},{error_code}:{error_details},Spec:{item.Spec},Min:{item.Limit_min},Value:{item.testValue},Max:{item.Limit_max}");
                        else
                            loggerError($"{item.ItemName} {(rReturn ? "PASS" : "FAIL")}!! ElapsedTime:{item.ElapsedTime},{error_code}:{error_details},Spec:{item.Spec},Min:{item.Limit_min},Value:{item.testValue},Max:{item.Limit_max}");

                        if (item.ItemName == "FIXTURE_CLOSE") {
                            MainForm.f1.FIXTURE_TIME = item.ElapsedTime;
                        }
                        if (item.ItemName == "DUT_PING") {
                            MainForm.f1.DUT_PING_TIME = item.ElapsedTime;
                        }
                        
                        MainForm.f1.UpdateDetailView(SN, item.ItemName, item.Spec, item.Limit_min, item.testValue, item.Limit_max, item.ElapsedTime, item.start_time.ToString(), rReturn.ToString() == "True" ? "Pass" : "Fail");
                        
                    
                    
                    }

                    // 给Json格式对象赋值
                    if ((item.Json != null && item.Json.ToLower() == "y"))
                    {
                        if ((item.IfElse.ToLower() == "if" || item.IfElse.ToLower() == "&if") && !rReturn) // if and test result=false,do not save to json.
                        {
                        }
                        else if ((item.IfElse.ToLower() == "elseif" ) && !rReturn) // if and test result=false,do not save to json.
                        {
                        }
                        else
                        {
                            JsonAndCsv(item, phaseItem, specFlag, rReturn);
                        }
                           
                    }
                    else if (!rReturn || item.ByPassFail.ToLower() == "f" || item.ByPassFail.ToLower() == "0")
                    {
                        if (!rReturn && item.ByPassFail.ToLower() == "p" && Global.STATIONNAME == "MBFT")
                        {

                        }
                        //先临时这么处理,针对 MBLT的elseif测试项
                        else if (!rReturn && (item.Json == "" || item.Json == null) && item.IfElse == "elseif") { 
                        
                        }
                        // else if (!rReturn && (item.Json == "" || item.Json == null) && item.IfElse == "if" && item.ItemName== "WaitingToStart_Start")
                        else if (!rReturn && (item.Json == "" || item.Json == null) && item.IfElse == "if")
                        {

                        }
                        else
                        {
                            JsonAndCsv(item, phaseItem, specFlag, rReturn);
                        }
                    }

                    // 用反射的方法给mesPhases变量赋值
                    if (IsNullOrEmpty(item.SubStr1) && IsNullOrEmpty(item.SubStr2) && !IsNullOrEmpty(item.MES_var))
                    {
                        //没有测试值则赋值测试结果给变量
                        SetVerReflection(mesPhases, item.MES_var, rReturn.ToString().ToUpper());
                    }

                    else {
                        SetVerReflection(mesPhases, item.MES_var, item.testValue);
                    }
                       

                    if (item.TestKeyword == "SetIpaddrEnv" && rReturn)
                    {
                        SetIPflag = true;
                        testPhase.phase_details = DUTMesIP;
                    }
                }

            }
            return rReturn;
        }








        #region  WPS 上下电

        public bool PowerCycleOutletWPS(int index)
        {
            if (Global.WPS == "1")
            {
                if (Power_OnOff_WPS(index, false))
                {
                    Thread.Sleep(200);
                    Power_OnOff_WPS(index, true);
                }
                else
                    return false;
                Thread.Sleep(1000);
                if (PDUSnmp.GetStatus(Global.WebPsIp, index) == 1)
                    return true;
                else
                {
                    Power_OnOff_WPS(index, true);
                    Thread.Sleep(1000);
                    if (PDUSnmp.GetStatus(Global.WebPsIp, index) == 1)
                        return true;
                    else
                    {
                        Power_OnOff_WPS(index, true);
                        Thread.Sleep(1000);
                        if (PDUSnmp.GetStatus(Global.WebPsIp, index) == 1)
                            return true;
                        else
                        {
                            loggerInfo($"PowerCycleOutlet fail in GetStatus !");
                            return false;
                        }
                    }
                }
            }
            else
            {
                bool rReturn;
                if (GetStatusWPS(index).GetAwaiter().GetResult())
                {
                    rReturn = C.CycleOutlet(index).GetAwaiter().GetResult();
                    Thread.Sleep(1000);
                }
                else
                {
                    bool rReturnoff = C.SetOutlet(index, false).GetAwaiter().GetResult();
                    Thread.Sleep(100);
                    bool rReturnon = C.SetOutlet(index, true).GetAwaiter().GetResult();
                    Thread.Sleep(1000);
                    rReturn = rReturnoff && rReturnon;
                }
                return rReturn;
            }
        }

        public async Task<bool> GetStatusWPS(int index)
        {
            try
            {
                var switchInfo = await C.GetSwitchInfo();
                if (switchInfo == null || switchInfo.Outlets.Length == 0)
                {
                    loggerInfo($"Get Outlet {index} Status FAIL! ");
                    return false;
                }
                loggerInfo($"Get Outlet {index} Status: " + switchInfo.Outlets[index - 1].ToDetailsString());
                return switchInfo.Outlets[index - 1].IsOn;
            }
            catch (Exception ex)
            {
                loggerInfo($"Get Outlet {index} Status Exception! {ex.Message}");
                return false;
            }
        }
        private   bool Power_OnOff_WPS(int index, bool desiredState)
        {



            if (Global.WPS == "1")
            {
                for (int i = 0; i < 3; i++)
                {
                    if (PDUSnmp.TurnOnOff(Global.WPSDUTIP, index, desiredState) == 0)
                    {
                        loggerInfo($"Power {(desiredState ? "On " : "Off")} {index}-: success-{i}!");
                        return true;
                    }
                    Thread.Sleep(200);
                }
                loggerInfo($"Power {(desiredState ? "On " : "Off")} {index}-: fail!");
                return false;
            }
            else
            {
                return C.SetOutlet(index, desiredState).GetAwaiter().GetResult();
            }
        }

        #endregion












        private static void HandleSpecialMethed(Items item, bool rReturn, string revStr)
        {

            //loggerInfo("屏蔽特殊处理!!!");
            return;


            if (!rReturn && Global.STATIONNAME == "MBLT" && 
                
                (
                   (item.ItemName == "USBMount" && item.EeroName == "USB_MOUNT")
                   ||
                   (item.ItemName == "USB_DETECT" && item.EeroName == "USB_DETECT")
                )
                
                )
            {
                //loggerInfo(">>>>>>>>>>>> do it again");
                //MessageBox.Show("重新拔插U盘，完成后点击确定/Đặt lại đĩa flash USB, và nhắp vào OK khi xong\r\n");
                USBConfirmDialog usbDialog = new USBConfirmDialog();
                usbDialog.TextHandler = (str) => { };
                usbDialog.StartPosition = FormStartPosition.CenterScreen;
                usbDialog.TopMost = true;
                usbDialog.ShowDialog();
                Thread.Sleep(3000);
            }


            
            loggerInfo("~~~~~~~~~~~~~~~~~~~~~~~~~~~StationName:" + Global.STATIONNAME + ",itemName:" + item.ItemName + ",eeroName:" + item.EeroName);

            if (

                !rReturn &&
                (
                (Global.STATIONNAME == "SFT" && (item.ItemName == "WaitingToStart") && item.EeroName == "ENTER_KERNEL")
                ||
                (Global.STATIONNAME == "RTT" && (item.ItemName == "DUT_PING") && item.EeroName == "DUT_PING")
                ||
                 (Global.STATIONNAME == "SFT" && (item.ItemName == "DUT_PING") && item.EeroName == "DUT_PING")
                 ||
                (Global.STATIONNAME == "SRF" && (item.ItemName == "DUT_PING") && item.EeroName == "DUT_PING")
                 ||
                (Global.STATIONNAME == "MBFT" && (item.ItemName == "DUT_PING") && item.EeroName == "DUT_PING")
                 ||
                (Global.STATIONNAME == "SFT" && (item.ItemName == "I_VDD_5V_OPEN") && item.EeroName == "I_VDD_5V_OPEN")
                 ||
                (Global.STATIONNAME == "SFT" && (item.ItemName == "ETH0_DATA_RATE"))
                ||
                (Global.STATIONNAME == "SFT" && (item.ItemName == "ETH1_DATA_RATE"))
                ||
                (Global.STATIONNAME == "MBLT" && (item.ItemName == "ETH0_DATA_RATE"))
                ||
                (Global.STATIONNAME == "MBLT" && (item.ItemName == "ETH1_DATA_RATE"))
                ||
                 (Global.STATIONNAME == "SFT" && (item.ItemName == "ETH0_PING"))
                 ||
                 (Global.STATIONNAME == "SFT" && (item.ItemName == "ETH1_PING"))
            


                )
                )
            {

                if (Global.FIXTUREFLAG == "1")
                {

                    FixSerialPort.OpenCOM();
                    var recvStr = "";


                  
                        if ((Global.STATIONNAME == "SRF" && (item.ItemName == "DUT_PING") && item.EeroName == "DUT_PING"))
                        {
                           

                            FixSerialPort.SendCommandToFix("AT+MOT_OUT%", ref recvStr, "OK", 10);

                            Sleep(500);

                            FixSerialPort.SendCommandToFix("AT+MOT_IN%", ref recvStr, "OK", 10);
                            //禁用和开启网卡
                              
                                string name = "DUTPOE";
                                NetshInterfaceDisEnable(name);
                                loggerDebug("sleep 27 s");
                                Thread.Sleep(27000);
                                NetshInterfaceEnable(name);
                                
                           

                        }
                        else {
                            
                            loggerInfo("----------------->begin pop RJ45 fixture");

                            FixSerialPort.SendCommandToFix("AT+PORTEJECT%", ref recvStr, "OK", 10);

                            Sleep(500);

                            loggerInfo("----------------->begin push RJ45 fixture");

                            FixSerialPort.SendCommandToFix("AT+PORTINSERT%", ref recvStr, "OK", 10);



                            Sleep(500);

                            if ((Global.STATIONNAME == "SFT" && (item.ItemName == "WaitingToStart") && item.EeroName == "ENTER_KERNEL") == false)
                            {
                                loggerInfo("-------------------> waiting for start");
                                var rr = "";
                                if (DUTCOMM != null)
                                {
                                    if (DUTCOMM.SendCommand("", ref rr, "luxshare SW Version :", 120))
                                    {
                                        // byte[] quit = { 0x03 };
                                        // var cmd = Encoding.ASCII.GetString(quit).ToUpper();
                                        if (DUTCOMM.SendCommand(" \r\n", ref rr, "root@OpenWrt:/#", 1))
                                        {

                                        }
                                    }

                                    //
                                    if (Global.STATIONNAME == "SFT")
                                    {

                                        DUTCOMM.SendCommand("modprobe qca_nss_netlink", ref rr, "root@OpenWrt:/#", 10);

                                        DUTCOMM.SendCommand("ifconfig br-lan down && brctl delbr br-lan && ifconfig eth0 192.168.1.1 up && ifconfig eth1 192.168.0.101 up", ref rr, "root@OpenWrt:/#", 10);


                                    }
                                }


                            }


                        }




                 


                    }
            }
        }


        private void JsonAndCsv(Items item, phase_items phaseItem, bool specFlag, bool rReturn)
        {
            switch (item.ByPassFail.ToLower())
            {
                case "f":
                case "0":
                    item.tResult = false;
                    error_code = item.ErrorCode.Split(new string[] { "\n", "\r\n" }, 0)[0].Split(':')[0].Trim();
                    break;
                case "p":
                case "1":
                    item.tResult = true;
                    break;
                default:
                    item.tResult = item.IfElse.Contains("if") || rReturn;
                    if (!rReturn)
                        item.start_time_json = f1.startTimeJson;
                    break;
            }

            if (item.testValue == null)
                item.testValue = item.tResult.ToString();
            item.error_code = error_code;
            phaseItem.CopyFrom(specFlag, item, rReturn);
            stationObj.tests.Add(phaseItem);
            loggerDebug($"{item.ItemName} add test item to station");
 


            //收集csv数据
            if (!IsNullOrEmpty(item.Limit_min) || !IsNullOrEmpty(item.Limit_max))
            {
                if (Global.STATIONNAME == "SRF" || Global.STATIONNAME == "MBFT" || Global.STATIONNAME == "MBLT")
                {
                    if (item.testValue.ToUpper() != "TRUE" && item.testValue.ToUpper() != "FALSE") {



                        ArrayListCsvHeader.AddRange(new string[] { item.EeroName });
                        ArrayListCsvHeaderMAX.AddRange(new string[] { IsNullOrEmpty(item.Limit_max) ? "NA" : item.Limit_max });
                        ArrayListCsvHeaderMIN.AddRange(new string[] { IsNullOrEmpty(item.Limit_min) ? "NA" : item.Limit_min });
                        ArrayListCsvHeaderUNIT.AddRange(new string[] { " " });
                        ArrayListCsv.AddRange(new string[] { item.testValue });



 
                    }
                   
                }
                else {



                    ArrayListCsvHeader.AddRange(new string[] { item.EeroName });
                    ArrayListCsvHeaderMAX.AddRange(new string[] { IsNullOrEmpty(item.Limit_max) ? "NA" : item.Limit_max });
                    ArrayListCsvHeaderMIN.AddRange(new string[] { IsNullOrEmpty(item.Limit_min) ? "NA" : item.Limit_min });
                    ArrayListCsvHeaderUNIT.AddRange(new string[] { " " });
                    ArrayListCsv.AddRange(new string[] { item.testValue });
                }
               
            }
            else if (!IsNullOrEmpty(item.Spec))
            {
                if (item.testValue.ToUpper() != "TRUE" && item.testValue.ToUpper() != "FALSE")
                {
                    ArrayListCsvHeader.AddRange(new string[] { item.EeroName, item.EeroName + "_SPEC" });
                    ArrayListCsvHeaderMAX.AddRange(new string[] { "NA", "NA" });
                    ArrayListCsvHeaderMIN.AddRange(new string[] { "NA", "NA" });
                    ArrayListCsvHeaderUNIT.AddRange(new string[] { " ", " " });
                    ArrayListCsv.AddRange(new string[] { item.testValue, item.Spec });

                }
                
            }
            else
            {
                if (item.testValue.ToUpper() != "TRUE" && item.testValue.ToUpper() != "FALSE")
                {
                    ArrayListCsvHeader.AddRange(new string[] { item.EeroName });
                    ArrayListCsvHeaderMAX.AddRange(new string[] { "NA" });
                    ArrayListCsvHeaderMIN.AddRange(new string[] { "NA" });
                    ArrayListCsvHeaderUNIT.AddRange(new string[] { " " });
                    ArrayListCsv.AddRange(new string[] { item.testValue });
                }
            }
        }
    }

}
