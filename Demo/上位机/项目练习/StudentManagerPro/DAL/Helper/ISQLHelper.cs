using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Helper
{
    public interface ISQLHelper
    {

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        bool ExecuteNonQuery(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        bool ExecuteNonQuery(string sql, Dictionary<string, object> pms);

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        bool ExecuteNonQuery(string sql);

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<T> ExecuteQuery<T>(string sql, params SqlParameter[] parameters) where T : new();

        /// <summary>
        ///  将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> pms) where T : new();

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> ExecuteQuery<T>(string sql) where T : new();

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T ExecuteQueryOne<T>(string sql, params SqlParameter[] parameters) where T : new();

        /// <summary>
        ///  将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        T ExecuteQueryOne<T>(string sql, Dictionary<string, object> pms) where T : new();

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        T ExecuteQueryOne<T>(string sql) where T : new();

        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        DataTable ExecuteQueryDataTable(string sql, SqlParameter[] pms = null);
        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        DataTable ExecuteQueryDataTable(string sql, Dictionary<string, object> pms);

        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable ExecuteQueryDataTable(string sql);

        /// <summary>
        /// 启用事务执行多条SQL语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool ExcuteQueryTrans(Dictionary<string, SqlParameter[]> parameters);
    }
}
