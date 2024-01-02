namespace XMLDemo
{
    partial class FrmReadXML
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReadXML));
            this.dgvStuList = new System.Windows.Forms.DataGridView();
            this.btnLoadXML = new System.Windows.Forms.Button();
            this.btnShowVersion = new System.Windows.Forms.Button();
            this.StuName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StuAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStuList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvStuList
            // 
            this.dgvStuList.AllowUserToAddRows = false;
            this.dgvStuList.AllowUserToDeleteRows = false;
            this.dgvStuList.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvStuList.ColumnHeadersHeight = 25;
            this.dgvStuList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StuName,
            this.StuAge,
            this.Gender,
            this.ClassName});
            this.dgvStuList.GridColor = System.Drawing.Color.Blue;
            this.dgvStuList.Location = new System.Drawing.Point(12, 12);
            this.dgvStuList.Name = "dgvStuList";
            this.dgvStuList.ReadOnly = true;
            this.dgvStuList.RowTemplate.Height = 23;
            this.dgvStuList.Size = new System.Drawing.Size(362, 187);
            this.dgvStuList.TabIndex = 0;
            // 
            // btnLoadXML
            // 
            this.btnLoadXML.Location = new System.Drawing.Point(257, 205);
            this.btnLoadXML.Name = "btnLoadXML";
            this.btnLoadXML.Size = new System.Drawing.Size(117, 23);
            this.btnLoadXML.TabIndex = 1;
            this.btnLoadXML.Text = "加载XML文件";
            this.btnLoadXML.UseVisualStyleBackColor = true;
            this.btnLoadXML.Click += new System.EventHandler(this.btnLoadXML_Click);
            // 
            // btnShowVersion
            // 
            this.btnShowVersion.Location = new System.Drawing.Point(13, 204);
            this.btnShowVersion.Name = "btnShowVersion";
            this.btnShowVersion.Size = new System.Drawing.Size(104, 23);
            this.btnShowVersion.TabIndex = 2;
            this.btnShowVersion.Text = "显示版本信息";
            this.btnShowVersion.UseVisualStyleBackColor = true;
            this.btnShowVersion.Click += new System.EventHandler(this.btnShowVersion_Click);
            // 
            // StuName
            // 
            this.StuName.DataPropertyName = "StuName";
            this.StuName.HeaderText = "姓名";
            this.StuName.Name = "StuName";
            this.StuName.ReadOnly = true;
            // 
            // StuAge
            // 
            this.StuAge.DataPropertyName = "StuAge";
            this.StuAge.HeaderText = "年龄";
            this.StuAge.Name = "StuAge";
            this.StuAge.ReadOnly = true;
            this.StuAge.Width = 60;
            // 
            // Gender
            // 
            this.Gender.DataPropertyName = "Gender";
            this.Gender.HeaderText = "性别";
            this.Gender.Name = "Gender";
            this.Gender.ReadOnly = true;
            this.Gender.Width = 40;
            // 
            // ClassName
            // 
            this.ClassName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ClassName.DataPropertyName = "ClassName";
            this.ClassName.HeaderText = "班级";
            this.ClassName.Name = "ClassName";
            this.ClassName.ReadOnly = true;
            // 
            // FrmReadXML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 237);
            this.Controls.Add(this.btnShowVersion);
            this.Controls.Add(this.btnLoadXML);
            this.Controls.Add(this.dgvStuList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmReadXML";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XML数据读取示例";
            ((System.ComponentModel.ISupportInitialize)(this.dgvStuList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStuList;
        private System.Windows.Forms.Button btnLoadXML;
        private System.Windows.Forms.Button btnShowVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn StuName;
        private System.Windows.Forms.DataGridViewTextBoxColumn StuAge;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClassName;
    }
}

