//=================================================================
// About
//=================================================================
//  
//  Copyright (C)2009-2013 YT7PWR Goran Radivojevic
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

/*
 * LimeSDR#  
 * Copyright (C)2018 YT7PWR Goran Radivojevic
 * contact via email at: yt7pwr@mts.rs
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace PowerSDR
{
    public partial class About : Form
    {
        Console console;

        public About(Console c, string radioInfo)
        {
            try
            {
                console = c;
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

                string[] data = radioInfo.Split('/');

                if(data != null && data.Length == 5)
                {
                    lblRadioModel.Text = data[0];
                    lblFirm_version.Text = data[1];
                    lblSerialNumber.Text = data[2];
                    lblGatewareVersion.Text = data[3];
                    lblLimeSuiteVersion.Text = data[4];
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Stream GetResource(string name)
        {
            return this.GetType().Assembly.GetManifestResourceStream(name);
        }

        private void About_Load(object sender, EventArgs e)
        {
            switch (console.CurrentModel)
            {

                case Model.LimeSDR:
                    {
                        lblRadioModel.Text = "LimeSDR";
                    }
                    break;

                case Model.MiniLimeSDR:
                    {
                        lblRadioModel.Text = "MiniLimeSDR";
                    }
                    break;

            }
        }
    }
}