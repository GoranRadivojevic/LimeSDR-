//=================================================================
//              SMeter2 class
//=================================================================
//
// Copyright (C)2012 YT7PWR Goran Radivojevic
// contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

namespace PowerSDR.Invoke
{
    public partial class SMeter2 : UserControl
    {
        #region variable

        private float signal_m_MaxValue = 200.0f;
        private float signal_m_MinValue = 0.0f;
        private float signal_m_value = 0.0f;
        private float swr_m_MaxValue = 200.0f;
        private float swr_m_MinValue = 0.0f;
        private float swr_m_value = 0.0f;
        int R = 0, G = 0, B = 0;
        int i = 0;
        float[] signal;
        int brightness = 255;

        #endregion

        #region constructor

        public SMeter2()
        {
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = (float)(8.25 / ratio);
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            this.PerformAutoScale();
            this.PerformLayout();
            signal = new float[picSignalLine.Width];

            for (int i = 0; i < picSignalLine.Width; i++)
            {
                signal[i] = Math.Max(0, i);
            }
        }

        #endregion

        #region properties

        public MeterTXMode PWR_SWR
        {
            set
            {
                switch (value)
                {
                    case MeterTXMode.SWR:
                        labelTS8.Text = "1.1";
                        labelTS9.Text = "1.3";
                        labelTS10.Text = "2.0";
                        labelTS11.Text = "3.0";
                        labelTS12.Text = "5.0";
                        labelTS13.Text = "50";
                        break;

                    case MeterTXMode.FORWARD_POWER:
                    case MeterTXMode.REVERSE_POWER:
                        labelTS8.Text = "1";
                        labelTS9.Text = "3";
                        labelTS10.Text = "5";
                        labelTS11.Text = "7";
                        labelTS12.Text = "10";
                        labelTS13.Text = "20";
                        break;

                    case MeterTXMode.OFF:
                        labelTS8.Text = "";
                        labelTS9.Text = "";
                        labelTS10.Text = "";
                        labelTS11.Text = "";
                        labelTS12.Text = "";
                        labelTS13.Text = "";
                        break;

                    case MeterTXMode.MIC:
                    case MeterTXMode.EQ:
                    case MeterTXMode.LEVELER:
                    case MeterTXMode.COMP:
                    case MeterTXMode.CPDR:
                    case MeterTXMode.ALC:
                        labelTS8.Text = "-15";
                        labelTS9.Text = "-5";
                        labelTS10.Text = "0";
                        labelTS11.Text = "1";
                        labelTS12.Text = "2";
                        labelTS13.Text = "3";
                        break;

                    case MeterTXMode.ALC_G:
                    case MeterTXMode.LVL_G:
                        labelTS8.Text = "0";
                        labelTS9.Text = "5";
                        labelTS10.Text = "10";
                        labelTS11.Text = "15";
                        labelTS12.Text = "20";
                        labelTS13.Text = "25";
                        break;
                }
            }
        }

        private Color meter_color = Color.White;
        public Color MeterForeColor
        {
            set
            {
                meter_color = value;
                ArrayList list = new ArrayList();
                ControlList(this, ref list);

                foreach (Label l in list)
                {
                    l.ForeColor = value;
                }

                if (value == Color.White)
                {
                    labelTS6.ForeColor = Color.Red;
                    labelTS5.ForeColor = Color.Red;
                    labelTS7.ForeColor = Color.Red;
                }

                picLineDown.Invalidate();
                picLineUp.Invalidate();
            }
        }

        public Single SignalMaxValue
        {
            get
            {
                return signal_m_MaxValue;
            }
            set
            {
                if ((signal_m_MaxValue != value)
                && (value > signal_m_MinValue))
                {
                    signal_m_MaxValue = picSignalLine.Width;
                    Refresh();
                }
            }
        }

        public Single swrMaxValue
        {
            get
            {
                return swr_m_MaxValue;
            }
            set
            {
                if ((swr_m_MaxValue != value)
                && (value > swr_m_MinValue))
                {
                    swr_m_MaxValue = value;
                    Refresh();
                }
            }
        }

        public Single SignalMinValue
        {
            get
            {
                return signal_m_MinValue;
            }
            set
            {
                if ((signal_m_MinValue != value)
                && (value < signal_m_MaxValue))
                {
                    signal_m_MinValue = value;
                    Refresh();
                }
            }
        }

        public Single swrMinValue
        {
            get
            {
                return swr_m_MinValue;
            }
            set
            {
                if ((swr_m_MinValue != value)
                && (value < swr_m_MaxValue))
                {
                    swr_m_MinValue = value;
                    Refresh();
                }
            }
        }

