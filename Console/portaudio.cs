//=================================================================
// portaudio.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2009  FlexRadio Systems 
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
 *  Copyright (C)2010,2011 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/



using System;
//using System.Collections;
//using System.Diagnostics;
using System.Collections;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
//using System.Threading;
using System.Windows.Forms;

using PaError = System.Int32;
using PadeviceIndex = System.Int32;
using PaHostApiIndex = System.Int32;
using PaTime = System.Double;
using PaSampleFormat = System.UInt32;
using PaStreamFlags = System.UInt32;
using PaStreamCallbackFlags = System.UInt32;


namespace PowerSDR
{
    public class PA19
    {
        #region Constants

        public const PadeviceIndex paNodevice = (PadeviceIndex)(-1);
        public const PadeviceIndex paUseHostApiSpecificdeviceSpecification = (PadeviceIndex)(-2);
        public const PaSampleFormat paFloat32 = (PaSampleFormat)0x01;
        public const PaSampleFormat paInt32 = (PaSampleFormat)0x02;
        public const PaSampleFormat paInt24 = (PaSampleFormat)0x04;
        public const PaSampleFormat paInt16 = (PaSampleFormat)0x08;
        public const PaSampleFormat paInt8 = (PaSampleFormat)0x10;
        public const PaSampleFormat paUInt8 = (PaSampleFormat)0x20;
        public const PaSampleFormat paCustomFormat = (PaSampleFormat)0x10000;
        public const PaSampleFormat paNonInterleaved = (PaSampleFormat)0x80000000;
        public const uint paFormatIsSupported = 0;
        public const uint paFramesPerBufferUnspecified = 0;
        public const PaStreamFlags paNoFlag = (PaStreamFlags)0;
        public const PaStreamFlags paClipOff = (PaStreamFlags)0x01;
        public const PaStreamFlags paDitherOff = (PaStreamFlags)0x02;
        public const PaStreamFlags paNeverDropInput = (PaStreamFlags)0x04;
        public const PaStreamFlags paPrimeOutputBuffersUsingStreamCallback = (PaStreamFlags)0x08;
        public const PaStreamFlags paPlatformSpecificFlags = (PaStreamFlags)0xFFFF0000;
        public const PaStreamCallbackFlags paInputUnderflow = (PaStreamCallbackFlags)0x01;
        public const PaStreamCallbackFlags paInputOverflow = (PaStreamCallbackFlags)0x02;
        public const PaStreamCallbackFlags paOutputUnderflow = (PaStreamCallbackFlags)0x04;
        public const PaStreamCallbackFlags paOutputOverflow = (PaStreamCallbackFlags)0x08;
        public const PaStreamCallbackFlags paPrimingOutput = (PaStreamCallbackFlags)0x10;
        public const UInt32 AUDCLNT_STREAMFLAGS_CROSSPROCESS = 0x00010000;
        public const UInt32 AUDCLNT_STREAMFLAGS_LOOPBACK = 0x00020000;
        public const UInt32 AUDCLNT_STREAMFLAGS_EVENTCALLBACK = 0x0004000;
        public const UInt32 AUDCLNT_STREAMFLAGS_NOPERSIST = 0x00080000;

        #endregion

        #region Enums

        public enum PaErrorCode
        {
            paNoError = 0, paNotInitialized = -10000, paUnanticipatedHostError, paInvalidChannelCount,
            paInvalidSampleRate, paInvaliddevice, paInvalidFlag, paSampleFormatNotSupported,
            paBadIOdeviceCombination, paInsufficientMemory, paBufferTooBig, paBufferTooSmall,
            paNullCallback, paBadStreamPtr, paTimedOut, paInternalError,
            padeviceUnavailable, paIncompatibleHostApiSpecificStreamInfo, paStreamIsStopped, paStreamIsNotStopped,
            paInputOverflowed, paOutputUnderflowed, paHostApiNotFound, paInvalidHostApi,
            paCanNotReadFromACallbackStream, paCanNotWriteToACallbackStream, paCanNotReadFromAnOutputOnlyStream, paCanNotWriteToAnInputOnlyStream,
            paIncompatibleStreamHostApi
        }

        public enum PaHostApiTypeId
        {
            paInDevelopment = 0, paDirectSound = 1, paMME = 2, paASIO = 3,
            paSoundManager = 4, paCoreAudio = 5, paOSS = 7, paALSA = 8,
            paAL = 9, paBeOS = 10, paWDMKS = 11, paJACK = 12, paWASAPI = 13, paAudioScience = 14,
        }

        public enum PaStreamCallbackResult
        { paContinue = 0, paComplete = 1, paAbort = 2 }

