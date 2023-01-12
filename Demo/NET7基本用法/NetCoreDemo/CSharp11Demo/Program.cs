using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using CSharp11Demo.example;

namespace CSharp11Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("================C#11新特性===============");
            Console.WriteLine("=========================================");
            try
            {

                #region 1. 抽象和静态方法 
                {
                    //C# 11 开始将 abstract 和 virtual 引入到静态方法中，允许开发者在接口中编写抽象和虚静态方法。

                    //接口与抽象类不同:
                    //接口用来抽象行为，通过不同类型实现接口来实现多态；
                    //而抽象类则拥有自己的状态，通过各子类型继承父类型来实现多态。这是两种不同的范式。 
                    //在 C# 11 中，虚静态方法的概念被引入，在接口中可以编写抽象和虚静态方法了。

                    //详见：接口IFoo  实现类Bar

                    int iResult = Bar.Foo1(); // ok
                }
                {
                    //由于运算符也属于静态方法。
                    //因此从 C# 11 开始，也可以用接口来对运算符进行抽象了。
                    //详见：接口ICanAdd<T>

                    //这样我们就可以给自己的类型实现该接口了，例如实现一个二维的点 Point：
                    //详见：Point

                    //然后我们就可以对两个 Point 进行相加了：
                    Point p1 = new Point(1, 2);
                    Point p2 = new Point(2, 3);
                    Console.WriteLine(p1 + p2); // Point { X = 3, Y = 5 }
                }

                {
                    //除了隐式实现接口之外，我们也可以显式实现接口：
                    //不过用显示实现接口的方式的话，+ 运算符没有通过 public 公开暴露到类型 Point 上，因此我们需要通过接口来调用 + 运算符，这可以利用泛型约束来做到： 

                    Point p1 = new Point(1, 2);
                    Point p2 = new Point(2, 3);

                    Point pResult = Add(p1, p2);

                    Console.WriteLine(pResult); // Point { X = 3, Y = 5 } 

                    T Add<T>(T left, T right) where T : ICanAdd<T>
                    {
                        return left + right;
                    }
                }

                {
                    //对于不是运算符的情况。则可以利用泛型参数来调用接口上的抽象和静态方法：
                    Bar.Foo1(); // ok
                    CallFoo1<Bar>(); // ok

                    void CallFoo1<T>() where T : IFoo
                    {
                        T.Foo1();
                    }
                }

                {
                    //此外，接口可以基于另一个接口扩展，因此对于抽象和虚静态方法而言。
                    //我们可以利用这个特性在接口上实现多态。
                    CallFoo<Bar1>(); // 5 5
                    CallFoo<Bar2>(); // 6 4
                    CallFoo<Bar3>(); // 3 7
                    CallFooFromIA<Bar4>(); // 1
                    CallFooFromIB<Bar4>(); // 2

                    void CallFoo<T>() where T : IC
                    {
                        CallFooFromIA<T>();
                        CallFooFromIB<T>();
                    }

                    void CallFooFromIA<T>() where T : IA
                    {
                        Console.WriteLine(T.Foo());
                    }

                    void CallFooFromIB<T>() where T : IB
                    {
                        Console.WriteLine(T.Foo());
                    }
                }

                {
                    //同时，.NET 7 也利用抽象和虚静态方法，对基础库中的数值类型进行了改进。在 System.Numerics 中新增了大量的用于数学的泛型接口，允许用户利用泛型编写通用的数学计算代码： 
                    V Eval<T, U, V>(T a, U b, V c) where T : IAdditionOperators<T, U, U>
                                                    where U : IMultiplyOperators<U, V, V>
                    {
                        return (a + b) * c;
                    }
                    Console.WriteLine(Eval(3, 4, 5)); // 35
                    Console.WriteLine(Eval(3.5f, 4.5f, 5.5f)); // 44 
                }
                #endregion 

                #region 2.泛型 attribute
                {
                    //C# 11 正式允许用户编写和使用泛型 attribute，因此我们可以不再需要使用 Type 来在 attribute 中存储类型信息，这不仅支持了类型推导，还允许用户通过泛型约束在编译时就能对类型进行限制。

                    [Foo<int>(3)] // ok
                    [Foo<float>(4.5f)] // ok
                                       //[Foo<string>("test")] // error
                    void MyFancyMethod()
                    {
                    }

                    MyFancyMethod();
                }
                #endregion

                #region 3.ref 字段和 scoped ref

                {
                    //C# 11 开始，开发者可以在 ref struct 中编写 ref 字段，这允许我们将其他对象的引用存储在一个 ref struct 中：
                    int x = 1;
                    Foo foo = new Foo(ref x);
                    foo.X = 2;
                    Console.WriteLine(x); // 2
                }
                //这里可以发现一个问题，那就是 ref field 的存在，可能会使得一个 ref 指向的对象的生命周期被扩展而导致错误，例如：
                {
                    ///////局部方法
                    //Foo MyFancyMethod()
                    //{
                    //    int x = 1;
                    //    Foo foo = new Foo(ref x);
                    //    return foo; // error
                    //}

                    ////上述代码编译时会报错，因为 foo 引用了局部变量 x，而局部变量 x 在函数返回后生命周期就结束了，但是返回 foo 的操作使得 foo 的生命周期比 x 的生命周期更长，这会导致无效引用的问题，因此编译器检测到了这一点，不允许代码通过编译。


                }
                ////但是上述代码中，虽然 foo 确实引用了 x，但是 foo 对象本身并没有长期持有 x 的引用，因为在构造函数返回后就不再持有对 x 的引用了，因此这里按理来说不应该报错。于是 C# 11 引入了 scoped 的概念，允许开发者显式标注 ref 的生命周期，标注了 scoped 的 ref 表示这个引用的生命周期不会超过当前函数的生命周期：
                {
                    FooNew MyFancyMethod()
                    {
                        int x = 1;
                        FooNew foo = new FooNew(ref x);
                        return foo; // ok
                    }
                    FooNew fooNew = MyFancyMethod();
                }
                //这样一来，编译器就知道 FooNew2 的构造函数不会使得 FooNew2 在构造函数返回后仍然持有 x 的引用，因此上述代码就能安全通过编译了。如果我们试图让一个 scoped ref 逃逸出当前函数的话，编译器就会报错：

                //如此一来，就实现了引用安全。
                {
                    //详见：FooNew2
                }


                //在字段中，ref 还可以配合 readonly 一起使用，用来表示不可修改的 ref，例如：
                //1.ref int：一个 int 的引用
                //2.readonly ref int：一个 int 的只读引用
                //3.ref readonly int：一个只读 int 的引用
                //4.readonly ref readonly int：一个只读 int 的只读引用 
                {
                    ReadonlyRef readonlyRef = new ReadonlyRef();
                }

                #endregion

                #region 4.文件局部类型
                {
                    InfoShow infoShow = new InfoShow();
                    infoShow.Show();

                    //var foo = new  FooFile(); // error      //file修饰的类，必须在这个类文件内部才能访问
                    //var bar = new BarFile(); // error      //file修饰的类，必须在这个类文件内部才能访问 

                    //这个特性将可访问性的粒度精确到了文件，对于代码生成器等一些要放在同一个项目中，但是又不想被其他人接触到的代码而言将会特别有用。 
                }
                #endregion

                #region 5.required 成员
                {
                    //C# 11 新增了 required 成员，标记有 required 的成员将会被要求使用时必须要进行初始化，例如：

                    // var foo1 = new RequiredFoo(); // error  
                    var foo2 = new RequiredFoo { X = 1 }; // ok 

                    //var foo3 = new RequiredFooClass(); // error   
                    //var foo4 = new RequiredFooClass() { X=1 }; // ok   
                    var foo4 = new RequiredFooClass(234) { X = 234 };
                }

                //开发者还可以利用 SetsRequiredMembers 这个 attribute 来对方法进行标注，表示这个方法会初始化 required 成员，因此用户在使用时可以不需要再进行初始化：
                {
                    //var p1 = new PointRequiredMembers(); // error
                    var p2 = new PointRequiredMembers { X = 1, Y = 2 }; // ok
                    var p3 = new PointRequiredMembers(1, 2); // ok
                }
                //利用 required 成员，我们可以要求其他开发者在使用我们编写的类型时必须初始化一些成员，使其能够正确地使用我们编写的类型，而不会忘记初始化一些成员。
                #endregion

                #region 6.checked 运算符
                {
                    //C# 自古以来就有 checked 和 unchecked 概念，分别表示检查和不检查算术溢出：
                    //不同的类型有数据存储的长度:

                    //byte的取值范围：0-255

                    byte x = 100;
                    byte y = 200;


                    unchecked
                    {
                        byte z = (byte)(x + y); // ok
                    }

                    //checked
                    //{
                    //    byte z = (byte)(x + y); // error
                    //}


                }

                //在 C# 11 中，引入了 checked 运算符概念，允许用户分别实现用于 checked 和 unchecked 的运算符：
                {
                    var foo1 = new CheckFoo();
                    var foo2 = new CheckFoo();
                    var foo3 = unchecked(foo1 + foo2); // 调用 operator +
                    var foo4 = checked(foo1 + foo2); // 调用 operator checked +


                    //对于自定义运算符而言，实现 checked 的版本是可选的，如果没有实现 checked 的版本，则都会调用 unchecked 的版本。
                }
                #endregion

                #region 8.IntPtr、UIntPtr 支持数值运算 
                {
                    //C# 11 中，IntPtr 和 UIntPtr 都支持数值运算了，这极大的方便了我们对指针进行操作：
                    {
                        UIntPtr addr = 0x80000048;
                        IntPtr offset = 0x00000016;
                        UIntPtr newAddr = addr + (UIntPtr)offset; // 0x8000005E
                    }

                    //当然，如同 Int32 和 int、Int64 和 long 的关系一样，C# 中同样存在 IntPtr 和 UIntPtr 的等价简写，分别为 nint 和 nuint，n 表示 native，用来表示这个数值的位数和当前运行环境的内存地址位数相同： 
                    {
                        nuint addr = 0x80000048;
                        nint offset = 0x00000016;
                        nuint newAddr = addr + (nuint)offset; // 0x8000005E
                    }

                }
                #endregion

                #region 9.列表模式匹配
                {
                    //C# 11 中新增了列表模式，允许我们对列表进行匹配。在列表模式中，我们可以利用 [ ] 来包括我们的模式，用 _ 代指一个元素，用 .. 代表 0 个或多个元素。在 .. 后可以声明一个变量，用来创建匹配的子列表，其中包含 .. 所匹配的元素。

                    var array = new int[] { 1, 2, 3, 4, 5 };
                    if (array is [1, 2, 3, 4, 5])
                    {
                        Console.WriteLine(1);// 1
                    }

                    if (array is [1, 2, 3, ..])
                    {
                        Console.WriteLine(2);// 2
                    }

                    if (array is [1, _, 3, _, 5])
                    {
                        Console.WriteLine(3);// 3
                    }

                    if (array is [.., _, 5])
                    {
                        Console.WriteLine(4); // 4
                    }

                    if (array is [1, 2, 3, .. var remaining])
                    {
                        Console.WriteLine(remaining[0]); // 4
                        Console.WriteLine(remaining.Length); // 2
                    }
                }

                //当然，和其他的模式一样，列表模式同样是支持递归的，因此我们可以将列表模式与其他模式组合起来使用：

                {
                    var array = new string[] { "hello", ",", "world", "~" };
                    if (array is ["hello", _, { Length: 5 }, { Length: 1 } elem, ..])
                    {
                        Console.WriteLine(elem); // ~
                    }
                }

                //除了在 if 中使用模式匹配以外，在 switch 中也同样能使用：
                {
                    var array = new string[] { "hello", ",", "world", "!" };
                    switch (array)
                    {
                        case ["hello", _, { Length: 5 }, { Length: 1 } elem, ..]:
                            // ...
                            break;
                        default:
                            // ...
                            break;
                    }

                    var value = array switch
                    {
                        ["hello", _, { Length: 5 }, { Length: 1 } elem, ..] => 1,
                        _ => 2
                    };
                    Console.WriteLine(value); // 1
                }
                #endregion

                #region 10.对 Span<char> 的模式匹配 
                {
                    //在 C# 中，Span<char> 和 ReadOnlySpan<char> 都可以看作是字符串的切片，因此 C# 11 也为这两个类型添加了字符串模式匹配的支持。例如：

                    int Foo(ReadOnlySpan<char> span)
                    {
                        if (span is "abcdefg")
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }

                    Foo("abcdefg".AsSpan()); // 1
                    Foo("test".AsSpan()); // 2
                }
                #endregion

                #region 11.原始字符串
                {
                    //C# 中自初便有 @ 用来表示不需要转义的字符串，但是用户还是需要将 " 写成 "" 才能在字符串中包含引号。C# 11 引入了原始字符串特性，允许用户利用原始字符串在代码中插入大量的无需转移的文本，方便开发者在代码中以字符串的方式塞入代码文本等。

                    //原始字符串需要被至少三个 " 包裹，例如 """ 和 """"" 等等，前后的引号数量要相等。
                    //另外，原始字符串的缩进由后面引号的位置来确定，例如：

                    //此时 str 是：带有换行符的字符串
                    {
                        string str = """ 
                                      hello 
                                      world 
                                     """;
                        Console.WriteLine(str);
                    }

                    //而如果是下面这样：
                    {
                        var str = """"
                                hello
                                    world
                                """";
                        Console.WriteLine(str);
                    }

                    {
                        //可以直接定义JSON格式
                        var json = """"
                                    {
                                        "a": 1,
                                        "b": {
                                               "c": "hello",
                                               "d": "world"
                                             },
                                        "c": [1, 2, 3, 4, 5]
                                    }
                                    """";
                        Console.WriteLine(json);
                        object obj = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(json);
                        Hashtable tb = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(json);
                    }

                    {
                        int age = 37;
                        string? jsonResult = $$"""
                                            {
                                               "Id":123,
                                               "Name":"Richard",
                                               "Age":"{{age}}"
                                            }
                                            """;
                    }
                }
                #endregion

                #region 12.UTF-8 字符串
                {
                    //C# 11 引入了 UTF-8 字符串，我们可以用 u8 后缀来创建一个 ReadOnlySpan<byte>，其中包含一个 UTF-8 字符串：
                    {
                        var str1 = "hello world"u8; // ReadOnlySpan<byte>
                        var str2 = "hello world"u8.ToArray(); // byte[]
                    }

                    //UTF-8 对于 Web 场景而言非常有用，因为在 HTTP 协议中，默认编码就是 UTF-8，而 .NET 则默认是 UTF-16 编码，因此在处理 HTTP 协议时，如果没有 UTF-8 字符串，则会导致大量的 UTF-8 和 UTF-16 字符串的相互转换，从而影响性能。

                    //有了 UTF-8 字符串后，我们就能非常方便的创建 UTF-8 字面量来使用了，不再需要手动分配一个 byte[] 然后在里面一个一个硬编码我们需要的字符。
                }
                #endregion

                #region 13.字符串插值允许换行
                {
                    //C# 11 开始，字符串的插值部分允许换行，因此如下代码变得可能：
                    object group = new object();
                    var str = $"hello, the object name is {group.
                                                                GetType()
                                                                .FullName}.";

                    //这样一来，当插值的部分代码很长时，我们就能方便的对代码进行格式化，而不需要将所有代码挤在一行。

                }
                #endregion

                #region 14.struct 自动初始化
                {
                    PointStruct pointStruct = new PointStruct(123);
                    Console.WriteLine(pointStruct.X);
                    Console.WriteLine(pointStruct.Y);
                }
                #endregion

                #region 15.支持对其他参数名进行 nameof
                {
                    //C# 11 允许了开发者在参数中对其他参数名进行 nameof，例如在使用 CallerArgumentExpression 这一 attribute 时，此前我们需要直接硬编码相应参数名的字符串，而现在只需要使用 nameof 即可：

                    void Assert(bool condition, [CallerArgumentExpression(nameof(condition))] string expression = "")
                    {
                        // ...
                    }

                    //这将允许我们在进行代码重构时，修改参数名 condition 时自动修改 nameof 里面的内容，方便的同时减少出错。

                }
                #endregion

                #region 16.自动缓存静态方法的委托
                {
                    //C# 11 开始，从静态方法创建的委托将会被自动缓存，例如：
                    void Foo()
                    {
                        Action action = Console.WriteLine;
                        Call(action);
                    }

                    void Call(Action action)
                    {
                        action();
                    }

                    //此前，每执行一次 Foo，就会从 Console.WriteLine 这一静态方法创建一个新的委托，因此如果大量执行 Foo，则会导致大量的委托被重复创建，导致大量的内存被分配，效率极其低下。
                    //在 C# 11 开始，将会自动缓存静态方法的委托，因此无论 Foo 被执行多少次，Console.WriteLine 的委托只会被创建一次，节省了内存的同时大幅提升了性能。

                }
                #endregion

                //从 C# 8 开始，C# 团队就在不断完善语言的类型系统，在确保静态类型安全的同时大幅提升语言表达力。
                //从而让类型系统成为编写程序的得力助手，而不是碍手碍脚的限制。

                //本次更新还完善了数值运算相关的内容，使得开发者利用 C# 编写数值计算方法时更加得心应手。

                //另外，模式匹配的探索旅程也终于接近尾声，引入列表模式之后，剩下的就只有字典模式和活动模式了。
                //模式匹配是一个非常强大的工具，允许我们像对字符串使用正则表达式那样非常方便地对数据进行匹配。

                //总的来说 C# 11 的新特性和改进内容非常多，每一项内容都对 C# 的使用体验有着不小的提升。
                //在未来的 C# 中还计划着角色和扩展等更加令人激动的新特性，让我们拭目以待。

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}