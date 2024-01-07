namespace UIDemo
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            panel1 = new Panel();
            btnClose = new Button();
            button1 = new Button();
            label1 = new Label();
            panel2 = new Panel();
            label4 = new Label();
            label5 = new Label();
            label3 = new Label();
            label2 = new Label();
            splitContainer = new SplitContainer();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button12 = new Button();
            button11 = new Button();
            button10 = new Button();
            button8 = new Button();
            button3 = new Button();
            button9 = new Button();
            button7 = new Button();
            btnMoodsInfo = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1280, 63);
            panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(18, 54, 112);
            btnClose.FlatAppearance.BorderColor = Color.RoyalBlue;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1205, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 28);
            btnClose.TabIndex = 1;
            btnClose.Text = "退出系统";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(18, 54, 112);
            button1.FlatAppearance.BorderColor = Color.RoyalBlue;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button1.ForeColor = Color.White;
            button1.Location = new Point(1135, 0);
            button1.Name = "button1";
            button1.Size = new Size(75, 28);
            button1.TabIndex = 1;
            button1.Text = "使用帮助";
            button1.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(18, 54, 112);
            label1.Font = new Font("微软雅黑", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 9);
            label1.Name = "label1";
            label1.Size = new Size(277, 39);
            label1.TabIndex = 0;
            label1.Text = "企业级MIS综合平台";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(18, 54, 112);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(0, 773);
            panel2.Name = "panel2";
            panel2.Size = new Size(1280, 27);
            panel2.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(1125, 2);
            label4.Name = "label4";
            label4.Size = new Size(143, 17);
            label4.TabIndex = 0;
            label4.Text = "技术支持：029-1234567";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = Color.White;
            label5.Location = new Point(585, 6);
            label5.Name = "label5";
            label5.Size = new Size(148, 17);
            label5.TabIndex = 0;
            label5.Text = "本机IP：198.196.200.100";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(47, 6);
            label3.Name = "label3";
            label3.Size = new Size(92, 17);
            label3.TabIndex = 0;
            label3.Text = "上位机学习项目";
            // 
            // label2
            // 
            label2.Image = (Image)resources.GetObject("label2.Image");
            label2.Location = new Point(12, 2);
            label2.Name = "label2";
            label2.Size = new Size(29, 23);
            label2.TabIndex = 0;
            // 
            // splitContainer
            // 
            splitContainer.Location = new Point(6, 69);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.BackColor = Color.FromArgb(12, 56, 117);
            splitContainer.Panel1.Controls.Add(button6);
            splitContainer.Panel1.Controls.Add(button5);
            splitContainer.Panel1.Controls.Add(button4);
            splitContainer.Panel1.Controls.Add(button12);
            splitContainer.Panel1.Controls.Add(button11);
            splitContainer.Panel1.Controls.Add(button10);
            splitContainer.Panel1.Controls.Add(button8);
            splitContainer.Panel1.Controls.Add(button3);
            splitContainer.Panel1.Controls.Add(button9);
            splitContainer.Panel1.Controls.Add(button7);
            splitContainer.Panel1.Controls.Add(btnMoodsInfo);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.BackColor = SystemColors.ButtonFace;
            splitContainer.Size = new Size(1266, 698);
            splitContainer.SplitterDistance = 223;
            splitContainer.TabIndex = 2;
            // 
            // button6
            // 
            button6.BackColor = Color.FromArgb(9, 163, 220);
            button6.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button6.ForeColor = Color.White;
            button6.Image = (Image)resources.GetObject("button6.Image");
            button6.ImageAlign = ContentAlignment.MiddleLeft;
            button6.Location = new Point(9, 191);
            button6.Name = "button6";
            button6.Size = new Size(197, 33);
            button6.TabIndex = 0;
            button6.Text = "     商 品 销 售 系 统";
            button6.TextAlign = ContentAlignment.MiddleLeft;
            button6.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            button5.BackColor = Color.FromArgb(9, 163, 220);
            button5.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button5.ForeColor = Color.White;
            button5.Image = (Image)resources.GetObject("button5.Image");
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.Location = new Point(9, 152);
            button5.Name = "button5";
            button5.Size = new Size(197, 33);
            button5.TabIndex = 0;
            button5.Text = "     商 品 销 售 管 理";
            button5.TextAlign = ContentAlignment.MiddleLeft;
            button5.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(9, 163, 220);
            button4.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button4.ForeColor = Color.White;
            button4.Image = (Image)resources.GetObject("button4.Image");
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.Location = new Point(9, 113);
            button4.Name = "button4";
            button4.Size = new Size(197, 33);
            button4.TabIndex = 0;
            button4.Text = "     商 品 采 购 管 理";
            button4.TextAlign = ContentAlignment.MiddleLeft;
            button4.UseVisualStyleBackColor = false;
            // 
            // button12
            // 
            button12.BackColor = Color.FromArgb(9, 163, 220);
            button12.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button12.FlatStyle = FlatStyle.Flat;
            button12.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button12.ForeColor = Color.White;
            button12.Image = (Image)resources.GetObject("button12.Image");
            button12.ImageAlign = ContentAlignment.MiddleLeft;
            button12.Location = new Point(111, 633);
            button12.Name = "button12";
            button12.Size = new Size(95, 33);
            button12.TabIndex = 0;
            button12.Text = "     在线升级";
            button12.TextAlign = ContentAlignment.MiddleLeft;
            button12.UseVisualStyleBackColor = false;
            // 
            // button11
            // 
            button11.BackColor = Color.FromArgb(9, 163, 220);
            button11.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button11.FlatStyle = FlatStyle.Flat;
            button11.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button11.ForeColor = Color.White;
            button11.Image = (Image)resources.GetObject("button11.Image");
            button11.ImageAlign = ContentAlignment.MiddleLeft;
            button11.Location = new Point(5, 633);
            button11.Name = "button11";
            button11.Size = new Size(100, 33);
            button11.TabIndex = 0;
            button11.Text = "     修改密码";
            button11.TextAlign = ContentAlignment.MiddleLeft;
            button11.UseVisualStyleBackColor = false;
            // 
            // button10
            // 
            button10.BackColor = Color.FromArgb(9, 163, 220);
            button10.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button10.FlatStyle = FlatStyle.Flat;
            button10.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button10.ForeColor = Color.White;
            button10.Image = (Image)resources.GetObject("button10.Image");
            button10.ImageAlign = ContentAlignment.MiddleLeft;
            button10.Location = new Point(9, 390);
            button10.Name = "button10";
            button10.Size = new Size(197, 33);
            button10.TabIndex = 0;
            button10.Text = "     系 统 配 置 管 理";
            button10.TextAlign = ContentAlignment.MiddleLeft;
            button10.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            button8.BackColor = Color.FromArgb(9, 163, 220);
            button8.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button8.FlatStyle = FlatStyle.Flat;
            button8.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button8.ForeColor = Color.White;
            button8.Image = (Image)resources.GetObject("button8.Image");
            button8.ImageAlign = ContentAlignment.MiddleLeft;
            button8.Location = new Point(9, 294);
            button8.Name = "button8";
            button8.Size = new Size(197, 33);
            button8.TabIndex = 0;
            button8.Text = "     供 应 商 信 息 管 理";
            button8.TextAlign = ContentAlignment.MiddleLeft;
            button8.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(9, 163, 220);
            button3.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button3.ForeColor = Color.White;
            button3.Image = (Image)resources.GetObject("button3.Image");
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.Location = new Point(9, 51);
            button3.Name = "button3";
            button3.Size = new Size(197, 33);
            button3.TabIndex = 0;
            button3.Text = "     商 品 库 存 管 理";
            button3.TextAlign = ContentAlignment.MiddleLeft;
            button3.UseVisualStyleBackColor = false;
            // 
            // button9
            // 
            button9.BackColor = Color.FromArgb(9, 163, 220);
            button9.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button9.ForeColor = Color.White;
            button9.Image = (Image)resources.GetObject("button9.Image");
            button9.ImageAlign = ContentAlignment.MiddleLeft;
            button9.Location = new Point(9, 351);
            button9.Name = "button9";
            button9.Size = new Size(197, 33);
            button9.TabIndex = 0;
            button9.Text = "     用 户 权 限 管 理";
            button9.TextAlign = ContentAlignment.MiddleLeft;
            button9.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            button7.BackColor = Color.FromArgb(9, 163, 220);
            button7.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            button7.ForeColor = Color.White;
            button7.Image = (Image)resources.GetObject("button7.Image");
            button7.ImageAlign = ContentAlignment.MiddleLeft;
            button7.Location = new Point(9, 255);
            button7.Name = "button7";
            button7.Size = new Size(197, 33);
            button7.TabIndex = 0;
            button7.Text = "     客 户 信 息 管 理";
            button7.TextAlign = ContentAlignment.MiddleLeft;
            button7.UseVisualStyleBackColor = false;
            // 
            // btnMoodsInfo
            // 
            btnMoodsInfo.BackColor = Color.FromArgb(9, 163, 220);
            btnMoodsInfo.FlatAppearance.BorderColor = Color.FromArgb(9, 163, 220);
            btnMoodsInfo.FlatStyle = FlatStyle.Flat;
            btnMoodsInfo.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            btnMoodsInfo.ForeColor = Color.White;
            btnMoodsInfo.Image = (Image)resources.GetObject("btnMoodsInfo.Image");
            btnMoodsInfo.ImageAlign = ContentAlignment.MiddleLeft;
            btnMoodsInfo.Location = new Point(9, 12);
            btnMoodsInfo.Name = "btnMoodsInfo";
            btnMoodsInfo.Size = new Size(197, 33);
            btnMoodsInfo.TabIndex = 0;
            btnMoodsInfo.Text = "     商 品 信 息 管 理";
            btnMoodsInfo.TextAlign = ContentAlignment.MiddleLeft;
            btnMoodsInfo.UseVisualStyleBackColor = false;
            btnMoodsInfo.Click += btnMoodsInfo_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(111, 184, 213);
            ClientSize = new Size(1280, 800);
            Controls.Add(splitContainer);
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrmMain";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Panel panel2;
        private Label label4;
        private Label label5;
        private Label label3;
        private Label label2;
        private SplitContainer splitContainer;
        private Button button1;
        private Button btnClose;
        private Button button6;
        private Button button5;
        private Button button4;
        private Button button12;
        private Button button11;
        private Button button10;
        private Button button8;
        private Button button3;
        private Button button9;
        private Button button7;
        private Button btnMoodsInfo;
    }
}