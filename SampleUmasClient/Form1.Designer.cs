namespace SampleUmasClient
{
    partial class Form1
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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            bConnect = new Button();
            tbIP = new TextBox();
            label1 = new Label();
            cbUMASFonction = new ComboBox();
            bSendInfo = new Button();
            lbLog = new ListBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48.61432F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(lbLog, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(866, 554);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.Controls.Add(bConnect, 2, 0);
            tableLayoutPanel2.Controls.Add(tbIP, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(cbUMASFonction, 1, 1);
            tableLayoutPanel2.Controls.Add(bSendInfo, 2, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.Size = new Size(860, 271);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // bConnect
            // 
            bConnect.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            bConnect.Location = new Point(433, 8);
            bConnect.Name = "bConnect";
            bConnect.Size = new Size(209, 38);
            bConnect.TabIndex = 0;
            bConnect.Text = "Connect";
            bConnect.UseVisualStyleBackColor = true;
            bConnect.Click += bConnect_Click;
            // 
            // tbIP
            // 
            tbIP.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbIP.Location = new Point(218, 15);
            tbIP.Name = "tbIP";
            tbIP.Size = new Size(209, 23);
            tbIP.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(209, 54);
            label1.TabIndex = 2;
            label1.Text = "IP";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cbUMASFonction
            // 
            cbUMASFonction.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cbUMASFonction.FormattingEnabled = true;
            cbUMASFonction.Location = new Point(218, 69);
            cbUMASFonction.Name = "cbUMASFonction";
            cbUMASFonction.Size = new Size(209, 23);
            cbUMASFonction.TabIndex = 4;
            // 
            // bSendInfo
            // 
            bSendInfo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            bSendInfo.Location = new Point(433, 64);
            bSendInfo.Name = "bSendInfo";
            bSendInfo.Size = new Size(209, 34);
            bSendInfo.TabIndex = 3;
            bSendInfo.Text = "SendRequest";
            bSendInfo.UseVisualStyleBackColor = true;
            bSendInfo.Click += bSendInfo_Click;
            // 
            // lbLog
            // 
            lbLog.Dock = DockStyle.Fill;
            lbLog.FormattingEnabled = true;
            lbLog.ItemHeight = 15;
            lbLog.Location = new Point(3, 280);
            lbLog.Name = "lbLog";
            lbLog.Size = new Size(860, 271);
            lbLog.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(866, 554);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Button bConnect;
        private TextBox tbIP;
        private Label label1;
        private ComboBox cbUMASFonction;
        private Button bSendInfo;
        private ListBox lbLog;
    }
}