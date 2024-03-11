using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Diagnostics;

namespace DBHelper.Tests
{
    //.NetCore单元测试项目无法读取app.config文件
    //1.Console控制台程序没有问题
    //2.单元测试不可以，读取到的值是NULL,两个解决方案：
    //(1) 在单元测试工程中将app.config文件改为：testhost.dll.config
    //(2) 修改单元测试工程文件，配置编译后事件，动态copy生成testhost.dll.config

    [TestClass()]
    public class DBHelperTests
    {

        [TestMethod()]
        public void ExecuteReaderTest1()
        {
            const string sql = "select * from RoomType";
            IDataReader reader = DBHelper.ExecuteReader(sql);
            while (reader.Read())
            {
                Debug.WriteLine(reader["TypeName"].ToString());
            }
            DBHelper.Close();  //记得关闭reader
        }
        [TestMethod()]
        public void ExecuteReaderTest2()
        {
            string sql = string.Format("select * from RoomType where TypeId={0}", 1);
            IDataReader reader = DBHelper.ExecuteReader(sql);
            while (reader.Read())
            {
                Debug.WriteLine(reader["TypeName"].ToString());
            }
            DBHelper.Close();
        }
        [TestMethod()]
        public void ExecuteReaderTest3()
        {
            string sql = "select * from RoomType where TypeName=@TypeName";

            //先创建参数，然后才能添加参数 

            DBHelper.CreateParameters(1);  //参数个数，1个
            DBHelper.AddParameters(0, "@TypeName", "情侣房");
            IDataReader reader = DBHelper.ExecuteReader(sql);
            while (reader.Read())
            {
                Debug.WriteLine(reader["TypeName"].ToString());
            }
            DBHelper.Close();
        }
    }
}