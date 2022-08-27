using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsShortcutFactory;

namespace WsaAssistant.Libs
{
    public class ShortcutHelper
    {
        //需要引入IWshRuntimeLibrary，搜索Windows Script Host Object Model 
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="directory">快捷方式所处的文件夹</param>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，
        /// 例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <remarks></remarks>
        public static void CreateShortcut(string directory, string shortcutName, string targetPath,
            string description = null, string iconLocation = null,string arguments=null)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            } 
            using var shortcut = new WindowsShortcut
            {
                Path = targetPath,
                Description = description,
                IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation,//设置图标路径
                WorkingDirectory = Path.GetDirectoryName(targetPath),
                Arguments = arguments
            }; 
            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            shortcut.Save(shortcutPath); 
        }

        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"</param>
        /// <remarks></remarks>
        public static void CreateShortcutOnDesktop(string shortcutName, string targetPath,
            string description = null, string iconLocation = null, string arguments = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//获取桌面文件夹路径
            CreateShortcut(desktop, shortcutName, targetPath, description, iconLocation, arguments);
        }

    }
}
