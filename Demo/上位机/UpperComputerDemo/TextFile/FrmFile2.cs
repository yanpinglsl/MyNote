using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//引入命名空间
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json;

namespace TextFile
{
    public partial class FrmFile2 : Form
    {
        public FrmFile2()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //封装对象信息
            Student objStu = new Student()
            {
                Name = this.txtName.Text.Trim(),
                Age = Convert.ToInt16(this.txtAge.Text.Trim()),
                Gender = this.txtGender.Text.Trim(),
                Birthday = Convert.ToDateTime(this.txtBirthday.Text.Trim())
            };
            //保存到文本文件
            FileStream fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "objStu.obj"), FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //一行一行写入文本
            sw.WriteLine(objStu.Name);
            sw.WriteLine(objStu.Age);
            sw.WriteLine(objStu.Gender);
            sw.WriteLine(objStu.Birthday.ToShortDateString());
            //关闭文件流和写入器
            sw.Close();
            fs.Close();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(Path.Combine(AppContext.BaseDirectory, "objStu.obj"), FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            //一行一行读取
            Student objStu = new Student()
            {
                Name = sr.ReadLine(),
                Age = Convert.ToInt16(sr.ReadLine()),
                Gender = sr.ReadLine(),
                Birthday = Convert.ToDateTime(sr.ReadLine())
            };
            sr.Close();
            fs.Close();
            this.txtName.Text = objStu.Name;
            this.txtAge.Text = objStu.Age.ToString();
            this.txtGender.Text = objStu.Gender;
            this.txtBirthday.Text = objStu.Birthday.ToShortDateString();
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            //封装对象信息
            Student objStu = new Student()
            {
                Name = this.txtName.Text.Trim(),
                Age = Convert.ToInt16(this.txtAge.Text.Trim()),
                Gender = this.txtGender.Text.Trim(),
                Birthday = Convert.ToDateTime(this.txtBirthday.Text.Trim())
            };
            ////【1】创建文件流
            //FileStream fs = new FileStream("objStu.stu", FileMode.Create);
            ////【2】创建二进制格式化器
            //BinaryFormatter formatter = new BinaryFormatter();
            ////【3】调用序列化方法
            //formatter.Serialize(fs, objStu);//已过时
            ////【4】关闭文件流
            //fs.Close();


            //【1】创建文件流
            FileStream fs = new FileStream("objStu.stu", FileMode.Create);
            //【2】创建写入器
            StreamWriter sw = new StreamWriter(fs);
            //【3】调用序列化方法
            string json = JsonConvert.SerializeObject(objStu);
            //【4】以流的方式写入数据
            sw.Write(json);
            //【5】关闭写入器
            sw.Close();
            //【6】关闭文件流
            fs.Close();
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            ////【1】创建文件流
            //FileStream fs = new FileStream("objStu.stu", FileMode.Open);
            ////【2】创建二进制格式化器
            //BinaryFormatter formatter = new BinaryFormatter();
            ////【3】调用序列化方法
            //Student objStu = (Student)formatter.Deserialize(fs);//已过时
            ////【4】关闭文件流
            //fs.Close();

            //【1】创建文件流
            FileStream fs = new FileStream("objStu.stu", FileMode.Open);

            //【2】创建读取器
            StreamReader sr = new StreamReader(fs);

            //【3】以流的方式读取数据
            string json = sr.ReadToEnd();
            //【3】调用序列化方法
           Student objStu =  JsonConvert.DeserializeObject<Student>(json);
            //【4】关闭读取器
            sr.Close();

            //【5】关闭文件流
            fs.Close();
            //显示对象属性
            this.txtName.Text = objStu.Name;
            this.txtAge.Text = objStu.Age.ToString();
            this.txtGender.Text = objStu.Gender;
            this.txtBirthday.Text = objStu.Birthday.ToShortDateString();

        }

    }
}
