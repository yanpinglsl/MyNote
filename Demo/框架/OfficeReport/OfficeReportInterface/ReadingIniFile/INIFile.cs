using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;


namespace OfficeReportInterface.ReadingIniFile
{
    public class INIFile
    {
        string _FileName;

        #region 导入DLL函数
        //[DllImport("kernel32.dll")]
        //        private extern static int GetPrivateProfileIntA(string segName, string keyName, int iDefault, string fileName);
        [DllImport("kernel32.dll")]
        private extern static int GetPrivateProfileStringA(string segName, string keyName, string sDefault, StringBuilder buffer, int nSize, string fileName);
        [DllImport("kernel32.dll")]
        private extern static int GetPrivateProfileSectionA(string segName, StringBuilder buffer, int nSize, string fileName);
        [DllImport("kernel32.dll")]
        private extern static int WritePrivateProfileSectionA(string segName, string sValue, string fileName);
        [DllImport("kernel32.dll")]
        private extern static int WritePrivateProfileStringA(string segName, string keyName, string sValue, string fileName);
        [DllImport("kernel32.dll")]
        private extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);
        #endregion

        public INIFile(string FileName)
        {
            _FileName = FileName;
            if (!FileExists())
                CreateFile();
        }
        #region AboutFile
        /// <summary>
        /// 表示OfficeReport.ini文件对象
        /// </summary>
        private static INIFile OfficeReportIniFile=null;
        /// <summary>
        /// 表示OfficeReportTemplates.ini文件对象
        /// </summary>
        private static INIFile OfficeReportTemplatesIniFile = null;
        public enum IniFileType
        {
            /// <summary>
            /// OfficeReport.ini
            /// </summary>
            OfficeReportIni=1,
            /// <summary>
            /// OfficeReportTemplates.ini
            /// </summary>
            OfficeReportTemplatesIni=2
        }

        /// <summary>
        /// 获取OfficeReport.ini配置文件对象。如果文件不存在，则返回null
        /// </summary>
        /// <returns></returns>
        public static INIFile GetIniFileByType(IniFileType type)
        {
            try
            {
                INIFile iniFileTemp = type == IniFileType.OfficeReportIni ? OfficeReportIniFile : OfficeReportTemplatesIniFile;

                if (iniFileTemp != null)
                    return iniFileTemp;
                string nameOfIniFile = type == IniFileType.OfficeReportIni ? "OfficeReport.ini" : "OfficeReportTemplates.ini";
                string fileName = Path.Combine(DbgTrace.GetAssemblyPath(), nameOfIniFile);

                if (!File.Exists(fileName))
                {
                    DbgTrace.dout("Can not find file " + fileName);
                }
                else
                {
                    var iniFile = new INIFile(fileName);
                    Interlocked.CompareExchange(ref iniFileTemp, iniFile, null);
                }
                return iniFileTemp;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return null;
            }
        }

        #endregion
        #region Read

        /// <summary>
        /// 返回字符串
        /// </summary>
        public string ReadString(string Section, string Key)
        {
            StringBuilder buffer = new StringBuilder(65535);
            GetPrivateProfileStringA(Section, Key, "", buffer, buffer.Capacity, _FileName);
            return buffer.ToString();
        }

        /// <summary>
        /// 返回int型的数
        /// </summary>
        public virtual int ReadInt(string Section, string Key)
        {
            int result;
            try
            {
                result = int.Parse(this.ReadString(Section, Key));
            }
            catch
            {
                result = -1;
            }
            return result;
        }
        /// <summary>
        /// 返回long型的数
        /// </summary>
        public virtual long ReadLong(string Section, string Key)
        {
            long result;
            try
            {
                result = long.Parse(this.ReadString(Section, Key));
            }
            catch
            {
                result = -1;
            }
            return result;
        }
        /// <summary>
        /// 返回byte型的数
        /// </summary>
        public virtual byte ReadByte(string Section, string Key)
        {
            byte result;
            try
            {
                result = byte.Parse(this.ReadString(Section, Key));
            }
            catch
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// 返回float型的数
        /// </summary>
        public virtual float ReadFloat(string Section, string Key)
        {
            float result;
            try
            {
                result = float.Parse(this.ReadString(Section, Key));
            }
            catch
            {
                result = -1;
            }
            return result;
        }
        /// <summary>
        /// 返回double型的数
        /// </summary>
        public virtual double ReadDouble(string Section, string Key)
        {
            double result;
            try
            {
                result = double.Parse(this.ReadString(Section, Key));
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        #endregion _Endregion;

        #region others
        /// <summary>
        /// 返回该配置文件中所有Section名称的集合
        /// </summary>
        public ArrayList ReadSections()
        {
            byte[] buffer = new byte[65535];
            int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), _FileName);
            int iCnt, iPos;
            ArrayList arrayList = new ArrayList();
            string tmp;
            if (rel > 0)
            {
                iCnt = 0; iPos = 0;
                for (iCnt = 0; iCnt < rel; iCnt++)
                {
                    if (buffer[iCnt] == 0x00)
                    {
                        tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt).Trim();
                        iPos = iCnt + 1;
                        if (tmp != "")
                            arrayList.Add(tmp);
                    }
                }
            }
            return arrayList;
        }
        /// <summary>
        /// 判断指定的节是否存在
        /// </summary>
        public bool SectionExists(string Section)
        {
            //done SectionExists
            StringBuilder buffer = new StringBuilder(65535);
            GetPrivateProfileSectionA(Section, buffer, buffer.Capacity, _FileName);
            if (buffer.ToString().Trim() == "")
                return false;
            else
                return true;
        }
        /// <summary>
        /// 判断指定的节中指定的键是否存在
        /// </summary>
        public bool ValueExits(string Section, string Key)
        {
            if (ReadString(Section, Key).Trim() == "")
                return false;
            else
                return true;
        }
        ///// <summary>
        // /// 删除指定的节中的指定键
        // /// </summary>
        // /// <param name="Section">该键所在的节的名称</param>
        // /// <param name="Key">该键的名称</param>
        // public void DeleteKey(string Section, string Key)
        // {            
        //     Write(BitVector32.Section, Key, null);
        // }
        /// <summary>
        /// 删除指定的节的所有内容
        /// </summary>
        /// <param name="Section">要删除的节的名字</param>
        public void DeleteSection(string Section)
        {
            WritePrivateProfileSectionA(Section, null, _FileName);
        }
        /// <summary>
        /// 添加一个节
        /// </summary>
        /// <param name="Section">要添加的节名称</param>
        public void AddSection(string Section)
        {
            WritePrivateProfileSectionA(Section, "", _FileName);
        }
        #endregion
        #region AboutFile
        /// <summary>
        /// 删除文件
        /// </summary>
        public void DeleteFile()
        {
            if (FileExists())
                File.Delete(_FileName);
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        public void CreateFile()
        {
            File.Create(_FileName).Close();
        }
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool FileExists()
        {
            return File.Exists(_FileName);
        }
        #endregion        
    
    }
}
