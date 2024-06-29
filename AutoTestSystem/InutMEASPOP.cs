using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTestSystem
{
    public partial class InutMEASPOP : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;
        static int initTimeCount = 60;

        System.Windows.Forms.Timer countdownTimer = new System.Windows.Forms.Timer();
        int counter;




        public string lowLimit = "";
        public string highLimit = "";
        public InutMEASPOP()
        {
            InitializeComponent();
            textBox1.Text = "";
            textBox1.Width = 200;
            textBox1.Height = 40;

            textBox1.KeyDown += TextBox1_KeyDown1;



            Thread thread = new Thread(() => {
               Thread.Sleep(1000);
                Action updateTitle = () => {
                    textBox1.Focus();
                };
                this.Invoke(updateTitle);

                
              

            });
            thread.IsBackground = true;
            thread.Start();

            
        }

        private void TextBox1_KeyDown1(object sender, KeyEventArgs e)
        {
            // e=null时候，不执行&&后面的
            if (e != null && e.KeyCode != Keys.Enter) return;

            确定_Click(null, null);
        }

         
            private void InutMEASPOP_Shown(object sender, System.EventArgs e)
        {
            textBox1.Focus();
        }

        private void USBConfirmDialog_Load(object sender, EventArgs e)
        {

            this.Size = new System.Drawing.Size(750, 401);
            // pictureBox1.Location = new System.Drawing.Point(this.Size.Width/2, this.Size.Height/2);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            ControlBox = false;

            label1.Font = new Font("Arial", 24, FontStyle.Bold);


            lblTime.Text = initTimeCount.ToString();
            lblTime.TextAlign = ContentAlignment.MiddleCenter;

            // 设置标签的位置和大小，这里的100, 50是标签的宽度和高度
            lblTime.SetBounds(textBox1.Bounds.X+100,textBox1.Bounds.Y+30,textBox1.Bounds.Width,textBox1.Bounds.Height);

            // 设置标签的字体和大小
            lblTime.Font = new Font("Arial", 12, FontStyle.Bold);



            counter = initTimeCount;

            countdownTimer.Interval = 1000; // Timer will tick every second
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Start(); // Start the timer

        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            if (counter > 0)
            {
                counter--;
                lblTime.Text = counter.ToString();
            }
            else
            {
                countdownTimer.Stop();
                

            }
        }


        public void ShowTip(string text="")
        {
            if (text == "")
            {
                label1.Text = "请输入阻抗值\r\nVui lòng nhập giá trị trở kháng:";
            }
            else {
                label1.Text = text;

            }

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void USBConfirmDialog_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Keys.Enter == (Keys)e.KeyChar)
            {
                if (null != TextHandler)
                {
                    TextHandler.Invoke("");
                    DialogResult = DialogResult.OK;
                }
            }
        }


        public void CloseDia()
        {
            确定_Click(null, null);


        }

        private void 确定_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {


                var input = textBox1.Text;

           
                double doubleValue;

                if (double.TryParse(input, out doubleValue))
                {

                    if (lowLimit != "") {
                        if (doubleValue < double.Parse(lowLimit)) {
                            return;
                        }
                    
                    }


                    if (highLimit != "")
                    {
                        if (doubleValue > double.Parse(highLimit))
                        {
                            return;
                        }

                    }



                    TextHandler.Invoke(input);
                    DialogResult = DialogResult.OK;
                    countdownTimer.Stop();
                }
                else
                {
                    Console.WriteLine(input + " 不是一个数字");
                }
               

            }
        }
    }
}
