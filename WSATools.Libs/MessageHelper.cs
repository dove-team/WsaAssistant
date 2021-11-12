using System;
using System.Runtime.InteropServices;
using System.Text;
using WSATools.Libs.Model;

namespace WSATools.Libs
{
    public sealed class MessageHelper
    {
        public const int WM_COPYDATA = 0x004A;
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static void SendMessage(string strMsg)
        {
            try
            {
                byte[] sarr = Encoding.UTF8.GetBytes(strMsg);
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = strMsg;
                cds.cbData = sarr.Length + 1;
                SendMessage(FindWindow(null, "MyDesktopToolHost"), WM_COPYDATA, 0, ref cds);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("SendMessage", ex);
            }
        }
    }
}