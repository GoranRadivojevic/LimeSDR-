//=================================================================
// console cross thread calls
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
 *  Copyright (C)2012 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using SDRSerialSupportII;
using Splash_Screen;
using System.Management;

#if DirectX
using SlimDX;
#endif

namespace PowerSDR
{
    unsafe public partial class Console : System.Windows.Forms.Form
    {
        #region variable

        public delegate void CATCrossThreadCallback(string type, int parm1, int[] parm2, string parm3);

        #endregion

        #region CAT Properties

        public int CATVFOMODE
        {
            get
            {
                switch (current_click_tune_mode)
                {

                    case ClickTuneMode.VFOB:
                        return 1;

                    default:
                        return 0;
                }
            }

            set
            {
                switch (value)
                {
                    case 0:
                        current_click_tune_mode = ClickTuneMode.VFOA;
                        break;

                    case 1:
                        current_click_tune_mode = ClickTuneMode.VFOB;
                        break;

                    case 0xa0:                          // equalize freq
                        VFOBFreq = VFOAFreq;
                        break;

                    case 0xb0:                          // exchange
                        double freq = vfoBFreq;
                        VFOAFreq = VFOBFreq;
                        VFOBFreq = freq;
                        break;

                    case 0xb1:                          // uqualize freq,mode,filters
                        VFOBFreq = VFOAFreq;
                        CurrentDSPModeSubRX = CurrentDSPMode;
                        CurrentFilterSubRX = CurrentFilter;
                        break;

                    case 0xc0:
                        chkEnableSubRX.Checked = false;
                        break;

                    case 0xc1:
                        chkEnableSubRX.Checked = true;
                        break;
                }
            }
        }

        public double CATVFOA
        {
            get { return vfoAFreq; }
        }

        public double CATVFOB
        {
            get { return vfoBFreq; }
        }

        public double CATLOSC
        {
            get { return loscFreq; }
        }

        public int CATTXProfileCount
        {
            get
            {
                return comboTXProfile.Items.Count;
            }
        }

        public int CATTXProfile
        {
            get
            {
                return comboTXProfile.SelectedIndex;
            }
            set
            {
                comboTXProfile.SelectedIndex = value;
            }
        }

       /* public string CATPanSwap
        {
            get
            {
                if (chkPanSwap.Checked)
                    return "1";
                else
                    return "0";
            }
            set
            {
                if (value == "1")
                    chkPanSwap.Checked = true;
                else
                    chkPanSwap.Checked = false;
            }
        }*/

        public string CATCWMonitor      // yt7pwr
        {
            get
            {
                if (chkMON.Checked)
                    return "1";
                else
                    return "0";
            }
            set
            {
                if (value == "1")
                    chkMON.Checked = true;
                else
                    chkMON.Checked = false;
            }
        }

        public string CATSubRX      // yt7pwr
        {
            get
            {
                if (chkEnableSubRX.Checked)
                    return "1";
                else
                    return "0";
            }
            set
            {
                if (value == "1")
                    chkEnableSubRX.Checked = true;
                else
                    chkEnableSubRX.Checked = false;
            }
        }

        public string CATPhoneDX
        {
            get
            {
                /*                if (chkDX.Checked)
                                    return "1";
                                else*/
                return "0";
            }
            set
            {
                /*                if (value == "1")
                                    chkDX.Checked = true;
                                else
                                    chkDX.Checked = false;*/
            }
        }

        public string CATRXEQ
        {
            get
            {
                /*                if (chkRXEQ.Checked)
                                    return "1";
                                else*/
                return "0";
            }
            set
            {
                /*                if (value == "1")
                                    chkRXEQ.Checked = true;
                                else
                                    chkRXEQ.Checked = false;*/
            }
        }

        public string CATTXEQ
        {
            get
            {
                /*                if (chkTXEQ.Checked)
                                    return "1";
                                else*/
                return "0";
            }
            set
            {
                /*                if (value == "1")
                                    chkTXEQ.Checked = true;
                                else
                                    chkTXEQ.Checked = false;*/
            }
        }

        public string CATDispPeak
        {
            get
            {
                if (chkDisplayPeak.Checked)
                    return "1";
                else
                    return "0";
            }
            set
            {
                if (value == "1")
                    chkDisplayPeak.Checked = true;
                else
                    chkDisplayPeak.Checked = false;
            }

        }

