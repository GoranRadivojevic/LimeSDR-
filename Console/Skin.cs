//=================================================================
// Skin.cs
//=================================================================
// Provides a way to easily save and restore appearance of common
// .NET controls to xml.
// Copyright (C) 2009  FlexRadio Systems
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
 
/*
 *  Changes for GenesisRadio
 *  Copyright (C)2008-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/


using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace PowerSDR 
{
    /// <summary>
    /// Designed to allow easy saving/restoring of common .NET controls to xml.
    /// </summary>
    public class Skin
    {
        #region Private Variables

        private enum ImageState
        {
            NormalUp,
            NormalDown,
            DisabledUp,
            DisabledDown,
            FocusedUp,
            FocusedDown,
            MouseOverUp,
            MouseOverDown,
        }

        private static string name;
        private static string path;
        private const string pic_file_ext = ".png";

        private Console console;

        #endregion

        #region constructor and destructor

        public Skin(Console c)
        {
            console = c;
        }
        ~Skin()
        {
        }

        #endregion

        #region Main

        /// <summary>
        /// Restores a forms appearance including properties of the form and its controls from xml.
        /// </summary>
        /// <param name="name">name of file to be used</param>
        /// <param name="path">path to file</param>
        /// <param name="f">Form to restore</param>
        /// <returns></returns>
        public bool Restore(string name, string p, Form f)
        {
            Skin.path = p + name;
            Skin.name = name;

            if (File.Exists(path + "\\" + f.Name + "\\" + f.Name + pic_file_ext))
                f.BackgroundImage = Image.FromFile(path + "\\" + f.Name + "\\" + f.Name + pic_file_ext);
            else f.BackgroundImage = null;

            foreach (Control c in f.Controls) // load in images
                ReadImages(p, c, f);

            return true;
        }

        private void ReadImages(string p, Control c, Form f)
        {
            Skin.path = p + name;
            Control temp;

            temp = c as Custom_controls.CustomGroupBox;
            if (temp != null)
            {
                Custom_controls.CustomGroupBox grp = (Custom_controls.CustomGroupBox)c;
                grp.GroupPanelColor = console.ConsoleColor;
                grp.TextBackColor = console.SkinsButtonTxtColor;

                if (grp.Name == "grpVFOnew")
                {
                    c.BackgroundImage = null;
                    c.BackColor = console.NewBackgroundVFOColor;
                    foreach (Control c2 in grp.Controls)
                    {
                        temp = c2 as PrettyTrackBar;
                        if (temp != null)
                        {
                            SetupPrettyTrackBarImages((PrettyTrackBar)c2);
                        }

                        temp = c2 as PictureBox;
                        if (temp != null)
                        {
                            SetBackgroundImage((PictureBox)c2);
                            return;
                        }
                    }
                }
                else
                {
                    if (File.Exists(path + "\\" + f.Name + "\\" + "grp" + pic_file_ext))
                    {
                        c.BackgroundImage = Image.FromFile(path + "\\" + f.Name + "\\" + "grp" + pic_file_ext);
                        c.ForeColor = console.SkinsButtonTxtColor;
                    }
                    else
                    {
                        c.BackgroundImage = null;
                        c.BackColor = console.ConsoleColor;
                        c.ForeColor = console.SkinsButtonTxtColor;
                    }

                    foreach (Control c2 in grp.Controls)
                        ReadImages(p, c2, f);
                    return;
                }
            }

            temp = c as Panel;
            if (temp != null)
            {
                Panel pnl = (Panel)c;
                if (File.Exists(path + "\\" + f.Name + "\\" + "panel" + pic_file_ext))
                {
                    c.BackgroundImage = Image.FromFile(path + "\\" + f.Name + "\\" + "panel" + pic_file_ext);
                    c.ForeColor = console.SkinsButtonTxtColor;
                }
                else
                {
                    c.BackgroundImage = null;
                    c.ForeColor = console.SkinsButtonTxtColor;
                }
                foreach (Control c2 in pnl.Controls)
                    ReadImages(p, c2, f);
                return;
            }

            temp = c as Button;
            if (temp != null)
            {
                SetupButtonImages((Button)c);
                return;
            }

            temp = c as CheckBox;
            if (temp != null)
            {
                SetupCheckBoxImages((CheckBox)c);
                return;
            }

            temp = c as Label;
            if (temp != null)
            {
                if (File.Exists(path + "\\" + f.Name + "\\" + "lbl" + pic_file_ext))
                {
                    if (temp.Name != console.lblMemoryNumber.Name &&
                        temp.Name != console.lblFMMemory.Name &&
                        temp.Name != console.lblVFOATX.Name &&
                        temp.Name != console.lblVFOBTX.Name &&
                        temp.Name != console.btnUSB.Name &&
                        temp.Name != console.lblAFValue.Name &&
                        temp.Name != console.lblRFValue.Name &&
                        temp.Name != console.lblPWRValue.Name)
                    {
                        c.BackgroundImage = Image.FromFile(path + "\\" + f.Name + "\\" + "lbl" + pic_file_ext);
                        c.ForeColor = console.SkinsButtonTxtColor;
                        console.menuStrip1.BackgroundImage = Image.FromFile(path + "\\" + f.Name + "\\" + "lbl" + pic_file_ext);
                    }
                }
                else
                {
                    if (temp.Name != console.lblMemoryNumber.Name)
                    {
                        c.BackgroundImage = null;
                        c.ForeColor = console.SkinsButtonTxtColor;
                        console.menuStrip1.BackgroundImage = null;
                        console.menuStrip1.BackColor = SystemColors.Control;
                    }
                    else if (temp.Name != console.lblFMMemory.Name)
                    {
                        c.BackgroundImage = null;
                        c.ForeColor = console.SkinsButtonTxtColor;
                        console.menuStrip1.BackgroundImage = null;
                        console.menuStrip1.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        c.BackgroundImage = null;
                        c.ForeColor = console.SkinsButtonTxtColor;
                    }
                }
                return;
            }

            temp = c as PrettyTrackBar;
            if (temp != null)
            {
                SetupPrettyTrackBarImages((PrettyTrackBar)c);
                return;
            }

            temp = c as PictureBox;
            if (temp != null)
            {
                SetBackgroundImage((PictureBox)c);
                return;
            }

            temp = c as RadioButtonTS;
            if (temp != null)
            {
                if (((RadioButtonTS)c).Appearance == Appearance.Button)
                    SetupRadioButtonImages((RadioButtonTS)c);
                return;
            }

            console.menuStrip1.ForeColor = console.SkinsButtonTxtColor;
            console.BackColor = console.ConsoleColor;
        }

        #endregion

        #region Control Specific

        #region Button

        private void SetupButtonImages(Button ctrl)
        {
            if (ctrl.ImageList == null)
                ctrl.ImageList = new ImageList();
            else ctrl.ImageList.Images.Clear();
            ctrl.ImageList.ImageSize = ctrl.Size; // may be an issue with smaller images
            ctrl.ImageList.ColorDepth = ColorDepth.Depth32Bit;

            // load images into image list property
            string s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "btn" + "-";
            for (int i = 0; i < 8; i++)
            {
                if (File.Exists(s + i.ToString() + pic_file_ext))
                    ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
            }
            EventHandler handler = new EventHandler(Button_StateChanged);
            ctrl.Click -= handler; // remove handlers first to ensure they don't get added multiple times
            ctrl.Click += handler;
            ctrl.EnabledChanged -= handler;
            ctrl.EnabledChanged += handler;
            ctrl.MouseEnter -= new EventHandler(Button_MouseEnter);
            ctrl.MouseEnter += new EventHandler(Button_MouseEnter);
            ctrl.MouseLeave -= handler;
            ctrl.MouseLeave += handler;
            ctrl.MouseDown -= new MouseEventHandler(Button_MouseDown);
            ctrl.MouseDown += new MouseEventHandler(Button_MouseDown);
            ctrl.MouseUp -= new MouseEventHandler(Button_MouseUp);
            ctrl.MouseUp += new MouseEventHandler(Button_MouseUp);
            ctrl.GotFocus -= handler;
            ctrl.GotFocus += handler;
            ctrl.LostFocus -= handler;
            ctrl.LostFocus += handler;

            ctrl.BackgroundImage = null;
            ctrl.ForeColor = console.SkinsButtonTxtColor;
            Button_StateChanged(ctrl, EventArgs.Empty);
        }

        private void Button_StateChanged(object sender, EventArgs e)
        {
            Button ctrl = (Button)sender;
            ImageState state = ImageState.NormalUp;

            if (!ctrl.Enabled &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.DisabledUp.ToString()) >= 0)
            {
                state = ImageState.DisabledUp;
            }
            else if (ctrl.Focused &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.FocusedUp.ToString()) >= 0)
            {
                state = ImageState.FocusedUp;
            }
            else
            {
                state = ImageState.NormalUp;
            }

            SetButtonImageState(ctrl, state);
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button ctrl = (Button)sender;
            if (!ctrl.Enabled) return;

            ImageState state = ImageState.MouseOverUp;

            SetButtonImageState(ctrl, state);
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            Button ctrl = (Button)sender;
            if (!ctrl.Enabled) return;

            ImageState state = ImageState.NormalDown;

            SetButtonImageState(ctrl, state);
        }

        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Button_StateChanged(sender, EventArgs.Empty);
        }

        private void SetButtonImageState(Button ctrl, ImageState state)
        {
            if (ctrl.ImageList == null) return;
            int index = ctrl.ImageList.Images.IndexOfKey(state.ToString());
            if (index < 0)
            {
                if (console.SkinsEnabled)
                {
                    switch (state)
                    {
                        case (ImageState.NormalDown):
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = console.ButtonSelectedColor;
                                return;
                            };
                        case (ImageState.MouseOverUp):
                            {
                                return;
                            };
                        default:
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = SystemColors.Control;
                                return;
                            }
                    }
                }
                else
                {
                    switch (state)
                    {
                        case (ImageState.NormalDown):
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = console.ButtonSelectedColor;
                                return;
                            };
                        case (ImageState.MouseOverUp):
                            {
                                return;
                            };
                        default:
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = SystemColors.Control;
                                return;
                            }
                    }
                }
            }
            ctrl.BackgroundImage = ctrl.ImageList.Images[index];
        }

        #endregion

        #region CheckBox

        private void SetupCheckBoxImages(CheckBox ctrl)
        {
            string s;
            if (ctrl.ImageList == null)
                ctrl.ImageList = new ImageList();
            else ctrl.ImageList.Images.Clear();
            ctrl.ImageList.ImageSize = ctrl.Size; // may be an issue with smaller images
            ctrl.ImageList.ColorDepth = ColorDepth.Depth32Bit;

            // load images into image list property
            if (ctrl.Name == console.chkRecordWav.Name)
            {
                s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "chkRecordWav" + "-";
                for (int i = 0; i < 8; i++)
                {
                    if (File.Exists(s + i.ToString() + pic_file_ext))
                        ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
                }
            }
            else if( ctrl.Name == console.chkPlayWav.Name)
            {
                s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "chkPlayWav" + "-";
                for (int i = 0; i < 8; i++)
                {
                    if (File.Exists(s + i.ToString() + pic_file_ext))
                        ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
                }
            }
            else if (ctrl.Name != console.chkPower.Name)
            {
                if (ctrl.Appearance == Appearance.Button)
                {
                    s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "chk" + "-";
                    for (int i = 0; i < 8; i++)
                    {
                        if (File.Exists(s + i.ToString() + pic_file_ext))
                            ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
                    }
                }
                else
                {
                    s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "grp" + "-";
                    for (int i = 0; i < 8; i++)
                    {
                        if (File.Exists(s + i.ToString() + pic_file_ext))
                            ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
                    }
                }
            }
            else
            {
                s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "chk_pwr" + "-";
                for (int i = 0; i < 8; i++)
                {
                    if (File.Exists(s + i.ToString() + pic_file_ext))
                        ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
                }
            }
            EventHandler handler = new EventHandler(CheckBox_StateChanged);
            ctrl.CheckedChanged -= handler; // remove handlers first to ensure they don't get added multiple times
            ctrl.CheckedChanged += handler;
            ctrl.EnabledChanged -= handler;
            ctrl.EnabledChanged += handler;
            ctrl.MouseEnter -= new EventHandler(CheckBox_MouseEnter);
            ctrl.MouseEnter += new EventHandler(CheckBox_MouseEnter);
            ctrl.MouseLeave -= handler;
            ctrl.MouseLeave += handler;
            ctrl.GotFocus -= handler;
            ctrl.GotFocus += handler;
            ctrl.LostFocus -= handler;
            ctrl.LostFocus += handler;

            ctrl.BackgroundImage = null;
            ctrl.ForeColor = console.SkinsButtonTxtColor;
            CheckBox_StateChanged(ctrl, EventArgs.Empty);
        }

        private void CheckBox_StateChanged(object sender, EventArgs e)
        {
            CheckBox ctrl = (CheckBox)sender;
            ImageState state = ImageState.NormalUp;

            if (!ctrl.Enabled &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.DisabledDown.ToString()) >= 0 &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.DisabledUp.ToString()) >= 0)
            {
                if (ctrl.Checked)
                    state = ImageState.DisabledDown;
                else
                    state = ImageState.DisabledUp;
            }
            else if (ctrl.Focused && 
                ctrl.ImageList.Images.IndexOfKey(ImageState.FocusedDown.ToString()) >= 0 &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.FocusedUp.ToString()) >= 0)
            {
                if (ctrl.Checked)
                    state = ImageState.FocusedDown;
                else
                    state = ImageState.FocusedUp;
            }
            else
            {
                if (ctrl.Checked)
                    state = ImageState.NormalDown;
                else
                    state = ImageState.NormalUp;
            }

            SetCheckBoxImageState(ctrl, state);
        }

        private void CheckBox_MouseEnter(object sender, EventArgs e)
        {
            CheckBox ctrl = (CheckBox)sender;
            if (!ctrl.Enabled) return;

            ImageState state = ImageState.MouseOverUp;
            if (ctrl.Checked) state = ImageState.MouseOverDown;

            SetCheckBoxImageState(ctrl, state);
        }

        private void SetCheckBoxImageState(CheckBox ctrl, ImageState state)
        {
            if (ctrl.ImageList == null) return;
            int index = ctrl.ImageList.Images.IndexOfKey(state.ToString());
            if (index < 0)
            {
                if (console.SkinsEnabled)
                {
                    switch (state)
                    {
                        case (ImageState.NormalDown):
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = console.ButtonSelectedColor;
                                return;
                            };
                        case (ImageState.MouseOverUp):
                            {
                                return;
                            };
                        case (ImageState.MouseOverDown):
                            {
                                return;
                            };
                        default:
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = SystemColors.Control;
                                return;
                            }
                    };
                }
                else
                {
                    switch (state)
                    {
                        case (ImageState.NormalDown):
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                return;
                            };
                        case (ImageState.MouseOverUp):
                            {
                                return;
                            };
                        case (ImageState.MouseOverDown):
                            {
                                return;
                            };
                        default:
                            {
                                ctrl.BackgroundImage = null;
                                ctrl.ForeColor = console.SkinsButtonTxtColor;
                                ctrl.BackColor = SystemColors.Control;
                                return;
                            }
                    };
                }
            }
            ctrl.BackgroundImage = ctrl.ImageList.Images[index];
        }

#endregion

        #region RadioButton

        private void SetupRadioButtonImages(RadioButtonTS ctrl)
        {
            if (ctrl.ImageList == null)
                ctrl.ImageList = new ImageList();
            else ctrl.ImageList.Images.Clear();
            ctrl.ImageList.ImageSize = ctrl.Size; // may be an issue with smaller images
            ctrl.ImageList.ColorDepth = ColorDepth.Depth32Bit;

            // load images into image list property
            string s = path + "\\" + ctrl.TopLevelControl.Name + "\\" + "rad" + "-";
            for (int i = 0; i < 8; i++)
            {
                if (File.Exists(s + i.ToString()+ pic_file_ext))
                    ctrl.ImageList.Images.Add(((ImageState)i).ToString(), Image.FromFile(s + i.ToString() + pic_file_ext));
                else
                {
                    if (ctrl.ImageList.Images.ContainsKey(((ImageState)i).ToString()))
                        ctrl.ImageList.Images.RemoveByKey(((ImageState)i).ToString());
                }
            }
            EventHandler handler = new EventHandler(RadioButton_StateChanged);
            ctrl.CheckedChanged -= handler; // remove handlers first to ensure they don't get added multiple times
            ctrl.CheckedChanged += handler;
            ctrl.EnabledChanged -= handler;
            ctrl.EnabledChanged += handler;
            ctrl.MouseEnter -= new EventHandler(RadioButton_MouseEnter);
            ctrl.MouseEnter += new EventHandler(RadioButton_MouseEnter);
            ctrl.MouseLeave -= handler;
            ctrl.MouseLeave += handler;
            ctrl.GotFocus += handler;
            ctrl.GotFocus -= handler;
            ctrl.LostFocus += handler;
            ctrl.LostFocus -= handler;

            ctrl.BackgroundImage = null;
            ctrl.ForeColor = console.SkinsButtonTxtColor;
            RadioButton_StateChanged(ctrl, EventArgs.Empty);
        }

        private void RadioButton_StateChanged(object sender, EventArgs e)
        {
            RadioButtonTS ctrl = (RadioButtonTS)sender;
            ImageState state = ImageState.NormalUp;

            if (!ctrl.Enabled &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.DisabledDown.ToString()) >= 0 &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.DisabledUp.ToString()) >= 0)
            {
                if (ctrl.Checked)
                    state = ImageState.DisabledDown;
                else
                    state = ImageState.DisabledUp;
            }
            else if (ctrl.Focused &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.FocusedDown.ToString()) >= 0 &&
                ctrl.ImageList.Images.IndexOfKey(ImageState.FocusedUp.ToString()) >= 0)
            {
                if (ctrl.Checked)
                    state = ImageState.FocusedDown;
                else
                    state = ImageState.FocusedUp;
            }
            else
            {
                if (ctrl.Checked)
                    state = ImageState.NormalDown;
                else
                    state = ImageState.NormalUp;
            }

            SetRadioButtonImageState(ctrl, state);
        }

        private void RadioButton_MouseEnter(object sender, EventArgs e)
        {
            RadioButtonTS ctrl = (RadioButtonTS)sender;
            if (!ctrl.Enabled) return;

            ImageState state = ImageState.MouseOverUp;
            if (ctrl.Checked) state = ImageState.MouseOverDown;

            SetRadioButtonImageState(ctrl, state);
        }

        private void SetRadioButtonImageState(RadioButtonTS ctrl, ImageState state)
        {
            if (ctrl.ImageList == null) return;
            int index = ctrl.ImageList.Images.IndexOfKey(state.ToString());
            if (index < 0) return;
            ctrl.BackgroundImage = ctrl.ImageList.Images[index];
        }

        #endregion        

        #region PrettyTrackBar

        private void SetupPrettyTrackBarImages(PrettyTrackBar ctrl)
        {
            if (ctrl.Orientation == Orientation.Horizontal)
            {
                // load images
                string s = path + "\\" + ctrl.TopLevelControl.Name + "\\";
                if (File.Exists(s + "slider_back" + pic_file_ext))
                    ctrl.BackgroundImage = Image.FromFile(s + "slider_back" + pic_file_ext);
                else ctrl.BackgroundImage = null;

                if (File.Exists(s + "slider_head" + pic_file_ext))
                    ctrl.HeadImage = Image.FromFile(s + "slider_head" + pic_file_ext);
                else ctrl.HeadImage = null;

                ctrl.Invalidate();
            }
            else if (ctrl.Orientation == Orientation.Vertical)
            {
                string s = path + "\\" + ctrl.TopLevelControl.Name + "\\";
                if (File.Exists(s + "slider_back_v" + pic_file_ext))
                {
                    ctrl.BackgroundImage = Image.FromFile(s + "slider_back_v" + pic_file_ext);
                }
                else ctrl.BackgroundImage = null;

                if (File.Exists(s + "slider_head_v" + pic_file_ext))
                    ctrl.HeadImage = Image.FromFile(s + "slider_head_v" + pic_file_ext);
                else ctrl.HeadImage = null;

                ctrl.Invalidate();
            }
        }

        #endregion

        #endregion

        #region Utility

        private void SetBackgroundImage(Control c)
        {
            if (c.Name == "console" && File.Exists(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext))
            {
                console.BackgroundImage = Image.FromFile(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext);
            }
            else if (c.Name == "picAGauge" && File.Exists(path + "\\" + c.TopLevelControl.Name + "\\" + 
                "NewVFOAnalogSignalGauge" + ".jpg"))
            {
                //c.BackgroundImage = Image.FromFile(path + "\\" + c.TopLevelControl.Name + "\\" + "NewVFOAnalogSignalGauge" + ".jpg");
                console.NewVFO_background_image = path + "\\" + c.TopLevelControl.Name + "\\" + "NewVFOAnalogSignalGauge" + ".jpg";
                console.NewVFOSignalGauge.gaugeBitmap = new Bitmap(System.Drawing.Image.FromFile(console.NewVFO_background_image, true));
            }
            else if (c.Name == "picSmallAGauge" && File.Exists(path + "\\" + c.TopLevelControl.Name + "\\" +
                "AnalogSignalGauge" + ".jpg"))
            {
                c.BackgroundImage = Image.FromFile(path + "\\" + c.TopLevelControl.Name + "\\" + "AnalogSignalGauge" + ".jpg");
                console.classicVFO_background_image = path + "\\" + c.TopLevelControl.Name + "\\" + "AnalogSignalGauge" + ".jpg";
            }
            else if (c.Name == "picDisplay" && File.Exists(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext))
            {
#if(DirectX)
                Display_DirectX.background_image = path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext;
#endif
                Display_GDI.panadapter_img = path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext;
                Display_GDI.panadapter_bmp = new Bitmap(System.Drawing.Image.FromFile(Display_GDI.panadapter_img, true));
                //c.BackgroundImage = Image.FromFile(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext);
                //console.DXform.BackgroundImage = Image.FromFile(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext);
            }

            else if (File.Exists(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext))
            {
                c.BackgroundImage = Image.FromFile(path + "\\" + c.TopLevelControl.Name + "\\" + c.Name + pic_file_ext);
            }
            else
                c.BackgroundImage = null;
        }

        #endregion
    }
}
