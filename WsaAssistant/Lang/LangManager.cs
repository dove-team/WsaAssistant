using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using WsaAssistant.Libs;
using WsaAssistant.Libs.Model;

namespace WsaAssistant
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
        public ResourceDictionary Resource { get; private set; }
        public LangType Current { get; private set; } = LangType.Chinese;
        public void Init()
        {
            var cultureName = CultureInfo.CurrentCulture.Name;
            if (!cultureName.Contains("zh", StringComparison.CurrentCultureIgnoreCase))
            {
                var langType = (cultureName.Contains("ru") || cultureName.Contains("be"))
                    ? LangType.Russian : LangType.English;
                Switch(langType);
            }
            else
                Resource = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                x.Source.ToString().Contains(Current.ToString(), StringComparison.CurrentCultureIgnoreCase));
        }
        public void Switch(LangType langType)
        {
            if (langType != Current)
            {
                Current = langType;
                try
                {
                    Uri uri = new Uri(string.Format("Lang/{0}.xaml", Current), UriKind.Relative);
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
                   x.Source.ToString().Contains("Russian", StringComparison.CurrentCultureIgnoreCase) ||
                   x.Source.ToString().Contains("English", StringComparison.CurrentCultureIgnoreCase));
                    if (resourceDictionary != null)
                        Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(Resource);
                }
            }
        }
    }
    static class Ext
    {
        public static string FindChar(this Window _, string key)
        {
            var obj = LangManager.Instance.Resource[key];
            return obj == null ? string.Empty : obj.ToString();
        }
    }
}