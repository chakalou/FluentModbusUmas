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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.bConnect = new System.Windows.Forms.Button();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbUMASFonction = new System.Windows.Forms.ComboBox();
            this.bSendInfo = new System.Windows.Forms.Button();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbCRC = new System.Windows.Forms.Label();
            this.lbCRCSHIFTED = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbCPU = new System.Windows.Forms.Label();
            this.lbFW = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.61432F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbLog, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(866, 554);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.bConnect, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tbIP, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbUMASFonction, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.bSendInfo, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.lbCRC, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.lbCRCSHIFTED, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.label5, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.lbCPU, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.lbFW, 3, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(860, 271);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // bConnect
            // 
            this.bConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.bConnect.Location = new System.Drawing.Point(433, 8);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(209, 38);
            this.bConnect.TabIndex = 0;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // tbIP
            // 
            this.tbIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbIP.Location = new System.Drawing.Point(218, 15);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(209, 23);
            this.tbIP.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 54);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbUMASFonction
            // 
            this.cbUMASFonction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUMASFonction.FormattingEnabled = true;
            this.cbUMASFonction.Location = new System.Drawing.Point(218, 69);
            this.cbUMASFonction.Name = "cbUMASFonction";
            this.cbUMASFonction.Size = new System.Drawing.Size(209, 23);
            this.cbUMASFonction.TabIndex = 4;
            // 
            // bSendInfo
            // 
            this.bSendInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.bSendInfo.Location = new System.Drawing.Point(433, 64);
            this.bSendInfo.Name = "bSendInfo";
            this.bSendInfo.Size = new System.Drawing.Size(209, 34);
            this.bSendInfo.TabIndex = 3;
            this.bSendInfo.Text = "SendRequest";
            this.bSendInfo.UseVisualStyleBackColor = true;
            this.bSendInfo.Click += new System.EventHandler(this.bSendInfo_Click_1);
            // 
            // lbLog
            // 
            this.lbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 15;
            this.lbLog.Location = new System.Drawing.Point(3, 280);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(860, 271);
            this.lbLog.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 54);
            this.label2.TabIndex = 5;
            this.label2.Text = "CRC";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(209, 54);
            this.label3.TabIndex = 6;
            this.label3.Text = "CRC SHIFTED";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCRC
            // 
            this.lbCRC.AutoSize = true;
            this.lbCRC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCRC.Location = new System.Drawing.Point(218, 108);
            this.lbCRC.Name = "lbCRC";
            this.lbCRC.Size = new System.Drawing.Size(209, 54);
            this.lbCRC.TabIndex = 7;
            this.lbCRC.Text = "label4";
            this.lbCRC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCRCSHIFTED
            // 
            this.lbCRCSHIFTED.AutoSize = true;
            this.lbCRCSHIFTED.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCRCSHIFTED.Location = new System.Drawing.Point(218, 162);
            this.lbCRCSHIFTED.Name = "lbCRCSHIFTED";
            this.lbCRCSHIFTED.Size = new System.Drawing.Size(209, 54);
            this.lbCRCSHIFTED.TabIndex = 8;
            this.lbCRCSHIFTED.Text = "label5";
            this.lbCRCSHIFTED.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(433, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 54);
            this.label4.TabIndex = 9;
            this.label4.Text = "CPU";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(433, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 54);
            this.label5.TabIndex = 10;
            this.label5.Text = "Firmware";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCPU
            // 
            this.lbCPU.AutoSize = true;
            this.lbCPU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCPU.Location = new System.Drawing.Point(648, 108);
            this.lbCPU.Name = "lbCPU";
            this.lbCPU.Size = new System.Drawing.Size(209, 54);
            this.lbCPU.TabIndex = 11;
            this.lbCPU.Text = "label6";
            this.lbCPU.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFW
            // 
            this.lbFW.AutoSize = true;
            this.lbFW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbFW.Location = new System.Drawing.Point(648, 162);
            this.lbFW.Name = "lbFW";
            this.lbFW.Size = new System.Drawing.Size(209, 54);
            this.lbFW.TabIndex = 12;
            this.lbFW.Text = "label7";
            this.lbFW.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 554);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

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
        private Label label2;
        private Label label3;
        private Label lbCRC;
        private Label lbCRCSHIFTED;
        private Label label4;
        private Label label5;
        private Label lbCPU;
        private Label lbFW;
    }
}