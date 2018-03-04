//=================================================================
// LimeSDR external control
//=================================================================
//
//  USB communication with LimeSDR
//  Copyright (C)2018 YT7PWR Goran Radivojevic
//  contact via email at: yt7pwr@mts.rs
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;


namespace PowerSDR
{
    unsafe public class LimeSDR
    {
        #region variables

        public unsafe delegate void SamplesAvailableDelegate(int thread, float *in_data, float *out_data, int len);
        public Console console;
        public bool connected = false;
        public delegate void AudioCallbackFunction(float* in_l, float* in_r, float* out_l, float* out_r, int count);
        public delegate void DebugCallbackFunction(string msg);
        public bool debug = false;
        public LimeSDRDevice device;
        private SamplesAvailableDelegate _callback;
        float[] buf_l;
        float[] buf_r;
        float[] buf_l_mox;
        float[] buf_r_mox;
        float[] out_buf_l;
        float[] out_buf_r;
        float[] resamp_out_buf_l;
        float[] resamp_out_buf_r;
        private const int MaxDecimationFactor = 1024;
        public int inputBufferSize = 2048;
        public int outputBufferSize = 2048;
        public int decimation = 0;
        public int interpolation = 0;
        double RXsampleRate = 768000.0;
        double TXsampleRate = 768000.0;
        double LPFBW = 1.5 * 1e6;
        int RXantenna = 1;
        int TXantenna = 0;
        int RX_channel = 0;
        int TX_channel = 0;
        long RX0_center_freq = (long)(100 * 1e6);
        long RX1_center_freq = (long)(100 * 1e6);
        long TX0_center_freq = (long)(100 * 1e6);
        long TX1_center_freq = (long)(100 * 1e6);


        #endregion

        #region properties

        private bool mox = false;
        public bool MOX
        {
            get { return mox; }
            set 
            {
                mox = value;
                device.MOX = value;
            }
        }

        private int buffer_length = 2048;
        public int BufferLength
        {
            get { return buffer_length; }
            set { buffer_length = value; }
        }

        #endregion

        #region constructor

        public LimeSDR(Console c)
        {
            console = c;
            device = new LimeSDRDevice();
            device.ExchangeSamples_RX0 += LimeDevice_SamplesAvailable_RX0;
            device.ExchangeSamples_RX1 += LimeDevice_SamplesAvailable_RX1;
        }

