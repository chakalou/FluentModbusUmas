using TrapilModbusUmas.Umas;

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
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            bConnect = new Button();
            tbIP = new TextBox();
            label1 = new Label();
            cbUMASFonction = new ComboBox();
            bSendInfo = new Button();
            label4 = new Label();
            lbCPU = new Label();
            label5 = new Label();
            lbFW = new Label();
            label2 = new Label();
            lbCRC = new Label();
            label3 = new Label();
            lbCRCSHIFTED = new Label();
            dataGridView1 = new DataGridView();
            nameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            relativeOffsetDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            baseoffsetDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            blockMemoryDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            variabletypeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            valeurDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            aPIDictionnaryVariableBindingSource = new BindingSource(components);
            lbLog = new ListBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)aPIDictionnaryVariableBindingSource).BeginInit();
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
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 68.95307F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 31.0469322F));
            tableLayoutPanel1.Size = new Size(866, 554);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 7;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857113F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857151F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857151F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857151F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857151F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857151F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857151F));
            tableLayoutPanel2.Controls.Add(bConnect, 2, 0);
            tableLayoutPanel2.Controls.Add(tbIP, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(bSendInfo, 2, 1);
            tableLayoutPanel2.Controls.Add(label4, 3, 0);
            tableLayoutPanel2.Controls.Add(lbCPU, 4, 0);
            tableLayoutPanel2.Controls.Add(label5, 3, 1);
            tableLayoutPanel2.Controls.Add(lbFW, 4, 1);
            tableLayoutPanel2.Controls.Add(label2, 5, 0);
            tableLayoutPanel2.Controls.Add(lbCRC, 6, 0);
            tableLayoutPanel2.Controls.Add(label3, 5, 1);
            tableLayoutPanel2.Controls.Add(lbCRCSHIFTED, 6, 1);
            tableLayoutPanel2.Controls.Add(dataGridView1, 0, 2);
            tableLayoutPanel2.Controls.Add(cbUMASFonction, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 7;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel2.Size = new Size(860, 376);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // bConnect
            // 
            bConnect.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            bConnect.Location = new Point(247, 7);
            bConnect.Name = "bConnect";
            bConnect.Size = new Size(116, 38);
            bConnect.TabIndex = 0;
            bConnect.Text = "Connect";
            bConnect.UseVisualStyleBackColor = true;
            bConnect.Click += bConnect_Click;
            // 
            // tbIP
            // 
            tbIP.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbIP.Location = new Point(125, 15);
            tbIP.Name = "tbIP";
            tbIP.Size = new Size(116, 23);
            tbIP.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(116, 53);
            label1.TabIndex = 2;
            label1.Text = "IP";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cbUMASFonction
            // 
            cbUMASFonction.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.SetColumnSpan(cbUMASFonction, 2);
            cbUMASFonction.FormattingEnabled = true;
            cbUMASFonction.Location = new Point(3, 68);
            cbUMASFonction.Name = "cbUMASFonction";
            cbUMASFonction.Size = new Size(238, 23);
            cbUMASFonction.TabIndex = 4;
            // 
            // bSendInfo
            // 
            bSendInfo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            bSendInfo.Location = new Point(247, 62);
            bSendInfo.Name = "bSendInfo";
            bSendInfo.Size = new Size(116, 34);
            bSendInfo.TabIndex = 3;
            bSendInfo.Text = "SendRequest";
            bSendInfo.UseVisualStyleBackColor = true;
            bSendInfo.Click += bSendInfo_Click_1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(369, 0);
            label4.Name = "label4";
            label4.Size = new Size(116, 53);
            label4.TabIndex = 9;
            label4.Text = "CPU";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbCPU
            // 
            lbCPU.AutoSize = true;
            lbCPU.Dock = DockStyle.Fill;
            lbCPU.Location = new Point(491, 0);
            lbCPU.Name = "lbCPU";
            lbCPU.Size = new Size(116, 53);
            lbCPU.TabIndex = 11;
            lbCPU.Text = "label6";
            lbCPU.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(369, 53);
            label5.Name = "label5";
            label5.Size = new Size(116, 53);
            label5.TabIndex = 10;
            label5.Text = "Firmware";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbFW
            // 
            lbFW.AutoSize = true;
            lbFW.Dock = DockStyle.Fill;
            lbFW.Location = new Point(491, 53);
            lbFW.Name = "lbFW";
            lbFW.Size = new Size(116, 53);
            lbFW.TabIndex = 12;
            lbFW.Text = "label7";
            lbFW.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(613, 0);
            label2.Name = "label2";
            label2.Size = new Size(116, 53);
            label2.TabIndex = 5;
            label2.Text = "CRC";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbCRC
            // 
            lbCRC.AutoSize = true;
            lbCRC.Dock = DockStyle.Fill;
            lbCRC.Location = new Point(735, 0);
            lbCRC.Name = "lbCRC";
            lbCRC.Size = new Size(122, 53);
            lbCRC.TabIndex = 7;
            lbCRC.Text = "label4";
            lbCRC.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(613, 53);
            label3.Name = "label3";
            label3.Size = new Size(116, 53);
            label3.TabIndex = 6;
            label3.Text = "CRC SHIFTED";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbCRCSHIFTED
            // 
            lbCRCSHIFTED.AutoSize = true;
            lbCRCSHIFTED.Dock = DockStyle.Fill;
            lbCRCSHIFTED.Location = new Point(735, 53);
            lbCRCSHIFTED.Name = "lbCRCSHIFTED";
            lbCRCSHIFTED.Size = new Size(122, 53);
            lbCRCSHIFTED.TabIndex = 8;
            lbCRCSHIFTED.Text = "label5";
            lbCRCSHIFTED.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { nameDataGridViewTextBoxColumn, relativeOffsetDataGridViewTextBoxColumn, baseoffsetDataGridViewTextBoxColumn, blockMemoryDataGridViewTextBoxColumn, variabletypeDataGridViewTextBoxColumn, valeurDataGridViewTextBoxColumn });
            tableLayoutPanel2.SetColumnSpan(dataGridView1, 7);
            dataGridView1.DataSource = aPIDictionnaryVariableBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 109);
            dataGridView1.Name = "dataGridView1";
            tableLayoutPanel2.SetRowSpan(dataGridView1, 5);
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(854, 264);
            dataGridView1.TabIndex = 13;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            nameDataGridViewTextBoxColumn.HeaderText = "Name";
            nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // relativeOffsetDataGridViewTextBoxColumn
            // 
            relativeOffsetDataGridViewTextBoxColumn.DataPropertyName = "RelativeOffset";
            relativeOffsetDataGridViewTextBoxColumn.HeaderText = "RelativeOffset";
            relativeOffsetDataGridViewTextBoxColumn.Name = "relativeOffsetDataGridViewTextBoxColumn";
            relativeOffsetDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // baseoffsetDataGridViewTextBoxColumn
            // 
            baseoffsetDataGridViewTextBoxColumn.DataPropertyName = "Baseoffset";
            baseoffsetDataGridViewTextBoxColumn.HeaderText = "Baseoffset";
            baseoffsetDataGridViewTextBoxColumn.Name = "baseoffsetDataGridViewTextBoxColumn";
            baseoffsetDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // blockMemoryDataGridViewTextBoxColumn
            // 
            blockMemoryDataGridViewTextBoxColumn.DataPropertyName = "BlockMemory";
            blockMemoryDataGridViewTextBoxColumn.HeaderText = "BlockMemory";
            blockMemoryDataGridViewTextBoxColumn.Name = "blockMemoryDataGridViewTextBoxColumn";
            blockMemoryDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // variabletypeDataGridViewTextBoxColumn
            // 
            variabletypeDataGridViewTextBoxColumn.DataPropertyName = "Variabletype";
            variabletypeDataGridViewTextBoxColumn.HeaderText = "Variabletype";
            variabletypeDataGridViewTextBoxColumn.Name = "variabletypeDataGridViewTextBoxColumn";
            variabletypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // valeurDataGridViewTextBoxColumn
            // 
            valeurDataGridViewTextBoxColumn.DataPropertyName = "Valeur";
            valeurDataGridViewTextBoxColumn.HeaderText = "Valeur";
            valeurDataGridViewTextBoxColumn.Name = "valeurDataGridViewTextBoxColumn";
            // 
            // aPIDictionnaryVariableBindingSource
            // 
            aPIDictionnaryVariableBindingSource.DataSource = typeof(APIDictionnaryVariable);
            // 
            // lbLog
            // 
            lbLog.Dock = DockStyle.Fill;
            lbLog.FormattingEnabled = true;
            lbLog.ItemHeight = 15;
            lbLog.Location = new Point(3, 385);
            lbLog.Name = "lbLog";
            lbLog.Size = new Size(860, 166);
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
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)aPIDictionnaryVariableBindingSource).EndInit();
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
        private Label label2;
        private Label label3;
        private Label lbCRC;
        private Label lbCRCSHIFTED;
        private Label label4;
        private Label label5;
        private Label lbCPU;
        private Label lbFW;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn relativeOffsetDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn baseoffsetDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn blockMemoryDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn variabletypeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn valeurDataGridViewTextBoxColumn;
        private BindingSource aPIDictionnaryVariableBindingSource;
    }
}