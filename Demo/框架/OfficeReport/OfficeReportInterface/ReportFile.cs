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
    /// �ṩͨ�ñ����ļ�����ṹ����
    /// </summary>
    public class CommonReportFile
    {
        public const int DefaultRows = 50;
        public const int DefaultColumns = 50;
        // ���������ļ����������������
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

        // �����������豣���������ļ���
        private string reportname;
        private bool bIsNewReport;      // ��ʶ�½���δ���汨��ʹ򿪵��ѱ��汨��
        private bool bNeedSaveReport;   // ��ʶ��ǰ������Ƿ���Ҫ����
        private ReportNode relatedReportNode;    // ��ǰ�����ļ������ı����ļ��ڵ�


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
        /// ��ȡ��Ԫ�񼯺�
        /// </summary>
        public List<List<ReportCell>> ReportCellSet
        {
            get { return reportCells; }
        }

        /// <summary>
        /// ��ȡ��ǰ����Ԫ����������
        /// </summary>
        /// <param name="row">�����</param>
        /// <param name="column">�����</param>
        /// <returns></returns>
        public ReportCell this[int row, int column]
        {
            get
            {
                // ������Χ�Ϸ�
                if (row >= 0 && row < RowNumber && column >= 0 && column < ColumnNumber)
                {
                    return reportCells[row][column];
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Ĭ�Ϲ��캯��������Pecstarϵͳ�µĿհױ���
        /// </summary>
        public CommonReportFile()
            : this(null, ReportNodeType.REPORTPECSTAR)
        {

        }

        /// <summary>
        /// ����ָ��ϵͳ���͵Ŀհױ���
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
                this.reportname = "δ����";
            }
            else
            {
                this.reporttype = relatedReportNode.NodeType;           // ���ݸñ����������Ե��ò�ͬ���������Ԫ�����
                this.bIsNewReport = false;
                this.bNeedSaveReport = false;
                this.reportname = relatedReportNode.NodeName;
            }

            // Ĭ������Ϊ�ձ�
            reportquerytype = ReportQueryType.ReportDay;

            // ����Ĭ�ϵ�������
            this.rows = 0;
            this.columns = 0;

            this.widths = new List<int>();
            this.heights = new List<int>();
            this.reportCells = new List<List<ReportCell>>();

            // ����������Ľڵ�
            this.relatedReportNode = relatedReportNode;

            // Ĭ�ϲ����г�ʼ��
        }

        /// <summary>
        /// ����ָ����������Ĭ�ϱ����ļ�
        /// ��Ҫ�����½�Ĭ�ϵı����ļ�
        /// </summary>
        /// <param name="relatedReportNode">�����ļ������ı���ڵ� null��ʾ�½�����</param>
        /// <param name="reportFileType">�����ļ�����</param>
        /// <param name="rownumber">����</param>
        /// <param name="columnnumber">����</param>
        public CommonReportFile(ReportNode relatedReportNode, uint reportFileType, int rownumber, int columnnumber)
        {
            this.reportflag = 1;
            this.reportversion = 3;

            if (relatedReportNode == null)
            {
                this.reporttype = reportFileType;  // ��reportFileTypeָ����������;
                this.bIsNewReport = true;
                this.bNeedSaveReport = true;
            }
            else
            {
                this.reporttype = relatedReportNode.NodeType;   // ���ݸñ����������Ե��ò�ͬ���������Ԫ�����
                this.bIsNewReport = false;
                this.bNeedSaveReport = false;
            }

            this.reportname = "δ����";
            // Ĭ������Ϊ�ձ�
            reportquerytype = ReportQueryType.ReportDay;

            // ����Ĭ�ϵ�������
            this.rows = rownumber;
            this.columns = columnnumber;

            this.widths = new List<int>();
            this.heights = new List<int>();
            this.reportCells = new List<List<ReportCell>>();

            // ����������Ľڵ�
            this.relatedReportNode = relatedReportNode;

            // ��ʼ����Ӧ�������ĵ�Ԫ��
            InitializeReportCells();
        }

        /// <summary>
        /// ���ݱ������ͺ͵�ǰ���С���������Ĭ�ϵĵ�Ԫ���б�
        /// ͬʱ����Ĭ�ϵ��иߺ��п�
        /// ��Ҫ���ڴ��Ѿ����ڵı����ļ�
        /// </summary>
        /// <returns></returns>
        private bool InitializeReportCells()
        {
            // ��������������������Ӧ�ĵ�Ԫ���б�
            for (int r = 0; r < rows; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < columns; c++)
                {
                    // ������Ԫ�����,���ݱ������ʹ�����ͬϵͳ�µı���Ԫ��
                    ReportCell cell = new ReportCell(reporttype);
                    // ���м��뵥Ԫ�����б���
                    cellColumnList.Add(cell);
                }
                // ���м��뵥Ԫ�����б����
                reportCells.Add(cellColumnList);
                // ��������Ĭ���и�
                heights.Add(DefaultRowHeight);
            }

            // ���������п�
            for (int c = 0; c < columns; c++)
                widths.Add(DefaultColumnWidth);

            return true;

        }

        /// <summary>
        /// �����ⲿ��ʼ����Ԫ��
        /// </summary>
        /// <param name="columnWidth">ָ���п�</param>
        /// <param name="rowHeight">ָ���и�</param>
        /// <returns></returns>
        public bool InitializeReportCells(int columnWidth, int rowHeight)
        {
            // ��������������������Ӧ�ĵ�Ԫ���б�
            for (int r = 0; r < rows; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < columns; c++)
                {
                    // ������Ԫ�����,���ݱ������ʹ�����ͬϵͳ�µı���Ԫ��
                    ReportCell cell = new ReportCell(reporttype);
                    // ���м��뵥Ԫ�����б���
                    cellColumnList.Add(cell);
                }
                // ���м��뵥Ԫ�����б����
                reportCells.Add(cellColumnList);
                // ��������Ĭ���и�
                heights.Add(rowHeight);
            }

            // ���������п�
            for (int c = 0; c < columns; c++)
                widths.Add(columnWidth);

            return true;
        }

        /// <summary>
        /// ����ģ�屨���ʼ����������Ԫ��-ֻ��Է�������
        /// </summary>
        /// <param name="rowNum">����������</param>
        /// <param name="rowIndex">ģ�屨�������ڴ��������еĲ�㵥Ԫ��������</param>
        /// <param name="reportFile">ģ�屨���ļ�</param>
        /// <returns></returns>
        public bool InitializeReportCellsByReportFile(int rowNum, int rowIndex, CommonReportFile reportFile)
        {
            // ����ģ�屨������Ԫ��
            for (int r = 0; r < rowIndex; r++)
            {
                List<ReportCell> cellColumnList = new List<ReportCell>();
                for (int c = 0; c < reportFile.ColumnNumber; c++)
                {
                    ReportCell cellItem = reportFile.GetReportCell(r, c);
                    cellColumnList.Add(cellItem);
                }
                // ��ģ�屨���ͷ��Ϣ��������
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

        #region ����Ԫ��༭����
        /// <summary>
        /// ��ȡָ��λ�õĵ�Ԫ�����
        /// </summary>
        /// <param name="rowindex">����� base 0</param>
        /// <param name="columnindex">����� base 0</param>
        /// <returns>��ǰ��Ԫ�����</returns>
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
        /// ����ָ��λ�õĵ�Ԫ�����
        /// </summary>
        /// <param name="rowindex">����� base 0</param>
        /// <param name="columnindex">����� base 0</param>
        /// <param name="cellobj">�����õĵ�Ԫ�����</param>
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
        /// �����е�Ԫ��
        /// </summary>
        /// <param name="rowpos">���������</param>
        /// <param name="defaultCell">�����Ĭ�ϵ�Ԫ�����</param>
        /// <returns>�����Ƿ�ִ�гɹ�</returns>
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
            // ����һ��
            reportCells.Insert(actpos, newRow);
            // ��Ӧ������е��и�
            heights.Insert(actpos, DefaultRowHeight);
            // ��������1
            rows += 1;

            return true;
        }
        /// <summary>
        ///  ɾ��ָ���е�Ԫ��
        /// </summary>
        /// <param name="rowpos">��ɾ���������</param>
        public void DeleteReportCellRow(int rowpos)
        {
            if (rowpos >= 0 && rowpos < RowNumber)
            {
                // ������е������е�Ԫ��
                reportCells[rowpos].Clear();
                // ɾ���ж���
                reportCells.RemoveAt(rowpos);
                // ɾ���ж�Ӧ���и�
                heights.RemoveAt(rowpos);
                // ������1
                rows -= 1;
            }

        }
        /// <summary>
        /// �����е�Ԫ��
        /// </summary>
        /// <param name="columnpos">������λ�����</param>
        /// <param name="defaultCell">�������Ĭ�ϵ�Ԫ�����</param>
        /// <returns>�������Ƿ�ɹ�</returns>
        public bool InsertReportCellColumn(int columnpos, ReportCell defaultCell)
        {
            int actcolumn = ColumnNumber;
            if (columnpos >= 0 && columnpos < actcolumn)
                actcolumn = columnpos;

            for (int r = 0; r < RowNumber; r++)
            {
                reportCells[r].Insert(actcolumn, (ReportCell)defaultCell.Clone());
            }
            // ���Ӳ����е��п�
            widths.Insert(actcolumn, DefaultColumnWidth);
            // ������1
            columns += 1;

            return true;
        }
        /// <summary>
        /// ɾ���е�Ԫ��
        /// </summary>
        /// <param name="columnpos">��ɾ�������</param>
        public void DeleteReportCellColumn(int columnpos)
        {
            if (columnpos >= 0 && columnpos < ColumnNumber)
            {
                for (int r = 0; r < RowNumber; r++)
                {
                    // ����ɾ��ָ��λ�õ���
                    reportCells[r].RemoveAt(columnpos);
                }
                // ɾ�����е��п�
                widths.RemoveAt(columnpos);
                // ������1
                columns -= 1;
            }
        }
        #endregion

        #region �������ݼ�����洢
        /// <summary>
        /// �����������ر����ļ�����
        /// </summary>
        /// <param name="mStream"></param>
        /// <returns></returns>
        public bool LoadReportFromStream(MemoryStream mStream)
        {
            if (mStream == null)
                return false;

            // �����е������ļ����أ����Ǵ��ѱ��汨��Ĭ�����豣��
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

                // �ٻ�ȡ��Ӧ�������޸�Ĭ�ϵ�Ԫ������
                // ��ȡ�и�
                for (int r = 0; r < RowNumber; r++)
                {
                    int rh = binReader.ReadInt32();
                    RowHeights.Add(rh);
                }

                // ��ȡ�п�
                for (int c = 0; c < ColumnNumber; c++)
                {
                    int cw = binReader.ReadInt32();
                    ColumnWidths.Add(cw);
                }


                // ��ȡ����������ȡ�����ļ������������Ԫ���б�
                for (int r = 0; r < RowNumber; r++)
                {
                    // ������դ�񼯺ϲ��������դ���б���
                    List<ReportCell> colList = new List<ReportCell>();
                    reportCells.Add(colList);

                    for (int c = 0; c < ColumnNumber; c++)
                    {

                        // ��ȡ��ǰ��Ԫ��Ĵ�С
                        long cellsize = binReader.ReadInt64();
                        ReportCell cellobj = null;

                        if (cellsize != 0)
                        {
                            cellobj = new ReportCell(ReportType);
                            cellobj.LoadDataFromStream(mStream);
                        }
                        // �����СΪ0 ����ֱ�Ӽ���NULL����
                        reportCells[r].Add(cellobj);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// �������ļ����ݱ���������������
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

            // д���и�
            foreach (int rh in RowHeights)
            {
                binWriter.Write(rh);
            }

            // д���п�
            foreach (int cw in ColumnWidths)
            {
                binWriter.Write(cw);
            }

            // ������ǰ�ĵ�Ԫ���б����д��
            foreach (List<ReportCell> rowCells in reportCells)
                foreach (ReportCell columnCell in rowCells)
                {
                    long cellsize = 0;
                    if (columnCell == null)         // null��Ԫ�����
                        binWriter.Write(cellsize);
                    else                            // ��null��Ԫ�����
                    {
                        // ��ȡ��ǰ��Ԫ��������л����stream��
                        MemoryStream cellstream = new MemoryStream();
                        // ����ǰ��Ԫ��������л�
                        columnCell.SaveDataToStream(cellstream);
                        // ��ȡ��ǰ���л�����ĳ���
                        cellsize = cellstream.Length;
                        // д�뵥Ԫ������С
                        binWriter.Write(cellsize);
                        // ����Ԫ����д���������ļ���
                        cellstream.Position = 0;
                        cellstream.WriteTo(binWriter.BaseStream);
                        // �رյ�ǰ��ʱ������
                        cellstream.Close();

                    }
                }

            return true;
        }
        #endregion

        /// <summary>
        /// ��⵱ǰ�����ļ��Ƿ�Ϸ�
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
    /// �ṩͨ�ñ����ļ���Ԫ��ṹ����
    /// </summary>
    [Serializable]
    public class ReportCell : ICloneable, ISerializable
    {
        // ���Ӿ�̬����NA����ʾ��Ч����ֵ
        public const double NA = -2147483648F;

        // ���ӵ�Ԫ�����İ汾��ʶ������δ������������չ
        private uint cellVersion;

        #region �������

        private Color backgroundColor;
        private Color foreColor;
        private Font cellFont;
        private int rowSpan;
        private int columnSpan;
        #endregion

        #region ���ݲ�ѯ����
        private int formatType;
        private int digiNum;
        private int valueMultipicity;

        private CellDataProperty cellData;

        #endregion

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="plugintype"></param>
        public ReportCell(uint plugintype)
        {

            cellVersion = ReportVersionSet.DEFAULTREPORTCELLVERSION;       // ���õ�ǰ��Ԫ��İ汾

            // Ĭ�ϱ߿����ö�����߿�
            backgroundColor = Color.White;
            foreColor = Color.Black;
            cellFont = (Font)SystemFonts.DefaultFont.Clone();
            // Ĭ�ϲ����С�������
            rowSpan = 1;
            columnSpan = 1;

            formatType = 1;
            digiNum = 2;
            valueMultipicity = 1;

        }

        /// <summary>
        /// �������캯��
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

            // �˴�Ҫʵ��CellDataProperty��Clone();
            CellData = (CellDataProperty)srcCell.CellData.Clone();
        }

        /// <summary>
        /// ����ʵ�����л��Ĺ��캯��-��ʱ���õ�
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected ReportCell(SerializationInfo info, StreamingContext context)
        {
            // �汾
            this.cellVersion = info.GetUInt32("version");
            // ��ɫ
            this.backgroundColor = Color.FromArgb(info.GetInt32("bcolor"));
            this.foreColor = Color.FromArgb(info.GetInt32("fcolor"));
            // ����  �����л���������
            // this.cellFont = (Font)info.GetValue("font",this.cellFont.GetType());
            // ����
            // ����
            this.rowSpan = info.GetInt32("row");
            this.columnSpan = info.GetInt32("column");
            // ���ݲ�ѯ����
            this.formatType = info.GetInt32("format");
            this.digiNum = info.GetInt32("diginum");
            this.valueMultipicity = info.GetInt32("multi");

            // ���ݶ���  �����л���������
            // this.cellData = (CellDataProperty)info.GetValue("celldata",this.cellData.GetType());  
        }

        #region ����
        /// <summary>
        /// ����Ԫ��汾����
        /// </summary>
        public uint CellVersion
        {
            get { return this.cellVersion; }
            set { this.cellVersion = value; }
        }

        /// <summary>
        /// ������ɫ
        /// </summary>
        public Color BackgroudColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        /// <summary>
        /// ǰ����ɫ
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        /// <summary>
        /// ��ʾ�ı���������
        /// </summary>
        public Font CellFont
        {
            get { return cellFont; }
            set { cellFont = value; }
        }
        /// <summary>
        /// ��Ԫ�������Ŀ
        /// </summary>
        public int CellRowSpan
        {
            get { return rowSpan; }
            set { rowSpan = value; }
        }
        /// <summary>
        /// ��Ԫ�������Ŀ
        /// </summary>
        public int CellColumnSpan
        {
            get { return columnSpan; }
            set { columnSpan = value; }
        }
        /// <summary>
        /// ��Ԫ���ı���ʽ�����ͣ�����, �ַ���
        /// </summary>
        public int CellFormatType
        {
            get { return formatType; }
            set { formatType = value; }
        }
        /// <summary>
        /// ��Ԫ���ı���ʾС��λ
        /// </summary>
        public int DigiNum
        {
            get { return digiNum; }
            set { digiNum = value; }
        }
        /// <summary>
        /// ��Ԫ����ֵ�����ű��� >0 �Ŵ�С��0 ��С 
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
        /// �������л������������ص�Ԫ������
        /// </summary>
        /// <param name="mStream">��������������</param>
        /// <returns></returns>
        public bool LoadDataFromStream(MemoryStream mStream)
        {
            return true;
        }

        /// <summary>
        /// ����ǰ��Ԫ��ĳ�Ա����ת���洢Ϊ������������
        /// �Զ������л����̿��Ա��ڿ��Ƹ�ʽ�洢
        /// </summary>
        /// <param name="mStream">���洢�Ķ�����������</param>
        /// <returns></returns>
        public bool SaveDataToStream(MemoryStream mStream)
        {
            return true;
        }

        #region ICloneable ��Ա

        public object Clone()
        {
            return new ReportCell(this);
        }

        #endregion

        /// <summary>
        /// ���л��������
        /// </summary>
        /// <param name="pObj">�������</param>
        /// <returns>����������ݵ��ֽ���</returns>
        public static byte[] SerializeFontObject(Font pObj)
        {
            if (pObj == null)
                return null;

            MemoryStream tmpStream = new MemoryStream();
            BinaryWriter binWriter = new BinaryWriter(tmpStream);
            binWriter.Write(pObj.FontFamily.Name);   // ��������
            binWriter.Write(pObj.Size);              // �����С
            binWriter.Write((int)pObj.Style);        // ��ʽ
            binWriter.Write((int)pObj.Unit);         // ������λ
            binWriter.Write(pObj.GdiCharSet);        // �ַ���
            binWriter.Write(pObj.GdiVerticalFont);   // �Ƿ�̳��Դ�ֱ����

            tmpStream.Position = 0;
            byte[] resultbytes = new byte[tmpStream.Length];
            tmpStream.Read(resultbytes, 0, resultbytes.Length);

            tmpStream.Close();
            tmpStream.Dispose();

            return resultbytes;
        }

        /// <summary>
        /// �����л��������
        /// </summary>
        /// <param name="pBytes">������ֽ���</param>
        /// <returns>�������</returns>
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

        #region ISerializable ��Ա
        /// <summary>
        /// ִ�����л�����-��ʱ���õ�
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // ����
            info.AddValue("row", this.rowSpan);
            info.AddValue("column", this.columnSpan);
            // ���ݲ�ѯ����
            info.AddValue("format", this.formatType);
            info.AddValue("diginum", this.digiNum);
            info.AddValue("multi", this.valueMultipicity);
            // ���ݶ��� 
            // info.AddValue("celldata", this.cellData, this.cellData.GetType());
        }

        #endregion

        /// <summary>
        /// ���õ�ǰ��Ԫ����������ֵΪĬ��ֵ
        /// </summary>
        internal void ResetCellContent()
        {
            // ��Ԫ���������������ִ������

            // ���õ�Ԫ�����ݶ�������ú�����������
            cellData.ResetCellPropertyContent();
        }
    }

    /// <summary>
    /// �йر����ļ��İ汾���ü���
    /// </summary>
    class ReportVersionSet
    {
        public const uint DEFAULTREPORTVERSION = 3;            // Ĭ�ϵı����ļ��İ汾
        public const uint DEFAULTREPORTCELLVERSION = 1;        // Ĭ�ϵı���Ԫ��İ汾
    }
}
