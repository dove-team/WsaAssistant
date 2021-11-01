using System.Collections.Generic;
using System.Windows.Forms;

namespace WSATools.Untils
{
    public sealed class WSA
    {
        public static List<string> PackageList { get; }
        static WSA()
        {
            PackageList = new List<string> { "Microsoft-Hyper-V", "HypervisorPlatform", "VirtualMachinePlatform" };
        }
        public static void Init()
        {
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
                if (MessageBox.Show("提示", "需要重启系统安装对应组件后进行安装！(确定后5s内重启系统，请保存好你的数据后进行重启！！！)",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    Command.Instance.Excute("shutdown -r -t 5", out _);
            }
        }
        public static void Install(string packageName)
        {
            Command.Instance.Excute($"DISM /Online /Enable-Feature /All /FeatureName:{packageName} /NoRestart", out _);
        }
        public static bool Check(string packageName)
        {
            Command.Instance.Excute($"DISM /Online /Get-FeatureInfo:{packageName}", out string message);
            return message.Contains("状态 : 已启用");
        }
    }
}