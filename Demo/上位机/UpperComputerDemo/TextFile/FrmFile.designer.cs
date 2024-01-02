namespace TextFile
{
    partial class FrmFile
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
            txtContent = new TextBox();
            btnWriteAll = new Button();
            btnReadAll = new Button();
            btnWriteLine = new Button();
            label1 = new Label();
            label2 = new Label();
            txtFrom = new TextBox();
            txtTo = new TextBox();
            btnCopy = new Button();
            btnRemove = new Button();
            btnDel = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            btnDelAllFiles = new Button();
            btnCreate = new Button();
            btnShowSubDir = new Button();
            btnShowAllFiles = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // txtContent
            // 
            txtContent.Location = new Point(14, 17);
            txtContent.Margin = new Padding(4, 4, 4, 4);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.Size = new Size(258, 395);
            txtContent.TabIndex = 0;
            // 
            // btnWriteAll
            // 
            btnWriteAll.Location = new Point(15, 41);
            btnWriteAll.Margin = new Padding(4, 4, 4, 4);
            btnWriteAll.Name = "btnWriteAll";
            btnWriteAll.Size = new Size(139, 33);
            btnWriteAll.TabIndex = 1;
            btnWriteAll.Text = "写入文本文件";
            btnWriteAll.UseVisualStyleBackColor = true;
            btnWriteAll.Click += btnWriteAll_Click;
            // 
            // btnReadAll
            // 
            btnReadAll.Location = new Point(181, 41);
            btnReadAll.Margin = new Padding(4, 4, 4, 4);
            btnReadAll.Name = "btnReadAll";
            btnReadAll.Size = new Size(155, 33);
            btnReadAll.TabIndex = 1;
            btnReadAll.Text = "从文本文件中读取";
            btnReadAll.UseVisualStyleBackColor = true;
            btnReadAll.Click += btnReadAll_Click;
            // 
            // btnWriteLine
            // 
            btnWriteLine.Location = new Point(368, 41);
            btnWriteLine.Margin = new Padding(4, 4, 4, 4);
            btnWriteLine.Name = "btnWriteLine";
            btnWriteLine.Size = new Size(142, 33);
            btnWriteLine.TabIndex = 2;
            btnWriteLine.Text = "模拟写入系统日志";
            btnWriteLine.UseVisualStyleBackColor = true;
            btnWriteLine.Click += btnWriteLine_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 40);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(71, 17);
            label1.TabIndex = 3;
            label1.Text = "源文件路径:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(284, 44);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(83, 17);
            label2.TabIndex = 3;
            label2.Text = "目的文件路径:";
            // 
            // txtFrom
            // 
            txtFrom.Location = new Point(91, 35);
            txtFrom.Margin = new Padding(4, 4, 4, 4);
            txtFrom.Name = "txtFrom";
            txtFrom.Size = new Size(152, 23);
            txtFrom.TabIndex = 4;
            txtFrom.Text = "C:\\\\myfile.txt";
            // 
            // txtTo
            // 
            txtTo.Location = new Point(379, 35);
            txtTo.Margin = new Padding(4, 4, 4, 4);
            txtTo.Name = "txtTo";
            txtTo.Size = new Size(145, 23);
            txtTo.TabIndex = 4;
            txtTo.Text = "D:\\\\myfile.txt";
            // 
            // btnCopy
            // 
            btnCopy.Location = new Point(181, 79);
            btnCopy.Margin = new Padding(4, 4, 4, 4);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(88, 33);
            btnCopy.TabIndex = 5;
            btnCopy.Text = "复制文件";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(368, 79);
            btnRemove.Margin = new Padding(4, 4, 4, 4);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(88, 33);
            btnRemove.TabIndex = 5;
            btnRemove.Text = "移动文件";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnDel
            // 
            btnDel.Location = new Point(15, 79);
            btnDel.Margin = new Padding(4, 4, 4, 4);
            btnDel.Name = "btnDel";
            btnDel.Size = new Size(88, 33);
            btnDel.TabIndex = 5;
            btnDel.Text = "删除文件";
            btnDel.UseVisualStyleBackColor = true;
            btnDel.Click += btnDel_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnReadAll);
            groupBox1.Controls.Add(btnWriteAll);
            groupBox1.Controls.Add(btnWriteLine);
            groupBox1.Location = new Point(280, 17);
            groupBox1.Margin = new Padding(4, 4, 4, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 4, 4, 4);
            groupBox1.Size = new Size(545, 106);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "文本文件读写";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtTo);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(btnDel);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(btnRemove);
            groupBox2.Controls.Add(btnCopy);
            groupBox2.Controls.Add(txtFrom);
            groupBox2.Location = new Point(280, 132);
            groupBox2.Margin = new Padding(4, 4, 4, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 4, 4, 4);
            groupBox2.Size = new Size(545, 130);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "文件操作";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnDelAllFiles);
            groupBox3.Controls.Add(btnCreate);
            groupBox3.Controls.Add(btnShowSubDir);
            groupBox3.Controls.Add(btnShowAllFiles);
            groupBox3.Location = new Point(281, 272);
            groupBox3.Margin = new Padding(4, 4, 4, 4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 4, 4, 4);
            groupBox3.Size = new Size(544, 142);
            groupBox3.TabIndex = 8;
            groupBox3.TabStop = false;
            groupBox3.Text = "文件目录操作";
            // 
            // btnDelAllFiles
            // 
            btnDelAllFiles.Location = new Point(274, 94);
            btnDelAllFiles.Margin = new Padding(4, 4, 4, 4);
            btnDelAllFiles.Name = "btnDelAllFiles";
            btnDelAllFiles.Size = new Size(262, 33);
            btnDelAllFiles.TabIndex = 3;
            btnDelAllFiles.Text = "删除指定目录下的所有子目录和文件";
            btnDelAllFiles.UseVisualStyleBackColor = true;
            btnDelAllFiles.Click += btnDelAllFiles_Click;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(14, 94);
            btnCreate.Margin = new Padding(4, 4, 4, 4);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(229, 33);
            btnCreate.TabIndex = 2;
            btnCreate.Text = "在指定目录下创建一个子目录";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnShowSubDir
            // 
            btnShowSubDir.Location = new Point(274, 42);
            btnShowSubDir.Margin = new Padding(4, 4, 4, 4);
            btnShowSubDir.Name = "btnShowSubDir";
            btnShowSubDir.Size = new Size(217, 33);
            btnShowSubDir.TabIndex = 1;
            btnShowSubDir.Text = "显示指定目录下的所有子目录";
            btnShowSubDir.UseVisualStyleBackColor = true;
            btnShowSubDir.Click += btnShowSubDir_Click;
            // 
            // btnShowAllFiles
            // 
            btnShowAllFiles.Location = new Point(14, 42);
            btnShowAllFiles.Margin = new Padding(4, 4, 4, 4);
            btnShowAllFiles.Name = "btnShowAllFiles";
            btnShowAllFiles.Size = new Size(229, 33);
            btnShowAllFiles.TabIndex = 0;
            btnShowAllFiles.Text = "显示指定目录下的所有文件";
            btnShowAllFiles.UseVisualStyleBackColor = true;
            btnShowAllFiles.Click += btnShowAllFiles_Click;
            // 
            // FrmFile
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(838, 434);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(txtContent);
            Margin = new Padding(4, 4, 4, 4);
            Name = "FrmFile";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "文件操作";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button btnWriteAll;
        private System.Windows.Forms.Button btnReadAll;
        private System.Windows.Forms.Button btnWriteLine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnShowAllFiles;
        private System.Windows.Forms.Button btnShowSubDir;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnDelAllFiles;
    }
}

