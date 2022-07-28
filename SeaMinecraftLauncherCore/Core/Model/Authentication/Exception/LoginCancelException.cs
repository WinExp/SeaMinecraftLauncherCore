using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Exception
{
    public class LoginCancelException : System.Exception
    {
        public LoginCancelException() : base("抛出了一个 LoginCancelException。") { }

        public LoginCancelException(string message) : base(message) { }
    }
}
