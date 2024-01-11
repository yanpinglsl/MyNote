using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

/// <summary>
/// 本地化资源管理类
/// </summary>
class LocalResourceManager
{
    /// <summary>
    /// 语言文件的路径
    /// 默认使用当前模块所在路径下的Language文件夹
    /// 也可以由外界传入，比如对于像Plugin\PecReport下的dll，就需要使用外界传入的路径
    /// </summary>
    private static string resourcesPath;
    public static string ResourcesPath
    {
        get
        {
            if (String.IsNullOrEmpty(resourcesPath))
                resourcesPath = Path.Combine(GetAssemblyPath(), "Language");
            return resourcesPath;
        }
        set { LocalResourceManager.resourcesPath = value; }
    }

    private static string GetAssemblyPath()
    {
        string executtempPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //如果执行程序存在语言文件目录，说明是Winform程序，否则是web上运行
        if (Directory.Exists(Path.Combine(executtempPath, "Language")))
            return executtempPath;
        string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
        codeBase = codeBase.Substring(8, codeBase.Length - 8);    // 8是file:/// 的长度        
        //如果程序运行的是插件目录，则对路径进行相应的调整
        if (codeBase.ToUpper().Contains("PLUGINS"))
            return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(codeBase)));
        return Path.GetDirectoryName(codeBase);
    }

    /// <summary>
    /// 当前语言文件的区域文化属性
    /// </summary>
    private string currentCultureName;
    public string CurrentCultureName
    {
        get { return currentCultureName; }
    }

    /// <summary>
    /// 资源管理唯一实例对像
    /// </summary>
    /// <returns>返回对象</returns>
    public static LocalResourceManager GetInstance()
    {
        if (resourceManagerInstance != null)
            return resourceManagerInstance;

        lock (lockHelper)
        {
            if (resourceManagerInstance == null)
            {
                resourceManagerInstance = new LocalResourceManager();
                resourceManagerInstance.InitialResourceManagerObject();
            }
        }
        return resourceManagerInstance;
    }

    /// <summary>
    /// 获取指定的资源名称，支持传入默认的名称
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="defaultText">默认的资源结果</param>
    /// <returns>获取到的词条</returns>
    public string GetString(string name, string defaultText)
    {
        try
        {
            if (resourceManager == null || currentCulture == null)
                return defaultText;

            string resultStr = resourceManager.GetString(name, currentCulture);
            if (String.IsNullOrEmpty(resultStr))
                return defaultText;
            return resultStr;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
        return defaultText;
    }

    /// <summary>
    /// 初始化创建资源关联对像实例
    /// </summary>
    /// <returns>是否成功</returns>
    private bool InitialResourceManagerObject()
    {
        string moduleName = Assembly.GetCallingAssembly().GetName().Name;
        string fileNameExpress = String.Format("{0}.*.resources", moduleName);
        string[] targetFileNames = Directory.GetFiles(ResourcesPath, fileNameExpress, SearchOption.TopDirectoryOnly);
        //只允许存在一个资源文件
        if (targetFileNames.Length != 1)
        {
            Debug.WriteLine(string.Format("more than one language file of module {0} exists", moduleName));
            return false;
        }
        string resourceFileName = Path.GetFileName(targetFileNames[0]);
        //resourceFileName
        //可以是PecReport.zh-cn.resources
        //也可以是Pec.Report.zh-cn.resources
        string[] brokenString = resourceFileName.Split(new char[] { '.' });
        if (brokenString.Length < 3)
        {
            Debug.WriteLine(string.Format("{0}'s language file's name format error, error file name is {1}", moduleName, resourceFileName));
            return false;
        }

        currentCultureName = brokenString[brokenString.Length - 2].Trim();
        currentCulture = new CultureInfo(currentCultureName);
        resourceManager = ResourceManager.CreateFileBasedResourceManager(moduleName, ResourcesPath, null);
        return true;
    }

    /// <summary>
    /// 类的唯一实例
    /// </summary>
    private static LocalResourceManager resourceManagerInstance = null;

    /// <summary>
    /// 单例锁
    /// </summary>
    private static object lockHelper = new object();

    /// <summary>
    /// 当前的区域文化
    /// </summary>
    private CultureInfo currentCulture;

    /// <summary>
    /// 资源管理类
    /// </summary>
    private ResourceManager resourceManager;
}