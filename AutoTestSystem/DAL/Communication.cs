using System;
using System.Threading;

namespace AutoTestSystem.DAL
{
    public abstract class Communication : IDisposable
    {
        // 是否连接成功
        public bool IsOpen = false;

        public string ConnectStatus = "";
        public byte[] Response = new byte[6];

        // 打印log路径
        public string logPath = "";

        // 打印log互斥锁
        public readonly object wLock = new object();

        // 所有接收
        public string sReceiveAll = "";

        // 接收到输入数据时,发送事件
        //public AutoResetEvent InputEvent = new AutoResetEvent(false);
      //  public ManualResetEvent InputEvent = new ManualResetEvent(true);

        public string HostIP { get; set; }
        public int Port { get; set; } = 22;
        public string Username { get; set; }
        public string Password { get; set; }

        public abstract bool Open();

        public abstract void Close();

        public abstract void Dispose();

        public abstract void Write(string data);

        public abstract string Read();

        /// <summary>
        /// 发送命令，超时时间内得不到期待的字符串则返回失败和输出
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="strRecAll">命令反馈</param>
        /// <param name="DataToWaitFor">等待期待的字符串</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public abstract bool SendCommand(string command, ref string strRecAll, string DataToWaitFor, int timeout = 10);

        // 父类虚方法，子类可重写可不重写，重写用override关键字。virtual方法必须有方法主体。
        public virtual bool Open(string Expstr) { return false; }

        public virtual void Write(byte[] data)
        {
        }

        public virtual void WriteLine(string data)
        {
        }
    }
}