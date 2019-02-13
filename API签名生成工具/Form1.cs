using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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
            textBox3.Text = GetConfigValue("key_textBox", "");
            checkBox1.CheckState = CheckState.Unchecked;
            
            if (GetConfigValue("UrlEncode_checkBox", "true") == "true")
            {
                checkBox1.CheckState = CheckState.Checked;
            }
            checkBox2.CheckState = CheckState.Unchecked;
            if (GetConfigValue("UrlEncodeFun_checkBox", "false") == "true")
            {
                checkBox2.CheckState = CheckState.Checked;
            }
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //记录日志
            string log = "";
            log += "Key值: " + textBox3.Text + ", UrlEncode: " + checkBox1.CheckState.ToString() + "\r\n";
            log += "源内容: " + textBox1.Text + "\r\n";
            //--------------------------

            List<string> lis = new List<string>();
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                string line = textBox1.Lines[i].ToString().Trim();//清除左右所有空格
                line = line.Replace("\'", "").Replace("\"", ""); //去掉引号
                line = line.Trim(',');//清除左右两边的逗号
                line = line.Replace(" => ", "\t").Replace(" =>", "\t").Replace("=> ", "\t").Replace("=>", "\t"); //兼容php语言
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
                try
                {
                    if (tmpArr.Length >= 2) //大于2段才写入字典
                    {
                        dict.Add(tmpArr[0], tmpArr[1]);
                    }
                    else if (tmpArr.Length == 1)
                    {
                        dict.Add(tmpArr[0], "");
                    }
                }
                catch {
                    MessageBox.Show("原内容存在相同的参数名: ["+ tmpArr[0] + "]！");
                }
                

            }


            //格式化textbox1的内容
            string textBoxFormat = "";
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                textBoxFormat += (kvp.Key + "\t" + kvp.Value + "\r\n");
            }
            textBox1.Text = textBoxFormat;


            /*
            foreach (KeyValuePair<string, string> kvp in dict)  //输出字典调试
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            */
            Boolean ifEncode = false;
            Boolean ifJmeterFun = false;
            if (checkBox1.CheckState == CheckState.Checked)
            {
                ifEncode = true;
            }else if(checkBox2.CheckState == CheckState.Checked)
            {
                ifJmeterFun = true;
            }

            textBox2.Text = http_build_query(dict, ifEncode, ifJmeterFun) + textBox3.Text;

            //记录日志
            log += "格式化: " + textBox1.Text + "\r\n";
            log += "生成后: " + textBox2.Text;
            //--------------------------
            if (GetConfigValue("writeToLog", "false") == "true")
            {
                new myLog().WriteLog(log);
            }
            //--------------------------

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

        //重写urlencode大写
        public static string UrlEncode(string str)
        {
            return !string.IsNullOrEmpty(str) ?
                WebUtility.UrlEncode(str)
                .Replace("+", "%20")
                .Replace("*", "%2A")
                .Replace("~", "%7E")
                .Replace("!", "%21")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29")
                : str;
        }


        //实现PHP中的http_build_query方法
        public static string http_build_query(Dictionary<string, string> dict, Boolean ifEncode, Boolean ifJmeterFun)
        {
            if (dict == null)
            {
                return "字典为空！";
            }

            //排序
            dict = dict.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

            string result = "";
            foreach (KeyValuePair<string, string> kvp in dict)  //输出字典调试
            {
                
                string tmp = kvp.Value;
                if (ifEncode) //自动编码 支持生成MD5
                {
                    //tmp = HttpUtility.UrlEncode(tmp, System.Text.Encoding.UTF8); //符号不会转义 弃用
                    tmp = UrlEncode(tmp);
                }
                if (ifJmeterFun) //加上urlencode函数，用于Jmeter使用
                {
                    tmp = "${__urlencode(" + tmp + ")}";
                }
                result += (kvp.Key + "=" + tmp + "&");
            }

            return result.Trim('&');
        }


        //生成MD5方法
        private static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider(); //实例化一个md5对像
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString); //加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++) //通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            {
                byte2String += targetData[i].ToString("x2"); // 将得到的字符串使用十六进制类型格式。X 表示大写， x 表示小写， X2和x2表示不省略首位为0的十六进制数字。
            }

            return byte2String;


        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                button2.Enabled = true;
                checkBox2.Enabled = false;
            }
            else {
                button2.Enabled = false;
                checkBox2.Enabled = true;
            }
        }

        private void listen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                //this.Close();
            }
        }

    }
}
