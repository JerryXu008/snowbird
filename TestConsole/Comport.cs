using System;
using System.IO.Ports;
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
                logger.Info($"{SerialPort.PortName} serialPort.OpenCOM()!!");
            }
            catch (Exception ex)
            {
                logger.Fatal($"{ex.ToString()}");
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
                logger.Info($"{SerialPort.PortName} serialPort.Open()!!");
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal($"{ex.ToString()}");
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
                    logger.Debug(readStr);
                }
            }
            catch (Exception ex)
            {
                logger.Fatal($"{ex.ToString()}");
            }
        }

        public override void Close()
        {
            try
            {
                logger.Info($"{SerialPort.PortName} serialPort.Close!!");
                SerialPort.Close();
            }
            catch (Exception ex)
            {
                logger.Fatal($"{ex.ToString()}");
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
                logger.Fatal(ex.ToString());
            }
        }

        
        public override void WriteLine(string sendstr)
        {
            SerialPort.WriteLine(sendstr);
        }
          

        public override bool SendCommand(string command, ref string strRecAll, string DataToWaitFor, int timeout = 10)
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
                logger.Debug($"{SerialPort.PortName.ToUpper()}SendComd-->{command}");
                SerialPort.ReadTimeout = (timeout + 1) * 1000;
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
                        logger.Error(strRecAll);
                        logger.Error($"Waiting for:\"{DataToWaitFor}\" TimeOut({timeout}),FAIL!!!");
                        return false;
                    }
                    Thread.Sleep(1);
                }
                strRecAll = sReceiveAll;
                sReceiveAll = "";
                //logger.Info(strRecAll);
                logger.Info($"Waiting for:\"{DataToWaitFor}\" succeed!!");
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
                logger.Debug($"{SerialPort.PortName.ToUpper()}SendComdToFix-->{command}");
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
                        logger.Error(strRecAll);
                        logger.Error($"Waiting for:{DataToWaitFor} TimeOut({timeout}),FAIL!!!");
                        return false;
                    }
                    Thread.Sleep(1);
                }
                strRecAll = sReceiveAll;
                sReceiveAll = "";
                logger.Info(strRecAll);
                logger.Info($"Waiting for:{DataToWaitFor} succeed!!");
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
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
                logger.Fatal(ex.ToString());
            }
            finally
            {

                if (!string.IsNullOrEmpty(readExisting))
                {
                    logger.Debug(readExisting);
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