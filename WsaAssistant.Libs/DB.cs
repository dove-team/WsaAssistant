using Newtonsoft.Json;
using SQLite;
using System;
using System.IO;
using WsaAssistant.Libs.Model;

namespace WsaAssistant.Libs
{
    public sealed class DB : IDisposable
    {
        private SQLiteConnection Connection { get; }
        public DB()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "data.db");
            Connection = new SQLiteConnection(path);
            Init();
        }
        private void Init()
        {
            try
            {
                Connection.Table<Setting>().FirstOrDefault();
            }
            catch
            {
                Connection.CreateTable<Setting>(CreateFlags.AutoIncPK);
            }
        }
        public bool GetData<T>(string name, out T value)
        {
            try
            {
                var entity = Connection.Table<Setting>().FirstOrDefault(x => x.Name == name);
                if (entity != null)
                {
                    value = JsonConvert.DeserializeObject<T>(entity.Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetData", ex);
            }
            value = default;
            return false;
        }
        public void SetData<T>(string name, T value)
        {
            try
            {
                var entity = Connection.Table<Setting>().FirstOrDefault(x => x.Name == name);
                if (entity != null)
                {
                    entity.Value = JsonConvert.SerializeObject(value);
                    Connection.Update(entity);
                }
                else
                {
                    entity = new Setting
                    {
                        Value = JsonConvert.SerializeObject(value),
                        Name = name
                    };
                    Connection.Insert(entity);
                }
            }
            catch { }
        }
        public void Dispose()
        {
            try
            {
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }
    }
}