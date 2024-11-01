using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELE
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Share.settings["dataLink"] = textBox1.Text;
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\" + "settings.ini", "dataLink=" + textBox1.Text);
            this.Close();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Share.settings["dataLink"];
        }
    }
}
