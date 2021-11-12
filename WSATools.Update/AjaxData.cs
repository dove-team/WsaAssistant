using System.Text;

namespace WSATools.Update
{
    sealed class AjaxData
    {
        public string Action { get; set; }
        public string Signs { get; set; }
        public string Sign { get; set; }
        public int Ves { get; set; } = 1;
        public string WebSign { get; set; }
        public string WebSignKey { get; set; }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{nameof(Action)}={Action}");
            builder.Append($"&{nameof(Signs)}={Signs}");
            builder.Append($"&{nameof(Sign)}={Sign}");
            builder.Append($"&{nameof(Ves)}={Ves}");

            return builder.ToString().ToLower();
        }
        public void Set(string sign, string signs)
        {
            Sign = sign;
            Signs = signs;
        }
        public void SetValue(string name, dynamic value)
        {
            switch (name.ToLower())
            {
                case "ves":
                    Ves = value;
                    break;
                case "action":
                    Action = value;
                    break;
                case "websign":
                    WebSign = value;
                    break;
                case "websignkey":
                    WebSignKey = value;
                    break;
            }
        }
    }
}