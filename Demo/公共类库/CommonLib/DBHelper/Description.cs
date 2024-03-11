namespace DBHelper
{
    public class Description
    {
        //1. DataProvider.cs     # 创建一个枚举类型  
        //2. DBManagerFactory.cs   # 建一个工厂类，用来产生以上不同数据库的实例  
        //3. IDBManager.cs  # 创建一个接口  
        //4. DBManager.cs   # 创建一个类来实现IDBManager接口  
        //5. DBHelper.cs  #  来调用DBManager类，外部来直接调用DBHelper类即可  
    }
}
