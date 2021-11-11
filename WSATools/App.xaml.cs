using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WSATools.Libs;
using WSATools.Libs.Lang;
using WSATools.ViewModels;

namespace WSATools
{
    public partial class App : Application
    {
        public App() : base()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("本程序需要管理员权限运行！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var applicationName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
            new Mutex(true, applicationName, out bool createNew);
            if (createNew)
            {
                base.OnStartup(e);
                LangManager.Instance.Init();
            }
            else
            {
                MessageBox.Show("程序已经启动了！");
                Current.Shutdown();
            }
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                LogManager.Instance.LogError("UnhandledException", ex);
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogManager.Instance.LogError("UnobservedTaskException", e.Exception);
        }
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogManager.Instance.LogError("DispatcherUnhandledException", e.Exception);
        }
    }
}