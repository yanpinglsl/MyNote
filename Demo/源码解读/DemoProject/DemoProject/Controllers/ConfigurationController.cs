using Microsoft.AspNetCore.Mvc;
using static DemoProject.Controllers.ConfigurationController;

namespace DemoProject.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        public ConfigurationController(IConfiguration configuration)
        {
            this._iConfiguration = configuration;
        }

        /// <summary>
        /// dotnet run --urls="http://*:5726"
        /// http://localhost:5726/Configuration/Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            string AllowedHosts = this._iConfiguration["AllowedHosts"];
            //string allowedHost = this._iConfiguration["AllowedHost"].ToString();//异常

            string today = this._iConfiguration["Today"];
            string writeConn = this._iConfiguration["ConnectionStrings:Write"];
            string readConn0 = this._iConfiguration["ConnectionStrings:Read:0"];
            string[] _SqlConnectionStringRead = this._iConfiguration.GetSection("ConnectionStrings").GetSection("Read").GetChildren().Select(s => s.Value).ToArray();

            Console.WriteLine($"AllowedHosts={AllowedHosts} today={today} writeConn={writeConn} readConn0={readConn0} _SqlConnectionStringRead={string.Join(",", _SqlConnectionStringRead)}");

            return View();
        }

        public IActionResult Bind()
        {
            //bind
            JsonOptions JsonOptions1 = new JsonOptions();
            this._iConfiguration.GetSection("JsonOptions").Bind(JsonOptions1);
            Console.WriteLine($"HostName={JsonOptions1.HostName}");

            //Get
            JsonOptions JsonOptions2 = this._iConfiguration.GetSection("JsonOptions").Get<JsonOptions>();
            Console.WriteLine($"HostName2={JsonOptions2.HostName}");

            return View();
        }

        public class JsonOptions
        {
            public string HostName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public List<string> ArrayTest { get; set; }
        }

        public class TestOptions
        {
            public string HostName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        ///  http://localhost:5726/Configuration/CommandLine
        /// </summary>
        /// <returns></returns>
        public IActionResult CommandLine()
        {
            /*
             var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddCommandLine(args)
            .Build();
             */
            string urls = this._iConfiguration["urls"];
            string ip = this._iConfiguration["ip"];
            string port = this._iConfiguration["port"];
            string writeConn = this._iConfiguration["ConnectionStrings:Write"];

            Console.WriteLine($"urls={urls} ip={ip} port={port} writeConn={writeConn} ");

            return View();
        }

        public IActionResult JSON()
        {
            Console.WriteLine("=====================JSON=========================");
            string TodayJson = this._iConfiguration["TodayJson"];
            string HostName = this._iConfiguration["JsonOptions:HostName"];
            string UserName = this._iConfiguration.GetValue<string>("JsonOptions:UserName");
            string array = this._iConfiguration["JsonOptions:ArrayTest:0"];
            Console.WriteLine($"TodayJson={TodayJson}");
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"UserName={UserName}");
            Console.WriteLine($"Array[0]={array}");

            JsonOptions testOption = new JsonOptions();
            this._iConfiguration.GetSection("JsonOptions").Bind(testOption);
            Console.WriteLine($"HostName={testOption.HostName}");
            Console.WriteLine($"Array[1]={testOption.ArrayTest[1]}");

            JsonOptions testOption2 = this._iConfiguration.GetSection("JsonOptions").Get<JsonOptions>();
            Console.WriteLine($"HostName2={testOption2.HostName}");
            Console.WriteLine($"Array[2]={testOption.ArrayTest[2]}");

            return View();
        }

        public IActionResult XML()
        {
            Console.WriteLine("=====================XML=========================");
            string TodayXML = this._iConfiguration["TodayXML"];
            string HostName = this._iConfiguration["XMLOptions:HostName"];
            string UserName = this._iConfiguration.GetValue<string>("XMLOptions:UserName");
            Console.WriteLine($"TodayXML={TodayXML}");
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"UserName={UserName}");

            TestOptions testOption = new TestOptions();
            this._iConfiguration.GetSection("XMLOptions").Bind(testOption);
            Console.WriteLine($"HostName={testOption.HostName}");

            TestOptions testOption2 = this._iConfiguration.GetSection("XMLOptions").Get<TestOptions>();
            Console.WriteLine($"HostName2={testOption2.HostName}");

            return View();
        }

        public IActionResult INI()
        {
            Console.WriteLine("=====================INI=========================");
            string TodayXML = this._iConfiguration["TodayINI:IniName"];
            string HostName = this._iConfiguration["INIOptions:HostName"];
            string UserName = this._iConfiguration.GetValue<string>("INIOptions:UserName");
            Console.WriteLine($"TodayINI={TodayXML}");
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"UserName={UserName}");

            TestOptions testOption = new TestOptions();
            this._iConfiguration.GetSection("INIOptions").Bind(testOption);
            Console.WriteLine($"HostName={testOption.HostName}");

            TestOptions testOption2 = this._iConfiguration.GetSection("INIOptions").Get<TestOptions>();
            Console.WriteLine($"HostName2={testOption2.HostName}");

            return View();
        }

        public IActionResult Memory()
        {
            Console.WriteLine("=====================Memory=========================");
            string HostName = this._iConfiguration["MemoryOptions:HostName"];
            string UserName = this._iConfiguration.GetValue<string>("MemoryOptions:UserName");
            string TodayMemory = this._iConfiguration["TodayMemory"];
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"TodayMemory={TodayMemory}");
            Console.WriteLine($"UserName={UserName}");

            TestOptions testOption = new TestOptions();
            this._iConfiguration.GetSection("MemoryOptions").Bind(testOption);
            Console.WriteLine($"HostName={testOption.HostName}");

            TestOptions testOption2 = this._iConfiguration.GetSection("MemoryOptions").Get<TestOptions>();
            Console.WriteLine($"HostName2={testOption2.HostName}");

            return View();
        }
        public IActionResult Custom()
        {
            Console.WriteLine("=====================Memory=========================");
            string HostName = this._iConfiguration["CustomOptions:HostName"];
            string UserName = this._iConfiguration.GetValue<string>("CustomOptions:UserName");
            string TodayMemory = this._iConfiguration["TodayCustom"];
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"TodayMemory={TodayMemory}");
            Console.WriteLine($"UserName={UserName}");

            TestOptions testOption = new TestOptions();
            this._iConfiguration.GetSection("CustomOptions").Bind(testOption);
            Console.WriteLine($"HostName={testOption.HostName}");

            TestOptions testOption2 = this._iConfiguration.GetSection("CustomOptions").Get<TestOptions>();
            Console.WriteLine($"HostName2={testOption2.HostName}");

            return View();
        }
    }
}
