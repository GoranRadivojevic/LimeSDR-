//=================================================================
// eqform.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004, 2005, 2006  FlexRadio Systems
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
//
// You may contact us via email at: sales@flex-radio.com.
// Paper mail may be sent to: 
//    FlexRadio Systems
//    12100 Technology Blvd.
//    Austin, TX 78727
//    USA
//=================================================================

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PowerSDR
{
	/// <summary>
	/// Summary description for EQForm.
	/// </summary>
	public class EQForm : System.Windows.Forms.Form
	{
		#region Variable Declaration

		private System.Windows.Forms.GroupBoxTS grpRXEQ;
		private System.Windows.Forms.GroupBoxTS grpTXEQ;
		private System.Windows.Forms.TrackBarTS tbRXEQ1;
		private System.Windows.Forms.TrackBarTS tbRXEQ3;
		private System.Windows.Forms.TrackBarTS tbRXEQ10;
		private System.Windows.Forms.TrackBarTS tbTXEQ10;
		private System.Windows.Forms.TrackBarTS tbTXEQ1;
		private System.Windows.Forms.TrackBarTS tbTXEQ6;
		private System.Windows.Forms.LabelTS lblRXEQ0dB;
		private System.Windows.Forms.LabelTS lblTXEQ0dB;
		private System.Windows.Forms.LabelTS lblRXEQ1;
		private System.Windows.Forms.LabelTS lblRXEQ2;
		private System.Windows.Forms.LabelTS lblRXEQ3;
		private System.Windows.Forms.LabelTS lblTXEQ3;
		private System.Windows.Forms.LabelTS lblTXEQ2;
		private System.Windows.Forms.LabelTS lblTXEQ1;
		private System.Windows.Forms.LabelTS lblRXEQPreamp;
		private System.Windows.Forms.LabelTS lblTXEQPreamp;
		private System.Windows.Forms.CheckBoxTS chkTXEQEnabled;
		private System.Windows.Forms.TrackBarTS tbRXEQPreamp;
		private System.Windows.Forms.TrackBarTS tbTXEQPreamp;
		private System.Windows.Forms.CheckBoxTS chkRXEQEnabled;
		private System.Windows.Forms.PictureBox picRXEQ;
		private System.Windows.Forms.PictureBox picTXEQ;
		private System.Windows.Forms.Button btnTXEQReset;
		private System.Windows.Forms.Button btnRXEQReset;
		private System.Windows.Forms.LabelTS lblRXEQ15db;
		private System.Windows.Forms.LabelTS lblTXEQ15db;
		private System.Windows.Forms.LabelTS lblRXEQminus12db;
		private System.Windows.Forms.LabelTS lblTXEQminus12db;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBoxTS chkTXEQ160Notch;
        private TrackBarTS tbRXEQ9;
        private TrackBarTS tbRXEQ2;
        private TrackBarTS tbRXEQ4;
        private TrackBarTS tbRXEQ5;
        private TrackBarTS tbRXEQ6;
        private TrackBarTS tbRXEQ7;
        private TrackBarTS tbRXEQ8;
        private TrackBarTS tbTXEQ9;
        private TrackBarTS tbTXEQ8;
        private TrackBarTS tbTXEQ7;
        private TrackBarTS tbTXEQ5;
        private TrackBarTS tbTXEQ4;
        private TrackBarTS tbTXEQ3;
        private TrackBarTS tbTXEQ2;
        private LabelTS labelTS7;
        private LabelTS labelTS6;
        private LabelTS labelTS5;
        private LabelTS labelTS4;
        private LabelTS labelTS3;
        private LabelTS labelTS2;
        private LabelTS labelTS1;
        private LabelTS labelTS8;
        private LabelTS labelTS9;
        private LabelTS labelTS10;
        private LabelTS labelTS11;
        private LabelTS labelTS12;
        private LabelTS labelTS13;
        private LabelTS labelTS14;
		private System.ComponentModel.IContainer components;
		
		#endregion

		#region Constructor and Destructor

		public EQForm()
		{
			//
			// Required for Windows Form Designer support
			//
            this.AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
            int dpi = (int)this.CreateGraphics().DpiX;

            if (dpi > 96)
            {
                string font_name = this.Font.Name;
                System.Drawing.Font new_font = new System.Drawing.Font(font_name, 6.5f);
                this.Font = new_font;
            }

			RestoreSettings();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.grpRXEQ = new System.Windows.Forms.GroupBoxTS();
            this.labelTS7 = new System.Windows.Forms.LabelTS();
            this.labelTS6 = new System.Windows.Forms.LabelTS();
            this.labelTS5 = new System.Windows.Forms.LabelTS();
            this.labelTS4 = new System.Windows.Forms.LabelTS();
            this.labelTS3 = new System.Windows.Forms.LabelTS();
            this.labelTS2 = new System.Windows.Forms.LabelTS();
            this.labelTS1 = new System.Windows.Forms.LabelTS();
            this.tbRXEQ4 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ5 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ6 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ7 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ8 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ9 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ2 = new System.Windows.Forms.TrackBarTS();
            this.picRXEQ = new System.Windows.Forms.PictureBox();
            this.btnRXEQReset = new System.Windows.Forms.Button();
            this.chkRXEQEnabled = new System.Windows.Forms.CheckBoxTS();
            this.tbRXEQ1 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ3 = new System.Windows.Forms.TrackBarTS();
            this.tbRXEQ10 = new System.Windows.Forms.TrackBarTS();
            this.lblRXEQ1 = new System.Windows.Forms.LabelTS();
            this.lblRXEQ2 = new System.Windows.Forms.LabelTS();
            this.lblRXEQ3 = new System.Windows.Forms.LabelTS();
            this.lblRXEQPreamp = new System.Windows.Forms.LabelTS();
            this.tbRXEQPreamp = new System.Windows.Forms.TrackBarTS();
            this.lblRXEQ15db = new System.Windows.Forms.LabelTS();
            this.lblRXEQ0dB = new System.Windows.Forms.LabelTS();
            this.lblRXEQminus12db = new System.Windows.Forms.LabelTS();
            this.grpTXEQ = new System.Windows.Forms.GroupBoxTS();
            this.labelTS8 = new System.Windows.Forms.LabelTS();
            this.labelTS9 = new System.Windows.Forms.LabelTS();
            this.labelTS10 = new System.Windows.Forms.LabelTS();
            this.labelTS11 = new System.Windows.Forms.LabelTS();
            this.labelTS12 = new System.Windows.Forms.LabelTS();
            this.labelTS13 = new System.Windows.Forms.LabelTS();
            this.labelTS14 = new System.Windows.Forms.LabelTS();
            this.tbTXEQ9 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ8 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ7 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ5 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ4 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ3 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ2 = new System.Windows.Forms.TrackBarTS();
            this.chkTXEQ160Notch = new System.Windows.Forms.CheckBoxTS();
            this.picTXEQ = new System.Windows.Forms.PictureBox();
            this.btnTXEQReset = new System.Windows.Forms.Button();
            this.chkTXEQEnabled = new System.Windows.Forms.CheckBoxTS();
            this.tbTXEQ1 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ6 = new System.Windows.Forms.TrackBarTS();
            this.tbTXEQ10 = new System.Windows.Forms.TrackBarTS();
            this.lblTXEQ1 = new System.Windows.Forms.LabelTS();
            this.lblTXEQ2 = new System.Windows.Forms.LabelTS();
            this.lblTXEQ3 = new System.Windows.Forms.LabelTS();
            this.lblTXEQPreamp = new System.Windows.Forms.LabelTS();
            this.tbTXEQPreamp = new System.Windows.Forms.TrackBarTS();
            this.lblTXEQ15db = new System.Windows.Forms.LabelTS();
            this.lblTXEQ0dB = new System.Windows.Forms.LabelTS();
            this.lblTXEQminus12db = new System.Windows.Forms.LabelTS();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpRXEQ.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRXEQ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQPreamp)).BeginInit();
            this.grpTXEQ.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTXEQ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQPreamp)).BeginInit();
            this.SuspendLayout();
            // 
            // grpRXEQ
            // 
            this.grpRXEQ.Controls.Add(this.labelTS7);
            this.grpRXEQ.Controls.Add(this.labelTS6);
            this.grpRXEQ.Controls.Add(this.labelTS5);
            this.grpRXEQ.Controls.Add(this.labelTS4);
            this.grpRXEQ.Controls.Add(this.labelTS3);
            this.grpRXEQ.Controls.Add(this.labelTS2);
            this.grpRXEQ.Controls.Add(this.labelTS1);
            this.grpRXEQ.Controls.Add(this.tbRXEQ4);
            this.grpRXEQ.Controls.Add(this.tbRXEQ5);
            this.grpRXEQ.Controls.Add(this.tbRXEQ6);
            this.grpRXEQ.Controls.Add(this.tbRXEQ7);
            this.grpRXEQ.Controls.Add(this.tbRXEQ8);
            this.grpRXEQ.Controls.Add(this.tbRXEQ9);
            this.grpRXEQ.Controls.Add(this.tbRXEQ2);
            this.grpRXEQ.Controls.Add(this.picRXEQ);
            this.grpRXEQ.Controls.Add(this.btnRXEQReset);
            this.grpRXEQ.Controls.Add(this.chkRXEQEnabled);
            this.grpRXEQ.Controls.Add(this.tbRXEQ1);
            this.grpRXEQ.Controls.Add(this.tbRXEQ3);
            this.grpRXEQ.Controls.Add(this.tbRXEQ10);
            this.grpRXEQ.Controls.Add(this.lblRXEQ1);
            this.grpRXEQ.Controls.Add(this.lblRXEQ2);
            this.grpRXEQ.Controls.Add(this.lblRXEQ3);
            this.grpRXEQ.Controls.Add(this.lblRXEQPreamp);
            this.grpRXEQ.Controls.Add(this.tbRXEQPreamp);
            this.grpRXEQ.Controls.Add(this.lblRXEQ15db);
            this.grpRXEQ.Controls.Add(this.lblRXEQ0dB);
            this.grpRXEQ.Controls.Add(this.lblRXEQminus12db);
            this.grpRXEQ.Location = new System.Drawing.Point(4, 8);
            this.grpRXEQ.Name = "grpRXEQ";
            this.grpRXEQ.Size = new System.Drawing.Size(371, 224);
            this.grpRXEQ.TabIndex = 1;
            this.grpRXEQ.TabStop = false;
            this.grpRXEQ.Text = "Receive Equalizer";
            // 
            // labelTS7
            // 
            this.labelTS7.AutoSize = true;
            this.labelTS7.Image = null;
            this.labelTS7.Location = new System.Drawing.Point(290, 56);
            this.labelTS7.Name = "labelTS7";
            this.labelTS7.Size = new System.Drawing.Size(20, 13);
            this.labelTS7.TabIndex = 125;
            this.labelTS7.Text = "8K";
            // 
            // labelTS6
            // 
            this.labelTS6.AutoSize = true;
            this.labelTS6.Image = null;
            this.labelTS6.Location = new System.Drawing.Point(261, 56);
            this.labelTS6.Name = "labelTS6";
            this.labelTS6.Size = new System.Drawing.Size(20, 13);
            this.labelTS6.TabIndex = 124;
            this.labelTS6.Text = "4K";
            // 
            // labelTS5
            // 
            this.labelTS5.AutoSize = true;
            this.labelTS5.Image = null;
            this.labelTS5.Location = new System.Drawing.Point(236, 56);
            this.labelTS5.Name = "labelTS5";
            this.labelTS5.Size = new System.Drawing.Size(20, 13);
            this.labelTS5.TabIndex = 123;
            this.labelTS5.Text = "2K";
            // 
            // labelTS4
            // 
            this.labelTS4.AutoSize = true;
            this.labelTS4.Image = null;
            this.labelTS4.Location = new System.Drawing.Point(211, 56);
            this.labelTS4.Name = "labelTS4";
            this.labelTS4.Size = new System.Drawing.Size(20, 13);
            this.labelTS4.TabIndex = 122;
            this.labelTS4.Text = "1K";
            // 
            // labelTS3
            // 
            this.labelTS3.AutoSize = true;
            this.labelTS3.Image = null;
            this.labelTS3.Location = new System.Drawing.Point(186, 56);
            this.labelTS3.Name = "labelTS3";
            this.labelTS3.Size = new System.Drawing.Size(25, 13);
            this.labelTS3.TabIndex = 121;
            this.labelTS3.Text = "500";
            // 
            // labelTS2
            // 
            this.labelTS2.AutoSize = true;
            this.labelTS2.Image = null;
            this.labelTS2.Location = new System.Drawing.Point(161, 56);
            this.labelTS2.Name = "labelTS2";
            this.labelTS2.Size = new System.Drawing.Size(25, 13);
            this.labelTS2.TabIndex = 120;
            this.labelTS2.Text = "250";
            // 
            // labelTS1
            // 
            this.labelTS1.AutoSize = true;
            this.labelTS1.Image = null;
            this.labelTS1.Location = new System.Drawing.Point(132, 56);
            this.labelTS1.Name = "labelTS1";
            this.labelTS1.Size = new System.Drawing.Size(25, 13);
            this.labelTS1.TabIndex = 119;
            this.labelTS1.Text = "125";
            // 
            // tbRXEQ4
            // 
            this.tbRXEQ4.AutoSize = false;
            this.tbRXEQ4.LargeChange = 3;
            this.tbRXEQ4.Location = new System.Drawing.Point(160, 72);
            this.tbRXEQ4.Maximum = 15;
            this.tbRXEQ4.Minimum = -12;
            this.tbRXEQ4.Name = "tbRXEQ4";
            this.tbRXEQ4.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ4.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ4.TabIndex = 118;
            this.tbRXEQ4.TickFrequency = 3;
            this.tbRXEQ4.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ4.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ5
            // 
            this.tbRXEQ5.AutoSize = false;
            this.tbRXEQ5.LargeChange = 3;
            this.tbRXEQ5.Location = new System.Drawing.Point(186, 72);
            this.tbRXEQ5.Maximum = 15;
            this.tbRXEQ5.Minimum = -12;
            this.tbRXEQ5.Name = "tbRXEQ5";
            this.tbRXEQ5.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ5.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ5.TabIndex = 117;
            this.tbRXEQ5.TickFrequency = 3;
            this.tbRXEQ5.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ5.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ6
            // 
            this.tbRXEQ6.AutoSize = false;
            this.tbRXEQ6.LargeChange = 3;
            this.tbRXEQ6.Location = new System.Drawing.Point(212, 72);
            this.tbRXEQ6.Maximum = 15;
            this.tbRXEQ6.Minimum = -12;
            this.tbRXEQ6.Name = "tbRXEQ6";
            this.tbRXEQ6.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ6.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ6.TabIndex = 116;
            this.tbRXEQ6.TickFrequency = 3;
            this.tbRXEQ6.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ6.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ7
            // 
            this.tbRXEQ7.AutoSize = false;
            this.tbRXEQ7.LargeChange = 3;
            this.tbRXEQ7.Location = new System.Drawing.Point(238, 72);
            this.tbRXEQ7.Maximum = 15;
            this.tbRXEQ7.Minimum = -12;
            this.tbRXEQ7.Name = "tbRXEQ7";
            this.tbRXEQ7.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ7.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ7.TabIndex = 115;
            this.tbRXEQ7.TickFrequency = 3;
            this.tbRXEQ7.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ7.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ8
            // 
            this.tbRXEQ8.AutoSize = false;
            this.tbRXEQ8.LargeChange = 3;
            this.tbRXEQ8.Location = new System.Drawing.Point(264, 72);
            this.tbRXEQ8.Maximum = 15;
            this.tbRXEQ8.Minimum = -12;
            this.tbRXEQ8.Name = "tbRXEQ8";
            this.tbRXEQ8.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ8.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ8.TabIndex = 114;
            this.tbRXEQ8.TickFrequency = 3;
            this.tbRXEQ8.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ8.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ9
            // 
            this.tbRXEQ9.AutoSize = false;
            this.tbRXEQ9.LargeChange = 3;
            this.tbRXEQ9.Location = new System.Drawing.Point(290, 72);
            this.tbRXEQ9.Maximum = 15;
            this.tbRXEQ9.Minimum = -12;
            this.tbRXEQ9.Name = "tbRXEQ9";
            this.tbRXEQ9.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ9.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ9.TabIndex = 113;
            this.tbRXEQ9.TickFrequency = 3;
            this.tbRXEQ9.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ9.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ2
            // 
            this.tbRXEQ2.AutoSize = false;
            this.tbRXEQ2.LargeChange = 3;
            this.tbRXEQ2.Location = new System.Drawing.Point(108, 72);
            this.tbRXEQ2.Maximum = 15;
            this.tbRXEQ2.Minimum = -12;
            this.tbRXEQ2.Name = "tbRXEQ2";
            this.tbRXEQ2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ2.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ2.TabIndex = 112;
            this.tbRXEQ2.TickFrequency = 3;
            this.tbRXEQ2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ2.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // picRXEQ
            // 
            this.picRXEQ.BackColor = System.Drawing.Color.Black;
            this.picRXEQ.Location = new System.Drawing.Point(85, 22);
            this.picRXEQ.Name = "picRXEQ";
            this.picRXEQ.Size = new System.Drawing.Size(248, 24);
            this.picRXEQ.TabIndex = 111;
            this.picRXEQ.TabStop = false;
            this.picRXEQ.Paint += new System.Windows.Forms.PaintEventHandler(this.picRXEQ_Paint);
            // 
            // btnRXEQReset
            // 
            this.btnRXEQReset.Location = new System.Drawing.Point(162, 200);
            this.btnRXEQReset.Name = "btnRXEQReset";
            this.btnRXEQReset.Size = new System.Drawing.Size(56, 20);
            this.btnRXEQReset.TabIndex = 110;
            this.btnRXEQReset.Text = "Reset";
            this.btnRXEQReset.Click += new System.EventHandler(this.btnRXEQReset_Click);
            // 
            // chkRXEQEnabled
            // 
            this.chkRXEQEnabled.Image = null;
            this.chkRXEQEnabled.Location = new System.Drawing.Point(16, 26);
            this.chkRXEQEnabled.Name = "chkRXEQEnabled";
            this.chkRXEQEnabled.Size = new System.Drawing.Size(72, 16);
            this.chkRXEQEnabled.TabIndex = 109;
            this.chkRXEQEnabled.Text = "Enabled";
            this.chkRXEQEnabled.CheckedChanged += new System.EventHandler(this.chkRXEQEnabled_CheckedChanged);
            // 
            // tbRXEQ1
            // 
            this.tbRXEQ1.AutoSize = false;
            this.tbRXEQ1.LargeChange = 3;
            this.tbRXEQ1.Location = new System.Drawing.Point(82, 72);
            this.tbRXEQ1.Maximum = 15;
            this.tbRXEQ1.Minimum = -12;
            this.tbRXEQ1.Name = "tbRXEQ1";
            this.tbRXEQ1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ1.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ1.TabIndex = 4;
            this.tbRXEQ1.TickFrequency = 3;
            this.tbRXEQ1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ1.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ3
            // 
            this.tbRXEQ3.AutoSize = false;
            this.tbRXEQ3.LargeChange = 3;
            this.tbRXEQ3.Location = new System.Drawing.Point(134, 72);
            this.tbRXEQ3.Maximum = 15;
            this.tbRXEQ3.Minimum = -12;
            this.tbRXEQ3.Name = "tbRXEQ3";
            this.tbRXEQ3.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ3.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ3.TabIndex = 5;
            this.tbRXEQ3.TickFrequency = 3;
            this.tbRXEQ3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ3.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // tbRXEQ10
            // 
            this.tbRXEQ10.AutoSize = false;
            this.tbRXEQ10.LargeChange = 3;
            this.tbRXEQ10.Location = new System.Drawing.Point(316, 72);
            this.tbRXEQ10.Maximum = 15;
            this.tbRXEQ10.Minimum = -12;
            this.tbRXEQ10.Name = "tbRXEQ10";
            this.tbRXEQ10.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQ10.Size = new System.Drawing.Size(20, 128);
            this.tbRXEQ10.TabIndex = 6;
            this.tbRXEQ10.TickFrequency = 3;
            this.tbRXEQ10.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRXEQ10.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // lblRXEQ1
            // 
            this.lblRXEQ1.AutoSize = true;
            this.lblRXEQ1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRXEQ1.Image = null;
            this.lblRXEQ1.Location = new System.Drawing.Point(85, 56);
            this.lblRXEQ1.Name = "lblRXEQ1";
            this.lblRXEQ1.Size = new System.Drawing.Size(19, 13);
            this.lblRXEQ1.TabIndex = 43;
            this.lblRXEQ1.Text = "32";
            // 
            // lblRXEQ2
            // 
            this.lblRXEQ2.AutoSize = true;
            this.lblRXEQ2.Image = null;
            this.lblRXEQ2.Location = new System.Drawing.Point(105, 56);
            this.lblRXEQ2.Name = "lblRXEQ2";
            this.lblRXEQ2.Size = new System.Drawing.Size(19, 13);
            this.lblRXEQ2.TabIndex = 44;
            this.lblRXEQ2.Text = "63";
            // 
            // lblRXEQ3
            // 
            this.lblRXEQ3.AutoSize = true;
            this.lblRXEQ3.Image = null;
            this.lblRXEQ3.Location = new System.Drawing.Point(315, 56);
            this.lblRXEQ3.Name = "lblRXEQ3";
            this.lblRXEQ3.Size = new System.Drawing.Size(26, 13);
            this.lblRXEQ3.TabIndex = 45;
            this.lblRXEQ3.Text = "16K";
            // 
            // lblRXEQPreamp
            // 
            this.lblRXEQPreamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRXEQPreamp.Image = null;
            this.lblRXEQPreamp.Location = new System.Drawing.Point(8, 56);
            this.lblRXEQPreamp.Name = "lblRXEQPreamp";
            this.lblRXEQPreamp.Size = new System.Drawing.Size(48, 16);
            this.lblRXEQPreamp.TabIndex = 74;
            this.lblRXEQPreamp.Text = "Preamp";
            this.lblRXEQPreamp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbRXEQPreamp
            // 
            this.tbRXEQPreamp.AutoSize = false;
            this.tbRXEQPreamp.LargeChange = 3;
            this.tbRXEQPreamp.Location = new System.Drawing.Point(16, 72);
            this.tbRXEQPreamp.Maximum = 15;
            this.tbRXEQPreamp.Minimum = -12;
            this.tbRXEQPreamp.Name = "tbRXEQPreamp";
            this.tbRXEQPreamp.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbRXEQPreamp.Size = new System.Drawing.Size(32, 128);
            this.tbRXEQPreamp.TabIndex = 35;
            this.tbRXEQPreamp.TickFrequency = 3;
            this.tbRXEQPreamp.Scroll += new System.EventHandler(this.tbRXEQ_Scroll);
            // 
            // lblRXEQ15db
            // 
            this.lblRXEQ15db.AutoSize = true;
            this.lblRXEQ15db.Image = null;
            this.lblRXEQ15db.Location = new System.Drawing.Point(336, 77);
            this.lblRXEQ15db.Name = "lblRXEQ15db";
            this.lblRXEQ15db.Size = new System.Drawing.Size(32, 13);
            this.lblRXEQ15db.TabIndex = 40;
            this.lblRXEQ15db.Text = "15dB";
            this.lblRXEQ15db.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRXEQ0dB
            // 
            this.lblRXEQ0dB.AutoSize = true;
            this.lblRXEQ0dB.Image = null;
            this.lblRXEQ0dB.Location = new System.Drawing.Point(336, 133);
            this.lblRXEQ0dB.Name = "lblRXEQ0dB";
            this.lblRXEQ0dB.Size = new System.Drawing.Size(32, 13);
            this.lblRXEQ0dB.TabIndex = 41;
            this.lblRXEQ0dB.Text = "  0dB";
            this.lblRXEQ0dB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRXEQminus12db
            // 
            this.lblRXEQminus12db.AutoSize = true;
            this.lblRXEQminus12db.Image = null;
            this.lblRXEQminus12db.Location = new System.Drawing.Point(334, 176);
            this.lblRXEQminus12db.Name = "lblRXEQminus12db";
            this.lblRXEQminus12db.Size = new System.Drawing.Size(35, 13);
            this.lblRXEQminus12db.TabIndex = 42;
            this.lblRXEQminus12db.Text = "-12dB";
            this.lblRXEQminus12db.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpTXEQ
            // 
            this.grpTXEQ.Controls.Add(this.labelTS8);
            this.grpTXEQ.Controls.Add(this.labelTS9);
            this.grpTXEQ.Controls.Add(this.labelTS10);
            this.grpTXEQ.Controls.Add(this.labelTS11);
            this.grpTXEQ.Controls.Add(this.labelTS12);
            this.grpTXEQ.Controls.Add(this.labelTS13);
            this.grpTXEQ.Controls.Add(this.labelTS14);
            this.grpTXEQ.Controls.Add(this.tbTXEQ9);
            this.grpTXEQ.Controls.Add(this.tbTXEQ8);
            this.grpTXEQ.Controls.Add(this.tbTXEQ7);
            this.grpTXEQ.Controls.Add(this.tbTXEQ5);
            this.grpTXEQ.Controls.Add(this.tbTXEQ4);
            this.grpTXEQ.Controls.Add(this.tbTXEQ3);
            this.grpTXEQ.Controls.Add(this.tbTXEQ2);
            this.grpTXEQ.Controls.Add(this.chkTXEQ160Notch);
            this.grpTXEQ.Controls.Add(this.picTXEQ);
            this.grpTXEQ.Controls.Add(this.btnTXEQReset);
            this.grpTXEQ.Controls.Add(this.chkTXEQEnabled);
            this.grpTXEQ.Controls.Add(this.tbTXEQ1);
            this.grpTXEQ.Controls.Add(this.tbTXEQ6);
            this.grpTXEQ.Controls.Add(this.tbTXEQ10);
            this.grpTXEQ.Controls.Add(this.lblTXEQ1);
            this.grpTXEQ.Controls.Add(this.lblTXEQ2);
            this.grpTXEQ.Controls.Add(this.lblTXEQ3);
            this.grpTXEQ.Controls.Add(this.lblTXEQPreamp);
            this.grpTXEQ.Controls.Add(this.tbTXEQPreamp);
            this.grpTXEQ.Controls.Add(this.lblTXEQ15db);
            this.grpTXEQ.Controls.Add(this.lblTXEQ0dB);
            this.grpTXEQ.Controls.Add(this.lblTXEQminus12db);
            this.grpTXEQ.Location = new System.Drawing.Point(381, 8);
            this.grpTXEQ.Name = "grpTXEQ";
            this.grpTXEQ.Size = new System.Drawing.Size(399, 224);
            this.grpTXEQ.TabIndex = 1;
            this.grpTXEQ.TabStop = false;
            this.grpTXEQ.Text = "Transmit Equalizer";
            // 
            // labelTS8
            // 
            this.labelTS8.AutoSize = true;
            this.labelTS8.Image = null;
            this.labelTS8.Location = new System.Drawing.Point(316, 56);
            this.labelTS8.Name = "labelTS8";
            this.labelTS8.Size = new System.Drawing.Size(20, 13);
            this.labelTS8.TabIndex = 132;
            this.labelTS8.Text = "8K";
            // 
            // labelTS9
            // 
            this.labelTS9.AutoSize = true;
            this.labelTS9.Image = null;
            this.labelTS9.Location = new System.Drawing.Point(290, 56);
            this.labelTS9.Name = "labelTS9";
            this.labelTS9.Size = new System.Drawing.Size(20, 13);
            this.labelTS9.TabIndex = 131;
            this.labelTS9.Text = "4K";
            // 
            // labelTS10
            // 
            this.labelTS10.AutoSize = true;
            this.labelTS10.Image = null;
            this.labelTS10.Location = new System.Drawing.Point(264, 56);
            this.labelTS10.Name = "labelTS10";
            this.labelTS10.Size = new System.Drawing.Size(20, 13);
            this.labelTS10.TabIndex = 130;
            this.labelTS10.Text = "2K";
            // 
            // labelTS11
            // 
            this.labelTS11.AutoSize = true;
            this.labelTS11.Image = null;
            this.labelTS11.Location = new System.Drawing.Point(162, 56);
            this.labelTS11.Name = "labelTS11";
            this.labelTS11.Size = new System.Drawing.Size(25, 13);
            this.labelTS11.TabIndex = 129;
            this.labelTS11.Text = "125";
            // 
            // labelTS12
            // 
            this.labelTS12.AutoSize = true;
            this.labelTS12.Image = null;
            this.labelTS12.Location = new System.Drawing.Point(240, 56);
            this.labelTS12.Name = "labelTS12";
            this.labelTS12.Size = new System.Drawing.Size(20, 13);
            this.labelTS12.TabIndex = 128;
            this.labelTS12.Text = "1K";
            // 
            // labelTS13
            // 
            this.labelTS13.AutoSize = true;
            this.labelTS13.Image = null;
            this.labelTS13.Location = new System.Drawing.Point(213, 56);
            this.labelTS13.Name = "labelTS13";
            this.labelTS13.Size = new System.Drawing.Size(25, 13);
            this.labelTS13.TabIndex = 127;
            this.labelTS13.Text = "500";
            // 
            // labelTS14
            // 
            this.labelTS14.AutoSize = true;
            this.labelTS14.Image = null;
            this.labelTS14.Location = new System.Drawing.Point(187, 56);
            this.labelTS14.Name = "labelTS14";
            this.labelTS14.Size = new System.Drawing.Size(25, 13);
            this.labelTS14.TabIndex = 126;
            this.labelTS14.Text = "250";
            // 
            // tbTXEQ9
            // 
            this.tbTXEQ9.AutoSize = false;
            this.tbTXEQ9.LargeChange = 3;
            this.tbTXEQ9.Location = new System.Drawing.Point(318, 72);
            this.tbTXEQ9.Maximum = 15;
            this.tbTXEQ9.Minimum = -12;
            this.tbTXEQ9.Name = "tbTXEQ9";
            this.tbTXEQ9.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ9.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ9.TabIndex = 122;
            this.tbTXEQ9.TickFrequency = 3;
            this.tbTXEQ9.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ9.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ8
            // 
            this.tbTXEQ8.AutoSize = false;
            this.tbTXEQ8.LargeChange = 3;
            this.tbTXEQ8.Location = new System.Drawing.Point(293, 72);
            this.tbTXEQ8.Maximum = 15;
            this.tbTXEQ8.Minimum = -12;
            this.tbTXEQ8.Name = "tbTXEQ8";
            this.tbTXEQ8.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ8.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ8.TabIndex = 121;
            this.tbTXEQ8.TickFrequency = 3;
            this.tbTXEQ8.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ8.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ7
            // 
            this.tbTXEQ7.AutoSize = false;
            this.tbTXEQ7.LargeChange = 3;
            this.tbTXEQ7.Location = new System.Drawing.Point(268, 72);
            this.tbTXEQ7.Maximum = 15;
            this.tbTXEQ7.Minimum = -12;
            this.tbTXEQ7.Name = "tbTXEQ7";
            this.tbTXEQ7.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ7.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ7.TabIndex = 120;
            this.tbTXEQ7.TickFrequency = 3;
            this.tbTXEQ7.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ7.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ5
            // 
            this.tbTXEQ5.AutoSize = false;
            this.tbTXEQ5.LargeChange = 3;
            this.tbTXEQ5.Location = new System.Drawing.Point(218, 72);
            this.tbTXEQ5.Maximum = 15;
            this.tbTXEQ5.Minimum = -12;
            this.tbTXEQ5.Name = "tbTXEQ5";
            this.tbTXEQ5.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ5.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ5.TabIndex = 119;
            this.tbTXEQ5.TickFrequency = 3;
            this.tbTXEQ5.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ5.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ4
            // 
            this.tbTXEQ4.AutoSize = false;
            this.tbTXEQ4.LargeChange = 3;
            this.tbTXEQ4.Location = new System.Drawing.Point(193, 72);
            this.tbTXEQ4.Maximum = 15;
            this.tbTXEQ4.Minimum = -12;
            this.tbTXEQ4.Name = "tbTXEQ4";
            this.tbTXEQ4.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ4.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ4.TabIndex = 116;
            this.tbTXEQ4.TickFrequency = 3;
            this.tbTXEQ4.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ4.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ3
            // 
            this.tbTXEQ3.AutoSize = false;
            this.tbTXEQ3.LargeChange = 3;
            this.tbTXEQ3.Location = new System.Drawing.Point(168, 72);
            this.tbTXEQ3.Maximum = 15;
            this.tbTXEQ3.Minimum = -12;
            this.tbTXEQ3.Name = "tbTXEQ3";
            this.tbTXEQ3.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ3.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ3.TabIndex = 115;
            this.tbTXEQ3.TickFrequency = 3;
            this.tbTXEQ3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ3.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ2
            // 
            this.tbTXEQ2.AutoSize = false;
            this.tbTXEQ2.LargeChange = 3;
            this.tbTXEQ2.Location = new System.Drawing.Point(143, 72);
            this.tbTXEQ2.Maximum = 15;
            this.tbTXEQ2.Minimum = -12;
            this.tbTXEQ2.Name = "tbTXEQ2";
            this.tbTXEQ2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ2.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ2.TabIndex = 114;
            this.tbTXEQ2.TickFrequency = 3;
            this.tbTXEQ2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ2.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // chkTXEQ160Notch
            // 
            this.chkTXEQ160Notch.AutoSize = true;
            this.chkTXEQ160Notch.Image = null;
            this.chkTXEQ160Notch.Location = new System.Drawing.Point(83, 202);
            this.chkTXEQ160Notch.Name = "chkTXEQ160Notch";
            this.chkTXEQ160Notch.Size = new System.Drawing.Size(89, 17);
            this.chkTXEQ160Notch.TabIndex = 113;
            this.chkTXEQ160Notch.Text = "160Hz Notch";
            this.chkTXEQ160Notch.CheckedChanged += new System.EventHandler(this.chkTXEQ160Notch_CheckedChanged);
            // 
            // picTXEQ
            // 
            this.picTXEQ.BackColor = System.Drawing.Color.Black;
            this.picTXEQ.Location = new System.Drawing.Point(111, 22);
            this.picTXEQ.Name = "picTXEQ";
            this.picTXEQ.Size = new System.Drawing.Size(248, 24);
            this.picTXEQ.TabIndex = 112;
            this.picTXEQ.TabStop = false;
            this.picTXEQ.Paint += new System.Windows.Forms.PaintEventHandler(this.picTXEQ_Paint);
            // 
            // btnTXEQReset
            // 
            this.btnTXEQReset.Location = new System.Drawing.Point(171, 200);
            this.btnTXEQReset.Name = "btnTXEQReset";
            this.btnTXEQReset.Size = new System.Drawing.Size(56, 20);
            this.btnTXEQReset.TabIndex = 107;
            this.btnTXEQReset.Text = "Reset";
            this.btnTXEQReset.Click += new System.EventHandler(this.btnTXEQReset_Click);
            // 
            // chkTXEQEnabled
            // 
            this.chkTXEQEnabled.Image = null;
            this.chkTXEQEnabled.Location = new System.Drawing.Point(33, 26);
            this.chkTXEQEnabled.Name = "chkTXEQEnabled";
            this.chkTXEQEnabled.Size = new System.Drawing.Size(72, 16);
            this.chkTXEQEnabled.TabIndex = 106;
            this.chkTXEQEnabled.Text = "Enabled";
            this.chkTXEQEnabled.CheckedChanged += new System.EventHandler(this.chkTXEQEnabled_CheckedChanged);
            // 
            // tbTXEQ1
            // 
            this.tbTXEQ1.AutoSize = false;
            this.tbTXEQ1.LargeChange = 3;
            this.tbTXEQ1.Location = new System.Drawing.Point(118, 72);
            this.tbTXEQ1.Maximum = 15;
            this.tbTXEQ1.Minimum = -12;
            this.tbTXEQ1.Name = "tbTXEQ1";
            this.tbTXEQ1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ1.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ1.TabIndex = 4;
            this.tbTXEQ1.TickFrequency = 3;
            this.tbTXEQ1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ1.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ6
            // 
            this.tbTXEQ6.AutoSize = false;
            this.tbTXEQ6.LargeChange = 3;
            this.tbTXEQ6.Location = new System.Drawing.Point(243, 72);
            this.tbTXEQ6.Maximum = 15;
            this.tbTXEQ6.Minimum = -12;
            this.tbTXEQ6.Name = "tbTXEQ6";
            this.tbTXEQ6.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ6.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ6.TabIndex = 5;
            this.tbTXEQ6.TickFrequency = 3;
            this.tbTXEQ6.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ6.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // tbTXEQ10
            // 
            this.tbTXEQ10.AutoSize = false;
            this.tbTXEQ10.LargeChange = 3;
            this.tbTXEQ10.Location = new System.Drawing.Point(343, 72);
            this.tbTXEQ10.Maximum = 15;
            this.tbTXEQ10.Minimum = -12;
            this.tbTXEQ10.Name = "tbTXEQ10";
            this.tbTXEQ10.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQ10.Size = new System.Drawing.Size(20, 128);
            this.tbTXEQ10.TabIndex = 6;
            this.tbTXEQ10.TickFrequency = 3;
            this.tbTXEQ10.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTXEQ10.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // lblTXEQ1
            // 
            this.lblTXEQ1.AutoSize = true;
            this.lblTXEQ1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTXEQ1.Image = null;
            this.lblTXEQ1.Location = new System.Drawing.Point(116, 56);
            this.lblTXEQ1.Name = "lblTXEQ1";
            this.lblTXEQ1.Size = new System.Drawing.Size(19, 13);
            this.lblTXEQ1.TabIndex = 74;
            this.lblTXEQ1.Text = "32";
            // 
            // lblTXEQ2
            // 
            this.lblTXEQ2.AutoSize = true;
            this.lblTXEQ2.Image = null;
            this.lblTXEQ2.Location = new System.Drawing.Point(140, 56);
            this.lblTXEQ2.Name = "lblTXEQ2";
            this.lblTXEQ2.Size = new System.Drawing.Size(19, 13);
            this.lblTXEQ2.TabIndex = 75;
            this.lblTXEQ2.Text = "64";
            // 
            // lblTXEQ3
            // 
            this.lblTXEQ3.AutoSize = true;
            this.lblTXEQ3.Image = null;
            this.lblTXEQ3.Location = new System.Drawing.Point(339, 56);
            this.lblTXEQ3.Name = "lblTXEQ3";
            this.lblTXEQ3.Size = new System.Drawing.Size(26, 13);
            this.lblTXEQ3.TabIndex = 76;
            this.lblTXEQ3.Text = "16K";
            // 
            // lblTXEQPreamp
            // 
            this.lblTXEQPreamp.AutoSize = true;
            this.lblTXEQPreamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTXEQPreamp.Image = null;
            this.lblTXEQPreamp.Location = new System.Drawing.Point(72, 55);
            this.lblTXEQPreamp.Name = "lblTXEQPreamp";
            this.lblTXEQPreamp.Size = new System.Drawing.Size(43, 13);
            this.lblTXEQPreamp.TabIndex = 105;
            this.lblTXEQPreamp.Text = "Preamp";
            this.lblTXEQPreamp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbTXEQPreamp
            // 
            this.tbTXEQPreamp.AutoSize = false;
            this.tbTXEQPreamp.LargeChange = 3;
            this.tbTXEQPreamp.Location = new System.Drawing.Point(28, 72);
            this.tbTXEQPreamp.Maximum = 15;
            this.tbTXEQPreamp.Minimum = -12;
            this.tbTXEQPreamp.Name = "tbTXEQPreamp";
            this.tbTXEQPreamp.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTXEQPreamp.Size = new System.Drawing.Size(25, 128);
            this.tbTXEQPreamp.TabIndex = 36;
            this.tbTXEQPreamp.TickFrequency = 3;
            this.tbTXEQPreamp.Scroll += new System.EventHandler(this.tbTXEQ_Scroll);
            // 
            // lblTXEQ15db
            // 
            this.lblTXEQ15db.AutoSize = true;
            this.lblTXEQ15db.Image = null;
            this.lblTXEQ15db.Location = new System.Drawing.Point(364, 77);
            this.lblTXEQ15db.Name = "lblTXEQ15db";
            this.lblTXEQ15db.Size = new System.Drawing.Size(32, 13);
            this.lblTXEQ15db.TabIndex = 43;
            this.lblTXEQ15db.Text = "15dB";
            this.lblTXEQ15db.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTXEQ0dB
            // 
            this.lblTXEQ0dB.Image = null;
            this.lblTXEQ0dB.Location = new System.Drawing.Point(364, 133);
            this.lblTXEQ0dB.Name = "lblTXEQ0dB";
            this.lblTXEQ0dB.Size = new System.Drawing.Size(33, 16);
            this.lblTXEQ0dB.TabIndex = 0;
            this.lblTXEQ0dB.Text = "  0dB";
            this.lblTXEQ0dB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTXEQminus12db
            // 
            this.lblTXEQminus12db.AutoSize = true;
            this.lblTXEQminus12db.Image = null;
            this.lblTXEQminus12db.Location = new System.Drawing.Point(362, 176);
            this.lblTXEQminus12db.Name = "lblTXEQminus12db";
            this.lblTXEQminus12db.Size = new System.Drawing.Size(35, 13);
            this.lblTXEQminus12db.TabIndex = 45;
            this.lblTXEQminus12db.Text = "-12dB";
            this.lblTXEQminus12db.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EQForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(784, 238);
            this.Controls.Add(this.grpRXEQ);
            this.Controls.Add(this.grpTXEQ);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 276);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 276);
            this.Name = "EQForm";
            this.Text = "Equalizer Settings";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.EQForm_Closing);
            this.grpRXEQ.ResumeLayout(false);
            this.grpRXEQ.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRXEQ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQ10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRXEQPreamp)).EndInit();
            this.grpTXEQ.ResumeLayout(false);
            this.grpTXEQ.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTXEQ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQ10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTXEQPreamp)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Misc Routines

        public void Init()
        {
            try
            {
                tbRXEQ_Scroll(this, EventArgs.Empty);
                tbTXEQ_Scroll(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

		private void ControlList(Control c, ref ArrayList a)
		{
			if(c.Controls.Count > 0)
			{
				foreach(Control c2 in c.Controls)
					ControlList(c2, ref a);
			}

			if(c.GetType() == typeof(CheckBoxTS) || c.GetType() == typeof(CheckBoxTS) ||
				c.GetType() == typeof(ComboBoxTS) || c.GetType() == typeof(ComboBox) ||
				c.GetType() == typeof(NumericUpDownTS) || c.GetType() == typeof(NumericUpDown) ||
				c.GetType() == typeof(RadioButtonTS) || c.GetType() == typeof(RadioButton) ||
				c.GetType() == typeof(TextBoxTS) || c.GetType() == typeof(TextBox) ||
				c.GetType() == typeof(TrackBarTS) || c.GetType() == typeof(TrackBar) ||
				c.GetType() == typeof(ColorButton))
				a.Add(c);
		}

		public void SaveSettings()
		{
			ArrayList a = new ArrayList();
			ArrayList temp = new ArrayList();

			ControlList(this, ref temp);

			foreach(Control c in temp)				// For each control
			{
				if(c.GetType() == typeof(CheckBoxTS))
					a.Add(c.Name+"/"+((CheckBoxTS)c).Checked.ToString());
				else if(c.GetType() == typeof(ComboBoxTS))
				{
					//if(((ComboBox)c).SelectedIndex >= 0)
					a.Add(c.Name+"/"+((ComboBoxTS)c).Text);
				}
				else if(c.GetType() == typeof(NumericUpDownTS))
					a.Add(c.Name+"/"+((NumericUpDownTS)c).Value.ToString());
				else if(c.GetType() == typeof(RadioButtonTS))
					a.Add(c.Name+"/"+((RadioButtonTS)c).Checked.ToString());
				else if(c.GetType() == typeof(TextBoxTS))
					a.Add(c.Name+"/"+((TextBoxTS)c).Text);
				else if(c.GetType() == typeof(TrackBarTS))
					a.Add(c.Name+"/"+((TrackBarTS)c).Value.ToString());
				else if(c.GetType() == typeof(ColorButton))
				{
					Color clr = ((ColorButton)c).Color;
					a.Add(c.Name+"/"+clr.R+"."+clr.G+"."+clr.B+"."+clr.A);
				}
#if(DEBUG)
				else if(c.GetType() == typeof(GroupBox) ||
					c.GetType() == typeof(CheckBoxTS) ||
					c.GetType() == typeof(ComboBox) ||
					c.GetType() == typeof(NumericUpDown) ||
					c.GetType() == typeof(RadioButton) ||
					c.GetType() == typeof(TextBox) ||
					c.GetType() == typeof(TrackBar))
					Debug.WriteLine(c.Name+" needs to be converted to a Thread Safe control.");
#endif
			}

			DB.SaveVars("EQForm", ref a);		// save the values to the DB
		}

		public void RestoreSettings()
		{
			ArrayList temp = new ArrayList();		// list of all first level controls
			ControlList(this, ref temp);

			ArrayList checkbox_list = new ArrayList();
			ArrayList combobox_list = new ArrayList();
			ArrayList numericupdown_list = new ArrayList();
			ArrayList radiobutton_list = new ArrayList();
			ArrayList textbox_list = new ArrayList();
			ArrayList trackbar_list = new ArrayList();
			ArrayList colorbutton_list = new ArrayList();

			//ArrayList controls = new ArrayList();	// list of controls to restore
			foreach(Control c in temp)
			{
				if(c.GetType() == typeof(CheckBoxTS))			// the control is a CheckBoxTS
					checkbox_list.Add(c);
				else if(c.GetType() == typeof(ComboBoxTS))		// the control is a ComboBox
					combobox_list.Add(c);
				else if(c.GetType() == typeof(NumericUpDownTS))	// the control is a NumericUpDown
					numericupdown_list.Add(c);
				else if(c.GetType() == typeof(RadioButtonTS))	// the control is a RadioButton
					radiobutton_list.Add(c);
				else if(c.GetType() == typeof(TextBoxTS))		// the control is a TextBox
					textbox_list.Add(c);
				else if(c.GetType() == typeof(TrackBarTS))		// the control is a TrackBar (slider)
					trackbar_list.Add(c);
				else if(c.GetType() == typeof(ColorButton))
					colorbutton_list.Add(c);
			}
			temp.Clear();	// now that we have the controls we want, delete first list 

			ArrayList a = DB.GetVars("EQForm");						// Get the saved list of controls
			a.Sort();
			
			// restore saved values to the controls
			foreach(string s in a)				// string is in the format "name,value"
			{
				string[] vals = s.Split('/');
				if(vals.Length > 2)
				{
					for(int i=2; i<vals.Length; i++)
						vals[1] += "/"+vals[i];
				}

				string name = vals[0];
				string val = vals[1];

				if(s.StartsWith("chk"))			// control is a CheckBoxTS
				{
					for(int i=0; i<checkbox_list.Count; i++)
					{	// look through each control to find the matching name
						CheckBoxTS c = (CheckBoxTS)checkbox_list[i];
						if(c.Name.Equals(name))		// name found
						{
							c.Checked = bool.Parse(val);	// restore value
							i = checkbox_list.Count+1;
						}
						if(i == checkbox_list.Count)
							MessageBox.Show("Control not found: "+name);
					}
				}
				else if(s.StartsWith("combo"))	// control is a ComboBox
				{
					for(int i=0; i<combobox_list.Count; i++)
					{	// look through each control to find the matching name
						ComboBoxTS c = (ComboBoxTS)combobox_list[i];
						if(c.Name.Equals(name))		// name found
						{
							c.Text = val;	// restore value
							i = combobox_list.Count+1;
						}
						if(i == combobox_list.Count)
							MessageBox.Show("Control not found: "+name);
					}
				}
				else if(s.StartsWith("ud"))
				{
					for(int i=0; i<numericupdown_list.Count; i++)
					{	// look through each control to find the matching name
						NumericUpDownTS c = (NumericUpDownTS)numericupdown_list[i];
						if(c.Name.Equals(name))		// name found
						{
							decimal num = decimal.Parse(val);

							if(num > c.Maximum)
                                num = c.Maximum;		// check endpoints
							else if(num < c.Minimum)
                                num = c.Minimum;

							c.Value = num;			// restore value
							i = numericupdown_list.Count+1;
						}
						if(i == numericupdown_list.Count)
							MessageBox.Show("Control not found: "+name);	
					}
				}
				else if(s.StartsWith("rad"))
				{	// look through each control to find the matching name
					for(int i=0; i<radiobutton_list.Count; i++)
					{
						RadioButtonTS c = (RadioButtonTS)radiobutton_list[i];
						if(c.Name.Equals(name))		// name found
						{
							if(!val.ToLower().Equals("true") && !val.ToLower().Equals("false"))
								val = "True";
							c.Checked = bool.Parse(val);	// restore value
							i = radiobutton_list.Count+1;
						}
						if(i == radiobutton_list.Count)
							MessageBox.Show("Control not found: "+name);
					}
				}
				else if(s.StartsWith("txt"))
				{	// look through each control to find the matching name
					for(int i=0; i<textbox_list.Count; i++)
					{
						TextBoxTS c = (TextBoxTS)textbox_list[i];
						if(c.Name.Equals(name))		// name found
						{
							c.Text = val;	// restore value
							i = textbox_list.Count+1;
						}
						if(i == textbox_list.Count)
							MessageBox.Show("Control not found: "+name);
					}
				}
				else if(s.StartsWith("tb"))
				{
					// look through each control to find the matching name
					for(int i=0; i<trackbar_list.Count; i++)
					{
						TrackBarTS c = (TrackBarTS)trackbar_list[i];

						if(c.Name.Equals(name))		// name found
						{
                            int num = Int32.Parse(val);

                            if (num > c.Maximum)
                                num = c.Maximum;		// check endpoints
                            else if (num < c.Minimum)
                                num = c.Minimum;

							c.Value = num;
							i = trackbar_list.Count+1;
						}

						if(i == trackbar_list.Count)
							MessageBox.Show("Control not found: "+name);
					}
				}
				else if(s.StartsWith("clrbtn"))
				{
					string[] colors = val.Split('.');
					if(colors.Length == 4)
					{
						int R,G,B,A;
						R = Int32.Parse(colors[0]);
						G = Int32.Parse(colors[1]);
						B = Int32.Parse(colors[2]);
						A = Int32.Parse(colors[3]);

						for(int i=0; i<colorbutton_list.Count; i++)
						{
							ColorButton c = (ColorButton)colorbutton_list[i];
							if(c.Name.Equals(name))		// name found
							{
								c.Color = Color.FromArgb(A, R, G, B);
								i = colorbutton_list.Count+1;
							}
							if(i == colorbutton_list.Count)
								MessageBox.Show("Control not found: "+name);
						}
					}
				}
			}
		}

        public void InitEQ()                            // yt7pwr
        {
            if (chkRXEQEnabled.Checked)
            {
                DttSP.SetGrphRXEQcmd(0, 0, true);
                DttSP.SetGrphRXEQcmd(0, 1, true);
                tbRXEQ_Scroll(null, null);
            }
            else
            {
                DttSP.SetGrphRXEQcmd(0, 0, false);
                DttSP.SetGrphRXEQcmd(0, 1, false);
            }

            picRXEQ.Invalidate();

            tbRXEQ_Scroll(null, null);

            if (chkTXEQEnabled.Checked)
            {
                DttSP.SetGrphTXEQcmd(0, true);
                tbTXEQ_Scroll(null, null);
            }
            else
            {
                DttSP.SetGrphTXEQcmd(0, false);
            }

            picTXEQ.Invalidate();

            tbTXEQ_Scroll(null, null);
        }

		#endregion

		#region Properties

		public int[] RXEQ
		{
			get
			{
				int[] eq = new int[11];
				eq[0] = tbRXEQPreamp.Value;
				eq[1] = tbRXEQ1.Value;
				eq[2] = tbRXEQ2.Value;
				eq[3] = tbRXEQ3.Value;
                eq[4] = tbRXEQ4.Value;
                eq[5] = tbRXEQ5.Value;
                eq[6] = tbRXEQ6.Value;
                eq[7] = tbRXEQ7.Value;
                eq[8] = tbRXEQ8.Value;
                eq[9] = tbRXEQ9.Value;
                eq[10] = tbRXEQ10.Value;
				return eq;
			}

			set
			{
                try
                {
                    // checks for min/max limits
                    tbRXEQPreamp.Value = Math.Max(tbRXEQPreamp.Minimum, Math.Min(tbRXEQPreamp.Maximum, value[0]));
                    tbRXEQ1.Value = Math.Max(tbRXEQ1.Minimum, Math.Min(tbRXEQ1.Maximum, value[1]));
                    tbRXEQ2.Value = Math.Max(tbRXEQ2.Minimum, Math.Min(tbRXEQ2.Maximum, value[2]));
                    tbRXEQ3.Value = Math.Max(tbRXEQ3.Minimum, Math.Min(tbRXEQ3.Maximum, value[3]));
                    tbRXEQ4.Value = Math.Max(tbRXEQ4.Minimum, Math.Min(tbRXEQ4.Maximum, value[4]));
                    tbRXEQ5.Value = Math.Max(tbRXEQ5.Minimum, Math.Min(tbRXEQ5.Maximum, value[5]));
                    tbRXEQ6.Value = Math.Max(tbRXEQ6.Minimum, Math.Min(tbRXEQ6.Maximum, value[6]));
                    tbRXEQ7.Value = Math.Max(tbRXEQ7.Minimum, Math.Min(tbRXEQ7.Maximum, value[7]));
                    tbRXEQ8.Value = Math.Max(tbRXEQ8.Minimum, Math.Min(tbRXEQ8.Maximum, value[8]));
                    tbRXEQ9.Value = Math.Max(tbRXEQ9.Minimum, Math.Min(tbRXEQ9.Maximum, value[9]));
                    tbRXEQ10.Value = Math.Max(tbRXEQ10.Minimum, Math.Min(tbRXEQ10.Maximum, value[10]));
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
			}
		}

		public int[] TXEQ
		{
			get 
			{
				int[] eq = new int[11];
				eq[0] = tbTXEQPreamp.Value;
				eq[1] = tbTXEQ1.Value;
				eq[2] = tbTXEQ2.Value;
				eq[3] = tbTXEQ3.Value;
                eq[4] = tbTXEQ4.Value;
                eq[5] = tbTXEQ5.Value;
                eq[6] = tbTXEQ6.Value;
                eq[7] = tbTXEQ7.Value;
                eq[8] = tbTXEQ8.Value;
                eq[9] = tbTXEQ9.Value;
                eq[10] = tbTXEQ10.Value;
				return eq;
			}
			set
			{
                try
                {
                    tbTXEQPreamp.Value = Math.Max(tbTXEQ1.Minimum, Math.Min(tbTXEQ1.Maximum, value[0]));
                    tbTXEQ1.Value = Math.Max(tbTXEQ1.Minimum, Math.Min(tbTXEQ1.Maximum, value[1]));
                    tbTXEQ2.Value = Math.Max(tbTXEQ2.Minimum, Math.Min(tbTXEQ2.Maximum, value[2]));
                    tbTXEQ3.Value = Math.Max(tbTXEQ3.Minimum, Math.Min(tbTXEQ3.Maximum, value[3]));
                    tbTXEQ4.Value = Math.Max(tbTXEQ4.Minimum, Math.Min(tbTXEQ4.Maximum, value[4]));
                    tbTXEQ5.Value = Math.Max(tbTXEQ5.Minimum, Math.Min(tbTXEQ5.Maximum, value[5]));
                    tbTXEQ6.Value = Math.Max(tbTXEQ6.Minimum, Math.Min(tbTXEQ6.Maximum, value[6]));
                    tbTXEQ7.Value = Math.Max(tbTXEQ7.Minimum, Math.Min(tbTXEQ7.Maximum, value[7]));
                    tbTXEQ8.Value = Math.Max(tbTXEQ8.Minimum, Math.Min(tbTXEQ8.Maximum, value[8]));
                    tbTXEQ9.Value = Math.Max(tbTXEQ9.Minimum, Math.Min(tbTXEQ9.Maximum, value[9]));
                    tbTXEQ10.Value = Math.Max(tbTXEQ10.Minimum, Math.Min(tbTXEQ10.Maximum, value[10]));
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
			}
		}

		public bool RXEQEnabled
		{
			get { return chkRXEQEnabled.Checked; }
			set { chkRXEQEnabled.Checked = value; }
		}

		public bool TXEQEnabled
		{
			get { return chkTXEQEnabled.Checked; }
			set { chkTXEQEnabled.Checked = value; }
		}

		#endregion

		#region Event Handlers

		private void EQForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
			SaveSettings();
		}

		private void tbRXEQ_Scroll(object sender, System.EventArgs e)
		{
            try
            {
                int[] rxeq = RXEQ;
                DttSP.SetGrphRXEQ10(0, 0, rxeq);
                DttSP.SetGrphRXEQ10(0, 1, rxeq);
                picRXEQ.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void tbTXEQ_Scroll(object sender, System.EventArgs e)
		{
            try
            {
                int[] txeq = TXEQ;
                DttSP.SetGrphTXEQ10(0, txeq);
                picTXEQ.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void picRXEQ_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            try
            {
                int[] rxeq = RXEQ;

                if (!chkRXEQEnabled.Checked)
                {
                    for (int i = 0; i < rxeq.Length; i++)
                        rxeq[i] = 0;
                }

                Point[] points = new Point[rxeq.Length - 1];
                for (int i = 1; i < rxeq.Length; i++)
                {
                    points[i - 1].X = (int)((i - 1) * picRXEQ.Width / (float)(rxeq.Length - 2));
                    points[i - 1].Y = picRXEQ.Height / 2 - (int)(rxeq[i] * (picRXEQ.Height - 6) / 2 / 15.0f +
                        tbRXEQPreamp.Value * 3 / 15.0f);
                }

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, picRXEQ.Width, picRXEQ.Height);
                e.Graphics.DrawLines(new Pen(Color.LightGreen), points);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void picTXEQ_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            try
            {
                int[] txeq = TXEQ;
                if (!chkTXEQEnabled.Checked)
                {
                    for (int i = 0; i < txeq.Length; i++)
                        txeq[i] = 0;
                }

                Point[] points = new Point[txeq.Length - 1];
                for (int i = 1; i < txeq.Length; i++)
                {
                    points[i - 1].X = (int)((i - 1) * picTXEQ.Width / (float)(txeq.Length - 2));
                    points[i - 1].Y = picTXEQ.Height / 2 - (int)(txeq[i] * (picTXEQ.Height - 6) / 2 / 15.0f +
                        tbTXEQPreamp.Value * 3 / 15.0f);
                }

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, picTXEQ.Width, picTXEQ.Height);
                e.Graphics.DrawLines(new Pen(Color.LightGreen), points);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void chkRXEQEnabled_CheckedChanged(object sender, System.EventArgs e)
		{
            try
            {
                if (chkRXEQEnabled.Checked)
                {
                    DttSP.SetGrphRXEQcmd(0, 0, true);
                    DttSP.SetGrphRXEQcmd(0, 1, true);
                    tbRXEQ_Scroll(sender, e);
                }
                else
                {
                    DttSP.SetGrphRXEQcmd(0, 0, false);
                    DttSP.SetGrphRXEQcmd(0, 1, false);
                }

                picRXEQ.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void chkTXEQEnabled_CheckedChanged(object sender, System.EventArgs e)
		{
            try
            {
                if (chkTXEQEnabled.Checked)
                {
                    DttSP.SetGrphTXEQcmd(0, true);
                    tbTXEQ_Scroll(sender, e);
                }
                else
                    DttSP.SetGrphTXEQcmd(0, false);

                picTXEQ.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void btnRXEQReset_Click(object sender, System.EventArgs e)
		{
            try
            {
                DialogResult dr = MessageBox.Show(
                    "Are you sure you want to reset the Receive Equalizer\n" +
                    "to flat (zero)?",
                    "Are you sure?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                    return;

                foreach (Control c in grpRXEQ.Controls)
                {
                    if (c.GetType() == typeof(TrackBarTS))
                        ((TrackBarTS)c).Value = 0;
                }

                tbRXEQ_Scroll(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void btnTXEQReset_Click(object sender, System.EventArgs e)
		{
            try
            {
                DialogResult dr = MessageBox.Show(
                    "Are you sure you want to reset the Transmit Equalizer\n" +
                    "to flat (zero)?",
                    "Are you sure?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                    return;

                foreach (Control c in grpTXEQ.Controls)
                {
                    if (c.GetType() == typeof(TrackBarTS))
                        ((TrackBarTS)c).Value = 0;
                }

                tbTXEQ_Scroll(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void chkTXEQ160Notch_CheckedChanged(object sender, System.EventArgs e)
		{
            try
            {
                if (chkTXEQ160Notch.Checked)
                    DttSP.SetNotch160(0, true);
                else
                    DttSP.SetNotch160(0, false);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		#endregion		
    }
}