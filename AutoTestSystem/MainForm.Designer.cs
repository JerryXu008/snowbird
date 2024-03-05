namespace AutoTestSystem
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripContinuFailNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_ContinuousFailNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.ContinuousFailReset = new System.Windows.Forms.ToolStripStatusLabel();
            this.runTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTestTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lable_totalpass = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_passNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_totalFail = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_FailNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_totalAbort = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_AbortNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.yieldLable = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_YieldNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.lb_loopTestStatistics = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelButton = new System.Windows.Forms.Panel();
            this.lb_IPaddress = new System.Windows.Forms.Label();
            this.bt_debug = new System.Windows.Forms.CheckBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonBegin = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_StationNo = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_mode = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_testMode = new System.Windows.Forms.Label();
            this.logButton = new System.Windows.Forms.Button();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.treeViewSeq = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.dataGridViewDetail = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bt_errorCode = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_failCount = new System.Windows.Forms.Label();
            this.bt_Status = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripRightKey = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ConsumbleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConsumbleMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.OneStepTestMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全不选ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全选ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStripRightKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.toolStripContinuFailNum,
        this.lb_ContinuousFailNum,
        this.ContinuousFailReset,
        this.runTime,
        this.toolStripTestTime,
        this.lable_totalpass,
        this.lb_passNum,
        this.lb_totalFail,
        this.lb_FailNum,
        this.lb_totalAbort,
        this.lb_AbortNum,
        this.yieldLable,
        this.lb_YieldNum,
        this.lb_loopTestStatistics});
            this.statusStrip1.Location = new System.Drawing.Point(0, 577);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1096, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripContinuFailNum
            // 
            this.toolStripContinuFailNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.toolStripContinuFailNum.Name = "toolStripContinuFailNum";
            this.toolStripContinuFailNum.Size = new System.Drawing.Size(121, 17);
            this.toolStripContinuFailNum.Text = "   ContinuousFail: ";
            this.toolStripContinuFailNum.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_ContinuousFailNum
            // 
            this.lb_ContinuousFailNum.Name = "lb_ContinuousFailNum";
            this.lb_ContinuousFailNum.Size = new System.Drawing.Size(15, 17);
            this.lb_ContinuousFailNum.Text = "0";
            // 
            // ContinuousFailReset
            // 
            this.ContinuousFailReset.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ContinuousFailReset.ForeColor = System.Drawing.Color.Blue;
            this.ContinuousFailReset.Name = "ContinuousFailReset";
            this.ContinuousFailReset.Size = new System.Drawing.Size(44, 21);
            this.ContinuousFailReset.Text = "Reset";
            this.ContinuousFailReset.Visible = false;
            this.ContinuousFailReset.Click += new System.EventHandler(this.ContinuousFailReset_Click);
            // 
            // runTime
            // 
            this.runTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.runTime.Name = "runTime";
            this.runTime.Size = new System.Drawing.Size(87, 17);
            this.runTime.Text = "   TestTime: ";
            // 
            // toolStripTestTime
            // 
            this.toolStripTestTime.Name = "toolStripTestTime";
            this.toolStripTestTime.Size = new System.Drawing.Size(21, 17);
            this.toolStripTestTime.Text = "0s";
            // 
            // lable_totalpass
            // 
            this.lable_totalpass.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lable_totalpass.Name = "lable_totalpass";
            this.lable_totalpass.Size = new System.Drawing.Size(104, 17);
            this.lable_totalpass.Text = "   Total_PASS: ";
            this.lable_totalpass.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_passNum
            // 
            this.lb_passNum.Name = "lb_passNum";
            this.lb_passNum.Size = new System.Drawing.Size(15, 17);
            this.lb_passNum.Text = "0";
            // 
            // lb_totalFail
            // 
            this.lb_totalFail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lb_totalFail.Name = "lb_totalFail";
            this.lb_totalFail.Size = new System.Drawing.Size(96, 17);
            this.lb_totalFail.Text = "   Total_FAIL: ";
            this.lb_totalFail.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_FailNum
            // 
            this.lb_FailNum.Name = "lb_FailNum";
            this.lb_FailNum.Size = new System.Drawing.Size(15, 17);
            this.lb_FailNum.Text = "0";
            // 
            // lb_totalAbort
            // 
            this.lb_totalAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lb_totalAbort.Name = "lb_totalAbort";
            this.lb_totalAbort.Size = new System.Drawing.Size(102, 17);
            this.lb_totalAbort.Text = "   Total_Abort: ";
            // 
            // lb_AbortNum
            // 
            this.lb_AbortNum.Name = "lb_AbortNum";
            this.lb_AbortNum.Size = new System.Drawing.Size(15, 17);
            this.lb_AbortNum.Text = "0";
            // 
            // yieldLable
            // 
            this.yieldLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.yieldLable.Name = "yieldLable";
            this.yieldLable.Size = new System.Drawing.Size(59, 17);
            this.yieldLable.Text = "   Yield: ";
            // 
            // lb_YieldNum
            // 
            this.lb_YieldNum.Name = "lb_YieldNum";
            this.lb_YieldNum.Size = new System.Drawing.Size(43, 17);
            this.lb_YieldNum.Text = "0.00%";
            // 
            // lb_loopTestStatistics
            // 
            this.lb_loopTestStatistics.Name = "lb_loopTestStatistics";
            this.lb_loopTestStatistics.Size = new System.Drawing.Size(0, 17);
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.panelButton, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.splitContainerMain, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 306F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1096, 577);
            this.tableLayoutPanelMain.TabIndex = 3;
            // 
            // panelButton
            // 
            this.panelButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelButton.Controls.Add(this.lb_IPaddress);
            this.panelButton.Controls.Add(this.bt_debug);
            this.panelButton.Controls.Add(this.buttonExit);
            this.panelButton.Controls.Add(this.buttonBegin);
            this.panelButton.Controls.Add(this.groupBox3);
            this.panelButton.Controls.Add(this.groupBox2);
            this.panelButton.Controls.Add(this.groupBox1);
            this.panelButton.Controls.Add(this.logButton);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButton.Location = new System.Drawing.Point(0, 0);
            this.panelButton.Margin = new System.Windows.Forms.Padding(0);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(1096, 50);
            this.panelButton.TabIndex = 3;
            // 
            // lb_IPaddress
            // 
            this.lb_IPaddress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_IPaddress.AutoSize = true;
            this.lb_IPaddress.BackColor = System.Drawing.Color.Transparent;
            this.lb_IPaddress.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_IPaddress.ForeColor = System.Drawing.Color.Black;
            this.lb_IPaddress.Location = new System.Drawing.Point(726, 11);
            this.lb_IPaddress.Name = "lb_IPaddress";
            this.lb_IPaddress.Size = new System.Drawing.Size(97, 19);
            this.lb_IPaddress.TabIndex = 1;
            this.lb_IPaddress.Text = "LocalIP:";
            this.lb_IPaddress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bt_debug
            // 
            this.bt_debug.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_debug.ForeColor = System.Drawing.Color.Red;
            this.bt_debug.Location = new System.Drawing.Point(610, 11);
            this.bt_debug.Margin = new System.Windows.Forms.Padding(2);
            this.bt_debug.Name = "bt_debug";
            this.bt_debug.Size = new System.Drawing.Size(101, 21);
            this.bt_debug.TabIndex = 1;
            this.bt_debug.Text = "调试";
            this.bt_debug.UseVisualStyleBackColor = true;
            this.bt_debug.Visible = false;
            this.bt_debug.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonExit.AutoSize = true;
            this.buttonExit.BackColor = System.Drawing.Color.Transparent;
            this.buttonExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonExit.ForeColor = System.Drawing.Color.Transparent;
            this.buttonExit.Image = global::AutoTestSystem.Properties.Resources.close;
            this.buttonExit.Location = new System.Drawing.Point(1011, 14);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(25, 25);
            this.buttonExit.TabIndex = 14;
            this.buttonExit.UseVisualStyleBackColor = false;
            this.buttonExit.Visible = false;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // buttonBegin
            // 
            this.buttonBegin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonBegin.AutoSize = true;
            this.buttonBegin.BackColor = System.Drawing.Color.Transparent;
            this.buttonBegin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonBegin.FlatAppearance.BorderSize = 0;
            this.buttonBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBegin.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonBegin.ForeColor = System.Drawing.Color.Transparent;
            this.buttonBegin.Image = global::AutoTestSystem.Properties.Resources.start;
            this.buttonBegin.Location = new System.Drawing.Point(967, 14);
            this.buttonBegin.Name = "buttonBegin";
            this.buttonBegin.Size = new System.Drawing.Size(25, 25);
            this.buttonBegin.TabIndex = 13;
            this.buttonBegin.UseVisualStyleBackColor = false;
            this.buttonBegin.Visible = false;
            this.buttonBegin.Click += new System.EventHandler(this.ButtonBegin_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lbl_StationNo);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(211, 2);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 8, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(112, 45);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "StationNo";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(38, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 19);
            this.label1.TabIndex = 1;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_StationNo
            // 
            this.lbl_StationNo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_StationNo.AutoSize = true;
            this.lbl_StationNo.BackColor = System.Drawing.Color.Transparent;
            this.lbl_StationNo.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_StationNo.ForeColor = System.Drawing.Color.Black;
            this.lbl_StationNo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_StationNo.Location = new System.Drawing.Point(6, 18);
            this.lbl_StationNo.Name = "lbl_StationNo";
            this.lbl_StationNo.Size = new System.Drawing.Size(53, 19);
            this.lbl_StationNo.TabIndex = 1;
            this.lbl_StationNo.Text = "XXXX";
            this.lbl_StationNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lb_mode);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(14, 2);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(112, 45);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mode";
            // 
            // lb_mode
            // 
            this.lb_mode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_mode.AutoSize = true;
            this.lb_mode.BackColor = System.Drawing.Color.Transparent;
            this.lb_mode.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_mode.ForeColor = System.Drawing.Color.Black;
            this.lb_mode.Location = new System.Drawing.Point(5, 18);
            this.lb_mode.Name = "lb_mode";
            this.lb_mode.Size = new System.Drawing.Size(86, 19);
            this.lb_mode.TabIndex = 1;
            this.lb_mode.Text = "Snowbird";
            this.lb_mode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.lbl_testMode);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(408, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(158, 45);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TestMode";
            // 
            // lbl_testMode
            // 
            this.lbl_testMode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_testMode.AutoSize = true;
            this.lbl_testMode.BackColor = System.Drawing.Color.Transparent;
            this.lbl_testMode.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_testMode.ForeColor = System.Drawing.Color.Black;
            this.lbl_testMode.Location = new System.Drawing.Point(5, 18);
            this.lbl_testMode.Name = "lbl_testMode";
            this.lbl_testMode.Size = new System.Drawing.Size(97, 19);
            this.lbl_testMode.TabIndex = 1;
            this.lbl_testMode.Text = "TESTMODE";
            this.lbl_testMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logButton
            // 
            this.logButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.logButton.BackColor = System.Drawing.Color.Transparent;
            this.logButton.FlatAppearance.BorderSize = 0;
            this.logButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.logButton.ForeColor = System.Drawing.Color.White;
            this.logButton.Image = global::AutoTestSystem.Properties.Resources.Openlog;
            this.logButton.Location = new System.Drawing.Point(1055, 14);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(25, 25);
            this.logButton.TabIndex = 2;
            this.logButton.UseVisualStyleBackColor = false;
            this.logButton.Click += new System.EventHandler(this.LogButton_Click);
            this.logButton.MouseEnter += new System.EventHandler(this.LogButton_MouseEnter);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(3, 53);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.treeViewSeq);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainerMain.Panel2.Controls.Add(this.dataGridViewDetail);
            this.splitContainerMain.Size = new System.Drawing.Size(1090, 215);
            this.splitContainerMain.SplitterDistance = 252;
            this.splitContainerMain.TabIndex = 8;
            // 
            // treeViewSeq
            // 
            this.treeViewSeq.BackColor = System.Drawing.Color.White;
            this.treeViewSeq.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewSeq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSeq.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewSeq.ForeColor = System.Drawing.Color.Black;
            this.treeViewSeq.FullRowSelect = true;
            this.treeViewSeq.ImageIndex = 0;
            this.treeViewSeq.ImageList = this.imageList1;
            this.treeViewSeq.Indent = 20;
            this.treeViewSeq.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(182)))), ((int)(((byte)(182)))));
            this.treeViewSeq.Location = new System.Drawing.Point(0, 0);
            this.treeViewSeq.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewSeq.Name = "treeViewSeq";
            this.treeViewSeq.SelectedImageIndex = 0;
            this.treeViewSeq.ShowNodeToolTips = true;
            this.treeViewSeq.Size = new System.Drawing.Size(252, 215);
            this.treeViewSeq.TabIndex = 9;
            this.treeViewSeq.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewSeq_AfterCheck);
            this.treeViewSeq.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewSeq_AfterSelect);
            this.treeViewSeq.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewSeq_NodeMouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "refactorings_obj.gif");
            this.imageList1.Images.SetKeyName(1, "filterfolder.gif");
            // 
            // dataGridViewDetail
            // 
            this.dataGridViewDetail.AllowUserToAddRows = false;
            this.dataGridViewDetail.AllowUserToDeleteRows = false;
            this.dataGridViewDetail.AllowUserToResizeRows = false;
            this.dataGridViewDetail.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewDetail.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dataGridViewDetail.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridViewDetail.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewDetail.ColumnHeadersHeight = 40;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDetail.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDetail.EnableHeadersVisualStyles = false;
            this.dataGridViewDetail.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(182)))), ((int)(((byte)(182)))));
            this.dataGridViewDetail.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewDetail.Name = "dataGridViewDetail";
            this.dataGridViewDetail.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewDetail.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewDetail.RowHeadersVisible = false;
            this.dataGridViewDetail.RowHeadersWidth = 20;
            this.dataGridViewDetail.RowTemplate.Height = 23;
            this.dataGridViewDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDetail.Size = new System.Drawing.Size(834, 215);
            this.dataGridViewDetail.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 274);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(1090, 300);
            this.splitContainer1.SplitterDistance = 252;
            this.splitContainer1.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.bt_errorCode, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(250, 298);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // bt_errorCode
            // 
            this.bt_errorCode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bt_errorCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_errorCode.Enabled = false;
            this.bt_errorCode.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_errorCode.Location = new System.Drawing.Point(3, 175);
            this.bt_errorCode.Name = "bt_errorCode";
            this.bt_errorCode.Size = new System.Drawing.Size(244, 120);
            this.bt_errorCode.TabIndex = 0;
            this.bt_errorCode.Text = "ErrorCode";
            this.bt_errorCode.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 128);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(244, 41);
            this.panel1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(37, 7);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(202, 26);
            this.textBox1.TabIndex = 3;

            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(5, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "SN:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbl_failCount);
            this.panel2.Controls.Add(this.bt_Status);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(244, 119);
            this.panel2.TabIndex = 2;
            // 
            // lbl_failCount
            // 
            this.lbl_failCount.AutoEllipsis = true;
           // this.lbl_failCount.AutoSize = true;
            this.lbl_failCount.BackColor = System.Drawing.Color.White;
            this.lbl_failCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_failCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbl_failCount.Location = new System.Drawing.Point(0, 69);
            this.lbl_failCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_failCount.Name = "lbl_failCount";
            this.lbl_failCount.Size = new System.Drawing.Size(259, 27);
            this.lbl_failCount.TabIndex = 1;
            this.lbl_failCount.Text = "Next:O-SFT /Current:O-RUNIN";
            this.lbl_failCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_failCount.UseCompatibleTextRendering = true;
            this.lbl_failCount.Visible = false;
            // 
            // bt_Status
            // 
            this.bt_Status.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bt_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_Status.Enabled = false;
            this.bt_Status.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_Status.Font = new System.Drawing.Font("宋体", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_Status.Location = new System.Drawing.Point(0, 0);
            this.bt_Status.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Status.Name = "bt_Status";
            this.bt_Status.Size = new System.Drawing.Size(244, 119);
            this.bt_Status.TabIndex = 0;
            this.bt_Status.Text = "Standby";
            this.bt_Status.UseVisualStyleBackColor = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(832, 298);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(56, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "ErrorCode";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // contextMenuStripRightKey
            // 
            this.contextMenuStripRightKey.ImageScalingSize = new System.Drawing.Size(20, 20);
            
            
            
            this.contextMenuStripRightKey.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                  this.ConsumbleMenuItem,
                  this.ConsumbleMenuItem2,
        this.OneStepTestMenuItem,
        this.CycleToolStripMenuItem,
        this.全不选ToolStripMenuItem,
        this.全选ToolStripMenuItem});





            this.contextMenuStripRightKey.Name = "contextMenuStripRightKey";
            this.contextMenuStripRightKey.Size = new System.Drawing.Size(125, 92);


            // 
            //  ConsumbleMenuItem
            // 
            this.ConsumbleMenuItem.Name = "ConsumbleMenuItem";
            this.ConsumbleMenuItem.Size = new System.Drawing.Size(124, 22);
            this.ConsumbleMenuItem.Text = "耗材查看";
            this.ConsumbleMenuItem.Click += new System.EventHandler(this.SeeConsumeMenuItem_Click);




            // 
            //  ConsumbleMenuItem
            // 
            this.ConsumbleMenuItem2.Name = "ConsumbleMenuItem2";
            this.ConsumbleMenuItem2.Size = new System.Drawing.Size(124, 22);
            this.ConsumbleMenuItem2.Text = "模拟摄像头";
            this.ConsumbleMenuItem2.Click += new System.EventHandler(this.SeeConsumeMenuItem_Click2);



            // 
            // OneStepTestMenuItem
            // 
            this.OneStepTestMenuItem.Name = "OneStepTestMenuItem";
            this.OneStepTestMenuItem.Size = new System.Drawing.Size(124, 22);
            this.OneStepTestMenuItem.Text = "单步测试";
            this.OneStepTestMenuItem.Click += new System.EventHandler(this.OneStepTestMenuItem_Click);
            // 
            // CycleToolStripMenuItem
            // 
            this.CycleToolStripMenuItem.Name = "CycleToolStripMenuItem";
            this.CycleToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.CycleToolStripMenuItem.Text = "循环测试";
            this.CycleToolStripMenuItem.Click += new System.EventHandler(this.CycleToolStripMenuItem_Click);
            // 
            // 全不选ToolStripMenuItem
            // 
            this.全不选ToolStripMenuItem.Name = "全不选ToolStripMenuItem";
            this.全不选ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.全不选ToolStripMenuItem.Text = "全不选";
            this.全不选ToolStripMenuItem.Click += new System.EventHandler(this.全不选ToolStripMenuItem_Click);
            // 
            // 全选ToolStripMenuItem
            // 
            this.全选ToolStripMenuItem.Name = "全选ToolStripMenuItem";
            this.全选ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.全选ToolStripMenuItem.Text = "全选";
            this.全选ToolStripMenuItem.Click += new System.EventHandler(this.全选ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1096, 599);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoTestSystem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelButton.ResumeLayout(false);
            this.panelButton.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetail)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.contextMenuStripRightKey.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripContinuFailNum;
        private System.Windows.Forms.ToolStripStatusLabel lb_ContinuousFailNum;
        private System.Windows.Forms.ToolStripStatusLabel ContinuousFailReset;
        private System.Windows.Forms.ToolStripStatusLabel runTime;
        private System.Windows.Forms.ToolStripStatusLabel lable_totalpass;
        private System.Windows.Forms.ToolStripStatusLabel lb_passNum;
        private System.Windows.Forms.ToolStripStatusLabel lb_totalFail;
        private System.Windows.Forms.ToolStripStatusLabel lb_FailNum;
        private System.Windows.Forms.ToolStripStatusLabel yieldLable;
        private System.Windows.Forms.ToolStripStatusLabel lb_YieldNum;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.CheckBox bt_debug;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonBegin;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_StationNo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lb_mode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbl_testMode;
        private System.Windows.Forms.Button logButton;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TreeView treeViewSeq;
        private System.Windows.Forms.DataGridView dataGridViewDetail;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button bt_errorCode;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_failCount;
        private System.Windows.Forms.Button bt_Status;
        public System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRightKey;
        private System.Windows.Forms.ToolStripMenuItem ConsumbleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConsumbleMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem OneStepTestMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CycleToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem 全不选ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全选ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lb_totalAbort;
        private System.Windows.Forms.ToolStripStatusLabel lb_AbortNum;
        private System.Windows.Forms.ToolStripStatusLabel toolStripTestTime;
        private System.Windows.Forms.ToolStripStatusLabel lb_loopTestStatistics;
        private System.Windows.Forms.Label lb_IPaddress;

        #endregion
        //// private System.Windows.Forms.Button button1;

    }

}