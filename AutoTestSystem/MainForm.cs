using AutoTestSystem.BLL;
using AutoTestSystem.DAL;
using AutoTestSystem.Model;
using Checkroute;
using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using static AutoTestSystem.BLL.Bd;

namespace AutoTestSystem
{
    public partial class MainForm : Form
    {
        public static MainForm f1;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
      
        
        //用例表头
        private readonly string[] colHeader = new string[] { "SN", "ItemName", "Spec", "LSL", "tValue", "ULS", "ElapsedTime", "StartTime", "tResult" }; //测试step表头
        private delegate void SaveTestResult();                                                          //定义生成结果委托
        private SaveTestResult saveTestResult;
        private Thread testThread;                                                                       //!运行主线程
        private Thread scanThread;
        private volatile bool startFlag = false;                                                                  //!启动信号
        private volatile bool _startScanFlag = true;                                                               //!开始扫描信号
        private bool pauseFlag = false;                                                                  //!暂停信号
        public static bool IsDebug = false;                                                                    //!调试信号
        private bool singleStepTest = false;                                                             //!单步调试信号
        private bool isCycle = false;                                                                    //!循环测试信号
        public static bool IfCond = true;                                                                //!IF条件语句结果
        private int sec = 0;                                                                             //!测试时间
        private int seqNo = -1;                                                                          //!当前测试用例号
        private int itemsNo = -1;
        private bool uploadJsonSpecialFail = false;
        public int Cycle_Count = -1;

        //打开程序后的第一次运行
        public bool isFirstRun = true;

        //UBOOT fail ,LED_W保存
        public string UBOOT_LED_W_ReMSG1="";
        public string UBOOT_LED_W_ReMSG2 = "";
        public string UBOOT_LED_W_ReMSG3 = "";
        public string UBOOT_LED_W_ReMSG4 = "";
        public string UBOOT_LED_W_ReMSG5 = "";

        //!当前测试项目号
        public static DateTime startTime = new DateTime();                                                     //!开始测试时间
        private DateTime endTime = new DateTime();                                                       //!结束测试时间
        private readonly ManualResetEvent pauseEvent = new ManualResetEvent(true);                       //!暂停信号
        private readonly ManualResetEvent autoScanEvent = new ManualResetEvent(true);                    //!自动扫描信号
        public int PassNumOfCycleTest { get; set; }                                                      //!循环测试pass次数
        public int FailNumOfCycleTest { get; set; }                                                      //!循环测试fail次数
        private System.Threading.Timer timer;
        public static System.Timers.Timer Timer = new System.Timers.Timer(1000);

        private INIHelper iniConfig;
        private List<Sequence> sequences = null;                                                         //!测试用例队列
        public static MesPhases mesPhases = null;
        public test_phases TestPhase = new test_phases();
        
        
        public static Station stationObj = null;
       
        
        
        public static Mescheckroute mescheckroute;
        public static Test test;

        public static GPIBInfo GpibInfo;
        public static ConnectionInfo sshconInfo;                                                          //!SSH连接信息
        public static SerialConnetInfo FixCOMinfo;                                                        //!治具COM口连接信息
        public static SerialConnetInfo DUTCOMinfo;                                                        //!DUT COM口连接信息
        public static TelnetInfo telnetInfo;                                                              //!Telnet连接
        public static Communication DUTCOMM;
        public static Communication DUTCOMM2;
        public static Comport FixSerialPort;
        //public static GPIB GPIBCOMM;                                                                      //!GPIB连接
        public static Communication SampleComm;
        public static Communication STAComm; //赔测板
        public static DosCmd dosCmd = new DosCmd();

        public static string error_code = "";
        public static string error_details = "";
        private string error_code_firstfail = "";
        private string error_details_firstfail = "";
        private string finalTestResult = "FAIL";                                                         //最终结果,默认值为FAIL：测试&&&&Json&&MES上传结果
        private bool stationStatus = true;                                                               //总的测试结果，默认ture，有一个用例fail则为fail
        private string mesUrl;                                                                           //上传MESInfo的地址
        public static string SN;                                                                               //测试SN
        private string cellLogPath;                                                                      //logPath路径
        public string DutMode = "unknow";                                                                          //机种

        public string FIXTURE_TIME = "";
        public string DUT_PING_TIME;

        public static DateTime PowerToTelnetStartTime;
        public static DateTime PowerToTelnetEndTime;

        //WPS
        public static DLIOutletClient C;
        private WebSwitchConInfo _webSwitchCon;


        bool isTestAgain = false;
        int AUTOTESTNOFIXUTRE_COUNT = -1;

        //SRF/MBFT RF 数据缓存

        public static Dictionary<string, string> RFCSV_Cache = new Dictionary<string, string>();



        ///***************************  定义DUT产品相关全局变量  ********************************\\
        public const string regexp_GATEWAY = @"^((N)([1-9]|[A-Z])([1-9]|[A-C])([A-HJ-KM-NP-TV-Z]|[1-9]))([A-HJ-KM-NP-TV-Z]|[0-9]){4}$";
        public const string regexp_LEAF = @"^((Q)([1-9]|[A-Z])([1-9]|[A-C])([A-HJ-KM-NP-TV-Z]|[1-9]))([A-HJ-KM-NP-TV-Z]|[0-9]){4}$";
        public const string regexp_Firefly = "";// @"^((G)([1-9]|[A-Z])([1-9]|[A-C])([A-HJ-KM-NP-TV-Z]|[1-9]))([A-HJ-KM-NP-TV-Z]|[0-9]){4}$";
        public string regexp = "";

        //private string[] colSFTResult = new string[] {
        //    "StartTime","SN","Result","EndTime","ErrorCode","csn","mac","throughput","eth0_send","eth0_receive",
        //    "eth1_send","eth1_receive","USBRW","USBRSPEED","USBWSPEED","ResetButton",
        //    "LEDTEST","Ledoff","W_x","W_y","W_L","B_x","B_y","B_L","G_x","G_y","G_Y","R_x","R_y","R_L" };//cvs收集测试数据

        private delegate void CollectCsvResult();                                                        //定义生成结果委托
        private CollectCsvResult collectCsvResult;
        public static List<string> ArrayListCsv;
        public static List<string> ArrayListCsvHeader;
        public static List<string> ArrayListCsvHeaderMIN;
        public static List<string> ArrayListCsvHeaderMAX;
        public static List<string> ArrayListCsvHeaderUNIT;



        public static bool SetDefaultIP;
        public static string inPutValue = "";
        public static string BtDevAddress = "";
        public static string[] csvLines = null;
        public static string[] csvGoldenLines = null;
        public static string CSN = "";
        public static string DUTIP = "";       // DUT默认IP地址
        public static string DUTMesIP = "";    //MES分配的IP地址
        public static string MesMac = "";                                                                  //MES反馈的Mac地址
        public static bool SetIPflag = false;                                                              //是否设置IP为默认
        private int ItemFailCount = 0;
        public static string SSID_2G = "";// Global.STATIONNO + "_2G";
        public static string SSID_5G = "";// Global.STATIONNO + "_5G";
        public static string MD52G = "";
        public static string MD55G = "";
        public static string mes_qsdk_version = "";
        public static string QSDKVER = "";
        public static string BURNINWAITTIME = "0";
        public static int retry = 0;
        private string CSVFilePath = "";
        public static string WorkOrder = "0";
        private bool startTimeJsonFlag = true;
        public DateTime startTimeJson = DateTime.Now;
        public static Limits Online_Limit = null;

        public static string ImageVer = "";
        public static string UploadImageIP = "";
        public static bool CalibrationFail = false;
        public static bool ValidationFail = false;


        private DateTime preDateTime = DateTime.Now;


        public static int SRF_POP_RETRY = 1;  // SRF 射频fail之后
        public static int RTT_PING_RETRY = 1;
        public static int SFT_TXRX_RETRY = 1;

        public string CellLogPath = "";


        //新版扫码软件管理器
        ScanManager scanManager;
        public static string ZhuBoPath = "";
        public bool StartScanFlag
        {
            get => _startScanFlag;
            set
            {
                if (_startScanFlag == value) return;
                _startScanFlag = value;
#if DEBUG
#else
                if (value)
                {

                    //  StartProcess("Scan_Barcode_C03", $@"{System.Environment.CurrentDirectory}\Scan_Barcode_C03.exe");
                }
                else
                {
                 //   20200228 操作的 KillProcess("Scan_Barcode_C03");
                }
#endif
                
            }
        }



        ///***************************  定义DUT产品相关全局变量End  ********************************\\

        public enum TestStatus
        {
            PASS = 1,   //测试pass
            FAIL = 2,   //测试fail
            START = 3,  //开始测试，正在测试中
            ABORT = 4,
        }

        public MainForm()
        {

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi; //设定按分辨率来缩放控件
            InitializeComponent();

          

            this.groupBox3.Location = new Point(this.groupBox2.Location.X + this.groupBox2.Size.Width + 30, this.groupBox2.Location.Y);
            this.groupBox1.Location = new Point(this.groupBox3.Location.X + this.groupBox3.Size.Width + 30, this.groupBox3.Location.Y);
            this.bt_debug.Location = new Point(this.groupBox1.Location.X + this.groupBox1.Size.Width + 30, this.groupBox1.Location.Y * 9);
            this.lb_IPaddress.Location = new Point(this.bt_debug.Location.X + this.bt_debug.Size.Width + 30, this.bt_debug.Location.Y);
            f1 = this;
            Global.InitStation();   //!读配置文件,初始化全局配置变量

            if (Global.CYCLE_COUNT != "" && Global.CYCLE_COUNT != "0")
            {
                Cycle_Count = int.Parse(Global.CYCLE_COUNT);
            }

            if (Global.CameraType=="1")
            {
                scanManager = new ScanManager();
                scanManager.ScanFinished += ScanManager_ScanFinished;
            }


            AUTOTESTNOFIXUTRE_COUNT = int.Parse(Global.AUTOTESTNOFIXUTRE_COUNT);




            this.Visible = false;   //!用来防止初次开启界面卡顿现象
            this.Opacity = 0;
            timer1.Start();
            f1.Text = f1.Text + " V" + Global.Version;
            string LocalIP = Bd.GetAllIpv4Address("10.90."); //电脑时区设置，显示本机IP地址，方便远程桌面。
            lb_IPaddress.Text += LocalIP;
            this.textBox1.MaxLength = Global.SN_LENGTH;
#if DEBUG
#else
            if (!RunDosCmd("tzutil /g").Contains("China Standard"))
            {
                RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC
            }
#endif
            InitCreateDirs();

            //test 





            if (Screen.PrimaryScreen.Bounds.Width != 1920 && Screen.PrimaryScreen.Bounds.Width != 1536)
            {
                if (Environment.MachineName != "YNLX22OF1181") {
                    if (!SetDisplay.ChangeRes(1366, 768))
                    {
                        MessageBox.Show($"设置屏幕分辨率为1366*768,60Hz失败！请确认显示器是否支持此分辨率.");
                    }
                }
               
            }
        }
         
        private string CheckVersion() {
            try {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                HttpResponseMessage httpResponse = client.GetAsync("http://10.90.122.80:9000/version/version/snowbird").GetAwaiter().GetResult();
                string responseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var dataSet = JsonToDictionary(responseBody);
                var version = "";
                if (dataSet != null)
                {
                    foreach (KeyValuePair<string, object> item in dataSet)
                    {
                        if (item.Key.ToString() == "version")//获取header数据
                        {
                            version = (string)item.Value;
                           
                            break;
                        }
                    }
                }
                loggerInfo("Get Online version:" + version);
                return version;
            }
            catch {
                loggerInfo("Get Online version: error");
                return "";
            }
           
        }

        private bool CheckPathloss(string type)
        {
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
             

                var url = $@"http://10.90.122.80:9000/pathloss/checkpathloss/snowbird/{Global.STATIONNAME}/{Global.FIXTURENAME}/{type}";

                //loggerInfo("是否存在接口:" + url);

                HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                string responseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var dataSet = JsonToDictionary(responseBody);
                var exist = "";
                if (dataSet != null)
                {
                    foreach (KeyValuePair<string, object> item in dataSet)
                    {
                        if (item.Key.ToString() == "exist")//获取header数据
                        {
                            exist = (string)item.Value;
                          
                            break;
                        }
                    }
                }
                loggerInfo("pathloss 是否存在:" + exist);
                return exist == "1";
            }
            catch
            {
                loggerInfo("check Pathloss error");
                return false;
            }

        }




        private async void  DownloadPathloss(string type) {
            try
            {
               // var url = $@"http://localhost:9000/upload/public/snowbird/{Global.STATIONNAME}/{Global.FIXTURENAME}/{type}/path_loss.csv";
                var url = $@"http://10.90.122.80:9000/upload/public/snowbird/{Global.STATIONNAME}/{Global.FIXTURENAME}/{type}/path_loss.csv";


               // MessageBox.Show("下载地址:" + url);
                
                var save = @"";
                if (type == "wifi")
                {
                    
                    save = @"E:\Eero_snowbird_LUX_MBFT_ATSuitev1.0.2\release\" + "path_loss.csv";
                }
                else if (type == "bluetooth")
                {
                    save = @"E:\ConsoleATEv1.0.4\bt\Setup\" + "path_loss.csv";
                }


                using (var web = new WebClient())
                {
                    await web.DownloadFileTaskAsync(url, save);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);


                loggerError("download path_loss fail：" + ex.Message);
            }
           
        }


        private async void DownloadPathloss2(string type)
        {
            try
            {
               
                
                var url = $@"http://10.90.122.80:9000/upload/public/snowbird/{Global.STATIONNAME}/{Global.FIXTURENAME}/{type}/path_loss.csv";


                // var url = $@"http://172.23.241.211:9000/upload/public/snowbird/SRF/SRF-8402/wifi/path_loss.csv";

               
                
                var save = @"";
                if (type == "wifi")
                {

                    save = @"E:\ConsoleATEv1.0.4\srf\" + "pathloss.ini";
                }
                else if (type == "bluetooth")
                {
                    save = @"E:\ConsoleATEv1.0.4\bt\" + "pathloss.ini";
                }


                using (var web = new WebClient())
                {
                    await web.DownloadFileTaskAsync(url, save);
                }
            }
            catch (Exception ex)
            {
                loggerError("download path_loss fail：" + ex.Message);
            }

        }


        private static void InitCreateDirs()
        {
            // 系统使用的文件夹确认和初始化
            if (IsDebug || Global.TESTMODE.ToLower() == "debug" || Global.TESTMODE.ToLower() == "fa")
                Global.LogPath = $@"{Global.LOGFOLDER}\{DateTime.Now:yyyyMMdd}\debug";
            else
                Global.LogPath = $@"{Global.LOGFOLDER}\{DateTime.Now:yyyyMMdd}";
            CheckFolder(Environment.CurrentDirectory + @"\OutPut");
            CheckFolder(Environment.CurrentDirectory + @"\Data");
            CheckFolder(Global.LogPath + @"\Json");
            CheckFolder(Global.LOGFOLDER + @"\CsvData\Upload");
        }