        public Single SignalValue
        {
            get
            {
                return signal_m_value;
            }
            set
            {
                value = ((picSignalLine.Width * value) / 18);

                if (signal_m_value != value)
                {
                    signal_m_value = Math.Min(Math.Max(value, signal_m_MinValue), signal_m_MaxValue);
                }

                picSignalLine.Invalidate();
            }
        }

        public Single swrValue
        {
            get
            {
                return swr_m_value;
            }
            set
            {
                value *= 10;

                if (swr_m_value != value)
                {
                    swr_m_value = Math.Min(Math.Max(value, swr_m_MinValue), swr_m_MaxValue);
                }

                picSignalLine.Invalidate();
            }
        }

        public int Brightness
        {
            get { return brightness; }
            set { brightness = Math.Max(0, value); }
        }

        #endregion

        #region render functions

        private void SMeter_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Pen pen = new Pen(Color.Black);
                Pen black_pen = new Pen(Color.Black);
                float overall_percent;
                int j = 0;

                for (i = 0; i < picSignalLine.Width; i++)
                {
                    overall_percent = signal[i] / picSignalLine.Width;

                    if (signal_m_value < i)
                    {
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                    else if (signal[i] >= SignalMaxValue)
                    {
                        R = 255;
                        G = 0;
                        B = 0;
                    }
                    else
                    {
                        if (overall_percent < (float)(2.0 / 9.0)) // green
                        {
                            R = 0;
                            G = 255;
                            B = 0;
                        }
                        else if (overall_percent < (float)(3.0 / 9.0)) // green to yellow-green
                        {
                            float local_percent = Math.Max(0, (overall_percent - (float)(1.8 / 9.0)) / ((float)(1.0 / 9.0)));
                            R = (int)Math.Min((local_percent) * 255, 255);
                            G = 255;
                            B = 0;
                        }
                        else if (overall_percent < (float)(7.0 / 9.0)) // yellow-green to yellow-red
                        {
                            float local_percent = Math.Max(0, (overall_percent - (float)(5.0 / 9.0)) / ((float)(2.0 / 9.0)));
                            R = 255;
                            G = (int)Math.Max((1.0 - local_percent) * 255, 0);
                            B = 0;
                        }
                        else if (overall_percent < (float)(8.0 / 9.0)) // yellow-red to red
                        {
                            float local_percent = Math.Max(0, (overall_percent - (float)(5.0 / 9.0)) / ((float)(2.0 / 9.0)));
                            R = 255;
                            G = (int)Math.Max((1.0 - local_percent) * 255, 0);
                            B = 0;
                        }
                        else // red
                        {
                            R = 255;
                            G = 0;
                            B = 0;
                        }
                    }

                    pen.Color = Color.FromArgb(brightness, R, G, B);

                    if (j == 7 || j ==8)
                    {
                        if (j == 8)
                            j = 0;

                        e.Graphics.DrawLine(black_pen, i, 0, i, picSignalLine.Height);
                    }
                    else
                        e.Graphics.DrawLine(pen, i, 0, i, picSignalLine.Height);

                    j++;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void picLineUp_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int j = 0;
                Pen black_pen = new Pen(Color.Black);
                Pen pen = new Pen(Color.FromArgb(136, 205, 205));

                if (meter_color != Color.White)
                {
                    pen.Color = meter_color;
                }

                for (i = 0; i < picLineUp.Width; i++)
                {
                    if (j == 7 || j == 8)
                    {
                        if (j == 8)
                            j = 0;

                        e.Graphics.DrawLine(black_pen, i, 0, i, picLineUp.Height);
                    }
                    else
                        e.Graphics.DrawLine(pen, i, 0, i, picLineUp.Height);

                    j++;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void picLineDown_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                int j = 0;
                Pen black_pen = new Pen(Color.Black);
                Pen pen = new Pen(Color.FromArgb(136, 205, 205));

                if (meter_color != Color.White)
                {
                    pen.Color = meter_color;
                }


                for (i = 0; i < picLineDown.Width; i++)
                {
                    if (j == 7 || j == 8)
                    {
                        if (j == 8)
                            j = 0;

                        e.Graphics.DrawLine(black_pen, i, 0, i, picLineDown.Height);
                    }
                    else
                        e.Graphics.DrawLine(pen, i, 0, i, picLineDown.Height);

                    j++;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void ControlList(Control c, ref ArrayList a)
        {
            if (c.Controls.Count > 0)
            {
                foreach (Control c2 in c.Controls)
                    ControlList(c2, ref a);
            }

            if (c.GetType() == typeof(LabelTS))
                a.Add(c);
        }

        #endregion
    }
}
