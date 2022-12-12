# 一、MongoDB入门

OLTP/OLAP

## 1.1 认识MongoDB及其特点

### 1.1.1 认识MongoDB

|               |                                                              |
| ------------- | ------------------------------------------------------------ |
| 什么是MongoDB | 一个以Json为数据模型的文档数据库                             |
|               | 文档来自于“Json Document”，并非我们一般理解的PDF，WORD文档   |
| 主要用途      | 应用数据库，类似于Oracle，MySQL。<br/>海量数据处理，数据平台 |
| 主要特点      | 建模为可选<br>JSON数据模型比较适合开发者<br/>横向扩展可以支撑很大数据量和并发 |
| 是否免费      | MongoDB有两个发布版本：社区版和企业版<br/>企业版是基于商业协议，需要付费 |
| 是否支持事务  | MongoDB4.X之后可以支持事务                                   |



### 1.1.2 MongoDB VS. 关系型数据库

|              | MongoDB                                         | RDBMS                            |
| ------------ | ----------------------------------------------- | -------------------------------- |
| 数据模型     | 文档模型                                        | 关系模型                         |
| 数据库类型   | OLTP                                            | OLTP                             |
| CRUD操作     | MQL/SQL                                         | SQL                              |
| 高可用       | 复制集                                          | 集群模式（要通过第三方软件搭建） |
| 横向扩展能力 | 通过原生分配完善支持                            | 数据分区或者应用侵入式           |
| 索引支持     | B-树、全文索引、地理位置索引、多键索引、TTL索引 | B树                              |
| 开发难度     | 容易                                            | 困难                             |
| 数据容量     | 没有理论上限                                    | 千万、亿                         |
| 扩展方式     | 垂直扩展+水平扩展                               | 垂直扩展                         |

### 1.1.3 特色及优势

- 面向开发者的易用性（简单直观、结构灵活、快速开发）+高效数据库
- 原生的高可用
  - 复制集（2-50个成员）
  - 可自恢复（主节点故障，可将从节点升级为主节点）
  - 多中心容灾能力
  - 滚动服务-最小化服务端（通过只升级一个节点的方式来升级数据库，可避免服务不可用）
- 横向扩展能力
  - 需要的时候无缝扩展
  - 应用全透明
  - 多种数据分布策略
  - 轻松支持TB-PB数量级

## 1.2 下载安装MongDB

### 1.2.1 本机安装

### 1.2.2 Docker安装

```shell
#启动容器
docker run --name mongodb -d -p 27017:27017 mongo
#进入容器
docker exec -it mongodb /bin/bash
#mongo5.0以上的版本使用mongo来执行mongodb命令已经不支持了，你需要改用mongosh来替代mongo！
mongosh
# 查询数据库
show db
#切换数据库
use test
#查询当前数据库下面的集合
show  collections
#创建集合
db.createCollection("集合名称")
#删除集合
db.集合名称.drop()
#删除数据库
db.dropDatabase() //首先要通过use切换到当前的数据库
```



## 1.3 MongDB基本操作

### 1.3.1 Insert

操作格式：

- db.<集合>.insertOne(<JSON对象>)
- db.<集合>.insertMany([<JSON 1>,<JSON 2>,....<JSON N>])

示例

```shell
 #切换数据库
 test> use mytest
 switched to db mytest
 
 #显示所有集合
 mytest> show  collections 
 
 #创建集合
 mytest> db.createCollection("fruit")
 { ok: 1 }
 
 mytest> show  collections
 fruit
 
 #向集合插入对象
 mytest> db.fruit.insertOne({name:"apple"})
 {
   acknowledged: true,
   insertedId: ObjectId("638fea4bb5ed07f127374568")
 }
 mytest> db.fruit.find()
 [ { _id: ObjectId("638fea4bb5ed07f127374568"), name: 'apple' } ]
 
 #向集合批量插入对象
 mytest> db.fruit.insertMany([{name:"pear"},{name:"banana"},{name:"orange"}])
 {
   acknowledged: true,
   insertedIds: {
     '0': ObjectId("638fea5ab5ed07f127374569"),
     '1': ObjectId("638fea5ab5ed07f12737456a"),
     '2': ObjectId("638fea5ab5ed07f12737456b")
   }
 }
 
 #查看所有数据
 mytest>  db.fruit.find()
 [
   { _id: ObjectId("638fea4bb5ed07f127374568"), name: 'apple' },
   { _id: ObjectId("638fea5ab5ed07f127374569"), name: 'pear' },
   { _id: ObjectId("638fea5ab5ed07f12737456a"), name: 'banana' },
   { _id: ObjectId("638fea5ab5ed07f12737456b"), name: 'orange' }
 ]
 mytest>
 
```



