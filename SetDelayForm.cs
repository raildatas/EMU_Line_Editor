using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELE
{
    public partial class SetDelayForm : Form
    {
        string station;

        public SetDelayForm()
        {
            InitializeComponent();
        }

        public void ShowDialog(string station)
        {
            this.station = station;
            base.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbx_delay.Text))
                tbx_delay.Text = "0";
            Share.delayformflag = true;
            Share.delayformdata1 = int.Parse(tbx_delay.Text);
            this.Close();
        }

        private void SetDelayForm_Load(object sender, EventArgs e)
        {
            if (station == "")
            {
                this.lbl_info.Text = "请设置列车编号";
                label1.Visible = false;
            }
            else
                this.lbl_info.Text = String.Format("请设置\"{0}\"站的正晚点信息\r\n早点为负，正点为0，晚点为正", station);
        }
    }
}