# SQL Server下7种“数据分页”方案

数据分页往往有三种常用方案。

第一种，把数据库中存放的相关数据，全部读入PHP/Java/C#代码/内存，再由代码对其进行分页操作（速度慢，简易性高）。

第二种，直接在数据库中对相关数据进行分页操作，再把分页后的数据输出给代码程序（速度中，简易性中）。

第三种，先把数据库中的相关数据全部读入“缓存”或第三方工具，再由代码程序对“缓存”或第三方工具中的数据进行读取+分页操作（速度快，简易性差）。

本文下面重点阐述上述【第二种】方案在SQL Server上的使用（其它种类数据库由于Sql语句略有差异，所以需要调整，但方案也类似）

### 1、ROW_NUMBER() OVER()方式（SQL2012以下推荐使用)

示例：

```SQL
SELECT * FROM
    (SELECT ROW_NUMBER() OVER(ORDER BY menuId) AS RowId,* FROM sys_menu ) AS r 
WHERE  RowId BETWEEN 1 AND 10
```

用子查询新增一列行号（ROW_NUMBER）RowId查询，比较高效的查询方式，只有在SQL Server2005或更高版本才支持。

BETWEEN 1 AND 10 是指查询第1到第10条数据（闭区间），在这里面需要注意的是OVER的括号里面可以写多个排序字段。

`通用用法`

```sql
--pageIndex 表示指定页
--pageSize  表示每页显示的条数
SELECT * FROM
    (SELECT ROW_NUMBER() OVER(ORDER BY 排序字段) AS RowId,* FROM 表名 ) AS r 
WHERE  RowId  BETWEEN ((pageIndex-1)*pageSize + 1) AND (pageIndex * PageSize)
```

### 2、offset fetch next方式（SQL2012及以上的版本才支持：推荐使用 ）

示例：

```sql
SELECT * FROM sys_menu 
ORDER BY menuId offset 0 ROWS FETCH NEXT 10 ROWS ONLY
```

offset 是跳过多少行，

next是取接下来的多少行， 

句式 offset...rows fetch nect ..rows only ，注意rows和末尾的only 不要写漏掉了，并且这种方式必须要接着Order by XX 使用，不然会报错。

`通用用法`

```sql
--pageIndex 表示指定页
--pageSize  表示每页显示的条数
SELECT * FROM 表名 
ORDER BY 排序字段 offset ((pageIndex - 1) * pageSize) ROWS FETCH NEXT pageSize ROWS ONLY
```

### top not in方式 （不推荐）

示例：

```sql
--查询第11-20条记录
SELECT TOP 10 menuId, *
FROM sys_menu 
WHERE menuId NOT IN (SELECT TOP 10 menuId FROM sys_menu)
```

这条语句的原理是先查询1-10条记录的ID，然后再查询ID不属于这1-10条记录的ID，并且只需要10条记录，因为每页大小就是10，

这就是获取到的第11-20条记录，这是非常简单的一种写法。 

另外IN语句与NOT IN语句类似，这是NOT IN的写法，但是这种写法数据量大的话效率太低。

`通用用法`

```sql
--pageIndex 表示指定页
--pageSize  表示每页显示的条数
SELECT TOP pageSize menuId, *
FROM sys_menu 
WHERE menuId NOT IN (SELECT TOP ((pageSize-1)*pageIndex) menuId FROM sys_menu)
```

通过升序与降序方式进行查询分页（不推荐）

示例：

```sql
--查询第11-20条记录
SELECT * FROM(
    SELECT TOP 10 * FROM(
        SELECT TOP 20 * FROM sys_menu ORDER BY menuId ASC) 
            AS TEMP1 ORDER BY menuId DESC)
        AS TEMP2 ORDER BY menuId ASC
```

这条语句首先查询前20条记录，然后在倒序查询前10条记录（即倒数10条记录）， 

这个时候就已经获取到了11-20条记录，但是他们的顺序是倒序，所以最后又进行升序排序。

`通用用法`

```sql
--pageIndex 表示指定页
--pageSize  表示每页显示的条数
SELECT * FROM(
    SELECT TOP pageSize * FROM(
        SELECT TOP ((pageIndex - 1) * pageSize +(pageSize*2)) * FROM sys_menu ORDER BY menuId ASC) 
            AS TEMP1 ORDER BY menuId DESC)
        AS TEMP2 ORDER BY menuId ASC
```

