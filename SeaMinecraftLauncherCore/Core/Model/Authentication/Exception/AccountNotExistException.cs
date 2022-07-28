using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Exception
{
    public class AccountNotExistException : System.Exception
    {
        public AccountNotExistException() : base("抛出了一个 AccountNotExistException。") { }

        public AccountNotExistException(string message) : base(message) { }
    }
}
