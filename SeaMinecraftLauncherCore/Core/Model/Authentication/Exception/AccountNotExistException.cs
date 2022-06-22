using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Exception
{
    public class AccountNotExistException : System.Exception
    {
        public AccountNotExistException() : base() { }

        public AccountNotExistException(string message) : base(message) { }
    }
}
