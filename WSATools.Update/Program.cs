using Newtonsoft.Json;
using System.Reflection;

namespace WSATools.Update
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var stringContent = Client.GetContent("https://michael-eddy.github.io/config/wsa-tools.json");
                var model = JsonConvert.DeserializeObject<VersionInfo>(stringContent);
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version != null && model != null)
                {
                    if (version.Major < model.Main || version.Minor < model.Second || version.Build < model.Fix)
                    {
                        var path = Client.DownloadPath(model);
                        if (!string.IsNullOrEmpty(path))
                        {

                        }
                    }
                }
            }
            catch { }
        }
    }
}