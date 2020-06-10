using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.SQLDataAccess
{
    public interface ISqlDataAccess
    {
        string ConnectionStringName { get; set; }

        List<T> LoadDataList<T, U>(string sql, U parameters);
        Task<U> LoadDataSingle<T, U>(string sql, T parameters);
        Task SaveData<T>(string sql, T parameters);
        Task<U> SaveData<T, U>(string sql, T parameters);

    }
}