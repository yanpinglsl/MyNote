namespace DelegateDemo03
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnCount = new Button();
            btnReset = new Button();
            SuspendLayout();
            // 
            // btnCount
            // 
            btnCount.Location = new Point(143, 29);
            btnCount.Name = "btnCount";
            btnCount.Size = new Size(197, 77);
            btnCount.TabIndex = 0;
            btnCount.Text = "单击我";
            btnCount.UseVisualStyleBackColor = true;
            btnCount.Click += btnCount_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(143, 173);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(197, 77);
            btnReset.TabIndex = 0;
            btnReset.Text = "复位";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(454, 333);
            Controls.Add(btnReset);
            Controls.Add(btnCount);
            Name = "FrmMain";
            Text = "主窗体";
            ResumeLayout(false);
        }

        #endregion

        private Button btnCount;
        private Button btnReset;
    }
}