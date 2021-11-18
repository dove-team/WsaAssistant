using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Linq;
using WSATools.Libs;

namespace WSATools.Background
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var fileName = Path.GetFileName(Environment.ProcessPath);
                var path = args.FirstOrDefault();
                LogManager.Instance.LogInfo(path);
                if (Adb.Instance.Connect)
                {
                    if (!Adb.Instance.Install(path))
                        Interaction.MsgBox("安装失败！", MsgBoxStyle.Critical, "ERROR");
                }
                else
                    Interaction.MsgBox("未连接设备！请检查子系统相关设置", MsgBoxStyle.Critical, "ERROR");
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Start Host", ex);
            }
        }
    }
}