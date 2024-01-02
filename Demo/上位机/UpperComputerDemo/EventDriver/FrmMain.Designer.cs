namespace EventDriver
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
            btnTest = new Button();
            btnAndy = new Button();
            btnCarry = new Button();
            btnCoco = new Button();
            btnTestEvent = new Button();
            btnLink = new Button();
            btnCut = new Button();
            btnTestMsgbox = new Button();
            SuspendLayout();
            // 
            // btnTest
            // 
            btnTest.Location = new Point(12, 71);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(135, 29);
            btnTest.TabIndex = 0;
            btnTest.Text = "测试事件的响应";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnAndy
            // 
            btnAndy.Location = new Point(182, 71);
            btnAndy.Name = "btnAndy";
            btnAndy.Size = new Size(94, 29);
            btnAndy.TabIndex = 1;
            btnAndy.Text = "Andy老师";
            btnAndy.UseVisualStyleBackColor = true;
            // 
            // btnCarry
            // 
            btnCarry.Location = new Point(282, 71);
            btnCarry.Name = "btnCarry";
            btnCarry.Size = new Size(94, 29);
            btnCarry.TabIndex = 1;
            btnCarry.Text = "Carry老师";
            btnCarry.UseVisualStyleBackColor = true;
            // 
            // btnCoco
            // 
            btnCoco.Location = new Point(382, 71);
            btnCoco.Name = "btnCoco";
            btnCoco.Size = new Size(94, 29);
            btnCoco.TabIndex = 1;
            btnCoco.Text = "Coco老师";
            btnCoco.UseVisualStyleBackColor = true;
            // 
            // btnTestEvent
            // 
            btnTestEvent.Location = new Point(12, 126);
            btnTestEvent.Name = "btnTestEvent";
            btnTestEvent.Size = new Size(135, 29);
            btnTestEvent.TabIndex = 0;
            btnTestEvent.Text = "测试按钮事件";
            btnTestEvent.UseVisualStyleBackColor = true;
            btnTestEvent.Click += btnTestEvent_Click;
            // 
            // btnLink
            // 
            btnLink.Location = new Point(182, 126);
            btnLink.Name = "btnLink";
            btnLink.Size = new Size(135, 29);
            btnLink.TabIndex = 0;
            btnLink.Text = "事件关联";
            btnLink.UseVisualStyleBackColor = true;
            btnLink.Click += btnLink_Click;
            // 
            // btnCut
            // 
            btnCut.Location = new Point(341, 126);
            btnCut.Name = "btnCut";
            btnCut.Size = new Size(135, 29);
            btnCut.TabIndex = 0;
            btnCut.Text = "事件断开";
            btnCut.UseVisualStyleBackColor = true;
            btnCut.Click += btnCut_Click;
            // 
            // btnTestMsgbox
            // 
            btnTestMsgbox.Location = new Point(12, 185);
            btnTestMsgbox.Name = "btnTestMsgbox";
            btnTestMsgbox.Size = new Size(135, 29);
            btnTestMsgbox.TabIndex = 2;
            btnTestMsgbox.Text = "测试消息框";
            btnTestMsgbox.UseVisualStyleBackColor = true;
            btnTestMsgbox.Click += btnTestMsgbox_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(559, 274);
            Controls.Add(btnTestMsgbox);
            Controls.Add(btnCoco);
            Controls.Add(btnCarry);
            Controls.Add(btnAndy);
            Controls.Add(btnCut);
            Controls.Add(btnLink);
            Controls.Add(btnTestEvent);
            Controls.Add(btnTest);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "事件驱动测试";
            ResumeLayout(false);
        }

        #endregion

        private Button btnTest;
        private Button btnAndy;
        private Button btnCarry;
        private Button btnCoco;
        private Button btnTestEvent;
        private Button btnLink;
        private Button btnCut;
        private Button btnTestMsgbox;
    }
}