        /* Jack connection type */
        public enum PaWasapiJackConnectionType
        {
            eJackConnTypeUnknown,
            eJackConnType3Point5mm,
            eJackConnTypeQuarter,
            eJackConnTypeAtapiInternal,
            eJackConnTypeRCA,
            eJackConnTypeOptical,
            eJackConnTypeOtherDigital,
            eJackConnTypeOtherAnalog,
            eJackConnTypeMultichannelAnalogDIN,
            eJackConnTypeXlrProfessional,
            eJackConnTypeRJ11Modem,
            eJackConnTypeCombination,
        }

        /* Jack geometric location */
        public enum PaWasapiJackGeoLocation
        {
            eJackGeoLocUnk = 0,
            eJackGeoLocRear = 0x1, /* matches EPcxGeoLocation::eGeoLocRear */
            eJackGeoLocFront,
            eJackGeoLocLeft,
            eJackGeoLocRight,
            eJackGeoLocTop,
            eJackGeoLocBottom,
            eJackGeoLocRearPanel,
            eJackGeoLocRiser,
            eJackGeoLocInsideMobileLid,
            eJackGeoLocDrivebay,
            eJackGeoLocHDMI,
            eJackGeoLocOutsideMobileLid,
            eJackGeoLocATAPI,
            eJackGeoLocReserved5,
            eJackGeoLocReserved6,
        }

        /* Jack general location */
        public enum PaWasapiJackGenLocation
        {
            eJackGenLocPrimaryBox = 0,
            eJackGenLocInternal,
            eJackGenLocSeparate,
            eJackGenLocOther
        }

        /* Jack's type of port */
        public enum PaWasapiJackPortConnection
        {
            eJackPortConnJack = 0,
            eJackPortConnIntegrateddevice,
            eJackPortConnBothIntegratedAndJack,
            eJackPortConnUnknown
        }

        /* device role */
        public enum PaWasapideviceRole
        {
            eRoleRemoteNetworkdevice = 0,
            eRoleSpeakers,
            eRoleLineLevel,
            eRoleHeadphones,
            eRoleMicrophone,
            eRoleHeadset,
            eRoleHandset,
            eRoleUnknownDigitalPassthrough,
            eRoleSPDIF,
            eRoleHDMI,
            eRoleUnknownFormFactor
        }

        /* Thread priority */
        public enum PaWasapiThreadPriority
        {
            eThreadPriorityNone = 0,
            eThreadPriorityAudio,            //!< Default for Shared mode.
            eThreadPriorityCapture,
            eThreadPriorityDistribution,
            eThreadPriorityGames,
            eThreadPriorityPlayback,
            eThreadPriorityProAudio,        //!< Default for Exclusive mode.
            eThreadPriorityWindowManager
        }

        /* Setup flags */
        public enum PaWasapiFlags
        {
            /* puts WASAPI into exclusive mode */
            paWinWasapiExclusive = 1,

            /* allows to skip internal PA processing completely */
            paWinWasapiRedirectHostProcessor = 2,

            /* assigns custom channel mask */
            paWinWasapiUseChannelMask = 4,

            /* selects non-Event driven method of data read/write
               Note: WASAPI Event driven core is capable of 2ms latency!!!, but Polling
                     method can only provide 15-20ms latency. */
            paWinWasapiPolling = 8,

