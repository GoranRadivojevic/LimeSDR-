//=================================================================
// AGauge.cs
//=================================================================
//
// Copyright (C)2011-2013 YT7PWR Goran Radivojevic
// contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
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
//=================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if DirectX
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows;
using System.Threading;
#endif

namespace PowerSDR
{
    public class AGauge : Control
    {
        #region variable

#if DirectX
        public SlimDX.Direct3D9.Device device = null;
        private Texture BackgroundTexture = null;
        private Sprite sprite = null;
        private Rectangle texture_size;
        SlimDX.Direct3D9.Line line;
        Vector2[] verts1;
        public bool DX_reinit = false;
#endif
        private delegate void DebugCallbackFunction(string name);
        public bool debug = false;
        public bool booting = true;
        public Console console;
        public Int32 m_BaseArcStart = 230;
        public Int32 m_BaseArcSweep = 77;
        private Single m_value = 0;
        public Int32 m_NeedleRadius = 160;
        private Point m_Center = new Point(120, 180);
        private Boolean drawGaugeBackground = true;
        public Bitmap gaugeBitmap;

        #endregion

        #region properties

#if DirectX
        private RenderType directx_render_type = RenderType.HARDWARE;
        public RenderType DirectXRenderType
        {
            get { return directx_render_type; }
            set { directx_render_type = value; }
        }
#endif

        private DisplayEngine display_engine = DisplayEngine.GDI_PLUS;
        public DisplayEngine displayEngine
        {
            get { return display_engine; }
            set { display_engine = value; }
        }

        private Control gauge_target = null;
        public Control GaugeTarget
        {
            get { return gauge_target; }
            set { gauge_target = value; }
        }

        private Single m_MinValue = 0;
        public Single MinValue
        {
            get
            {
                return m_MinValue;
            }
            set
            {
                if ((m_MinValue != value)
                && (value < m_MaxValue))
                {
                    m_MinValue = value;
                }
            }
        }

        private Single m_MaxValue = 18;
        public Single MaxValue
        {
            get
            {
                return m_MaxValue;
            }
            set
            {
                if ((m_MaxValue != value)
                && (value > m_MinValue))
                {
                    m_MaxValue = value;
                }
            }
        }

        public Point Center
        {
            get
            {
                return m_Center;
            }
            set
            {
                if (m_Center != value)
                {
                    m_Center = value;
                }
            }
        }

        public Single Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_value != value)
                {
                    m_value = Math.Min(Math.Max(value, m_MinValue), m_MaxValue);
                }
            }
        }

        private int agauge_width = 0;
        public int AGaugeWidth
        {
            get { return agauge_width; }
            set { agauge_width = value; }
        }

        private int agauge_height = 0;
        public int AGaugeHeight
        {
            get { return agauge_height; }
            set { agauge_height = value; }
        }

        #endregion

        #region constructor

        public AGauge(Console c)
        {
            console = c;
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = (float)(8.25 / ratio);
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            gaugeBitmap = new Bitmap(console.picAGauge.Width, console.picAGauge.Height, PixelFormat.Format24bppRgb);
            this.PerformLayout();
        }

        ~AGauge()
        {

        }

        #endregion

        #region functions

        #region DirectX

