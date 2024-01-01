namespace DelegateDemo03
{
    partial class FrmOther01
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
            lblCount = new Label();
            SuspendLayout();
            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Location = new Point(159, 73);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(18, 20);
            lblCount.TabIndex = 0;
            lblCount.Text = "0";
            // 
            // FrmOther01
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(414, 207);
            Controls.Add(lblCount);
            Name = "FrmOther01";
            Text = "从窗体01";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblCount;
    }
}