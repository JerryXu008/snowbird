namespace AutoTestSystem
{
    partial class ConsumalForm
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
            this.lblCableText = new System.Windows.Forms.Label();
            this.btnCable = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblCableNum = new System.Windows.Forms.Label();
            this.lblETHNum = new System.Windows.Forms.Label();
            this.lblETHText = new System.Windows.Forms.Label();
            this.lblTypeCNum = new System.Windows.Forms.Label();
            this.lblTypeCText = new System.Windows.Forms.Label();
            this.btnETH = new System.Windows.Forms.Button();
            this.btnTypeC = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCableText
            // 
            this.lblCableText.AutoSize = true;
            this.lblCableText.Location = new System.Drawing.Point(30, 38);
            this.lblCableText.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblCableText.Name = "lblCableText";
            this.lblCableText.Size = new System.Drawing.Size(310, 24);
            this.lblCableText.TabIndex = 1;
            this.lblCableText.Text = "Num of Cable Consumables:";
            this.lblCableText.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnCable
            // 
            this.btnCable.Location = new System.Drawing.Point(469, 22);
            this.btnCable.Margin = new System.Windows.Forms.Padding(6);
            this.btnCable.Name = "btnCable";
            this.btnCable.Size = new System.Drawing.Size(204, 56);
            this.btnCable.TabIndex = 2;
            this.btnCable.Text = "Cable Clear";
            this.btnCable.UseVisualStyleBackColor = true;
            this.btnCable.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(293, 284);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(204, 56);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblCableNum
            // 
            this.lblCableNum.AutoSize = true;
            this.lblCableNum.Location = new System.Drawing.Point(404, 38);
            this.lblCableNum.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblCableNum.Name = "lblCableNum";
            this.lblCableNum.Size = new System.Drawing.Size(22, 24);
            this.lblCableNum.TabIndex = 4;
            this.lblCableNum.Text = "0";
            // 
            // lblETHNum
            // 
            this.lblETHNum.AutoSize = true;
            this.lblETHNum.Location = new System.Drawing.Point(404, 38);
            this.lblETHNum.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblETHNum.Name = "lblETHNum";
            this.lblETHNum.Size = new System.Drawing.Size(22, 24);
            this.lblETHNum.TabIndex = 6;
            this.lblETHNum.Text = "0";
            // 
            // lblETHText
            // 
            this.lblETHText.AutoSize = true;
            this.lblETHText.Location = new System.Drawing.Point(30, 38);
            this.lblETHText.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblETHText.Name = "lblETHText";
            this.lblETHText.Size = new System.Drawing.Size(286, 24);
            this.lblETHText.TabIndex = 5;
            this.lblETHText.Text = "Num of ETH Consumables:";
            // 
            // lblTypeCNum
            // 
            this.lblTypeCNum.AutoSize = true;
            this.lblTypeCNum.Location = new System.Drawing.Point(404, 121);
            this.lblTypeCNum.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTypeCNum.Name = "lblTypeCNum";
            this.lblTypeCNum.Size = new System.Drawing.Size(22, 24);
            this.lblTypeCNum.TabIndex = 8;
            this.lblTypeCNum.Text = "0";
            // 
            // lblTypeCText
            // 
            this.lblTypeCText.AutoSize = true;
            this.lblTypeCText.Location = new System.Drawing.Point(30, 121);
            this.lblTypeCText.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTypeCText.Name = "lblTypeCText";
            this.lblTypeCText.Size = new System.Drawing.Size(310, 24);
            this.lblTypeCText.TabIndex = 7;
            this.lblTypeCText.Text = "Num of TypeC Consumables:";
            // 
            // btnETH
            // 
            this.btnETH.Location = new System.Drawing.Point(469, 22);
            this.btnETH.Margin = new System.Windows.Forms.Padding(6);
            this.btnETH.Name = "btnETH";
            this.btnETH.Size = new System.Drawing.Size(204, 56);
            this.btnETH.TabIndex = 9;
            this.btnETH.Text = "ETH Clear";
            this.btnETH.UseVisualStyleBackColor = true;
            this.btnETH.Click += new System.EventHandler(this.btnETH_Click);
            // 
            // btnTypeC
            // 
            this.btnTypeC.Location = new System.Drawing.Point(469, 105);
            this.btnTypeC.Margin = new System.Windows.Forms.Padding(6);
            this.btnTypeC.Name = "btnTypeC";
            this.btnTypeC.Size = new System.Drawing.Size(204, 56);
            this.btnTypeC.TabIndex = 10;
            this.btnTypeC.Text = "TypeC Clear";
            this.btnTypeC.UseVisualStyleBackColor = true;
            this.btnTypeC.Click += new System.EventHandler(this.btnTypeC_Click);
            // 
            // ConsumalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 521);
            this.Controls.Add(this.btnTypeC);
            this.Controls.Add(this.btnETH);
            this.Controls.Add(this.lblTypeCNum);
            this.Controls.Add(this.lblTypeCText);
            this.Controls.Add(this.lblETHNum);
            this.Controls.Add(this.lblETHText);
            this.Controls.Add(this.lblCableNum);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCable);
            this.Controls.Add(this.lblCableText);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ConsumalForm";
            this.Text = "Consumable";
            this.Load += new System.EventHandler(this.PassWordForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion
        private System.Windows.Forms.Label lblCableText;
        private System.Windows.Forms.Button btnCable;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblCableNum;
        private System.Windows.Forms.Label lblETHNum;
        private System.Windows.Forms.Label lblETHText;
        private System.Windows.Forms.Label lblTypeCNum;
        private System.Windows.Forms.Label lblTypeCText;
        private System.Windows.Forms.Button btnETH;
        private System.Windows.Forms.Button btnTypeC;
    }
}