        public void Dispose()
        {
            try
            {
                device.Dispose();
                device = null;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private unsafe void LimeDevice_SamplesAvailable_RX0(int thread, float *input_samples, float *output_samples, int length)
        {
            _callback(thread, input_samples, output_samples, length);
        }

        private unsafe void LimeDevice_SamplesAvailable_RX1(int thread, float* input_samples, float* output_samples, int length)
        {
            _callback(thread, input_samples, output_samples, length);
        }

        #endregion

        public bool Open()
        {
            try
            {
                if (device != null)
                {
                    device.Dispose();
                }

                if (device == null)
                {
                    device = new LimeSDRDevice();
                    device.ExchangeSamples_RX0 += LimeDevice_SamplesAvailable_RX0;
                    device.ExchangeSamples_RX1 += LimeDevice_SamplesAvailable_RX1;
                }

                if (device.Open())
                {
                    connected = true;
                    return true;
                }
                else
                {
                    connected = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Open error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
                connected = false;
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                connected = false;
                device.Close();
                device.Dispose();
                device = null;

                return true;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Close error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
                connected = false;
                return false;
            }
        }

        public bool Start(SamplesAvailableDelegate callback, int bufSize, int rx_antenna, int tx_antenna,
            int rx_channel, int tx_channel, double lpfbw, double RXsr, double TXsr)
        {
            try
            {
                if (this.device == null)
                    throw new ApplicationException("No device selected");

                _callback = callback;
                inputBufferSize = bufSize;
                outputBufferSize = bufSize;
                RXantenna = rx_antenna;
                TXantenna = tx_antenna;
                RX_channel = rx_channel;
                TX_channel = tx_channel;
                LPFBW = lpfbw;
                RXsampleRate = RXsr;
                TXsampleRate = TXsr;

                if (connected)
                {
                    Close();
                    Open();
                    decimation = 4;
                    device.Antenna_RX0 = (uint)RXantenna;
                    device.Antenna_TX0 = (uint)TXantenna;
                    device.RX0_centerFrequency = RX0_center_freq;
                    device.TX0_centerFrequency = TX0_center_freq;
                    device.RX_channel = (uint)RX_channel;
                    device.TX0_channel = (uint)TX_channel;
                    buf_l = new float[inputBufferSize*2];
                    buf_r = new float[inputBufferSize*2];
                    buf_l_mox = new float[inputBufferSize*2];
                    buf_r_mox = new float[inputBufferSize*2];
                    out_buf_l = new float[inputBufferSize*2];
                    out_buf_r = new float[inputBufferSize*2];
                    resamp_out_buf_r = new float[inputBufferSize*2];
                    resamp_out_buf_l = new float[inputBufferSize*2];
                    device.bufferIn_0 = new float[inputBufferSize * 8];
                    device.bufferIn_L = new float[inputBufferSize * 8];
                    device.bufferIn_R = new float[inputBufferSize * 8];
                    device.bufferOut = new float[inputBufferSize * 8];
                    device.bufferIn_1 = new float[inputBufferSize * 8];
                    device.bufferOut_1 = new float[inputBufferSize * 8];
                    device._frameLength = (uint)inputBufferSize;

                    try
                    {
                        device.RX0_centerFrequency = RX0_center_freq;
                        device.TX0_centerFrequency = TX0_center_freq;
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }

                    device.Start(lpfbw * 1e6, RXsr, TXsr);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Start error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
                return false;
            }
        }

        public int GetDecimationStageCount(double inputSampleRate)
        {
            try
            {
                if (inputSampleRate <= 48000.0)
                {
                    return 0;
                }

                int result = MaxDecimationFactor;
                while (inputSampleRate < (48000.0 * result) && result > 0)
                {
                    result /= 2;
                }

                return (int)Math.Log(result, 2.0);
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                        "GetDecimationStageCount error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
                return -1;
            }
        }

        public void SetLOSC(Int32 freq)
        {
            if (device != null)
            {
                if (freq < 48 * 1e6 && RX0_center_freq >= 48 * 1e6 && device.isStreaming)
                {
                    Stop();
                    Thread.Sleep(200);
                    Close();
                    Open();
                    device.RX0_centerFrequency = freq;
                    device.TX0_centerFrequency = freq;
                    Start(_callback, inputBufferSize, RXantenna, TXantenna, RX_channel, TX_channel, LPFBW, RXsampleRate, TXsampleRate);
                    Thread.Sleep(200);
                    device.RX0_Frequency = freq;
                    device.TX0_Frequency = freq;
                }
                else if (freq > 48 * 1e6 && RX0_center_freq <= 48 * 1e6 && device.isStreaming)
                {
                    Stop();
                    Thread.Sleep(200);
                    Close();
                    Open();
                    device.RX0_centerFrequency = freq;
                    device.TX0_centerFrequency = freq;
                    Start(_callback, inputBufferSize, RXantenna, RX_channel, TX_channel, TXantenna, LPFBW, RXsampleRate, TXsampleRate);
                    device.RX0_Frequency = freq;
                    device.TX0_Frequency = freq;
                }
                else
                {
                    if (device != null && device.isStreaming)
                    {
                        device.RX0_Frequency = freq;
                        device.TX0_Frequency = freq;
                    }
                    else if (device != null)
                    {
                        device.RX0_centerFrequency = freq;
                        device.TX0_centerFrequency = freq;
                    }
                }

                RX0_center_freq = freq;
                TX0_center_freq = freq;
            }
        }

        public void Set_gain(ushort value)
        {
            try
            {
                if (device != null && connected)
                {
                    device.RX0_gain = value;
                    //device.RX1_gain = value;
                }
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Set gain error: \n" + ex.ToString());

                Debug.Write("Error setting new Gain!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetLNA_gain(ushort value)
        {
            try
            {
                device.LNA_Gain = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetLNA gain error: \n" + ex.ToString());

                Debug.Write("Error setting new LNA gain!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetTIA_gain(ushort value)
        {
            try
            {
                device.TIA_Gain = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetTIA gain error: \n" + ex.ToString());

                Debug.Write("Error setting new TIA gain!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetPGA_gain(ushort value)
        {
            try
            {
                device.PGA_Gain = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetPGA gain error: \n" + ex.ToString());

                Debug.Write("Error setting new PGA gain!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetRXgain(ushort value)
        {
            try
            {
                device.RX0_gain = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetRXgain error: \n" + ex.ToString());

                Debug.Write("Error setting new RXgain!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetTXgain(ushort value)
        {
            try
            {
                if (device != null && connected)
                {
                    device.TX0_gain = value;
                    //device.TX1_gain = 0;
                }
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetTXgain error: \n" + ex.ToString());

                Debug.Write("Error setting new TXgain!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetRXAntenna(uint value)
        {
            try
            {
                device.Antenna_RX0 = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetRXAntenna error: \n" + ex.ToString());

                Debug.Write("Error setting new SetRXAntenna!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetTXAntenna(uint value)
        {
            try
            {
                device.Antenna_TX0 = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetTXAntenna error: \n" + ex.ToString());

                Debug.Write("Error setting new SetTXAntenna!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetRXChannel(uint value)
        {
            try
            {
                device.RXChannel = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetRXChannel error: \n" + ex.ToString());

                Debug.Write("Error setting new SetRXChannel!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void SetTXChannel(uint value)
        {
            try
            {
                device.TXChannel = value;
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetTXChannel error: \n" + ex.ToString());

                Debug.Write("Error setting new SetTXChannel!\nValue is wrong!\n",
                    "Error!\n" + ex.ToString());
            }
        }

        public void Set_Buffer_Size(int new_buffer_size)
        {
            try
            {
                if (connected)
                {
                    buffer_length = new_buffer_size;
                    //SetBufferSize(new_buffer_size);
                }
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetBufferSize error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
            }
        }

        public void SetRXSampleRate(double sr)
        {
            try
            {
                if (connected)
                {
                    RXsampleRate = sr;

                    if(device != null && device.RXSampleRate != RXsampleRate)
                        device.RXSampleRate = RXsampleRate;
                }
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetBufferSize error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
            }
        }

        public void SetTXSampleRate(double sr)
        {
            try
            {
                if (connected)
                {
                    RXsampleRate = sr;

                    if (device != null && device.TXSampleRate != RXsampleRate)
                        device.TXSampleRate = RXsampleRate;
                }
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "SetBufferSize error: \n" + ex.ToString());

                Debug.Write(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                if (connected)
                    device.Stop();
            }
            catch (Exception ex)
            {
                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "LimeSDR: \n" + ex.ToString());

                Debug.Write("Error in Stop!\n",
                    "Error!\n" + ex.ToString());
            }
        }
    }

    #region structures

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct lms_range_t
    {
        public double min;
        public double max;
        public double step;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LMS7Parameter
    {
        public UInt16 address;
        public byte msb;
        public byte lsb;
        public UInt16 defaultValue;
        public string name;
        public string tooltip;
    };

    public enum dataFmt
    {
        LMS_FMT_F32 = 0,    /**<32-bit floating point*/
        LMS_FMT_I16 = 1,      /**<16-bit integers*/
        LMS_FMT_I12 = 2       /**<12-bit integers stored in 16-bit variables*/
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct lms_stream_t
    {
        public uint handle;
        public bool isTx;
        public UInt32 channel;
        public UInt32 fifoSize;
        public float throughputVsLatency;
        internal dataFmt dataFmt;
    }

    [StructLayout(LayoutKind.Sequential)]

    /**Metadata structure used in sample transfers*/
    public struct lms_stream_meta_t
    {
        /**
         * Timestamp is a value of HW counter with a tick based on sample rate.
         * In RX: time when the first sample in the returned buffer was received
         * In TX: time when the first sample in the submitted buffer should be send
         */
        public UInt64 timestamp;

        /**In TX: wait for the specified HW timestamp before broadcasting data over
         * the air
         * In RX: wait for the specified HW timestamp before starting to receive
         * samples
         */
        public bool waitForTimestamp;

        /**Indicates the end of send/receive transaction. Currently has no effect
         * @todo force send samples to HW (ignore transfer size) when selected
         */
        public bool flushPartialPacket;

    }

    public enum lms_loopback_t
    {
        LMS_LOOPBACK_NONE   /**<Return to normal operation (disable loopback)*/
    }

    public enum lms_testsig_t
    {
        LMS_TESTSIG_NONE = 0,     /**<Disable test signals. Return to normal operation*/
        LMS_TESTSIG_NCODIV8,    /**<Test signal from NCO half scale*/
        LMS_TESTSIG_NCODIV4,    /**<Test signal from NCO half scale*/
        LMS_TESTSIG_NCODIV8F,   /**<Test signal from NCO full scale*/
        LMS_TESTSIG_NCODIV4F,   /**<Test signal from NCO full scale*/
        LMS_TESTSIG_DC          /**<DC test signal*/
    }

    #endregion

    public class LimeSDRDevice
    {
        #region Dll import

        [DllImport("LimeSuite", EntryPoint = "LMS_GetDeviceList", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_GetDeviceList(string dev_list);

        [DllImport("LimeSuite", EntryPoint = "LMS_Open", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_Open(out IntPtr device, string info, string args);

        [DllImport("LimeSuite", EntryPoint = "LMS_Close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_Close(IntPtr device);

        [DllImport("LimeSuite", EntryPoint = "LMS_Disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_Disconnect(IntPtr device);

        [DllImport("LimeSuite", EntryPoint = "LMS_IsOpen", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LMS_IsOpen(IntPtr device, int port);

        [DllImport("LimeSuite", EntryPoint = "LMS_Init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_Init(IntPtr device);

        [DllImport("LimeSuite", EntryPoint = "LMS_EnableChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_EnableChannel(IntPtr device, bool dir_tx, uint chan, bool enabled);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetLOFrequency", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetLOFrequency(IntPtr device, bool dir_tx, uint chan, double frequency);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetLOFrequency", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_GetLOFrequency(IntPtr device, bool dir_tx, uint chan, ref double frequency);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetNCOFrequency", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_SetNCOFrequency(IntPtr device, bool dir_tx, uint chan, double* frequency, 
            double pho);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetNCOFrequency", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_GetNCOFrequency(IntPtr device, bool dir_tx, uint chan, double* frequency, 
            double* pho);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetNCOIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetNCOIndex(IntPtr device, bool dir_tx, uint chan, int index, bool downconv);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetNCOIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_GetNCOIndex(IntPtr device, bool dir_tx, uint chan);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetNCOPhase", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetNCOPhase(IntPtr device, bool dir_tx, uint chan, double phase, double fcw);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetNCOPhase", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_GetNCOPhase(IntPtr device, bool dir_tx, uint chan, ref double phase, ref double fcw);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetSampleRateDir", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetSampleRateDir(IntPtr device, bool dir_tx, double rate, uint oversample);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetSampleRate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetSampleRate(IntPtr device, double rate, uint oversample);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetSampleRate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_GetSampleRate(IntPtr device, bool dir_tx, uint chan, ref double host_Hz,
            ref double rf_Hz);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetupStream", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_SetupStream(IntPtr dev, IntPtr stream);

        [DllImport("LimeSuite", EntryPoint = "LMS_StartStream", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_StartStream(IntPtr stream);

        [DllImport("LimeSuite", EntryPoint = "LMS_StopStream", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_StopStream(IntPtr stream);

        [DllImport("LimeSuite", EntryPoint = "LMS_RecvStream", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_RecvStream(IntPtr stream, void* samples, uint sample_count,
            ref lms_stream_meta_t meta, uint timeout_ms);

        [DllImport("LimeSuite", EntryPoint = "LMS_SendStream", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_SendStream(IntPtr stream, void* samples, uint sample_count,
            ref lms_stream_meta_t meta, uint timeout_ms);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetLastErrorMessage", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LMS_GetLastErrorMessage();

        public static string limesdr_strerror()
        {
            IntPtr ret = LMS_GetLastErrorMessage();
            if (ret != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(ret);
            return String.Empty;
        }

        [DllImport("LimeSuite", EntryPoint = "LMS_SetAntenna", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_SetAntenna(IntPtr device, bool dir_tx, uint chan, uint index);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetTestSignal", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int LMS_SetTestSignal(IntPtr device, bool dir_tx, uint chan, lms_testsig_t sig, 
            Int16 dc_i, Int16 dc_q);

        [DllImport("LimeSuite", EntryPoint = "LMS_WriteParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_WriteParam(IntPtr device, LMS7Parameter param, UInt16 val);

        [DllImport("LimeSuite", EntryPoint = "LMS_ReadParam", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_ReadParam(IntPtr device, LMS7Parameter param, ushort* val);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetGaindB", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetGaindB(IntPtr device, bool dir_tx, uint chan, uint gain);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetGaindB", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetGaindB(IntPtr device, bool dir_tx, uint chan, uint* gain);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetNormalizedGain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LMS_SetNormalizedGain(IntPtr device, bool dir_tx, uint chan, float gain);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetNormalizedGain", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetNormalizedGain(IntPtr device, bool dir_tx, uint chan, float* gain);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetChipTemperature", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetChipTemperature(IntPtr dev, uint ind, float* temp);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetLPFBW", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_SetLPFBW(IntPtr device, bool dir_tx, uint chan, double bandwidth);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetLPFBW", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetLPFBW(IntPtr device, bool dir_tx, uint chan, double* bandwidth);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetLPFBWRange", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetLPFBWRange(IntPtr device, bool dir_tx, uint chan, lms_range_t* range);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetLPF", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_SetLPF(IntPtr device, bool dir_tx, uint chan, bool enable);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetGFIRLPF", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_SetGFIRLPF(IntPtr device, bool dir_tx, uint chan, bool enable, float bandwidth);

        [DllImport("LimeSuite", EntryPoint = "LMS_WriteFPGAReg", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_WriteFPGAReg(IntPtr device, UInt32 address, UInt16 val);

        [DllImport("LimeSuite", EntryPoint = "LMS_ReadFPGAReg", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_ReadFPGAReg(IntPtr device, UInt32 address, UInt16* val);

        [DllImport("LimeSuite", EntryPoint = "LMS_SetGFIRCoeff", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_SetGFIRCoeff(IntPtr device, bool dir_tx, uint chan, IntPtr filt,
            float* coef, uint count);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetGFIRCoeff", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetGFIRCoeff(IntPtr device, bool dir_tx, uint chan, IntPtr filt,
            float* coef);

        [DllImport("LimeSuite", EntryPoint = "LMS_GetAntennaBW", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GetAntennaBW(IntPtr device, bool dir_tx, uint chan, uint path,
            lms_range_t* range);

        [DllImport("LimeSuite", EntryPoint = "LMS_Reset", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_Reset(IntPtr device);

        [DllImport("LimeSuite", EntryPoint = "LMS_GPIOWrite", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GPIOWrite(IntPtr device, uint *buffer, uint length);

        [DllImport("LimeSuite", EntryPoint = "LMS_GPIODirWrite", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GPIODirWrite(IntPtr device, uint* buffer, uint length);

        [DllImport("LimeSuite", EntryPoint = "LMS_GPIORead", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GPIORead(IntPtr device, uint* buffer, uint length);

        [DllImport("LimeSuite", EntryPoint = "LMS_GPIODirRead", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int LMS_GPIODirRead(IntPtr device, uint* buffer, uint length);

        #endregion

        #region variable

        public unsafe delegate void ExchangeSamplesDelegate(int thread, float* input_samples, 
            float* output_samples, int length);
        private uint SampleTimeoutMs = 1000;
        private IntPtr _device = IntPtr.Zero;
        IntPtr _streamRX_0 = IntPtr.Zero;
        IntPtr _streamTX_0 = IntPtr.Zero;
        IntPtr _streamRX_1 = IntPtr.Zero;
        IntPtr _streamTX_1 = IntPtr.Zero;
        private GCHandle _gcHandle;
        public bool isStreaming;
        private Thread _sampleThread = null;
        public uint RX_channel = 0;
        public uint TX0_channel = 0;
        public uint TX1_channel = 0;
        private uint RX0_ant = 2;              // Low input
        private uint RX1_ant = 2;              // Low input
        private uint TX0_ant = 0;
        public uint _frameLength = 2048;
        public double RXsampleRate = 768000.0;
        public double TXsampleRate = 768000.0;
        public double RX0_centerFrequency = 144400000.0;
        public double RX1_centerFrequency = 144400000.0;
        public double TX0_centerFrequency = 144400000.0;
        public double TX1_centerFrequency = 144400000.0;
        private uint rx0_gain = 40;
        private uint rx1_gain = 40;
        private uint tx0_gain = 20;
        private uint tx1_gain = 0;
        private ushort pga_gain = 11;
        private ushort tia_gain = 3;
        private ushort lna_gain = 15;
        public float[] bufferIn_0;
        public float[] bufferIn_L;
        public float[] bufferIn_R;
        public float[] bufferOut;
        public float[] bufferIn_1;
        public float[] bufferOut_1;

        public const bool LMS_CH_TX = true;
        public const bool LMS_CH_RX = false;

        [method: CompilerGenerated]
        public event ExchangeSamplesDelegate ExchangeSamples_RX0;
        public event ExchangeSamplesDelegate ExchangeSamples_RX1;

        LMS7Parameter PGA_gain = new LMS7Parameter();
        LMS7Parameter TIA_gain = new LMS7Parameter();
        LMS7Parameter LNA_gain = new LMS7Parameter();
        LMS7Parameter CMIX_BYP_RXTSP = new LMS7Parameter();
        LMS7Parameter CMIX_BYP_TXTSP = new LMS7Parameter();

        public Decimator.IQDecimator baseBandDecimator;
        double LPFBW = 1500000.0;

        uint band_Filter = 0;

        #endregion

        #region properties

        private bool mox = false;
        public unsafe bool MOX
        {
            get { return mox; }
            set 
            {
                mox = value;
                uint[] buffer = new uint[1];
                buffer[0] = (uint)band_Filter;

                if (mox)
                    buffer[0] = (uint)(band_Filter | 0x10);

                fixed (uint* buf = &buffer[0])
                    if (LMS_GPIOWrite(_device, buf, 1) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
            }
        }

        #endregion

        #region callback

        private unsafe void ReceiveSamples_sync()
        {
            int j = 0;
            lms_stream_meta_t rx_meta = new lms_stream_meta_t();
            //lms_stream_meta_t rx_meta_1 = new lms_stream_meta_t();
            lms_stream_meta_t tx_meta = new lms_stream_meta_t();
            //lms_stream_meta_t tx_meta_1 = new lms_stream_meta_t();

            //fixed (float* in_l = &bufferIn_L[0])
            //fixed (float* in_r = &bufferIn_R[0])
            fixed (float* input_data = &bufferIn_0[0])
            fixed (float* output_data = &bufferOut[0])
            //fixed (float* input_data_1 = &bufferIn_1[0])
            //fixed (float* output_data_1 = &bufferOut_1[0])
            {
                rx_meta.timestamp = 0;
                rx_meta.flushPartialPacket = true;
                //rx_meta_1.timestamp = 0;
                //rx_meta_1.flushPartialPacket = true;

                while (isStreaming)
                {
                    LMS_RecvStream(_streamRX_0, input_data, _frameLength, ref rx_meta, SampleTimeoutMs);

                    /*j = 0;

                    for (int i = 0; i < _frameLength; i++)
                    {
                        in_l[j] = input_data[i];
                        in_r[j] = input_data[i + 1];
                        j++;
                        i++;
                    }

                    baseBandDecimator.Process(in_l, in_r, (int)_frameLength);

                    j = 0;

                    for (int i = 0; i < _frameLength; i++)
                    {
                        input_data[i] = in_l[j];
                        input_data[i + 1] = in_r[j];
                        j+=2;
                        i++;
                    }*/

                    //if(!mox)
                        ExchangeSamples_RX0(0, input_data, output_data, (int)_frameLength);

                    //LMS_RecvStream(_streamRX_1, input_data_1, _frameLength, ref rx_meta_1, SampleTimeoutMs);
                    //ExchangeSamples_RX1(1, input_data_1, output_data_1, (int)_frameLength);

                    if (mox)
                    {
                        //ExchangeSamples(input_data_1, output_data_1, (int)_frameLength * 2);
                        tx_meta.timestamp = rx_meta.timestamp;
                        tx_meta.flushPartialPacket = true;
                        //tx_meta_1.timestamp = rx_meta.timestamp;
                        //tx_meta_1.flushPartialPacket = true;
                        LMS_SendStream(_streamTX_0, output_data, _frameLength, ref tx_meta, SampleTimeoutMs);
                        //LMS_SendStream(_streamTX_1, output_data_1, _frameLength, ref tx_meta_1, SampleTimeoutMs);
                    }
                }
            }
        }

        public unsafe void Transmit_async(float* out_l, float* out_r, int frameLength)
        {
            int j = 0;
            lms_stream_meta_t tx_meta = new lms_stream_meta_t();

            fixed (float* output_data = &bufferOut_1[0])
            {
                for (int i = 0; i < frameLength; i++)
                {
                    output_data[j] = out_l[i];
                    output_data[j+1] = out_r[i];
                    j += 2;
                }

                if (isStreaming)
                {
                    LMS_SendStream(_streamTX_0, output_data, (uint)frameLength*2, ref tx_meta, SampleTimeoutMs);
                }
            }
        }

        #endregion

        #region properties

        public bool IsStreaming
        {
            get
            {
                return isStreaming;
            }
        }

        #endregion

        #region constructor/destructor

        public LimeSDRDevice()
        {
            try
            {
                PGA_gain.address = 0x0119;
                PGA_gain.msb = 4;
                PGA_gain.lsb = 0;
                PGA_gain.defaultValue = 11;
                PGA_gain.name = "G_PGA_RBB";
                PGA_gain.tooltip = "This is the gain of the PGA";

                TIA_gain.address = 0x0113;
                TIA_gain.msb = 1;
                TIA_gain.lsb = 0;
                TIA_gain.defaultValue = 3;
                TIA_gain.name = "G_TIA_RFE";
                TIA_gain.tooltip = "Controls the Gain of the TIA";

                LNA_gain.address = 0x0113;
                LNA_gain.msb = 9;
                LNA_gain.lsb = 6;
                LNA_gain.defaultValue = 15;
                LNA_gain.name = "G_LNA_RFE";
                LNA_gain.tooltip = "Controls the gain of the LNA";

                CMIX_BYP_RXTSP.address = 0x040C;
                CMIX_BYP_RXTSP.msb = 7;
                CMIX_BYP_RXTSP.lsb = 7;
                CMIX_BYP_RXTSP.defaultValue = 0;
                CMIX_BYP_RXTSP.name = "CMIX_BYP_RXTSP";
                CMIX_BYP_RXTSP.tooltip = "CMIX bypass";

                CMIX_BYP_TXTSP.address = 0x020C;
                CMIX_BYP_TXTSP.msb = 8;
                CMIX_BYP_TXTSP.lsb = 8;
                CMIX_BYP_TXTSP.defaultValue = 0;
                CMIX_BYP_TXTSP.name = "CMIX_BYP_TXTSP";
                CMIX_BYP_TXTSP.tooltip = "CMIX bypass";

                _gcHandle = GCHandle.Alloc(this);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        ~LimeSDRDevice()
        {
            Dispose();
        }

        #endregion

        public void Stop()
        {
            if (!isStreaming)
                return;

            isStreaming = false;

            if (_sampleThread != null)
            {
                if (_sampleThread.ThreadState == System.Threading.ThreadState.Running)
                    _sampleThread.Join();
                _sampleThread = null;
            }

            LMS_StopStream(_streamRX_0);
            LMS_StopStream(_streamTX_0);
            //LMS_StopStream(_streamRX_1);
            //LMS_StopStream(_streamTX_1);
        }

        public void Dispose()
        {
            this.Stop();

            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }

            GC.SuppressFinalize(this);
            //LMS_Close(_device);
            _device = IntPtr.Zero;
        }

        public unsafe bool Open()
        {
            if (LMS_GetDeviceList(null) < 1)
            {
                throw new Exception("Cannot found LimeSDR device. Is the device locked somewhere?");
            }

            if (LMS_Open(out _device, null, null) != 0)
            {
                throw new ApplicationException("Cannot open LimeSDR device. Is the device locked somewhere?");
                return false;
            }

            return true;
        }

        public unsafe bool Close()
        {
            if (LMS_Close(_device) != 0)
            {
                throw new ApplicationException("Cannot open LimeSDR device. Is the device locked somewhere?");
                return false;
            }

            return true;
        }

        public unsafe bool Start(double lpfbw, double RXsr, double TXsr)
        {
            LPFBW = lpfbw;
            RXsampleRate = RXsr;
            TXsampleRate = TXsr;

            if (LMS_GetDeviceList(null) < 1)
            {
                throw new Exception("Cannot found LimeSDR device. Is the device locked somewhere?");
            }

            if (LMS_Open(out _device, null, null) != 0)
            {
                throw new ApplicationException("Cannot open LimeSDR device. Is the device locked somewhere?");
                return false;
            }

            LMS_Init(_device);

            uint[] buffer = new uint[1];
            buffer[0] = 0x1f;               // 0-5 output GPIO (band filters, mox)

            fixed (uint* buf = &buffer[0])
                if (LMS_GPIODirWrite(_device, buf, 1) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }

            buffer[0] = 0xc0;               // 7-8 input GPIO (keyer dot, dash)
            fixed (uint* buf = &buffer[0])
                if (LMS_GPIODirRead(_device, buf, 1) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }

            if (LMS_EnableChannel(_device, LMS_CH_RX, RX_channel, true) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            /*if (LMS_EnableChannel(_device, LMS_CH_RX, RX1_channel, true) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }*/

            if (LMS_EnableChannel(_device, LMS_CH_TX, TX0_channel, true) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            /*if (LMS_EnableChannel(_device, LMS_CH_TX, TX1_channel, true) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }*/

            if (LMS_SetAntenna(_device, LMS_CH_RX, RX_channel, RX0_ant) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            /*if (LMS_SetAntenna(_device, LMS_CH_RX, RX1_channel, RX1_ant) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }*/

            if (LMS_SetAntenna(_device, LMS_CH_TX, TX0_channel, TX0_ant) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            if (TXSampleRate <= 384000)
            {
                if (LMS_SetSampleRateDir(_device, LMS_CH_TX, TXSampleRate, 32) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }
            else
            {
                if (LMS_SetSampleRateDir(_device, LMS_CH_TX, TXSampleRate, 32) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }

            if (RXSampleRate < 384000)
            {
                if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 32) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }
            else
            {
                if (RXsampleRate > 32000000)
                {
                    if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 0) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
                else if (RXsampleRate > 16000000)
                {
                    if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 4) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
                else if (RXsampleRate > 8000000)
                {
                    if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 8) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
                else if (RXsampleRate > 4000000)
                {
                    if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 16) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
                else if (RXsampleRate > 2000000)
                {
                    if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 32) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
                else
                {
                    if (LMS_SetSampleRateDir(_device, LMS_CH_RX, RXSampleRate, 32) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }

            RX0_Frequency = (long)RX0_centerFrequency;
            //RX1_Frequency = (long)RX1_centerFrequency;
            TX0_Frequency = (long)TX0_centerFrequency;
            //TX1_Frequency = (long)TX1_centerFrequency;

            RX0_gain = RX0_gain;
            TX0_gain = TX0_gain;
            //RX1_gain = rx1_gain;
            //TX1_gain = tx1_gain;

            if (RX0_centerFrequency >= 48 * 1e6)
            {
                if (RXSampleRate < 1.5 * 1e6)
                {
                    if (LMS_SetLPFBW(_device, LMS_CH_RX, RX_channel, 1500000.0) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
                else
                {
                    if (LMS_SetLPFBW(_device, LMS_CH_RX, RX_channel, RXSampleRate + RXSampleRate * 0.2) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }
            else
            {
                if (LMS_SetLPFBW(_device, LMS_CH_RX, RX_channel, LPFBW) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }

            /*if (RX1_centerFrequency >= 48 * 1e6)
            {
                if (LMS_SetLPFBW(_device, LMS_CH_RX, RX1_channel, 1500000.0) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }
            else
            {
                if (LMS_SetLPFBW(_device, LMS_CH_RX, RX1_channel, 60000000.0) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }*/

            lms_stream_t streamRX = new lms_stream_t();
            streamRX.handle = 0;
            streamRX.channel = RX_channel;                 //channel number
            streamRX.fifoSize = 1024 * 1024;                  //fifo size in samples
            streamRX.throughputVsLatency = 0.5f;            //optimize for max throughput
            streamRX.isTx = false;                          //RX channel
            streamRX.dataFmt = dataFmt.LMS_FMT_F32;
            _streamRX_0 = Marshal.AllocHGlobal(Marshal.SizeOf(streamRX));

            Marshal.StructureToPtr(streamRX, _streamRX_0, false);

            if (LMS_SetupStream(_device, _streamRX_0) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            lms_stream_t streamTX = new lms_stream_t();
            streamTX.handle = 0;
            streamTX.channel = TX0_channel;                 //channel number
            streamTX.fifoSize = 1024 * 1024;                  //fifo size in samples
            streamTX.throughputVsLatency = 0.5f;            //optimize for max throughput
            streamTX.isTx = true;                           //TX channel
            streamTX.dataFmt = dataFmt.LMS_FMT_F32;
            _streamTX_0 = Marshal.AllocHGlobal(Marshal.SizeOf(streamTX));

            Marshal.StructureToPtr(streamTX, _streamTX_0, false);

            if (LMS_SetupStream(_device, _streamTX_0) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            /*lms_stream_t streamRX_1 = new lms_stream_t();
            streamRX_1.handle = 0;
            streamRX_1.channel = RX1_channel;                 //channel number
            streamRX_1.fifoSize = 16 * 1024;                  //fifo size in samples
            streamRX_1.throughputVsLatency = 0.1f;            //optimize for max throughput
            streamRX_1.isTx = false;                          //RX channel
            streamRX_1.dataFmt = dataFmt.LMS_FMT_F32;
            _streamRX_1 = Marshal.AllocHGlobal(Marshal.SizeOf(streamRX_1));

            Marshal.StructureToPtr(streamRX_1, _streamRX_1, false);

            if (LMS_SetupStream(_device, _streamRX_1) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            lms_stream_t streamTX_1 = new lms_stream_t();
            streamTX_1.handle = 0;
            streamTX_1.channel = TX1_channel;                 //channel number
            streamTX_1.fifoSize = 16 * 1024;                  //fifo size in samples
            streamTX_1.throughputVsLatency = 0.1f;            //optimize for max throughput
            streamTX_1.isTx = true;                           //TX channel
            streamTX_1.dataFmt = dataFmt.LMS_FMT_F32;
            _streamTX_1 = Marshal.AllocHGlobal(Marshal.SizeOf(streamTX_1));

            Marshal.StructureToPtr(streamTX_1, _streamTX_1, false);

            if (LMS_SetupStream(_device, _streamTX_1) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }*/

            if (LMS_StartStream(_streamRX_0) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            if (LMS_StartStream(_streamTX_0) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            /*if (LMS_StartStream(_streamRX_1) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }

            if (LMS_StartStream(_streamTX_1) != 0)
            {
                throw new ApplicationException(limesdr_strerror());
            }*/

            _sampleThread = new Thread(ReceiveSamples_sync);
            _sampleThread.Name = "limesdr_samples_rx";
            _sampleThread.Priority = ThreadPriority.Highest;
            isStreaming = true;
            _sampleThread.Start();

            return true;
        }

        public unsafe long RX0_Frequency
        {
            get
            {
                if (_device != IntPtr.Zero)
                {
                    if (LMS_GetLOFrequency(_device, LMS_CH_RX, RX_channel, ref RX0_centerFrequency) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }

                return (long)RX0_centerFrequency;
            }
            set
            {
                RX0_centerFrequency = value;

                if (_device != IntPtr.Zero)
                {
                    if (value >= 48 * 1e6)
                    {
                        if (LMS_SetNCOIndex(_device, LMS_CH_RX, RX_channel, 15, true) != 0)   // 0.0 NCO
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }

                        if (LMS_SetLOFrequency(_device, LMS_CH_RX, RX_channel, RX0_centerFrequency) != 0)
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }

                        if (LMS_WriteParam(_device, CMIX_BYP_RXTSP, 1) < 0)
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }
                    }
                    else
                    {
                        if (LMS_SetLOFrequency(_device, LMS_CH_RX, RX_channel, 48.0 * 1e6) != 0)
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }

                        double[] losc_freq = new double[16];
                        double[] pho = new double[1];

                        fixed (double* freq = &losc_freq[0])
                        fixed (double* pho_ptr = &pho[0])
                        {
                            losc_freq[0] = 48.0 * 1e6 - RX0_centerFrequency;
                            losc_freq[15] = 0.0;

                            if (LMS_SetNCOFrequency(_device, LMS_CH_RX, RX_channel, freq, 0.0) != 0)
                            {
                                Debug.Write("Wrong RX0 NCO frequency value!" + freq[0].ToString("f3") + "\n");
                                throw new ApplicationException(limesdr_strerror());
                            }

                            if (LMS_SetNCOIndex(_device, LMS_CH_RX, RX_channel, 0, true) != 0)
                            {
                                Debug.Write("Wrong RX0 NCO index value!\n");
                                throw new ApplicationException(limesdr_strerror());
                            }

                            if (LMS_WriteParam(_device, CMIX_BYP_RXTSP, 0) < 0)
                            {
                                throw new ApplicationException(limesdr_strerror());
                            }
                        }
                    }
                }
            }
        }

        public unsafe long RX1_Frequency
        {
            get
            {
                if (_device != IntPtr.Zero)
                {
                    if (LMS_GetLOFrequency(_device, LMS_CH_RX, RX_channel, ref RX1_centerFrequency) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }

                return (long)RX1_centerFrequency;
            }
            set
            {
                RX1_centerFrequency = value;

                if (value >= 48 * 1e6)
                {
                    if (_device != IntPtr.Zero)
                    {
                        if (LMS_SetNCOIndex(_device, LMS_CH_RX, RX_channel, 15, false) != 0)   // 0.0 NCO
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }

                        if (LMS_SetLOFrequency(_device, LMS_CH_RX, RX_channel, RX1_centerFrequency) != 0)
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }
                    }
                }
                else
                {
                    if (LMS_SetLOFrequency(_device, LMS_CH_RX, RX_channel, 48.0 * 1e6) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                    double[] losc_freq = new double[16];
                    double[] pho = new double[1];

                    fixed (double* freq = &losc_freq[0])
                    fixed (double* pho_ptr = &pho[0])
                    {
                        losc_freq[0] = 48.0 * 1e6 - RX1_centerFrequency;
                        losc_freq[15] = 0.0;

                        if (LMS_SetNCOFrequency(_device, LMS_CH_RX, RX_channel, freq, 0.0) != 0)
                        {
                            //throw new ApplicationException(limesdr_strerror());
                            Debug.Write("Wrong RX1 NCO frequency value!" + freq[0].ToString("f3") + "\n");
                        }

                        if (LMS_SetNCOIndex(_device, LMS_CH_RX, RX_channel, 0, false) != 0)
                        {
                            //throw new ApplicationException(limesdr_strerror());
                            Debug.Write("Wrong RX1 NCO index value!" + freq[0].ToString("f3") + "\n");
                        }

                        /*if (LMS_GetNCOFrequency(_device, LMS_CH_RX, RX0_channel, freq, pho_ptr) != 0)
                        {
                            //throw new ApplicationException(limesdr_strerror());
                        }*/
                    }
                }
            }
        }

        public unsafe long TX0_Frequency
        {
            get
            {
                if (_device != IntPtr.Zero)
                {
                    if (LMS_GetLOFrequency(_device, LMS_CH_RX, TX0_channel, ref TX0_centerFrequency) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }

                return (long)TX0_centerFrequency;
            }
            set
            {
                TX0_centerFrequency = value;

                if (value >= 48 * 1e6)
                {
                    if (_device != IntPtr.Zero)
                    {
                        if (LMS_SetNCOIndex(_device, LMS_CH_TX, TX0_channel, 15, true) != 0)   // 0.0 NCO
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }

                        if (LMS_SetLOFrequency(_device, LMS_CH_TX, TX0_channel, TX0_centerFrequency) != 0)
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }
                    }
                }
                else
                {
                    if (LMS_SetLOFrequency(_device, LMS_CH_TX, TX0_channel, 48.0 * 1e6) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                    double[] losc_freq = new double[16];
                    double[] pho = new double[1];

                    fixed (double* freq = &losc_freq[0])
                    fixed (double* pho_ptr = &pho[0])
                    {
                        losc_freq[0] = 48.0 * 1e6 - TX0_centerFrequency;
                        losc_freq[15] = 0.0;

                        if (LMS_SetNCOFrequency(_device, LMS_CH_TX, TX0_channel, freq, 0.0) != 0)
                        {
                            Debug.Write("Wrong TX0 NCO frequency value!" + freq[0].ToString("f3") + "\n");
                            throw new ApplicationException(limesdr_strerror());
                        }

                        if (LMS_SetNCOIndex(_device, LMS_CH_TX, TX0_channel, 0, true) != 0)
                        {
                            Debug.Write("Wrong TX0 NCO index value!" + freq[0].ToString("f3") + "\n");
                            throw new ApplicationException(limesdr_strerror());
                        }
                    }
                }
            }
        }

        public unsafe long TX1_Frequency
        {
            get
            {
                if (_device != IntPtr.Zero)
                {
                    if (LMS_GetLOFrequency(_device, LMS_CH_RX, TX1_channel, ref TX1_centerFrequency) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }

                return (long)TX1_centerFrequency;
            }
            set
            {
                TX1_centerFrequency = value;

                if (value >= 48 * 1e6)
                {
                    if (_device != IntPtr.Zero)
                    {
                        if (LMS_SetNCOIndex(_device, LMS_CH_TX, TX1_channel, 15, false) != 0)   // 0.0 NCO
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }

                        if (LMS_SetLOFrequency(_device, LMS_CH_TX, TX1_channel, TX1_centerFrequency) != 0)
                        {
                            throw new ApplicationException(limesdr_strerror());
                        }
                    }
                }
                else
                {
                    if (LMS_SetLOFrequency(_device, LMS_CH_TX, TX1_channel, 48.0 * 1e6) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                    double[] losc_freq = new double[16];
                    double[] pho = new double[1];

                    fixed (double* freq = &losc_freq[0])
                    fixed (double* pho_ptr = &pho[0])
                    {
                        losc_freq[0] = 48.0 * 1e6 - TX1_centerFrequency;
                        losc_freq[15] = 0.0;

                        if (LMS_SetNCOFrequency(_device, LMS_CH_TX, TX1_channel, freq, 0.0) != 0)
                        {
                            //throw new ApplicationException(limesdr_strerror());
                            Debug.Write("Wrong TX1 NCO frequency value!" + freq[0].ToString("f3") + "\n");
                        }

                        if (LMS_SetNCOIndex(_device, LMS_CH_TX, TX1_channel, 0, false) != 0)
                        {
                            //throw new ApplicationException(limesdr_strerror());
                            Debug.Write("Wrong TX 1NCO index value!" + freq[0].ToString("f3") + "\n");
                        }

                        /*if (LMS_GetNCOFrequency(_device, LMS_CH_RX, RX0_channel, freq, pho_ptr) != 0)
                        {
                            //throw new ApplicationException(limesdr_strerror());
                        }*/
                    }
                }
            }
        }

        public uint RXChannel
        {
            get
            {
                return RX_channel;
            }
            set
            {
                RX_channel = value;
            }
        }

        public uint TXChannel
        {
            get
            {
                return TX0_channel;
            }
            set
            {
                TX0_channel = value;
            }
        }

        public unsafe uint Antenna_RX0
        {
            get
            {
                return RX0_ant;
            }
            set
            {
                RX0_ant = value;

                if (LMS_SetAntenna(_device, LMS_CH_RX, RX_channel, RX0_ant) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }

                /*uint[] buffer = new uint[10];

                buffer[0] = 15;
                fixed (uint* buf = &buffer[0])
                    if (LMS_GPIODirWrite(_device, buf, 1) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                buffer[0] = 64;
                fixed (uint* buf = &buffer[0])
                    if (LMS_GPIOWrite(_device, buf, 1) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }*/
            }
        }

        public uint Antenna_RX1
        {
            get
            {
                return RX1_ant;
            }
            set
            {
                RX1_ant = value;

                if (LMS_SetAntenna(_device, LMS_CH_RX, RX_channel, RX1_ant) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }
        }

        public unsafe uint Antenna_TX0
        {
            get
            {
                return TX0_ant;
            }
            set
            {
                TX0_ant = value;

                if (LMS_SetAntenna(_device, LMS_CH_TX, TX0_channel, TX0_ant) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }
        }

        public void SetLPFBW(double lpfbw)
        {
            LPFBW = lpfbw;

            if (isStreaming)
            {
                if (LMS_SetLPFBW(_device, LMS_CH_RX, RX_channel, LPFBW) != 0)
                {
                    throw new ApplicationException(limesdr_strerror());
                }
            }
        }

        public double RXSampleRate
        {
            get { return RXsampleRate; }

            set
            {
                RXsampleRate = value;

                if (isStreaming)
                {
                    isStreaming = false;
                    Thread.Sleep(100);
                }
            }
        }

        public double TXSampleRate
        {
            get { return TXsampleRate; }

            set
            {
                TXsampleRate = value;

                if (isStreaming)
                {
                    isStreaming = false;
                    Thread.Sleep(100);
                }
            }
        }

        public double RX0_gain
        {
            get { return rx0_gain; }

            set
            {
                rx0_gain = (uint)value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_SetGaindB(_device, LMS_CH_RX, RX_channel, rx0_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }
        }

        public double RX1_gain
        {
            get { return rx1_gain; }

            set
            {
                rx1_gain = (uint)value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_SetGaindB(_device, LMS_CH_RX, RX_channel, rx1_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }
        }

        public double TX0_gain
        {
            get { return tx0_gain; }

            set
            {
                tx0_gain = (uint)value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_SetGaindB(_device, LMS_CH_TX, TX0_channel, tx0_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }
        }

        public double TX1_gain
        {
            get { return tx0_gain; }

            set
            {
                tx1_gain = (uint)value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_SetGaindB(_device, LMS_CH_TX, TX1_channel, tx1_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }
        }

        public ushort PGA_Gain
        {
            get { return pga_gain; }

            set
            {
                pga_gain = value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_WriteParam(_device, PGA_gain, pga_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                }
            }
        }

        public ushort TIA_Gain
        {
            get { return tia_gain; }

            set
            {
                tia_gain = value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_WriteParam(_device, TIA_gain, tia_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                }
            }
        }

        public ushort LNA_Gain
        {
            get { return lna_gain; }

            set
            {
                lna_gain = value;

                if (_device != IntPtr.Zero)
                {
                    if (LMS_WriteParam(_device, LNA_gain, lna_gain) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }
                }
            }
        }

        public unsafe bool BandFilter(uint bandFilter)
        {
            try
            {
                band_Filter = bandFilter;
                uint[] buffer = new uint[1];
                buffer[0] = (uint)bandFilter;

                if (mox)
                    buffer[0] = (uint)(bandFilter | 0x10);

                fixed (uint* buf = &buffer[0])
                    if (LMS_GPIOWrite(_device, buf, 1) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public unsafe uint ReadKeyer()
        {
            try
            {
                uint[] buffer = new uint[1];

                fixed (uint* buf = &buffer[0])
                    if (LMS_GPIORead(_device, buf, 1) != 0)
                    {
                        throw new ApplicationException(limesdr_strerror());
                    }

                return buffer[0];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return 255;
            }
        }
    }
}