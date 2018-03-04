/*
 *  Changes for GenesisRadio
 *  Copyright (C)2008-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace PowerSDR
{
	/// <summary>
	/// Summary description for FilterForm.
	/// </summary>
	public class FilterForm : System.Windows.Forms.Form
	{
		#region Variable Declaration 

        private FilterPreset[] filter_presets = new FilterPreset[(int)DSPMode.LAST];
        private FilterPreset[] filter_presets_subrx = new FilterPreset[(int)DSPMode.LAST];
        public bool show_subRX = false;
		private Console console;
		private System.Windows.Forms.ComboBox comboDSPMode;
		private System.Windows.Forms.RadioButtonTS radFilter1;
		private System.Windows.Forms.RadioButtonTS radFilter2;
		private System.Windows.Forms.RadioButtonTS radFilter3;
		private System.Windows.Forms.RadioButtonTS radFilter4;
		private System.Windows.Forms.RadioButtonTS radFilter5;
		private System.Windows.Forms.RadioButtonTS radFilter6;
		private System.Windows.Forms.RadioButtonTS radFilter7;
		private System.Windows.Forms.RadioButtonTS radFilter8;
		private System.Windows.Forms.RadioButtonTS radFilter9;
		private System.Windows.Forms.RadioButtonTS radFilter10;
		private System.Windows.Forms.RadioButtonTS radFilterVar1;
		private System.Windows.Forms.RadioButtonTS radFilterVar2;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label lblMode;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.NumericUpDown udLow;
		private System.Windows.Forms.NumericUpDown udHigh;
		private System.Windows.Forms.Label lblLow;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.PictureBox picDisplay;
		private System.Windows.Forms.Label lblWidth;
		private System.Windows.Forms.NumericUpDown udWidth;
        private ButtonTS btnApply;
        private ButtonTS btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructor and Destructor

        public FilterForm(Console c, DSPMode mode, Filter filter, bool subrx)
        {
            //
            // Required for Windows Form Designer support
            //
            console = c;

            if (subrx)
                show_subRX = true;

            this.AutoScaleMode = AutoScaleMode.Inherit;
            // used to initialize all the filter variables
            for (int m = (int)DSPMode.FIRST + 1; m < (int)DSPMode.LAST; m++)
            {
                filter_presets[m] = new FilterPreset();
                filter_presets_subrx[m] = new FilterPreset();

                for (Filter f = Filter.F1; f != Filter.LAST; f++)
                {
                    filter_presets[m].CopyFilter(f, console.filter_presets[m].GetLow(f),
                        console.filter_presets[m].GetHigh(f), console.filter_presets[m].LastFilter,
                        console.filter_presets[m].GetName(f));

                    filter_presets_subrx[m].CopyFilter(f, console.filter_presets_subRX[m].GetLow(f),
                        console.filter_presets_subRX[m].GetHigh(f), console.filter_presets_subRX[m].LastFilter,
                        console.filter_presets_subRX[m].GetName(f));
                }
            }

            InitializeComponent();
            int dpi = (int)this.CreateGraphics().DpiX;

            if (dpi > 96)
            {
                string font_name = this.Font.Name;
                System.Drawing.Font new_font = new System.Drawing.Font(font_name, 6.5f);
                this.Font = new_font;
            }

            if(show_subRX)
                this.Text += " VFOB";
            else
                this.Text += " VFOA";

            Win32.SetWindowPos(this.Handle.ToInt32(),
                -1, this.Left, this.Top, this.Width, this.Height, 0);

            CurrentDSPMode = mode;
            CurrentFilter = filter;
            comboDSPMode.SelectedIndex = 0;
            radFilter1.Checked = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterForm));
            this.comboDSPMode = new System.Windows.Forms.ComboBox();
            this.radFilter1 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter2 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter3 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter4 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter5 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter6 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter7 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter8 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter9 = new System.Windows.Forms.RadioButtonTS();
            this.radFilter10 = new System.Windows.Forms.RadioButtonTS();
            this.radFilterVar1 = new System.Windows.Forms.RadioButtonTS();
            this.radFilterVar2 = new System.Windows.Forms.RadioButtonTS();
            this.lblMode = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.udLow = new System.Windows.Forms.NumericUpDown();
            this.udHigh = new System.Windows.Forms.NumericUpDown();
            this.lblLow = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.udWidth = new System.Windows.Forms.NumericUpDown();
            this.lblWidth = new System.Windows.Forms.Label();
            this.picDisplay = new System.Windows.Forms.PictureBox();
            this.btnApply = new System.Windows.Forms.ButtonTS();
            this.btnCancel = new System.Windows.Forms.ButtonTS();
            ((System.ComponentModel.ISupportInitialize)(this.udLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHigh)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // comboDSPMode
            // 
            this.comboDSPMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDSPMode.Items.AddRange(new object[] {
            "LSB",
            "USB",
            "DSB",
            "CWU",
            "CWL",
            "FMN",
            "WFM",
            "AM",
            "SAM",
            "DIGL",
            "DIGU"});
            this.comboDSPMode.Location = new System.Drawing.Point(64, 16);
            this.comboDSPMode.Name = "comboDSPMode";
            this.comboDSPMode.Size = new System.Drawing.Size(64, 21);
            this.comboDSPMode.TabIndex = 0;
            this.comboDSPMode.SelectedIndexChanged += new System.EventHandler(this.comboDSPMode_SelectedIndexChanged);
            // 
            // radFilter1
            // 
            this.radFilter1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter1.Image = null;
            this.radFilter1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter1.Location = new System.Drawing.Point(8, 48);
            this.radFilter1.Name = "radFilter1";
            this.radFilter1.Size = new System.Drawing.Size(48, 18);
            this.radFilter1.TabIndex = 37;
            this.radFilter1.Text = "6.0k";
            this.radFilter1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter1.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter2
            // 
            this.radFilter2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter2.Image = null;
            this.radFilter2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter2.Location = new System.Drawing.Point(56, 48);
            this.radFilter2.Name = "radFilter2";
            this.radFilter2.Size = new System.Drawing.Size(48, 18);
            this.radFilter2.TabIndex = 39;
            this.radFilter2.Text = "4.0k";
            this.radFilter2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter2.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter3
            // 
            this.radFilter3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter3.Image = null;
            this.radFilter3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter3.Location = new System.Drawing.Point(104, 48);
            this.radFilter3.Name = "radFilter3";
            this.radFilter3.Size = new System.Drawing.Size(48, 18);
            this.radFilter3.TabIndex = 38;
            this.radFilter3.Text = "2.6k";
            this.radFilter3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter3.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter4
            // 
            this.radFilter4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter4.Image = null;
            this.radFilter4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter4.Location = new System.Drawing.Point(8, 66);
            this.radFilter4.Name = "radFilter4";
            this.radFilter4.Size = new System.Drawing.Size(48, 18);
            this.radFilter4.TabIndex = 40;
            this.radFilter4.Text = "2.1k";
            this.radFilter4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter4.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter5
            // 
            this.radFilter5.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter5.Image = null;
            this.radFilter5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter5.Location = new System.Drawing.Point(56, 66);
            this.radFilter5.Name = "radFilter5";
            this.radFilter5.Size = new System.Drawing.Size(48, 18);
            this.radFilter5.TabIndex = 41;
            this.radFilter5.Text = "1.0k";
            this.radFilter5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter5.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter6
            // 
            this.radFilter6.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter6.Image = null;
            this.radFilter6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter6.Location = new System.Drawing.Point(104, 66);
            this.radFilter6.Name = "radFilter6";
            this.radFilter6.Size = new System.Drawing.Size(48, 18);
            this.radFilter6.TabIndex = 42;
            this.radFilter6.Text = "500";
            this.radFilter6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter6.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter7
            // 
            this.radFilter7.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter7.Image = null;
            this.radFilter7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter7.Location = new System.Drawing.Point(8, 84);
            this.radFilter7.Name = "radFilter7";
            this.radFilter7.Size = new System.Drawing.Size(48, 18);
            this.radFilter7.TabIndex = 43;
            this.radFilter7.Text = "250";
            this.radFilter7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter7.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter8
            // 
            this.radFilter8.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter8.Image = null;
            this.radFilter8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter8.Location = new System.Drawing.Point(56, 84);
            this.radFilter8.Name = "radFilter8";
            this.radFilter8.Size = new System.Drawing.Size(48, 18);
            this.radFilter8.TabIndex = 44;
            this.radFilter8.Text = "100";
            this.radFilter8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter8.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter9
            // 
            this.radFilter9.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter9.Image = null;
            this.radFilter9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter9.Location = new System.Drawing.Point(104, 84);
            this.radFilter9.Name = "radFilter9";
            this.radFilter9.Size = new System.Drawing.Size(48, 18);
            this.radFilter9.TabIndex = 45;
            this.radFilter9.Text = "50";
            this.radFilter9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter9.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilter10
            // 
            this.radFilter10.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilter10.Image = null;
            this.radFilter10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilter10.Location = new System.Drawing.Point(8, 102);
            this.radFilter10.Name = "radFilter10";
            this.radFilter10.Size = new System.Drawing.Size(48, 18);
            this.radFilter10.TabIndex = 46;
            this.radFilter10.Text = "25";
            this.radFilter10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilter10.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilterVar1
            // 
            this.radFilterVar1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilterVar1.Image = null;
            this.radFilterVar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilterVar1.Location = new System.Drawing.Point(56, 102);
            this.radFilterVar1.Name = "radFilterVar1";
            this.radFilterVar1.Size = new System.Drawing.Size(48, 18);
            this.radFilterVar1.TabIndex = 47;
            this.radFilterVar1.Text = "Var 1";
            this.radFilterVar1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilterVar1.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // radFilterVar2
            // 
            this.radFilterVar2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radFilterVar2.Image = null;
            this.radFilterVar2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radFilterVar2.Location = new System.Drawing.Point(104, 102);
            this.radFilterVar2.Name = "radFilterVar2";
            this.radFilterVar2.Size = new System.Drawing.Size(48, 18);
            this.radFilterVar2.TabIndex = 48;
            this.radFilterVar2.Text = "Var 2";
            this.radFilterVar2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radFilterVar2.CheckedChanged += new System.EventHandler(this.radFilter_CheckedChanged);
            // 
            // lblMode
            // 
            this.lblMode.Location = new System.Drawing.Point(24, 16);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(40, 23);
            this.lblMode.TabIndex = 49;
            this.lblMode.Text = "Mode:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(72, 16);
            this.txtName.MaxLength = 6;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(56, 20);
            this.txtName.TabIndex = 50;
            this.txtName.LostFocus += new System.EventHandler(this.txtName_LostFocus);
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(8, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(48, 23);
            this.lblName.TabIndex = 51;
            this.lblName.Text = "Name:";
            // 
            // udLow
            // 
            this.udLow.Location = new System.Drawing.Point(72, 64);
            this.udLow.Maximum = new decimal(new int[] {
            125000,
            0,
            0,
            0});
            this.udLow.Minimum = new decimal(new int[] {
            125000,
            0,
            0,
            -2147483648});
            this.udLow.Name = "udLow";
            this.udLow.Size = new System.Drawing.Size(64, 20);
            this.udLow.TabIndex = 52;
            this.udLow.ValueChanged += new System.EventHandler(this.udLow_ValueChanged);
            this.udLow.LostFocus += new System.EventHandler(this.udLow_LostFocus);
            // 
            // udHigh
            // 
            this.udHigh.Location = new System.Drawing.Point(72, 40);
            this.udHigh.Maximum = new decimal(new int[] {
            125000,
            0,
            0,
            0});
            this.udHigh.Minimum = new decimal(new int[] {
            125000,
            0,
            0,
            -2147483648});
            this.udHigh.Name = "udHigh";
            this.udHigh.Size = new System.Drawing.Size(64, 20);
            this.udHigh.TabIndex = 53;
            this.udHigh.ValueChanged += new System.EventHandler(this.udHigh_ValueChanged);
            this.udHigh.LostFocus += new System.EventHandler(this.udHigh_LostFocus);
            // 
            // lblLow
            // 
            this.lblLow.Location = new System.Drawing.Point(8, 64);
            this.lblLow.Name = "lblLow";
            this.lblLow.Size = new System.Drawing.Size(48, 23);
            this.lblLow.TabIndex = 54;
            this.lblLow.Text = "Low:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 23);
            this.label1.TabIndex = 55;
            this.label1.Text = "High:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radFilter10);
            this.groupBox1.Controls.Add(this.radFilter2);
            this.groupBox1.Controls.Add(this.radFilterVar1);
            this.groupBox1.Controls.Add(this.comboDSPMode);
            this.groupBox1.Controls.Add(this.radFilter1);
            this.groupBox1.Controls.Add(this.radFilterVar2);
            this.groupBox1.Controls.Add(this.lblMode);
            this.groupBox1.Controls.Add(this.radFilter3);
            this.groupBox1.Controls.Add(this.radFilter4);
            this.groupBox1.Controls.Add(this.radFilter5);
            this.groupBox1.Controls.Add(this.radFilter6);
            this.groupBox1.Controls.Add(this.radFilter7);
            this.groupBox1.Controls.Add(this.radFilter8);
            this.groupBox1.Controls.Add(this.radFilter9);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 128);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.udWidth);
            this.groupBox2.Controls.Add(this.lblWidth);
            this.groupBox2.Controls.Add(this.udHigh);
            this.groupBox2.Controls.Add(this.lblLow);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.lblName);
            this.groupBox2.Controls.Add(this.udLow);
            this.groupBox2.Location = new System.Drawing.Point(176, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 128);
            this.groupBox2.TabIndex = 57;
            this.groupBox2.TabStop = false;
            // 
            // udWidth
            // 
            this.udWidth.Location = new System.Drawing.Point(72, 88);
            this.udWidth.Maximum = new decimal(new int[] {
            192000,
            0,
            0,
            0});
            this.udWidth.Minimum = new decimal(new int[] {
            192000,
            0,
            0,
            -2147483648});
            this.udWidth.Name = "udWidth";
            this.udWidth.Size = new System.Drawing.Size(64, 20);
            this.udWidth.TabIndex = 56;
            this.udWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udWidth.ValueChanged += new System.EventHandler(this.udWidth_ValueChanged);
            // 
            // lblWidth
            // 
            this.lblWidth.Location = new System.Drawing.Point(8, 88);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(64, 23);
            this.lblWidth.TabIndex = 57;
            this.lblWidth.Text = "Width:";
            // 
            // picDisplay
            // 
            this.picDisplay.BackColor = System.Drawing.SystemColors.ControlText;
            this.picDisplay.Location = new System.Drawing.Point(8, 144);
            this.picDisplay.Name = "picDisplay";
            this.picDisplay.Size = new System.Drawing.Size(328, 50);
            this.picDisplay.TabIndex = 58;
            this.picDisplay.TabStop = false;
            this.picDisplay.MouseLeave += new System.EventHandler(this.picDisplay_MouseLeave);
            this.picDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picDisplay_MouseMove);
            this.picDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picDisplay_MouseDown);
            this.picDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.picDisplay_Paint);
            this.picDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picDisplay_MouseUp);
            // 
            // btnApply
            // 
            this.btnApply.Image = null;
            this.btnApply.Location = new System.Drawing.Point(72, 221);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 59;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = null;
            this.btnCancel.Location = new System.Drawing.Point(198, 221);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 60;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FilterForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(344, 256);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.picDisplay);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(360, 294);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 294);
            this.Name = "FilterForm";
            this.Text = "Filter Setup";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.udLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHigh)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDisplay)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Properties

		private Filter current_filter = Filter.F1;
		public Filter CurrentFilter
		{
			get { return current_filter; }
			set
			{
				current_filter = value;

				switch(current_filter)
				{
					case Filter.F1:
						radFilter1.Checked = true;
						break;
					case Filter.F2:
						radFilter2.Checked = true;
						break;
					case Filter.F3:
						radFilter3.Checked = true;
						break;
					case Filter.F4:
						radFilter4.Checked = true;
						break;
					case Filter.F5:
						radFilter5.Checked = true;
						break;
					case Filter.F6:
						radFilter6.Checked = true;
						break;
					case Filter.F7:
						radFilter7.Checked = true;
						break;
					case Filter.F8:
						radFilter8.Checked = true;
						break;
					case Filter.F9:
						radFilter9.Checked = true;
						break;
					case Filter.F10:
						radFilter10.Checked = true;
						break;
					case Filter.VAR1:
						radFilterVar1.Checked = true;
						break;
					case Filter.VAR2:
						radFilterVar2.Checked = true;
						break;
				}

				GetFilterInfo();
			}
		}       

		private DSPMode current_dsp_mode = DSPMode.FIRST;
		public DSPMode CurrentDSPMode               // changes yt7pwr
		{
			get { return current_dsp_mode; }
            set
            {
                current_dsp_mode = value;

                switch (current_dsp_mode)
                {
                    case DSPMode.LSB:
                        comboDSPMode.Text = "LSB";
                        break;
                    case DSPMode.USB:
                        comboDSPMode.Text = "USB";
                        break;
                    case DSPMode.DSB:
                        comboDSPMode.Text = "DSB";
                        break;
                    case DSPMode.CWL:
                        comboDSPMode.Text = "CWL";
                        break;
                    case DSPMode.CWU:
                        comboDSPMode.Text = "CWU";
                        break;
                    case DSPMode.FMN:
                        comboDSPMode.Text = "FMN";
                        break;
                    case DSPMode.WFM:
                        comboDSPMode.Text = "WFM";
                        break;
                    case DSPMode.AM:
                        comboDSPMode.Text = "AM";
                        break;
                    case DSPMode.SAM:
                        comboDSPMode.Text = "SAM";
                        break;
                    case DSPMode.DIGL:
                        comboDSPMode.Text = "DIGL";
                        break;
                    case DSPMode.DIGU:
                        comboDSPMode.Text = "DIGU";
                        break;
                }

                radFilter1.Text = filter_presets[(int)value].GetName(Filter.F1);
                radFilter2.Text = filter_presets[(int)value].GetName(Filter.F2);
                radFilter3.Text = filter_presets[(int)value].GetName(Filter.F3);
                radFilter4.Text = filter_presets[(int)value].GetName(Filter.F4);
                radFilter5.Text = filter_presets[(int)value].GetName(Filter.F5);
                radFilter6.Text = filter_presets[(int)value].GetName(Filter.F6);
                radFilter7.Text = filter_presets[(int)value].GetName(Filter.F7);
                radFilter8.Text = filter_presets[(int)value].GetName(Filter.F8);
                radFilter9.Text = filter_presets[(int)value].GetName(Filter.F9);
                radFilter10.Text = filter_presets[(int)value].GetName(Filter.F10);
                radFilterVar1.Text = filter_presets[(int)value].GetName(Filter.VAR1);
                radFilterVar2.Text = filter_presets[(int)value].GetName(Filter.VAR2);

                GetFilterInfo();
            }
		}

		#endregion

		#region Misc Routines

		private void GetFilterInfo()        // changes yt7pwr
		{
            try
            {
                DSPMode m = DSPMode.FIRST;
                Filter f = Filter.FIRST;

                m = (DSPMode)Enum.Parse(typeof(DSPMode), comboDSPMode.Text);
                f = current_filter;

                if (show_subRX)
                {
                    txtName.Text = filter_presets_subrx[(int)m].GetName(f);
                    udLow.Value = filter_presets_subrx[(int)m].GetLow(f);
                    udHigh.Value = filter_presets_subrx[(int)m].GetHigh(f);
                }
                else
                {
                    txtName.Text = filter_presets[(int)m].GetName(f);
                    udLow.Value = filter_presets[(int)m].GetLow(f);
                    udHigh.Value = filter_presets[(int)m].GetHigh(f);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private int HzToPixel(float freq)
		{
            try
            {
                int low = (int)(-10000 * console.RXSampleRate / 48000.0);
                int high = (int)(10000 * console.RXSampleRate / 48000.0);

                return picDisplay.Width / 2 + (int)(freq / (high - low) * picDisplay.Width);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 0;
            }
		}

		private float PixelToHz(float x)
		{
            try
            {
                int low = (int)(-10000 * console.RXSampleRate / 48000.0);
                int high = (int)(10000 * console.RXSampleRate / 48000.0);

                return (float)(low + ((double)x * (high - low) / picDisplay.Width));
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 0;
            }
		}

		private bool filter_updating = false;
		private void UpdateFilter(int low, int high)
		{
            try
            {
                filter_updating = true;
                if (low < udLow.Minimum) low = (int)udLow.Minimum;
                if (low > udLow.Maximum) low = (int)udLow.Maximum;
                if (high < udHigh.Minimum) high = (int)udHigh.Minimum;
                if (high > udHigh.Maximum) high = (int)udHigh.Maximum;

                udLow.Value = Math.Min(udLow.Maximum, low);
                udHigh.Value = Math.Min(udHigh.Maximum, high);

                udWidth.Value = Math.Abs(Math.Min(udWidth.Maximum, high - low));
                filter_updating = false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		#endregion

		#region Event Handlers

		private void radFilter_CheckedChanged(object sender, System.EventArgs e)
		{
            try
            {
                RadioButtonTS r = (RadioButtonTS)sender;
                if (((RadioButtonTS)sender).Checked)
                {
                    string filter = r.Name.Substring(r.Name.IndexOf("Filter") + 6);

                    if (!filter.StartsWith("V"))
                        filter = "F" + filter;
                    else
                        filter = filter.ToUpper();

                    current_filter = (Filter)Enum.Parse(typeof(Filter), filter);
                    CurrentFilter = current_filter;
                    r.BackColor = console.ButtonSelectedColor;
                }
                else
                {
                    r.BackColor = SystemColors.Control;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void comboDSPMode_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            try
            {
                current_dsp_mode = (DSPMode)Enum.Parse(typeof(DSPMode), comboDSPMode.Text);
                CurrentDSPMode = current_dsp_mode;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void txtName_LostFocus(object sender, System.EventArgs e)   // changes yt7pwr
		{
            try
            {
                if (show_subRX)
                {
                    filter_presets_subrx[(int)current_dsp_mode].SetName(current_filter, txtName.Text);
                    GetFilterInfo();

                    switch (current_filter)
                    {
                        case Filter.F1:
                            radFilter1.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F1);
                            break;
                        case Filter.F2:
                            radFilter2.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F2);
                            break;
                        case Filter.F3:
                            radFilter3.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F3);
                            break;
                        case Filter.F4:
                            radFilter4.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F4);
                            break;
                        case Filter.F5:
                            radFilter5.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F5);
                            break;
                        case Filter.F6:
                            radFilter6.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F6);
                            break;
                        case Filter.F7:
                            radFilter7.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F7);
                            break;
                        case Filter.F8:
                            radFilter8.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F8);
                            break;
                        case Filter.F9:
                            radFilter9.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F9);
                            break;
                        case Filter.F10:
                            radFilter10.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.F10);
                            break;
                        case Filter.VAR1:
                            radFilterVar1.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.VAR1);
                            break;
                        case Filter.VAR2:
                            radFilterVar2.Text = filter_presets_subrx[(int)current_dsp_mode].GetName(Filter.VAR2);
                            break;
                    }
                }
                else
                {
                    filter_presets[(int)current_dsp_mode].SetName(current_filter, txtName.Text);
                    GetFilterInfo();

                    switch (current_filter)
                    {
                        case Filter.F1:
                            radFilter1.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F1);
                            break;
                        case Filter.F2:
                            radFilter2.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F2);
                            break;
                        case Filter.F3:
                            radFilter3.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F3);
                            break;
                        case Filter.F4:
                            radFilter4.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F4);
                            break;
                        case Filter.F5:
                            radFilter5.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F5);
                            break;
                        case Filter.F6:
                            radFilter6.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F6);
                            break;
                        case Filter.F7:
                            radFilter7.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F7);
                            break;
                        case Filter.F8:
                            radFilter8.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F8);
                            break;
                        case Filter.F9:
                            radFilter9.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F9);
                            break;
                        case Filter.F10:
                            radFilter10.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.F10);
                            break;
                        case Filter.VAR1:
                            radFilterVar1.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.VAR1);
                            break;
                        case Filter.VAR2:
                            radFilterVar2.Text = filter_presets[(int)current_dsp_mode].GetName(Filter.VAR2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void udLow_ValueChanged(object sender, System.EventArgs e)
		{
            try
            {
                if (show_subRX)
                {
                    //if (udLow.Value + 10 > udHigh.Value) udLow.Value = udHigh.Value - 10;
                    filter_presets_subrx[(int)current_dsp_mode].SetLow(current_filter, (int)udLow.Value);
                    if (!filter_updating) UpdateFilter((int)udLow.Value, (int)udHigh.Value);
                }
                else
                {
                    //if (udLow.Value + 10 > udHigh.Value) udLow.Value = udHigh.Value - 10;
                    int q = (int)current_dsp_mode;
                    Debug.Write("DSP mode: " + q.ToString() + "\n");
                    filter_presets[(int)current_dsp_mode].SetLow(current_filter, (int)udLow.Value);
                    if (!filter_updating) UpdateFilter((int)udLow.Value, (int)udHigh.Value);
                }

                picDisplay.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

        private void udHigh_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (show_subRX)
                {
                    //if (udHigh.Value - 10 < udLow.Value) udHigh.Value = udLow.Value + 10;
                    filter_presets_subrx[(int)current_dsp_mode].SetHigh(current_filter, (int)udHigh.Value);
                    if (!filter_updating) UpdateFilter((int)udLow.Value, (int)udHigh.Value);
                }
                else
                {
                    //if (udHigh.Value - 10 < udLow.Value) udHigh.Value = udLow.Value + 10;
                    filter_presets[(int)current_dsp_mode].SetHigh(current_filter, (int)udHigh.Value);
                    if (!filter_updating) UpdateFilter((int)udLow.Value, (int)udHigh.Value);
                }

                picDisplay.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

		private void udLow_LostFocus(object sender, EventArgs e)
		{
            try
            {
                udLow_ValueChanged(sender, e);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void udHigh_LostFocus(object sender, EventArgs e)
		{
            try
            {
                udHigh_ValueChanged(sender, e);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void picDisplay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            try
            {
                // draw background
                e.Graphics.FillRectangle(
                    new SolidBrush(Display_GDI.DisplayBackgroundColor),
                    0, 0, picDisplay.Width, picDisplay.Height);

                e.Graphics.FillRectangle(
                    new SolidBrush(Display_GDI.DisplayFilterColor),
                    HzToPixel((int)udLow.Value), 0,
                    Math.Max(1, HzToPixel((int)udHigh.Value) - HzToPixel((int)udLow.Value)), picDisplay.Height);

                // draw center line
                e.Graphics.DrawLine(new Pen(Display_GDI.GridZeroColor, 1.0f),
                    picDisplay.Width / 2, 0, picDisplay.Width / 2, picDisplay.Height);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void picDisplay_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            try
            {
                int low = HzToPixel((float)udLow.Value);
                int high = HzToPixel((float)udHigh.Value);

                if (Math.Abs(e.X - low) < 2 || Math.Abs(e.X - high) < 2)
                    Cursor = Cursors.SizeWE;
                else if (e.X > low && e.X < high)
                    Cursor = Cursors.NoMoveHoriz;
                else
                    Cursor = Cursors.Arrow;

                if (drag_low) udLow.Value = Math.Max(Math.Min(udLow.Maximum, (int)PixelToHz((float)e.X)), udLow.Minimum);
                if (drag_high) udHigh.Value = Math.Max(Math.Min(udHigh.Maximum, (int)PixelToHz((float)e.X)), udHigh.Minimum); ;
                if (drag_filter)
                {
                    int delta = (int)(PixelToHz((float)e.X) - PixelToHz(drag_filter_start));
                    udLow.Value = Math.Max(Math.Min(udLow.Maximum, drag_filter_low + delta), udLow.Minimum);
                    udHigh.Value = Math.Max(Math.Min(udHigh.Maximum, drag_filter_high + delta), udHigh.Minimum);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private bool drag_low = false;
		private bool drag_high = false;
		private bool drag_filter = false;
		private int drag_filter_low = -1;
		private int drag_filter_high = -1;
		private int drag_filter_start = -1;

		private void picDisplay_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    int low = HzToPixel((float)udLow.Value);
                    int high = HzToPixel((float)udHigh.Value);

                    if (Math.Abs(e.X - low) < 2)
                        drag_low = true;
                    else if (Math.Abs(e.X - high) < 2)
                        drag_high = true;
                    else if (e.X > low && e.X < high)
                    {
                        drag_filter = true;
                        drag_filter_low = (int)udLow.Value;
                        drag_filter_high = (int)udHigh.Value;
                        drag_filter_start = e.X;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

		private void picDisplay_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    drag_low = false;
                    drag_high = false;
                    drag_filter = false;
                    drag_filter_low = -1;
                    drag_filter_high = -1;
                    drag_filter_start = -1;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}

        private void picDisplay_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.Arrow;
                drag_low = false;
                drag_high = false;
                drag_filter = false;
                drag_filter_low = -1;
                drag_filter_high = -1;
                drag_filter_start = -1;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

		private void udWidth_ValueChanged(object sender, System.EventArgs e)
		{
            try
            {
                if (udWidth.Focused)
                {
                    int low = 0, high = 0;
                    switch (comboDSPMode.Text)
                    {
                        case "CWL":
                            low = (int)(-console.CWPitch - udWidth.Value / 2);
                            high = (int)(-console.CWPitch + udWidth.Value / 2);
                            break;
                        case "CWU":
                            low = (int)(console.CWPitch - udWidth.Value / 2);
                            high = (int)(console.CWPitch + udWidth.Value / 2);
                            break;
                        case "LSB":
                        case "DIGL":
                            high = -console.DefaultLowCut;
                            low = high - (int)udWidth.Value;
                            break;
                        case "USB":
                        case "DIGU":
                            low = console.DefaultLowCut;
                            high = low + (int)udWidth.Value;
                            break;
                        case "AM":
                        case "SAM":
                        case "FMN":
                            low = -(int)udWidth.Value / 2;
                            high = (int)udWidth.Value / 2;
                            break;
                    }

                    if (!filter_updating) UpdateFilter(low, high);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
		}
		
		#endregion		

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (show_subRX)
                {
                    for (int m = (int)DSPMode.FIRST + 1; m < (int)DSPMode.LAST; m++)
                    {
                        for (Filter f = Filter.F1; f != Filter.LAST; f++)
                        {
                            console.filter_presets_subRX[m].CopyFilter(f, filter_presets_subrx[m].GetLow(f),
                                filter_presets_subrx[m].GetHigh(f), filter_presets_subrx[m].LastFilter,
                                filter_presets_subrx[m].GetName(f));
                        }
                    }

                    if (console.CurrentDSPModeSubRX == current_dsp_mode &&
                        console.CurrentFilterSubRX == current_filter)
                        console.UpdateFiltersSubRX((int)udLow.Value, (int)udHigh.Value);
                }
                else
                {
                    for (int m = (int)DSPMode.FIRST + 1; m < (int)DSPMode.LAST; m++)
                    {
                        for (Filter f = Filter.F1; f != Filter.LAST; f++)
                        {
                            console.filter_presets[m].CopyFilter(f, filter_presets[m].GetLow(f),
                                filter_presets[m].GetHigh(f), filter_presets[m].LastFilter,
                                filter_presets[m].GetName(f));
                        }
                    }

                    if (console.CurrentDSPMode == current_dsp_mode &&
                        console.CurrentFilter == current_filter)
                        console.UpdateFilters((int)udLow.Value, (int)udHigh.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
	}
}
