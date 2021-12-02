using System.Collections.Generic;

namespace WsaAssistant.Libs.Model
{
    public sealed class VersionInfo
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public List<VersionUri> Urls { get; set; }
        public string ChMessage { get; set; }
        public string EnMessage { get; set; }
    }
    public sealed class VersionUri
    {
        public string Ext { get; set; }
        public string Platform { get; set; }
        public string Url { get; set; }
    }
}