//=================================================================
// VoiceMessages control
//=================================================================
//
//  Copyright (C)2009,2010,2011,2012 YT7PWR Goran Radivojevic
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
using System.Collections;
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
    public partial class VoiceMessages : System.Windows.Forms.Form
    {
        #region properties

        Console console;
        private string[] Message1_txt = new string[64];
        private string[] Message2_txt = new string[64];
        private string[] Message3_txt = new string[64];
        private string[] Message4_txt = new string[64];
        private string[] Message5_txt = new string[64];
        private string[] Message6_txt = new string[64];

        public bool PlayStop
        {
            set { btnStop_Click(null,null); }
        }

        public bool Msg1Play
        {
           set{ btnMsg1.Checked = value;}
        }

        public bool Msg2Play
        {
            set { btnMsg2.Checked = value; }
        }

        public bool Msg3Play
        {
            set { btnMsg3.Checked = value; }
        }

        public bool Msg4Play
        {
            set { btnMsg4.Checked = value; }
        }

        public bool Msg5Play
        {
            set { btnMsg5.Checked = value; }
        }

        public bool Msg6Play
        {
            set { btnMsg6.Checked = value; }
        }

        public bool Msg7Play
        {
            set { btnMsg7.Checked = value; }
        }

        public bool Msg8Play
        {
            set { btnMsg8.Checked = value; }
        }

        public bool Msg9Play
        {
            set { btnMsg9.Checked = value; }
        }

        public bool Msg10Play
        {
            set { btnMsg10.Checked = value; }
        }

        public bool Msg11Play
        {
            set { btnMsg11.Checked = value; }
        }

        public bool Msg12Play
        {
            set { btnMsg12.Checked = value; }
        }

        private bool end_playback = false;
        public bool PlaybackEnd
        {
            get{return end_playback;}
            set
            {
                if(value)
                {
                    btnMsg1.Checked = false;
                    btnMsg2.Checked = false;
                    btnMsg3.Checked = false;
                    btnMsg4.Checked = false;
                    btnMsg5.Checked = false;
                    btnMsg6.Checked = false;
                    btnMsg7.Checked = false;
                    btnMsg8.Checked = false;
                    btnMsg9.Checked = false;
                    btnMsg10.Checked = false;
                    btnMsg11.Checked = false;
                    btnMsg12.Checked = false;
                    console.btnMsg1.Checked = false;
                    console.btnMsg2.Checked = false;
                    console.btnMsg3.Checked = false;
                    console.btnMsg4.Checked = false;
                    console.btnMsg5.Checked = false;
                    console.btnMsg6.Checked = false;
                    console.MOX = false;
                }
                end_playback = value;
            }
        }

        public bool RecordingEnd
        {
            set
            {
                if (value)
                {
                    btnRecMsg1.Checked = false;
                    btnRecMsg2.Checked = false;
                    btnRecMsg3.Checked = false;
                    btnRecMsg4.Checked = false;
                    btnRecMsg5.Checked = false;
                    btnRecMsg6.Checked = false;
                    btnRecMsg7.Checked = false;
                    btnRecMsg8.Checked = false;
                    btnRecMsg9.Checked = false;
                    btnRecMsg10.Checked = false;
                    btnRecMsg11.Checked = false;
                    btnRecMsg12.Checked = false;
                }
            }
        }

        #endregion

        public VoiceMessages(Console c)
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
            RestoreSettings();
        }

        ~VoiceMessages()
        {
            SaveSettings();
        }

        private void FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Hide();
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void VoiceMessages_Load(object sender, EventArgs e)
        {
            RestoreSettings();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Hide();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void btnMsg1_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg1.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "1.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #1!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg2_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg2.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "2.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if(!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #2!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg3_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg3.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "3.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if(!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #3!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg4_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {           
                if (btnMsg4.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "4.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if(!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #4!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg5_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg5.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "5.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if(!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #5!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg6_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg6.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "6.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if(!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #6!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg7_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg7.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "7.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #7!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg8_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg8.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "8.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #8!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg9_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg9.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "9.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #9!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg10_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg10.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "10.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #10!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg11_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg11.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "11.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #11!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private void btnMsg12_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                if (btnMsg12.Checked && console.chkPower.Checked)
                {
                    string path = Application.StartupPath + "\\VoiceMsg\\" + "12.wav";
                    if (File.Exists(path))
                    {
                        console.MOX = true;
                        Thread.Sleep(20);
                        if (console.MOX)
                        {
                            if (!Play_message(path))
                            {
                                console.MOX = false;
                                PlaybackEnd = true;
                                btnStop_Click(null, null);
                            }
                        }
                        else
                            btnStop_Click(null, null);
                    }
                    else
                    {
                        console.MOX = false;
                        PlaybackEnd = true;
                        MessageBox.Show("Cannot open Voice Msg #12!",
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    btnStop_Click(null, null);
            }
        }

        private bool Play_message(string filename)
        {
            if (console.MOX)
            {
                RIFFChunk riff = null;
                fmtChunk fmt = null;
                dataChunk data_chunk = null;
                BinaryReader reader = null;

                try
                {
                    reader = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Voice message play: error openning file.\n" + ex.ToString());
                    console.MOX = false;
                    return false;
                }

                try
                {
                    if (reader.PeekChar() != 'R')
                    {
                        reader.Close();
                        MessageBox.Show("File is not in the correct format.",
                            "Wrong File Format",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }

                    while ((data_chunk == null ||
                        riff == null || fmt == null) &&
                        reader.PeekChar() != -1)
                    {
                        Chunk chunk = Chunk.ReadChunk(ref reader);
                        if (chunk.GetType() == typeof(RIFFChunk))
                            riff = (RIFFChunk)chunk;
                        else if (chunk.GetType() == typeof(fmtChunk))
                            fmt = (fmtChunk)chunk;
                        else if (chunk.GetType() == typeof(dataChunk))
                            data_chunk = (dataChunk)chunk;
                    }

                    if (reader.PeekChar() == -1)
                    {
                        reader.Close();
                        MessageBox.Show("File is not in the correct format.",
                            "Wrong File Format",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }

                    if (riff.riff_type != 0x45564157)
                    {
                        reader.Close();
                        MessageBox.Show("File is not an RIFF Wave file.",
                            "Wrong file format",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }

                    if (!CheckSampleRate(fmt.sample_rate))// || fmt.format == 1)
                    {
                        reader.Close();
                        MessageBox.Show("File has the wrong sample rate.",
                            "Wrong Sample Rate",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }

                    if (fmt.channels != 2)
                    {
                        reader.Close();
                        MessageBox.Show("Wave File is not stereo.",
                            "Wrong Number of Channels",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }

                    Audio.voice_msg_file_reader = new VoiceMsgWaveFileReader(
                        this,
                        console.RXBlockSize,
                        (int)fmt.format,	// use floating point
                        (int)fmt.sample_rate,
                        (int)fmt.channels,
                        ref reader, this);

                    Audio.voice_message_playback = true;

                    return true;
                }
                catch (Exception ex)
                {
                    reader.Close();
                    MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace.ToString(), "Fatal Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        private bool CheckSampleRate(int rate)
        {
            bool retval = false;
            switch (rate)
            {
                case 6000:
                case 12000:
                case 24000:
                case 48000:
                case 96000:
                case 192000:
                    retval = true;
                    break;
                default:
                    return false;
            }
            return retval;
        }

        private void SaveSettings()
        {
			ArrayList a = new ArrayList();
			ArrayList temp = new ArrayList();

			ControlList(this, ref temp);

			foreach(Control c in temp)
			{
				if(c.GetType() == typeof(TextBoxTS))
					a.Add(c.Name+"/"+((TextBoxTS)c).Text);
				else if(c.GetType() == typeof(ButtonTS))
				{
					Color clr = ((ColorButton)c).Color;
					a.Add(c.Name+"/"+clr.R+"."+clr.G+"."+clr.B+"."+clr.A);
				}
			}

			DB.SaveVars("VoiceMessages", ref a);
        }

        private void RestoreSettings()
        {
			ArrayList temp = new ArrayList();
			ControlList(this, ref temp);

			ArrayList textbox_list = new ArrayList();
			ArrayList button_list = new ArrayList();

			foreach(Control c in temp)
			{
				if(c.GetType() == typeof(TextBoxTS))
					textbox_list.Add(c);
				else if(c.GetType() == typeof(Button))
					button_list.Add(c);
			}
			temp.Clear();

			ArrayList a = DB.GetVars("VoiceMessages");
			a.Sort();
			
			// restore saved values to the controls
			foreach(string s in a)
			{
				string[] vals = s.Split('/');
				if(vals.Length > 2)
				{
					for(int i=2; i<vals.Length; i++)
						vals[1] += "/"+vals[i];
				}

				string name = vals[0];
				string val = vals[1];

				if(s.StartsWith("txt"))
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
                c.GetType() == typeof(ComboBoxTS) || c.GetType() == typeof(ComboBox) ||
                c.GetType() == typeof(NumericUpDownTS) || c.GetType() == typeof(NumericUpDownTS) ||
                c.GetType() == typeof(RadioButtonTS) || c.GetType() == typeof(RadioButton) ||
                c.GetType() == typeof(TextBoxTS) || c.GetType() == typeof(TextBoxTS) ||
                c.GetType() == typeof(TrackBarTS) || c.GetType() == typeof(TrackBar) ||
                c.GetType() == typeof(ColorButton))
                a.Add(c);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                console.MOX = false;

                if (Audio.voice_msg_file_reader != null)
                    Audio.voice_msg_file_reader.playback = false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnRecMsg1_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg1.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg1.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "1.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg1.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg1.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg1.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg1.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg1.Checked = false;
                btnRecMsg1.Text = "REC";
                btnRecMsg1.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg2_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg2.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg2.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "2.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg2.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg2.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg2.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg2.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg2.Checked = false;
                btnRecMsg2.Text = "REC";
                btnRecMsg2.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg3_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg3.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg3.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "3.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg3.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg3.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg3.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg3.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg3.Checked = false;
                btnRecMsg3.Text = "REC";
                btnRecMsg3.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg4_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg4.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg4.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "4.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg4.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg4.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg4.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg4.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg4.Checked = false;
                btnRecMsg4.Text = "REC";
                btnRecMsg4.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg5_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg5.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg5.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "5.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg5.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg5.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg5.Text = "REC";
                    }

                    Audio.voice_message_record = btnRecMsg5.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg5.Checked = false;
                btnRecMsg5.Text = "REC";
                btnRecMsg5.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg6_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg6.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg6.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "6.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg6.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg6.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg6.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg6.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg6.Checked = false;
                btnRecMsg6.Text = "REC";
                btnRecMsg6.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg7_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg7.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg7.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "7.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg7.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg7.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg7.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg7.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg7.Checked = false;
                btnRecMsg7.Text = "REC";
                btnRecMsg7.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg8_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg8.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg8.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "8.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg8.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg8.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg8.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg8.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg8.Checked = false;
                btnRecMsg8.Text = "REC";
                btnRecMsg8.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg9_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg9.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg9.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "9.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg9.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg9.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg9.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg9.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg9.Checked = false;
                btnRecMsg9.Text = "REC";
                btnRecMsg9.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg10_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg10.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg10.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "10.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg10.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg10.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg10.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg10.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg10.Checked = false;
                btnRecMsg10.Text = "REC";
                btnRecMsg10.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg11_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg11.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg11.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "11.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg11.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg11.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg11.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg11.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg11.Checked = false;
                btnRecMsg11.Text = "REC";
                btnRecMsg11.BackColor = SystemColors.Control;
            }
        }

        private void btnRecMsg12_CheckedChanged(object sender, EventArgs e)
        {
            if (console.chkPower.Checked)
            {
                try
                {
                    if (btnRecMsg12.Checked && console.chkPower.Checked && !console.MOX)
                    {
                        console.VoiceRecording = true;
                        btnRecMsg12.BackColor = console.ButtonSelectedColor;
                        if (!Directory.Exists(Application.StartupPath + "\\VoiceMsg"))
                            Directory.CreateDirectory(Application.StartupPath + "\\VoiceMsg");
                        string file_name = Application.StartupPath + "\\VoiceMsg\\" + "12.wav";
                        Audio.voice_msg_file_writer = new VoiceMsgWaveFileWriter(console.RXBlockSize, 2,
                            Audio.RXSampleRate, file_name);
                        console.VoiceMSG = true;
                        btnRecMsg12.Text = "Stop";
                    }
                    else
                    {
                        console.VoiceRecording = false;
                        string file_name = Audio.voice_msg_file_writer.Stop();
                        btnRecMsg12.BackColor = SystemColors.Control;
                        console.VoiceMSG = false;
                        btnRecMsg12.Text = "REC";
                    }
                    Audio.voice_message_record = btnRecMsg12.Checked;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!" + ex.ToString());
                    console.VoiceMSG = false;
                }
            }
            else
            {
                btnRecMsg12.Checked = false;
                btnRecMsg12.Text = "REC";
                btnRecMsg12.BackColor = SystemColors.Control;
            }
        }
    }

    #region Messages Wave File Reader Class

    unsafe public class VoiceMsgWaveFileReader
    {
        private VoiceMessages wave_form;
        private BinaryReader reader;
        private int format;
        public bool playback;
        private int frames_per_buffer;
        private RingBufferFloat rb_l;
        private RingBufferFloat rb_r;
        private float[] buf_l_in;
        private float[] buf_r_in;
        private float[] buf_l_out;
        private float[] buf_r_out;
        private int IN_BLOCK;
        private int OUT_BLOCK;
        private byte[] io_buf;
        private int io_buf_size;
        VoiceMessages VoiceMsgForm;
        private int sample_rate;
        private int channels;
        private bool eof = false;
        unsafe private static void* cs_play_voice;

        unsafe private void* resamp_l, resamp_r;

        unsafe public VoiceMsgWaveFileReader(
            VoiceMessages form,
            int frames,
            int fmt,
            int samp_rate,
            int chan,
            ref BinaryReader binread,
            VoiceMessages voice_msg_form)
        {
            VoiceMsgForm = voice_msg_form;
            wave_form = form;
            frames_per_buffer = frames;
            format = fmt;
            sample_rate = samp_rate;
            channels = chan;
            IN_BLOCK = 2048;
            OUT_BLOCK = (int)Math.Ceiling(IN_BLOCK * Audio.RXSampleRate / (double)sample_rate);
            if (OUT_BLOCK < IN_BLOCK)
                OUT_BLOCK = IN_BLOCK;
            rb_l = new RingBufferFloat(16 * OUT_BLOCK);
            rb_r = new RingBufferFloat(16 * OUT_BLOCK);
            buf_l_in = new float[IN_BLOCK];
            buf_r_in = new float[IN_BLOCK];
            buf_l_out = new float[OUT_BLOCK];
            buf_r_out = new float[OUT_BLOCK];

            if (format == 1)
                io_buf_size = 2048 * 2 * 2;
            else if (format == 3)
            {
                io_buf_size = 2048 * 4 * 2;
            }

            if (sample_rate != Audio.RXSampleRate)
            {
                resamp_l = DttSP.NewResamplerF(sample_rate, Audio.RXSampleRate);

                if (channels > 1) 
                    resamp_r = DttSP.NewResamplerF(sample_rate, Audio.RXSampleRate);
            }

            io_buf = new byte[io_buf_size];

            playback = true;
            reader = binread;

            cs_play_voice = (void*)0x0;
            cs_play_voice = Win32.NewCriticalSection();

            if (Win32.InitializeCriticalSectionAndSpinCount(cs_play_voice, 0x00000080) == 0)
            {
                Debug.WriteLine("Voice Play criticalSection Failed!\n");
            }

            Thread t = new Thread(new ThreadStart(ProcessBuffers));
            t.Name = "Wave File Read Thread";
            t.IsBackground = true;
            t.Priority = ThreadPriority.Normal;
            t.Start();
        }

        private void ProcessBuffers()
        {
            try
            {
                int watchdog = 0;

                while (playback == true)
                {
                    while (rb_l.WriteSpace() >= OUT_BLOCK && !eof )
                    {
                        //Debug.WriteLine("loop 2");
                        ReadBuffer(ref reader);

                        if (playback == false)
                            goto end;

                    }

                    if (playback == false)
                        goto end;

                    Thread.Sleep(1);
                }

            end:
                {
                    while (rb_l.ReadSpace() >= IN_BLOCK && watchdog < 100)       // 1000mS watchdog period
                    {
                        Thread.Sleep(10);
                        watchdog++;
                    }

                    Win32.DeleteCriticalSection(cs_play_voice);
                    reader.Close();
                    VoiceMsgForm.PlaybackEnd = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                reader.Close();
                playback = false;
                VoiceMsgForm.PlaybackEnd = true;
            }
        }

        private void ReadBuffer(ref BinaryReader reader)
        {
            try
            {
                //Debug.WriteLine("ReadBuffer ("+rb_l.ReadSpace()+")");
                int i = 0, num_reads = IN_BLOCK;
                int val = reader.Read(io_buf, 0, io_buf_size);

                if (val == 0)
                {
                    eof = true;
                    playback = false;
                    num_reads = 0;
                }
                else if (val < io_buf_size)
                {
                    switch (format)
                    {
                        case 1:		// ints
                            num_reads = val / 4;
                            break;
                        case 3:		// floats
                            num_reads = val / 8;
                            break;
                    }
                }

                for (; i < num_reads; i++)
                {
                    switch (format)
                    {
                        case 1:		// ints
                            buf_l_in[i] = (float)((double)BitConverter.ToInt16(io_buf, i * 4) / 32767.0);
                            buf_r_in[i] = (float)((double)BitConverter.ToInt16(io_buf, i * 4 + 2) / 32767.0);
                            break;
                        case 3:		// floats
                            buf_l_in[i] = BitConverter.ToSingle(io_buf, i * 8);
                            buf_r_in[i] = BitConverter.ToSingle(io_buf, i * 8 + 4);
                            break;
                    }
                }

                if (num_reads < IN_BLOCK)
                {
                    for (int j = i; j < IN_BLOCK; j++)
                        buf_l_in[j] = buf_r_in[j] = 0.0f;

                    //reader.Close();
                    //playback = false;
                }

                int out_cnt = IN_BLOCK;
                
                if (sample_rate != Audio.RXSampleRate)
                {
                    fixed (float* in_ptr = &buf_l_in[0])
                    fixed (float* out_ptr = &buf_l_out[0])
                        DttSP.DoResamplerF(in_ptr, out_ptr, IN_BLOCK, &out_cnt, resamp_l);
                    if (channels > 1)
                    {
                        fixed (float* in_ptr = &buf_r_in[0])
                        fixed (float* out_ptr = &buf_r_out[0])
                            DttSP.DoResamplerF(in_ptr, out_ptr, IN_BLOCK, &out_cnt, resamp_r);
                    }
                }
                else
                {
                    buf_l_in.CopyTo(buf_l_out, 0);
                    buf_r_in.CopyTo(buf_r_out, 0);
                }

                Win32.EnterCriticalSection(cs_play_voice);
                rb_l.Write(buf_l_out, out_cnt);

                if (channels > 1) 
                    rb_r.Write(buf_r_out, out_cnt);

                else
                    rb_r.Write(buf_l_out, out_cnt);

                Win32.LeaveCriticalSection(cs_play_voice);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                reader.Close();
                playback = false;
            }
        }

        unsafe public void GetPlayBuffer(float* l_ptr, float* r_ptr)
        {
            try
            {
                //Debug.WriteLine("GetPlayBuffer ("+rb_l.ReadSpace()+")");
                int count = rb_l.ReadSpace();
                if (count == 0) return;

                if (count > frames_per_buffer)
                    count = frames_per_buffer;

                Win32.EnterCriticalSection(cs_play_voice);
                rb_l.Read(buf_l_out, count);
                rb_r.Read(buf_r_out, count);
                Win32.LeaveCriticalSection(cs_play_voice);

                if (count < frames_per_buffer)
                {
                    for (int i = count; i < frames_per_buffer; i++)
                        buf_l_out[i] = buf_r_out[i] = 0.0f;
                }

                for (int i = 0; i < frames_per_buffer; i++)
                {
                    *l_ptr++ = buf_l_out[i];
                    *r_ptr++ = buf_r_out[i];
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                reader.Close();
                playback = false;
            }
        }

        // FIXME: implement interleaved version of Get Play Buffer


        public void Stop()
        {
            playback = false;
        }
    }
    #endregion

    #region Voice messages Wave File Writer Class

    unsafe public class VoiceMsgWaveFileWriter
    {
        private BinaryWriter writer;
        private bool record;
        private int frames_per_buffer;
        private short channels;
        private int sample_rate;
        private int length_counter;
        private RingBufferFloat rb_l;
        private RingBufferFloat rb_r;
        private float[] in_buf_l;
        private float[] in_buf_r;
        private float[] out_buf_l;
        private float[] out_buf_r;
        private float[] out_buf;
        private byte[] byte_buf;
        private int IN_BLOCK = 2048;
        private string filename;

        unsafe private void* resamp_l, resamp_r;

        public VoiceMsgWaveFileWriter(int frames, short chan, int samp_rate, string file)
        {
            frames_per_buffer = frames;
            channels = chan;
            sample_rate = samp_rate;
            IN_BLOCK = frames;
            int OUT_BLOCK = (int)Math.Ceiling(IN_BLOCK * (double)sample_rate / Audio.RXSampleRate);
            rb_l = new RingBufferFloat(IN_BLOCK * 16);
            rb_r = new RingBufferFloat(IN_BLOCK * 16);
            in_buf_l = new float[IN_BLOCK];
            in_buf_r = new float[IN_BLOCK];
            out_buf_l = new float[OUT_BLOCK];
            out_buf_r = new float[OUT_BLOCK];
            out_buf = new float[OUT_BLOCK * 2];
            byte_buf = new byte[OUT_BLOCK * 2 * 4];

            length_counter = 0;
            record = true;

            if (sample_rate != Audio.RXSampleRate)
            {
                resamp_l = DttSP.NewResamplerF(Audio.RXSampleRate, sample_rate);
                if (channels > 1) resamp_r = DttSP.NewResamplerF(Audio.RXSampleRate, sample_rate);
            }

            writer = new BinaryWriter(File.Open(file, FileMode.Create));
            filename = file;

            Thread t = new Thread(new ThreadStart(ProcessRecordBuffers));
            t.Name = "Wave File Write Thread";
            t.IsBackground = true;
            t.Priority = ThreadPriority.Normal;
            t.Start();
        }

        private void ProcessRecordBuffers()
        {
            try
            {
                WriteWaveHeader(ref writer, channels, sample_rate, 32, 0);

                while (record == true || rb_l.ReadSpace() > 0)
                {
                    if (rb_l.ReadSpace() > IN_BLOCK ||
                        (record == false && rb_l.ReadSpace() > 0))
                    {
                        WriteBuffer(ref writer, ref length_counter);
                    }
                    if (!record)
                        goto end;
                    Thread.Sleep(1);
                }

                end:
                writer.Seek(0, SeekOrigin.Begin);
                WriteWaveHeader(ref writer, channels, sample_rate, 32, length_counter);
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                writer.Seek(0, SeekOrigin.Begin);
                WriteWaveHeader(ref writer, channels, sample_rate, 32, length_counter);
                writer.Flush();
                writer.Close();
            }
        }

        unsafe public void AddWriteBuffer(float* left, float* right)
        {
            rb_l.WritePtr(left, frames_per_buffer);
            rb_r.WritePtr(right, frames_per_buffer);
            //Debug.WriteLine("ReadSpace: "+rb.ReadSpace());
        }

        public string Stop()
        {
            record = false;
            return filename;
        }

        private void WriteBuffer(ref BinaryWriter writer, ref int count)
        {
            try
            {
                int cnt = rb_l.Read(in_buf_l, IN_BLOCK);
                rb_r.Read(in_buf_r, IN_BLOCK);
                int out_cnt = IN_BLOCK;

                // resample
                if (sample_rate != Audio.RXSampleRate)
                {
                    fixed (float* in_ptr = &in_buf_l[0])
                    fixed (float* out_ptr = &out_buf_l[0])
                        DttSP.DoResamplerF(in_ptr, out_ptr, cnt, &out_cnt, resamp_l);
                    if (channels > 1)
                    {
                        fixed (float* in_ptr = &in_buf_r[0])
                        fixed (float* out_ptr = &out_buf_r[0])
                            DttSP.DoResamplerF(in_ptr, out_ptr, cnt, &out_cnt, resamp_r);
                    }
                }
                else
                {
                    in_buf_l.CopyTo(out_buf_l, 0);
                    in_buf_r.CopyTo(out_buf_r, 0);
                }

                if (channels > 1)
                {
                    // interleave samples
                    for (int i = 0; i < out_cnt; i++)
                    {
                        out_buf[i * 2] = out_buf_l[i];
                        out_buf[i * 2 + 1] = out_buf_r[i];
                    }
                }
                else
                {
                    out_buf_l.CopyTo(out_buf, 0);
                }

                byte[] temp = new byte[4];
                int length = out_cnt;
                if (channels > 1) length *= 2;
                for (int i = 0; i < length; i++)
                {
                    temp = BitConverter.GetBytes(out_buf[i]);
                    for (int j = 0; j < 4; j++)
                        byte_buf[i * 4 + j] = temp[j];
                }

                writer.Write(byte_buf, 0, out_cnt * 2 * 4);
                count += out_cnt * 2 * 4;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void WriteWaveHeader(ref BinaryWriter writer, short channels, int sample_rate, short bit_depth, int data_length)
        {
            writer.Write(0x46464952);								        // "RIFF"		-- descriptor chunk ID
            writer.Write(data_length + 36);							        // size of whole file -- 1 for now
            writer.Write(0x45564157);								        // "WAVE"		-- descriptor type
            writer.Write(0x20746d66);								        // "fmt "		-- format chunk ID
            writer.Write((int)16);									        // size of fmt chunk
            writer.Write((short)3);									        // FormatTag	-- 3 for floats
            writer.Write((short)channels);									// wChannels
            writer.Write((int)sample_rate);								    // dwSamplesPerSec
            writer.Write((int)(channels * sample_rate * bit_depth / 8));	// dwAvgBytesPerSec
            writer.Write((short)(channels * bit_depth / 8));			    // wBlockAlign
            writer.Write((short)bit_depth);								    // wBitsPerSample
            writer.Write(0x61746164);								        // "data" -- data chunk ID
            writer.Write(data_length);								        // chunkSize = length of data
            writer.Flush();											        // write the file
        }
    }

    #endregion
}
