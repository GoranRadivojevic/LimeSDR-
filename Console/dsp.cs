//=================================================================
// dsp.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2008  FlexRadio Systems
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
//    8900 Marybank Dr.
//    Austin, TX 78750
//    USA
//=================================================================

/*
 *  Changes for GenesisRadio
 *  Copyright (C)2009-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

/*
 * LimeSDR#  
 * Copyright (C)2018 YT7PWR Goran Radivojevic
 * contact via email at: yt7pwr@mts.rs
 */

using System;
using System.Collections;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace PowerSDR
{
	unsafe class DttSP
	{
		#region Enums

		public enum MeterType
		{
			SIGNAL_STRENGTH=0, 
			AVG_SIGNAL_STRENGTH, 
			ADC_REAL, 
			ADC_IMAG,
			AGC_GAIN,
			MIC,  
			PWR,
			ALC,
			EQ,
			LEVELER,
			COMP,
			CPDR,
			ALC_G,
			LVL_G,
			MIC_PK,
			ALC_PK,
			EQ_PK,
			LEVELER_PK,
			COMP_PK,
			CPDR_PK,
		}

        public enum TransmitState
        {
            OFF = 0,
            ON = 1,
        }

        public enum Window
        {
            FIRST = -1,
            RECTANGULAR,
            HANNING,
            WELCH,
            PARZEN,
            BARTLETT,
            HAMMING,
            BLACKMAN2,
            BLACKMAN3,
            BLACKMAN4,
            EXPONENTIAL,
            RIEMANN,
            BLKHARRIS,
            LAST,
        }

		#endregion

        #region Properties

        public static int SpectrumSize = 32768;

        private static FilterMode AF_filter_mode = FilterMode.PASS_BAND;
        public static FilterMode AFFilterMode
        {
            set { AF_filter_mode = value; }
        }

        private static bool not_pan = false;
        public static bool NotPan
        {
            get { return not_pan; }
            set { not_pan = value; }
        }

        private static int block_size;
        public static int BlockSize
        {
            get { return block_size; }
        }

        private static DSPMode current_mode_subRX;
        public static DSPMode CurrentModeSubRX
        {
            get { return current_mode_subRX; }
            set
            {
                current_mode_subRX = value;
                int i = SetMode(0, 1, current_mode_subRX);
                int i1 = SetMode(1, 1, current_mode_subRX);

                if (i != 0 && i1 != 0)
                    MessageBox.Show("Error in DttSP.SetModeSubRX: " + i);
            }
        }

        private static bool split_enable = false;  // yt7pwr
        public static bool SplitEnable
        {
            set { split_enable = value; }
        }

        private static DSPMode current_mode;
        public static DSPMode CurrentMode
        {
            get { return current_mode; }
            set
            {
                current_mode = value;
                int i = SetMode(0, 0, current_mode);
                int i1 = SetMode(1, 0, current_mode);

                if (i != 0 && i1 != 0)
                    MessageBox.Show("Error in DttSP.SetModeMainRX: " + i);
            }
        }

        private static Window current_window = Window.HANNING;
        public static Window CurrentWindow
        {
            get { return current_window; }
            set
            {
                current_window = value;
                SetWindow(0, current_window);
            }
        }

        private static int rx_display_low = -4000;
        public static int RXDisplayLow
        {
            get { return rx_display_low; }
            set
            {
                rx_display_low = value;
                Display_GDI.RXDisplayLow = value;
#if(DirectX)
                Display_DirectX.RXDisplayLow = value;
#endif
            }
        }

        private static int rx_display_high = 24000;
        public static int RXDisplayHigh
        {
            get { return rx_display_high; }
            set
            {
                rx_display_high = value;
                Display_GDI.RXDisplayHigh = value;
#if(DirectX)
                Display_DirectX.RXDisplayHigh = value;
#endif
            }
        }

        private static int tx_display_low = -24000;
        public static int TXDisplayLow
        {
            get { return tx_display_low; }
            set
            {
                tx_display_low = value;
                Display_GDI.TXDisplayLow = value;
#if(DirectX)
                Display_DirectX.TXDisplayLow = value;
#endif
            }
        }

        private static int tx_display_high = 4000;
        public static int TXDisplayHigh
        {
            get { return tx_display_high; }
            set
            {
                tx_display_high = value;
                Display_GDI.TXDisplayHigh = value;
#if(DirectX)
                Display_DirectX.TXDisplayHigh = value;
#endif
            }
        }

        private static int rx_filter_low_cut_subRX = 200;
        public static int RXFilterLowCutSubRX                   // Sub rx
        {
            get { return rx_filter_low_cut_subRX; }
            set
            {
                rx_filter_low_cut_subRX = value;
                if (not_pan) UpdateRXDisplayVars();

                int i = 0;
                if (rx_filter_high_cut_subRX - rx_filter_low_cut_subRX >= 10)
                {
                    i = DttSP.SetRXFilter(0, 1, rx_filter_low_cut_subRX, rx_filter_high_cut_subRX);
                }

                /*if (i != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(RXFilterLowCutSubRX): " + i);*/
            }
        }

        private static int rx_filter_low_cut = 200;
        public static int RXFilterLowCut
        {
            get { return rx_filter_low_cut; }
            set
            {
                rx_filter_low_cut = value;
                if (not_pan) UpdateRXDisplayVars();

                int i = 0;
                if (rx_filter_high_cut - rx_filter_low_cut >= 10)
                {
                    i = DttSP.SetRXFilter(0, 0, rx_filter_low_cut, rx_filter_high_cut);
                }

                /*if (i != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(RXFilterLowCut): " + i);*/
            }
        }

        private static int rx_filter_high_cut_subRX = 2800;
        public static int RXFilterHighCutSubRX                  // Sub RX
        {
            get { return rx_filter_high_cut_subRX; }
            set
            {
                rx_filter_high_cut_subRX = value;
                if (not_pan) UpdateRXDisplayVars();

                int i = 0;
                if (rx_filter_high_cut_subRX - rx_filter_low_cut_subRX >= 10)
                {
                    i = DttSP.SetRXFilter(0, 1, rx_filter_low_cut_subRX, rx_filter_high_cut_subRX);
                }

                /*if (i != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(RXFilterHighCutSubRX): " + i);*/
            }
        }

        private static int rx_filter_high_cut = 2800;
        public static int RXFilterHighCut
        {
            get { return rx_filter_high_cut; }
            set
            {
                rx_filter_high_cut = value;
                if (not_pan) UpdateRXDisplayVars();

                int i = 0;
                if (rx_filter_high_cut - rx_filter_low_cut >= 10)
                {
                    i = DttSP.SetRXFilter(0, 0, rx_filter_low_cut, rx_filter_high_cut);
                }

                /*if (i != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(RXFilterHighCut): " + i);*/
            }
        }

        private static int tx_filter_low_cut = 300;
        public static int TXFilterLowCut
        {
            get { return tx_filter_low_cut; }
            set
            {
                tx_filter_low_cut = value;
                if (not_pan) UpdateTXDisplayVars();

                int i = DttSP.SetTXFilter(0, tx_filter_low_cut, tx_filter_high_cut);
                int i1 = DttSP.SetTXFilter(1, tx_filter_low_cut, tx_filter_high_cut);

                if (i != 0 && i1 != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(TXFilterLowCut): " + i);
            }
        }

        private static int tx_fm_filter_low_cut = -6000;
        public static int TXFMFilterLowCut
        {
            get { return tx_fm_filter_low_cut; }
            set
            {
                tx_fm_filter_low_cut = value;

                int i = DttSP.SetTXFilter(0, tx_fm_filter_low_cut, tx_fm_filter_high_cut);
                int i1 = DttSP.SetTXFilter(1, tx_fm_filter_low_cut, tx_fm_filter_high_cut);

                if (i != 0 && i1 != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(TXFilterLowCut): " + i);
            }
        }

        private static int tx_filter_high_cut = 3000;
        public static int TXFilterHighCut
        {
            get { return tx_filter_high_cut; }
            set
            {
                tx_filter_high_cut = value;
                if (not_pan) UpdateTXDisplayVars();

                int i = DttSP.SetTXFilter(0, tx_filter_low_cut, tx_filter_high_cut);
                int i1 = DttSP.SetTXFilter(0, tx_filter_low_cut, tx_filter_high_cut);

                if (i != 0 && i1 != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(TXFilterHighCut): " + i);
            }
        }

        private static int tx_fm_filter_high_cut = 6000;
        public static int TXFMFilterHighCut
        {
            get { return tx_fm_filter_high_cut; }
            set
            {
                tx_fm_filter_high_cut = value;

                int i = DttSP.SetTXFilter(0, tx_fm_filter_low_cut, tx_fm_filter_high_cut);
                int i1 = DttSP.SetTXFilter(1, tx_fm_filter_low_cut, tx_fm_filter_high_cut);

                if (i != 0 && i1 != 0)
                    MessageBox.Show("Error in DttSP.SetFilter(TXFilterHighCut): " + i);
            }
        }

        private static double RXsample_rate = 48000;
        public static double RXSampleRate
        {
            get { return RXsample_rate; }
            set
            {
                RXsample_rate = value;
                SetSpectrumSize(0, SpectrumSize);
                int i = SetSampleRate(0, Math.Max(24000, RXsample_rate));
            }
        }

        private static double TXsample_rate = 48000;
        public static double TXSampleRate
        {
            get { return TXsample_rate; }
            set
            {
                TXsample_rate = value;
                SetSpectrumSize(1, SpectrumSize);
                int i = SetSampleRate(1, Math.Max(24000, RXsample_rate));
            }
        }

        #endregion

		#region Dll Method Definitions

        [DllImport("DttSP.dll", EntryPoint = "ResetRingBuffer")]
        public static extern void ResetRB(uint thread);

		[DllImport("DttSP.dll", EntryPoint="Setup_SDR")]
		public static extern void SetupSDR(string data_path);

        [DllImport("DttSP.dll", EntryPoint = "SetSPEClen")]
        public static extern void SetSpectrumSize(uint thread, int size);

		[DllImport("DttSP.dll", EntryPoint="SetDSPBuflen")]
		public static extern void ResizeSDR(uint thread, int DSPsize);
        public static int bufsize = 2048;
		
		[DllImport("DttSP.dll", EntryPoint="Destroy_SDR")]
		public static extern void Exit();

        [DllImport("DttSP.dll", EntryPoint = "AudioReset")]
        public static extern void AudioReset();

		[DllImport("DttSP.dll", EntryPoint="process_samples_thread")]
		public static extern void ProcessSamplesThread(uint thread);

		[DllImport("DttSP.dll", EntryPoint="cwtimerfired")]
		public static extern void CWTimerFired();
 
		[DllImport("DttSP.dll", EntryPoint="StartKeyer")]
		public static extern void StartKeyer();

		[DllImport("DttSP.dll", EntryPoint="StopKeyer")]
		public static extern void StopKeyer();

		[DllImport("DttSP.dll", EntryPoint="CWRingRestart")]
		public static extern void CWRingRestart();
		///<summary>
		/// The KeyValue function sends timing data and key depressions and keying selection
		/// </summary>
		/// <param name="del">This is the time since the last call to this function</param>
		/// <param name="dash">This is a DttSP style boolean (a byte) which asserts dash or not-dash </param>
		/// <param name="dot">This is a DttSP style boolean (a byte) which asserts dot or not-dot</param>
		/// <param name="keyprog">This is a DttSP style boolean (a byte) which asserts iambic keyer or not-iambic</param>
		///
		[DllImport("DttSP.dll", EntryPoint="key_thread_process")]
		public static extern void KeyValue(float del, bool dash, bool dot, bool keyprog);

		[DllImport("DttSP.dll", EntryPoint="NewKeyer")]
		public static extern void NewKeyer(float freq, bool iambic, float gain, float ramp,
			float wpm, float SampleRate);

		[DllImport("DttSP.dll",EntryPoint="SetThreadProcessingMode")]
		public static extern void SetThreadProcessingMode(uint thread, int runmode);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerPerf")]///
		public static extern void SetKeyerPerf(bool hiperf);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerSpeed")]///
		public static extern void SetKeyerSpeed(float speed);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerFreq")]///
		public static extern void SetKeyerFreq(float freq);

        [DllImport("DttSP.dll", EntryPoint = "SetMonitorFreq")]///
        public static extern void SetMonitorFreq(float freq);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerSampleRate")]///
		public static extern void SetKeyerSampleRate(float freq);

        [DllImport("DttSP.dll", EntryPoint = "SetKeyerRise")]///
        public static extern void SetKeyerRise(float rise);

        [DllImport("DttSP.dll", EntryPoint = "SetKeyerFall")]///
        public static extern void SetKeyerFall(float fall);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerIambic")]///
        public static extern void SetKeyerIambic(bool iambic);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerRevPdl")]///
        public static extern void SetKeyerRevPdl(bool revpdl);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerDeBounce")]///
        public static extern void SetKeyerDeBounce(int debounce);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerWeight")]///
        public static extern void SetKeyerWeight(int weight);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerResetSize")]///
        public static extern void SetKeyerResetSize(int size);

		[DllImport("DttSP.dll", EntryPoint="SetKeyerMode")]///
        public static extern void SetKeyerMode(int mode);

		[DllImport("DttSP.dll", EntryPoint="KeyerClockFireRelease")]
		public static extern void KeyerClockFireRelease();

		[DllImport("DttSP.dll", EntryPoint="KeyerPlaying")]
		[return:MarshalAs(UnmanagedType.I1)]
		public static extern bool KeyerPlaying();

		[DllImport("DttSP.dll", EntryPoint="KeyerRunning")]
		[return:MarshalAs(UnmanagedType.I1)]
		public static extern bool KeyerRunning();

		[DllImport("DttSP.dll", EntryPoint="KeyerClockFireWait")]
		public static extern void KeyerClockFireWait();

		[DllImport("DttSP.dll", EntryPoint="KeyerStartedRelease")]
		public static extern void KeyerStartedRelease();

		[DllImport("DttSP.dll", EntryPoint="KeyerStartedWait")]
		public static extern void KeyerStartedWait();

		[DllImport("DttSP.dll", EntryPoint="SetWhichKey")]
        public static extern void SetWhichKey(byte keyselect);

		[DllImport("DttSP.dll", EntryPoint="PollTimerRelease")]
		public static extern void PollTimerRelease();

		[DllImport("DttSP.dll", EntryPoint="PollTimerWait")]
		public static extern void PollTimerWait();

        [DllImport("DttSP.dll", EntryPoint = "TimerRead")]              // zt7pwr
        public static extern double TimerRead();

		[DllImport("DttSP.dll", EntryPoint="DeleteKeyer")]
		public static extern void DeleteKeyer();

		[DllImport("DttSP.dll", EntryPoint="sound_thread_keyd")]
		public static extern void KeyerSoundThread();

        [DllImport("DttSP.dll", EntryPoint = "monitor_thread_keyd")]        // yt7pwr
        public static extern void KeyerMonitorThread();

		[DllImport("DttSP.dll", EntryPoint="CWtoneExchange")]
        public static extern void CWtoneExchange(float* bufl, float* bufr, int nframes);

        [DllImport("DttSP.dll", EntryPoint = "CWMonitorExchange")]          // yt7pwr
        public static extern void CWMonitorExchange(float* bufl, float* bufr, int nframes);

		[DllImport("DttSP.dll", EntryPoint="Audio_Callback")]
		public static extern void ExchangeSamples(int thread, void *input_l, void *input_r, void *output_l, void *output_r, int numsamples);

        [DllImport("DttSP.dll", EntryPoint = "Audio_Input_Callback")]       // yt7pwr
        public static extern void ExchangeInputSamples(int thread, void* input_l, void* input_r, int numsamples);

        [DllImport("DttSP.dll", EntryPoint = "Audio_Output_Callback")]      // yt7pwr
        public static extern void ExchangeOutputSamples(int thread, void* output_l, void* output_r, int numsamples);

		[DllImport("DttSP.dll", EntryPoint="Audio_Callback2")]
        public static extern void ExchangeSamples2(int thread, void* input, void* output, int numsamples);

		[DllImport("DttSP.dll", EntryPoint="SetAudioSize")]///
		public static extern void SetAudioSize(uint thread, int size);

		[DllImport("DttSP.dll", EntryPoint="SetMode")]///
		public static extern int SetMode(uint thread, uint subrx, DSPMode m);

        [DllImport("DttSP.dll", EntryPoint = "SetTXMode")]///
        public static extern int SetTXMode(uint thread, DSPMode m);

        [DllImport("DttSP.dll", EntryPoint = "FMreload")]///            // yt7pwr
        public static extern int FMreload(uint thread, uint subrx, double low, double high, double bandwidth, int wide);

        [DllImport("DttSP.dll", EntryPoint = "FMenableStereo")]///      // yt7pwr
        public static extern int FMenableStereo(uint thread, uint subrx, int stereo);

        [DllImport("DttSP.dll", EntryPoint = "FMStereoDetected")]///    // yt7pwr
        public static extern int FMStereoDetected(uint thread, uint subrx, int stereo);

		[DllImport("DttSP.dll", EntryPoint="SetRXFilter")]///
		public static extern int SetRXFilter(uint thread, uint subrx, double low, double high);

        [DllImport("DttSP.dll", EntryPoint = "SetRXStopBandFilter")]  // yt7pwr
        public static extern int SetRXStopBandFilter(uint thread, uint subrx, double low, double high);

        [DllImport("DttSP.dll", EntryPoint = "SetRXLowPassFilter")]  // yt7pwr
        public static extern int SetRXLowPassFilter(uint thread, uint subrx, double cut_freq);

        [DllImport("DttSP.dll", EntryPoint = "SetRXHighPassFilter")]  // yt7pwr
        public static extern int SetRXHighPassFilter(uint thread, uint subrx, double cut_freq);

        [DllImport("DttSP.dll", EntryPoint = "SetRXFilterMode")]  // yt7pwr
        public static extern int SetRXFilterMode(uint thread, uint subrx, FilterMode mode);
	
		[DllImport("DttSP.dll", EntryPoint="SetTXFilter")]///
		public static extern int SetTXFilter(uint thread, double low, double high);

		[DllImport("DttSP.dll", EntryPoint="SetTXOsc")]///
		public static extern int SetTXOsc(uint thread, double freq);

        [DllImport("DttSP.dll", EntryPoint = "SetCTCSSOscFreq")]
        public static extern int SetCTCSSOscFreq(uint thread, double freq);

        [DllImport("DttSP.dll", EntryPoint = "SetCTCSSOscAmplitude")]
        public static extern int SetCTCSSAmplitude(uint thread, double amplitude);

        [DllImport("DttSP.dll", EntryPoint = "EnableCTCSS")]
        public static extern int EnableCTCSS(uint thread, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetSampleRate")]
		public static extern int SetSampleRate(uint thread, double sampleRate);

		[DllImport("DttSP.dll", EntryPoint="SetNR")]///
		public static extern void SetNR(uint thread, uint subrx, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetNRvals")]///
		public static extern void SetNRvals(uint thread, uint subrx, int taps, int delay, double gain, double leak);

		[DllImport("DttSP.dll", EntryPoint="SetANF")]///
		public static extern void SetANF(uint thread, uint subrx, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetANFvals")]///
		public static extern void SetANFvals(uint thread, uint subrx, int taps, int delay, double gain, double leak);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGC")]///
		public static extern void SetRXAGC(uint thread, uint subrx, AGCMode setit);

		[DllImport("DttSP.dll", EntryPoint="SetTXAGCFF")]///
		public static extern void SetTXAGCFF(uint thread, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetTXAGCFFCompression")]///
		public static extern void SetTXAGCFFCompression(uint thread, double txcompression);

        [DllImport("DttSP.dll", EntryPoint = "SetTXDCBlock")]///
        public static extern void SetTXDCBlock(uint thread, bool setit);

        [DllImport("DttSP.dll", EntryPoint = "SetRXDCBlock")]///
        public static extern void SetRXDCBlock(uint thread, uint subrx, bool setit);

        [DllImport("DttSP.dll", EntryPoint = "SetRXDCBlockGain")]///
        public static extern void SetRXDCBlockGain(uint thread, uint subrx, float gain);

		[DllImport("DttSP.dll", EntryPoint="SetDCBlock")]///
		public static extern void SetDCBlock(uint thread, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetTXEQ")]
		public static extern void SetTXEQ(uint thread, int[] txeq);

        [DllImport("DttSP.dll", EntryPoint = "SetTXFMDeviation")]///
        public static extern void SetTXFMDev(uint thread, double deviation);

		[DllImport("DttSP.dll", EntryPoint="SetGrphTXEQcmd")]///
		public static extern void SetGrphTXEQcmd(uint thread, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetGrphTXEQ")]///
		public static extern void SetGrphTXEQ(uint thread, int[] txeq);
		
		[DllImport("DttSP.dll", EntryPoint="SetGrphTXEQ10")]///
		public static extern void SetGrphTXEQ10(uint thread, int[] txeq);

		[DllImport("DttSP.dll", EntryPoint="SetGrphRXEQcmd")]///
		public static extern void SetGrphRXEQcmd(uint thread, uint subrx, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetGrphRXEQ")]///
		public static extern void SetGrphRXEQ(uint thread, uint subrx,int[] rxeq);

		[DllImport("DttSP.dll", EntryPoint="SetGrphRXEQ10")]///
		public static extern void SetGrphRXEQ10(uint thread, uint subrx,int[] rxeq);

		[DllImport("DttSP.dll", EntryPoint="SetNotch160")]///
		public static extern void SetNotch160(uint thread, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetNB")]///
		public static extern void SetNB(uint thread, uint subrx, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetNBvals")]///
		public static extern void SetNBvals(uint thread, uint subrx, double threshold);

        [DllImport("DttSP.dll", EntryPoint = "GetSAMFreq")]///
        public static extern void GetSAMFreq(uint thread, uint subrx, float* freq);

        [DllImport("DttSP.dll", EntryPoint = "GetSAMPLLvals")]///
        public static extern void GetSAMPLLvals(uint thread, uint subrx, float* alpha, float* beta);

        [DllImport("DttSP.dll", EntryPoint = "SetSAMPLLvals")]///
        public static extern void SetSAMPLLvals(uint thread, uint subrx, float alpha, float beta);

		[DllImport("DttSP.dll", EntryPoint="SetCorrectIQGain")]///
		public static extern void SetCorrectIQGain(uint thread, uint subrx, double setit);

		[DllImport("DttSP.dll", EntryPoint="SetCorrectIQPhase")]///
		public static extern void SetCorrectIQPhase(uint thread, uint subrx, double setit);

		[DllImport("DttSP.dll", EntryPoint="SetCorrectTXIQGain")]///
		public static extern void SetTXIQGain(uint thread, double setit);

		[DllImport("DttSP.dll", EntryPoint="SetCorrectTXIQPhase")]///
		public static extern void SetTXIQPhase(uint thread, double setit);

        [DllImport("DttSP.dll", EntryPoint = "SetIQSuspended")]                     // yt7pwr
        public static extern void SetIQSuspended(uint setit);

        [DllImport("DttSP.dll", EntryPoint = "SetIQFixed")]                         // yt7pwr
        public static extern void SetIQFixed(uint thread, uint subrx, uint setit, float gain, float phase);

        [DllImport("DttSP.dll", EntryPoint = "SetCorrectIQEnable")]
        public static extern void SetCorrectIQEnable(uint setit);

        [DllImport("DttSP.dll", EntryPoint = "GetCorrectRXIQw")]
        public static extern void GetCorrectRXIQw(uint thread, uint subrx, float* real, float* imag, uint index);

        [DllImport("DttSP.dll", EntryPoint = "SetCorrectRXIQwReal")]///
        public static extern void SetCorrectRXIQwReal(uint thread, uint subrx, float setit, uint index);

        [DllImport("DttSP.dll", EntryPoint = "SetCorrectRXIQwImag")]///
        public static extern void SetCorrectRXIQwImag(uint thread, uint subrx, float setit, uint index);

        [DllImport("DttSP.dll", EntryPoint = "SetCorrectRXIQw")]///
        public static extern void SetCorrectRXIQw(uint thread, uint subrx, float real, float imag, uint index);

        [DllImport("DttSP.dll", EntryPoint = "SetCorrectRXIQMu")]///
        public static extern void SetCorrectIQMu(uint thread, uint subrx, double setit);

        [DllImport("DttSP.dll", EntryPoint = "GetCorrectRXIQMu")]///
        public static extern float GetCorrectIQMu(uint thread, uint subrx);

        [DllImport("DttSP.dll", EntryPoint = "SetCorrectTXIQMu")]///
        public static extern void SetTXIQMu(uint thread, double setit);

		[DllImport("DttSP.dll", EntryPoint="SetSDROM")]///
		public static extern void SetSDROM(uint thread, uint subrx, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetSDROMvals")]///
		public static extern void SetSDROMvals(uint thread, uint subrx, double threshold);

		[DllImport("DttSP.dll", EntryPoint="SetFixedAGC")]///
		public static extern void SetFixedAGC(uint thread, uint subrx, double fixed_agc);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGCTop")]///
		public static extern void SetRXAGCMaxGain(uint thread, uint subrx, double max_agc);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGCAttack")]///
		public static extern void SetRXAGCAttack(uint thread, uint subrx, int attack);

		[DllImport("DttSP.dll", EntryPoint="SetRingBufferOffset")]
		public static extern void SetRingBufferOffset(uint thread, int offset);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGCDecay")]///
		public static extern void SetRXAGCDecay(uint thread, uint subrx, int decay);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGCHang")]///
		public static extern void SetRXAGCHang(uint thread, uint subrx, int hang);

		[DllImport("DttSP.dll", EntryPoint="SetRXOutputGain")]///
		public static extern void SetRXOutputGain(uint thread, uint subrx, double g);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGCSlope")]///
		public static extern void SetRXAGCSlope(uint thread, uint subrx, int slope);

		[DllImport("DttSP.dll", EntryPoint="SetRXAGCHangThreshold")]///
		public static extern void SetRXAGCHangThreshold(uint thread, uint subrx, int hangthreshold);

		[DllImport("DttSP.dll", EntryPoint="SetTXAMCarrierLevel")]///
		public static extern void SetTXAMCarrierLevel(uint thread, double carrier_level);

		[DllImport("DttSP.dll", EntryPoint="SetTXALCBot")]///
		public static extern void SetTXALCBot(uint thread, double max_agc);

		[DllImport("DttSP.dll", EntryPoint="SetTXALCAttack")]///
		public static extern void SetTXALCAttack(uint thread, int attack);

		[DllImport("DttSP.dll", EntryPoint="SetTXALCDecay")]///
		public static extern void SetTXALCDecay(uint thread, int attack);

		[DllImport("DttSP.dll", EntryPoint="SetTXALCHang")]///
		public static extern void SetTXALCHang(uint thread, int hang);

		[DllImport("DttSP.dll", EntryPoint="SetTXLevelerTop")]
		public static extern void SetTXLevelerMaxGain(uint thread, double max_agc);

		[DllImport("DttSP.dll", EntryPoint="SetTXLevelerAttack")]///
		public static extern void SetTXLevelerAttack(uint thread, int attack);

		[DllImport("DttSP.dll", EntryPoint="SetTXLevelerDecay")]///
		public static extern void SetTXLevelerDecay(uint thread, int attack);

		[DllImport("DttSP.dll", EntryPoint="SetTXLevelerHang")]///
		public static extern void SetTXLevelerHang(uint thread, int hang);

		[DllImport("DttSP.dll", EntryPoint="SetTXLevelerSt")]///
		public static extern void SetTXLevelerSt(uint thread, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetWindow")]///
		public static extern void SetWindow(uint thread, Window windowset);

		[DllImport("DttSP.dll", EntryPoint="SetSpectrumPolyphase")]///
		public static extern void SetSpectrumPolyphase(uint thread, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetBIN")]///
		public static extern void SetBIN(uint thread, uint subrx, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetSquelchVal")]///
		public static extern void SetSquelchVal(uint thread, uint subrx, float setit);

		[DllImport("DttSP.dll", EntryPoint="SetSquelchState")]///
		public static extern void SetSquelchState(uint thread, uint subrx, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetTXSquelchVal")]///
		public static extern void SetTXSquelchVal(uint thread, float setit);

		[DllImport("DttSP.dll", EntryPoint="SetTXSquelchSt")]///
		public static extern void SetTXSquelchState(uint thread, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetTXCompandSt")]///
		public static extern void SetTXCompandSt(uint thread, bool state);

		[DllImport("DttSP.dll", EntryPoint="SetTXCompand")]///
		public static extern void SetTXCompand(uint thread, double setit);

		[DllImport("DttSP.dll", EntryPoint="SetPWSmode")]///
		public static extern void SetPWSmode(uint thread, uint subrx, bool setit);
		public static void SetTXPWSmode(uint thread, bool setit)
		{
			SetPWSmode(thread, 0, setit);
		}

		[DllImport("DttSP.dll", EntryPoint="Process_Spectrum")]
		unsafe public static extern void GetSpectrum(uint thread, float* results);

		[DllImport("DttSP.dll", EntryPoint="Process_ComplexSpectrum")]
		unsafe public static extern void GetComplexSpectrum(uint thread, float* results);

		[DllImport("DttSP.dll", EntryPoint="Process_Panadapter")]
		unsafe public static extern void GetPanadapter(uint thread, float* results);

		[DllImport("DttSP.dll", EntryPoint="Process_Phase")]
		unsafe public static extern void GetPhase(uint thread, float* results, int numpoints);

		[DllImport("DttSP.dll", EntryPoint="Process_Scope")]
		unsafe public static extern void GetScope(uint thread, float* results, int numpoints);

		[DllImport("DttSP.dll", EntryPoint="SetTRX")]
		unsafe public static extern void SetTRX(uint thread, bool trx_on);

		[DllImport("DttSP.dll", EntryPoint="CalculateRXMeter")]
		public static extern float CalculateRXMeter(uint thread, uint subrx, MeterType MT);

		[DllImport("DttSP.dll", EntryPoint="CalculateTXMeter")]
		public static extern float CalculateTXMeter(uint thread,MeterType MT);

		[DllImport("DttSP.dll", EntryPoint="Release_Update")]
		unsafe public static extern void ReleaseUpdate();
 
		[DllImport("DttSP.dll", EntryPoint="NewResampler")]
		unsafe public static extern void *NewResampler(int sampin, int sampout);

		[DllImport("DttSP.dll", EntryPoint="DoResampler")]
		unsafe public static extern void DoResampler(float *input, float *output, int numsamps, int *outsamps,void *ptr);

		[DllImport("DttSP.dll", EntryPoint="DelPolyPhaseFIR")]
		unsafe public static extern void DelResampler(void *ptr);

        [DllImport("DttSP.dll", EntryPoint = "SetResamplerF")]                      // yt7pwr
        unsafe public static extern void* SetResamplerF(int thread, int subrx, int samplerate_in, int samplerate_out);

        [DllImport("DttSP.dll", EntryPoint = "EnableResamplerF")]                   // yt7pwr
        unsafe public static extern void* EnableResamplerF(int thread, int subrx, int enable);

		[DllImport("DttSP.dll", EntryPoint="NewResamplerF")]
        unsafe public static extern void* NewResamplerF(int interpFactor, int deciFactor);

        [DllImport("DttSP.dll", EntryPoint = "NewResamplerF_LimeSDR")]
        unsafe public static extern void* NewResamplerF_LimeSDR(int sampin, int sampout);

		[DllImport("DttSP.dll", EntryPoint="DoResamplerF")]
		unsafe public static extern void DoResamplerF(float *input, float *output, int numsamps, int *outsamps,void *ptr);

		[DllImport("DttSP.dll", EntryPoint="DelPolyPhaseFIRF")]
		unsafe public static extern void DelResamplerF(void *ptr);

		[DllImport("DttSP.dll", EntryPoint="SetThreadNo")]///
		unsafe public static extern void SetThreadNo(uint threadno);

		[DllImport("DttSP.dll", EntryPoint="SetThreadCom")]///
		unsafe public static extern void SetThreadCom(uint thread_com);

		[DllImport("DttSP.dll", EntryPoint="SetSubRXSt")]///
		unsafe public static extern void SetRXOn(uint thread, uint subrx, bool setit);

		[DllImport("DttSP.dll", EntryPoint="SetRXPan")]///
		unsafe public static extern void SetRXPan(uint thread, uint subrx, float pan); // takes values from 0 to 1.0 for L to R.

        public static double RXOsc = 0;
		[DllImport("DttSP.dll", EntryPoint="SetRXOsc")]///
		unsafe public static extern int SetRXOsc(uint thread, uint subrx, double freq);

		[DllImport("DttSP.dll", EntryPoint = "GetLoopPTT")]
		public static extern byte GetLoopPTT(); 

		[DllImport("DttSP.dll", EntryPoint = "GetLoopPresent")]
		public static extern byte GetLoopPresent(); 

		[DllImport("DttSP.dll", EntryPoint = "SetLoopEnabled")]
		public static extern byte SetLoopEnabled(int Enabled);

        [DllImport("DttSP.dll", EntryPoint = "SetRXOscPhase")]///
        unsafe public static extern int SetRXOscPhase(double phase);

		#endregion

        #region Misc routines

        public static bool Init()
        {
            try
            {
                block_size = 2048;
                SetSpectrumSize(0, SpectrumSize);
                SetupSDR(Application.StartupPath.ToString() + "\\wisdom");
                ReleaseUpdate();
                SetSpectrumSize(0, SpectrumSize);
                SetSpectrumSize(1, SpectrumSize);
                SetSampleRate(0, 48000.0);
                SetSampleRate(1, 48000.0);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public static void SetFilterSubRX(int low, int high)
        {
            try
            {
                rx_filter_low_cut_subRX = low;
                rx_filter_high_cut_subRX = high;
                if (not_pan) UpdateRXDisplayVars();

                int i = 0;
                if (rx_filter_high_cut_subRX - rx_filter_low_cut_subRX >= 10)
                {
                    i = DttSP.SetRXFilter(0, 1, rx_filter_low_cut_subRX, rx_filter_high_cut_subRX);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void SetNotchFilter(int low, int high)
        {
            try
            {
                int i = 0;
                if (high - low >= 10)
                {
                    i = DttSP.SetRXLowPassFilter(0, 0, Math.Abs(low));
                    i = DttSP.SetRXHighPassFilter(0, 0, Math.Abs(high));
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void SetStopBandFilter(int low, int high)
        {
            try
            {
                int i = 0;
                if (high - low >= 10)
                {
                    i = DttSP.SetRXStopBandFilter(0, 0, low, high);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void SetFilter(int low, int high)     // changes yt7pwr
        {
            try
            {
                rx_filter_low_cut = low;
                rx_filter_high_cut = high;
                if (not_pan) UpdateRXDisplayVars();

                int i = 0;
                if (rx_filter_high_cut - rx_filter_low_cut >= 10)
                {
                    i = DttSP.SetRXFilter(0, 0, rx_filter_low_cut, rx_filter_high_cut);
                }

                if (CurrentMode == DSPMode.FMN)
                {
                    DttSP.FMreload(0, 0, low, high, high - low, 0);
                    DttSP.FMreload(0, 1, low, high, high - low, 0);
                }
                else if (CurrentMode == DSPMode.WFM)
                {
                    DttSP.FMreload(0, 0, low, high, (high - low), 1);
                    DttSP.FMreload(0, 1, low, high, (high - low), 1);
                }

                /*if (i != 0)
                    MessageBox.Show("Error in DttSP.SetRXFilters (SetFilter): " + i);*/
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void SetTXFilters(int low, int high)
        {
            if (split_enable)
            {
                switch (CurrentModeSubRX)
                {
                    case DSPMode.LSB:
                    case DSPMode.CWL:
                    case DSPMode.DIGL:
                        tx_filter_low_cut = -high;
                        tx_filter_high_cut = -low;
                        break;
                    case DSPMode.USB:
                    case DSPMode.CWU:
                    case DSPMode.DIGU:
                        tx_filter_low_cut = low;
                        tx_filter_high_cut = high;
                        break;
                    case DSPMode.DSB:
                        tx_filter_low_cut = -high;
                        tx_filter_high_cut = high;
                        break;
                    case DSPMode.AM:
                    case DSPMode.SAM:
                        tx_filter_low_cut = -high;
                        tx_filter_high_cut = high;
                        break;
                    case DSPMode.FMN:
                        tx_filter_low_cut = tx_fm_filter_low_cut;
                        tx_filter_high_cut = tx_fm_filter_high_cut;
                        break;
                }
            }
            else
            {
                switch (CurrentMode)
                {
                    case DSPMode.LSB:
                    case DSPMode.CWL:
                    case DSPMode.DIGL:
                        tx_filter_low_cut = -high;
                        tx_filter_high_cut = -low;
                        break;
                    case DSPMode.USB:
                    case DSPMode.CWU:
                    case DSPMode.DIGU:
                        tx_filter_low_cut = low;
                        tx_filter_high_cut = high;
                        break;
                    case DSPMode.DSB:
                        tx_filter_low_cut = -high;
                        tx_filter_high_cut = high;
                        break;
                    case DSPMode.AM:
                    case DSPMode.SAM:
                        tx_filter_low_cut = -high;
                        tx_filter_high_cut = high;
                        break;
                    case DSPMode.FMN:
                        tx_filter_low_cut = tx_fm_filter_low_cut;
                        tx_filter_high_cut = tx_fm_filter_high_cut;
                        break;
                }
            }
            if (not_pan) UpdateTXDisplayVars();

            int i = DttSP.SetTXFilter(0, tx_filter_low_cut, tx_filter_high_cut);
            int i1 = DttSP.SetTXFilter(1, tx_filter_low_cut, tx_filter_high_cut);

            if (i != 0 && i1 != 0)
                MessageBox.Show("Error in DttSP.SetTXFilters (SetFilter): " + i);
        }

        public static void UpdateRXDisplayVars()
        {
            int low = 0, high = 0;
            if (rx_filter_low_cut < 0 && rx_filter_high_cut <= 0)
            {
                high = 0;
                if (rx_filter_low_cut >= -910)
                    low = -1000;
                else
                    low = (int)(rx_filter_low_cut * 1.1);
            }
            else if (rx_filter_low_cut >= 0 && rx_filter_high_cut > 0)
            {
                low = 0;
                if (rx_filter_high_cut <= 910)
                    high = 1000;
                else
                    high = (int)(rx_filter_high_cut * 1.1);
            }
            else if (rx_filter_low_cut < 0 && rx_filter_high_cut > 0)
            {
                int max_edge = Math.Max(-rx_filter_low_cut, rx_filter_high_cut);
                low = (int)(max_edge * -1.1);
                high = (int)(max_edge * 1.1);
            }
        }

        public static void UpdateTXDisplayVars()
        {
            int low = 0, high = 0;
            if (tx_filter_low_cut < 0 && tx_filter_high_cut <= 0)
            {
                high = 0;
                if (tx_filter_low_cut >= -910)
                    low = -1000;
                else
                    low = (int)(tx_filter_low_cut * 1.1);
            }
            else if (tx_filter_low_cut >= 0 && tx_filter_high_cut > 0)
            {
                low = 0;
                if (tx_filter_high_cut <= 910)
                    high = 1000;
                else
                    high = (int)(tx_filter_high_cut * 1.1);
            }
            else if (tx_filter_low_cut < 0 && tx_filter_high_cut > 0)
            {
                int max_edge = Math.Max(-tx_filter_low_cut, tx_filter_high_cut);
                low = (int)(max_edge * -1.1);
                high = (int)(max_edge * 1.1);
            }

            TXDisplayLow = low;
            TXDisplayHigh = high;
        }

        public static void SetRXFilters(int low, int high)
        {
            SetFilter(low, high);
        }

        public static void SetRXFilterSubRX(int low, int high)
        {
            SetFilterSubRX(low, high);
        }

        #endregion
    }
}