        void AddConsume()
        {
            try
            { 
                var URL = Global.VersionMURL;


               // URL = "http://172.23.241.211:9000";

                var name = Global.STATIONNAME;
                var NO = Global.STATIONNO;
                //清理耗材
                var url = URL + "/consumables/add/snowbird/" + name + "/" + NO;
                var client = new HttpClient();

                HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                string result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();


                if (result.Contains("ok"))
                {

                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                loggerError("error1:"+ex.Message);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
           


            //先进行线上版本检测

            if (Global.VERSION_CHECK == "1") { 
               var versionOnline = CheckVersion();
              
                if (versionOnline != Global.Version.ToString()) {
                    MessageBox.Show("当前版本与线上版本不一致\r\nPhiên bản hiện tại không phù hợp với phiên bản trực tuyến");
                    Application.Exit();
                    return;
                }
            }
            //下载pathloss文件
            if (Global.STATIONNAME == "MBFT") {
                if (Global.IsDownloadPathloss == "1") {

                    if (CheckPathloss("wifi") && CheckPathloss("bluetooth")) {
                        DownloadPathloss("wifi");
                        DownloadPathloss("bluetooth");
                    }
                   
                }
               
                
               

            }
            else if (Global.STATIONNAME == "SRF") {
                if (Global.IsDownloadPathloss == "1")
                {
                    if (CheckPathloss("wifi") && CheckPathloss("bluetooth"))
                    {
                        DownloadPathloss2("wifi");
                        DownloadPathloss2("bluetooth");
                    }
                       
                }
               
            }

            
                mescheckroute = new Mescheckroute(Global.MescheckrouteIP);
#if DEBUG
            //if (Global.DEBUTBUTTON == "0")
            bt_debug.Visible = true;
#else
            if (Global.DEBUTBUTTON == "1" && (Global.TESTMODE == "debug" || Global.TESTMODE.ToLower() == "fa"))
            {
                bt_debug.Visible = true;
            }
            else {
                bt_debug.Visible = false;
            }
                
#endif
            DUTIP = Global.DUTIP;
            // 初始化通信信息
            if (!string.IsNullOrEmpty(Global.DUTIP) && !string.IsNullOrEmpty(Global.SSH_USERNAME) && !string.IsNullOrEmpty(Global.SSH_PASSWORD) && !string.IsNullOrEmpty(Global.SSH_PORT))
            {
                sshconInfo = new ConnectionInfo(Global.DUTIP, Int16.Parse(Global.SSH_PORT), Global.SSH_USERNAME,
                  new AuthenticationMethod[] { new PasswordAuthenticationMethod(Global.SSH_USERNAME, Global.SSH_PASSWORD) });
                loggerDebug("initialize sshconInfo success!");
            }
            if (!string.IsNullOrEmpty(Global.FIXCOM) && !string.IsNullOrEmpty(Global.FIXBaudRate))
            {
                FixCOMinfo = new SerialConnetInfo { PortName = Global.FIXCOM, BaudRate = int.Parse(Global.FIXBaudRate) };
                loggerDebug("initialize FixCOMinfo success!");
            }
            if (!string.IsNullOrEmpty(Global.DUTCOM) && !string.IsNullOrEmpty(Global.DUTBaudRate))
            {
                DUTCOMinfo = new SerialConnetInfo { PortName = Global.DUTCOM, BaudRate = int.Parse(Global.DUTBaudRate) };
                loggerDebug("initialize DUTCOMinfo success!");
            }
            if (!string.IsNullOrEmpty(DUTIP))
            {
                telnetInfo = new TelnetInfo { _Address = DUTIP };
                loggerDebug("initialize TelnetInfo success!");
            }
            if (!string.IsNullOrEmpty(Global.GPIBADDRESS))
            {
                GpibInfo = new GPIBInfo { _GPIBAddress = Global.GPIBADDRESS, _Board = "0" };
                loggerDebug("initialize GPIBInfo success!");
            }
            iniConfig = new INIHelper(Global.IniConfigFile);

            UpdateContLable();                                         //!更新状态栏
            InitdataGridViewColumns();                                 //!初始化dataGridView表头
#if DEBUG
           
            GetFixName();
#else
            GetFixName();  //!获取治具号和MES工站编号，更新主界面Lable。
#endif
            Global.LoadSequnces();                                     //!从表格加载测试用例序列
            loggerDebug($"upload test-case form {Global.JsonFilePath}.");
            sequences = Global.Sequences;                              //!浅复制测试用例序列对象
            //Sequences = ObjectCopier.Clone<List<Sequence>>(Global.Sequences);           //!克隆测试用例序列对象
            lbl_testMode.Text = Global.TESTMODE;
            lbl_testMode.BackColor = (Global.TESTMODE.ToLower() == "debug" || Global.TESTMODE.ToLower() == "fa") ? Color.Red : panelButton.BackColor;
            lbl_StationNo.Text = Global.STATIONNO;
            saveTestResult = new SaveTestResult(SaveTestResultToCsv);           //!委托初始化更新测试结果
            collectCsvResult = new CollectCsvResult(CollectResultToCsv);    //!委托初始化更新测试结果
            LoadSeqTreeView();                                         //!加载测试用例界面treeview
            SetTextBox(textBox1);                                      //!设置当前窗口的活动控件和焦点为textBox1

            //初始化WS
            _webSwitchCon = new WebSwitchConInfo { IPAddress = Global.WebPsIp, Username = "admin", Password = "1234" };
            C = new DLIOutletClient(_webSwitchCon);


            if (Global.CameraType == "1" && Global.STATIONNAME != "CCT")
            {
                scanThread = new Thread(new ThreadStart(ScanThreadNew))
                {
                    IsBackground = true
                   
                };      

            }
            else if (Global.CameraType == "0" && Global.STATIONNAME != "CCT") {




                if (Global.AUTOTESTNOFIXUTRE_COUNT == "-1")
                {
                    scanThread = new Thread(new ThreadStart(ScanThread1))
                    {
                        IsBackground = true
                    };

                }
                else {


                    scanThread = new Thread(new ThreadStart(ScanThread1_Nofixutre))
                    {
                        IsBackground = true
                    };

                    


                }
                
               
           
            
            
            
            }





#if DEBUG
            //if (Global.STATIONNAME != "CCT")
            //{

            //    scanThread.Start();
            //}

#else

            if (Global.STATIONNAME != "CCT")
            {

                scanThread.Start();
            }
          
           
#endif
            testThread = new Thread(new ThreadStart(TestThread))
            {
                IsBackground = true
            };      //!开始测试主线程
            testThread.Start();


          // textBox1.Text = "GGC3530132850621";
        }

        private void ScanManager_ScanFinished(string sn)
        {
            if (sn.Length > 0) {
                Global.time2 = DateTime.Now;
                SetTextBox(textBox1, true, sn);
                scanManager.Close();
            }
              
        }



        /// <summary>
        /// 自动扫描进程
        /// </summary>
        private void ScanThreadNew()
        {

            string fixcomRcv = "";
         
            try
            {
                if (Global.STATIONNAME != "CCT" && Global.FIXTUREFLAG == "1")
                {
                    while (true)
                    {
                        if (StartScanFlag)

                        {
                            fixcomRcv += FixSerialPort.Read();
                            if (

                                (fixcomRcv.Contains("AT+SCAN") && !startFlag)
                                ||
                                (isFirstRun == false && Cycle_Count > 0 && !startFlag)

                                ) // 通过按治具的scan button发送执行扫描命令
                              {

                                if (isFirstRun == false && Cycle_Count > 0 && !startFlag)
                                {
                                    Cycle_Count--;
                                }

                                fixcomRcv = "";
                                loggerDebug("Check Prompt AT+SCAN OK...");
                                Global.time1 = DateTime.Now;




                                //loggerInfo("把窗体放在最上层1");
                                //窗体放在最上层
                                var process = Process.GetProcessesByName("AutoTestSystem")[0];
                                var handle = process.MainWindowHandle;

                                //窗体放在最上层
                                SwitchToThisWindow(handle, true);


                                //开启扫描
                                scanManager.Scan();

                                StartScanFlag = false; //跳出循环
                            }
                            else
                            {
                                Thread.Sleep(1);
                            }

                        }
                        else {
                            Thread.Sleep(1);
                        }
                          
                    }
                }
                // scanThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                //abort线程忽略报错
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        private void ScanThread1_Nofixutre()
        {

            string fixcomRcv = "";
            string barcode = $@"{System.Environment.CurrentDirectory}\Barcode.txt";
            try
            {
                if (Global.STATIONNAME != "CCT" && Global.FIXTUREFLAG == "1")
                {
                    while (true)
                    {
                        if (StartScanFlag)
                        {
                            int ii = 0;

                            if (
                                isTestAgain

                                )


                            {
                                isTestAgain = false;
                                SetTextBox(textBox1, true, SN);
                                TextBox1_KeyDown(null, null);
                            }
                            else
                                Thread.Sleep(1);
                        }
                        else
                            Thread.Sleep(1);
                    }
                }
                // scanThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                //abort线程忽略报错
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }


        /// <summary>
        /// 自动扫描进程
        /// </summary>
        private void ScanThread1()
        {

            string fixcomRcv = "";
            string barcode = $@"{System.Environment.CurrentDirectory}\Barcode.txt";
            try
            {
                if (Global.STATIONNAME != "CCT" && Global.FIXTUREFLAG == "1")
                {
                    while (true)
                    {
                        if (StartScanFlag)
                        {
                             
                                       
                            fixcomRcv += FixSerialPort.Read();
                                if (

                                    (fixcomRcv.Contains("AT+SCAN") && !startFlag)
                                    ||
                                    (isFirstRun == false && Cycle_Count > 0 && !startFlag)

                                    ) // 通过按治具的scan button发送执行扫描命令


                                {
                                    try
                                    {
                                        //杀死驻波程序


                                        if (Global.STATIONNAME == "SRF")
                                        {
                                            KillProcessNoRes("QCATestSuite"); //SRF
                                            KillProcessNoRes("QPSTConfig"); //SRF
                                            KillProcessNoRes("BTTestSuite");//SRF
                                            KillProcessNoRes("QPSTServer");//SRF

                                        }

                                        if (Global.STATIONNAME == "MBFT")
                                        {
                                            KillProcessNoRes("ATSuite"); //MBFT
                                            KillProcessNoRes("BTTestSuiteRD");//MBFT
                                            KillProcessNoRes("QPSTConfig"); //MBFT
                                            KillProcessNoRes("QPSTServer");//MBFT
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        loggerError("error2:" + ex.ToString());
                                    }



                                    if (isFirstRun == false && Cycle_Count > 0 && !startFlag)
                                    {
                                        Cycle_Count--;
                                    }

                                    fixcomRcv = "";
                                    loggerDebug("Check Prompt AT+SCAN OK...");
                                    Global.time1 = DateTime.Now;



                                    int retryTimes = 6;
                                    for (int i = 1; i <= retryTimes; i++)
                                    {
                                        if (File.Exists(barcode))
                                            File.Delete(barcode);





                                        loggerDebug("------------------>start run1 Scan_Barcode_C03");

                                        //修改电脑时间
                                        preDateTime = DateTime.Now;//保存当前时间

                                        DateTime oldTime = Convert.ToDateTime("2021-10-02 10:00:00");

                                        SyDateTimeHelper.SetLocalDateTime2(oldTime);

                                        Thread.Sleep(50);

                                        RunDosCmd2("Scan_Barcode_C03.exe");
                                        // StartProcess("Scan_Barcode_C03", $@"{System.Environment.CurrentDirectory}\Scan_Barcode_C03.exe");
                                        var lngStart = DateTime.Now.AddSeconds(3).Ticks;
                                        while (DateTime.Now.Ticks <= lngStart)
                                        {
                                            //loggerDebug("开始查找Barcode.txt......");
                                            // loggerInfo($@"{System.Environment.CurrentDirectory}\Barcode.txt");
                                            bool found = false;
                                            var re = RunDosCmd("type " + $@"{System.Environment.CurrentDirectory}\Barcode.txt");
                                            //  loggerInfo("dos返回:" + re);
                                            if (re.Contains("找不到") || re.Contains("cannot find "))
                                            {
                                                found = false;
                                            }
                                            else
                                            {
                                                found = true;
                                            }
                                            if (found)
                                            {
                                                loggerDebug(" find Barcode.txt，close scan software");
                                                //关闭扫码软件

                                                // 20200228 操作的   KillProcess("Scan_Barcode_C03");

                                                //扫码完成，重置时间，获取的是谷歌时间

                                                SyDateTimeHelper.SetLocalDateTime2(SyDateTimeHelper.GetNetDateTime(preDateTime.AddMilliseconds(0)));

                                                using (var sr = new StreamReader(barcode, Encoding.Default))
                                                {
                                                    string readAll = sr.ReadToEnd();
                                                    loggerDebug(readAll);
                                                    string textBox1Sn = readAll.Trim().Replace("1_", "").Replace("\n", "").Replace("\r\n", "");
                                                    loggerDebug("------------------>give textbox value:" + textBox1Sn);
                                                    Global.time2 = DateTime.Now;

                                                    SetTextBox(textBox1, true, textBox1Sn);
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                // loggerDebug("没找到");
                                            }




                                            Thread.Sleep(1);
                                        }

                                        if (textBox1.Text.Length == Global.SN_LENGTH && Regex.IsMatch(textBox1.Text, regexp))
                                        {
                                            loggerDebug("------------------>give standard SN value:" + textBox1.Text);
                                            TextBox1_KeyDown(null, null);
                                            fixcomRcv = "";
                                            SyDateTimeHelper.SetLocalDateTime2(SyDateTimeHelper.GetNetDateTime(preDateTime.AddMilliseconds(0)));
                                            break;
                                        }
                                        else
                                        {
                                            loggerDebug("------------------> SN not standard:" + textBox1.Text);
                                            KillProcess("Scan_Barcode_C03");

                                        }

                                        loggerDebug($"Scan_Barcode get SN:{textBox1.Text} error.");
                                        //重置UTC时间
                                        SyDateTimeHelper.SetLocalDateTime2(SyDateTimeHelper.GetNetDateTime(preDateTime.AddMilliseconds(0)));

                                        if (i != retryTimes) continue;

                                        loggerDebug($"Scan_Barcode have fail {retryTimes} times,pls tell TE.");
                                        // MessageBox.Show($"Scan_Barcode have fail {retryTimes} times,pls tell TE.");
                                    }
                                }
                                else {
                                    Thread.Sleep(1);
                                }
                              
                            
                        }
                        else
                            Thread.Sleep(1);
                    }
                }
                // scanThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                //abort线程忽略报错
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }



        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
           

                // e=null时候，不执行&&后面的
             if ( e != null && e.KeyCode != Keys.Enter) {
                return;
            }



            isTestAgain = false;



            // 扫描SN
            string ScanSN = textBox1.Text.Trim().TrimEnd(new char[] { '\n', '\t', '\r' }).ToUpper();


            if (Global.STATIONNAME == "BURNIN" && Global.TESTMODE == "production")
            {
                WorkOrder = "2";
            }
            else
            {
                WorkOrder = "1";
            }


       



            if (Global.STATIONNAME == "MBFT" || Global.STATIONNAME == "SRF") {
                RestartProcess("QUTSStatusApp", @"C:\Program Files (x86)\QUALCOMM\QUTSStatusApp\QUTSStatusApp.exe");
            }

         


            StartScanFlag = false;
            //autoScanEvent.Reset();      // 停止自动扫描
            //KillProcess("Scan_Barcode_C03");
            SN = "";
            // 重置treeViewSeq.Node颜色
            for (int i = 0; i < sequences.Count; i++)
            {
                for (int j = 0; j < sequences[i].SeqItems.Count; j++)
                { treeViewSeq.Nodes[i].Nodes[j].BackColor = Color.White; }
            }
            if (IsDebug)
            {
            }
            else
            {
                sequences = ObjectCopier.Clone<List<Sequence>>(Global.Sequences);           //!克隆测试用例序列对象
                LoadSeqTreeView();
            }
            // 连续FAIL提示
            if (!IsDebug && Global.TESTMODE.ToLower() != "debug" && Global.TESTMODE.ToLower()!="fa" && !CheckContinueFailNum()) { return; }
            // 创建测试主线程
            if (!testThread.IsAlive)
            {
                testThread = new Thread(new ThreadStart(TestThread))
                {
                    IsBackground = true
                };
                testThread.Start();
            }
            
           
            // 检查扫描的SN的长度
            if (ScanSN.Length != Global.SN_LENGTH && !IsDebug)
            {
                MessageBox.Show($"SN:{ScanSN} length {ScanSN.Length} is wrong,{Global.SN_LENGTH} is right.Please Scan again!", "SN Length Fail", 0, MessageBoxIcon.Error);
                SetTextBox(textBox1); return;
            }
            // 根据扫描的SN判断机种
            if (!JudgeProdMode(ScanSN)) return;
            // 检查SN规则是否正确
            if (!Regex.IsMatch(ScanSN, regexp) && !IsDebug)
            {
                MessageBox.Show($"SN matching rule is wrong ,Please find TE!", "SN Match Rule", 0, MessageBoxIcon.Error);
                SetTextBox(textBox1); return;
            }
            SN = ScanSN;

            


            //解析goldenSN数据

            if (Global.GoldenSN.Contains(SN) && Global.STATIONNAME == "MBFT")
            {

                var GoldenSNDirectoryPath = $@"{ System.Environment.CurrentDirectory}\Config\GoldenSN";
                if (Directory.Exists(GoldenSNDirectoryPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(GoldenSNDirectoryPath);
                    FileSystemInfo[] filesArray = directory.GetFileSystemInfos();
                    bool hasFound = false;
                    var GoldenSNPath = "";
                    foreach (var item2 in filesArray)
                    {
                        //是否是一个文件夹
                        if (item2.Attributes != FileAttributes.Directory)
                        {
                            if (item2.FullName.Contains(SN))
                            {
                                hasFound = true;
                                GoldenSNPath = item2.FullName;
                                break;
                            }

                        }

                    }

                    if (hasFound && GoldenSNPath.Length > 0)
                    {
                        loggerDebug($">>>>>>>>>>>>>>>>>>>>>>find goldenSN csv File:{GoldenSNPath}");
                        //Thread.Sleep(3000);
                        using (var sr = new StreamReader(GoldenSNPath))
                        {
                            csvGoldenLines = sr.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        }

                    }
                }
            }






 
            SaveRichText(true);



#if DEBUG
            Global.OnlineLimit = "0";
#else
            if (!GetLimit()) {

                Thread.Sleep(3000);

                if (!GetLimit()) {
                    return;
                }

               
            }
               
               
              
#endif
            StartTestInit();
        }

        #region 测试前期处理函数

        /// <summary>
        /// 更新状态栏计数
        /// </summary>
        public void UpdateContLable()
        {
            lb_ContinuousFailNum.InvokeOnToolStripItem(lb_ContinuousFailNum => lb_ContinuousFailNum.Text = Global.ContinueFailNum.ToString());
            lb_passNum.InvokeOnToolStripItem(lb_passNum => lb_passNum.Text = Global.Total_Pass_Num.ToString());
            lb_FailNum.InvokeOnToolStripItem(lb_FailNum => lb_FailNum.Text = Global.Total_Fail_Num.ToString());
            lb_AbortNum.InvokeOnToolStripItem(lb_AbortNum => lb_AbortNum.Text = Global.Total_Abort_Num.ToString());
            lb_YieldNum.InvokeOnToolStripItem(lb_YieldNum => lb_YieldNum.Text = $@"{(Global.Total_Pass_Num / (double)(Global.Total_Pass_Num + Global.Total_Fail_Num)),0:P2}");
        }

        private void InitdataGridViewColumns()
        {
            for (int i = 0; i < colHeader.Length; i++)
            {
                dataGridViewDetail.Columns.Add(colHeader[i], colHeader[i]);
                dataGridViewDetail.Columns[colHeader[i]].HeaderText = colHeader[i];
                dataGridViewDetail.Columns[colHeader[i]].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewDetail.Columns[colHeader[i]].DefaultCellStyle.ForeColor = Color.White;
                dataGridViewDetail.Columns[colHeader[i]].HeaderCell.Style.BackColor = Color.LightSlateGray;
                dataGridViewDetail.Columns[colHeader[i]].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewDetail.Columns[colHeader[i]].ReadOnly = true;
                dataGridViewDetail.Columns[colHeader[i]].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        /// <summary>
        /// 显示测试用例,更新测试内容
        /// </summary>
        public void LoadSeqTreeView()
        {
            if (sequences.Count > 0)
            {
                seqNo = 0;
                itemsNo = 0;
                ShowTreeView();
            }
            else
            { MessageBox.Show($"Failed to load the test case! Global.Sequences number is {sequences.Count}"); }
        }

        /// <summary>
        /// 检查连续Fail次数是否超过限定值
        /// </summary>
        private bool CheckContinueFailNum()
        {
            if (Global.ContinueFailNum >= Global.CONTINUE_FAIL_LIMIT)
            {
                toolStripContinuFailNum.InvokeOnToolStripItem(toolStripContinuFailNum => toolStripContinuFailNum.ForeColor = Color.Red);
                ContinuousFailReset_Click(null, null);
                return false;
            }
            else
            {
                toolStripContinuFailNum.InvokeOnToolStripItem(toolStripContinuFailNum => toolStripContinuFailNum.ForeColor = Color.Black);
                return true;
            }
        }

        /// <summary>
        /// 根据扫描的SN判断机种
        /// </summary>
        public bool JudgeProdMode(string sn, bool isJudge = true)
        {
            bool rReturn = true;
            if (isJudge && !string.IsNullOrEmpty(sn))
            {
                switch (sn[0])
                {
                    case 'J':
                    case '6':
                        //DutMode = Global.ProMode0;
                        //regexp = regexp_Cento;
                        break;

                    case 'N':
                    case '7':
                        if (DutMode != Global.ProMode1)
                            Online_Limit = null;
                        DutMode = Global.ProMode1;
                        regexp = regexp_GATEWAY;

                        break;

                    case 'Q':
                    case '8':
                        if (DutMode != Global.ProMode2)
                            Online_Limit = null;
                        DutMode = Global.ProMode2;
                        regexp = regexp_LEAF;
                        break;

                    case 'S':
                    case 'G':
                        if (DutMode != Global.ProMode3)
                            Online_Limit = null;
                        DutMode = Global.ProMode3;
                        regexp = regexp_Firefly;
                        break;
                    default:
                        if (IsDebug)
                            DutMode = Global.ProMode;
                        else
                        {
                            MessageBox.Show("无法根据SN判断机种！");
                            DutMode = "unknown";
                            rReturn = false;
                        }
                        break;
                }
            }
            SetLables(this.lb_mode, DutMode, panelButton.BackColor);
            return rReturn;
        }


        public void GetFixName()
        {
            //return;



            var Name = Environment.MachineName;
             //var Name = "BURNIN-16555";

            if (Name.Contains("-"))
            {
                Global.STATIONNO = Name;
                Global.FIXTURENAME = Global.STATIONNO;

                int lastIndex = Global.STATIONNO.LastIndexOf('-');
                Global.STATIONNAME = Global.STATIONNO.Substring(0, lastIndex);


            }



            if (Global.FIXTUREFLAG == "0") {
                FixSerialPort = new Comport(FixCOMinfo);
                FixSerialPort.OpenCOM();
                return;
            }
            try
            {
               
                FixSerialPort = new Comport(FixCOMinfo);
                FixSerialPort.OpenCOM();

                //Global.FIXTURENAME = "MBFT-10100";
                // Global.FIXTURENAME = "MBLT-10001";
                //lbl_StationNo.Text = Global.FIXTURENAME;
                //Global.STATIONNO = Global.FIXTURENAME;
                //Global.STATIONNAME = Global.FIXTURENAME.Substring(0, Global.FIXTURENAME.IndexOf("-"));





                return;
                string recvStr = "";
                for (int i = 0; i < 3; i++)
                {
                    loggerDebug($"READ_FIXNUM {i}");
                   if (FixSerialPort.SendCommandToFix("AT+READ_FIXNUM%", ref recvStr, "\r\n", 1))
                    
                    {
                        // Global.FIXTURENAME = recvStr.Replace("\r\n", "").Trim();

                      //  Global.FIXTURENAME = "MBFT-10100";
                      //  Global.FIXTURENAME = "MBLT-10001";
                     //   lbl_StationNo.Text = Global.FIXTURENAME;
                     //   Global.STATIONNO = Global.FIXTURENAME;
                     //   Global.STATIONNAME = Global.FIXTURENAME.Substring(0, Global.FIXTURENAME.IndexOf("-"));
                       
                        
                        
                        iniConfig.Writeini("Station", "STATIONNAME", Global.STATIONNAME);
                        iniConfig.Writeini("Station", "STATIONNO", Global.STATIONNO);
                        //iniConfig.Writeini("Station", "FIXTURENAME", Global.FIXTURENAME);
                        loggerDebug($"Read fix number success,stationName:{ Global.STATIONNAME}");
                        break;
                    }

                    if (i == 2)
                    {

                        if (!Environment.MachineName.Contains("CCT")) {
                            MessageBox.Show($"Read FixNum error,Please check it!");
                            System.Environment.Exit(0);
                        
                        }
                      



                        ////CCT
                        //lbl_StationNo.Text = "CCT-1630";
                        //Global.STATIONNO = "CCT-1630";
                        //Global.STATIONNAME = "CCT";
                        //Global.FIXTURENAME = "CCT-1630";
                        //iniConfig.Writeini("Station", "STATIONNAME", Global.STATIONNAME);
                        //iniConfig.Writeini("Station", "STATIONNO", Global.STATIONNO);
                        //iniConfig.Writeini("Station", "FIXTURENAME", Global.FIXTURENAME);

                    }
                    
                }
                if (Global.STATIONNAME != "CCT") {
                  //  GetPoePort();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                Application.Exit();
            }
        }



        public void GetPoePort()
        {
            if (Global.FIXTUREFLAG == "0" || (Global.STATIONNAME != "MBLT" && Global.STATIONNAME != "SRF"))
            { return; }

            try
            {
                string recvStr = "";
                for (int i = 0; i < 3; i++)
                {
                    loggerDebug($"READ_POE_Port {i}");
                    if (FixSerialPort.SendCommandToFix("AT+READ_POE%", ref recvStr, "\r\n", 1))
                    {
                        string port = recvStr.Replace("\r\n", "").Replace("POE-", "").Replace("%", "").Trim();
                        Global.POE_PORT = port.Substring(port.LastIndexOf("0") + 1);
                        if (int.TryParse(Global.POE_PORT, out int ds))
                        {
                            iniConfig.Writeini("DUT", "POE_PORT", Global.POE_PORT);
                            loggerDebug($"Read fix POE port success:{ Global.POE_PORT}");
                            break;
                        }
                        else
                        {
                            MessageBox.Show($"Error! Read fix POE port:{Global.POE_PORT},pls check it.");
                            Application.Exit();
                        }
                    }

                    if (i == 2)
                    {
                        MessageBox.Show($"Read  POE port error,Please check it!");
                        System.Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                Application.Exit();
            }
        }

        /// <summary>
        /// 按下开始测试按钮,初始化内容
        /// </summary>
        private void StartTestInit()
        {
#if !DEBUG
            if (!RunDosCmd("tzutil /g").Contains("China Standard"))
            {
                RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC
            }
#endif

           
            loggerInfo("~~~~~~~~~~~~~~~~~~~Scan Start Time:" + Global.time1.ToLongTimeString());
            loggerInfo("~~~~~~~~~~~~~~~~~~~Scan End Time:" + Global.time2.ToLongTimeString());


            DUTCOMM = null;
            test = new Test(SN, DutMode, DUTIP);
           
            ResetData();
            SRF_POP_RETRY = 1;
            RTT_PING_RETRY = 1;
            SetLables(lbl_failCount, "", Color.White, false);

           
            SetTestStatus(TestStatus.START);
        }

        void ResetData() {
            mesPhases = new MesPhases();
            stationObj = new Station(SN, Global.FIXTURENAME, Global.STATIONNAME, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Global.TESTMODE, Global.QSDKVER, Global.Version.ToString());
            InitCreateDirs();
            ArrayListCsv = new List<string>();
            ArrayListCsvHeader = new List<string>();

            ArrayListCsvHeaderMIN = new List<string>();
            ArrayListCsvHeaderMAX = new List<string>();
            ArrayListCsvHeaderUNIT = new List<string>();


            error_code = "";
            error_details = "";
            error_code_firstfail = "";
            error_details_firstfail = "";
            finalTestResult = "FAIL";
            SetIPflag = false;
            IfCond = true;
            DUTMesIP = "";
            MesMac = "";
            QSDKVER = Global.QSDKVER;
            ImageVer = QSDKVER.Substring(QSDKVER.LastIndexOf(".") + 1).ToLower();
            UploadImageIP = Global.UploadImageIP; 
            CSN = "";
            sec = 0;
            seqNo = 0;
            itemsNo = 0;
            stationStatus = true;
            inPutValue = "";
            BtDevAddress = "";
            ItemFailCount = 0;
            ZhuBoPath = "";
            SSID_2G = "2G_" + Global.STATIONNO;
            SSID_5G = "5G_" + Global.STATIONNO;
            MD52G = "xxx";
            MD55G = "xxx";
            retry = 0;
            WorkOrder = "1";

            BURNINWAITTIME = Global.BURNINWAITTIME;

            if (Global.STATIONNAME == "BURNIN" && Global.TESTMODE == "production") {
                WorkOrder = "2";
            }


            CalibrationFail = false;
            uploadJsonSpecialFail = false;

            RFCSV_Cache = new Dictionary<string, string>();



               PowerToTelnetStartTime = new DateTime();
               PowerToTelnetEndTime = new DateTime();

            DataManager.ShareInstance.ClearData();
        }


        private bool GetLimit()
        {
            if (Global.STATIONNAME == "BURNIN" || Global.STATIONNAME.ToLower() == "revert")
            {

                return true;
            }

          

            bool rReturn = false;
            if (Online_Limit != null) {

              //  loggerInfo("不获取在线");
                return true;
            }
            try
            {
                if (Global.OnlineLimit.Trim()=="1")
                {
                //    loggerInfo("获取在线");
                    //loggerInfo("开始获取在线limit");
                    var cookies = new CookieContainer();
                    var client = GetClient(cookies, "luxshare", "bento");

                    string url = $@"http://luxshare:bento@10.90.116.15:8101/api/1/limits";
                    //string data = "{" + $"\"station_type\": \"{Global.STATIONNAME}\", \"model\": \"{DutMode.ToUpper()}\"" + "}";
                    var content = new FormUrlEncodedContent(new[]{
                               new KeyValuePair<string, string>("station_type", Global.STATIONNAME),
                               new KeyValuePair<string, string>("model", "snowbird"),
                            });
                    //StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = client.PostAsync(url, content).Result;

                    var response_content = result.Content.ReadAsByteArrayAsync().Result;
                    var responseStr = System.Text.Encoding.UTF8.GetString(response_content);

                    loggerDebug(responseStr);



                    responseStr = responseStr.Replace("\"limits\": {", "\"limits\": [").Trim().TrimEnd('}');

                    Regex r = new Regex("\"[\\S]*\"\\: {");
                    MatchCollection mc = r.Matches(responseStr);
                    for (int i = 0; i < mc.Count; i++)
                        responseStr = responseStr.Replace(mc[i].Value, "{");

                    int a = responseStr.LastIndexOf("}");
                    string responseStr1 = responseStr.Remove(a);
                    string responseStr2 = responseStr.Substring(a).Replace('}', ']');
                    responseStr = responseStr1 + responseStr2 + "}";

                    
                    if (result.IsSuccessStatusCode)
                    {
                        Online_Limit = JsonConvert.DeserializeObject<Limits>(responseStr);
                        if (Online_Limit.model.ToUpper() == DutMode.ToUpper() && Online_Limit.station_type.ToUpper() == Global.STATIONNAME.ToUpper())
                            rReturn = true;
                        else
                            loggerError($"online limit model or station_type is wrong!");
                    }
                    else
                        loggerError($"Get online limit fail! statusCode:{result.StatusCode}");
                }
                else
                {
                    rReturn = true;
                    loggerWarn("OnlineLimit is false,using program limit in excel script:");
                }
            }
            catch (Exception ex)
            {
                loggerFatal($"GetLimit Exception:{ex}");
            }
            return rReturn;
        }
        
        public void SetTestStatus(TestStatus testStatus)
        {
            try
            {
                switch (testStatus)
                {
                    case TestStatus.START:
                        SetTextBox(textBox1, false);
                        SetButton(this.bt_Status, "Testing", Color.Yellow);
                        SetButton(this.bt_errorCode, " ", Color.Yellow);

 


                        startTime = DateTime.Now;

                        var dateTimeNow = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                        bool same = false;
                        same = (startTime.ToShortDateString() == dateTimeNow.ToShortDateString());


                        if (startTime.ToLongDateString().Contains("2021") || same == false)
                        { //时间不对
                            loggerInfo("startTime error,update");

                            RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC

                            var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                            startTime = dateTime;
                            loggerInfo("get updated time:" + dateTime.ToLongTimeString());
                            SyDateTimeHelper.SetLocalDateTime(dateTime);

                        }


                        UBOOT_LED_W_ReMSG1 = "";
                        UBOOT_LED_W_ReMSG2 = "";
                        UBOOT_LED_W_ReMSG3 = "";
                        UBOOT_LED_W_ReMSG4 = "";
                        UBOOT_LED_W_ReMSG5 = "";

                        timer = new System.Threading.Timer(TimerCallBack, null, 0, 1000);
                        SetButtonPro(buttonBegin, Properties.Resources.pause);
                        SetButtonPro(buttonExit, Properties.Resources.stop);
                        singleStepTest = false;
                        startFlag = true;
                        pauseEvent.Set();
                        loggerDebug($"Start test...SN:{SN},Station:{Global.FIXTURENAME},DUTMode:{DutMode},TestMode:{Global.TESTMODE},Isdebug:{IsDebug.ToString()},FCT:{Global.FAIL_CONTINUE},onlineLimit:{Global.OnlineLimit},SoftVersion:{Global.Version}");
                        UpdateDetailViewClear();
                        break;

                    case TestStatus.FAIL:
                        logger.Info(">>>>>>>>>>>>>>:跳到fail");
                        if (testStatus == TestStatus.FAIL && SetIPflag)
                        {//SRF 测试失败设回默认IP重测
                            if (DUTCOMM != null) {
                                string recvStr = "";
                                DUTCOMM.SendCommand($"luxsetip {DUTIP} 255.255.255.0", ref recvStr, Global.PROMPT, 10);
                            }
                         
                        }
                        if (!Global.GoldenSN.Contains(SN) && Global.STATIONNAME == "MBFT")
                        {
                            if (CalibrationFail)
                            {
                                string recvStr = "";
                                DUTCOMM.SendCommand($"dd if=/dev/zero of=/dev/mmcblk0p9", ref recvStr, Global.PROMPT, 10);
                            }

                            if (Global.ClearCalWhenValidation && ValidationFail)
                            {
                                string recvStr = "";
                                DUTCOMM.SendCommand($"dd if=/dev/zero of=/dev/mmcblk0p9", ref recvStr, Global.PROMPT, 10);
                            }
                        }
                        if (Global.STATIONNAME == "RTT")
                        {
                            string recvStr = "";
                            DUTCOMM.SendCommand($"wifi_init 0", ref recvStr, Global.PROMPT, 10);
                        }
                       
                        Global.Total_Fail_Num++;

                      
                        
                        if (lbl_failCount.Text == "")
                        {
                            this.bt_Status.Font = new System.Drawing.Font("宋体",36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                            SetButton(this.bt_Status, "FAIL", Color.Red);
                        }
                        else {
                            this.bt_Status.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                            SetButton(this.bt_Status, "FAIL\r\n"+"("+lbl_failCount.Text+")", Color.Red);
                        }

                        SetLables(lbl_failCount, lbl_failCount.Text, Color.White,false);



                        SetButton(this.bt_errorCode, error_details_firstfail, Color.Red);
                        UpdateContinueFail(false);



                        break;

                    case TestStatus.PASS:
                        logger.Info(">>>>>>>>>>>>>>:跳到pass");
                        Global.Total_Pass_Num++;
                        SetButton(this.bt_Status, "PASS", Color.Green);
                        SetButton(this.bt_errorCode, sec.ToString(), Color.Green);
                        UpdateContinueFail(true);
                        break;

                    case TestStatus.ABORT:
                        Global.Total_Abort_Num++;
                        testThread.Abort();
                        testThread.Join(3000);
                        SetButton(this.bt_Status, "Abort", Color.Gray);
                        SetButton(this.bt_errorCode, error_details, Color.Gray);
                        saveTestResult();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
            finally
            {
                try
                {
                    switch (testStatus)
                    {
                        case TestStatus.START:
                            break;

                        case TestStatus.FAIL:
                        case TestStatus.PASS:
                        case TestStatus.ABORT:
                            // 关闭DUT通信
                            if (DUTCOMM != null)
                            {
                                DUTCOMM.Close();
                            }
                            // 无论测试pass/fail，都弹出治具
                            if (Global.FIXTUREFLAG == "1")
                            {



                                if ((!Global.PopFixture) && testStatus != TestStatus.PASS)
                                {
                                    loggerDebug("Don't pop-up fixture.");
                                    if (FixSerialPort != null)
                                    {
                                        // FixSerialPort.Close();
                                    }

                                }

                                else
                                {

                                    FixSerialPort.OpenCOM();
                                    var recvStr = "";
                                  
                                 
                                  //  FixSerialPort.SendCommandToFix("AT+VBUS_OFF%", ref recvStr, "OK", 5);

                                };

                            }

                            if (testStatus != TestStatus.PASS) {
                                if (Global.STATIONNAME == "MBFT" || Global.STATIONNAME == "SRF" || Global.STATIONNAME == "BURNIN")
                                {
                                    loggerInfo("失败了连续ping 10次");
                                    PingIP("192.168.1.1", 10);
                                }

                            }





                            if (Global.STATIONNAME != "BURNIN" && Global.STATIONNAME!="CCT" && Global.STATIONNAME != "SRF") {
                                logger.Info("关闭poe");
                                Bd.PoeConfigSetting(Global.POE_PORT, "disable");

                            }



                            //Thread thread = new Thread(() =>
                            //{
                            //    try
                            //    {
                            //        AddConsume();
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        loggerError("error3:" + ex.Message);
                            //    }

                            //});
                            //thread.IsBackground = true;
                            //thread.Start();

                            if (Global.STATIONNAME == "SRF")
                            {
                                KillProcessNoRes("QUTSStatusApp");
                                KillProcessNoRes("QCATestSuite"); //SRF
                                KillProcessNoRes("QPSTConfig"); //SRF
                                KillProcessNoRes("BTTestSuite");//SRF
                                KillProcessNoRes("QPSTServer");//SRF

                            }

                            if (Global.STATIONNAME == "MBFT") {

                                KillProcessNoRes("QUTSStatusApp");//MBFT

                                KillProcessNoRes("ATSuite"); //MBFT
                                KillProcessNoRes("BTTestSuiteRD");//MBFT
                                KillProcessNoRes("QPSTConfig"); //MBFT
                                KillProcessNoRes("QPSTServer");//MBFT
                            }



                            //// loggerInfo("把窗体放在最上层2");
                            // //窗体放在最上层
                            // var process = Process.GetProcessesByName("AutoTestSystem")[0];
                            // var handle = process.MainWindowHandle;
                            ////窗体放在最上层
                            // SwitchToThisWindow(handle, true);







                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    loggerFatal("SetTestStatus finally Exception:" + ex.ToString());
                    //throw;
                }
                finally
                {
                    try
                    {
                        if (testStatus != TestStatus.START)
                        {
                            timer.Dispose();
                            endTime = DateTime.Now;
                            loggerDebug($"Test end,ElapsedTime:{sec}s.");
                            if (testStatus == TestStatus.PASS || testStatus == TestStatus.FAIL) {
                                collectCsvResult();
                            }
                          

                            startFlag = false;
                            SetTextBox(textBox1);
                            UpdateContLable();
                            WriteCountNumToFile();
                            //cellLogPath = $@"{Global.LogPath}\{finalTestResult}_{SN}_{error_details_firstfail}_{DateTime.Now.ToString("hh-mm-ss")}.txt";


                            string path = $@"{Global.LogPath}\{WorkOrder}";
                            // loggerInfo("开始创建工单目录:" + path);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            // loggerInfo("创建工单目录完成:"+path);
                             
                                // {finalTestResult}_{SN}_{error_details_firstfail}_{DateTime.Now.ToString("hh-mm-ss")}


                            cellLogPath = $@"{Global.LogPath}\{WorkOrder}\{finalTestResult}_{SN}_{error_details_firstfail}_{DateTime.Now.ToString("hh-mm-ss")}.txt";
                            
                            loggerInfo("final path:" + cellLogPath);

                            if (Global.STATIONNAME == "BURNIN") {
                               
                                FileInfo fi = new FileInfo( Global.LogPath  + "\\" + Bd.TempCellLogPath);

                                var newPath = Global.LogPath + "\\" + $@"{finalTestResult}_{SN}_{error_details_firstfail}_{DateTime.Now.ToString("hh-mm-ss")}.txt";


                                loggerInfo(">>>>>>>>>>>>>From:" + Global.LogPath + "\\" + Bd.TempCellLogPath);


                                loggerInfo(">>>>>>>>>>>>>To:" + newPath);

                                fi.MoveTo(newPath);
                            }
                          


                            if (Global.STATIONNAME == "SRF" || Global.STATIONNAME == "MBFT") {

                                try
                                {


                                    if (!Directory.Exists(path + @"\pass"))
                                    {
                                        Directory.CreateDirectory(path + @"\pass");
                                    }
                                    if (!Directory.Exists(path + @"\fail"))
                                    {
                                        Directory.CreateDirectory(path + @"\fail");
                                    }

                                    string mode = "";
                                    if (IsDebug || Global.TESTMODE.ToLower() == "debug" || Global.TESTMODE.ToLower() == "fa")
                                    {
                                        mode = "debug";
                                    }
                                    else
                                    {
                                        mode = "production";
                                    }



                                    string successPath = $@"D:\SaveData\{mode}\pass";
                                    string failPath = $@"D:\SaveData\{mode}\fail";

                                    if (!Directory.Exists(successPath)) { 
                                       Directory.CreateDirectory (successPath);
                                    }
                                    if (!Directory.Exists(failPath))
                                    {
                                        Directory.CreateDirectory(failPath);
                                    }

                                    List<string> snList = new List<string>();
                                    List<string> logsArr = new List<string> { successPath, failPath };
                                    for (var i = 0; i < logsArr.Count; i++)
                                    {
                                        var LogParent = logsArr[i];
                                        DirectoryInfo directoryInfo = new DirectoryInfo(LogParent);
                                        FileInfo[] fileInfos = directoryInfo.GetFiles();
                                        foreach (FileInfo fileInfo in fileInfos)
                                        {
                                            if (fileInfo.Name.StartsWith(SN) && fileInfo.Name.EndsWith(".zip"))
                                            {

                                                loggerDebug("~~~~~~~~~~~~~~~~:" + fileInfo.FullName);
                                                snList.Add(fileInfo.FullName);
                                            }
                                        }
                                    }

                                    


                                    //找到时间戳最新的那一个，肯定就是刚测试的
                                    var newesFullPath = getNewestFile(snList);
                                    loggerInfo("<<<<<<<<<<<<<最新的路径:" + newesFullPath);
                                    var newesName = Path.GetFileName(newesFullPath);
                                    if (newesFullPath.Contains("pass"))
                                    {
                                        if (!File.Exists(path + @"\pass" + $@"\{newesName}"))
                                        {
                                            // path
                                            File.Copy(newesFullPath, path + @"\pass" + $@"\{newesName}");
                                            ZhuBoPath = path + @"\pass" + $@"\{newesName}";
                                            loggerInfo("copy final path:" + path + @"\pass" + $@"\{newesName}");
                                        }

                                    }
                                    else
                                    {
                                        if (!File.Exists(path + @"\fail" + $@"\{newesName}"))
                                        {
                                            File.Copy(newesFullPath, path + @"\fail" + $@"\{newesName}");
                                            ZhuBoPath = path + @"\fail" + $@"\{newesName}";
                                            loggerInfo("copy final path:" + path + @"\fail" + $@"\{newesName}");
                                        }
                                    }
                                }
                                catch (Exception ex) {
                                    loggerInfo("copy error log:"+ex.Message.ToString());
                                }
                            }
                         
                            
                            SaveRichText(false, RichTextBoxStreamType.UnicodePlainText, cellLogPath);
                           
                            if (finalTestResult == "FAIL")
                            {
                                SetButton(this.bt_errorCode, error_details_firstfail, Color.Red);
                            }

#if DEBUG
#else

                            Thread thread2 = new Thread(() =>
                            {
                                UploadLogToSftp();
                            });
                            thread2.IsBackground = true;
                            thread2.Start();
#endif



                            StartScanFlag = true;
                       
                            isFirstRun = false;


                            if (Global.AUTOTESTNOFIXUTRE_COUNT != "-1" && AUTOTESTNOFIXUTRE_COUNT>0)
                            {
                                AUTOTESTNOFIXUTRE_COUNT = AUTOTESTNOFIXUTRE_COUNT - 1;
                                logger.Info("等待1s");
                                Thread.Sleep(1000);

                                isTestAgain = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StartScanFlag = true;
                        isFirstRun = false;
                        //autoScanEvent.Set();
                        loggerFatal("错误:"+ex.ToString());
                    }
                }
            }
        }


        //D:\SaveData\production\pass\GGC21D0422540108_2022-06-20_12-22-29.zip

        static string getNewestFile(List<string>FullNameArray)
        {
            var ret = "";

            var dayList = new List<string>();
            var newTime = "";
            for (var i = 0; i < FullNameArray.Count;i++)
            {
                var fullName = FullNameArray[i];
                //提取时间
                var fileName = Path.GetFileName(fullName);
                fileName = Regex.Replace(fileName,".zip","");
                var fArr = fileName.Split('_');
                var date = "";
                var time = "";
                if (fArr.Length >= 3) { 
                   date = fArr[1];
                   time = fArr[2].Replace('-',':');
                }
                var dateTime = date + " " + time;

                 
                if (newTime == "")
                {
                    newTime = dateTime;
                    ret = fullName;
                }
                else
                {
                    if (getMaxDate(newTime, dateTime) == false)
                    {
                        newTime = dateTime;
                        ret = fullName;
                    }
                }

            }
            return ret;
             

        }

        static bool getMaxDate(string time1, string time2)
        {
            DateTime dt1 = Convert.ToDateTime(time1);
            DateTime dt2 = Convert.ToDateTime(time2);

            if (DateTime.Compare(dt1, dt2) > 0)//大于  
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private void UploadLogToSftp()
        {


            if (Global.STATIONNAME == "CCT") {
                return;
            }

            try
            {
                loggerInfo("开始上传SFTP");
                var CSVFilePath = $@"{Global.LOGFOLDER}\CsvData\{DateTime.Now.AddDays(0).ToString("yyyy-MM-dd")}_{Global.STATIONNO}.csv";

                if (File.Exists(CSVFilePath))
                {

                    using (SFTPHelper sFTP = new SFTPHelper(Global.LOGSERVER, Global.LOGSERVERUser, Global.LOGSERVERPwd, "22"))
                    {
                        sFTP.Connect();
                        string csvFileName = Path.GetFileName(CSVFilePath);
                        var day = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                        loggerInfo("csv Path:" + CSVFilePath);
                        try
                        {
                            sFTP.CreateDir($"/{Global.STATIONNAME}");
                            sFTP.CreateDir($"/{Global.STATIONNAME}/{Global.STATIONNO}");

                            sFTP.CreateDir($"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}");



                            sFTP.CreateDir($"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}/{day}");
                            sFTP.CreateDir($"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}/{day}/{WorkOrder}");
                            sFTP.CreateDir($"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}/{day}/{WorkOrder}/{finalTestResult}");

                            logger.Info("上传测试程序csv");
                            sFTP.PutNoClose(CSVFilePath, $"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}/{day}/{WorkOrder}/{finalTestResult}/{csvFileName}");

                            logger.Info("上传测试程序log");
                            var logName = Path.GetFileName(cellLogPath);

                            sFTP.PutNoClose(cellLogPath, $"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}/{day}/{WorkOrder}/{finalTestResult}/{logName}");

                            //驻波log
                            if (Global.STATIONNAME == "MBFT" || Global.STATIONNAME == "SRF")
                            {


                                if (ZhuBoPath != null && ZhuBoPath != "")
                                {
                                    logger.Info("上传驻波文件");
                                    sFTP.PutNoClose(ZhuBoPath, $"/{Global.STATIONNAME}/{Global.STATIONNO}/{Global.TESTMODE.ToLower()}/{day}/{WorkOrder}/{finalTestResult}/{Path.GetFileName(ZhuBoPath)}");
                                }

                            }



                        }
                        catch (IOException)
                        {

                        }

                        sFTP.Disconnect();
                    }

                }
                else {

                    loggerError("没有找到csv:"+ CSVFilePath);
                }
            }
            catch (Exception ex)
            {
                loggerInfo("SFTP异常:" + ex.Message);
            }

        }

        
        /// <summary>
        /// 根据测试结果更新连续Fail计数
        /// </summary>
        /// <param name="testResult">测试结果</param>
        public void UpdateContinueFail(bool testResult)
        {
            if (IsDebug || Global.TESTMODE.ToLower() == "debug" || Global.TESTMODE.ToLower() == "fa")
            {
                return;
            }
            if (testResult)
                Global.ContinueFailNum = 0;
            else
                Global.ContinueFailNum++;
            iniConfig.Writeini("CountNum", "ContinueFailNum", Global.ContinueFailNum.ToString());
            lb_ContinuousFailNum.InvokeOnToolStripItem(lb_ContinuousFailNum => lb_ContinuousFailNum.Text = Global.ContinueFailNum.ToString());
        }

        /// <summary>
        /// 把Fail PASS数量写入ini配置文件
        /// </summary>
        private void WriteCountNumToFile()
        {
            try
            {
                iniConfig.Writeini("CountNum", "Total_Pass_Num", Global.Total_Pass_Num.ToString());
                iniConfig.Writeini("CountNum", "Total_Fail_Num", Global.Total_Fail_Num.ToString());
                iniConfig.Writeini("CountNum", "Total_Abort_Num", Global.Total_Abort_Num.ToString());
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        #endregion 测试前期处理函数

        /// <summary>
        /// 运行线程
        /// </summary>
        private void TestThread()
        {
            try
            {
                while (true)
                {
                    // 如果开始标志为假则不运行程序
                    if (startFlag)
                    {
                        TX_RX_RETRY:
                        // 如果是第一个测试项，则记录sequence开始测试时间
                        if (itemsNo == 0)
                        {

                            if (seqNo == 0)
                            {
                                //重置时间，防止starttime出错
                                RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC
                                var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                                loggerInfo("get updated time>>>:" + dateTime.ToLongTimeString());
                                SyDateTimeHelper.SetLocalDateTime2(dateTime);
                            }

 
                            sequences[seqNo].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            loggerDebug($"---------Start testSuite:{sequences[seqNo].SeqName}  starttime:{ sequences[seqNo].start_time}----------");
                        }
                        // 当前测试用例项
                        Items tempItem = sequences[seqNo].SeqItems[itemsNo];
                        // 用于保存item测试结果，生成JSON格式文件
                        phase_items phase_item = new phase_items();
                        // 把测试结果置真,所有的测试步的结果,如果有一个假的测试项结果就会为假
                        //Sequences[seqNo].TestResult = true;
                        tempItem.tResult = true;
                        sequences[seqNo].IsTest = sequences[seqNo].IsTest | tempItem.isTest;
                        // 根据机型执行不同的测试用例
                        if (!String.IsNullOrEmpty(tempItem.Mode) && !tempItem.Mode.ToLower().Contains(DutMode.ToLower()))
                        {
                            tempItem.isTest = false;
                            SetTreeViewSeqColor(Color.Gray);
                        }

                        // 根据测试步骤的IF条件决定下一步骤不执行，
                        // if (tempItem.IfElse.ToLower() == "else")
                        if (tempItem.IfElse.ToLower() == "else" || tempItem.IfElse.ToLower()=="elseif")
                        {
                            // if为true不执行else部分的,false执行。
                            tempItem.isTest = !IfCond;
                            if (!tempItem.isTest)
                            {
                                SetTreeViewSeqColor(Color.Gray);
                            }
                        }

                        bool fail_continue = Global.FAIL_CONTINUE == "1" ? true : false;

                        if (tempItem.isTest || singleStepTest)
                        {
                            SetTreeViewSeqColor(Color.Yellow);
                            if (pauseEvent.WaitOne())
                            {
                                // 每次测试前清除上次测试记录
                                tempItem.Clear();
                                loggerDebug($"Start:{tempItem.ItemName},Keyword:{tempItem.TestKeyword},Retry {tempItem.RetryTimes},Timeout {tempItem.TimeOut}s,SubStr:{tempItem.SubStr1}-{tempItem.SubStr2},MesVer:{tempItem.MES_var},FTC:{tempItem.FTC}");
                                //纠正一下时间：
                                if (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Contains("0001-01-01")) {
                                    //重置时间，防止starttime出错
                                    RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC
                                    var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
                                    loggerInfo("get updated time>>>:" + dateTime.ToLongTimeString());
                                    SyDateTimeHelper.SetLocalDateTime2(dateTime);
                                }



                                if (string.IsNullOrEmpty(tempItem.Json) && startTimeJsonFlag)
                                {
                                    startTimeJson = DateTime.Now;
                                    startTimeJsonFlag = false;
                                }
                                else if (string.IsNullOrEmpty(tempItem.Json) && !startTimeJsonFlag)
                                {
                                }
                                else if (!string.IsNullOrEmpty(tempItem.Json) && !startTimeJsonFlag)
                                {
                                    tempItem.start_time_json = startTimeJson;
                                    startTimeJsonFlag = true;
                                }
                                else if (!string.IsNullOrEmpty(tempItem.Json) && startTimeJsonFlag)
                                {
                                    tempItem.start_time_json = DateTime.Now;
                                }

                                tempItem.start_time = DateTime.Now;



                                tempItem.start_time_json = DateTime.Now;



                                int retryTimes = 0;
                                int retryInterval = 0; //retry之间间隔多少秒
                                if (!string.IsNullOrEmpty(tempItem.RetryTimes))
                                {
                                    if (!tempItem.RetryTimes.Contains("-"))
                                    {
                                        retryTimes = int.Parse(tempItem.RetryTimes);
                                    }
                                    else {
                                        //提取出retry之间的间隔时间
                                        var arr = tempItem.RetryTimes.Split('-');
                                        if (arr.Length >= 2) {
                                            retryTimes = int.Parse(arr[0]);
                                            retryInterval = int.Parse(arr[1]);
                                        }
                                    }
                                    
                                }
                                // 运行测试步骤
                                bool result = false;

                                //为了应对SFT 网口通信断开的问题，把retry==0 的 改为retry==1
                               
                                
                                








                                //这个地方P1阶段要屏蔽





                                
                                //if ( (Global.STATIONNAME == "SFT" || Global.STATIONNAME == "MBFT" 
                                //    || Global.STATIONNAME == "RTT" || Global.STATIONNAME == "SRF")
                                    
                                    
                                //    && retryTimes == 0 && tempItem.TestKeyword == "default"
                                    
                                //    )
                                //{
                                //    loggerInfo(Global.STATIONNAME +" net problem，add retryTimes=1");
                                //    retryTimes = 1;
                                //}

















                                for (retry = retryTimes; retry > -1; retry--)
                                {
                                    if (test.StepTest(TestPhase, tempItem, retry, phase_item,ref retry))
                                    {
                                        result = true;
                                        break;
                                    }
                                    else
                                    {
                                        inPutValue = "";
                                        if (retry == 0)
                                            result = false;
                                    }
                                    if (retry > 0) {
                                        if (retryInterval > 0) {
                                            loggerInfo("retry Interval:" + retryInterval + " seconds");
                                            Thread.Sleep(retryInterval * 1000);
                                        }
                                    }

                                }

                                // 根据测试结果显示测试步为绿色或者红色
                                SetTreeViewSeqColor(result ? Color.Green : Color.Red);

                                // 让测试步骤bypass
                                if ((tempItem.ByPassFail.ToLower() == "p" || tempItem.ByPassFail.ToLower() == "1") && !result)
                                {
                                    loggerWarn($"Let this step:{tempItem.ItemName} bypass.");
                                    result = true;
                                    SetTreeViewSeqColor(Color.GreenYellow);
                                }
                                else if ((tempItem.ByPassFail.ToLower() == "f" || tempItem.ByPassFail.ToLower() == "0") && result)
                                {
                                    // 测试失败后的showlog步骤，show完log后测试结果设置为fail，并定义error_code和error_details
                                    error_code = tempItem.ErrorCode.Split(new string[] { "\n" }, 0)[0].Split(':')[0].Trim();
                                    error_details = tempItem.ErrorCode.Split(new string[] { "\n" }, 0)[0].Split(':')[1].Trim();
                                    loggerError($"Let this step:{tempItem.ItemName} byfail.Set error_code:{error_code},error_details:{error_details}");
                                    result = false;
                                    SetTreeViewSeqColor(Color.MediumVioletRed);
                                }

                                // if条件语句
                                switch (tempItem.IfElse.ToLower())
                                {
                                    case "if": 
                                        {
                                            SetTreeViewSeqColor(result ? Color.Green : Color.Pink);
                                            // 设置if执行反馈结果，下面的测试步骤根据这个结果决定是否执行。
                                            IfCond = result;
                                            if (!result) {
                                                loggerInfo($"if statement FAIL needs to continue, setting the test result to true");
                                            }
                                               
                                            result = true;
                                            break;
                                        }


                                    case "elseif": {

                                            SetTreeViewSeqColor(result ? Color.Green : Color.Pink);
                                            // 设置if执行反馈结果，下面的测试步骤根据这个结果决定是否执行。
                                            IfCond = result;
                                            if (!result)
                                            {
                                                loggerInfo($"elseif statement FAIL needs to continue, setting the test result to true");
                                            }

                                            result = true;

                                            break;
                                        }
                                        
                                    case "else":
                                        break;

                                  

                                    case "&&if":
                                        {
                                            IfCond &= result;
                                        }
                                        break;
                                    case "&if":
                                        {
                                            SetTreeViewSeqColor(result ? Color.Green : Color.Pink);
                                            IfCond &= result;
                                            if (!result)
                                                loggerInfo($"if& statement FAIL needs to continue, setting the test result to true");
                                            result = true;
                                        }
                                        break;
                                    case "||if":
                                        {
                                            IfCond |= result;
                                        }
                                        break;
                                    default:
                                        IfCond = true;
                                        break;
                                }

                                // 记录测试结果
                                tempItem.tResult &= result;
                                sequences[seqNo].TestResult &= tempItem.tResult;
                                stationStatus &= sequences[seqNo].TestResult;
                                // 如果是单步模式不往下运行,只运行当前测试项
                                if (singleStepTest)
                                {
                                    startFlag = false;
                                    SetTextBox(textBox1);
                                    continue;
                                }
                                else
                                {
                                    stationObj.status = stationStatus.ToString();
                                }
                                // 记录第一次fail的错误码和详细
                                if (!tempItem.tResult)
                                {
                                    ItemFailCount++;
                                    if (ItemFailCount == 1 && string.IsNullOrEmpty(error_code_firstfail))
                                    {
                                        mesPhases.FIRST_FAIL = sequences[seqNo].SeqName;
                                        error_code_firstfail = error_code;
                                        error_details_firstfail = error_details;
                                    }
                                }
                                

                                if (!result)
                                {
                                    if (tempItem.FTC == "n")
                                    {
                                        fail_continue = false;
                                    }

                                    if (tempItem.FTC == "y")
                                    {
                                        fail_continue = true;
                                    }
                                    else
                                    {
                                        if (Global.FAIL_CONTINUE != "1")
                                        {
                                            fail_continue = false;
                                        }
                                    }


                                }
                                // 循环测试处理
                                if (!isCycle)
                                {
                                    // 测试fail停止测试,生成结果or fail继续测试。
                                    if (!tempItem.tResult && !fail_continue)
                                    {
                                        //if (Global.STATIONNAME == "SFT")
                                        //{
                                        //    if (tempItem.ItemName == "ETH0_SPEED_TX_Special"
                                        //        ||
                                        //       tempItem.ItemName == "ETH0_SPEED_RX_Special"
                                        //       ||
                                        //       tempItem.ItemName == "ETH1_SPEED_TX_Special"
                                        //       ||
                                        //       tempItem.ItemName == "ETH1_SPEED_RX_Special"
                                        //        )
                                        //    {

                                        //        SFT_TXRX_RETRY--;
                                        //        if (SFT_TXRX_RETRY >= 0)
                                        //        {

                                        //            FixSerialPort.OpenCOM();
                                        //            var recvStr = "";
                                        //            FixSerialPort.SendCommandToFix("AT+PORTEJECT%", ref recvStr, "OK", 10);
                                        //            Sleep(500);
                                        //            FixSerialPort.SendCommandToFix("AT+PORTINSERT%", ref recvStr, "OK", 10);
                                        //            var rr = "";
                                        //            if (DUTCOMM != null)
                                        //            {
                                        //                if (DUTCOMM.SendCommand("", ref rr, "luxshare SW Version :", 120))
                                        //                {

                                        //                    if (DUTCOMM.SendCommand(" \r\n", ref rr, "root@OpenWrt:/#", 1))
                                        //                    {

                                        //                    }
                                        //                }
                                        //                DUTCOMM.SendCommand("modprobe qca_nss_netlink", ref rr, "root@OpenWrt:/#", 10);
                                        //                DUTCOMM.SendCommand("ifconfig br-lan down && brctl delbr br-lan && ifconfig eth0 192.168.1.101 up && ifconfig eth1 192.168.0.1 up", ref rr, "root@OpenWrt:/#", 10);

                                        //            }
                                        //            sequences = ObjectCopier.Clone<List<Sequence>>(Global.Sequences);
                                        //            seqNo = 8;
                                        //            itemsNo = 0;
                                        //            ResetData();

                                        //            Thread.Sleep(2000);
                                        //            goto TX_RX_RETRY;

                                        //        }

                                        //    }
                                        //}


                                        // if (Global.STATIONNAME == "MBFT" || Global.STATIONNAME == "SRF")
                                       //else  if (Global.STATIONNAME == "MBFT")
                                       // {
                                       //     if (
                                       //         tempItem.ErrorCode == "1.4.1.1: Equipment.DUT.Initiate"
                                       //          || 
                                       //         tempItem.ErrorCode.Contains("1.4.3.8:")
                                       //         ) //这种类型的错误要重新开始测试
                                       //       {
                                       //         if (
                                       //             Global.STATIONNAME == "SRF"
                                       //             ||
                                       //             (
                                       //               Global.STATIONNAME == "MBFT" &&
                                       //               (
                                       //                 tempItem.ItemName == "Calibration_0_5180"
                                       //                 ||
                                       //                 tempItem.ItemName == "Calibration_1_5180"
                                       //                 ||
                                       //                 tempItem.ItemName == "Calibration_0_5320"
                                       //                  ||
                                       //                 tempItem.ItemName == "Calibration_1_5320"
                                       //                  ||
                                       //                 tempItem.ItemName == "Calibration_0_5500"
                                       //                  ||
                                       //                 tempItem.ItemName == "Calibration_1_5500"
                                       //                  ||
                                       //                 tempItem.ItemName == "Calibration_0_5745"
                                       //                  ||
                                       //                 tempItem.ItemName == "Calibration_1_5745"
                                       //                  ||
                                       //                 tempItem.ItemName == "WIFI_5G_TX_CAL"
                                       //                  ||
                                       //                 tempItem.ItemName == "WIFI_5G_RX_CAL"
                                       //                  ||
                                       //                 tempItem.ItemName == "WIFI_2G_RX_CAL"
                                       //             ||
                                       //                 tempItem.ItemName == "WIFI_RX_PER_F2412_BW20_MCS0_P-91_C0"
                                       //             ||
                                       //                 tempItem.ItemName == "WIFI_RX_PER_F2412_BW20_MCS0_P-91_C1"
                                       //             ||
                                       //                 tempItem.ItemName == "WIFI_RX_PER_F2437_BW20_MCS0_P-91_C0"
                                       //             ||
                                       //                 tempItem.ItemName == "WIFI_RX_PER_F2437_BW20_MCS0_P-91_C1"

                                       //                ||            
                                       //                 tempItem.ItemName == "BLE_TX_POWER_F2402"
                                                       
                                       //               )
                                                    

                                       //             )
                                       //             ) { 
                                               

                                       //         SRF_POP_RETRY--;
                                       //         if (SRF_POP_RETRY >= 0)
                                       //         {


                                       //             sequences = ObjectCopier.Clone<List<Sequence>>(Global.Sequences);
                                       //             seqNo = 0;
                                       //             itemsNo = 0;


                                       //             ResetData();


                                       //             if (Global.STATIONNAME == "MBFT" || Global.STATIONNAME == "SRF") {
                                       //                 //插拔重启
                                       //                 var recvStr = "";
                                                         

                                       //                 FixSerialPort.SendCommandToFix("AT+PORTEJECT%", ref recvStr, "OK", 10);

                                       //                 Sleep(500);
                                       //                 FixSerialPort.SendCommandToFix("AT+PRESSUP%", ref recvStr, "OK", 10);
                                       //                 Sleep(500);
                                       //                 FixSerialPort.SendCommandToFix("AT+PRESSDOWN%", ref recvStr, "OK", 10);
                                       //                 Sleep(500);
                                       //                 FixSerialPort.SendCommandToFix("AT+PORTINSERT%", ref recvStr, "OK", 10);
                                       //             }
                                                  


                                       //             if (Global.STATIONNAME == "SRF")
                                       //             {
                                       //                 KillProcessNoResForce("QCATestSuite"); //SRF
                                       //                 KillProcessNoResForce("QPSTConfig"); //SRF
                                       //                 KillProcessNoResForce("BTTestSuite");//SRF
                                       //             }

                                       //             if (Global.STATIONNAME == "MBFT")
                                       //             {
                                       //                 KillProcessNoResForce("ATSuite"); //MBFT
                                       //                 KillProcessNoResForce("BTTestSuiteRD");//MBFT
                                       //                 KillProcessNoResForce("QPSTConfig"); //MBFT
                                       //             }

                                       //             Thread.Sleep(2000);
                                       //             goto TX_RX_RETRY;
                                       //         }
                                       //         }
                                       //     }

                                       // }
                                      
                                        
                                        
                                        
                                        //else if (Global.STATIONNAME == "RTT")
                                        //{

                                        //    if (tempItem.ItemName == "5GWiFiPing50" || tempItem.ItemName == "2GWiFiPing2" || tempItem.ItemName == "WiFiPing5")
                                        //    {


                                        //        SRF_POP_RETRY--;
                                        //        if (SRF_POP_RETRY >= 0)
                                        //        {
                                        //            sequences = ObjectCopier.Clone<List<Sequence>>(Global.Sequences);
                                        //            seqNo = 0;
                                        //            itemsNo = 0;


                                        //            ResetData();

                                        //            //插拔重启
                                        //            var recvStr = "";
                                        //            loggerInfo("----------------->begin pop RJ45 fixture");

                                        //            FixSerialPort.SendCommandToFix("AT+PORTEJECT%", ref recvStr, "OK", 10);

                                        //            Sleep(500);

                                        //            loggerInfo("----------------->begin push RJ45 fixture");

                                        //            FixSerialPort.SendCommandToFix("AT+PORTINSERT%", ref recvStr, "OK", 10);

                                        //            Thread.Sleep(2000);
                                        //            goto TX_RX_RETRY;



                                        //        }

                                        //    }
                                        //}


                                        ////UBOOT fail 之后 发送LED，保存到CSV
                                        //if (tempItem.ItemName == "ENTER_UBOOT") {

                                        //    FixSerialPort.OpenCOM();
                                        //    for (var i = 0; i < 5; i++) {
                                                
                                        //        string recvStr = "";
                                        //        loggerDebug("ENTER_UBOOT Fail,Send LED_W cmd");
                                        //        if (FixSerialPort.SendCommandToFix("AT+LEDSTATUS%", ref recvStr, "%END", 5))
                                        //        {
                                        //            recvStr = Regex.Replace(recvStr, "\r", "");
                                        //            recvStr = Regex.Replace(recvStr, "\n", "");
                                        //            if (i == 0)
                                        //            {
                                        //                UBOOT_LED_W_ReMSG1 = recvStr;
                                        //            }
                                        //            else if (i == 1) {
                                        //                UBOOT_LED_W_ReMSG2 = recvStr;
                                        //            }
                                        //            else if (i == 2)
                                        //            {
                                        //                UBOOT_LED_W_ReMSG3 = recvStr;
                                        //            }
                                        //            else if (i == 3)
                                        //            {
                                        //                UBOOT_LED_W_ReMSG4 = recvStr;
                                        //            }
                                        //            else if (i == 4)
                                        //            {
                                        //                UBOOT_LED_W_ReMSG5 = recvStr;
                                        //            }
                                                    
                                        //        }
                                        //        Thread.Sleep(1000);
                                        //    }
                                            

 
                                        //}

                                        //if(Global.STATIONNAME=="SFT" && tempItem.ItemName== "ETH0_SPEED_TX")
                                        //{
                                        //    //查看userspace进程
                                        //    loggerInfo("查看userspace进程:");
                                        //    RunDosCmd("tasklist | findstr /i \"userspace_speedtest.exe\""); 

                                        //    string rr = "";
                                        //    loggerInfo("开始连续ping 10次");
                                        //    if (DUTCOMM.SendCommand("ping -c 10 -I eth0 192.168.1.10", ref rr, "root@OpenWrt:/#", 30)) { 
                                            
                                        //    }

                                        //}
                                        //else if (Global.STATIONNAME == "SFT" && tempItem.ItemName == "ETH1_SPEED_TX")
                                        //{
                                        //    loggerInfo("查看userspace进程:");
                                        //    RunDosCmd("tasklist | findstr /i \"userspace_speedtest.exe\"");
                                        //    string rr = "";
                                        //    loggerInfo("开始连续ping 10次");
                                        //    if (DUTCOMM.SendCommand("ping -c 10 -I eth1 192.168.0.10", ref rr, "root@OpenWrt:/#", 30))
                                        //    {

                                        //    }

                                        //}


                                        // sequence结束时间.
                                        sequences[seqNo].finish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        DateTime.TryParse(sequences[seqNo].start_time, out DateTime datetime);
                                        string elapsedTime = String.Format("{0,0:F0}", Convert.ToDouble((DateTime.Now - datetime).TotalSeconds));
                                        // 把seq测试结果保存到test_phase变量中.
                                        TestPhase.Copy(sequences[seqNo], this);
                                        // 加入station实例,记录测试结果 用于序列化Json文件.
                                        //if (DutMode.ToLower() == "leaf" && tempItem.ItemName == "ReadBTZFwVersion") { }
                                        //else { Station.tests.Add(TestPhase); }
                                        //if (tempItem.Json != null && tempItem.Json.ToLower() == "y")
                                        //{ stationObj.tests.Add(phase_item); loggerDebug($"{tempItem.ItemName} add test item to station"); }
                                        // 把testPhase初始化
                                        TestPhase = new test_phases();
                                        AddStationResult(false, error_code_firstfail, error_details_firstfail);
                                        if (sequences[seqNo].IsTest)
                                        {
                                            // 上传测试时间到EMS
                                            SetVerReflection(mesPhases, sequences[seqNo].SeqName + "_Time", elapsedTime);
                                            SetVerReflection(mesPhases, sequences[seqNo].SeqName, sequences[seqNo].TestResult.ToString().ToUpper());
                                        }
                                        //MES错误不上传
                                        if (
                                               (tempItem.ErrorCode.Contains("ShopFloor.WrongStation"))
                                            || (tempItem.ErrorCode.Contains("ShopFloor.ABXRetestError"))
                                            || (tempItem.ErrorCode.Contains("1.4.1.4:") &&( (Global.STATIONNAME=="MBFT") || (Global.STATIONNAME == "SRF")))
                                            //|| (tempItem.ErrorCode.Contains("1.4.3.6:") && ((Global.STATIONNAME == "MBFT") || (Global.STATIONNAME == "SRF")))
                                            || (tempItem.ErrorCode.Contains("1.11.5:"))
                                            || (tempItem.ErrorCode.Contains("3.1.0:"))

                                            )
                                        {

                                            if ((tempItem.ErrorCode.Contains("3.1.0:"))) {
                                                MessageBox.Show("COM口打开失败,请检查\nPlease call TE check com port");
                                            }



                                        }
                                        else {

                                            bool rReturn = false;
                                            //针对于MBLT uboot fail需要做下判断fail 后 ping dut 192.168.1.101
                                            //如果ping pass这个SN fail 不上传MES和JSON
                                            if (tempItem.ItemName == "ENTER_UBOOT_Spe" && Global.STATIONNAME == "MBLT") {
                                                loggerInfo("ENTER_UBOOT fail,开始ping");
                                                rReturn = PingIP("192.168.1.1",10);
                                                if (rReturn) {
                                                    loggerInfo("开机没问题,不上传");
                                                }

                                            }

                                            if (rReturn == false) {

                                                loggerInfo("再次校准时间");
                                                SyDateTimeHelper.SetLocalDateTime2(SyDateTimeHelper.GetNetDateTime(preDateTime.AddMilliseconds(0)));

                                                UploadJsonToClient();

                                                if (uploadJsonSpecialFail == false)
                                                {
                                                    PostAsyncJsonToMes();
                                                }
                                            }



                                           
                                        }
                                       

                                        saveTestResult();
                                        SetTestStatus(TestStatus.FAIL);
                                    }
                                }
                                else
                                {
                                    // 循环测试，fail不停止
                                    if (tempItem.tResult)
                                    { PassNumOfCycleTest++; }
                                    else
                                    { FailNumOfCycleTest++; }
                                    lb_FailNum.InvokeOnToolStripItem(lb_FailNum => lb_FailNum.Text = Global.Total_Fail_Num.ToString());
                                    lb_loopTestStatistics.InvokeOnToolStripItem(lb_loopTestStatistics => lb_loopTestStatistics.Text = $"  LoopTest: Pass {PassNumOfCycleTest}/Fail {FailNumOfCycleTest}");
                                }
                            }
                        }

                        // 测试项序号+1,继续下一个item.
                        itemsNo++;
                        // 测试结果处理.
                        if (tempItem.tResult || fail_continue)
                        {
                            // 如果测试item是所在Seq中最后一个or测试失败.
                            if (itemsNo >= sequences[seqNo].SeqItems.Count)
                            {
                                // sequence结束时间.
                                sequences[seqNo].finish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                DateTime.TryParse(sequences[seqNo].start_time, out DateTime datetime);
                                string elapsedTime = String.Format("{0,0:F0}", Convert.ToDouble((DateTime.Now - datetime).TotalSeconds));
                                // 把seq测试结果保存到test_phase变量中.
                                TestPhase.Copy(sequences[seqNo], this);
                              
                                // 把testPhase初始化
                                TestPhase = new test_phases();
                                if (sequences[seqNo].IsTest)
                                {
                                    // 上传测试时间到EMS
                                    SetVerReflection(mesPhases, sequences[seqNo].SeqName + "_Time", elapsedTime);
                                    SetVerReflection(mesPhases, sequences[seqNo].SeqName, sequences[seqNo].TestResult.ToString().ToUpper());
                                }
                                itemsNo = 0;
                                // 测试结束标志为真
                                sequences[seqNo].IsTestFinished = true;
                                // 测试用例号+1
                                seqNo++;
                                // 如果是最后一个测试用例则上传测试结果,结束测试
                                if (seqNo >= sequences.Count)
                                {
                                    seqNo = 0;

                                    // 循环测试不生成结果
                                    if (!isCycle)
                                    {
                                        AddStationResult(stationStatus, error_code_firstfail, error_details_firstfail);
                                        //上传结果到MES失败,此处用&而不用&&。


 

                                        //上传结果到MES失败,此处用&而不用&&。
                                        bool uploadjsonResult = UploadJsonToClient();
                                        if (uploadjsonResult)//客户系统上传成功
                                        {
                                            if (PostAsyncJsonToMes() & stationStatus)
                                            {
                                                finalTestResult = stationStatus.ToString().ToUpper() == "TRUE" ? "PASS" : "FAIL";
                                                //collectCsvResult();
                                                SetTestStatus(TestStatus.PASS);
                                            }
                                            else
                                            {
                                                // collectCsvResult();
                                                SetTestStatus(TestStatus.FAIL);
                                            }
                                        }
                                        else
                                        { //客户系统上传失败
                                            if (!uploadJsonSpecialFail) //不是特殊失败类型
                                            {
                                                PostAsyncJsonToMes();

                                                SetTestStatus(TestStatus.FAIL);


                                            }
                                            else
                                            { //特殊失败类型，不上传MES
                                                SetTestStatus(TestStatus.FAIL);
                                            }

                                        }






                                        StartScanFlag = true;
                                        saveTestResult();




                                         
                                    }
                                    Thread.Sleep(10);
                                }
                                else
                                {
                                    // 初始化要测试的测试用例参数,重复测试的时候,这些值需要清除
                                    sequences[seqNo].Clear();
                                    inPutValue = "";
                                }
                            }
                        }
                    }
                    else
                        Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException ex)
            {
                //abort线程忽略报错
                //abort线程忽略报错
                loggerWarn(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                loggerFatal("TestThread() Exception:" + ex.ToString());
                SetTestStatus(TestStatus.ABORT);
                //throw;
            }
            finally
            {
                testThread.Abort();
            }
        }


        #region 窗体响应处理函数


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //KillProcess("Scan_Barcode_C03");
            if (scanThread != null)
                scanThread.Abort();
            if (testThread != null)
                testThread.Abort();
        }

        private void LogButton_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip((Button)sender, "open test log");
        }

        /// <summary>
        /// 这样处理界面显示时不卡
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Visible = true;
            this.Opacity = 1;
        }

        private void LogButton_Click(object sender, EventArgs e)
        {
            string path = Global.LOGFOLDER;
            System.Diagnostics.Process.Start(path);
        }

        /// <summary>
        /// 当节点被选择时进入事件
        /// </summary>
        private void TreeViewSeq_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                // 如果正在测试,则不做任何动作
                if (startFlag)
                    return;

                if (e.Node.Nodes.Count > 0)
                {
                    e.Node.SelectedImageIndex = 1;
                    // 如果是测试用例,有子节点,展开节点,显示第一个子节点的内容
                    if (!e.Node.IsExpanded)
                        e.Node.Expand();
                    seqNo = (int)e.Node.Tag;
                    itemsNo = 0;
                }
                else
                {
                    // 如果是测试项,没有子节点,直接显示测试项里的测试步
                    seqNo = (int)e.Node.Parent.Tag;
                    itemsNo = (int)e.Node.Tag;
                    e.Node.SelectedImageIndex = 0;
                }
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        /// <summary>
        /// 当节点的checkbox发生改变时进入
        /// </summary>
        private void TreeViewSeq_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (startFlag)
                    return;
                //this.treeViewSeq.SelectedNode = e.Node;
                if (e.Node.Parent == null)
                {
                    int pNo = (int)e.Node.Tag;

                    sequences[pNo].IsTest = e.Node.Checked;
                    for (int i = 0; i < e.Node.Nodes.Count; i++)
                    {
                        e.Node.Nodes[i].Checked = e.Node.Checked;
                    }
                }
                else
                {
                    int pNo = (int)e.Node.Parent.Tag;
                    int iNo = (int)e.Node.Tag;
                    sequences[pNo].SeqItems[iNo].isTest = e.Node.Checked;
                }
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        /// <summary>
        /// 定时更新测试时间
        /// </summary>
        private void TimerCallBack(object stateInfo)
        {
            try
            {
                if (startFlag)
                {
                    sec++;
                    SetButton(bt_errorCode, sec.ToString());
                    toolStripTestTime.InvokeOnToolStripItem(toolStripTestTime => toolStripTestTime.Text = sec.ToString() + "s");
                    if (isCycle)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                //throw;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (startFlag)
                {
                    sec++;
                    SetButton(bt_errorCode, sec.ToString());
                    toolStripTestTime.InvokeOnToolStripItem(toolStripTestTime => toolStripTestTime.Text = sec.ToString() + "s");
                    if (isCycle)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                //throw;
            }
        }

        private void TreeViewSeq_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Node.Parent != null)
                {
                    int len_suite = richTextBox1.Text.IndexOf($"Start testSuite:{e.Node.Parent.Text.Substring(e.Node.Parent.Text.IndexOf(".") + 1).Trim()}");
                    int len = -1;
                    if (len_suite > 0)
                        len = richTextBox1.Text.IndexOf(e.Node.Text.Substring(e.Node.Text.IndexOf(")") + 1).Trim(), len_suite);
                    if (len > 0)
                    {
                        richTextBox1.Select(len, 0);
                        richTextBox1.ScrollToCaret();
                    }
                }
            }

        

            if (startFlag == false)
            {
                this.ConsumbleMenuItem.Visible = true;
            }
            else
            {
                this.ConsumbleMenuItem.Visible = false;
            }
            if (startFlag || IsDebug)
            {

                this.OneStepTestMenuItem.Visible = true;
                this.CycleToolStripMenuItem.Visible = true;
                this.全不选ToolStripMenuItem.Visible = true;
                this.全选ToolStripMenuItem.Visible = true;
            }
            else
            {
                //  this.ConsumbleMenuItem.Visible = true;
                this.OneStepTestMenuItem.Visible = false;
                this.CycleToolStripMenuItem.Visible = false;
                this.全不选ToolStripMenuItem.Visible = false;
                this.全选ToolStripMenuItem.Visible = false;
            }



            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                treeViewSeq.SelectedNode = e.Node;
                if (e.Node.Nodes.Count <= 0)
                {
                    //!显示右键菜单
                    contextMenuStripRightKey.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        /// <summary>
        /// 单步测试
        /// </summary>
        private void OneStepTestMenuItem_Click(object sender, EventArgs e)
        {
            sec = 0;
            startTime = DateTime.Now;
            singleStepTest = true;
            sequences[seqNo].Clear();
            textBox1.Enabled = false;
            startFlag = true;
            pauseEvent.Set();
        }

        /// <summary>
        /// 查看耗材
        /// </summary>
        private void SeeConsumeMenuItem_Click(object sender, EventArgs e)
        {
            ConsumalForm consumalForm = new ConsumalForm();
            consumalForm.StationName = Global.STATIONNAME;
            consumalForm.TextHandler = (string value) =>
            {

            };
            consumalForm.Show();

        }




        /// <summary>
        /// 循环测试
        /// </summary>
        private void CycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

               

                //// 创建测试主线程
                if (!testThread.IsAlive)
                {
                    testThread = new Thread(new ThreadStart(TestThread));
                    testThread.Start();
                }
                lb_loopTestStatistics.Text = "";
                UpdateDetailViewClear();
                sec = 0;
                timer = new System.Threading.Timer(TimerCallBack, null, 0, 1000);
                textBox1.Enabled = false;
                singleStepTest = false;
                isCycle = true;
                startTime = DateTime.Now;
                seqNo = 0;
                itemsNo = 0;
                sequences[seqNo].Clear();
                startFlag = true;
                pauseEvent.Set();
                buttonBegin.Enabled = false;
                lb_loopTestStatistics.Visible = Enabled;
            }
            catch
            {
                // ignored
            }
        }

        private void 全不选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTreeView(false);
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTreeView(true);
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            Global.LoadSequnces();
            if (bt_debug.CheckState == CheckState.Checked)
            {
                IsDebug = true;
                //SN = "DEBUG001";
                this.treeViewSeq.CheckBoxes = true;
                ShowTreeView();
                this.buttonBegin.Enabled = true;
                this.buttonBegin.Visible = true;
                this.buttonExit.Enabled = true;
                this.buttonExit.Visible = true;
            }
            else
            {
                IsDebug = false;
                this.treeViewSeq.CheckBoxes = false;
                ShowTreeView();
                this.buttonBegin.Enabled = false;
                this.buttonBegin.Visible = false;
                this.buttonExit.Enabled = false;
                this.buttonExit.Visible = false;
            }
        }

        private void ButtonBegin_Click(object sender, EventArgs e)
        {
            if (startFlag)
            {
                //!当startflag信号为真是进入,说明正在测试
                if (!pauseFlag)
                {
                    //!按下暂停键,pause信号为真
                    pauseFlag = true;
                    SetButtonPro(buttonBegin, Properties.Resources.start);
                    pauseEvent.Reset();
                }
                else
                {
                    //!暂停状态下,按下开始键,!发送信号,pause结束
                    pauseEvent.Set();
                    SetButtonPro(buttonBegin, Properties.Resources.pause);
                    //!pause信号值假
                    pauseFlag = false;
                }
            }
            else
            {
                //!startflag信号为假的时候进入,说明未开始测试
                TextBox1_KeyDown(null, null);
            }
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            if (startFlag)
            {
                //!当startflag为真时,结束测试,显示测试结果
                pauseEvent.Reset();
                saveTestResult();
                sequences[seqNo].IsTestFinished = true;
                startFlag = false;
                isCycle = false;
                testThread.Abort();
                testThread.Join(3000);
                timer.Dispose();
                testThread = new Thread(TestThread);
                testThread.Start();
                buttonBegin.Enabled = true;
                SetTextBox(textBox1);
            }
            //else  //!当startflag信号为假时,退出测试
            //this.Close();
        }

        private void ContinuousFailReset_Click(object sender, EventArgs e)
        {
            var resetForm = new Reset();
            resetForm.Show();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            this.groupBox3.Location = new Point(this.groupBox2.Location.X + this.groupBox2.Size.Width + 30, this.groupBox2.Location.Y);
            this.groupBox1.Location = new Point(this.groupBox3.Location.X + this.groupBox3.Size.Width + 30, this.groupBox3.Location.Y);
            this.bt_debug.Location = new Point(this.groupBox1.Location.X + this.groupBox1.Size.Width + 30, this.groupBox1.Location.Y * 9);
            this.lb_IPaddress.Location = new Point(this.bt_debug.Location.X + this.bt_debug.Size.Width + 30, this.bt_debug.Location.Y);
        }

        #endregion 窗体响应处理函数


        #region 测试功能函数

        public void AddStationResult(bool _result, string _errorcode, string _errordetails)
        {
            stationObj.finish_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            stationObj.status = _result ? "passed" : "failed";
            stationObj.error_code = _errorcode;
            stationObj.error_details = _errordetails;
            stationObj.CopyToMES(mesPhases);
        }

        private bool UploadJsonToClient()
        {

           

            DateTime startUpload = DateTime.Now;
            bool result = false;
            string JsonPath = $@"{Global.LogPath}\Json\{SN}_{DateTime.Now:HHmmss}.json";
            try
            {

                if (stationObj.mode == "fa")
                {
                    stationObj.mode = "debug";
                }
                try
                {
                    //遍历
                    for (var i = 0; i < stationObj.tests.Count; i++)
                    {
                        var test = stationObj.tests[i];
                        if (test.start_time == null || test.start_time == "" || test.start_time == "0001-01-01 00:00:00")
                        {
                            test.start_time = test.finish_time;
                        }
                        if (test.start_time == null || test.start_time == "" || test.start_time == "0001-01-01 00:00:00")
                        {
                            test.start_time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (test.finish_time == null || test.finish_time == "" || test.finish_time == "0001-01-01 00:00:00")
                        {
                            test.finish_time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }


                        DateTime dateTime1 = DateTime.Parse(test.start_time);
                        DateTime dateTime2 = DateTime.Parse(test.finish_time);

                        if (DateTime.Compare(dateTime1, dateTime2) > 0)
                        {
                            test.start_time = test.finish_time;
                        }



                    }
                }
                catch (Exception ex)
                {

                    SaveLog("时间1 转换失败");
                }
                try
                {
                    //开始测试时间要小于 第一个测试项开始时间
                    if (stationObj.tests.Count > 0)
                    {
                        DateTime dateTime1 = DateTime.Parse(stationObj.start_time);
                        DateTime dateTime2 = DateTime.Parse(stationObj.tests[0].start_time);

                        if (DateTime.Compare(dateTime1, dateTime2) > 0)
                        {
                            stationObj.start_time = stationObj.tests[0].start_time;
                        }
                    }
                }
                catch
                {
                    SaveLog("时间2 转换失败");

                }




                if (!JsonSerializer(stationObj, out string JsonStr, JsonPath))
                {
                    loggerError("Serialize station Json info error!!!...");
                    return false;
                }

                cellLogPath = $@"{System.Environment.CurrentDirectory}\Data\{SN}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt";
                SaveRichText(false, RichTextBoxStreamType.UnicodePlainText, cellLogPath);

                Thread.Sleep(1000);
                //var fileInfo = new FileInfo(cellLogPath);
                //if (Math.Ceiling(fileInfo.Length / 1024.0) < 100)
                //    Thread.Sleep(3000);
                result = UploadJson(JsonPath);
            }
            catch (Exception ex)
            {
                result = false;
                loggerFatal(ex.ToString());
            }

            //上传失败
            if (result == false)
            {
                //RTT DHCP模式取消,特殊处理
                if (Global.STATIONNAME == "RTT")
                {
                    loggerInfo("StationName:RTT uploadfail ,Cancel DHCP Mode...");
                    var STAComm = new Telnet(new TelnetInfo { _Address = "192.168.1.1" });
                    string revStr = "";
                    if (STAComm.Open("root@OpenWrt:/#") && STAComm.SendCommand("fw_setenv bootcmd bootipq", ref revStr, "root@OpenWrt:/#", 10))
                    {
                        loggerInfo("StationName:RTT  DHCP Mode cancel success!");
                    }
                    else
                    {
                        loggerInfo("StationName:RTT  DHCP Mode cancel fail!");
                    }

                }

                //// toandv re-set sample IP to defaul
                //if (Global.STATIONNAME == "SRF")
                //{
                //    var rr = "";
                //    DUTCOMM.SendCommand("luxsetip 192.168.1.101 255.255.255.0", ref rr, "OK!", 10);
                //}
            }


            var elapsedTime = $"{Convert.ToDouble((DateTime.Now - startUpload).TotalSeconds),0:F0}";
            UpdateDetailView(SN, "UploadJson", null, null, null, null, elapsedTime, startUpload.ToString(), result.ToString());
            loggerDebug($"UploadJsonToClient {(result ? "PASS" : "FAIL")}!! ElapsedTime:{elapsedTime}.");
            mesPhases.JSON_UPLOAD_Time = elapsedTime;

            if (result == false)
            {
                loggerDebug("uploadjson fail, setipflag = true");
                SetIPflag = true;
            }


            return result;
        }

        public bool UploadJson(string JsonFilePath)
        {
            if (Global.STATIONNAME == "BURNIN" || Global.STATIONNAME.ToLower() == "revert")
            {

                return true;
            }



            try
            {
                string cmd = $@"python {System.Environment.CurrentDirectory}\Config\{Global.PySCRIPT} -s {Global.STATIONNAME} -f {JsonFilePath} -l {cellLogPath}";
              
                loggerDebug("执行命令:"+cmd);

                if (Global.JSON == "0")
                {
                    loggerDebug($"JSON={Global.JSON},don't upload Json to API");
                    return true;
                }


                // string responds = RunDosCmd(cmd, out string errors, 30000);
                string responds = RunDosCmd(cmd, out string errors, 180000);

               
                if (responds.Contains("Result:200"))
                {
                    mesPhases.JSON_UPLOAD = "TRUE";
                    return true;
                }
                else
                {

                    if (responds.Contains("time and timezone"))
                    {
                        uploadJsonSpecialFail = true; //不上传
                    }
                    //else if (responds.Contains("Result:400") && !responds.Contains("Incorrect limits"))
                    //{
                    //    uploadJsonSpecialFail = true;//不上传
                    //}


                    loggerError("Json-info upload to client fail:" + errors);
                    mesPhases.JSON_UPLOAD = "FALSE";
                    if (stationObj.status == "passed")
                    {
                        mesPhases.error_code = "JSON_UPLOAD";
                        mesPhases.status = "failed";
                        error_code = "JSON_UPLOAD"; 
                        error_details = "JSON_UPLOAD";
                        error_code_firstfail = "JSON_UPLOAD";
                        if (responds.Contains("No valid session found from cloud admin API"))
                        {   
                            error_details_firstfail = "JSON_UPLOAD\r\n" + "(" + "No valid session found from cloud admin API" + ")";
                        }
                        else
                        {
                            error_details_firstfail = "JSON_UPLOAD";
                        }
                       
                        
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                loggerFatal($"json_upload_Exception:{ex.ToString()}");
                return false;
            }
        }

        private bool UploadToCustomServerByBurnin(bool result)
        {
            if (Global.TESTMODE.ToLower() == "debug") {
                logger.Info("debug 模式 skip");
                return true;
            
            }

            if (result == false)
            {
                logger.Info("测试结果fail，不上传burnin服务器");
                return true;
            }

            //var SN = "GGC21D0A33823203";

            var dict = new Dictionary<string, string>();
            dict.Add("sn", SN);
            dict.Add("stationtype", "snowbird");
            dict.Add("stationname", "burnin");

            JsonSerializer(dict, out string currentMes);

            var url = $"http://172.23.241.211:9000/stationprocess/passstation";
            var client = new HttpClient();
            StringContent content = new StringContent(currentMes, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = client.PostAsync(url, content).GetAwaiter().GetResult();
            string responseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            logger.Info(responseBody);
            if (responseBody.Contains("成功"))
            {
                return true;
            }
            return false;


        }
        /// <summary>
        /// 上传json文件to Mes.
        /// </summary>
        /// <returns></returns>
        private bool PostAsyncJsonToMes()
        {
            if (Global.STATIONNAME == "CancelDHCP" || Global.STATIONNAME.ToLower() == "revert")
            {
                return true;
            }


            if (Global.STATIONNAME == "BURNIN")
            {

                            
                return UploadToCustomServerByBurnin(stationStatus);


            }



            DateTime startUpload = DateTime.Now;
            bool result = false;
            try
            {
                //增加扫码时间
                TimeSpan timeSpan = Global.time1.Subtract(Global.time2).Duration();
                var timeSpan2 = (timeSpan.TotalMilliseconds / 1000);

                var timeSpanStr = Math.Round(timeSpan2, 2).ToString() + " s";
                mesPhases.AT_SCAN_TIME = timeSpanStr;

                mesPhases.AT_SCAN_SATRT_TIME = Global.time1.ToLongTimeString();
                mesPhases.AT_SCAN_END_TIME = Global.time2.ToLongTimeString();

                mesPhases.TotalTimeExceptScanAndPopFixutre = $"{sec}s";
                if (SN != null) {
                    mesPhases.SN = SN;
                }
                if (MesMac != null) {
                    mesPhases.MAC = MesMac;
                }


               



             

                MesPhases mesPhasesUpload = ForeachClassFields<MesPhases>(mesPhases, "TRUE");

                if (Global.TESTMODE.ToLower() == "fa") {
                    if (mesPhasesUpload.status == "PASS") {
                        mesPhasesUpload.status = "passed";
                    }
                    else if (mesPhasesUpload.status == "FAIL") {
                        mesPhasesUpload.status = "failed";
                    }
                       
                
                }

                //转换成字典对象：
                var firstDict = ConvertToDictionary(mesPhasesUpload);
                //从jsonupload 取出第二个字典
                var secDict = new Dictionary<string, string>();
                if (stationObj != null)
                {
                    var testItems = stationObj.tests;
                    for (var i = 0; i < testItems.Count; i++)
                    {
                        if (testItems[i].test_value.ToLower() == "true")
                        {
                            continue;
                        }

                        secDict.Add(testItems[i].test_name, testItems[i].test_value);


                    }

                }
                //合并两个字典
                var dictionaries = new List<Dictionary<string, string>> { firstDict, secDict };



                var finalDict = dictionaries
                    .SelectMany(dict => dict)
                    .ToLookup(pair => pair.Key, pair => pair.Value)
                    .ToDictionary(group => group.Key, group => group.First());




      
                JsonSerializer(finalDict, out string currentMes);
              

                if (Global.TESTMODE.ToLower() == "fa")
                {
                
                    mesUrl = $"http://10.90.108.172:8086/api/1/FA/serial/{SN}/station/{Global.FIXTURENAME}/result";
                }
                else
                {

                    mesUrl = $"http://{Global.MESIP}:{Global.MESPORT}/api/2/serial/{SN}/station/{Global.FIXTURENAME}/info";
                }
                
                
                loggerDebug($"mesUrl:{mesUrl}");
                if (Global.TESTMODE.ToLower() == "debug" || IsDebug)
                {
                    loggerDebug($"TESTMODE=debug or IsDebug={IsDebug},don't upload result to MES");
                    return true;
                }
                else
                {
                    if (mesPhases.FIRST_FAIL == "VerifySFIS")
                        return true;
                }
                loggerDebug("Start to upload MES info...");
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                StringContent content = new StringContent(currentMes, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = client.PostAsync(mesUrl, content).GetAwaiter().GetResult();
                if (httpResponse.IsSuccessStatusCode || httpResponse.StatusCode == HttpStatusCode.BadRequest)
                {


                    mesPhases.MES_UPLOAD = "TRUE";
                    result = true;
                }
                else
                {
                    if (stationObj.status == "passed")
                    {
                        error_code = "UploadJsonToMES";
                        error_details = "UploadJsonToMES";
                        error_code_firstfail = "UploadJsonToMES";
                        error_details_firstfail = "UploadJsonToMES";
                    }
                    mesPhases.MES_UPLOAD = "FALSE";
                    loggerError($"MES-info Upload Fail.Response code:{httpResponse.StatusCode}");
                }
                loggerInfo(">>>>>>>>>>>>开始请求");
                string responseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                loggerDebug("MES responseBody:" + responseBody);
               
                
                
                if (responseBody.ToLower().Contains("first test fail"))
                    SetLables(lbl_failCount, "1st fail", Color.White);
                else if (responseBody.ToLower().Contains("second test fail"))
                    SetLables(lbl_failCount, "2nd fail", Color.White);
                else if (responseBody.ToLower().Contains("ng"))
                    SetLables(lbl_failCount, "final fail", Color.White);
                //SetDefaultIP = true;
                else if (responseBody.ToLower().Contains("next"))
                {
                    SetLables(lbl_failCount, responseBody.ToString(), Color.Red);
                }
                else if (responseBody.ToLower().Contains("未将对象引用设置到对象的实例"))
                {
                    SetLables(lbl_failCount, responseBody.ToString(), Color.Red);
                    mesPhases.MES_UPLOAD = "FALSE";
                    result = false;


                }
                SetIPflag = true;


            }
            catch (Exception ex)
            {

                if (ex.ToString().Contains("TaskCanceledException"))
                {
                    //超时了，再次走一下路由检查，看看是否过站
                    if (Global.STATIONNAME == "SRF")
                    {
                        loggerDebug("~~~~~~~~~~~begin checkroute");
                        //检查路由:
                        //  if (mescheckroute.checkroute(SN, Global.FIXTURENAME, out string mesMsg) && mesMsg.Contains("OK"))
                        //  {
                        //没过站
                        // loggerDebug("not pass station,setIPflag=true,MesMsg="+ mesMsg);
                        // SetIPflag = true;
                        //  }
                        //  else {
                        // loggerDebug("pass station,setIPflag=false");
                        // SetIPflag = false;
                        // }
                        SetIPflag = false;
                    }


                }
                else {
                    SetIPflag = true;
                }
             


                loggerFatal("UploadJsonToMESException:" + ex.ToString());

              
            }
            string elapsedTime = $"{Convert.ToDouble((DateTime.Now - startUpload).TotalSeconds),0:F0}";
            UpdateDetailView(SN, "UploadMes", null, null, null, null, elapsedTime, startUpload.ToString(), result.ToString());
            loggerDebug($"PostAsyncJsonToMES {(result ? "PASS" : "FAIL")}!! ElapsedTime:{elapsedTime}.");


            if (result == false) {

                
                    //RTT DHCP模式取消,特殊处理
                    if (Global.STATIONNAME == "RTT")
                    {
                        loggerInfo("StationName:RTT uploadfail ,Cancel DHCP Mode...");
                        var STAComm = new Telnet(new TelnetInfo { _Address = "192.168.1.1" });
                        string revStr = "";
                        if (STAComm.Open("root@OpenWrt:/#") && STAComm.SendCommand("fw_setenv bootcmd bootipq", ref revStr, "root@OpenWrt:/#", 10))
                        {
                            loggerInfo("StationName:RTT  DHCP Mode cancel success!");
                        }
                        else
                        {
                            loggerInfo("StationName:RTT  DHCP Mode cancel fail!");
                        }

                    }
               


            }




            return result;
        }

        /// <summary>
        /// 生成测试report,收集测试结果
        /// </summary>
        private void SaveTestResultToCsv()
        {
            try
            {
                SetButtonPro(buttonBegin, Properties.Resources.start);
                SetButtonPro(buttonExit, Properties.Resources.close);
                string reportPath = Environment.CurrentDirectory + @"\Output\result.csv";
                CreatCSVFile(reportPath, colHeader);
                DataGridViewToCSV(dataGridViewDetail, true, reportPath);
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        /// <summary>
        /// 收集测试数据到CSV文件.
        /// </summary>
        //private void CollectResultToCsv()
        //{
        //    List<string[]> testDataList = new List<String[]>();
        //    //CSVFilePath = $@"{Global.LOGFOLDER}\CsvData\{DateTime.Now.ToString("yyyy-MM-dd--HH")}-00-00_{Global.STATIONNO}.csv";
        //    string csvColumnPath = $@"{Environment.CurrentDirectory}\Config\CSV_COLUMN.txt";
        //    //CSVFilePath = $@"{Environment.CurrentDirectory}\Output\{DateTime.Now.ToString("yyyy-MM-dd")}_{Global.STATIONNO}.csv";
        //    CSVFilePath = $@"{Global.LOGFOLDER}\CsvData\{DateTime.Now.ToString("yyyy-MM-dd")}_{Global.STATIONNO}.csv";
        //    try
        //    {
        //        SetButtonPro(buttonBegin, Properties.Resources.start);
        //        SetButtonPro(buttonExit, Properties.Resources.close);

        //        ArrayListCsvHeader.InsertRange(0, new string[] {
        //         "UBOOT_LED_W_ReMSG1","UBOOT_LED_W_ReMSG2","UBOOT_LED_W_ReMSG3","UBOOT_LED_W_ReMSG4","UBOOT_LED_W_ReMSG5","FIXTURE_CLOSE","DUT_PING" , "AT-ScanTimeSpan","DEVICE_TYPE", "STATION_TYPE", "FACILITY_ID", "LINE_ID", "FIXTURE_ID", "DUT_POSITION", "SN", "FW_VERSION",  "HW_REVISION", "SW_VERSION",  "START_TIME", "TEST_DURATION","DUT_TEST_RESULT", "FIRST_FAIL", "ERROR_CODE", "TIME_ZONE", "TEST_DEBUG", "JSON_UPLOAD", "MES_UPLOAD",
        //            });
        //        File.Delete(csvColumnPath);
        //        using (StreamWriter sw = new StreamWriter(csvColumnPath, true, Encoding.Default))
        //        {
        //            foreach (var item in ArrayListCsvHeader)
        //                sw.Write(item + "\t");
        //        }
        //        bool updateColumn = finalTestResult.ToUpper() == "PASS" && !IsDebug;
        //        CreatCSVFile(CSVFilePath, csvColumnPath, updateColumn);


        //        TimeSpan timeSpan = Global.time1.Subtract(Global.time2).Duration();
        //        var timeSpanStr = timeSpan.TotalMilliseconds + " ms";
        //        // loggerInfo(">>>>>>>>>>>>>>治具按下-获取SN时间间隔:" + timeSpan.TotalMilliseconds + " ms");
        //        UBOOT_LED_W_ReMSG1 = Regex.Replace(UBOOT_LED_W_ReMSG1, " ", "");
        //        UBOOT_LED_W_ReMSG2 = Regex.Replace(UBOOT_LED_W_ReMSG2, " ", "");
        //        UBOOT_LED_W_ReMSG3 = Regex.Replace(UBOOT_LED_W_ReMSG3, " ", "");
        //        UBOOT_LED_W_ReMSG4 = Regex.Replace(UBOOT_LED_W_ReMSG4, " ", "");
        //        UBOOT_LED_W_ReMSG5 = Regex.Replace(UBOOT_LED_W_ReMSG5, " ", "");


        //        UBOOT_LED_W_ReMSG1 = Regex.Replace(UBOOT_LED_W_ReMSG1, "\r", "");
        //        UBOOT_LED_W_ReMSG2 = Regex.Replace(UBOOT_LED_W_ReMSG2, "\r", "");
        //        UBOOT_LED_W_ReMSG3 = Regex.Replace(UBOOT_LED_W_ReMSG3, "\r", "");
        //        UBOOT_LED_W_ReMSG4 = Regex.Replace(UBOOT_LED_W_ReMSG4, "\r", "");
        //        UBOOT_LED_W_ReMSG5 = Regex.Replace(UBOOT_LED_W_ReMSG5, "\r", "");

        //        UBOOT_LED_W_ReMSG1 = Regex.Replace(UBOOT_LED_W_ReMSG1, "\n", "");
        //        UBOOT_LED_W_ReMSG2 = Regex.Replace(UBOOT_LED_W_ReMSG2, "\n", "");
        //        UBOOT_LED_W_ReMSG3 = Regex.Replace(UBOOT_LED_W_ReMSG3, "\n", "");
        //        UBOOT_LED_W_ReMSG4 = Regex.Replace(UBOOT_LED_W_ReMSG4, "\n", "");
        //        UBOOT_LED_W_ReMSG5 = Regex.Replace(UBOOT_LED_W_ReMSG5, "\n", "");

        //        UBOOT_LED_W_ReMSG1 = Regex.Replace(UBOOT_LED_W_ReMSG1, ",", "$");
        //        UBOOT_LED_W_ReMSG2 = Regex.Replace(UBOOT_LED_W_ReMSG2, ",", "$");
        //        UBOOT_LED_W_ReMSG3 = Regex.Replace(UBOOT_LED_W_ReMSG3, ",", "$");
        //        UBOOT_LED_W_ReMSG4 = Regex.Replace(UBOOT_LED_W_ReMSG4, ",", "$");
        //        UBOOT_LED_W_ReMSG5 = Regex.Replace(UBOOT_LED_W_ReMSG5, ",", "$");

        //        ArrayListCsv.InsertRange(0, new string[] {
        //         UBOOT_LED_W_ReMSG1,UBOOT_LED_W_ReMSG2,UBOOT_LED_W_ReMSG3,UBOOT_LED_W_ReMSG4,UBOOT_LED_W_ReMSG5 ,MainForm.f1.FIXTURE_TIME,MainForm.f1.DUT_PING_TIME   , timeSpanStr,  DutMode, Global.STATIONNAME, "Luxshare", WorkOrder, Global.FIXTURENAME, "1", SN, mesPhases.FW_VERSION, mesPhases.HW_REVISION, mesPhases.test_software_version, startTime.ToString("yyyy/MM/dd HH:mm:ss"), sec.ToString(), finalTestResult, mesPhases.FIRST_FAIL, error_details_firstfail, "UTC", (Global.TESTMODE == "debug" || Global.TESTMODE.ToLower() == "fa") ? "1" : "0", mesPhases.JSON_UPLOAD, mesPhases.MES_UPLOAD,
        //            });
        //        testDataList.Add(ArrayListCsv.ToArray());
        //        WriteCSV(CSVFilePath, true, testDataList);
        //        testDataList.Clear();
        //        loggerDebug($"Export test results to {CSVFilePath} succeed");
        //    }
        //    catch (Exception ex)
        //    {
        //        loggerFatal($"Export test results to CSVFilePath error!:{ ex.Message} ");
        //    }
        //}

        private void CollectResultToCsv()
        {
            List<string[]> testDataList = new List<String[]>();
            CSVFilePath = $@"{Global.LOGFOLDER}\CsvData\{DateTime.Now.ToString("yyyy-MM-dd")}_{Global.STATIONNO}.csv";
            string csvColumnPath = $@"{Environment.CurrentDirectory}\Config\CSV_COLUMN.txt";
            string csvColumnPathMAX = $@"{Environment.CurrentDirectory}\Config\CSV_COLUMNMAX.txt";
            string csvColumnPathMIN = $@"{Environment.CurrentDirectory}\Config\CSV_COLUMNMIN.txt";
            string csvColumnPathUNIT = $@"{Environment.CurrentDirectory}\Config\CSV_COLUMNUNIT.txt";
            //CSVFilePath = $@"{Environment.CurrentDirectory}\Output\{DateTime.Now.ToString("yyyy-MM-dd")}_{Global.STATIONNO}.csv";

            try
            {
                SetButtonPro(buttonBegin, Properties.Resources.start);
                SetButtonPro(buttonExit, Properties.Resources.close);
                var headerArr = new string[] {
                      "AT-ScanTimeSpan",  "Product", "Station Name", "Site", "LINE_ID", "Station ID", "DUT_POSITION", "SerialNumber", "FW_VERSION",  "HW_REVISION", "SW_VERSION",  "StartTime", "EndTime", "TEST_DURATION","Test Pass/Fail Status", "FIRST_FAIL", "Fail Message", "TIME_ZONE", "TEST_DEBUG", "JSON_UPLOAD", "MES_UPLOAD",
                    };
                ArrayListCsvHeader.InsertRange(0, headerArr);


                var headerEmptyMAX = new List<string>();
                for (var i = 0; i < headerArr.Length; i++)
                {
                    if (i == 0)
                    {
                        headerEmptyMAX.Add("Upper Limit ----->");
                    }
                    else
                        headerEmptyMAX.Add(" ");
                }
                var headerEmptyMIN = new List<string>();
                for (var i = 0; i < headerArr.Length; i++)
                {
                    if (i == 0)
                    {
                        headerEmptyMIN.Add("Lower Limit ----->");
                    }
                    else
                        headerEmptyMIN.Add(" ");
                }

                var headerEmptyUNIT = new List<string>();
                for (var i = 0; i < headerArr.Length; i++)
                {
                    if (i == 0)
                    {
                        headerEmptyUNIT.Add("Measurement Unit ----->");
                    }
                    else
                        headerEmptyUNIT.Add(" ");
                }




                ArrayListCsvHeaderMAX.InsertRange(0, headerEmptyMAX.ToArray());
                ArrayListCsvHeaderMIN.InsertRange(0, headerEmptyMIN.ToArray());
                ArrayListCsvHeaderUNIT.InsertRange(0, headerEmptyUNIT.ToArray());


                File.Delete(csvColumnPath);
                File.Delete(csvColumnPathMAX);
                File.Delete(csvColumnPathMIN);
                File.Delete(csvColumnPathUNIT);

                using (StreamWriter sw = new StreamWriter(csvColumnPath, true, Encoding.Default))
                {
                    foreach (var item in ArrayListCsvHeader)
                        sw.Write(item + "\t");
                }
                using (StreamWriter sw = new StreamWriter(csvColumnPathMAX, true, Encoding.Default))
                {
                    foreach (var item in ArrayListCsvHeaderMAX)
                        sw.Write(item + "\t");
                }
                using (StreamWriter sw = new StreamWriter(csvColumnPathMIN, true, Encoding.Default))
                {
                    foreach (var item in ArrayListCsvHeaderMIN)
                        sw.Write(item + "\t");
                }
                using (StreamWriter sw = new StreamWriter(csvColumnPathUNIT, true, Encoding.Default))
                {
                    foreach (var item in ArrayListCsvHeaderUNIT)
                        sw.Write(item + "\t");
                }




                bool updateColumn = finalTestResult.ToUpper() == "PASS" && !IsDebug;



                CreatCSVFileWithMINAndMAX(CSVFilePath, csvColumnPath, csvColumnPathMAX, csvColumnPathMIN, csvColumnPathUNIT, updateColumn);





                TimeSpan timeSpan = Global.time1.Subtract(Global.time2).Duration();
                var timeSpan2 = (timeSpan.TotalMilliseconds / 1000);

                var timeSpanStr = Math.Round(timeSpan2, 2).ToString() + " s";
                //    mesPhases.AT_SCAN_TIME = timeSpanStr;


                ArrayListCsv.InsertRange(0, new string[] {
                       timeSpanStr, DutMode, Global.STATIONNAME, "Luxshare", WorkOrder, Global.FIXTURENAME, "1", SN, mesPhases.FW_VERSION, mesPhases.HW_REVISION, mesPhases.test_software_version, startTime.ToString("yyyy/MM/dd HH:mm:ss"),startTime.AddSeconds(sec).ToString("yyyy/MM/dd HH:mm:ss"), sec.ToString(), finalTestResult, mesPhases.FIRST_FAIL, error_details_firstfail, "UTC", (Global.TESTMODE == "debug" || Global.TESTMODE == "fa") ? "1" : "0", mesPhases.JSON_UPLOAD, mesPhases.MES_UPLOAD,
                    });
                testDataList.Add(ArrayListCsv.ToArray());



                WriteCSV(CSVFilePath, true, testDataList);



                testDataList.Clear();
                loggerDebug($"Export test results to {CSVFilePath} succeed");
            }
            catch (Exception ex)
            {
                loggerFatal($"Export test results to CSVFilePath error!:{ ex.Message} ");
            }
        }




        public static void CreatCSVFileWithMINAndMAX(string csvFilePath, string columnFilePath, string columnPathMAX, string columnPathMIN, string columnPathUNIT, bool updateColumn = false)
        {
            string[] colHeader;
            string[] colHeaderMAX;
            string[] colHeaderMIN;
            string[] colHeaderUNIT;
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

                    using (var sr = new StreamReader(columnPathMAX))
                    {
                        colHeaderMAX = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    using (var sr = new StreamReader(columnPathMIN))
                    {
                        colHeaderMIN = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    using (var sr = new StreamReader(columnPathUNIT))
                    {
                        colHeaderUNIT = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }


                    for (var i = 0; i < colHeaderMAX.Length; i++)
                    {

                        var one = colHeaderMAX[i];
                        if (Regex.Replace(one, " ", "") == "")
                        {
                            colHeaderMAX[i] = "";
                        }
                    }

                    for (var i = 0; i < colHeaderMIN.Length; i++)
                    {

                        var one = colHeaderMIN[i];
                        if (Regex.Replace(one, " ", "") == "")
                        {
                            colHeaderMIN[i] = "";
                        }
                    }

                    for (var i = 0; i < colHeaderUNIT.Length; i++)
                    {

                        var one = colHeaderUNIT[i];
                        if (Regex.Replace(one, " ", "") == "")
                        {
                            colHeaderUNIT[i] = "";
                        }
                    }


                    rowList.Add(colHeader);
                    rowList.Add(colHeaderMAX);
                    rowList.Add(colHeaderMIN);
                    rowList.Add(colHeaderUNIT);

                    WriteCSV(csvFilePath, false, rowList);
                }
            }
            else
            {
                if (updateColumn)
                {
                    var rowList = new List<string[]>();
                    using (var sr = new StreamReader(columnFilePath))
                    {
                        colHeader = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    using (var sr = new StreamReader(columnPathMAX))
                    {
                        colHeaderMAX = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    using (var sr = new StreamReader(columnPathMIN))
                    {
                        colHeaderMIN = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    using (var sr = new StreamReader(columnPathUNIT))
                    {
                        colHeaderUNIT = sr.ReadToEnd().Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    var ls = ReadCSV(csvFilePath);
                    if (!Enumerable.SequenceEqual(ls[0], colHeader))
                    {
                        //ls[0] = colHeader;
                        //WriteCSV(csvFilePath, false, ls);
                        rowList.Add(colHeader);
                        rowList.Add(colHeaderMAX);
                        rowList.Add(colHeaderMIN);
                        rowList.Add(colHeaderUNIT);
                        WriteCSV(csvFilePath, false, rowList);


                    }
                    else
                    {
                        return;
                    }
                }
            }
        }



        #endregion 测试功能函数















        #region 控件更新处理函数


        private delegate void treeViewSeqDelegate(Color colors);//定义更新log委托

        /// <summary>
        /// 更新测试项颜色
        /// </summary>
        public void SetTreeViewSeqColor(Color colors)
        {
            if (this.treeViewSeq.InvokeRequired)
            {
                treeViewSeqDelegate d = new treeViewSeqDelegate(SetTreeViewSeqColor); //实体化委托
                this.Invoke(d, new object[] { colors });
            }
            else
            {
                if (this.treeViewSeq.IsDisposed) return;
                treeViewSeq.Nodes[seqNo].Expand();
                treeViewSeq.Nodes[seqNo].Nodes[itemsNo].BackColor = colors;
                treeViewSeq.Nodes[seqNo].Nodes[itemsNo].EnsureVisible();
            }
        }

        private delegate void ShowTreeViewDelegate(bool checkAll = true);//定义更新log委托

        /// <summary>
        /// 显示测试项
        /// </summary>
        public void ShowTreeView(bool checkAll = true)
        {
            if (this.treeViewSeq.InvokeRequired)
            {
                ShowTreeViewDelegate d = new ShowTreeViewDelegate(ShowTreeView); //实体化委托
                this.Invoke(d, new object[] { checkAll });
            }
            else
            {
                if (!this.treeViewSeq.IsDisposed)
                {
                    try
                    {
                        treeViewSeq.Nodes.Clear();
                        if (sequences.Count <= 0)
                            return;
                        for (int i = 0; i < sequences.Count; i++)
                        {
                            TreeNode seq = new TreeNode
                            {
                                Tag = i,
                                Text = (i + 1) + ". " + sequences[i].SeqName,
                                ImageIndex = 1
                            };
                            for (int j = 0; j < sequences[i].SeqItems.Count; j++)
                            {
                                Items item = sequences[i].SeqItems[j];
                                // 全不选
                                if (!checkAll)
                                {
                                    item.isTest = false;
                                    sequences[i].IsTest = false;
                                }
                                else
                                {
                                    item.isTest = true;
                                    sequences[i].IsTest = true;
                                }
                                TreeNode iNode = new TreeNode
                                {
                                    Text = (j + 1) + ") " + item.ItemName,   //!显示标号
                                    ImageIndex = 0,
                                    // iNode.Name = item.;
                                    Tag = j,  //!保存序列号
                                    Checked = item.isTest
                                };
                                //if (item.TestStep.Count <= 0)
                                //    iNode.ForeColor = Color.Gray;
                                seq.Nodes.Add(iNode);
                            }
                            seq.Checked = sequences[i].IsTest;
                            treeViewSeq.Nodes.Add(seq);
                        }
                        treeViewSeq.Nodes[0].Expand();
                    }
                    catch (Exception ex)
                    {
                        loggerFatal("TestThread() Exception:" + ex.ToString());
                    }
                }
            }
        }

        private delegate void UpdateDataViewDeClear();

        public void UpdateDetailViewClear()
        {
            if (this.dataGridViewDetail.InvokeRequired)
            {
                UpdateDataViewDeClear d = new UpdateDataViewDeClear(UpdateDetailViewClear); //实体化委托
                this.Invoke(d, new object[] { });
            }
            else
            {
                if (!this.dataGridViewDetail.IsDisposed)
                {
                    this.dataGridViewDetail.Rows.Clear();
                }
            }
        }

        private delegate void UpdateDataViewDe(string UUT_SN, string ItemName, string Spec, string Limit_min, string CurrentValue, string Limit_max, string ElapsedTime, string StartTime, string TestResult);

        /// <summary>
        /// 定义更新dataGridView委托 更新测试结果
        /// </summary>
        public void UpdateDetailView(string UUT_SN, string ItemName, string Spec, string Limit_min, string CurrentValue, string Limit_max, string ElapsedTime, string StartTime, string TestResult)
        {
            Spec = Spec ?? "--";
            Spec = Spec == "" ? "--" : Spec;
            Limit_min = Limit_min ?? "--";
            Limit_min = Limit_min == "" ? "--" : Limit_min;
            CurrentValue = CurrentValue ?? "--";
            Limit_max = Limit_max ?? "--";
            Limit_max = Limit_max == "" ? "--" : Limit_max;

            if (this.dataGridViewDetail.InvokeRequired)
            {
                UpdateDataViewDe d = new UpdateDataViewDe(UpdateDetailView); //实体化委托
                this.Invoke(d, new object[] { UUT_SN, ItemName, Spec, Limit_min, CurrentValue, Limit_max, ElapsedTime, StartTime, TestResult });
            }
            else
            {
                if (!this.dataGridViewDetail.IsDisposed)
                {
                    string[] paramArray = new string[] { UUT_SN, ItemName, Spec, Limit_min, CurrentValue, Limit_max, ElapsedTime, StartTime, TestResult };
                    int i = dataGridViewDetail.Rows.Add();
                    for (int j = 0; j < colHeader.Length; j++)
                    {
                        dataGridViewDetail.Rows[i].Cells[colHeader[j]].Value = paramArray[j];
                        dataGridViewDetail.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    if (TestResult.ToLower() == "false" || TestResult.ToLower() == "fail")
                    {
                        //dataGridViewDetail.Rows[i].DefaultCellStyle.BackColor = (TestResult == "False") ? Red : Color.Blue;
                        dataGridViewDetail.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                    }
                }
            }
        }


        private delegate void SaveRichTextDelegate(bool isClear, RichTextBoxStreamType a, string path);//定义更新log委托

        /// <summary>
        /// 保存测试结果到文件
        /// </summary>
        public void SaveRichText(bool isClear = false, RichTextBoxStreamType a = 0, string path = "")
        {
            try
            {
                if (this.richTextBox1.InvokeRequired)
                {
                    SaveRichTextDelegate d = new SaveRichTextDelegate(SaveRichText);    //实体化委托
                    this.Invoke(d, new object[] { isClear, a, path });
                }
                else
                {
                    if (!this.richTextBox1.IsDisposed)
                    {
                        if (isClear)
                        {
                            richTextBox1.Clear();
                            loggerDebug($"Clear Data OK!");
                            return;
                        }
                        loggerDebug($"Save test log OK.{path}");
                        richTextBox1.SaveFile(path, a);

                    }
                }
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }


        /// <summary>
        /// 使用本地映射盘的方式操作共享文件夹
        /// </summary>
        /// <param name="mapDrive">y映射的本地盘符名Z:</param>
        /// <param name="logfile">要上传的文件路径</param>
        private void CopyLogToServer(string mapDrive, string logfile)
        {
            try
            {
                //net use Z: /del&net use Z: \\10.177.4.201\NOKIA_LOG Luxshare /USER:Luxshare\nokia_test
                string comline = $@"net use {mapDrive}: /del /y&net use {mapDrive}: {Global.LOGSERVER} {Global.LOGSERVERPwd} /USER:{Global.LOGSERVERUser}";
                string FileName = Path.GetFileName(logfile);
                string destPath = $@"{mapDrive}:\{Global.STATIONNAME}\{Global.FIXTURENAME}\{DateTime.Now.ToString("yyyyMMdd")}";
                RunDosCmd(comline, out string output);
                //  if (output.Contains("命令成功完成"))
                //  {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath); 
                }
                File.Copy(logfile, destPath + @"\" + FileName, true);
                loggerDebug("Upload test log to logServer success.");
                //  }
            }
            catch (Exception ex)
            {
                loggerFatal("Upload test log to logServer Exception:" + ex);
            }
            finally
            {
                RunDosCmd($"net use {mapDrive}: /del /y", out string output);
            }
        }

        private delegate void SetButtonDelegate(Button bts, string text, Color color, bool isEnable);

        public void SetButton(Button button, string strInfo, Color color, bool isEnable = true)
        {
            if (button.InvokeRequired)
            {
                SetButtonDelegate d = new SetButtonDelegate(SetButton); //实体化委托
                this.Invoke(d, new object[] { button, strInfo, color, isEnable });
            }
            else
            {
                if (!button.IsDisposed)
                {
                    button.BackColor = color;
                    button.Text = strInfo;
                    button.Enabled = isEnable;
                }
            }
        }

        private delegate void SetButtonDelegate1(Button bts, string text);

        public void SetButton(Button button, string strInfo)
        {
            if (button.InvokeRequired)
            {
                SetButtonDelegate1 d = new SetButtonDelegate1(SetButton); //实体化委托
                this.Invoke(d, new object[] { button, strInfo });
            }
            else
            {
                if (!button.IsDisposed)
                {
                    button.Text = strInfo;
                }
            }
        }

        private delegate void SetButtonProDelegate(Button bts, Image text);

        public void SetButtonPro(Button button, Image strInfo)
        {
            if (button.InvokeRequired)
            {
                SetButtonProDelegate d = new SetButtonProDelegate(SetButtonPro); //实体化委托
                this.Invoke(d, new object[] { button, strInfo });
            }
            else
            {
                if (!button.IsDisposed)
                {
                    button.Image = strInfo;
                }
            }
        }

        private delegate void SetLableDelegate(Label bts, string text, Color color, bool visible);

        public void SetLables(Label label, string strInfo, Color color, bool visible = true)
        {
            if (label.InvokeRequired)
            {
                SetLableDelegate d = new SetLableDelegate(SetLables); //实体化委托
                this.Invoke(d, new object[] { label, strInfo, color, visible });
            }
            else
            {
                if (!label.IsDisposed)
                {
                    label.BackColor = color;
                    label.Text = strInfo;
                    label.Visible = visible;
                }
            }
        }

        public void ShowLbl_FAIL_TEXT(string text) {
            SetLables(lbl_failCount, text, Color.White,false);
        }






        private delegate void SetTextBoxDelegate(TextBox textBox, bool isEnable, string text);

        public void SetTextBox(TextBox textBox, bool isEnable = true, string text = "")
        {
            if (textBox.InvokeRequired)
            {
                SetTextBoxDelegate d = new SetTextBoxDelegate(SetTextBox); //实体化委托
                this.Invoke(d, new object[] { textBox, isEnable, text });
            }
            else
            {
                if (!textBox.IsDisposed)
                {
                    textBox.Enabled = isEnable;
                    textBox.Text = text;
                    this.ActiveControl = textBox; //设置当前窗口的活动控件为textBox1
                    textBox.Focus();
                    if (Global.CameraType == "1" && text!="") {
                        SendKeys.Send("{ENTER}");
                    }
                }
            }
        }



        //private delegate void SetWindowFrontDelegate(Form f);

        //public void SetWindowFront(Form f)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        SetWindowFrontDelegate d = new SetWindowFrontDelegate(SetWindowFront); //实体化委托
        //        this.Invoke(d, new object[] { });
        //    }
        //    else
        //    {
        //        if (!this.IsDisposed)
        //        {

        //            loggerInfo("把窗体放在最上层");
        //            //窗体放在最上层
        //            SwitchToThisWindow(Global.processInfo.MainWindowHandle, true);
        //        }
        //    }
        //}


        #endregion 控件更新处理函数

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}