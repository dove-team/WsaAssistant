using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WSATools.Libs
{
    public sealed class Drives
    {
        private static Drives instance;
        public static Drives Instance
        {
            get
            {
                if (instance == null)
                    instance = new Drives();
                return instance;
            }
        }
        private Drives() { }
        public async Task InstallOpen()
        {
            var packages = await AppX.Instance.GetPackages("9nqpsl29bfff");

        }
    }
}