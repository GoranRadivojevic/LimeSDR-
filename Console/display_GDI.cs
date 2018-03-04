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
 *  Copyright (C)2008-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace PowerSDR
{

    public enum RenderType
    {
        HARDWARE = 0,
        SOFTWARE,
        NONE,
    }

    static class Display_GDI
    {
        #region Variable Declaration
        private static double avg_last_ddsfreq = 0;				// Used to move the display average when tuning
        private static double avg_last_dttsp_osc = 0;
        public static Console console;
        //private static Mutex background_image_mutex;			// used to lock the base display image
        //private static Bitmap background_bmp;					// saved background picture for display
        public static Bitmap panadapter_bmp;  				    // Bitmap for use when drawing
        //private static int waterfall_counter;
        private static Bitmap waterfall_bmp;					// saved waterfall picture for display
        //private static Graphics display_graphics;				// GDI graphics object
        private static int[] histogram_data;					// histogram display buffer
        private static int[] histogram_history;					// histogram counter
        public const float CLEAR_FLAG = -999.999F;				// for resetting buffers
        public static int BUFFER_SIZE = 32768;
        public static float[] new_display_data;					// Buffer used to store the new data from the DSP for the display
        public static float[] new_scope_data;					// Buffer used to store the new data from the DSP for the scope
        public static float[] new_waterfall_data;    			// Buffer used to store the new data from the DSP for the waterfall
        public static float[] current_display_data;				// Buffer used to store the current data for the display
        public static float[] current_scope_data;   		    // Buffer used to store the current data for the scope
        public static float[] current_waterfall_data;		    // Buffer used to store the current data for the waterfall
        public static float[] waterfall_display_data;            // Buffer for waterfall

        public static float[] average_buffer;					// Averaged display data buffer for Panadapter
        public static float[] average_waterfall_buffer;  		// Averaged display data buffer for Waterfall
        public static float[] peak_buffer;						// Peak hold display data buffer
        public static Mutex display_data_mutex = new Mutex();

        public static int server_W = 1024;                       // for Server screen width
        public static int client_W = 1024;                       // for Client screen width
        public static byte[] server_display_data;
        public static byte[] client_display_data;
        private static System.Drawing.Font swr_font = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
        public static string panadapter_img = "";

        #endregion

        #region Properties

        private static ColorSheme color_sheme = ColorSheme.original;        // yt7pwr
        public static ColorSheme ColorSheme
        {
            get { return color_sheme; }

            set { color_sheme = value; }
        }


        private static bool reverse_waterfall = false;
        public static bool ReverseWaterfall
        {
            get { return reverse_waterfall; }
            set { reverse_waterfall = value; }
        }

        public static bool smooth_line = false;
        public static bool pan_fill = false;

        private static Font panadapter_font = new System.Drawing.Font("Arial", 9);
        public static Font PanadapterFont
        {
            get { return panadapter_font; }
            set { panadapter_font = value; }
        }

        private static Color pan_fill_color = Color.FromArgb(100, 0, 0, 127);
        public static Color PanFillColor
        {
            get { return pan_fill_color; }
            set { pan_fill_color = value; }
        }

        private static Color scope_color = Color.FromArgb(100, 0, 0, 127);
        public static Color ScopeColor
        {
            get { return scope_color; }
            set { scope_color = value; }
        }

        private static Color display_text_background = Color.FromArgb(127, 0, 0, 0);
        public static Color DisplayTextBackground
        {
            get { return display_text_background; }
            set { display_text_background = value; }
        }

        private static Color display_filter_color = Color.FromArgb(65, 0, 255, 0);
        public static Color DisplayFilterColor
        {
            get { return display_filter_color; }
            set
            {
                display_filter_color = value;
            }
        }

        private static bool show_horizontal_grid = false;
        public static bool Show_Horizontal_Grid
        {
            get { return show_horizontal_grid; }
            set { show_horizontal_grid = value; }
        }

        private static bool show_vertical_grid = false;
        public static bool Show_Vertical_Grid
        {
            get { return show_vertical_grid; }
            set { show_vertical_grid = value; }
        }

        private static int phase_num_pts = 100;
        public static int PhaseNumPts
        {
            get { return phase_num_pts; }
            set { phase_num_pts = value; }
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

        private static Color sub_rx_zero_line_color = Color.Red;
        public static Color SubRXZeroLine
        {
            get { return sub_rx_zero_line_color; }
            set
            {
                sub_rx_zero_line_color = value;
            }
        }

        private static Color main_rx_filter_color = Color.FromArgb(127, 0, 128, 128);
        public static Color MainRXFilterColor
        {
            get { return main_rx_filter_color; }
            set
            {
                main_rx_filter_color = value;
            }
        }

        private static Color sub_rx_filter_color = Color.FromArgb(127, 0, 0, 255);  // blue
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

        private static int H = 0;	// target height
        private static int W = 0;	// target width
        private static Control target = null;
        public static Control Target
        {
            get { return target; }
            set
            {
                target = value;
                H = target.Height;
                W = target.Width;
                Audio.ScopeDisplayWidth = W;
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

                if (!peak_on)
                    ResetDisplayPeak();
            }
        }

        public static bool scope_data_ready = false;
        private static bool data_ready;					// True when there is new display data ready from the DSP
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
            }
        }

        private static int spectrum_grid_min = -150;
        public static int SpectrumGridMin
        {
            get { return spectrum_grid_min; }
            set
            {
                spectrum_grid_min = value;
            }
        }

        private static int spectrum_grid_step = 10;
        public static int SpectrumGridStep
        {
            get { return spectrum_grid_step; }
            set
            {
                spectrum_grid_step = value;
            }
        }

        private static Color grid_text_color = Color.Yellow;
        public static Color GridTextColor
        {
            get { return grid_text_color; }
            set
            {
                grid_text_color = value;
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

        #region General Routines

        public static void Init()               // changes yt7pwr
        {
            try
            {
                histogram_data = new int[W];
                histogram_history = new int[W];
                for (int i = 0; i < W; i++)
                {
                    histogram_data[i] = Int32.MaxValue;
                    histogram_history[i] = 0;
                }

                if (waterfall_bmp != null)
                    waterfall_bmp.Dispose();

                if (panadapter_bmp != null)
                    panadapter_bmp.Dispose();

                if (panadapter_img != "")
                {
                    try
                    {
                        panadapter_bmp = new Bitmap(System.Drawing.Image.FromFile(panadapter_img, true));	// initialize panadapter display
                    }
                    catch (Exception ex)
                    {
                        panadapter_bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);	                    // initialize panadapter display
                        Debug.Write(ex.ToString());
                    }
                }
                else
                    panadapter_bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);	                    // initialize paterfall display

                waterfall_bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);	                        // initialize waterfall display
                average_buffer = new float[BUFFER_SIZE];	                                            // initialize averaging buffer array
                average_buffer[0] = CLEAR_FLAG;		                                                    // set the clear flag
                average_waterfall_buffer = new float[BUFFER_SIZE];	                                    // initialize averaging buffer array
                average_waterfall_buffer[0] = CLEAR_FLAG;		                                        // set the clear flag
                peak_buffer = new float[BUFFER_SIZE];
                peak_buffer[0] = CLEAR_FLAG;
                scope_min = new float[W];
                scope_max = new float[W];
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

                if (display_data_mutex == null)
                    display_data_mutex = new Mutex();

                server_display_data = new byte[server_W];
                client_display_data = new byte[client_W];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void Close() // yt7pwr
        {
            try
            {
                histogram_data = null;
                histogram_history = null;

                if (waterfall_bmp != null)
                    waterfall_bmp.Dispose();

                average_buffer = null;
                average_waterfall_buffer = null;
                peak_buffer = null;
                scope_min = null;
                scope_max = null;
                new_display_data = null;
                new_scope_data = null;
                new_waterfall_data = null;
                current_display_data = null;
                current_scope_data = null;
                current_waterfall_data = null;
                waterfall_display_data = null;
                if (display_data_mutex != null)
                    display_data_mutex = null;
                server_display_data = null;
                client_display_data = null;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region GDI+

        unsafe public static bool RenderWaterfall(ref PaintEventArgs e)
        {
            try
            {
                bool result = false;

                switch (current_display_mode)
                {
                    case DisplayMode.PANASCOPE:
                        result = DrawScope(e.Graphics, W, H);
                        break;
                    case DisplayMode.PANAFALL:
                    case DisplayMode.PANAFALL_INV:
                    case DisplayMode.WATERFALL:
                        result = DrawWaterfall(e.Graphics, W, H);
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        unsafe public static bool RenderGDIPlus(ref PaintEventArgs e)
        {
            try
            {
                bool result = true;

                switch (current_display_mode)
                {
                    case DisplayMode.PANAFALL:
                    case DisplayMode.PANAFALL_INV:
                        result = DrawPanadapter(e.Graphics, W, H);
                        break;
                    case DisplayMode.PANADAPTER:
                        result = DrawPanadapter(e.Graphics, W, H);
                        break;
                    case DisplayMode.SPECTRUM:
                        result = DrawSpectrum(e.Graphics, W, H);
                        break;
                    case DisplayMode.SCOPE:
                        result = DrawScope(e.Graphics, W, H);
                        break;
                    case DisplayMode.PHASE:
                        result = DrawPhase(e.Graphics, W, H);
                        break;
                    case DisplayMode.PHASE2:
                        DrawPhase2(e.Graphics, W, H);
                        break;
                    case DisplayMode.HISTOGRAM:
                        result = DrawHistogram(e.Graphics, W, H);
                        break;
                    case DisplayMode.PANASCOPE:
                        result = DrawPanadapter(e.Graphics, W, H);
                        break;
                    case DisplayMode.OFF:
                        break;
                    default:
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        private static void UpdateDisplayPeak()
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

        #endregion

        #region Drawing Routines

        public static int center_line_x = 415;
        public static int filter_left_x = 150;
        public static int filter_right_x = 2550;

        private static void DrawPanadapterGrid(ref Graphics g, int W, int H)  // changes yt7pwr
        {
            // draw background
            if (!Display_GDI.console.SkinsEnabled)
                g.FillRectangle(new SolidBrush(display_background_color), 0, 0, W, H);

            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int mid_w = W / 2;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 50;
            int inbetweenies = 4;
            int grid_step = spectrum_grid_step;

            SolidBrush grid_text_brush = new SolidBrush(grid_text_color);
            Pen grid_pen = new Pen(grid_color);
            Pen grid_pen_dark = new Pen(Color.FromArgb(Math.Max(grid_color.A - 40, 0), grid_color));
            grid_pen_dark.DashStyle = DashStyle.DashDot;
            Pen tx_filter_pen = new Pen(display_filter_tx_color);
            int y_range = spectrum_grid_max - spectrum_grid_min;
            int filter_low = 0, filter_high = 0;
            int filter_low_subRX = 0;
            int filter_high_subRX = 0;
            int notch_low = 0;
            int notch_high = 0;
            double[] BandEdges = new double[100];
            int i;

            center_line_x = W / 2;

            if (mox && !split_enabled && !(current_dsp_mode == DSPMode.CWL ||
                current_dsp_mode == DSPMode.CWU)) // get filter limits
            {
                filter_low = DttSP.TXFilterLowCut;
                filter_high = DttSP.TXFilterHighCut;
            }
            else if (mox && split_enabled && !(current_dsp_mode_subRX == DSPMode.CWL ||
                current_dsp_mode_subRX == DSPMode.CWU)) // get filter limits
            {
                filter_low_subRX = DttSP.RXFilterLowCutSubRX;
                filter_high_subRX = DttSP.RXFilterHighCutSubRX;
            }
            else
            {
                filter_low = DttSP.RXFilterLowCut;
                filter_high = DttSP.RXFilterHighCut;
                filter_low_subRX = DttSP.RXFilterLowCutSubRX;
                filter_high_subRX = DttSP.RXFilterHighCutSubRX;
                notch_low = rx_display_notch_low_cut;
                notch_high = rx_display_notch_high_cut;
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
            double h_pixel_step = (double)H / h_steps;
            int top = (int)((double)grid_step * H / y_range);

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
                vfo = losc_hz; //  +rit_hz;
            }

            long vfo_round = ((long)(vfo / freq_step_size)) * freq_step_size;
            long vfo_delta = (long)(vfo - vfo_round);

            // Draw vertical lines
            int vert_num;
            vert_num = 10;

            for (i = 0; i <= vert_num; i++)
            {
                string label;
                int offsetL;

                int fgrid = i * freq_step_size + (low / freq_step_size) * freq_step_size;
                double actual_fgrid = ((double)(vfo_round + fgrid)) / 1000000;
                int vgrid = (int)((double)(fgrid - vfo_delta - low) / (high - low) * W);

                DB.GetBandLimitsEdges(ref BandEdges);

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
                    g.DrawLine(new Pen(band_edge_color), vgrid, 2 * panadapter_font.Size, vgrid, H);

                    label = actual_fgrid.ToString("f3");
                    if (actual_fgrid < 10) offsetL = (int)((label.Length + 1) * 4.1) - 14;
                    else if (actual_fgrid < 100.0) offsetL = (int)((label.Length + 1) * 4.1) - 11;
                    else offsetL = (int)((label.Length + 1) * 4.1) - 8;

                    g.DrawString(label, panadapter_font, new SolidBrush(band_edge_color),
                        vgrid - offsetL, (float)Math.Floor(H * .01));

                    if (show_vertical_grid)
                    {
                        int fgrid_2 = ((i + 1) * freq_step_size) + (int)((low / freq_step_size) * freq_step_size);
                        int x_2 = (int)(((float)(fgrid_2 - vfo_delta - low) / width * W));
                        float scale = (float)(x_2 - vgrid) / inbetweenies;

                        for (int j = 1; j < inbetweenies; j++)
                        {
                            float x3 = (float)vgrid + (j * scale);
                            g.DrawLine(grid_pen_dark, x3, 2 * panadapter_font.Size, x3, H);
                        }
                    }
                }
                else
                {
                    if (show_vertical_grid)
                    {
                        g.DrawLine(grid_pen, vgrid, 2 * panadapter_font.Size, vgrid, H);			//wa6ahl

                        int fgrid_2 = ((i + 1) * freq_step_size) + (int)((low / freq_step_size) * freq_step_size);
                        int x_2 = (int)(((float)(fgrid_2 - vfo_delta - low) / width * W));
                        float scale = (float)(x_2 - vgrid) / inbetweenies;

                        for (int j = 1; j < inbetweenies; j++)
                        {
                            float x3 = (float)vgrid + (j * scale);
                            g.DrawLine(grid_pen_dark, x3, 2 * panadapter_font.Size, x3, H);
                        }
                    }

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

                    g.DrawString(label, panadapter_font, grid_text_brush, vgrid - offsetL, (float)Math.Floor(H * .01));
                }
            }

            //int[] band_edge_list = { 135700, 137800, 415000, 525000, 10150000, 14350000, 18068000, 18168000, 24880000, 24990000 };

            foreach (double b_edge in BandEdges)
            //for (i = 0; i < band_edge_list.Length; i++)
            {
                double band_edge_offset = b_edge * 1e6 - losc_hz;
                if (band_edge_offset >= low && band_edge_offset <= high)
                {
                    int temp_vline = (int)((double)(band_edge_offset - low) / (high - low) * W);//wa6ahl
                    g.DrawLine(new Pen(band_edge_color), temp_vline, 2 * panadapter_font.Size, temp_vline, H);//wa6ahl
                }
                //                    if (i == 1 && !show_freq_offset) break;
            }

            // Draw horizontal lines
            for (i = 1; i < h_steps; i++)
            {
                int xOffset = 0;
                int num = spectrum_grid_max - i * spectrum_grid_step;
                int y = (int)((double)(spectrum_grid_max - num) * H / y_range);
                if (show_horizontal_grid)
                    g.DrawLine(grid_pen, 0, y, W, y);

                // Draw horizontal line labels
                if (i != 1) // avoid intersecting vertical and horizontal labels
                {
                    num = spectrum_grid_max - i * spectrum_grid_step;
                    string label = num.ToString();
                    if (label.Length == 3) xOffset = 7;
                    //int offset = (int)(label.Length*4.1);
                    if (display_label_align != DisplayLabelAlignment.LEFT &&
                        display_label_align != DisplayLabelAlignment.AUTO &&
                        (current_dsp_mode == DSPMode.USB ||
                        current_dsp_mode == DSPMode.CWU))
                        xOffset -= 32;
                    SizeF size = g.MeasureString(label, panadapter_font);

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
                            x = (int)(W - size.Width);
                            break;
                        case DisplayLabelAlignment.AUTO:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = W;
                            break;
                    }
                    y -= 8;
                    if (y + 9 < H)
                        g.DrawString(label, panadapter_font, grid_text_brush, x, y);
                }
            }

            if (sub_rx_enabled && !mox)
            {
                if (current_dsp_mode_subRX == DSPMode.CWL || current_dsp_mode_subRX == DSPMode.CWU)
                {
                    // draw Sub RX filter
                    // get filter screen coordinates
                    int filter_left_x = (int)((float)(-low - ((filter_high_subRX - filter_low_subRX) / 2)
                        + vfob_hz + rit_hz - losc_hz) / (high - low) * W);
                    int filter_right_x = (int)((float)(-low + ((filter_high_subRX - filter_low_subRX) / 2)
                        + vfob_hz + rit_hz - losc_hz) / (high - low) * W);

                    // make the filter display at least one pixel wide.
                    if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                    // draw rx filter
                    g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw filter overlay
                        filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                }
                else
                {
                    // draw Sub RX filter
                    // get filter screen coordinates
                    int filter_left_x = (int)((float)(filter_low_subRX - low + vfob_hz + rit_hz - losc_hz) / (high - low) * W);
                    int filter_right_x = (int)((float)(filter_high_subRX - low + vfob_hz + rit_hz - losc_hz) / (high - low) * W);

                    // make the filter display at least one pixel wide.
                    if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                    // draw rx filter
                    g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw filter overlay
                        filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);

                    // draw Sub RX 0Hz line
                    int x = (int)((float)(vfob_hz - rit_hz - losc_hz - low) / (high - low) * W);
                    g.DrawLine(new Pen(sub_rx_zero_line_color), x, 2 * panadapter_font.Size, x, H);
                    g.DrawLine(new Pen(sub_rx_zero_line_color), x - 1, 2 * panadapter_font.Size, x - 1, H);
                }
            }

            if (mox)
            {
                if (!(current_dsp_mode == DSPMode.CWL || current_dsp_mode == DSPMode.CWU) ||
                    !(current_dsp_mode_subRX == DSPMode.CWU || current_dsp_mode_subRX == DSPMode.CWL))
                {
                    // get filter screen coordinates
                    if (split_enabled)
                    {
                        int filter_left_x = (int)((float)(filter_low_subRX - low) / (high - low) * W);
                        int filter_right_x = (int)((float)(filter_high_subRX - low) / (high - low) * W);
                        // make the filter display at least one pixel wide.
                        if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;
                        // draw rx filter
                        g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw subRX filter
                            filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                        // draw Sub RX 0Hz line
                        int x = (int)((float)(-low) / (high - low) * W);
                        g.DrawLine(new Pen(sub_rx_zero_line_color), x, 2 * panadapter_font.Size, x, H);
                        g.DrawLine(new Pen(sub_rx_zero_line_color), x - 1, 2 * panadapter_font.Size, x - 1, H);
                    }
                    else
                    {
                        filter_left_x = (int)((float)(filter_low - low) / (high - low) * W);
                        filter_right_x = (int)((float)(filter_high - low) / (high - low) * W);
                        // make the filter display at least one pixel wide.
                        if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;
                        // draw rx filter
                        g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                            filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                        // draw Main RX 0Hz line
                        int x = (int)((float)(-low) / (high - low) * W);
                        g.DrawLine(new Pen(main_rx_zero_line_color), x, 2 * panadapter_font.Size, x, H);
                        g.DrawLine(new Pen(main_rx_zero_line_color), x - 1, 2 * panadapter_font.Size, x - 1, H);
                    }
                }
                else
                {
                    // get filter screen coordinates
                    if (split_enabled)
                    {
                        int filter_left_x = (int)((float)(-low - ((filter_high - filter_low) / 2) + xit_hz) / (high - low) * W);
                        int filter_right_x = (int)((float)(-low + ((filter_high - filter_low) / 2) + xit_hz) / (high - low) * W);
                        if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;
                        // draw rx filter
                        g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw filter overlay
                            filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                    }
                    else
                    {
                        filter_left_x = (int)((float)(-low - ((filter_high - filter_low) / 2) + xit_hz) / (high - low) * W);
                        filter_right_x = (int)((float)(-low + ((filter_high - filter_low) / 2) + xit_hz) / (high - low) * W);
                        // make the filter display at least one pixel wide.
                        if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;
                        // draw rx filter
                        g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                            filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                    }
                }
            }
            else
            {
                switch (current_dsp_mode)
                {
                    case DSPMode.CWU:
                    case DSPMode.CWL:
                        {
                            // get filter screen coordinates
                            int filter_left_x = (int)((float)(-low - ((filter_high - filter_low) / 2) + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                            int filter_right_x = (int)((float)(-low + ((filter_high - filter_low) / 2) + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);

                            // make the filter display at least one pixel wide.
                            if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                            // draw rx filter
                            g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                                filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                        }
                        break;

                    case DSPMode.DRM:
                        {
                            // get filter screen coordinates
                            int filter_left_x = (int)((float)(filter_low - filter_high / 2 - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                            int filter_right_x = (int)((float)(filter_high / 2 - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);

                            // draw rx filter
                            g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                                filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);

                            // draw Main RX 0Hz line
                            int x = (int)((float)(vfoa_hz + rit_hz - losc_hz - low) / (high - low) * W);
                            g.DrawLine(new Pen(main_rx_zero_line_color), x, 2 * panadapter_font.Size, x, H);
                            g.DrawLine(new Pen(main_rx_zero_line_color), x - 1, 2 * panadapter_font.Size, x - 1, H);
                        }
                        break;

                    default:
                        {
                            // get filter screen coordinates
                            int filter_left_x = (int)((float)(filter_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                            int filter_right_x = (int)((float)(filter_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);

                            // make the filter display at least one pixel wide.
                            if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

                            // draw rx filter
                            g.FillRectangle(new SolidBrush(main_rx_filter_color),	// draw filter overlay
                                filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);

                            // draw Main RX 0Hz line
                            int x = (int)((float)(vfoa_hz + rit_hz - losc_hz - low) / (high - low) * W);
                            g.DrawLine(new Pen(main_rx_zero_line_color), x, 2 * panadapter_font.Size, x, H);
                            g.DrawLine(new Pen(main_rx_zero_line_color), x - 1, 2 * panadapter_font.Size, x - 1, H);

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
                                                filter_left_x = (int)((float)(-notch_high - low + vfoa_hz - (filter_low + filter_high) / 2 + rit_hz - losc_hz) / (high - low) * W);
                                                filter_right_x = (int)((float)(-notch_low - low + vfoa_hz - (filter_low + filter_high) / 2 + rit_hz - losc_hz) / (high - low) * W);
                                            }
                                            else if ((notch_high - notch_low) < (filter_high - filter_low))
                                            {
                                                filter_left_x = (int)((float)(notch_low - low + vfoa_hz - filter_high / 2 + rit_hz - losc_hz) / (high - low) * W);
                                                filter_right_x = (int)((float)(notch_high - low + vfoa_hz - filter_high / 2 + rit_hz - losc_hz) / (high - low) * W);
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            // get filter screen coordinates
                                            if (filter_high < 0 && filter_low < 0)
                                            {
                                                filter_left_x = (int)((float)(-notch_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                                                filter_right_x = (int)((float)(-notch_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                                            }
                                            else
                                            {
                                                filter_left_x = (int)((float)(notch_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                                                filter_right_x = (int)((float)(notch_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * W);
                                            }
                                        }
                                        break;
                                }

                                // draw notch
                                g.FillRectangle(new SolidBrush(Color.FromArgb(main_rx_filter_color.A / 2, main_rx_filter_color.R / 2,
                                main_rx_filter_color.G / 2, main_rx_filter_color.B / 2)),	// draw filter overlay
                                    filter_left_x, 2 * panadapter_font.Size, filter_right_x - filter_left_x, H);
                            }
                        }
                        break;
                }
            }

            if (!mox && draw_tx_cw_freq &&
                (current_dsp_mode == DSPMode.CWL || current_dsp_mode == DSPMode.CWU))
            {
                int cw_line_x;
                if (!split_enabled)
                    cw_line_x = (int)((float)(-low + vfoa_hz - losc_hz + xit_hz) / (high - low) * W);
                else
                    cw_line_x = (int)((float)(-low + xit_hz + vfob_hz - losc_hz) / (high - low) * W);

                g.DrawLine(tx_filter_pen, cw_line_x, 2 * panadapter_font.Size, cw_line_x, H);
                g.DrawLine(tx_filter_pen, cw_line_x + 1, 2 * panadapter_font.Size, cw_line_x + 1, H);
            }

            g.FillRectangle(new SolidBrush(display_text_background), 0,
                    0, W, 2 * panadapter_font.Size);

            if (high_swr)
                g.DrawString("High SWR", swr_font, new SolidBrush(Color.Red), 245, 20);
        }

        private static PointF[] points;

        unsafe static private bool DrawPanadapter(Graphics g, int W, int H)  // changes yt7pwr
        {
            try
            {
                if (pan_fill)
                {
                    if (points == null || points.Length != W + 2)
                        points = new PointF[W + 2];
                }
                else
                {
                    if (points == null || points.Length != W)
                        points = new PointF[W];			    // array of points to display
                }

                if (smooth_line)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias; // SmoothingMode.HighSpeed;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                }

                float slope = 0.0F;						// samples to process per pixel
                UInt64 num_samples = 0;					// number of samples to process
                int start_sample_index = 0;				// index to begin looking at samples
                int Low = rx_display_low;
                int High = rx_display_high;
                int y_range = spectrum_grid_max - spectrum_grid_min;
                int yRange = spectrum_grid_max - spectrum_grid_min;
                int top = (int)((double)spectrum_grid_step * H / y_range);

                if (Display_GDI.console.PowerOn)
                {
                    max_y = Int32.MinValue;

                    if (data_ready)
                    {
                        if (!split_enabled && (console.TUN || mox && ((current_dsp_mode == DSPMode.CWL
                            || current_dsp_mode == DSPMode.CWU))))
                        {
                        }
                        else if (split_enabled && (console.TUN || mox &&
                            ((current_dsp_mode_subRX == DSPMode.CWL || current_dsp_mode_subRX == DSPMode.CWU))))
                        {
                        }
                        else
                        {
                            // get new data
                            fixed (void* rptr = &new_display_data[0])
                            fixed (void* wptr = &current_display_data[0])
                                Win32.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));
                        }

                        data_ready = false;
                    }

                    if (average_on)
                    {
                        if (average_on && (Audio.CurrentAudioState1 != Audio.AudioState.SWITCH ||
                            Audio.NextAudioState1 != Audio.AudioState.SWITCH))
                            UpdateDisplayPanadapterAverage();
                        else if (Audio.CurrentAudioState1 == Audio.AudioState.SWITCH || Audio.NextAudioState1 == Audio.AudioState.SWITCH)
                        {
                            average_buffer[0] = CLEAR_FLAG;
                            UpdateDisplayPanadapterAverage();
                        }
                        else if (!UpdateDisplayPanadapterAverage())
                        {
                            average_buffer = null;
                            average_buffer = new float[BUFFER_SIZE];	// initialize averaging buffer array
                            average_buffer[0] = CLEAR_FLAG;		// set the clear flag   
                            Debug.Write("Reset display average buffer!");
                            return false;
                        }
                    }

                    if (peak_on)
                        UpdateDisplayPeak();

                        g.DrawImage(panadapter_bmp, 0, 0, W, H);
                        DrawPanadapterGrid(ref g, W, H);

                    start_sample_index = (BUFFER_SIZE >> 1) + (int)(((double)Low * (double)BUFFER_SIZE) / (double)sample_rate);
                    num_samples = (UInt64)(((((double)High - (double)Low) / 1e4) * BUFFER_SIZE) / ((double)sample_rate / 1e4));
                    if (start_sample_index < 0) start_sample_index += BUFFER_SIZE;
                    if (((int)num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                        num_samples = (uint)(BUFFER_SIZE - start_sample_index);
                    slope = (float)num_samples / (float)W;

                    for (int i = 0; i < W; i++)
                    {
                        float max = float.MinValue;
                        float dval = i * slope + start_sample_index;
                        int lindex = (int)Math.Floor(dval);
                        int rindex = (int)Math.Floor(dval + slope);

                        if (slope <= 1.0 || lindex == rindex)
                        {
                            max = current_display_data[lindex % BUFFER_SIZE] * ((float)lindex - dval + 1)
                                + current_display_data[(lindex + 1) % BUFFER_SIZE] * (dval - (float)lindex);
                        }
                        else
                        {
                            for (int j = lindex; j < rindex; j++)
                                if (current_display_data[j % BUFFER_SIZE] > max) max = current_display_data[j % BUFFER_SIZE];
                        }

                        max += display_cal_offset;

                        if (max > max_y)
                        {
                            max_y = max;
                            max_x = i;
                        }

                        points[i].X = i;
                        points[i].Y = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * H / yRange)), H);
                    }

                    if (pan_fill)
                    {
                        points[W].X = W;
                        points[W].Y = H;
                        points[W + 1].X = 0;
                        points[W + 1].Y = H;
                        data_line_pen.Color = pan_fill_color;
                        g.FillPolygon(data_line_pen.Brush, points);
                        points[W] = points[W - 1];
                        points[W + 1] = points[W - 1];
                        data_line_pen.Color = data_line_color;
                        g.DrawLines(data_line_pen, points);
                    }
                    else
                    {
                        points[0].X = 1;
                        points[0].Y = points[1].Y;
                        points[W - 1].Y = points[W - 2].Y;
                        g.DrawLines(data_line_pen, points);
                    }

                    // draw vertical line
                    if (current_click_tune_mode != ClickTuneMode.Off)
                    {
                        Pen p;
                        if (current_click_tune_mode == ClickTuneMode.VFOA)
                            p = new Pen(grid_text_color);
                        else p = new Pen(Color.Red);
                        g.DrawLine(p, display_cursor_x, 2 * panadapter_font.Size, display_cursor_x, H);
                        g.DrawLine(p, display_cursor_x + 1, 2 * panadapter_font.Size, display_cursor_x + 1, H);
                    }
                }

                if (console.MinimalScreen && console.CurrentDisplayMode == DisplayMode.PANADAPTER &&
                    console.SetupForm.OnScreenDisplay)
                {
                    g.DrawString("Radio:" + console.CurrentModel.ToString(),
                        new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Red), 50, 30);
                    g.DrawString("VFOA mode:" + console.CurrentDSPMode.ToString(),
                        new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Red), 50, 50);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write("Error in DrawPanadapter function!" + ex.ToString());
                return false;
            }

        }

        private static HiPerfTimer timer_waterfall = new HiPerfTimer();
        private static float[] waterfall_data;
        unsafe static private bool DrawWaterfall(Graphics g, int W, int H)  // changes yt7pwr
        {
            try
            {
                if (waterfall_data == null || waterfall_data.Length < W)
                    waterfall_data = new float[W];			                    // array of points to display
                float slope = 0.0F;						                        // samples to process per pixel
                UInt64 num_samples = 0;					                        // number of samples to process
                int start_sample_index = 0;				                        // index to begin looking at samples
                int Low = 0;
                int High = 0;
                Low = rx_display_low;
                High = rx_display_high;
                max_y = Int32.MinValue;
                int R = 0, G = 0, B = 0;	                                	// variables to save Red, Green and Blue component values

                if (Display_GDI.console.PowerOn)
                {
                    int yRange = spectrum_grid_max - spectrum_grid_min;

                    if (waterfall_data_ready && !mox)
                    {
                        if (!split_enabled && (console.TUN || mox && ((current_dsp_mode == DSPMode.CWL
                            || current_dsp_mode == DSPMode.CWU))))
                        {
                        }
                        else if (split_enabled && (console.TUN || mox &&
                            ((current_dsp_mode_subRX == DSPMode.CWL || current_dsp_mode_subRX == DSPMode.CWU))))
                        {
                        }
                        else
                        {
                            // get new data
                            fixed (void* rptr = &new_waterfall_data[0])
                            fixed (void* wptr = &current_waterfall_data[0])
                                Win32.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));
                        }
                    }

                    if (current_display_mode == DisplayMode.WATERFALL)
                        DrawWaterfallGrid(ref g, W, H);

                    if (average_on)
                        UpdateDisplayWaterfallAverage();
                    if (peak_on)
                        UpdateDisplayPeak();

                    start_sample_index = (BUFFER_SIZE >> 1) + (int)(((double)Low * (double)BUFFER_SIZE) / (double)sample_rate);
                    num_samples = (UInt64)(((((double)High - (double)Low) / 1e4) * BUFFER_SIZE) / ((double)sample_rate / 1e4));
                    if (start_sample_index < 0) start_sample_index += BUFFER_SIZE;
                    if (((int)num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                        num_samples = (uint)(BUFFER_SIZE - start_sample_index);
                    slope = (float)num_samples / (float)W;

                    for (int i = 0; i < W; i++)
                    {
                        float max = float.MinValue;
                        float dval = i * slope + start_sample_index;
                        int lindex = (int)Math.Floor(dval);
                        int rindex = (int)Math.Floor(dval + slope);

                        if (slope <= 1 || lindex == rindex)
                            max = current_waterfall_data[lindex] * ((float)lindex - dval + 1) +
                                current_waterfall_data[(lindex + 1) % BUFFER_SIZE] * (dval - (float)lindex);
                        else
                        {
                            for (int j = lindex; j < rindex; j++)
                                if (current_waterfall_data[j % BUFFER_SIZE] > max) max = current_waterfall_data[j % BUFFER_SIZE];
                        }

                        max += display_cal_offset;

                        if (max > max_y)
                        {
                            max_y = max;
                            max_x = i;
                        }

                        waterfall_data[i] = max;
                    }

                    BitmapData bitmapData = waterfall_bmp.LockBits(
                            new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height),
                            ImageLockMode.ReadWrite,
                            waterfall_bmp.PixelFormat);

                    int pixel_size = 3;
                    byte* row = null;

                    if (!console.MOX)
                    {
                        if (reverse_waterfall)
                        {
                            // first scroll image up
                            int total_size = bitmapData.Stride * bitmapData.Height;		// find buffer size
                            Win32.memcpy(bitmapData.Scan0.ToPointer(),
                                new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                                total_size - bitmapData.Stride);

                            row = (byte*)(bitmapData.Scan0.ToInt32() + total_size - bitmapData.Stride);
                        }
                        else
                        {
                            // first scroll image down
                            int total_size = bitmapData.Stride * bitmapData.Height;		// find buffer size
                            Win32.memcpy(new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                                bitmapData.Scan0.ToPointer(),
                                total_size - bitmapData.Stride);

                            row = (byte*)(bitmapData.Scan0.ToInt32());
                        }

                        int i = 0;
                        switch (color_sheme)
                        {
                            case (ColorSheme.original):                        // tre color only
                                {
                                    // draw new data
                                    for (i = 0; i < W; i++)	// for each pixel in the new line
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
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
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
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
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
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
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
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
                                    }
                                }
                                break;
                        }
                    }

                    waterfall_bmp.UnlockBits(bitmapData);

                    if (current_display_mode == DisplayMode.WATERFALL)
                        g.DrawImageUnscaled(waterfall_bmp, 0, 20);	// draw the image on the background	
                    else
                        g.DrawImageUnscaled(waterfall_bmp, 0, 0);
                }

                if (console.MinimalScreen && console.SetupForm.OnScreenDisplay)
                {
                    g.DrawString("Radio:" + console.CurrentModel.ToString(),
                        new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Red), 10, 30);
                    g.DrawString("VFOA mode:" + console.CurrentDSPMode.ToString(),
                        new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Red), 10, 50);
                }

                waterfall_data_ready = false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        private static void DrawWaterfallGrid(ref Graphics g, int W, int H)  // changes yt7pwr
        {
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

/*            if (current_dsp_mode == DSPMode.DRM)
            {
                filter_low = -5000;
                filter_high = 5000;
            }*/

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
                            grid_pen.DashStyle = DashStyle.Dot;
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
                                grid_pen.DashStyle = DashStyle.Dot;
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
                                grid_pen.DashStyle = DashStyle.Dot;
                            }
                        }
                    }
                    //g.DrawLine(grid_pen, vgrid, top, vgrid, H);			//wa6ahl
                    grid_pen.DashStyle = DashStyle.Solid;

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

            int[] band_edge_list = { 135700, 137800, 415000, 525000, 18068000, 18168000, 1800000, 2000000, 3500000, 4000000,
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
                g.DrawString("High SWR", new System.Drawing.Font("Arial", 14, FontStyle.Bold),
                    new SolidBrush(Color.Red), 245, 20);
        }

        public static void ResetDisplayAverage()
        {
            try
            {
                average_buffer[0] = CLEAR_FLAG;	// set reset flag
                average_waterfall_buffer[0] = CLEAR_FLAG;
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
                peak_buffer[0] = CLEAR_FLAG; // set reset flag
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
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

        unsafe private static bool DrawScope(Graphics g, int W, int H)
        {
            try
            {
                if (!console.booting)
                {
                    DrawScopeGrid(ref g, W, H);

                    if (scope_min.Length != W)
                    {
                        scope_min = new float[W];
                    }
                    if (scope_max.Length != W)
                    {
                        scope_max = new float[W];
                    }

                    Point[] points = new Point[W * 2];
                    for (int i = 0; i < W; i++)
                    {
                        int pixel = 0;
                        pixel = (int)(H / 2 * Audio.scope_max[i]);
                        int y = H / 2 - pixel;
                        points[i].X = i;
                        points[i].Y = Math.Min(y, H);

                        pixel = (int)(H / 2 * Audio.scope_min[i]);
                        y = H / 2 - pixel;
                        points[W * 2 - 1 - i].X = i;
                        points[W * 2 - 1 - i].Y = y;
                    }

                    // draw the connected points
                    g.DrawLines(data_line_pen, points);
                    g.FillPolygon(new SolidBrush(pan_fill_color), points);

                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        unsafe private static bool DrawPhase(Graphics g, int W, int H)
        {
            DrawPhaseGrid(ref g, W, H);
            int num_points = phase_num_pts;

            if (data_ready)
            {
                // get new data
                fixed (void* rptr = &new_display_data[0])
                fixed (void* wptr = &current_display_data[0])
                    Win32.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));

                data_ready = false;
            }

            Point[] points = new Point[num_points];		// declare Point array
            for (int i = 0, j = 0; i < num_points; i++, j += 8)	// fill point array
            {
                int x = (int)(current_display_data[i * 2] * H / 2);
                int y = (int)(current_display_data[i * 2 + 1] * H / 2);
                points[i].X = W / 2 + x;
                points[i].Y = Math.Min(H / 2 + y, H);
            }

            // draw each point
            for (int i = 0; i < num_points; i++)
                g.DrawRectangle(data_line_pen, points[i].X, points[i].Y, 1, 1);

            // draw long cursor
            if (current_click_tune_mode != ClickTuneMode.Off)
            {
                Pen p;
                if (current_click_tune_mode == ClickTuneMode.VFOA)
                    p = new Pen(grid_text_color);
                else p = new Pen(Color.Red);
                g.DrawLine(p, display_cursor_x, 0, display_cursor_x, H);
                g.DrawLine(p, 0, display_cursor_y, W, display_cursor_y);
            }

            return true;
        }

        unsafe private static void DrawPhase2(Graphics g, int W, int H)
        {
            DrawPhaseGrid(ref g, W, H);
            int num_points = phase_num_pts;

            if (data_ready)
            {
                // get new data
                fixed (void* rptr = &new_display_data[0])
                fixed (void* wptr = &current_display_data[0])
                    Win32.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));

                data_ready = false;
            }

            Point[] points = new Point[num_points];		// declare Point array
            for (int i = 0; i < num_points; i++)	// fill point array
            {
                int x = (int)(current_display_data[i * 2] * H * 0.5 * 500);
                int y = (int)(current_display_data[i * 2 + 1] * H * 0.5 * 500);
                points[i].X = (int)(W * 0.5 + x);
                points[i].Y = (int)Math.Min((H * 0.5 + y), H);
            }

            // draw each point
            for (int i = 0; i < num_points; i++)
                g.DrawRectangle(data_line_pen, points[i].X, points[i].Y, 1, 1);

            // draw long cursor
            if (current_click_tune_mode != ClickTuneMode.Off)
            {
                Pen p;
                if (current_click_tune_mode == ClickTuneMode.VFOA)
                    p = new Pen(grid_text_color);
                else p = new Pen(Color.Red);
                g.DrawLine(p, display_cursor_x, 0, display_cursor_x, H);
                g.DrawLine(p, 0, display_cursor_y, W, display_cursor_y);
            }
        }

        unsafe static private bool DrawSpectrum(Graphics g, int W, int H)
        {
            DrawSpectrumGrid(ref g, W, H);
            if (points == null || points.Length < W)
                points = new PointF[W];			// array of points to display
            float slope = 0.0f;						// samples to process per pixel
            int num_samples = 0;					// number of samples to process
            int start_sample_index = 0;				// index to begin looking at samples
            int low = 0;
            int high = 0;

            max_y = Int32.MinValue;

            if (!mox)
            {
                low = rx_display_low;
                high = rx_display_high;
            }
            else
            {
                low = tx_display_low;
                high = tx_display_high;
            }

/*            if (current_dsp_mode == DSPMode.DRM)
            {
                low = 2500;
                high = 21500;
            }*/

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

            start_sample_index = (BUFFER_SIZE >> 1) + (int)((low * BUFFER_SIZE) / DttSP.RXSampleRate);
            num_samples = (int)((high - low) * BUFFER_SIZE / DttSP.RXSampleRate);

            if (start_sample_index < 0) start_sample_index = 0;
            if ((num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                num_samples = BUFFER_SIZE - start_sample_index;

            slope = (float)num_samples / (float)W;
            for (int i = 0; i < W; i++)
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

                if (max > max_y)
                {
                    max_y = max;
                    max_x = i;
                }

                points[i].X = i;
                points[i].Y = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * H / yRange)), H);
            }

            g.DrawLines(data_line_pen, points);

            // draw long cursor
            if (current_click_tune_mode != ClickTuneMode.Off)
            {
                Pen p;
                if (current_click_tune_mode == ClickTuneMode.VFOA)
                    p = new Pen(grid_text_color);
                else p = new Pen(Color.Red);
                g.DrawLine(p, display_cursor_x, 0, display_cursor_x, H);
                g.DrawLine(p, 0, display_cursor_y, W, display_cursor_y);
            }

            return true;
        }

        private static void DrawSpectrumGrid(ref Graphics g, int W, int H)
        {
            SolidBrush grid_text_brush = new SolidBrush(grid_text_color);
            Pen grid_pen = new Pen(grid_color);

            // draw background
            g.FillRectangle(new SolidBrush(display_background_color), 0, 0, W, H);

            int low = 0;								// init limit variables
            int high = 0;

            if (!mox)
            {
                low = rx_display_low;				// get RX display limits
                high = rx_display_high;
            }
            else
            {
                if (current_dsp_mode == DSPMode.CWL || current_dsp_mode == DSPMode.CWU)
                {
                    low = rx_display_low;
                    high = rx_display_high;
                }
                else
                {
                    low = tx_display_low;			// get RX display limits
                    high = tx_display_high;
                }
            }

            int mid_w = W / 2;
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
                float pixel_step_size = (float)(W * freq_step_size / f);

                int num_steps = f / freq_step_size;

                // Draw vertical lines
                for (int i = 1; i <= num_steps; i++)
                {
                    int x = W - (int)Math.Floor(i * pixel_step_size);	// for negative numbers

                    g.DrawLine(grid_pen, x, 0, x, H);				// draw right line

                    // Draw vertical line labels
                    int num = i * freq_step_size;
                    string label = num.ToString();
                    int offset = (int)((label.Length + 1) * 4.1);
                    if (x - offset >= 0)
                        g.DrawString("-" + label, panadapter_font, grid_text_brush, x - offset, (float)Math.Floor(H * .01));
                }

                // Draw horizontal lines
                int V = (int)(spectrum_grid_max - spectrum_grid_min);
                num_steps = V / spectrum_grid_step;
                pixel_step_size = H / num_steps;

                for (int i = 1; i < num_steps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)Math.Floor((double)(spectrum_grid_max - num) * H / y_range);

                    g.DrawLine(grid_pen, 0, y, W, y);

                    // Draw horizontal line labels
                    string label = num.ToString();
                    int offset = (int)(label.Length * 4.1);
                    if (label.Length == 3)
                        xOffset = (int)g.MeasureString("-", panadapter_font).Width - 2;
                    SizeF size = g.MeasureString(label, panadapter_font);

                    y -= 8;
                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                            x = W / 2 + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                        case DisplayLabelAlignment.AUTO:
                            x = (int)(W - size.Width);
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = W;
                            break;
                    }

                    if (y + 9 < H)
                        g.DrawString(label, panadapter_font, grid_text_brush, x, y);
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
                float pixel_step_size = (float)(W * freq_step_size / f);
                int num_steps = f / freq_step_size;

                // Draw vertical lines
                for (int i = 1; i <= num_steps; i++)
                {
                    int x = (int)Math.Floor(i * pixel_step_size);// for positive numbers

                    g.DrawLine(grid_pen, x, 0, x, H);			// draw right line

                    // Draw vertical line labels
                    int num = i * freq_step_size;
                    string label = num.ToString();
                    int offset = (int)(label.Length * 4.1);
                    if (x - offset + label.Length * 7 < W)
                        g.DrawString(label, panadapter_font, grid_text_brush, x - offset, (float)Math.Floor(H * .01));
                }

                // Draw horizontal lines
                int V = (int)(spectrum_grid_max - spectrum_grid_min);
                int numSteps = V / spectrum_grid_step;
                pixel_step_size = H / numSteps;
                for (int i = 1; i < numSteps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)Math.Floor((double)(spectrum_grid_max - num) * H / y_range);

                    g.DrawLine(grid_pen, 0, y, W, y);

                    // Draw horizontal line labels
                    string label = num.ToString();
                    if (label.Length == 3)
                        xOffset = (int)g.MeasureString("-", panadapter_font).Width - 2;
                    int offset = (int)(label.Length * 4.1);
                    SizeF size = g.MeasureString(label, panadapter_font);

                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                        case DisplayLabelAlignment.AUTO:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                            x = W / 2 + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                            x = (int)(W - size.Width);
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = W;
                            break;
                    }

                    y -= 8;
                    if (y + 9 < H)
                        g.DrawString(label, panadapter_font, grid_text_brush, x, y);
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
                int pixel_step_size = W / 2 * freq_step_size / f;
                int num_steps = f / freq_step_size;

                // Draw vertical lines
                for (int i = 1; i <= num_steps; i++)
                {
                    int xLeft = mid_w - (i * pixel_step_size);			// for negative numbers
                    int xRight = mid_w + (i * pixel_step_size);		// for positive numbers
                    g.DrawLine(grid_pen, xLeft, 0, xLeft, H);		// draw left line
                    g.DrawLine(grid_pen, xRight, 0, xRight, H);		// draw right line

                    // Draw vertical line labels
                    int num = i * freq_step_size;
                    string label = num.ToString();
                    int offsetL = (int)((label.Length + 1) * 4.1);
                    int offsetR = (int)(label.Length * 4.1);
                    if (xLeft - offsetL >= 0)
                    {
                        g.DrawString("-" + label, panadapter_font, grid_text_brush, xLeft - offsetL, (float)Math.Floor(H * .01));
                        g.DrawString(label, panadapter_font, grid_text_brush, xRight - offsetR, (float)Math.Floor(H * .01));
                    }
                }

                // Draw horizontal lines
                int V = (int)(spectrum_grid_max - spectrum_grid_min);
                int numSteps = V / spectrum_grid_step;
                pixel_step_size = H / numSteps;
                for (int i = 1; i < numSteps; i++)
                {
                    int xOffset = 0;
                    int num = spectrum_grid_max - i * spectrum_grid_step;
                    int y = (int)Math.Floor((double)(spectrum_grid_max - num) * H / y_range);
                    g.DrawLine(grid_pen, 0, y, W, y);

                    // Draw horizontal line labels
                    string label = num.ToString();
                    if (label.Length == 3) xOffset = 7;
                    int offset = (int)(label.Length * 4.1);
                    SizeF size = g.MeasureString(label, panadapter_font);

                    int x = 0;
                    switch (display_label_align)
                    {
                        case DisplayLabelAlignment.LEFT:
                            x = xOffset + 3;
                            break;
                        case DisplayLabelAlignment.CENTER:
                        case DisplayLabelAlignment.AUTO:
                            x = W / 2 + xOffset;
                            break;
                        case DisplayLabelAlignment.RIGHT:
                            x = (int)(W - size.Width);
                            break;
                        case DisplayLabelAlignment.OFF:
                            x = W;
                            break;
                    }

                    y -= 8;
                    if (y + 9 < H)
                        g.DrawString(label, panadapter_font, grid_text_brush, x, y);
                }
            }

            if (high_swr)
                g.DrawString("High SWR", new System.Drawing.Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Red), 245, 20);
        }

        private static void DrawPhaseGrid(ref Graphics g, int W, int H)
        {
            // draw background
            g.FillRectangle(new SolidBrush(display_background_color), 0, 0, W, H);

            for (double i = 0.50; i < 3; i += .50)	// draw 3 concentric circles
            {
                g.DrawEllipse(new Pen(grid_color), (int)(W / 2 - H * i / 2), (int)(H / 2 - H * i / 2), (int)(H * i), (int)(H * i));
            }

            if (high_swr)
                g.DrawString("High SWR", new System.Drawing.Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Red), 245, 20);
        }

        private static void DrawScopeGrid(ref Graphics g, int W, int H)
        {
            // draw background
            g.FillRectangle(new SolidBrush(display_background_color), 0, 0, W, H);

            if (show_horizontal_grid)
                g.DrawLine(new Pen(grid_color), 0, H / 2, W, H / 2);	// draw horizontal line

            if (show_vertical_grid)
                g.DrawLine(new Pen(grid_color), W / 2, 0, W / 2, H);	// draw vertical line

            if (high_swr)
                g.DrawString("High SWR", new System.Drawing.Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Red), 245, 20);
        }

        unsafe static private bool DrawHistogram(Graphics g, int W, int H)
        {
            DrawSpectrumGrid(ref g, W, H);

            if (points == null)
                points = new PointF[W];			// array of points to display
            else if (points != null && points.Length != W)
            {
                points = null;
                points = new PointF[W];
            }

            float slope = 0.0F;						// samples to process per pixel
            int num_samples = 0;					// number of samples to process
            int start_sample_index = 0;				// index to begin looking at samples
            int low = 0;
            int high = 0;
            max_y = Int32.MinValue;

            if (!mox)								// Receive Mode
            {
                low = rx_display_low;
                high = rx_display_high;
            }
            else									// Transmit Mode
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

            num_samples = (high - low);

            start_sample_index = (BUFFER_SIZE >> 1) + (int)((low * BUFFER_SIZE) / DttSP.RXSampleRate);
            num_samples = (int)((high - low) * BUFFER_SIZE / DttSP.RXSampleRate);
            if (start_sample_index < 0) start_sample_index = 0;
            if ((num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                num_samples = BUFFER_SIZE - start_sample_index;

            slope = (float)num_samples / (float)W;
            for (int i = 0; i < W; i++)
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
                points[i].Y = (int)Math.Min((Math.Floor((spectrum_grid_max - max) * H / yRange)), H);
            }

            // get the average
            float avg = 0.0F;
            int sum = 0;
            foreach (PointF p in points)
                sum += (int)p.Y;

            avg = (float)((float)sum / points.Length / 1.12);

            for (int i = 0; i < W; i++)
            {
                if (points[i].Y < histogram_data[i])
                {
                    histogram_history[i] = 0;
                    histogram_data[i] = (int)points[i].Y;
                }
                else
                {
                    histogram_history[i]++;
                    if (histogram_history[i] > 51)
                    {
                        histogram_history[i] = 0;
                        histogram_data[i] = (int)points[i].Y;
                    }

                    int alpha = (int)Math.Max(255 - histogram_history[i] * 5, 0);
                    Color c = Color.FromArgb(alpha, 0, 255, 0);
                    int height = (int)points[i].Y - histogram_data[i];
                    g.DrawRectangle(new Pen(c), i, histogram_data[i], 1, height);
                }

                if (points[i].Y >= avg)		// value is below the average
                {
                    Color c = Color.FromArgb(150, 0, 0, 255);
                    g.DrawRectangle(new Pen(c), points[i].X, points[i].Y, 1, H - points[i].Y);
                }
                else
                {
                    g.DrawRectangle(new Pen(Color.FromArgb(150, 0, 0, 255)), points[i].X, (int)Math.Floor(avg), 1, H - (int)Math.Floor(avg));
                    g.DrawRectangle(new Pen(Color.FromArgb(150, 255, 0, 0)), points[i].X, points[i].Y, 1, (int)Math.Floor(avg) - points[i].Y);
                }
            }

            // draw long cursor
            if (current_click_tune_mode != ClickTuneMode.Off)
            {
                Pen p;
                if (current_click_tune_mode == ClickTuneMode.VFOA)
                    p = new Pen(grid_text_color);
                else p = new Pen(Color.Red);
                g.DrawLine(p, display_cursor_x, 0, display_cursor_x, H);
                g.DrawLine(p, 0, display_cursor_y, W, display_cursor_y);
            }

            return true;
        }

        private static bool UpdateDisplayWaterfallAverage()
        {
            try
            {
                double dttsp_osc = (losc_hz - vfoa_hz) * 1e6;
                // Debug.WriteLine("last vfo: " + avg_last_ddsfreq + " vfo: " + DDSFreq); 
                if (Display_GDI.average_waterfall_buffer[0] == Display_GDI.CLEAR_FLAG)
                {
                    //Debug.WriteLine("Clearing average buf"); 
                    for (int i = 0; i < Display_GDI.BUFFER_SIZE; i++)
                        Display_GDI.average_waterfall_buffer[i] = Display_GDI.current_waterfall_data[i];
                }

                float new_mult = 0.0f;
                float old_mult = 0.0f;

                new_mult = Display_GDI.waterfall_avg_mult_new;
                old_mult = Display_GDI.waterfall_avg_mult_old;

                for (int i = 0; i < Display_GDI.BUFFER_SIZE; i++)
                    Display_GDI.average_waterfall_buffer[i] = Display_GDI.current_waterfall_data[i] =
                        (float)(Display_GDI.current_waterfall_data[i] * new_mult +
                        Display_GDI.average_waterfall_buffer[i] * old_mult);

                if (Display_GDI.average_waterfall_buffer[0] == Display_GDI.CLEAR_FLAG)
                {
                    avg_last_ddsfreq = 0;
                    avg_last_dttsp_osc = 0;
                }
                else
                {
                    avg_last_ddsfreq = losc_hz;
                    avg_last_dttsp_osc = dttsp_osc;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        private static bool UpdateDisplayPanadapterAverage()
        {
            try
            {
                double dttsp_osc = (losc_hz - vfoa_hz) * 1e6;
                // Debug.WriteLine("last vfo: " + avg_last_ddsfreq + " vfo: " + DDSFreq); 
                if (Display_GDI.average_buffer[0] == Display_GDI.CLEAR_FLAG ||
                    float.IsNaN(Display_GDI.average_buffer[0]))
                {
                    //Debug.WriteLine("Clearing average buf"); 
                    for (int i = 0; i < Display_GDI.BUFFER_SIZE; i++)
                        Display_GDI.average_buffer[i] = Display_GDI.current_display_data[i];
                }
                float new_mult = 0.0f;
                float old_mult = 0.0f;

                new_mult = Display_GDI.display_avg_mult_new;
                old_mult = Display_GDI.display_avg_mult_old;

                for (int i = 0; i < Display_GDI.BUFFER_SIZE; i++)
                    Display_GDI.average_buffer[i] = Display_GDI.current_display_data[i] =
                        (float)(Display_GDI.current_display_data[i] * new_mult +
                        Display_GDI.average_buffer[i] * old_mult);

                if (Display_GDI.average_buffer[0] == Display_GDI.CLEAR_FLAG)
                {
                    avg_last_ddsfreq = 0;
                    avg_last_dttsp_osc = 0;
                }
                else
                {
                    avg_last_ddsfreq = losc_hz;
                    avg_last_dttsp_osc = dttsp_osc;
                }


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