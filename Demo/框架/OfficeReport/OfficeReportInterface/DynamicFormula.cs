using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

namespace OfficeReportInterface
{
    /// <summary>
    /// 对excel模板中的动态公式进行处理
    /// </summary>
    public class DynamicFormula
    {
        private ExcelFileOperate excelFileOperate;

        public DynamicFormula(ExcelFileOperate excelFileOperate)
        {
            this.excelFileOperate = excelFileOperate;
        }

        /// <summary>
        /// 根据选择的时间跨度长度，进行公式填充。
        /// </summary>
        /// <param name="dataNum">数据长度</param>
        public void OperateFormulaRanges(int dataNum)
        {
            foreach (Excel.Worksheet sheet in excelFileOperate.ObjBook.Worksheets)
            {
                List<Excel.Range> datasourceLabelRangeList = GetLabelRangeList(sheet, "&&Formula");
                SetFormulaToCell(sheet, datasourceLabelRangeList, dataNum);
            }
        }

        /// <summary>
        /// 搜索含有关键字（标签）的单元格
        /// </summary>
        /// <param name="keyStr"></param>
        /// <returns></returns>
        private List<Excel.Range> GetLabelRangeList(Excel.Worksheet objSheet, params string[] keyStr)
        {
            List<Excel.Range> dataSourceLabelRangeList = new List<Excel.Range>();
            foreach (string keyString in keyStr)
            {
                Excel.Range item = (Excel.Range)objSheet.UsedRange.Find(keyString, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
                if (item == null)
                    continue;
                RecordDataSourceLabelRange(objSheet, ref dataSourceLabelRangeList, item);
            }
            return dataSourceLabelRangeList;
        }

        /// <summary>
        /// 记录所有的公式标签
        /// </summary>
        /// <param name="objSheet"></param>
        /// <param name="dataSourceLabelRangeList"></param>
        /// <param name="item"></param>
        private void RecordDataSourceLabelRange(Excel.Worksheet objSheet, ref List<Excel.Range> dataSourceLabelRangeList, Excel.Range item)
        {
            int firstRangeRow = item.Row;
            int firstRangeColumn = item.Column;
            do
            {
                dataSourceLabelRangeList.Add(item);
                item = objSheet.UsedRange.FindNext(item);
            }
            while (item.Column != firstRangeColumn || item.Row != firstRangeRow);
        }

        /// <summary>
        /// 遍历所有动态公式进行赋值
        /// </summary>
        /// <param name="objSheet"></param>
        /// <param name="dataSourceLabelRangeList"></param>
        /// <param name="dataNum"></param>
        private void SetFormulaToCell(Excel.Worksheet objSheet, List<Excel.Range> dataSourceLabelRangeList, int dataNum)
        {
            foreach (Excel.Range range in dataSourceLabelRangeList)
            {
                string formula = ParseFormula(range.Value2.ToString());
                bool isNeedInsert = FormulaNeedInsert(range.Value2.ToString());
                bool isColumn = FormulaDirectRow(range.Value2.ToString());

                //插入方式动态生成公式
                if (isNeedInsert)
                    InsertFormulaToCell(objSheet, range, dataNum, isColumn);
                else
                    OffsetFormulToCell(objSheet, range, dataNum, isColumn);
            }
        }

        /// <summary>
        /// 偏移方式插入公式
        /// </summary>
        /// <param name="objSheet"></param>
        /// <param name="range"></param>
        /// <param name="dataNum"></param>
        /// <param name="isColumn"></param>
        private void OffsetFormulToCell(Excel.Worksheet objSheet, Excel.Range range, int dataNum, bool isColumn)
        {
            string formula = ParseFormula(range.Value2.ToString());
            int row = range.Row;
            int column = range.Column;
            Excel.Range tempRange = objSheet.Cells[row, column] as Excel.Range;
            for (int i = 0; i < dataNum - 1; i++)
            {
                string offsetFormula = GetFormulaString(formula, isColumn, i);
                if (isColumn)
                    column++;
                else
                    row++;
                tempRange = objSheet.Cells[row, column] as Excel.Range; 
                tempRange.Value2 = "=" + offsetFormula;
            }
        }

        /// <summary>
        /// 插入方式填充公式
        /// </summary>
        /// <param name="objSheet"></param>
        /// <param name="range"></param>
        /// <param name="dataNum"></param>
        /// <param name="isColumn"></param>
        private void InsertFormulaToCell(Excel.Worksheet objSheet,Excel.Range range,int dataNum,bool isColumn)
        {
            string formula = ParseFormula(range.Value2.ToString());
            int row = range.Row;
            int column = range.Column;
            Excel.Range tempRange = objSheet.Cells[row, column] as Excel.Range; 
            for (int i = 0; i < dataNum - 1; i++)
            {
                tempRange = objSheet.Cells[row, column] as Excel.Range;
                tempRange.Value2 = "=" + formula;
                Excel.XlInsertShiftDirection directrion = Excel.XlInsertShiftDirection.xlShiftDown;
                if (isColumn)
                    directrion = Excel.XlInsertShiftDirection.xlShiftToRight;
                tempRange.Insert(directrion);
            }
            tempRange = objSheet.Cells[row, column] as Excel.Range; 
            tempRange.Value2 = "=" + formula; 
        }

        /// <summary>
        /// 根据单元格公式获取下一个单元格公式
        /// </summary>
        /// <param name="forMula"></param>
        /// <param name="IsColumn"></param>
        /// <param name="index"></param>
        private string GetFormulaString(string forMula, bool IsColumn, int index)
        {
            MatchCollection matchs = Regex.Matches(forMula, "[A-Za-z]+\\d+");//查找公式中的单元格名称，如A1,B2

            if (IsColumn)//对于公式计算，如果是关联整列，则行增加，关联的是整行，则列增加
            {
                for (int j = 0; j < matchs.Count; j++)
                {
                    string match = matchs[j].Value;
                    Match mathcCellNumber = Regex.Match(match, "\\d+");
                    int tempInt = 0;
                    if (int.TryParse(mathcCellNumber.Value, out tempInt))
                    {
                        tempInt += index;
                        string number = match.Substring(0, match.Length - tempInt.ToString().Length) + tempInt;
                        forMula = forMula.Replace(match, number);
                    }
                }
            }
            else
            {
                for (int j = 0; j < matchs.Count; j++)
                {
                    string match = matchs[j].Value;
                    string mathcColumnName = Regex.Match(match, "[A-Za-z]+").Value;
                    char[] cellColumnNameArray = mathcColumnName.ToCharArray();
                    if (cellColumnNameArray.Length > 2)
                        continue;
                    if (cellColumnNameArray.Length == 1)
                    {
                        char offsetColumnName = (char)(int)(cellColumnNameArray[0] + index);
                        string tempCellName = offsetColumnName.ToString();
                        if (offsetColumnName > 'Z')
                        {
                            int mod;
                            int div = Math.DivRem(offsetColumnName - 'Z', 26, out mod);
                            tempCellName = (char)('A' + div) + ((char)('A' + mod - 1)).ToString();
                        }
                        string number = tempCellName + match.Substring(1, match.Length - 1);
                        forMula = forMula.Replace(match, number);
                    }
                    else
                    {
                        char offsetColumnName = (char)(int)(cellColumnNameArray[1] + index);
                        string tempCellName = cellColumnNameArray[1] + offsetColumnName.ToString();
                        if (offsetColumnName > 'Z')
                        {
                            int mod;
                            int div = Math.DivRem(offsetColumnName - 'Z', 26, out mod);
                            tempCellName = (char)(cellColumnNameArray[0] + div + 1) + ((char)('A' + mod - 1)).ToString();
                        }
                        string number = tempCellName + match.Substring(2, match.Length - 2);
                        forMula = forMula.Replace(match, number);
                    }
                }
            }
            return forMula;
        }

        /// <summary>
        /// 解析公式
        /// </summary>uo
        /// <param name="formula"></param>
        /// <returns></returns>
        public string ParseFormula(string formula)
        {
            string result = "";
            string[] formulaArray = formula.Split(']');
            for (int i = 0; i < formulaArray.Length; i++)
            {
                if (formulaArray[i].Contains("Formula"))
                {
                    return formulaArray[i].Replace("&&Formula[", "");
                }
            }
            return result;
        }

        /// <summary>
        /// 行方向还是列方向扩展
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public bool FormulaDirectRow(string formula)
        {
            string[] formulaArray = formula.Split(']');
            for (int i = 0; i < formulaArray.Length; i++)
            {
                if (formulaArray[i].Contains("Direction"))
                {
                    if (formulaArray[i].Contains("row"))
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否需要通过插入方式写入
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public bool FormulaNeedInsert(string formula)
        {
            string[] formulaArray = formula.Split(']');
            for (int i = 0; i < formulaArray.Length; i++)
            {
                if (formulaArray[i].Contains("Insert"))
                    return true;
            }
            return false;
        }
    }
}
