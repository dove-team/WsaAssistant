using Microsoft.VisualBasic;
using System;
using System.IO;
using WsaAssistant.Libs;

namespace WsaAssistant.Background
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = string.Join(" ", args).Trim();
                if (File.Exists(path))
                {
                    if (Interaction.MsgBox("是否安装？", MsgBoxStyle.YesNo, "INSTALL") == MsgBoxResult.Yes)
                    {
                        if (Adb.Instance.TryConnect())
                        {
                            if (!Adb.Instance.Install(path))
                                Interaction.MsgBox("安装失败！", MsgBoxStyle.Critical, "ERROR");
                            else
                                Interaction.MsgBox("安装成功！", MsgBoxStyle.Information, "SUCCESS");
                        }
                        else
                            Interaction.MsgBox("未连接设备！请检查子系统相关设置", MsgBoxStyle.Critical, "ERROR");
                    }
                    //else
                    //Interaction.MsgBox("安装失败！未找到文件:" + Path.GetFileName(path), MsgBoxStyle.Critical, "ERROR");
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Start Host", ex);
                Interaction.MsgBox("安装失败！未知错误：" + ex.Message, MsgBoxStyle.Critical, "ERROR");
            }
        }
    }
}