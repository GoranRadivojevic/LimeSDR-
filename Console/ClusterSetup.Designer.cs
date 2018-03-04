//=================================================================
// DXClusterSetup
//=================================================================
// Copyright (C) 2011,2012 YT7PWR
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//=================================================================

namespace PowerSDR
{
    partial class ClusterSetup
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
            this.txtHosts = new System.Windows.Forms.RichTextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Hosts = new System.Windows.Forms.TabPage();
            this.Buttons = new System.Windows.Forms.TabPage();
            this.txtButton4 = new System.Windows.Forms.TextBox();
            this.txtButton2 = new System.Windows.Forms.TextBox();
            this.txtButton3 = new System.Windows.Forms.TextBox();
            this.txtButton1 = new System.Windows.Forms.TextBox();
            this.lblCWMsg4 = new System.Windows.Forms.Label();
            this.btn3cmd = new System.Windows.Forms.TextBox();
            this.lblCWMsg3 = new System.Windows.Forms.Label();
            this.btn4cmd = new System.Windows.Forms.TextBox();
            this.lblCWMsg2 = new System.Windows.Forms.Label();
            this.btn1cmd = new System.Windows.Forms.TextBox();
            this.lblCWMsg1 = new System.Windows.Forms.Label();
            this.btn2cmd = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.Hosts.SuspendLayout();
            this.Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtHosts
            // 
            this.txtHosts.Location = new System.Drawing.Point(39, 27);
            this.txtHosts.Name = "txtHosts";
            this.txtHosts.Size = new System.Drawing.Size(272, 253);
            this.txtHosts.TabIndex = 0;
            this.txtHosts.Text = "";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(95, 359);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(215, 359);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Hosts);
            this.tabControl1.Controls.Add(this.Buttons);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(359, 331);
            this.tabControl1.TabIndex = 3;
            // 
            // Hosts
            // 
            this.Hosts.BackColor = System.Drawing.SystemColors.Control;
            this.Hosts.Controls.Add(this.txtHosts);
            this.Hosts.Location = new System.Drawing.Point(4, 22);
            this.Hosts.Name = "Hosts";
            this.Hosts.Padding = new System.Windows.Forms.Padding(3);
            this.Hosts.Size = new System.Drawing.Size(351, 305);
            this.Hosts.TabIndex = 0;
            this.Hosts.Text = "Hosts";
            // 
            // Buttons
            // 
            this.Buttons.BackColor = System.Drawing.SystemColors.Control;
            this.Buttons.Controls.Add(this.txtButton4);
            this.Buttons.Controls.Add(this.txtButton2);
            this.Buttons.Controls.Add(this.txtButton3);
            this.Buttons.Controls.Add(this.txtButton1);
            this.Buttons.Controls.Add(this.lblCWMsg4);
            this.Buttons.Controls.Add(this.btn3cmd);
            this.Buttons.Controls.Add(this.lblCWMsg3);
            this.Buttons.Controls.Add(this.btn4cmd);
            this.Buttons.Controls.Add(this.lblCWMsg2);
            this.Buttons.Controls.Add(this.btn1cmd);
            this.Buttons.Controls.Add(this.lblCWMsg1);
            this.Buttons.Controls.Add(this.btn2cmd);
            this.Buttons.Location = new System.Drawing.Point(4, 22);
            this.Buttons.Name = "Buttons";
            this.Buttons.Padding = new System.Windows.Forms.Padding(3);
            this.Buttons.Size = new System.Drawing.Size(351, 305);
            this.Buttons.TabIndex = 1;
            this.Buttons.Text = "Buttons";
            // 
            // txtButton4
            // 
            this.txtButton4.Location = new System.Drawing.Point(237, 168);
            this.txtButton4.MaxLength = 16;
            this.txtButton4.Name = "txtButton4";
            this.txtButton4.Size = new System.Drawing.Size(79, 20);
            this.txtButton4.TabIndex = 62;
            this.txtButton4.Text = "VHF up";
            // 
            // txtButton2
            // 
            this.txtButton2.Location = new System.Drawing.Point(237, 57);
            this.txtButton2.MaxLength = 16;
            this.txtButton2.Name = "txtButton2";
            this.txtButton2.Size = new System.Drawing.Size(79, 20);
            this.txtButton2.TabIndex = 65;
            this.txtButton2.Text = "Show DX";
            // 
            // txtButton3
            // 
            this.txtButton3.Location = new System.Drawing.Point(87, 168);
            this.txtButton3.MaxLength = 16;
            this.txtButton3.Name = "txtButton3";
            this.txtButton3.Size = new System.Drawing.Size(79, 20);
            this.txtButton3.TabIndex = 63;
            this.txtButton3.Text = "No VHF";
            // 
            // txtButton1
            // 
            this.txtButton1.Location = new System.Drawing.Point(87, 57);
            this.txtButton1.MaxLength = 16;
            this.txtButton1.Name = "txtButton1";
            this.txtButton1.Size = new System.Drawing.Size(79, 20);
            this.txtButton1.TabIndex = 61;
            this.txtButton1.Text = "No DX";
            // 
            // lblCWMsg4
            // 
            this.lblCWMsg4.AutoSize = true;
            this.lblCWMsg4.Location = new System.Drawing.Point(184, 171);
            this.lblCWMsg4.Name = "lblCWMsg4";
            this.lblCWMsg4.Size = new System.Drawing.Size(47, 13);
            this.lblCWMsg4.TabIndex = 56;
            this.lblCWMsg4.Text = "Button 4";
            // 
            // btn3cmd
            // 
            this.btn3cmd.Location = new System.Drawing.Point(37, 188);
            this.btn3cmd.MaxLength = 256;
            this.btn3cmd.Multiline = true;
            this.btn3cmd.Name = "btn3cmd";
            this.btn3cmd.Size = new System.Drawing.Size(129, 59);
            this.btn3cmd.TabIndex = 53;
            this.btn3cmd.Text = "set dx filter freq<30000";
            // 
            // lblCWMsg3
            // 
            this.lblCWMsg3.AutoSize = true;
            this.lblCWMsg3.Location = new System.Drawing.Point(34, 171);
            this.lblCWMsg3.Name = "lblCWMsg3";
            this.lblCWMsg3.Size = new System.Drawing.Size(47, 13);
            this.lblCWMsg3.TabIndex = 54;
            this.lblCWMsg3.Text = "Button 3";
            // 
            // btn4cmd
            // 
            this.btn4cmd.Location = new System.Drawing.Point(187, 188);
            this.btn4cmd.MaxLength = 256;
            this.btn4cmd.Multiline = true;
            this.btn4cmd.Name = "btn4cmd";
            this.btn4cmd.Size = new System.Drawing.Size(129, 59);
            this.btn4cmd.TabIndex = 55;
            this.btn4cmd.Text = "set dx filter freq>30000";
            // 
            // lblCWMsg2
            // 
            this.lblCWMsg2.AutoSize = true;
            this.lblCWMsg2.Location = new System.Drawing.Point(184, 60);
            this.lblCWMsg2.Name = "lblCWMsg2";
            this.lblCWMsg2.Size = new System.Drawing.Size(47, 13);
            this.lblCWMsg2.TabIndex = 52;
            this.lblCWMsg2.Text = "Button 2";
            // 
            // btn1cmd
            // 
            this.btn1cmd.Location = new System.Drawing.Point(37, 77);
            this.btn1cmd.MaxLength = 256;
            this.btn1cmd.Multiline = true;
            this.btn1cmd.Name = "btn1cmd";
            this.btn1cmd.Size = new System.Drawing.Size(129, 59);
            this.btn1cmd.TabIndex = 49;
            this.btn1cmd.Text = "set dx default";
            // 
            // lblCWMsg1
            // 
            this.lblCWMsg1.AutoSize = true;
            this.lblCWMsg1.Location = new System.Drawing.Point(34, 60);
            this.lblCWMsg1.Name = "lblCWMsg1";
            this.lblCWMsg1.Size = new System.Drawing.Size(47, 13);
            this.lblCWMsg1.TabIndex = 50;
            this.lblCWMsg1.Text = "Button 1";
            // 
            // btn2cmd
            // 
            this.btn2cmd.Location = new System.Drawing.Point(187, 77);
            this.btn2cmd.MaxLength = 256;
            this.btn2cmd.Multiline = true;
            this.btn2cmd.Name = "btn2cmd";
            this.btn2cmd.Size = new System.Drawing.Size(129, 59);
            this.btn2cmd.TabIndex = 51;
            this.btn2cmd.Text = "show dx";
            // 
            // ClusterSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 404);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(399, 442);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(399, 442);
            this.Name = "ClusterSetup";
            this.Text = "ClusterSetup";
            this.tabControl1.ResumeLayout(false);
            this.Hosts.ResumeLayout(false);
            this.Buttons.ResumeLayout(false);
            this.Buttons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtHosts;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Hosts;
        private System.Windows.Forms.TabPage Buttons;
        public System.Windows.Forms.TextBox txtButton4;
        public System.Windows.Forms.TextBox txtButton2;
        public System.Windows.Forms.TextBox txtButton3;
        public System.Windows.Forms.TextBox txtButton1;
        private System.Windows.Forms.Label lblCWMsg4;
        public System.Windows.Forms.TextBox btn3cmd;
        private System.Windows.Forms.Label lblCWMsg3;
        public System.Windows.Forms.TextBox btn4cmd;
        private System.Windows.Forms.Label lblCWMsg2;
        public System.Windows.Forms.TextBox btn1cmd;
        private System.Windows.Forms.Label lblCWMsg1;
        public System.Windows.Forms.TextBox btn2cmd;
    }
}