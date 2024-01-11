using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using ServiceStack.Redis;
using TextFile;

namespace OfficeReportInterface
{
    /// <summary>
    /// Redis工具类
    /// </summary>
    public class RedisWrap
    {
        private static  PooledRedisClientManager pool = null;
        private static  string[] redisHosts = null;
        private static int redisMaxReadPool = 3; 
        private static int redisMaxWritePool = 1;

        #region redis中存在的key。这些key在初始化后会增加后缀，后缀是从OfficeReportTemplate.ini中读取的pipename
        /// <summary>
        /// 数据库连接信息
        /// </summary>
        public static string Key_DatabaseInfo = "DatabaseInfo";

        public static string Key_deviceDatalogPrivateValueMap = "deviceDatalogPrivateValueMap";
        public static string Key_devDataToAllDataMap = "devDataToAllDataMap";
        public static string Key_deviceDataIDToMeasIDMap = "deviceDataIDToMeasIDMap";
        /// <summary>
        /// Redis保存设备更新时间的key
        /// </summary>
        public static string key_DeviceUpdateTime = "DeviceUpdateTime";
        #endregion

        private static DatabaseInfo dbInfo = null;
        private static int DbIndex = 0; //保存当前数据库的索引，默认是第1个
        /// <summary>
        /// 检测redis是否安装的标识，根据这个标识判断是否要从redis缓存读取数据
        /// </summary>
        public static bool IsRedisInstalled = true;
        static  RedisWrap()
        {
        }

