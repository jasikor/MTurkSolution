using MTurk.Models;
using MTurk.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public class SettingsService : ISettingsService
    {
        private readonly ISqlDataAccess _db;

        public SettingsService(ISqlDataAccess db)
        {
            _db = db;
        }

        public List<SettingModel> GetAllSettings()
        {
            string sql = @"select * from dbo.Settings order by [Key]";
            return _db.LoadDataList<SettingModel, dynamic>(sql, new { });
        }

        /// <inheritdoc/>
        public string GetSetting(string key)
        {
            string sql = @"select * from Settings where [Key] = @Key";
            var res = _db.LoadDataSingle<dynamic, SettingModel>(sql, new { Key = key });
            if (res is null)
                return null;
            else
                return res.Value;

        }
        /// <inheritdoc/>
        public DateTime GetSettingDateTime(string key)
        {
            var val = GetSetting(key);
            if (val is null)
                return default;
            DateTime res;
            if (DateTime.TryParse(val, null, System.Globalization.DateTimeStyles.RoundtripKind, out res))
                return res;
            else
                return default;
        }

        public void SetSetting(string key, string value)
        {
            string sql = @"update [dbo].[settings]
                           set [Value] = @Value
                           where [Key] = @Key";

            _db.SaveData<dynamic>(sql, new { Key = key, Value = value });
        }

        public void SetSetting(string key, DateTime value)
        {
            SetSetting(key, value.ToString("s", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
