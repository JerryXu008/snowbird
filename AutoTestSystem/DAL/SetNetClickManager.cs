using AutoTestSystem.BLL;
using AutoTestSystem.Model;
using AutoTestSystem.Properties;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoTestSystem.DAL
{
    public class SetNetClickManager
    {

        public static ILog logger = Log4NetHelper.GetLogger(typeof(SetNetClickManager));

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
        public bool CheckSSID(string wifiName) {

            string pythonPath = Global.PythonPath + "\\python.exe"; // 替换为实际的 Python 安装路径
            string scriptPath = @"searchnet.py"; // 替换为实际的 Python 脚本路径
            string imagePath = wifiName; // 替换为实际的图像路径

            var cmd = pythonPath + " " + scriptPath + " " + imagePath;

            logger.Debug(cmd);

            var txt = RunDosCmd(cmd);
            logger.Debug("return:" + txt);
            if (txt.Contains("True"))
            {
                return true;
            }
            else {
                return false;
            }
           

        }
        public bool CheckBLE(string mac)
        {

            string pythonPath = Global.PythonPath + "\\python.exe"; // 替换为实际的 Python 安装路径
            string scriptPath = @"searchble.py"; // 替换为实际的 Python 脚本路径
            string imagePath = mac; // 替换为实际的图像路径

            var cmd = pythonPath + " " + scriptPath + " " + imagePath;

            logger.Debug(cmd);

            var txt = RunDosCmd(cmd);
            logger.Debug("return:" + txt);
            if (txt.Contains("True"))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

    }
}