        /// <summary>
        /// 用于标识是哪个程序。winform程序还是web程序调用的这个函数。winform程序OfficeReport传入"OfficeReport",webservice程序OfficeReportWebService传入"OfficeReportWebService"，windowsService程序OfficeReportGenerateService传入"OfficeReportGenerateService"
        /// </summary>
        /// <param name="type"></param>
        public static void Initialize(string type)
        {
            try
            {
                //从OfficeReportTemplate.ini文件读取配置信息
                string filePath = Path.Combine(DbgTrace.GetAssemblyPath(), "OfficeReportTemplates.ini");
                DbgTrace.dout("从OfficeReportTemplate.ini文件读取Redis配置信息。filePath=" + filePath);
                INIFile iniFile = new INIFile(filePath);
                int tempMaxReadPool;
                if (int.TryParse(iniFile.ReadString("Redis", "redis_max_read_pool"), out tempMaxReadPool) && tempMaxReadPool > 0)
                {
                    redisMaxReadPool = tempMaxReadPool;
                }
                int tempMaxWritePool;
                if (int.TryParse(iniFile.ReadString("Redis", "redis_max_write_pool"), out tempMaxWritePool) && tempMaxWritePool > 0)
                {
                    redisMaxWritePool = tempMaxWritePool;
                }
                //string tailName = iniFile.ReadString("config", "PipeName");  暂时先注释掉这些代码。考虑到多个程序缓存同样的数据可能存在内存不够用的问题
                //tailName = AddTailToString(tailName , type);//将程序名称也加上
                //if (!string.IsNullOrEmpty(tailName)) //在各个redis的key中增加后缀，用以区分iEMSWeb，iEEMWeb等不同的产品，目的是让redis存储的数据互不干扰
                //{
                //    Key_DatabaseInfo = AddTailToString(Key_DatabaseInfo, tailName);
                //    Key_deviceDatalogPrivateValueMap = AddTailToString(Key_deviceDatalogPrivateValueMap, tailName);
                //    Key_devDataToAllDataMap = AddTailToString(Key_devDataToAllDataMap, tailName);
                //    Key_deviceDataIDToMeasIDMap = AddTailToString(Key_deviceDataIDToMeasIDMap, tailName);
                //    key_DeviceUpdateTime = AddTailToString(key_DeviceUpdateTime, tailName);
                //}

                var redisHostStr = iniFile.ReadString("Redis", "redis_server_session");
                DbgTrace.dout("从OfficeReportTemplate.ini文件读取Redis配置信息。redis_max_read_pool=" + redisMaxReadPool + "redis_max_write_pool =" + redisMaxWritePool + "redis_server_session=" + redisHostStr);

                //var redisHostStr = "127.0.0.1:6379";//ConfigurationManager.AppSettings["redis_server_session"];

                if (!string.IsNullOrEmpty(redisHostStr))
                {
                    redisHosts = redisHostStr.Split(',');

                    if (redisHosts.Length > 0)
                    {
                        pool = new PooledRedisClientManager(redisHosts, redisHosts,
                            new RedisClientManagerConfig()
                            {
                                MaxWritePoolSize = redisMaxWritePool,
                                MaxReadPoolSize = redisMaxReadPool,
                                AutoStart = true
                            });
                    }
                }
                if (dbInfo == null)
                {
                    DatabaseStatus.DataManager.GetDataSource();
                    dbInfo = new DatabaseInfo(DatabaseStatus.DataManager._projectName, DatabaseStatus.DataManager._primaryDataSource);
                    DbIndex = GetDbIndex(); //初始化DBIndex
                    using (var redisClient = pool.GetClient())
                    {
                        redisClient.Db = DbIndex;
                        if (redisClient != null)
                        {
                            redisClient.SendTimeout = 1000;
                            if (dbInfo != null && !redisClient.ContainsKey(Key_DatabaseInfo))
                            {
                                //用JsonConvert或JavaScriptSerializer序列化后，反序列化的时候会报异常，因为可能出现转义字符，比如“172.16.6.119\Demo”，导致反序列化异常，所以在序列化后将转义字符换成|，再存储到redis里面；从redis里面读取后将|换成\，再进行反序列化就没有问题了
                                string jsonStr = JsonConvert.SerializeObject(dbInfo);
                                jsonStr = jsonStr.Replace(@"\", @"|");
                                redisClient.Set(Key_DatabaseInfo, jsonStr);

                                //测试用的代码
                                //string dbInfoStr = redisClient.Get<string>(Key_DatabaseInfo);
                                //dbInfoStr = dbInfoStr.Replace(@"\", @"").Replace(@"|", @"\");
                                //DatabaseInfo dbInfoTmp = JsonConvert.DeserializeObject<DatabaseInfo>(dbInfoStr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsRedisInstalled = false; //表明当前环境没有安装redis，不从redis缓存读取数据
                DbgTrace.dout(ex.Message+ex.StackTrace);
            }
        }
        /// <summary>
        /// 连接两个字符串，中间加个下划线
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        private static string AddTailToString(string name, string tail)
        {
            return string.Format("{0}_{1}", name, tail);
        }
        public static int GetDbIndex()
        {
            int DbIndex = -1; //存储数据库的索引
            try
            {
                if (pool != null)
                {
                    using (var redisClient = pool.GetClient())
                    {
                        if (redisClient != null)
                        {
                            redisClient.SendTimeout = 1000;
                            for (int i = 0; i < 16; i++) //默认只有16个数据库
                            {
                                redisClient.Db = i; //切换到当前数据库，判断是否匹配
                                if (redisClient.ContainsKey(Key_DatabaseInfo))
                                {
                                    string dbInfoStr = redisClient.Get<string>(Key_DatabaseInfo);
                                    dbInfoStr = dbInfoStr.Replace(@"\", @"").Replace(@"|", @"\");
                                    DatabaseInfo dbInfoTmp = JsonConvert.DeserializeObject<DatabaseInfo>(dbInfoStr);
                                    if (dbInfo.ProjectName == dbInfoTmp.ProjectName && dbInfo.DataSource == dbInfoTmp.DataSource) //如果匹配就用这个数据库
                                        return i;
                                }
                                else //说明是空数据库
                                {
                                    if (DbIndex == -1) //将第1个空数据库保存下来，如果找到最后没有找到匹配的，则返回第1个遇到的空数据库
                                    {
                                        DbIndex = i;
                                    }
                                }
                            }
                            //遍历一遍没有找到匹配的数据库，则判断是否有空数据库
                            if (DbIndex != -1) //如果有空数据库，则直接返回第1个空数据库
                            {
                                return DbIndex;
                            }
                            else //如果没有空数据库，则说明都已被占用，此时直接清空第1个默认的数据库，并返回该数据库的索引
                            {
                                redisClient.Db = 0;
                                redisClient.FlushDb();
                                return 0; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "GetClientByDBInfo", "获取对应的缓存数据库", ex.Message);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
            return DbIndex;
        }

        public static void Add<T>(string key, T value, DateTime expiry)
        {
            if (value == null)
            {
                return;
            }

            if (expiry <= DateTime.Now)
            {
                Remove(key);
                return;
            }

            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        redisClient.Set(key, value, expiry - DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }

        }

        public static void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (value == null)
            {
                return;
            }

            if (slidingExpiration.TotalSeconds <= 0)
            {
                Remove(key);
                return;
            }

            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        if (slidingExpiration == TimeSpan.MaxValue)
                            redisClient.Set(key, value);
                        else
                            redisClient.Set(key, value, slidingExpiration);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }

        }

        public static void AddRangeToList(string key, List<string> list, TimeSpan slidingExpiration)
        {
            if (list == null) //list为空的时候也要写入，否则list里面的数据不会被清空
            {
                return;
            }

            if (slidingExpiration.TotalSeconds <= 0)
            {
                Remove(key);
                return;
            }
            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        //r.RemoveAllFromList(key); //先删除原来的数据
                        redisClient.Remove(key); //用上面的方法不会清除list里面的数据
                        redisClient.AddRangeToList(key, list);
                        if (slidingExpiration != TimeSpan.MaxValue) //设置过期时间
                            redisClient.ExpireEntryIn(key, slidingExpiration);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "AddRangeToList", key);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
        }

        public static List<string> GetAllItemsFromList(string listId)
        {
            List<string> result = new List<string>();
            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        return redisClient.GetAllItemsFromList(listId);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "添加到List", listId);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
            return result;
        }

        public static List<string> GetAllKeys()
        {
            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        return redisClient.GetAllKeys();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "获取所有key", string.Empty);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
            return new List<string>();
        }

        public static IDictionary<string, T> GetAll<T>(List<string> keys)
        {
            IDictionary<string, T> obj = default(IDictionary<string, T>);

            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        obj = redisClient.GetAll<T>(keys);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "获取", string.Join(",", keys));
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
            return obj;
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            T obj = default(T);

            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        obj = redisClient.Get<T>(key);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "获取", key);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
            return obj;
        }

        public static void Remove(string key)
        {
            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        redisClient.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "删除", key);
                DbgTrace.dout(ex.Message + ex.StackTrace + msg);
            }
        }

        public static bool Exists(string key)
        {
            try
            {
                using (var redisClient = pool.GetClient())
                {
                    redisClient.Db = DbIndex;
                    if (redisClient != null)
                    {
                        redisClient.SendTimeout = 1000;
                        return redisClient.ContainsKey(key);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "是否存在", key);
                DbgTrace.dout(ex.Message+ex.StackTrace+msg);
            }
            return false;
        }

    }

    public class DatabaseInfo
    {
        public string ProjectName;
        public string DataSource;

        public DatabaseInfo() //必须添加无参数的构造函数，否则会在直接打开EMSWebService.asmx网页的时候报错
        {
            
        }

        public DatabaseInfo(string pjName, string dataSource)
        {
            this.ProjectName = pjName;
            this.DataSource = dataSource;
        }
    }
}
