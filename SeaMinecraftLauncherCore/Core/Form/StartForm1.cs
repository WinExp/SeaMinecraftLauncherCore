using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaMinecraftLauncherCore.Core.Form
{
    internal class StartForm1
    {
        internal static async Task<string> Start(string url)
        {
            return await Task.Run(() =>
            {
                string code = null;
                Thread thread = new Thread(() =>
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Form1 form1 = new Form1(url);
                    Application.Run(form1);
                    code = Form1.Code;
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                return code;
            });
        }
    }
}
