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
    public partial class ReadyToTest : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;


        public ReadyToTest()
        {
            InitializeComponent();
         

 


        }

        private void TextBox1_KeyDown1(object sender, KeyEventArgs e)
        {
            // e=null时候，不执行&&后面的
            if (e != null && e.KeyCode != Keys.Enter) return;

            确定_Click(null, null);
        }


        private void InutMEASPOP_Shown(object sender, System.EventArgs e)
        {
            
        }

        private void USBConfirmDialog_Load(object sender, EventArgs e)
        {

            this.Size = new System.Drawing.Size(750, 401);
            // pictureBox1.Location = new System.Drawing.Point(this.Size.Width/2, this.Size.Height/2);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            ControlBox = false;

            label1.Font = new Font("Arial", 24, FontStyle.Bold);




        }



        public void ShowTip()
        {
            label1.Text = "确定开始/Được rồi để bắt đầu";

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
            确定_Click(null, null);


        }

        private void 确定_Click(object sender, EventArgs e)
        {
             
               


               
                    DialogResult = DialogResult.OK;
              


            
        }
    }
}
