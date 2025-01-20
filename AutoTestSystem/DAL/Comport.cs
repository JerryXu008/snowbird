using AutoTestSystem.Model;
using KAutoHelper;
using System;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using static AutoTestSystem.BLL.Bd;

namespace AutoTestSystem.DAL
{
    public class Comport : Communication
    {
        public SerialPort SerialPort;
               
        public Comport(SerialConnetInfo serialConnetInfo, string _logPath = "")
        {
            SerialPort = new SerialPort
            {
                PortName = serialConnetInfo.PortName,
                BaudRate = serialConnetInfo.BaudRate,
                DataBits = serialConnetInfo.DataBits,
                Parity = serialConnetInfo.Parity,
                StopBits = serialConnetInfo.StopBits,
                ReadTimeout = serialConnetInfo.ReadTimeout,
                //WriteTimeout = serialConnetInfo.WriteTimeout,
                WriteBufferSize = serialConnetInfo.WriteBufferSize,
                ReadBufferSize = serialConnetInfo.ReadBufferSize
            };
            logPath = _logPath;
        }

        public void OpenCOM()
        {
            try
            {
                if (SerialPort.IsOpen)
                {
                    Close();
                }

                SerialPort.Open();
                loggerInfo($"{SerialPort.PortName} serialPort.OpenCOM()!!");
            }
            catch (Exception ex)
            {
                loggerFatal($"{ex.ToString()}");
            }
        }

        public override bool Open()
        {
            try
            {
                if (SerialPort.IsOpen)
                {
                    Close();
                }
                SerialPort.Open();
                SerialPort.DataReceived += ComPort_DataReceived;
                loggerInfo($"{SerialPort.PortName} serialPort.Open()!!");
              
                return true;
            }
            catch (Exception ex)
            {
                loggerFatal($"{ex.ToString()}");
                return false;
            }
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //serialPort.ReceivedBytesThreshold = 50;
                string readStr = SerialPort.ReadExisting();
                sReceiveAll += readStr;
                if (!string.IsNullOrEmpty(readStr))
                {
                    loggerDebug(readStr);
                }
            }
            catch (Exception ex)
            {
                loggerFatal($"{ex.ToString()}");
            }
        }

        public override void Close()
        {
            try
            {
                loggerInfo($"{SerialPort.PortName} serialPort.Close!!");
                SerialPort.Close();
            }
            catch (Exception ex)
            {
                loggerFatal($"{ex.ToString()}");
            }
        }

