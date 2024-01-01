namespace EventDemo02
{
    partial class FrmClient02
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
            label1 = new Label();
            btnSend = new Button();
            SuspendLayout();
            // 
            // txtMsg
            // 
            txtMsg.Location = new Point(29, 57);
            txtMsg.Margin = new Padding(4);
            txtMsg.Multiline = true;
            txtMsg.Name = "txtMsg";
            txtMsg.Size = new Size(565, 179);
            txtMsg.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 23);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(99, 20);
            label1.TabIndex = 7;
            label1.Text = "请输入信息：";
            // 
            // btnSend
            // 
            btnSend.Location = new Point(429, 244);
            btnSend.Margin = new Padding(4);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(166, 61);
            btnSend.TabIndex = 6;
            btnSend.Text = "发送消息";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // FrmClient02
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(608, 316);
            Controls.Add(txtMsg);
            Controls.Add(label1);
            Controls.Add(btnSend);
            Name = "FrmClient02";
            Text = "客户端（2）";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMsg;
        private Label label1;
        private Button btnSend;
    }
}