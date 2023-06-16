using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Configuration;

namespace Dev_Log
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> dicResults = new Dictionary<string, string>();
        string[] errList;
        string file = "";


        public Form1()
        {
            InitializeComponent();
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = dicResults[listBox1.SelectedItem.ToString()];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 開啟檔案
        private void fileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(*.log)|*.log|(*.txt*)|*.txt*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                textBox1.Text = file.ToString();
            }
        }

        // 檢視 log 錯誤清單
        private void fileRead_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;

            // 網址
            if (textBox1.Text.Contains("https") || textBox1.Text.Contains("http"))
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        if (textBox1.Text != null)
                        {
                            Stream stream = wc.OpenRead(textBox1.Text);
                            StreamReader sr = new StreamReader(stream);

                            string log = sr.ReadToEnd();
                            string[] data = log.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);

                            if (listBox1.Items.Count > 0)
                            {
                                listBox1.Items.Clear();
                            }

                            dicResults.Clear();

                            foreach (var d in data)
                            {
                                if (d.Contains("類型：ERR") == true)
                                {
                                    errList = d.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                                    dicResults.Add(errList[1] + " / " + errList[5], d);
                                    listBox1.Items.Add(errList[1] + " / " + errList[5]);
                                }
                            }
                            sr.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (listBox1.Items.Count > 0)
                    {
                        listBox1.Items.Clear();
                    }

                    MessageBox.Show("查無資料");
                }
            }
            // 本機
            else
            {
                try
                {
                    richTextBox1.Text = string.Empty;

                    StreamReader sr = new StreamReader(file);

                    string log = sr.ReadToEnd();
                    string[] data = log.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);

                    if (listBox1.Items.Count > 0)
                    {
                        listBox1.Items.Clear();
                    }

                    dicResults.Clear();

                    foreach (var d in data)
                    {
                        if (d.Contains("類型：ERR") == true)
                        {
                            errList = d.Split(new string[] { "\r\n" }, StringSplitOptions.None);                                                       
                            dicResults.Add(errList[1] + " / " + errList[5], d);
                            listBox1.Items.Add(errList[1] + " / " + errList[5]);                            
                        }
                    }
                    sr.Dispose();
                }
                catch (Exception ex)
                {
                    if (listBox1.Items.Count > 0)
                    {
                        listBox1.Items.Clear();
                    }

                    MessageBox.Show("查無資料");
                }
            }
        }

        // 
        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"E:\\.txt";
            string[] array = new string[listBox1.Items.Count];
            int i = 0;
            
            foreach(var lt in listBox1.Items)
            {
                string list = lt.ToString();
                array[i++] = list;
            };           

            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);

                //foreach(var txt in array)
                //{
                //    string text  = dicResults[txt.ToString()];
                //    sw.WriteLine(text);
                //    sw.Write("\r\n");
                //}

                for (int j = 0;j < listBox1.Items.Count; j++)
                {
                    sw.Write(dicResults[listBox1.Items[j].ToString()]);
                    sw.Write("\r\n");
                }

                sw.Close();
                fs.Close();

                MessageBox.Show("匯出成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show("匯出失敗");
            }
            
        }
    }
}