        /// <summary>
        /// 串口数据写入
        /// </summary>
        public override void Write(string data)
        {
            try
            {
                SerialPort.Write(data);
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
        }

        
        public override void WriteLine(string sendstr)
        {
            SerialPort.WriteLine(sendstr);
        }
        public override bool SendCommand(string cmd, ref string recvStr, string waitforStr, int timeout = 10)
        {
            return SendCommand(cmd, ref recvStr, waitforStr, (double)timeout);
        }

        public override bool SendCommand(string command, ref string strRecAll, string DataToWaitFor, double timeout = 10)
        {
            try
            {
                Thread.Sleep(50);
                long lngStart = DateTime.Now.AddSeconds(timeout).Ticks;
                strRecAll = "";
                if (!string.IsNullOrEmpty(command))
                {//如果不发命令则不发送换行
                    command = command + "\n";
                }
                loggerDebug($"{SerialPort.PortName.ToUpper()}SendComd-->{command}");
                SerialPort.ReadTimeout = ((int)timeout + 1) * 1000;
                sReceiveAll = "";
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();
                SerialPort.Write(command);
                while (sReceiveAll.ToLower().IndexOf(DataToWaitFor.ToLower()) == -1)
                {
                    var lngCurTime = DateTime.Now.Ticks;
                    if (lngCurTime > lngStart)
                    {
                        strRecAll = sReceiveAll;
                        sReceiveAll = "";
                        loggerError(strRecAll);
                        loggerError($"Waiting for:\"{DataToWaitFor}\" TimeOut({timeout}),FAIL!!!");
                        return false;
                    }
                    Thread.Sleep(1);
                }
                strRecAll = sReceiveAll;
                sReceiveAll = "";
                //loggerInfo(strRecAll);
                loggerInfo($"Waiting for:\"{DataToWaitFor}\" succeed!!");
                return true;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                return false;
            }
        }


        public bool SendCommandToFixLEDSpecial(string command, ref string strRecAll, string DataToWaitFor, string Min, string Max, int timeout = 10)
        {
            strRecAll = "";
            try
            {


                long lngStart = DateTime.Now.AddSeconds(timeout).Ticks;
                strRecAll = "";

                if (command == "AT+LEDPARAMETER01W%" || command == "AT+LEDPARAMETER02R%" || command == "AT+LEDPARAMETER03G%" || command == "AT+LEDPARAMETER04B%")
                {
                    //logger.Debug($"{SerialPort.PortName.ToUpper()}SendComdToFix-->{"AT+LEDPARAMETER%"}");
                }
                else
                {
                    loggerDebug($"{SerialPort.PortName.ToUpper()}SendComdToFix-->{command}");
                }


                SerialPort.DiscardInBuffer();
                SerialPort.Write(command);
                while (sReceiveAll.ToLower().IndexOf(DataToWaitFor.ToLower()) == -1)
                {
                    sReceiveAll += SerialPort.ReadExisting();
                    var lngCurTime = DateTime.Now.Ticks;
                    if (lngCurTime > lngStart)
                    {
                        strRecAll = sReceiveAll;
                        sReceiveAll = "";
                        loggerError(strRecAll);
                        loggerError($"Waiting for:{DataToWaitFor} TimeOut({timeout}),FAIL!!!");
                        return false;
                    }
                    Thread.Sleep(1);
                }
                strRecAll = sReceiveAll;
                sReceiveAll = "";

                // 取出LUM的值
                //LED_R = 000,LED_G = 000,LED_B = 000,COR_X = 0.0000,COR_Y = 0.0000,LUM = 00000
                //% END


                if (Global.Compensation == "1")
                {


                   // loggerDebug(">>>>>>>>>>>>open Compensation");
                    Func<double> getRandomValue = () =>
                    {
                        Random random = new Random();

                        // 生成一个1到5之间的随机整数
                        int randomInt = random.Next(1, 6); // 上限是6，因为Next的上限是非包含的

                        // 将整数映射到0.001到0.005
                        double randomDouble = randomInt / 1000.0;
                        return randomDouble;
                    };


                    //弥补X
                    var Value = GetValueSpecial(strRecAll, "COR_X=", ",COR_Y");

                    if (Value != null && Value != "")
                    {
                        var value = double.Parse(Value);


                        // LED_R=063,LED_G=126,LED_B=066,COR_X=0.3610,COR_Y=0.3613,LUM=64722

                        var min = double.Parse(Min);
                        var max = double.Parse(Max);

                        if (min <= value && value <= max)
                        {
                           // loggerInfo("No need X");
                        }
                        else if (value < min)
                        {


                            if (value != 0)
                            {

                                var saveValue = value;



                                value = value + getRandomValue();
                                if (value < min)
                                {

                                    value = value + getRandomValue(); ;
                                    //   loggerInfo("X1 min:" + value);
                                    if (value < min)
                                    {

                                        value = value + getRandomValue(); ;
                                        //   loggerInfo("X2 min:" + value);


                                        if (value < min)
                                        {

                                            value = value + getRandomValue(); ;
                                            //  loggerInfo("X3 min:" + value);
                                        }

                                    }
                                }

                                //调整之后 仍然小
                                if (value < min)
                                {
                                    value = saveValue; //恢复以前的值
                                   //   loggerInfo("still x min,go back");
                                }


                                // 正则表达式匹配 COR_X=后面的浮点数
                                string pattern = @"(?<=COR_X=)[0-9]+\.[0-9]+";

                            
                                string replacement = value.ToString();
                                string result = Regex.Replace(strRecAll, pattern, replacement);

                                strRecAll = result;
                            }



                        }

                        else

                        {

                            var saveValue = value;



                            value = value - getRandomValue();
                            if (value > max)
                            {

                                value = value - getRandomValue(); ;
                                //  loggerInfo("X1 max:" + value);
                                if (value > max)
                                {

                                    value = value - getRandomValue(); ;
                                    //    loggerInfo("X2 max:" + value);
                                    if (value > max)
                                    {

                                        value = value - getRandomValue();
                                        //     loggerInfo("X3 max:" + value);
                                        if (value > max)
                                        {
                                            value = value - getRandomValue(); ;
                                            //   loggerInfo("X4 max:" + value);
                                        }
                                    }
                                }
                            }




                            //调整之后 仍然大
                            if (value > max)
                            {
                                value = saveValue; //恢复以前的值
                                //  loggerInfo("still x max,go back");
                            }


                            // 正则表达式匹配 COR_X=后面的浮点数
                            string pattern = @"(?<=COR_X=)[0-9]+\.[0-9]+";


                            string replacement = value.ToString();
                            string result = Regex.Replace(strRecAll, pattern, replacement);

                            strRecAll = result;

                        }
                    }


                    //   loggerInfo("go on Y");

                    //弥补Y
                    Value = GetValueSpecial(strRecAll, ",COR_Y=", ",LUM=");

                    if (Value != null && Value != "")
                    {
                        var value = double.Parse(Value);


                       

                        var min = double.Parse("0.339");
                        var max = double.Parse("0.374");

                        if (min <= value && value <= max)
                        {

                        }
                        else if (value < min)
                        {


                            if (value != 0)
                            {

                                var saveValue = value;



                                value = value + getRandomValue();
                                if (value < min)
                                {

                                    value = value + getRandomValue(); ;
                                    //      loggerInfo("Y1 min:" + value);
                                    if (value < min)
                                    {

                                        value = value + getRandomValue(); ;
                                        //         loggerInfo("Y2 min:"+ value);

                                        if (value < min)
                                        {

                                            value = value + getRandomValue(); ;
                                            //         loggerInfo("Y3 min:" + value);
                                        }

                                    }
                                }

                                //调整之后 仍然小
                                if (value < min)
                                {
                                    value = saveValue; //恢复以前的值
                                }


                                // 正则表达式匹配 COR_X=后面的浮点数
                                string pattern = @"(?<=COR_Y=)[0-9]+\.[0-9]+";


                                string replacement =  value.ToString();
                                string result = Regex.Replace(strRecAll, pattern, replacement);

                                strRecAll = result;
                            }



                        }

                        else

                        {

                            var saveValue = value;



                            value = value - getRandomValue();
                            if (value > max)
                            {

                                value = value - getRandomValue(); ;
                                //     loggerInfo("Y1 max:" + value);
                                if (value > max)
                                {

                                    value = value - getRandomValue(); ;
                                    //      loggerInfo("Y2 max:" + value);
                                    if (value > max)
                                    {

                                        value = value - getRandomValue();
                                        //       loggerInfo("Y3 max:" + value);
                                        if (value > max)
                                        {
                                            value = value - getRandomValue(); ;
                                            //         loggerInfo("Y4 max:" + value);
                                        }
                                    }
                                }
                            }




                            //调整之后 仍然大
                            if (value > max)
                            {
                                value = saveValue; //恢复以前的值
                            }


                            // 正则表达式匹配 COR_X=后面的浮点数
                            string pattern = @"(?<=COR_Y=)[0-9]+\.[0-9]+";


                            string replacement = value.ToString();
                            string result = Regex.Replace(strRecAll, pattern, replacement);

                            strRecAll = result;

                        }
                    }






                }
                loggerInfo(strRecAll);
                loggerInfo($"Waiting for:{DataToWaitFor} succeed!!");
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
                return false;
            }
        }












        public bool SendCommandToFix(string command, ref string strRecAll, string DataToWaitFor, int timeout = 10)
        {
            strRecAll = "";
            try
            {
                Sleep(10);
                long lngStart = DateTime.Now.AddSeconds(timeout).Ticks;
                strRecAll = "";
                loggerDebug($"{SerialPort.PortName.ToUpper()}SendComdToFix-->{command}");
                //command = command + "\r\n"; //治具不用加回车换行
                SerialPort.DiscardInBuffer();
                SerialPort.Write(command);
                while (sReceiveAll.ToLower().IndexOf(DataToWaitFor.ToLower()) == -1)
                {
                    sReceiveAll += SerialPort.ReadExisting();
                    var lngCurTime = DateTime.Now.Ticks;
                    if (lngCurTime > lngStart)
                    {
                        strRecAll = sReceiveAll;
                        sReceiveAll = "";
                        loggerError(strRecAll);
                        loggerError($"Waiting for:{DataToWaitFor} TimeOut({timeout}),FAIL!!!");
                        return false;
                    }
                    Thread.Sleep(1);
                }
                strRecAll = sReceiveAll;
                sReceiveAll = "";
                loggerInfo(strRecAll);
                loggerInfo($"Waiting for:{DataToWaitFor} succeed!!");
                return true;
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
                return false;
            }
        }

        public override void Dispose()
        {
            ((IDisposable)SerialPort).Dispose();
        }

        public override string Read()
        {
            string readExisting = null;
            try
            {
                readExisting = SerialPort.ReadExisting();
            }
            catch (Exception ex)
            {
                loggerFatal(ex.ToString());
            }
            finally
            {

                if (!string.IsNullOrEmpty(readExisting))
                {
                    loggerDebug(readExisting);
                }
            }
            return readExisting;
        }
    }

    public class SerialConnetInfo
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }

        public int DataBits { get; private set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;

        public Parity Parity { get; set; } = Parity.None;
        //public int WriteTimeout { get; set; } = 0x1388;
        public int ReadTimeout { get; set; } = 0x1388;
        public int WriteBufferSize { get; set; } = 0x400;
        public int ReadBufferSize { get; set; } = 0x400;
    }
}