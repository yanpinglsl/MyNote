using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Xml;//引入命名空间

namespace XMLDemo
{
    public partial class FrmReadXML : Form
    {
        public FrmReadXML()
        {
            InitializeComponent();
        }
        private void btnLoadXML_Click(object sender, EventArgs e)
        {
            XmlDocument objDoc = new XmlDocument();  //【1】创建XML文档操作对象
            objDoc.Load("StuScore.xml");  //【2】加载XML文件到文档对象中
            XmlNode rootNode = objDoc.DocumentElement;  //【3】获取XML文档根目录
            List<Student> list = new List<Student>();//创建对象集合         
            foreach (XmlNode stuNode in rootNode.ChildNodes)  //【4】遍历根节点（根节点包含所有节点）
            {
                if (stuNode.Name == "Student")
                {
                    Student objStu = new Student();
                    foreach (XmlNode subNode in stuNode)  //【5】遍历子节点
                    {
                        switch (subNode.Name)//根据子节点的名称封装到对象的属性
                        {
                            case "StuName":
                                objStu.StuName = subNode.InnerText;//获取《节点名称》对应的《节点值》
                                break;
                            case "StuAge":
                                objStu.StuAge = Convert.ToInt16(subNode.InnerText);
                                break;
                            case "Gender":
                                objStu.Gender = subNode.InnerText;
                                break;
                            case "ClassName":
                                objStu.ClassName = subNode.InnerText;
                                break;
                        }
                    }
                    list.Add(objStu);
                }
            }
            this.dgvStuList.DataSource = list;
        }
        //显示版本信息
        private void btnShowVersion_Click(object sender, EventArgs e)
        {
            //创建XML读取器
            XmlTextReader tReader = new XmlTextReader("StuScore.xml");

            string info = string.Empty;

            //循环查询
            while (tReader.Read())
            {
                if (tReader.Name == "Version")
                {
                    info = "版本：" + tReader.GetAttribute("vNo") + " 发布时间：" 
                        + tReader.GetAttribute("pTime");
                    break;
                }
            }
            MessageBox.Show(info,"数据版本");
        }
    }
}