### 1.3.2 find

- 使用find查询文档

  关于find：

  - find是MongoDB中查询数据的基本指令，相当于SQL中的select
  - find返回的是游标

  示例:

  ```shell
  #显示集合所有数据
  mytest> db.movies.find()
  [
    {
      _id: ObjectId("638fecba1dc014c873525fc9"),
      year: 1989,
      title: 'Batman',
      category: 'action'
    },
    {
      _id: ObjectId("638fed191dc014c873525fca"),
      year: 1976,
      title: 'My Gril',
      category: 'love'
    },
    {
      _id: ObjectId("638fed831dc014c873525fcb"),
      year: 1975,
      title: 'zhenzi',
      category: 'thrill'
    },
    {
      _id: ObjectId("638fedcd1dc014c873525fcc"),
      year: 1989,
      title: 'first love',
      category: 'love'
    },
    {
      _id: ObjectId("638fedd11dc014c873525fcd"),
      year: 1975,
      title: 'Best Friend',
      category: 'action'
    }
  ]
  
  #单条件查询
  mytest> db.movies.find({"year":1975})
  [
    {
      _id: ObjectId("638fed831dc014c873525fcb"),
      year: 1975,
      title: 'zhenzi',
      category: 'thrill'
    },
    {
      _id: ObjectId("638fedd11dc014c873525fcd"),
      year: 1975,
      title: 'Best Friend',
      category: 'action'
    }
  ]
  
  #多条件and查询
  mytest> db.movies.find({"year":1989,"title":"Batman"})
  [
    {
      _id: ObjectId("638fecba1dc014c873525fc9"),
      year: 1989,
      title: 'Batman',
      category: 'action'
    }
  ]
  
  #and查询的另一种形式
  mytest> db.movies.find({$and:[{"title":"Batman"},{"category":"action"}]})
  [
    {
      _id: ObjectId("638fecba1dc014c873525fc9"),
      year: 1989,
      title: 'Batman',
      category: 'action'
    }
  ]
  
  #多条件Or查询
  mytest> db.movies.find({$or:[{"year":1989},{"title":"Batman"}]})
  [
    {
      _id: ObjectId("638fecba1dc014c873525fc9"),
      year: 1989,
      title: 'Batman',
      category: 'action'
    },
    {
      _id: ObjectId("638fedcd1dc014c873525fcc"),
      year: 1989,
      title: 'first love',
      category: 'love'
    }
  ]
  
  #正则表达式查询
  mytest> db.movies.find({"title":/^B/})
  [
    {
      _id: ObjectId("638fecba1dc014c873525fc9"),
      year: 1989,
      title: 'Batman',
      category: 'action'
    },
    {
      _id: ObjectId("638fedd11dc014c873525fcd"),
      year: 1975,
      title: 'Best Friend',
      category: 'action'
    }
  ]
  
  ```

  

- 查询条件对照表

  | SQL              | MQL                                               |
  | ---------------- | ------------------------------------------------- |
  | a = 1            | { a: 1 }                                          |
  | a <> 1           | { a: { $ne: 1 } }                                 |
  | a > 1            | { a: { $gt: 1 } }                                 |
  | a >= 1           | { a: { $gte: 1 } }                                |
  | a < 1            | { a: { $lt: 1 } }                                 |
  | a <= 1           | { a: { $lte: 1 } }                                |
  | a = 1 AND b = 1  | { a: 1 , b: 1 } 或 { $and: [{ a: 1 }, { b: 1 }] } |
  | a = 1 OR b = 1   | { $or: [{ a: 1 }, { b: 1 }] }                     |
  | a IS NULL        | { a: { $exists: false} }                          |
  | a IN (1,2,3)     | { a: { $in: [1, 2, 3]} }                          |
  | a NOT IN (1,2,3) | { a: { $nin: [1, 2, 3]} }                         |

