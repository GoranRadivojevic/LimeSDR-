//=================================================================
// Debug form
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2011-2013 YT7PWR
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;
using System.Diagnostics;

namespace PowerSDR
{
    public partial class DebugForm : Form
    {
        Console console;
        string file = "debug report.txt";

        public DebugForm(Console c, bool enable_debug)
        {
            try
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

                if (enable_debug)
                {
                    chkAudio.Checked = true;
                    chkCAT.Checked = true;
                    chkConsole.Checked = true;
                    chkDirectX.Checked = true;
                    chkIRRemote.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                rtbDebugMsg.Clear();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Title = "Save debug report";
                saveFileDialog1.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #region enable/disable debugging

        private void chkAudio_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Audio.debug = chkAudio.Checked;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void chkDirectX_CheckedChanged(object sender, EventArgs e)
        {
#if(DirectX)
            Display_DirectX.debug = chkDirectX.Checked;
#endif
        }

        private void chkCAT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                console.Siolisten.debug = chkCAT.Checked;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void chkConsole_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                console.debug_enabled = chkConsole.Checked;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void chkEthernet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void chkIRRemote_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (console.ir_remote != null)
                    console.ir_remote.debug = chkIRRemote.Checked;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (saveFileDialog1.FileName != "")
                    file = saveFileDialog1.FileName.ToString();

                BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Create));
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(rtbDebugMsg.Text.ToCharArray(), 0, rtbDebugMsg.Text.Length);
                writer.Flush();
                writer.Close();
                writer = null;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void buttonTS1_Click(object sender, EventArgs e)
        {
            try
            {
                Direct3D w = new Direct3D();
                Capabilities q = w.GetDeviceCaps(0, DeviceType.Hardware);
                rtbDebugMsg.AppendText("Vertex Shader ver: \n" + q.VertexShaderVersion.ToString() + "\n");
                rtbDebugMsg.AppendText("Device Type: \n" + q.DeviceType.ToString() + "\n");
                rtbDebugMsg.AppendText("Device Caps: \n" + q.DeviceCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("Device Caps2: \n" + q.DeviceCaps2.ToString() + "\n");
                rtbDebugMsg.AppendText("Pixel Shader: \n" + q.PixelShaderVersion.ToString() + "\n");
                rtbDebugMsg.AppendText("Texture Caps: \n" + q.TextureCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("FVF Caps: \n" + q.FVFCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("Line Caps: \n" + q.LineCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("MaxVertexIndex Caps: \n" + q.MaxVertexIndex.ToString() + "\n");
                rtbDebugMsg.AppendText("PrimitiveMiscCaps: \n" + q.PrimitiveMiscCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("RatserCaps: \n" + q.RasterCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("VertexProcessingCaps: \n" + q.VertexProcessingCaps.ToString() + "\n");
                rtbDebugMsg.AppendText("VS20Caps: \n" + q.VS20Caps.ToString() + "\n");
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}
