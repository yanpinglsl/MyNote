namespace StudentManagerPro
{
    partial class FrmAddStudent
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAddStudent));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            button1 = new Button();
            imgList = new ImageList(components);
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            pictureBox2 = new PictureBox();
            button6 = new Button();
            button7 = new Button();
            groupBox1 = new GroupBox();
            comboBox1 = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox5 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label3 = new Label();
            label5 = new Label();
            label4 = new Label();
            label8 = new Label();
            label7 = new Label();
            label9 = new Label();
            label6 = new Label();
            label2 = new Label();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
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
            label1.Size = new Size(178, 41);
            label1.TabIndex = 1;
            label1.Text = "添加新学员";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 65);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(204, 166);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageIndex = 0;
            button1.ImageList = imgList;
            button1.Location = new Point(263, 65);
            button1.Name = "button1";
            button1.Size = new Size(100, 34);
            button1.TabIndex = 3;
            button1.Text = "启动摄像头";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.UseVisualStyleBackColor = true;
            // 
            // imgList
            // 
            imgList.ColorDepth = ColorDepth.Depth8Bit;
            imgList.ImageStream = (ImageListStreamer)resources.GetObject("imgList.ImageStream");
            imgList.TransparentColor = Color.Transparent;
            imgList.Images.SetKeyName(0, "4.png");
            imgList.Images.SetKeyName(1, "5.jpg");
            imgList.Images.SetKeyName(2, "exit.ico");
            imgList.Images.SetKeyName(3, "lklLoginExit.ico");
            imgList.Images.SetKeyName(4, "MngIncdown.bmp");
            imgList.Images.SetKeyName(5, "turn.BMP");
            imgList.Images.SetKeyName(6, "关闭.bmp");
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageIndex = 3;
            button2.ImageList = imgList;
            button2.Location = new Point(406, 65);
            button2.Name = "button2";
            button2.Size = new Size(100, 34);
            button2.TabIndex = 3;
            button2.Text = "关闭摄像头";
            button2.TextAlign = ContentAlignment.MiddleRight;
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.ImageIndex = 1;
            button3.ImageList = imgList;
            button3.Location = new Point(263, 124);
            button3.Name = "button3";
            button3.Size = new Size(100, 34);
            button3.TabIndex = 3;
            button3.Text = "开始拍照";
            button3.TextAlign = ContentAlignment.MiddleRight;
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.ImageIndex = 2;
            button4.ImageList = imgList;
            button4.Location = new Point(406, 124);
            button4.Name = "button4";
            button4.Size = new Size(100, 34);
            button4.TabIndex = 3;
            button4.Text = "清除照片";
            button4.TextAlign = ContentAlignment.MiddleRight;
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.ImageIndex = 5;
            button5.ImageList = imgList;
            button5.Location = new Point(263, 191);
            button5.Name = "button5";
            button5.Size = new Size(100, 34);
            button5.TabIndex = 3;
            button5.Text = "选择照片";
            button5.TextAlign = ContentAlignment.MiddleRight;
            button5.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(545, 65);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(204, 166);
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            // 
            // button6
            // 
            button6.ImageAlign = ContentAlignment.MiddleLeft;
            button6.ImageIndex = 6;
            button6.ImageList = imgList;
            button6.Location = new Point(905, 65);
            button6.Name = "button6";
            button6.Size = new Size(100, 34);
            button6.TabIndex = 3;
            button6.Text = "关闭窗口";
            button6.TextAlign = ContentAlignment.MiddleRight;
            button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.ImageAlign = ContentAlignment.MiddleLeft;
            button7.ImageIndex = 4;
            button7.ImageList = imgList;
            button7.Location = new Point(905, 191);
            button7.Name = "button7";
            button7.Size = new Size(100, 34);
            button7.TabIndex = 3;
            button7.Text = "确认添加";
            button7.TextAlign = ContentAlignment.MiddleRight;
            button7.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Controls.Add(textBox4);
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(textBox5);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(15, 260);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(990, 157);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "学生基本信息";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(804, 34);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 25);
            comboBox1.TabIndex = 4;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(547, 35);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(118, 23);
            dateTimePicker1.TabIndex = 3;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(354, 36);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(38, 21);
            radioButton2.TabIndex = 2;
            radioButton2.TabStop = true;
            radioButton2.Text = "女";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(310, 36);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(38, 21);
            radioButton1.TabIndex = 2;
            radioButton1.TabStop = true;
            radioButton1.Text = "男";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(804, 73);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(123, 23);
            textBox4.TabIndex = 1;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(547, 73);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(123, 23);
            textBox3.TabIndex = 1;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(100, 115);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(565, 23);
            textBox5.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(100, 73);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(292, 23);
            textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(100, 35);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(123, 23);
            textBox1.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(260, 38);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 0;
            label3.Text = "性别：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(730, 38);
            label5.Name = "label5";
            label5.Size = new Size(68, 17);
            label5.TabIndex = 0;
            label5.Text = "所在班级：";
            label5.Click += label2_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(473, 38);
            label4.Name = "label4";
            label4.Size = new Size(68, 17);
            label4.TabIndex = 0;
            label4.Text = "出生年月：";
            label4.Click += label2_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(730, 76);
            label8.Name = "label8";
            label8.Size = new Size(68, 17);
            label8.TabIndex = 0;
            label8.Text = "联系电话：";
            label8.Click += label2_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(473, 76);
            label7.Name = "label7";
            label7.Size = new Size(68, 17);
            label7.TabIndex = 0;
            label7.Text = "考勤卡号：";
            label7.Click += label2_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(26, 118);
            label9.Name = "label9";
            label9.Size = new Size(68, 17);
            label9.TabIndex = 0;
            label9.Text = "家庭住址：";
            label9.Click += label2_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(26, 76);
            label6.Name = "label6";
            label6.Size = new Size(68, 17);
            label6.TabIndex = 0;
            label6.Text = "身份证号：";
            label6.Click += label2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(26, 38);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 0;
            label2.Text = "学生姓名：";
            label2.Click += label2_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = SystemColors.ButtonFace;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6, Column7 });
            dataGridView1.Location = new Point(12, 430);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(993, 227);
            dataGridView1.TabIndex = 5;
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
            Column2.Width = 120;
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
            Column4.HeaderText = "出生年月";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // Column5
            // 
            Column5.HeaderText = "身份证号";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 150;
            // 
            // Column6
            // 
            Column6.HeaderText = "考勤卡号";
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            Column6.Width = 150;
            // 
            // Column7
            // 
            Column7.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column7.HeaderText = "所在班级";
            Column7.Name = "Column7";
            Column7.ReadOnly = true;
            // 
            // FrmAddStudent
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 669);
            Controls.Add(dataGridView1);
            Controls.Add(groupBox1);
            Controls.Add(button4);
            Controls.Add(button2);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAddStudent";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "添加新学员";
            Load += FrmAddStudent_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Button button1;
        private ImageList imgList;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private PictureBox pictureBox2;
        private Button button6;
        private Button button7;
        private GroupBox groupBox1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private TextBox textBox1;
        private Label label3;
        private Label label2;
        private Label label4;
        private DateTimePicker dateTimePicker1;
        private Label label5;
        private ComboBox comboBox1;
        private TextBox textBox2;
        private Label label6;
        private TextBox textBox3;
        private Label label7;
        private TextBox textBox4;
        private Label label8;
        private TextBox textBox5;
        private Label label9;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
    }
}