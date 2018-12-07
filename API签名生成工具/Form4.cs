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
    public partial class Form4 : Form
    {
        public Form4(String md5Str)
        {
            InitializeComponent();
            textBox1.Text = md5Str;
            textBox2.Text = md5Str.ToUpper();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false; //使最大化窗口失效
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; //下一句用来禁止对窗口大小进行拖拽
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
