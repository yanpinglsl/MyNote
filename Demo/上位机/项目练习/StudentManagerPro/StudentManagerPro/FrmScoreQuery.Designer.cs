namespace StudentManagerPro
{
    partial class FrmScoreQuery
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmScoreQuery));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            label1 = new Label();
            label2 = new Label();
            comboBox1 = new ComboBox();
            button1 = new Button();
            button12 = new Button();
            groupBox1 = new GroupBox();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            groupBox2 = new GroupBox();
            listBox1 = new ListBox();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(192, 0, 192);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(210, 41);
            label1.TabIndex = 0;
            label1.Text = "学员成绩管理";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 80);
            label2.Name = "label2";
            label2.Size = new Size(128, 17);
            label2.TabIndex = 1;
            label2.Text = "请选择要统计的班级：";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(170, 80);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(348, 25);
            comboBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageKey = "(无)";
            button1.Location = new Point(591, 69);
            button1.Name = "button1";
            button1.Size = new Size(172, 38);
            button1.TabIndex = 5;
            button1.Text = "统计全校考试信息";
            button1.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            button12.Image = (Image)resources.GetObject("button12.Image");
            button12.ImageAlign = ContentAlignment.MiddleLeft;
            button12.Location = new Point(947, 69);
            button12.Name = "button12";
            button12.Size = new Size(100, 38);
            button12.TabIndex = 6;
            button12.Text = "关闭窗口";
            button12.TextAlign = ContentAlignment.MiddleRight;
            button12.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox4);
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(818, 133);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(229, 260);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "考试成绩统计";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(105, 207);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(93, 23);
            textBox4.TabIndex = 2;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(105, 155);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(93, 23);
            textBox3.TabIndex = 2;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(105, 104);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(93, 23);
            textBox2.TabIndex = 2;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(105, 53);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(93, 23);
            textBox1.TabIndex = 2;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(19, 210);
            label6.Name = "label6";
            label6.Size = new Size(73, 17);
            label6.TabIndex = 3;
            label6.Text = "DB平均分：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(20, 158);
            label5.Name = "label5";
            label5.Size = new Size(72, 17);
            label5.TabIndex = 3;
            label5.Text = "C#平均分：";
            label5.TextAlign = ContentAlignment.TopRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 107);
            label4.Name = "label4";
            label4.Size = new Size(80, 17);
            label4.TabIndex = 3;
            label4.Text = "缺考总人数：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 56);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 3;
            label3.Text = "参考总人数：";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(listBox1);
            groupBox2.Location = new Point(818, 420);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(229, 260);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "缺考人员列表";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(33, 36);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(164, 191);
            listBox1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeight = 30;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6 });
            dataGridView1.Location = new Point(12, 133);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(762, 547);
            dataGridView1.TabIndex = 9;
            // 
            // Column1
            // 
            Column1.HeaderText = "学号";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 150;
            // 
            // Column2
            // 
            Column2.HeaderText = "姓名";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 150;
            // 
            // Column3
            // 
            Column3.HeaderText = "性别";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 80;
            // 
            // Column4
            // 
            Column4.HeaderText = "班级";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 150;
            // 
            // Column5
            // 
            Column5.HeaderText = "C#成绩";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            // 
            // Column6
            // 
            Column6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column6.HeaderText = "DB成绩";
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            // 
            // FrmScoreQuery
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1063, 692);
            Controls.Add(dataGridView1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(button12);
            Controls.Add(button1);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmScoreQuery";
            Text = "FrmScoreQuery";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private ComboBox comboBox1;
        private Button button1;
        private Button button12;
        private GroupBox groupBox1;
        private TextBox textBox1;
        private Label label3;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private Label label6;
        private Label label5;
        private Label label4;
        private GroupBox groupBox2;
        private ListBox listBox1;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
    }
}