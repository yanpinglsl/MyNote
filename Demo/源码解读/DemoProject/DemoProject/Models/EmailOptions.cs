namespace DemoProject.Models
{   
    /// <summary>
     /// 通常用Options结尾---要求有无参数构造函数
     /// </summary>
    public class EmailOptions
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string From { get; set; }
    }
}
