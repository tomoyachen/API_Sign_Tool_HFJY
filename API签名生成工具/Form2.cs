using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace API签名生成工具
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Text = "说明";
            //textBox1.Enabled = false;
            //textBox1.ReadOnly = true;
            textBox2.Enabled = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            String help = null;
            help += @"使用方式：" + "\r\n";
            help += @"1.本工具用于生成sign参数" + "\r\n";
            help += @"2.也可以对sign明文进行md5加密" + "\r\n";
            help += "\r\n备注: \r\n";
            help += @"config文件可以配置默认参数" + "\r\n";
            help += @"原内容中 => \t : , = 会被转义";
            textBox1.Text = help;

            String author = null;
            author += @"tomoya_chen" + "\r\n";
            author += @"海风教育 内部服务组";
            textBox2.Text = author;
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://blog.csdn.net/tomoya_chen");
        }


        private void listen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