        public string CATDispCenter
        {
            set
            {
                ptbDisplayPan.Value = 0;
            }
        }

        public string CATDispZoom
        {
            set
            {
                switch (value)
                {
                    case "0":
                        radDisplayZoom1x.PerformClick();
                        break;
                    case "1":
                        radDisplayZoom2x.PerformClick();
                        break;
                    case "2":
                        radDisplayZoom4x.PerformClick();
                        break;
                    case "3":
                        radDisplayZoom8x.PerformClick();
                        break;
                    case "4":
                        radDisplayZoom16x.PerformClick();
                        break;
                    case "5":
                        radDisplayZoom32x.PerformClick();
                        break;
                    default:
                        radDisplayZoom1x.PerformClick();
                        break;
                }
            }

            get
            {
                if (radDisplayZoom1x.Checked)
                    return "0";
                else if (radDisplayZoom2x.Checked)
                    return "1";
                else if (radDisplayZoom4x.Checked)
                    return "2";
                else if (radDisplayZoom8x.Checked)
                    return "3";
                else if (radDisplayZoom16x.Checked)
                    return "4";
                else if (radDisplayZoom32x.Checked)
                    return "5";
                else
                    return "0";
            }
        }

        public string CATTuneStepUp
        {
            set
            {
                if (value == "1")
                    ChangeWheelTuneLeft();
            }
        }

        public string CATTuneStepDown
        {
            set
            {
                if (value == "1")
                    ChangeWheelTuneRight();
            }
        }

        // props for cat control 
        // Added 06/20/05 BT for CAT commands
        private int cat_nr_status = 0;
        public int CATNR
        {
            get { return cat_nr_status; }
            set
            {
                if (value == 0)
                {
                    chkNR.Checked = false;
                    cat_nr_status = 0;
                }
                else if (value == 1)
                {
                    chkNR.Checked = true;
                    cat_nr_status = 1;
                }
            }
        }

        private int cat_nr_gain = 0;
        public int CATNRgain
        {
            get { return (int)SetupForm.udLMSNRgain.Value; }
            set
            {
                if (SetupForm.udLMSNRgain != null)
                {
                    value = Math.Max(1, value);
                    value = Math.Min(9999, value);
                    SetupForm.udLMSNRgain.Value = value;
                    cat_nr_gain = value;
                }
            }
        }

        private int cat_notch_status = 0;
        public int CATNOTCHenable
        {
            get { return cat_notch_status; }
            set
            {
                if (value == 0)
                {
                    chkManualNotchFilter.Checked = false;
                    cat_notch_status = 0;
                }
                else if (value == 1)
                {
                    chkManualNotchFilter.Checked = true;
                    cat_notch_status = 1;
                }
            }
        }

        public int CATNOTCHgain
        {
            get { return (int)SetupForm.udLMSANFgain.Value; }
            set
            {
                value = Math.Max(1, value);
                value = Math.Min(9999, value);
                SetupForm.udLMSANFgain.Value = value;
                cat_notch_status = 0;
            }
        }

        public int CATNOTCHManual
        {
            get { return (int)ptbNotchShift.Value; }
            set
            {
                value = Math.Max(-5000, value);
                value = Math.Min(5000, value);
                ptbNotchShift.Value = value;
                ptbNotchShift_Scroll(this, EventArgs.Empty);
            }
        }

        // Added 06/20/05 BT for CAT commands
        private int cat_anf_status = 0;
        public int CATANF
        {
            get { return cat_anf_status; }
            set
            {
                if (value == 0)
                {
                    chkANF.Checked = false;
                    cat_anf_status = 0;
                }
                else if (value == 1)
                {
                    chkANF.Checked = true;
                    cat_anf_status = 1;
                }
            }
        }

        // Added 06/21/05 BT for CAT Commands
        private int cat_nb1_status = 0;
        public int CATNB1
        {
            get { return cat_nb1_status; }
            set
            {
                if (value == 0)
                {
                    chkNB.Checked = false;
                    cat_nb1_status = 0;
                }
                else if (value == 1)
                {
                    chkNB.Checked = true;
                    cat_nb1_status = 1;
                }
            }
        }

