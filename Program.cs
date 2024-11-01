using System;
using System.IO;
using System.Windows.Forms;

namespace ELE
{
    internal static class Program
    {
        [STAThread] private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Directory.SetCurrentDirectory(Application.StartupPath);
            if(args.Length > 0)
                Application.Run(new Form1(args[0].Replace("\"", "")));
            else
                Application.Run(new Form1());
        }
    }
}