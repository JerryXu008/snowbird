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
    public partial class USBConfirmDialog : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;


        public USBConfirmDialog()
        {
            InitializeComponent();
        }

        private void USBConfirmDialog_Load(object sender, EventArgs e)
        {
            label1.Text = "重新拔插U盘，完成后点击确定 / Đặt lại đĩa flash USB, và nhắp vào OK khi xong";
            this.Size = new System.Drawing.Size(698, 401);
            // pictureBox1.Location = new System.Drawing.Point(this.Size.Width/2, this.Size.Height/2);

        }

        public void ShowTip()
        {
            label1.Text = "Vui lòng cắm cáp nguồn và cáp mạng rồi nhấn OK/请插入电源和网线，然后点击确定";

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
                TextHandler.Invoke("");
                DialogResult = DialogResult.OK;
            }
        }
    }
}
