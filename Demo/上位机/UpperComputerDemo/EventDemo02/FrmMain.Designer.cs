namespace EventDemo02
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
            txtMsg = new TextBox();
            SuspendLayout();
            // 
            // txtMsg
            // 
            txtMsg.Location = new Point(13, 13);
            txtMsg.Margin = new Padding(4);
            txtMsg.Multiline = true;
            txtMsg.Name = "txtMsg";
            txtMsg.Size = new Size(565, 179);
            txtMsg.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(591, 204);
            Controls.Add(txtMsg);
            Name = "Form1";
            Text = "主界面";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMsg;
    }
}