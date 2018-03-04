//=================================================================
// XTRV form
//=================================================================
//
//  Copyright (C)2011 YT7PWR Goran Radivojevic
//  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 3
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
//=================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Diagnostics;

namespace PowerSDR
{  
    public partial class XTRV : Form
    {

        public Console console;
        delegate void XTRV_cross_thread_callback(int nr, string btn_txt, double f_min, double f_max, double losc,
            double pa_pwr, double pa_gain, bool rx_only);

        public XTRV(Console c)
        {
            this.AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = (float)(8.25 / ratio);
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            this.PerformAutoScale();
            this.PerformLayout();

            console = c;
            GetOptions();
            XTRV_update();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveOptions();
        }

        private void udBandXPwrGain_ValueChanged(object sender, EventArgs e)
        {
            XTRV_update();
        }

        private void udBandXPwr_ValueChanged(object sender, EventArgs e)
        {
            XTRV_update();
        }

        public void SaveOptions()
        {
            try
            {
                ArrayList a = new ArrayList();
                ArrayList temp = new ArrayList();

                ControlList(this, ref temp);

                foreach (Control c in temp)				                    // For each control
                {
                    if (c.GetType() == typeof(CheckBoxTS))
                        a.Add(c.Name + "/" + ((CheckBoxTS)c).Checked.ToString());
                    else if (c.GetType() == typeof(NumericUpDownTS))
                        a.Add(c.Name + "/" + ((NumericUpDownTS)c).Value.ToString());
                    else if (c.GetType() == typeof(TextBoxTS))
                        a.Add(c.Name + "/" + ((TextBoxTS)c).Text);
                }

                if (a.Count == 96)
                {
                    DB.SaveVars("XTRVsettings", ref a);		                // save the values to the DB
                    XTRV_update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in XTRV SaveOption function!\n" + ex.ToString());
            }
        }

        private void ControlList(Control c, ref ArrayList a)
        {
            if (c.Controls.Count > 0)
            {
                foreach (Control c2 in c.Controls)
                    ControlList(c2, ref a);
            }

            if (c.GetType() == typeof(CheckBoxTS) || c.GetType() == typeof(CheckBoxTS) ||
                c.GetType() == typeof(NumericUpDownTS) || c.GetType() == typeof(NumericUpDown) ||
                c.GetType() == typeof(TextBoxTS) || c.GetType() == typeof(TextBox))
                a.Add(c);
        }

        public void GetOptions()  // changes yt7pwr
        {
            try
            {
                // get list of live controls
                ArrayList temp = new ArrayList();		                // list of all first level controls
                ControlList(this, ref temp);
                ArrayList checkbox_list = new ArrayList();
                ArrayList numericupdown_list = new ArrayList();
                ArrayList textbox_list = new ArrayList();

                //ArrayList controls = new ArrayList();	                // list of controls to restore
                foreach (Control c in temp)
                {
                    if (c.GetType() == typeof(CheckBoxTS))			    // the control is a CheckBoxTS
                        checkbox_list.Add(c);
                    else if (c.GetType() == typeof(NumericUpDownTS))	// the control is a NumericUpDown
                        numericupdown_list.Add(c);
                    else if (c.GetType() == typeof(TextBoxTS))		    // the control is a TextBox
                        textbox_list.Add(c);
                }

                temp.Clear();	// now that we have the controls we want, delete first list 

                ArrayList a;
                a = DB.GetVars("XTRVsettings");						    // Get the saved list of controls
                a.Sort();
                int num_controls = checkbox_list.Count +
                    numericupdown_list.Count + 
                    textbox_list.Count;

                // restore saved values to the controls
                foreach (string s in a)				                    // string is in the format "name,value"
                {
                    string[] vals = s.Split('/');
                    if (vals.Length > 2)
                    {
                        for (int i = 2; i < vals.Length; i++)
                            vals[1] += "/" + vals[i];
                    }

                    string name = vals[0];
                    string val = vals[1];

                    if (s.StartsWith("chk"))			                // control is a CheckBoxTS
                    {
                        for (int i = 0; i < checkbox_list.Count; i++)
                        {	// look through each control to find the matching name
                            CheckBoxTS c = (CheckBoxTS)checkbox_list[i];
                            if (c.Name.Equals(name))		            // name found
                            {
                                c.Checked = bool.Parse(val);	        // restore value
                                i = checkbox_list.Count + 1;
                            }
                            if (i == checkbox_list.Count)
                                MessageBox.Show("Control not found: " + name);
                        }
                    }
                    else if (s.StartsWith("ud"))
                    {
                        for (int i = 0; i < numericupdown_list.Count; i++)
                        {	// look through each control to find the matching name
                            NumericUpDownTS c = (NumericUpDownTS)numericupdown_list[i];
                            if (c.Name.Equals(name))		            // name found
                            {
                                decimal num = decimal.Parse(val);

                                if (num > c.Maximum) num = c.Maximum;	// check endpoints
                                else if (num < c.Minimum) num = c.Minimum;
                                c.Value = num;			                // restore value
                                i = numericupdown_list.Count + 1;
                            }
                            if (i == numericupdown_list.Count)
                                MessageBox.Show("Control not found: " + name);
                        }
                    }
                    else if (s.StartsWith("txt"))
                    {	// look through each control to find the matching name
                        for (int i = 0; i < textbox_list.Count; i++)
                        {
                            TextBoxTS c = (TextBoxTS)textbox_list[i];
                            if (c.Name.Equals(name))		            // name found
                            {
                                c.Text = val;	                        // restore value
                                i = textbox_list.Count + 1;
                            }
                            if (i == textbox_list.Count)
                                MessageBox.Show("Control not found: " + name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in XTRV GetOption function!\n" + ex.ToString());
            }
        }

        public void XTRV_update()
        {
            try
            {
                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 1, txtBandX1.Text.ToString(),
                    (double)udBandX1FreqMin.Value, (double)udBandX1FreqMax.Value, (double)udBandX1Losc.Value + (double)udBandX1LoscErr.Value / 1e3,
                    (double)udBandX1Pwr.Value, (double)udBandX1PwrGain.Value, chkBandX1RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 2, txtBandX2.Text.ToString(),
                    (double)udBandX2FreqMin.Value, (double)udBandX2FreqMax.Value, (double)udBandX2Losc.Value + (double)udBandX2LoscErr.Value / 1e3,
                    (double)udBandX2Pwr.Value, (double)udBandX2PwrGain.Value, chkBandX2RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 3, txtBandX3.Text.ToString(),
                    (double)udBandX3FreqMin.Value, (double)udBandX3FreqMax.Value, (double)udBandX3Losc.Value + (double)udBandX3LoscErr.Value / 1e3,
                    (double)udBandX3Pwr.Value, (double)udBandX3PwrGain.Value, chkBandX3RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 4, txtBandX4.Text.ToString(),
                    (double)udBandX4FreqMin.Value, (double)udBandX4FreqMax.Value, (double)udBandX4Losc.Value + (double)udBandX4LoscErr.Value / 1e3,
                    (double)udBandX4Pwr.Value, (double)udBandX4PwrGain.Value, chkBandX4RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 5, txtBandX5.Text.ToString(),
                    (double)udBandX5FreqMin.Value, (double)udBandX5FreqMax.Value, (double)udBandX5Losc.Value + (double)udBandX5LoscErr.Value / 1e3,
                    (double)udBandX5Pwr.Value, (double)udBandX5PwrGain.Value, chkBandX5RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 6, txtBandX6.Text.ToString(),
                    (double)udBandX6FreqMin.Value, (double)udBandX6FreqMax.Value, (double)udBandX6Losc.Value + (double)udBandX6LoscErr.Value / 1e3,
                    (double)udBandX6Pwr.Value, (double)udBandX6PwrGain.Value, chkBandX6RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 7, txtBandX7.Text.ToString(),
                    (double)udBandX7FreqMin.Value, (double)udBandX7FreqMax.Value, (double)udBandX7Losc.Value + (double)udBandX7LoscErr.Value / 1e3,
                    (double)udBandX7Pwr.Value, (double)udBandX7PwrGain.Value, chkBandX7RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 8, txtBandX8.Text.ToString(),
                    (double)udBandX8FreqMin.Value, (double)udBandX8FreqMax.Value, (double)udBandX8Losc.Value + (double)udBandX8LoscErr.Value / 1e3,
                    (double)udBandX8Pwr.Value, (double)udBandX8PwrGain.Value, chkBandX8RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 9, txtBandX9.Text.ToString(),
                    (double)udBandX9FreqMin.Value, (double)udBandX9FreqMax.Value, (double)udBandX9Losc.Value + (double)udBandX9LoscErr.Value / 1e3,
                    (double)udBandX9Pwr.Value, (double)udBandX9PwrGain.Value, chkBandX9RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 10, txtBandX10.Text.ToString(),
                    (double)udBandX10FreqMin.Value, (double)udBandX10FreqMax.Value, (double)udBandX10Losc.Value + (double)udBandX10LoscErr.Value / 1e3,
                    (double)udBandX10Pwr.Value, (double)udBandX10PwrGain.Value, chkBandX10RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 11, txtBandX11.Text.ToString(),
                    (double)udBandX11FreqMin.Value, (double)udBandX11FreqMax.Value, (double)udBandX11Losc.Value + (double)udBandX11LoscErr.Value / 1e3,
                    (double)udBandX11Pwr.Value, (double)udBandX11PwrGain.Value, chkBandX11RXonly.Checked);

                console.Invoke(new XTRV_cross_thread_callback(console.XTRV_cross_thread_callback), 12, txtBandX12.Text.ToString(),
                    (double)udBandX12FreqMin.Value, (double)udBandX12FreqMax.Value, (double)udBandX12Losc.Value + (double)udBandX12LoscErr.Value / 1e3,
                    (double)udBandX12Pwr.Value, (double)udBandX12PwrGain.Value, chkBandX12RXonly.Checked);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}
