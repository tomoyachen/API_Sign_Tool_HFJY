using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace API签名生成工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.Text = "API签名生成工具";
            init();

        }



        private void init() {
            textBox3.Text = "${key}";
            textBox3.Text = GetConfigValue("key_textBox", "${key}");
            checkBox1.CheckState = CheckState.Unchecked;
            if (GetConfigValue("UrlDecode_checkBox", "false") == "true")
            {
                checkBox1.CheckState = CheckState.Checked;
            }
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            List<string> lis = new List<string>();
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                string line = textBox1.Lines[i].ToString().Trim();//清除左右所有空格
                line = line.Replace("\'", "").Replace("\"", ""); //去掉引号
                if (line.Length != 0)//非空行才写入数组
                { 
                    lis.Add(line);
                }
                
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < lis.Count; i++)
            {
                string line = lis[i];
                //string[] tmpArr = line.Split('\t'); //根据tab分割
                string[] tmpArr = line.Split(new char[4] { '\t', ':', ',', '=' });  //根据tab分割
                if (tmpArr.Length >= 2) //大于2段才写入字典
                { 
                    dict.Add(tmpArr[0], tmpArr[1]);
                }

            }


            /*
            foreach (KeyValuePair<string, string> kvp in dict)  //输出字典调试
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            */

            //生成query字符串，并拼接key值
            string queryStr = http_build_query(dict) + textBox3.Text;
           
            if (checkBox1.CheckState == CheckState.Checked)
            {
                textBox2.Text = HttpUtility.UrlDecode(queryStr); //urldecode
            }
            else {
                textBox2.Text = queryStr; 
            }


            //记录日志
            string log = "";
            log += "Key值: " + textBox3.Text + ", UrlDecode: " + checkBox1.CheckState.ToString() + "\r\n";
            log += "源内容: " + textBox1.Text + "\r\n";
            log += "生成后: " + textBox2.Text;
            new myLog().WriteLog(log);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1_Click(null, null);
            String md5Str = GetMD5(textBox2.Text);

            //记录日志
            new myLog().WriteLog("MD5: " + md5Str);

            Form4 md5Form = new Form4(md5Str);
            md5Form.StartPosition = FormStartPosition.CenterParent;
            md5Form.ShowDialog();

        }


 
        /// 读取指定key的值
        public static string GetConfigValue(string key, string default_value)
        {
            if (System.Configuration.ConfigurationSettings.AppSettings[key] == null)
                return default_value;
            else
                return System.Configuration.ConfigurationSettings.AppSettings[key].ToString();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //关闭窗口事件
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭时执行
        }

        private void 说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 about = new Form2();
            about.StartPosition = FormStartPosition.CenterParent;
            about.ShowDialog();
        }

        private void 重置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            init();

        }


        //实现PHP中的http_build_query方法
        public static string http_build_query(Dictionary<string, string> dict = null)
        {
            if (dict == null)
            {
                return "字典为空！";
            }
            var builder = new UriBuilder();
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var item in dict.Keys)
            {
                query[item] = dict[item];
            }
            return query.ToString().Trim('?');
        }

        //生成MD5方法
        private static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;


        }

    }
}
