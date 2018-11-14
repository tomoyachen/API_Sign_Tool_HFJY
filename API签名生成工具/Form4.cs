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
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
