namespace PowerSDR
{
    partial class DebugForm
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
            this.rtbDebugMsg = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.ButtonTS();
            this.btnSave = new System.Windows.Forms.ButtonTS();
            this.chkAudio = new System.Windows.Forms.CheckBoxTS();
            this.chkDirectX = new System.Windows.Forms.CheckBoxTS();
            this.chkCAT = new System.Windows.Forms.CheckBoxTS();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonTS1 = new System.Windows.Forms.ButtonTS();
            this.chkConsole = new System.Windows.Forms.CheckBoxTS();
            this.chkIRRemote = new System.Windows.Forms.CheckBoxTS();
            this.SuspendLayout();
            // 
            // rtbDebugMsg
            // 
            this.rtbDebugMsg.Location = new System.Drawing.Point(88, 13);
            this.rtbDebugMsg.MaxLength = 65536;
            this.rtbDebugMsg.Name = "rtbDebugMsg";
            this.rtbDebugMsg.Size = new System.Drawing.Size(417, 354);
            this.rtbDebugMsg.TabIndex = 0;
            this.rtbDebugMsg.Text = "";
            // 
            // btnClear
            // 
            this.btnClear.Image = null;
            this.btnClear.Location = new System.Drawing.Point(273, 381);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = null;
            this.btnSave.Location = new System.Drawing.Point(178, 381);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkAudio
            // 
            this.chkAudio.AutoSize = true;
            this.chkAudio.Image = null;
            this.chkAudio.Location = new System.Drawing.Point(15, 29);
            this.chkAudio.Name = "chkAudio";
            this.chkAudio.Size = new System.Drawing.Size(53, 17);
            this.chkAudio.TabIndex = 3;
            this.chkAudio.Text = "Audio";
            this.chkAudio.UseVisualStyleBackColor = true;
            this.chkAudio.CheckedChanged += new System.EventHandler(this.chkAudio_CheckedChanged);
            // 
            // chkDirectX
            // 
            this.chkDirectX.AutoSize = true;
            this.chkDirectX.Image = null;
            this.chkDirectX.Location = new System.Drawing.Point(15, 73);
            this.chkDirectX.Name = "chkDirectX";
            this.chkDirectX.Size = new System.Drawing.Size(61, 17);
            this.chkDirectX.TabIndex = 4;
            this.chkDirectX.Text = "DirectX";
            this.chkDirectX.UseVisualStyleBackColor = true;
            this.chkDirectX.CheckedChanged += new System.EventHandler(this.chkDirectX_CheckedChanged);
            // 
            // chkCAT
            // 
            this.chkCAT.AutoSize = true;
            this.chkCAT.Image = null;
            this.chkCAT.Location = new System.Drawing.Point(15, 117);
            this.chkCAT.Name = "chkCAT";
            this.chkCAT.Size = new System.Drawing.Size(47, 17);
            this.chkCAT.TabIndex = 5;
            this.chkCAT.Text = "CAT";
            this.chkCAT.UseVisualStyleBackColor = true;
            this.chkCAT.CheckedChanged += new System.EventHandler(this.chkCAT_CheckedChanged);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // buttonTS1
            // 
            this.buttonTS1.Image = null;
            this.buttonTS1.Location = new System.Drawing.Point(12, 304);
            this.buttonTS1.Name = "buttonTS1";
            this.buttonTS1.Size = new System.Drawing.Size(54, 53);
            this.buttonTS1.TabIndex = 6;
            this.buttonTS1.Text = "Read VGA cfg.";
            this.buttonTS1.UseVisualStyleBackColor = true;
            this.buttonTS1.Click += new System.EventHandler(this.buttonTS1_Click);
            // 
            // chkConsole
            // 
            this.chkConsole.AutoSize = true;
            this.chkConsole.Image = null;
            this.chkConsole.Location = new System.Drawing.Point(15, 161);
            this.chkConsole.Name = "chkConsole";
            this.chkConsole.Size = new System.Drawing.Size(64, 17);
            this.chkConsole.TabIndex = 7;
            this.chkConsole.Text = "Console";
            this.chkConsole.UseVisualStyleBackColor = true;
            this.chkConsole.CheckedChanged += new System.EventHandler(this.chkConsole_CheckedChanged);
            // 
            // chkIRRemote
            // 
            this.chkIRRemote.AutoSize = true;
            this.chkIRRemote.Image = null;
            this.chkIRRemote.Location = new System.Drawing.Point(15, 249);
            this.chkIRRemote.Name = "chkIRRemote";
            this.chkIRRemote.Size = new System.Drawing.Size(72, 17);
            this.chkIRRemote.TabIndex = 10;
            this.chkIRRemote.Text = "IR remote";
            this.chkIRRemote.UseVisualStyleBackColor = true;
            this.chkIRRemote.CheckedChanged += new System.EventHandler(this.chkIRRemote_CheckedChanged);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(527, 417);
            this.Controls.Add(this.chkIRRemote);
            this.Controls.Add(this.chkConsole);
            this.Controls.Add(this.buttonTS1);
            this.Controls.Add(this.chkCAT);
            this.Controls.Add(this.chkDirectX);
            this.Controls.Add(this.chkAudio);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.rtbDebugMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Location = new System.Drawing.Point(114, 119);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(537, 449);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(537, 449);
            this.Name = "DebugForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Debug";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Debug_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Debug_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion

        public System.Windows.Forms.RichTextBox rtbDebugMsg;
        private System.Windows.Forms.ButtonTS btnClear;
        private System.Windows.Forms.ButtonTS btnSave;
        private System.Windows.Forms.CheckBoxTS chkAudio;
        private System.Windows.Forms.CheckBoxTS chkDirectX;
        private System.Windows.Forms.CheckBoxTS chkCAT;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ButtonTS buttonTS1;
        private System.Windows.Forms.CheckBoxTS chkConsole;
        private System.Windows.Forms.CheckBoxTS chkIRRemote;
    }
}