- 使用find搜索子文档

  find支持使用“field.sub_field”的形式查询子文档

  示例：

  ```shell
  #准备测试数据
  mytest> db.fruit.insertMany([{
  ...     name: "apple",
  ...     from: {
  ...             country:"China",
  ...             province:"Guangdong"
  ...     }
  ... },{
  ...     name: "banana",
  ...     from: {country:"China"}
  ... }])
  {
    acknowledged: true,
    insertedIds: {
      '0': ObjectId("63903e3fb5ed07f12737456d"),
      '1': ObjectId("63903e3fb5ed07f12737456e")
    }
  }
  
  #查看集合所有数据
  mytest> db.fruit.find()
  [
    {
      _id: ObjectId("63903e3fb5ed07f12737456d"),
      name: 'apple',
      from: { country: 'China', province: 'Guangdong' }
    },
    {
      _id: ObjectId("63903e3fb5ed07f12737456e"),
      name: 'banana',
      from: { country: 'China' }
    }
  ]
  
  # 搜索子文档
  mytest> db.fruit.find({"from.country":"China"})
  [
    {
      _id: ObjectId("63903e3fb5ed07f12737456d"),
      name: 'apple',
      from: { country: 'China', province: 'Guangdong' }
    },
    {
      _id: ObjectId("63903e3fb5ed07f12737456e"),
      name: 'banana',
      from: { country: 'China' }
    }
  ]
  
  #注意该处与搜索子文档的区别
  mytest> db.fruit.find({"from": {"country":"China"}})
  [
    {
      _id: ObjectId("63903e3fb5ed07f12737456e"),
      name: 'banana',
      from: { country: 'China' }
    }
  ]
  mytest>
  
  ```

  

  

- 控制find返回的字段

  - find可以指定只返回指定的字段
  - _id字段必须明确指明不反悔，否则默认返回
  - 在MongoBD中我们称这为投影
  - db.movies.find({"category":"action"},{"_id":0,title:1})   //不返回__id,返回title

### 1.3.3 remove

- remove命令需要配合查询条件使用。
- 匹配查询条件的文档会被删除。
- 指定一个空文档条件会删除所有文档。

示例：

| 示例                                 | 说明             |
| ------------------------------------ | ---------------- |
| db.testcol.remove({a:1})             | 删除a等于1的记录 |
| db.testcol.remove({a: { $lt : 5 }} ) | 删除a小于5的记录 |
| db.testcol.remove({})                | 删除所有记录     |
| db.testcol.remove()                  | 报错             |

### 1.3.4 update

​	操作格式：db.<集合>.update(<查询条件>,<更新字段>)

- 使用updateOne表示无论条件匹配多少记录，始终只更新第一条
- 使用updateMany表示条件匹配多少条就更新多少条
- updateOne/updateMany方法要求更新条件部分必须具备以下之一，否则将报错
  - $set
  - $unset
  - $push：增加一个对象到数组底部
  - $pushAll：增加多个对象到数组底部
  - $pop：从数组底部删除一个对象
  - $pull：如果匹配到指定值，从数组中删除相应的对象
  - $pullAll：如果匹配到任意值，从数组中删除相应的对象
  - $addToSet：如果不存在则增加一个值到数组

​	示例：

```shell
mytest> db.fruit.insertMany([ { name: "apple" }, { name: "pear" }, { name: "orange" }])
{
  acknowledged: true,
  insertedIds: {
    '0': ObjectId("639043c0b5ed07f127374575"),
    '1': ObjectId("639043c0b5ed07f127374576"),
    '2': ObjectId("639043c0b5ed07f127374577")
  }
}
mytest> db.fruit.find()
[
  { _id: ObjectId("639043c0b5ed07f127374575"), name: 'apple' },
  { _id: ObjectId("639043c0b5ed07f127374576"), name: 'pear' },
  { _id: ObjectId("639043c0b5ed07f127374577"), name: 'orange' }
]

#更新单条数据
mytest> db.fruit.updateOne({name:"apple"},{$set:{name:"apple test",from:"China"}})
{
  acknowledged: true,
  insertedId: null,
  matchedCount: 1,
  modifiedCount: 1,
  upsertedCount: 0
}

mytest> db.fruit.find()
[
  {
    _id: ObjectId("639043c0b5ed07f127374575"),
    name: 'apple test',
    from: 'China'
  },
  { _id: ObjectId("639043c0b5ed07f127374576"), name: 'pear' },
  { _id: ObjectId("639043c0b5ed07f127374577"), name: 'orange' }
]

#如果不存在$set等关键字，则会报错
mytest> db.fruit.updateOne({name:"apple"},{from:"apple test"})
MongoInvalidArgumentError: Update document requires atomic operators

```



### 1.3.5 drop

- 使用db.<集合>.drop()来删除一个集合（集合中的全部文档都会被删除，集合相关索引也会被删除）
- 使用db.dropDatabase()来删除数据库（数据库相应文件也会被删除，磁盘空间将被释放）

## 1.4 聚合查询

### 1.4.1 基本格式

pipeline = [$stage1, $stage2, ... $stageN];

