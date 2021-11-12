using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using WSATools.Libs;

namespace WSATools
{
    public sealed class LangManager
    {
        private static LangManager instance;
        public static LangManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new LangManager();
                return instance;
            }
        }
        private LangManager() { }
        private string Current = "Chinese";
        public ResourceDictionary Resource { get; private set; }
        public void Init()
        {
            if (!CultureInfo.CurrentCulture.Name.Contains("zh", StringComparison.CurrentCultureIgnoreCase))
                Switch("English");
            else
                Resource = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                x.Source.ToString().Contains(Current, StringComparison.CurrentCultureIgnoreCase));
        }
        public void Switch(string langName)
        {
            if (langName != Current)
            {
                Current = langName;
                try
                {
                    Uri uri = new Uri(string.Format("/WSATools.Libs;Component/Lang/{0}.xaml", Current), UriKind.Relative);
                    Resource = Application.LoadComponent(uri) as ResourceDictionary;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogError("Switch", ex);
                    throw;
                }
                if (Resource != null)
                {
                    var resourceDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                    x.Source.ToString().Contains("Chinese", StringComparison.CurrentCultureIgnoreCase) ||
                   x.Source.ToString().Contains("English", StringComparison.CurrentCultureIgnoreCase));
                    if (resourceDictionary != null)
                        Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(Resource);
                }
            }
        }
    }
}