//=================================================================
// SerialPortPTT.cs
//=================================================================
// Copyright (C) 2005  Bill Tracey
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
//=================================================================
// This class is used to implement a PTT using RTS or DTS 
//=================================================================

#define DBG_PRINT

using System;
using System.Windows.Forms;
using SDRSerialSupportII;
using System.IO.Ports; 

namespace PowerSDR
{
	/// <summary>
	/// Summary description for SerialPortPTT.
	/// </summary>
	public class SerialPortPTT
	{

		// instance vars 
		// 
		private string portName = null;

        private bool rtsIsPTT = false;
        public bool RTSIsPTT
        {
            get { return rtsIsPTT; }
            set { rtsIsPTT = value; }
        }

		private bool dtrIsPTT = false; 
		public bool DTRIsPTT 
		{
			get {return dtrIsPTT;}
			set {dtrIsPTT = value;}
		}
		private bool Initialized = false;

        public SerialPort sp = new SerialPort();

		//
		// 
		//

		public SerialPortPTT(string port_name, bool dtr_is_ptt, bool rts_is_ptt)
		{
			portName = port_name;  
			dtrIsPTT = dtr_is_ptt;
            rtsIsPTT = rts_is_ptt;
		}

        public void enable_tx(bool tx)
        {
            if (sp.IsOpen)
                sp.RtsEnable = tx;
        }

		public void Init() 
		{ 
			lock ( this )  // do this only once -- keep the lock until we're ready to go less we hose up the poll ptt thread 
			{
                if (Initialized) return;

                if (sp.IsOpen) sp.Close();

                try
                {
                    sp.PortName = portName;
                    sp.Open();
                    sp.DtrEnable = dtrIsPTT;
                    Initialized = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not open COM port!");
                }
			}
			return; 
		}

		public bool isPTT() 
		{
            bool dsr = false;
            bool cts = false;
            if (sp.IsOpen)
            {
                dsr = System.Convert.ToBoolean(sp.DsrHolding);
                cts = System.Convert.ToBoolean(sp.CtsHolding);
            }
			if ( !Initialized ) return false;
            if (dtrIsPTT && dsr || rtsIsPTT && cts) return true;

			return false; 
		}

        public void Close()
        {
            if (sp.IsOpen) sp.Close();
        }
	}
}