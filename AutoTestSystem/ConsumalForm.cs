using AutoTestSystem.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTestSystem
{
    public partial class ConsumalForm : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;

        public string StationName = "";


        public ConsumalForm()
        {
            InitializeComponent();
        }

        private void PassWordForm_Load(object sender, EventArgs e)
        {

        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            //获取当前耗材:
            try
            {
                var name = Global.STATIONNAME;
                var NO = Global.STATIONNO;

                if (name == "MBLT" || name == "MBFT")
                {
                    lblCableText.Visible = true;
                    lblCableNum.Visible = true;
                    btnCable.Visible = true;

                    lblTypeCText.Visible = false;
                    lblTypeCNum.Visible = false;
                    lblETHText.Visible = false;
                    lblETHNum.Visible = false;
                    btnTypeC.Visible = false;
                    btnETH.Visible = false;

                    //查看耗材
                    var url = Global.VersionMURL + "/consumables/view/hornbill/" + name + "/" + NO + "/" + "Probe";
                    var client = new HttpClient();

                    HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                    string result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.Contains("ok"))
                    {
                        var num = JObject.Parse(result)["num"].ToString();
                        lblCableNum.Text = num;

                    }
                    else
                    {
                        var msg = JObject.Parse(result)["msg"].ToString();
                        lblCableNum.Text = "0";
                        MessageBox.Show(msg);
                    }




                }
                else if (name == "SFT" || name == "SRF" || name == "RTT")
                {
                    lblCableText.Visible = false;
                    lblCableNum.Visible = false;
                    btnCable.Visible = false;

                    lblTypeCText.Visible = true;
                    lblTypeCNum.Visible = true;
                    lblETHText.Visible = true;
                    lblETHNum.Visible = true;
                    btnTypeC.Visible = true;
                    btnETH.Visible = true;

                    //查看耗材
                    var url = Global.VersionMURL + "/consumables/view/hornbill/" + name + "/" + NO + "/" + "ETH";
                    var client = new HttpClient();

                    HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                    string result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.Contains("ok"))
                    {
                        var num = JObject.Parse(result)["num"].ToString();
                        lblETHNum.Text = num;

                    }
                    else
                    {
                        var msg = JObject.Parse(result)["msg"].ToString();
                        lblETHNum.Text = "0";
                        MessageBox.Show(msg);
                    }


                    //查看耗材
                    url = Global.VersionMURL + "/consumables/view/hornbill/" + name + "/" + NO + "/" + "TypeC";
                    client = new HttpClient();

                    httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                    result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.Contains("ok"))
                    {
                        var num = JObject.Parse(result)["num"].ToString();
                        lblTypeCNum.Text = num;

                    }
                    else
                    {
                        var msg = JObject.Parse(result)["msg"].ToString();
                        lblTypeCNum.Text = "0";
                        MessageBox.Show(msg);
                    }


                }














            }
            catch (Exception ex)
            {
                lblCableNum.Text = "0";
                MessageBox.Show(ex.Message);
            }

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                // TextHandler.Invoke("");
                try
                {
                    var name = Global.STATIONNAME;
                    var NO = Global.STATIONNO;
                    //清理耗材
                    var url = Global.VersionMURL + "/consumables/clear/hornbill/" + name + "/" + NO + "/" + "Probe";
                    var client = new HttpClient();

                    HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                    string result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.Contains("ok"))
                    {
                        lblCableNum.Text = "0";
                        MessageBox.Show("清理成功!");
                    }
                    else
                    {
                        var msg = JObject.Parse(result)["msg"].ToString();
                        MessageBox.Show(msg);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }






                // this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TextHandler.Invoke("Cancel");
            // DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void TxtString_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Keys.Enter == (Keys)e.KeyChar)
            {
                if (null != TextHandler)
                {
                    TextHandler.Invoke("Cancel");
                    this.Close();
                    //  DialogResult = DialogResult.OK;
                }
            }
        }

        private void btnETH_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                // TextHandler.Invoke("");
                try
                {
                    var name = Global.STATIONNAME;
                    var NO = Global.STATIONNO;
                    //清理耗材
                    var url = Global.VersionMURL + "/consumables/clear/hornbill/" + name + "/" + NO + "/" + "ETH";
                    var client = new HttpClient();

                    HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                    string result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.Contains("ok"))
                    {
                        lblETHNum.Text = "0";
                        MessageBox.Show("清理成功!");
                    }
                    else
                    {
                        var msg = JObject.Parse(result)["msg"].ToString();
                        MessageBox.Show(msg);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }






                // this.Close();
            }
        }

        private void btnTypeC_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                // TextHandler.Invoke("");
                try
                {
                    var name = Global.STATIONNAME;
                    var NO = Global.STATIONNO;
                    //清理耗材
                    var url = Global.VersionMURL + "/consumables/clear/hornbill/" + name + "/" + NO + "/" + "TypeC";
                    var client = new HttpClient();

                    HttpResponseMessage httpResponse = client.GetAsync(url).GetAwaiter().GetResult();
                    string result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.Contains("ok"))
                    {
                        lblTypeCNum.Text = "0";
                        MessageBox.Show("清理成功!");
                    }
                    else
                    {
                        var msg = JObject.Parse(result)["msg"].ToString();
                        MessageBox.Show(msg);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }






                // this.Close();
            }
        }
    }
}
