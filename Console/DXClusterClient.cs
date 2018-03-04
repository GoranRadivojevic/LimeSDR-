//=================================================================
// DXClusterClient.cs
//=================================================================
// Copyright (C) 2011-2013 YT7PWR
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace PowerSDR
{
    public partial class DXClusterClient : Form
    {
        #region DLL imports

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

        #endregion

        #region variable

        Console MainWindow;
        const int WM_VSCROLL = 0x115;
        const int SB_BOTTOM = 7;
        TelnetClient client;
        public string CALL = "";
        public string NAME = "";
        public string QTH = "";
        public string CMD1 = "";
        public string CMD2 = "";
        public string CMD3 = "";
        public string CMD4 = "";
        public ClusterSetup ClusterSetupForm;
        public bool closing = false;

        #endregion

        #region constructor/destructor

        public DXClusterClient(Console ptr, string call, string name, string qth)
        {
            MainWindow = ptr;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = (float)(8.25 / ratio);
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            this.PerformAutoScale();
            this.PerformLayout();

            client = new TelnetClient(this);
            CALL = call;
            NAME = name;
            QTH = qth;
            GetOptions();
            ClusterSetupForm = new ClusterSetup(this);

            if (comboDXCluster.Items.Count > 0)
                comboDXCluster.SelectedIndex = 0;

            CMD1 = ClusterSetupForm.btn1cmd.Text.ToString();
            CMD2 = ClusterSetupForm.btn2cmd.Text.ToString();
            CMD3 = ClusterSetupForm.btn3cmd.Text.ToString();
            CMD4 = ClusterSetupForm.btn4cmd.Text.ToString();
        }

        void ClosingForm(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (client.ClusterClient.Client != null && client.ClusterClient.Client.Connected)
                {
                    client.SendMessage(1, "BYE");
                    e.Cancel = true;
                }
                else if (client.ClusterClient.Client != null)
                {
                    closing = true;
                    client.ClusterClient.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region button events

        private void comboDXCluster_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (client.ClusterClient != null && client.ClusterClient.Connected)
                client.SendMessage(1, "BYE");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (client.ClusterClient == null)
            {
                client.Start(comboDXCluster.Text.ToString());
            }
            else if (client.ClusterClient != null && !client.ClusterClient.Connected)
                client.Start(comboDXCluster.Text.ToString());
        }

        private void btnBye_Click(object sender, EventArgs e)
        {
            if (client.ClusterClient != null && client.ClusterClient.Connected)
                client.SendMessage(1, "BYE");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (ClusterSetupForm != null && !ClusterSetupForm.IsDisposed)
                ClusterSetupForm.Show();
            else
            {
                ClusterSetupForm = new ClusterSetup(this);
                ClusterSetupForm.Show();
            }
        }

        private void btnNoDX_Click(object sender, EventArgs e)
        {
            if (client.ClusterClient != null && client.ClusterClient.Connected)
                client.SendMessage(0, CMD1);
        }

        private void btnShowDX_Click(object sender, EventArgs e)
        {
            if (client.ClusterClient != null && client.ClusterClient.Connected)
                client.SendMessage(0, CMD2);
        }

        private void btnNoVHF_Click(object sender, EventArgs e)
        {
            if (client.ClusterClient != null && client.ClusterClient.Connected)
                client.SendMessage(0, CMD3);
        }

        private void btnVHFandUP_Click(object sender, EventArgs e)      // button 4
        {
            if (client.ClusterClient != null && client.ClusterClient.Connected)
                client.SendMessage(0, CMD4);
        }

        private void btnClearTxt_Click(object sender, EventArgs e)
        {
            rtbDXClusterText.Clear();
            rtbDXClusterText.Refresh();
        }

        #endregion

        #region CrossThread

        public void CrossThreadCommand(int command, byte[] data, int count)
        {
            try
            {
                switch (command)
                {
                    case 0:
                        {                                                   // regular text
                            ASCIIEncoding buffer = new ASCIIEncoding();
                            string text = buffer.GetString(data, 0, count);
                            text = text.Replace('\a', ' ');
                            text = text.Replace('\r', ' ');

                            rtbDXClusterText.AppendText(text);
                            SendMessage(rtbDXClusterText.Handle, WM_VSCROLL, SB_BOTTOM, 0);

                            if (text.StartsWith("login:") || text.EndsWith("login: "))
                            {
                                client.SendMessage(0, CALL);

                                if (NAME != "")
                                    client.SendMessage(0, "set/name " + NAME);

                                if (QTH != "")
                                    client.SendMessage(0, "set/QTH " + QTH);
                            }
                            else if (text.Contains("enter your call"))
                            {
                                client.SendMessage(0, CALL);

                                if (NAME != "")
                                    client.SendMessage(0, "set/station/name " + NAME);
                            }
                        }
                        break;

                    case 1:
                        {                                                   // screen caption
                            ASCIIEncoding buffer = new ASCIIEncoding();
                            string text = buffer.GetString(data, 0, count);
                            this.Text = text;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region Save/Restore settings

        public void SaveOptions()
        {
            try
            {
                ArrayList a = new ArrayList();

                int items = comboDXCluster.Items.Count;

                for (int i = 0; i < items; i++)
                {
                    comboDXCluster.SelectedIndex = i;
                    a.Add("Cluster" + i.ToString() + "/" + comboDXCluster.Text.ToString());
                }

                a.Add("cluster_top/" + this.Top.ToString());		// save form positions
                a.Add("cluster_left/" + this.Left.ToString());
                a.Add("cluster_width/" + this.Width.ToString());
                a.Add("cluster_height/" + this.Height.ToString());

                a.Add("btn1text/" + btnNoDX.Text.ToString());
                a.Add("btn2text/" + btnShowDX.Text.ToString());
                a.Add("btn3text/" + btnNoVHF.Text.ToString());
                a.Add("btn4text/" + btnVHFandUP.Text.ToString());
                a.Add("btn1cmd/" + CMD1);
                a.Add("btn2cmd/" + CMD2);
                a.Add("btn3cmd/" + CMD3);
                a.Add("btn4cmd/" + CMD4);

                DB.SaveVars("DXClusterOptions", ref a);		    // save the values to the DB
                DB.Update();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in DXCluster SaveOptions function!\n" + ex.ToString());
            }
        }

        public void GetOptions()
        {
            try
            {
                ArrayList a = DB.GetVars("DXClusterOptions");
                a.Sort();

                foreach (string s in a)
                {
                    string[] vals = s.Split('/');
                    string name = vals[0];
                    string val = vals[1];

                    if (s.StartsWith("cluster_top"))
                    {
                        int top = Int32.Parse(vals[1]);
                        this.Top = top;
                    }
                    else if (s.StartsWith("cluster_left"))
                    {
                        int left = Int32.Parse(vals[1]);
                        this.Left = left;
                    }
                    else if (s.StartsWith("cluster_width"))
                    {
                        int width = Int32.Parse(vals[1]);
                        this.Width = width;
                    }
                    else if (s.StartsWith("cluster_height"))
                    {
                        int height = Int32.Parse(vals[1]);
                        this.Height = height;
                    }
                    else if (s.StartsWith("Cluster"))
                    {
                        comboDXCluster.Items.Add(val);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void UpdateHostsList(string[] vals)
        {
            try
            {
                comboDXCluster.Items.Clear();

                foreach (string b in vals)
                {
                    if (b != "")
                        comboDXCluster.Items.Add(b);
                }

                if (comboDXCluster.Items.Count > 0)
                {
                    comboDXCluster.SelectedIndex = 0;
                    SaveOptions();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public void UpdateButtonsText()
        {
            try
            {
                btnNoDX.Text = ClusterSetupForm.txtButton1.Text;
                btnShowDX.Text = ClusterSetupForm.txtButton2.Text;
                btnNoVHF.Text = ClusterSetupForm.txtButton3.Text;
                btnVHFandUP.Text = ClusterSetupForm.txtButton4.Text;
                CMD1 = ClusterSetupForm.btn1cmd.Text;
                CMD2 = ClusterSetupForm.btn2cmd.Text;
                CMD3 = ClusterSetupForm.btn3cmd.Text;
                CMD4 = ClusterSetupForm.btn4cmd.Text;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region misc function

        private void rtbDXClusterText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void txtMessage_KeyUP(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (client.ClusterClient != null && client.ClusterClient.Connected)
                        client.SendMessage(0, txtMessage.Text.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        private void DXClusterClient_Resize(object sender, EventArgs e)
        {
            try
            {
                Point loc = new Point(12, 56);
                rtbDXClusterText.Location = loc;
                rtbDXClusterText.Width = this.Width - 40;
                rtbDXClusterText.Height = this.Height - 154;

                int space = Math.Max((rtbDXClusterText.Width - 24 - btnBye.Width * 8 - 5) / 7, 5);
                loc = btnConnect.Location;
                loc.Y = this.Height - 78;
                btnConnect.Location = loc;
                loc.X += btnBye.Width + space;
                btnBye.Location = loc;
                loc.X += btnBye.Width + space;
                btnNoDX.Location = loc;
                loc.X += btnBye.Width + space;
                btnShowDX.Location = loc;
                loc.X += btnShowDX.Width + space;
                btnNoVHF.Location = loc;
                loc.X += btnBye.Width + space;
                btnVHFandUP.Location = loc;
                loc.X += btnBye.Width + space;
                btnClearTxt.Location = loc;
                loc.X += btnBye.Width + space;
                btnSettings.Location = loc;
                space = (rtbDXClusterText.Width - (lblMessage.Width + txtMessage.Width + comboDXCluster.Width)) / 2;
                loc.X = space;
                loc.Y = 22;
                lblMessage.Location = loc;
                loc.X += 56;
                loc.Y = 19;
                txtMessage.Location = loc;
                loc.X += 280;
                loc.Y = 19;
                comboDXCluster.Location = loc;
                SendMessage(rtbDXClusterText.Handle, WM_VSCROLL, SB_BOTTOM, 0);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void rtbDXClusterText_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                double vfoa = 0.0;
                Band band = Band.B20M;

                if (double.TryParse(rtbDXClusterText.SelectedText, out vfoa))
                {
                    vfoa = Math.Round(vfoa / 1e3, 6);
                    DB.GetBandLimits(vfoa, out band);
                    MainWindow.CurrentBand = band;

                    if ((vfoa > (MainWindow.LOSCFreq * 1e6 + Audio.RXSampleRate / 2) / 1e6) ||
                    (vfoa < (MainWindow.LOSCFreq * 1e6 - Audio.RXSampleRate / 2) / 1e6))
                    {
                        double losc = Math.Round(vfoa - 0.01, 3);
                        MainWindow.LOSCFreq = losc;
                    }

                    MainWindow.VFOAFreq = Math.Round(vfoa, 6);
                    MainWindow.VFOBFreq = Math.Round(vfoa, 6);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }

    #region Telnet Client

    class TelnetClient
    {
        #region Variable

        public TcpClient ClusterClient;
        Stream sr;
        Stream sw;
        delegate void CrossThreadCallback(int command, byte[] data, int count);
        DXClusterClient ClusterForm;
        string remote_addr = "";
        string remote_port = "";

        #endregion

        #region constructor/destructor

        public TelnetClient(DXClusterClient form)
        {
            try
            {
                ClusterForm = form;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        ~TelnetClient()
        {
        }

        #endregion

        #region misc function

        public bool SendMessage(int type, string message)
        {
            try
            {
                switch (type)
                {
                    case 0:
                        {                                               // regular message
                            if (ClusterClient.Connected)
                            {
                                ASCIIEncoding asen = new ASCIIEncoding();
                                byte[] ba = asen.GetBytes(message);
                                byte[] data = new byte[ba.Length + 2];

                                for (int i = 0; i < data.Length - 2; i++)
                                    data[i] = ba[i];

                                data[data.Length - 2] = 0x0d;
                                data[data.Length - 1] = 0x0a;
                                sw.Write(data, 0, data.Length);
                            }
                        }
                        break;

                    case 1:
                        {                                               // bye
                            if (ClusterClient.Client.Connected)
                            {
                                ASCIIEncoding asen = new ASCIIEncoding();
                                byte[] ba = asen.GetBytes(message);
                                byte[] data = new byte[5];

                                for (int i = 0; i < message.Length; i++)
                                    data[i] = ba[i];

                                data[3] = 0x0d;
                                data[4] = 0x0a;
                                sw.Write(data, 0, data.Length);
                                Thread.Sleep(1000);
                                ClusterClient.Client.Shutdown(SocketShutdown.Both);
                                ClusterClient.Client.Close(1000);
                            }
                        }
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                ClusterClient.Client.Shutdown(SocketShutdown.Both);
                ClusterClient.Client.Close();
                return false;
            }
        }

        public void Start(string remote_address)
        {
            try
            {
                string[] address = remote_address.Split(':');
                remote_addr = address[0];
                remote_port = address[1];
                ClusterClient = new TcpClient();
                Thread t = new Thread(new ThreadStart(ClientServiceLoop));
                t.Start();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                MessageBox.Show("Error creating client!", "DX Cluster error");
            }
        }

        public bool Close()
        {
            try
            {
                sr.Close();
                sr.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public void ClientServiceLoop()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(remote_addr);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint ipepRemote = new IPEndPoint(ipAddress, int.Parse(remote_port));
                byte[] buffer = new byte[2048];
                int count = 0;
                string text = "";
                ASCIIEncoding buf = new ASCIIEncoding();

                if (!ClusterForm.closing)
                {
                    text = "DXClusterClient - Connecting to " + remote_addr.ToString();
                    buf.GetBytes(text, 0, text.Length, buffer, 0);
                    ClusterForm.Invoke(new CrossThreadCallback(ClusterForm.CrossThreadCommand), 1, buffer, text.Length);
                }

                ClusterClient.Connect(ipepRemote);

                if (ClusterClient.Connected && !ClusterForm.closing)
                {
                    sw = ClusterClient.GetStream();
                    sr = ClusterClient.GetStream();
                    text = "DXClusterClient - Connected to " + remote_addr.ToString();
                    buf.GetBytes(text, 0, text.Length, buffer, 0);
                    ClusterForm.Invoke(new CrossThreadCallback(ClusterForm.CrossThreadCommand), 1, buffer, text.Length);
                }
                else
                {
                    if (!ClusterForm.closing)
                    {
                        text = "DXClusterClient - Unable to connect to " + remote_addr.ToString();
                        buf.GetBytes(text, 0, text.Length, buffer, 0);
                        ClusterForm.Invoke(new CrossThreadCallback(ClusterForm.CrossThreadCommand), 1, buffer, text.Length);
                        ClusterClient.Close();
                    }
                    return;
                }

                while (ClusterClient.Connected)
                {
                    count = sr.Read(buffer, 0, 2048);
                    ClusterForm.Invoke(new CrossThreadCallback(ClusterForm.CrossThreadCommand), 0, buffer, count);
                }

                if (!ClusterForm.closing)
                {
                    text = "DXClusterClient - Disconnected";
                    buf = new ASCIIEncoding();
                    buf.GetBytes(text, 0, text.Length, buffer, 0);
                    ClusterForm.Invoke(new CrossThreadCallback(ClusterForm.CrossThreadCommand), 1, buffer, text.Length);
                }

                ClusterClient.Close();
            }
            catch (Exception ex)
            {
                if (!ClusterForm.closing)
                {
                    byte[] buffer = new byte[100];
                    string text = "";
                    ASCIIEncoding buf = new ASCIIEncoding();
                    text = "DXClusterClient - Error while connecting to " + remote_addr.ToString();
                    buf.GetBytes(text, 0, text.Length, buffer, 0);
                    ClusterForm.Invoke(new CrossThreadCallback(ClusterForm.CrossThreadCommand), 1, buffer, text.Length);
                    Debug.Write(ex.ToString());
                }
            }
        }

        #endregion
    }

    #endregion
}
