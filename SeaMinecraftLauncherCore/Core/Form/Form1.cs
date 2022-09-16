using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SeaMinecraftLauncherCore
{
    internal partial class Form1 : Form
    {
        private string _url;
        internal static string Code;

        internal Form1(string url)
        {
            InitializeComponent();
            _url = url;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri(_url);
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri.Contains("https://login.live.com/oauth20_desktop.srf"))
            {
                Code = e.Url.AbsoluteUri.Remove(0, e.Url.AbsoluteUri.IndexOf("?code=") + 6);
                this.Close();
            }
        }
    }
}
