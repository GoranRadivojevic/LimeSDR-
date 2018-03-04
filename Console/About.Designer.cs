namespace PowerSDR
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblRadioModel = new System.Windows.Forms.LabelTS();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblSerialNumber = new System.Windows.Forms.LabelTS();
            this.lblSerialNo = new System.Windows.Forms.Label();
            this.lblFirm_version = new System.Windows.Forms.LabelTS();
            this.labelTS1 = new System.Windows.Forms.LabelTS();
            this.lblPowerSDR = new System.Windows.Forms.Label();
            this.lblGenesis = new System.Windows.Forms.Label();
            this.lblFIRMWARE = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.buttonOK.ForeColor = System.Drawing.Color.Yellow;
            this.buttonOK.Location = new System.Drawing.Point(161, 379);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(92, 37);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblRadioModel);
            this.groupBox1.Controls.Add(this.lblModel);
            this.groupBox1.Controls.Add(this.lblSerialNumber);
            this.groupBox1.Controls.Add(this.lblSerialNo);
            this.groupBox1.Controls.Add(this.lblFirm_version);
            this.groupBox1.Controls.Add(this.labelTS1);
            this.groupBox1.Controls.Add(this.lblPowerSDR);
            this.groupBox1.Controls.Add(this.lblGenesis);
            this.groupBox1.Controls.Add(this.lblFIRMWARE);
            this.groupBox1.ForeColor = System.Drawing.Color.Yellow;
            this.groupBox1.Location = new System.Drawing.Point(29, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 325);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // lblRadioModel
            // 
            this.lblRadioModel.AutoSize = true;
            this.lblRadioModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRadioModel.Image = null;
            this.lblRadioModel.Location = new System.Drawing.Point(139, 153);
            this.lblRadioModel.Name = "lblRadioModel";
            this.lblRadioModel.Size = new System.Drawing.Size(78, 20);
            this.lblRadioModel.TabIndex = 12;
            this.lblRadioModel.Text = "LimeSDR";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModel.Location = new System.Drawing.Point(112, 123);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(134, 24);
            this.lblModel.TabIndex = 11;
            this.lblModel.Text = "Radio model:";
            // 
            // lblSerialNumber
            // 
            this.lblSerialNumber.AutoSize = true;
            this.lblSerialNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSerialNumber.Image = null;
            this.lblSerialNumber.Location = new System.Drawing.Point(160, 265);
            this.lblSerialNumber.Name = "lblSerialNumber";
            this.lblSerialNumber.Size = new System.Drawing.Size(36, 20);
            this.lblSerialNumber.TabIndex = 8;
            this.lblSerialNumber.Text = "000";
            this.lblSerialNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSerialNo.Location = new System.Drawing.Point(105, 235);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(147, 24);
            this.lblSerialNo.TabIndex = 7;
            this.lblSerialNo.Text = "Serial number:";
            // 
            // lblFirm_version
            // 
            this.lblFirm_version.AutoSize = true;
            this.lblFirm_version.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirm_version.Image = null;
            this.lblFirm_version.Location = new System.Drawing.Point(156, 209);
            this.lblFirm_version.Name = "lblFirm_version";
            this.lblFirm_version.Size = new System.Drawing.Size(44, 20);
            this.lblFirm_version.TabIndex = 6;
            this.lblFirm_version.Text = "0.0.0";
            // 
            // labelTS1
            // 
            this.labelTS1.AutoSize = true;
            this.labelTS1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTS1.Image = null;
            this.labelTS1.Location = new System.Drawing.Point(116, 97);
            this.labelTS1.Name = "labelTS1";
            this.labelTS1.Size = new System.Drawing.Size(125, 20);
            this.labelTS1.TabIndex = 4;
            this.labelTS1.Text = "from Flex Radio)";
            // 
            // lblPowerSDR
            // 
            this.lblPowerSDR.AutoSize = true;
            this.lblPowerSDR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPowerSDR.Location = new System.Drawing.Point(95, 71);
            this.lblPowerSDR.Name = "lblPowerSDR";
            this.lblPowerSDR.Size = new System.Drawing.Size(167, 20);
            this.lblPowerSDR.TabIndex = 3;
            this.lblPowerSDR.Text = "(based on PowerSDR ";
            // 
            // lblGenesis
            // 
            this.lblGenesis.AutoSize = true;
            this.lblGenesis.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenesis.Location = new System.Drawing.Point(27, 40);
            this.lblGenesis.Name = "lblGenesis";
            this.lblGenesis.Size = new System.Drawing.Size(303, 25);
            this.lblGenesis.TabIndex = 2;
            this.lblGenesis.Text = "LimeSDR#  0.1 by YT7PWR";
            // 
            // lblFIRMWARE
            // 
            this.lblFIRMWARE.AutoSize = true;
            this.lblFIRMWARE.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFIRMWARE.Location = new System.Drawing.Point(90, 179);
            this.lblFIRMWARE.Name = "lblFIRMWARE";
            this.lblFIRMWARE.Size = new System.Drawing.Size(177, 24);
            this.lblFIRMWARE.TabIndex = 1;
            this.lblFIRMWARE.Text = "Firmware version:";
            // 
            // About
            // 
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(407, 442);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(423, 480);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(423, 480);
            this.Name = "About";
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblGenesis;
        private System.Windows.Forms.Label lblFIRMWARE;
        private System.Windows.Forms.Label lblPowerSDR;
        private System.Windows.Forms.LabelTS labelTS1;
        private System.Windows.Forms.LabelTS lblFirm_version;
        private System.Windows.Forms.Label lblSerialNo;
        private System.Windows.Forms.LabelTS lblSerialNumber;
        private System.Windows.Forms.LabelTS lblRadioModel;
        private System.Windows.Forms.Label lblModel;
    }
}