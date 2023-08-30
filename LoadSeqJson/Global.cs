using AutoTestSystem.BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AutoTestSystem.Model
{
    /// <summary>
    /// 全局变量类
    /// </summary>
    public static class Global
    {
        public static List<Sequence> Sequences = new List<Sequence>();                                               //!测试用例队列
        public static string[] itemHeader;                                                                           //!用例表头
        //public static Version Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;         //!程序版本
        public static string debugPath = System.Environment.CurrentDirectory + @".\debug.txt";                                   //!debug log保存的初始路径
        public static readonly object wLock = new object();                                                          //!互斥锁
        public static string IniConfigFile = System.Environment.CurrentDirectory + @"\Config\Config.ini";                        //!配置文件路径文件;
        public static string LogPath;

        ///**************************ini配置文件测试Station全局配置变量**********************/
        //public static string DEBUTBUTTON;

        //public static string STATIONNAME;               //!工站名
        public static string STATIONALL;                 //!all工站名,
        //public static string FIXTURENAME;               //!工站编号
        //public static string STATIONNO;                 //!MES工站名
        //public static string MESIP;                     //!MES IP地址
        //public static string MESPORT;                   //!MES端口号
        //public static string LOGFOLDER;                 //!log保存路径
        //public static string LOGSERVER;                 //!log上传到FTP server路径
        //public static string LOGSERVERUser;             //!log FTP server登录用户名
        //public static string LOGSERVERPwd;              //!log FTP server登录密码
        //public static string FIXTUREFLAG;               //!是否使用治具，0=手动，1=使用治具
        //public static string FIXCOM;                    //!
        //public static string FIXBaudRate;               //!
        //public static string GPIBADDRESS;               //! Power Supply GPIB地址
        //public static string TESTMODE;                  //!测试模式
        public static string TestCasePath;              //!测试用例文件
        //public static string PySCRIPT;                  //!上传结果的JSON脚本
        //public static string PROMPT;

        ///**************************ini配置文件DUT全局配置变量**********************/
        //public static string DUTIP;

        //public static string SSH_PORT;
        //public static string SSH_USERNAME;
        //public static string SSH_PASSWORD;
        //public static string DUTCOM;
        //public static string DUTBaudRate;

        ///***************************ini配置文件Product产品全局配置变量**********************/
        //public static string ProMode;                   //产品机种

        //public static string ProMode1;
        //public static string ProMode2;
        //public static string ProMode3;
        //public static int SN_Length;
        //public static string SFC_Mode;
        //public static string IsSetAgingMode;
        //public static string IsSetQAMode;
        //public static string SWVersion;
        //public static string BTServerID;                //!Bluetooth ID
        //public static string BigTaoLogPath;
        //public static string UplaodQCNFilePath;
        //public static string SGN5171BIP;
        //public static string RFSwitch1P2TIP;
        //public static string RFSwitch1P8TIP;
        //public static string PowermeterNRP8SSN;
        //public static string QSDKVER;

        ///**************************ini配置文件CountNum全局配置变量**********************/
        //public static short CONTINUE_FAIL_LIMIT;        //

        //public static short ContinueFailNum;
        //public static Int32 Total_Fail_Num;
        //public static Int32 Total_Pass_Num;
        //public static Int32 Total_Abort_Num;

        ///**************************ini配置文件各工站全局配置变量**********************/
        //public static string CalTreeName;

        //public static string VerTreeName;

        public static void LoadSequnces()
        {
            ClearDirectory($@"{System.Environment.CurrentDirectory}\Config", ".json");
            string testCasePath2 = $@"{System.Environment.CurrentDirectory}\Config\{TestCasePath}";//.Replace("\\bin\\Debug", "");
            Console.WriteLine($"加载Excel用例文件路径:{testCasePath2}");
            // 用例excel加密密码123
            LoadSeq loadSeq = new LoadSeq(testCasePath2);
            string[] tempStation = Global.STATIONALL.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tempStation.Length; i++)
            {
                DataTable testCaseTable = loadSeq.GetFirstSheetToDT(tempStation[i]);
                // 获取表头
                itemHeader = loadSeq.GetColumnsByDataTable(testCaseTable);
                // 根据testCaseTable加载测试用例到Global.sequences
                Global.Sequences = loadSeq.GetSeq(testCaseTable, itemHeader, "SeqName", "ItemName");
                string jsonClintContent = JsonConvert.SerializeObject(Global.Sequences, Formatting.Indented);        //!Station Json数据序列化
                string jsonPath = $@"{System.Environment.CurrentDirectory}\Config\{tempStation[i]}.json";
                File.WriteAllText(jsonPath, jsonClintContent, Encoding.Default);
                //File.Copy($@"{System.Environment.CurrentDirectory}\Config\{tempStation[i]}.json", $@"F:\eero\AutoTestSystem\Runin\bin\Debug\Config\{tempStation[i]}.json", true);
                Console.WriteLine($"生成{tempStation[i]}站Json用例文件成功");
                //File.SetAttributes(jsonPath, FileAttributes.Hidden | FileAttributes.ReadOnly);

                Global.Sequences = new List<Sequence>();
                jsonClintContent = null;
                testCaseTable = new DataTable();
                itemHeader = null;
            }
            //File.Copy(TestCasePath2, $@"F:\eero\AutoTestSystem\Runin\bin\Debug\Config\fireflyALL.xlsx", true);
        }

        /// <summary>
        /// 清空目录下指定类型的文件
        /// </summary>
        /// <param name="DirPath">目录路径</param>
        /// <param name="extension ">扩展名，后缀.txt</param>
        /// <returns></returns>
        public static bool ClearDirectory(string DirPath, string extension)
        {
            bool rReturn = false;
            if (Directory.Exists(DirPath))
            {
                DirectoryInfo dir = new DirectoryInfo(DirPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i.Extension == extension)
                    {
                        File.SetAttributes(i.FullName, FileAttributes.Normal);
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
                Console.WriteLine($"删除旧的Json用例文件");
                rReturn = true;
            }
            else
            {
                rReturn = true;
            }
            return rReturn;
        }

        public static bool InitStation()
        {
            File.Delete(debugPath);
            INIHelper iniConfig = new INIHelper(Global.IniConfigFile);
            try
            {
                iniConfig.CheckPath(Global.IniConfigFile);
                //DEBUTBUTTON = iniConfig.Readini("Station", "DEBUTBUTTON").Trim();
                //STATIONNAME = iniConfig.Readini("Station", "STATIONNAME").Trim();
                STATIONALL = iniConfig.Readini("Station", "STATIONALL").Trim();
                //FIXTURENAME = iniConfig.Readini("Station", "FIXTURENAME").Trim();
                //STATIONNO = iniConfig.Readini("Station", "STATIONNO").Trim();
                //MESIP = iniConfig.Readini("Station", "MESIP").Trim();
                //MESPORT = iniConfig.Readini("Station", "MESPORT").Trim();
                //LOGFOLDER = iniConfig.Readini("Station", "LOGFOLDER").Trim();
                //LOGSERVER = iniConfig.Readini("Station", "LOGSERVER").Trim();
                //LOGSERVERUser = iniConfig.Readini("Station", "LOGSERVERUser").Trim();
                //LOGSERVERPwd = iniConfig.Readini("Station", "LOGSERVERPwd").Trim();
                //FIXTUREFLAG = iniConfig.Readini("Station", "FIXTUREFLAG").Trim();
                //FIXCOM = iniConfig.Readini("Station", "FIXCOM").Trim();
                //FIXBaudRate = iniConfig.Readini("Station", "FIXBaudRate").Trim();
                //GPIBADDRESS = iniConfig.Readini("Station", "GPIBADDRESS").Trim();
                //TESTMODE = iniConfig.Readini("Station", "TESTMODE").Trim();
                TestCasePath = iniConfig.Readini("Station", "TestCasePath").Trim();
                //PySCRIPT = iniConfig.Readini("Station", "PySCRIPT").Trim();
                //PROMPT = iniConfig.Readini("Station", "PROMPT").Trim();

                //DUTIP = iniConfig.Readini("DUT", "DUTIP").Trim();
                //SSH_PORT = iniConfig.Readini("DUT", "SSH_PORT").Trim();
                //SSH_USERNAME = iniConfig.Readini("DUT", "SSH_USERNAME").Trim();
                //SSH_PASSWORD = iniConfig.Readini("DUT", "SSH_PASSWORD").Trim();
                //DUTCOM = iniConfig.Readini("DUT", "DUTCOM").Trim();
                //DUTBaudRate = iniConfig.Readini("DUT", "DUTBaudRate").Trim();
                //ProMode = iniConfig.Readini("Product", "ProMode").Trim();
                //ProMode1 = iniConfig.Readini("Product", "ProMode1").Trim();
                //ProMode2 = iniConfig.Readini("Product", "ProMode2").Trim();
                //ProMode3 = iniConfig.Readini("Product", "ProMode3").Trim();
                //SN_Length = Int16.Parse(iniConfig.Readini("Product", "SN_Length").Trim());
                //SFC_Mode = iniConfig.Readini("Product", "SFC_Mode").Trim();
                //IsSetAgingMode = iniConfig.Readini("Product", "IsSetAgingMode").Trim();
                //IsSetQAMode = iniConfig.Readini("Product", "IsSetQAMode").Trim();
                //SWVersion = iniConfig.Readini("Product", "SWVersion").Trim();
                //BTServerID = iniConfig.Readini("Product", "BTServerID").Trim();
                //BigTaoLogPath = iniConfig.Readini("Product", "BigTaoLogPath").Trim();
                //UplaodQCNFilePath = iniConfig.Readini("Product", "UplaodQCNFilePath").Trim();
                //SGN5171BIP = iniConfig.Readini("Product", "SGN5171BIP").Trim();
                //RFSwitch1P2TIP = iniConfig.Readini("Product", "RFSwitch1P2TIP").Trim();
                //RFSwitch1P8TIP = iniConfig.Readini("Product", "RFSwitch1P8TIP").Trim();
                //PowermeterNRP8SSN = iniConfig.Readini("Product", "DUTBPowermeterNRP8SSNaudRate").Trim();
                //QSDKVER = iniConfig.Readini("Product", "QSDKVER").Trim();

                //CONTINUE_FAIL_LIMIT = short.Parse(iniConfig.Readini("CountNum", "CONTINUE_FAIL_LIMIT").Trim());
                //ContinueFailNum = short.Parse(iniConfig.Readini("CountNum", "ContinueFailNum").Trim());
                //Total_Fail_Num = Int32.Parse(iniConfig.Readini("CountNum", "Total_Fail_Num").Trim());
                //Total_Pass_Num = Int32.Parse(iniConfig.Readini("CountNum", "Total_Pass_Num").Trim());
                //Total_Abort_Num = Int32.Parse(iniConfig.Readini("CountNum", "Total_Abort_Num").Trim());
                //CalTreeName = iniConfig.Readini("TMORFTest", "CalTreeName").Trim();
                //VerTreeName = iniConfig.Readini("TMORFTest", "CalTreeName").Trim();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read iniconfig error,initStation:{ex.ToString()}", "ERROR!");
                throw;
            }
        }

        public static void SaveLog(string log, int type = 2)
        {
            try
            {
                lock (wLock)
                {
                    using (StreamWriter sw = new StreamWriter(debugPath, true, Encoding.Default))
                    {
                        if (type == 1) //不换行打印log
                        {
                            sw.Write(log);
                        }
                        else
                        {
                            sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {log}");
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString());
                // SaveLog(ex.ToString());
                //throw;
            }
        }
    }
}