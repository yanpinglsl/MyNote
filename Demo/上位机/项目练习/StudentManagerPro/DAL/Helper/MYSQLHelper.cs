using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
namespace DAL.Helper
{
    public static class MYSQLHelper
    {
        private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(string sql, Dictionary<string, object> pms)
        {
            MySqlParameter[] parameters = null;
            if (parameters != null & parameters.Length > 0)
            {
                parameters = DictionaryToMyMySqlParameters(pms).ToArray();
            }
            return ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, new MySqlParameter[] { });
        }

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> ExecuteQuery<T>(string sql, params MySqlParameter[] parameters) where T : new()
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<T> tList = new List<T>();
                        while (reader.Read()) //遍历结果返回每一行数据
                        {
                            tList.Add(ConvertToModel<T>(reader));
                        }
                        return tList;
                    }
                }
            }
        }

        /// <summary>
        ///  将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> pms) where T : new()
        {
            MySqlParameter[] parameters = null;
            if (parameters != null & parameters.Length > 0)
            {
                parameters = DictionaryToMyMySqlParameters(pms).ToArray();
            }
            return ExecuteQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> ExecuteQuery<T>(string sql) where T : new()
        {
            return ExecuteQuery<T>(sql, new MySqlParameter[] { });
        }



        /// <summary>
        /// 将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T ExecuteQueryOne<T>(string sql, params MySqlParameter[] parameters) where T : new()
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    object result = cmd.ExecuteScalar();
                    if (result is T)
                    {
                        return (T)result;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

        /// <summary>
        ///  将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T ExecuteQueryOne<T>(string sql, Dictionary<string, object> pms) where T : new()
        {
            MySqlParameter[] parameters = null;
            if (parameters != null & parameters.Length > 0)
            {
                parameters = DictionaryToMyMySqlParameters(pms).ToArray();
            }
            return ExecuteQueryOne<T>(sql, parameters);
        }

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T ExecuteQueryOne<T>(string sql) where T : new()
        {
            return ExecuteQueryOne<T>(sql, new MySqlParameter[] { });
        }



        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public static DataTable ExecuteQueryDataTable(string sql, MySqlParameter[] pms = null)
        {
            DataTable dt = new DataTable();

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, connString))
            {
                if (pms != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public static DataTable ExecuteQueryDataTable(string sql, Dictionary<string, object> pms)
        {
            MySqlParameter[] parameters = null;

            if (pms != null)
            {
                parameters = DictionaryToMyMySqlParameters(pms).ToArray();
            }

            return ExecuteQueryDataTable(sql, parameters);
        }

        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable ExecuteQueryDataTable(string sql)
        {
            MySqlParameter[] parameters = null;

            return ExecuteQueryDataTable(sql, parameters);
        }

        /// <summary>
        /// 启用事务执行多条SQL语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool ExcuteQueryTrans(Dictionary<string, MySqlParameter[]> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = conn.BeginTransaction();
                        foreach (var parameter in parameters)
                        {
                            cmd.CommandText = parameter.Key;
                            cmd.Parameters.Clear();
                            if (parameter.Value != null && parameter.Value.Length > 0)
                            {
                                cmd.Parameters.AddRange(parameter.Value);
                            }
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        if (cmd.Transaction != null)
                            cmd.Transaction.Rollback();//回滚事务(同时自动清除事务)
                        throw new Exception("调用事务方法UpdateByTran(List<string> sqlList)时出现错误：" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime()
        {
            return ExecuteQueryOne<DateTime>("SELECT GETDATE()");
        }

        /// <summary>
        /// 按列名转换（单条数据使用比较方便）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static T ConvertToModel<T>(MySqlDataReader reader)
        {
            T result = default;
            PropertyInfo[] propertys = result.GetType().GetProperties();
            List<string> columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }
            foreach (PropertyInfo item in propertys)
            {
                if (columnNames.Contains(item.Name))
                {
                    if (!item.CanWrite)
                    {
                        continue;
                    }
                    var value = reader[item.Name];
                    if (value != DBNull.Value)
                    {
                        item.SetValue(result, value, null);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 字典转MySqlParameter
        /// </summary>
        /// <param name="pms"></param>
        /// <returns></returns>
        private static MySqlParameter[] DictionaryToMyMySqlParameters(Dictionary<string, object> pms)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            foreach (var item in pms)
            {
                string parameterName = item.Key;
                object paramterValue = item.Value;
                parameters.Add(new MySqlParameter(parameterName, paramterValue));
            }
            return parameters.ToArray();
        }

    }
}
