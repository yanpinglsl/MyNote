using System;


using System.Diagnostics;
using System.Reflection;
using System.Threading;
using OfficeReportInterface.ReadingIniFile;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;


namespace OfficeReportInterface
{
    /// <summary>
    /// 对excel文件本身的打开、关闭等操作
    /// </summary>
    public class ExcelFileOperate
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        /// <summary>
        /// 程序只打开一个Excel进程
        /// </summary>
        public static Excel.Application m_objExcel;

        /// <summary>
        /// 模板文件路径
        /// </summary>
        private string tempFilePath;

        /// <summary>
        /// 打开模板文件是否成功
        /// </summary>
        private bool openTempExcel;

        /// <summary>
        /// 是否是需要导出CSV的情况  IBD群创项目需要导出CSV文件 值是3表示需要导出CSV
        /// </summary>
        private static int _ExportTypeWithCSV = int.MinValue;

        /// <summary>
        /// 打开Excel的次数 修改程序，改为可配置，可以配置执行一次打开或者执行2次打开(Microsoft Office Home and Business 2016)，因为有的版本Excel，执行2次存在问题（Excel 2007），正因为这样才改成了执行1次，现在又在有的环境有问题。因此改为可配置。
        /// </summary>
        private static int _OpenExcelTimes = int.MinValue;

        /// <summary>
        /// 生成文件类型
        /// </summary>
        public enum ExportFileType
        {
            /// <summary>
            /// 只导出Excel
            /// </summary>
            OnlyExcel = 0,

            /// <summary>
            /// 只生成html
            /// </summary>
            OnlyHtml = 1,

            /// <summary>
            /// 生成Excel和html
            /// </summary>
            ExcelAndHtml = 2,

            /// <summary>
            /// 生成Excel和CSV
            /// </summary>
            ExcelAndCSV = 3
        }

        /// <summary>
        /// 导出的时候需要打开Excel的次数
        /// </summary>
        public enum OpenExcelTimesOfExport
        {
            /// <summary>
            /// 尚未初始化
            /// </summary>
            NoInitialized = 0,

            /// <summary>
            /// 只需要打开一次
            /// </summary>
            Once = 1,

            /// <summary>
            /// 需要打开2次
            /// </summary>
            Twice = 2
        }

        private Excel.Application objExcel
        {
            get
            {
                if (m_objExcel != null)
                    return m_objExcel;
                Excel.Application temp = null;

                temp = new Excel.Application();
                Interlocked.CompareExchange(ref m_objExcel, temp, null);
                return m_objExcel;
            }
            set { m_objExcel = value; }
        }

        private bool isInitialize = false;
        public Excel.Workbook objBook;
        private Excel.Worksheet objSheet;
        private Excel.Range objRange;

        /// <summary>
        /// 执行宏的时候的锁
        /// </summary>
        private static object excelLocker = new object();

        #region 属性

        /// <summary>
        /// 是否是查询条件报表
        /// </summary>
        public bool isQueryCondition;

        /// <summary>
        /// ObjBook
        /// </summary>
        public Excel.Workbook ObjBook
        {
            get { return this.objBook; }
            set { this.objBook = value; }
        }

        /// <summary>
        /// ObjSheet
        /// </summary>
        public Excel.Worksheet ObjSheet
        {
            get { return this.objSheet; }
            set { this.objSheet = value; }
        }

        /// <summary>
        /// ObjRange
        /// </summary>
        public Excel.Range ObjRange
        {
            get { return this.objRange; }
            set { this.objRange = value; }
        }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string TempFilePath
        {
            get { return this.tempFilePath; }
            set { this.tempFilePath = value; }
        }

        /// <summary>
        /// 打开模板文件是否成功
        /// </summary>
        public bool OpenTempExcel
        {
            get { return this.openTempExcel; }
            set { this.openTempExcel = value; }
        }

        #endregion

