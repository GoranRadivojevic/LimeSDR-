/* 	This file is part of a program that implements a Software-Defined Radio.
	The code in this file is derived from routines originally written by
	Pierre-Philippe Coupard for his CWirc X-chat program. That program
	is issued under the GPL and is
	Copyright (C) Pierre-Philippe Coupard - 18/06/2003
	This derived version is
	Copyright (C) 2004, 2005, 2006 by Frank Brickle, AB2KT and Bob McGwier, N4HY

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

	The authors can be reached by email at

	ab2kt@arrl.net
	or
	rwmcgwier@comcast.net

	or by paper mail at

	The DTTS Microwave Society
	6 Kathleen Place
	Bridgewater, NJ 08807
*/

/*
 *  Changes for GenesisRadio
 *  Copyright (C)2008-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.Ports;

namespace PowerSDR
{

    public class CWKeyer2
    {
        #region Variables and Properties

        public Thread Keyer, CWTone, Monitor;
        private bool primary_comm_port_clossing = true;
        private bool secondary_comm_port_clossing = true;
        private bool CW_monitor_off = false;
        private bool CW_monitor_on = false;
        private int keyermode = 0;
        public bool primary_keyer_dash = false, primary_keyer_dot = false;
        public bool secondary_keyer_dash = false, secondary_keyer_dot = false;
        private bool keyprog = false;
        public bool CWMonitorState = false;
        public bool debug = false;
        private delegate void DebugCallbackFunction(string name);

        private bool ptt_bit_bang_enabled;
        public bool PTTBitBangEnabled                       // yt7pwr
        {
            set { ptt_bit_bang_enabled = value; }
        }

        private bool runKeyer = true;                      // yt7pwr
        public bool RunKeyer
        {
            set 
            {
                runKeyer = value;

                if (runKeyer && console.chkPower.Checked)
                {
                    StartKeyerThread();
                }
                else if (!value)
                    DttSP.PollTimerRelease();
            }
        }

        private bool secondary_keyer_mox = false;           // yt7pwr
        public bool SecondaryKeyerMox
        {
            set { secondary_keyer_mox = value; }
        }

        private bool primary_keyer_mox = false;             // yt7pwr
        public bool PrimaryKeyerMox
        {
            set { primary_keyer_mox = value; }
        }

        private bool cw_monitor_enabled = false;            // yt7pwr
        public bool CWMonitorEnabled
        {
            set { cw_monitor_enabled = value; }
        }

        private bool enabled_primary_keyer = false;            // yt7pwr
        public bool PrimaryKeyer
        {
            set { enabled_primary_keyer = value; }
        }

        private bool enabled_secondary_keyer = false;            // yt7pwr
        public bool SecondaryKeyer
        {
            set { enabled_secondary_keyer = value; }
        }

        private bool tuneCW = false;            // yt7pwr
        public bool TuneCW
        {
            set { tuneCW = value; }
        }

        private bool dtr_cw_monitor = false;     // yt7pwr
        public bool DTRCWMonitor
        {
            set { dtr_cw_monitor = value; }
        }

        public int KeyerMode
        {
            get { return keyermode; }
            set
            {
                keyermode = value;
                DttSP.SetKeyerMode(keyermode);
            }
        }
        private HiPerfTimer timer;
        private float msdel;
        private Console console;

        public bool QRP2000CW1 = true;
        public bool QRP2000CW2 = false;

        private SecondaryPTTKeyerLine secondary_ptt_line = SecondaryPTTKeyerLine.NONE;
        public SecondaryPTTKeyerLine SecondaryPTTLine
        {
            get { return secondary_ptt_line; }
            set
            {
                secondary_ptt_line = value;
            }
        }

        private KeyerLine secondary_dot_line = KeyerLine.NONE;
        public KeyerLine SecondaryDOTLine
        {
            get { return secondary_dot_line; }
            set
            {
                secondary_dot_line = value;
            }
        }

        private KeyerLine secondary_dash_line = KeyerLine.NONE;
        public KeyerLine SecondaryDASHLine
        {
            get { return secondary_dash_line; }
            set
            {
                secondary_dash_line = value;
            }
        }

        private SIOListenerII siolisten;
        public SIOListenerII Siolisten
        {
            get { return siolisten; }
            set
            {
                siolisten = value;
            }
        }
        private bool cat_enabled = false;
        public bool CATEnabled
        {
            set { cat_enabled = value; }
        }

        private KeyerLine secondary_key_line = KeyerLine.NONE;
        public KeyerLine SecondaryKeyLine // changes yt7pwr
        {
            get { return secondary_key_line; }
            set
            {
                secondary_key_line = value;
                switch (secondary_key_line)
                {
                    case KeyerLine.NONE:
                        break;
                    case KeyerLine.DSR:
                        DttSP.SetWhichKey(1);
                        SP2DotKey = true;
                        break;
                    case KeyerLine.CTS:
                        SP2DotKey = false;
                        DttSP.SetWhichKey(0);
                        break;
                }
            }
        }

        private bool memorykey = false;
        public bool MemoryKey
        {
            get { return memorykey; }
            set { memorykey = value; }
        }

        private bool memoryptt = false;
        public bool MemoryPTT
        {
            get { return memoryptt; }
            set
            {
                memoryptt = value;
            }
        }

        private string primary_conn_port = "None";
        public string PrimaryConnPort  // changes yt7pwr
        {
            get { return primary_conn_port; }
            set
            {
                try
                {
                    primary_conn_port = value;

                    switch (primary_conn_port)
                    {
                        case "None":
                            primary_comm_port_clossing = true;
                            Thread.Sleep(10);
                            if (sp.IsOpen) sp.Close();
                            break;
                        case "USB":
                        case "QRP2000":
                            primary_comm_port_clossing = true;
                            Thread.Sleep(10);
                            if (sp.IsOpen) sp.Close();
                            break;
                        case "NET":
                            primary_comm_port_clossing = true;
                            Thread.Sleep(10);
                            if (sp.IsOpen) sp.Close();
                            break;
                        default:
                            primary_comm_port_clossing = true;
                            if (sp.IsOpen) sp.Close();
                            sp.PortName = primary_conn_port;

                            try
                            {
                                sp.Open();
                                if (!console.DTRCWMonitor)
                                    sp.DtrEnable = true;        // default
                                else
                                    sp.DtrEnable = false;       // require the PCB alteration!

                                if (sp.IsOpen)
                                    primary_comm_port_clossing = false;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Primary Keyer Port [" + primary_conn_port + "] could not be opened.");
                                primary_conn_port = "USB";
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }
        }

        private string secondary_conn_port = "None";
        public string SecondaryConnPort // changes yt7pwr
        {
            get { return secondary_conn_port; }
            set
            {
                try
                {
                    secondary_conn_port = value;
                    switch (secondary_conn_port)
                    {
                        case "None":
                            secondary_comm_port_clossing = true;
                            Thread.Sleep(10);
                            if (sp2.IsOpen) sp2.Close();
                            enabled_secondary_keyer = false;
                            break;
                        default: // COMx
                            secondary_comm_port_clossing = true;
                            Thread.Sleep(10);
                            if (sp2.IsOpen) sp2.Close();
                            sp2.PortName = secondary_conn_port;

                            try
                            {
                                sp2.Open();
                                sp2.DtrEnable = true;
                                sp2.RtsEnable = true;
                                if (sp2.IsOpen)
                                {
                                    secondary_comm_port_clossing = false;
                                    enabled_secondary_keyer = true;
                                }
                                else
                                {
                                    secondary_comm_port_clossing = true;
                                    enabled_secondary_keyer = false;
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("COM port [" + secondary_conn_port + "] for secondary keyer could not be opened\n");
                                secondary_conn_port = "None";
                                enabled_secondary_keyer = false;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }
        }

        private bool sp2dotkey = false;
        public bool SP2DotKey
        {
            set { sp2dotkey = value; }
            get { return sp2dotkey; }
        }

        private bool keyerptt = false;
        public bool KeyerPTT
        {
            get
            {
                isPTT();
                return keyerptt;
            }
        }

        public SerialPort sp = new SerialPort();
        public SerialPort sp2 = new SerialPort();

        #endregion

        #region Constructor and Destructor

        unsafe public CWKeyer2(Console c)
        {
            console = c;
            siolisten = console.Siolisten;
            Thread.Sleep(50);
            DttSP.NewKeyer(600.0f, true, 0.0f, 3.0f, 25.0f, (float)Audio.RXSampleRate);
            DttSP.SetKeyerMode(0);
            Thread.Sleep(50);


            CWTone = new Thread(new ThreadStart(DttSP.KeyerSoundThread));
            CWTone.Name = "CW Sound Thread";
            CWTone.Priority = ThreadPriority.Highest;
            CWTone.IsBackground = true;
            CWTone.Start();

            Monitor = new Thread(new ThreadStart(DttSP.KeyerMonitorThread));
            Monitor.Name = "CW Monitor Thread";
            Monitor.Priority = ThreadPriority.Highest;
            Monitor.IsBackground = true;
            Monitor.Start();

            Thread.Sleep(100);

            timer = new HiPerfTimer();
        }

        ~CWKeyer2()
        {
            // Destructor logic here, make sure threads cleaned up
            DttSP.StopKeyer();
            Thread.Sleep(10);
            if (CWTone != null && CWTone != null)
                CWTone.Abort();
            if (Monitor != null && Monitor != null)
                Monitor.Abort();
            if (Keyer != null && Keyer.IsAlive)
                Keyer.Abort();
            Thread.Sleep(50);
            DttSP.DeleteKeyer();
        }

        public void StartKeyerThread()
        {
            Keyer = null;
            Keyer = new Thread(new ThreadStart(KeyThread));
            Keyer.Name = "CW KeyThread";
            Keyer.Priority = ThreadPriority.Highest;
            Keyer.IsBackground = true;
            Keyer.Start();
        }

        #endregion

        #region Thread Functions

        unsafe private void KeyThread() // changes yt7pwr
        {
            try
            {
                int[] tmp = new int[1];
                bool tune_CW = false;

                //do
                {
                    DttSP.KeyerStartedWait();

                    while (runKeyer)
                    {
                        timer.Start();
                        DttSP.PollTimerWait();
                        CWMonitorState = DttSP.KeyerPlaying();

                        if (tuneCW && !tune_CW)
                        {
                            tune_CW = true;
                            secondary_keyer_dash = false;
                            secondary_keyer_dot = true;
                        }
                        else if (!tuneCW && tune_CW)
                        {
                            tune_CW = false;
                            secondary_keyer_dash = false;
                            secondary_keyer_dot = false;
                        }
                        else if (memoryptt)
                        {
                            //console ptt on
                            keyprog = true;
                            secondary_keyer_dot = secondary_keyer_dash = memorykey;

                            if (console.CWMonitorEnabled)
                            {
                                if (console.CurrentModel == Model.LimeSDR)
                                {

                                }
                            }
                        }
                        else if (!tune_CW)
                        {
                            secondary_keyer_dash = false;
                            secondary_keyer_dot = false;
                            keyprog = false;
                        }

                        if (enabled_secondary_keyer && (secondary_keyer_mox || ptt_bit_bang_enabled))
                        {
                            keyprog = false;

                            switch (secondary_conn_port)
                            {
                                case "None":
                                    break;
                                default: // comm port
                                    if (sp2.IsOpen)
                                    {
                                        switch (secondary_dot_line)
                                        {
                                            case KeyerLine.DSR:
                                                secondary_keyer_dot = sp2.DsrHolding;
                                                break;
                                            case KeyerLine.CTS:
                                                secondary_keyer_dot = sp2.CtsHolding;
                                                break;
                                            case KeyerLine.DCD:
                                                secondary_keyer_dot = sp2.CDHolding;
                                                break;
                                            case KeyerLine.NONE:
                                                secondary_keyer_dot = false;
                                                break;
                                        }
                                        switch (secondary_dash_line)
                                        {
                                            case KeyerLine.DSR:
                                                secondary_keyer_dash = sp2.DsrHolding;
                                                break;
                                            case KeyerLine.CTS:
                                                secondary_keyer_dash = sp2.CtsHolding;
                                                break;
                                            case KeyerLine.DCD:
                                                secondary_keyer_dash = sp2.CDHolding;
                                                break;
                                            case KeyerLine.NONE:
                                                secondary_keyer_dash = false;
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }

                        if (enabled_primary_keyer && !secondary_keyer_mox && !memoryptt)
                        {
                            switch (primary_conn_port)
                            {
                                case "USB":
                                    {
                                        switch (console.CurrentModel)
                                        {
                                            case Model.LimeSDR:
                                                {
                                                    if (console.limeSDR.device != null && console.limeSDR.connected)
                                                    {
                                                        uint key = console.limeSDR.device.ReadKeyer();
                                                        key = key & 0xc0;
                                                        //Debug.Write("Keyer: " + key.ToString() + " \n");

                                                        if (key == 0x40)
                                                        {
                                                            primary_keyer_dash = true;
                                                            primary_keyer_dot = false;
                                                        }
                                                        else if (key == 0x80)
                                                        {
                                                            primary_keyer_dot = true;
                                                            primary_keyer_dash = false;
                                                        }
                                                        else
                                                        {
                                                            primary_keyer_dot = false;
                                                            primary_keyer_dash = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        primary_keyer_dot = false;
                                                        primary_keyer_dash = false;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    if (sp.IsOpen)
                                    {
                                        keyprog = false;

                                        keyprog = false;
                                        primary_keyer_dash = sp.CtsHolding;
                                        primary_keyer_dot = sp.DsrHolding;

                                        if (dtr_cw_monitor && console.CWMonitorEnabled)
                                        {
                                            if (CWMonitorState)
                                                CW_monitor(true);
                                            else
                                                CW_monitor(false);
                                        }
                                    }
                                    break;
                            }
                        }

                        timer.Stop();
                        msdel = (float)timer.DurationMsec;
                        //msdel = (float)DttSP.TimerRead();
                        //Debug.Write(msdel.ToString() + "\n");

                        if (keyprog || secondary_keyer_mox || tune_CW || ptt_bit_bang_enabled)
                        {
                            //keyprog = false;
                            DttSP.KeyValue(msdel, secondary_keyer_dash, secondary_keyer_dot, keyprog);
                            keyprog = false;
                        }
                        else if (primary_keyer_mox)
                        {
                            DttSP.KeyValue(msdel, primary_keyer_dash, primary_keyer_dot, keyprog);
                        }
                        else if (enabled_primary_keyer && !secondary_keyer_mox && !memoryptt)
                        {
                            DttSP.KeyValue(msdel, primary_keyer_dash, primary_keyer_dot, keyprog);
                        }
                        else
                        {
                            DttSP.KeyValue(msdel, false, false, false);
                            primary_keyer_mox = false;
                            secondary_keyer_mox = false;
                            keyprog = false;
                        }
                    }
                }// while (true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in KeyThread!\n" + ex.ToString());
            }
        }

        #endregion

        public void enable_tx(bool tx) // yt7pwr
        {
            if (sp.IsOpen)
                sp.RtsEnable = tx;
        }

        public bool isPTT()  // yt7pwr
        {
            bool dsr = false,cts = false;

            if (!primary_comm_port_clossing)
            {
                if (sp.IsOpen)                   // primary COM port
                {
                    dsr = System.Convert.ToBoolean(sp.DsrHolding);
                    cts = System.Convert.ToBoolean(sp.CtsHolding);

                    if (dsr || cts)
                    {
                        keyerptt = true;
                        return true;
                    }
                    else
                    {
                        keyerptt = false;
                    }
                }
            }

            if (sp2.IsOpen)
            {
                switch (secondary_ptt_line)     // secondary COM port
                {
                    case SecondaryPTTKeyerLine.DTR:
                        dsr = System.Convert.ToBoolean(sp2.DsrHolding);
                        break;
                    case SecondaryPTTKeyerLine.DCD:
                        dsr = System.Convert.ToBoolean(sp2.CDHolding);
                        break;
                    case SecondaryPTTKeyerLine.CTS:
                        dsr = System.Convert.ToBoolean(sp2.CtsHolding);
                        break;
                }

                if (dsr)
                {
                    secondary_keyer_mox = true;
                    keyerptt = true;
                    return true;
                }
                else
                {
                    keyerptt = false;
                    secondary_keyer_mox = false;
                }
            }

            return false;
        }

        private void CW_monitor(bool state)      // yt7pwr
        {
            if (dtr_cw_monitor)
            {
                if (sp.IsOpen)
                    sp.DtrEnable = state;
            }
        }
    }
}
