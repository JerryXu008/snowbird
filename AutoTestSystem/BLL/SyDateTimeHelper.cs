using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTestSystem.BLL
{

    public class SyDateTimeHelper
    {
        public static ILog logger = Log4NetHelper.GetLogger(typeof(Bd));



        [StructLayout(LayoutKind.Sequential)]
        private struct Systemtime
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref Systemtime time);

        private static uint swapEndian(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
            ((x & 0x0000ff00) << 8) +
            ((x & 0x00ff0000) >> 8) +
            ((x & 0xff000000) >> 24));
        }

        /// <summary>
        /// 设置系统时间
        /// </summary>
        /// <param name="dt">需要设置的时间</param>
        /// <returns>返回系统时间设置状态，true为成功，false为失败</returns>
        public static bool SetLocalDateTime222(DateTime dt)
        {
            Systemtime st;
            st.year = (short)dt.Year;
            st.month = (short)dt.Month;
            st.dayOfWeek = (short)dt.DayOfWeek;
            st.day = (short)dt.Day;
            st.hour = (short)dt.Hour;
            st.minute = (short)dt.Minute;
            st.second = (short)dt.Second;
            st.milliseconds = (short)dt.Millisecond;
            bool rt = SetLocalTime(ref st);
            // MessageBox.Show(rt ? "成功" : "失败");
            return rt;
        }


        /// <summary>
        /// 设置本地电脑的年月日
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public static void SetLocalDate(int year, int month, int day)
        {
            //实例一个Process类，启动一个独立进程
            Process p = new Process();
            //Process类有一个StartInfo属性
            //设定程序名
            p.StartInfo.FileName = @"C:\Windows\system32\cmd.exe"; ;
            //设定程式执行参数 “/C”表示执行完命令后马上退出
            p.StartInfo.Arguments = string.Format("/c date {0}-{1}-{2}", year, month, day);
            //关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            //设置不显示doc窗口
            p.StartInfo.CreateNoWindow = true;
            //启动
            p.Start();
            //从输出流取得命令执行结果
            p.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// 设置本机电脑的时分秒
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="min"></param>
        /// <param name="sec"></param>
        public static void SetLocalTime(int hour, int min, int sec)
        {
            //实例一个Process类，启动一个独立进程
            Process p = new Process();
            //Process类有一个StartInfo属性
            //设定程序名
            p.StartInfo.FileName = @"C:\Windows\system32\cmd.exe";
            //设定程式执行参数 “/C”表示执行完命令后马上退出
            p.StartInfo.Arguments = string.Format("/c time {0}:{1}:{2}", hour, min, sec);
            //关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            //设置不显示doc窗口
            p.StartInfo.CreateNoWindow = true;
            //启动
            p.Start();
            //从输出流取得命令执行结果
            p.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// 设置本机电脑的年月日和时分秒
        /// </summary>
        /// <param name="time"></param>
        public static void SetLocalDateTime(DateTime time)
        {
            // Console.WriteLine($"设置本地时间为:{time.ToLongTimeString()}");
            // SetLocalDate(time.Year, time.Month, time.Day);
            //  SetLocalTime(time.Hour, time.Minute, time.Second);
            //  MessageBox.Show("ddd:" + time.Hour);


            //logger.Info("执行DOS命令:" + string.Format(" date {0}-{1}-{2} & time {3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second));
            //RunDosCmd(string.Format(" date {0}-{1}-{2} & time {3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second));
            ////获取本地时间
            //if (DateTime.Now.ToLongDateString().Contains("2021")) { //没成功，换别的方法
            //    logger.Info("修改没成功，换别的方法");
            //    var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
            //   bool result = SetLocalDateTime222(dateTime);
            //    if (result)
            //    {
            //        logger.Info("更新成功");
            //    }
            //    else {
            //        logger.Info("更新失败");
            //    }
            //}

            var dateTime = SyDateTimeHelper.GetNetDateTime(DateTime.Now);
            bool result = SetLocalDateTime222(dateTime);
            if (result)
            {
                logger.Info("时间更新成功,时间:" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString());
            }
            else
            {
                logger.Info("时间更新失败,时间:" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString());
            }



        }


        public static void SetLocalDateTime2(DateTime time)
        {

            try
            {

                var dateTime = time;
                bool result = SetLocalDateTime222(dateTime);
                if (result)
                {
                    logger.Info("时间更新成功,时间:" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString());
                }
                else
                {
                    logger.Info("时间更新失败,时间:" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString());
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString());
            }



        }

        public static string RunDosCmd(string command, int timeout = 0)
        {

            using (var p = new Process())
            {
                p.StartInfo.FileName = @"C:\Windows\system32\cmd.exe";
                //  MessageBox.Show(p.StartInfo.FileName);
                p.StartInfo.Arguments = "/c " + command;    //命令运行之后窗口关闭
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                var error = "";
                p.ErrorDataReceived += (sender, e) => { error += e.Data; };
                p.Start();
                //  p.BeginErrorReadLine();                     //获取cmd窗口的输出信息
                //  var output = p.StandardOutput.ReadToEnd();
                // p.WaitForExit(timeout * 1000);

                //p.Close();

                // return output + error;
                return "";

            }
        }











        /// <summary>
        /// 获取互联网时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNetDateTime(DateTime preTime)
        {
            WebRequest request = null;
            WebResponse response = null;
            WebHeaderCollection headerCollection = null;
            string datetime = string.Empty;
            try
            {
                // request = WebRequest.Create("http://www.google.com");
                request = WebRequest.Create("http://10.90.122.1");

                request.Timeout = 1000;

                request.Credentials = CredentialCache.DefaultCredentials;
                response = (WebResponse)request.GetResponse();
                headerCollection = response.Headers;

                foreach (var h in headerCollection.AllKeys)
                {
                    if (h == "Date")
                    {
                        datetime = headerCollection[h];

                        RunDosCmd("tzutil /s \"UTC_dstoff\"");// 设置电脑时区为UTC
                        logger.Debug("更改为UTC时区");

                        var dt = DateTime.Parse(datetime);
                        logger.Debug("得到最新的UTC时间:" + datetime);

                        return dt;
                    }
                }
                //
                //return DateTime.Now.AddYears(2);
                return preTime;
            }
            catch (Exception ex)
            {
                // return DateTime.Now.AddYears(2);
                return preTime;
            }
            finally
            {
                if (request != null)
                { request.Abort(); }
                if (response != null)
                { response.Close(); }
                if (headerCollection != null)
                { headerCollection.Clear(); }
            }
        }















        private static IPAddress iPAddress = null;
        public static bool Synchronization(string host, out DateTime syncDateTime, out string message)
        {
            syncDateTime = DateTime.Now;
            try
            {
                message = "";
                if (iPAddress == null)
                {
                    var iphostinfo = Dns.GetHostEntry(host);
                    var ntpServer = iphostinfo.AddressList[0];
                    iPAddress = ntpServer;
                }
                DateTime dtStart = DateTime.Now;
                //NTP消息大小摘要是16字节 (RFC 2030)
                byte[] ntpData = new byte[48];
                //设置跳跃指示器、版本号和模式值
                // LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
                ntpData[0] = 0x1B;
                IPAddress ip = iPAddress;
                // NTP服务给UDP分配的端口号是123
                IPEndPoint ipEndPoint = new IPEndPoint(ip, 123);
                // 使用UTP进行通讯
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = 3000;
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket?.Close();
                socket?.Dispose();
                DateTime dtEnd = DateTime.Now;
                //传输时间戳字段偏移量，以64位时间戳格式，应答离开客户端服务器的时间
                const byte serverReplyTime = 40;
                // 获得秒的部分
                ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
                //获取秒的部分
                ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
                //由big-endian 到 little-endian的转换
                intPart = swapEndian(intPart);
                fractPart = swapEndian(fractPart);
                ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000UL);
                // UTC时间
                DateTime webTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(milliseconds);
                //本地时间
                DateTime dt = webTime.ToLocalTime();

                syncDateTime = dt;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return true;

        }
    }
}