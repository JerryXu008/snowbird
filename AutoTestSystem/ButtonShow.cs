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
    public partial class ButtonShow : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;


        public ButtonShow()
        {
            InitializeComponent();
        }

        private void USBConfirmDialog_Load(object sender, EventArgs e)
        {
            
            this.Size = new System.Drawing.Size(750, 401);
            // pictureBox1.Location = new System.Drawing.Point(this.Size.Width/2, this.Size.Height/2);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            ControlBox = false;

           label1.Font = new Font("Arial", 24, FontStyle.Bold);

            btnOK.Visible = false;

            
        }

        public void ShowPressTip() {

            label1.Text = "请按下按钮/vui lòng nhấn nút";
        }
        public void ShowReleaseTip()
        {

            label1.Text = "请释放按钮/hãy thả nút ra";
        }

        public void ShowTip(string type) {
            if (type == "1")
            {
                ShowPressTip();

            }
            else {
                ShowReleaseTip();
            }
        
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void USBConfirmDialog_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (Keys.Enter == (Keys)e.KeyChar)
            //{
            //    if (null != TextHandler)
            //    {
            //        TextHandler.Invoke("");
            //        DialogResult = DialogResult.OK;
            //    }
            //}
        }


        public void CloseDia() {
            确定_Click(null, null);


        }

        private void 确定_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                TextHandler.Invoke("");
                DialogResult = DialogResult.OK;
            }
        }
    }
}
