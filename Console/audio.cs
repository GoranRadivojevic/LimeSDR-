//=================================================================
// audio.cs
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

/*
 * LimeSDR#  
 * Copyright (C)2018 YT7PWR Goran Radivojevic
 * contact via email at: yt7pwr@mts.rs
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PaError = System.Int32;
//using FMdemod;

namespace PowerSDR
{
    #region Enum

    public enum TestChannels
    {
        Left = 0,
        Right,
        Both,
        None,
    }

    public enum MuteChannels
    {
        Left = 0,
        Right,
        Both,
        None,
    }

    #endregion

    public class Audio
    {
        #region Enums

        public enum AudioState
        {
            DTTSP = 0,
            CW,
            SINL_COSR,
            SINL_SINR,
            SINL_NOR,
            COSL_SINR,
            NOL_SINR,
            NOL_NOR,
            PIPE,
            SWITCH,
            CW_COSL_SINR,
        }

        public enum SignalSource
        {
            SOUNDCARD,
            SINE,
            NOISE,
            TRIANGLE,
            SAWTOOTH,
        }

        #endregion

        #region PowerSDR Specific Variables

        unsafe private static PA19.PaStreamCallback callbackVAC = new PA19.PaStreamCallback(CallbackVAC);

        public static int callback_return = 0;
        public static int VAC_callback_return = 0;
        private static bool vac_callback = false;
        public static TestChannels ChannelTest = TestChannels.Left;

        public static byte[] iq_data_l;
        public static byte[] iq_data_r;
        public static Console console = null;
        unsafe private static void* stream1;    // input
        private static int block_size2 = 2048;
        public static float[] phase_buf_l;
        public static float[] phase_buf_r;
        public static bool phase = false;
        public static bool wave_record = false;
        public static bool wave_playback = false;
        public static bool voice_message_playback = false;
        public static bool voice_message_record = false;
        public static WaveFileWriter wave_file_writer;
        public static WaveFileReader wave_file_reader;
        public static VoiceMsgWaveFileWriter voice_msg_file_writer;
        public static VoiceMsgWaveFileReader voice_msg_file_reader;
        public static bool two_tone = false;
        public static bool high_pwr_am = false;
        public static bool testing = false;
        public static bool TX_out_1_2 = true;             // 4 channel audio card
        public static bool RX_input_1_2 = true;           // 4 channel audio card
        private static float[] zero_buffer = new float[2048];
        public static bool VACDirectI_Q = false;
        public static bool PrimaryDirectI_Q = false;
        public static bool Primary_RXshift_enabled = false;
        public static bool VAC_RXshift_enabled = false;
        private delegate void DebugCallbackFunction(string name);
        public static bool debug = false;
        public static bool large_vac_buffer = false;
        public static bool echo_enable = false;
        private static float[] mix_buffer = new float[2048];
        private static float[] mix_buffer_delay = new float[2048];
        private static bool audio_stop = false;
        private static bool VAC_audio_stop = false;
        public static bool CTCSS = false;
        public static MuteChannels vac_mute_ch = MuteChannels.None;
        public static MuteChannels mute_ch = MuteChannels.None;
        public static Mutex mox_mutex = new Mutex();

        private static float echo_gain = 0.0f;
        public static float EchoGain
        {
            get { return echo_gain; }
            set { echo_gain = value; }
        }

        private static bool spike = false;
        public static bool Spike
        {
            get { return spike; }
            set { spike = value; }
        }

        private static double input_source_scale = 1.0;
        public static double InputSourceScale
        {
            get { return input_source_scale; }
            set { input_source_scale = value; }
        }

        private static SignalSource current_input_signal = SignalSource.SOUNDCARD;
        public static SignalSource CurrentInputSignal
        {
            get { return current_input_signal; }
            set { current_input_signal = value; }
        }

        private static SignalSource current_output_signal = SignalSource.SOUNDCARD;
        public static SignalSource CurrentOutputSignal
        {
            get { return current_output_signal; }
            set { current_output_signal = value; }
        }

        private static bool record_rx_preprocessed = true;
        public static bool RecordRXPreProcessed
        {
            get { return record_rx_preprocessed; }
            set { record_rx_preprocessed = value; }
        }

        private static bool record_tx_preprocessed = false;
        public static bool RecordTXPreProcessed
        {
            get { return record_tx_preprocessed; }
            set { record_tx_preprocessed = value; }
        }

        private static float peak = 0.0f;
        public static float Peak
        {
            get { return peak; }
            set { peak = value; }
        }

        private static bool vox_enabled = false;
        public static bool VOXEnabled
        {
            get { return vox_enabled; }
            set { vox_enabled = value; }
        }

        private static float vox_threshold = 0.001f;
        public static float VOXThreshold
        {
            get { return vox_threshold; }
            set { vox_threshold = value; }
        }

        public static double TXScale
        {
            get { return high_swr_scale * radio_volume; }
        }

        private static double high_swr_scale = 1.0;
        public static double HighSWRScale
        {
            get { return high_swr_scale; }
            set { high_swr_scale = value; }
        }

        private static double mic_preamp = 1.0;
        public static double MicPreamp
        {
            get { return mic_preamp; }
            set { mic_preamp = value; }
        }

        private static double voice_message_playback_preamp = 1.0;
        public static double VoiceMessagePlaybackPreamp             // yt7pwr
        {
            get { return voice_message_playback_preamp; }
            set { voice_message_playback_preamp = value; }
        }

        private static double wave_preamp = 1.0;
        public static double WavePreamp
        {
            get { return wave_preamp; }
            set { wave_preamp = value; }
        }

        private static double monitor_volume_left = 0.0;
        public static double MonitorVolumeLeft
        {
            get { return monitor_volume_left; }
            set { monitor_volume_left = value; }
        }

        private static double monitor_volume_right = 0.0;
        public static double MonitorVolumeRight
        {
            get { return monitor_volume_right; }
            set { monitor_volume_right = value; }
        }

        private static double vac_volume_left = 0.0;
        public static double VACVolumeLeft
        {
            get { return vac_volume_left; }
            set { vac_volume_left = value; }
        }

        private static double vac_volume_right = 0.0;
        public static double VACVolumeRight
        {
            get { return vac_volume_right; }
            set { vac_volume_right = value; }
        }

        private static double radio_volume = 0.0;
        public static double RadioVolume
        {
            get { return radio_volume; }
            set { radio_volume = value; }
        }

        private static bool next_mox = false;
        public static bool NextMox
        {
            get { return next_mox; }
            set { next_mox = value; }
        }

        private static int ramp_samples = (int)(RXsample_rate * 0.005);
        private static double ramp_step = 1.0 / ramp_samples;
        private static int ramp_count = 0;
        private static double ramp_val = 0.0;

        private static bool ramp_down = false;
        public static bool RampDown
        {
            get { return ramp_down; }
            set
            {
                ramp_down = value;
                ramp_samples = (int)(RXsample_rate * 0.005);
                ramp_step = 1.0 / ramp_samples;
                ramp_count = 0;
                ramp_val = 1.0;
            }
        }

        private static bool ramp_up = false;
        public static bool RampUp
        {
            get { return ramp_up; }
            set
            {
                ramp_up = value;
                ramp_samples = (int)(RXsample_rate * 0.005);
                ramp_step = 1.0 / ramp_samples;
                ramp_count = 0;
                ramp_val = 0.0;
            }
        }

        private static int ramp_up_num = 1;
        public static int RampUpNum
        {
            get { return ramp_up_num; }
            set { ramp_up_num = value; }
        }

        private static int switch_count = 1;
        public static int SwitchCount
        {
            get { return switch_count; }
            set { switch_count = value; }
        }

        private static AudioState current_audio_state1 = AudioState.DTTSP;
        public static AudioState CurrentAudioState1
        {
            get { return current_audio_state1; }
            set { current_audio_state1 = value; }
        }

        private static AudioState next_audio_state1 = AudioState.NOL_NOR;
        public static AudioState NextAudioState1
        {
            get { return next_audio_state1; }
            set { next_audio_state1 = value; }
        }

        private static AudioState save_audio_state1 = AudioState.NOL_NOR;
        public static AudioState SaveAudioState1
        {
            get { return save_audio_state1; }
            set { save_audio_state1 = value; }
        }

        private static double sine_freq1 = 1250.0;
        private static double phase_step1 = sine_freq1 / RXsample_rate * 2 * Math.PI;
        private static double phase_accumulator1 = 0.0;

        private static double sine_freq2 = 1900.0;
        private static double phase_step2 = sine_freq2 / RXsample_rate * 2 * Math.PI;
        private static double phase_accumulator2 = 0.0;

        public static double SineFreq1
        {
            get { return sine_freq1; }
            set
            {
                sine_freq1 = value;
                phase_step1 = sine_freq1 / RXsample_rate * 2 * Math.PI;
            }
        }

        public static double SineFreq2
        {
            get { return sine_freq2; }
            set
            {
                sine_freq2 = value;
                phase_step2 = sine_freq2 / RXsample_rate * 2 * Math.PI;
            }
        }

        #region VAC Variables

        public static RingBufferFloat rb_vacIN_l;
        public static RingBufferFloat rb_vacIN_r;
        public static RingBufferFloat rb_vacOUT_l;
        public static RingBufferFloat rb_vacOUT_r;

        private static float[] res_inl;
        private static float[] res_inr;
        private static float[] res_outl;
        private static float[] res_outr;
        private static float[] vac_outl;
        private static float[] vac_outr;
        private static float[] vac_inl;
        private static float[] vac_inr;
        private static float[] ctcss_outl = new float[2048];
        private static float[] ctcss_outr = new float[2048];

        unsafe public static void* resampPtrIn_l;
        unsafe public static void* resampPtrIn_r;
        unsafe public static void* resampPtrOut_l;
        unsafe public static void* resampPtrOut_r;

        public static bool vac_RXresample = false;
        //public static bool vac_TXresample = false;

        #endregion

        #endregion

        #region Properties

        private static double rx_shift = 0.0f;
        public static double RXShift
        {
            get { return rx_shift; }
            set { rx_shift = value; }
        }

        private static float primary_iq_phase = 0.0f;
        public static float PrimaryIQPhase
        {
            set
            {
                float new_phase = 0.001f * value;
                primary_iq_phase = new_phase;
            }
        }

        private static float primary_iq_gain = 1.0f;
        public static float PrimaryIQGain
        {
            set
            {
                float new_gain = 1.0f + 0.001f * value;
                primary_iq_gain = new_gain;
            }
        }

        private static float vac_iq_phase = 0.0f;
        public static float VACIQPhase
        {
            set
            {
                float new_phase = 0.001f * value;
                vac_iq_phase = new_phase;
            }
        }

        private static float vac_iq_gain = 1.0f;
        public static float VACIQGain
        {
            set
            {
                float new_gain = 1.0f + 0.001f * value;
                vac_iq_gain = new_gain;
            }
        }

        private static bool primary_correct_iq = true;
        public static bool PrimaryCorrectIQ
        {
            set { primary_correct_iq = value; }
        }

        private static bool vac_correct_iq = true;
        public static bool VACCorrectIQ
        {
            set { vac_correct_iq = value; }
        }

        private static int thread_no = 0;                       // yt7pwr
        private static bool mox = false;
        unsafe public static bool MOX
        {
            set
            {
                if (vac_primary_audiodev)      // TX
                {
                    try
                    {
                        Win32.EnterCriticalSection(cs_vac);
                        rb_vacIN_l.Reset();
                        rb_vacIN_r.Reset();
                        rb_vacOUT_l.Reset();
                        rb_vacOUT_r.Reset();
                        Win32.LeaveCriticalSection(cs_vac);
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
                }

                mox = value;
            }
        }

        unsafe public static void* cs_vac;

        private static bool mon = false;
        public static bool MON
        {
            get { return mon; }
            set { mon = value; }
        }

        private static bool vac_mon = false;    // yt7pwr
        public static bool VAC_MON
        {
            get { return vac_mon; }
            set { vac_mon = value; }
        }

        private static bool vac_enabled = false;
        public static bool VACEnabled
        {
            set { vac_enabled = value; }
            get { return vac_enabled; }
        }

        private static bool vac_rb_reset = false;
        public static bool VACRBReset
        {
            set
            {
                vac_rb_reset = value;
            }
            get { return vac_rb_reset; }
        }

        private static double vac_preamp = 1.0;
        public static double VACPreamp
        {
            get { return vac_preamp; }
            set
            {
                //Debug.WriteLine("vac_preamp: "+value.ToString("f3"));
                vac_preamp = value;
            }
        }

        private static double vac_rx_scale = 1.0;
        public static double VACRXScale
        {
            get { return vac_rx_scale; }
            set
            {
                //Debug.WriteLine("vac_rx_scale: "+value.ToString("f3"));
                vac_rx_scale = value;
            }
        }

        private static DSPMode dsp_mode = DSPMode.LSB;
        public static DSPMode CurDSPMode
        {
            set { dsp_mode = value; }
        }

        private static int RXsample_rate = 48000;
        public static int RXSampleRate
        {
            get { return RXsample_rate; }
            set
            {
                RXsample_rate = value;
                SineFreq1 = sine_freq1;
                SineFreq2 = sine_freq2;
            }
        }

        private static int TXsample_rate = 48000;
        public static int TXSampleRate
        {
            get { return TXsample_rate; }
            set
            {
                TXsample_rate = value;
                //SineFreq1 = sine_freq1;
                //SineFreq2 = sine_freq2;
            }
        }

        private static int sample_rateVAC = 48000;
        public static int SampleRateVAC
        {
            get { return sample_rateVAC; }
            set
            {
                sample_rateVAC = value;
            }
        }

        private static int rx_block_size = 2048;
        public static int RXBlockSize
        {
            get { return rx_block_size; }
            set { rx_block_size = value; }
        }

        private static int tx_block_size = 2048;
        public static int TXBlockSize
        {
            get { return tx_block_size; }
            set { tx_block_size = value; }
        }

        private static int block_size_vac = 2048;
        public static int BlockSizeVAC
        {
            get { return block_size_vac; }
            set { block_size_vac = value; }
        }

        private static double audio_volts1 = 2.23;
        public static double AudioVolts1
        {
            get { return audio_volts1; }
            set { audio_volts1 = value; }
        }

        //[patch_7
        private static bool vac_primary_audiodev = true;
        public static bool VACPrimaryAudiodevice
        {
            get { return vac_primary_audiodev; }
            set { vac_primary_audiodev = value; }
        }
        //patch_7]

        private static SoundCard soundcard = SoundCard.LimeSDR;
        public static SoundCard CurSoundCard
        {
            set { soundcard = value; }
        }

        private static bool vox_active = false;
        public static bool VOXActive
        {
            get { return vox_active; }
            set { vox_active = value; }
        }

        private static int num_channels = 2;
        public static int NumChannels
        {
            set { num_channels = value; }
        }

        private static int host1 = 0;
        public static int Host1
        {
            set { host1 = value; }
        }

        private static int host2 = 0;
        public static int Host2
        {
            set { host2 = value; }
        }

        private static int input_dev1 = 0;
        public static int Input1
        {
            set { input_dev1 = value; }
        }

        private static int input_dev2 = 0;
        public static int Input2
        {
            set { input_dev2 = value; }
        }

        private static int input_dev3 = 0;      // yt7pwr
        public static int Input3
        {
            set { input_dev3 = value; }
        }

        private static int output_dev1 = 0;
        public static int Output1
        {
            set { output_dev1 = value; }
        }

        private static int output_dev2 = 0;
        public static int Output2
        {
            set { output_dev2 = value; }
        }

        private static int output_dev3 = 0;
        public static int Output3
        {
            set { output_dev3 = value; }
        }

        private static int latency1 = 50;
        public static int Latency1
        {
            set { latency1 = value; }
        }

        private static int latency2 = 50;
        public static int Latency2
        {
            set { latency2 = value; }
        }

        #endregion

        #region Callback Routines

        #region classic callback

        unsafe public static int CallbackVAC(void* input, void* output, int frameCount,         // changes yt7pwr
            PA19.PaStreamCallbackTimeInfo* timeInfo, int statusFlags, void* userData)
        {
            try
            {
                if (audio_stop)
                    return callback_return;
#if (WIN64)
                Int64* array_ptr = (Int64*)input;
                float* in_l_ptr1 = (float*)array_ptr[0];
                float* in_r_ptr1 = (float*)array_ptr[1];
                array_ptr = (Int64*)output;
                float* out_l_ptr1 = (float*)array_ptr[0];
                float* out_r_ptr1 = (float*)array_ptr[1];
#endif

#if (WIN32)
                int* array_ptr = (int*)input;
                float* in_l_ptr1 = (float*)array_ptr[0];
                float* in_r_ptr1 = null;

                if (console.SetupForm.VACchNumber == 1)     // mono
                    in_r_ptr1 = (float*)array_ptr[0];
                else
                    in_r_ptr1 = (float*)array_ptr[1];       // stereo

                array_ptr = (int*)output;
                float* out_l_ptr1 = (float*)array_ptr[0];
                float* out_r_ptr1 = null;

                if (console.SetupForm.VACchNumber == 1)     // mono
                    out_r_ptr1 = (float*)array_ptr[0];
                else
                    out_r_ptr1 = (float*)array_ptr[1];      // stereo
#endif

                if (vac_rb_reset)
                {
                    vac_rb_reset = false;
                    ClearBuffer(out_l_ptr1, frameCount);
                    ClearBuffer(out_r_ptr1, frameCount);
                    Win32.EnterCriticalSection(cs_vac);
                    rb_vacIN_l.Reset();
                    rb_vacIN_r.Reset();
                    rb_vacOUT_l.Reset();
                    rb_vacOUT_r.Reset();
                    Win32.LeaveCriticalSection(cs_vac);
                    return 0;
                }

                #region VOX
                if (vox_enabled)
                {
                    float* vox_l = null, vox_r = null;
                    vox_l = in_l_ptr1;
                    vox_r = in_r_ptr1;

                    if (dsp_mode == DSPMode.LSB ||
                        dsp_mode == DSPMode.USB ||
                        dsp_mode == DSPMode.DSB ||
                        dsp_mode == DSPMode.AM ||
                        dsp_mode == DSPMode.SAM ||
                        dsp_mode == DSPMode.FMN)
                    {
                        Peak = MaxSample(vox_l, vox_r, frameCount);

                        // compare power to threshold
                        if (Peak > vox_threshold)
                            vox_active = true;
                        else
                            vox_active = false;
                    }
                }
                #endregion

                if (mox || voice_message_record)
                {
                    if (vac_RXresample)
                    {
                        if (dsp_mode == DSPMode.CWU || dsp_mode == DSPMode.CWL)
                        {
                            if ((rb_vacOUT_l.ReadSpace() >= frameCount) && 
                                (rb_vacOUT_r.ReadSpace() >= frameCount))
                            {
                                Win32.EnterCriticalSection(cs_vac);
                                rb_vacOUT_l.ReadPtr(out_l_ptr1, frameCount);
                                rb_vacOUT_r.ReadPtr(out_r_ptr1, frameCount);
                                Win32.LeaveCriticalSection(cs_vac);
                            }
                            else
                            {
                                Win32.EnterCriticalSection(cs_vac);
                                rb_vacOUT_l.ReadPtr(out_l_ptr1, frameCount);
                                rb_vacOUT_r.ReadPtr(out_r_ptr1, frameCount);
                                Win32.LeaveCriticalSection(cs_vac);
                                VACDebug("rb_vacIN underflow CBvac");
                            }

                            ScaleBuffer(out_l_ptr1, out_l_ptr1, frameCount, (float)vac_rx_scale);
                            ScaleBuffer(out_r_ptr1, out_r_ptr1, frameCount, (float)vac_rx_scale);
                        }
                        else
                        {
                            int outsamps = 0;
                            fixed (float* res_inl_ptr = &(res_inl[0]))
                            fixed (float* res_inr_ptr = &(res_inr[0]))
                            {
                                DttSP.DoResamplerF(in_l_ptr1, res_inl_ptr, frameCount, &outsamps, resampPtrIn_l);
                                DttSP.DoResamplerF(in_r_ptr1, res_inr_ptr, frameCount, &outsamps, resampPtrIn_r);

                                if (rb_vacIN_l.WriteSpace() >= outsamps)
                                {
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.WritePtr(res_inl_ptr, outsamps);
                                    rb_vacIN_r.WritePtr(res_inr_ptr, outsamps);
                                    Win32.LeaveCriticalSection(cs_vac);

                                    if (vac_primary_audiodev &&
                                        (wave_record && record_tx_preprocessed))
                                        wave_file_writer.AddWriteBuffer(res_inl_ptr, res_inr_ptr);
                                }
                                else
                                {
                                    VACDebug("rb_vacIN overflow CBvac");
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.WritePtr(res_inl_ptr, outsamps);
                                    rb_vacIN_r.WritePtr(res_inr_ptr, outsamps);
                                    Win32.LeaveCriticalSection(cs_vac);
                                }

                                if (vac_mon && mon)
                                {
                                    ScaleBuffer(in_l_ptr1, out_l_ptr1, frameCount, (float)vac_rx_scale);
                                    ScaleBuffer(in_r_ptr1, out_r_ptr1, frameCount, (float)vac_rx_scale);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((dsp_mode == DSPMode.CWU || dsp_mode == DSPMode.CWL) && vac_mon)
                        {
                            DttSP.CWMonitorExchange(out_l_ptr1, out_r_ptr1, frameCount);
                        }
                        else if (dsp_mode != DSPMode.CWU && dsp_mode != DSPMode.CWL)
                        {
                            //DttSP.ExchangeSamples(0, in_l_ptr1, in_r_ptr1, out_l_ptr1, out_r_ptr1, console.TXBlockSize);
                            //console.limeSDR.device.Transmit_async(out_l_ptr1, out_r_ptr1, frameCount);

                            if ((rb_vacIN_l.WriteSpace() >= frameCount) && (rb_vacIN_r.WriteSpace() >= frameCount))
                            {
                                Win32.EnterCriticalSection(cs_vac);
                                rb_vacIN_l.WritePtr(out_l_ptr1, frameCount);
                                rb_vacIN_r.WritePtr(out_r_ptr1, frameCount);
                                Win32.LeaveCriticalSection(cs_vac);

                                if (vac_primary_audiodev &&
                                    (wave_record && record_tx_preprocessed))
                                    wave_file_writer.AddWriteBuffer(in_l_ptr1, in_r_ptr1);
                            }
                            else
                            {
                                Win32.EnterCriticalSection(cs_vac);
                                rb_vacIN_l.WritePtr(out_l_ptr1, frameCount);
                                rb_vacIN_r.WritePtr(out_r_ptr1, frameCount);
                                Win32.LeaveCriticalSection(cs_vac);
                                VACDebug("rb_vacIN overflow CBvac");
                            }

                            if (vac_mon && mon)
                            {
                                ScaleBuffer(in_l_ptr1, out_l_ptr1, frameCount, (float)vac_rx_scale);
                                ScaleBuffer(in_r_ptr1, out_r_ptr1, frameCount, (float)vac_rx_scale);
                            }
                        }
                    }
                }
                else
                {
                    if (console.CurrentModel == Model.LimeSDR)
                    {
                        if (rb_vacOUT_l.ReadSpace() >= frameCount)
                        {
                            Win32.EnterCriticalSection(cs_vac);
                            rb_vacOUT_l.ReadPtr(out_l_ptr1, frameCount);
                            rb_vacOUT_r.ReadPtr(out_r_ptr1, frameCount);
                            Win32.LeaveCriticalSection(cs_vac);
                        }
                        else
                        {
                            Debug.Write("rb_vacOUT underflow! \n");
                            Win32.EnterCriticalSection(cs_vac);
                            rb_vacOUT_l.ReadPtr(out_l_ptr1, frameCount);
                            rb_vacOUT_r.ReadPtr(out_r_ptr1, frameCount);
                            Win32.LeaveCriticalSection(cs_vac);
                        }

                        if (wave_record && !record_rx_preprocessed && vac_primary_audiodev)
                            wave_file_writer.AddWriteBuffer(out_l_ptr1, out_r_ptr1);
                    }
                    else
                    {
                        //if ((rb_vacOUT_l.ReadSpace() >= frameCount))
                        {
                            Win32.EnterCriticalSection(cs_vac);
                            rb_vacOUT_l.ReadPtr(out_l_ptr1, frameCount);
                            rb_vacOUT_r.ReadPtr(out_r_ptr1, frameCount);
                            Win32.LeaveCriticalSection(cs_vac);

                            if (wave_record && !record_rx_preprocessed && vac_primary_audiodev)
                                wave_file_writer.AddWriteBuffer(out_l_ptr1, out_r_ptr1);
                        }
                        /*else
                        {
                            ClearBuffer(out_l_ptr1, frameCount);
                            ClearBuffer(out_r_ptr1, frameCount);
                            VACDebug("rb_vacOUT underflow CBvac");
                        }*/
                    }
                }

                if (VACPrimaryAudiodevice && mox && !mon && !VACDirectI_Q)
                {
                    ClearBuffer(out_l_ptr1, frameCount);
                    ClearBuffer(out_r_ptr1, frameCount);
                }

                if (mox && wave_record && record_tx_preprocessed && vac_primary_audiodev)
                    wave_file_writer.AddWriteBuffer(out_l_ptr1, out_r_ptr1);

                double vol_l = vac_volume_left;
                double vol_r = vac_volume_right;

                if (!vac_primary_audiodev)
                {
                    vol_l = console.AF / 100.0;
                    vol_r = console.AF / 100.0;
                }

                if (Audio.VACDirectI_Q && !mox)
                {
                    ScaleBuffer(out_l_ptr1, out_l_ptr1, frameCount, (float)vac_rx_scale);  // RX gain
                    ScaleBuffer(out_r_ptr1, out_r_ptr1, frameCount, (float)vac_rx_scale);
                }
                else
                {
                    switch (vac_mute_ch)
                    {
                        case MuteChannels.Left:
                            ScaleBuffer(out_r_ptr1, out_r_ptr1, frameCount, (float)vol_r);
                            ScaleBuffer(out_r_ptr1, out_l_ptr1, frameCount, 1.0f);
                            break;

                        case MuteChannels.Right:
                            {
                                ScaleBuffer(out_l_ptr1, out_l_ptr1, frameCount, (float)vol_l);
                                ScaleBuffer(out_l_ptr1, out_r_ptr1, frameCount, 1.0f);
                            }
                            break;

                        case MuteChannels.Both:
                            {
                                ClearBuffer(out_l_ptr1, frameCount);
                                ClearBuffer(out_r_ptr1, frameCount);
                            }
                            break;

                        case MuteChannels.None:
                            {
                                ScaleBuffer(out_l_ptr1, out_l_ptr1, frameCount, (float)vol_l);
                                ScaleBuffer(out_r_ptr1, out_r_ptr1, frameCount, (float)vol_r);
                            }
                            break;
                    }

                    if (console.CurrentModel == Model.LimeSDR)
                    {
                        if (console.CurrentDisplayMode == DisplayMode.SCOPE ||
                            console.CurrentDisplayMode == DisplayMode.PANASCOPE)
                            DoScope(out_l_ptr1, frameCount);
                    }
                }

                return VAC_callback_return;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 0;
            }
        }

        #endregion

        #endregion

        #region Buffer Operations

        unsafe private static void ChangeVolume(float[] inbuf, float* outbuf, int samples, float scale)     // yt7pwr
        {
            for (int i = 0; i < samples; i++)
                outbuf[i] = inbuf[i] * scale;
        }

        unsafe private static void ClearBuffer(float* buf, int samples)
        {
            Win32.memset(buf, 0, samples * sizeof(float));
        }

        unsafe private static void CopyBuffer(float* inbuf, float* outbuf, int samples)
        {
            Win32.memcpy(outbuf, inbuf, (uint)(samples * sizeof(float)));
        }

        unsafe public static void ScaleBuffer(float* inbuf, float* outbuf, int samples, float scale)
        {
            for (int i = 0; i < samples; i++)
                outbuf[i] = inbuf[i] * scale;
        }

        unsafe public static float MaxSample(float* buf, int samples)
        {
            float max = float.MinValue;
            for (int i = 0; i < samples; i++)
                if (buf[i] > max) max = buf[i];

            return max;
        }

        unsafe public static float MaxSample(float* buf1, float* buf2, int samples)
        {
            float max = float.MinValue;
            for (int i = 0; i < samples; i++)
            {
                if (buf1[i] > max) max = buf1[i];
                if (buf2[i] > max) max = buf2[i];
            }
            return max;
        }

        unsafe public static float MinSample(float* buf, int samples)
        {
            float min = float.MaxValue;
            for (int i = 0; i < samples; i++)
                if (buf[i] < min) min = buf[i];

            return min;
        }

        unsafe public static float MinSample(float* buf1, float* buf2, int samples)
        {
            float min = float.MaxValue;
            for (int i = 0; i < samples; i++)
            {
                if (buf1[i] < min) min = buf1[i];
                if (buf2[i] < min) min = buf2[i];
            }

            return min;
        }

        unsafe private static void CorrectIQBuffer(float* buff_l, float* buff_r,
            float gain, float phase, int samples)
        {
            try
            {
                for (int i = 0; i < samples; i++)
                {
                    buff_r[i] += phase * buff_l[i];
                    buff_l[i] *= gain;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        // returns updated phase accumulator
        unsafe public static double SineWave(float* buf, int samples, double phase, double freq)
        {
            double phase_step = freq / RXsample_rate * 2 * Math.PI;
            double cosval = Math.Cos(phase);
            double sinval = Math.Sin(phase);
            double cosdelta = Math.Cos(phase_step);
            double sindelta = Math.Sin(phase_step);
            double tmpval;

            for (int i = 0; i < samples; i++)
            {
                tmpval = cosval * cosdelta - sinval * sindelta;
                sinval = cosval * sindelta + sinval * cosdelta;
                cosval = tmpval;

                buf[i] = (float)(sinval);

                phase += phase_step;

                if (phase > Math.PI)
                    phase -= 2 * Math.PI;
            }

            return phase;
        }

        // returns updated phase accumulator
        unsafe public static double CosineWave(float* buf, int samples, double phase, double freq)
        {
            double phase_step = freq / RXsample_rate * 2 * Math.PI;
            double cosval = Math.Cos(phase);
            double sinval = Math.Sin(phase);
            double cosdelta = Math.Cos(phase_step);
            double sindelta = Math.Sin(phase_step);
            double tmpval;

            for (int i = 0; i < samples; i++)
            {
                tmpval = cosval * cosdelta - sinval * sindelta;
                sinval = cosval * sindelta + sinval * cosdelta;
                cosval = tmpval;

                buf[i] = (float)(cosval);

                phase += phase_step;

                if (phase > Math.PI)
                    phase -= 2 * Math.PI;
            }

            return phase;
        }

        unsafe public static void SineWave2Tone(float* buf, int samples,
            double phase1, double phase2,
            double freq1, double freq2,
            out double updated_phase1, out double updated_phase2)
        {
            double phase_step1 = freq1 / RXsample_rate * 2 * Math.PI;
            double cosval1 = Math.Cos(phase1);
            double sinval1 = Math.Sin(phase1);
            double cosdelta1 = Math.Cos(phase_step1);
            double sindelta1 = Math.Sin(phase_step1);

            double phase_step2 = freq2 / RXsample_rate * 2 * Math.PI;
            double cosval2 = Math.Cos(phase2);
            double sinval2 = Math.Sin(phase2);
            double cosdelta2 = Math.Cos(phase_step2);
            double sindelta2 = Math.Sin(phase_step2);
            double tmpval;

            for (int i = 0; i < samples; i++)
            {
                tmpval = cosval1 * cosdelta1 - sinval1 * sindelta1;
                sinval1 = cosval1 * sindelta1 + sinval1 * cosdelta1;
                cosval1 = tmpval;

                tmpval = cosval2 * cosdelta2 - sinval2 * sindelta2;
                sinval2 = cosval2 * sindelta2 + sinval2 * cosdelta2;
                cosval2 = tmpval;

                buf[i] = (float)(sinval1 * 0.5 + sinval2 * 0.5);

                phase1 += phase_step1;
                phase2 += phase_step2;
            }

            updated_phase1 = phase1;
            updated_phase2 = phase2;
        }

        unsafe public static void CosineWave2Tone(float* buf, int samples,
            double phase1, double phase2,
            double freq1, double freq2,
            out double updated_phase1, out double updated_phase2)
        {
            double phase_step1 = freq1 / RXsample_rate * 2 * Math.PI;
            double cosval1 = Math.Cos(phase1);
            double sinval1 = Math.Sin(phase1);
            double cosdelta1 = Math.Cos(phase_step1);
            double sindelta1 = Math.Sin(phase_step1);

            double phase_step2 = freq2 / RXsample_rate * 2 * Math.PI;
            double cosval2 = Math.Cos(phase2);
            double sinval2 = Math.Sin(phase2);
            double cosdelta2 = Math.Cos(phase_step2);
            double sindelta2 = Math.Sin(phase_step2);
            double tmpval;

            for (int i = 0; i < samples; i++)
            {
                tmpval = cosval1 * cosdelta1 - sinval1 * sindelta1;
                sinval1 = cosval1 * sindelta1 + sinval1 * cosdelta1;
                cosval1 = tmpval;

                tmpval = cosval2 * cosdelta2 - sinval2 * sindelta2;
                sinval2 = cosval2 * sindelta2 + sinval2 * cosdelta2;
                cosval2 = tmpval;

                buf[i] = (float)(cosval1 * 0.5 + cosval2 * 0.5);

                phase1 += phase_step1;
                phase2 += phase_step2;
            }

            updated_phase1 = phase1;
            updated_phase2 = phase2;
        }


        private static Random r = new Random();
        private static double y2 = 0.0;
        private static bool use_last = false;
        private static double boxmuller(double m, double s)
        {
            double x1, x2, w, y1;
            if (use_last)		        /* use value from previous call */
            {
                y1 = y2;
                use_last = false;
            }
            else
            {
                do
                {
                    x1 = (2.0 * r.NextDouble() - 1.0);
                    x2 = (2.0 * r.NextDouble() - 1.0);
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                use_last = true;
            }

            return (m + y1 * s);
        }
        unsafe public static void Noise(float* buf, int samples)
        {
            for (int i = 0; i < samples; i++)
            {
                buf[i] = (float)boxmuller(0.0, 0.2);
            }
        }

        private static double tri_val = 0.0;
        private static int tri_direction = 1;
        unsafe public static void Triangle(float* buf, int samples, double freq)
        {
            double step = freq / RXsample_rate * 2 * tri_direction;
            for (int i = 0; i < samples; i++)
            {
                buf[i] = (float)tri_val;
                tri_val += step;
                if (tri_val >= 1.0 || tri_val <= -1.0)
                {
                    step = -step;
                    tri_val += 2 * step;
                    if (step < 0) tri_direction = -1;
                    else tri_direction = 1;
                }
            }
        }

        private static double saw_val = 0.0;
        private static int saw_direction = 1;
        unsafe public static void Sawtooth(float* buf, int samples, double freq)
        {
            double step = freq / RXsample_rate * saw_direction;
            for (int i = 0; i < samples; i++)
            {
                buf[i] = (float)saw_val;
                saw_val += step;
                if (saw_val >= 1.0) saw_val -= 2.0;
                if (saw_val <= -1.0) saw_val += 2.0;
            }
        }

        #endregion

        #region Misc Routines

        public static void VACDebug(string s)
        {
            Debug.WriteLine(s);

            if (debug && !console.ConsoleClosing)
                console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Audio:" + s);
        }

        unsafe public static void InitVAC()
        {
            try
            {
                int lcm = 61440000;
                int buf_size = Math.Max(16384, (RXsample_rate / sample_rateVAC) * console.BlockSize2 + 2);

                rb_vacOUT_l = new RingBufferFloat(buf_size);
                rb_vacOUT_l.Restart(buf_size);

                rb_vacOUT_r = new RingBufferFloat(buf_size);
                rb_vacOUT_r.Restart(buf_size);

                rb_vacIN_l = new RingBufferFloat(buf_size);
                rb_vacIN_l.Restart(buf_size);

                rb_vacIN_r = new RingBufferFloat(buf_size);
                rb_vacIN_r.Restart(buf_size);

                if (sample_rateVAC != RXsample_rate)
                {
                    switch (RXsample_rate)
                    {
                        case 192000:
                        case 384000:
                        case 768000:
                        case 1536000:
                            lcm = 61440000;
                            break;

                        case 3072000:
                        case 6144000:
                        case 12288000:
                            lcm = 92160000;
                            break;
                        case 24576000:
                        case 49152000:
                            lcm = 98304000;
                            break;

                        case 2304000:
                            lcm = 73728000;
                            break;

                        default:
                            lcm = 61440000;
                            break;
                    }

                    vac_RXresample = true;

                    res_inl = new float[4 * buf_size];
                    res_inr = new float[4 * buf_size];

                    resampPtrIn_l = DttSP.NewResamplerF_LimeSDR(lcm, sample_rateVAC, RXsample_rate);
                    resampPtrIn_r = DttSP.NewResamplerF_LimeSDR(lcm, sample_rateVAC, RXsample_rate);

                    res_outl = new float[4 * buf_size];
                    res_outr = new float[4 * buf_size];

                    resampPtrOut_l = DttSP.NewResamplerF_LimeSDR(lcm, RXsample_rate, sample_rateVAC);
                    resampPtrOut_r = DttSP.NewResamplerF_LimeSDR(lcm, RXsample_rate, sample_rateVAC);
                }
                else
                {
                    vac_RXresample = false;
                }

                /*if (sample_rateVAC != TXsample_rate)
                {
                    vac_TXresample = true;

                    if (res_outl == null) res_outl = new float[buf_size];
                    if (res_outr == null) res_outr = new float[buf_size];

                    resampPtrOut_l = DttSP.NewResamplerF(TXsample_rate, sample_rateVAC);
                    resampPtrOut_r = DttSP.NewResamplerF(TXsample_rate, sample_rateVAC);
                }
                else
                {
                    vac_TXresample = false;
                }*/
                
                cs_vac = (void*)0x0;
                cs_vac = Win32.NewCriticalSection();

                if (Win32.InitializeCriticalSectionAndSpinCount(cs_vac, 0x00000080) == 0)
                {
                    vac_enabled = false;
                    VACDebug("VAC CriticalSection Failed!");
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && console.ConsoleClosing)
                    VACDebug(ex.ToString());
            }
        }

        /*unsafe private static void CleanUpVAC()
        {
            try
            {
                Win32.DeleteCriticalSection(cs_vac);
                rb_vacOUT_l = null;
                rb_vacOUT_r = null;
                rb_vacIN_l = null;
                rb_vacIN_r = null;

                res_outl = null;
                res_outr = null;
                res_inl = null;
                res_inr = null;

                resampPtrIn_l = null;
                resampPtrIn_r = null;
                resampPtrOut_l = null;
                resampPtrOut_r = null;

                Win32.DestroyCriticalSection(cs_vac);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug)
                    VACDebug(ex.ToString());
            }
        }*/

        public static ArrayList GetPAHosts() // returns a text list of driver types
        {
            try
            {
                ArrayList a = new ArrayList();

                for (int i = 0; i < PA19.PA_GetHostApiCount(); i++)
                {
                    PA19.PaHostApiInfo info = PA19.PA_GetHostApiInfo(i);
                    a.Add(info.name);
                }

                return a;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }

        public static ArrayList GetPAInputdevices(int hostIndex)
        {
            try
            {
                ArrayList a = new ArrayList();
                PA19.PaHostApiInfo hostInfo = PA19.PA_GetHostApiInfo(hostIndex);

                for (int i = 0; i < hostInfo.deviceCount; i++)
                {
                    int devIndex = PA19.PA_HostApiDeviceIndexToDeviceIndex(hostIndex, i);
                    PA19.PADeviceInfo devInfo = PA19.PA_GetDeviceInfo(devIndex);
                    if (devInfo.maxInputChannels > 0)
                        a.Add(new PADeviceInfo(devInfo.name, i));
                }

                return a;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }

        public static ArrayList GetPAOutputdevices(int hostIndex)
        {
            try
            {
                ArrayList a = new ArrayList();
                PA19.PaHostApiInfo hostInfo = PA19.PA_GetHostApiInfo(hostIndex);

                for (int i = 0; i < hostInfo.deviceCount; i++)
                {
                    int devIndex = PA19.PA_HostApiDeviceIndexToDeviceIndex(hostIndex, i);
                    PA19.PADeviceInfo devInfo = PA19.PA_GetDeviceInfo(devIndex);
                    if (devInfo.maxOutputChannels > 0)
                        a.Add(new PADeviceInfo(devInfo.name, i)/* + " - " + devIndex*/);
                }

                return a;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }

        unsafe public static bool Start()           // changes yt7pwr
        {
            if (audio_stop || VAC_audio_stop)
                return false;

            bool retval = false;
            phase_buf_l = new float[rx_block_size];
            phase_buf_r = new float[rx_block_size];

            if (!vac_enabled)
                DttSP.EnableResamplerF(0, 0, 0);   //disable resampler

            if (console.CurrentModel == Model.LimeSDR)
            {
                retval = true;
                buf_l = new float[console.RXBlockSize * 8];
                buf_r = new float[console.RXBlockSize * 8];
                out_buf_l = new float[console.RXBlockSize * 8];
                out_buf_r = new float[console.RXBlockSize * 8];
                vac_outl = new float[console.RXBlockSize * 8];
                vac_outr = new float[console.RXBlockSize * 8];
                vac_inl = new float[console.RXBlockSize * 8];
                vac_inr = new float[console.RXBlockSize * 8];
            }
            else if (console.CurrentModel == Model.MiniLimeSDR)
            {

            }

            #region VAC

            if (vac_enabled)
            {
                int num_chan = console.SetupForm.VACchNumber;
                vac_rb_reset = true;
                vac_callback = true;
                retval = StartVACAudio(ref callbackVAC, (uint)block_size2, sample_rateVAC,
                    host2, input_dev2, output_dev2, num_chan, latency2);
                Thread.Sleep(100);
                vac_callback = false;
            }
            #endregion

            return retval;
        }

        public unsafe static bool StartVACAudio(ref PA19.PaStreamCallback callback,
            uint block_size, double sample_rate, int host_api_index, int input_dev_index,
            int output_dev_index, int num_channels, int latency_ms)         // yt7pwr
        {
            try
            {
                int in_dev = PA19.PA_HostApiDeviceIndexToDeviceIndex(host_api_index, input_dev_index);
                int out_dev = PA19.PA_HostApiDeviceIndexToDeviceIndex(host_api_index, output_dev_index);

                PA19.PaStreamParameters inparam = new PA19.PaStreamParameters();
                PA19.PaStreamParameters outparam = new PA19.PaStreamParameters();

                inparam.device = in_dev;
                inparam.channelCount = num_channels;
                inparam.sampleFormat = PA19.paFloat32 | PA19.paNonInterleaved;
                inparam.suggestedLatency = ((float)latency_ms / 1000);

                outparam.device = out_dev;
                outparam.channelCount = num_channels;
                outparam.sampleFormat = PA19.paFloat32 | PA19.paNonInterleaved;
                outparam.suggestedLatency = ((float)latency_ms / 1000);

                if (host_api_index == PA19.PA_HostApiTypeIdToHostApiIndex(PA19.PaHostApiTypeId.paWASAPI) &&
                    (console.WinVer == WindowsVersion.Windows7 || console.WinVer == WindowsVersion.Windows8 ||
                    console.WinVer == WindowsVersion.WindowsVista || console.WinVer == WindowsVersion.Windows10))
                {
                    PA19.PaWasapiStreamInfo stream_info = new PA19.PaWasapiStreamInfo();
                    stream_info.hostApiType = PA19.PaHostApiTypeId.paWASAPI;
                    stream_info.version = 1;
                    stream_info.flags = 0;
                    stream_info.threadPriority = PA19.PaWasapiThreadPriority.eThreadPriorityNone;
                    stream_info.size = (UInt32)sizeof(PA19.PaWasapiStreamInfo);
                    inparam.hostApiSpecificStreamInfo = &stream_info;
                    outparam.hostApiSpecificStreamInfo = &stream_info;
                }

                int error = PA19.PA_OpenStream(out stream1, &inparam, &outparam, sample_rate, block_size, 0, callback, 0, 0);

                if (error != 0)
                {
#if(WIN32)
                    MessageBox.Show(PA19.PA_GetErrorText(error), "PortAudio Error\n VAC settings error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
#if(WIN64)
                    string err = PA19.PA_GetErrorText(error);
                    byte[] berr = System.Text.Encoding.Unicode.GetBytes(err);
                    string text = Encoding.UTF8.GetString(berr);

                    MessageBox.Show(text, "PortAudio Error\n VAC settings error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                    return false;
                }

                error = PA19.PA_StartStream(stream1);

                if (error != 0)
                {
#if(WIN32)
                    MessageBox.Show(PA19.PA_GetErrorText(error), "PortAudio Error\nVAC settings error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
#if(WIN64)
                    string err = PA19.PA_GetErrorText(error);
                    byte[] berr = System.Text.Encoding.Unicode.GetBytes(err);
                    string text = Encoding.UTF8.GetString(berr);

                    MessageBox.Show(text, "PortAudio Error\nVAC settings error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public unsafe static void StopAudioVAC()
        {
            try
            {
                VAC_audio_stop = true;
                Thread.Sleep(100);
                PA19.PA_AbortStream(stream1);
                PA19.PA_CloseStream(stream1);
                VAC_audio_stop = false;
                Win32.DestroyCriticalSection(cs_vac);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && console.ConsoleClosing)
                    VACDebug(ex.ToString());
            }
        }

        public unsafe static PA19.PaStreamInfo GetStreamInfo()
        {
            PA19.PaStreamInfo stream_info = new PA19.PaStreamInfo();
            PA19.PaStreamInfo stream_info5;
            stream_info = PA19.PA_GetStreamInfo(stream1);

            return stream_info;
        }

        #endregion

        #region Scope Stuff

        private static int scope_samples_per_pixel = 512;
        public static int ScopeSamplesPerPixel
        {
            get { return scope_samples_per_pixel; }
            set { scope_samples_per_pixel = value; }
        }

        private static int scope_display_width = 704;
        public static int ScopeDisplayWidth
        {
            get { return scope_display_width; }
            set { scope_display_width = value; }
        }

        private static float scope_level = 0.0f;
        public static float ScopeLevel
        {
            set { scope_level = value / 50; }
        }

        private static int scope_sample_index = 0;
        private static int scope_pixel_index = 0;
        private static float scope_pixel_min = float.MaxValue;
        private static float scope_pixel_max = float.MinValue;
        public static float[] scope_min = new float[2048];
        public static float[] scope_max = new float[2048];

        unsafe private static void DoScope(float* buf, int frameCount)
        {
            try
            {
                if (console.CurrentDisplayEngine == DisplayEngine.GDI_PLUS)
                {
                    if (scope_min == null || scope_min.Length != scope_display_width)
                    {
                        if (Display_GDI.ScopeMin == null || Display_GDI.ScopeMin.Length < scope_display_width)
                            Display_GDI.ScopeMin = new float[scope_display_width];
                        scope_min = Display_GDI.ScopeMin;
                    }
                    if (scope_max == null || scope_max.Length != scope_display_width)
                    {
                        if (Display_GDI.ScopeMax == null || Display_GDI.ScopeMax.Length < scope_display_width)
                            Display_GDI.ScopeMax = new float[scope_display_width];
                        scope_max = Display_GDI.ScopeMax;
                    }
                }
#if(DirectX)
                else if (console.CurrentDisplayEngine == DisplayEngine.DIRECT_X)
                {
                    if (scope_min == null || scope_min.Length != scope_display_width)
                    {
                        if (Display_DirectX.ScopeMin == null || Display_DirectX.ScopeMin.Length < scope_display_width)
                            Display_DirectX.ScopeMin = new float[scope_display_width];
                        {
                            scope_min = Display_DirectX.ScopeMin;
                            scope_pixel_index = 0;
                        }
                    }
                    if (scope_max == null || scope_max.Length != scope_display_width)
                    {
                        if (Display_DirectX.ScopeMax == null || Display_DirectX.ScopeMax.Length < scope_display_width)
                            Display_DirectX.ScopeMax = new float[scope_display_width];
                        {
                            scope_max = Display_DirectX.ScopeMax;
                            scope_pixel_index = 0;
                        }
                    }
                }
#endif
                for (int i = 0; i < frameCount; i++)
                {
                    if (buf[i] * scope_level < scope_pixel_min) scope_pixel_min = buf[i] * scope_level;
                    if (buf[i] * scope_level > scope_pixel_max) scope_pixel_max = buf[i] * scope_level;

                    scope_sample_index++;

                    if (scope_pixel_index >= scope_display_width)
                        scope_pixel_index = 0;

                    if (scope_sample_index >= scope_samples_per_pixel)
                    {
                        scope_sample_index = 0;
                        scope_min[scope_pixel_index] = scope_pixel_min;
                        scope_max[scope_pixel_index] = scope_pixel_max;

                        scope_pixel_min = float.MaxValue;
                        scope_pixel_max = float.MinValue;

                        scope_pixel_index++;

                        if (scope_pixel_index >= scope_display_width)
                            scope_pixel_index = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "DoScope error!\n" + ex.ToString());
            }
        }

        #endregion

        static float[] buf_l;
        static float[] buf_r;
        static float[] out_buf_l;
        static float[] out_buf_r;

        unsafe public static void LimeSDR_Callback_RX0(int thread, float* in_l_ptr1, float* in_r_ptr1, float* output,
            int frameCount, bool mox)
        {
            try
            {
                fixed (float* out_l_ptr1 = &out_buf_l[0])
                fixed (float* out_r_ptr1 = &out_buf_r[0])
                {
                    if (wave_playback)
                        wave_file_reader.GetPlayBuffer(in_l_ptr1, in_r_ptr1);
                    else if (wave_record && !mox && record_rx_preprocessed)
                        wave_file_writer.AddWriteBuffer(in_l_ptr1, in_r_ptr1);
                    else if (wave_record && mox && !vac_primary_audiodev && record_tx_preprocessed)
                        wave_file_writer.AddWriteBuffer(in_l_ptr1, in_r_ptr1);

                    if (phase)
                    {
                        Marshal.Copy(new IntPtr(in_l_ptr1), phase_buf_l, 0, frameCount);
                        Marshal.Copy(new IntPtr(in_r_ptr1), phase_buf_r, 0, frameCount);
                    }

                    float* in_l = null, in_l_VAC = null, in_r = null, in_r_VAC = null, out_l = null, out_r = null;

                    if (!mox && !voice_message_record)   // rx
                    {
                        if (!console.RX_IQ_channel_swap)
                        {
                            in_r = in_l_ptr1;
                            in_l = in_r_ptr1;

                            out_l = out_l_ptr1;
                            out_r = out_r_ptr1;
                        }
                        else
                        {
                            in_r = in_r_ptr1;
                            in_l = in_l_ptr1;

                            out_l = out_r_ptr1;
                            out_r = out_l_ptr1;
                        }
                    }
                    else if (mox && !voice_message_record)
                    {       // tx
                        if (console.TX_IQ_channel_swap)
                        {
                            in_r = in_l_ptr1;
                            in_l = in_r_ptr1;

                            out_r = out_l_ptr1;
                            out_l = out_r_ptr1;
                        }
                        else
                        {
                            in_r = in_r_ptr1;
                            in_l = in_l_ptr1;

                            out_l = out_l_ptr1;
                            out_r = out_r_ptr1;
                        }

                        if (voice_message_playback)
                        {
                            voice_msg_file_reader.GetPlayBuffer(in_l, in_r);
                        }
                    }
                    else if (voice_message_record)
                    {
                        in_l = in_l_ptr1;
                        in_r = in_r_ptr1;
                        out_l = out_l_ptr1;
                        out_r = out_r_ptr1;
                    }

                    if (voice_message_record)
                    {
                        try
                        {
                            if (vac_enabled)
                            {
                                if (rb_vacIN_l.ReadSpace() >= frameCount &&
                                    rb_vacIN_r.ReadSpace() >= frameCount)
                                {
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                    rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                    Win32.LeaveCriticalSection(cs_vac);
                                }
                                else
                                {
                                    ClearBuffer(in_l_ptr1, frameCount);
                                    ClearBuffer(in_r_ptr1, frameCount);
                                    VACDebug("rb_vacIN underflow VoiceMsg record");
                                }
                            }

                            ScaleBuffer(in_l, in_l, frameCount, (float)mic_preamp);
                            ScaleBuffer(in_r, in_r, frameCount, (float)mic_preamp);
                            voice_msg_file_writer.AddWriteBuffer(in_l, in_r);
                        }
                        catch (Exception ex)
                        {
                            VACDebug("Audio: " + ex.ToString());
                        }
                    }

                    switch (current_audio_state1)
                    {
                        case AudioState.DTTSP:
                            // scale input with mic preamp
                            if ((mox || voice_message_record) && !vac_enabled &&
                                (dsp_mode == DSPMode.LSB ||
                                dsp_mode == DSPMode.USB ||
                                dsp_mode == DSPMode.DSB ||
                                dsp_mode == DSPMode.AM ||
                                dsp_mode == DSPMode.SAM ||
                                dsp_mode == DSPMode.FMN))
                            {
                                if (wave_playback)
                                {
                                    ScaleBuffer(in_l, in_l, frameCount, (float)wave_preamp);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)wave_preamp);
                                }
                                else
                                {
                                    ScaleBuffer(in_l, in_l, frameCount, (float)mic_preamp);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)mic_preamp);
                                }
                            }

                            #region Input Signal Source

                            switch (current_input_signal)
                            {
                                case SignalSource.SOUNDCARD:
                                    break;
                                case SignalSource.SINE:
                                    if (console.RX_IQ_channel_swap)
                                    {
                                        SineWave(in_r, frameCount, phase_accumulator1, sine_freq1);
                                        phase_accumulator1 = CosineWave(in_l, frameCount, phase_accumulator1, sine_freq1);
                                    }
                                    else
                                    {
                                        SineWave(in_l, frameCount, phase_accumulator1, sine_freq1);
                                        phase_accumulator1 = CosineWave(in_r, frameCount, phase_accumulator1 + 0.0001f, sine_freq1);
                                    }
                                    ScaleBuffer(in_l, in_l, frameCount, (float)input_source_scale);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)input_source_scale);
                                    break;
                                case SignalSource.NOISE:
                                    Noise(in_l, frameCount);
                                    Noise(in_r, frameCount);
                                    break;
                                case SignalSource.TRIANGLE:
                                    Triangle(in_l, frameCount, sine_freq1);
                                    CopyBuffer(in_l, in_r, frameCount);
                                    break;
                                case SignalSource.SAWTOOTH:
                                    Sawtooth(in_l, frameCount, sine_freq1);
                                    CopyBuffer(in_l, in_r, frameCount);
                                    break;
                            }

                            #endregion

                            if (vac_enabled && rb_vacIN_l != null && rb_vacIN_r != null &&
                                rb_vacOUT_l != null && rb_vacOUT_r != null
                                 && !voice_message_playback)
                            {
                                if (mox && !voice_message_record)
                                {
                                    if (rb_vacIN_l.ReadSpace() >= console.RXBlockSize &&
                                        rb_vacIN_r.ReadSpace() >= console.RXBlockSize)
                                    {
                                        Win32.EnterCriticalSection(cs_vac);
                                        rb_vacIN_l.ReadPtr(in_l, console.RXBlockSize);
                                        rb_vacIN_r.ReadPtr(in_r, console.RXBlockSize);
                                        Win32.LeaveCriticalSection(cs_vac);
                                    }
                                    else
                                    {
                                        Win32.EnterCriticalSection(cs_vac);
                                        rb_vacIN_l.ReadPtr(in_l, console.RXBlockSize);
                                        rb_vacIN_r.ReadPtr(in_r, console.RXBlockSize);
                                        Win32.LeaveCriticalSection(cs_vac);
                                        VACDebug("LimeSDR rb_vacIN underflow");
                                    }

                                    ScaleBuffer(in_l, in_l, frameCount, (float)vac_preamp);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)vac_preamp);

                                    //DttSP.ExchangeSamples(1, in_l, in_r, out_l, out_r, console.TXBlockSize);
                                }
                                else if (voice_message_record)
                                {

                                }
                                //else
                                DttSP.ExchangeSamples(0, in_l, in_r, out_l, out_r, frameCount);
                            }
                            else
                                DttSP.ExchangeSamples(0, in_l, in_r, out_l, out_r, frameCount);

                            #region Output Signal Source

                            switch (current_output_signal)
                            {
                                case SignalSource.SOUNDCARD:
                                    break;
                                case SignalSource.SINE:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Left:
                                            SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator1 = CosineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            break;
                                        case TestChannels.Right:
                                            SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator1 = CosineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            break;
                                        case TestChannels.Both:
                                            SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator1 = CosineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator2 = CosineWave(out_r_ptr1, frameCount, phase_accumulator2, sine_freq1);
                                            break;
                                    }
                                    break;
                                case SignalSource.NOISE:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Both:
                                            Noise(out_l_ptr1, frameCount);
                                            Noise(out_r_ptr1, frameCount);
                                            break;
                                        case TestChannels.Left:
                                            Noise(out_l_ptr1, frameCount);
                                            break;
                                        case TestChannels.Right:
                                            Noise(out_r_ptr1, frameCount);
                                            break;
                                    }
                                    break;
                                case SignalSource.TRIANGLE:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Both:
                                            Triangle(out_l_ptr1, frameCount, sine_freq1);
                                            CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                                            break;
                                        case TestChannels.Left:
                                            Triangle(out_l_ptr1, frameCount, sine_freq1);
                                            break;
                                        case TestChannels.Right:
                                            Triangle(out_r_ptr1, frameCount, sine_freq1);
                                            break;
                                    }
                                    break;
                                case SignalSource.SAWTOOTH:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Both:
                                            Sawtooth(out_l_ptr1, frameCount, sine_freq1);
                                            CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                                            break;
                                        case TestChannels.Left:
                                            Sawtooth(out_l_ptr1, frameCount, sine_freq1);
                                            break;
                                        case TestChannels.Right:
                                            Sawtooth(out_r_ptr1, frameCount, sine_freq1);
                                            break;
                                    }
                                    break;
                            }

                            #endregion

                            break;
                        case AudioState.CW:
                            if (next_audio_state1 == AudioState.SWITCH)
                            {
                                Win32.memset(in_l_ptr1, 0, frameCount * sizeof(float));
                                Win32.memset(in_r_ptr1, 0, frameCount * sizeof(float));

                                if (vac_enabled)
                                {
                                    if ((rb_vacIN_l.ReadSpace() >= frameCount) &&
                                        (rb_vacIN_r.ReadSpace() >= frameCount))
                                    {
                                        Win32.EnterCriticalSection(cs_vac);
                                        rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                        rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                        Win32.LeaveCriticalSection(cs_vac);
                                    }
                                    else
                                    {
                                        //VACDebug("rb_vacIN underflow Switch time CB1!");
                                    }
                                }

                                ClearBuffer(out_l, frameCount);
                                ClearBuffer(out_r, frameCount);

                                if (switch_count == 0) next_audio_state1 = AudioState.CW;
                                switch_count--;
                            }
                            else
                                DttSP.CWtoneExchange(out_l, out_r, frameCount);

                            break;
                        case AudioState.SINL_COSR:
                            if (two_tone)
                            {
                                double dump;

                                SineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out dump, out dump);

                                CosineWave2Tone(out_r_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                            }
                            else
                            {
                                SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                phase_accumulator1 = CosineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                            }
                            break;
                        case AudioState.SINL_SINR:
                            if (two_tone)
                            {
                                SineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);

                                CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                            }
                            else
                            {
                                phase_accumulator1 = SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                            }
                            break;
                        case AudioState.SINL_NOR:
                            if (two_tone)
                            {
                                SineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                                ClearBuffer(out_r_ptr1, frameCount);
                            }
                            else
                            {
                                phase_accumulator1 = SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                ClearBuffer(out_r_ptr1, frameCount);
                            }
                            break;
                        case AudioState.CW_COSL_SINR:
                            if (mox)
                            {
                                if (two_tone)
                                {
                                    double dump;

                                    if (console.tx_IF)
                                    {
                                        CosineWave2Tone(out_r, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + console.TX_IF_shift * 1e5, sine_freq2 + console.TX_IF_shift * 1e5,
                                            out dump, out dump);

                                        SineWave2Tone(out_l, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + console.TX_IF_shift * 1e5, sine_freq2 + console.TX_IF_shift * 1e5,
                                            out phase_accumulator1, out phase_accumulator2);
                                    }
                                    else
                                    {
                                        double osc = (console.VFOAFreq - console.LOSCFreq) * 1e6;

                                        CosineWave2Tone(out_r, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + osc, sine_freq2 + osc,
                                            out dump, out dump);

                                        SineWave2Tone(out_l, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + osc, sine_freq2 + osc,
                                            out phase_accumulator1, out phase_accumulator2);
                                    }
                                }
                                else
                                {
                                    if (console.tx_IF)
                                    {
                                        CosineWave(out_r, frameCount, phase_accumulator1, sine_freq1 + console.TX_IF_shift * 1e5);
                                        phase_accumulator1 = SineWave(out_l, frameCount, phase_accumulator1, sine_freq1 +
                                            console.TX_IF_shift * 1e5);
                                    }
                                    else
                                    {
                                        double osc = (console.VFOAFreq - console.LOSCFreq) * 1e6;
                                        CosineWave(out_r, frameCount, phase_accumulator1, sine_freq1 + osc);
                                        phase_accumulator1 = SineWave(out_l, frameCount, phase_accumulator1, sine_freq1 + osc);
                                    }
                                }

                                float iq_gain = 1.0f + (1.0f - (1.0f + 0.001f * (float)console.SetupForm.udDSPImageGainTX.Value));
                                float iq_phase = 0.001f * (float)console.SetupForm.udDSPImagePhaseTX.Value;

                                CorrectIQBuffer(out_l, out_r, iq_gain, iq_phase, frameCount);
                            }
                            break;
                        case AudioState.COSL_SINR:
                            if (two_tone)
                            {
                                double dump;

                                CosineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out dump, out dump);

                                SineWave2Tone(out_r_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                            }
                            else
                            {
                                CosineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                phase_accumulator1 = SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                            }


                            break;
                        case AudioState.NOL_SINR:
                            if (two_tone)
                            {
                                ClearBuffer(out_l_ptr1, frameCount);
                                SineWave2Tone(out_r_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                            }
                            else
                            {
                                ClearBuffer(out_l_ptr1, frameCount);
                                phase_accumulator1 = SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                            }
                            break;
                        case AudioState.NOL_NOR:
                            ClearBuffer(out_l_ptr1, frameCount);
                            ClearBuffer(out_r_ptr1, frameCount);
                            break;
                        case AudioState.PIPE:
                            CopyBuffer(in_l_ptr1, out_l_ptr1, frameCount);
                            CopyBuffer(in_r_ptr1, out_r_ptr1, frameCount);
                            break;
                        case AudioState.SWITCH:
                            if (!ramp_down && !ramp_up)
                            {
                                ClearBuffer(in_l, frameCount);
                                ClearBuffer(in_r, frameCount);
                                if (mox != next_mox) mox = next_mox;
                            }
                            if (vac_enabled)
                            {
                                if ((rb_vacIN_l.ReadSpace() >= frameCount) && 
                                    (rb_vacIN_r.ReadSpace() >= frameCount))
                                {
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                    rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                    Win32.LeaveCriticalSection(cs_vac);
                                }
                                else
                                {
                                    VACDebug("LimeSDR rb_vacIN underflow switch time!");
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                    rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                    Win32.LeaveCriticalSection(cs_vac);
                                }
                            }

                            DttSP.ExchangeSamples(0, in_l, in_r, out_l, out_r, frameCount);

                            if (ramp_down)
                            {
                                int i;
                                for (i = 0; i < frameCount; i++)
                                {
                                    float w = (float)Math.Sin(ramp_val * Math.PI / 2.0);
                                    out_l_ptr1[i] *= w;
                                    out_r_ptr1[i] *= w;
                                    ramp_val += ramp_step;
                                    if (++ramp_count >= ramp_samples)
                                    {
                                        ramp_down = false;
                                        break;
                                    }
                                }

                                if (ramp_down)
                                {
                                    for (; i < frameCount; i++)
                                    {
                                        out_l[i] = 0.0f;
                                        out_r[i] = 0.0f;
                                    }
                                }
                            }
                            else if (ramp_up)
                            {
                                for (int i = 0; i < frameCount; i++)
                                {
                                    float w = (float)Math.Sin(ramp_val * Math.PI / 2.0);
                                    out_l[i] *= w;
                                    out_r[i] *= w;
                                    ramp_val += ramp_step;
                                    if (++ramp_count >= ramp_samples)
                                    {
                                        ramp_up = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ClearBuffer(out_l, frameCount);
                                ClearBuffer(out_r, frameCount);
                            }

                            if (next_audio_state1 == AudioState.CW)
                            {
                                //cw_delay = 1;
                                DttSP.CWtoneExchange(out_l, out_r, frameCount);
                            }
                            else if (switch_count == 1)
                                DttSP.CWRingRestart();

                            switch_count--;
                            if (switch_count == ramp_up_num) RampUp = true;
                            if (switch_count == 0)
                                current_audio_state1 = next_audio_state1;
                            break;
                    }

                    if (vac_enabled &&
                        rb_vacIN_l != null && rb_vacIN_r != null &&
                        rb_vacOUT_l != null && rb_vacOUT_r != null)
                    {
                        fixed (float* outl_ptr = &(vac_outl[0]))
                        fixed (float* outr_ptr = &(vac_outr[0]))
                        {
                            if (!mox)
                            {
                                ScaleBuffer(out_l, outl_ptr, frameCount, (float)vac_rx_scale);
                                ScaleBuffer(out_r, outr_ptr, frameCount, (float)vac_rx_scale);
                            }
                            else if (mox && vac_mon && (dsp_mode == DSPMode.CWU || dsp_mode == DSPMode.CWL))
                            {
                                ScaleBuffer(out_l, outl_ptr, frameCount, 0.0f);
                                ScaleBuffer(out_r, outr_ptr, frameCount, 0.0f);
                            }
                            else // zero samples going back to VAC since TX monitor is off
                            {
                                ScaleBuffer(out_l, outl_ptr, frameCount, 0.0f);
                                ScaleBuffer(out_r, outr_ptr, frameCount, 0.0f);
                            }

                            if (!mox)
                            {
                                if (sample_rateVAC == RXsample_rate)
                                {
                                    if ((rb_vacOUT_l.WriteSpace() >= frameCount) &&
                                        (rb_vacOUT_r.WriteSpace() >= frameCount))
                                    {
                                        if (VACDirectI_Q)
                                        {
                                            if (vac_correct_iq)
                                                CorrectIQBuffer(in_l, in_r, vac_iq_gain, vac_iq_phase, frameCount);

                                            Win32.EnterCriticalSection(cs_vac);
                                            rb_vacOUT_l.WritePtr(in_l, frameCount);
                                            rb_vacOUT_r.WritePtr(in_r, frameCount);
                                            Win32.LeaveCriticalSection(cs_vac);
                                        }
                                        else
                                        {
                                            Win32.EnterCriticalSection(cs_vac);
                                            rb_vacOUT_l.WritePtr(outl_ptr, frameCount);
                                            rb_vacOUT_r.WritePtr(outr_ptr, frameCount);
                                            Win32.LeaveCriticalSection(cs_vac);
                                        }
                                    }
                                    else
                                    {
                                        VACDebug("LimeSDR rb_vacOUT overflow!");
                                        Win32.EnterCriticalSection(cs_vac);
                                        rb_vacOUT_l.WritePtr(outl_ptr, frameCount);
                                        rb_vacOUT_r.WritePtr(outr_ptr, frameCount);
                                        Win32.LeaveCriticalSection(cs_vac);
                                    }
                                }
                                else
                                {
                                    fixed (float* res_outl_ptr = &(res_outl[0]))
                                    fixed (float* res_outr_ptr = &(res_outr[0]))
                                    {
                                        int outsamps;

                                        if (VACDirectI_Q)
                                        {
                                            DttSP.DoResamplerF(in_l_ptr1, res_outl_ptr, frameCount, &outsamps, resampPtrOut_l);
                                            DttSP.DoResamplerF(in_r_ptr1, res_outr_ptr, frameCount, &outsamps, resampPtrOut_r);

                                            if ((rb_vacOUT_l.WriteSpace() >= outsamps) && (rb_vacOUT_r.WriteSpace() >= outsamps))
                                            {
                                                if (vac_correct_iq)
                                                    CorrectIQBuffer(res_outl_ptr, res_outr_ptr, vac_iq_gain, vac_iq_phase, frameCount);

                                                Win32.EnterCriticalSection(cs_vac);
                                                rb_vacOUT_l.WritePtr(res_outl_ptr, outsamps);
                                                rb_vacOUT_r.WritePtr(res_outr_ptr, outsamps);
                                                Win32.LeaveCriticalSection(cs_vac);
                                            }
                                            else
                                            {
                                                VACDebug("LimeSDR rb_vacOUT overflow!");

                                                Win32.EnterCriticalSection(cs_vac);
                                                rb_vacOUT_l.WritePtr(res_outl_ptr, outsamps);
                                                rb_vacOUT_r.WritePtr(res_outr_ptr, outsamps);
                                                Win32.LeaveCriticalSection(cs_vac);
                                            }
                                        }
                                        else
                                        {
                                            DttSP.DoResamplerF(outl_ptr, res_outl_ptr, frameCount, &outsamps, resampPtrOut_l);
                                            DttSP.DoResamplerF(outr_ptr, res_outr_ptr, frameCount, &outsamps, resampPtrOut_r);

                                            if ((rb_vacOUT_l.WriteSpace() >= outsamps) && 
                                                (rb_vacOUT_r.WriteSpace() >= outsamps))
                                            {
                                                Win32.EnterCriticalSection(cs_vac);
                                                rb_vacOUT_l.WritePtr(res_outl_ptr, outsamps);
                                                rb_vacOUT_r.WritePtr(res_outr_ptr, outsamps);
                                                Win32.LeaveCriticalSection(cs_vac);
                                            }
                                            else
                                            {
                                                VACDebug("LimeSDR rb_vacOUT overflow!");
                                                Win32.EnterCriticalSection(cs_vac);
                                                rb_vacOUT_l.WritePtr(res_outl_ptr, outsamps);
                                                rb_vacOUT_r.WritePtr(res_outr_ptr, outsamps);
                                                Win32.LeaveCriticalSection(cs_vac);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (console.CurrentDisplayMode == DisplayMode.SCOPE ||
                        console.CurrentDisplayMode == DisplayMode.PANASCOPE)
                        DoScope(out_l, frameCount);

                    double vol_l = monitor_volume_left;
                    double vol_r = monitor_volume_right;

                    if (mox)
                    {
                        vol_l = TXScale;
                        vol_r = TXScale;

                        if (high_pwr_am)
                        {
                            if (dsp_mode == DSPMode.AM ||
                                dsp_mode == DSPMode.SAM)
                            {
                                vol_l *= 1.414;
                                vol_r *= 1.414;
                            }
                        }
                    }

                    if (wave_record && !mox && !record_rx_preprocessed)
                        wave_file_writer.AddWriteBuffer(out_r_ptr1, out_l_ptr1);
                    else if (wave_record && mox && !record_tx_preprocessed)
                        wave_file_writer.AddWriteBuffer(out_r_ptr1, out_l_ptr1);

                    if (PrimaryDirectI_Q && !mox)
                    {
                        if (primary_correct_iq)
                        {
                            CorrectIQBuffer(in_l, in_r, primary_iq_gain, primary_iq_phase, frameCount);
                        }

                        ScaleBuffer(in_l, out_r, frameCount, (float)vol_l);
                        ScaleBuffer(in_r, out_l, frameCount, (float)vol_r);
                    }
                    else
                    {
                        if (mox)
                        {
                            ScaleBuffer(out_l, out_l, frameCount, (float)vol_l);
                            ScaleBuffer(out_r, out_r, frameCount, (float)vol_r);
                        }
                        else
                        {
                            switch (mute_ch)
                            {
                                case MuteChannels.Left:
                                    ScaleBuffer(out_r, out_r, frameCount, (float)vol_r);
                                    ScaleBuffer(out_r, out_l, frameCount, 1.0f);
                                    break;

                                case MuteChannels.Right:
                                    ScaleBuffer(out_l, out_l, frameCount, (float)vol_l);
                                    ScaleBuffer(out_l, out_r, frameCount, 1.0f);
                                    break;


                                case MuteChannels.Both:
                                    ClearBuffer(out_l, frameCount);
                                    ClearBuffer(out_r, frameCount);
                                    break;

                                case MuteChannels.None:
                                    ScaleBuffer(out_l, out_l, frameCount, (float)vol_l);
                                    ScaleBuffer(out_r, out_r, frameCount, (float)vol_r);
                                    break;
                            }
                        }
                    }

                    for (int i = 0; i < frameCount; i++)
                    {
                        output[i * 2] = out_buf_r[i];
                        output[i * 2 + 1] = out_buf_l[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        unsafe public static void LimeSDR_Callback_RX1(int thread, float* input, float* output, int frameCount)
        {
            try
            {
#if(WIN64)
                /*Int64* array_ptr = (Int64*)input;
                float* in_l_ptr1 = (float*)array_ptr[0];
                float* in_r_ptr1 = (float*)array_ptr[1];
                double* VAC_in = (double*)input;
                array_ptr = (Int64*)output;
                float* out_l_ptr1 = (float*)array_ptr[1];
                float* out_r_ptr1 = (float*)array_ptr[0];*/
#endif

#if(WIN32)
                int j = 0;
                for (int i = 0; i < frameCount; i++)
                {
                    buf_l[j] = input[i * 2];
                    buf_r[j] = input[i * 2 + 1];
                    j++;
                    //i++;
                }
#endif
                fixed (float* in_l_ptr1 = &buf_l[0])
                fixed (float* in_r_ptr1 = &buf_r[0])
                fixed (float* out_l_ptr1 = &out_buf_l[0])
                fixed (float* out_r_ptr1 = &out_buf_r[0])
                {
                    if (wave_playback)
                        wave_file_reader.GetPlayBuffer(in_l_ptr1, in_r_ptr1);
                    else if (wave_record && !mox && record_rx_preprocessed)
                        wave_file_writer.AddWriteBuffer(in_l_ptr1, in_r_ptr1);
                    else if (wave_record && mox && !vac_primary_audiodev && record_tx_preprocessed)
                        wave_file_writer.AddWriteBuffer(in_l_ptr1, in_r_ptr1);

                    if (phase)
                    {
                        //phase_mutex.WaitOne();
                        Marshal.Copy(new IntPtr(in_l_ptr1), phase_buf_l, 0, frameCount);
                        Marshal.Copy(new IntPtr(in_r_ptr1), phase_buf_r, 0, frameCount);
                        //phase_mutex.ReleaseMutex();
                    }

                    float* in_l = null, in_l_VAC = null, in_r = null, in_r_VAC = null, out_l = null, out_r = null;

                    if (!mox && !voice_message_record)   // rx
                    {
                        if (!console.RX_IQ_channel_swap)
                        {
                            in_r = in_l_ptr1;
                            in_l = in_r_ptr1;

                            out_l = out_l_ptr1;
                            out_r = out_r_ptr1;
                        }
                        else
                        {
                            in_r = in_r_ptr1;
                            in_l = in_l_ptr1;

                            out_l = out_r_ptr1;
                            out_r = out_l_ptr1;
                        }
                    }
                    else if (mox && !voice_message_record)
                    {       // tx
                        if (console.TX_IQ_channel_swap)
                        {
                            in_r = in_l_ptr1;
                            in_l = in_r_ptr1;

                            out_r = out_l_ptr1;
                            out_l = out_r_ptr1;
                        }
                        else
                        {
                            in_r = in_r_ptr1;
                            in_l = in_l_ptr1;

                            out_l = out_l_ptr1;
                            out_r = out_r_ptr1;
                        }

                        if (voice_message_playback)
                        {
                            voice_msg_file_reader.GetPlayBuffer(in_l, in_r);
                        }
                    }
                    else if (voice_message_record)
                    {
                        in_l = in_l_ptr1;
                        in_r = in_r_ptr1;
                        out_l = out_l_ptr1;
                        out_r = out_r_ptr1;
                    }

                    if (voice_message_record)
                    {
                        try
                        {
                            if (vac_enabled)
                            {
                                if (rb_vacIN_l.ReadSpace() >= frameCount &&
                                    rb_vacIN_r.ReadSpace() >= frameCount)
                                {
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                    rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                    Win32.LeaveCriticalSection(cs_vac);
                                }
                                else
                                {
                                    ClearBuffer(in_l_ptr1, frameCount);
                                    ClearBuffer(in_r_ptr1, frameCount);
                                    VACDebug("rb_vacIN underflow VoiceMsg record");
                                }
                            }

                            ScaleBuffer(in_l, in_l, frameCount, (float)mic_preamp);
                            ScaleBuffer(in_r, in_r, frameCount, (float)mic_preamp);
                            voice_msg_file_writer.AddWriteBuffer(in_l, in_r);
                        }
                        catch (Exception ex)
                        {
                            VACDebug("Audio: " + ex.ToString());
                        }
                    }

                    switch (current_audio_state1)
                    {
                        case AudioState.DTTSP:
                            // scale input with mic preamp
                            if ((mox || voice_message_record) && !vac_enabled &&
                                (dsp_mode == DSPMode.LSB ||
                                dsp_mode == DSPMode.USB ||
                                dsp_mode == DSPMode.DSB ||
                                dsp_mode == DSPMode.AM ||
                                dsp_mode == DSPMode.SAM ||
                                dsp_mode == DSPMode.FMN))
                            {
                                if (wave_playback)
                                {
                                    ScaleBuffer(in_l, in_l, frameCount, (float)wave_preamp);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)wave_preamp);
                                }
                                else
                                {
                                    ScaleBuffer(in_l, in_l, frameCount, (float)mic_preamp);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)mic_preamp);
                                }
                            }

                            #region Input Signal Source

                            switch (current_input_signal)
                            {
                                case SignalSource.SOUNDCARD:
                                    break;
                                case SignalSource.SINE:
                                    if (console.RX_IQ_channel_swap)
                                    {
                                        SineWave(in_r, frameCount, phase_accumulator1, sine_freq1);
                                        phase_accumulator1 = CosineWave(in_l, frameCount, phase_accumulator1, sine_freq1);
                                    }
                                    else
                                    {
                                        SineWave(in_l, frameCount, phase_accumulator1, sine_freq1);
                                        phase_accumulator1 = CosineWave(in_r, frameCount, phase_accumulator1 + 0.0001f, sine_freq1);
                                    }
                                    ScaleBuffer(in_l, in_l, frameCount, (float)input_source_scale);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)input_source_scale);
                                    break;
                                case SignalSource.NOISE:
                                    Noise(in_l, frameCount);
                                    Noise(in_r, frameCount);
                                    break;
                                case SignalSource.TRIANGLE:
                                    Triangle(in_l, frameCount, sine_freq1);
                                    CopyBuffer(in_l, in_r, frameCount);
                                    break;
                                case SignalSource.SAWTOOTH:
                                    Sawtooth(in_l, frameCount, sine_freq1);
                                    CopyBuffer(in_l, in_r, frameCount);
                                    break;
                            }

                            #endregion

                            if (vac_enabled &&
                                rb_vacIN_l != null && rb_vacIN_r != null &&
                                rb_vacOUT_l != null && rb_vacOUT_r != null
                                 && !voice_message_playback)
                            {
                                /*if (mox && !voice_message_record)
                                {
                                    if (rb_vacIN_l.ReadSpace() >= frameCount &&
                                        rb_vacIN_r.ReadSpace() >= frameCount)
                                    {
                                        Win32.EnterCriticalSection(cs_vac);
                                        rb_vacIN_l.ReadPtr(in_l, frameCount);
                                        rb_vacIN_r.ReadPtr(in_r, frameCount);
                                        Win32.LeaveCriticalSection(cs_vac);
                                    }
                                    else
                                    {
                                        ClearBuffer(in_l, frameCount);
                                        ClearBuffer(in_r, frameCount);
                                        VACDebug("rb_vacIN underflow CB1");
                                    }

                                    ScaleBuffer(in_l, in_l, frameCount, (float)vac_preamp);
                                    ScaleBuffer(in_r, in_r, frameCount, (float)vac_preamp);

                                    if (echo_enable && (dsp_mode != DSPMode.DIGL || dsp_mode != DSPMode.DIGU))
                                    {
                                        if (!echo_pause)
                                        {
                                            echoRB.WritePtr(in_l, frameCount);

                                            if (echoRB.ReadSpace() > echo_delay - 2)
                                            {
                                                EchoMixer(in_l, in_r, frameCount);
                                            }
                                        }
                                    }
                                }
                                else if (voice_message_record)
                                {

                                }*/
                            }

                            DttSP.ExchangeSamples(thread, in_l, in_r, out_l, out_r, frameCount);

                            #region Output Signal Source

                            switch (current_output_signal)
                            {
                                case SignalSource.SOUNDCARD:
                                    break;
                                case SignalSource.SINE:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Left:
                                            SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator1 = CosineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            break;
                                        case TestChannels.Right:
                                            SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator1 = CosineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            break;
                                        case TestChannels.Both:
                                            SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator1 = CosineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                            phase_accumulator2 = CosineWave(out_r_ptr1, frameCount, phase_accumulator2, sine_freq1);
                                            break;
                                    }
                                    break;
                                case SignalSource.NOISE:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Both:
                                            Noise(out_l_ptr1, frameCount);
                                            Noise(out_r_ptr1, frameCount);
                                            break;
                                        case TestChannels.Left:
                                            Noise(out_l_ptr1, frameCount);
                                            break;
                                        case TestChannels.Right:
                                            Noise(out_r_ptr1, frameCount);
                                            break;
                                    }
                                    break;
                                case SignalSource.TRIANGLE:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Both:
                                            Triangle(out_l_ptr1, frameCount, sine_freq1);
                                            CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                                            break;
                                        case TestChannels.Left:
                                            Triangle(out_l_ptr1, frameCount, sine_freq1);
                                            break;
                                        case TestChannels.Right:
                                            Triangle(out_r_ptr1, frameCount, sine_freq1);
                                            break;
                                    }
                                    break;
                                case SignalSource.SAWTOOTH:
                                    switch (ChannelTest)
                                    {
                                        case TestChannels.Both:
                                            Sawtooth(out_l_ptr1, frameCount, sine_freq1);
                                            CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                                            break;
                                        case TestChannels.Left:
                                            Sawtooth(out_l_ptr1, frameCount, sine_freq1);
                                            break;
                                        case TestChannels.Right:
                                            Sawtooth(out_r_ptr1, frameCount, sine_freq1);
                                            break;
                                    }
                                    break;
                            }

                            #endregion

                            break;
                        case AudioState.CW:
                            if (next_audio_state1 == AudioState.SWITCH)
                            {
                                Win32.memset(in_l_ptr1, 0, frameCount * sizeof(float));
                                Win32.memset(in_r_ptr1, 0, frameCount * sizeof(float));

                                if (vac_enabled)
                                {
                                    if ((rb_vacIN_l.ReadSpace() >= frameCount) && (rb_vacIN_r.ReadSpace() >= frameCount))
                                    {
                                        Win32.EnterCriticalSection(cs_vac);
                                        rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                        rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                        Win32.LeaveCriticalSection(cs_vac);
                                    }
                                    else
                                    {
                                        //VACDebug("rb_vacIN underflow Switch time CB1!");
                                    }
                                }

                                ClearBuffer(out_l, frameCount);
                                ClearBuffer(out_r, frameCount);

                                if (switch_count == 0) next_audio_state1 = AudioState.CW;
                                switch_count--;
                            }
                            else
                                DttSP.CWtoneExchange(out_l, out_r, frameCount);

                            break;
                        case AudioState.SINL_COSR:
                            if (two_tone)
                            {
                                double dump;

                                SineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out dump, out dump);

                                CosineWave2Tone(out_r_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                            }
                            else
                            {
                                SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                phase_accumulator1 = CosineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                            }
                            break;
                        case AudioState.SINL_SINR:
                            if (two_tone)
                            {
                                SineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);

                                CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                            }
                            else
                            {
                                phase_accumulator1 = SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                CopyBuffer(out_l_ptr1, out_r_ptr1, frameCount);
                            }
                            break;
                        case AudioState.SINL_NOR:
                            if (two_tone)
                            {
                                SineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                                ClearBuffer(out_r_ptr1, frameCount);
                            }
                            else
                            {
                                phase_accumulator1 = SineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                ClearBuffer(out_r_ptr1, frameCount);
                            }
                            break;
                        case AudioState.CW_COSL_SINR:
                            if (mox)
                            {
                                if (two_tone)
                                {
                                    double dump;

                                    if (console.tx_IF)
                                    {
                                        CosineWave2Tone(out_r, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + console.TX_IF_shift * 1e5, sine_freq2 + console.TX_IF_shift * 1e5,
                                            out dump, out dump);

                                        SineWave2Tone(out_l, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + console.TX_IF_shift * 1e5, sine_freq2 + console.TX_IF_shift * 1e5,
                                            out phase_accumulator1, out phase_accumulator2);
                                    }
                                    else
                                    {
                                        double osc = (console.VFOAFreq - console.LOSCFreq) * 1e6;

                                        CosineWave2Tone(out_r, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + osc, sine_freq2 + osc,
                                            out dump, out dump);

                                        SineWave2Tone(out_l, frameCount,
                                            phase_accumulator1, phase_accumulator2,
                                            sine_freq1 + osc, sine_freq2 + osc,
                                            out phase_accumulator1, out phase_accumulator2);
                                    }
                                }
                                else
                                {
                                    if (console.tx_IF)
                                    {
                                        CosineWave(out_r, frameCount, phase_accumulator1, sine_freq1 + console.TX_IF_shift * 1e5);
                                        phase_accumulator1 = SineWave(out_l, frameCount, phase_accumulator1, sine_freq1 +
                                            console.TX_IF_shift * 1e5);
                                    }
                                    else
                                    {
                                        double osc = (console.VFOAFreq - console.LOSCFreq) * 1e6;
                                        CosineWave(out_r, frameCount, phase_accumulator1, sine_freq1 + osc);
                                        phase_accumulator1 = SineWave(out_l, frameCount, phase_accumulator1, sine_freq1 + osc);
                                    }
                                }

                                float iq_gain = 1.0f + (1.0f - (1.0f + 0.001f * (float)console.SetupForm.udDSPImageGainTX.Value));
                                float iq_phase = 0.001f * (float)console.SetupForm.udDSPImagePhaseTX.Value;

                                CorrectIQBuffer(out_l, out_r, iq_gain, iq_phase, frameCount);
                            }
                            break;
                        case AudioState.COSL_SINR:
                            if (two_tone)
                            {
                                double dump;

                                CosineWave2Tone(out_l_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out dump, out dump);

                                SineWave2Tone(out_r_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                            }
                            else
                            {
                                CosineWave(out_l_ptr1, frameCount, phase_accumulator1, sine_freq1);
                                phase_accumulator1 = SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                            }


                            break;
                        case AudioState.NOL_SINR:
                            if (two_tone)
                            {
                                ClearBuffer(out_l_ptr1, frameCount);
                                SineWave2Tone(out_r_ptr1, frameCount,
                                    phase_accumulator1, phase_accumulator2,
                                    sine_freq1, sine_freq2,
                                    out phase_accumulator1, out phase_accumulator2);
                            }
                            else
                            {
                                ClearBuffer(out_l_ptr1, frameCount);
                                phase_accumulator1 = SineWave(out_r_ptr1, frameCount, phase_accumulator1, sine_freq1);
                            }
                            break;
                        case AudioState.NOL_NOR:
                            ClearBuffer(out_l_ptr1, frameCount);
                            ClearBuffer(out_r_ptr1, frameCount);
                            break;
                        case AudioState.PIPE:
                            CopyBuffer(in_l_ptr1, out_l_ptr1, frameCount);
                            CopyBuffer(in_r_ptr1, out_r_ptr1, frameCount);
                            break;
                        case AudioState.SWITCH:
                            if (!ramp_down && !ramp_up)
                            {
                                ClearBuffer(in_l, frameCount);
                                ClearBuffer(in_r, frameCount);
                                if (mox != next_mox) mox = next_mox;
                            }
                            if (vac_enabled)
                            {
                                if ((rb_vacIN_l.ReadSpace() >= frameCount) && (rb_vacIN_r.ReadSpace() >= frameCount))
                                {
                                    Win32.EnterCriticalSection(cs_vac);
                                    rb_vacIN_l.ReadPtr(in_l_ptr1, frameCount);
                                    rb_vacIN_r.ReadPtr(in_r_ptr1, frameCount);
                                    Win32.LeaveCriticalSection(cs_vac);
                                }
                                else
                                {
                                    //VACDebug("rb_vacIN underflow switch time CB1!");
                                }
                            }

                            DttSP.ExchangeSamples(thread, in_l, in_r, out_l, out_r, frameCount);

                            if (ramp_down)
                            {
                                int i;
                                for (i = 0; i < frameCount; i++)
                                {
                                    float w = (float)Math.Sin(ramp_val * Math.PI / 2.0);
                                    out_l_ptr1[i] *= w;
                                    out_r_ptr1[i] *= w;
                                    ramp_val += ramp_step;
                                    if (++ramp_count >= ramp_samples)
                                    {
                                        ramp_down = false;
                                        break;
                                    }
                                }

                                if (ramp_down)
                                {
                                    for (; i < frameCount; i++)
                                    {
                                        out_l[i] = 0.0f;
                                        out_r[i] = 0.0f;
                                    }
                                }
                            }
                            else if (ramp_up)
                            {
                                for (int i = 0; i < frameCount; i++)
                                {
                                    float w = (float)Math.Sin(ramp_val * Math.PI / 2.0);
                                    out_l[i] *= w;
                                    out_r[i] *= w;
                                    ramp_val += ramp_step;
                                    if (++ramp_count >= ramp_samples)
                                    {
                                        ramp_up = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ClearBuffer(out_l, frameCount);
                                ClearBuffer(out_r, frameCount);
                            }

                            if (next_audio_state1 == AudioState.CW)
                            {
                                //cw_delay = 1;
                                DttSP.CWtoneExchange(out_l, out_r, frameCount);
                            }
                            else if (switch_count == 1)
                                DttSP.CWRingRestart();

                            switch_count--;
                            if (switch_count == ramp_up_num) RampUp = true;
                            if (switch_count == 0)
                                current_audio_state1 = next_audio_state1;
                            break;
                    }

                    /*if (!MultiPSK_server_enable && vac_enabled && !loopDLL_enabled &&
                        rb_vacIN_l != null && rb_vacIN_r != null &&
                        rb_vacOUT_l != null && rb_vacOUT_r != null)
                    {
                        fixed (float* outl_ptr = &(vac_outl[0]))
                        fixed (float* outr_ptr = &(vac_outr[0]))
                        {
                            if (!mox)
                            {
                                ScaleBuffer(out_l, outl_ptr, frameCount, (float)vac_rx_scale);
                                ScaleBuffer(out_r, outr_ptr, frameCount, (float)vac_rx_scale);
                            }
                            else if (mox && vac_mon && (dsp_mode == DSPMode.CWU || dsp_mode == DSPMode.CWL))
                            {
                                ScaleBuffer(out_l, outl_ptr, frameCount, 0.0f);
                                ScaleBuffer(out_r, outr_ptr, frameCount, 0.0f);
                            }
                            else // zero samples going back to VAC since TX monitor is off
                            {
                                ScaleBuffer(out_l, outl_ptr, frameCount, 0.0f);
                                ScaleBuffer(out_r, outr_ptr, frameCount, 0.0f);
                            }

                            int count = 0;

                            while (!(rb_vacOUT_l.WriteSpace() >= frameCount && rb_vacOUT_r.WriteSpace() >= frameCount))
                            {
                                Thread.Sleep(1);
                                count++;

                                if (count > latency1)
                                    break;
                            }

                            if (count > 0 && debug)
                                VACDebug("VAC WriteSpace count: " + count.ToString());

                            if (!mox)
                            {
                                if (sample_rateVAC == sample_rate1)
                                {
                                    if ((rb_vacOUT_l.WriteSpace() >= frameCount) && (rb_vacOUT_r.WriteSpace() >= frameCount))
                                    {
                                        if (VACDirectI_Q)
                                        {
                                            if (vac_correct_iq)
                                                CorrectIQBuffer(in_l, in_r, vac_iq_gain, vac_iq_phase, frameCount);

                                            Win32.EnterCriticalSection(cs_vac);
                                            rb_vacOUT_l.WritePtr(in_l, frameCount);
                                            rb_vacOUT_r.WritePtr(in_r, frameCount);
                                            Win32.LeaveCriticalSection(cs_vac);
                                        }
                                        else
                                        {
                                            Win32.EnterCriticalSection(cs_vac);
                                            rb_vacOUT_l.WritePtr(outl_ptr, frameCount);
                                            rb_vacOUT_r.WritePtr(outr_ptr, frameCount);
                                            Win32.LeaveCriticalSection(cs_vac);
                                        }
                                    }
                                    else
                                    {
                                        VACDebug("rb_vacOUT overflow CB1");
                                    }
                                }
                                else
                                {
                                    fixed (float* res_outl_ptr = &(res_outl[0]))
                                    fixed (float* res_outr_ptr = &(res_outr[0]))
                                    {
                                        int outsamps;

                                        if (VACDirectI_Q)
                                        {
                                            DttSP.DoResamplerF(in_l_ptr1, res_outl_ptr, frameCount, &outsamps, resampPtrOut_l);
                                            DttSP.DoResamplerF(in_r_ptr1, res_outr_ptr, frameCount, &outsamps, resampPtrOut_r);

                                            if ((rb_vacOUT_l.WriteSpace() >= outsamps) && (rb_vacOUT_r.WriteSpace() >= outsamps))
                                            {
                                                if (vac_correct_iq)
                                                    CorrectIQBuffer(res_outl_ptr, res_outr_ptr, vac_iq_gain, vac_iq_phase, frameCount);

                                                Win32.EnterCriticalSection(cs_vac);
                                                rb_vacOUT_l.WritePtr(res_outl_ptr, outsamps);
                                                rb_vacOUT_r.WritePtr(res_outr_ptr, outsamps);
                                                Win32.LeaveCriticalSection(cs_vac);
                                            }
                                            else
                                            {
                                                VACDebug("rb_vacOUT overflow CB1");
                                            }
                                        }
                                        else
                                        {
                                            DttSP.DoResamplerF(outl_ptr, res_outl_ptr, frameCount, &outsamps, resampPtrOut_l);
                                            DttSP.DoResamplerF(outr_ptr, res_outr_ptr, frameCount, &outsamps, resampPtrOut_r);

                                            if ((rb_vacOUT_l.WriteSpace() >= outsamps) && (rb_vacOUT_r.WriteSpace() >= outsamps))
                                            {
                                                Win32.EnterCriticalSection(cs_vac);
                                                rb_vacOUT_l.WritePtr(res_outl_ptr, outsamps);
                                                rb_vacOUT_r.WritePtr(res_outr_ptr, outsamps);
                                                Win32.LeaveCriticalSection(cs_vac);
                                            }
                                            else
                                            {
                                                VACDebug("rb_vacOUT overflow CB1");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }*/

                    if (console.CurrentDisplayMode == DisplayMode.SCOPE ||
                        console.CurrentDisplayMode == DisplayMode.PANASCOPE)
                        DoScope(out_l, frameCount);

                    double vol_l = monitor_volume_left;
                    double vol_r = monitor_volume_right;

                    if (mox)
                    {
                        vol_l = TXScale;
                        vol_r = TXScale;

                        if (high_pwr_am)
                        {
                            if (dsp_mode == DSPMode.AM ||
                                dsp_mode == DSPMode.SAM)
                            {
                                vol_l *= 1.414;
                                vol_r *= 1.414;
                            }
                        }
                    }

                    if (wave_record && !mox && !record_rx_preprocessed)
                        wave_file_writer.AddWriteBuffer(out_r_ptr1, out_l_ptr1);
                    else if (wave_record && mox && !record_tx_preprocessed)
                        wave_file_writer.AddWriteBuffer(out_r_ptr1, out_l_ptr1);

                    if (PrimaryDirectI_Q && !mox)
                    {
                        if (primary_correct_iq)
                        {
                            CorrectIQBuffer(in_l, in_r, primary_iq_gain, primary_iq_phase, frameCount);
                        }

                        ScaleBuffer(in_l, out_r, frameCount, (float)vol_l);
                        ScaleBuffer(in_r, out_l, frameCount, (float)vol_r);
                    }
                    else
                    {
                        if (mox)
                        {
                            ScaleBuffer(out_l, out_l, frameCount, (float)vol_l);
                            ScaleBuffer(out_r, out_r, frameCount, (float)vol_r);
                        }
                        else
                        {
                            switch (mute_ch)
                            {
                                case MuteChannels.Left:
                                    ScaleBuffer(out_r, out_r, frameCount, (float)vol_r);
                                    ScaleBuffer(out_r, out_l, frameCount, 1.0f);
                                    break;

                                case MuteChannels.Right:
                                    ScaleBuffer(out_l, out_l, frameCount, (float)vol_l);
                                    ScaleBuffer(out_l, out_r, frameCount, 1.0f);
                                    break;


                                case MuteChannels.Both:
                                    ClearBuffer(out_l, frameCount);
                                    ClearBuffer(out_r, frameCount);
                                    break;

                                case MuteChannels.None:
                                    ScaleBuffer(out_l, out_l, frameCount, (float)vol_l);
                                    ScaleBuffer(out_r, out_r, frameCount, (float)vol_r);
                                    break;
                            }
                        }
                    }

                    for (int i = 0; i < frameCount; i++)
                    {
                        output[i * 2] = out_buf_r[i];
                        output[i * 2 + 1] = out_buf_l[i];
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