### 采用MAX(ID)或者MIN(ID)函数（不推荐）

示例：

```sql
--查询第11-20条记录
SELECT TOP 10 * FROM sys_menu WHERE menuId>
    (SELECT MAX(menuId) FROM(SELECT TOP 10 menuId FROM sys_menu ORDER BY menuId) AS TEMP1) --（第10条的id）
```

这个理解起来也简单，先把第10条记录的id找出来（当然这里面是直接使用MAX()进行查找，MIN()函数的用法也是类似的），

然后再对比取比第10条记录的id大的前10条记录即为我们需要的结果。

这里要注意开始时的边界值调整。

`通用用法`

```sql
--pageIndex 表示指定页
--pageSize  表示每页显示的条数
SELECT TOP pageSize * FROM sys_menu WHERE menuId>
    (SELECT MAX(menuId) FROM(SELECT TOP ((PageIndex-1)*PageSize) menuId FROM sys_menu ORDER BY menuId) AS TEMP1) --（第10条的id）
```

上述1~5方案，再配合**存储过程**，你就能制造出适合你自己的“分页”轮子，日后反复使用。

但它们有一定自身局限性：方案1、2、5都需要依赖一个排序Id（这个Id要么是个排序列，要么是个主键）。方案3、4则效率太低，完全不推荐。

### 7、不依赖排序/排序Id的终极方案

此方案在DeveloperSharp框架中有提供（基于.Net/.Net Core/.Net Framework），方案被广东省的多个公司/项目采用，得到了实战验证+稳定性。

【第一步】：从NuGet引用DeveloperSharp包。

【第二步】：创建一个用来与数据库进行通信的“数据源类”（文本示例为：TestData.cs），内容如下：

```C#

using DeveloperSharp.Structure.Model;
using DeveloperSharp.Framework.QueryEngine;

namespace YZZ
{
    [DataSource(DatabaseType.SQLServer, "Server=localhost;Database=Test;Uid=sa;Pwd=123")]
    public class TestData : DeveloperSharp.Structure.Model.DataLayer
    {
        //类中没有任何代码
    }
}
```

说 明 ：“数据源类”（文本示例为：TestData.cs）必 须 继 承 自 DeveloperSharp.Structure.Model.DataLayer 类 ， 并 且 在 其 上 设 置DataSource属 性 的 初 始 化 值 为“数据库类型”及其“链接字符串”。

【第三步】：添加通过“数据源类”（TestData）调用其PagePartition方法进行数据分页的代码。注 意：核心代码就一行而已！！

代码如下：

```C#

using DeveloperSharp.Extension;//Table扩展所在的命名空间
-----------------------------
    class Program
    {
        static void Main(string[] args)
        {
            TestData td = new TestData();

            //分页
            var pp = td.PagePartition("select top 5000 * from t_Order where Id>10 order by Id desc", 20, 162);

            List<Product> Products = pp.Table.ToList<Product>();
            foreach (var P in Products)
            {
                Console.WriteLine(P.Name);
            }

            Console.ReadLine();
        }
    }
```

Product类代码如下：

```C#
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
```

此处的PagePartition方法有两个重载方法，其详细功能说明如下：

```C#

PagePartition
声明：public PagePiece PagePartition(string RecordSet, string Id, int PageSize, int PageIndex)
用途：分页功能(有主键)
参数：（1）string RecordSet     --需要分页的记录集，可以是表、视图、或者SQL语句
（2）string Id     --主键
（3）int PageSize     --页面大小
（4）int PageIndex     --当前页码
返回：PagePiece  --页片实体

PagePartition
声明：public PagePiece PagePartition(string RecordSet, int PageSize, int PageIndex)
用途：分页功能(无主键)
参数：（1）string RecordSet     -- 需要分页的记录集，可以是表、视图、或者SQL语句
     （2）int PageSize    --页面大小
（3）int PageIndex    --当前页码
返回：PagePiece  --页片实体
```

`注意：`

- `当你需要分页的数据表有“主键”字段时，使用“分页功能(有主键)”。反之，使用“分页功能(无主键)”。`
-  `RecordSet是你需要分页的“数据总集”的SQL语句。该SQL语句的形式丰富多样，可以带条件、排序、甚至还能是多表的联合查询、等。`
-  `此方法符合最开始的【第二种】方案，是在SQL Server内部进行的分页操作。而且可以不依赖于排序/排序Id。`