            /* forces custom thread priority setting. must be used if PaWasapiStreamInfo::threadPriority 
               is set to custom value. */
            paWinWasapiThreadPriority = 16
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct PaHostApiInfo
        {
            public int structVersion;
            public int type;
            [MarshalAs(UnmanagedType.LPStr)]
            public string name;
            public int deviceCount;
            public PadeviceIndex defaultInputdevice;
            public PadeviceIndex defaultOutputdevice;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PaHostErrorInfo
        {
            public PaHostApiTypeId hostApiType;
            public int errorCode;
            [MarshalAs(UnmanagedType.LPStr)]
            public string errorText;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PADeviceInfo
        {
            public int structVersion;
            [MarshalAs(UnmanagedType.LPStr)]
            public string name;
            public PaHostApiIndex hostApi;
            public int maxInputChannels;
            public int maxOutputChannels;
            public PaTime defaultLowInputLatency;
            public PaTime defaultLowOutputLatency;
            public PaTime defaultHighInputLatency;
            public PaTime defaultHighOutputLatency;
            public double defaultSampleRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        unsafe public struct PaStreamParameters
        {
            public PadeviceIndex device;
            public int channelCount;
            public PaSampleFormat sampleFormat;
            public PaTime suggestedLatency;
            public void* hostApiSpecificStreamInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PaStreamCallbackTimeInfo
        {
            public PaTime inputBufferAdcTime;
            public PaTime currentTime;
            public PaTime outputBufferDacTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PaStreamInfo
        {
            public int structVersion;
            public PaTime inputLatency;
            public PaTime outputLatency;
            public double sampleRate;
        }


        /* Wasapi Jack descriptor. */
        [StructLayout(LayoutKind.Sequential)]
        public struct PaWasapiJackDescription
        {
            public UInt32 channelMapping;
            public UInt32 color; /* derived from macro: #define RGB(r,g,b) ((COLORREF)(((BYTE)(r)|((WORD)((BYTE)(g))<<8))|(((DWORD)(BYTE)(b))<<16))) */
            public PaWasapiJackConnectionType connectionType;
            public PaWasapiJackGeoLocation geoLocation;
            public PaWasapiJackGenLocation genLocation;
            public PaWasapiJackPortConnection portConnection;
            public UInt32 isConnected;
        }

        /* Stream descriptor. */
        [StructLayout(LayoutKind.Sequential)]
        public struct PaWasapiStreamInfo
        {
            public UInt32 size;             /**< sizeof(PaWasapiStreamInfo) */
            public PaHostApiTypeId hostApiType;    /**< paWASAPI */
            public UInt32 version;          /**< 1 */

            public UInt32 flags;            /**< collection of PaWasapiFlags */

            /* Support for WAVEFORMATEXTENSIBLE channel masks. If flags contains
               paWinWasapiUseChannelMask this allows you to specify which speakers 
               to address in a multichannel stream. Constants for channelMask
               are specified in pa_win_waveformat.h. Will be used only if 
               paWinWasapiUseChannelMask flag is specified.
            */
            public UInt32 channelMask;

            /* Delivers raw data to callback obtained from GetBuffer() methods skipping 
               internal PortAudio processing inventory completely. userData parameter will 
               be the same that was passed to Pa_OpenStream method. Will be used only if 
               paWinWasapiRedirectHostProcessor flag is specified.
            */
            //public PaWasapiHostProcessorCallback hostProcessorOutput;
            //public PaWasapiHostProcessorCallback hostProcessorInput;

            /* Specifies thread priority explicitly. Will be used only if paWinWasapiThreadPriority flag
               is specified.

               Please note, if Input/Output streams are opened simultaniously (Full-Duplex mode)
               you shall specify same value for threadPriority or othervise one of the values will be used
               to setup thread priority.
            */
            public PaWasapiThreadPriority threadPriority;
        }


        #endregion

        #region Function Definitions

        [DllImport("PA19.dll")]
        public static extern int PA_GetVersion();

#if(WIN32)
        [DllImport("PA19.dll")]
        public static extern String PA_GetVersionText();
#endif
#if(WIN64)
        [DllImport("PA19.dll", CharSet = CharSet.None, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.LPTStr)]
        public static extern String PA_GetVersionText();
#endif

        // note that using the stock source and calling this function
        // on errorCode = 0 will result in an Exception (no object
        // reference.  To fix this, I added a single statement in
        // pa_front.c.  The new line 444 is below.
        // case paNoError:                  result = "1"; result = "Success"; break;
#if(WIN32)
        [DllImport("PA19.dll")]
        public static extern String PA_GetErrorText(PaError errorCode);
#endif
#if(WIN64)
        [DllImport("PA19.dll", CharSet = CharSet.None, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.LPTStr)]
        public static extern String PA_GetErrorText(PaError errorCode);
#endif

        [DllImport("PA19.dll")]
        public static extern PaError PA_Initialize();

        [DllImport("PA19.dll")]
        public static extern PaError PA_Terminate();

        [DllImport("PA19.dll")]
        public static extern PaHostApiIndex PA_GetHostApiCount();

        [DllImport("PA19.dll")]
        public static extern PaHostApiIndex PA_GetDefaultHostApi();

        // Added layer to convert from the struct pointer to a C# 
        // struct automatically.
        [DllImport("PA19.dll", EntryPoint = "PA_GetHostApiInfo")]
        public static extern IntPtr PA_GetHostApiInfoPtr(int hostId);
        public static PaHostApiInfo PA_GetHostApiInfo(int hostId)
        {
            IntPtr ptr = PA_GetHostApiInfoPtr(hostId);
            PaHostApiInfo info = (PaHostApiInfo)Marshal.PtrToStructure(ptr, typeof(PaHostApiInfo));
            return info;
        }

        [DllImport("PA19.dll")]
        public static extern PaHostApiIndex PA_HostApiTypeIdToHostApiIndex(PaHostApiTypeId type);

        [DllImport("PA19.dll")]
        public static extern PadeviceIndex PA_HostApiDeviceIndexToDeviceIndex(int hostAPI, int hostApideviceIndex);

        [DllImport("PA19.dll", EntryPoint = "PA_GetLastHostErrorInfo")]
        public static extern IntPtr PA_GetLastHostErrorInfoPtr();
        public static PaHostErrorInfo PA_GetLastHostErrorInfo()
        {
            IntPtr ptr = PA_GetLastHostErrorInfoPtr();
            PaHostErrorInfo info = (PaHostErrorInfo)Marshal.PtrToStructure(ptr, typeof(PaHostErrorInfo));
            return info;
        }

        [DllImport("PA19.dll")]
        public static extern PadeviceIndex PA_GetDeviceCount();

        [DllImport("PA19.dll")]
        public static extern PadeviceIndex PA_GetDefaultInputDevice();

        [DllImport("PA19.dll")]
        public static extern PadeviceIndex PA_GetDefaultOutputDevice();

        [DllImport("PA19.dll", EntryPoint = "PA_GetDeviceInfo")]
        public static extern IntPtr PA_GetDeviceInfoPtr(int device);
        public static PADeviceInfo PA_GetDeviceInfo(int device)
        {
            IntPtr ptr = PA_GetDeviceInfoPtr(device);
            PADeviceInfo info = (PADeviceInfo)Marshal.PtrToStructure(ptr, typeof(PADeviceInfo));
            return info;
        }

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_IsFormatSupported(
            PaStreamParameters* inputParameters,
            PaStreamParameters* outputParameters,
            double sampleRate);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_OpenStream(
            out void* stream,
            PaStreamParameters* inputParameters,
            PaStreamParameters* outputParameters,
            double sampleRate,
            uint framesPerBuffer,
            PaStreamFlags streamFlags,
            PaStreamCallback streamCallback,
            int user_data, int callback_id);		// 0 for callback1, 1 callback2...

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_OpenDefaultStream(
            out void* stream,
            int numInputChannels,
            int numOutputChannels,
            PaSampleFormat sampleFormat,
            double sampleRate,
            uint framesPerBuffer,
            PaStreamCallback streamCallback,
            int user_data, int callback_id);		// 0 for callback1, 1 callback2...

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_CloseStream(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_SetStreamFinishedCallback(
            void* stream, PaStreamFinishedCallback streamFinishedCallback);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_StartStream(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_StopStream(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_AbortStream(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_IsStreamStopped(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_IsStreamActive(void* stream);

        [DllImport("PA19.dll", EntryPoint = "PA_GetStreamInfo")]
        unsafe public static extern IntPtr PA_GetStreamInfoPtr(void* stream);

        unsafe public static PaStreamInfo PA_GetStreamInfo(void* stream)
        {
            IntPtr ptr = PA_GetStreamInfoPtr(stream);
            PaStreamInfo info = (PaStreamInfo)Marshal.PtrToStructure(ptr, typeof(PaStreamInfo));
            return info;
        }

        [DllImport("PA19.dll")]
        unsafe public static extern PaTime PA_GetStreamTime(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern double PA_GetStreamCpuLoad(void* stream);

        // note: These next 4 blocking IO functions are only currently implemented
        // in MME (not DirectSound or ASIO)
        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_ReadStream(void* stream, void* buffer, uint frames);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PA_WriteStream(void* stream, void* buffer, uint frames);

        [DllImport("PA19.dll")]
        unsafe public static extern int PA_GetStreamReadAvailable(void* stream);

        [DllImport("PA19.dll")]
        unsafe public static extern int PA_GetStreamWriteAvailable(void* stream);

        [DllImport("PA19.dll")]
        public static extern PaError PA_GetSampleSize(PaSampleFormat format);

        [DllImport("PA19.dll")]
        public static extern void PA_Sleep(int msec);

        [DllImport("PA19.dll")]
        public static extern int PAWasapi_GetDeviceRole(PadeviceIndex index);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PAWasapi_GetJackCount(PadeviceIndex index, int* count);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PAWasapi_GetJackDescription(PadeviceIndex index, int jack_index, PaWasapiJackDescription* pJackDescription);

        [DllImport("PA19.dll")]
        unsafe public static extern int PAWasapi_GetDeviceDefaultFormat(void* pFormat, UInt32 nFormatSize, PadeviceIndex ndevice);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PAWasapi_ThreadPriorityBoost(void** hTask, PaWasapiThreadPriority nPriorityClass);

        [DllImport("PA19.dll")]
        unsafe public static extern PaError PAWasapi_ThreadPriorityRevert(void* hTask);

        unsafe public delegate int PaStreamCallback(void* input, void* output, int frameCount,
            PaStreamCallbackTimeInfo* timeInfo, int statusFlags, void* userData);

        unsafe public delegate void PaStreamFinishedCallback(void* userData);

        /*unsafe public delegate void PaWasapiHostProcessorCallback(void* inputBuffer, UInt32 inputFrames,
                                               void* outputBuffer, UInt32 outputFrames,
                                               void* userData);*/

        public delegate void PaWasapiHostProcessorCallback(float[] inputBuffer, UInt32 inputFrames,
                                      float[] outputBuffer, UInt32 outputFrames,
                                       int[] userData);

        #endregion	// Function Definitions
    }
}
