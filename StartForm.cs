using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrianStopTimer
{
    public partial class StartForm : Form
    {
        Thread th;

        public StartForm()
        {
            InitializeComponent();
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            th = new Thread(LoadData) { IsBackground = true };
            th.Start();
        }
        [STAThread]
        private void LoadData()
        {
            string[] railways = Directory.GetFiles(".\\RailwayDatas", "*.rdf", SearchOption.TopDirectoryOnly);
            string[] emus = Directory.GetFiles(".\\EMUDatas", "*.emu", SearchOption.TopDirectoryOnly);
            Form1 frm = new Form1();

        }
    }
}