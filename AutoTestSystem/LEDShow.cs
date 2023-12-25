using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTestSystem
{
    public partial class LEDSHow : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;


        public LEDSHow()
        {
            InitializeComponent();
        }

        private void USBConfirmDialog_Load(object sender, EventArgs e)
        {
             
            this.Size = new System.Drawing.Size(698, 401);
            // pictureBox1.Location = new System.Drawing.Point(this.Size.Width/2, this.Size.Height/2);

        }

        public void ShowTip(string redtype)
        {
            this.pictureBox1.Visible = true;
            label1.Font = new Font("宋体", 22, FontStyle.Bold);
           
            if (redtype == "red")
            {
                this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.redled;
                label1.Text = "当前灯是否为红色\r\nĐèn hiện tại có màu đỏ không";
            }
            else if (redtype == "green")
            {
                this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.greenled;
                label1.Text = "当前灯是否为绿色\r\nĐèn hiện tại có màu xanh không";
            }
            else if (redtype == "blue")
            {
                this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.blueled;
                label1.Text = "当前灯是否为蓝色\r\nĐèn hiện tại có màu xanh không";
            }
            else if (redtype == "white")
            {
                this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.whiteled;
                label1.Text = "当前灯是否为白色\r\nĐèn hiện tại có màu trắng hay không";
            }
            else if (redtype == "off")
            {
                this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.offled;
                label1.Text = "当前灯是否关闭\r\nĐèn hiện tại có tắt không?";

            }
            else if (redtype == "nopic") {
                // this.pictureBox1.Visible = false;
                this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.whiteled;
                label1.Text = "白色呼吸灯是否闪亮?\r\nĐèn thở màu trắng có nhấp nháy không?";
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

        private void 确定_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                TextHandler.Invoke("yes");
                DialogResult = DialogResult.OK;
            }
        }

        private void lblNo_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                TextHandler.Invoke("no");
                DialogResult = DialogResult.OK;
            }
        }
    }
}
