using System.Collections.Generic;

namespace WsaAssistant.Libs.Model
{
    public sealed class HttpHeader
    {
        public string Referer { get; set; }
        public List<string> Headers { get; set; }
    }
}