db.<collection>.aggregate(

​	pipeline,

​	{ options }

)

### 1.4.2 关键字

| 步骤          | 作用     | SQL等价运算符   |
| ------------- | -------- | --------------- |
| $match        | 过滤     | WHERE           |
| $project      | 投影     | AS              |
| $sort         | 排序     | ORDER BY        |
| $group        | 分组     | GROUP BY        |
| $skip/$limit  | 结果限制 | SKIP/LIMIT      |
| $lookup       | 左外连接 | LEFT OUTER JOIN |
| $unwind       | 展开数组 | /               |
| $graphLookup  | 图搜索   | /               |
| $facet/$buket | 分面搜索 | /               |



### 1.4.3 适用场景

| OLTP | OLAP                                                         |
| ---- | ------------------------------------------------------------ |
| 计算 | 分析一段时间内的销售总额、均值<br>计算一段时间内的净利润<br>分析购买人的年龄分布<br>分析学生成绩分布<br>统计员工绩效 |

### 1.4.4 MSQL与SQL对比

![image-20221207182626135](images/image-20221207182626135.png)

![image-20221207182901671](images/image-20221207182901671.png)

![image-20221207183020516](images/image-20221207183020516.png)

![image-20221207183130835](images/image-20221207183130835.png)

![image-20221207183232195](images/image-20221207183232195.png)

### 1.4.5 示例





![image-20221206183221781](images/image-20221206183221781.png)

![image-20221206183256018](images/image-20221206183256018.png)



![image-20221206183506578](images/image-20221206183506578.png)





可使用MongoDB Compass来构建

![image-20221206184329456](images/image-20221206184329456.png)

![image-20221206184347731](images/image-20221206184347731.png)

## 1.5 复制集机制和原理

### 1.5.1 复制集的作用

- MongoDB复制集的主要意义在于实现服务高可用
- 它的实现依赖于两个方面的功能
  - 数据写入时将数据迅速复制到另一个独立节点上
  - 在接受写入的节点发生故障时自动选举出一个新的替代节点

- 在实现高可用的同时，复制集实现了其他几个附加作用
  - 数据分发
  - 读写分离
  - 异地容灾

### 1.5.2 复制集原理

- 典型复制集结构

  一个典型的复制集由3个以上具有投票权的节点组成，包括：

  - 一个主节点：接受写操作和选举投票
  - 两个或多个从节点：复制主节点的新数据和选举投票
  - 不推荐使用投票节点（即只用于投票，不存储数据的节点）

![image-20221207184046174](images/image-20221207184046174.png)

- 数据复制原理

  - 当一个修改操作，无论是插入、更新或删除，到达主节点时，它对数据的操作将被记录下来，这些记录成为oplog
  - 从节点通过在主节点上打开一个tailable游标不到获取新进入主节点的oplog，并在自己的数据时恢复，以此保持跟主节点数据一致

  ![image-20221207184450202](images/image-20221207184450202.png)

- 选举故障恢复

  - 具有投票权的节点之间两两互相发送心跳
  - 当5次心跳未收到时判断为节点失联
  - 如果失联的是主节点，从节点会发起选举，选出新的主节点
  - 如果失联的是从节点则不会产生新的选举
  - 选举基于RAFT一致性算法实现，选举成功的必要条件是大多数投票节点存活
  - 复制集中最多可以有50个节点，但具有投票权的节点最多7个

- 常见选项

  复制集节点有以下常见的选配项

  - 是否具有投票权（v参数）：有则参与投票
  - 优先级（priority参数）：优先级越高的节点越优先成为主节点。优先级为0的节点无法成为主节点
  - 隐藏（hidden参数）：复制数据，但对应用不可见。隐藏节点可以具有投票权，但优先级必须为0
  - 延迟（slaveDelay参数）：复制n秒之前的数据，保持与主节点的时间差。

### 1.5.3 搭建复制集

- docker启动

  ```shell
  docker run --name mongo1 -p 27018:27017  -d mongo mongod --replSet "rs0"
  docker run --name mongo2 -p 27019:27017  -d mongo mongod --replSet "rs0"
  docker run --name mongo3 -p 27020:27017  -d mongo mongod --replSet "rs0"
  ```

  

