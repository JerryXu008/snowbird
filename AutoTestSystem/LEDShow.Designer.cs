namespace AutoTestSystem
{
    partial class LEDSHow
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblYes = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblNo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // lblYes
            // 
            this.lblYes.Location = new System.Drawing.Point(975, 159);
            this.lblYes.Margin = new System.Windows.Forms.Padding(4);
            this.lblYes.Name = "lblYes";
            this.lblYes.Size = new System.Drawing.Size(171, 85);
            this.lblYes.TabIndex = 2;
            this.lblYes.Text = "Yes";
            this.lblYes.UseVisualStyleBackColor = true;
            this.lblYes.Click += new System.EventHandler(this.确定_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::AutoTestSystem.Properties.Resources.whiteled;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1159, 685);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblNo
            // 
            this.lblNo.Location = new System.Drawing.Point(975, 316);
            this.lblNo.Margin = new System.Windows.Forms.Padding(4);
            this.lblNo.Name = "lblNo";
            this.lblNo.Size = new System.Drawing.Size(171, 85);
            this.lblNo.TabIndex = 3;
            this.lblNo.Text = "No";
            this.lblNo.UseVisualStyleBackColor = true;
            this.lblNo.Click += new System.EventHandler(this.lblNo_Click);
            // 
            // LEDSHow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1159, 685);
            this.Controls.Add(this.lblNo);
            this.Controls.Add(this.lblYes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LEDSHow";
            this.Text = "LEDConfirmDialog";
            this.Load += new System.EventHandler(this.USBConfirmDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button lblYes;
        private System.Windows.Forms.Button lblNo;
    }
}