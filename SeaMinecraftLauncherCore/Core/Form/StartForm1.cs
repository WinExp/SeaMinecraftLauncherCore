using System;
using System.Windows.Forms;

namespace SeaMinecraftLauncherCore.Core.Form
{
    internal class StartForm1
    {
        [STAThread]
        internal static string Start(string url)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1(url);
            Application.Run(form1);
            return Form1.Code;
        }
    }
}
