//=================================================================
// display.cs
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

/*
 *  Changes for GenesisRadio
 *  Copyright (C)2010-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

#if(DirectX)

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows;

namespace PowerSDR
{
    #region structures

    struct Vertex
    {
        public Vector4 Position;
        public int Color;
    }

    struct DXRectangle
    {
        public int x1;
        public int x2;
        public int x3;
        public int x4;
        public int y1;
        public int y2;
        public int y3;
        public int y4;
    }

    struct VerticalString
    {
        public int pos_x;
        public int pos_y;
        public string label;
        public Color color;
    }

    struct HorizontalString
    {
        public int pos_x;
        public int pos_y;
        public string label;
        public Color color;
    }

    struct HistogramData
    {
        public int X;
        public int Y;
        public Color color;
    }

    #endregion

    static class Display_DirectX
    {
        #region Variable Declaration

        public static Console console;
        private static int[] histogram_history;					// histogram counter
        public const float CLEAR_FLAG = -999.999F;				// for resetting buffers
        public static int BUFFER_SIZE = 32768;
        public static float[] new_display_data;					// Buffer used to store the new data from the DSP for the display
        public static float[] new_scope_data;					// Buffer used to store the new data from the DSP for the scope
        public static float[] new_waterfall_data;    			// Buffer used to store the new data from the DSP for the waterfall
        public static float[] current_display_data;				// Buffer used to store the current data for the display
        public static float[] current_waterfall_data;		    // Buffer used to store the current data for the scope
        public static float[] current_scope_data;		        // Buffer used to store the current data for the waterfall
        public static float[] waterfall_display_data;           // Buffer for waterfall
        public static float[] average_buffer;					// Averaged display data buffer for Panadapter
        public static float[] average_waterfall_buffer;  		// Averaged display data buffer for Waterfall
        public static float[] peak_buffer;						// Peak hold display data buffer

        private static Device device = null;
        private static Texture PanadapterTexture = null;
        private static Texture WaterfallTexture = null;
        private static Texture WaterfallBackgroundTexture = null;
        private static Sprite Panadapter_Sprite = null;
        private static Sprite Waterfall_Sprite = null;
        private static Rectangle Panadapter_texture_size;
        private static Rectangle Waterfall_texture_size;
        private static DXRectangle VFOArect;
        private static DXRectangle VFOBrect;
        private static VertexBuffer VerLine_vb = null;
        private static VertexBuffer HorLine_vb = null;
        private static VertexBuffer VerLines_vb = null;
        private static VertexBuffer HorLines_vb = null;
        private static VertexBuffer Panadapter_vb = null;
        private static VertexBuffer Waterfall_vb = null;
        //private static VertexBuffer ScopeLine_vb = null;
        private static VertexBuffer PanLine_vb_fill = null;
        private static Vertex[] Panadapter_verts = null;
        private static Vertex[] Waterfall_verts = null;
        //private static Vertex[] ScopeLine_verts = null;
        private static Vertex[] Panadapter_verts_fill = null;
        //private static Vertex[] Phase_verts = null;
        //private static VertexBuffer Phase_vb = null;
        private static Vertex[] HistogramLine_verts = null;
        private static VertexBuffer Histogram_vb = null;
        private static float[] waterfallX_data = null;
        private static float[] panadapterX_data = null;
        private static float[] panadapterX_scope_data = null;
        private static int[] histogram_data = null;
        private static System.Drawing.Bitmap waterfall_bmp = null;
        private static Device waterfall_dx_device = null;
        private static Point[] points = null;
        private static HistogramData[] histogram_verts = null;
        private static SlimDX.Direct3D9.Font high_swr_font;
        private static Rectangle waterfall_rect;
        private static byte[] waterfall_memory;
        private static Surface backbuf;
        private static int waterfall_bmp_size;
        private static int waterfall_bmp_stride;
        private static DataStream waterfall_data_stream;
        private delegate void DebugCallbackFunction(string name);
        public static bool debug = false;
        public static bool booting = true;
        public static string background_image = null;
        private static SlimDX.Direct3D9.Line panadapter_line;
        private static SlimDX.Direct3D9.Line panadapter_fill_line;
        private static Vector2[] panadapter_verts;
        private static Vector2[] panadapter_fill_verts;
        private static Color4 linecolor = new Color4(Color.Blue);
        private static PresentParameters presentParms;
        public static bool DX_reinit = false;

        #endregion

        #region Properties

        private static RenderType directx_render_type = RenderType.HARDWARE;
        public static RenderType DirectXRenderType
        {
            get { return directx_render_type; }
            set { directx_render_type = value; }
        }

        private static float[] scope_min;
        public static float[] ScopeMin
        {
            get { return scope_min; }
            set { scope_min = value; }
        }

        private static float[] scope_max;
        public static float[] ScopeMax
        {
            get { return scope_max; }
            set { scope_max = value; }
        }

        private static bool refresh_grid = true;                            // yt7pwr
        public static bool RefreshGrid
        {
            set { refresh_grid = value; }
        }

        private static bool refresh_panadapter_grid = true;                 // yt7pwr
        public static bool RefreshPanadapterGrid
        {
            set { refresh_panadapter_grid = value; }
        }

        private static ColorSheme color_sheme = ColorSheme.original;        // yt7pwr
        public static ColorSheme ColorSheme
        {
            get { return color_sheme; }

            set { color_sheme = value; }
        }

        private static bool reverse_waterfall = false;                      // yt7pwr
        public static bool ReverseWaterfall
        {
            get { return reverse_waterfall; }
            set { reverse_waterfall = value; }
        }

        private static bool smooth_line = false;                             // yt7pwr
        public static bool SmoothLine
        {
            get { return smooth_line; }
            set
            {
                smooth_line = value;

                if (panadapter_line != null && panadapter_fill_line != null)
                {
                    panadapter_fill_line.Antialias = value;
                    panadapter_line.Antialias = value;
                }
            }
        }

        public static bool pan_fill = false; 

        private static System.Drawing.Font pan_font = new System.Drawing.Font("Arial", 12);
        private static SlimDX.Direct3D9.Font panadapter_font = null;
        public static System.Drawing.Font PanFont
        {
            get { return pan_font; }
            set
            {
                pan_font = value;
                refresh_panadapter_grid = true;

                if (!console.booting)
                {
                    if (panadapter_font != null)
                        panadapter_font.Dispose();

                    panadapter_font = new SlimDX.Direct3D9.Font(device, pan_font);
                }
            }
        }

        private static Color scope_color = Color.FromArgb(100, 0, 0, 127);
        public static Color ScopeColor
        {
            get { return scope_color; }
            set { scope_color = value; }
        }

        private static Color pan_fill_color = Color.FromArgb(100, 0, 0, 127);
        public static Color PanFillColor
        {
            get { return pan_fill_color; }
            set { pan_fill_color = value; }
        }

        private static Color display_text_background = Color.FromArgb(255, 127, 127, 127);
        public static Color DisplayTextBackground
        {
            get { return display_text_background; }
            set { display_text_background = value; }
        }

        private static Color display_filter_color = Color.FromArgb(65, 255, 255, 255);
        public static Color DisplayFilterColor
        {
            get { return display_filter_color; }
            set { display_filter_color = value; }
        }

        private static bool show_horizontal_grid = false;
        public static bool Show_Horizontal_Grid
        {
            set
            {
                show_horizontal_grid = value;
                refresh_panadapter_grid = true;
            }
        }

        private static bool show_vertical_grid = false;
        public static bool Show_Vertical_Grid
        {
            set
            {
                show_vertical_grid = value;
                refresh_panadapter_grid = true;
            }
        }

        private static int phase_num_pts = 100;
        public static int PhaseNumPts
        {
            get { return phase_num_pts; }
            set { phase_num_pts = value; }
        }

        private static int waterfall_alpha = 255;
        public static int WaterfallAlpha
        {
            get { return waterfall_alpha; }
            set { waterfall_alpha = value; }
        }

        private static Color main_rx_zero_line_color = Color.LightSkyBlue;
        public static Color MainRXZeroLine
        {
            get { return main_rx_zero_line_color; }
            set
            {
                main_rx_zero_line_color = value;
            }
        }

        private static Color sub_rx_zero_line_color = Color.LightSkyBlue;
        public static Color SubRXZeroLine
        {
            get { return sub_rx_zero_line_color; }
            set
            {
                sub_rx_zero_line_color = value;
            }
        }

        private static Color main_rx_filter_color = Color.FromArgb(100, 0, 255, 0);  // green
        public static Color MainRXFilterColor
        {
            get { return main_rx_filter_color; }
            set
            {
                main_rx_filter_color = value;
            }
        }

        private static Color sub_rx_filter_color = Color.FromArgb(100, 0, 0, 255);  // blue
        public static Color SubRXFilterColor
        {
            get { return sub_rx_filter_color; }
            set
            {
                sub_rx_filter_color = value;
            }
        }

        private static bool sub_rx_enabled = false;
        public static bool SubRXEnabled
        {
            get { return sub_rx_enabled; }
            set
            {
                sub_rx_enabled = value;
            }
        }

        private static bool split_enabled = false;
        public static bool SplitEnabled
        {
            get { return split_enabled; }
            set
            {
                split_enabled = value;
            }
        }

        private static bool show_freq_offset = false;
        public static bool ShowFreqOffset
        {
            get { return show_freq_offset; }
            set
            {
                show_freq_offset = value;
            }
        }

        private static Color band_edge_color = Color.Red;
        public static Color BandEdgeColor
        {
            get { return band_edge_color; }
            set
            {
                band_edge_color = value;
            }
        }

        private static long losc_hz; // yt7pwr
        public static long LOSC
        {
            get { return losc_hz; }
            set 
            {
                losc_hz = value;

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "New LOSC value: " + losc_hz.ToString());
            }
        }

        private static long vfoa_hz;
        public static long VFOA
        {
            get { return vfoa_hz; }
            set
            {
                vfoa_hz = value;
            }
        }

        private static long vfob_hz;
        public static long VFOB
        {
            get { return vfob_hz; }
            set
            {
                vfob_hz = value;
            }
        }

        private static int rit_hz;
        public static int RIT
        {
            get { return rit_hz; }
            set
            {
                rit_hz = value;
            }
        }

        private static int xit_hz;
        public static int XIT
        {
            get { return xit_hz; }
            set
            {
                xit_hz = value;
            }
        }

        private static int cw_pitch = 600;
        public static int CWPitch
        {
            get { return cw_pitch; }
            set { cw_pitch = value; }
        }

        private static int panadapter_H = 0;	// target height
        private static int panadapter_W = 0;	// target width
        private static Control panadapter_target = null;
        public static Control PanadapterTarget
        {
            get { return panadapter_target; }
            set
            {
                panadapter_target = value;
                panadapter_H = panadapter_target.Height;
                panadapter_W = panadapter_target.Width;
            }
        }

        private static int waterfall_H = 0;	// target height
        private static int waterfall_W = 0;	// target width
        private static Control waterfall_target = null;
        public static Control WaterfallTarget
        {
            get { return waterfall_target; }
            set
            {
                waterfall_target = value;
                waterfall_H = waterfall_target.Height;
                waterfall_W = waterfall_target.Width;
            }
        }

        public static bool DisplayNotchFilter = false;
        private static int rx_display_notch_low_cut = -4000;        // yt7pwr
        public static int RXDisplayNotchLowCut
        {
            set { rx_display_notch_low_cut = value; }
        }

        private static int rx_display_notch_high_cut = 4000;        // yt7pwr
        public static int RXDisplayNotchHighCut
        {
            set { rx_display_notch_high_cut = value; }
        }

        private static int rx_display_low = -4000;
        public static int RXDisplayLow
        {
            get { return rx_display_low; }
            set { rx_display_low = value; }
        }

        private static int rx_display_high = 4000;
        public static int RXDisplayHigh
        {
            get { return rx_display_high; }
            set { rx_display_high = value; }
        }

        private static int tx_display_low = -4000;
        public static int TXDisplayLow
        {
            get { return tx_display_low; }
            set { tx_display_low = value; }
        }

        private static int tx_display_high = 4000;
        public static int TXDisplayHigh
        {
            get { return tx_display_high; }
            set { tx_display_high = value; }
        }

        private static float display_cal_offset;					// display calibration offset per volume setting in dB
        public static float DisplayCalOffset
        {
            get { return display_cal_offset; }
            set { display_cal_offset = value; }
        }

        private static Model current_model = Model.LimeSDR;
        public static Model CurrentModel
        {
            get { return current_model; }
            set { current_model = value; }
        }

        private static int display_cursor_x;						// x-coord of the cursor when over the display
        public static int DisplayCursorX
        {
            get { return display_cursor_x; }
            set { display_cursor_x = value; }
        }

        private static int display_cursor_y;						// y-coord of the cursor when over the display
        public static int DisplayCursorY
        {
            get { return display_cursor_y; }
            set { display_cursor_y = value; }
        }

        private static ClickTuneMode current_click_tune_mode = ClickTuneMode.Off;
        public static ClickTuneMode CurrentClickTuneMode
        {
            get { return current_click_tune_mode; }
            set { current_click_tune_mode = value; }
        }

        private static int sample_rate = 48000;
        public static int SampleRate
        {
            get { return sample_rate; }
            set { sample_rate = value; }
        }

        private static bool high_swr = false;
        public static bool HighSWR
        {
            get { return high_swr; }
            set { high_swr = value; }
        }

        private static DisplayEngine current_display_engine = DisplayEngine.GDI_PLUS;
        public static DisplayEngine CurrentDisplayEngine
        {
            get { return current_display_engine; }
            set { current_display_engine = value; }
        }

        private static bool mox = false;
        public static bool MOX
        {
            get { return mox; }
            set { mox = value; }
        }

        private static DSPMode current_dsp_mode_subRX = DSPMode.USB;  // yt7pwr
        public static DSPMode CurrentDSPModeSubRX
        {
            get { return current_dsp_mode_subRX; }
            set { current_dsp_mode_subRX = value; }
        }

        private static DSPMode current_dsp_mode = DSPMode.USB;
        public static DSPMode CurrentDSPMode
        {
            get { return current_dsp_mode; }
            set { current_dsp_mode = value; }
        }

        private static DisplayMode current_display_mode = DisplayMode.PANADAPTER;
        public static DisplayMode CurrentDisplayMode // changes yt7pwr
        {
            get { return current_display_mode; }
            set
            {
                current_display_mode = value;

                switch (current_display_mode)
                {
                    case DisplayMode.PANAFALL:
                    case DisplayMode.PANAFALL_INV:
                    case DisplayMode.WATERFALL:
                    case DisplayMode.PANADAPTER:
                        DttSP.NotPan = false;
                        break;
                    default:
                        DttSP.NotPan = true;
                        break;
                }

                switch (current_display_mode)
                {
                    case DisplayMode.PHASE:
                    case DisplayMode.PHASE2:
                        Audio.phase = true;
                        break;
                    default:
                        Audio.phase = false;
                        break;
                }

                if (average_on) ResetDisplayAverage();
                if (peak_on) ResetDisplayPeak();
            }
        }

        private static float max_x;								// x-coord of maxmimum over one display pass
        public static float MaxX
        {
            get { return max_x; }
            set { max_x = value; }
        }

        private static float scope_max_x;								// x-coord of maxmimum over one display pass
        public static float ScopeMaxX
        {
            get { return scope_max_x; }
            set { scope_max_x = value; }
        }

        private static float max_y;								// y-coord of maxmimum over one display pass
        public static float MaxY
        {
            get { return max_y; }
            set { max_y = value; }
        }

        private static float scope_max_y;								// y-coord of maxmimum over one display pass
        public static float ScopeMaxY
        {
            get { return scope_max_y; }
            set { scope_max_y = value; }
        }

        private static bool average_on;							// True if the Average button is pressed
        public static bool AverageOn
        {
            get { return average_on; }
            set
            {
                average_on = value;
                if (!average_on) ResetDisplayAverage();
            }
        }

        private static bool peak_on;							// True if the Peak button is pressed
        public static bool PeakOn
        {
            get { return peak_on; }
            set
            {
                peak_on = value;
                if (!peak_on) ResetDisplayPeak();
            }
        }

        public static bool scope_data_ready = false;
        private static bool data_ready = false;			// True when there is new display data ready from the DSP
        public static bool DataReady
        {
            get { return data_ready; }
            set { data_ready = value; }
        }

        private static bool waterfall_data_ready;	    // True when there is new display data ready from the DSP
        public static bool WaterfallDataReady
        {
            get { return waterfall_data_ready; }
            set { waterfall_data_ready = value; }
        }

        public static float display_avg_mult_old = 1 - (float)1 / 2;
        public static float display_avg_mult_new = (float)1 / 2;
        private static int display_avg_num_blocks = 2;
        public static int DisplayAvgBlocks
        {
            get { return display_avg_num_blocks; }
            set
            {
                display_avg_num_blocks = value;
                display_avg_mult_old = 1 - (float)1 / display_avg_num_blocks;
                display_avg_mult_new = (float)1 / display_avg_num_blocks;
            }
        }

        public static float waterfall_avg_mult_old = 1 - (float)1 / 18;
        public static float waterfall_avg_mult_new = (float)1 / 18;
        private static int waterfall_avg_num_blocks = 18;
        public static int WaterfallAvgBlocks
        {
            get { return waterfall_avg_num_blocks; }
            set
            {
                waterfall_avg_num_blocks = value;
                waterfall_avg_mult_old = 1 - (float)1 / waterfall_avg_num_blocks;
                waterfall_avg_mult_new = (float)1 / waterfall_avg_num_blocks;
            }
        }

        private static int spectrum_grid_max = 0;
        public static int SpectrumGridMax
        {
            get { return spectrum_grid_max; }
            set
            {
                spectrum_grid_max = value;
                refresh_panadapter_grid = true;
            }
        }

        private static int spectrum_grid_min = -150;
        public static int SpectrumGridMin
        {
            get { return spectrum_grid_min; }
            set
            {
                spectrum_grid_min = value;
                refresh_panadapter_grid = true;
            }
        }

        private static int spectrum_grid_step = 10;
        public static int SpectrumGridStep
        {
            get { return spectrum_grid_step; }
            set
            {
                spectrum_grid_step = value;
                refresh_panadapter_grid = true;
            }
        }

        private static Color grid_text_color = Color.Yellow;
        public static Color GridTextColor
        {
            get { return grid_text_color; }
            set
            {
                grid_text_color = value;
                refresh_panadapter_grid = true;
            }
        }

        private static Color grid_zero_color = Color.Red;
        public static Color GridZeroColor
        {
            get { return grid_zero_color; }
            set
            {
                grid_zero_color = value;
            }
        }

        private static Color grid_color = Color.Purple;
        public static Color GridColor
        {
            get { return grid_color; }
            set
            {
                grid_color = value;
                refresh_panadapter_grid = true;
            }
        }

        private static Pen data_line_pen = new Pen(new SolidBrush(Color.LightGreen), display_line_width);
        private static Color data_line_color = Color.LightGreen;
        public static Color DataLineColor
        {
            get { return data_line_color; }
            set
            {
                data_line_color = value;
                data_line_pen = new Pen(new SolidBrush(data_line_color), display_line_width);
                linecolor = new Color4(data_line_color.ToArgb());
            }
        }

        private static Color display_filter_tx_color = Color.Yellow;
        public static Color DisplayFilterTXColor
        {
            get { return display_filter_tx_color; }
            set
            {
                display_filter_tx_color = value;
            }
        }

        private static bool draw_tx_cw_freq = false;
        public static bool DrawTXCWFreq
        {
            get { return draw_tx_cw_freq; }
            set
            {
                draw_tx_cw_freq = value;
            }
        }

        private static Color display_background_color = Color.Black;
        public static Color DisplayBackgroundColor
        {
            get { return display_background_color; }
            set
            {
                display_background_color = value;
            }
        }

        private static Color waterfall_low_color = Color.Black;
        public static Color WaterfallLowColor
        {
            get { return waterfall_low_color; }
            set { waterfall_low_color = value; }
        }

        private static Color waterfall_mid_color = Color.Red;
        public static Color WaterfallMidColor
        {
            get { return waterfall_mid_color; }
            set { waterfall_mid_color = value; }
        }

        private static Color waterfall_high_color = Color.Yellow;
        public static Color WaterfallHighColor
        {
            get { return waterfall_high_color; }
            set { waterfall_high_color = value; }
        }

        private static float waterfall_high_threshold = -80.0F;
        public static float WaterfallHighThreshold
        {
            get { return waterfall_high_threshold; }
            set { waterfall_high_threshold = value; }
        }

        private static float waterfall_low_threshold = -110.0F;
        public static float WaterfallLowThreshold
        {
            get { return waterfall_low_threshold; }
            set { waterfall_low_threshold = value; }
        }

        private static float display_line_width = 1.0F;
        public static float DisplayLineWidth
        {
            get { return display_line_width; }
            set
            {
                display_line_width = value;
                data_line_pen = new Pen(new SolidBrush(data_line_color), display_line_width);

                if (panadapter_fill_line != null && panadapter_line != null)
                {
                    panadapter_line.Width = value;
                    panadapter_fill_line.Width = value;
                }
            }
        }

        private static DisplayLabelAlignment display_label_align = DisplayLabelAlignment.LEFT;
        public static DisplayLabelAlignment DisplayLabelAlign
        {
            get { return display_label_align; }
            set
            {
                display_label_align = value;
            }
        }

        #endregion

        #region Misc routine

        private static void UpdateDisplayPeak()
        {
            try
            {
                if (peak_buffer[0] == CLEAR_FLAG)
                {
                    for (int i = 0; i < BUFFER_SIZE; i++)
                        peak_buffer[i] = current_display_data[i];
                }
                else
                {
                    for (int i = 0; i < BUFFER_SIZE; i++)
                    {
                        if (current_display_data[i] > peak_buffer[i])
                            peak_buffer[i] = current_display_data[i];
                        current_display_data[i] = peak_buffer[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void ResetDisplayAverage()
        {
            try
            {
                if (average_buffer != null)
                {
                    average_buffer[0] = CLEAR_FLAG;	            // set reset flag
                    average_waterfall_buffer[0] = CLEAR_FLAG;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void ResetDisplayPeak()
        {
            try
            {
                if (peak_buffer != null)
                    peak_buffer[0] = CLEAR_FLAG;                // set reset flag
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static bool DirectXInit()
        {
            if (!booting && !DX_reinit)
            {
                try
                {
                    try
                    {
                        BUFFER_SIZE = 32768;
                        DX_reinit = true;

                        switch (current_display_mode)
                        {
                            case DisplayMode.PANADAPTER:
                            case DisplayMode.PANAFALL:
                            case DisplayMode.PANAFALL_INV:
                            case DisplayMode.PANASCOPE:
                                panadapter_target = (Control)console.picDisplay;
                                panadapter_W = panadapter_target.Width;
                                panadapter_H = panadapter_target.Height;
                                WaterfallTarget = (Control)console.picWaterfall;
                                waterfallX_data = new float[waterfall_W];
                                panadapterX_scope_data = new float[waterfall_W * 2];
                                break;
                            case DisplayMode.WATERFALL:
                                WaterfallTarget = (Control)console.picWaterfall;
                                panadapterX_scope_data = new float[waterfall_W * 2];
                                break;
                            default:
                                panadapter_H = panadapter_target.Height;
                                panadapter_W = panadapter_target.Width;
                                panadapterX_scope_data = new float[panadapter_W * 2];
                                panadapter_target = (Control)console.picDisplay;
                                break;
                        }

                        panadapterX_data = new float[panadapter_W];
                        refresh_panadapter_grid = true;
                        histogram_data = new int[panadapter_W];
                        histogram_verts = new HistogramData[panadapter_W * 4];
                        scope_min = new float[waterfall_target.Width];
                        scope_max = new float[waterfall_target.Width];

                        histogram_history = new int[panadapter_W];
                        for (int i = 0; i < panadapter_W; i++)
                        {
                            histogram_data[i] = Int32.MaxValue;
                            histogram_history[i] = 0;
                            histogram_verts[i].X = i;
                            histogram_verts[i].Y = panadapter_H;
                            histogram_verts[i].color = Color.Green;
                        }

                        average_buffer = new float[BUFFER_SIZE];	            // initialize averaging buffer array
                        average_buffer[0] = CLEAR_FLAG;		                    // set the clear flag

                        average_waterfall_buffer = new float[BUFFER_SIZE];	    // initialize averaging buffer array
                        average_waterfall_buffer[0] = CLEAR_FLAG;		        // set the clear flag

                        peak_buffer = new float[BUFFER_SIZE];
                        peak_buffer[0] = CLEAR_FLAG;

                        new_display_data = new float[BUFFER_SIZE];
                        new_scope_data = new float[BUFFER_SIZE];
                        new_waterfall_data = new float[BUFFER_SIZE];
                        current_display_data = new float[BUFFER_SIZE];
                        current_scope_data = new float[BUFFER_SIZE];
                        current_waterfall_data = new float[BUFFER_SIZE];
                        waterfall_display_data = new float[BUFFER_SIZE];

                        for (int i = 0; i < BUFFER_SIZE; i++)
                        {
                            new_display_data[i] = -200.0f;
                            new_scope_data[i] = -200.0f;
                            new_waterfall_data[i] = -200.0f;
                            current_display_data[i] = -200.0f;
                            current_scope_data[i] = -200.0f;
                            current_waterfall_data[i] = -200.0f;
                            waterfall_display_data[i] = -200.0f;
                        }

                        presentParms = new PresentParameters();
                        presentParms.Windowed = true;
                        presentParms.SwapEffect = SwapEffect.Discard;
                        presentParms.Multisample = MultisampleType.None;
                        presentParms.EnableAutoDepthStencil = true;
                        presentParms.AutoDepthStencilFormat = Format.D24X8;
                        presentParms.PresentFlags = PresentFlags.DiscardDepthStencil;
                        presentParms.PresentationInterval = PresentInterval.Default;
                        presentParms.BackBufferFormat = Format.X8R8G8B8;
                        presentParms.BackBufferHeight = panadapter_target.Height;
                        presentParms.BackBufferWidth = panadapter_target.Width;
                        presentParms.Windowed = true;
                        presentParms.BackBufferCount = 1;

                        switch (directx_render_type)
                        {
                            case RenderType.HARDWARE:
                                try
                                {
                                    device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        panadapter_target.Handle, CreateFlags.HardwareVertexProcessing |
                                    CreateFlags.FpuPreserve | CreateFlags.Multithreaded,
                                        presentParms);

                                    waterfall_dx_device = new Device(new Direct3D(), 0,
                                        DeviceType.Hardware, waterfall_target.Handle,
                                        CreateFlags.HardwareVertexProcessing |
                                    CreateFlags.FpuPreserve | CreateFlags.Multithreaded, presentParms);
                                }
                                catch (Direct3D9Exception ex)
                                {
                                    if (debug && !console.ConsoleClosing)
                                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                            "DirectX hardware init error!" + ex.ToString());
                                }
                                break;

                            case RenderType.SOFTWARE:
                                try
                                {
                                    device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        panadapter_target.Handle, CreateFlags.SoftwareVertexProcessing |
                                    CreateFlags.FpuPreserve | CreateFlags.Multithreaded, presentParms);

                                    waterfall_dx_device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        waterfall_target.Handle, CreateFlags.SoftwareVertexProcessing |
                                    CreateFlags.FpuPreserve | CreateFlags.Multithreaded, presentParms);
                                }
                                catch (Direct3D9Exception exe)
                                {
                                    if (debug && !console.ConsoleClosing)
                                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                            "DirectX software init error!" + exe.ToString());

                                    return false;
                                }
                                break;
                        }

                        var vertexElems = new[] {
                        new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                        new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                        VertexElement.VertexDeclarationEnd
                        };

                        var vertexDecl = new VertexDeclaration(device, vertexElems);
                        device.VertexDeclaration = vertexDecl;
                        var vertexDecl1 = new VertexDeclaration(waterfall_dx_device, vertexElems);
                        waterfall_dx_device.VertexDeclaration = vertexDecl1;

                        waterfall_bmp = new System.Drawing.Bitmap(waterfall_target.Width, waterfall_target.Height,
                            System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                        BitmapData bitmapData = waterfall_bmp.LockBits(
                            new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height),
                            ImageLockMode.ReadWrite, waterfall_bmp.PixelFormat);

                        waterfall_bmp_size = bitmapData.Stride * waterfall_bmp.Height;
                        waterfall_bmp_stride = bitmapData.Stride;
                        waterfall_memory = new byte[waterfall_bmp_size];
                        waterfall_bmp.UnlockBits(bitmapData);
                        waterfall_rect = new Rectangle(0, 0, waterfall_target.Width, waterfall_target.Height);
                        //backbuf = waterfall_dx_device.GetBackBuffer(0, 0);

                        panadapter_font = new SlimDX.Direct3D9.Font(device, pan_font);

                        if (background_image != null && File.Exists(background_image))
                        {
                            PanadapterTexture = Texture.FromFile(device, background_image, panadapter_target.Width, panadapter_target.Height,
                                1, Usage.None, Format.Unknown, Pool.Managed, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);
                            Panadapter_texture_size.Width = panadapter_target.Width;
                            Panadapter_texture_size.Height = panadapter_target.Height;
                            Panadapter_Sprite = new Sprite(device);
                            WaterfallTexture = Texture.FromFile(waterfall_dx_device, background_image, waterfall_target.Width, waterfall_target.Height,
                                1, Usage.None, Format.Unknown, Pool.Managed, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);
                            Waterfall_texture_size.Width = waterfall_target.Width;
                            Waterfall_texture_size.Height = waterfall_target.Height;
                            WaterfallBackgroundTexture = Texture.FromFile(waterfall_dx_device, background_image, waterfall_target.Width, waterfall_target.Height,
                                1, Usage.None, Format.Unknown, Pool.Managed, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);
                            Waterfall_Sprite = new Sprite(waterfall_dx_device);
                        }
                        else
                        {
                            Panadapter_Sprite = null;
                            WaterfallTexture = new Texture(waterfall_dx_device, waterfall_target.Width, waterfall_target.Height, 0,
                                Usage.None, Format.X8R8G8B8, Pool.Managed);
                            WaterfallBackgroundTexture = new Texture(waterfall_dx_device, waterfall_target.Width, waterfall_target.Height, 0,
                                Usage.None, Format.X8R8G8B8, Pool.Managed);
                            Waterfall_texture_size.Width = waterfall_target.Width;
                            Waterfall_texture_size.Height = waterfall_target.Height;
                            Waterfall_Sprite = new Sprite(waterfall_dx_device);
                        }

                        if (directx_render_type == RenderType.HARDWARE)
                        {
                            Waterfall_vb = new VertexBuffer(waterfall_dx_device, panadapterX_data.Length * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                            Waterfall_verts = new Vertex[waterfall_W * 2];
                            Panadapter_vb = new VertexBuffer(device, panadapterX_data.Length * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                            PanLine_vb_fill = new VertexBuffer(device, panadapter_W * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                            Panadapter_verts = new Vertex[panadapter_W * 2];
                            Panadapter_verts_fill = new Vertex[panadapter_W * 2];
                            HistogramLine_verts = new Vertex[panadapter_W * 6];
                            Histogram_vb = new VertexBuffer(device, panadapter_W * 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                        }
                        else if (directx_render_type == RenderType.SOFTWARE)
                        {
                            Waterfall_vb = new VertexBuffer(waterfall_dx_device, panadapterX_data.Length * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                            Waterfall_verts = new Vertex[waterfall_W];
                            Panadapter_vb = new VertexBuffer(device, panadapterX_data.Length * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                            PanLine_vb_fill = new VertexBuffer(device, panadapter_W * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                            Panadapter_verts = new Vertex[panadapter_W];
                            Panadapter_verts_fill = new Vertex[panadapter_W * 2];
                            HistogramLine_verts = new Vertex[panadapter_W * 6];
                            Histogram_vb = new VertexBuffer(device, panadapter_W * 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                        }

                        panadapter_verts = new Vector2[panadapter_W];
                        panadapter_line = new Line(device);
                        panadapter_line.Antialias = smooth_line;
                        panadapter_line.Width = display_line_width;
                        panadapter_line.GLLines = true;
                        panadapter_fill_verts = new Vector2[panadapter_W * 2];
                        panadapter_fill_line = new Line(device);
                        panadapter_fill_line.Antialias = smooth_line;
                        panadapter_fill_line.Width = 1;
                        panadapter_fill_line.GLLines = true;

                        high_swr_font = new SlimDX.Direct3D9.Font(device,
                            new System.Drawing.Font("Arial", 14.0f, FontStyle.Bold));

                        DX_reinit = false;

                        return true;
                    }
                    catch (Direct3D9Exception ex)
                    {
                        //MessageBox.Show("DirectX init general fault!\n" + ex.ToString());
                        DX_reinit = false;

                        if (debug && !console.ConsoleClosing)
                            console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                "DirectX init general fault!\n" + ex.ToString());

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                    DX_reinit = false;
                    return false;
                }
            }

            return true;
        }

        public static void DirectXRelease()
        {
            try
            {
                if (!booting && !DX_reinit)
                {
                    DX_reinit = true;

                    //backbuf = null;
                    waterfallX_data = null;
                    panadapterX_data = null;
                    scope_min = null;
                    scope_max = null;
                    new_display_data = null;
                    new_scope_data = null;
                    new_waterfall_data = null;
                    current_display_data = null;
                    current_scope_data = null;
                    current_waterfall_data = null;
                    waterfall_display_data = null;
                    histogram_data = null;
                    histogram_history = null;
                    average_buffer = null;
                    average_waterfall_buffer = null;
                    peak_buffer = null;

                    if (waterfall_bmp != null)
                        waterfall_bmp.Dispose();

                    waterfall_bmp = null;

                    /*if (Panadapter_Sprite != null)
                    {
                        Panadapter_Sprite.OnLostDevice();
                        Panadapter_Sprite.Dispose();
                    }

                    Panadapter_Sprite = null;

                    if (Waterfall_Sprite != null)
                    {
                        Waterfall_Sprite.Dispose();
                    }

                    Waterfall_Sprite = null;

                    if (PanadapterTexture != null)
                    {
                        PanadapterTexture.Dispose();
                        PanadapterTexture = null;
                    }

                    if (WaterfallTexture != null)
                    {
                        WaterfallTexture.Dispose();
                        WaterfallTexture = null;
                    }*/

                    if (VerLine_vb != null)
                    {
                        VerLine_vb.Dispose();
                        VerLine_vb = null;
                    }

                    if (VerLines_vb != null)
                    {
                        VerLines_vb.Dispose();
                        VerLines_vb = null;
                    }

                    if (HorLine_vb != null)
                    {
                        HorLine_vb.Dispose();
                        HorLine_vb = null;
                    }

                    if (HorLines_vb != null)
                    {
                        HorLines_vb.Dispose();
                        HorLines_vb = null;
                    }

                    if (Panadapter_vb != null)
                    {
                        Panadapter_vb.Dispose();
                        Panadapter_vb.Dispose();
                    }

                    /*if (ScopeLine_vb != null)
                    {
                        ScopeLine_vb.Dispose();
                        ScopeLine_vb.Dispose();
                    }*/

                    if (PanLine_vb_fill != null)
                    {
                        PanLine_vb_fill.Dispose();
                        PanLine_vb_fill.Dispose();
                    }

                    if (vertical_label != null)
                        vertical_label = null;

                    if (horizontal_label != null)
                        horizontal_label = null;

                    /*if (Phase_vb != null)
                    {
                        Phase_vb.Dispose();
                        Phase_vb.Dispose();
                    }*/

                    if (device != null)
                    {
                        device.Dispose();
                        device = null;
                    }

                    if (waterfall_dx_device != null)
                    {
                        waterfall_dx_device.Dispose();
                        waterfall_dx_device = null;
                    }

                    if(high_swr_font != null)
                        high_swr_font.Dispose();

                    panadapter_fill_verts = null;
                    panadapter_line = null;
                    panadapter_fill_verts = null;
                    panadapter_verts = null;

                    DX_reinit = false;
                }
            }
            catch (Exception ex)
            {
                Debug.Write("DX release error!" + ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "DirectX release fault!\n" + ex.ToString());

                DX_reinit = false;
            }
        }

        public static void Render_VFOA()  // yt7pwr
        {
            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int filter_low, filter_high;
            int[] step_list = { 10, 20, 25, 50 };
            //int step_power = 1;
            //int step_index = 0;
            int freq_step_size = 50;
            int filter_left = 0;
            int filter_right = 0;
            int notch_low = 0;
            int notch_high = 0;

            if (mox && !(current_dsp_mode == DSPMode.CWL ||
                current_dsp_mode == DSPMode.CWU))
            {
                filter_low = DttSP.TXFilterLowCut;
                filter_high = DttSP.TXFilterHighCut;
            }
            else
            {
                filter_low = DttSP.RXFilterLowCut;
                filter_high = DttSP.RXFilterHighCut;
                notch_low = rx_display_notch_low_cut;
                notch_high = rx_display_notch_high_cut;
            }

            if (!mox)
            {
                switch (current_dsp_mode)
                {
                    case DSPMode.CWU:
                    case DSPMode.CWL:
                        {
                            // get filter screen coordinates
                            filter_left = (int)((float)(-low - ((filter_high - filter_low) / 2) + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                            filter_right = (int)((float)(-low + ((filter_high - filter_low) / 2) + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);

                            // make the filter display at least one pixel wide.
                            if (filter_left == filter_right) filter_right = filter_left + 1;
                            VFOArect.x1 = filter_right;
                            VFOArect.y1 = (int)(pan_font.Height);
                            VFOArect.x2 = filter_right;
                            VFOArect.y2 = panadapter_H;
                            VFOArect.x3 = filter_left;
                            VFOArect.y3 = (int)(pan_font.Height);
                            VFOArect.x4 = filter_left;
                            VFOArect.y4 = panadapter_H;
                            RenderRectangle(device, VFOArect, main_rx_filter_color);

                        }
                        break;
                    case DSPMode.DRM:
                        // get filter screen coordinates
                        filter_left = (int)((float)(filter_low - filter_high/2 - low + rit_hz + vfoa_hz - losc_hz) / (high - low) * panadapter_W);
                        filter_right = (int)((float)(filter_high / 2 - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);

                        // draw Main RX 0Hz line
                        int rx_zero_line = (int)((float)(vfoa_hz + rit_hz - losc_hz - low) / (high - low) * panadapter_W);
                        RenderVerticalLine(device, rx_zero_line, panadapter_H, main_rx_zero_line_color);
                        VFOArect.x1 = filter_right;
                        VFOArect.y1 = (int)(pan_font.Size);
                        VFOArect.x2 = filter_right;
                        VFOArect.y2 = panadapter_H;
                        VFOArect.x3 = filter_left;
                        VFOArect.y3 = (int)(pan_font.Size);
                        VFOArect.x4 = filter_left;
                        VFOArect.y4 = panadapter_H;
                        RenderRectangle(device, VFOArect, main_rx_filter_color);
                        break;
                    default:
                        {
                            // get filter screen coordinates
                            filter_left = (int)((float)(filter_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                            filter_right = (int)((float)(filter_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);

                            // make the filter display at least one pixel wide.
                            if (filter_left == filter_right) filter_right = filter_left + 1;

                            // draw Main RX 0Hz line
                            int main_rx_zero_line = (int)((float)(vfoa_hz + rit_hz - losc_hz - low) / (high - low) * panadapter_W);
                            RenderVerticalLine(device, main_rx_zero_line, panadapter_H, main_rx_zero_line_color);
                            VFOArect.x1 = filter_right;
                            VFOArect.y1 = (int)(pan_font.Height);
                            VFOArect.x2 = filter_right;
                            VFOArect.y2 = panadapter_H;
                            VFOArect.x3 = filter_left;
                            VFOArect.y3 = (int)(pan_font.Height);
                            VFOArect.x4 = filter_left;
                            VFOArect.y4 = panadapter_H;
                            RenderRectangle(device, VFOArect, main_rx_filter_color);
                        }
                        break;
                }

                if (DisplayNotchFilter)
                {
                    switch (current_dsp_mode)
                    {
                        case DSPMode.CWL:
                        case DSPMode.CWU:
                            {
                                // get filter screen coordinates
                                if ((filter_high < 0 && filter_low < 0) && (notch_high - notch_low < filter_high - filter_low))
                                {
                                    filter_left = (int)((float)(-notch_high - low + vfoa_hz - (filter_low + filter_high) / 2 + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                    filter_right = (int)((float)(-notch_low - low + vfoa_hz - (filter_low + filter_high) / 2 + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                }
                                else if ((notch_high - notch_low) < (filter_high - filter_low))
                                {
                                    filter_left = (int)((float)(notch_low - low + vfoa_hz - filter_high / 2 + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                    filter_right = (int)((float)(notch_high - low + vfoa_hz - filter_high / 2 + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                }
                            }
                            break;
                        default:
                            {
                                // get filter screen coordinates
                                if (filter_high < 0 && filter_low < 0)
                                {
                                    filter_left = (int)((float)(-notch_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                    filter_right = (int)((float)(-notch_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                }
                                else
                                {
                                    filter_left = (int)((float)(notch_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                    filter_right = (int)((float)(notch_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                }
                            }
                            break;
                    }

                    if (filter_right > filter_left)
                    {
                        VFOArect.x1 = filter_right;
                        VFOArect.y1 = (int)(pan_font.Height);
                        VFOArect.x2 = filter_right;
                        VFOArect.y2 = panadapter_H;
                        VFOArect.x3 = filter_left;
                        VFOArect.y3 = (int)(pan_font.Height);
                        VFOArect.x4 = filter_left;
                        VFOArect.y4 = panadapter_H;
                        RenderRectangle(device, VFOArect, Color.FromArgb(main_rx_filter_color.A / 2, main_rx_filter_color.R / 2,
                            main_rx_filter_color.G / 2, main_rx_filter_color.B / 2));
                    }
                }
            }
            else
            {
                DSPMode mode;
                if(split_enabled)
                    mode = current_dsp_mode_subRX;
                else
                    mode = current_dsp_mode;

                switch (mode)
                {
                    case DSPMode.CWU:
                    case DSPMode.CWL:
                        {
                            // get filter screen coordinates
                            filter_left = (int)((float)(-low - ((filter_high - filter_low) / 2) + vfoa_hz + xit_hz - losc_hz) / (high - low) * panadapter_W);
                            filter_right = (int)((float)(-low + ((filter_high - filter_low) / 2) + vfoa_hz + xit_hz - losc_hz) / (high - low) * panadapter_W);

                            // make the filter display at least one pixel wide.
                            if (filter_left == filter_right) filter_right = filter_left + 1;
                            VFOArect.x1 = filter_right;
                            VFOArect.y1 = (int)(pan_font.Height);
                            VFOArect.x2 = filter_right;
                            VFOArect.y2 = panadapter_H;
                            VFOArect.x3 = filter_left;
                            VFOArect.y3 = (int)(pan_font.Height);
                            VFOArect.x4 = filter_left;
                            VFOArect.y4 = panadapter_H;
                            RenderRectangle(device, VFOArect, main_rx_filter_color);
                        }
                        break;
                    default:
                        {
                            if (!split_enabled)
                            {
                                // get filter screen coordinates
                                filter_left = (int)((float)(filter_low - low) / (high - low) * panadapter_W);
                                filter_right = (int)((float)(filter_high - low) / (high - low) * panadapter_W);

                                // make the filter display at least one pixel wide.
                                if (filter_left == filter_right) filter_right = filter_left + 1;

                                // draw Main TX 0Hz line
                                int x = (int)((float)(-low) / (high - low) * panadapter_W);
                                RenderVerticalLine(device, x, panadapter_H, main_rx_zero_line_color);
                                VFOArect.x1 = filter_right;
                                VFOArect.y1 = (int)(pan_font.Height);
                                VFOArect.x2 = filter_right;
                                VFOArect.y2 = panadapter_H;
                                VFOArect.x3 = filter_left;
                                VFOArect.y3 = (int)(pan_font.Height);
                                VFOArect.x4 = filter_left;
                                VFOArect.y4 = panadapter_H;
                                RenderRectangle(device, VFOArect, main_rx_filter_color);
                            }
                        }
                        break;
                }
            }
        }

        public static void Render_VFOB()  // yt7pwr
        {
            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int filter_low, filter_high;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 50;
            int filter_right = 0;
            int filter_left = 0;
            int filter_low_subRX = 0;
            int filter_high_subRX = 0;

            if (mox && !(current_dsp_mode_subRX == DSPMode.CWL ||
                 current_dsp_mode_subRX == DSPMode.CWU)) // get filter limits
            {
                filter_low = DttSP.TXFilterLowCut;
                filter_high = DttSP.TXFilterHighCut;
            }
            else
            {
                filter_low = DttSP.RXFilterLowCut;
                filter_high = DttSP.RXFilterHighCut;
                filter_low_subRX = DttSP.RXFilterLowCutSubRX;
                filter_high_subRX = DttSP.RXFilterHighCutSubRX;
            }

            // Calculate horizontal step size
            int width = high - low;
            while (width / freq_step_size > 10)
            {
                freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                step_index = (step_index + 1) % 4;
                if (step_index == 0) step_power++;
            }

            int w_steps = width / freq_step_size;

            // calculate vertical step size
            int h_steps = (spectrum_grid_max - spectrum_grid_min) / spectrum_grid_step;
            double h_pixel_step = (double)panadapter_H / h_steps;

            if (sub_rx_enabled && !mox)
            {
                if (current_dsp_mode_subRX == DSPMode.CWL || current_dsp_mode_subRX == DSPMode.CWU)
                {
                    // draw Sub RX filter
                    // get filter screen coordinates
                    filter_left = (int)((float)(-low - ((filter_high_subRX - filter_low_subRX) / 2)
                        + vfob_hz - losc_hz) / (high - low) * panadapter_W);
                    filter_right = (int)((float)(-low + ((filter_high_subRX - filter_low_subRX) / 2)
                        + vfob_hz - losc_hz) / (high - low) * panadapter_W);

                    // make the filter display at least one pixel wide.
                    if (filter_left == filter_right) filter_right = filter_left + 1;
                    VFOBrect.x1 = filter_right;
                    VFOBrect.y1 = (int)(pan_font.Height);
                    VFOBrect.x2 = filter_right;
                    VFOBrect.y2 = panadapter_H;
                    VFOBrect.x3 = filter_left;
                    VFOBrect.y3 = (int)(pan_font.Height);
                    VFOBrect.x4 = filter_left;
                    VFOBrect.y4 = panadapter_H;
                    RenderRectangle(device, VFOBrect, sub_rx_filter_color);
                }
                else
                {
                    // draw Sub RX filter
                    // get filter screen coordinates
                    filter_left = (int)((float)(filter_low_subRX - low + vfob_hz - losc_hz) / (high - low) * panadapter_W);
                    filter_right = (int)((float)(filter_high_subRX - low + vfob_hz - losc_hz) / (high - low) * panadapter_W);

                    // make the filter display at least one pixel wide.
                    if (filter_left == filter_right) filter_right = filter_left + 1;

                    // draw Sub RX 0Hz line
                    int sub_rx_zero_line = (int)((float)(vfob_hz - losc_hz - low) / (high - low) * panadapter_W);
                    RenderVerticalLine(device, sub_rx_zero_line, panadapter_H, sub_rx_zero_line_color);
                    VFOBrect.x1 = filter_right;
                    VFOBrect.y1 = (int)(pan_font.Height);
                    VFOBrect.x2 = filter_right;
                    VFOBrect.y2 = panadapter_H;
                    VFOBrect.x3 = filter_left;
                    VFOBrect.y3 = (int)(pan_font.Height);
                    VFOBrect.x4 = filter_left;
                    VFOBrect.y4 = panadapter_H;
                    RenderRectangle(device, VFOBrect, sub_rx_filter_color);
                }
            }

            if (split_enabled && mox)
            {
                if (current_dsp_mode_subRX == DSPMode.CWL || current_dsp_mode_subRX == DSPMode.CWU)
                {
                    // draw Sub RX filter
                    // get filter screen coordinates
                    filter_left = (int)((float)(-low - ((filter_high_subRX - filter_low_subRX) / 2)
                        + vfob_hz - losc_hz) / (high - low) * panadapter_W);
                    filter_right = (int)((float)(-low + ((filter_high_subRX - filter_low_subRX) / 2)
                        + vfob_hz - losc_hz) / (high - low) * panadapter_W);

                    // make the filter display at least one pixel wide.
                    if (filter_left == filter_right) filter_right = filter_left + 1;
                    VFOBrect.x1 = filter_right;
                    VFOBrect.y1 = (int)(pan_font.Height);
                    VFOBrect.x2 = filter_right;
                    VFOBrect.y2 = panadapter_H;
                    VFOBrect.x3 = filter_left;
                    VFOBrect.y3 = (int)(pan_font.Height);
                    VFOBrect.x4 = filter_left;
                    VFOBrect.y4 = panadapter_H;
                    RenderRectangle(device, VFOBrect, sub_rx_filter_color);
                }
                else
                {
                    // get filter screen coordinates
                    filter_left = (int)((float)(filter_low - low) / (high - low) * panadapter_W);
                    filter_right = (int)((float)(filter_high - low) / (high - low) * panadapter_W);

                    int x = (int)((float)(-low) / (high - low) * panadapter_W);
                    RenderVerticalLine(device, x, panadapter_H, sub_rx_zero_line_color);
                    VFOBrect.x1 = filter_right;
                    VFOBrect.y1 = (int)(pan_font.Size);
                    VFOBrect.x2 = filter_right;
                    VFOBrect.y2 = panadapter_H;
                    VFOBrect.x3 = filter_left;
                    VFOBrect.y3 = (int)(pan_font.Size);
                    VFOBrect.x4 = filter_left;
                    VFOBrect.y4 = panadapter_H;
                    RenderRectangle(device, VFOBrect, sub_rx_filter_color);
                }
            }
        }

        private static void RenderRectangle(Device dev, DXRectangle rect, Color color)
        {
            try
            {
                try
                {
                    Vertex[] verts = new Vertex[4];

                    var vb = new VertexBuffer(dev, 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

                    verts[0] = new Vertex();
                    verts[0].Color = color.ToArgb();
                    verts[0].Position = new Vector4(rect.x1, rect.y1, 0.0f, 0.0f);
                    verts[1] = new Vertex();
                    verts[1].Color = color.ToArgb();
                    verts[1].Position = new Vector4(rect.x2, rect.y2, 0.0f, 0.0f);
                    verts[2] = new Vertex();
                    verts[2].Color = color.ToArgb();
                    verts[2].Position = new Vector4(rect.x3, rect.y3, 0.0f, 0.0f);
                    verts[3] = new Vertex();
                    verts[3].Color = color.ToArgb();
                    verts[3].Position = new Vector4(rect.x4, rect.y4, 0.0f, 0.0f);

                    vb.Lock(0, 0, LockFlags.None).WriteRange(verts, 0, 4);
                    vb.Unlock();
                    device.SetStreamSource(0, vb, 0, 20);
                    device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

                    vb.Dispose();
                }
                catch (Direct3D9Exception ex)
                {
                    Debug.Write(ex.ToString());

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                            "Render Rectangle fault!\n" + ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static void RenderVerticalLines(Device dev, VertexBuffer vertex, int count)         // yt7pwr
        {
            dev.SetStreamSource(0, vertex, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, count);
        }

        private static void RenderHorizontalLines(Device dev, VertexBuffer vertex, int count)        // yt7pwr
        {
            dev.SetStreamSource(0, vertex, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, count);
        }

        private static void RenderVerticalLine(Device dev, int x, int y, Color color)                // yt7pwr
        {
            var vb = new VertexBuffer(dev, 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            vb.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)x, (float)(pan_font.Height), 0.0f, 0.0f) },
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)x, (float)y, 0.0f, 0.0f) }
                 });
            vb.Unlock();

            dev.SetStreamSource(0, vb, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, 1);

            vb.Dispose();
        }

        private static void RenderHorizontalLine(Device dev, int x, int y, Color color)              // yt7pwr
        {
            var vb = new VertexBuffer(dev, 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            vb.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)x, (float)y, 0.0f, 0.0f) },
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)panadapter_W, (float)y, 0.0f, 0.0f) }
                 });
            vb.Unlock();

            dev.SetStreamSource(0, vb, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, 1);

            vb.Dispose();
        }

        private static void RenderHistogram(Device dev)     // yt7pwr
        {
            int i = 0;
            int j = 0;
            int k = 0;

            try
            {
                for (i = 0; i < panadapter_W * 2; i++)
                {
                    HistogramLine_verts[i].Color = histogram_verts[j].color.ToArgb();
                    HistogramLine_verts[i].Position.X = j;
                    HistogramLine_verts[i].Position.Y = points[j].Y;
                    HistogramLine_verts[i + 1].Color = histogram_verts[j].color.ToArgb();
                    HistogramLine_verts[i + 1].Position.X = j;
                    HistogramLine_verts[i + 1].Position.Y = histogram_data[j];
                    i++;
                    j++;
                }

                Histogram_vb.Lock(0, 0, LockFlags.None).WriteRange(HistogramLine_verts, 0, panadapter_W * 2);
                Histogram_vb.Unlock();

                dev.SetStreamSource(0, Histogram_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineList, 0, panadapter_W);

                j = 0;
                for (i = 0; i < panadapter_W * 4; i++)
                {
                    HistogramLine_verts[panadapter_W + i].Color = histogram_verts[panadapter_W + j].color.ToArgb();
                    HistogramLine_verts[panadapter_W + i].Position.X = k;
                    HistogramLine_verts[panadapter_W + i].Position.Y = panadapter_H;
                    HistogramLine_verts[panadapter_W + i + 1].Color = histogram_verts[panadapter_W + j].color.ToArgb();
                    HistogramLine_verts[panadapter_W + i + 1].Position.X = k;
                    HistogramLine_verts[panadapter_W + i + 1].Position.Y = histogram_verts[panadapter_W + j].Y;
                    HistogramLine_verts[panadapter_W + i + 2].Color = histogram_verts[panadapter_W + j + 1].color.ToArgb();
                    HistogramLine_verts[panadapter_W + i + 2].Position.X = k;
                    HistogramLine_verts[panadapter_W + i + 2].Position.Y = histogram_verts[panadapter_W + j].Y;
                    HistogramLine_verts[panadapter_W + i + 3].Color = histogram_verts[panadapter_W + j + 1].color.ToArgb();
                    HistogramLine_verts[panadapter_W + i + 3].Position.X = k;
                    HistogramLine_verts[panadapter_W + i + 3].Position.Y = histogram_verts[panadapter_W + j + 1].Y;
                    i += 3;
                    j +=2;
                    k++;
                }

                Histogram_vb.Lock(0, 0, LockFlags.None).WriteRange(HistogramLine_verts, panadapter_W, panadapter_W * 4);
                Histogram_vb.Unlock();

                dev.SetStreamSource(0, Histogram_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineList, 0, panadapter_W*2);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Rendering Histogram fault!\n" + ex.ToString());
            }
        }

        private static void RenderPanadapterScope(Device dev, int count)        // yt7pwr
        {
            try
            {
                int j = 0;

                for (int i = 0; i < count * 2; i++)
                {
                    Panadapter_verts[i].Color = scope_color.ToArgb();
                    Panadapter_verts[i].Position.X = i / 2;
                    Panadapter_verts[i].Position.Y = panadapterX_scope_data[i];
                    Panadapter_verts[i + 1].Color = scope_color.ToArgb();
                    Panadapter_verts[i + 1].Position.X = i / 2;
                    Panadapter_verts[i + 1].Position.Y = panadapterX_scope_data[i + 1];
                    i++;
                    j++;
                }

                Panadapter_vb.Lock(0, 0, LockFlags.None).WriteRange(Panadapter_verts, 0, panadapter_W * 2);
                Panadapter_vb.Unlock();

                dev.SetStreamSource(0, Panadapter_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineList, 0, count);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Rendering Scope fault!\n" + ex.ToString());
            }
        }

        private static void RenderWaterfallrScope(Device dev, int count)        // yt7pwr
        {
            try
            {
                int j = 0;

                for (int i = 0; i < count * 2; i++)
                {
                    Waterfall_verts[i].Color = scope_color.ToArgb();
                    Waterfall_verts[i].Position.X = i / 2; 
                    Waterfall_verts[i].Position.Y = panadapterX_scope_data[i];
                    Waterfall_verts[i + 1].Color = scope_color.ToArgb();
                    Waterfall_verts[i + 1].Position.X = i / 2;
                    Waterfall_verts[i + 1].Position.Y = panadapterX_scope_data[i + 1];
                    i++;
                    j++;
                }

                Waterfall_vb.Lock(0, 0, LockFlags.None).WriteRange(Waterfall_verts, 0, panadapter_W * 2);
                Waterfall_vb.Unlock();

                dev.SetStreamSource(0, Waterfall_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineList, 0, count);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Rendering Scope fault!\n" + ex.ToString());
            }
        }

        private static void RenderPanadapterLine(Device dev)        // yt7pwr
        {
            try
            {
                if (pan_fill)
                {
                    int j = 0;
                    int i = 0;

                    for (i = 0; i < panadapter_W * 2; i++)
                    {
                        Panadapter_verts_fill[i].Color = pan_fill_color.ToArgb();
                        Panadapter_verts_fill[i].Position.X = i / 2;
                        Panadapter_verts_fill[i].Position.Y = panadapterX_data[j];
                        Panadapter_verts_fill[i + 1].Color = pan_fill_color.ToArgb();
                        Panadapter_verts_fill[i + 1].Position.X = i / 2;
                        Panadapter_verts_fill[i + 1].Position.Y = panadapter_H;
                        i++;
                        j++;
                    }

                    PanLine_vb_fill.Lock(0, 0, LockFlags.None).WriteRange(Panadapter_verts_fill, 0, panadapter_W * 2);
                    PanLine_vb_fill.Unlock();

                    dev.SetStreamSource(0, PanLine_vb_fill, 0, 20);
                    dev.DrawPrimitives(PrimitiveType.LineList, 0, panadapter_W);
                }

                for (int i = 0; i < panadapter_W; i++)
                {
                    Panadapter_verts[i].Color = data_line_color.ToArgb();
                    Panadapter_verts[i].Position.X = i;
                    Panadapter_verts[i].Position.Y = panadapterX_data[i];
                }

                Panadapter_vb.Lock(0, 0, LockFlags.None).WriteRange(Panadapter_verts, 0, panadapter_W);
                Panadapter_vb.Unlock();

                dev.SetStreamSource(0, Panadapter_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineStrip, 0, panadapter_W - 1);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static void RenderPhase(Device dev)        // yt7pwr
        {
            try
            {
                int x, y;

                for (int i = 0, j = 0; i < phase_num_pts; i++, j += 8)	// fill point array
                {
                    x = (int)(current_display_data[i * 2] * panadapter_H / 2);
                    y = (int)(current_display_data[i * 2 + 1] * panadapter_H / 2);
                    Panadapter_verts[i].Color = data_line_color.ToArgb();
                    Panadapter_verts[i].Position = new Vector4(panadapter_W / 2 + x, panadapter_H / 2 + y, 0.0f, 0.0f);
                }

                Panadapter_vb.Lock(0, 0, LockFlags.None).WriteRange(Panadapter_verts, 0, phase_num_pts);
                Panadapter_vb.Unlock();

                dev.SetStreamSource(0, Panadapter_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineStrip, 0, phase_num_pts - 1);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static void RenderPhase2(Device dev)        // yt7pwr
        {
            try
            {
                int x, y;

                for (int i = 0, j = 0; i < phase_num_pts; i++, j += 8)	// fill point array
                {
                    x = (int)(current_display_data[i * 2] * panadapter_H * 0.5 * 500);
                    y = (int)(current_display_data[i * 2 + 1] * panadapter_H * 0.5 * 500);
                    Panadapter_verts[i].Color = data_line_color.ToArgb();
                    Panadapter_verts[i].Position.X = panadapter_W * 0.5f + x;
                    Panadapter_verts[i].Position.Y = panadapter_H * 0.5f + y;
                }

                Panadapter_vb.Lock(0, 0, LockFlags.None).WriteRange(Panadapter_verts, 0, phase_num_pts);
                Panadapter_vb.Unlock();

                dev.SetStreamSource(0, Panadapter_vb, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineStrip, 0, phase_num_pts - 1);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static int h_steps = 0;
        private static int h_steps_old = 0;
        private static VerticalString[] vertical_label;
        private static int vgrid;
        private static int offsetL;
        private static HorizontalString[] horizontal_label;
        private static void RenderPanadapterGrid(int W, int H)      // yt7pwr
        {
            try
            {
                int low = rx_display_low;					// initialize variables
                int high = rx_display_high;
                int mid_w = W / 2;
                int[] step_list = { 10, 20, 25, 50 };
                int step_power = 1;
                int step_index = 0;
                int freq_step_size = 50;
                int y_range = spectrum_grid_max - spectrum_grid_min;
                int center_line_x = W / 2;
                int first_vgrid = 0;
                double[] BandEdges = new double[100];
                int i;

                // Calculate horizontal step size
                int width = high - low;

                while (width / freq_step_size > 10)
                {
                    freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                    step_index = (step_index + 1) % 4;
                    if (step_index == 0) step_power++;
                }

                // calculate vertical step size
                h_steps = (spectrum_grid_max - spectrum_grid_min) / spectrum_grid_step;
                double vfo;

                if (mox && !(console.CurrentDSPMode == DSPMode.CWL || console.CurrentDSPMode == DSPMode.CWU))
                {
                    if (split_enabled)
                        vfo = vfob_hz;
                    else

                        vfo = vfoa_hz;
                    vfo += xit_hz;
                }
                else
                {
                    vfo = LOSC; //  +rit_hz;

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                            "Rendering Panadapter grid! " + losc_hz.ToString());

                }

                float scale = 0.0f;
                long vfo_round = ((long)(vfo / freq_step_size)) * freq_step_size;
                long vfo_delta = (long)(vfo - vfo_round);

                if (VerLines_vb == null)
                    VerLines_vb = new VertexBuffer(device, (200) * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                if (HorLines_vb == null || h_steps != h_steps_old)
                {
                    if (HorLines_vb != null)
                        HorLines_vb.Dispose();
                    HorLines_vb = null;
                    HorLines_vb = new VertexBuffer(device, h_steps * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                }
                if (vertical_label == null)
                    vertical_label = new VerticalString[40 + 2];
                if (horizontal_label == null || horizontal_label.Length < h_steps)
                    horizontal_label = new HorizontalString[h_steps];

                h_steps_old = h_steps;
                DB.GetBandLimitsEdges(ref BandEdges);

                // Draw vertical lines
                for (i = 3; i <= 40; i++)
                {
                    int fgrid = (i / 4) * freq_step_size + (low / freq_step_size) * freq_step_size;
                    double actual_fgrid = ((double)(vfo_round + fgrid)) / 1000000;

                    vgrid = (int)((double)(fgrid - vfo_delta - low) / (high - low) * W);

                    if (i == 3)
                        first_vgrid = vgrid;

                    VerLines_vb.Lock(i * 40, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4((float)vgrid, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4((float)vgrid, (float)H, 0.0f, 0.0f) },
                    });
                    VerLines_vb.Unlock();

                    RenderVerticalLine(device, vgrid, H, grid_color);

                        if (
                        actual_fgrid == console.xBand[1].freq_max || actual_fgrid == console.xBand[1].freq_min ||
                        actual_fgrid == console.xBand[2].freq_max || actual_fgrid == console.xBand[2].freq_min ||
                        actual_fgrid == console.xBand[3].freq_max || actual_fgrid == console.xBand[3].freq_min ||
                        actual_fgrid == console.xBand[4].freq_max || actual_fgrid == console.xBand[4].freq_min ||
                        actual_fgrid == console.xBand[5].freq_max || actual_fgrid == console.xBand[5].freq_min ||
                        actual_fgrid == console.xBand[6].freq_max || actual_fgrid == console.xBand[6].freq_min ||
                        actual_fgrid == console.xBand[7].freq_max || actual_fgrid == console.xBand[7].freq_min ||
                        actual_fgrid == console.xBand[8].freq_max || actual_fgrid == console.xBand[8].freq_min ||
                        actual_fgrid == console.xBand[9].freq_max || actual_fgrid == console.xBand[9].freq_min ||
                        actual_fgrid == console.xBand[10].freq_max || actual_fgrid == console.xBand[10].freq_min ||
                        actual_fgrid == console.xBand[11].freq_max || actual_fgrid == console.xBand[11].freq_min ||
                        actual_fgrid == console.xBand[12].freq_max || actual_fgrid == console.xBand[12].freq_min)
                        {
                            VerLines_vb.Lock(i * 40, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4((float)vgrid, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4((float)vgrid, (float)H, 0.0f, 0.0f) },
                            });
                            VerLines_vb.Unlock();

                            RenderVerticalLine(device, vgrid, H, band_edge_color);

                            vertical_label[i / 4].label = actual_fgrid.ToString("f3");
                            if (actual_fgrid < 10) offsetL = (int)((vertical_label[i / 4].label.Length + 1) * 4.1) - 14;
                            else if (actual_fgrid < 100.0) offsetL = (int)((vertical_label[i / 4].label.Length + 1) * 4.1) - 11;
                            else offsetL = (int)((vertical_label[i / 4].label.Length + 1) * 4.1) - 8;

                            panadapter_font.DrawString(null, vertical_label[i / 4].label, vgrid - offsetL, 0, grid_zero_color.ToArgb());
                            vertical_label[i / 4].pos_x = (vgrid - offsetL);
                            vertical_label[i / 4].pos_y = 0;
                            vertical_label[i / 4].color = grid_zero_color;
                        }
                        else
                        {
                            if (((double)((int)(actual_fgrid * 1000))) == actual_fgrid * 1000)
                            {
                                vertical_label[i / 4].label = actual_fgrid.ToString("f3");

                                if (actual_fgrid < 10) offsetL = (int)((vertical_label[i / 4].label.Length + 1) * 4.1) - 14;
                                else if (actual_fgrid < 100.0) offsetL = (int)((vertical_label[i / 4].label.Length + 1) * 4.1) - 11;
                                else offsetL = (int)((vertical_label[i / 4].label.Length + 1) * 4.1) - 8;
                            }
                            else
                            {
                                string temp_string;
                                int jper;
                                vertical_label[i / 4].label = actual_fgrid.ToString("f4");
                                temp_string = vertical_label[i / 4].label;
                                jper = vertical_label[i / 4].label.IndexOf('.') + 4;
                                vertical_label[i / 4].label = vertical_label[i / 4].label.Insert(jper, " ");

                                if (actual_fgrid < 10) offsetL = (int)((vertical_label[i / 4].label.Length) * 4.1) - 14;
                                else if (actual_fgrid < 100.0) offsetL = (int)((vertical_label[i / 4].label.Length) * 4.1) - 11;
                                else offsetL = (int)((vertical_label[i / 4].label.Length) * 4.1) - 8;
                            }

                            vertical_label[i / 4].pos_x = (vgrid - offsetL);
                            vertical_label[i / 4].pos_y = 0;
                            vertical_label[i / 4].color = grid_text_color;
                        }

                    int fgrid_2 = ((i / 4 + 1) * freq_step_size) + (int)((low / freq_step_size) * freq_step_size);
                    int x_2 = (int)(((float)(fgrid_2 - vfo_delta - low) / width * W));
                    scale = (float)(x_2 - vgrid) / 4;

                    for (int j = 1; j < 4; j++)
                    {
                        float x3 = (float)vgrid + (j * scale);

                        VerLines_vb.Lock((i + j) * 40, 40, LockFlags.None).WriteRange(new[] 
                    {
                        new Vertex() { Color = Color.FromArgb(Math.Max(grid_color.A - 30, 0), grid_color).ToArgb(), Position = new Vector4(x3, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = Color.FromArgb(Math.Max(grid_color.A - 30, 0), grid_color).ToArgb(), Position = new Vector4(x3, (float)H, 0.0f, 0.0f) },
                    });
                        VerLines_vb.Unlock();

                        RenderVerticalLine(device, (int)x3, H, Color.FromArgb(Math.Max(grid_color.A - 30, 0), grid_color));
                    }

                    i += 3;
                }

                for (int j = 0; j < 3; j++)
                {
                    int x3 = 0;
                    if (first_vgrid > 0 && first_vgrid > (int)scale)
                        x3 = first_vgrid - (j + 1) * (int)scale;
                    else
                        x3 = -1;

                    VerLines_vb.Lock((j) * 40, 40, LockFlags.None).WriteRange(new[] 
                    {
                        new Vertex() { Color = Color.FromArgb(Math.Max(grid_color.A - 30, 0), grid_color).ToArgb(), Position = new Vector4(x3, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = Color.FromArgb(Math.Max(grid_color.A - 30, 0), grid_color).ToArgb(), Position = new Vector4(x3, (float)H, 0.0f, 0.0f) },
                    });
                    VerLines_vb.Unlock();

                    RenderVerticalLine(device, (int)x3, H, Color.FromArgb(Math.Max(grid_color.A - 30, 0), grid_color));
                }

                //int[] band_edge_list = { 135700, 138700, 415000, 525000, 10150000, 14350000, 18068000, 18168000, 24890000, 24990000 };

                int k = 0;
                bool first = true;

                for(k=41; k<100; k++)
                VerLines_vb.Lock(k * 40, 80, LockFlags.None).WriteRange(new[] {    // clear first!
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4(0.0f, 0.0f, 0.0f, 0.0f) },
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4(0.0f, 0.0f, 0.0f, 0.0f) },
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4(0.0f, 0.0f, 0.0f, 0.0f) },
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4(0.0f, 0.0f, 0.0f, 0.0f) },
                            });

                k = 0;
                foreach (double b_edge in BandEdges)
                //for (int i = 0; i < band_edge_list.Length; i++)
                {
                    double band_edge_offset = b_edge * 1e6 - losc_hz;

                    if (band_edge_offset >= low && band_edge_offset <= high)
                    {
                        int temp_vline = (int)((double)(band_edge_offset - low) / (high - low) * W);//wa6ahl

                        if (temp_vline > 0 && temp_vline < W)
                        {
                            VerLines_vb.Lock((41 + k) * 40, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4((float)temp_vline, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = band_edge_color.ToArgb(), Position = new Vector4((float)temp_vline, (float)H, 0.0f, 0.0f) },
                            });
                            VerLines_vb.Unlock();

                            RenderVerticalLine(device, temp_vline, pan_font.Height, band_edge_color);
                            first = false;
                            k++;
                        }
                    }

                    i++;
                }

                ///////////////////// tx filter ////////////////////////////////

                if (draw_tx_cw_freq && !split_enabled && (current_dsp_mode != DSPMode.CWL || current_dsp_mode != DSPMode.CWU))
                {
                    int cw_line_x;
                    if (!split_enabled)
                        cw_line_x = (int)((float)(-low + vfoa_hz - losc_hz + xit_hz) / (high - low) * W);
                    else
                        cw_line_x = (int)((float)(-low + xit_hz + vfob_hz - losc_hz) / (high - low) * W);

                    VerLines_vb.Lock(42 * 40, 40, LockFlags.None).WriteRange(new[] 
                    {
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(cw_line_x, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(cw_line_x, (float)H, 0.0f, 0.0f) },
                    });
                    VerLines_vb.Unlock();

                    if (!mox && draw_tx_cw_freq &&
                        (current_dsp_mode != DSPMode.CWL && current_dsp_mode != DSPMode.CWU))
                        RenderVerticalLine(device, (int)cw_line_x, H, display_filter_tx_color);
                }
                else if (draw_tx_cw_freq && split_enabled &&
                    (current_dsp_mode_subRX != DSPMode.CWU && current_dsp_mode_subRX != DSPMode.CWL))
                {
                    // get tx filter limits
                    int filter_left_x;
                    int filter_right_x;

                    if (!split_enabled)
                    {
                        filter_left_x = (int)((float)(DttSP.TXFilterLowCut - low + vfoa_hz - losc_hz + xit_hz) / (high - low) * W);
                        filter_right_x = (int)((float)(DttSP.TXFilterHighCut - low + vfoa_hz - losc_hz + xit_hz) / (high - low) * W);
                    }
                    else
                    {
                        filter_left_x = (int)((float)(DttSP.TXFilterLowCut - low + xit_hz + (vfob_hz - losc_hz)) / (high - low) * W);
                        filter_right_x = (int)((float)(DttSP.TXFilterHighCut - low + xit_hz + (vfob_hz - losc_hz)) / (high - low) * W);
                    }

                    VerLines_vb.Lock(42 * 40, 40, LockFlags.None).WriteRange(new[] 
                    {
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(filter_left_x, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(filter_left_x, (float)H, 0.0f, 0.0f) },
                    });
                    VerLines_vb.Unlock();

                    VerLines_vb.Lock(43 * 40, 40, LockFlags.None).WriteRange(new[] 
                    {
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(filter_right_x, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(filter_right_x, (float)H, 0.0f, 0.0f) },
                    });
                    VerLines_vb.Unlock();
                }
                else if (draw_tx_cw_freq && split_enabled && (current_dsp_mode != DSPMode.CWL || current_dsp_mode != DSPMode.CWU))
                {
                    int cw_line_x;
                    if (!split_enabled)
                        cw_line_x = (int)((float)(-low + vfoa_hz - losc_hz + xit_hz) / (high - low) * W);
                    else
                        cw_line_x = (int)((float)(-low + xit_hz + vfob_hz - losc_hz) / (high - low) * W);

                    VerLines_vb.Lock(42 * 40, 40, LockFlags.None).WriteRange(new[] 
                    {
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(cw_line_x, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = Color.FromArgb(display_filter_tx_color.A, display_filter_tx_color).ToArgb(),
                            Position = new Vector4(cw_line_x, (float)H, 0.0f, 0.0f) },
                    });
                    VerLines_vb.Unlock();

                    if (!mox && draw_tx_cw_freq &&
                        (current_dsp_mode != DSPMode.CWL && current_dsp_mode != DSPMode.CWU))
                        RenderVerticalLine(device, (int)cw_line_x, H, display_filter_tx_color);
                }

                // Draw horizontal lines
                for (i = 1; i < h_steps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)((double)(spectrum_grid_max - num) * H / y_range); // +(int)pan_font.Size;

                    if (show_horizontal_grid)
                    {
                        HorLines_vb.Lock(i * 40, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4(0.0f, (float)y, 0.0f, 0.0f) },
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4((float)W, (float)y, 0.0f, 0.0f) },
                    });
                        HorLines_vb.Unlock();

                        RenderHorizontalLine(device, 0, y, grid_color);
                    }

                    // Draw horizontal line labels
                    num = spectrum_grid_max - i * spectrum_grid_step;
                    horizontal_label[i].label = num.ToString();

                    if (horizontal_label[i].label.Length == 3) xOffset = 5;
                    //int offset = (int)(label.Length*4.1);
                    if (display_label_align != DisplayLabelAlignment.LEFT &&
                        display_label_align != DisplayLabelAlignment.AUTO &&
                        (current_dsp_mode == DSPMode.USB ||
                        current_dsp_mode == DSPMode.CWU))
                        xOffset -= 32;
                    float size = pan_font.Size * 3;

                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                            x = center_line_x + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                            x = (int)(W - size);
                            break;
                        case DisplayLabelAlignment.AUTO:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = W;
                            break;
                    }

                    y -= 8;

                    if (mox && y + 9 < H)
                    {
                        panadapter_font.DrawString(null, horizontal_label[i].label, x, y, grid_text_color.ToArgb());
                        horizontal_label[i].pos_x = x;
                        horizontal_label[i].pos_y = y;
                        horizontal_label[i].color = grid_text_color;
                    }
                    else if (y + 9 < H)
                    {
                        panadapter_font.DrawString(null, horizontal_label[i].label, x, y, grid_text_color.ToArgb());
                        horizontal_label[i].pos_x = x;
                        horizontal_label[i].pos_y = y;
                        horizontal_label[i].color = grid_text_color;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static void RenderWaterfallGrid()  // changes yt7pwr
        {/*
            int W = waterfall_W;
            int H = waterfall_H;
            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int mid_w = W / 2;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 50;

            System.Drawing.Font font = new System.Drawing.Font("Arial", 9);
            SolidBrush grid_text_brush = new SolidBrush(grid_text_color);
            Pen grid_pen = new Pen(grid_color);
            Pen tx_filter_pen = new Pen(display_filter_tx_color);
            int y_range = spectrum_grid_max - spectrum_grid_min;
            int filter_low, filter_high;

            int center_line_x = (int)(-(double)low / (high - low) * W);

            if (mox) // get filter limits
            {
                filter_low = DttSP.TXFilterLowCut;
                filter_high = DttSP.TXFilterHighCut;
            }
            else
            {
                filter_low = DttSP.RXFilterLowCut;
                filter_high = DttSP.RXFilterHighCut;
            }

            // Calculate horizontal step size
            int width = high - low;
            while (width / freq_step_size > 10)
            {
                freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                step_index = (step_index + 1) % 4;
                if (step_index == 0) step_power++;
            }
            double w_pixel_step = (double)W * freq_step_size / width;
            int w_steps = width / freq_step_size;

            // calculate vertical step size
            int h_steps = (spectrum_grid_max - spectrum_grid_min) / spectrum_grid_step;
            double h_pixel_step = (double)H / h_steps;
            int top = (int)((double)spectrum_grid_step * H / y_range);

            if (sub_rx_enabled)
            {
                if (console.CurrentDSPMode == DSPMode.CWL || console.CurrentDSPMode == DSPMode.CWU)
                {
                    // draw Sub RX filter
                    // get filter screen coordinates
                    int filter_left_x = (int)((float)(-low - ((filter_high - filter_low) / 2) + vfob_hz - losc_hz) / (high - low) * W);
                    int filter_right_x = (int)((float)(-low + ((filter_high - filter_low) / 2) + vfob_hz - losc_hz) / (high - low) * W);

                    // make the filter display at least one pixel wide.
                    if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                    // draw rx filter
                    g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw filter overlay
                        filter_left_x, 0, filter_right_x - filter_left_x, top);

                    // draw Sub RX 0Hz line
                    int x = (filter_right_x - filter_left_x) / 2;
                    x += filter_left_x;
                    g.DrawLine(new Pen(sub_rx_zero_line_color), x, 0, x, top);
                    g.DrawLine(new Pen(sub_rx_zero_line_color), x - 1, 0, x - 1, top);
                }
                else
                {
                    // get filter screen coordinates
                    int filter_left_x = (int)((float)(filter_low - low + vfob_hz - losc_hz) / (high - low) * W);
                    int filter_right_x = (int)((float)(filter_high - low + vfob_hz - losc_hz) / (high - low) * W);

                    // make the filter display at least one pixel wide.
                    if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                    // draw rx filter
                    g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw filter overlay
                        filter_left_x, 0, filter_right_x - filter_left_x, top);

                    // draw Sub RX 0Hz line
                    int x = (int)((float)(vfob_hz - losc_hz - low) / (high - low) * W);
                    g.DrawLine(new Pen(sub_rx_zero_line_color), x, 0, x, top);
                    g.DrawLine(new Pen(sub_rx_zero_line_color), x - 1, 0, x - 1, top);
                }
            }

            if (!mox)
            {
                if (!(current_dsp_mode == DSPMode.CWL || current_dsp_mode == DSPMode.CWU))
                {
                    // get filter screen coordinates
                    int filter_left_x = (int)((float)(filter_low - low + vfoa_hz - losc_hz) / (high - low) * W);
                    int filter_right_x = (int)((float)(filter_high - low + vfoa_hz - losc_hz) / (high - low) * W);

                    // make the filter display at least one pixel wide.
                    if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                    // draw rx filter
                    g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                        filter_left_x, 0, filter_right_x - filter_left_x, top);

                    // draw Main RX 0Hz line
                    int x = (int)((float)(vfoa_hz - losc_hz - low) / (high - low) * W);
                    g.DrawLine(new Pen(main_rx_zero_line_color), x, 0, x, top);
                    g.DrawLine(new Pen(main_rx_zero_line_color), x - 1, 0, x - 1, top);
                }
                else
                {
                    // draw RX filter
                    // get filter screen coordinates
                    int filter_left_x = (int)((float)(-low - ((filter_high - filter_low) / 2) + vfoa_hz - losc_hz) / (high - low) * W);
                    int filter_right_x = (int)((float)(-low + ((filter_high - filter_low) / 2) + vfoa_hz - losc_hz) / (high - low) * W);

                    // make the filter display at least one pixel wide.
                    if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                    // draw rx filter
                    g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                        filter_left_x, 0, filter_right_x - filter_left_x, top);

                    // draw Sub RX 0Hz line
                    int x = (filter_right_x - filter_left_x) / 2;
                    x += filter_left_x;
                    g.DrawLine(new Pen(main_rx_zero_line_color), x, 0, x, top);
                    g.DrawLine(new Pen(main_rx_zero_line_color), x - 1, 0, x - 1, top);

                }
            }

            if (!mox && draw_tx_filter &&
                (current_dsp_mode != DSPMode.CWL && current_dsp_mode != DSPMode.CWU))
            {
                // get tx filter limits
                int filter_left_x;
                int filter_right_x;

                if (!split_enabled)
                {
                    filter_left_x = (int)((float)(DttSP.TXFilterLowCut - low + xit_hz) / (high - low) * W);
                    filter_right_x = (int)((float)(DttSP.TXFilterHighCut - low + xit_hz) / (high - low) * W);
                }
                else
                {
                    filter_left_x = (int)((float)(DttSP.TXFilterLowCut - low + xit_hz + (vfob_hz - vfoa_hz)) / (high - low) * W);
                    filter_right_x = (int)((float)(DttSP.TXFilterHighCut - low + xit_hz + (vfob_hz - vfoa_hz)) / (high - low) * W);
                }

                g.DrawLine(tx_filter_pen, filter_left_x, 0, filter_left_x, top);		// draw tx filter overlay
                g.DrawLine(tx_filter_pen, filter_left_x + 1, 0, filter_left_x + 1, top);	// draw tx filter overlay
                g.DrawLine(tx_filter_pen, filter_right_x, 0, filter_right_x, top);		// draw tx filter overlay
                g.DrawLine(tx_filter_pen, filter_right_x + 1, 0, filter_right_x + 1, top);	// draw tx filter overlay
            }

            double vfo;

            if (mox)
            {
                if (split_enabled)
                    vfo = vfob_hz;
                else
                    vfo = vfoa_hz;
                vfo += xit_hz;
            }
            else
            {
                vfo = losc_hz; // +rit_hz;
            }

            long vfo_round = ((long)(vfo / freq_step_size)) * freq_step_size;
            long vfo_delta = (long)(vfo - vfo_round);

            // Draw vertical lines
            for (int i = 0; i <= h_steps + 1; i++)
            {
                string label;
                int offsetL;

                int fgrid = i * freq_step_size + (low / freq_step_size) * freq_step_size;
                double actual_fgrid = ((double)(vfo_round + fgrid)) / 1000000;
                int vgrid = (int)((double)(fgrid - vfo_delta - low) / (high - low) * W);

                if (actual_fgrid == 1.8 || actual_fgrid == 2.0 ||
                    actual_fgrid == 3.5 || actual_fgrid == 4.0 ||
                    actual_fgrid == 7.0 || actual_fgrid == 7.3 ||
                    actual_fgrid == 10.1 || actual_fgrid == 10.15 ||
                    actual_fgrid == 14.0 || actual_fgrid == 14.35 ||
                    actual_fgrid == 18.068 || actual_fgrid == 18.168 ||
                    actual_fgrid == 21.0 || actual_fgrid == 21.45 ||
                    actual_fgrid == 24.89 || actual_fgrid == 24.99 ||
                    actual_fgrid == 21.0 || actual_fgrid == 21.45 ||
                    actual_fgrid == 28.0 || actual_fgrid == 29.7 ||
                    actual_fgrid == 50.0 || actual_fgrid == 54.0 ||
                    actual_fgrid == 144.0 || actual_fgrid == 148.0)
                {
                    g.DrawLine(new Pen(band_edge_color), vgrid, top, vgrid, H);

                    label = actual_fgrid.ToString("f3");
                    if (actual_fgrid < 10) offsetL = (int)((label.Length + 1) * 4.1) - 14;
                    else if (actual_fgrid < 100.0) offsetL = (int)((label.Length + 1) * 4.1) - 11;
                    else offsetL = (int)((label.Length + 1) * 4.1) - 8;

                    g.DrawString(label, font, new SolidBrush(band_edge_color), vgrid - offsetL, (float)Math.Floor(H * .01));
                }
                else
                {

                    if (freq_step_size >= 2000)
                    {
                        double t100;
                        double t1000;
                        t100 = (actual_fgrid * 100);
                        t1000 = (actual_fgrid * 1000);

                        int it100 = (int)t100;
                        int it1000 = (int)t1000;

                        int it100x10 = it100 * 10;

                        if (it100x10 == it1000)
                        {
                        }
                        else
                        {
//                            grid_pen.DashStyle = DashStyle.Dot;
                        }
                    }
                    else
                    {
                        if (freq_step_size == 1000)
                        {
                            double t200;
                            double t2000;
                            t200 = (actual_fgrid * 200);
                            t2000 = (actual_fgrid * 2000);

                            int it200 = (int)t200;
                            int it2000 = (int)t2000;

                            int it200x10 = it200 * 10;

                            if (it200x10 == it2000)
                            {
                            }
                            else
                            {
//                                grid_pen.DashStyle = DashStyle.Dot;
                            }
                        }
                        else
                        {
                            double t1000;
                            double t10000;
                            t1000 = (actual_fgrid * 1000);
                            t10000 = (actual_fgrid * 10000);

                            int it1000 = (int)t1000;
                            int it10000 = (int)t10000;

                            int it1000x10 = it1000 * 10;

                            if (it1000x10 == it10000)
                            {
                            }
                            else
                            {
//                                grid_pen.DashStyle = DashStyle.Dot;
                            }
                        }
                    }
                    //g.DrawLine(grid_pen, vgrid, top, vgrid, H);			//wa6ahl
//                    grid_pen.DashStyle = DashStyle.Solid;

                    if (((double)((int)(actual_fgrid * 1000))) == actual_fgrid * 1000)
                    {
                        label = actual_fgrid.ToString("f3"); //wa6ahl

                        //if(actual_fgrid > 1300.0)
                        //	label = label.Substring(label.Length-4);

                        if (actual_fgrid < 10) offsetL = (int)((label.Length + 1) * 4.1) - 14;
                        else if (actual_fgrid < 100.0) offsetL = (int)((label.Length + 1) * 4.1) - 11;
                        else offsetL = (int)((label.Length + 1) * 4.1) - 8;
                    }
                    else
                    {
                        string temp_string;
                        int jper;
                        label = actual_fgrid.ToString("f4");
                        temp_string = label;
                        jper = label.IndexOf('.') + 4;
                        label = label.Insert(jper, " ");

                        //if(actual_fgrid > 1300.0)
                        //	label = label.Substring(label.Length-4);

                        if (actual_fgrid < 10) offsetL = (int)((label.Length) * 4.1) - 14;
                        else if (actual_fgrid < 100.0) offsetL = (int)((label.Length) * 4.1) - 11;
                        else offsetL = (int)((label.Length) * 4.1) - 8;
                    }

                    g.DrawString(label, font, grid_text_brush, vgrid - offsetL, (float)Math.Floor(H * .01));
                }
            }

            int[] band_edge_list = { 18068000, 18168000, 1800000, 2000000, 3500000, 4000000,
									   7000000, 7300000, 10100000, 10150000, 14000000, 14350000, 21000000, 21450000,
									   24890000, 24990000, 28000000, 29700000, 50000000, 54000000, 144000000, 148000000 };

            for (int i = 0; i < band_edge_list.Length; i++)
            {
                double band_edge_offset = band_edge_list[i] - losc_hz;
                if (band_edge_offset >= low && band_edge_offset <= high)
                {
                    int temp_vline = (int)((double)(band_edge_offset - low) / (high - low) * W);//wa6ahl
                    g.DrawLine(new Pen(band_edge_color), temp_vline, 0, temp_vline, top);//wa6ahl
                }
                if (i == 1 && !show_freq_offset) break;
            }

            // Draw 0Hz vertical line if visible
            if (center_line_x >= 0 && center_line_x <= W)
            {
                g.DrawLine(new Pen(grid_zero_color), center_line_x, 0, center_line_x, top);
                g.DrawLine(new Pen(grid_zero_color), center_line_x - 1, 0, center_line_x - 1, top);
            }

            if (high_swr)
                g.DrawString("High SWR", new System.Drawing.Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Red), 245, 20);*/
        }

        private static float[] waterfall_data;
        private static bool ConvertDataForWaterfall()                 // yt7pwr
        {
            int W = waterfall_W;
            if (current_display_mode == DisplayMode.WATERFALL)
                RenderWaterfallGrid();
            if (waterfall_data == null || waterfall_data.Length < W)
                waterfall_data = new float[W];			                    // array of points to display

            float slope = 0.0F;						                        // samples to process per pixel
            UInt64 num_samples = 0;					                        // number of samples to process
            int start_sample_index = 0;				                        // index to begin looking at samples
            int low = 0;
            int high = 0;
            low = rx_display_low;
            high = rx_display_high;
            max_y = Int32.MinValue;
            int R = 0, G = 0, B = 0;	                                	 // variables to save Red, Green and Blue component values

            if (console.PowerOn)
            {
                int yRange = spectrum_grid_max - spectrum_grid_min;

                if (waterfall_data_ready && !console.MOX)
                {
                    if (console.TUN || mox && (current_dsp_mode == DSPMode.CWL || current_dsp_mode == DSPMode.CWU))
                    {
                        for (int i = 0; i < current_waterfall_data.Length; i++)
                            current_waterfall_data[i] = -200.0f;
                    }
                    else
                    {
                        // get new data
                        Array.Copy(new_waterfall_data, current_waterfall_data, current_waterfall_data.Length);
                        waterfall_data_ready = false;
                    }
                }

                if (average_on)
                    console.UpdateDirectXDisplayWaterfallAverage();
                if (peak_on)
                    UpdateDisplayPeak();

                start_sample_index = (BUFFER_SIZE >> 1) + (int)(((double)low * (double)BUFFER_SIZE) / DttSP.RXSampleRate);
                num_samples = (UInt64)((((double)high - (double)low)/1e4) * (double)BUFFER_SIZE / (DttSP.RXSampleRate/1e4));
                start_sample_index = (start_sample_index + 32768) % 32768;

                if (((int)num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                    num_samples = (UInt64)(BUFFER_SIZE - start_sample_index);

                slope = (float)num_samples / (float)W;

                for (int i = 0; i < W; i++)
                {
                    float max = float.MinValue;
                    float dval = i * slope + start_sample_index;
                    int lindex = (int)Math.Floor(dval);
                    int rindex = (int)Math.Floor(dval + slope);

                    if (slope <= 1 || lindex == rindex)
                        max = current_waterfall_data[lindex] * ((float)lindex - dval + 1) +
                            current_waterfall_data[(lindex + 1) % 32768] * (dval - (float)lindex);
                    else
                    {
                        for (int j = lindex; j < rindex; j++)
                            if (current_waterfall_data[j % 32768] > max) max = current_waterfall_data[j % 32768];
                    }

                    max += display_cal_offset;

                    if (max > max_y)
                    {
                        max_y = max;
                        max_x = i;
                    }

                    waterfall_data[i] = max;
                }

                int pixel_size = 4;
                int ptr = 0;

                if (!console.MOX)
                {
                    if (reverse_waterfall)
                    {
                        Array.Copy(waterfall_memory, waterfall_bmp_stride, waterfall_memory, 0, waterfall_bmp_size - waterfall_bmp_stride);
                        ptr = waterfall_bmp_size - waterfall_bmp_stride;
                    }
                    else
                    {
                        Array.Copy(waterfall_memory, 0, waterfall_memory, waterfall_bmp_stride, waterfall_bmp_size - waterfall_bmp_stride);
                    }

                    int i = 0;
                    switch (color_sheme)
                    {
                        case (ColorSheme.original):                         // tre color only
                            {
                                // draw new data
                                for (i = 0; i < W; i++)	                    // for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)		// if less than low threshold, just use low color
                                    {
                                        R = WaterfallLowColor.R;
                                        G = WaterfallLowColor.G;
                                        B = WaterfallLowColor.B;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold)// if more than high threshold, just use high color
                                    {
                                        R = WaterfallHighColor.R;
                                        G = WaterfallHighColor.G;
                                        B = WaterfallHighColor.B;
                                    }
                                    else // use a color between high and low
                                    {
                                        float percent = (waterfall_data[i] - waterfall_low_threshold) / (WaterfallHighThreshold - waterfall_low_threshold);
                                        if (percent <= 0.5)	// use a gradient between low and mid colors
                                        {
                                            percent *= 2;

                                            R = (int)((1 - percent) * WaterfallLowColor.R + percent * WaterfallMidColor.R);
                                            G = (int)((1 - percent) * WaterfallLowColor.G + percent * WaterfallMidColor.G);
                                            B = (int)((1 - percent) * WaterfallLowColor.B + percent * WaterfallMidColor.B);
                                        }
                                        else				// use a gradient between mid and high colors
                                        {
                                            percent = (float)(percent - 0.5) * 2;

                                            R = (int)((1 - percent) * WaterfallMidColor.R + percent * WaterfallHighColor.R);
                                            G = (int)((1 - percent) * WaterfallMidColor.G + percent * WaterfallHighColor.G);
                                            B = (int)((1 - percent) * WaterfallMidColor.B + percent * WaterfallHighColor.B);
                                        }
                                    }

                                    // set pixel color
                                    waterfall_memory[i * pixel_size + 3 + ptr] = (byte)waterfall_alpha;	// set color in memory
                                    waterfall_memory[i * pixel_size + 2 + ptr] = (byte)R;	// set color in memory
                                    waterfall_memory[i * pixel_size + 1 + ptr] = (byte)G;
                                    waterfall_memory[i * pixel_size + 0 + ptr] = (byte)B;
                                }
                            }
                            break;

                        case (ColorSheme.enhanced):
                            {
                                // draw new data
                                for (i = 0; i < W; i++)	// for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)
                                    {
                                        R = WaterfallLowColor.R;
                                        G = WaterfallLowColor.G;
                                        B = WaterfallLowColor.B;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold)
                                    {
                                        R = 192;
                                        G = 124;
                                        B = 255;
                                    }
                                    else // value is between low and high
                                    {
                                        float range = WaterfallHighThreshold - waterfall_low_threshold;
                                        float offset = waterfall_data[i] - waterfall_low_threshold;
                                        float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.

                                        if (overall_percent < (float)2 / 9) // background to blue
                                        {
                                            float local_percent = overall_percent / ((float)2 / 9);
                                            R = (int)((1.0 - local_percent) * WaterfallLowColor.R);
                                            G = (int)((1.0 - local_percent) * WaterfallLowColor.G);
                                            B = (int)(WaterfallLowColor.B + local_percent * (255 - WaterfallLowColor.B));
                                        }
                                        else if (overall_percent < (float)3 / 9) // blue to blue-green
                                        {
                                            float local_percent = (overall_percent - (float)2 / 9) / ((float)1 / 9);
                                            R = 0;
                                            G = (int)(local_percent * 255);
                                            B = 255;
                                        }
                                        else if (overall_percent < (float)4 / 9) // blue-green to green
                                        {
                                            float local_percent = (overall_percent - (float)3 / 9) / ((float)1 / 9);
                                            R = 0;
                                            G = 255;
                                            B = (int)((1.0 - local_percent) * 255);
                                        }
                                        else if (overall_percent < (float)5 / 9) // green to red-green
                                        {
                                            float local_percent = (overall_percent - (float)4 / 9) / ((float)1 / 9);
                                            R = (int)(local_percent * 255);
                                            G = 255;
                                            B = 0;
                                        }
                                        else if (overall_percent < (float)7 / 9) // red-green to red
                                        {
                                            float local_percent = (overall_percent - (float)5 / 9) / ((float)2 / 9);
                                            R = 255;
                                            G = (int)((1.0 - local_percent) * 255);
                                            B = 0;
                                        }
                                        else if (overall_percent < (float)8 / 9) // red to red-blue
                                        {
                                            float local_percent = (overall_percent - (float)7 / 9) / ((float)1 / 9);
                                            R = 255;
                                            G = 0;
                                            B = (int)(local_percent * 255);
                                        }
                                        else // red-blue to purple end
                                        {
                                            float local_percent = (overall_percent - (float)8 / 9) / ((float)1 / 9);
                                            R = (int)((0.75 + 0.25 * (1.0 - local_percent)) * 255);
                                            G = (int)(local_percent * 255 * 0.5);
                                            B = 255;
                                        }
                                    }

                                    // set pixel color
                                    waterfall_memory[i * pixel_size + 3 + ptr] = (byte)waterfall_alpha;
                                    waterfall_memory[i * pixel_size + 2 + ptr] = (byte)R;	// set color in memory
                                    waterfall_memory[i * pixel_size + 1 + ptr] = (byte)G;
                                    waterfall_memory[i * pixel_size + 0 + ptr] = (byte)B;
                                }
                            }
                            break;

                        case (ColorSheme.SPECTRAN):
                            {
                                // draw new data
                                for (i = 0; i < W; i++)	// for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)
                                    {
                                        R = 0;
                                        G = 0;
                                        B = 0;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                    {
                                        R = 240;
                                        G = 240;
                                        B = 240;
                                    }
                                    else // value is between low and high
                                    {
                                        float range = WaterfallHighThreshold - waterfall_low_threshold;
                                        float offset = waterfall_data[i] - waterfall_low_threshold;
                                        float local_percent = ((100.0f * offset) / range);

                                        if (local_percent < 5.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 11.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 22.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 44.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 51.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 66.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 2;
                                            B = 255;
                                        }
                                        else if (local_percent < 77.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 3;
                                            B = 255;
                                        }
                                        else if (local_percent < 88.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 4;
                                            B = 255;
                                        }
                                        else if (local_percent < 99.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 5;
                                            B = 255;
                                        }
                                    }

                                    // set pixel color
                                    waterfall_memory[i * pixel_size + 0 + ptr] = (byte)waterfall_alpha;
                                    waterfall_memory[i * pixel_size + 1 + ptr] = (byte)R;	// set color in memory
                                    waterfall_memory[i * pixel_size + 2 + ptr] = (byte)G;
                                    waterfall_memory[i * pixel_size + 3 + ptr] = (byte)B;
                                }
                            }
                            break;

                        case (ColorSheme.BLACKWHITE):
                            {
                                // draw new data
                                for (i = 0; i < W; i++)	// for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)
                                    {
                                        R = 0;
                                        G = 0;
                                        B = 0;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                    {
                                        R = 255;
                                        G = 255;
                                        B = 255;
                                    }
                                    else // value is between low and high
                                    {
                                        float range = WaterfallHighThreshold - waterfall_low_threshold;
                                        float offset = waterfall_data[i] - waterfall_low_threshold;
                                        float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.
                                        float local_percent = ((100.0f * offset) / range);
                                        float contrast = (console.SetupForm.DisplayContrast / 100);
                                        R = (int)((local_percent / 100) * 255);
                                        G = R;
                                        B = R;
                                    }

                                    // set pixel color
                                    waterfall_memory[i * pixel_size + 3 + ptr] = (byte)waterfall_alpha;
                                    waterfall_memory[i * pixel_size + 2 + ptr] = (byte)R;	// set color in memory
                                    waterfall_memory[i * pixel_size + 1 + ptr] = (byte)G;
                                    waterfall_memory[i * pixel_size + 0 + ptr] = (byte)B;
                                }
                                break;
                            }
                    }
                }
            }

            return true;
        }

        public static bool RenderDirectX()  // changes yt7pwr
        {
            try
            {
                if (device == null || waterfall_dx_device == null)
                {
                    return false;
                }

                if (DX_reinit)
                    return true;

                // setup data
                switch (current_display_mode)
                {
                    case DisplayMode.PANASCOPE:
                        if (mox)
                        {
                            if (current_dsp_mode == DSPMode.CWU || current_dsp_mode == DSPMode.CWL)
                                ConvertDataForScope(waterfall_W, waterfall_H);
                            else
                            {
                                ConvertDataForScope(waterfall_W, waterfall_H);
                                ConvertDataForPanadapter();
                            }
                        }
                        else
                        {
                            ConvertDataForPanadapter();
                            ConvertDataForScope(waterfall_W, waterfall_H);
                        }
                        break;
                    case DisplayMode.HISTOGRAM:
                        ConvertDataForHistogram();
                        break;
                    case DisplayMode.PANADAPTER:
                        ConvertDataForPanadapter();
                        break;
                    case DisplayMode.SPECTRUM:
                        ConvertDataForSpectrum();
                        break;
                    case DisplayMode.PHASE:
                    case DisplayMode.PHASE2:
                        ConvertDataForPhase();
                        break;
                    case DisplayMode.SCOPE:
                        ConvertDataForScope(panadapter_W, panadapter_H);
                        break;
                    case DisplayMode.PANAFALL:
                    case DisplayMode.PANAFALL_INV:
                    case DisplayMode.WATERFALL:
                        ConvertDataForPanadapter();
                        ConvertDataForWaterfall();
                        break;
                    default:
                        ConvertDataForPanadapter();
                        break;
                }


                try
                {
                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);

                    if (Panadapter_Sprite != null)
                    {
                        try
                        {
                            Panadapter_Sprite.Begin(SpriteFlags.AlphaBlend);

                            if (PanadapterTexture != null)
                                Panadapter_Sprite.Draw(PanadapterTexture, Panadapter_texture_size, (Color4)Color.White);

                            if (high_swr)
                            {
                                high_swr_font.DrawString(Panadapter_Sprite, string.Format("High SWR"),
                                    new Rectangle(40, 20, 0, 0), DrawTextFormat.NoClip, Color.Red);
                            }

                            Panadapter_Sprite.End();
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.ToString());

                            if (debug && !console.ConsoleClosing)
                                console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                    "Rendering Panadapter Sprite error!\n" + ex.ToString());
                        }
                    }

                    if (current_display_mode == DisplayMode.PANASCOPE)
                    {
                        try
                        {
                            waterfall_dx_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);
                            Waterfall_Sprite.Begin(SpriteFlags.AlphaBlend);
                            Waterfall_Sprite.Draw(WaterfallBackgroundTexture, Waterfall_texture_size, (Color4)Color.White);
                            Waterfall_Sprite.End();
                            //Begin the scene
                            waterfall_dx_device.BeginScene();
                            waterfall_dx_device.SetRenderState(RenderState.AlphaBlendEnable, true);
                            waterfall_dx_device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                            waterfall_dx_device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationAlpha);
                            waterfall_dx_device.SetRenderState(RenderState.Lighting, true);

                            if (console.chkPower.Checked)
                            {
                                RenderWaterfallrScope(waterfall_dx_device, waterfall_W);
                            }

                            waterfall_dx_device.EndScene();
                            waterfall_dx_device.Present();
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.ToString());

                            if (debug && !console.ConsoleClosing)
                                console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                    "Rendering Panascope fault!\n" + ex.ToString());
                        }
                    }

                    //Begin the scene
                    device.BeginScene();
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                    device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationAlpha);

                    switch (current_display_mode)
                    {
                        case DisplayMode.SPECTRUM:
                            RenderSpectrum();
                            RenderPanadapterLine(device);
                            break;
                        case DisplayMode.SCOPE:
                            RenderScopeGrid();
                            RenderPanadapterScope(device, panadapter_W);
                            break;
                        case DisplayMode.WATERFALL:
                        case DisplayMode.PANADAPTER:
                        case DisplayMode.PANAFALL:
                        case DisplayMode.PANAFALL_INV:
                        case DisplayMode.PANASCOPE:
                            if (console.chkPower.Checked)
                            {
                                if (current_display_mode == DisplayMode.PANAFALL ||
                                    current_display_mode == DisplayMode.PANAFALL_INV ||
                                current_display_mode == DisplayMode.WATERFALL)
                                {
                                    try
                                    {
                                        if (!MOX)
                                        {
                                            DataRectangle data;
                                            data = WaterfallTexture.LockRectangle(0, waterfall_rect, LockFlags.None);
                                            waterfall_data_stream = data.Data;
                                            waterfall_data_stream.Position = 0;
                                            waterfall_data_stream.Write(waterfall_memory, 0, (int)waterfall_data_stream.Length);
                                            WaterfallTexture.UnlockRectangle(0);
                                            waterfall_dx_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);
                                            Waterfall_Sprite.Begin(SpriteFlags.AlphaBlend);
                                            Waterfall_Sprite.Draw(WaterfallTexture, Waterfall_texture_size, (Color4)Color.White);
                                            Waterfall_Sprite.End();
                                            waterfall_dx_device.BeginScene();
                                            waterfall_dx_device.SetRenderState(RenderState.AlphaBlendEnable, true);
                                            waterfall_dx_device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                                            waterfall_dx_device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationAlpha);
                                            RenderVerticalLine(waterfall_dx_device, 0, 0, Color.Black);
                                            waterfall_dx_device.EndScene();
                                            waterfall_dx_device.Present();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Write(ex.ToString());

                                        if (debug && !console.ConsoleClosing)
                                            console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                                "Rendering Waterfall fault!\n" + ex.ToString());
                                    }
                                }

                                if (refresh_panadapter_grid || refresh_grid)
                                {
                                    vertical_label = null;
                                    horizontal_label = null;

                                    if (refresh_panadapter_grid)
                                        refresh_panadapter_grid = false;

                                    if (refresh_grid)
                                        refresh_grid = false;

                                    RenderPanadapterGrid(panadapter_W, panadapter_H);
                                }
                                else
                                {
                                    try
                                    {
                                        if (show_horizontal_grid)
                                            RenderHorizontalLines(device, HorLines_vb, h_steps);
                                        if (show_vertical_grid && draw_tx_cw_freq && !mox)
                                            RenderVerticalLines(device, VerLines_vb, 100);
                                        else if (show_vertical_grid)
                                            RenderVerticalLines(device, VerLines_vb, 100);
                                    }
                                    catch (Direct3D9Exception ex)
                                    {
                                        Debug.Write("Error rendering grid\n" + ex.ToString());

                                        if (debug && !console.ConsoleClosing)
                                            console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                                "Render grid fault!\n" + ex.ToString());

                                        return false;
                                    }
                                }

                                for (int i = 0; i < 10; i++)
                                {
                                    panadapter_font.DrawString(null, vertical_label[i].label,
                                        vertical_label[i].pos_x, vertical_label[i].pos_y, vertical_label[i].color.ToArgb());
                                }

                                for (int i = 1; i < h_steps; i++)
                                {
                                    panadapter_font.DrawString(null, horizontal_label[i].label,
                                        horizontal_label[i].pos_x, horizontal_label[i].pos_y, horizontal_label[i].color.ToArgb());
                                }

                                Render_VFOA();

                                if (sub_rx_enabled || split_enabled)
                                    Render_VFOB();

                                RenderPanadapterLine(device);
                            }
                            break;
                        case DisplayMode.PHASE:
                            if (console.chkPower.Checked)
                                RenderPhase(device);
                            break;
                        case DisplayMode.PHASE2:
                            if (console.chkPower.Checked)
                                RenderPhase2(device);
                            break;
                        case DisplayMode.HISTOGRAM:
                            if (console.chkPower.Checked)
                            {
                                if (refresh_panadapter_grid || refresh_grid)
                                {
                                    vertical_label = null;
                                    horizontal_label = null;

                                    if (refresh_panadapter_grid)
                                        refresh_panadapter_grid = false;

                                    if (refresh_grid)
                                        refresh_grid = false;

                                    RenderPanadapterGrid(panadapter_W, panadapter_H);
                                }
                                else
                                {
                                    if (show_horizontal_grid)
                                        RenderHorizontalLines(device, HorLines_vb, h_steps);
                                    if (show_vertical_grid)
                                        RenderVerticalLines(device, VerLines_vb, 100);
                                }

                                for (int i = 0; i < 10; i++)
                                {
                                    panadapter_font.DrawString(null, vertical_label[i].label,
                                        vertical_label[i].pos_x, vertical_label[i].pos_y, vertical_label[i].color.ToArgb());
                                }

                                for (int i = 2; i < h_steps; i++)
                                {
                                    panadapter_font.DrawString(null, horizontal_label[i].label,
                                        horizontal_label[i].pos_x, horizontal_label[i].pos_y, horizontal_label[i].color.ToArgb());
                                }

                                RenderHistogram(device);
                            }
                            break;
                    }

                    if (current_click_tune_mode == ClickTuneMode.VFOA)
                    {
                        RenderVerticalLine(device, display_cursor_x, panadapter_H, grid_text_color);
                    }
                    else if (current_click_tune_mode == ClickTuneMode.VFOB)
                    {
                        RenderVerticalLine(device, display_cursor_x, panadapter_H, sub_rx_zero_line_color);
                    }

                    //End the scene
                    device.EndScene();
                    device.Present();
                }
                catch (Direct3D9Exception ex)
                {
                    Debug.Write(ex.ToString());

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                            "Render DirectX fault!\n" + ex.ToString());

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write("Error in RenderDirectX()\n" + ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Render DirectX fault!\n" + ex.ToString());

                if (DX_reinit)
                    return true;
                else
                    return false;
            }
        }

        private static void ConvertDataForPanadapter()  // changes yt7pwr
        {
            try
            {
                /*if (mox)
                    BUFFER_SIZE = 4096;
                else
                    BUFFER_SIZE = 32768;*/

                if (panadapterX_data == null || panadapterX_data.Length != panadapter_W)
                    panadapterX_data = new float[panadapter_W];    			        // array of points to display
                if (waterfall_data == null || waterfall_data.Length != panadapter_W)
                    waterfall_data = new float[panadapter_W];			                    // array of points to display

                int W = waterfall_W;
                float slope = 0.0f;				        	            	// samples to process per pixel
                UInt64 num_samples = 0;					                    // number of samples to process
                int start_sample_index = 0;			        	            // index to begin looking at samples
                int Low = rx_display_low;
                int High = rx_display_high;
                int yRange = spectrum_grid_max - spectrum_grid_min;

                max_y = Int32.MinValue;

                if (data_ready)
                {
                    // get new data
                    Array.Copy(new_display_data, current_display_data, current_display_data.Length);
                }

                DataReady = false;

                if (average_on && (Audio.CurrentAudioState1 == Audio.AudioState.SWITCH ||
                    Audio.NextAudioState1 == Audio.AudioState.SWITCH))
                {
                    average_buffer[0] = CLEAR_FLAG;
                    UpdateDisplayPanadapterAverage();
                }
                else if (average_on && (Audio.CurrentAudioState1 != Audio.AudioState.SWITCH ||
                    Audio.NextAudioState1 != Audio.AudioState.SWITCH))
                {
                    if (!UpdateDisplayPanadapterAverage())
                    {
                        average_buffer = null;
                        average_buffer = new float[BUFFER_SIZE];	// initialize averaging buffer array
                        average_buffer[0] = CLEAR_FLAG;		// set the clear flag   
                        Debug.Write("Reset display average buffer!");
                    }
                }

                if (peak_on)
                    UpdateDisplayPeak();

                start_sample_index = (BUFFER_SIZE >> 1) + (int)(((double)Low * (double)BUFFER_SIZE) / (double)sample_rate);
                num_samples = (UInt64)(((((double)High - (double)Low)/1e4) * BUFFER_SIZE) / ((double)sample_rate/1e4));
                if (start_sample_index < 0) start_sample_index += 32768;
                if (((int)num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                    num_samples = (uint)(BUFFER_SIZE - start_sample_index);
                slope = (float)num_samples / (float)panadapter_W;

                for (int i = 0; i < panadapter_W; i++)
                {
                    float max = float.MinValue;
                    float dval = i * slope + start_sample_index;
                    int lindex = (int)Math.Floor(dval);
                    int rindex = (int)Math.Floor(dval + slope);

                    if (slope <= 1.0 || lindex == rindex)
                    {
                        max = current_display_data[lindex % 32768] * ((float)lindex - dval + 1) + current_display_data[(lindex + 1) % 32768] * (dval - (float)lindex);
                    }
                    else
                    {
                        for (int j = lindex; j < rindex; j++)
                            if (current_display_data[j % 32768] > max) max = current_display_data[j % 32768];
                    }

                    max += display_cal_offset;

                    if (max > max_y)
                    {
                        max_y = max;
                        max_x = i;
                    }

                    double q = Math.Floor((spectrum_grid_max - max) * panadapter_H / yRange);
                    panadapter_verts[i].X = i;
                    panadapter_verts[i].Y = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * panadapter_H / yRange)), panadapter_H);
                    panadapterX_data[i] = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * panadapter_H / yRange)), panadapter_H);
                }

                panadapterX_data[0] = panadapterX_data[panadapter_W - 1];
                panadapterX_data[panadapter_W - 1] = panadapterX_data[panadapter_W - 2];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Convert data for Panadapter fault!\n" + ex.ToString());
            }
        }

        static private bool ConvertDataForHistogram()
        {
            try
            {
                if (points == null || points.Length < panadapter_W)
                    points = new Point[panadapter_W];			// array of points to display

                float slope = 0.0F;					        	// samples to process per pixel
                int num_samples = 0;	        				// number of samples to process
                int start_sample_index = 0;		        		// index to begin looking at samples
                int low = 0;
                int high = 0;
                max_y = Int32.MinValue;

                if (!mox)								        // Receive Mode
                {
                    low = rx_display_low;
                    high = rx_display_high;
                }
                else									        // Transmit Mode
                {
                    low = tx_display_low;
                    high = tx_display_high;
                }

                int yRange = spectrum_grid_max - spectrum_grid_min;

                if (data_ready)
                {
                    // get new data
                    Array.Copy(new_display_data, current_display_data, current_display_data.Length);
                    data_ready = false;
                }

                if (average_on)
                    UpdateDisplayPanadapterAverage();
                if (peak_on)
                    UpdateDisplayPeak();

                num_samples = (high - low);

                start_sample_index = (BUFFER_SIZE >> 1) + (int)((low * BUFFER_SIZE) / DttSP.RXSampleRate);
                num_samples = (int)((high - low) * BUFFER_SIZE / DttSP.RXSampleRate);
                if (start_sample_index < 0) start_sample_index = 0;
                if ((num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                    num_samples = BUFFER_SIZE - start_sample_index;

                slope = (float)num_samples / (float)panadapter_W;
                for (int i = 0; i < panadapter_W; i++)
                {
                    float max = float.MinValue;
                    float dval = i * slope + start_sample_index;
                    int lindex = (int)Math.Floor(dval);
                    if (slope <= 1)
                        max = current_display_data[lindex] * ((float)lindex - dval + 1) + current_display_data[lindex + 1] * (dval - (float)lindex);
                    else
                    {
                        int rindex = (int)Math.Floor(dval + slope);
                        if (rindex > BUFFER_SIZE) rindex = BUFFER_SIZE;
                        for (int j = lindex; j < rindex; j++)
                            if (current_display_data[j] > max) max = current_display_data[j];

                    }

                    max += display_cal_offset;

                    switch (current_dsp_mode)
                    {
                        case DSPMode.SPEC:
                            max += 6.0F;
                            break;
                    }
                    if (max > max_y)
                    {
                        max_y = max;
                        max_x = i;
                    }

                    points[i].X = i;
                    points[i].Y = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * panadapter_H / yRange)), panadapter_H);
                }

                // get the average
                float avg = 0.0F;
                int sum = 0;
                int k = 0;
                foreach (Point p in points)
                    sum += p.Y;

                avg = (float)((float)sum / points.Length / 1.12);

                for (int i = 0; i < panadapter_W; i++)
                {
                    if (points[i].Y < histogram_data[i])
                    {
                        histogram_history[i] = 0;
                        histogram_data[i] = points[i].Y;
                    }
                    else
                    {
                        histogram_history[i]++;
                        if (histogram_history[i] > 51)
                        {
                            histogram_history[i] = 0;
                            histogram_data[i] = points[i].Y;
                        }

                        int alpha = (int)Math.Max(255 - histogram_history[i] * 5, 0);
                        Color color = Color.FromArgb(alpha, 0, 255, 0);
                        int height = points[i].Y - histogram_data[i];
                        histogram_verts[i].Y = histogram_data[i];
                        histogram_verts[i].color = color;
                    }

                    if (points[i].Y >= avg)		// value is below the average
                    {
                        Color color = Color.FromArgb(150, 0, 0, 255);
                        histogram_verts[panadapter_W + k].Y = points[i].Y;
                        histogram_verts[panadapter_W + k].color = color;
                        histogram_verts[panadapter_W + k+1].Y = points[i].Y;
                        histogram_verts[panadapter_W + k+1].color = color;
                    }
                    else
                    {
                        Color color = Color.FromArgb(150, 0, 0, 255);
                        histogram_verts[panadapter_W + k].Y = points[i].Y;
                        histogram_verts[panadapter_W + k].color = color;
                        color = Color.FromArgb(255, 255, 0, 0);
                        histogram_verts[panadapter_W + k + 1].Y = (int)Math.Floor(avg);
                        histogram_verts[panadapter_W + k + 1].color = color;
                    }

                    k += 2;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        unsafe static private void ConvertDataForSpectrum()
        {
            try
            {
                float slope = 0.0f;						// samples to process per pixel
                UInt64 num_samples = 0;					// number of samples to process
                int start_sample_index = 0;				// index to begin looking at samples
                int low = 0;
                int high = 0;
                max_y = Int32.MinValue;

                if (!console.MOX)
                {
                    low = rx_display_low;
                    high = rx_display_high;
                }
                else
                {
                    low = tx_display_low;
                    high = tx_display_high;
                }

                int yRange = spectrum_grid_max - spectrum_grid_min;

                if (data_ready)
                {
                    // get new data
                    fixed (void* rptr = &new_display_data[0])
                    fixed (void* wptr = &current_display_data[0])
                        Win32.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));

                    data_ready = false;
                }

                if (average_on)
                    UpdateDisplayPanadapterAverage();
                if (peak_on)
                    UpdateDisplayPeak();


                start_sample_index = (BUFFER_SIZE >> 1) + (int)(((double)low * (double)BUFFER_SIZE) / (double)sample_rate);
                num_samples = (UInt64)(((((double)high - (double)low) / 1e4) * BUFFER_SIZE) / ((double)sample_rate / 1e4));
                if (start_sample_index < 0) start_sample_index += 32768;
                if (((int)num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                    num_samples = (uint)(BUFFER_SIZE - start_sample_index);
                slope = (float)num_samples / (float)panadapter_W;

                for (int i = 0; i < panadapter_W; i++)
                {
                    float max = float.MinValue;
                    float dval = i * slope + start_sample_index;
                    int lindex = (int)Math.Floor(dval);
                    int rindex = (int)Math.Floor(dval + slope);

                    if (slope <= 1.0 || lindex == rindex)
                    {
                        max = current_display_data[lindex % 32768] * ((float)lindex - dval + 1) +
                            current_display_data[(lindex + 1) % 32768] * (dval - (float)lindex);
                    }
                    else
                    {
                        for (int j = lindex; j < rindex; j++)
                            if (current_display_data[j % 32768] > max) max = current_display_data[j % 32768];
                    }

                    max += display_cal_offset;

                    if (max > max_y)
                    {
                        max_y = max;
                        max_x = i;
                    }

                    panadapterX_data[i] = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * panadapter_H / yRange)),
                        panadapter_H);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static void ConvertDataForPhase()
        {
            try
            {
                int num_points = phase_num_pts;

                if (data_ready)
                {
                    // get new data
                    Array.Copy(new_display_data, current_display_data, current_display_data.Length);
                    data_ready = false;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static void ConvertDataForScope(int scope_W, int scope_H)  // changes yt7pwr
        {
            try
            {
                if (!console.booting)
                {
                    int i;

                    if (scope_min.Length != waterfall_target.Width)
                    {
                        if (scope_min != null)
                            scope_min = null;

                        scope_min = new float[waterfall_target.Width];
                    }

                    if (scope_max.Length != waterfall_target.Width)
                    {
                        if (scope_max != null)
                            scope_max = null;

                        scope_max = new float[waterfall_target.Width];
                    }

                    for (i = 0; i < scope_W * 2; i++)
                    {
                        int pixel = 0;
                        pixel = (int)(scope_H / 2 * Audio.scope_max[i / 2]);
                        int y = scope_H / 2 - pixel;
                        panadapterX_scope_data[i] = y;

                        pixel = (int)(scope_H / 2 * Audio.scope_min[i / 2]);
                        y = scope_H / 2 - pixel;
                        panadapterX_scope_data[i + 1] = y;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Convert data for scope error!\n" + ex.ToString());
            }
        }

        private static void RenderScopeGrid()  // changes yt7pwr
        {
            try
            {
                switch (current_display_mode)
                {
                    case DisplayMode.SCOPE:
                        {
                            // Add horizontal line
                            if (show_horizontal_grid)
                                RenderHorizontalLine(device, 0, panadapter_H / 2, grid_color);

                            // Add vertical line
                            if (show_vertical_grid)
                                RenderVerticalLine(device, panadapter_W / 2, panadapter_H, grid_color);
                        }
                        break;

                    case DisplayMode.PANASCOPE:
                        {
                            // Add horizontal line
                            if (show_horizontal_grid)
                                RenderHorizontalLine(waterfall_dx_device, 0, waterfall_H / 2, grid_color);

                            // Add vertical line
                            if (show_vertical_grid)
                                RenderVerticalLine(waterfall_dx_device, waterfall_W / 2, waterfall_H, grid_color);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "Rendering Scope Grid error!\n" + ex.ToString());
            }
        }

        private static void RenderSpectrum()  // changes yt7pwr
        {
            System.Drawing.Font font = new System.Drawing.Font("Arial", 9);
            //device.VertexFormat = VertexFormat.None;

            int low = 0;								// init limit variables
            int high = 0;

            if (!console.MOX)
            {
                low = rx_display_low;				// get RX display limits
                high = rx_display_high;
            }
            else
            {
                low = tx_display_low;				// get TX display limits
                high = tx_display_high;
            }

            int mid_w = panadapter_W / 2;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 50;

            int y_range = spectrum_grid_max - spectrum_grid_min;

            if (high == 0)
            {
                int f = -low;
                // Calculate horizontal step size
                while (f / freq_step_size > 7)
                {
                    freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                    step_index = (step_index + 1) % 4;
                    if (step_index == 0) step_power++;
                }
                float pixel_step_size = (float)(panadapter_W * freq_step_size / f);

                int num_steps = f / freq_step_size;

                // Draw vertical lines
                for (int i = 1; i <= num_steps; i++)
                {
                    int x = panadapter_W - (int)Math.Floor(i * pixel_step_size);	// for negative numbers

                    RenderVerticalLine(device, x, panadapter_H, grid_color);

                    // Draw vertical line labels
                    int num = i * freq_step_size;
                    string label = num.ToString();
                    int offset = (int)((label.Length + 1) * 4.1);
                    if (x - offset >= 0)
                    {
                        panadapter_font.DrawString(null, label, x - offset, 0, grid_text_color.ToArgb());
                    }
                }

                // Draw horizontal lines
                int V = (int)(spectrum_grid_max - spectrum_grid_min);
                num_steps = V / spectrum_grid_step;
                pixel_step_size = panadapter_H / num_steps;

                for (int i = 1; i < num_steps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)Math.Floor((double)(spectrum_grid_max - num) * panadapter_H / y_range);

                    RenderHorizontalLine(device, 0, y, grid_color);

                    // Draw horizontal line labels
                    string label = num.ToString();
                    int offset = (int)(label.Length * 4.1);
                    if (label.Length == 3) xOffset = 7;
                    float size = pan_font.Size * 3;
                    y -= 8;
                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                            x = panadapter_W / 2 + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                        case DisplayLabelAlignment.AUTO:
                            x = (int)(panadapter_W - size);
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = panadapter_W;
                            break;
                    }

                    if (y + 9 < panadapter_H)
                    {
                        panadapter_font.DrawString(null, label, x, y, grid_text_color.ToArgb());
                    }
                }
            }
            else if (low == 0)
            {
                int f = high;
                // Calculate horizontal step size
                while (f / freq_step_size > 7)
                {
                    freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                    step_index = (step_index + 1) % 4;
                    if (step_index == 0) step_power++;
                }
                float pixel_step_size = (float)(panadapter_W * freq_step_size / f);
                int num_steps = f / freq_step_size;

                // Draw vertical lines
                for (int i = 1; i <= num_steps; i++)
                {
                    int x = (int)Math.Floor(i * pixel_step_size);// for positive numbers

                    RenderVerticalLine(device, x, panadapter_H, grid_color);

                    // Draw vertical line labels
                    int num = i * freq_step_size;
                    string label = num.ToString();
                    int offset = (int)(label.Length * 4.1);
                    if (x - offset + label.Length * 7 < panadapter_W)
                    {
                        panadapter_font.DrawString(null, label, x - offset, 0, grid_text_color.ToArgb());
                    }
                }

                // Draw horizontal lines
                int V = (int)(spectrum_grid_max - spectrum_grid_min);
                int numSteps = V / spectrum_grid_step;
                pixel_step_size = panadapter_H / numSteps;
                for (int i = 1; i < numSteps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)Math.Floor((double)(spectrum_grid_max - num) * panadapter_H / y_range);

                    RenderHorizontalLine(device, 0, y, grid_color);

                    // Draw horizontal line labels
                    string label = num.ToString();
                    if (label.Length == 3) xOffset = 7;
                    float size = pan_font.Size * 3;

                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                        case DisplayLabelAlignment.AUTO:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                            x = panadapter_W / 2 + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                            x = (int)(panadapter_W - size);
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = panadapter_W;
                            break;
                    }

                    y -= 8;
                    if (y + 9 < panadapter_H)
                    {
                        panadapter_font.DrawString(null, label, x, y, grid_text_color.ToArgb());
                    }
                }
            }
            if (low < 0 && high > 0)
            {
                int f = high;

                // Calculate horizontal step size
                while (f / freq_step_size > 4)
                {
                    freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                    step_index = (step_index + 1) % 4;
                    if (step_index == 0) step_power++;
                }
                int pixel_step_size = panadapter_W / 2 * freq_step_size / f;
                int num_steps = f / freq_step_size;

                // Draw vertical lines
                for (int i = 1; i <= num_steps; i++)
                {
                    int xLeft = mid_w - (i * pixel_step_size);			// for negative numbers
                    int xRight = mid_w + (i * pixel_step_size);		// for positive numbers
                    RenderVerticalLine(device, xLeft, panadapter_H, grid_color);
                    RenderVerticalLine(device, xRight, panadapter_H, grid_color);

                    // Draw vertical line labels
                    int num = i * freq_step_size;
                    string label = num.ToString();
                    int offsetL = (int)((label.Length + 1) * 4.1);
                    int offsetR = (int)(label.Length * 4.1);
                    if (xLeft - offsetL >= 0)
                    {
                        panadapter_font.DrawString(null, "-" + label, xLeft - offsetL, 0, grid_text_color.ToArgb());
                        panadapter_font.DrawString(null, "-" + label, xRight - offsetR, 0, grid_text_color.ToArgb());
                    }
                }

                // Draw horizontal lines
                int V = (int)(spectrum_grid_max - spectrum_grid_min);
                int numSteps = V / spectrum_grid_step;
                pixel_step_size = panadapter_H / numSteps;
                for (int i = 1; i < numSteps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)Math.Floor((double)(spectrum_grid_max - num) * panadapter_H / y_range);
                    //g.DrawLine(grid_pen, 0, y, W, y);
                    RenderHorizontalLine(device, 0, y, grid_color);

                    // Draw horizontal line labels
                    string label = num.ToString();
                    if (label.Length == 3) xOffset = 7;
                    int offset = (int)(label.Length * 4.1);
                    float size = pan_font.Size * 3;

                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                        case DisplayLabelAlignment.AUTO:
                            x = panadapter_W / 2 + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                            x = (int)(panadapter_W - size);
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = panadapter_W;
                            break;
                    }

                    y -= 8;
                    if (y + 9 < panadapter_H)
                    {
                        panadapter_font.DrawString(null, label, x, y, grid_text_color.ToArgb());
                    }
                }
            }

            if (console.HighSWR)
            {
                panadapter_font.DrawString(null, "High SWR!",245, 20, Color.Red.ToArgb());
            }
        }

        private static bool UpdateDisplayPanadapterAverage()
        {
            try
            {
                double dttsp_osc = (losc_hz - vfoa_hz) * 1e6;
                if ((Display_DirectX.average_buffer[0] == Display_DirectX.CLEAR_FLAG) ||
                    float.IsNaN(Display_DirectX.average_buffer[0]))
                {
                    //Debug.WriteLine("Clearing average buf"); 
                    for (int i = 0; i < Display_DirectX.BUFFER_SIZE; i++)
                        Display_DirectX.average_buffer[i] = Display_DirectX.current_display_data[i];
                    return true;
                }

                float new_mult = 0.0f;
                float old_mult = 0.0f;
                new_mult = Display_DirectX.display_avg_mult_new;
                old_mult = Display_DirectX.display_avg_mult_old;

                for (int i = 0; i < Display_DirectX.BUFFER_SIZE; i++)
                    Display_DirectX.average_buffer[i] = Display_DirectX.current_display_data[i] =
                        (float)(Display_DirectX.current_display_data[i] * new_mult +
                        Display_DirectX.average_buffer[i] * old_mult);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }       

        #endregion
    }
}
#endif