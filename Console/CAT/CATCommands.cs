//=================================================================
// CATCommands.cs
//=================================================================
// Copyright (C) 2005  Bob Tracy
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
// You may contact the author via email at: k5kdn@arrl.net
//=================================================================

/*
 *  Changes for GenesisRadio
 *  Copyright (C)2010-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/


using System;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PowerSDR
{
    /// <summary>
    /// Summary description for CATCommands.
    /// </summary>
    public class CATCommands
    {
        #region Variable Definitions

        private Console console;
        private CATParser parser;
        private string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private Band[] BandList;
        private int LastBandIndex;
        private ASCIIEncoding AE = new ASCIIEncoding();
        private string lastFR = "0";
        private string lastFT = "0";
        public delegate void CATCrossThreadCallback(string type, int parm1, int[] parm2, string parm3);  // yt7pwr

        //		public static Mutex CATmut = new Mutex();

        #endregion Variable Definitions

        #region Constructors

        public CATCommands()
        {
        }

        public CATCommands(Console c, CATParser p)
        {
            console = c;
            parser = p;
            MakeBandList();
        }

        #endregion Constructors

        // Commands getting this far have been checked for a valid prefix, a correct suffix length,
        // and a terminator.  All we need to do in this class is to decide what kind of command
        // (read or set) and execute it.  Only read commands generate answers.

        #region Standard CAT Methods A-F

        // Sets or reads the Audio Gain control
        public string AG(string s)
        {
            if (s.Length == parser.nSet)	// if the length of the parameter legal for setting this prefix
            {
                int raw = Convert.ToInt32(s.Substring(1));
                int af = (int)Math.Round(raw / 2.55, 0);	// scale 255:100 (Kenwood vs SDR)
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                {
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AF", af, parm2, "");
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)	// if this is a read command
            {
                int af = (int)Math.Round(console.AF / 0.392, 0);
                //				return AddLeadingZeros(console.AF);		// Get the console setting
                return AddLeadingZeros(af);
            }
            else
            {
                return parser.Error1;	// return a ?
            }
        }

        public string AI(string s)
        {
            return ZZAI(s);
        }

        // Moves one band down from the currently selected band
        // write only
        public string BD()
        {
            return ZZBD();
        }

        // Moves one band up from the currently selected band
        // write only
        public string BU()
        {
            return ZZBU();
        }

        //Moves the VFO A frequency by the step size set on the console
        public string DN()
        {
            return ZZSA();
        }

        // Message 1
        public string F1(string s)
        {
            return ZZF1(s);
        }

        // Message 2
        public string F2(string s)
        {
            return ZZF2(s);
        }

        // Message 3
        public string F3(string s)
        {
            return ZZF3(s);
        }

        // Message 4
        public string F4(string s)
        {
            return ZZF4(s);
        }

        // Message 5
        public string F5(string s)
        {
            return ZZF5(s);
        }

        // Message 6
        public string F6(string s)
        {
            return ZZF6(s);
        }

        // Sets or reads the frequency of VFO A
        public string FA(string s)
        {
            return ZZFA(s);
        }

        // Sets or reads the frequency of VFO B
        public string FB(string s)
        {
            return ZZFB(s);
        }

        // Sets the frequency up of VFO A
        public string FD(string s)
        {
            return ZZFD(s);
        }

        // Sets the frequency up of VFO A
        public string FU(string s)
        {
            return ZZFU(s);
        }

        // Sets or reads the frequency of LOSC  yt7pwr
        public string FL(string s)
        {
            /*if (s.Length == parser.nSet)
            {
                s = s.Insert(5, separator);
                console.LOSCFreq = double.Parse(s);
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return StrVFOFreq("L");
            else
                return parser.Error1;*/

            return ZZFO(s);
        }

        // Sets VFO A to control rx
        // this is a dummy command to keep other software happy
        // since the SDR-1000 always uses VFO A for rx
        public string FR(string s)
        {
            if (s.Length == parser.nSet)
            {
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return "0";
            else
                return parser.Error1;
        }

        // Sets or reads VFO B to control tx
        // another "happiness" command
        public string FT(string s)
        {
            return ZZSP(s);
        }

        // Sets or reads the DSP filter width
        //OBSOLETE
        public string FW(string s)
        {
            if (s.Length == parser.nSet)
            {
                Filter new_filter = String2Filter(s);
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                {
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", (int)new_filter, parm2, "");
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return Filter2String(console.CurrentFilter);
            else
                return parser.Error1;
        }

        #endregion Standard CAT Methods A-F

        #region Standard CAT Methods G-M

        // Sets or reads the AGC constant
        // this is a wrapper that calls ZZGT
        public string GT(string s)
        {
            if (ZZGT(s).Length > 0)
                return ZZGT(s).PadLeft(3, '0');		//Added padleft fix 4/2/2007 BT
            else
                return "";
        }

        // Reads the transceiver ID number
        // this needs changing when 3rd party folks on line.
        public string ID()
        {
            string id;

            switch (console.CATRigType)
            {
                case 900:
                    id = "900";		//SDR-1000
                    break;
                case 13:
                    id = "013";		//TS-50S
                    break;
                case 19:
                    id = "019";		//TS-2000
                    break;
                case 20:
                    id = "020";		//TS-480
                    break;
                case 1:
                    id = "001";		//IC-7000
                    break;
                default:
                    id = "019";
                    break;
            }
            return (id);
        }

        // Reads the transceiver status
        // needs work in the split area
        public string IF()
        {
            string rtn = "";
            string rit = "0";
            string xit = "0";
            string incr = "";
            string tx = "0";
            string tempmode = "";
            int ITValue = 0;
            //string temp;

            // Get the rit/xit status
            if (console.RITOn)
                rit = "1";
            else if (console.XITOn)
                xit = "1";
            // Get the incremental tuning value for whichever control is selected
            if (rit == "1")
                ITValue = console.RITValue;
            else if (xit == "1")
                ITValue = console.XITValue;
            // Format the IT value
            if (ITValue < 0)
                incr = "-" + Convert.ToString(Math.Abs(ITValue)).PadLeft(5, '0');
            else
                incr = "+" + Convert.ToString(Math.Abs(ITValue)).PadLeft(5, '0');
            // Get the rx - tx status
            if (console.MOX)
                tx = "1";
            // Get the step size
            int step = console.StepSize;
            string stepsize = Step2String(step);
            // Get the vfo split status
            string split = "0";
            bool retval = console.VFOSplit;
            if (retval)
                split = "1";
            //Get the mode
            //			temp = Mode2KString(console.CurrentDSPMode);   //possible fix for SAM problem
            //			if(temp == parser.Error1)
            //				temp = " ";

            //string f = ZZFA("");
            string f = "";
            if (f.Length > 11)
            {
                f = f.Substring(f.Length - 11, 11);
            }
            rtn += f;
            rtn += StrVFOFreq("A");						// VFO A frequency			11 bytes
            rtn += stepsize;							// Console step frequency	 4 bytes
            rtn += incr;								// incremental tuning value	 6 bytes
            rtn += rit;									// RIT status				 1 byte
            rtn += xit;									// XIT status				 1 byte
            rtn += "000";								// dummy for memory bank	 3 bytes
            rtn += tx;									// tx-rx status				 1 byte
            //			rtn += temp;
            //			rtn += Mode2KString(console.CurrentDSPMode);	// current mode			 1 bytes
            tempmode = Mode2KString(console.CurrentDSPMode);
            if (tempmode == "?;")
                rtn += "2";
            else
                rtn += tempmode;
            rtn += "0";									// dummy for FR/FT			 1 byte
            rtn += "0";									// dummy for scan status	 1 byte
            rtn += split;								// VFO Split status			 1 byte
            rtn += "0000";								// dummy for the balance	 4 bytes
            return rtn;									// total bytes				35
        }

        //Sets or reads the CWX CW speed
        public string KS(string s)
        {
            return ZZKS(s);
        }

        //Sends text data to CWX for conversion to Morse
        public string KY(string s)
        {
            // Make sure we have an instance of the form
            if (console.CWXForm == null || console.CWXForm.IsDisposed)
            {
                try
                {
                    console.CWXForm = new CWX(console);
                }
                catch
                {
                    return parser.Error1;
                }
            }

            int[] parm2 = new int[1];
            // Make sure we are in a cw mode.
            switch (console.CurrentDSPMode)
            {
                case DSPMode.AM:
                case DSPMode.DRM:
                case DSPMode.DSB:
                case DSPMode.FMN:
                case DSPMode.SAM:
                case DSPMode.SPEC:
                case DSPMode.LSB:
                case DSPMode.USB:
                    if (console.CurrentBand >= Band.B160M && console.CurrentBand <= Band.B40M)
                    {
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOA", (int)DSPMode.CWL, parm2, "");
                    }
                    else
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOA", (int)DSPMode.CWU, parm2, "");
                    break;
                case DSPMode.CWL:
                case DSPMode.CWU:
                    break;
                default:
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOA", (int)DSPMode.CWU, parm2, "");
                    break;
            }

            if (s.Length == parser.nSet)
            {

                string trms = "";
                byte[] msg;
                string x = s.Trim();

                if (x.Length == 0)
                    trms = " ";
                else
                    trms = s.TrimEnd();

                if (trms.Length > 1)
                {
                    msg = AE.GetBytes(trms);

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Remote Msg", 0, msg, "");
                    return "";
                }
                else
                {
                    char ss = Convert.ToChar(trms);

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Remote Msg", 0, ss, "");

                    return "";
                }
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int ch = console.CWXForm.Characters2Send;

                if (ch < 72)
                    return "0";
                else
                    return "1";
            }
            else
                return parser.Error1;
        }


        // Sets or reads the transceiver mode
        public string MD(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (Convert.ToInt32(s) > 0 && Convert.ToInt32(s) <= 9)
                {
                    DSPMode new_mode = DSPMode.USB;

                    switch (s)
                    {
                        case "1":
                            new_mode = DSPMode.LSB;
                            break;
                        case "2":
                            new_mode = DSPMode.USB;
                            break;
                        case "3":
                            new_mode = DSPMode.CWU;
                            break;
                        case "4":
                            new_mode = DSPMode.FMN;
                            break;
                        case "5":
                            new_mode = DSPMode.AM;
                            break;
                        case "6":
                            new_mode = DSPMode.DIGL;
                            break;
                        case "7":
                            new_mode = DSPMode.CWL;
                            break;
                        case "9":
                            new_mode = DSPMode.DIGU;
                            break;
                        default:
                            new_mode = DSPMode.USB;
                            break;
                    }

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOA", (int)new_mode, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return Mode2KString(console.CurrentDSPMode);

            }
            else
                return parser.Error1;
        }

        // Sets or reads the Mic Gain thumbwheel
        public string MG(string s)
        {
            int n;
            if (s.Length == parser.nSet)
            {
                n = Convert.ToInt32(s);
                n = Math.Max(0, n);
                n = Math.Min(100, n);
                int mg = (int)Math.Round(n / 1.43, 0);	// scale 100:70 (Kenwood vs SDR)
                s = AddLeadingZeros(mg);
                return ZZMG(s);
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                s = ZZMG("");
                n = Convert.ToInt32(s);
                int mg = (int)Math.Round(n / .7, 0);
                s = AddLeadingZeros(mg);
                return s;
            }
            else
                return parser.Error1;
        }

        // Sets or reads the Monitor status
        public string MO(string s)
        {
            return ZZMO(s);
        }

        #endregion Standard CAT Methods G-M

        #region Standard CAT Methods N-Q

        // Sets or reads the Noise Blanker 1 status
        public string NB(string s)
        {
            return ZZNA(s);
        }

        // Sets or reads the Automatic Notch Filter status
        public string NT(string s)
        {
            return ZZNT(s);
        }

        // Sets or reads the RF preamp state
        public string PA(string s)
        {
            return ZZPA(s);
        }

        // Sets or reads the PA output thumbwheel
        public string PC(string s)
        {
            return ZZPC(s);
        }

        // Sets or reads the Speech Compressor status
        public string PR(string s)
        {
            return ZZPK(s);
        }

        // Sets or reads the console power on/off status
        public string PS(string s)
        {
            return ZZPS(s);
        }

        // Sets or reads the PITCH
        public string PT(string s)
        {
            return ZZCL(s);
        }

        // Sets the Quick Memory with the current contents of VFO A
        public string QI()
        {
            return ZZQS();
        }

        #endregion Standard CAT Methods N-Q

        #region Standard CAT Methods R-Z

        public string RA(string s)
        {
            return ZZRA(s);
        }

        // Clears the RIT value
        // write only
        public string RC()
        {
            return ZZRC();
        }

        //Decrements RIT
        public string RD(string s)
        {
            return ZZRD(s);
        }

        // Read/Set RIT value
        public string RI(string s)
        {
            return ZZRF(s);
        }

        // RF gain level
        public string RG(string s)
        {
            return ZZPA(s);
        }

        // Sets or reads the RIT status (on/off)
        public string RT(string s)
        {
            return ZZRT(s);
        }

        //Increments RIT
        public string RU(string s)
        {
            return ZZRU(s);
        }

        // Sets or reads the transceiver receive mode status
        // write only but spec shows an answer parameter for a read???
        public string RX(string s)
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 0, parm2, "");

            return "";
        }

        // Sets or reads the variable DSP filter high side
        public string SH(string s)
        {
            if (s.Length == parser.nSet)
            {
                SetFilter(s, "SH");
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.AM:
                    case DSPMode.CWU:
                    case DSPMode.DRM:
                    case DSPMode.DSB:
                    case DSPMode.FMN:
                    case DSPMode.SAM:
                    case DSPMode.USB:
                    case DSPMode.DIGU:
                        return Frequency2Code(console.FilterHighValue, "SH");
                    case DSPMode.CWL:
                    case DSPMode.LSB:
                    case DSPMode.DIGL:
                        return Frequency2Code(console.FilterLowValue, "SH");
                    default:
                        return Frequency2Code(console.FilterHighValue, "SH");
                }
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the variable DSP filter low side
        public string SL(string s)
        {
            if (s.Length == parser.nSet)
            {
                SetFilter(s, "SL");
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.AM:
                    case DSPMode.CWU:
                    case DSPMode.DRM:
                    case DSPMode.DSB:
                    case DSPMode.FMN:
                    case DSPMode.SAM:
                    case DSPMode.USB:
                    case DSPMode.DIGU:
                        return Frequency2Code(console.FilterLowValue, "SL");
                    case DSPMode.CWL:
                    case DSPMode.LSB:
                    case DSPMode.DIGL:
                        return Frequency2Code(console.FilterHighValue, "SL");
                    default:
                        return Frequency2Code(console.FilterLowValue, "SL");
                }
            }
            else
            {
                return parser.Error1;
            }
        }

        // Reads the S Meter value
        public string SM(string s)
        {
            int sm = 0;
            double sx = 0.0;

            if (s == "0" || s == "2")	// read the main transceiver s meter
            {
                float num = 0f;

                if (console.PowerOn)
                    num = DttSP.CalculateRXMeter(0, 0, DttSP.MeterType.SIGNAL_STRENGTH);

                num = num + console.MultimeterCalOffset;
                num = Math.Max(-140, num);
                num = Math.Min(-10, num);
                sx = (num + 127) / 6;

                if (sx < 0) sx = 0;

                if (sx <= 9.0F)
                {
                    sm = Math.Abs((int)(sx * 1.6667));
                }
                else
                {
                    double over_s9 = num + 73;
                    sm = 15 + (int)over_s9;
                }

                if (sm < 0) sm = 0;
                if (sm > 30) sm = 30;

                return sm.ToString().PadLeft(5, '0');
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the Squelch value
        public string SQ(string s)
        {
            string rx = s.Substring(0, 1);
            double level = 0.0;

            //Will need code to select receiver when n Receivers enabled.
            //for now, ignore rx number.

            if (s.Length == parser.nSet)
            //convert to a double and add the scale factor (160 = 255)
            {
                int[] parm2 = new int[1];
                level = Convert.ToDouble(s.Substring(1));
                level = Math.Max(0, level);			// lower bound
                level = Math.Min(255, level);		// upper bound
                level = level * 0.62745;				// scale factor

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOA", (int)Math.Round(level, 0), parm2, "");

                //console.SquelchMainRX = Convert.ToInt32(Math.Round(level,0));
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return rx + AddLeadingZeros(console.SquelchMainRX).Substring(1);
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets the transmitter on, write only
        // will eventually need eiter Commander change or ZZ code
        // since it is not CAT compliant as it is
        public string TX(string s)
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 1, parm2, "");

            return "";  // ZZTX("1");
        }

        //Moves the VFO A frequency up by the step size set on the console
        public string UP()
        {
            return ZZSB();
        }

        // Sets or reads the transceiver XIT status (on/off)
        public string XT(string s)
        {
            return ZZXS(s);
        }

        #endregion Standard CAT Methods R-Z

        #region Extended CAT Methods ZZA-ZZF


        //Sets or reads the console step size VFOB(also see zzst(read only)
        public string ZZAB(string s)
        {
            int step = 0;
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                step = Convert.ToInt32(s);
                if (step >= 0 || step <= 14)
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "StepSize VFOB", step, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                step = console.StepSizeSubRX;
                return AddLeadingZeros(step);
            }
            else
                return parser.Error1;
        }

        //Sets or reads the console step size VFOA(also see zzst(read only)
        public string ZZAC(string s)
        {
            int step = 0;
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                step = Convert.ToInt32(s);
                if (step >= 0 || step <= 14)
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "StepSize VFOA", step, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                step = console.StepSize;
                string answer = step.ToString().PadLeft(2, '0');
                return answer;
            }
            else
                return parser.Error1;
        }

        //Sets VFO A down nn Tune Steps
        public string ZZAD(string s)
        {
            int step = 0;
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                step = Convert.ToInt32(s);
                if (step >= 0 || step <= 14)
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOA down", step, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        // Sets or reads the SDR-1000 Audio Gain control
        public string ZZAG(string s)
        {
            int af = 0;

            if (s.Length == parser.nSet)	// if the length of the parameter legal for setting this prefix
            {
                int[] parm2 = new int[1];
                af = Convert.ToInt32(s);
                af = Math.Max(0, af);
                af = Math.Min(100, af);

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AF", af, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)	// if this is a read command
            {
                string answer = console.AF.ToString().PadLeft(3, '0');
                return answer;
            }
            else
            {
                return parser.Error1;	// return a ?
            }

        }

        public string ZZAI(string s)
        {
            if (console.SetupForm.AllowFreqBroadcast)
            {
                if (s.Length == parser.nSet)
                {
                    int[] parm2 = new int[1];

                    if (s == "0")
                    {
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "FreqBroadcast", 0, parm2, "");
                    }
                    else
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "FreqBroadcast", 1, parm2, "");

                    return "";
                }
                else if (s.Length == parser.nGet || s.Length == 0)
                {
                    if (console.KWAutoInformation)
                        return "1";
                    else
                        return "0";
                }
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the AGC RF gain
        public string ZZAR(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-20, n);
                n = Math.Min(120, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.RF;
                // we have to remove the leading zero and replace it with the sign.
                return AddLeadingZeros(x);
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets VFO A up nn Tune Steps
        public string ZZAU(string s)
        {
            int step = 0;
            if (s.Length == parser.nSet)
            {
                step = Convert.ToInt32(s);
                if (step >= 0 || step <= 14)
                {
                    int[] parm2 = new int[1];

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOA up", step, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        //Moves the bandswitch down one band
        public string ZZBD()
        {
            BandDown();
            return "";
        }

        // Sets the Band Group (HF/VHF)
        public string ZZBG(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];
                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "BandGrp", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "BandGrp", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATBandGroup.ToString();
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the BIN button status
        public string ZZBI(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "BIN", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATBIN.ToString();
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets VFO B down nn Tune Steps
        public string ZZBM(string s)
        {
            int step = 0;
            if (s.Length == parser.nSet)
            {
                step = Convert.ToInt32(s);
                if (step >= 0 || step <= 14)
                {
                    int[] parm2 = new int[1];

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOB down", step, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        //Sets VFO B up nn Tune Steps
        public string ZZBP(string s)
        {
            int step = 0;
            if (s.Length == parser.nSet)
            {
                step = Convert.ToInt32(s);
                if (step >= 0 || step <= 14)
                {
                    int[] parm2 = new int[1];

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOB up", step, parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the current band setting
        public string ZZBS(string s)
        {
            return GetBand(s);
        }

        //Moves the bandswitch up one band
        public string ZZBU()
        {
            BandUp();
            return "";
        }

        //Shuts down the console
        public string ZZBY()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CLOSE", 0, parm2, "");

            return "";
        }

        // Sets or reads the CW Break In Enabled checkbox
        public string ZZCB(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW BreakIn", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW BreakIn", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.BreakInEnabled)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the CW Break In Delay
        public string ZZCD(string s)
        {
            int n = 0;

            if (s != null && s != "")
                n = Convert.ToInt32(s);
            n = Math.Max(150, n);
            n = Math.Min(5000, n);
            int[] parm2 = new int[1];

            if (s.Length == parser.nSet)
            {
                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "BreakIn Delay", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros((int)console.SetupForm.BreakInDelay);
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the Show CW Frequency checkbox
        public string ZZCF(string s)
        {
            switch (console.CurrentDSPMode)
            {
                case DSPMode.CWL:
                case DSPMode.CWU:
                    if (s.Length == parser.nSet && (s == "0" || s == "1"))
                    {
                        int[] parm2 = new int[1];

                        if (s == "1")
                        {
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Show CW TXfreq", 1, parm2, "");
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Show CW TXfreq", 0, parm2, "");

                        return "";
                    }
                    else if (s.Length == parser.nGet || s.Length == 0)
                    {
                        if (console.ShowCWTXFreq)
                            return "1";
                        else
                            return "0";
                    }
                    else
                    {
                        return parser.Error1;
                    }
                default:
                    return parser.Error1;
            }
        }

        // Sets or reads the CW Iambic checkbox
        public string ZZCI(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW Iambic", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW Iambic", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.CWIambic)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the CW Pitch thumbwheel
        public string ZZCL(string s)
        {
            int n = 0;
            if (s != "")
                n = Convert.ToInt32(s);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW Pitch", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.SetupForm.CATCWPitch);
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the CW Monitor Disable button status  yt7pwr
        public string ZZCM(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW Monitor", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW Monitor", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.CATCWMonitor == "1")
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the compander button status
        public string ZZCP(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CMPD", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CMPD", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATCmpd.ToString();
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the CW Speed thumbwheel
        public string ZZCS(string s)
        {
            int n = 1;

            if (s != "")
                n = Convert.ToInt32(s);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CW Speed", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.CATCWSpeed);
            }
            else
            {
                return parser.Error1;
            }
        }

        //Reads or sets the compander threshold
        public string ZZCT(string s)
        {
            int n = 0;

            if (s != null && s != "")
                n = Convert.ToInt32(s);
            n = Math.Max(0, n);
            n = Math.Min(10, n);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CPDR", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros((int)console.CPDRVal);
            }
            else
            {
                return parser.Error1;
            }

        }

        // Reads the CPU Usage
        public string ZZCU()
        {
            string answer = console.CpuUsage.ToString("f").PadLeft(6, '0');
            return answer;
        }

        // Sets or reads the Display Average status
        public string ZZDA(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DisplayAVG", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATDisplayAvg.ToString();
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the current display mode
        public string ZZDM(string s) // changes yt7pwr
        {
            int n = -1;

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                n = Convert.ToInt32(s);

                if (n > (int)DisplayMode.FIRST && n < (int)DisplayMode.LAST)
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Display Mode", n, parm2, "");
                }
                else
                    return parser.Error1;

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string answer = ((int)console.CurrentDisplayMode).ToString();
                return answer;
            }
            else
            {
                return parser.Error1;
            }

        }

        /// <summary>
        /// Reads DDUtil command word
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZDU(string s)                // yt7pwr
        {
            if (s.Length == parser.nSet)
            {
                return "Set not allowed!";
            }
            else if (s.Length == parser.nGet)
            {
                string answer = "ZZDU";
                answer += ZZSW(s) + ":" + ZZSP(s) + ":" + ZZTU(s) + ":" + ZZTX(s) + ":" + "0:" + "0:" +
                    "0:" + "0:" + ZZRT(s) + ":" + ZZDM(s) + ":" + ZZGT(s) + ":" + ZZMU(s) + ":" + ZZXS(s) +
                    ":" + ZZAC(s) + ":" + ZZMD(s) + ":" + ZZME(s) + ":" + ZZFJ(s) + ":" +
                    ZZFI(s) + ":" + "000:" + ZZBS(s) + ":" + ZZPC(s) + ":" + ZZBS(s) + ":" + ZZAG(s) + ":" +
                    ZZKS(s) + ":" + ZZTO(s) + ":" + "0012:" + ZZSM(s) + ":" + ZZRF(s) + ":" + ZZTS(s) + ":" +
                    ZZXF(s) + ":" + ZZCU() + ":" + ZZFA(s) + ":" + ZZFB(s) + ";";

                return answer;
            }
            else
                return parser.Error1;
        }

        /// <summary>
        /// Sets or reads the DX button status
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZDX(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DX", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATPhoneDX;
            }
            else
                return parser.Error1;
        }

        /// <summary>
        /// Reads or sets the RX equalizer.
        /// The CAT suffix string is 36 characters constant.
        /// Each value in the string occupies exactly three characters
        /// starting with the number of bands (003 or 010) followed by
        /// the preamp setting (-12 to 015) followed by 3 or 10 three digit
        /// EQ thumbwheel positions.  If the number of bands is 3, the
        /// last seven positions (21 characters) are all set to zero.
        /// Example:  10 band ZZEA010-09009005000-04-07-09-05000005009;
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZEA(string s)
        {
            if (s.Length == parser.nSet)
            {
                int nb = Int32.Parse(s.Substring(0, 3));			//Get the number of bands
                int[] ans = new int[nb + 1];						//Create the integer array
                s = s.Remove(0, 3);								//Get rid of the band count

                for (int x = 0; x <= nb; x++)						//Parse the string into the array
                {
                    ans[x] = Int32.Parse(s.Substring(0, 3));
                    s = s.Remove(0, 3);							//Remove the last three used
                }

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RX EQU", 0, ans);

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int[] eqarray = console.EQForm.RXEQ;			//Get the equalizer array
                int nb = 10;                    				//Get the number of bands in the array
                int val;										//Holds a temporary value
                string ans = nb.ToString().PadLeft(3, '0');		//The return string with the number of bands added

                for (int x = 0; x <= nb; x++)					//Loop thru the array
                {
                    if (eqarray[x] < 0)
                    {
                        val = Math.Abs(eqarray[x]);					//If the value is negative, format the answer
                        ans += "-" + val.ToString().PadLeft(2, '0');
                    }
                    else
                        ans += eqarray[x].ToString().PadLeft(3, '0');
                }
                ans = ans.PadRight(36, '0');							//Add the padding if it's a 3 band eq
                return ans;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the TX EQ settings
        public string ZZEB(string s)
        {
            if (s.Length == parser.nSet)
            {
                int nb = Int32.Parse(s.Substring(0, 3));			//Get the number of bands
                int[] ans = new int[nb + 1];						//Create the integer array
                s = s.Remove(0, 3);								//Get rid of the band count

                for (int x = 0; x <= nb; x++)						//Parse the string into the array
                {
                    ans[x] = Int32.Parse(s.Substring(0, 3));
                    s = s.Remove(0, 3);							//Remove the last three used
                }

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TX EQU", 0, ans);

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int[] eqarray = console.EQForm.TXEQ;			//Get the equalizer array
                int nb = 10;                    				//Get the number of bands in the array
                int val;										//Holds a temporary value
                string ans = nb.ToString().PadLeft(3, '0');		//The return string with the number of bands added

                for (int x = 0; x <= nb; x++)					//Loop thru the array
                {
                    if (eqarray[x] < 0)
                    {
                        val = Math.Abs(eqarray[x]);					//If the value is negative, format the answer
                        ans += "-" + val.ToString().PadLeft(2, '0');
                    }
                    else
                        ans += eqarray[x].ToString().PadLeft(3, '0');
                }
                ans = ans.PadRight(36, '0');							//Add the padding if it's a 3 band eq
                return ans;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the RXEQ button statusl
        public string ZZER(string s)
        {
            if (s.Length == parser.nSet && (s == "1" || s == "0"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RX EQU Enable", 1, parm2, "");
                }
                else if (s == "0")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RX EQU Enable", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATRXEQ;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the TXEQ button status
        public string ZZET(string s)
        {
            if (s.Length == parser.nSet && (s == "1" || s == "0"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TX EQU Enable", 1, parm2, "");
                }
                else if (s == "0")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TX EQU Enable", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATTXEQ;
            }
            else
                return parser.Error1;
        }

        #region voice/cwx messages

        //Starts or stop message 1
        public string ZZF1(string s)
        {
            if (s.Length == parser.nSet)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        console.btnCWX1.Checked = true;
                        console.btnCWX1_Click(this, EventArgs.Empty);
                        break;

                    case DSPMode.FMN:
                        console.chkFMMsg1.Checked = !console.chkFMMsg1.Checked;
                        break;
                    default:
                        console.btnMsg1.Checked = true;
                        console.btnMsg1_Click(this, EventArgs.Empty);
                        break;
                }

                return "ZZF11;";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string result = "false";

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (console.btnCWX1.Checked)
                            result = "true";
                        break;

                    case DSPMode.FMN:
                        if (console.chkFMMsg1.Checked)
                            result = "true";
                        break;
                    default:
                        if (console.btnMsg1.Checked)
                            result = "true";
                        break;
                }

                return result;
            }
            else
                return parser.Error1;
        }

        //Starts or stop message 2
        public string ZZF2(string s)
        {
            if (s.Length == parser.nSet)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        console.btnCWX2.Checked = true;
                        console.btnCWX2_Click(this, EventArgs.Empty);
                        break;

                    case DSPMode.FMN:
                        console.chkFMMsg2.Checked = !console.chkFMMsg2.Checked;
                        break;
                    default:
                        console.btnMsg2.Checked = true;
                        console.btnMsg2_Click(this, EventArgs.Empty);
                        break;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string result = "false";

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (console.btnCWX2.Checked)
                            result = "true";
                        break;

                    case DSPMode.FMN:
                        if (console.chkFMMsg2.Checked)
                            result = "true";
                        break;
                    default:
                        if (console.btnMsg2.Checked)
                            result = "true";
                        break;
                }

                return result;
            }
            else
                return parser.Error1;
        }

        //Starts or stop message 3
        public string ZZF3(string s)
        {
            if (s.Length == parser.nSet)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        console.btnCWX3.Checked = true;
                        console.btnCWX3_Click(this, EventArgs.Empty);
                        break;

                    case DSPMode.FMN:
                        console.chkFMMsg3.Checked = !console.chkFMMsg3.Checked;
                        break;
                    default:
                        console.btnMsg3.Checked = true;
                        console.btnMsg3_Click(this, EventArgs.Empty);
                        break;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string result = "false";

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (console.btnCWX3.Checked)
                            result = "true";
                        break;

                    case DSPMode.FMN:
                        if (console.chkFMMsg3.Checked)
                            result = "true";
                        break;
                    default:
                        if (console.btnMsg3.Checked)
                            result = "true";
                        break;
                }

                return result;
            }
            else
                return parser.Error1;
        }

        //Starts or stop message 4
        public string ZZF4(string s)
        {
            if (s.Length == parser.nSet)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        console.btnCWX4.Checked = true;
                        console.btnCWX4_Click(this, EventArgs.Empty);
                        break;

                    case DSPMode.FMN:
                        console.chkFMMsg4.Checked = console.chkFMMsg4.Checked;
                        break;
                    default:
                        console.btnMsg4.Checked = true;
                        console.btnMsg4_Click(this, EventArgs.Empty);
                        break;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string result = "false";

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (console.btnCWX4.Checked)
                            result = "true";
                        break;

                    case DSPMode.FMN:
                        if (console.chkFMMsg4.Checked)
                            result = "true";
                        break;
                    default:
                        if (console.btnMsg4.Checked)
                            result = "true";
                        break;
                }

                return result;
            }
            else
                return parser.Error1;
        }
        //Starts or stop message 5
        public string ZZF5(string s)
        {
            if (s.Length == parser.nSet)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        console.btnCWX5.Checked = true;
                        console.btnCWX5_Click(this, EventArgs.Empty);
                        break;

                    case DSPMode.FMN:
                        console.chkFMMsg5.Checked = console.chkFMMsg5.Checked;
                        break;
                    default:
                        console.btnMsg5.Checked = true;
                        console.btnMsg5_Click(this, EventArgs.Empty);
                        break;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string result = "false";

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (console.btnCWX5.Checked)
                            result = "true";
                        break;

                    case DSPMode.FMN:
                        if (console.chkFMMsg5.Checked)
                            result = "true";
                        break;
                    default:
                        if (console.btnMsg5.Checked)
                            result = "true";
                        break;
                }

                return result;
            }
            else
                return parser.Error1;
        }

        //Starts or stop message 6
        public string ZZF6(string s)
        {
            if (s.Length == parser.nSet)
            {
                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        console.btnCWX6.Checked = true;
                        console.btnCWX6_Click(this, EventArgs.Empty);
                        break;

                    case DSPMode.FMN:
                        console.chkFMMsg6.Checked = console.chkFMMsg6.Checked;
                        break;
                    default:
                        console.btnMsg6.Checked = true;
                        console.btnMsg6_Click(this, EventArgs.Empty);
                        break;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string result = "false";

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (console.btnCWX6.Checked)
                            result = "true";
                        break;

                    case DSPMode.FMN:
                        if (console.chkFMMsg6.Checked)
                            result = "true";
                        break;
                    default:
                        if (console.btnMsg6.Checked)
                            result = "true";
                        break;
                }

                return result;
            }
            else
                return parser.Error1;
        }


        #endregion

        //Sets or reads VFO A frequency
        public string ZZFA(string s)
        {
            if (s.Length == parser.nSet)
            {
                s = s.Insert(5, separator);		//reinsert the global numeric separator
                double vfoA = double.Parse(s);

                if (vfoA > (console.MaxFreq) || vfoA < console.MinFreq)
                {
                    if (console.BandByFreq(vfoA) != console.CurrentBand)
                        console.SaveBand();

                    console.cat_vfoa = true;
                    console.VFOAFreq = double.Parse(s);
                    console.cat_vfoa = false;

                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(s) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }
                    else
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(s) - 0.015;                    // yt7pwr
                        console.cat_losc = false;
                    }

                    console.cat_vfoa = true;
                    console.VFOAFreq = double.Parse(s);

                    if (console.VFOAFreq != double.Parse(s))
                        console.VFOAFreq = double.Parse(s);

                    console.cat_vfoa = false;
                }
                else
                {
                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(s) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }

                    console.cat_vfoa = true;
                    console.VFOAFreq = double.Parse(s);

                    if (console.VFOAFreq != double.Parse(s))
                        console.VFOAFreq = double.Parse(s);

                    console.cat_vfoa = false;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return StrVFOFreq("A");
            else
                return parser.Error1;
        }

        //Sets or reads VFO A down frequency
        public string ZZFD(string s)
        {
            if (s.Length == parser.nSet)
            {
                s = s.Insert(5, separator);		//reinsert the global numeric separator
                double vfoA = console.VFOAFreq - double.Parse(s);

                if (vfoA > (console.MaxFreq) || vfoA < console.MinFreq)
                {
                    if (console.BandByFreq(vfoA) != console.CurrentBand)
                        console.SaveBand();

                    console.cat_vfoa = true;
                    console.VFOAFreq = Math.Round(console.VFOAFreq - double.Parse(s), 6);
                    console.cat_vfoa = false;

                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(s) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }
                    else
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = Math.Round(console.VFOAFreq - 0.015, 2);   // yt7pwr
                        console.cat_losc = false;
                    }
                }
                else
                {
                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = console.VFOAFreq - double.Parse(s) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }

                    console.cat_vfoa = true;
                    console.VFOAFreq = Math.Round(console.VFOAFreq - double.Parse(s), 6);
                    console.cat_vfoa = false;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return StrVFOFreq("A");
            else
                return parser.Error1;
        }

        //Sets or reads VFO A up frequency
        public string ZZFU(string s)
        {
            if (s.Length == parser.nSet)
            {
                s = s.Insert(5, separator);		//reinsert the global numeric separator
                double vfoA = console.VFOAFreq + double.Parse(s);

                if (vfoA > (console.MaxFreq) || vfoA < console.MinFreq)
                {
                    if (console.BandByFreq(vfoA) != console.CurrentBand)
                        console.SaveBand();

                    console.cat_vfoa = true;
                    console.VFOAFreq = Math.Round(console.VFOAFreq + double.Parse(s), 6);
                    console.cat_vfoa = false;

                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(s) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }
                    else
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = Math.Round(console.VFOAFreq - 0.015, 2);   // yt7pwr
                        console.cat_losc = false;
                    }
                }
                else
                {
                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = console.VFOAFreq + double.Parse(s) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }

                    console.cat_vfoa = true;
                    console.VFOAFreq = Math.Round(console.VFOAFreq + double.Parse(s), 6);
                    console.cat_vfoa = false;
                }

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return StrVFOFreq("A");
            else
                return parser.Error1;
        }

        //Sets or reads VFO B frequency
        public string ZZFB(string s)
        {
            if (s.Length == parser.nSet)
            {
                s = s.Insert(5, separator);		//reinsert the global numeric separator
                double vfoB = double.Parse(s);
                console.cat_vfob = true;

                if (vfoB > (console.MaxFreq))
                {
                    console.VFOBFreq = console.MaxFreq;
                }
                else if (vfoB < console.MinFreq)
                {
                    console.VFOBFreq = console.MinFreq;
                }
                else
                {
                    console.VFOBFreq = double.Parse(s);
                }

                console.cat_vfob = false;
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
                return StrVFOFreq("B");
            else
                return parser.Error1;
        }

        //Sets or reads LOSC frequency
        public string ZZFO(string s)        // yt7pwr
        {
            if (s.Length == parser.nSet)
            {
                int f = int.Parse(s);
                s = AddLeadingZeros(f);
                s = s.Insert(5, separator);

                console.cat_losc = true;
                console.LOSCFreq = double.Parse(s);
                console.cat_losc = false;
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int f = Convert.ToInt32(Math.Round(console.CATLOSC, 6) * 1e6);
                return AddLeadingZeros(f);
            }
            else
                return parser.Error1;

        }

        //Sets or reads the MainRX filter index number
        public string ZZFI(string s)
        {
            int n = 0;

            if (s != "")
                n = Convert.ToInt32(s);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (n < (int)Filter.LAST)
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", n, parm2, "");
                }
                else
                    return parser.Error1;

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string answer = ((int)console.CurrentFilter).ToString().PadLeft(2, '0');
                return answer;
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the SubRX filter index number
        public string ZZFJ(string s)
        {
            int n = 0;

            if (s != "")
                n = Convert.ToInt32(s);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (n < (int)Filter.LAST)
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter VFOB", n, parm2, "");
                }
                else
                    return parser.Error1;

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string answer = ((int)console.CurrentFilterSubRX).ToString().PadLeft(2, '0');
                return answer;
            }
            else
            {
                return parser.Error1;
            }
        }

        /// <summary>
        /// Reads or sets the DSP Filter Low value
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZFL(string s)
        {
            string sign;
            int n;

            if (s.Length == parser.nSet)
            {
                n = Convert.ToInt32(s);
                n = Math.Min(20000, n);
                n = Math.Max(-20000, n);
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Low", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                n = console.FilterLowValue;
                if (n < 0)
                    sign = "-";
                else
                    sign = "+";

                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(n)).Substring(1);
                //				return AddLeadingZeros((int) console.FilterLowValue);
            }
            else
                return parser.Error1;
        }

        /// <summary>
        /// Reads or sets the DSP Filter High value
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZFH(string s)
        {
            string sign;
            int n;

            if (s.Length == parser.nSet)
            {
                n = Convert.ToInt32(s);
                n = Math.Min(20000, n);
                n = Math.Max(-20000, n);
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter High", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                n = console.FilterHighValue;
                if (n < 0)
                    sign = "-";
                else
                    sign = "+";

                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(n)).Substring(1);
            }
            else
                return parser.Error1;
        }

        public string ZZFM()
        {
            return "0";

            /*string radio = console.CurrentModel.ToString();

            if (radio == "GENESIS_G59USB")
                return "0";
            else if (radio == "GENESIS_G3020")
                return "1";
            else if (radio == "GENESIS_G40")
                return "2";
            else if (radio == "GENESIS_G80")
                return "3";
            else if (radio == "GENESIS_G160")
                return "4";
            else if (radio == "GENESIS_G59NET")
                return "5";
            else if (radio == "GENESIS_G6")
                return "6";
            else if (radio == "GENESIS_G11")
                return "7";
            else
                return parser.Error1;*/
        }

        #endregion Extended CAT Methods ZZA-ZZF

        #region Extended CAT Methods ZZG-ZZM

        // Sets or reads the G59 AF button status
        public string ZZGA(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AF preamp", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AF preamp", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the RF button status
        public string ZZGR(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF preamp", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF preamp", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the G59 ATT button status
        public string ZZGN(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ATT", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ATT", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the noise gate enable button status
        public string ZZGE(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Noise Gate", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Noise Gate", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.NoiseGateEnabled)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the noise gate level control
        public string ZZGL(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-160, n);
                n = Math.Min(0, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Noise Gate Level", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.NoiseGate;
                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(x)).Substring(1);
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the AGC constant
        public string ZZGT(string s)
        {
			if(s.Length == parser.nSet)
			{
                int[] parm2 = new int[1];

                if ((Convert.ToInt32(s) > (int)AGCMode.FIRST && Convert.ToInt32(s) < (int)AGCMode.LAST))
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AGC mode",
                        Convert.ToInt32(s), parm2, "");
                }
                else
                    return parser.Error1;

				return "";
			}
			else if(s.Length == parser.nGet)
			{
				return ((int) console.CurrentAGCMode).ToString();
			}
			else
			{
				return parser.Error1;
			}
        }

        // Sets or reads the Audio Buffer Size
        public string ZZHA(string s)
        {

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Size", Index2Width(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return Width2Index(console.SetupForm.RXDSPBufferSize);
            }
            else
                return parser.Error1;
        }

        //Sets or reads the DSP Phone RX Buffer Size
        public string ZZHR(string s)
        {
            /*			if(s.Length == parser.nSet)
                        {
                            int width = Index2Width(s);
                            console.DSPBufPhoneRX = width;
                            console.SetupForm.DSPPhoneRXBuffer = width;
                            return "";
                        }
                        else if(s.Length == parser.nGet)
                        {
                            return Width2Index(console.DSPBufPhoneRX);
                        }
                        else
                            return parser.Error1;*/

            return parser.Error1;
        }

        //Sets or reads the DSP Phone TX Buffer Size
        public string ZZHT(string s)
        {
            /*			if(s.Length == parser.nSet)
                        {
                            int width = Index2Width(s);
                            console.DSPBufPhoneTX = width;
                            console.SetupForm.DSPPhoneTXBuffer = width;
                            return "";
                        }
                        else if(s.Length == parser.nGet)
                        {
                            return Width2Index(console.DSPBufPhoneTX);
                        }
                        else
                            return parser.Error1;*/

            return parser.Error1;
        }

        //Sets or reads the DSP CW RX Buffer Size
        public string ZZHU(string s)
        {
            /*			if(s.Length == parser.nSet)
                        {
                            int width = Index2Width(s);
                            console.DSPBufCWRX = width;
                            console.SetupForm.DSPCWRXBuffer = width;
                            return "";
                        }
                        else if(s.Length == parser.nGet)
                        {
                            return Width2Index(console.DSPBufCWRX);
                        }
                        else
                            return parser.Error1;*/

            return parser.Error1;
        }

        //Sets or reads the DSP CW TX Buffer Size
        public string ZZHV(string s)
        {
            /*			if(s.Length == parser.nSet)
                        {
                            int width = Index2Width(s);
                            console.DSPBufCWTX = width;
                            console.SetupForm.DSPCWTXBuffer = width;
                            return "";
                        }
                        else if(s.Length == parser.nGet)
                        {
                            return Width2Index(console.DSPBufCWTX);
                        }
                        else
                            return parser.Error1;*/

            return parser.Error1;
        }

        //Sets or reads the DSP Digital RX Buffer Size
        public string ZZHW(string s)
        {
            /*			if(s.Length == parser.nSet)
                        {
                            int width = Index2Width(s);
                            console.DSPBufDigRX = width;
                            console.SetupForm.DSPDigRXBuffer = width;
                            return "";
                        }
                        else if(s.Length == parser.nGet)
                        {
                            return Width2Index(console.DSPBufDigRX);
                        }
                        else
                            return parser.Error1;*/

            return parser.Error1;
        }

        //Sets or reads the DSP Digital TX Buffer Size
        public string ZZHX(string s)
        {
            /*			if(s.Length == parser.nSet)
                        {
                            int width = Index2Width(s);
                            console.DSPBufDigTX = width;
                            console.SetupForm.DSPDigTXBuffer = width;
                            return "";
                        }
                        else if(s.Length == parser.nGet)
                        {
                            return Width2Index(console.DSPBufDigTX);
                        }
                        else
                            return parser.Error1;*/

            return parser.Error1;
        }

        // Sets the CAT Rig Type to SDR-1000
        //Modified 10/12/08 BT changed "SDR-1000" to "PowerSDR"
        public string ZZID()
        {
            //			if(s.Length == parser.nSet)
            //			{
            //				return CAT2RigType(s);
            //			}
            //			else if(s.Length == parser.nGet)
            //			{
            //				return RigType2CAT();
            //			}
            //			else
            //				return parser.Error1;
            console.SetupForm.CATSetRig("PowerSDR");
            return "";
        }

        // Reads the SDR-1000 transceiver status
        public string ZZIF(string s)
        {
            string rtn = "";
            string rit = "0";
            string xit = "0";
            string incr;
            string tx = "0";
            int ITValue = 0;

            // Get the rit/xit status
            if (console.RITOn)
                rit = "1";
            else if (console.XITOn)
                xit = "1";
            // Get the incremental tuning value for whichever control is selected
            if (rit == "1")
                ITValue = console.RITValue;
            else if (xit == "1")
                ITValue = console.XITValue;
            // Format the IT value
            if (ITValue < 0)
                incr = "-" + Convert.ToString(Math.Abs(ITValue)).PadLeft(5, '0');
            else
                incr = "+" + Convert.ToString(Math.Abs(ITValue)).PadLeft(5, '0');
            // Get the rx - tx status
            if (console.MOX)
                tx = "1";
            // Get the step size
            int step = console.StepSize;
            string stepsize = Step2String(step);
            // Get the vfo split status
            string split = "0";
            bool retval = console.VFOSplit;
            if (retval)
                split = "1";

            string f = ZZFA("");
            if (f.Length > 11)
            {
                f = f.Substring(f.Length - 11, 11);
            }
            rtn += f;
            //			rtn += StrVFOFreq("A");						// VFO A frequency			11 bytes
            rtn += stepsize;							// Console step frequency	 4 bytes
            rtn += incr;								// incremental tuning value	 6 bytes
            rtn += rit;									// RIT status				 1 byte
            rtn += xit;									// XIT status				 1 byte
            rtn += "000";								// dummy for memory bank	 3 bytes
            rtn += tx;									// tx-rx status				 1 byte
            rtn += Mode2String(console.CurrentDSPMode);	// current mode				 2 bytes
            rtn += "0";									// dummy for FR/FT			 1 byte
            rtn += "0";									// dummy for scan status	 1 byte
            rtn += split;								// VFO Split status			 1 byte
            rtn += "0000";								// dummy for the balance	 4 bytes
            return rtn;
        }

        // Sets or reads the IF width
        public string ZZIS(string s)
        {
            int n = 0;

            if (s != "")
                n = Convert.ToInt32(s);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Width", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.CATFilterWidth);
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the IF Shift
        public string ZZIT(string s)
        {
            int n = 0;
            string sign = "-";

            if (s != "")
                n = Convert.ToInt32(s);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Shift", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                n = console.CATFilterShift;
                if (n >= 0)
                {
                    sign = "+";
                }
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(n)).Substring(1);
            }
            else
            {
                return parser.Error1;
            }
        }

        // Resets the Filter Shift to zero.  Write only
        public string ZZIU()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Shift", 0, parm2, "");

            return "";
        }

        //Sends a CWX macro
        public string ZZKM(string s)
        {
            int qn = 0;
            if (s != "0" && s.Length > 0)
            {
                qn = Convert.ToInt32(s);
                // Make sure we have an instance of the form
                if (console.CWXForm == null || console.CWXForm.IsDisposed)
                {
                    try
                    {
                        int[] parm2 = new int[1];

                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Start", 0, parm2, "");
                    }
                    catch
                    {
                        return parser.Error1;
                    }
                }
                if (qn > 0 || qn < 10)
                {
                    console.CWXForm.StartQueue = qn;
                    return "";
                }
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the CWX CW speed
        public string ZZKS(string s)
        {
            int cws = 0;
            int[] parm2 = new int[1];

            // Make sure we have an instance of the form
            if (console.CWXForm == null || console.CWXForm.IsDisposed)
            {
                try
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Start", 0, parm2, "");
                }
                catch
                {
                    return parser.Error1;
                }
            }

            if (s.Length == parser.nSet)
            {
                cws = Convert.ToInt32(s);
                cws = Math.Max(1, cws);
                cws = Math.Min(99, cws);

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Speed", cws, parm2, "");

                return "";

            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string answer = console.CWXForm.WPM.ToString().PadLeft(3, '0');
                return answer;
            }
            else
                return parser.Error1;
        }

        //Sends text to CWX for conversion to Morse
        public string ZZKY(string s)
        {
            int[] parm2 = new int[1];

            // Make sure we have an instance of the form
            if (console.CWXForm == null || console.CWXForm.IsDisposed)
            {
                try
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Start", 0, parm2, "");
                }
                catch
                {
                    return parser.Error1;
                }
            }

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOA", (int)DSPMode.CWU, parm2, "");

            Thread.Sleep(100);

            if (s.Length == parser.nSet)
            {

                string trms = "";
                byte[] msg;
                string x = s.Trim();

                if (x.Length == 0)
                    trms = " ";
                else
                    trms = s.TrimEnd();

                if (trms.Length > 1)
                {
                    msg = AE.GetBytes(trms);

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Remote Msg", 0, msg);

                    return "";
                }
                else
                {
                    char ss = Convert.ToChar(trms);

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Remote Msg", 0, ss);

                    return "";
                }
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int ch = console.CWXForm.Characters2Send;
                if (ch > 0 && ch < 72)
                    return "0";
                else if (ch >= 72)
                    return "1";
                else if (ch == 0)
                    return "2";
                else
                    return parser.Error1;
            }
            else
                return parser.Error1;
        }

        // Sets or reads the MUT button on/off status
        public string ZZMA(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MUT", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MUT", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool retval = console.MUT;
                if (retval)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the Main RX DSP mode
        public string ZZMD(string s)
        {
            if (s.Length == parser.nSet)
            {
                if (Convert.ToInt32(s) >= 0 && Convert.ToInt32(s) <= 11)
                {
                    int[] parm2 = new int[1];

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOA", Int32.Parse(s), parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return Mode2String(console.CurrentDSPMode);
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the SUB RX DSP mode
        public string ZZME(string s)
        {
            if (s.Length == parser.nSet)
            {
                if (Convert.ToInt32(s) >= 0 && Convert.ToInt32(s) <= 11)
                {
                    int[] parm2 = new int[1];

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "DSP Mode VFOB", Int32.Parse(s), parm2, "");

                    return "";
                }
                else
                    return parser.Error1;
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return Mode2String(console.CurrentDSPModeSubRX);
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the Mic gain control
        public string ZZMG(string s)
        {
            int n;
            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Min(70, n);
                n = Math.Max(0, n);
                s = AddLeadingZeros(n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MIC", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.CATMIC);
            }
            else
                return parser.Error1;
        }

        //Sets or reads the Monitor (MON) button status
        public string ZZMO(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MON", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MON", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool retval = console.MON;
                if (retval)
                    return "1";
                else
                    return "0";
            }
            else
                return parser.Error1;
        }

        // Sets or reads the RX meter mode
        public string ZZMR(string s)
        {
            int m = -1;
            if (s != "")
                m = Convert.ToInt32(s);

            if (s.Length == parser.nSet &&
                (m > (int)MeterRXMode.FIRST && m < (int)MeterRXMode.LAST))
            {
                String2RXMeter(m);
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return RXMeter2String();
            }
            else
            {
                return parser.Error1;
            }

        }

        //Sets or reads the MultiRX Swap checkbox
/*        public string ZZMS(string s)
        {
            if (s.Length == parser.nSet && (s == "1" || s == "0"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "PAN Swap", 1, parm2, "");
                }
                else if (s == "0")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "PAN Swap", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATPanSwap;
            }
            else
                return parser.Error1;
        }*/

        //Sets or reads the SubRX checkbox  yt7pwr
        public string ZZMU(string s)
        {
            if (s.Length == parser.nSet && (s == "1" || s == "0"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SUB Rx", 1, parm2, "");
                }
                else if (s == "0")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SUB Rx", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATSubRX;
            }
            else
                return parser.Error1;
        }

        // Sets or reads the TX meter mode
        public string ZZMT(string s)
        {
            int m = -1;
            if (s != "")
                m = Convert.ToInt32(s);

            if (s.Length == parser.nSet &&
                (m > (int)MeterTXMode.FIRST && m < (int)MeterTXMode.LAST))
            {
                String2TXMeter(m);
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return TXMeter2String().PadLeft(2, '0');		//Added padleft 4/2/2007 BT
            }
            else
            {
                return parser.Error1;
            }
        }

        #endregion Extended CAT Methods ZZG-ZZM

        #region Extended CAT Methods ZZN-ZZQ

        //Sets or reads Noise Blanker 2 status
        public string ZZNA(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NB1", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATNB1.ToString();
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the Noise Blanker 2 status
        public string ZZNB(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NB2", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATNB2.ToString();
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the Noise Blanker 1 threshold
        public string ZZNL(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NB1 Threshold", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.SetupForm.CATNB1Threshold);
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the Noise Blanker 2 threshold
        public string ZZNM(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NB2 Threshold", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.SetupForm.CATNB2Threshold);
            }
            else
            {
                return parser.Error1;
            }

        }


        // Sets or reads the Noise Reduction status
        public string ZZNR(string s)
        {
            int sx = 0;

            if (s != "")
                sx = Convert.ToInt32(s);

            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NR", sx, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATNR.ToString();
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the ANF button status
        public string ZZNT(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ANF", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATANF.ToString();
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the RF satate
        public string ZZPA(string s)
        {
            int rf = 0;

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                rf = Convert.ToInt32(s);
                rf = (int)((rf - 20) * 140 / 255);

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF", rf, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                rf = console.RF;
                rf = (int)((rf + 20) * 255 / 140);
                return "ZZPA" + AddLeadingZeros(rf) + ";";
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the Drive level
        public string ZZPC(string s)
        {
            int pwr = 0;

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                pwr = Convert.ToInt32(s);

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "PWR", pwr, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string answer = console.PWR.ToString().PadLeft(3, '0');
                return answer;
            }
            else
            {
                return parser.Error1;
            }
        }

        //Centers the Display Pan scroll
        public string ZZPD()
        {
            console.CATDispCenter = "1";
            return "";
        }

        //Sets or reads the Speech Compressor button status
        public string ZZPK(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    parm2[0] = 0;

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "COMP", 0, parm2, "");
                }
                else if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "COMP", 1, parm2, "");
                }
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool comp = console.COMP;
                if (comp)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return "";
            }
        }

        // Sets or reads the Speech Compressor threshold
        public string ZZPL(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "COMP Threshold", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.SetupForm.CATCompThreshold);
            }
            else
            {
                return parser.Error1;
            }

        }

        //Sets or reads the Display Peak button status
        public string ZZPO(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Display Peak", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATDispPeak;
            }
            else
                return parser.Error1;
        }

        //Sets or reads the Power button status
        public string ZZPS(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Power", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Power", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool pwr = console.PowerOn;

                if (pwr)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets the Display Zoom buttons
        public string ZZPZ(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Display Zoom", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATDispZoom;
            }
            else
                return parser.Error1;

        }

        // Reads the Quick Memory Save value
        public string ZZQM()
        {
            return StrVFOFreq("C");
        }

        // Recalls Memory Quick Save
        public string ZZQR()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Memory Recall", 0, parm2, "");

            return "";
        }

        //Saves Quick Memory value
        public string ZZQS()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Memory Save", 0, parm2, "");

            return "";
        }


        #endregion Extended CAT Methods ZZN-ZZQ

        #region Extended CAT Methods ZZR-ZZZ

        // Sets or reads the RTTY Offset Enable VFO A checkbox
        public string ZZRA(string s)
        {
            if (s.Length == parser.nSet)
            {
                /*				if(s == "0")
                                    console.SetupForm.RttyOffsetEnabledA = false;
                                else if(s == "1") 
                                    console.SetupForm.RttyOffsetEnabledA = true;*/
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool ans = console.SetupForm.RttyOffsetEnabledA;
                if (ans)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the RTTY Offset Enable VFO B checkbox
        public string ZZRB(string s)
        {
            if (s.Length == parser.nSet)
            {
                /*				if(s == "0")
                                    console.SetupForm.RttyOffsetEnabledB = false;
                                else if(s == "1") 
                                    console.SetupForm.RttyOffsetEnabledB = true;*/
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool ans = console.SetupForm.RttyOffsetEnabledB;
                if (ans)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        //Clears the RIT frequency
        public string ZZRC()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Clear", 0, parm2, "");

            return "";
        }

        //Decrements RIT
        public string ZZRD(string s)
        {
            if (s.Length == parser.nSet)
            {
                return ZZRF(s);
            }
            else if (s.Length == parser.nGet && console.RITOn)
            {
                int[] parm2 = new int[1];

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Down", 10, parm2, "");
                        break;
                    case DSPMode.LSB:
                    case DSPMode.USB:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Down", 50, parm2, "");
                        break;
                }

                return "";
            }
            else
                return parser.Error1;
        }

        // Sets or reads the RIT frequency value
        public string ZZRF(string s)
        {
            int n = 0;
            int x = 0;
            string sign = "";

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-20000, n);
                n = Math.Min(20000, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Value", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.RITValue;

                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";

                // we have to remove the leading zero and replace it with the sign.
                return sign + Convert.ToString(Math.Abs(x)).PadLeft(4, '0');
            }
            else
            {
                return parser.Error1;
            }
        }


        //Sets or reads the RTTY DIGH offset frequency ud counter
        public string ZZRH(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-3000, n);
                n = Math.Min(3000, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RTTY OffsetHigh", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.SetupForm.RttyOffsetHigh;
                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(x)).Substring(1);
            }
            else
            {
                return parser.Error1;
            }

        }

        //Sets or reads the RTTY DIGL offset frequency ud counter
        public string ZZRL(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-3000, n);
                n = Math.Min(3000, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RTTY OffsetLow", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.SetupForm.RttyOffsetLow;
                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(x)).Substring(1);
            }
            else
            {
                return parser.Error1;
            }

        }

        // Reads the Console RX meter
        public string ZZRM(string s)
        {
            string output = parser.Error1;
            if (!console.MOX)
            {
                switch (s)
                {
                    case "0":
                        output = console.CATReadSigStrength().PadLeft(20);
                        break;
                    case "1":
                        output = console.CATReadAvgStrength().PadLeft(20);
                        break;
                    case "2":
                        output = console.CATReadADC_L().PadLeft(20);
                        break;
                    case "3":
                        output = console.CATReadADC_R().PadLeft(20);
                        break;
                    default:
                        output = console.CATReadAvgStrength().PadLeft(20);
                        break;
                }
            }
            else
            {
                switch (s)
                {
                    case "4":
                        output = console.CATReadALC().PadLeft(20);
                        break;
                    case "5":
                        output = console.CATReadFwdPwr().PadLeft(20);
                        break;
                    case "6":
                        output = parser.Error1;
                        break;
                    case "7":
                        output = console.CATReadRevPwr().PadLeft(20);
                        break;
                    case "8":
                        output = console.CATReadSWR().PadLeft(20);
                        break;
                    default:
                        output = console.CATReadFwdPwr().PadLeft(20);
                        break;
                }
            }
            return output;
        }
        //Sets or reads the SubRX button status
        public string ZZRS(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SUB RX Enable", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SUB RX Enable", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.EnableSubRX)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }


        //Sets or reads the RIT button status
        public string ZZRT(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];
                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Enable", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Enable", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool rit = console.RITOn;
                if (rit)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        //Increments RIT
        public string ZZRU(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-20000, n);
                n = Math.Min(20000, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                switch (console.CurrentDSPMode)
                {
                    case DSPMode.CWL:
                    case DSPMode.CWU:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Up", 10, parm2, "");
                        break;
                    case DSPMode.LSB:
                    case DSPMode.USB:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RIT Up", 50, parm2, "");
                        break;
                }
                return "";
            }
            else if (s.Length == parser.nGet && console.RITOn)
            {
                x = console.RITValue;
                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(x)).Substring(1);
            }
            else
                return parser.Error1;
        }

        //Moves VFO A down one Tune Step
        public string ZZSA()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOA down", console.StepSize, parm2, "");

            return "";
        }

        //Moves VFO A up one Tune Step
        public string ZZSB()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOA up", console.StepSize, parm2, "");

            return "";
        }

        //Moves the mouse wheel tuning step down
        public string ZZSD()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "StepSize VFOA down", 1, parm2, "");

            return "";
        }

        // ZZSFccccwwww  Set Filter, cccc=center freq www=width both in hz 
        public string ZZSF(string s)
        {
            if (s == "")
                return "";
            else
            {
                int center = Convert.ToInt32(s.Substring(0, 4), 10);
                int width = Convert.ToInt32(s.Substring(4), 10);
                SetFilterCenterAndWidth(center, width);
                return "";
            }
        }


        //Moves VFO B down one Tune Step
        public string ZZSG()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOB down", console.StepSizeSubRX, parm2, "");

            return "";
        }

        //Moves VFO B up one Tune Step
        public string ZZSH()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOB up", console.StepSizeSubRX, parm2, "");

            return "";
        }

        // Reads the S Meter value
        public string ZZSM(string s)
        {
            int sm = 0;

            if (s == "0" || s == "1" || s == "")	// read the main transceiver s meter
            {
                float num = 0f;

                if (console.PowerOn)
                    if (s == "0" || s == "")
                        num = DttSP.CalculateRXMeter(0, 0, DttSP.MeterType.SIGNAL_STRENGTH);
                    else
                        num = DttSP.CalculateRXMeter(0, 1, DttSP.MeterType.SIGNAL_STRENGTH);

                num = num +
                    console.MultimeterCalOffset +
                    console.filter_size_cal_offset;

                num = Math.Max(-140, num);
                num = Math.Min(-10, num);
                sm = ((int)num + 140) * 2;
                return sm.ToString().PadLeft(3, '0');
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the VFO Split status
        public string ZZSP(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SPLIT", 0, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SPLIT", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool retval = console.VFOSplit;
                if (!retval)
                    return "0";
                else
                    return "1";
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the VFOA Squelch on/off status
        public string ZZSO(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOA Enable", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATSquelch.ToString();
            }
            else
                return parser.Error1;
        }

        // Sets or reads the VFOB Squelch on/off status    yt7pwr
        public string ZZS0(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOB Enable", Int32.Parse(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return console.CATSquelchSubRX.ToString();
            }
            else
                return parser.Error1;
        }

        // Sets or reads the MainRX Squelch contorl
        public string ZZSQ(string s)
        {
            int level = 0;

            if (s.Length == parser.nSet)
            {
                level = Convert.ToInt32(s);
                level = Math.Max(0, level);			// lower bound
                level = Math.Min(160, level);		// upper bound
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOA", level, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.SquelchMainRX);
            }
            else
            {
                return parser.Error1;
            }
        }

        // Sets or reads the SubRX Squelch contorl  yt7pwr
        public string ZZS1(string s)
        {
            int level = 0;

            if (s.Length == parser.nSet)
            {
                level = Convert.ToInt32(s);
                level = Math.Max(0, level);			// lower bound
                level = Math.Min(160, level);		// upper bound
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOB", level, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.SquelchSubRX);
            }
            else
            {
                return parser.Error1;
            }
        }

        public string ZZSS()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CWX Stop", 0, parm2, "");

            return "";
        }

        // Reads/Write the current console MainRX step size
        public string ZZST(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "StepSize VFOA", String2Step(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int step = console.StepSize;
                return Step2String(step);
            }
            else
                return parser.Error1;
        }

        // Read/Write the current console SubRX step size
        public string ZZSV(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "StepSize VFOB", String2Step(s), parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                int step = console.StepSizeSubRX;
                return Step2String(step);
            }
            else
                return parser.Error1;
        }

        // Moves the mouse wheel step tune up
        public string ZZSU()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFOA up", 1, parm2, "");

            return "";
        }

        //Swaps VFO A/B TX buttons
        public string ZZSW(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SPLIT", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SPLIT", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool retval = console.SplitAB_TX;
                if (retval)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }

        // Sets or reads the TX filter high setting
        public string ZZTH(string s)
        {
            int th = 0;

            if (s.Length == parser.nSet)	// check the min/max control settings
            {
                th = Convert.ToInt32(s);
                th = Math.Max(500, th);
                th = Math.Min(20000, th);
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TXFilter high", th, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)	// if this is a read command
            {
                return AddLeadingZeros(console.SetupForm.TXFilterHigh);
            }
            else
            {
                return parser.Error1;	// return a ?
            }
        }

        //Inhibits power output when using external antennas, tuners, etc.
        public string ZZTI(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RXOnly", 0, parm2, "");
                }
                else if (s == "1")
                {
                    if (!console.ConsoleClosing)
                    {
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RXOnly", 1, parm2, "");
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 0, parm2, "");
                    }
                }

                return "";
            }
            else
                return parser.Error1;
        }

        // Sets or reads the TX filter low setting
        public string ZZTL(string s)
        {
            int tl = 0;

            if (s.Length == parser.nSet)	// check the min/max control settings
            {
                tl = Convert.ToInt32(s);
                tl = Math.Max(0, tl);
                tl = Math.Min(2000, tl);
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TXFilter low", tl, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)	// if this is a read command
            {
                return AddLeadingZeros(console.SetupForm.TXFilterLow);
            }
            else
            {
                return parser.Error1;	// return a ?
            }
        }

        //Sets or reads the Tune Power level
        public string ZZTO(string s)
        {
            int tl = 0;

            if (s.Length == parser.nSet)	// check the min/max control settings
            {
                tl = Convert.ToInt32(s);
                tl = Math.Max(0, tl);
                tl = Math.Min(100, tl);
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TUN Power", tl, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)	// if this is a read command
            {
                string answer = console.SetupForm.TunePower.ToString().PadLeft(3, '0');
                return answer;
            }
            else
            {
                return parser.Error1;	// return a ?
            }
        }


        //Sets or reads the TX Profile
        public string ZZTP(string s)
        {
            int items = console.CATTXProfileCount;
            int cnt = 0;
            if (s != "")
                cnt = Convert.ToInt32(s);

            if (s.Length == parser.nSet && cnt < items)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TX Profile", cnt, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros(console.CATTXProfile);
            }
            else
                return parser.Error1;
        }

        //Reads temperature
        public string ZZTS(string s)
        {
            if (s.Length == parser.nSet)
            {
                return "?";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return "00030";
            }
            else
                return parser.Error1;
        }

        // Sets or reads the TUN button on/off status
        public string ZZTU(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TUN Enable", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TUN Enable", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool retval = console.TUN;
                if (retval)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }

        //Sets or reads the MOX button status
        public string ZZTX(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.CATPTT)
                    return "1";
                else
                    return "0";
            }
            else
                return parser.Error1;

        }

        // Reads or sets the VAC Enable checkbox (Setup Form)
        public string ZZVA(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.SetupForm.VACEnable)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }

        }


        /// <summary>
        /// Sets or reads the VAC RX Gain 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZVB(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-40, n);
                n = Math.Min(20, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC RX gain", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.VACRXGain;
                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(x)).Substring(1);
            }
            else
            {
                return parser.Error1;
            }
        }

        /// <summary>
        /// Sets or reads the VAC TX Gain
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZVC(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-40, n);
                n = Math.Min(20, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC TX gain", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.VACTXGain;
                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + AddLeadingZeros(Math.Abs(x)).Substring(1);
            }
            else
            {
                return parser.Error1;
            }
        }

        /// <summary>
        /// Sets or reads the VAC Sample Rate
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZVD(string s)
        {
            int n = -1;

            if (s.Length == parser.nSet)
            {
                n = Convert.ToInt32(s);
                int[] parm2 = new int[1];

                switch (n)
                {
                    case 0:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 6000, parm2, "");
                        break;
                    case 1:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 8000, parm2, "");
                        break;
                    case 2:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 11025, parm2, "");
                        break;
                    case 3:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 12000, parm2, "");
                        break;
                    case 4:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 22050, parm2, "");
                        break;
                    case 5:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 24000, parm2, "");
                        break;
                    case 6:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 44100, parm2, "");
                        break;
                    case 7:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 48000, parm2, "");
                        break;
                    case 8:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 96000, parm2, "");
                        break;
                    case 9:
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VAC SampleRate", 192000, parm2, "");
                        break;
                }
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                string rate = console.VACSampleRate;
                string ans = "";

                switch (rate)
                {
                    case "6000":
                        ans = "0";
                        break;
                    case "8000":
                        ans = "1";
                        break;
                    case "11025":
                        ans = "2";
                        break;
                    case "12000":
                        ans = "3";
                        break;
                    case "24000":
                        ans = "4";
                        break;
                    case "22050":
                        ans = "5";
                        break;
                    case "41000":
                        ans = "6";
                        break;
                    case "48000":
                        ans = "7";
                        break;
                    default:
                        ans = parser.Error1;
                        break;
                }
                return ans;
            }
            else
            {
                return parser.Error1;
            }
        }

        //Reads or sets the VOX Enable button status
        public string ZZVE(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "1")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VOX", 1, parm2, "");
                }
                else
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VOX", 0, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                if (console.VOXEnable)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }


        /// <summary>
        /// Sets or reads the VAC Stereo checkbox
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZVF(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return "1";     // always 1!
            }
            else
            {
                return parser.Error1;
            }
        }

        //Reads or set the VOX Gain control
        public string ZZVG(string s)
        {
            int n = 0;

            if (s != null && s != "")
                n = Convert.ToInt32(s);
            n = Math.Max(0, n);
            n = Math.Min(1000, n);

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VOX Gain", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                return AddLeadingZeros((int)console.VOXSens);
            }
            else
            {
                return parser.Error1;
            }

        }

        // Reads or sets the VFO Lock button status
        public string ZZVL(string s)
        {
            if (s.Length == parser.nSet && (s == "0" || s == "1"))
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO Lock", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO Lock", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool retval = console.CATVFOLock;
                if (retval)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        // Returns the version number of the PowerSDR program
        public string ZZVN()
        {
            return "ZZVN02.4.4.0000;";
            //return console.CATGetVersion().PadLeft(10, '0');
        }

        // Sets the VFO swap status
        // write only
        public string ZZVS(string s)
        {
            if (s.Length == parser.nSet & Convert.ToInt32(s) <= 6)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO Swap", Int32.Parse(s), parm2, "");

                return "";
            }
            else
            {
                return parser.Error1;
            }
        }

        // Clears the XIT frequency
        // write only
        public string ZZXC()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "XIT Value", 0, parm2, "");

            return "";
        }

        // Sets or reads the XIT frequency value
        public string ZZXF(string s)
        {
            int n = 0;
            int x = 0;
            string sign;

            if (s != "")
            {
                n = Convert.ToInt32(s);
                n = Math.Max(-20000, n);
                n = Math.Min(20000, n);
            }

            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (!console.ConsoleClosing)
                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "XIT Value", n, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                x = console.XITValue;

                if (x >= 0)
                    sign = "+";
                else
                    sign = "-";
                // we have to remove the leading zero and replace it with the sign.
                return sign + x.ToString().PadLeft(4, '0');
            }
            else
            {
                return parser.Error1;
            }
        }

        //Sets or reads the XIT button status
        public string ZZXS(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "XIT Status", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "XIT Status", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool xit = console.XITOn;
                if (xit)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        /// <summary>
        /// CAT over ethernet sync
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ZZXX(string s)            // yt7pwr
        {
            try
            {
                if (s.Length == parser.nSet)
                {
                    return "?";
                }
                else if (s.Length == parser.nGet)
                {
                    string answer = "";
                    answer = "ZZSW" + ZZSW(s) + ";" + "ZZSP" + ZZSP(s) + ";" + "ZZTU" + ZZTU(s) + ";" + "ZZTX" + ZZTX(s) + ";" +
                        "ZZRT" + ZZRT(s) + ";" + "ZZGT" + ZZGT(s) + ";" + "ZZMU" + ZZMU(s) + ";" + "ZZPS" + ZZPS(s) + ";" +
                        "ZZXS" + ZZXS(s) + ";" + "ZZAC" + ZZAC(s) + ";" + "ZZMD" + ZZMD(s) + ";" + "ZZME" + ZZME(s) + ";" +
                        "ZZFJ" + ZZFJ(s) + ";" + "ZZFI" + ZZFI(s) + ";" + "ZZPC" + ZZPC(s) + ";" + 
                        "ZZAG" + ZZAG(s) + ";" + "ZZKS" + ZZKS(s) + ";" + "ZZTO" + ZZTO(s) + ";" +
                        "ZZFA" + ZZFA(s) + ";" + "ZZFB" + ZZFB(s) + ";" + 
                        "ZZFO" + ZZFO(s) + ";";

                    return answer;
                }
                else
                    return parser.Error1;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return parser.Error1;
            }
        }

        //Reads USB connection status
        public string ZZUB(string s)
        {
            if (s.Length == parser.nSet)
            {
                int[] parm2 = new int[1];

                if (s == "0")
                {
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "USB", 0, parm2, "");
                }
                else if (s == "1")
                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "USB", 1, parm2, "");

                return "";
            }
            else if (s.Length == parser.nGet || s.Length == 0)
            {
                bool usb = false;

                if (usb)
                    return "1";
                else
                    return "0";
            }
            else
            {
                return parser.Error1;
            }
        }

        public string ZZZZ()
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "CAT Serial Destroy", 0, parm2, "");

            return "";
        }

        #endregion Extended CAT Methods ZZR-ZZZ

        #region Helper methods

        #region General Helpers

        private string AddLeadingZeros(int n)
        {
            string num = n.ToString();

            while (num.Length < parser.nAns)
                num = num.Insert(0, "0");

            return num;
        }

        #endregion General Helpers

        #region Split Methods

        private string GetP10()
        {
            return lastFT;
        }

        private string GetP12()
        {
            return lastFT;
        }

        #endregion Split Methods

        #region VFO Methods

        // Converts a vfo frequency to a proper CAT frequency string
        private string StrVFOFreq(string vfo)
        {
            double freq = 0;
            string cmd_string = "";

            if (vfo == "A")
                freq = Math.Round(console.CATVFOA, 6);
            else if (vfo == "B")
                freq = Math.Round(console.CATVFOB, 6);
            else if (vfo == "L")
                freq = console.LOSCFreq;        // yt7pwr


            if ((int)freq < 10)
            {
                cmd_string += "0000" + freq.ToString();
            }
            else if ((int)freq < 100)
            {
                cmd_string += "000" + freq.ToString();
            }
            else if ((int)freq < 1000)
            {
                cmd_string += "00" + freq.ToString();
            }
            else if ((int)freq < 10000)
            {
                cmd_string += "0" + freq.ToString();
            }
            else
                cmd_string = freq.ToString();

            // Get rid of the decimal separator and pad the right side with 0's 
            // Modified 05/01/05 BT for globalization
            if (cmd_string.IndexOf(separator) > 0)
                cmd_string = cmd_string.Remove(cmd_string.IndexOf(separator), 1);
            cmd_string = cmd_string.PadRight(11, '0');
            return cmd_string;
        }
        #endregion VFO Methods

        #region Filter Methods

        public string Filter2String(Filter f)
        {
            string fw = f.ToString();
            string strfilt = "";
            int retval = 0;

            switch (fw)
            {
                case "F6000":
                    strfilt = "6000";
                    break;
                case "F4000":
                    strfilt = "4000";
                    break;
                case "F2600":
                    strfilt = "2600";
                    break;
                case "F2100":
                    strfilt = "2100";
                    break;
                case "F1000":
                    strfilt = "1000";
                    break;
                case "F500":
                    strfilt = "0500";
                    break;
                case "F250":
                    strfilt = "0250";
                    break;
                case "F100":
                    strfilt = "0100";
                    break;
                case "F50":
                    strfilt = "0050";
                    break;
                case "F25":
                    strfilt = "0025";
                    break;
                case "VAR1":
                    retval = Math.Abs(console.FilterHighValue - console.FilterLowValue);
                    strfilt = AddLeadingZeros(retval);
                    break;
                case "VAR2":
                    retval = Math.Abs(console.FilterHighValue - console.FilterLowValue);
                    strfilt = AddLeadingZeros(retval);
                    break;
            }
            return strfilt;
        }

        public Filter String2Filter(string f)
        {
            Filter filter = Filter.FIRST;

            switch (f)
            {
                case "6000":
                    filter = Filter.F1;
                    break;
                case "4000":
                    filter = Filter.F2;
                    break;
                case "2600":
                    filter = Filter.F3;
                    break;
                case "2100":
                    filter = Filter.F4;
                    break;
                case "1000":
                    filter = Filter.F5;
                    break;
                case "0500":
                    filter = Filter.F6;
                    break;
                case "0250":
                    filter = Filter.F7;
                    break;
                case "0100":
                    filter = Filter.F8;
                    break;
                case "0050":
                    filter = Filter.F9;
                    break;
                case "0025":
                    filter = Filter.F10;
                    break;
                case "VAR1":
                    filter = Filter.VAR1;
                    break;
                case "VAR2":
                    filter = Filter.VAR2;
                    break;
            }
            return filter;
        }

        // set variable filter 1 to indicate center and width 
        // 
        // if either center or width is zero, current value of center or width is 
        // contained 
        // fixme ... what should this thing do for am, fm, dsb ... ignore width? 
        private void SetFilterCenterAndWidth(int center, int width)
        {
            int new_lo;
            int new_hi;

            if (center == 0 || width == 0)  // need to get current values 
            {
                return; // not implemented yet 
            }
            else
            {
                // Debug.WriteLine("center: " + center  + " width: " + width); 
                new_lo = center - (width / 2);
                new_hi = center + (width / 2);
                if (new_lo < 0) new_lo = 0;
            }

            // new_lo and new_hi calculated assuming a USB mode .. do the right thing 
            // for lsb and other modes 
            // fixme -- needs more thinking 
            switch (console.CurrentDSPMode)
            {
                case DSPMode.LSB:
                    int scratch = new_hi;
                    new_hi = -new_lo;
                    new_lo = -scratch;
                    break;

                case DSPMode.AM:
                case DSPMode.SAM:
                    new_lo = -new_hi;
                    break;
            }


            // System.Console.WriteLine("zzsf: " + new_lo + " " + new_hi); 
            console.SelectVarFilter();
            console.UpdateFilters(new_lo, new_hi);

            return;
        }

        // Converts interger filter frequency into Kenwood SL/SH codes
        private string Frequency2Code(int f, string n)
        {
            f = Math.Abs(f);
            string code = "";
            switch (console.CurrentDSPMode)
            {
                case DSPMode.CWL:
                case DSPMode.CWU:
                case DSPMode.LSB:
                case DSPMode.USB:
                case DSPMode.DIGU:
                case DSPMode.DIGL:
                    switch (n)
                    {
                        case "SH":
                            if (f >= 0 && f <= 1500)
                                code = "00";
                            else if (f > 1500 && f <= 1700)
                                code = "01";
                            else if (f > 1700 && f <= 1900)
                                code = "02";
                            else if (f > 1900 && f <= 2100)
                                code = "03";
                            else if (f > 2100 && f <= 2300)
                                code = "04";
                            else if (f > 2300 && f <= 2500)
                                code = "05";
                            else if (f > 2500 && f <= 2700)
                                code = "06";
                            else if (f > 2700 && f <= 2900)
                                code = "07";
                            else if (f > 2900 && f <= 3200)
                                code = "08";
                            else if (f > 3200 && f <= 3700)
                                code = "09";
                            else if (f > 3700 && f <= 4500)
                                code = "10";
                            else if (f > 4500)
                                code = "11";
                            break;
                        case "SL":
                            if (f >= 0 && f <= 25)
                                code = "00";
                            else if (f > 25 && f <= 75)
                                code = "01";
                            else if (f > 75 && f <= 150)
                                code = "02";
                            else if (f > 150 && f <= 250)
                                code = "03";
                            else if (f > 250 && f <= 350)
                                code = "04";
                            else if (f > 350 && f <= 450)
                                code = "05";
                            else if (f > 450 && f <= 550)
                                code = "06";
                            else if (f > 550 && f <= 650)
                                code = "07";
                            else if (f > 650 && f <= 750)
                                code = "08";
                            else if (f > 750 && f <= 850)
                                code = "09";
                            else if (f > 850 && f <= 950)
                                code = "10";
                            else if (f > 950)
                                code = "11";
                            break;
                    }
                    break;
                case DSPMode.AM:
                case DSPMode.DRM:
                case DSPMode.DSB:
                case DSPMode.FMN:
                case DSPMode.SAM:
                    switch (n)
                    {
                        case "SH":
                            if (f >= 0 && f <= 2750)
                                code = "00";
                            else if (f > 2750 && f <= 3500)
                                code = "01";
                            else if (f > 3500 && f <= 4500)
                                code = "02";
                            else if (f > 4500)
                                code = "03";
                            break;
                        case "SL":
                            if (f >= 0 && f <= 50)
                                code = "00";
                            else if (f > 50 && f <= 150)
                                code = "01";
                            else if (f > 150 && f <= 350)
                                code = "02";
                            else if (f > 350)
                                code = "03";
                            break;
                    }
                    break;

                /*case DSPMode.DIGU:
                case DSPMode.DIGL:
                switch (n)
                {
                    case "SH":
                        break;

                    case "SL":
                        break;
                }
                break;*/
            }
            return code;
        }

        // Converts a frequency code pair to frequency in hz according to
        // the Kenwood TS-2000 spec.  Receives code and calling methd as parameters
        private int Code2Frequency(string c, string n)
        {
            int freq = 0;
            string mode = "SSB";
            int fgroup = 0;

            // Get the current console mode
            switch (console.CurrentDSPMode)
            {
                case DSPMode.LSB:
                case DSPMode.USB:
                    break;
                case DSPMode.AM:
                case DSPMode.DSB:
                case DSPMode.DRM:
                case DSPMode.FMN:
                case DSPMode.SAM:
                    mode = "DSB";
                    break;
            }
            // Get the frequency group(SSB/SL, SSB/SH, DSB/SL, and DSB/SH)
            switch (n)
            {
                case "SL":
                    if (mode == "SSB")
                        fgroup = 1;
                    else
                        fgroup = 3;
                    break;
                case "SH":
                    if (mode == "SSB")
                        fgroup = 2;
                    else
                        fgroup = 4;
                    break;
            }
            // return the frequency for the current DSP mode and calling method
            switch (fgroup)
            {
                case 1:		//SL SSB
                    switch (c)
                    {
                        case "00":
                            freq = 0;
                            break;
                        case "01":
                            freq = 50;
                            break;
                        case "02":
                            freq = 100;
                            break;
                        case "03":
                            freq = 200;
                            break;
                        case "04":
                            freq = 300;
                            break;
                        case "05":
                            freq = 400;
                            break;
                        case "06":
                            freq = 500;
                            break;
                        case "07":
                            freq = 600;
                            break;
                        case "08":
                            freq = 700;
                            break;
                        case "09":
                            freq = 800;
                            break;
                        case "10":
                            freq = 900;
                            break;
                        case "11":
                            freq = 1000;
                            break;
                    }
                    break;
                case 2:		//SH SSB
                    switch (c)
                    {
                        case "00":
                            freq = 1400;
                            break;
                        case "01":
                            freq = 1600;
                            break;
                        case "02":
                            freq = 1800;
                            break;
                        case "03":
                            freq = 2000;
                            break;
                        case "04":
                            freq = 2200;
                            break;
                        case "05":
                            freq = 2400;
                            break;
                        case "06":
                            freq = 2600;
                            break;
                        case "07":
                            freq = 2800;
                            break;
                        case "08":
                            freq = 3000;
                            break;
                        case "09":
                            freq = 3400;
                            break;
                        case "10":
                            freq = 4000;
                            break;
                        case "11":
                            freq = 5000;
                            break;
                    }
                    break;
                case 3:		//SL DSB
                    switch (c)
                    {
                        case "00":
                            freq = 0;
                            break;
                        case "01":
                            freq = 100;
                            break;
                        case "02":
                            freq = 200;
                            break;
                        case "03":
                            freq = 500;
                            break;
                    }
                    break;
                case 4:		//SH DSB
                    switch (c)
                    {
                        case "00":
                            freq = 2500;
                            break;
                        case "01":
                            freq = 3000;
                            break;
                        case "02":
                            freq = 4000;
                            break;
                        case "03":
                            freq = 5000;
                            break;
                    }
                    break;
            }
            return freq;
        }

        private void SetFilter(string c, string n)
        {
            // c = code, n = SH or SL
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", (int)Filter.VAR1, parm2, "");

            Thread.Sleep(100);
            int freq = 0;
            int offset = 0;
            string code;

            switch (console.CurrentDSPMode)
            {
                case DSPMode.USB:
                case DSPMode.CWU:
                    freq = Code2Frequency(c, n);

                    if (n == "SH")
                    {
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter High", freq, parm2, "");
                    }
                    else
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Low", freq, parm2, "");
                    break;
                case DSPMode.LSB:
                case DSPMode.CWL:
                    if (n == "SH")
                    {
                        freq = Code2Frequency(c, "SH");	// get the upper limit from the lower value set

                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Low", freq, parm2, "");
                    }
                    else
                    {
                        freq = Code2Frequency(c, "SL");	// do the reverse here, the less positive value

                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter High", freq, parm2, "");
                    }
                    break;
                case DSPMode.AM:
                case DSPMode.DRM:
                case DSPMode.DSB:
                case DSPMode.FMN:
                case DSPMode.SAM:
                    if (n == "SH")
                    {
                        // Set the bandwith equally across the center freq
                        freq = Code2Frequency(c, "SH");

                        if (!console.ConsoleClosing)
                        {
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter High", freq / 2, parm2, "");
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Low", -freq / 2, parm2, "");
                        }
                    }
                    else
                    {
                        // reset the frequency to the nominal value (in case it's been changed)
                        freq = console.FilterHighValue * 2;
                        code = Frequency2Code(freq, "SH");
                        freq = Code2Frequency(code, "SH");

                        if (!console.ConsoleClosing)
                        {
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter High", freq / 2, parm2, "");
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Low", -freq / 2, parm2, "");
                        }

                        offset = Code2Frequency(c, "SL");

                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter Low",
                                console.FilterLowValue + offset, parm2, "");
                    }
                    break;
            }
        }

        #endregion Filter Methods

        #region Mode Methods

        public string Mode2String(DSPMode pMode)
        {
            DSPMode s = pMode;
            string retval = "";

            switch (s)
            {
                case DSPMode.LSB:
                    retval = "00";
                    break;
                case DSPMode.USB:
                    retval = "01";
                    break;
                case DSPMode.DSB:
                    retval = "02";
                    break;
                case DSPMode.CWL:
                    retval = "03";
                    break;
                case DSPMode.CWU:
                    retval = "04";
                    break;
                case DSPMode.FMN:
                    retval = "05";
                    break;
                case DSPMode.AM:
                    retval = "06";
                    break;
                case DSPMode.DIGU:
                    retval = "07";
                    break;
                case DSPMode.SPEC:
                    retval = "08";
                    break;
                case DSPMode.DIGL:
                    retval = "09";
                    break;
                case DSPMode.SAM:
                    retval = "10";
                    break;
                case DSPMode.DRM:
                    retval = "11";
                    break;
                default:
                    retval = parser.Error1;
                    break;
            }

            return retval;
        }

        // converts SDR mode to Kenwood single digit mode code
        public string Mode2KString(DSPMode pMode)
        {
            DSPMode s = pMode;
            string retval = "";

            switch (s)
            {
                case DSPMode.LSB:
                    retval = "1";
                    break;
                case DSPMode.USB:
                    retval = "2";
                    break;
                case DSPMode.CWU:
                    retval = "3";
                    break;
                case DSPMode.FMN:
                    retval = "4";
                    break;
                case DSPMode.AM:
                    //				case DSPMode.SAM:		//possible fix for SAM problem
                    retval = "5";
                    break;
                case DSPMode.DIGL:
                    retval = "6";
                    break;
                case DSPMode.CWL:
                    retval = "7";
                    break;
                case DSPMode.DIGU:
                    retval = "9";
                    break;
                default:
                    retval = parser.Error1;
                    break;
            }

            return retval;
        }

        #endregion Mode Methods

        #region Band Methods

        private void MakeBandList()
        //Construct an array of the PowerSDR.Band enums.
        //If the 2m xverter is present, set the last index to B2M
        //otherwise, set it to B6M.
        {
            int ndx = 0;
            BandList = new Band[(int)Band.LAST + 2];
            foreach (Band b in Enum.GetValues(typeof(Band)))
            {
                BandList.SetValue(b, ndx);
                ndx++;
            }
            LastBandIndex = Array.IndexOf(BandList, Band.B6M);
        }

        private void SetBandGroup(int band)
        {
            int oldval = parser.nSet;
            parser.nSet = 1;
            if (band == 0)
                ZZBG("0");
            else
                ZZBG("1");

            parser.nSet = oldval;
        }

        private string GetBand(string b)
        {
            if (b.Length == parser.nSet)
            {
                if (b.StartsWith("V") || b.StartsWith("v"))
                    SetBandGroup(1);
                else
                    SetBandGroup(0);
            }

            if (b.Length == parser.nSet)
            {
                console.SetCATBand(String2Band(b));
                return "";
            }
            else if (b.Length == parser.nGet)
            {
                string answer = Band2String(console.CurrentBand);
                return answer;
            }
            else
            {
                return parser.Error1;
            }
        }

        private void BandUp()
        {
            Band nextband;
            Band current = console.CurrentBand;
            int currndx = Array.IndexOf(BandList, current);

            if (currndx == LastBandIndex)
                nextband = BandList[0];
            else
                nextband = BandList[currndx + 1];

            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Band set", (int)nextband, parm2, "");
        }

        private void BandDown()
        {
            Band nextband;
            Band current = console.CurrentBand;
            int currndx = Array.IndexOf(BandList, current);
            if (currndx > 0)
                nextband = BandList[currndx - 1];
            else
                nextband = BandList[LastBandIndex];

            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Band set", (int)nextband, parm2, "");
        }

        private string Band2String(Band pBand)
        {
            Band band = pBand;
            string retval;

            switch (band)
            {
                case Band.GEN:
                    retval = "888";
                    break;
                case Band.B160M:
                    retval = "160";
                    break;
                case Band.B60M:
                    retval = "060";
                    break;
                case Band.B80M:
                    retval = "080";
                    break;
                case Band.B40M:
                    retval = "040";
                    break;
                case Band.B30M:
                    retval = "030";
                    break;
                case Band.B20M:
                    retval = "020";
                    break;
                case Band.B17M:
                    retval = "017";
                    break;
                case Band.B15M:
                    retval = "015";
                    break;
                case Band.B12M:
                    retval = "012";
                    break;
                case Band.B10M:
                    retval = "010";
                    break;
                case Band.B6M:
                    retval = "006";
                    break;
                case Band.B2M:
                    retval = "002";
                    break;
                case Band.WWV:
                    retval = "999";
                    break;
                default:
                    retval = "888";
                    break;
            }
            return retval;
        }

        private Band String2Band(string pBand)
        {
            string band = pBand.ToUpper(); ;
            Band retval;

            switch (band)
            {
                case "888":
                    retval = Band.GEN;
                    break;
                case "160":
                    retval = Band.B160M;
                    break;
                case "060":
                    retval = Band.B60M;
                    break;
                case "080":
                    retval = Band.B80M;
                    break;
                case "040":
                    retval = Band.B40M;
                    break;
                case "030":
                    retval = Band.B30M;
                    break;
                case "020":
                    retval = Band.B20M;
                    break;
                case "017":
                    retval = Band.B17M;
                    break;
                case "015":
                    retval = Band.B15M;
                    break;
                case "012":
                    retval = Band.B12M;
                    break;
                case "010":
                    retval = Band.B10M;
                    break;
                case "006":
                    retval = Band.B6M;
                    break;
                case "002":
                    retval = Band.B2M;
                    break;
                case "999":
                    retval = Band.WWV;
                    break;
                default:
                    retval = Band.GEN;
                    break;
            }

            return retval;
        }

        #endregion Band Methods

        #region Step Methods

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

        private string Step2String(int pSize)
        {
            // Modified 2/25/07 to accomodate changes to console where odd step sizes added.  BT
            string stepval = "";
            int step = pSize;

            switch (step)
            {
                case 0:
                    stepval = "0000";	//10e0 = 1 hz
                    break;
                case 1:
                    stepval = "0001";	//10e1 = 10 hz
                    break;
                case 2:
                    stepval = "1000";	//special default for 50 hz
                    break;
                case 3:
                    stepval = "0010";	//10e2 = 100 hz
                    break;
                case 4:
                    stepval = "1001";	//special default for 250 hz
                    break;
                case 5:
                    stepval = "1010";	//10e3 = 1 kHz default for 500 hz
                    break;
                case 6:
                    stepval = "0011";	//10e3 = 1 kHz
                    break;
                case 7:
                    stepval = "1011";	//special default for 5 kHz
                    break;
                case 8:
                    stepval = "1100";	//special default for 9 kHz
                    break;
                case 9:
                    stepval = "0100";	//10e4 = 10 khZ
                    break;
                case 10:
                    stepval = "0101";	//10e5 = 100 kHz
                    break;
                case 11:
                    stepval = "1101";   //special default for 250 kHz
                    break;
                case 12:
                    stepval = "1110";   //special default for 500 kHz
                    break;
                case 13:
                    stepval = "0110";	//10e6 = 1 mHz
                    break;
                case 14:
                    stepval = "0111";	//10e7 = 10 mHz
                    break;
            }

            return stepval;
        }

        private int String2Step(string step_string)       // yt7pwr
        {
            int stepval = -1;

            switch (step_string)
            {
                case "0000":
                    stepval = 0;	//10e0 = 1 hz
                    break;
                case "0001":
                    stepval = 1;	//10e1 = 10 hz
                    break;
                case "1000":
                    stepval = 2;	//special default for 50 hz
                    break;
                case "0010":
                    stepval = 3;	//10e2 = 100 hz
                    break;
                case "1001":
                    stepval = 4;	//special default for 250 hz
                    break;
                case "1010":
                    stepval = 5;	//10e3 = 1 kHz default for 500 hz
                    break;
                case "0011":
                    stepval = 6;	//10e3 = 1 kHz
                    break;
                case "1011":
                    stepval = 7;	//special default for 5 kHz
                    break;
                case "1100":
                    stepval = 8;	//special default for 9 kHz
                    break;
                case "0100":
                    stepval = 9;	//10e4 = 10 khZ
                    break;
                case "0101":
                    stepval = 10;	//10e5 = 100 kHz
                    break;
                case "1101":
                    stepval = 11;   //special default for 250 kHz
                    break;
                case "1110":
                    stepval = 12;   //special default for 500 kHz
                    break;
                case "0110":
                    stepval = 13;	//10e6 = 1 mHz
                    break;
                case "0111":
                    stepval = 14;	//10e7 = 10 mHz
                    break;
            }
            return stepval;
        }

        #endregion Step Methods

        #region Meter Methods

        private void String2RXMeter(int m)
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Meter RXMode", m, parm2, "");
        }

        private string RXMeter2String()
        {
            return ((int)console.CurrentMeterRXMode).ToString();
        }

        private void String2TXMeter(int m)
        {
            int[] parm2 = new int[1];

            if (!console.ConsoleClosing)
                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Meter TXMode", m, parm2, "");
        }

        private string TXMeter2String()
        {
            return ((int)console.CurrentMeterTXMode).ToString();
        }

        #endregion Meter Methods

        #region Rig ID Methods

        private string CAT2RigType()
        {
            return "";
        }

        private string RigType2CAT()
        {
            return "";
        }

        #endregion Rig ID Methods

        #region DSP Filter Size Methods

        private string Width2Index(int txt)
        {
            string ans = "";

            switch (txt)
            {
                case 256:
                    ans = "0";
                    break;
                case 512:
                    ans = "1";
                    break;
                case 1024:
                    ans = "2";
                    break;
                case 2048:
                    ans = "3";
                    break;
                case 4096:
                    ans = "4";
                    break;
                default:
                    ans = "0";
                    break;
            }
            return ans;
        }

        private int Index2Width(string ndx)
        {
            int ans;

            switch (ndx)
            {
                case "0":
                    ans = 256;
                    break;
                case "1":
                    ans = 512;
                    break;
                case "2":
                    ans = 1024;
                    break;
                case "3":
                    ans = 2048;
                    break;
                case "4":
                    ans = 4096;
                    break;
                default:
                    ans = 256;
                    break;
            }
            return ans;
        }

        #endregion DSP Filter Size Methods

        #endregion Helper methods

        #region ICOM CAT commands    // yt7pwr

        public byte[] CMD_00(byte[] command)     // transfer op freq data
        {
            try
            {
                return command;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_01(byte[] command)     // transfer op mode data
        {
            try
            {
                return command;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_02(byte[] command)     // read lower/upper freq data
        {
            try
            {
                return command;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_03(byte[] command)     // read op freq data
        {
            try
            {
                if (command[5] == 0xfd)
                {
                    byte[] answer = new byte[11];
                    byte[] frequency = new byte[10];
                    string freq = (console.VFOAFreq * 1e6).ToString();
                    freq = freq.PadLeft(10, '0');
                    ASCIIEncoding buff = new ASCIIEncoding();
                    buff.GetBytes(freq, 0, freq.Length, frequency, 0);
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];         // swapp address
                    answer[3] = command[2];
                    answer[4] = 0x03;
                    answer[5] = (byte)((frequency[8] - 0x30) << 4 | (frequency[9] - 0x30));
                    answer[6] = (byte)((frequency[6] - 0x30) << 4 | (frequency[7] - 0x30));
                    answer[7] = (byte)((frequency[4] - 0x30) << 4 | (frequency[5] - 0x30));
                    answer[8] = (byte)((frequency[2] - 0x30) << 4 | (frequency[3] - 0x30));
                    answer[9] = (byte)((frequency[0] - 0x30) << 4 | (frequency[1] - 0x30));
                    answer[10] = 0xfd;

                    return answer;
                }
                else
                {
                    byte[] answer = new byte[6];
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];         // swapp address
                    answer[3] = command[2];
                    answer[4] = 0xfa;               // error
                    answer[5] = 0xfd;
                    return answer;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_04(byte[] command)     // read op mode data
        {
            try
            {
                byte[] answer = new byte[8];

                if (command[5] == 0xfd)
                {
                    switch (console.CurrentDSPMode)
                    {
                        case DSPMode.LSB:
                            answer[5] = 0;
                            break;

                        case DSPMode.USB:
                            answer[5] = 1;
                            break;

                        case DSPMode.AM:
                            answer[5] = 2;
                            break;

                        case DSPMode.CWU:
                            answer[5] = 3;
                            break;

                        case DSPMode.FMN:
                            answer[5] = 5;
                            break;

                        case DSPMode.CWL:
                            answer[5] = 7;
                            break;

                        case DSPMode.SAM:
                            answer[5] = 0x11;
                            break;
                    }

                    switch (console.CurrentFilter)
                    {
                        case Filter.F5:
                            answer[6] = 1;
                            break;

                        case Filter.F7:
                            answer[6] = 2;
                            break;

                        case Filter.F9:
                            answer[6] = 3;
                            break;

                        default:
                            answer[6] = 1;
                            break;
                    }

                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];         // swapp address
                    answer[3] = command[2];
                    answer[4] = 0x04;
                    answer[7] = 0xfd;
                    return answer;
                }
                else
                {
                    answer = new byte[6];
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];         // swapp address
                    answer[3] = command[2];
                    answer[4] = 0xfa;               // error
                    answer[5] = 0xfd;
                    return answer;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_05(byte[] command)     // write op freq to VFO
        {
            try
            {
                ASCIIEncoding buff = new ASCIIEncoding();
                byte[] answer = new byte[6];
                byte[] frequency = new byte[10];
                int[] parm2 = new int[1];

                frequency[1] = (byte)((command[9] & 0x0f) + 0x30);
                frequency[0] = (byte)((command[9] >> 4 & 0x0f) + 0x30);
                frequency[3] = (byte)((command[8] & 0x0f) + 0x30);
                frequency[2] = (byte)((command[8] >> 4 & 0x0f) + 0x30);
                frequency[5] = (byte)((command[7] & 0x0f) + 0x30);
                frequency[4] = (byte)((command[7] >> 4 & 0x0f) + 0x30);
                frequency[7] = (byte)((command[6] & 0x0f) + 0x30);
                frequency[6] = (byte)((command[6] >> 4 & 0x0f) + 0x30);
                frequency[9] = (byte)((command[5] & 0x0f) + 0x30);
                frequency[8] = (byte)((command[5] >> 4 & 0x0f) + 0x30);
                string freq = buff.GetString(frequency);

                double vfoA = double.Parse(freq);
                vfoA /= 1e6;

                if (vfoA > (console.MaxFreq) || vfoA < console.MinFreq)
                {
                    if (console.BandByFreq(vfoA) != console.CurrentBand)
                            console.SaveBand();

                    console.cat_vfoa = true;
                    console.VFOAFreq = double.Parse(freq) / 1e6;
                    console.cat_vfoa = false;

                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(freq) - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }
                    else
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(freq) / 1e6 - 0.015;                    // yt7pwr
                        console.cat_losc = false;
                    }

                    console.cat_vfoa = true;
                    console.VFOAFreq = double.Parse(freq) / 1e6;

                    if (console.VFOAFreq != double.Parse(freq) / 1e6)
                        console.VFOAFreq = double.Parse(freq) / 1e6;

                    console.cat_vfoa = false;
                }
                else
                {
                    if ((Audio.VACEnabled && Audio.VACDirectI_Q && Audio.VAC_RXshift_enabled) ||
                        (Audio.PrimaryDirectI_Q && Audio.Primary_RXshift_enabled))
                    {
                        console.cat_losc = true;
                        console.LOSCFreq = double.Parse(freq) / 1e6 - Audio.RXShift / 1e6;     // yt7pwr
                        console.cat_losc = false;
                    }

                    console.cat_vfoa = true;
                    console.VFOAFreq = double.Parse(freq) / 1e6;

                    if (console.VFOAFreq != double.Parse(freq) / 1e6)
                        console.VFOAFreq = double.Parse(freq) / 1e6;

                    console.cat_vfoa = false;
                }

                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];         // swapp address
                answer[3] = command[2];
                answer[4] = 0xfb;               // OK
                answer[5] = 0xfd;

                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_06(byte[] command)                                                // write op mode to VFO
        {
            try
            {
                if (command[5] == 0xfd)
                {

                }
                else                                                                        // set mode
                {
                    switch (command[5])
                    {
                        case 0:
                            console.CurrentDSPMode = DSPMode.LSB;
                            break;

                        case 1:
                            console.CurrentDSPMode = DSPMode.USB;
                            break;

                        case 2:
                            console.CurrentDSPMode = DSPMode.AM;
                            break;

                        case 3:
                            console.CurrentDSPMode = DSPMode.CWU;
                            break;

                        case 5:
                        case 6:
                            console.CurrentDSPMode = DSPMode.FMN;
                            break;

                        case 7:
                            console.CurrentDSPMode = DSPMode.CWL;
                            break;

                        case 0x11:
                            console.CurrentDSPMode = DSPMode.SAM;
                            break;
                    }

                    switch (command[6])
                    {
                        case 1:
                            console.CurrentFilter = Filter.F5;
                            break;

                        case 2:
                            console.CurrentFilter = Filter.F7;
                            break;

                        case 3:
                            console.CurrentFilter = Filter.F9;
                            break;
                    }
                }

                byte[] answer = new byte[6];
                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0xfb;           // ok
                answer[5] = 0xfd;
                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_07(byte[] command)                                                // select VFO mode
        {
            try
            {
                byte[] answer = new byte[7];
                int[] parm2 = new int[1];

                if (command[5] == 0xfd)
                {
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];         // swapp address
                    answer[3] = command[2];
                    answer[4] = 0x07;
                    answer[5] = (byte)console.CATVFOMODE;
                    answer[6] = 0xfd;
                }
                else                                                                        // set mode
                {
                    switch (command[5])
                    {
                        case 0:         // VFOA
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 0, parm2, "");
                            break;

                        case 1:         // VFOB
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 1, parm2, "");
                            break;

                        case 0xa0:      // VFOA=VFOB
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 0xa0, parm2, "");
                            break;

                        case 0xb0:      // VFOA/VFOB exchange
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 0xb0, parm2, "");
                            break;

                        case 0xb1:      // VFOA=VFOB
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 0xb1, parm2, "");
                            break;

                        case 0xc0:      // SUB RX off
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 0xc0, parm2, "");
                            break;

                        case 0xc1:      // SUB RX on
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO mode", 0xc1, parm2, "");
                            break;

                        default:
                            answer = new byte[6];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfa;           // error
                            answer[5] = 0xfd;
                            return answer;
                    }

                    switch (command[6])
                    {
                        case 1:
                            console.CurrentFilter = Filter.F5;
                            break;

                        case 2:
                            console.CurrentFilter = Filter.F7;
                            break;

                        case 3:
                            console.CurrentFilter = Filter.F9;
                            break;
                    }
                }

                answer = new byte[6];
                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0xfb;           // ok
                answer[5] = 0xfd;
                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_0F(byte[] command)     // SPLIT mode
        {
            try
            {
                byte[] answer = new byte[7];
                int[] parm2 = new int[1];

                if (command[5] == 0xfd)
                {
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0x0f;

                    if (!console.SplitAB_TX)
                        answer[5] = 0x00;
                    else
                        answer[5] = 0x01;

                    answer[6] = 0xfd;
                    return answer;
                }
                else
                {
                    answer = new byte[6];

                    if (!console.ConsoleClosing)
                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SPLIT", command[5], parm2, "");

                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0xfb;               // ok
                    answer[5] = 0xfd;
                    return answer;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_11(byte[] command)        // ATT state
        {
            try
            {
                byte[] answer = new byte[7];
                int[] parm2 = new int[1];

                if (command[5] == 0xfd)
                {
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0x11;

                    answer[5] = 0x00;

                    answer[6] = 0xfd;
                    return answer;
                }
                else
                {
                    answer = new byte[6];

                    if (command[5] == 0)
                    {
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ATT", 0, parm2, "");
                    }
                    else if (command[5] == 0x12)
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ATT", 1, parm2, "");

                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0xfb;               // ok
                    answer[5] = 0xfd;
                    return answer;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_14(byte[] command)                        // Various Level Settings
        {
            try
            {
                byte[] answer = new byte[6];
                int[] parm2 = new int[1];
                int parm1 = 0;
                string v = "";
                string s = "";

                if (command[6] == 0xfd)
                {
                    switch (command[5])
                    {
                        case 1:                                         // AF level
                            answer = new byte[9];

                            if (console.MUT)
                            {
                                answer[6] = 0x00;
                                answer[7] = 0x00;
                            }
                            else
                            {
                                int vol = (int)(console.AF * 2.55);
                                s = vol.ToString("D");

                                if (s.Length == 3)
                                {
                                    string s1 = s.Remove(0, 1);
                                    int j = int.Parse(s1, NumberStyles.HexNumber);
                                    string s2 = s.Remove(1);
                                    int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                    answer[6] = (byte)(j1);
                                    answer[7] = (byte)(j);
                                }
                                else if (s.Length == 2 || s.Length == 1)
                                {
                                    int j = int.Parse(s, NumberStyles.HexNumber);
                                    answer[7] = (byte)(j & 0x00ff);
                                    answer[6] = (byte)(j >> 8);
                                }
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x01;
                            answer[8] = 0xfd;
                            return answer;

                        case 2:                                         // RF level
                            answer = new byte[9];
                            int rf = (int)(((console.RF + 20) * 255) / 140);
                            s = rf.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x02;
                            answer[8] = 0xfd;
                            return answer;

                        case 3:                                         // SQL level
                            answer = new byte[9];

                            int sql = (int)((console.SquelchMainRX * 255) / 160);
                            s = sql.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x03;
                            answer[8] = 0xfd;
                            return answer;

                        case 6:                                         // NR gain
                            answer = new byte[9];
                            int nr = (int)((console.CATNRgain * 255) / 1000);
                            s = nr.ToString("D");

                            if (s.Length == 3 || s.Length == 4)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2 || s.Length == 1)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x06;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x0a:                                         // PWR level
                            answer = new byte[9];

                            int pwr = (int)(console.PWR * 2.55);
                            s = pwr.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x0a;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x0b:                                         // mic gain
                            answer = new byte[9];

                            int mic = (int)(console.CATMIC * 2.55);
                            s = mic.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x0b;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x0d:                                         // NOTCH gain
                            answer = new byte[9];
                            int notch = (int)(((console.CATNOTCHManual + 5000) * 255) / 10000);
                            s = notch.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x0d;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x0e:                                         // COMP level
                            answer = new byte[9];

                            int cmp = (int)(console.COMPVal * 255 / 20);
                            s = cmp.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2 || s.Length == 1)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x0e;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x12:                                         // NB level
                            answer = new byte[9];

                            int nb_level = (int)(console.SetupForm.CATNB1Threshold * 2.55);
                            s = nb_level.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x12;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x16:                                         // VOX level
                            answer = new byte[9];
                            int vox = (int)((console.VOXSens * 255) / 1000);
                            s = vox.ToString("D");

                            if (s.Length == 3)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x16;
                            answer[8] = 0xfd;
                            return answer;

                        case 0x1a:                                         // manual NOTCH shift
                            answer = new byte[9];
                            int notch_manual = (int)(((console.CATNOTCHManual + 5000) * 255) / 10000);
                            s = notch_manual.ToString("D");

                            if (s.Length == 3 || s.Length == 4)
                            {
                                string s1 = s.Remove(0, 1);
                                int j = int.Parse(s1, NumberStyles.HexNumber);
                                string s2 = s.Remove(1);
                                int j1 = int.Parse(s2, NumberStyles.HexNumber);
                                answer[6] = (byte)(j1);
                                answer[7] = (byte)(j);
                            }
                            else if (s.Length == 2)
                            {
                                int j = int.Parse(s, NumberStyles.HexNumber);
                                answer[7] = (byte)(j & 0x00ff);
                                answer[6] = (byte)(j >> 8);
                            }

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x14;
                            answer[5] = 0x1a;
                            answer[8] = 0xfd;
                            return answer;

                        default:
                            answer = new byte[6];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfa;               // error
                            answer[5] = 0xfd;
                            return answer;

                    }
                }
                else
                {
                    switch (command[5])
                    {
                        case 1:                                         // AF level
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 100) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AF", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 2:                                         // RF gain
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)(((parm1 - 20) * 140) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 3:                                         // SQL level
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOA", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 6:                                         // NR gain
                            answer = new byte[6];
                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 1000) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NR gain", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 9:                                         //  gain
                            answer = new byte[6];
                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 1000) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NR gain", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 0x0a:                                     // PWR gain
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 100) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "PWR", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 0x0b:                                     // MIC gain
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 70) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MIC", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 0x0d:                                     // ANF state
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 10000) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NOTCH manual", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 0x0e:                                     // COMP level
                            answer = new byte[6];

                            if (command[7] == 0xfd)
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }
                            else
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }

                            parm1 = (int)((parm1 * 20) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "COMP level", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 0x16:                                     // VOX sense
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            parm1 = (int)((parm1 * 1000) / 255);

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VOX Gain", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        case 0x1a:                                     // manual NOTCH frequency
                            answer = new byte[6];

                            if (command[8] == 0xfd)                     // 2 byte value
                            {
                                v = command[7].ToString("X");
                                parm1 = int.Parse(v);
                                parm1 += command[6] * 100;
                            }
                            else if (command[7] == 0xfd)                // 1 byte value
                            {
                                v = command[6].ToString("X");
                                parm1 = int.Parse(v);
                            }

                            if (parm1 > +128)
                                parm1 = (int)((parm1 - 128) * (5000 / 64));
                            else
                                parm1 = (int)(((parm1 - 64) * 5000) / 64) - 5000;

                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NOTCH manual", parm1, parm2, "");

                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfb;
                            answer[5] = 0xfd;
                            return answer;

                        default:
                            answer = new byte[6];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0xfa;               // error
                            answer[5] = 0xfd;
                            return answer;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_15(byte[] command)                        // S meter
        {
            try
            {
                byte[] answer = new byte[6];
                int[] parm2 = new int[1];

                switch (command[5])
                {
                    case 2:                                         // S meter
                        short sm = 0;
                        float num = 0f;

                        if (console.PowerOn)
                            num = DttSP.CalculateRXMeter(0, 0, DttSP.MeterType.AVG_SIGNAL_STRENGTH);

                        num = num + console.MultimeterCalOffset + 140.0f;
                        num = Math.Max(0, num);
                        num = Math.Min(512, num);
                        sm = (short)(num * (512 / 130));

                        if (sm > 0x0100)
                            sm += 0x80;

                        answer = new byte[9];
                        answer[0] = 0xfe;
                        answer[1] = 0xfe;
                        answer[2] = command[3];
                        answer[3] = command[2];
                        answer[4] = 0x15;
                        answer[5] = 0x02;
                        answer[6] = (byte)(sm >> 8);
                        answer[7] = (byte)(sm & 0x00ff);
                        answer[8] = 0xfd;
                        return answer;

                    case 0x11:
                        sm = 0;
                        int pwr = 0;

                        if (console.PowerOn)
                            pwr = 50; // *(int)console.PAPower((int)console.g59.fwd_PWR);

                        answer = new byte[9];
                        answer[0] = 0xfe;
                        answer[1] = 0xfe;
                        answer[2] = command[3];
                        answer[3] = command[2];
                        answer[4] = 0x15;
                        answer[5] = 0x11;
                        answer[6] = (byte)(pwr >> 8);
                        answer[7] = (byte)(pwr & 0x00ff);
                        answer[8] = 0xfd;
                        return answer;

                    case 0x12:
                        sm = 0;
                        int swr = 0;

                        if (console.PowerOn)
                            swr = 50; // *(int)console.PAPower((int)console.g59.Ref_PWR);

                        answer = new byte[9];
                        answer[0] = 0xfe;
                        answer[1] = 0xfe;
                        answer[2] = command[3];
                        answer[3] = command[2];
                        answer[4] = 0x15;
                        answer[5] = 0x12;
                        answer[6] = (byte)(swr >> 8);
                        answer[7] = (byte)(swr & 0x00ff);
                        answer[8] = 0xfd;
                        return answer;

                    default:
                        answer[0] = 0xfe;
                        answer[1] = 0xfe;
                        answer[2] = command[3];
                        answer[3] = command[2];
                        answer[4] = 0xfa;           // error
                        answer[5] = 0xfd;
                        return answer;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_16(byte[] command)                        // many stuff
        {
            try
            {
                byte[] answer = new byte[6];
                int[] parm2 = new int[1];

                switch (command[5])
                {
                    case 0x02:                                      // RF preamp
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x02;

                            answer[6] = 0;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else if (command[6] == 0)
                        {
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF preamp", 0, parm2, "");
                        }
                        else if (command[6] == 1 || command[6] == 2)
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RF preamp", 1, parm2, "");
                        break;

                    case 0x12:                                      // AGC
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x12;

                            if (console.current_agc_mode == AGCMode.FAST)
                                answer[6] = 1;
                            else if (console.current_agc_mode == AGCMode.MED)
                                answer[6] = 2;
                            if (console.current_agc_mode == AGCMode.SLOW)
                                answer[6] = 3;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                        {
                            switch (command[6])
                            {
                                case 1:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AGC mode",
                                        (int)AGCMode.FAST, parm2, "");
                                    break;

                                case 2:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AGC mode",
                                        (int)AGCMode.MED, parm2, "");
                                    break;

                                case 3:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "AGC mode",
                                        (int)AGCMode.SLOW, parm2, "");
                                    break;
                            }
                        }
                        break;

                    case 0x22:                                      // NB
                        {
                            if (command[6] == 0xfd)
                            {
                                answer = new byte[8];
                                answer[0] = 0xfe;
                                answer[1] = 0xfe;
                                answer[2] = command[3];
                                answer[3] = command[2];
                                answer[4] = 0x16;
                                answer[5] = 0x22;

                                if (console.CATNB1 == 1)
                                    answer[6] = 0x01;
                                else
                                    answer[6] = 0x00;

                                answer[7] = 0xfd;
                                return answer;
                            }
                            else if (command[6] == 1)
                            {
                                if (!console.ConsoleClosing)
                                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NB1", 1, parm2, "");
                            }
                            else
                                if (!console.ConsoleClosing)
                                    console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NB1", 0, parm2, "");
                        }
                        break;

                    case 0x40:                                      // NR
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x40;

                            if (console.CATNR == 1)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else if (command[6] == 1)
                        {
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NR", 1, parm2, "");
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NR", 0, parm2, "");
                        break;

                    case 0x41:                                      // ANF
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x41;

                            if (console.CATANF == 1)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else if (command[6] == 1)
                        {
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ANF", 1, parm2, "");
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "ANF", 0, parm2, "");
                        break;

                    case 0x42:                                      // Repeater tone
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x42;

                            if (console.CTCSS)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "RPT tone", command[6], parm2, "");
                        break;

                    case 0x43:                                      // squelch
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x43;

                            if (console.CATSquelch == 1)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "SQL VFOA Enable", command[6], parm2, "");
                        break;

                    case 0x44:                                      // COMP
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x44;

                            if (console.COMP == true)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "COMP", command[6], parm2, "");
                        break;

                    case 0x45:                                      // MON state
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x45;

                            if (console.MON)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MON", command[6], parm2, "");
                        break;

                    case 0x46:                                      // VOX
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x46;

                            if (console.VOXEnable)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VOX", command[6], parm2, "");
                        break;

                    case 0x48:                                      // Manual NOTCH or ANF
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x48;

                            if (console.CATNOTCHenable == 1)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NOTCH manual enable",
                                command[6], parm2, "");
                        break;

                    case 0x50:                                      // dial lock
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x50;

                            if (console.CATVFOLock)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "VFO Lock", command[6], parm2, "");
                        break;

                    case 0x51:                                      // Manual NOTCH2
                        if (command[6] == 0xfd)
                        {
                            answer = new byte[8];
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = command[3];
                            answer[3] = command[2];
                            answer[4] = 0x16;
                            answer[5] = 0x51;

                            if (console.CATNOTCHenable == 1)
                                answer[6] = 0x01;
                            else
                                answer[6] = 0x00;

                            answer[7] = 0xfd;
                            return answer;
                        }
                        else
                            if (!console.ConsoleClosing)
                                console.Invoke(new CATCrossThreadCallback(console.CATCallback), "NOTCH manual enable",
                                command[6], parm2, "");
                        break;


                    default:
                        answer[0] = 0xfe;
                        answer[1] = 0xfe;
                        answer[2] = command[3];
                        answer[3] = command[2];
                        answer[4] = 0xfa;                           // error
                        answer[5] = 0xfd;
                        return answer;
                }

                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0xfb;                                   // ok
                answer[5] = 0xfd;

                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_19(byte[] command)        // ID
        {
            try
            {
                byte[] answer = new byte[7];
                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0x19;
                answer[5] = (byte)console.CATRigAddress;           // IC-7000 adress
                answer[6] = 0xfd;
                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_1B(byte[] command)     // read repeater mode
        {
            try
            {
                byte[] answer = new byte[7];
                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0x19;
                answer[5] = 0x70;
                answer[6] = 0xfd;
                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_1A_00(byte[] command)     // R/W ext memory
        {
            try
            {
                byte[] answer = new byte[7];
                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0x19;
                answer[5] = 0x70;
                answer[6] = 0xfd;
                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_1A_03(byte[] command)     // R/W IF filter
        {
            try
            {
                byte[] answer = new byte[8];
                int[] parm2 = new int[1];

                answer[0] = 0xfe;
                answer[1] = 0xfe;
                answer[2] = command[3];
                answer[3] = command[2];
                answer[4] = 0x1a;
                answer[5] = 0x03;

                if (command[6] == 0xfd)
                {
                    switch (console.CurrentDSPMode)
                    {
                        case DSPMode.CWL:
                        case DSPMode.CWU:
                            switch (console.CurrentFilter)
                            {
                                case Filter.F1:
                                    answer[6] = 14;
                                    break;

                                case Filter.F2:
                                    answer[6] = 12;
                                    break;

                                case Filter.F3:
                                    answer[6] = 11;
                                    break;

                                case Filter.F4:
                                    answer[6] = 10;
                                    break;

                                case Filter.F5:
                                    answer[6] = 9;
                                    break;

                                case Filter.F6:
                                    answer[6] = 7;
                                    break;

                                case Filter.F7:
                                    answer[6] = 4;
                                    break;

                                case Filter.F8:
                                    answer[6] = 1;
                                    break;

                                case Filter.F9:
                                    answer[6] = 0;
                                    break;

                                default:
                                    answer[6] = 0;
                                    break;
                            }
                            break;

                        case DSPMode.USB:
                        case DSPMode.LSB:
                            switch (console.CurrentFilter)
                            {
                                case Filter.F1:
                                    answer[6] = 0;
                                    break;

                                case Filter.F2:
                                    answer[6] = 0;
                                    break;

                                case Filter.F3:
                                    answer[6] = 58;
                                    break;

                                case Filter.F4:
                                    answer[6] = 55;
                                    break;

                                case Filter.F5:
                                    answer[6] = 45;
                                    break;

                                case Filter.F6:
                                    answer[6] = 43;
                                    break;

                                case Filter.F7:
                                    answer[6] = 40;
                                    break;

                                case Filter.F8:
                                    answer[6] = 37;
                                    break;

                                case Filter.F9:
                                    answer[6] = 34;
                                    break;

                                case Filter.F10:
                                    answer[6] = 14;
                                    break;

                                default:
                                    answer[6] = 0;
                                    break;
                            }
                            break;

                        default:

                            break;
                    }

                    answer[7] = 0xfd;
                    return answer;
                }
                else
                {
                    switch (console.CurrentDSPMode)
                    {
                        case DSPMode.CWL:
                        case DSPMode.CWU:
                            switch (command[6])
                            {
                                case 0x14:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F1, parm2, "");
                                    break;

                                case 0x12:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F2, parm2, "");
                                    break;

                                case 0x11:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F3, parm2, "");
                                    break;

                                case 0x10:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F4, parm2, "");
                                    break;

                                case 9:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F5, parm2, "");
                                    break;

                                case 7:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F6, parm2, "");
                                    break;

                                case 4:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F7, parm2, "");
                                    break;

                                case 1:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F8, parm2, "");
                                    break;

                                case 0:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F9, parm2, "");
                                    break;

                                default:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F1, parm2, "");
                                    break;
                            }
                            break;

                        case DSPMode.USB:
                        case DSPMode.LSB:
                            switch (command[6])
                            {
                                case 0x40:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F3, parm2, "");
                                    break;

                                case 0x37:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F4, parm2, "");
                                    break;

                                case 0x33:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F5, parm2, "");
                                    break;

                                case 0x31:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F6, parm2, "");
                                    break;

                                case 0x28:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F7, parm2, "");
                                    break;

                                case 0x25:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F8, parm2, "");
                                    break;

                                case 0x22:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F9, parm2, "");
                                    break;

                                case 0x14:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F10, parm2, "");
                                    break;

                                default:
                                    if (!console.ConsoleClosing)
                                        console.Invoke(new CATCrossThreadCallback(console.CATCallback), "Filter", Filter.F1, parm2, "");
                                    break;
                            }
                            break;

                        default:
                            break;
                    }

                    answer = new byte[6];
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0xfb;
                    answer[5] = 0xfd;
                    return answer;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_1C_00(byte[] command)     // PTT
        {
            try
            {
                byte[] answer = new byte[6];
                int[] parm2 = new int[1];

                if (command[6] == 0xfd)
                {
                    answer = new byte[8];
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0x1c;
                    answer[5] = 0x00;

                    if (console.MOX)
                        answer[6] = 0x01;
                    else
                        answer[6] = 0x00;

                    answer[7] = 0xfd;
                }
                else
                {
                    if (command[6] == 0x00)
                    {
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 0, parm2, "");
                    }
                    else if (command[6] == 0x01)
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "MOX", 1, parm2, "");

                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0xfb;           // ok
                    answer[5] = 0xfd;
                }

                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        public byte[] CMD_1C_01(byte[] command)     // Tuner on/off
        {
            try
            {
                byte[] answer = new byte[8];
                int[] parm2 = new int[1];

                if (command[6] == 0xfd)
                {
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0x1c;
                    answer[5] = 0x00;

                    if (console.TUN)
                        answer[6] = 0x01;
                    else
                        answer[6] = 0x00;

                    answer[7] = 0xfd;
                }
                else
                {
                    if (command[6] == 0x00)
                    {
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TUN Enable", 0, parm2, "");
                    }
                    else if (command[6] == 0x01 || command[6] == 0x02)
                        if (!console.ConsoleClosing)
                            console.Invoke(new CATCrossThreadCallback(console.CATCallback), "TUN Enable", 1, parm2, "");

                    answer = new byte[6];
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = command[3];
                    answer[3] = command[2];
                    answer[4] = 0xfb;           // ok
                    answer[5] = 0xfd;
                }

                return answer;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return command;
            }
        }

        #endregion
    }
}

