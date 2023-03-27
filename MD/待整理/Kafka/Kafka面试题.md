1、**Kafka 中的 ISR(InSyncRepli)、OSR(OutSyncRepli)、AR(AllRepli)代表什么？**

```shell
ISR(InSyncRepli)：内部副本同步队列
OSR(OutSyncRepli)：外部副本同步队列
AR(AllRepli)：所有副本
AR = ISR + OSR
```

**2、Kafka 中的 HW、LEO 等分别代表什么？** 

```shell
HW：所有副本的最小的LEO
LEO：每个副本的最后一个offset
```

**3、Kafka 中是怎么体现消息顺序性的？**

```shell
Kafka中的broker中的每个topic的partition的消息在写入时都是有序的。消费时，每个partition只能被每一个消费者组的一个消费者消费，保证了消费时也是有序的。但一个Topic的多个partition不能保证有序。
```

**4、Kafka 中的分区器、序列化器、拦截器是否了解？它们之间的处理顺序是什么？** 

```shell
分区器->序列化器->拦截器
```

**5、“消费组中的消费者个数如果超过 topic 的分区，那么就会有消费者消费不到数据”这句 话是否正确？**

```shell
正确。
为了保证数据消费的有序性，一个消费者实例只能消费一个Partition，所以如果消费者实例多了，那么会出现消费者空闲的情况。如果是自定义分区，可以继承AbstractPartitionAssignor实现自定义消费策略，从而实现同一消费组内的任意消费者都可以消费订阅主题的所有分区
```

**7、消费者提交消费位移时提交的是当前消费到的最新消息的 offset 还是 offset+1？**  

```shell
offset+1
```

**8、有哪些情形会造成重复消费** 

```shell
消费者消费后没有提交offset(程序崩溃/强行kill/消费耗时/自动提交偏移情况下
```

**9、哪些情景会造成消息漏消费？** 

```shell
消费者没有处理完消息就提交offset(自动提交偏移 未处理情况下程序异常结束)
```

**10、当你使用 kafka-topics.sh 创建（删除）了一个 topic 之后，Kafka 背后会执行什么逻辑？**

```shell
	1）会在 zookeeper 中的/brokers/topics 节点下创建一个新的 topic 节点，如： /brokers/topics/first
	2）触发 Controller 的监听程序 
	3）kafka Controller 负责 topic 的创建工作，并更新 metadata cache 
```

**11、topic 的分区数可不可以增加？如果可以怎么增加？如果不可以，那又是为什么？** 

```shell
可以
```

**12、topic 的分区数可不可以减少？如果可以怎么减少？如果不可以，那又是为什么？** 

```shell
不可以
```

**13、Kafka 有内部的 topic 吗？如果有是什么？有什么所用？** 

```shell
有，__consumer_offsets，保存消费者offset
```

**14、Kafka 分区分配的概念？** 

```shell
一个topic由多个分区组成，一个消费者组有多个消费者，故需要将分区分配给消费者，即确定哪个partition由哪个consumer来消费
roundrobin、range两种分配方式。
```

**15、聊一聊 Kafka Controller 的作用？** 

```shell
负责管理集群broker的上下线，所有topic的分区副本分配和leader选举等工作
```

**16、Kafka 中有那些地方需要选举？这些地方的选举策略又有哪些？** 

```shell
partition leader（ISR），controller（先到先得）
```

**17、失效副本是指什么？有那些应对措施？** 

```shell
不能及时与leader同步，暂时踢出ISR，等其追上leader之后再重新加入
```

**18、Kafka 的哪些设计让它有如此高的性能？** 

```shell
(1)分区
(2)Cache Filesystem Cache PageCache缓存
(3)顺序写磁盘，由于现代的操作系统提供了预读和写技术，磁盘的顺序写大多数情况下比随机写内存还要快。
(4)Batching of Messages 批量量处理。合并小的请求，然后以流的方式进行交互，直顶网络上限
(5)Pull 拉模式 使用拉模式进行消息的获取消费，与消费端处理能力相符。
(6)0-copy 零拷技术减少拷贝次数

```