        public ExcelFileOperate(string path)
        {
            object lockHelper = new object();
            try
            {
                Monitor.Enter(lockHelper);
                if (!isInitialize)
                {
                    isInitialize = true;
                    if (objExcel.Application.DisplayAlerts)
                        objExcel.Application.DisplayAlerts = false;
                    if (objExcel.AskToUpdateLinks)
                        objExcel.AskToUpdateLinks = false;
                    if (objExcel.Visible)
                        objExcel.Visible = false;
                }
                tempFilePath = path;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            finally
            {
                Monitor.Exit(lockHelper);
            }
        }

        private bool GetWorkBorkByName(out Excel.Workbook objBookResult)
        {
            objBookResult = null;
            for (int i = 1; i < objExcel.Workbooks.Count + 1; i++)
            {
                Excel.Workbook tempBook = (Excel.Workbook) objExcel.Workbooks.get_Item(i);
                if (tempBook.FullName == tempFilePath)
                {
                    objBookResult = (Excel.Workbook) objExcel.Workbooks.get_Item(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 打开模板excel文件
        /// </summary>
        public bool ExcelFileOpened(ref string error)
        {
            try
            {
                DbgTrace.dout("开始: 打开 workbook " + tempFilePath);
                if (objExcel == null)
                    objExcel = new Excel.Application();
                objExcel.Application.DisplayAlerts = false;

                objExcel.Workbooks.Open(tempFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                for (int i = 1; i < objExcel.Workbooks.Count + 1; i++)
                {
                    Excel.Workbook tempBook = (Excel.Workbook) objExcel.Workbooks.get_Item(i);
                    try
                    {
                        DbgTrace.dout("workbook.FullName=" + tempBook.FullName + "是否等于要打开的workbook？" + (tempBook.FullName == tempFilePath));
                    }
                    catch (Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }

                    if (tempBook.FullName == tempFilePath)
                    {
                        objBook = (Excel.Workbook) objExcel.Workbooks.get_Item(i);
                        break;
                    }
                }
                if (objBook != null)
                {
                    openTempExcel = true;
                }
                else
                {
                    openTempExcel = false;
                }
                DbgTrace.dout("结束: 打开 workbook " + tempFilePath);
                return openTempExcel;
            }
            catch (System.Exception ex)
            {
                error = ex.Message + ex.StackTrace;
                DbgTrace.dout(ex.Message + ex.StackTrace);
                objExcel = null;

                openTempExcel = false;
                return openTempExcel;
            }
        }

        /// <summary>
        /// 保存Excel，needExport为0时只生成excel，为1时只生成html，为2时两者都生成
        /// </summary>
        /// <param name="saveExcelFilePath"></param>
        /// <param name="saveHtmlFilePath"></param>
        /// <param name="needExport"></param>
        /// <param name="sourceId">标识报表的id，目前主要是为了区分是否是智慧安全用电评估报告，如果是，则web需要考虑性能问题，高并发问题，做特殊处理。其他报表暂不统一处理是考虑到可能引入新问题，例如PQ报表之前就发现执行宏只执行一半就异常了，必须是使用Excel文件读写的方式实现生成html。</param>
        /// <returns></returns>
        public bool SaveExcelApp(string saveExcelFilePath, string saveHtmlFilePath, int needExport, uint sourceId)
        {
            if (!File.Exists(tempFilePath))
                return false;
            if (objBook == null)
                return false;
            try
            {
                objExcel.AlertBeforeOverwriting = false;
                Excel.Workbook tempWorkBook = objBook;
                object ofmt = Excel.XlFileFormat.xlHtml;
                if (!Directory.Exists(Path.GetDirectoryName(saveExcelFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(saveExcelFilePath));
                var tempTime = DateTime.Now;
                if (needExport == 1 && sourceId == (uint) RepServFileType.Safety /*&&(!isQueryCondition)*/) //智慧安全用电运行评估报告，只需要生成html的时候，就只生成html文件，不生成Excel了。原因：Excel的一个saveAs耗时6~11s，不能满足智慧安全用电评估报告5s内返回结果的需要。
                {
                    try
                    {
                        objExcel.Run(string.Format("{0}!{1}", (new FileInfo(tempFilePath)).Name, "ThisWorkbook.Workbook_Open"));
                    }
                    catch (Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }
                    tempWorkBook.SaveAs(saveHtmlFilePath, ofmt, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    this.CloseExcelConnect();
                    DbgTrace.dout("ExportHtmlRunMacro.objBook.SaveAs saveHtmlFilePath" + saveHtmlFilePath + "  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                    return true;
                }

                //保存为excel
                DbgTrace.dout("saveExcelFilePath = {0} ", saveExcelFilePath.ToString());
                tempTime = DateTime.Now;
                tempWorkBook.SaveAs(saveExcelFilePath, Type.Missing, Type.Missing, Type.Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                DbgTrace.dout("WorkBook.SaveAs " + saveExcelFilePath + " 耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                if (needExport == (int) ExportFileType.OnlyExcel && IsCSVExport()) //如果需要导出csv 
                {
                    ExportCSVFile(saveExcelFilePath);
                }
                //修改程序，改为可配置，可以配置执行一次打开或者执行2次打开(Microsoft Office Home and Business 2016)，因为有的版本Excel，执行2次存在问题（Excel 2007），正因为这样才改成了执行1次，现在又在有的环境有问题。因此改为可配置。
                if (NeedOpenExcelTwice() && (needExport == (int) ExportFileType.OnlyHtml || needExport == (int) ExportFileType.ExcelAndHtml)) //保存为html的时候才需要打开2次用于执行宏，只是生成Excel,不需要打开2次
                {
                    DbgTrace.dout("OfficeReport.ini [ExcelConfig] _OpenExcelTimes=2");
                    ExcelFileOperate ExportExcelRunMacro = new ExcelFileOperate(saveExcelFilePath);
                    string tempErrorString1 = string.Empty;
                    tempTime = DateTime.Now;
                    ExportExcelRunMacro.ExcelFileOpened(ref tempErrorString1);
                    DbgTrace.dout("Open Excel耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                    tempTime = DateTime.Now;
                    try
                    {
                        ExportExcelRunMacro.objBook.Save(); //这里可能已保存会报错，万一报错要保证继续执行
                    }
                    catch (Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }
                    DbgTrace.dout("ExportExcelRunMacro.objBook.Save()耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                    tempTime = DateTime.Now;
                    ExportExcelRunMacro.CloseExcelConnect();
                    DbgTrace.dout("ExportExcelRunMacro.CloseExcelConnect()  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                }
                //保存为html
                if (needExport == (int) ExportFileType.OnlyHtml || needExport == (int) ExportFileType.ExcelAndHtml)
                {
                    //由于宏不能运行，需要重新打开excel再运行
                    tempTime = DateTime.Now;
                    ExcelFileOperate ExportHtmlRunMacro = new ExcelFileOperate(saveExcelFilePath);
                    DbgTrace.dout("ExcelFileOperate ExportHtmlRunMacro = new ExcelFileOperate(saveExcelFilePath)  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                    string tempErrorString = string.Empty;

                    tempTime = DateTime.Now;
                    ExportHtmlRunMacro.ExcelFileOpened(ref tempErrorString);
                    DbgTrace.dout("ExportHtmlRunMacro.ExcelFileOpened(ref tempErrorString)  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");

                    try
                    {
                        tempTime = DateTime.Now;
                        ExportHtmlRunMacro.objBook.SaveAs(saveHtmlFilePath, ofmt, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        DbgTrace.dout("ExportHtmlRunMacro.objBook.SaveAs saveHtmlFilePath  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                    }
                    catch (Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }

                    tempTime = DateTime.Now;
                    ExportHtmlRunMacro.CloseExcelConnect();
                    DbgTrace.dout("ExportHtmlRunMacro.CloseExcelConnect()  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                }

                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 是否配置了是CSV报表导出
        /// </summary>
        /// <returns></returns>
        public static bool IsCSVExport()
        {
            if (_ExportTypeWithCSV < (int) ExportFileType.OnlyExcel) //说明还没有读取过ini配置文件
            {
                var iniFile = INIFile.GetIniFileByType(INIFile.IniFileType.OfficeReportIni);
                if (iniFile != null && iniFile.ReadInt("ExcelConfig", "exportTypeForSendEmail") == (int) ExportFileType.ExcelAndCSV) //3表示需要导出csv格式的文件。这里取3，是考虑到后面可能做到界面上，支持0，1，2，3的情况。（0时只生成excel，为1时只生成html，为2时两者都生成）
                {
                    _ExportTypeWithCSV = (int) ExportFileType.ExcelAndCSV;
                }
                else
                {
                    _ExportTypeWithCSV = (int) ExportFileType.OnlyExcel; //用于标识已经读过ini配置文件了
                }
            }

            return _ExportTypeWithCSV == (int) ExportFileType.ExcelAndCSV;
        }

        /// <summary>
        /// 是否需要打开Excel2次
        /// </summary>
        /// <returns></returns>
        public bool NeedOpenExcelTwice()
        {
            if (_OpenExcelTimes <= 0)
            {
                var iniFile = INIFile.GetIniFileByType(INIFile.IniFileType.OfficeReportIni); //修改程序，改为可配置，可以配置执行一次打开或者执行2次打开(Microsoft Office Home and Business 2016)，因为有的版本Excel，执行2次存在问题（Excel 2007），正因为这样才改成了执行1次，现在又在有的环境有问题。因此改为可配置。
                if (iniFile != null && iniFile.ReadInt("ExcelConfig", "OpenExcelTimes") == (int) OpenExcelTimesOfExport.Twice)
                    _OpenExcelTimes = (int) OpenExcelTimesOfExport.Twice;
                else
                {
                    _OpenExcelTimes = (int) OpenExcelTimesOfExport.Once;
                }
            }
            return _OpenExcelTimes == (int) OpenExcelTimesOfExport.Twice;
        }

        /// <summary>
        /// 拷贝excel文件的副本，再做后面的操作，避免操作原件引起并发占用问题
        /// </summary>
        /// <param name="saveExcelFilePath"></param>
        /// <param name="tempXlsFilePath"></param>
        /// <returns></returns>
        public static bool CopyExcelFile(string saveExcelFilePath, out string tempXlsFilePath)
        {
            lock (_copyExcelFileLocker)
            {
                try
                {
                    string folderPath = Path.GetDirectoryName(saveExcelFilePath);
                    string fileName = Path.GetFileNameWithoutExtension(saveExcelFilePath);
                    tempXlsFilePath = GetFileFullNameByTime(folderPath, fileName);
                    while (true)
                    {
                        if (File.Exists(tempXlsFilePath))
                        {
                            tempXlsFilePath = GetFileFullNameByTime(folderPath, fileName);
                        }
                        else
                        {
                            break;
                        }
                    }

                    File.Copy(saveExcelFilePath, tempXlsFilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorInfoManager.Instance.WriteLogMessage("Copy excel file " + saveExcelFilePath + " failed. " + ex.Message + ex.StackTrace);
                    tempXlsFilePath = string.Empty;
                    return false;
                }
            }
        }

        private static object _copyExcelFileLocker = new object();

        private static string GetFileFullNameByTime(string folderPath, string fileName)
        {
            return Path.Combine(folderPath, fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        }

        /// <summary>
        /// 导出CSV文件
        /// </summary>
        /// <param name="saveExcelFilePath"></param>
        private static void ExportCSVFile(string saveExcelFilePath)
        {
            DateTime tempTime;
            try
            {
                //string folderPath = Path.GetDirectoryName(saveExcelFilePath);
                //string fileNameOfCSV = Path.GetFileNameWithoutExtension(saveExcelFilePath);
                //string tempXlsFilePath = Path.Combine(folderPath, fileNameOfCSV + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                //if (File.Exists(tempXlsFilePath))
                //    tempXlsFilePath = tempXlsFilePath + "(2)";
                //File.Copy(saveExcelFilePath, tempXlsFilePath);
                string tempXlsFilePath;
                if (!CopyExcelFile(saveExcelFilePath, out tempXlsFilePath))
                    return;

                ExcelFileOperate ExportExcelRunMacro = new ExcelFileOperate(tempXlsFilePath);
                string tempErrorString1 = string.Empty;
                tempTime = DateTime.Now;
                ExportExcelRunMacro.ExcelFileOpened(ref tempErrorString1);
                DbgTrace.dout("Open Excel耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                tempTime = DateTime.Now;
                string csvFilePath = GetCsvFileNameByExcelFileName(saveExcelFilePath);

                if (File.Exists(csvFilePath)) //如果同名文件已存在，则删除再生成
                    File.Delete(csvFilePath);
                ExportExcelRunMacro.objBook.SaveAs(csvFilePath, Excel.XlFileFormat.xlCSV, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                DbgTrace.dout("ExportExcelRunMacro.objBook.Save()耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                tempTime = DateTime.Now;
                ExportExcelRunMacro.CloseExcelConnect();
                DbgTrace.dout("ExportExcelRunMacro.CloseExcelConnect()  耗时：" + (DateTime.Now - tempTime).TotalSeconds + "s");
                File.Delete(tempXlsFilePath);
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 通过xls文件名称获取csv文件名称
        /// </summary>
        /// <returns></returns>
        public static string GetCsvFileNameByExcelFileName(string saveExcelFilePath)
        {
            try
            {
                if (saveExcelFilePath == null)
                    return string.Empty;
                string folderPath = Path.GetDirectoryName(saveExcelFilePath);
                string fileNameOfCSV = Path.GetFileNameWithoutExtension(saveExcelFilePath);
                string csvFilePath = Path.Combine(folderPath, fileNameOfCSV + ".csv");
                return csvFilePath;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return string.Empty;
            }
        }

        /// <summary>
        /// 关闭对象，清理释放资源
        /// </summary>
        /// <returns></returns>
        public bool CloseExcelConnect()
        {
            try
            {
                DbgTrace.dout("开始: 关闭 workbook " + tempFilePath);
                if (objRange != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(objRange);
                    }
                    catch (System.Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }
                }
                if (objSheet != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(objSheet);
                    }
                    catch (System.Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }
                }
                if (objBook != null)
                {
                    try
                    {
                        objBook.Close(false, tempFilePath, Type.Missing);
                    }
                    catch (System.Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }
                }
                if (objBook != null)
                {
                    try
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(objBook);
                    }
                    catch (System.Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                    }
                }
                DbgTrace.dout("结束: 关闭 workbook " + tempFilePath);
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                //KillExcelProcess();
            }
        }

        /// <summary>
        /// 主要用于发生异常时强制关闭当前启动的EXCEL进行
        /// </summary>
        public static void KillExcelProcess()
        {
            try
            {
                if (m_objExcel != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objExcel);
                    IntPtr t = new IntPtr(m_objExcel.Hwnd);
                    int k = 0;
                    GetWindowThreadProcessId(t, out k);
                    Process p = Process.GetProcessById(k);
                    p.Kill();
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }
    }
}

