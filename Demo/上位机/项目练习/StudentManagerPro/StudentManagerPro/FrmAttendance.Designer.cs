namespace StudentManagerPro
{
    partial class FrmAttendance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAttendance));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            button6 = new Button();
            pictureBox1 = new PictureBox();
            groupBox1 = new GroupBox();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            label18 = new Label();
            label17 = new Label();
            label19 = new Label();
            label20 = new Label();
            label21 = new Label();
            label22 = new Label();
            label23 = new Label();
            textBox1 = new TextBox();
            label24 = new Label();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(192, 0, 192);
            label1.ImageAlign = ContentAlignment.MiddleLeft;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(266, 41);
            label1.TabIndex = 2;
            label1.Text = "考勤打卡进行中...";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Black;
            label2.BorderStyle = BorderStyle.Fixed3D;
            label2.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(12, 72);
            label2.Name = "label2";
            label2.Size = new Size(76, 33);
            label2.TabIndex = 3;
            label2.Text = "0000";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(94, 73);
            label3.Name = "label3";
            label3.Size = new Size(38, 31);
            label3.TabIndex = 4;
            label3.Text = "年";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Black;
            label4.BorderStyle = BorderStyle.Fixed3D;
            label4.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.Red;
            label4.Location = new Point(138, 72);
            label4.Name = "label4";
            label4.Size = new Size(46, 33);
            label4.TabIndex = 3;
            label4.Text = "00";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label5.Location = new Point(190, 73);
            label5.Name = "label5";
            label5.Size = new Size(38, 31);
            label5.TabIndex = 4;
            label5.Text = "月";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Black;
            label6.BorderStyle = BorderStyle.Fixed3D;
            label6.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label6.ForeColor = Color.Red;
            label6.Location = new Point(234, 72);
            label6.Name = "label6";
            label6.Size = new Size(46, 33);
            label6.TabIndex = 3;
            label6.Text = "00";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label7.Location = new Point(286, 73);
            label7.Name = "label7";
            label7.Size = new Size(38, 31);
            label7.TabIndex = 4;
            label7.Text = "日";
            label7.Click += label7_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Black;
            label8.BorderStyle = BorderStyle.Fixed3D;
            label8.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label8.ForeColor = Color.Red;
            label8.Location = new Point(330, 72);
            label8.Name = "label8";
            label8.Size = new Size(120, 33);
            label8.TabIndex = 3;
            label8.Text = "00:00 00";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Black;
            label9.BorderStyle = BorderStyle.Fixed3D;
            label9.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label9.ForeColor = Color.Red;
            label9.Location = new Point(524, 72);
            label9.Name = "label9";
            label9.Size = new Size(31, 33);
            label9.TabIndex = 3;
            label9.Text = "0";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label10.Location = new Point(456, 73);
            label10.Name = "label10";
            label10.Size = new Size(62, 31);
            label10.TabIndex = 4;
            label10.Text = "星期";
            // 
            // button6
            // 
            button6.Image = (Image)resources.GetObject("button6.Image");
            button6.ImageAlign = ContentAlignment.MiddleLeft;
            button6.Location = new Point(905, 70);
            button6.Name = "button6";
            button6.Size = new Size(100, 34);
            button6.TabIndex = 5;
            button6.Text = "关闭窗口";
            button6.TextAlign = ContentAlignment.MiddleRight;
            button6.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 128);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(204, 190);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label24);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label22);
            groupBox1.Controls.Add(label20);
            groupBox1.Controls.Add(label17);
            groupBox1.Controls.Add(label21);
            groupBox1.Controls.Add(label19);
            groupBox1.Controls.Add(label18);
            groupBox1.Controls.Add(label16);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(label15);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(label23);
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(label11);
            groupBox1.Location = new Point(246, 129);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(759, 189);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "考勤信息";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label11.Location = new Point(25, 32);
            label11.Name = "label11";
            label11.Size = new Size(52, 27);
            label11.TabIndex = 0;
            label11.Text = "应到";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label12.Location = new Point(277, 32);
            label12.Name = "label12";
            label12.Size = new Size(52, 27);
            label12.TabIndex = 0;
            label12.Text = "实到";
            label12.Click += label12_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label13.Location = new Point(538, 32);
            label13.Name = "label13";
            label13.Size = new Size(52, 27);
            label13.TabIndex = 0;
            label13.Text = "缺勤";
            label13.Click += label12_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label14.Location = new Point(26, 78);
            label14.Name = "label14";
            label14.Size = new Size(52, 27);
            label14.TabIndex = 0;
            label14.Text = "姓名";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label15.Location = new Point(278, 78);
            label15.Name = "label15";
            label15.Size = new Size(52, 27);
            label15.TabIndex = 0;
            label15.Text = "学号";
            label15.Click += label12_Click;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label16.Location = new Point(539, 78);
            label16.Name = "label16";
            label16.Size = new Size(52, 27);
            label16.TabIndex = 0;
            label16.Text = "班级";
            label16.Click += label12_Click;
            // 
            // label18
            // 
            label18.BackColor = Color.White;
            label18.BorderStyle = BorderStyle.Fixed3D;
            label18.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label18.Location = new Point(85, 28);
            label18.Name = "label18";
            label18.Size = new Size(119, 34);
            label18.TabIndex = 2;
            label18.Text = "0";
            label18.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            label17.BackColor = Color.FromArgb(192, 192, 255);
            label17.BorderStyle = BorderStyle.Fixed3D;
            label17.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label17.Location = new Point(84, 74);
            label17.Name = "label17";
            label17.Size = new Size(120, 34);
            label17.TabIndex = 2;
            label17.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            label19.BackColor = Color.White;
            label19.BorderStyle = BorderStyle.Fixed3D;
            label19.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label19.Location = new Point(335, 28);
            label19.Name = "label19";
            label19.Size = new Size(121, 34);
            label19.TabIndex = 2;
            label19.Text = "0";
            label19.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            label20.BackColor = Color.FromArgb(192, 192, 255);
            label20.BorderStyle = BorderStyle.Fixed3D;
            label20.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label20.Location = new Point(334, 74);
            label20.Name = "label20";
            label20.Size = new Size(121, 34);
            label20.TabIndex = 2;
            label20.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            label21.BackColor = Color.White;
            label21.BorderStyle = BorderStyle.Fixed3D;
            label21.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label21.ForeColor = Color.Red;
            label21.Location = new Point(596, 28);
            label21.Name = "label21";
            label21.Size = new Size(121, 34);
            label21.TabIndex = 2;
            label21.Text = "0";
            label21.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            label22.BackColor = Color.FromArgb(192, 192, 255);
            label22.BorderStyle = BorderStyle.Fixed3D;
            label22.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label22.Location = new Point(595, 74);
            label22.Name = "label22";
            label22.Size = new Size(121, 34);
            label22.TabIndex = 2;
            label22.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label23.Location = new Point(25, 130);
            label23.Name = "label23";
            label23.Size = new Size(52, 27);
            label23.TabIndex = 0;
            label23.Text = "卡号";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.Location = new Point(83, 127);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(121, 33);
            textBox1.TabIndex = 3;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label24.ForeColor = Color.Red;
            label24.Location = new Point(225, 130);
            label24.Name = "label24";
            label24.Size = new Size(74, 22);
            label24.TabIndex = 4;
            label24.Text = "准备打卡";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = SystemColors.ButtonFace;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column7 });
            dataGridView1.Location = new Point(12, 346);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(993, 311);
            dataGridView1.TabIndex = 8;
            // 
            // Column1
            // 
            Column1.HeaderText = "打卡时间";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 150;
            // 
            // Column2
            // 
            Column2.HeaderText = "学号";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 150;
            // 
            // Column3
            // 
            Column3.HeaderText = "考勤卡号";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 120;
            // 
            // Column4
            // 
            Column4.HeaderText = "姓名";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 120;
            // 
            // Column5
            // 
            Column5.HeaderText = "性别";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 80;
            // 
            // Column7
            // 
            Column7.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column7.HeaderText = "所在班级";
            Column7.Name = "Column7";
            Column7.ReadOnly = true;
            // 
            // FrmAttendance
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 669);
            Controls.Add(dataGridView1);
            Controls.Add(groupBox1);
            Controls.Add(pictureBox1);
            Controls.Add(button6);
            Controls.Add(label7);
            Controls.Add(label10);
            Controls.Add(label5);
            Controls.Add(label9);
            Controls.Add(label3);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label8);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmAttendance";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "【考勤打卡进行中...】";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Button button6;
        private PictureBox pictureBox1;
        private GroupBox groupBox1;
        private Label label12;
        private Label label11;
        private Label label13;
        private Label label17;
        private Label label18;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label22;
        private Label label20;
        private Label label21;
        private Label label19;
        private TextBox textBox1;
        private Label label23;
        private Label label24;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column7;
    }
}