        // Added 06/21/05 BT for CAT commands
        private int cat_nb2_status = 0;
        public int CATNB2
        {
            get { return cat_nb2_status; }
            set
            {
                if (value == 0)
                {
                    chkDSPNB2.Checked = false;
                    cat_nb2_status = 0;
                }
                else if (value == 1)
                {
                    chkDSPNB2.Checked = true;
                    cat_nb2_status = 1;
                }
            }
        }

        // Added 06/22/05 BT for CAT commands
        private int cat_cmpd_status = 0;
        public int CATCmpd
        {
            get { return cat_cmpd_status; }
            set
            {
                if (value == 0)
                {
                    chkDSPCompander.Checked = false;
                    cat_cmpd_status = 0;
                }
                else if (value == 1)
                {
                    chkDSPCompander.Checked = true;
                    cat_cmpd_status = 1;
                }
            }
        }

        // Added 06/22/05 BT for CAT commands
        private int cat_mic_status = 0;
        public int CATMIC
        {
            get
            {
                cat_mic_status = (int)udMIC.Value;
                return cat_mic_status;
            }
            set
            {
                value = Math.Max(0, value);
                value = Math.Min(70, value);
                udMIC.Value = value;
            }
        }

        // Added 06/22/05 BT for CAT commands
        // modified 07/22/05 to fix display problem
        private int cat_filter_width = 0;
        public int CATFilterWidth
        {
            get
            {
                cat_filter_width = ptbFilterWidth.Value;
                return cat_filter_width;
            }
            set
            {
                value = Math.Max(1, value);
                value = Math.Min(10000, value);
                ptbFilterWidth.Value = value;
                tbFilterWidth_Scroll(this, EventArgs.Empty);	// added
            }
        }

        // Added 07/22/05 for cat commands
        public int CATFilterShift
        {
            get
            {
                return ptbFilterShift.Value;
            }
            set
            {
                value = Math.Max(-1000, value);
                value = Math.Min(1000, value);
                ptbFilterShift.Value = value;
                tbFilterShift_Scroll(this.ptbFilterShift, EventArgs.Empty);
            }
        }

        // Added 07/22/05 for CAT commands
        public int CATFilterShiftReset
        {
            set
            {
                if (value == 1)
                    btnFilterShiftReset.PerformClick();
            }
        }

        // Added 06/22/05 BT for CAT commands
        private int cat_bin_status = 0;
        public int CATBIN
        {
            get
            { return cat_bin_status; }

            set
            {
                if (value == 1)
                {
                    chkBIN.Checked = true;
                    cat_bin_status = 1;
                }
                else if (value == 0)
                {
                    chkBIN.Checked = false;
                    cat_bin_status = 0;
                }
            }
        }

        // Added 06/30/05 BT for CAT commands
        public int CATCWSpeed
        {
            get
            {
                return (int)udCWSpeed.Value;
            }
            set
            {
                value = Math.Max(7, value);
                value = Math.Min(60, value);
                udCWSpeed.Value = value;
            }
        }

        // Added 06/30/05 BT for CAT commands
        private int cat_display_avg_status = 0;
        public int CATDisplayAvg
        {
            get
            {
                if (chkDisplayAVG.Checked)
                    cat_display_avg_status = 1;
                else
                    cat_display_avg_status = 0;

                return cat_display_avg_status;
            }
            set
            {
                if (value == 1)
                    chkDisplayAVG.Checked = true;
                else
                    chkDisplayAVG.Checked = false;
            }
        }

        // Added 06/30/05 BT for CAT commands
        private int cat_squelch_status = 0;
        public int CATSquelch
        {
            get
            {
                if (chkSQLMainRX.Checked)
                    cat_squelch_status = 1;
                else
                    cat_squelch_status = 0;

                return cat_squelch_status;
            }
            set
            {
                if (value == 1)
                {
                    chkSQLMainRX.Checked = true;
                    chkSQLMainRX.Checked = true;
                }
                else
                {
                    chkSQLMainRX.Checked = false;
                    chkSQLMainRX.Checked = false;
                }
            }
        }

        private int cat_squelch_subrx_status = 0;       // yt7pwr
        public int CATSquelchSubRX
        {
            get
            {
                if (chkSQLSubRX.Checked)
                    cat_squelch_subrx_status = 1;
                else
                    cat_squelch_subrx_status = 0;

                return cat_squelch_subrx_status;
            }
            set
            {
                if (value == 1)
                {
                    chkSQLSubRX.Checked = true;
                }
                else
                {
                    chkSQLSubRX.Checked = false;
                }
            }
        }

