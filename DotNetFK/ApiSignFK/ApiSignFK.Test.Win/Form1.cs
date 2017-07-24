using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApiSignFK.Test.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ApiHttpHelper api = new ApiHttpHelper("http://localhost:8080");

        private void button1_Click(object sender, EventArgs e)
        {
           

            var d = new Dictionary<string, string>();
            d.Add("id", "3");
            d.Add("type", "ios");
            d.Add("name", "张三");
            d.Add("address", "北京");

            var response = api.HttpGet("api/v1/user", d);
            tbox.Text = response.Content;
        }

        private void button2_Click(object sender, EventArgs e)
        {
           var resp= api.HttpPost("api/v1/add", new {Name="张三",Age=34,Address="北京" });
            tbox.Text = resp.Content;
        }
    }
}