- docker-compose.yml启动

  启动之前记得把挂载的目录删除掉 rm -rf data/mongodbtest

  ```shell
  version: '2'
  services:
    rs1:
      image: mongo
      volumes:
       - /data/mongodbtest/replset/rs1:/data/db
      ports:
       - 27017:27017
      command: mongosh --dbpath /data/db --replSet rs0
  
    rs2:
      image: mongo
      volumes:
       - /data/mongodbtest/replset/rs2:/data/db
      ports:
       - 27018:27017
      command: mongosh --dbpath /data/db --replSet rs0
  
    rs3:
      image: mongo
      volumes:
       - /data/mongodbtest/replset/rs3:/data/db
      ports:
       - 27019:27017
      command: mongosh --dbpath /data/db --replSet rs0
  ```

  

- 加入副本集，验证复制集功能

  ```shell
  #进入mongo1容器
  docker exec -it mongodb1 /bin/bash
  
  #连接mondb
  mongosh
  
  #初始化副本集 注意的是rs0是启动节点的时候--replSet rs0 的名字
  # 加这个字段"arbiterOnly":true,说明该节点就是仲裁不存放数据
  rs.initiate({"_id": "rs0", "members": [{"_id":0, "host":"192.168.200.101:27018"}, {"_id":1, "host":"192.168.200.101:27019","arbiterOnly":false}, {"_id":2, "host":"192.168.200.101:27020"}]})
  
  #查看副本集配置信息
  rs.conf()
  
  #查看副本集状态信息
  rs.status()
  
  #进入从节点
  docker exec -it mongodb2 /bin/bash
  mongosh
  #从节点开启读数据模式
  rs.secondaryOk()
  
  #验证
  #（1）主节点写入，观察从节点是否可以后取得数据
  #（2）只需要把主节点stop ,然后过一会,进入从节点.当从节点SECONDARY变成 PRIMARY 说明切换成功
  ```

  

## 1.6 MongoDB全家桶



![image-20221206190502394](images/image-20221206190502394.png)

![image-20221206190558411](images/image-20221206190558411.png)

![image-20221206190741515](images/image-20221206190741515.png)

![image-20221206190853372](images/image-20221206190853372.png)

![image-20221206190937772](images/image-20221206190937772.png)





![image-20221206191220473](images/image-20221206191220473.png)

二、模型设计

- json文档模型设计的特点

![image-20221208183253006](images/image-20221208183253006.png)

![image-20221208183313006](images/image-20221208183313006.png)

![image-20221208183355728](images/image-20221208183355728.png)

![image-20221208183437952](images/image-20221208183437952.png)

![image-20221208183526569](images/image-20221208183526569.png)

![image-20221208183541177](images/image-20221208183541177.png)



- 文档模型设计之一：基础设计

![image-20221208183734711](images/image-20221208183734711.png)

![image-20221208183753021](images/image-20221208183753021.png)

![image-20221208183852561](images/image-20221208183852561.png)



![image-20221208183928551](images/image-20221208183928551.png)

![image-20221208184000072](images/image-20221208184000072.png)

![image-20221208184107430](images/image-20221208184107430.png)



- 文档模型设计之一：工况细化

![image-20221208184238362](images/image-20221208184238362.png)

![image-20221208184335181](images/image-20221208184335181.png)

![image-20221208184431611](images/image-20221208184431611.png)

![image-20221208184521511](images/image-20221208184521511.png)

![image-20221208184714182](images/image-20221208184714182.png)

![image-20221208184832390](images/image-20221208184832390.png)



![image-20221208185131626](images/image-20221208185131626.png)

- 文档模型设计之三：模式套用

![image-20221208185352598](images/image-20221208185352598.png)

![image-20221208185509522](images/image-20221208185509522.png)

![image-20221208185609252](images/image-20221208185609252.png)

![image-20221208185633509](images/image-20221208185633509.png)



![image-20221208185713655](images/image-20221208185713655.png)

![image-20221208185935237](images/image-20221208185935237.png)

![image-20221208190020961](images/image-20221208190020961.png)

- 设计模式集锦

  ![image-20221208190107332](images/image-20221208190107332.png)

  ![image-20221208190200581](images/image-20221208190200581.png)

  ![image-20221208190302581](images/image-20221208190302581.png)

  

  ![image-20221208190345252](images/image-20221208190345252.png)

  ![image-20221208190425076](images/image-20221208190425076.png)

  ![image-20221208190443492](images/image-20221208190443492.png)

  ![image-20221208190519536](images/image-20221208190519536.png)

  ![image-20221208190606848](images/image-20221208190606848.png)

  ![image-20221208190631798](images/image-20221208190631798.png)

  ![image-20221208190648464](images/image-20221208190648464.png)

  ![image-20221208190734766](images/image-20221208190734766.png)

  ![image-20221208190809459](images/image-20221208190809459.png)

  

  

  
