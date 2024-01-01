
//【3】创建委托对象，关联具体方法
CalculatorDelegate objCal = new CalculatorDelegate(Add);

//【4】通过委托去调用方法，而不是直接使用方法


// objCal -= Add;//断开委托对象所关联的方法
//  objCal += Sub;//重新指向一个新的方法（减法）

int result = objCal(10, 20);

Console.WriteLine("a-b=" + result);

Console.ReadLine();

//【2】根据委托定义一个“具体方法”实现加法功能
static int Add(int a, int b)
{
    return a + b;
}
//static int Sub(int a, int b)
//{
//    return a - b;
//}


//【1】声明委托（定义一个方法的原型：返回值  +  参数类型和个数）
public delegate int CalculatorDelegate(int a, int b);
