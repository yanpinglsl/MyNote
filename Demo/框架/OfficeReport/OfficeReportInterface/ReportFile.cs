using PecsReport.CommomNode;
using PecsReport.PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace OfficeReportInterface
{
    /// <summary>
    /// 提供通用报表文件的类结构定义
    /// </summary>
    public class CommonReportFile
    {
        public const int DefaultRows = 50;
        public const int DefaultColumns = 50;
        // 新增报表文件的行列数最大限制
        public const int MaxRowsNum = 130;
        public const int MaxColumnsNum = 100;
        public const int DefaultRowHeight = 20;
        public const int DefaultColumnWidth = 60;

        private int reportflag;
        private int reportversion;
        private uint reporttype;

        private ReportQueryType reportquerytype;
        private bool overtimereport;

        private int rows;
        private int columns;
        private List<int> widths;
        private List<int> heights;
        private List<List<ReportCell>> reportCells;

        // 以下属性无需保存至数据文件中
        private string reportname;
        private bool bIsNewReport;      // 标识新建的未保存报表和打开的已保存报表
        private bool bNeedSaveReport;   // 标识当前报表程是否需要保存
        private ReportNode relatedReportNode;    // 当前报表文件关联的报表文件节点


        public int ReportFlag
        {
            get { return reportflag; }
        }

        public int ReportVersion
        {
            get { return reportversion; }
        }

        public uint ReportType
        {
            get { return reporttype; }
            set { reporttype = value; }
        }

        public string ReportName
        {
            get { return reportname; }
            set { reportname = value; }
        }

        public ReportQueryType QueryType
        {
            get { return reportquerytype; }
            set { reportquerytype = value; }
        }

        public bool IsOvertimeType
        {
            get { return overtimereport; }
            set { overtimereport = value; }
        }

        public int RowNumber
        {
            get { return rows; }
            set { rows = value; }
        }

        public int ColumnNumber
        {
            get { return columns; }
            set { columns = value; }
        }

        public List<int> RowHeights
        {
            get { return heights; }
        }

        public List<int> ColumnWidths
        {
            get { return widths; }
        }

        public bool IsNewReport
        {
            get
            {
                if (relatedReportNode == null)
                    return true;
                else
                    return false;
            }
        }

        public bool IsNeedSaveReport
        {
            get { return bNeedSaveReport; }
            set { bNeedSaveReport = value; }
        }

        public ReportNode RelatedReportNode
        {
            get { return relatedReportNode; }
            set { relatedReportNode = value; }
        }

        /// <summary>
        /// 获取单元格集合
        /// </summary>
        public List<List<ReportCell>> ReportCellSet
        {
            get { return reportCells; }
        }

        /// <summary>
        /// 获取当前报表单元格对像的索引
        /// </summary>
        /// <param name="row">行序号</param>
        /// <param name="column">列序号</param>
        /// <returns></returns>
        public ReportCell this[int row, int column]
        {
            get
            {
                // 索引范围合法
                if (row >= 0 && row < RowNumber && column >= 0 && column < ColumnNumber)
                {
                    return reportCells[row][column];
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 默认构造函数，创建Pecstar系统下的空白报表
        /// </summary>
        public CommonReportFile()
            : this(null, ReportNodeType.REPORTPECSTAR)
        {

        }

        /// <summary>
        /// 创建指定系统类型的空白报表
        /// </summary>
        public CommonReportFile(ReportNode relatedReportNode, uint reportFileType)
        {
            this.reportflag = 1;
            this.reportversion = 3;

            if (relatedReportNode == null)
            {
                this.reporttype = reportFileType;
                this.bIsNewReport = true;
                this.bNeedSaveReport = true;
                this.reportname = "未命名";
            }
            else
            {
                this.reporttype = relatedReportNode.NodeType;           // 根据该报表类型属性调用不同插件创建单元格对像
                this.bIsNewReport = false;
                this.bNeedSaveReport = false;
                this.reportname = relatedReportNode.NodeName;
            }

            // 默认设置为日报
            reportquerytype = ReportQueryType.ReportDay;

            // 设置默认的行列数
            this.rows = 0;
            this.columns = 0;

            this.widths = new List<int>();
            this.heights = new List<int>();
            this.reportCells = new List<List<ReportCell>>();

            // 设置相关联的节点
            this.relatedReportNode = relatedReportNode;

            // 默认不进行初始化
        }

        /// <summary>
        /// 创建指定行列数的默认报表文件
        /// 主要用于新建默认的报表文件
        /// </summary>
        /// <param name="relatedReportNode">报表文件关联的报表节点 null表示新建报表</param>
        /// <param name="reportFileType">报表文件类型</param>
        /// <param name="rownumber">行数</param>
        /// <param name="columnnumber">列数</param>
        public CommonReportFile(ReportNode relatedReportNode, uint reportFileType, int rownumber, int columnnumber)
        {
            this.reportflag = 1;
            this.reportversion = 3;

            if (relatedReportNode == null)
            {
                this.reporttype = reportFileType;  // 由reportFileType指定报表类型;
                this.bIsNewReport = true;
                this.bNeedSaveReport = true;
            }
            else
            {
                this.reporttype = relatedReportNode.NodeType;   // 根据该报表类型属性调用不同插件创建单元格对像
                this.bIsNewReport = false;
                this.bNeedSaveReport = false;
            }

            this.reportname = "未命名";
            // 默认设置为日报
            reportquerytype = ReportQueryType.ReportDay;

            // 设置默认的行列数
            this.rows = rownumber;
            this.columns = columnnumber;

            this.widths = new List<int>();
            this.heights = new List<int>();
            this.reportCells = new List<List<ReportCell>>();

            // 设置相关联的节点
            this.relatedReportNode = relatedReportNode;

            // 初始化相应行列数的单元格
            InitializeReportCells();
        }

        /// <summary>
        /// 根据报表类型和当前的行、列数创建默认的单元格列表
        /// 同时设置默认的行高和列宽
        /// 主要用于打开已经存在的报表文件
        /// </summary>
        /// <returns></returns>
        private bool InitializeReportCells()
        {
            // 根据行数和列数创建相应的单元格列表
            for (int r = 0; r < rows; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < columns; c++)
                {
                    // 创建单元格对像,根据报表类型创建不同系统下的报表单元格
                    ReportCell cell = new ReportCell(reporttype);
                    // 逐列加入单元格列列表中
                    cellColumnList.Add(cell);
                }
                // 逐行加入单元格列列表对像
                reportCells.Add(cellColumnList);
                // 逐行设置默认行高
                heights.Add(DefaultRowHeight);
            }

            // 逐列设置列宽
            for (int c = 0; c < columns; c++)
                widths.Add(DefaultColumnWidth);

            return true;

        }

        /// <summary>
        /// 用于外部初始化单元格
        /// </summary>
        /// <param name="columnWidth">指定列宽</param>
        /// <param name="rowHeight">指定行高</param>
        /// <returns></returns>
        public bool InitializeReportCells(int columnWidth, int rowHeight)
        {
            // 根据行数和列数创建相应的单元格列表
            for (int r = 0; r < rows; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < columns; c++)
                {
                    // 创建单元格对像,根据报表类型创建不同系统下的报表单元格
                    ReportCell cell = new ReportCell(reporttype);
                    // 逐列加入单元格列列表中
                    cellColumnList.Add(cell);
                }
                // 逐行加入单元格列列表对像
                reportCells.Add(cellColumnList);
                // 逐行设置默认行高
                heights.Add(rowHeight);
            }

            // 逐列设置列宽
            for (int c = 0; c < columns; c++)
                widths.Add(columnWidth);

            return true;
        }

        /// <summary>
        /// 根据模板报表初始化分区报表单元格-只针对分区报表
        /// </summary>
        /// <param name="rowNum">分区报表行</param>
        /// <param name="rowIndex">模板报表中用于创建报表行的测点单元格所在行</param>
        /// <param name="reportFile">模板报表文件</param>
        /// <returns></returns>
        public bool InitializeReportCellsByReportFile(int rowNum, int rowIndex, CommonReportFile reportFile)
        {
            // 根据模板报表创建单元格
            for (int r = 0; r < rowIndex; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < reportFile.ColumnNumber; c++)
                {
                    ReportCell cellItem = reportFile.GetReportCell(r, c);
                    cellColumnList.Add(cellItem);
                }
                // 将模板报表表头信息赋给报表
                reportCells.Add(cellColumnList);
            }
            for (int r = rowIndex; r < rowNum; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < reportFile.ColumnNumber; c++)
                {
                    ReportCell cellItem = reportFile.GetReportCell(rowIndex, c);
                    if (cellItem != null)
                    {
                        cellColumnList.Add((ReportCell)cellItem.Clone());
                    }
                    else
                    {
                        cellColumnList.Add(cellItem);
                    }
                }
                reportCells.Add(cellColumnList);
            }
            return true;
        }

        #region 报表单元格编辑处理
        /// <summary>
        /// 获取指定位置的单元格对像
        /// </summary>
        /// <param name="rowindex">行序号 base 0</param>
        /// <param name="columnindex">列序号 base 0</param>
        /// <returns>当前单元格对像</returns>
        public ReportCell GetReportCell(int rowindex, int columnindex)
        {
            if (rowindex >= 0 && rowindex < reportCells.Count)
            {
                if (columnindex >= 0 && columnindex < reportCells[rowindex].Count)
                    return reportCells[rowindex][columnindex];
            }

            return null;
        }
        /// <summary>
        /// 设置指定位置的单元格对像
        /// </summary>
        /// <param name="rowindex">行序号 base 0</param>
        /// <param name="columnindex">列序号 base 0</param>
        /// <param name="cellobj">待设置的单元格对像</param>
        public void SetReportCell(int rowindex, int columnindex, ReportCell cellobj)
        {
            if (rowindex >= 0 && rowindex < reportCells.Count)
            {
                if (columnindex >= 0 && columnindex < reportCells[rowindex].Count)
                {
                    reportCells[rowindex][columnindex] = cellobj;
                }
            }
        }
        /// <summary>
        /// 插入行单元格
        /// </summary>
        /// <param name="rowpos">插入行序号</param>
        /// <param name="defaultCell">插入的默认单元格对像</param>
        /// <returns>插入是否执行成功</returns>
        public bool InsertReportCellRow(int rowpos, ReportCell defaultCell)
        {
            int actpos = reportCells.Count;
            if (rowpos >= 0 && rowpos < actpos)
                actpos = rowpos;

            List<ReportCell> newRow = new List<ReportCell>();
            for (int c = 0; c < ColumnNumber; c++)
            {
                newRow.Add((ReportCell)defaultCell.Clone());
            }
            // 插入一行
            reportCells.Insert(actpos, newRow);
            // 相应插入该行的行高
            heights.Insert(actpos, DefaultRowHeight);
            // 行数增加1
            rows += 1;

            return true;
        }
        /// <summary>
        ///  删除指定行单元格
        /// </summary>
        /// <param name="rowpos">待删除的行序号</param>
        public void DeleteReportCellRow(int rowpos)
        {
            if (rowpos >= 0 && rowpos < RowNumber)
            {
                // 清除该行的所有列单元格
                reportCells[rowpos].Clear();
                // 删除行对像
                reportCells.RemoveAt(rowpos);
                // 删除行对应的行高
                heights.RemoveAt(rowpos);
                // 行数减1
                rows -= 1;
            }

        }
        /// <summary>
        /// 插入列单元格
        /// </summary>
        /// <param name="columnpos">插入列位置序号</param>
        /// <param name="defaultCell">待插入的默认单元格对像</param>
        /// <returns>插入列是否成功</returns>
        public bool InsertReportCellColumn(int columnpos, ReportCell defaultCell)
        {
            int actcolumn = ColumnNumber;
            if (columnpos >= 0 && columnpos < actcolumn)
                actcolumn = columnpos;

            for (int r = 0; r < RowNumber; r++)
            {
                reportCells[r].Insert(actcolumn, (ReportCell)defaultCell.Clone());
            }
            // 增加插入列的列宽
            widths.Insert(actcolumn, DefaultColumnWidth);
            // 列数加1
            columns += 1;

            return true;
        }
        /// <summary>
        /// 删除列单元格
        /// </summary>
        /// <param name="columnpos">待删除列序号</param>
        public void DeleteReportCellColumn(int columnpos)
        {
            if (columnpos >= 0 && columnpos < ColumnNumber)
            {
                for (int r = 0; r < RowNumber; r++)
                {
                    // 逐行删除指定位置的列
                    reportCells[r].RemoveAt(columnpos);
                }
                // 删除该列的列宽
                widths.RemoveAt(columnpos);
                // 列数减1
                columns -= 1;
            }
        }
        #endregion

        #region 报表内容加载与存储
        /// <summary>
        /// 从数据流加载报表文件内容
        /// </summary>
        /// <param name="mStream"></param>
        /// <returns></returns>
        public bool LoadReportFromStream(MemoryStream mStream)
        {
            if (mStream == null)
                return false;

            // 从已有的数据文件加载，则是打开已保存报表，默认无需保存
            bIsNewReport = false;
            bNeedSaveReport = false;

            if (mStream.Length > 0)
            {
                mStream.Position = 0;
                BinaryReader binReader = new BinaryReader(mStream);
                reportflag = binReader.ReadInt32();
                reportversion = binReader.ReadInt32();
                reporttype = binReader.ReadUInt32();

                int tmpQueryType = binReader.ReadInt32();
                if (Enum.IsDefined(typeof(ReportQueryType), tmpQueryType))
                    reportquerytype = (ReportQueryType)tmpQueryType;
                else
                    reportquerytype = ReportQueryType.ReportDay;

                rows = binReader.ReadInt32();
                columns = binReader.ReadInt32();
                overtimereport = binReader.ReadBoolean();

                // 再获取相应的内容修改默认单元格属性
                // 获取行高
                for (int r = 0; r < RowNumber; r++)
                {
                    int rh = binReader.ReadInt32();
                    RowHeights.Add(rh);
                }

                // 获取列宽
                for (int c = 0; c < ColumnNumber; c++)
                {
                    int cw = binReader.ReadInt32();
                    ColumnWidths.Add(cw);
                }


                // 读取报表流，获取报表文件对像，添加至单元格列表
                for (int r = 0; r < RowNumber; r++)
                {
                    // 创建列栅格集合并添加至行栅格列表中
                    List<ReportCell> colList = new List<ReportCell>();
                    reportCells.Add(colList);

                    for (int c = 0; c < ColumnNumber; c++)
                    {

                        // 获取当前单元格的大小
                        long cellsize = binReader.ReadInt64();
                        ReportCell cellobj = null;

                        if (cellsize != 0)
                        {
                            cellobj = new ReportCell(ReportType);
                            cellobj.LoadDataFromStream(mStream);
                        }
                        // 如果大小为0 ，则直接加入NULL对像
                        reportCells[r].Add(cellobj);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 将报表文件内容保存至数据流对像
        /// </summary>
        /// <param name="mStream"></param>
        /// <returns></returns>
        public bool SaveReportToStream(MemoryStream mStream)
        {

            if (mStream == null)
                return false;

            BinaryWriter binWriter = new BinaryWriter(mStream);
            binWriter.Write(reporttype);
            binWriter.Write(reportversion);
            binWriter.Write(reporttype);
            binWriter.Write((int)reportquerytype);
            binWriter.Write(rows);
            binWriter.Write(columns);
            binWriter.Write(overtimereport);

            // 写入行高
            foreach (int rh in RowHeights)
            {
                binWriter.Write(rh);
            }

            // 写入列宽
            foreach (int cw in ColumnWidths)
            {
                binWriter.Write(cw);
            }

            // 遍历当前的单元格列表，逐个写入
            foreach (List<ReportCell> rowCells in reportCells)
                foreach (ReportCell columnCell in rowCells)
                {
                    long cellsize = 0;
                    if (columnCell == null)         // null单元格对像
                        binWriter.Write(cellsize);
                    else                            // 非null单元格对像
                    {
                        // 获取当前单元格对像序列化后的stream流
                        MemoryStream cellstream = new MemoryStream();
                        // 将当前单元格对像序列化
                        columnCell.SaveDataToStream(cellstream);
                        // 获取当前序列化结果的长度
                        cellsize = cellstream.Length;
                        // 写入单元格流大小
                        binWriter.Write(cellsize);
                        // 将单元格流写入至报表文件流
                        cellstream.Position = 0;
                        cellstream.WriteTo(binWriter.BaseStream);
                        // 关闭当前临时流对像
                        cellstream.Close();

                    }
                }

            return true;
        }
        #endregion

        /// <summary>
        /// 检测当前报表文件是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsIllegalFile()
        {
            if (RowNumber == 0 || ColumnNumber == 0)
                return false;

            return true;

        }
    }

    /// <summary>
    /// 提供通用报表文件单元格结构定义
    /// </summary>
    [Serializable]
    public class ReportCell : ICloneable, ISerializable
    {
        // 增加静态常量NA，表示无效的数值
        public const double NA = -2147483648F;

        // 增加单元格对像的版本标识，便于未来的升级和扩展
        private uint cellVersion;

        #region 外观属性

        private Color backgroundColor;
        private Color foreColor;
        private Font cellFont;
        private int rowSpan;
        private int columnSpan;
        #endregion

        #region 数据查询属性
        private int formatType;
        private int digiNum;
        private int valueMultipicity;

        private CellDataProperty cellData;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="plugintype"></param>
        public ReportCell(uint plugintype)
        {

            cellVersion = ReportVersionSet.DEFAULTREPORTCELLVERSION;       // 设置当前单元格的版本

            // 默认边框不设置顶、左边框
            backgroundColor = Color.White;
            foreColor = Color.Black;
            cellFont = (Font)SystemFonts.DefaultFont.Clone();
            // 默认不跨行、不跨列
            rowSpan = 1;
            columnSpan = 1;

            formatType = 1;
            digiNum = 2;
            valueMultipicity = 1;

        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="srcCell"></param>
        public ReportCell(ReportCell srcCell)
        {
            cellVersion = srcCell.CellVersion;
            foreColor = srcCell.ForeColor;
            cellFont = (Font)srcCell.CellFont.Clone();

            rowSpan = srcCell.CellRowSpan;
            columnSpan = srcCell.CellColumnSpan;

            formatType = srcCell.CellFormatType;
            digiNum = srcCell.DigiNum;
            valueMultipicity = srcCell.ValueMultipicity;

            // 此处要实现CellDataProperty的Clone();
            CellData = (CellDataProperty)srcCell.CellData.Clone();
        }

        /// <summary>
        /// 用于实现序列化的构造函数-暂时无用到
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected ReportCell(SerializationInfo info, StreamingContext context)
        {
            // 版本
            this.cellVersion = info.GetUInt32("version");
            // 颜色
            this.backgroundColor = Color.FromArgb(info.GetInt32("bcolor"));
            this.foreColor = Color.FromArgb(info.GetInt32("fcolor"));
            // 字体  反序列化存在问题
            // this.cellFont = (Font)info.GetValue("font",this.cellFont.GetType());
            // 对齐
            // 行列
            this.rowSpan = info.GetInt32("row");
            this.columnSpan = info.GetInt32("column");
            // 数据查询属性
            this.formatType = info.GetInt32("format");
            this.digiNum = info.GetInt32("diginum");
            this.valueMultipicity = info.GetInt32("multi");

            // 数据对像  反序列化存在问题
            // this.cellData = (CellDataProperty)info.GetValue("celldata",this.cellData.GetType());  
        }

        #region 属性
        /// <summary>
        /// 报表单元格版本属性
        /// </summary>
        public uint CellVersion
        {
            get { return this.cellVersion; }
            set { this.cellVersion = value; }
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackgroudColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        /// <summary>
        /// 前景颜色
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        /// <summary>
        /// 显示文本对像属性
        /// </summary>
        public Font CellFont
        {
            get { return cellFont; }
            set { cellFont = value; }
        }
        /// <summary>
        /// 单元格跨行数目
        /// </summary>
        public int CellRowSpan
        {
            get { return rowSpan; }
            set { rowSpan = value; }
        }
        /// <summary>
        /// 单元格跨列数目
        /// </summary>
        public int CellColumnSpan
        {
            get { return columnSpan; }
            set { columnSpan = value; }
        }
        /// <summary>
        /// 单元格文本格式，整型，浮点, 字符串
        /// </summary>
        public int CellFormatType
        {
            get { return formatType; }
            set { formatType = value; }
        }
        /// <summary>
        /// 单元格文本显示小数位
        /// </summary>
        public int DigiNum
        {
            get { return digiNum; }
            set { digiNum = value; }
        }
        /// <summary>
        /// 单元格数值的缩放倍数 >0 放大，小于0 缩小 
        /// </summary>
        public int ValueMultipicity
        {
            get { return valueMultipicity; }
            set { valueMultipicity = value; }
        }

        public CellDataProperty CellData
        {
            get { return cellData; }
            set { cellData = value; }
        }

        #endregion

        /// <summary>
        /// 解析序列化二进制流加载单元格属性
        /// </summary>
        /// <param name="mStream">待解析二进制流</param>
        /// <returns></returns>
        public bool LoadDataFromStream(MemoryStream mStream)
        {
            return true;
        }

        /// <summary>
        /// 将当前单元格的成员属性转化存储为二进制流对像
        /// 自定义序列化过程可以便于控制格式存储
        /// </summary>
        /// <param name="mStream">待存储的二进制流对像</param>
        /// <returns></returns>
        public bool SaveDataToStream(MemoryStream mStream)
        {
            return true;
        }

        #region ICloneable 成员

        public object Clone()
        {
            return new ReportCell(this);
        }

        #endregion

        /// <summary>
        /// 序列化字体对像
        /// </summary>
        /// <param name="pObj">字体对像</param>
        /// <returns>保存对像数据的字节流</returns>
        public static byte[] SerializeFontObject(Font pObj)
        {
            if (pObj == null)
                return null;

            MemoryStream tmpStream = new MemoryStream();
            BinaryWriter binWriter = new BinaryWriter(tmpStream);
            binWriter.Write(pObj.FontFamily.Name);   // 字体名称
            binWriter.Write(pObj.Size);              // 字体大小
            binWriter.Write((int)pObj.Style);        // 样式
            binWriter.Write((int)pObj.Unit);         // 度量单位
            binWriter.Write(pObj.GdiCharSet);        // 字符集
            binWriter.Write(pObj.GdiVerticalFont);   // 是否继承自垂直字体

            tmpStream.Position = 0;
            byte[] resultbytes = new byte[tmpStream.Length];
            tmpStream.Read(resultbytes, 0, resultbytes.Length);

            tmpStream.Close();
            tmpStream.Dispose();

            return resultbytes;
        }

        /// <summary>
        /// 反序列化字体对像
        /// </summary>
        /// <param name="pBytes">对像的字节流</param>
        /// <returns>结果对像</returns>
        public static Font DeSerializeFontObject(byte[] pBytes)
        {
            Font newObj = SystemFonts.DefaultFont;
            if (pBytes != null)
            {
                MemoryStream tmpStream = new MemoryStream(pBytes);
                tmpStream.Position = 0;
                BinaryReader binReader = new BinaryReader(tmpStream);

                string fontName = binReader.ReadString();
                float fontSize = binReader.ReadSingle();

                FontStyle fontStyle = FontStyle.Regular;
                int aligntype = binReader.ReadInt32();
                try
                {
                    fontStyle = (FontStyle)aligntype;
                }
                catch 
                {
                }

                GraphicsUnit fontUnit = GraphicsUnit.Point;
                aligntype = binReader.ReadInt32();
                if (Enum.IsDefined(typeof(GraphicsUnit), aligntype))
                    fontUnit = (GraphicsUnit)aligntype;

                byte charSetType = binReader.ReadByte();
                bool verticalType = binReader.ReadBoolean();

                tmpStream.Close();
                tmpStream.Dispose();
                try
                {
                    newObj = new Font(fontName, fontSize, fontStyle, fontUnit, charSetType, verticalType);
                }
                catch 
                {

                }
            }

            return newObj;
        }

        #region ISerializable 成员
        /// <summary>
        /// 执行序列化函数-暂时无用到
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // 行列
            info.AddValue("row", this.rowSpan);
            info.AddValue("column", this.columnSpan);
            // 数据查询属性
            info.AddValue("format", this.formatType);
            info.AddValue("diginum", this.digiNum);
            info.AddValue("multi", this.valueMultipicity);
            // 数据对像 
            // info.AddValue("celldata", this.cellData, this.cellData.GetType());
        }

        #endregion

        /// <summary>
        /// 重置当前单元格对像的属性值为默认值
        /// </summary>
        internal void ResetCellContent()
        {
            // 单元格自身的属性无需执行重置

            // 调用单元格数据对像的重置函数进行重置
            cellData.ResetCellPropertyContent();
        }
    }

    /// <summary>
    /// 有关报表文件的版本设置集合
    /// </summary>
    class ReportVersionSet
    {
        public const uint DEFAULTREPORTVERSION = 3;            // 默认的报表文件的版本
        public const uint DEFAULTREPORTCELLVERSION = 1;        // 默认的报表单元格的版本
    }
}
