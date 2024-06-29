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
    public partial class TestingNoise : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;
        System.Windows.Forms.Timer countdownTimer = new System.Windows.Forms.Timer();

        static int  initTimeCount = 60;

        int counter;

        bool isFirst = true;
        public TestingNoise()
        {
            InitializeComponent();




        }

        void SetUI() {




            this.Size = new System.Drawing.Size(750*2, 401*2);
            // pictureBox1.Location = new System.Drawing.Point(this.Size.Width/2, this.Size.Height/2);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            ControlBox = false;

            label1.Font = new Font("Arial", 24, FontStyle.Bold);



            lblTime.Text = initTimeCount.ToString();
            lblTime.TextAlign = ContentAlignment.MiddleCenter;

            // 设置标签的位置和大小，这里的100, 50是标签的宽度和高度
            lblTime.SetBounds((this.ClientSize.Width - 100) / 2, (this.ClientSize.Height - 50) / 2-250, 100, 50);

            // 设置标签的字体和大小
            lblTime.Font = new Font("Arial", 12, FontStyle.Bold);



           groupBox1.SetBounds((this.ClientSize.Width) / 2-300, (this.ClientSize.Height) / 2 - 150, 600, 300);


            


            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;

        }

        private void TextBox1_KeyDown1(object sender, KeyEventArgs e)
        {
            // e=null时候，不执行&&后面的
            if (e != null && e.KeyCode != Keys.Enter) return;

             
        }


        private void InutMEASPOP_Shown(object sender, System.EventArgs e)
        {

        }

        private void USBConfirmDialog_Load(object sender, EventArgs e)
        {
            

            SetUI();

            counter = initTimeCount;

            countdownTimer.Interval = 1000; // Timer will tick every second
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Start(); // Start the timer

        }

        private void TestingNoise_Shown(object sender, System.EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
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
                lblTime.Text = "测试时间完成，请做出你的判断!";

            }
        }


        public void ShowTip()
        {
            label1.Text = "While testing, pay attention to what you hear and make your own judgment\r\nTrong khi kiểm tra, hãy chú ý đến những gì bạn nghe được và đưa ra phán đoán của riêng mình";

        }

        public void ShowTip(string text)
        {
            label1.Text = text;

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
            
 

        }

 
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var radio = (RadioButton)sender;
            if (isFirst) { 
                isFirst = false;
                return;
            
            }
            if (radio.Checked)
            {
                if (radio == radioButton1)
                {
                    TextHandler("0");
                    DialogResult = DialogResult.OK;

                    countdownTimer.Stop();


                }
                else if (radio == radioButton2)
                {
                    TextHandler("1");
                   DialogResult = DialogResult.OK;
                    countdownTimer.Stop();
                }
                else {
                    TextHandler("2");
                DialogResult = DialogResult.OK;

                    countdownTimer.Stop();

                }


              
            }
        }
    }
}
