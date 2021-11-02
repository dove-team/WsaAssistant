using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WSATools.Libs
{
    public sealed class WSA
    {
        public static IEnumerable<string> PackageList { get; }
        private const string WSA_PACKAGE_NAME = "";
        static WSA()
        {
            PackageList = new [] { "Microsoft-Hyper-V", "HypervisorPlatform", "VirtualMachinePlatform" };
        }
        public static bool Init()
        {
            if (!RuntimeInformation.OSDescription.Contains("Windows 11"))
            {
                //MessageBox.Show("只支持Windows 11系统！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return false;
            }
            int count = 0;
            foreach (var package in PackageList)
            {
                if (!Check(package))
                    Install(package);
                else
                    count++;
            }
            if (count < 3)
            {
                if (MessageBox.Show("需要重启系统安装对应组件后进行安装！(确定后5s内重启系统，请保存好你的数据后进行重启！！！)", "提示",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    Command.Instance.Excute("shutdown -r -t 5", out _);
                return false;
            }
            return true;
        }
        private static void Install(string packageName)
        {
            Command.Instance.Excute($"DISM /Online /Enable-Feature /All /FeatureName:{packageName} /NoRestart", out _);
        }
        private static bool Check(string packageName)
        {
            Command.Instance.Excute($"DISM /Online /Get-FeatureInfo:{packageName}", out string message);
            return message.Contains("状态 : 已启用");
        }
        public static bool Pepair()
        {
            bool result = false;
            return result;
            RegistryKey key = default;
            try
            {
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                result = key.GetSubKeyNames().Any(x => x.Contains(WSA_PACKAGE_NAME));
                if (!result)
                {
                    key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
                    result = key.GetSubKeyNames().Any(x => x.Contains(WSA_PACKAGE_NAME));
                }
            }
            catch
            {
                key?.Close();
                key?.Dispose();
            }
            return result;
        }
    }
}