using System.Collections.Generic;

namespace WSATools.Update
{
    public sealed class VersionInfo
    {
        public int Main { get; set; }
        public int Second { get; set; }
        public int Fix { get; set; }
        public List<Uri> Urls { get; set; }
    }
    public sealed class Uri
    {
        public string Platform { get; set; }
        public string Url { get; set; }
    }
}