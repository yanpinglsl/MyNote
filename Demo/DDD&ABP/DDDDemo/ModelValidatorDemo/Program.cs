namespace ModelValidatorDemo
{
    internal class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // 模型验证
            //（1）实现IValidationEnabled接口
            Sample01.Sample.Run();


            // （2）构造函数依赖注入IObjectValidator，然后调用_objectValidator.ValidateAsync(input)实现模型验证
            //Sample02.Sample.Run();


            // （3）使用FluentVaildation实现模型验证
            //Sample03.Sample.Run();
        }
    }
}
