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
    public partial class ShowFixtureTip2 : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;


        public ShowFixtureTip2()
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

            label1.Font = new Font("Arial", 22, FontStyle.Bold);




        }



        public void ShowTip(string txt = "")
        {
            if (txt == "")
            {
                label1.Text =
   "1.请确定iPod连接是否成功，如果不成功请点击 NO \r\n Xác định kết nối iPod có thành công hay không, nếu không nhấn NO" +
   "\r\n\r\n" +
   "2.请插入DevBoard，然后reboot，最后点击 YES \r\n Vui lòng chèn DevBoard, sau đó khởi động lại sản phẩm và cuối cùng nhấp vào YES";
            }
            else
            {
                label1.Text = txt;
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
                TextHandler.Invoke("");
                DialogResult = DialogResult.OK;
            }
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                TextHandler.Invoke("");
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}