        // Added 7/9/05 BT for cat commands
        public string CATQMSValue
        {
            get { return txtMemory.Text; }
        }

        private SDRSerialSupportII.SDRSerialPort.Parity cat_parity;
        public SDRSerialSupportII.SDRSerialPort.Parity CATParity
        {
            set { cat_parity = value; }
            get { return cat_parity; }
        }


        private SDRSerialSupportII.SDRSerialPort.StopBits cat_stop_bits;
        public SDRSerialSupportII.SDRSerialPort.StopBits CATStopBits
        {
            set { cat_stop_bits = value; }
            get { return cat_stop_bits; }
        }

        private SDRSerialSupportII.SDRSerialPort.DataBits cat_data_bits;
        public SDRSerialSupportII.SDRSerialPort.DataBits CATDataBits
        {
            set { cat_data_bits = value; }
            get { return cat_data_bits; }
        }

        private SDRSerialSupportII.SDRSerialPort.HandshakeBits cat_handshake = SDRSerialPort.HandshakeBits.None;       // yt7pwr
        public SDRSerialSupportII.SDRSerialPort.HandshakeBits CATHandshake
        {
            set { cat_handshake = value; }
            get { return cat_handshake; }
        }

        private int cat_baud_rate;
        public int CATBaudRate
        {
            set { cat_baud_rate = value; }
            get { return cat_baud_rate; }
        }

        private bool cat_enabled;
        public bool CATEnabled
        {
            set
            {
                cat_enabled = value;
                Keyer.CATEnabled = value;

                if (siolisten != null)  // if we've got a listener tell them about state change 
                {
                    if (cat_enabled && value)
                    {
                        Siolisten.disableCAT();
                        Siolisten.enableCAT();
                    }
                    else if (cat_enabled)
                    {
                        Siolisten.enableCAT();
                    }
                    else
                    {
                        Siolisten.disableCAT();
                    }
                }
            }
            get { return cat_enabled; }
        }

        private int cat_rig_type;
        public int CATRigType
        {
            get { return cat_rig_type; }
            set { cat_rig_type = value; }
        }

        private int cat_rig_address = 0x70;         // 112 0x70 ICOM IC-7000 default
        public int CATRigAddress
        {
            get { return cat_rig_address; }
            set { cat_rig_address = value; }
        }

        private bool cat_echo = false;
        public bool CATEcho
        {
            get { return cat_echo; }
            set { cat_echo = value; }
        }

        private int cat_port;
        public int CATPort
        {
            get { return cat_port; }
            set { cat_port = value; }
        }

        private bool cat_ptt_rts = false;
        public bool CATPTTRTS
        {
            get { return cat_ptt_rts; }
            set { cat_ptt_rts = value; }
        }

        private bool cat_ptt_dtr;
        public bool CATPTTDTR
        {
            get { return cat_ptt_dtr; }
            set { cat_ptt_dtr = value; }
        }

