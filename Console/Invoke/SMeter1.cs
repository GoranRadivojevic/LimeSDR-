//=================================================================
//              SMeter1 class
//=================================================================
//
// Copyright (C)2011-2012 YT7PWR Goran Radivojevic
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
using System.Collections;

namespace PowerSDR.Invoke
{
    public partial class SMeter1 : UserControl
    {
        private float signal_m_MaxValue = 200.0f;
        private float signal_m_MinValue = 0.0f;
        private float signal_m_value = 0.0f;
        private float swr_m_MaxValue = 200.0f;
        private float swr_m_MinValue = 0.0f;
        private float swr_m_value = 0.0f;

        public SMeter1()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = (float)(8.25 / ratio);
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            this.PerformAutoScale();
            this.PerformLayout();
        }

        public Color MeterForeColor
        {
            set
            {
                ArrayList list = new ArrayList();
                ControlList(this, ref list);

                foreach (Label l in list)
                {
                    l.ForeColor = value;
                }
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
                    signal_m_MaxValue = value;
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
                value *= 10;

                if (signal_m_value != value)
                {
                    signal_m_value = Math.Min(Math.Max(value, signal_m_MinValue), signal_m_MaxValue);
                }

                progressSignal.Value = (int)signal_m_value;
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

                //progressSWR.Value = (int)swr_m_value;
                progressBottom.Value = (int)swr_m_value;
                progressTop.Value = (int)swr_m_value;
            }
        }

        public Color SignalLineColorLit
        {
            set
            {
                fruityLoopsProgressPainterSigLine.OnLit = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSigLine.pOnLit = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnLit), 1f);
                fruityLoopsProgressPainterSigLine.OnLitTop = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSigLine.pOnLitTop = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnLitTop), 1f);
                fruityLoopsProgressPainterSigLine.OnLitBot = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSigLine.pOnLitBot = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnLitBot), 1f);
            }
        }

        public Color SignalLineColorDrk
        {
            set
            {
                fruityLoopsProgressPainterSigLine.OnDrk = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSigLine.pOnDrk = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnDrk), 1f);
                fruityLoopsProgressPainterSigLine.OnDrkTop = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSigLine.pOnDrkTop = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnDrkTop), 1f);
                fruityLoopsProgressPainterSigLine.OnDrkBot = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSigLine.pOnDrkBot = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnDrkBot), 1f);

                fruityLoopsProgressPainterSWRLine.OnDrk = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSWRLine.pOnDrk = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnDrk), 1f);
                fruityLoopsProgressPainterSWRLine.OnDrkTop = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSWRLine.pOnDrkTop = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnDrkTop), 1f);
                fruityLoopsProgressPainterSWRLine.OnDrkBot = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSWRLine.pOnDrkBot = new Pen(new SolidBrush(fruityLoopsProgressPainterSigLine.OnDrkBot), 1f);
            }
        }

        public Color SWRLineColor
        {
            set
            {
                //progressSWR.ForeColor = value;
                fruityLoopsProgressPainterSWRLine.OnLit = Color.FromArgb(value.ToArgb());
                fruityLoopsProgressPainterSWRLine.pOnLit = new Pen(new SolidBrush(fruityLoopsProgressPainterSWRLine.OnLit), 1f);
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
    }
}
