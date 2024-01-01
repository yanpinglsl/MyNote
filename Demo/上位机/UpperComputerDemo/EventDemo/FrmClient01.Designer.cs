namespace EventDemo
{
    partial class FrmClient01
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
            txtMsg = new TextBox();
            SuspendLayout();
            // 
            // txtMsg
            // 
            txtMsg.Location = new Point(12, 12);
            txtMsg.Multiline = true;
            txtMsg.Name = "txtMsg";
            txtMsg.Size = new Size(440, 153);
            txtMsg.TabIndex = 3;
            // 
            // FrmClient01
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(465, 180);
            Controls.Add(txtMsg);
            Name = "FrmClient01";
            Text = "客户端（1）";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMsg;
    }
}