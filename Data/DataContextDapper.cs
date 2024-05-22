using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TODOAPP.Data
{
  public class DataContextDapper 
  {
    private readonly IConfiguration _config;
    private IDbConnection _db;
    public DataContextDapper(IConfiguration config)
    {
      _config = config;
      _db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
    async public Task<IEnumerable<T>> LoadData<T>(string sql,DynamicParameters? parameters)=> await _db.QueryAsync<T>(sql,(parameters));
    async public Task<T?> LoadSingleData<T>(string sql,DynamicParameters? parameters)=> await _db.QueryFirstOrDefaultAsync<T>(sql,parameters);
    async public Task<bool> ExecuteSql(string sql,DynamicParameters? parameters)=> await _db.ExecuteAsync(sql,parameters)>0;
  }
}