using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace OfficeReportInterface
{
    /// <summary>
    ///ConfigFileManager 的摘要说明
    /// </summary>
    public class ConfigFileManager
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string IniPath
        {
            set { privateIniPath = value; }
            get { return privateIniPath; }
        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary> 
        /// 构造方法 
        /// </summary>     
        public ConfigFileManager()
        {            
        }

        /// <summary>
        /// 私有文件路径
        /// </summary>
        private string privateIniPath;

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static ConfigFileManager DataManager = new ConfigFileManager();

        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public bool WriteValue(string Section, string Key, string Value)
        {
            bool result = false;
            try
            {
                long code = WritePrivateProfileString(Section, Key, Value, this.privateIniPath);
                if (code == 0)
                    result = false;
                else
                    result = true;
            }
            catch(Exception ex)
            {
                ErrorInfoManager.Instance.WriteLogMessage("ConfigFileManager.WriteValue", ex);
                result = false;
            }
            return result;
        }

        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string IniReadValue(string Section, string Key, string iniPath)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, iniPath);
            return temp.ToString();
        }

        /// <summary>
        /// 读出INI文件（使用属性中的路径查找文件）
        /// </summary>
        /// <param name="Section">项目名称</param>
        /// <param name="Key">键</param>
        /// <returns></returns>
        public string ReadString(string Section, string Key)
        {
            string result = string.Empty;
            result = IniReadValue(Section,Key,this.privateIniPath);
            if (string.IsNullOrWhiteSpace(result))
                result = string.Empty;
            return result.Trim();
        }

        public int ReadInt32(string Section, string Key)
        {
            int result = int.MinValue;
            string s = IniReadValue(Section, Key, this.privateIniPath);
            try
            {
                result = Convert.ToInt32(s);
            }
            catch
            {
                result = int.MinValue;
            }
            return result;
        }

        public uint ReadUInt32(string Section, string Key)
        {
            uint result = uint.MinValue;
            string s = IniReadValue(Section, Key, this.privateIniPath);
            try
            {
                result = Convert.ToUInt32(s);
            }
            catch
            {
                result = uint.MinValue;
            }
            return result;
        }

        public double ReadDouble(string Section, string Key)
        {
            double result = double.NaN;
            string s = IniReadValue(Section, Key, this.privateIniPath);
            try
            {
                result = Convert.ToDouble(s);
            }
            catch
            {
                result = double.NaN;
            }
            return result;
        }

        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile(string iniPath)
        {
            return File.Exists(iniPath);
        }

        /// <summary>
        /// 验证文件是否存在（使用属性中的路径查找文件）
        /// </summary>
        /// <returns></returns>
        public bool ExistINIFile()
        {
            return File.Exists(this.privateIniPath);
        }
    }
}