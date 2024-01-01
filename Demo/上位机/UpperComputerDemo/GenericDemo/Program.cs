#region 出栈入栈操作
//static void Main(string[] args)
//{

//    ////【1】创建泛型对象
//    //MyStack<int> stack1 = new MyStack<int>(5);
//    ////【2】入栈
//    //stack1.Push(1);
//    //stack1.Push(2);
//    //stack1.Push(3);
//    //stack1.Push(4);
//    //stack1.Push(5);
//    ////【3】出栈
//    //Console.WriteLine(stack1.Pop());
//    //Console.WriteLine(stack1.Pop());
//    //Console.WriteLine(stack1.Pop());
//    //Console.WriteLine(stack1.Pop());
//    //Console.WriteLine(stack1.Pop());

//    //【1】创建泛型对象
//    MyStack<string> stack1 = new MyStack<string>(5);
//    //【2】入栈
//    stack1.Push("产品1");
//    stack1.Push("产品2");
//    stack1.Push("产品3");
//    stack1.Push("产品4");
//    stack1.Push("产品5");
//    //【3】出栈
//    Console.WriteLine(stack1.Pop());
//    Console.WriteLine(stack1.Pop());
//    Console.WriteLine(stack1.Pop());
//    Console.WriteLine(stack1.Pop());
//    Console.WriteLine(stack1.Pop());

//    Console.ReadLine();

//    //总结：以上泛型类的使用，增加了类型安全，实例化时int类型的约束下是不能添加string类型的。无需拆装箱操作。


//}
#endregion

#region 测试带约束的泛型类

//【1】实例化泛型类型对象
MyGenericCalss2<int, Course, Teacher> myClass2 = new MyGenericCalss2<int, Course, Teacher>();

//【2】给对象属性赋值
myClass2.Publisher = new Teacher() { Name = "常老师", Count = 20 };
myClass2.ProductList = new List<Course>()
            {
                new Course(){CourseName =".NET-CS+BS高级工程师VIP班",Period=6},
                new Course(){CourseName =".NET-CS高级工程师VIP班",Period=3},
                new Course(){CourseName =".NET-BS高级工程师VIP班",Period=3},
            };

//【3】调用对象方法
Course myCourse = myClass2.Buy(0);
//数据处理
string info = string.Format("我购买的课程是：{0}  学期:{1}个月  课程主讲：{2}",
    myCourse.CourseName,
    myCourse.Period,
    myClass2.Publisher.Name);
Console.WriteLine(info);

Console.ReadLine();

#endregion

#region 测试泛型方法
Console.WriteLine(Add1("大家好", "我正在学习常老师的VIP课程！"));
Console.WriteLine("20+30={0}", Add1(20, 30));

Console.WriteLine("-----------------");
Console.WriteLine("20.5+56.5={0}", Add1(20.5, 56.5));
Console.WriteLine("-----------------");
Console.WriteLine("20.5+56={0}", Add2(20.5, 56));
Console.WriteLine("-----------------");
Console.WriteLine("20.5-56={0}", Sub(20.5, 56));
Console.WriteLine("-----------------");
Console.WriteLine("20.5*56={0}", Multiply(20.5, 56));

Console.WriteLine("---------------计算一个数的求和----------------");
Console.WriteLine(Sum(10));
Console.ReadLine();


//写一个计算器，要求使用泛型方法，并且基于面向对象思想
//边际报酬递减   
//年薪30万    价值放大-->细节的深化 +  同一个知识的不断研究  
//成功就是复杂的事情简单的做，简单的事情重复的做，重复的事情认真的做
//简单的事情重复做，你就是专家；重复的事情用心做，你就是赢家


#region 【1】实现四则混合运算

static T Add1<T>(T a, T b)
{
    dynamic a1 = a;
    dynamic b1 = b;
    return a1 + b1;
}
static T Add2<T>(T a, T b) where T : struct
{
    dynamic a1 = a;
    dynamic b1 = b;
    return a1 + b1;
}
static T Sub<T>(T a, T b) where T : struct
{
    dynamic a1 = a;
    dynamic b1 = b;
    return a1 - b1;
}

static T Multiply<T>(T a, T b) where T : struct
{
    dynamic a1 = a;
    dynamic b1 = b;
    return a1 * b1;
}
static T Div<T>(T a, T b) where T : struct
{
    dynamic a1 = a;
    dynamic b1 = b;
    return a1 / b1;
}
#endregion

#region 【2】编写一个泛型方法实现一个数求和

static T Sum<T>(T a) where T : struct
{
    dynamic sum = a;
    for (dynamic i = 0; i < a; i++)
    {
        sum += i;
    }
    return (T)sum;
    #endregion
}

#endregion

#region 1.default关键字的使用

class MyGenericClass1<T1, T2>
{
    private T1? obj1;

    private MyGenericClass1()
    {
        //obj1 = null; //不能这么用
        //obj1 = new T1();//不能随便假设某种类型，这种类型也许没有构造方法，也许是私有的。

        //解决方法：default关键字
        obj1 = default(T1);//如果T1是引用类型，就赋值null，如果是值类型就给默认值，对于数值类型就是0，结构类型的话要依据具体的成员类型确定为0或者null
    }
}

#endregion

#region 2.添加约束类型的泛型类

class MyGenericCalss2<T1, T2, T3>
    where T1 : struct //说明：类型必须是值类型
    where T2 : class //说明：类型必须是引用类型
    where T3 : new() //说明：类型必须有一个无参数的构造方法，且必须放到最后
                     //其他类型：基类类型、接口类型
{
    //产品列表
    public List<T2> ProductList { get; set; }

    //发行者
    public T3 Publisher { get; set; }

    public MyGenericCalss2()
    {
        ProductList = new List<T2>();
        Publisher = new T3();
    }
    /// <summary>
    /// 购买第几个产品
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public T2 Buy(T1 num)
    {
        // return ProductList[num];//直接写是错误的。因为数组通过下标访问的时候必须是int类型，而int和T1又是冲突的

        dynamic a = num;
        return ProductList[a];
    }

}

#endregion

#region 3.根据泛型类的要求设计参数

class Course
{
    /// <summary>
    /// 课程名称
    /// </summary>
    public string? CourseName { get; set; }
    /// <summary>
    /// 学习周期
    /// </summary>
    public int Period { get; set; }
}

class Teacher
{
    public string? Name { get; set; }//姓名
    public int Count { get; set; }//授课数量
}

#endregion