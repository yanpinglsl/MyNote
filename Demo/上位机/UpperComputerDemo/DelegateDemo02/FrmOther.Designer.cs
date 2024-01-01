namespace DelegateDemo02
{
    partial class FrmOther
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
            btnClick = new Button();
            SuspendLayout();
            // 
            // btnClick
            // 
            btnClick.Location = new Point(135, 53);
            btnClick.Name = "btnClick";
            btnClick.Size = new Size(262, 127);
            btnClick.TabIndex = 0;
            btnClick.Text = "单击我";
            btnClick.UseVisualStyleBackColor = true;
            btnClick.Click += btnClick_Click;
            // 
            // FrmOther
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(540, 248);
            Controls.Add(btnClick);
            Name = "FrmOther";
            Text = "从窗体";
            ResumeLayout(false);
        }

        #endregion

        private Button btnClick;
    }
}