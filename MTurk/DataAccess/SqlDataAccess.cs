using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.SQLDataAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;
        public string ConnectionStringName { get; set; } = "Default";
        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }
        public List<T> LoadDataList<T, U>(string sql, U parameters)
        {
            string connectionString = _config.GetConnectionString(ConnectionStringName);
            using IDbConnection connection = new SqlConnection(connectionString);
            var data = connection.Query<T>(sql, parameters);
            return data.ToList();

        }
        public async Task<U> LoadDataSingle<T, U>(string sql, T parameters)
        {
            string connectionString = _config.GetConnectionString(ConnectionStringName);
            using IDbConnection connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<U>(sql, parameters);
        }
        public async Task SaveData<T>(string sql, T parameters)
        {
            string connectionString = _config.GetConnectionString(ConnectionStringName);
            using IDbConnection connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(sql, parameters);
        }
        public async Task<U> SaveData<T, U>(string sql, T parameters)
        {
            string connectionString = _config.GetConnectionString(ConnectionStringName);
            using IDbConnection connection = new SqlConnection(connectionString);
            return await connection.QuerySingleAsync<U>(sql, parameters);
        }
    }
}
