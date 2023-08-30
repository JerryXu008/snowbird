using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoTestSystem.BLL
{
    public class DateTimeHelper
    {
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
            p.StartInfo.FileName = "cmd.exe";
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
            p.StartInfo.FileName = "cmd.exe";
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
            //SetLocalTime(time.Hour, time.Minute, time.Second);
            RunDosCmd(string.Format(" date {0}-{1}-{2}", time.Year, time.Month, time.Day));
            RunDosCmd(string.Format(" time {0}:{1}:{2}", time.Hour, time.Minute, time.Second));
        }

        public static string RunDosCmd(string command, int timeout = 0)
        {
            
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
                
                return output + error;
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
                request = WebRequest.Create("https://www.google.com");
                request.Timeout = 1000;

                request.Credentials = CredentialCache.DefaultCredentials;
                response = (WebResponse)request.GetResponse();
                headerCollection = response.Headers;

                foreach (var h in headerCollection.AllKeys)
                {
                    if (h == "Date")
                    {
                        datetime = headerCollection[h];

                        var dt = DateTime.Parse(datetime);
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
    }
}
