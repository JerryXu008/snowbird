using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AutoTestSystem.Model
{
    /// <summary>
    /// 加载用例，生成序列
    /// </summary>
    public class LoadSeq
    {
        public Sequence tempSeq = new Sequence();   //! 正在加载的seq内容
        private Items tempItem = null;      //!当前正在加载的测试项
        public List<Sequence> tempSequencesList = new List<Sequence>();  //!已经加载的测试用例列表,
        public int ReadLine = 0;  //!当前正在读取的行数
        private string TestCasePath { get; set; }  //测试用例路径

        public LoadSeq(string _testCasePath)
        {
            TestCasePath = _testCasePath;
        }

        /// <summary>
        /// 把excel文件的第一个sheet表读取到DataTable
        /// </summary>
        public DataTable GetSheetToDT(string STATIONNAME)
        {
            DataTable dt = new DataTable();
            try
            {
                //连接字符串 Office 07及以上版本 不能出现多余的空格 而且分号注意
                string connstring = "Provider=Microsoft.Ace.OLEDB.12.0;" + "Data Source=" + TestCasePath + ";" + "Extended Properties=\'Excel 12.0 Xml;HDR=Yes;IMEX=1;\'";
                using (OleDbConnection conn = new OleDbConnection(connstring))
                {
                    conn.Open();
                    DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    //string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                    for (int i = 0; i < sheetsName.Rows.Count; i++)
                    {
                        string sheetName = sheetsName.Rows[i]["TABLE_NAME"].ToString();
                        Global.SaveLog($"获取Excel用例文件sheetName：{sheetName}");
                    }
                    string sql = string.Format("SELECT * FROM [{0}$]", STATIONNAME);
                    using (OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring))
                    {
                        ada.Fill(dt);
                    }
                    return dt;
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                string linkUrl = "http://www.microsoft.com/zh-CN/download/confirmation.aspx?id=23734";
                string infoMessage = "请安装AccessDatabaseEngine,下载链接:\r\n " + linkUrl;
                result = MessageBox.Show(infoMessage + "\n\n点击 '确定' 按钮复制链接", "Error", buttons);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Clipboard.SetText(linkUrl);
                }
                throw ex;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public DataTable ExcelToDatatable()
        {
            try
            {
                DataTable dt = new DataTable();
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + TestCasePath + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'";
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    using (OleDbCommand comm = new OleDbCommand())
                    {
                        string sheetName = "Sheet1";
                        comm.CommandText = "Select * from [" + sheetName + "$]";
                        comm.Connection = conn;
                        using (OleDbDataAdapter da = new OleDbDataAdapter())
                        {
                            da.SelectCommand = comm;
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        ///// <summary>
        ///// 将Csv读入DataTable
        ///// </summary>
        ///// <param name="filePath">csv文件路径</param>
        ///// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        ///// <param name="k">可选参数表示最后K行不算记录默认0</param>
        public DataTable CsvToDT(int n, DataTable dt) //这个dt 是个空白的没有任何行列的DataTable
        {
            String csvSplitBy = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            using (StreamReader reader = new StreamReader(TestCasePath, System.Text.Encoding.Default, false))
            {
                int i = 0, m = 0;
                reader.Peek();
                while (reader.Peek() > 0)
                {
                    m = m + 1;
                    string str = reader.ReadLine();
                    if (m >= n + 1)
                    {
                        if (m == n + 1) //如果是字段行，则自动加入字段。
                        {
                            MatchCollection mcs = Regex.Matches(str, csvSplitBy);
                            foreach (Match mc in mcs)
                            {
                                dt.Columns.Add(mc.Value); //增加列标题
                            }
                        }
                        else
                        {
                            MatchCollection mcs = Regex.Matches(str, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                            i = 0;
                            System.Data.DataRow dr = dt.NewRow();
                            foreach (Match mc in mcs)
                            {
                                dr[i] = mc.Value;
                                i++;
                            }
                            dt.Rows.Add(dr);  //DataTable 增加一行
                        }
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 根据datatable获得列名
        /// </summary>
        public string[] GetColumnsByDataTable(DataTable dt)
        {
            string[] strColumns = null;

            if (dt.Columns.Count > 0)
            {
                int columnNum = 0;
                columnNum = dt.Columns.Count;
                strColumns = new string[columnNum];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strColumns[i] = dt.Columns[i].ColumnName;
                }
            }
            return strColumns;
        }

        /// <summary>
        /// 用反射的方式修改实例的变量值。给变量赋值。
        /// </summary>
        public void SetItems(Items tempStep, string varName, string varNewValue)
        {
            try
            {
                Type myType = typeof(Items);
                FieldInfo myFieldInfo = myType.GetField(varName, BindingFlags.Public | BindingFlags.Instance);
                //Global.SaveLog("GetItems:" + varName + "*******" + varNewValue.Trim());
                if (myFieldInfo != null)
                {
                    if (string.IsNullOrEmpty(varNewValue))
                    {
                        myFieldInfo.SetValue(tempStep, "");
                    }
                    else
                    {
                        myFieldInfo.SetValue(tempStep, varNewValue.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Global.SaveLog(ex.ToString(), 2);
                throw;
            }
        }

        public void GetItems(DataTable dataTableOfTestCase, string[] itemHeader, int rowIndex, string columOfItemName)
        {
            try
            {
                string itemname = dataTableOfTestCase.Rows[rowIndex][columOfItemName].ToString();
                if (!String.IsNullOrEmpty(itemname))
                {
                    tempItem = new Items { testNumber = tempSeq.TotalNumber };
                    for (int i = 0; i < itemHeader.Length; i++)
                    {
                        string varName = itemHeader[i];
                        SetItems(tempItem, varName, dataTableOfTestCase.Rows[rowIndex][varName].ToString());
                    }
                    tempSeq.TotalNumber++;
                    tempSeq.SeqItems.Add(tempItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetItems:{ex.ToString()}", "ERROR!");
                throw;
            }
        }

        public List<Sequence> GetSeq(DataTable dataTableOfTestCase, string[] itemHeader, string coloumOfSeqName, string columOfItemName)
        {
            try
            {
                for (int i = 0; i < dataTableOfTestCase.Rows.Count; i++)
                {
                    ReadLine = i;
                    string seqName = dataTableOfTestCase.Rows[i][coloumOfSeqName].ToString();
                    if (!String.IsNullOrEmpty(seqName))
                    {
                        tempSeq = new Sequence
                        {
                            SeqName = dataTableOfTestCase.Rows[i][coloumOfSeqName].ToString().Trim()
                        };
                        tempSeq.TestSerial = tempSequencesList.Count;
                        tempSequencesList.Add(tempSeq);
                    }
                    GetItems(dataTableOfTestCase, itemHeader, i, columOfItemName);
                    Console.WriteLine(">>>>>>>>>>>:"+"看看多慢");
                }
                return tempSequencesList.ToList(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetSeq:{ex.ToString()}", "ERROR!");
                throw;
            }
            finally
            {
                tempSequencesList.Clear();
            }
        }
    }
}