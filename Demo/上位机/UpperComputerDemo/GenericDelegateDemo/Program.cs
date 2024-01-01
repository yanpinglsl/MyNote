namespace GenericDelegateDemo
{
    #region 泛型委托：匿名方法、Lambda表达式
    //class Program
    //{
    //    //泛型委托：匿名方法、Lambda表达式
    //    static void Main(string[] args)
    //    {
    //        //【1】使用委托
    //        MyGenericDeleage<int> objDelegate1 = Add;
    //        MyGenericDeleage<double> objDelegate2 = Sub;

    //        Console.WriteLine(objDelegate1(10, 20));
    //        Console.WriteLine(objDelegate2(10, 20));
    //        Console.WriteLine("-----------------");

    //        //【2】使用匿名方法
    //        MyGenericDeleage<int> objDelegate3 = delegate (int a, int b) { return a + b; };
    //        MyGenericDeleage<double> objDelegate4 = delegate (double a, double b) { return a - b; };

    //        Console.WriteLine(objDelegate3(10, 20));
    //        Console.WriteLine(objDelegate4(10, 20));
    //        Console.WriteLine("-----------------");

    //        //【3】使用Lambda表达式
    //        MyGenericDeleage<int> objDelegate5 = (a, b) => a + b;
    //        MyGenericDeleage<double> objDelegate6 = (a, b) => a - b;

    //        Console.WriteLine(objDelegate5(10, 20));
    //        Console.WriteLine(objDelegate6(10, 20));
    //        Console.ReadLine();

    //    }


    //    static int Add(int a, int b)
    //    {
    //        return a + b;
    //    }
    //    static double Sub(double a, double b)
    //    {
    //        return a - b;
    //    }


    //}
    ////定义泛型委托
    //public delegate T MyGenericDeleage<T>(T obj1, T obj2);
    #endregion

    #region  Func系列委托
    // /*
    //* 为了方法开发者使用，.NET基类库针对在实践中最常用的情况，提供了几个预定好的委托，这些委托使用非常广泛
    //* 尤其在编写Lambda表达式和开发并行计算的时候经常遇到
    //*/
    // class Program
    // {
    //     #region 【1】Func委托的基本使用
    //     //static void Main(string[] args)
    //     //{
    //     //    //Func<int, int, double> func = Add;
    //     //    //double result = func(10, 20);

    //     //    Func<int, int, double> func = (a, b) => a + b;


    //     //    Console.WriteLine(func(10,20));

    //     //    Console.ReadLine();
    //     //}

    //     //static double Add(int a, int b)
    //     //{
    //     //    return a + b;
    //     //}


    //     #endregion

    //     #region 【2】Func委托的重要使用

    //     static void Main(string[] args)
    //     {
    //         int[] nums = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    //         //Console.WriteLine(GetSum(nums, 0, 3));

    //         //Console.WriteLine(GetMulti(nums, 0, 3));

    //         Console.WriteLine(CommonMethod(Add, nums, 0, 3));
    //         Console.WriteLine(CommonMethod(Multiply, nums, 0, 3));

    //         //直接使用Lambda表达式
    //         Console.WriteLine(CommonMethod((a, b) => a + b, nums, 0, 3));
    //         Console.WriteLine(CommonMethod((a, b) => a * b, nums, 0, 3));

    //         Console.ReadLine();
    //     }
    //     //编写一个方法，从数组中指定位置抽取3个数，求和、求积  int[] nums={10,9,8,7,6,5,4,3,2};
    //     //【0-3】

    //     static int GetSum(int[] nums, int from, int to)
    //     {
    //         int result = 0;
    //         for (int i = from; i <= to; i++)
    //         {
    //             result += nums[i];
    //         }
    //         return result;
    //     }
    //     static int GetMulti(int[] nums, int from, int to)
    //     {
    //         int result = 1;
    //         for (int i = from; i <= to; i++)
    //         {
    //             result *= nums[i];
    //         }
    //         return result;
    //     }

    //     //使用Func委托，将“运算”本身作为方法参数
    //     static int CommonMethod(Func<int, int, int> operation, int[] nums, int a, int b)
    //     {
    //         int result = nums[a];//把第一个值，作为基数(a+1:表示下一个值)
    //         for (int i = a + 1; i <= b; i++)
    //         {
    //             result = operation(result, nums[i]);
    //         }
    //         return result;
    //     }

    //     static int Add(int i, int j)
    //     {
    //         return i + j;
    //     }
    //     static int Multiply(int i, int j)
    //     {
    //         return i * j;
    //     }

    //     #endregion
    // }

    // //【1】Func系列委托：多个重载版本
    // //public delegate TResult Func<TResult>();
    // //public delegate TResult Func<T,TResult>(T arg);
    // //public delegate TResult Func<T1,T2,TResult>(T1 arg1,T2 arg2);
    // //public delegate TResult Func<T1,T2,T3,TResult>(T1 arg1,T2 arg2,T3 arg3);
    // //public delegate TResult Func<T1,T2,T3,T4,TResult>(T1 arg1,T2 arg2,T3 arg3,T4 args);

    // /*
    //  *【注意问题】
    //  * Func委托声明的最后一个泛型类型参数是委托所接收方法的《返回值类型》
    //  * 如果前面有泛型类型的参数，这个参数就是委托方法的形参类型
    //  * 
    //  *【牢记内容】
    //  * Func委托系列引用一个《有返回值的方法》，也就是将方法作为另一个方法的“参数”
    //  */

    #endregion

    #region Action委托
    ///*
    // *Func委托必须要求所接收的方法有一个返回值，那么没有返回值的方法怎么办？
    // *Action委托接收一个没有返回值的方法
    // *应用：在跨线程访问可视化控件的时候经常使用。
    // */
    //class Program
    //{
    //    static void Main(string[] args)
    //    {

    //        Action<string> act = (a) => Console.WriteLine("欢迎参加喜科堂{0}老师讲解的VIP课程！", a);

    //        act("常");
    //        Console.ReadLine();
    //    }
    //}

    ///*
    // * Action委托与Func系列类似，有若干个重载方法...,可以接收0-4个参数，且返回值为void类型的方法
    // * public delegate void Action();
    // * public delegate void Action<T>(T obj);
    // * public delegate void Action<T1,T2>(T1 arg1,T2 arg2);
    // * public delegate void Action<T1,T2,T3>(T1 arg1,T2 arg2,T3 arg3);
    // * public delegate void Action<T1,T2,T3,T4>(T1 arg1,T2 arg2,T3 arg3,T4 arg4);
    // * 
    // *
    //*/
    #endregion

    #region Predicate委托
    /*
     * Predicate<T>委托定义如下：
     * public delegate bool Predicate<T>(T obj);
     * 解释：此委托返回一个bool值的方法
     * 在实际开发中，Predicate<T> 委托变量引用一个“判断条件函数”，
     * 在判断条件函数内部书写代码表明函数参数所引用的对象应该满足的条件，条件满足时返回true
     */
    class Program
    {
        static void Main(string[] args)
        {
            List<Student> stuList = new List<Student>()
            {
                   new Student(){ StudentId=1001,StudentName="小张"},
                   new Student(){ StudentId=1008,StudentName="小李"},
                   new Student(){ StudentId=1009,StudentName="小王"},
                   new Student(){ StudentId=1003,StudentName="小赵"},
                   new Student(){ StudentId=1005,StudentName="小刘"}
            };
            //查询学号大于1003的学员
            //List<T>集合中定义了一个FindAll方法：public T FindAll(Predicate<T> match)

            List<Student> list = stuList.FindAll(s => s.StudentId > 1003);

            foreach (var student in list)
            {
                Console.WriteLine(student.StudentId);
            }
            Console.ReadLine();
        }
    }
    #endregion
}