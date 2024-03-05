 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTestSystem.BLL
{
    public class ScanManager
    { // 在你的应用程序开始时执行



        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ts_scan_get_data_fun(ref byte pParam, string pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_init();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_deinit();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr ts_scan_get_version();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static byte ts_scan_get_product_type();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_uart_open(int usUartNum, int uiBaudrate);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_uart_close();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_uart_state();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_data_fun_register(ref byte pParam, ts_scan_get_data_fun fGetDataFun);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_decode_start();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_decode_stop();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_rs485_net_mode(int ucmode);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_fixe_rn_add_decode_data(int usrndeviceadd);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_product_id(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_product_name(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_product_date(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_custom_name(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_custom_id(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_custom_date(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_hardware_version(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_firmware_version(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_complex_sn(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_kbw_enable(byte ucEn);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_custom_data_write(byte[] pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_custom_data_read(ref byte pBuf, int uiBufLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_light_mode(byte ucMode, byte isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_light_mode();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_beep_play(byte ucEn);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_play_voice(byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_one_scan_time(byte pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_one_scan_time();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_Prefix(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_Prefix(byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_Suffix(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_Suffix(byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_sw(int ucEn);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_device_state();

        /***********************NFC Start***************************/
        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_work_mode(byte ucMode, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_work_mode();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_id_format(byte ucFormat, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_id_format();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_data_format(byte ucFormat, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_data_format();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_block_num(byte ucBlock, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_block_num();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_key_type(byte ucKeyType, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_key_type();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_key_a(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_key_a(byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_key_b(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_key_b(byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_nfc_cmd_read_data(byte ucReadType, byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_nfc_block_data_write(byte[] pBuf, int usLen);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_en_write_ctrl(byte ucEnable, int isTemp);

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_en_write_ctrl();

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_nfc_change_mf1_key(byte ucKeyType, byte[] pBuf, int usLen);
        /***********************NFC End***************************/

        [DllImport("C++_uart_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_decode_status_request();


        static int s_iScanCnt = 0;
        /**上报解码数据**/


        public delegate void ScanFinishedDelegate(string sn);

        public event ScanFinishedDelegate ScanFinished;

        public delegate void ScanErrorDelegate(int portIndex, string errorMsg);

        public event ScanErrorDelegate ScanError;





        static ts_scan_get_data_fun fScanGetDataFun;

        //public   bool IsMainThread
        //{
        //    get
        //    {
        //        return System.Threading.Thread.CurrentThread.ManagedThreadId == Global.mainThreadId;
        //    }
        //}


        /**上报解码数据**/
        int my_scan_decode_data(ref byte pParam, string pBuf, int uiBufLen)
        {

            ScanFinished(pBuf);
            Console.WriteLine("scan data len:{0},cnt:{1}\r\n{2}", uiBufLen, ++s_iScanCnt, pBuf);
            return 0;
        }

        int testtest = 0;
        public ScanManager()
        {

            init();



        }
        public void init()
        {




            byte[] BufSn = new byte[64];
            int iRet = -1;
            int imode;
            int irndeviceadd = 0;

            /**用户数据读写测试**/
            byte[] ucWrite = new byte[2048];
            byte[] ucRead = new byte[2048];
            int usWriteLen = 0;
            int usReadLen = 0;
            int i, j;

            /***********************NFC Start***************************/
            string strInput;/**设置工作模式**/
            byte sucInput = 0;

            /**NFC读出数据的缓存区**/
            byte[] ucReadBuf = new byte[512];
            /**NFC写入块数据**/
            byte[] ucGroup1 = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0x00, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };
            byte[] ucGroup2 = { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 };
            byte[] ucGroup3 = { 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99 };

            byte[] ucKey1 = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
            byte[] ucKey2 = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] ucKey3 = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            /***********************NFC End***************************/

            usWriteLen = 2048;/**最大支持2K**/
            for (i = 0; i < usWriteLen; i++)
                ucWrite[i] = 0x05;

            Console.WriteLine("hello ts scan");

            /**注册扫描数据上报函数**/
            byte[] Nop = new byte[16];
            fScanGetDataFun = my_scan_decode_data;
            ts_scan_get_data_fun_register(ref Nop[0], fScanGetDataFun);

            /**初始化**/
            ts_scan_init();

            /**获取扫描模块版本号**/
            IntPtr pVer = ts_scan_get_version();
            string strVer = Marshal.PtrToStringAnsi(pVer);
            Console.WriteLine("tt scan ver:" + strVer);

            /**设置RS485 组网模式 1为组网模式，0为普通模式**/
            imode = 0;
            ts_scan_set_rs485_net_mode(imode);
            if (imode > 0)
                Console.WriteLine("set rs485 net mode");
            else
                Console.WriteLine("set nor mode");


            byte uctime;
            uctime = 255;
            iRet = ts_scan_set_one_scan_time(uctime, 0);


        }

        public void Scan()
        {



            // Thread thread = new Thread(() => { 


            string usuartNum;
            string uiBaudrate;
            int siuartnum = 0;
            int siBaudrate = 0;

            usuartNum = "COM9";
            uiBaudrate = "115200";

            siuartnum = int.Parse(Regex.Replace(usuartNum, "COM", ""));
            siBaudrate = int.Parse(uiBaudrate);



            // string[] portNames = System.IO.Ports.SerialPort.GetPortNames();

            //if (!portNames.Contains(usuartNum))
            //{
            //    ScanError(portIndex, $"COM{COM} is not exist");
            //    return;
            //}


            ts_scan_uart_close();
            //ts_scan_decode_stop();
            var iRet = ts_scan_uart_open(siuartnum, siBaudrate);

            if (iRet != 0)
            {
                ts_scan_uart_close();
                ts_scan_decode_stop();
                if (ScanError != null)
                {
                   
                }


                return;
            }



            byte uctime;
            uctime = 255;
            iRet = ts_scan_set_one_scan_time(uctime, 0);


            int re = ts_scan_decode_start();
            if (0 > re)
            {
                ts_scan_uart_close();
                ts_scan_decode_stop();
                if (ScanError != null)
                {
                     
                }



            }

            else
            {
                Console.WriteLine("开始扫描");
            }


            //});
            //thread.IsBackground = true;
            //thread.Start();

        }

        public void Close()
        {
            ts_scan_decode_stop();


        }
    }
}
