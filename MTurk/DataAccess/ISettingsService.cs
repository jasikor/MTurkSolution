using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public interface ISettingsService
    {
        /// <summary>
        /// Gets value of a setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <returns>value or null if <paramref name="key"/> not found</returns>
        string GetSetting(string key);
        void SetSetting(string key, string value);
        /// <summary>
        /// Gets value of DateTime setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <returns>date or default(DateTime) if <paramref name="key"/> not found</returns>
        DateTime GetSettingDateTime(string key);
        void SetSetting(string key, DateTime value);
    }
}