        public SerialPortPTT serialPTT = null;
        private bool ptt_bit_bang_enabled;
        public bool PTTBitBangEnabled  // changes yt7pwr
        {
            get { return ptt_bit_bang_enabled; }
            set
            {
                ptt_bit_bang_enabled = value;
                if (serialPTT != null)  // kill current serial PTT if we have one 
                {
                    serialPTT.Close();
                    serialPTT = null;
                }
                if (ptt_bit_bang_enabled)
                {
                    // wjt -- don't really like popping a msg box in here ...   nasty when we do a remoted 
                    // setup ... will let that wait for the great console refactoring 
                    try
                    {
                        //string ptt = SetupForm.comboCATPTTPort.Text;
                        serialPTT = new SerialPortPTT(CATPTTBingBangPort_name, cat_ptt_dtr, cat_ptt_rts);
                        serialPTT.Init();
                    }
                    catch (Exception ex)
                    {
                        ptt_bit_bang_enabled = false;
                        if (SetupForm != null)
                        {
                            SetupForm.copyCATPropsToDialogVars(); // need to make sure the props on the setup page get reset 
                        }
                        MessageBox.Show("Could not initialize PTT Bit Bang control.  Exception was:\n\n " + ex.Message +
                            "\n\nPTT Bit Bang control has been disabled.", "Error Initializing PTT control",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private int cat_ptt_bit_bang_port;
        public int CATPTTBitBangPort
        {
            get { return cat_ptt_bit_bang_port; }
            set { cat_ptt_bit_bang_port = value; }
        }

        private string cat_ptt_name = "COM1";
        public string CATPTTBingBangPort_name
        {
            get { return cat_ptt_name; }

            set { cat_ptt_name = value; }
        }

        private bool kw_auto_information = false;
        public bool KWAutoInformation
        {
            get { return kw_auto_information; }
            set { kw_auto_information = value; }
        }

        private int cat_att_status = 0;       // yt7pwr
        public int CATATTStatus
        {
            get
            {
                switch (current_model)
                {
                    default:
                        return 0;
                }
            }
            set
            {
                if (value == 0)
                {
                    switch (current_model)
                    {

                    }
                }
                else if (value == 1)
                {
                    switch (current_model)
                    {

                    }
                }

                cat_att_status = value;
            }
        }

        private int cat_rf_preamp_status = 0;       // yt7pwr
        public int CATRFPreampStatus
        {
            get 
            {
                switch (current_model)
                {
                    default:
                        return 0;
                }
            }
            set
            {
                if (value == 0)
                {

                }
                else if (value == 1)
                {
                
                }

                cat_rf_preamp_status = value;
            }
        }

        private int cat_af_preamp_status = 0;       // yt7pwr
        public int CATAFPreampStatus
        {
            get 
            {
                switch (current_model)
                {
                    default:
                        return 0;
                        break;
                }
            }
            set
            {
                switch (current_model)
                {

                }
            }
        }

        public bool CATCTCSSEnable          // yt7pwr
        {
            get { return chkCTCSS.Checked; }
            set
            {
                chkCTCSS.Checked = value;
            }
        }

        #endregion

        #region crossthread callbacks

        public void CATCallback(string type, int parm1, int[] parm2, string parm3)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CATCrossThreadCallback(CATCallback), type, parm1, parm2, parm3);
                return;
            }

            switch (type)
            {
                case "Restore":
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.BringToFront();
                        this.Show();
                    }
                    else
                        this.WindowState = FormWindowState.Minimized;
                    break;
                case "AF":
                    AF = parm1;
                    break;
                case "AF_mute":
                    chkMUT.Checked = !chkMUT.Checked;
                    break;
                case "AF+":
                    AF += 1;
                    break;
                case "AF-":
                    AF -= 1;
                    break;
                case "RF":
                    RF = parm1;
                    break;
                case "MIC":
                    CATMIC = parm1;
                    break;
                case "CW Monitor":
                    CATCWMonitor = parm1.ToString();
                    break;
                case "CW Speed":
                    CATCWSpeed = parm1;
                    break;
                case "CW Pitch":
                    SetupForm.CATCWPitch = parm1;
                    break;
                case "BreakIn Delay":
                    BreakInDelay = parm1;
                    break;
                case "BreakIn":
                    if (parm1 == 1)
                        BreakInEnabled = true;
                    else
                        BreakInEnabled = false;
                    break;
                case "Show CW TXfreq":
                    if (parm1 == 1)
                        ShowCWTXFreq = true;
                    else
                        ShowCWTXFreq = false;
                    break;
                case "CW Iambic":
                    if (parm1 == 1)
                        CWIambic = true;
                    else
                        CWIambic = false;
                    break;
                case "COMP":
                    if (parm1 == 1)
                        COMP = true;
                    else
                        COMP = false;
                    break;
                case "COMP Threshold":
                    SetupForm.CATCompThreshold = parm1;
                    break;
                case "COMP level":
                    parm1 = (int)Math.Min(udCOMP.Maximum, parm1);
                    parm1 = (int)Math.Max(udCOMP.Minimum, parm1);
                    udCOMP.Value = parm1;
                    break;
                case "CMPD":
                    CATCmpd = parm1;
                    break;
                case "CPDR":
                    CPDRVal = parm1;
                    break;
                case "VOX":
                    if (parm1 == 1)
                        VOXEnable = true;
                    else
                        VOXEnable = false;
                    break;
                case "VOX Gain":
                    VOXSens = parm1;
                    break;
                case "DSP Mode VFOA":
                    CurrentDSPMode = (DSPMode)parm1;
                    break;
                case "DSP Mode VFOB":
                    CurrentDSPModeSubRX = (DSPMode)parm1;
                    break;
                case "AGC mode":
                    CurrentAGCMode = (AGCMode)parm1;
                    break;
                case "Filter":
                    if(current_filter != (Filter)parm1)
                        CurrentFilter = (Filter)parm1;
                    break;
                case "Filter Width":
                    CATFilterWidth = parm1;
                    break;
                case "Filter Shift":
                    CATFilterShift = parm1;
                    break;
                case "Filter Low":
                    FilterLowValue = parm1;
                    UpdateFilters(parm1, FilterHighValue);
                    break;
                case "Filter High":
                    FilterHighValue = parm1;
                    UpdateFilters(FilterLowValue, parm1);
                    break;
                case "Filter VFOB":
                    if(current_filter_subRX != (Filter)parm1)
                        CurrentFilterSubRX = (Filter)parm1;
                    break;
                case "TXFilter High":
                    SetupForm.TXFilterHigh = parm1;
                    break;
                case "TXFilter Low":
                    SetupForm.TXFilterLow = parm1;
                    break;
                case "SQL VFOA":
                    SquelchMainRX = parm1;
                    break;
                case "SQL VFOA Enable":
                    CATSquelch = parm1;
                    break;
                case "SQL VFOB":
                    SquelchSubRX = parm1;
                    break;
                case "SQL VFOB Enable":
                    CATSquelchSubRX = parm1;
                    break;
                case "CWX Remote Msg":
                    byte[] msg = new byte[parm2.Length];

                    for (int i = 0; i < msg.Length; i++)
                        msg[i] = (byte)parm2[i];

                    CWXForm.RemoteMessage(msg);
                    break;
                case "CWX Stop":
                    CWX_Playing = false;
                    break;
                case "CWX Start":
                    if (CWXForm == null || CWXForm.IsDisposed)
                    {
                        try
                        {
                            CWXForm = new CWX(this);
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.ToString());
                        }
                    }
                    break;
                case "PWR":
                    PWR = parm1;
                    break;
                case "RIT Up":
                    break;
                case "RIT Down":
                    RITValue -= parm1;
                    break;
                case "RIT Clear":
                    RITValue = 0;
                    break;
                case "RIT Enable":
                    if (parm1 == 1)
                        RITOn = true;
                    else
                        RITOn = false;
                    break;
                case "RIT Value":
                    RITValue = parm1;
                    break;
                case "XIT Up":
                    break;
                case "XIT Down":
                    break;
                case "XIT Status":
                    if (parm1 == 1)
                        XITOn = true;
                    else
                        XITOn = false;
                    break;
                case "XIT Value":
                    XITValue = parm1;
                    break;
                case "StepSize VFOA":
                    StepSize = parm1;
                    break;
                case "StepSize VFOA up":
                    CATTuneStepUp = parm1.ToString();
                    break;
                case "StepSize VFOA down":
                    CATTuneStepDown = parm1.ToString();
                    break;
                case "StepSize VFOB":
                    StepSizeSubRX = parm1;
                    break;
                case "VFOA down":
                    VFOAFreq = vfoAFreq - Step2Freq(parm1);
                    break;
                case "VFOA step down":
                    VFOAFreq = vfoAFreq - wheel_tune_list[StepSize];
                    break;
                case "VFOA step up":
                    VFOAFreq = vfoAFreq + wheel_tune_list[StepSize];
                    break;
                case "VFOA up":
                    VFOAFreq = vfoAFreq + Step2Freq(parm1);
                    break;
                case "VFOA freq":
                    VFOAFreq = double.Parse(parm3) / 1e6;
                    break;
                case "VFOB freq":
                    VFOBFreq = double.Parse(parm3) / 1e6;
                    break;
                case "VFOB down":
                    VFOBFreq = vfoBFreq - Step2Freq(parm1);
                    break;
                case "VFOB up":
                    VFOBFreq = vfoBFreq + Step2Freq(parm1);
                    break;
                case "VFO Lock":
                    if (parm1 == 1)
                        chkVFOLock.Checked = true;
                    else
                        chkVFOLock.Checked = false;
                    break;
                case "VFO Swap":
                    CATVFOSwap(parm1.ToString());
                    break;
                case "VFO mode":
                    CATVFOMODE = parm1;
                    break;
                case "BandGrp":
                    CATBandGroup = parm1;
                    break;
                case "BIN":
                    CATBIN = parm1;
                    break;
                case "DisplayAVG":
                    CATDisplayAvg = parm1;
                    break;
                case "Display Mode":
                    //CurrentDisplayMode = (DisplayMode)parm1;
                    comboDisplayMode.Text = "Panafall";
                    break;
                case "Display Peak":
                    CATDispPeak = parm1.ToString();
                    break;
                case "Display Zoom":
                    CATDispZoom = parm1.ToString();
                    break;
                case "DX":
                    CATPhoneDX = parm1.ToString();
                    break;
                case "RX EQU":
                    EQForm.RXEQ = parm2;
                    break;
                case "RX EQU Enable":
                    if (parm1 == 1)
                        EQForm.RXEQEnabled = true;
                    else
                        EQForm.RXEQEnabled = false;
                    break;
                case "TX EQU":
                    EQForm.TXEQ = parm2;
                    break;
                case "TX EQU Enable":
                    if (parm1 == 1)
                        EQForm.TXEQEnabled = true;
                    else
                        EQForm.TXEQEnabled = false;
                    break;
                case "Power":
                    if (parm1 == 1)
                        chkPower.Checked = true;
                    else
                        chkPower.Checked = false;
                    break;
                case "Power toggle":
                    chkPower.Checked = !chkPower.Checked;
                    break;
                case "Memory Recall":
                    CATMemoryQR();
                    break;
                case "Memory Save":
                    CATMemoryQS();
                    break;
                case "RTTY OffsetHigh":
                    SetupForm.RttyOffsetHigh = parm1;
                    break;
                case "RTTY OffsetLow":
                    SetupForm.RttyOffsetLow = parm1;
                    break;
                case "SUB RX Enable":
                    if (parm1 == 1)
                        EnableSubRX = true;
                    else
                        EnableSubRX = false;
                    break;
                case "MOX":
                    if (parm1 == 1)
                        MOX = true;
                    else
                        MOX = false;
                    break;
                case "RXOnly":
                    if (parm1 == 1)
                        SetupForm.RXOnly = true;
                    else
                        SetupForm.RXOnly = false;
                    break;
                case "TUN Power":
                    SetupForm.TunePower = parm1;
                    break;
                case "TX Profile":
                    CATTXProfile = parm1;
                    break;
                case "TUN Enable":
                    if (parm1 == 1)
                        TUN = true;
                    else
                        TUN = false;
                    break;
                case "VAC":
                    if (parm1 == 1)
                        SetupForm.VACEnable = true;
                    else
                        SetupForm.VACEnable = false;
                    break;
                case "VAC RX gain":
                    VACRXGain = parm1;
                    break;
                case "VAC TX gain":
                    VACTXGain = parm1;
                    break;
                case "VAC SampleRate":
                    VACSampleRate = parm1.ToString();
                    break;
                case "CAT Serial Destroy":
                    Siolisten.SIO.run = false;
                    Siolisten.SIO.rx_event.Set();
                    Siolisten.SIO.Destroy();
                    break;
                case "Band set":
                    CurrentBand = (Band)parm1;
                    break;
                case "Band up":
                    int band = Math.Min((int)Band.LAST, (int)(current_band + 1));
                    CurrentBand = (Band)band;
                    break;
                case "Band down":
                    band = Math.Max((int)Band.FIRST, (int)(current_band - 1));
                    CurrentBand = (Band)band;
                    break;
                case "Meter RXMode":
                    CurrentMeterRXMode = (MeterRXMode)parm1;
                    break;
                case "Meter TXMode":
                    CurrentMeterTXMode = (MeterTXMode)parm1;
                    break;
                case "AF preamp":
                    /*if (parm1 == 1)
                        AF_button = true;
                    else
                        AF_button = false;*/
                    break;
                case "RF preamp":
                    CATRFPreampStatus = parm1;
                    break;
                case "ATT":
                    /*if (parm1 == 1)
                        ATT_button = true;
                    else
                        ATT_button = false;*/
                    break;
                case "Noise Gate":
                    if (parm1 == 1)
                        NoiseGateEnabled = true;
                    else
                        NoiseGateEnabled = false;
                    break;
                case "Noise Gate Level":
                    NoiseGate = parm1;
                    break;
                case "DSP Size":
                    SetupForm.RXDSPBufferSize = parm1;
                    break;
                case "MUT":
                    if (parm1 == 1)
                        MUT = true;
                    else
                        MUT = false;
                    break;
                case "MON":
                    if (parm1 == 1)
                        MON = true;
                    else
                        MON = false;
                    break;
                case "PAN Swap":
                    //CATPanSwap = parm1.ToString();
                    break;
                case "SUB Rx":
                    CATSubRX = parm1.ToString();
                    break;
                case "NB1":
                    CATNB1 = parm1;
                    break;
                case "NB1 Threshold":
                    SetupForm.CATNB1Threshold = parm1;
                    break;
                case "NB2":
                    CATNB2 = parm1;
                    break;
                case "NB2 Threshold":
                    SetupForm.CATNB2Threshold = parm1;
                    break;
                case "NR":
                    CATNR = parm1;
                    break;
                case "NR gain":
                    CATNRgain = parm1;
                    break;
                case "ANF":
                    CATANF = parm1;
                    break;
                case "ANF gain":
                    CATNOTCHgain = parm1;
                    break;
                case "NOTCH manual":
                    CATNOTCHManual = parm1;
                    break;
                case "NOTCH manual enable":
                    CATNOTCHenable = parm1;
                    break;
                case "RPT tone":
                    if (parm1 == 1)
                        CTCSS = true;
                    else
                        CTCSS = false;
                    break;
                case "SPLIT":
                    switch (parm1)
                    {
                        case 0:             // SPLIT disable
                            SplitAB_TX = false;
                            break;

                        case 1:             // SPLIT enable
                            SplitAB_TX = true;
                            break;

                        case 0x10:          // cancel DUPLEX
                            RPTRmode = RPTRmode.simplex;
                            break;

                        case 0x11:          // DUP-
                            RPTRmode = RPTRmode.low;
                            break;

                        case 0x12:          // DUP+
                            RPTRmode = RPTRmode.high;
                            break;
                    }
                    break;
                case "Memory up":
                    if (MemoryNumber < 99)
                        MemoryNumber++;
                    else if (MemoryNumber == 99)
                        MemoryNumber = 1;

                    txtMemory_fill();
                    break;
                case "Memory down":
                    if (MemoryNumber > 1)
                        MemoryNumber--;
                    else if (MemoryNumber == 1)
                        MemoryNumber = 99;

                    txtMemory_fill();
                    break;
                case "Memory recall":
                    btnMemoryQuickRestore_Click(this, EventArgs.Empty);
                    break;
                case "Memory store":
                    btnMemoryQuickSave_Click(this, EventArgs.Empty);
                    break;
                case "Memory clear":
                    btnEraseMemory_Click(this, EventArgs.Empty);
                    break;
                case "Restore VFOA":
                    btnVFOA_Click(this, EventArgs.Empty);
                    break;
                case "USB":
                    if (parm1 == 1)
                        btnUSB.BackColor = Color.Green;
                    else
                        btnUSB.BackColor = Color.Red;
                    break;
                case "CLOSE":
                    this.Close();
                    break;
            }
        }

        private double Step2Freq(int step)
        {
            double freq = 0.0;
            switch (step)
            {
                case 0:
                    freq = 0.000001;
                    break;
                case 1:
                    freq = 0.000010;
                    break;
                case 2:
                    freq = 0.000050;
                    break;
                case 3:
                    freq = 0.000100;
                    break;
                case 4:
                    freq = 0.000250;
                    break;
                case 5:
                    freq = 0.000500;
                    break;
                case 6:
                    freq = 0.001000;
                    break;
                case 7:
                    freq = 0.005000;
                    break;
                case 8:
                    freq = 0.009000;
                    break;
                case 9:
                    freq = 0.010000;
                    break;
                case 10:
                    freq = 0.100000;
                    break;
                case 11:
                    freq = 0.250000;
                    break;
                case 12:
                    freq = 0.500000;
                    break;
                case 13:
                    freq = 1.000000;
                    break;
                case 14:
                    freq = 10.000000;
                    break;
            }
            return freq;
        }

        #endregion
    }
}