namespace TextFile
{
    partial class FrmFile2
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFile2));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label5 = new Label();
            txtName = new TextBox();
            txtGender = new TextBox();
            txtAge = new TextBox();
            txtBirthday = new TextBox();
            btnSave = new Button();
            btnRead = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            btnSerialize = new Button();
            btnDeserialize = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 20);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(44, 17);
            label1.TabIndex = 0;
            label1.Text = "姓名：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 64);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 0;
            label2.Text = "性别：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(248, 24);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 0;
            label3.Text = "年龄：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(220, 61);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(68, 17);
            label5.TabIndex = 0;
            label5.Text = "出生日期：";
            // 
            // txtName
            // 
            txtName.Location = new Point(83, 17);
            txtName.Margin = new Padding(4, 4, 4, 4);
            txtName.Name = "txtName";
            txtName.Size = new Size(116, 23);
            txtName.TabIndex = 0;
            // 
            // txtGender
            // 
            txtGender.Location = new Point(83, 60);
            txtGender.Margin = new Padding(4, 4, 4, 4);
            txtGender.Name = "txtGender";
            txtGender.Size = new Size(116, 23);
            txtGender.TabIndex = 2;
            // 
            // txtAge
            // 
            txtAge.Location = new Point(304, 20);
            txtAge.Margin = new Padding(4, 4, 4, 4);
            txtAge.Name = "txtAge";
            txtAge.Size = new Size(116, 23);
            txtAge.TabIndex = 1;
            // 
            // txtBirthday
            // 
            txtBirthday.Location = new Point(304, 57);
            txtBirthday.Margin = new Padding(4, 4, 4, 4);
            txtBirthday.Name = "txtBirthday";
            txtBirthday.Size = new Size(116, 23);
            txtBirthday.TabIndex = 3;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(54, 33);
            btnSave.Margin = new Padding(4, 4, 4, 4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(117, 33);
            btnSave.TabIndex = 0;
            btnSave.Text = "保存到文本文件";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnRead
            // 
            btnRead.Location = new Point(222, 33);
            btnRead.Margin = new Padding(4, 4, 4, 4);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(117, 33);
            btnRead.TabIndex = 1;
            btnRead.Text = "从文本文件读取信息";
            btnRead.UseVisualStyleBackColor = true;
            btnRead.Click += btnRead_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(btnRead);
            groupBox1.Location = new Point(29, 115);
            groupBox1.Margin = new Padding(4, 4, 4, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 4, 4, 4);
            groupBox1.Size = new Size(392, 92);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "使用文本保存对象状态";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnSerialize);
            groupBox2.Controls.Add(btnDeserialize);
            groupBox2.Location = new Point(27, 215);
            groupBox2.Margin = new Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4);
            groupBox2.Size = new Size(392, 92);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "使用序列化存取对象";
            // 
            // btnSerialize
            // 
            btnSerialize.Location = new Point(54, 33);
            btnSerialize.Margin = new Padding(4);
            btnSerialize.Name = "btnSerialize";
            btnSerialize.Size = new Size(117, 33);
            btnSerialize.TabIndex = 0;
            btnSerialize.Text = "序列化对象";
            btnSerialize.UseVisualStyleBackColor = true;
            btnSerialize.Click += btnSerialize_Click;
            // 
            // btnDeserialize
            // 
            btnDeserialize.Location = new Point(222, 33);
            btnDeserialize.Margin = new Padding(4);
            btnDeserialize.Name = "btnDeserialize";
            btnDeserialize.Size = new Size(117, 33);
            btnDeserialize.TabIndex = 1;
            btnDeserialize.Text = "反序列化对象";
            btnDeserialize.UseVisualStyleBackColor = true;
            btnDeserialize.Click += btnDeserialize_Click;
            // 
            // FrmFile2
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 319);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(txtBirthday);
            Controls.Add(txtAge);
            Controls.Add(txtGender);
            Controls.Add(txtName);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 4, 4, 4);
            Name = "FrmFile2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "对象状态的保存";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtGender;
        private System.Windows.Forms.TextBox txtAge;
        private System.Windows.Forms.TextBox txtBirthday;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btnSerialize;
        private Button btnDeserialize;
    }
}

