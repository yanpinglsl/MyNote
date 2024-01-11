using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
namespace DAL.Helper
{
    public class MSSQLHelper:ISQLHelper
    {
        //public static ISQLHelper Instance;

        //// 定义一个标识确保线程同步
        //private static readonly object locker = new object();

        //// 定义私有构造函数，使外界不能创建该类实例
        //private MSSQLHelper()
        //{
        //}

        ///// <summary>
        ///// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        ///// </summary>
        ///// <returns></returns>
        //public static ISQLHelper GetInstance()
        //{
        //    // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
        //    // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
        //    // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
        //    lock (locker)
        //    {
        //        // 如果类的实例不存在则创建，否则直接返回
        //        if (Instance == null)
        //        {
        //            Instance = new MSSQLHelper();
        //        }
        //    }

        //    return Instance;
        //}

        private string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();
        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  bool ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
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
        public  bool ExecuteNonQuery(string sql, Dictionary<string, object> pms)
        {
            SqlParameter[] parameters = null;
            if (parameters != null & parameters.Length > 0)
            {
                parameters = DictionaryToMySqlParameters(pms).ToArray();
            }
            return ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 执行增、删、改的方法：ExecuteNonQuery,返回true,false
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  bool ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, new SqlParameter[] { });
        }

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public  List<T> ExecuteQuery<T>(string sql, params SqlParameter[] parameters) where T : new()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader())
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
        public  List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> pms) where T : new()
        {
            SqlParameter[] parameters = null;
            if (parameters != null & parameters.Length > 0)
            {
                parameters = DictionaryToMySqlParameters(pms).ToArray();
            }
            return ExecuteQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个List
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  List<T> ExecuteQuery<T>(string sql) where T : new()
        {
            return ExecuteQuery<T>(sql, new SqlParameter[] { });
        }



        /// <summary>
        /// 将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteQueryOne<T>(string sql, params SqlParameter[] parameters) where T : new()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
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
        public  T ExecuteQueryOne<T>(string sql, Dictionary<string, object> pms) where T : new()
        {
            SqlParameter[] parameters = null;
            if (parameters != null & parameters.Length > 0)
            {
                parameters = DictionaryToMySqlParameters(pms).ToArray();
            }
            return ExecuteQueryOne<T>(sql, parameters);
        }

        /// <summary>
        /// 将查出的数据装到实体里面，返回一个实体
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  T ExecuteQueryOne<T>(string sql) where T : new()
        {
            return ExecuteQueryOne<T>(sql, new SqlParameter[] { });
        }



        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public  DataTable ExecuteQueryDataTable(string sql, SqlParameter[] pms = null)
        {
            DataTable dt = new DataTable();

            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connString))
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
        public  DataTable ExecuteQueryDataTable(string sql, Dictionary<string, object> pms)
        {
            SqlParameter[] parameters = null;

            if (pms != null)
            {
                parameters = DictionaryToMySqlParameters(pms).ToArray();
            }

            return ExecuteQueryDataTable(sql, parameters);
        }

        /// <summary>
        /// 将查出的数据装到table里，返回一个DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  DataTable ExecuteQueryDataTable(string sql)
        {
            SqlParameter[] parameters = null;

            return ExecuteQueryDataTable(sql, parameters);
        }

        /// <summary>
        /// 启用事务执行多条SQL语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public  bool ExcuteQueryTrans(Dictionary<string, SqlParameter[]> parameters)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
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
        /// 按列名转换（单条数据使用比较方便）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private  T ConvertToModel<T>(SqlDataReader reader)
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
        /// 字典转SqlParameter
        /// </summary>
        /// <param name="pms"></param>
        /// <returns></returns>
        private  SqlParameter[] DictionaryToMySqlParameters(Dictionary<string, object> pms)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            foreach (var item in pms)
            {
                string parameterName = item.Key;
                object paramterValue = item.Value;
                parameters.Add(new SqlParameter(parameterName, paramterValue));
            }
            return parameters.ToArray();
        }

    }
}
