using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using CSharpDBPlugin;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;

namespace OfficeReportInterface
{
    /// <summary>
    /// 用于上传下载数据库的管理
    /// </summary>
    public class UpdateManager : Singleton<UpdateManager>
    {
        private DownloadFilesDat _mFilesDat;
        private DownloadFilesTemplates _mFilesTemplates;


        /// <summary>
        /// 设置OfficeReport的路径，例如输入"D:\CET\Excel&Word Reporter"。
        /// </summary>
        /// <param name="path"></param>
        public bool SetPath(string path)
        {
            try
            {
                AbstractDownloadFiles.CurrentDirectoryOfficeReport = path;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private List<AbstractDownloadFiles> m_fileList;

        private List<AbstractDownloadObject> m_objList;

        private UpdateTimeMessage m_updateTimeMessage;

        public UpdateManager()
        {
            _mFilesDat = new DownloadFilesDat();
            _mFilesTemplates = new DownloadFilesTemplates();

            m_fileList = new List<AbstractDownloadFiles>();
            m_fileList.Add(_mFilesTemplates);//模板文件
            m_fileList.Add(_mFilesDat);//OfficeReport.dat

            m_updateTimeMessage = new UpdateTimeMessage();

            m_objList = new List<AbstractDownloadObject>();
            m_objList.Add(m_updateTimeMessage);//更新时间
        }

        /// <summary>
        /// 是否数据库的文件更新
        /// </summary>
        /// <returns></returns>
        public bool IsDatabaseNewer(out bool isEqual)
        {
            isEqual = false;
            if (!m_updateTimeMessage.ReadFromDatabase())
            {
                DbgTrace.dout("{0}", "m_updateTimeMessage.ReadFromDatabase() failed. Have you ever writed to databse ? ");
                return false;
            }
            DbgTrace.dout("{0}", "m_updateTimeMessage.ReadFromDatabase() succeed.");
            DateTime lastWriteTime;
            if (IsAllFilesExist())
            {
                if (_mFilesDat.GetLastWriteTime(out lastWriteTime))
                {
                    if (m_updateTimeMessage.IsLocalNew(lastWriteTime, out isEqual))
                    {
                        DbgTrace.dout("{0}", "Local files are new");
                        return false;
                    }
                }
            }

            DbgTrace.dout("{0}", " I will download.");
            return true;
        }

        /// <summary>
        /// 从数据库获取更新时间
        /// </summary>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public bool GetUpdateTimeFromDB(out DateTime updateTime)
        {
            updateTime = DateTime.Now;
            if (!m_updateTimeMessage.ReadFromDatabase())
            {
                DbgTrace.dout("{0}", "m_updateTimeMessage.ReadFromDatabase() failed. Have you ever writed to databse ? ");
                return false;
            }
            updateTime = m_updateTimeMessage.UpdateTime;
            return true;
        }

        private bool DoDownload()
        {
            //设置文件属性，去掉只读，隐藏属性
            if (!SetNormal())
            {
                DbgTrace.dout("{0}", "SetNormal() failed.");
                return false;
            }


            foreach (var item in m_fileList)
            {
                if (!item.ReadFromDatabase())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 是否每次必须下载
        /// </summary>
        public bool mustDownloadEveryTime = false;

        /// <summary>
        /// 用于记载下载的OfficeReport.dat文件对应的数据库连接信息
        /// </summary>
        private static string dbconnInfoFileName = "dbconnInfo.json";

        /// <summary>
        /// 当前运行程序的数据库连接信息
        /// </summary>
        private static string dbconnInfoString = string.Empty;

        /// <summary>
        /// 设置当前使用的数据库连接信息，保存到成员变量dbconnInfoString 
        /// </summary>
        public static void SetDbconnInfoString(string dbconnInfoStr)
        {
            dbconnInfoString = dbconnInfoStr;
        }

        /// <summary>
        /// 获取保存了数据库连接信息的文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetDbconnInfoPath()
        {
            return Path.Combine(DbgTrace.GetAssemblyPath(), dbconnInfoFileName);
        }

        /// <summary>
        /// 判断是否需要下载
        /// </summary>
        /// <returns></returns>
        public bool NeedDownLoad()
        {
            if (mustDownloadEveryTime)
                return true;
            string dbconnInfoPath = GetDbconnInfoPath();
            if (!File.Exists(dbconnInfoPath))
                return true;
            //读取该文件中的数据库连接信息，如果不一致，则需要下载
            try
            {
                string dbString = File.ReadAllText(dbconnInfoPath, Encoding.UTF8);
                if (!string.Equals(dbString, dbconnInfoString)) //切换了数据库连接就必须重新下载文件了
                    return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return true;
            }

            //如果OfficeReport.dat文件正在被使用，则不做下载。实际上是先判断一下后面这个文件的备份等操作会不会抛出异常。因为先上传下载的是TemplateFiles，这个做完了再做dat的，如果dat的失败了，数据库上的dat与TemplateFiles文件就不匹配了。
            if (FileAttributeNormal.IsFileInUse(_mFilesDat.GetAbsoluteFilePath()))
                return false;
            //如果本地的更新，就不需要下载
            bool isEqual;
            bool isDatabaseNewer = IsDatabaseNewer(out isEqual);
            if ((!isEqual) && (isDatabaseNewer))
                return true;
            return false;
        }


        private bool RenameZipFiles()
        {
            return RenameBackZipFiles() && RenameDownloadZipFiles();
        }

        private bool RenameDownloadZipFiles()
        {
            foreach (var item in m_fileList)
            {
                if (!item.RenameDownloadZipFile())
                    return false;
            }
            return true;
        }

        private bool RenameBackZipFiles()
        {
            foreach (var item in m_fileList)
            {
                if (!item.RenameBackZipFile())
                    continue;
            }
            return true;
        }

        private bool UpdateFiles()
        {
            try
            {
                bool succ = true;
                if (!DoDownload())
                {
                    DbgTrace.dout("{0}", "Download from database failed.");
                    succ = false;
                }
                else
                {
                    DbgTrace.dout("{0}", "Download from database succeed.");
                }
                RenameZip();
                try
                {
                    if (!mustDownloadEveryTime) //桌面版程序才需要这样写标识。web版程序不会上传，所以不需要，而且web的bin目录下是不允许加文件的
                    {
                        string dbconnInfoPath = GetDbconnInfoPath();
                        File.WriteAllText(dbconnInfoPath, dbconnInfoString, Encoding.UTF8); //将数据库连接信息写入文件
                    }
                }
                catch (Exception ex)
                {
                    DbgTrace.dout(ex.Message + ex.StackTrace);
                }

                return succ;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        /// <returns></returns>
        public bool ReadFromDatabase()
        {
            try
            {
                //if (!NeedDownLoad())
                //{
                //    DbgTrace.dout("{0}", "There is no need to download from database.");
                //    return false;
                //}
                //else
                //{
                //    DbgTrace.dout("{0}", "Need to download from database.");
                //}

                return UpdateFiles();
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.StackTrace + ":" + ex.Message);
                return false;
            }
        }

        private bool DeleteAllFiles()
        {
            foreach (var item in m_fileList)
            {
                //就算TemplateFiles文件夹不存在，删除失败，也不影响后面的下载。所以这里不对返回值的true or false 做处理
                FileDeleting.DeleteFolderOrFile(item.GetAbsoluteFilePath());
            }
            return true;
        }

        /// <summary>
        /// 是否所有文件都存在
        /// </summary>
        /// <returns></returns>
        private bool IsAllFilesExist()
        {
            foreach (var item in m_fileList)
            {
                if (!item.IsFileExist())
                {
                    DbgTrace.dout("{0}", "Not all files exists,you can check that if both OfficeReport.dat file and TemplateFiles folder exist .");
                    return false;
                }
            }
            return true;
        }


        private bool SetNormal()
        {
            foreach (var item in m_fileList)
            {
                if (!item.IsFileExist())
                    continue;
                if (!FileAttributeNormal.SetFileNormal(item.GetAbsoluteFilePath()))
                    return false;
            }

            return true;
        }



        /// <summary>
        /// 判断是否需要上传到数据库
        /// </summary>
        /// <returns></returns>
        public bool NeedWriteToDatabase()
        {
            try
            {
                string dbconnInfoPath = GetDbconnInfoPath();
                if (!File.Exists(dbconnInfoPath)) //如果不能准确判断数据库连接信息，则不上传数据库
                    return false;
                string dbString = File.ReadAllText(dbconnInfoPath, Encoding.UTF8);
                if (!string.Equals(dbString, dbconnInfoString)) //本地的文件不是该数据库对应的文件，就不上传了
                    return false;

                //如果OfficeReport.dat文件正在被使用，则不做下载。实际上是先判断一下后面这个文件的备份等操作会不会抛出异常。因为先上传下载的是TemplateFiles，这个做完了再做dat的，如果dat的失败了，数据库上的dat与TemplateFiles文件就不匹配了。
                if (FileAttributeNormal.IsFileInUse(_mFilesDat.GetAbsoluteFilePath()))
                    return false;
                if (!IsAllFilesExist())
                {
                    DbgTrace.dout("{0}", "Not all files exists,so I will not write to database.");
                    return false;
                }
                if (!SetNormal())
                {
                    DbgTrace.dout("{0}", "Some files are not writable,so I will not write to database.");
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 执行上传到数据库的操作，只做上传的操作，不做判断。
        /// </summary>
        /// <returns></returns>
        public bool DoWriteToDatabase()
        {
            try
            {
                var succ = WriteToDatabaseDo();
                RenameZip();
                return succ;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 上传到数据库。首先判断是否需要上传，如果需要上传，则上传。做两件事：1.判断是否上传；2.执行上传到数据库
        /// </summary>
        /// <returns></returns>
        public bool WriteToDatabase()
        {
            try
            {
                //if (!NeedWriteToDatabase())
                //    return false;
                //bool isEqual;
                //bool isDatabaseNewer = IsDatabaseNewer(out isEqual);
                //if (isDatabaseNewer)
                //{
                //    DbgTrace.dout("{0}", "There is no need to write to database.Database has newer one.isEqual="+isEqual);
                //    return false;
                //}
                //else
                //{
                //    DbgTrace.dout("{0}", "Need to write to database.");
                //}
                return DoWriteToDatabase();
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 执行上传数据库
        /// </summary>
        /// <returns></returns>
        private bool WriteToDatabaseDo()
        {
            bool succ = true;
            foreach (var item in m_fileList)
            {
                if (!item.WriteToDatabase())
                {
                    //WriteTimeToDatabase(DateTime.MinValue);
                    DbgTrace.dout("{0}", "Write to database failed.");
                    succ = false;
                    break;
                }
            }
            if (succ)
            {
                WriteOfficeReportDatTimeToDatabase();
            }
            return succ;
        }

        /// <summary>
        /// 将OfficeReport.dat文件的最后修改时间写到数据库
        /// </summary>
        private void WriteOfficeReportDatTimeToDatabase()
        {
            WriteTimeToDatabase(DateTime.Now);
            //DownloadFilesDat dat = new DownloadFilesDat();
            //DateTime tempTime;
            //if (!dat.GetLastWriteTime(out tempTime))
            //{
            //    WriteTimeToDatabase(DateTime.MinValue);
            //    DbgTrace.dout("{0}", "Failed to get officeReport.dat 's last modified time.");
            //}
            //else
            //{
            //    WriteTimeToDatabase(tempTime);
            //}
        }

        private bool WriteTimeToDatabase(DateTime time)
        {
            //设置上传时间
            m_updateTimeMessage.UpdateTime = time;
            if (!m_updateTimeMessage.WriteToDatabase())
            {
                DbgTrace.dout("{0}", "Failed to write  time to database.");
                return false;
            }
            DbgTrace.dout("{0}", "Succeed to write  time to database.");
            return true;
        }

        private void RenameZip()
        {
            //将压缩文件重命名
            if (!RenameZipFiles())
            {
                DbgTrace.dout("{0}", "Failed to rename zip files.");
            }
            else
            {
                DbgTrace.dout("{0}", "Succeed to rename zip files.");
            }
        }
    }

    #region 下载内容相关结构定义

    /// <summary>
    /// 用于将时间转换为字符串
    /// </summary>
    public class DateTimeToStringManager
    {
        private const string FormatString = "yyyy-MM-dd HH:mm:ss.fff";
        private const string CultureInfoString = "en-US";
        private string m_errorString = string.Empty;

        /// <summary>
        /// 获取上次的错误字符串
        /// </summary>
        /// <returns></returns>
        public string GetLastErrorString()
        {
            return m_errorString;
        }

        /// <summary>
        /// 传入字符串，得到对应的DateTime
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public bool GetDateTimeFromString(string dateTimeString, out DateTime dateValue)
        {
            dateValue = new DateTime();
            try
            {
                if (!DateTime.TryParseExact(dateTimeString, FormatString, new CultureInfo(CultureInfoString),
                    DateTimeStyles.None, out dateValue))
                    return false;
                return true;
            }
            catch (System.Exception ex)
            {
                m_errorString = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// 获取DateTime对应的字符串
        /// </summary>
        /// <param name="time"></param>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
        public bool GetStringFromDateTime(DateTime time, out string dateTimeString)
        {
            dateTimeString = string.Empty;
            try
            {
                dateTimeString = time.ToString(FormatString, new CultureInfo(CultureInfoString));
                return true;
            }
            catch (System.Exception ex)
            {
                m_errorString = ex.Message;
                return false;
            }
        }
    }

    /// <summary>
    /// 用于下载TemplateFiles文件到本地，或者从本地上传文件
    /// </summary>
    public class DownloadManager
    {

    }

    public class UpdateTimeMessage : AbstractDownloadObject
    {
        public DateTime UpdateTime { get; set; }

        public UpdateTimeMessage()
        {

        }

        /// <summary>
        /// 判断是否本地的文件比较新
        /// </summary>
        /// <param name="localDatUpdateTime"></param>
        /// <returns></returns>
        public bool IsLocalNew(DateTime localDatUpdateTime, out bool isEqual)
        {
            if (localDatUpdateTime.Year == UpdateTime.Year && localDatUpdateTime.Month == UpdateTime.Month
                && localDatUpdateTime.Day == UpdateTime.Day && localDatUpdateTime.Hour == UpdateTime.Hour
                && localDatUpdateTime.Minute == UpdateTime.Minute && localDatUpdateTime.Second == UpdateTime.Second
                && localDatUpdateTime.Millisecond == UpdateTime.Millisecond
                )
            {
                isEqual = true;
            }
            else
            {
                isEqual = false;
            }

            if (localDatUpdateTime >= UpdateTime)
            {
                DbgTrace.dout("{0}", "localDatUpdateTime >= UpdateTime");
                return true;
            }

            return false;
        }

        protected override bool WriteToStream(BinaryWriter bw)
        {
            try
            {
                DateTimeToStringManager node = new DateTimeToStringManager();
                string tempTime;
                if (!node.GetStringFromDateTime(UpdateTime, out tempTime))
                    return false;
                bw.Write(tempTime);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }

        protected override ModuleFileTypes GetModuleType()
        {
            return ModuleFileTypes.UpdateTime;
        }

        protected override bool ReadFromStream(BinaryReader br)
        {
            try
            {
                string tempStr = br.ReadString();
                DateTime dateValue;
                var node = new DateTimeToStringManager();
                if (!node.GetDateTimeFromString(tempStr, out dateValue))
                    return false;
                UpdateTime = dateValue;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }
    }

    internal class MemoryStreamToBytes
    {
        public static byte[] GetBytesByMemoryStream(MemoryStream profileData)
        {
            var dataToSave = new byte[profileData.Length];
            profileData.Seek(0, SeekOrigin.Begin);
            profileData.Read(dataToSave, 0, dataToSave.Length);
            return dataToSave;
        }
    }

    internal class MacMessage : AbstractDownloadObject
    {
        /// <summary>
        /// Mac地址字符串
        /// </summary>
        private string m_macAddress = string.Empty;

        public string MacAddress
        {
            get { return m_macAddress; }
            private set { m_macAddress = value; }
        }

        public MacMessage()
        {
            MacAddress = string.Empty;

        }

        protected override ModuleFileTypes GetModuleType()
        {
            return ModuleFileTypes.MacAddress;
        }

        protected override bool ReadFromStream(BinaryReader br)
        {
            try
            {
                MacAddress = br.ReadString();
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "MacMessage.ReadFromStream(BinaryReader br) catch (System.Exception ex): ", ex.Message);
                return false;
            }
        }

        protected override bool WriteToStream(BinaryWriter bw)
        {
            try
            {
                string macAddress = MacManager.GetMacAddressByDos();
                bw.Write(macAddress);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "MacMessage. WriteToStream(BinaryWriter bw) catch (System.Exception ex): ", ex.Message);
                return false;
            }
        }
    }

    internal class MacManager
    {
        /// <summary>    
        /// 通过DOS命令获得MAC地址    
        /// </summary>    
        /// <returns></returns>    
        public static string GetMacAddressByDos()
        {
            string macAddress = "";
            Process p = null;
            StreamReader reader = null;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo("cmd.exe");

                start.FileName = "ipconfig";
                start.Arguments = "/all";

                start.CreateNoWindow = true;

                start.RedirectStandardOutput = true;

                start.RedirectStandardInput = true;

                start.UseShellExecute = false;

                p = Process.Start(start);

                reader = p.StandardOutput;

                string line = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    if (line.ToLower().IndexOf("physical address") > 0 || line.ToLower().IndexOf("物理地址") > 0)
                    {
                        int index = line.IndexOf(":");
                        index += 2;
                        macAddress = line.Substring(index);
                        macAddress = macAddress.Replace('-', ':');
                        break;
                    }
                    line = reader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout("{0}{1}", "class MacManager.public static string GetMacAddressByDos()  catch(Exception ex)：", ex.Message);
            }
            finally
            {
                if (p != null)
                {
                    p.WaitForExit();
                    p.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return macAddress;
        }
    }

    internal interface IUpdate
    {
        bool WriteToDatabase();
        bool ReadFromDatabase();
    }

    internal class DownloadFilesTemplates : AbstractDownloadFiles
    {
        protected override ModuleFileTypes GetModuleType()
        {
            return ModuleFileTypes.AllTemplates;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        protected override string GetFileName()
        {
            return "TemplateFiles";
        }

        /// <summary>
        /// 压缩文件名称
        /// </summary>
        /// <returns></returns>
        protected override string GetFileZipName()
        {
            return "TemplateFilesCompress.zip";
        }

        private string GetDefaultTemplateName()
        {
            return "DefaultTemplate";
        }

        private string GetDefaultTemplateZipName()
        {
            return "DefaultTemplateZip.zip";
        }

        public string GetDefaultTemplatePath()
        {
            return Path.Combine(GetAbsoluteFilePath(), GetDefaultTemplateName());
        }



        private string GetCGIDefaultTemplate(string webservicePath)
        {
            return Path.Combine(webservicePath, "CGI", GetDefaultTemplateName());
        }

        protected override bool DownloadFromDatabase(out DataTable resultDT)
        {
            //从数据库读取到byte[]
            if (!DatabaseWriter.Instance.ReadFromDB((uint)ModuleFileTypes.AllTemplates, out resultDT))
                return false;
            return true;
        }

        protected override string GetDeCompressedFolder()
        {
            return Path.Combine(CurrentDirectoryOfficeReport, GetFileName());
        }

        public override bool IsFileExist()
        {
            if (!Directory.Exists(GetAbsoluteFilePath()))
                return false;
            return true;
        }


        ///// <summary>
        ///// 拷贝DefaultTemplate文件夹到Webservice的asmx文件所在路径下的CGI文件夹中
        ///// </summary>
        ///// <param name="path">Webservice的当前路径</param>
        ///// <returns></returns>
        //public bool CopyDefaultTemplateToCgi(string webservicePath)
        //{
        //    string cgiDefaultTemplatePath = GetCGIDefaultTemplate(webservicePath);
        //    FileAttributeNormal.SetFileNormal(cgiDefaultTemplatePath);
        //    //首先将CGI目录下的DefaultTemplate备份删除
        //    if (!CompressManager.BackAFileOrFolder(GetBackFolder(), cgiDefaultTemplatePath,
        //        GetDefaultTemplateZipName(),
        //        false))
        //        return false;
        //    //将CGI目录下的DefaultTemplate文件夹删除
        //    FileDeleting.DeleteFolderOrFile(cgiDefaultTemplatePath);
        //    //将D:\CET\Excel&Word Reporter\TemplateFiles\DefaultTemplate压缩到download目录下,不删除
        //    if (!CompressManager.BackAFileOrFolder(GetDestinationFolderToDownLoad(), GetDefaultTemplatePath(),
        //        GetDefaultTemplateZipName(),
        //        false))
        //        return false;
        //    //将download目录下的压缩文件解压到CGI目录下的DefaultTemplate
        //    string sourceFile = Path.Combine(GetDestinationFolderToDownLoad(), GetDefaultTemplateZipName());
        //    if (!CompressManager.DeCompressFileOrFolder(sourceFile, cgiDefaultTemplatePath))
        //    {
        //        CompressManager.DeCompressFileOrFolder(GetDefaultTemplateZipName(), cgiDefaultTemplatePath);
        //        return false;
        //    }

        //    RenameFile(GetDefaultTemplateZipName());
        //    //删除这份没必要保留的压缩文件,因为它的非压缩版本存在
        //    //  FileDeleting.DeleteFolderOrFile(sourceFile);
        //    RenameFile(sourceFile);
        //    return true;
        //}
    }

    internal class DownloadFilesDat : AbstractDownloadFiles
    {
        protected override ModuleFileTypes GetModuleType()
        {
            return ModuleFileTypes.OfficeReportDat;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        protected override string GetFileName()
        {
            return "OfficeReport.dat";
        }

        /// <summary>
        /// 压缩文件名称,例如"OfficeReportCompress.zip";
        /// </summary>
        /// <returns></returns>
        protected override string GetFileZipName()
        {
            return "OfficeReportCompress.zip";
        }

        /// <summary>
        /// 从数据库下载OfficeReport.dat
        /// </summary>
        /// <returns></returns>
        protected override bool DownloadFromDatabase(out DataTable resultDT)
        {
            //从数据库读取到byte[]
            if (!DatabaseWriter.Instance.ReadFromDB((uint)ModuleFileTypes.OfficeReportDat, out resultDT))
                return false;
            return true;
        }

        protected override string GetDeCompressedFolder()
        {
            return CurrentDirectoryOfficeReport;
        }

        public override bool IsFileExist()
        {
            if (!File.Exists(GetAbsoluteFilePath()))
                return false;
            return true;
        }




        /// <summary>
        /// 获取dat文件最后一次被修改的时间
        /// </summary>
        /// <param name="lastWriteTime"></param>
        /// <returns></returns>
        public bool GetLastWriteTime(out DateTime lastWriteTime)
        {
            lastWriteTime = new DateTime(1, 1, 1);
            try
            {
                if (!IsFileExist())
                {
                    DbgTrace.dout("{0}", "Can not get last time becuse OfficeReport.dat file is not exists.");
                    return false;
                }

                FileInfo f = new FileInfo(GetAbsoluteFilePath());
                lastWriteTime = f.LastWriteTime;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}",
                    "DownloadFilesDat.GetLastWriteTime(out DateTime lastWriteTime) catch (System.Exception ex):",
                    ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// 上传下载对象
    /// </summary>
    public abstract class AbstractDownloadObject : IUpdate
    {
        protected abstract ModuleFileTypes GetModuleType();

        public bool WriteToDatabase()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    try
                    {
                        if (!WriteToStream(bw))
                            return false;
                        byte[] timeMessage = MemoryStreamToBytes.GetBytesByMemoryStream(ms);
                        return DatabaseWriter.Instance.SaveProfileDataToDB((uint)GetModuleType(), timeMessage);
                    }
                    catch (System.Exception ex)
                    {
                        DbgTrace.dout("{0}{1}{2}", GetModuleType().ToString(), " WriteToDatabase() catch (System.Exception ex)：", ex.Message);
                        return false;
                    }
                }
            }
        }

        public bool ReadFromDatabase()
        {
            try
            {
                DataTable resultDT;
                if (!DatabaseWriter.Instance.ReadFromDB((uint)GetModuleType(), out resultDT))
                    return false;
                byte[] tmpByte;
                if (!AbstractDownloadFiles.ReadBytesFromResultDT(resultDT, out tmpByte))
                    return false;
                using (MemoryStream ms = new MemoryStream(tmpByte))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        if (!ReadFromStream(br))
                            return false;
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}{2}", GetModuleType().ToString(), " ReadFromDatabase() catch (System.Exception ex)：", ex.Message + ex.StackTrace);
                return false;
            }
        }

        protected abstract bool ReadFromStream(BinaryReader br);
        protected abstract bool WriteToStream(BinaryWriter bw);
    }

    /// <summary>
    /// 上传下载文件
    /// </summary>
    public abstract class AbstractDownloadFiles : IUpdate
    {
        /// <summary>
        /// OfficeReport的当前路径，获取路径的方式，OfficeReport，webService不一样。
        /// </summary>
        public static string CurrentDirectoryOfficeReport { get; set; }

        protected abstract ModuleFileTypes GetModuleType();

        protected AbstractDownloadFiles()
        {
            CurrentDirectoryOfficeReport = DbgTrace.GetAssemblyPath();
        }

        protected string GetBackFolder()
        {
            return Path.Combine(CurrentDirectoryOfficeReport, OfficeReportBackFolder);
        }

        /// <summary>
        /// 在进行删除前，将备份的文件放到这个文件夹，以备后面要是从数据库下载或下载后解析失败了，还可以用这个文件还原。这里放的是压缩文件
        /// </summary>
        protected const string OfficeReportBackFolder = "OfficeReportBack";

        /// <summary>
        /// 从数据库下载，临时先放到这个目录下。这里放的是压缩文件
        /// </summary>
        protected const string DownloadFolder = "DownloadFolder";

        /// <summary>
        /// 文件名称,例如"OfficeReport.dat";
        /// </summary>
        protected abstract string GetFileName();

        /// <summary>
        /// 压缩文件名称,例如"OfficeReportCompress.zip";
        /// </summary>
        /// <returns></returns>
        protected abstract string GetFileZipName();

        /// <summary>
        /// 备份文件的名称，包含路径
        /// </summary>
        /// <returns></returns>
        private string GetBackZipFileName()
        {
            return Path.Combine(CurrentDirectoryOfficeReport, OfficeReportBackFolder, GetFileZipName());
        }

        /// <summary>
        /// 重命名备份文件
        /// </summary>
        /// <returns></returns>
        public bool RenameBackZipFile()
        {
            return RenameBackZipFile(GetBackZipFileName());
        }

        private bool RenameBackZipFile(string fileName)
        {
            try
            {
                if (!RenameFile(fileName))
                    return false;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "class AbstractDownloadFiles.RenameBackZipFile() catch (System.Exception ex)", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 重命名下载的压缩文件
        /// </summary>
        /// <returns></returns>
        public bool RenameDownloadZipFile()
        {
            try
            {
                string fileName = GetDownloadFileZipName();
                if (!RenameFile(fileName))
                    return false;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "class AbstractDownloadFiles.RenameDownloadZipFile() catch (System.Exception ex)", ex.Message);
                return false;
            }
        }

        protected bool RenameFile(string fileName)
        {
            try
            {
                FileInfo fi = new FileInfo(fileName);
                if (!fi.Exists)
                    return true;
                DateTime date = DateTime.Now;
                string tempName = fileName.Substring(0, fileName.Length - 4);
                string newFileName = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", tempName, date.Year, date.Month, date.Day, date.Hour,
                    date.Minute, date.Second, date.Millisecond, ".zip");
                fi.MoveTo(newFileName);
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.StackTrace + ":" + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 获取文件绝对路径,例如CurrentDirectoryOfficeReport+OfficeReport.dat
        /// </summary>
        /// <returns></returns>
        public string GetAbsoluteFilePath()
        {
            return Path.Combine(CurrentDirectoryOfficeReport, GetFileName());
        }

        /// <summary>
        /// 得到用于保存从数据库下载的文件的文件夹
        /// </summary>
        /// <returns></returns>
        protected string GetDestinationFolderToDownLoad()
        {
            return Path.Combine(CurrentDirectoryOfficeReport, DownloadFolder);
        }

        /// <summary>
        /// 从数据库下载到out resultDT
        /// </summary>
        /// <returns></returns>
        protected abstract bool DownloadFromDatabase(out DataTable resultDT);

        public static bool ReadBytesFromResultDT(DataTable resultDT, out byte[] tmpByte)
        {
            tmpByte = new byte[0];
            //如果没有得到数据，则返回false
            if (resultDT.Rows.Count == 0)
                return false;

            foreach (DataRow row in resultDT.Rows)
                tmpByte = (byte[])(row["DATA"]);
            return true;
        }

        /// <summary>
        /// 获取下载的zip文件的名称，是包含路径的名称
        /// </summary>
        /// <returns></returns>
        private string GetDownloadFileZipName()
        {
            return Path.Combine(GetDestinationFolderToDownLoad(), GetFileZipName());
        }

        /// <summary>
        /// 将Byte[]转换成压缩文件
        /// </summary>
        /// <param name="tmpByte"></param>
        /// <returns></returns>
        private bool ReadBytesToZipFile(byte[] tmpByte)
        {
            try
            {
                string folder = GetDestinationFolderToDownLoad();
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                DbgTrace.dout(folder + " exists? " + Directory.Exists(folder));
                //将byte[]解析成OfficeReport.dat文件，放到destinationFolder中
                if (!BytesTranslator.writeFile(tmpByte, GetDownloadFileZipName()))
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 将从数据库下载的压缩文件解压到目标目录,获取这个目标的文件夹，绝对路径
        /// </summary>
        /// <returns></returns>
        protected abstract string GetDeCompressedFolder();

        public abstract bool IsFileExist();

        /// <summary>
        /// 将从数据库下载的压缩文件解压到目标目录
        /// </summary>
        /// <returns></returns>
        private bool DepressedToDestination()
        {
            DbgTrace.dout("下载的临时文件路径: " + GetDownloadFileZipName() + "   .  备份路径: " + GetBackZipFileName() + "  GetBackFolder()=" + GetBackFolder() + "   GetAbsoluteFilePath()=" + GetAbsoluteFilePath() + "  GetFileZipName()=" + GetFileZipName());

            if (IsFileExist())
            {
                if (!CompressManager.BackAFileOrFolder(GetBackFolder(), GetAbsoluteFilePath(), GetFileZipName(), true)) // 覆盖之前先备份一下
                    return false;
            }

            if (!CompressManager.DeCompressFileOrFolder(GetDownloadFileZipName(), GetDeCompressedFolder()))
            {
                CompressManager.DeCompressFileOrFolder(GetBackZipFileName(), CurrentDirectoryOfficeReport);
                return false;
            }
            FileDeleting.DeleteFolderOrFile(GetDownloadFileZipName());
            return true;
        }

        /// <summary>
        /// 从数据库下载的整个过程
        /// </summary>
        /// <returns></returns>
        public bool ReadFromDatabase()
        {
            DataTable resultDT;
            if (!DownloadFromDatabase(out resultDT))
                return false;
            byte[] tmpByte;
            if (!ReadBytesFromResultDT(resultDT, out tmpByte))
                return false;
            if (!ReadBytesToZipFile(tmpByte))
                return false;
            if (!DepressedToDestination())
                return false;
            return true;
        }

        public bool WriteToDatabase()
        {
            //将文件夹压缩成文件
            if (!CompressManager.BackAFileOrFolder(GetBackFolder(), GetAbsoluteFilePath(), GetFileZipName(), false))
                return false;
            //将文件转换成byte[]
            byte[] resultBytes;
            if (!BytesTranslator.ReadFile(GetBackZipFileName(), out resultBytes))
                return false;
            if (!DatabaseWriter.Instance.SaveProfileDataToDB((uint)GetModuleType(), resultBytes))
                return false;
            return true;
        }
    }


    #endregion

    #region 写数据库相关

    internal class DatabaseWriter
    {
        private static DatabaseWriter m_instance;

        public static DatabaseWriter Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new DatabaseWriter();
                return m_instance;
            }
        }

        /// <summary>
        /// 进程ID
        /// </summary>
        private const int OFFICE_REPORT_PRO_ID = 10;

        /// <summary>
        /// 计算机名称.实际上不能根据实际的计算机名称来写，因为多个计算机可以共用一个数据库
        /// </summary>
        private const string COMPUTER_NAME = "Office_Report_A918BB49-A229-4D22-AE58-80537426DBBD";

        /// <summary>
        /// 用户ID。这个要从PC_TB_05表来获取
        /// </summary>
        private readonly uint UserID;

        private DatabaseWriter()
        {
            UserID = GetUserID();
        }

        /// <summary>
        /// 获取当前的UserID。
        /// </summary>
        /// <returns></returns>
        private uint GetUserID()
        {
            //后面要补上
            return 1;
        }

        /// <summary>
        /// 使用PQSYS3.5中通用数据接口保存定制信息
        /// </summary>
        /// <param name="computerName">计算机名称</param>
        /// <param name="userID">用户ID</param>
        /// <param name="appType">应用程序类型</param>
        /// <param name="moduleType">定制信息类型</param>
        /// <param name="profileData">定制信息</param>
        /// <returns>返回0成功</returns>
        public bool SaveProfileDataToDB(uint moduleType, byte[] dataToSave)
        {
            int errorCode = UserProfileProvider.Instance.WriteUserProfile(0, COMPUTER_NAME, OFFICE_REPORT_PRO_ID,
                UserID, moduleType, dataToSave);

            if (errorCode != 0)
            {
                DbgTrace.dout("{0}", string.Format("Please check database connection. The error code is:{0} ", errorCode));
                return false;
            }
            return true;
        }

        public bool ReadFromDB(uint moduleId, out DataTable resultDT)
        {
            resultDT = new DataTable();
            int errorCode = UserProfileProvider.Instance.ReadUserProfiles(0, COMPUTER_NAME, OFFICE_REPORT_PRO_ID,
                UserID, moduleId, ref resultDT);
            if (errorCode != 0)
            {
                DbgTrace.dout("{0}", string.Format("Please check database connection. The error code is:{0} ", errorCode));
                return false;
            }
            return true;
        }

    }

    #endregion

    #region 文件操作相关

    internal class BytesTranslator
    {
        /// <summary>
        /// 读取文件到byte[]
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="resultBytes"></param>
        /// <returns></returns>
        public static bool ReadFile(string fileName, out byte[] pReadByte)
        {
            bool succeed = false;
            FileStream pFileStream = null;
            pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin); //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                succeed = true;
            }

            catch (Exception ex)
            {
                DbgTrace.dout("{0}{1}", "class BytesTranslator. public static bool ReadFile(string fileName, out byte[] pReadByte) catch(Exception ex):", ex.Message);
                succeed = false;
            }

            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            return succeed;
        }

        /// <summary>
        /// 写byte[]到fileName
        /// </summary>
        /// <param name="pReadByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool writeFile(byte[] pReadByte, string fileName)
        {
            FileStream pFileStream = null;
            bool succeed = false;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
                succeed = true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout("{0}{1}", "class BytesTranslator. public static bool writeFile(byte[] pReadByte, string fileName) catch(Exception ex):", ex.Message);
                succeed = false;
            }

            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            return succeed;
        }
    }

    /// <summary>    
    /// 文件(夹)压缩、解压缩    
    /// </summary>    
    public abstract class CompressManager
    {
        /// <summary>
        /// 压缩文件或者文件夹
        /// </summary>
        /// <param name="selectedFolder"></param>
        /// <param name="sourcePath"></param>
        /// <param name="compressedfileName"></param>
        /// <param name="deleteFile"></param>
        /// <returns></returns>
        public static bool BackAFileOrFolder(string selectedFolder, string sourcePath, string compressedfileName, bool deleteFile)
        {
            try
            {
                if (String.IsNullOrEmpty(selectedFolder))
                {
                    DbgTrace.dout(selectedFolder + " not exists.");
                    return false;
                }

                if (!Directory.Exists(selectedFolder))
                {
                    Directory.CreateDirectory(selectedFolder);
                }
                DbgTrace.dout(selectedFolder + " exists? " + Directory.Exists(selectedFolder));

                string compressedfileNameFull = Path.Combine(selectedFolder, compressedfileName);
                if (File.Exists(compressedfileNameFull))
                    FileDeleting.DeleteFolderOrFile(compressedfileNameFull);
                if (File.Exists(sourcePath))
                {
                    FileInfo node = new FileInfo(sourcePath);
                    List<FileInfo> fileNames = new List<FileInfo>();
                    fileNames.Add(node);
                    return CompressFile(fileNames, compressedfileNameFull, 1, deleteFile);
                }
                if (Directory.Exists(sourcePath))
                {
                    return CompressDirectory(sourcePath, compressedfileNameFull, 1, deleteFile);
                }
                return false;
            }
            catch (System.Exception ex)
            {
                DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "abstract class CompressManager.public static bool BackAFileOrFolder(string selectedFolder, string sourcePath, string compressedfileName,bool deleteFile) catch (System.Exception ex):{0}", ex.Message + ":" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 解压缩文件到指定目录下
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool DeCompressFileOrFolder(string sourceFile, string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFile))
                    return false;

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                bool succeed = Decompress(sourceFile, folderPath);
                return succeed;
            }
            catch (System.Exception ex)
            {
                DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "还原文件异常，错误信息为 {0}。", ex.Message);
                return false;
            }
        }

        #region 压缩文件

        /// <summary>      
        /// 压缩文件      
        /// </summary>      
        /// <param name="fileNames">要打包的文件列表</param>      
        /// <param name="GzipFileName">目标文件名</param>      
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>      
        /// <param name="deleteFile">是否删除原文件</param>    
        private static bool CompressFile(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, bool deleteFile)
        {
            try
            {
                DbgTrace.dout("CompressFile : GzipFileName= " + GzipFileName + " fileNames:");
                foreach (var item in fileNames)
                {
                    DbgTrace.dout(item.FullName);
                }
            }
            catch (Exception ex)
            {

            }

            bool succ = false;
            ZipOutputStream s = new ZipOutputStream(File.Create(GzipFileName));
            try
            {
                s.SetLevel(CompressionLevel); //0 - store only to 9 - means best compression      
                foreach (FileInfo file in fileNames)
                {
                    FileStream fs = null;
                    try
                    {
                        fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                    }
                    catch (System.Exception ex)
                    {
                        DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "压缩文件失败，错误信息为 {0}， 文件为 {1}", ex.ToString(), file.Name);
                        continue;
                    }

                    byte[] data = new byte[2048];
                    int size = 2048;
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file.Name));
                    entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                    s.PutNextEntry(entry);
                    while (true)
                    {
                        size = fs.Read(data, 0, size);
                        if (size <= 0) break;
                        s.Write(data, 0, size);
                    }
                    fs.Close();
                    if (deleteFile)
                    {
                        FileDeleting.DeleteFolderOrFile(file.FullName);

                    }
                }
                succ = true;
            }
            catch (Exception ex)
            {
                DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "abstract class CompressManager.private static bool CompressFile(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, bool deleteFile) catch (System.Exception ex):{0}", ex.Message);
                succ = false;
            }
            finally
            {
                s.Finish();
                s.Close();
            }

            return succ;
        }

        private static void GetAllDirectories(string rootPath, ref Dictionary<string, DateTime> pathsList, ref Dictionary<string, DateTime> filesList)
        {
            if (IsDefaultTemplatesDir(new DirectoryInfo(rootPath)))
                return;

            string[] subPaths = Directory.GetDirectories(rootPath); //得到所有子目录
            foreach (string path in subPaths)
            {
                GetAllDirectories(path, ref pathsList, ref filesList); //对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                var fileInfo = new FileInfo(file);
                filesList.Add(file, fileInfo.LastWriteTime); //将当前目录中的所有文件全名存入文件List
            }
            if (subPaths.Length == files.Length && files.Length == 0) //如果是空目录
            {
                var folderInfo = new DirectoryInfo(rootPath);
                pathsList.Add(rootPath, folderInfo.LastWriteTime); //记录空目录
            }
        }

        private static bool IsDefaultTemplatesDir(DirectoryInfo directoryInfo)
        {
            return IsDefaultTemplateClass.IsDefaultTemplatesDir(directoryInfo);
        }


        /// <summary>      
        /// 压缩文件夹      
        /// </summary>      
        /// <param name="rootPath">要打包的文件夹</param>      
        /// <param name="GzipFileName">目标文件名</param>      
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>      
        /// <param name="deleteDir">是否删除原文件夹</param>    
        private static bool CompressDirectory(string rootPath, string GzipFileName, int CompressionLevel, bool deleteDir)
        {
            if (Path.GetExtension(GzipFileName) != ".zip")
                GzipFileName = GzipFileName + ".zip";
            DbgTrace.dout("CompressDirectory : rootPath=" + rootPath + " GzipFileName= " + GzipFileName);
            try
            {
                ZipClass zipClass = new ZipClass();
                bool succ = zipClass.ZipFileFromDirectory(rootPath, GzipFileName, CompressionLevel);
                //    using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(GzipFileName)))
                //    {
                //        zipoutputstream.SetLevel(CompressionLevel);
                //        Crc32 crc = new Crc32();

                //        Dictionary<string, DateTime> pathsList = new Dictionary<string, DateTime>();
                //        Dictionary<string, DateTime> filesList = new Dictionary<string, DateTime>();
                //        GetAllDirectories(rootPath, ref pathsList, ref filesList);
                //        string rootMark = rootPath + "\\";//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。
                ////        Dictionary<string, DateTime> fileList = GetAllFies(dirPath);
                //        foreach (KeyValuePair<string, DateTime> item in filesList)
                //        {
                //            FileStream fs = File.OpenRead(item.Key.ToString());
                //            byte[] buffer = new byte[fs.Length];
                //            fs.Read(buffer, 0, buffer.Length);
                //            ZipEntry entry = new ZipEntry(item.Key.Replace(rootMark, string.Empty));
                //            entry.DateTime = item.Value;
                //            entry.Size = fs.Length;
                //            fs.Close();
                //            crc.Reset();
                //            crc.Update(buffer);
                //            entry.Crc = crc.Value;
                //            zipoutputstream.PutNextEntry(entry);
                //            zipoutputstream.Write(buffer, 0, buffer.Length);
                //        }

                //        //空文件夹
                //        //Dictionary<string, DateTime> emptyFolderList = GetEmptyFolders(dirPath);
                //        foreach (var emptyPath in pathsList)
                //        {
                //            ZipEntry entry = new ZipEntry(emptyPath.Key.Replace(rootMark, string.Empty) + "/");

                //            zipoutputstream.PutNextEntry(entry);
                //        }
                //        zipoutputstream.Finish();
                //        zipoutputstream.Close();
                //        GC.Collect();
                //    }
                if (deleteDir)
                {
                    FileDeleting.DeleteFolderOrFile(rootPath);
                }


                return succ;
            }
            catch (Exception ex)
            {
                DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "CompressDirectory(string dirPath, string GzipFileName, int CompressionLevel, bool deleteDir) catch (System.Exception ex): {0}。", ex.Message);
                return false;
            }
        }

        /// <summary>      
        /// 获取所有文件      
        /// </summary>      
        /// <returns></returns>      
        private static Dictionary<string, DateTime> GetAllFies(string dir)
        {
            Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
            DirectoryInfo fileDire = new DirectoryInfo(dir);
            if (!fileDire.Exists)
            {
                DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "目录 {0} 没有找到。", dir);
                return FilesList;
            }
            GetAllDirFiles(fileDire, FilesList);
            GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
            return FilesList;
        }

        private static Dictionary<string, DateTime> GetEmptyFolders(string dir)
        {
            Dictionary<string, DateTime> emptyFolderList = new Dictionary<string, DateTime>();
            DirectoryInfo fileDire = new DirectoryInfo(dir);
            if (!fileDire.Exists)
            {
                DbgTrace.doutc(DbgTrace.DbgTraceColor.RedOnWhite, "目录 {0} 没有找到。", dir);
                return emptyFolderList;
            }
            var nodes = fileDire.GetDirectories();
            if (nodes.GetLength(0) > 0)
            {
                GetAllEmptyFolders(nodes, emptyFolderList);
            }
            else
            {
                emptyFolderList.Add(fileDire.FullName, fileDire.LastWriteTime);
            }

            return emptyFolderList;
        }

        private static void GetAllEmptyFolders(DirectoryInfo[] dirs, Dictionary<string, DateTime> emptyFolderList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                if (IsDefaultTemplatesDir(dir)) continue;
                var nodes = dir.GetDirectories();
                if (nodes.GetLength(0) > 0)
                {
                    GetAllEmptyFolders(dir.GetDirectories(), emptyFolderList);
                }
                else
                {
                    emptyFolderList.Add(dir.FullName, dir.LastWriteTime);
                }
            }
        }

        /// <summary>      
        /// 获取一个文件夹下的所有文件夹里的文件      
        /// </summary>      
        /// <param name="dirs"></param>      
        /// <param name="filesList"></param>      
        private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                if (IsDefaultTemplatesDir(dir)) continue;

                GetAllDirFiles(dir, filesList);
                GetAllDirsFiles(dir.GetDirectories(), filesList);
            }
        }


        /// <summary>      
        /// 获取一个文件夹下的文件      
        /// </summary>      
        /// <param name="dir">目录名称</param>      
        /// <param name="filesList">文件列表HastTable</param>      
        private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
        }

        #endregion

        #region 解压缩文件

        /// <summary>      
        /// 解压缩文件      
        /// </summary>      
        /// <param name="GzipFile">压缩包文件名</param>      
        /// <param name="targetPath">解压缩目标路径</param>             
        private static bool Decompress(string GzipFile, string targetPath)
        {
            if (Path.GetExtension(GzipFile) != ".zip")
                return false;

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            ZipClass zipClass = new ZipClass();
            bool succ = zipClass.UnZip(GzipFile, targetPath);
            //-------------输出调试信息，方便分析问题------------------------------------------
            DbgTrace.dout("目标文件夹是" + targetPath);
            DirectoryInfo info = new DirectoryInfo(targetPath);
            StringBuilder str = new StringBuilder();
            foreach (FileInfo fileInfo in info.GetFiles())
            {
                str.Append(fileInfo.FullName + "   ");
            }
            DbgTrace.dout(str.ToString());
            //---------------------------------------------------------
            return succ;
        }

        #endregion
    }


    public class FileAttributeNormal
    {
        /// <summary>
        /// 判断文件是否正在被使用
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            //文件不存在则一定没有被使用
            if (!File.Exists(fileName))
                return false;


            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                inUse = true;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return inUse; //true表示正在使用,false没有使用
        }


        public static bool SetFileNormal(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return NormalFile(path);
                }
                if (Directory.Exists(path))
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    NormalFileByDirectory(info);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public static void SetFileAttributesNormal(FileInfo fi)
        {
            fi.Attributes = fi.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
        }

        public static void SetDirectoryNormal(DirectoryInfo info)
        {
            info.Attributes = info.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
        }

        private static bool NormalFile(string savedPath)
        {
            try
            {
                if (!File.Exists(savedPath))
                    return false;
                FileInfo fi = new FileInfo(savedPath);
                SetFileAttributesNormal(fi);

                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private static void NormalFileByDirectory(DirectoryInfo info)
        {
            try
            {
                if (!info.Exists)
                    return;
                foreach (DirectoryInfo newInfo in info.GetDirectories())
                {
                    NormalFileByDirectory(newInfo);
                }
                foreach (FileInfo newInfo in info.GetFiles())
                {

                    SetFileAttributesNormal(newInfo);
                }

                SetDirectoryNormal(info);
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }
    }

    public class FileDeleting
    {
        public static bool DeleteFolderOrFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return DeleteFile(path);
                }
                if (Directory.Exists(path))
                {
                    return DeleteFolder(path);
                }
                return false;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }


        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool DeleteFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                    return false;
                DeleteFileByDirectory(dir);
                DbgTrace.dout("删除文件夹" + path);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        ///   用来遍历删除目录下的文件以及该文件夹
        /// </summary>
        private static void DeleteFileByDirectory(DirectoryInfo info)
        {
            try
            {
                if (!info.Exists)
                    return;
                foreach (DirectoryInfo newInfo in info.GetDirectories())
                {
                    if (IsDefaultTemplateClass.IsDefaultTemplatesDir(newInfo)) continue;
                    DeleteFileByDirectory(newInfo);
                }
                foreach (FileInfo newInfo in info.GetFiles())
                {

                    SetFileAttributesNormal(newInfo);
                    newInfo.Delete();
                }
                SetDirectoryNormal(info);
                //TemplateFiles文件夹下还有DefaultTemplate文件夹，文件不为空，删除的时候会抛出异常
                if (!IsDefaultTemplateClass.IsTemplateFilesDir(info))
                    info.Delete();
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }

        private static void SetDirectoryNormal(DirectoryInfo info)
        {
            FileAttributeNormal.SetDirectoryNormal(info);
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="savedPath"></param>
        /// <returns></returns>
        private static bool DeleteFile(string savedPath)
        {
            try
            {
                if (!File.Exists(savedPath))
                    return false;
                FileInfo fi = new FileInfo(savedPath);
                SetFileAttributesNormal(fi);


                File.Delete(savedPath);
                DbgTrace.dout("删除文件" + savedPath);
                return true;
            }
            catch (System.Exception ex)
            {
                string log = string.Format(ex.Message + ex.StackTrace);
                DbgTrace.dout("{0}", log);

                return false;
            }
        }

        private static void SetFileAttributesNormal(FileInfo fi)
        {
            FileAttributeNormal.SetFileAttributesNormal(fi);
        }
    }

    internal class IsDefaultTemplateClass
    {
        /// <summary>
        /// 判断是否是不参与上传下载的DefaultTemplat目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsDefaultTemplatesDir(DirectoryInfo dir)
        {
            if (dir.Parent != null)
            {
                if (dir.Parent.Parent != null)
                {
                    if (string.Equals(dir.Name, "DefaultTemplate") && string.Equals(dir.Parent.Name, "TemplateFiles") /*&& string.Equals(dir.Parent.Parent.Name, "Excel&Word Reporter")*/)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否是TemplateFiles文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsTemplateFilesDir(DirectoryInfo dir)
        {
            if (dir.Parent != null)
            {
                if (dir.Parent.Parent != null)
                {
                    if (string.Equals(dir.Name, "TemplateFiles") /*&& string.Equals(dir.Parent.Name, "Excel&Word Reporter")*/)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    internal class ZipClass
    {
        /// <summary>
        /// 所有文件缓存
        /// </summary>
        private List<string> files = new List<string>();

        /// <summary>
        /// 所有空目录缓存
        /// </summary>
        private List<string> paths = new List<string>();

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件</param>
        /// <param name="zipedFile">压缩后的文件全名</param>
        /// <param name="compressionLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>
        /// <param name="blockSize">分块大小</param>
        public void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            if (!System.IO.File.Exists(fileToZip)) //如果文件没有找到，则报错
            {
                throw new FileNotFoundException("The specified file " + fileToZip + " could not be found. Zipping aborderd");
            }

            FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read);
            FileStream zipFile = File.Create(zipedFile);
            ZipOutputStream zipStream = new ZipOutputStream(zipFile);
            ZipEntry zipEntry = new ZipEntry(fileToZip);
            zipStream.PutNextEntry(zipEntry);
            zipStream.SetLevel(compressionLevel);
            byte[] buffer = new byte[blockSize];
            int size = streamToZip.Read(buffer, 0, buffer.Length);
            zipStream.Write(buffer, 0, size);

            try
            {
                while (size < streamToZip.Length)
                {
                    int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                throw ex;
            }

            zipStream.Finish();
            zipStream.Close();
            streamToZip.Close();
            GC.Collect();
        }

        /// <summary>
        /// 压缩目录（包括子目录及所有文件）
        /// </summary>
        /// <param name="rootPath">要压缩的根目录</param>
        /// <param name="destinationPath">保存路径</param>
        /// <param name="compressLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>
        public bool ZipFileFromDirectory(string rootPath, string destinationPath, int compressLevel)
        {
            ZipOutputStream outPutStream = new ZipOutputStream(File.Create(destinationPath));
            try
            {

                GetAllDirectories(rootPath);

                string rootMark = rootPath + "\\"; //得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。
                Crc32 crc = new Crc32();

                outPutStream.SetLevel(compressLevel); // 0 - store only to 9 - means best compression
                foreach (string file in files)
                {
                    FileStream fileStream = File.OpenRead(file); //打开压缩文件
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(file.Replace(rootMark, string.Empty));
                    entry.DateTime = DateTime.Now;
                    // set Size and the crc, because the information
                    // about the size and crc should be stored in the header
                    // if it is not set it is automatically written in the footer.
                    // (in this case size == crc == -1 in the header)
                    // Some ZIP programs have problems with zip files that don't store
                    // the size and crc in the header.
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    outPutStream.PutNextEntry(entry);
                    outPutStream.Write(buffer, 0, buffer.Length);
                }

                foreach (string emptyPath in paths)
                {
                    ZipEntry entry = new ZipEntry(emptyPath.Replace(rootMark, string.Empty) + "/");
                    outPutStream.PutNextEntry(entry);
                }
                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                this.files.Clear();
                this.paths.Clear();
                outPutStream.Finish();
                outPutStream.Close();
                GC.Collect();
            }
        }

        /// <summary>
        /// 取得目录下所有文件及文件夹，分别存入files及paths
        /// </summary>
        /// <param name="rootPath">根目录</param>
        private void GetAllDirectories(string rootPath)
        {
            string[] subPaths = Directory.GetDirectories(rootPath); //得到所有子目录
            foreach (string path in subPaths)
            {
                if (IsDefaultTemplateClass.IsDefaultTemplatesDir(new DirectoryInfo(path)))
                    continue;
                GetAllDirectories(path); //对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                this.files.Add(file); //将当前目录中的所有文件全名存入文件List
            }
            if (subPaths.Length == files.Length && files.Length == 0) //如果是空目录
            {
                this.paths.Add(rootPath); //记录空目录
            }
        }

        private static void GetAllDirectories(string rootPath, ref List<string> pathsList, ref List<string> filesList)
        {
            string[] subPaths = Directory.GetDirectories(rootPath); //得到所有子目录
            foreach (string path in subPaths)
            {
                GetAllDirectories(path, ref pathsList, ref filesList); //对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                filesList.Add(file); //将当前目录中的所有文件全名存入文件List
            }
            if (subPaths.Length == files.Length && files.Length == 0) //如果是空目录
            {
                pathsList.Add(rootPath); //记录空目录
            }
        }

        /// <summary>
        /// 解压缩文件(压缩文件中含有子目录)
        /// </summary>
        /// <param name="zipfilepath">待解压缩的文件路径</param>
        /// <param name="unzippath">解压缩到指定目录</param>
        /// <returns>解压后的文件列表</returns>
        public bool UnZip(string zipfilepath, string unzippath)
        {

            ZipInputStream s = new ZipInputStream(File.OpenRead(zipfilepath));
            //解压出来的文件列表
            List<string> unzipFiles = new List<string>();
            try
            {
                //检查输出目录是否以“\\”结尾
                if (unzippath.EndsWith("\\") == false || unzippath.EndsWith(":\\") == false)
                {
                    unzippath += "\\";
                }

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsDirectory)
                    {
                        UnCompressFolder(unzippath, theEntry);
                    }
                    else
                    {
                        UncomressFile(unzippath, unzipFiles, s, theEntry);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout("{0}{1}", "UnZip(string zipfilepath, string unzippath)  catch (Exception ex) :", ex.Message);
                return false;
            }
            finally
            {
                s.Close();
                GC.Collect();
            }
        }

        private static void UnCompressFolder(string unzippath, ZipEntry theEntry)
        {
            string directoryName = Path.GetDirectoryName(unzippath);
            string fileName = theEntry.Name; //Path.GetFileName(theEntry.Name);

            //生成解压目录【用户解压到硬盘根目录时，不需要创建】
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (fileName != String.Empty)
            {
                string folderPath = Path.Combine(unzippath, fileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
        }

        private static void UncomressFile(string unzippath, List<string> unzipFiles, ZipInputStream s, ZipEntry theEntry)
        {
            string directoryName = Path.GetDirectoryName(unzippath);
            string fileName = Path.GetFileName(theEntry.Name);

            //生成解压目录【用户解压到硬盘根目录时，不需要创建】
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (fileName != String.Empty)
            {
                //解压文件到指定的目录
                directoryName = Path.GetDirectoryName(unzippath + theEntry.Name);
                //建立下面的目录和子目录
                Directory.CreateDirectory(directoryName);

                //记录导出的文件
                unzipFiles.Add(unzippath + theEntry.Name);

                FileStream streamWriter = File.Create(unzippath + theEntry.Name);

                int size = 2048;
                byte[] data = new byte[2048];
                while (true)
                {
                    size = s.Read(data, 0, data.Length);
                    if (size > 0)
                    {
                        streamWriter.Write(data, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
                streamWriter.Close();
            }
        }

    }


    #endregion

    #region 结构定义

    public enum ModuleFileTypes
    {
        /// <summary>
        /// 所有模板，包括预置模板和非预置模板
        /// </summary>
        AllTemplates = 1,

        /// <summary>
        /// OfficeReport.dat文件
        /// </summary>
        OfficeReportDat = 2,

        /// <summary>
        /// 更新时间
        /// </summary>
        UpdateTime = 3,

        /// <summary>
        /// Mac地址，用于标识上传数据的计算机
        /// </summary>
        MacAddress = 4,
    }

    #endregion

    #region 单例类

    /// <summary>
    /// 用于协助各测试模式封装类实现单例的辅助类
    /// 实际上并不是真正的单例，客户还是可以直接创建T类型的对象
    /// 因为T类型的构造函数不是私有的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static object _lockHelper = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHelper)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
    }

    #endregion

}
