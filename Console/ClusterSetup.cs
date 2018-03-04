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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

namespace PowerSDR
{
    public partial class ClusterSetup : Form
    {
        #region DLL imports

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

        #endregion

        const int WM_VSCROLL = 0x115;
        const int SB_BOTTOM = 7;
        DXClusterClient ParrentForm;

        public ClusterSetup(DXClusterClient form)
        {
            this.AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
            int dpi = (int)this.CreateGraphics().DpiX;

            if (dpi > 96)
            {
                string font_name = this.Font.Name;
                System.Drawing.Font new_font = new System.Drawing.Font(font_name, 6.5f);
                this.Font = new_font;
            }

            ParrentForm = form;
            GetOptions();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string[] vals = txtHosts.Text.Split('\n');
                ParrentForm.UpdateButtonsText();
                ParrentForm.UpdateHostsList(vals);
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void GetOptions()
        {
            try
            {
                ArrayList a = DB.GetVars("DXClusterOptions");
                a.Sort();

                foreach (string s in a)
                {
                    string[] vals = s.Split('/');
                    string name = vals[0];
                    string val = vals[1];

                    if (s.StartsWith("Cluster"))
                    {
                        txtHosts.AppendText(val + "\n");
                        SendMessage(txtHosts.Handle, WM_VSCROLL, SB_BOTTOM, 0);
                    }
                    else if (s.StartsWith("btn1text"))
                    {
                        txtButton1.Text = vals[1];
                    }
                    else if (s.StartsWith("btn2text"))
                    {
                        txtButton2.Text = vals[1];
                    }
                    else if (s.StartsWith("btn3text"))
                    {
                        txtButton3.Text = vals[1];
                    }
                    else if (s.StartsWith("btn4text"))
                    {
                        txtButton4.Text = vals[1];
                    }
                    else if (s.StartsWith("btn1cmd"))
                    {
                        btn1cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn2cmd"))
                    {
                        btn2cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn3cmd"))
                    {
                        btn3cmd.Text = vals[1];
                    }
                    else if (s.StartsWith("btn4cmd"))
                    {
                        btn4cmd.Text = vals[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}
