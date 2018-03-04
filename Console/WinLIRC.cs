//=================================================================
//              WinLIRC remote controller class
//=================================================================
//
// Copyright (C)2012 YT7PWR Goran Radivojevic
// contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

namespace PowerSDR
{
    public class WinLIRC
    {
        #region variable

        string WinLIRC_addr = "127.0.0.1";
        int WinLIRC_port = 8765;
        Socket CliendSocket;
        public bool debug = false;
        delegate void DebugCallbackFunction(string name);
        delegate void CrossThreadCallback(string type, int parm1, int[] parm2, string parm3);
        public Console console = null;
        bool run_client = false;
        bool IPv6_enabled = false;
        byte[] receive_buffer = new byte[64];
        private AutoResetEvent connect_event = new AutoResetEvent(false);

        #endregion

        #region constructor

        public WinLIRC(Console c)
        {
            console = c;
        }

        ~WinLIRC()
        {

        }

        #endregion

        #region Misc function

        public void Start(string addr, int port)
        {
            try
            {
                WinLIRC_addr = addr;
                WinLIRC_port = port;
                run_client = true;
                SetupSocket();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                run_client = false;

                if (CliendSocket != null)
                {
                    CliendSocket.Shutdown(SocketShutdown.Both);
                    CliendSocket.Close();
                }

                Debug.Write("Disconnected! \n");
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private bool SetupSocket()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(WinLIRC_addr);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint ipepServer = new IPEndPoint(ipAddress, WinLIRC_port);

                switch (console.WinVer)
                {
                    case WindowsVersion.Windows2000:
                    case WindowsVersion.WindowsXP:
                        {
                            CliendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            CliendSocket.Blocking = false;
                            CliendSocket.BeginConnect(WinLIRC_addr, WinLIRC_port, new AsyncCallback(ConnectCallback), CliendSocket);
                        }
                        break;

                    case WindowsVersion.WindowsVista:
                    case WindowsVersion.Windows7:
                    case WindowsVersion.Windows8:
                        {
                            if (IPv6_enabled && ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                CliendSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                                CliendSocket.Blocking = false;
                                CliendSocket.BeginConnect(ipepServer, new AsyncCallback(ConnectCallback), CliendSocket);
                            }
                            else
                            {
                                CliendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                CliendSocket.Blocking = false;
                                CliendSocket.BeginConnect(WinLIRC_addr, WinLIRC_port, new AsyncCallback(ConnectCallback), CliendSocket);
                            }
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                Socket sock = (Socket)result.AsyncState;

                if (sock.Connected)
                {
                    if (console.SetupForm != null)
                        console.SetupForm.txtWinLIRCaddress.ForeColor = Color.Green;

                    Debug.Write("Connected!\n");

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Connected to WinLIRC!");

                    sock.BeginReceive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), sock);

                }
                else
                {
                    if (console.SetupForm != null)
                        console.SetupForm.txtWinLIRCaddress.ForeColor = Color.Red;

                    if (run_client)
                        SetupSocket();
                }
            }
            catch (Exception ex)
            {
                connect_event.Set();
                Debug.Write(ex.Message);

                if (console.SetupForm != null)
                    console.SetupForm.txtWinLIRCaddress.ForeColor = Color.Red;
            }
        }

        void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                Debug.Write("Received: \n");

                Socket sock = (Socket)result.AsyncState;
                int num_read = 0;

                if (sock.Connected)
                    num_read = sock.EndReceive(result);

                if (num_read > 0)
                {
                    ProcessData(receive_buffer);

                    for (int i = 0; i < receive_buffer.Length; i++)
                        receive_buffer[i] = 0;

                    sock.BeginReceive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), sock);
                }
                else
                {
                    if (console.SetupForm != null)
                        console.SetupForm.txtWinLIRCaddress.ForeColor = Color.Red;

                    Debug.Write("Disconnected!\n");

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback), "Disconnected!");

                    if (CliendSocket != null)
                    {
                        CliendSocket.Shutdown(SocketShutdown.Both);
                        CliendSocket.Close();
                    }

                    if (run_client)
                        SetupSocket();
                }
            }
            catch (SocketException socketException)
            {
                if (socketException.ErrorCode == 10054)
                {
                    CliendSocket.Close(1000);
                }

                if (run_client)
                    SetupSocket();

            }
            catch (ObjectDisposedException)
            {
                CliendSocket.Close(1000);

                if (run_client)
                    SetupSocket();
            }
        }

        private void ProcessData(byte[] data)                       // answer received from server
        {
            try
            {
                int[] parm2 = new int[1];
                ASCIIEncoding buffer = new ASCIIEncoding();
                string command_type = buffer.GetString(data, 0, data.Length);
                Debug.Write(command_type + "\n");
                string[] vals = command_type.Split(' ');

                if (vals.Length > 3)
                {
                    switch (vals[2])
                    {
                        case "Restore":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Restore", 0, parm2, "");
                            break;

                        case "Freq_up":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "VFOA step up", 2, parm2, "");
                            break;

                        case "Freq_down":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "VFOA step down", 2, parm2, "");
                            break;

                        case "Step_down":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "StepSize VFOA down", 1, parm2, "");
                            break;

                        case "Step_up":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "StepSize VFOA up", 1, parm2, "");
                            break;

                        case "Band_up":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Band up", 1, parm2, "");
                            break;

                        case "Band_down":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Band down", 1, parm2, "");
                            break;

                        case "Vol+":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "AF+", 1, parm2, "");
                            break;

                        case "Vol-":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "AF-", 1, parm2, "");
                            break;

                        case "Mute":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "AF_mute", 0, parm2, "");
                            break;

                        case "Power":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Power toggle", 1, parm2, "");
                            break;

                        case "Mem+":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Memory up", 0, parm2, "");
                            break;

                        case "Mem-":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Memory down", 0, parm2, "");
                            break;

                        case "MemRecall":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Memory recall", 0, parm2, "");
                            break;

                        case "MemClear":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Mmeory clear", 0, parm2, "");
                            break;

                        case "MemStore":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Memory store", 0, parm2, "");
                            break;

                        case "Return":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "Restore VFOA", 0, parm2, "");
                            break;

                        case "Exit":
                            console.Invoke(new CrossThreadCallback(console.CATCallback), "CLOSE", 0, parm2, "");
                            break;
                    }
                }

                if (debug && !console.ConsoleClosing)
                    console.Invoke(new DebugCallbackFunction(console.DebugCallback), "WinLIRC command: " + command_type);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}
