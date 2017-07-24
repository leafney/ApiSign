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

        private void button3_Click(object sender, EventArgs e)
        {
            var d = new Dictionary<string, string>();
            d.Add("id", "3");
            d.Add("name", "诸葛亮");


            var res = api.HttpGetV2("api/v2/user",d);
            tbox.Text = res.Content;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var res = api.HttpPostV2("api/v2/user", new { Name = "Wang 五"});
            tbox.Text = res.Content;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var res = api.HttpPutV2("api/v2/user", new { Name = "刘备" });
            tbox.Text = res.Content;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var d = new Dictionary<string, string>();
            d.Add("id", "5");

            var res = api.HttpDeleteV2("api/v2/user", d);
            tbox.Text = res.Content;
        }
    }
}
