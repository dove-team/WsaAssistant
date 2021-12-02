using SQLite;

namespace WsaAssistant.Libs.Model
{
    [Table("Setting")]
    public sealed class Setting
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}