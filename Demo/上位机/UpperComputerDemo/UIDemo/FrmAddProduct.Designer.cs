namespace UIDemo
{
    partial class FrmAddProduct
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
            btnClose = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(18, 54, 112);
            btnClose.FlatAppearance.BorderColor = Color.RoyalBlue;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(713, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 28);
            btnClose.TabIndex = 2;
            btnClose.Text = "关闭窗口";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("微软雅黑", 21.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(134, 155);
            label1.Name = "label1";
            label1.Size = new Size(510, 38);
            label1.TabIndex = 3;
            label1.Text = "该页面仅为了测试【嵌入子窗体】功能";
            // 
            // FrmAddProduct
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmAddProduct";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrmAddProduct";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnClose;
        private Label label1;
    }
}