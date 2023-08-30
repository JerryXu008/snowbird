using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoTestSystem.BLL
{
    public class ScanManager
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ts_scan_get_data_fun(ref byte pParam, [MarshalAs(UnmanagedType.LPArray, SizeConst = 8000)] byte[] pBuf, int uiBufLen);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ts_scan_state_fun(ref byte pParam, byte ucState);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_init();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_deinit();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr ts_scan_get_version();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static byte ts_scan_get_product_type();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_data_fun_register(ref byte pParam, ts_scan_get_data_fun fGetDataFun);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_state_fun_register(ref byte pParam, ts_scan_state_fun fStateFun);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_decode_start();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_decode_stop();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_product_id(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_product_name(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_product_date(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_custom_name(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_custom_id(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_custom_date(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_hardware_version(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_firmware_version(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_complex_sn(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_kbw_enable(byte ucEn);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_custom_data_write(byte[] pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_custom_data_read(ref byte pBuf, int uiBufLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_light_mode(byte ucMode, byte isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_light_mode();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_beep_play(byte ucEn);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_play_voice(byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_one_scan_time(byte pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_one_scan_time();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_Prefix(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_Prefix(byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_Suffix(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_Suffix(byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_sw(int ucEn);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_device_state();

        /***********************NFC Start***************************/
        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_work_mode(byte ucMode, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_work_mode();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_id_format(byte ucFormat, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_id_format();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_data_format(byte ucFormat, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_data_format();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_block_num(byte ucBlock, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_block_num();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_key_type(byte ucKeyType, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_key_type();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_key_a(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_key_a(byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_key_b(byte[] pBuf, int usLen, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_key_b(byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_nfc_cmd_read_data(byte ucReadType, byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_nfc_block_data_write(byte[] pBuf, int usLen);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_set_nfc_en_write_ctrl(byte ucEnable, int isTemp);

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_get_nfc_en_write_ctrl();

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_nfc_change_mf1_key(byte ucKeyType, byte[] pBuf, int usLen);
        /***********************NFC End***************************/

        [DllImport("C++_hidpos_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int ts_scan_decode_status_request();


        public delegate void ScanFinishedDelegate(string sn);

        public event ScanFinishedDelegate ScanFinished;




        static int s_iScanCnt = 0;
        /**上报解码数据**/
         void my_scan_decode_data(ref byte pParam, byte[] pBuf, int uiBufLen)
        {
            int tlen = pBuf.Length;
            Console.WriteLine("len:{0}", tlen);
            byte[] tBuf = new byte[uiBufLen];

            for (int i = 0; i < uiBufLen; i++)
            {
                tBuf[i] = pBuf[i];
            }

            string str = System.Text.Encoding.Default.GetString(tBuf);
            Console.WriteLine("scan data len:{0},cnt:{1}\r\n{2}", uiBufLen, ++s_iScanCnt, str);

            ScanFinished(str);




            //ts_scan_decode_stop();/**停止扫描**/
            //Thread.Sleep(1000);/**延时1000ms**/
            //ts_scan_decode_start();/**开始扫描**/
        }

        /**上报状态,0:设备断开,1:设备连接**/
        static void my_scan_state(ref byte pParam, byte ucState)
        {
            Console.WriteLine("scan state:{0}", ucState);

            //#if (TRUE)
            if (1 == ucState)
            {
                byte[] byBuf = new byte[30];
                int iEn = 0;
                int iRetLen;
                string strSn = null;
                string strName = null;
                string strVer = null;
                string strCplSn = null;
                string strKbw = null;

                iRetLen = ts_scan_get_product_name(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---product name:{0}, {1}, {2}", strSn, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_product_id(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---product id:{0}, {1}, {2}", strSn, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_product_date(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---product date:{0}, {1}, {2}", strSn, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_custom_name(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---custom name:{0}, {1}, {2}", strSn, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_custom_id(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---custom id:{0}, {1}, {2}", strSn, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_custom_date(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---custom date:{0}, {1}, {2}", strSn, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_firmware_version(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strName = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---firmware version:{0}, {1}, {2}", strName, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_hardware_version(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strVer = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---hardware version:{0}, {1}, {2}", strVer, iRetLen, byBuf.Length);

                iRetLen = ts_scan_get_complex_sn(ref byBuf[0], byBuf.Length);
                if (iRetLen > 0)
                    strCplSn = System.Text.Encoding.Default.GetString(byBuf, 0, iRetLen);
                Console.WriteLine("---CPL SN:{0}, {1}, {2}", strCplSn, iRetLen, byBuf.Length);

                //				iRetLen = ts_scan_kbw_enable(iEn);

            }
            //#endif
        }

        static ts_scan_get_data_fun fScanGetDataFun;
        static ts_scan_state_fun fScanStateFun;

        public ScanManager() {


            byte[] BufSn = new byte[64];

            /**用户数据读写测试**/
            byte[] ucWrite = new byte[2048];
            byte[] ucRead = new byte[2048];
            int usWriteLen = 0;
            int usReadLen = 0;
            int i, j;
            int iRet = -1;

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
            byte[] Nop1 = new byte[16];
            fScanGetDataFun = my_scan_decode_data;
            ts_scan_get_data_fun_register(ref Nop1[0], fScanGetDataFun);

            /**注册状态上报函数**/
            byte[] Nop2 = new byte[16];
            fScanStateFun = my_scan_state;
            ts_scan_state_fun_register(ref Nop2[0], fScanStateFun);

            /**初始化**/
            ts_scan_init();

            /**获取扫描模块版本号**/
            IntPtr pVer = ts_scan_get_version();
            string strVer = Marshal.PtrToStringAnsi(pVer);
            byte Type = ts_scan_get_product_type();
            Console.WriteLine("ts scan ver:{0} [{1}]", strVer, Type);
        }

        public void Scan() {
            int re = ts_scan_decode_start();
            if (0 > re)
            {
                Console.WriteLine("扫码开启失败");
            }

            else
            {
                Console.WriteLine("开始扫描");
            }

        }

        public void Close()
        {
            ts_scan_decode_stop();


        }
    }
}