#if DirectX

        public bool DirectX_Init(string background_image)
        {
            if (!booting && !DX_reinit)
            {
                try
                {
                    DX_reinit = true;
                    PresentParameters presentParms = new PresentParameters();
                    presentParms.Windowed = true;
                    presentParms.SwapEffect = SwapEffect.Discard;
                    presentParms.Multisample = MultisampleType.None;
                    presentParms.EnableAutoDepthStencil = true;
                    presentParms.AutoDepthStencilFormat = Format.D24X8;
                    presentParms.PresentFlags = PresentFlags.DiscardDepthStencil;
                    presentParms.PresentationInterval = PresentInterval.Default;
                    presentParms.BackBufferFormat = Format.X8R8G8B8;
                    presentParms.BackBufferHeight = gauge_target.Height;
                    presentParms.BackBufferWidth = gauge_target.Width;
                    presentParms.Windowed = true;
                    presentParms.BackBufferCount = 1;

                    switch (directx_render_type)
                    {
                        case RenderType.HARDWARE:
                            {
                                try
                                {
                                    device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        gauge_target.Handle, CreateFlags.HardwareVertexProcessing |
                                        CreateFlags.FpuPreserve | CreateFlags.Multithreaded,
                                        presentParms);
                                }
                                catch (Direct3D9Exception ex)
                                {
                                    if (debug && !console.ConsoleClosing)
                                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                            "DirectX hardware init error(AGauge)!\n" + ex.ToString());
                                }
                            }
                            break;

                        case RenderType.SOFTWARE:
                            {

                                try
                                {
                                    device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                        gauge_target.Handle, CreateFlags.SoftwareVertexProcessing |
                                        CreateFlags.FpuPreserve | CreateFlags.Multithreaded, presentParms);
                                }
                                catch (Direct3D9Exception exe)
                                {
                                    if (debug && !console.ConsoleClosing)
                                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                                            "DirectX software init error(AGauge)!\n" + exe.ToString());

                                    return false;
                                }
                            }
                            break;
                    }

                    var vertexElems = new[] {
                        new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                        new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                        VertexElement.VertexDeclarationEnd
                        };

                    var vertexDecl = new VertexDeclaration(device, vertexElems);
                    device.VertexDeclaration = vertexDecl;

                    if (background_image != null && File.Exists(background_image))
                    {
                        BackgroundTexture = Texture.FromFile(device, background_image, gauge_target.Width, gauge_target.Height,
                            1, Usage.None, Format.Unknown, Pool.Managed, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);
                    }

                    texture_size.Width = gauge_target.Width;
                    texture_size.Height = gauge_target.Height;
                    sprite = new Sprite(device);

                    verts1 = new Vector2[2];
                    line = new Line(device);
                    line.Antialias = true;
                    line.Width = 3;
                    line.GLLines = true;
                    device.SetRenderState(RenderState.AntialiasedLineEnable, true);
                    DX_reinit = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                    DX_reinit = false;

                    if (debug && !console.ConsoleClosing)
                        console.Invoke(new DebugCallbackFunction(console.DebugCallback),
                            "Init AGauge error!\n" + ex.ToString());

                    return false;
                }
            }

            return true;
        }

        public void DirectXRelease()
        {
            try
            {
                if (!booting && !DX_reinit)
                {
                    DX_reinit = true;

                    if (device != null)
                    {
                        device.Dispose();
                        device = null;
                    }

                    DX_reinit = false;
                }
            }
            catch (Exception ex)
            {
                Debug.Write("DX release error!" + ex.ToString());
                DX_reinit = false;
            }
        }

        public bool RenderGauge()
        {
            try
            {
                if (device != null && !DX_reinit)
                {
                    Single brushAngle = (Int32)(m_BaseArcStart + (m_value - m_MinValue) * m_BaseArcSweep /
                        (m_MaxValue - m_MinValue)) % 360;
                    Double needleAngle = brushAngle * Math.PI / 180;
                    verts1[0].X = (float)(Center.X + m_NeedleRadius / 4 * Math.Cos(needleAngle));
                    verts1[0].Y = (float)(Center.Y + m_NeedleRadius / 4 * Math.Sin(needleAngle));
                    verts1[1].X = (float)(Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                    verts1[1].Y = (float)(Center.Y + m_NeedleRadius * Math.Sin(needleAngle));

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 0.0f, 0);
                    sprite.Begin(SpriteFlags.AlphaBlend);

                    if (BackgroundTexture != null)
                        sprite.Draw(BackgroundTexture, texture_size, (Color4)Color.White);

                    sprite.End();
                    //Begin the scene
                    device.BeginScene();
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.SourceAlpha);
                    device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.DestinationAlpha);
                    line.Draw(verts1, Color.Red);
                    device.EndScene();
                    device.Present();
                    return true;
                }

                if (DX_reinit)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());

                if (DX_reinit)
                    return true;
                else
                    return false;
            }
        }

#endif

        #endregion

        #region GDI+

        public void PaintGauge(PaintEventArgs pe)
        {
            try
            {
                if (display_engine == DisplayEngine.GDI_PLUS || !console.chkPower.Checked)
                {
                    PointF[] points = new PointF[3];
                    //gaugeBitmap = new Bitmap(pe.ClipRectangle.Width, pe.ClipRectangle.Height, pe.Graphics);
                    Graphics g = pe.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(gaugeBitmap, 0, 0, pe.ClipRectangle.Width, pe.ClipRectangle.Height);

                    Single brushAngle = (Int32)(m_BaseArcStart + (m_value - m_MinValue) * m_BaseArcSweep /
                        (m_MaxValue - m_MinValue)) % 360;
                    Double needleAngle = brushAngle * Math.PI / 180;
                    points[0].X = (float)(Center.X + m_NeedleRadius / 4 * Math.Cos(needleAngle));
                    points[0].Y = (float)(Center.Y + m_NeedleRadius / 4 * Math.Sin(needleAngle));
                    points[1].X = (float)(Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                    points[1].Y = (float)(Center.Y + m_NeedleRadius * Math.Sin(needleAngle));

                    pe.Graphics.DrawLine(new Pen(Color.Red, 3.0f), Center.X, Center.Y, points[0].X, points[0].Y);
                    pe.Graphics.DrawLine(new Pen(Color.Red, 3.0f), Center.X, Center.Y, points[1].X, points[1].Y);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #endregion
    }
}
