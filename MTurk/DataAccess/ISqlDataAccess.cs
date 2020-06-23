using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.SQLDataAccess
{
    public interface ISqlDataAccess
    {
        string ConnectionStringName { get; set; }

        List<T> LoadDataList<T, U>(string sql, U parameters);
        Task<U> LoadDataSingleAsync<T, U>(string sql, T parameters);
        U LoadDataSingle<T, U>(string sql, T parameters);
        Task SaveDataAsync<T>(string sql, T parameters);
        void SaveData<T>(string sql, T parameters);
        Task<U> SaveData<T, U>(string sql, T parameters);

    }
}