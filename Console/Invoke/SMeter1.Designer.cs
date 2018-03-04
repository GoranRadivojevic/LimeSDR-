using System;
using System.Windows.Forms;
using System.Drawing;

namespace PowerSDR.Invoke
{
    partial class SMeter1
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fruityLoopsBackgroundPainterSWRLine = new ProgressODoom.FruityLoopsBackgroundPainter();
            this.fruityLoopsProgressPainterSWRLine = new ProgressODoom.FruityLoopsProgressPainter();
            this.fruityLoopsBackgroundPainterSigLine = new ProgressODoom.FruityLoopsBackgroundPainter();
            this.fruityLoopsProgressPainterSigLine = new ProgressODoom.FruityLoopsProgressPainter();
            this.progressSignal = new ProgressODoom.ProgressBarEx();
            this.labelTS13 = new System.Windows.Forms.LabelTS();
            this.labelTS11 = new System.Windows.Forms.LabelTS();
            this.labelTS6 = new System.Windows.Forms.LabelTS();
            this.labelTS12 = new System.Windows.Forms.LabelTS();
            this.labelTS14 = new System.Windows.Forms.LabelTS();
            this.labelTS15 = new System.Windows.Forms.LabelTS();
            this.labelTS17 = new System.Windows.Forms.LabelTS();
            this.labelTS10 = new System.Windows.Forms.LabelTS();
            this.labelTS9 = new System.Windows.Forms.LabelTS();
            this.labelTS8 = new System.Windows.Forms.LabelTS();
            this.labelTS7 = new System.Windows.Forms.LabelTS();
            this.labelTS5 = new System.Windows.Forms.LabelTS();
            this.labelTS4 = new System.Windows.Forms.LabelTS();
            this.labelTS3 = new System.Windows.Forms.LabelTS();
            this.labelTS2 = new System.Windows.Forms.LabelTS();
            this.labelTS1 = new System.Windows.Forms.LabelTS();
            this.progressTop = new ProgressODoom.ProgressBarEx();
            this.progressBottom = new ProgressODoom.ProgressBarEx();
            this.SuspendLayout();
            // 
            // fruityLoopsBackgroundPainterSWRLine
            // 
            this.fruityLoopsBackgroundPainterSWRLine.FruityType = ProgressODoom.FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer;
            // 
            // fruityLoopsProgressPainterSWRLine
            // 
            this.fruityLoopsProgressPainterSWRLine.FruityType = ProgressODoom.FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer;
            this.fruityLoopsProgressPainterSWRLine.ProgressBorderPainter = null;
            // 
            // fruityLoopsBackgroundPainterSigLine
            // 
            this.fruityLoopsBackgroundPainterSigLine.FruityType = ProgressODoom.FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer;
            // 
            // fruityLoopsProgressPainterSigLine
            // 
            this.fruityLoopsProgressPainterSigLine.FruityType = ProgressODoom.FruityLoopsProgressPainter.FruityLoopsProgressType.DoubleLayer;
            this.fruityLoopsProgressPainterSigLine.ProgressBorderPainter = null;
            // 
            // progressSignal
            // 
            this.progressSignal.BackColor = System.Drawing.Color.Black;
            this.progressSignal.BackgroundPainter = this.fruityLoopsBackgroundPainterSigLine;
            this.progressSignal.ForeColor = System.Drawing.Color.DeepPink;
            this.progressSignal.Location = new System.Drawing.Point(5, 54);
            this.progressSignal.MarqueePercentage = 50;
            this.progressSignal.MarqueeSpeed = 30;
            this.progressSignal.MarqueeStep = 1;
            this.progressSignal.Maximum = 200;
            this.progressSignal.Minimum = 0;
            this.progressSignal.Name = "progressSignal";
            this.progressSignal.ProgressPadding = 0;
            this.progressSignal.ProgressPainter = this.fruityLoopsProgressPainterSigLine;
            this.progressSignal.ProgressType = ProgressODoom.ProgressType.Smooth;
            this.progressSignal.ShowPercentage = false;
            this.progressSignal.Size = new System.Drawing.Size(230, 24);
            this.progressSignal.TabIndex = 23;
            this.progressSignal.Value = 0;
            // 
            // labelTS13
            // 
            this.labelTS13.AutoSize = true;
            this.labelTS13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS13.ForeColor = System.Drawing.Color.White;
            this.labelTS13.Image = null;
            this.labelTS13.Location = new System.Drawing.Point(44, 90);
            this.labelTS13.Name = "labelTS13";
            this.labelTS13.Size = new System.Drawing.Size(14, 13);
            this.labelTS13.TabIndex = 33;
            this.labelTS13.Text = "3";
            // 
            // labelTS11
            // 
            this.labelTS11.AutoSize = true;
            this.labelTS11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS11.ForeColor = System.Drawing.Color.White;
            this.labelTS11.Image = null;
            this.labelTS11.Location = new System.Drawing.Point(102, 101);
            this.labelTS11.Name = "labelTS11";
            this.labelTS11.Size = new System.Drawing.Size(36, 13);
            this.labelTS11.TabIndex = 32;
            this.labelTS11.Text = "PWR";
            // 
            // labelTS6
            // 
            this.labelTS6.AutoSize = true;
            this.labelTS6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS6.ForeColor = System.Drawing.Color.White;
            this.labelTS6.Image = null;
            this.labelTS6.Location = new System.Drawing.Point(204, 90);
            this.labelTS6.Name = "labelTS6";
            this.labelTS6.Size = new System.Drawing.Size(33, 13);
            this.labelTS6.TabIndex = 31;
            this.labelTS6.Text = "20";
            // 
            // labelTS12
            // 
            this.labelTS12.AutoSize = true;
            this.labelTS12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS12.ForeColor = System.Drawing.Color.White;
            this.labelTS12.Image = null;
            this.labelTS12.Location = new System.Drawing.Point(141, 90);
            this.labelTS12.Name = "labelTS12";
            this.labelTS12.Size = new System.Drawing.Size(21, 13);
            this.labelTS12.TabIndex = 29;
            this.labelTS12.Text = "10";
            // 
            // labelTS14
            // 
            this.labelTS14.AutoSize = true;
            this.labelTS14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS14.ForeColor = System.Drawing.Color.White;
            this.labelTS14.Image = null;
            this.labelTS14.Location = new System.Drawing.Point(73, 90);
            this.labelTS14.Name = "labelTS14";
            this.labelTS14.Size = new System.Drawing.Size(14, 13);
            this.labelTS14.TabIndex = 27;
            this.labelTS14.Text = "5";
            // 
            // labelTS15
            // 
            this.labelTS15.AutoSize = true;
            this.labelTS15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS15.ForeColor = System.Drawing.Color.White;
            this.labelTS15.Image = null;
            this.labelTS15.Location = new System.Drawing.Point(18, 90);
            this.labelTS15.Name = "labelTS15";
            this.labelTS15.Size = new System.Drawing.Size(14, 13);
            this.labelTS15.TabIndex = 26;
            this.labelTS15.Text = "1";
            // 
            // labelTS17
            // 
            this.labelTS17.AutoSize = true;
            this.labelTS17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS17.ForeColor = System.Drawing.Color.White;
            this.labelTS17.Image = null;
            this.labelTS17.Location = new System.Drawing.Point(1, 90);
            this.labelTS17.Name = "labelTS17";
            this.labelTS17.Size = new System.Drawing.Size(14, 13);
            this.labelTS17.TabIndex = 24;
            this.labelTS17.Text = "0";
            // 
            // labelTS10
            // 
            this.labelTS10.AutoSize = true;
            this.labelTS10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS10.ForeColor = System.Drawing.Color.White;
            this.labelTS10.Image = null;
            this.labelTS10.Location = new System.Drawing.Point(99, 10);
            this.labelTS10.Name = "labelTS10";
            this.labelTS10.Size = new System.Drawing.Size(42, 13);
            this.labelTS10.TabIndex = 11;
            this.labelTS10.Text = "Signal";
            // 
            // labelTS9
            // 
            this.labelTS9.AutoSize = true;
            this.labelTS9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS9.ForeColor = System.Drawing.Color.White;
            this.labelTS9.Image = null;
            this.labelTS9.Location = new System.Drawing.Point(199, 31);
            this.labelTS9.Name = "labelTS9";
            this.labelTS9.Size = new System.Drawing.Size(43, 13);
            this.labelTS9.TabIndex = 10;
            this.labelTS9.Text = "+60dB";
            // 
            // labelTS8
            // 
            this.labelTS8.AutoSize = true;
            this.labelTS8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS8.ForeColor = System.Drawing.Color.White;
            this.labelTS8.Image = null;
            this.labelTS8.Location = new System.Drawing.Point(172, 31);
            this.labelTS8.Name = "labelTS8";
            this.labelTS8.Size = new System.Drawing.Size(28, 13);
            this.labelTS8.TabIndex = 9;
            this.labelTS8.Text = "+40";
            // 
            // labelTS7
            // 
            this.labelTS7.AutoSize = true;
            this.labelTS7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS7.ForeColor = System.Drawing.Color.White;
            this.labelTS7.Image = null;
            this.labelTS7.Location = new System.Drawing.Point(144, 31);
            this.labelTS7.Name = "labelTS7";
            this.labelTS7.Size = new System.Drawing.Size(28, 13);
            this.labelTS7.TabIndex = 8;
            this.labelTS7.Text = "+20";
            // 
            // labelTS5
            // 
            this.labelTS5.AutoSize = true;
            this.labelTS5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS5.ForeColor = System.Drawing.Color.White;
            this.labelTS5.Image = null;
            this.labelTS5.Location = new System.Drawing.Point(113, 31);
            this.labelTS5.Name = "labelTS5";
            this.labelTS5.Size = new System.Drawing.Size(14, 13);
            this.labelTS5.TabIndex = 6;
            this.labelTS5.Text = "9";
            // 
            // labelTS4
            // 
            this.labelTS4.AutoSize = true;
            this.labelTS4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS4.ForeColor = System.Drawing.Color.White;
            this.labelTS4.Image = null;
            this.labelTS4.Location = new System.Drawing.Point(77, 31);
            this.labelTS4.Name = "labelTS4";
            this.labelTS4.Size = new System.Drawing.Size(14, 13);
            this.labelTS4.TabIndex = 5;
            this.labelTS4.Text = "5";
            // 
            // labelTS3
            // 
            this.labelTS3.AutoSize = true;
            this.labelTS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS3.ForeColor = System.Drawing.Color.White;
            this.labelTS3.Image = null;
            this.labelTS3.Location = new System.Drawing.Point(46, 31);
            this.labelTS3.Name = "labelTS3";
            this.labelTS3.Size = new System.Drawing.Size(14, 13);
            this.labelTS3.TabIndex = 4;
            this.labelTS3.Text = "3";
            // 
            // labelTS2
            // 
            this.labelTS2.AutoSize = true;
            this.labelTS2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS2.ForeColor = System.Drawing.Color.White;
            this.labelTS2.Image = null;
            this.labelTS2.Location = new System.Drawing.Point(21, 31);
            this.labelTS2.Name = "labelTS2";
            this.labelTS2.Size = new System.Drawing.Size(14, 13);
            this.labelTS2.TabIndex = 3;
            this.labelTS2.Text = "1";
            // 
            // labelTS1
            // 
            this.labelTS1.AutoSize = true;
            this.labelTS1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS1.ForeColor = System.Drawing.Color.White;
            this.labelTS1.Image = null;
            this.labelTS1.Location = new System.Drawing.Point(3, 31);
            this.labelTS1.Name = "labelTS1";
            this.labelTS1.Size = new System.Drawing.Size(14, 13);
            this.labelTS1.TabIndex = 2;
            this.labelTS1.Text = "0";
            // 
            // progressTop
            // 
            this.progressTop.BackColor = System.Drawing.Color.Black;
            this.progressTop.BackgroundPainter = this.fruityLoopsBackgroundPainterSWRLine;
            this.progressTop.ForeColor = System.Drawing.Color.DeepPink;
            this.progressTop.Location = new System.Drawing.Point(5, 45);
            this.progressTop.MarqueePercentage = 50;
            this.progressTop.MarqueeSpeed = 30;
            this.progressTop.MarqueeStep = 1;
            this.progressTop.Maximum = 200;
            this.progressTop.Minimum = 0;
            this.progressTop.Name = "progressTop";
            this.progressTop.ProgressPadding = 0;
            this.progressTop.ProgressPainter = this.fruityLoopsProgressPainterSWRLine;
            this.progressTop.ProgressType = ProgressODoom.ProgressType.Smooth;
            this.progressTop.ShowPercentage = false;
            this.progressTop.Size = new System.Drawing.Size(230, 7);
            this.progressTop.TabIndex = 34;
            this.progressTop.Value = 0;
            // 
            // progressBottom
            // 
            this.progressBottom.BackColor = System.Drawing.Color.Black;
            this.progressBottom.BackgroundPainter = this.fruityLoopsBackgroundPainterSWRLine;
            this.progressBottom.ForeColor = System.Drawing.Color.DeepPink;
            this.progressBottom.Location = new System.Drawing.Point(5, 80);
            this.progressBottom.MarqueePercentage = 50;
            this.progressBottom.MarqueeSpeed = 30;
            this.progressBottom.MarqueeStep = 1;
            this.progressBottom.Maximum = 200;
            this.progressBottom.Minimum = 0;
            this.progressBottom.Name = "progressBottom";
            this.progressBottom.ProgressPadding = 0;
            this.progressBottom.ProgressPainter = this.fruityLoopsProgressPainterSWRLine;
            this.progressBottom.ProgressType = ProgressODoom.ProgressType.Smooth;
            this.progressBottom.ShowPercentage = false;
            this.progressBottom.Size = new System.Drawing.Size(230, 7);
            this.progressBottom.TabIndex = 35;
            this.progressBottom.Value = 0;
            // 
            // SMeter1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.progressBottom);
            this.Controls.Add(this.progressTop);
            this.Controls.Add(this.labelTS13);
            this.Controls.Add(this.labelTS11);
            this.Controls.Add(this.labelTS6);
            this.Controls.Add(this.labelTS12);
            this.Controls.Add(this.labelTS14);
            this.Controls.Add(this.labelTS15);
            this.Controls.Add(this.labelTS17);
            this.Controls.Add(this.progressSignal);
            this.Controls.Add(this.labelTS10);
            this.Controls.Add(this.labelTS9);
            this.Controls.Add(this.labelTS8);
            this.Controls.Add(this.labelTS7);
            this.Controls.Add(this.labelTS5);
            this.Controls.Add(this.labelTS4);
            this.Controls.Add(this.labelTS3);
            this.Controls.Add(this.labelTS2);
            this.Controls.Add(this.labelTS1);
            this.MaximumSize = new System.Drawing.Size(240, 133);
            this.MinimumSize = new System.Drawing.Size(240, 133);
            this.Name = "SMeter1";
            this.Size = new System.Drawing.Size(240, 133);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LabelTS labelTS1;
        private System.Windows.Forms.LabelTS labelTS2;
        private System.Windows.Forms.LabelTS labelTS3;
        private System.Windows.Forms.LabelTS labelTS4;
        private System.Windows.Forms.LabelTS labelTS5;
        private System.Windows.Forms.LabelTS labelTS7;
        private System.Windows.Forms.LabelTS labelTS8;
        private System.Windows.Forms.LabelTS labelTS9;
        private System.Windows.Forms.LabelTS labelTS10;
        private ProgressODoom.ProgressBarEx progressSignal;
        public ProgressODoom.FruityLoopsProgressPainter fruityLoopsProgressPainterSigLine;
        private ProgressODoom.FruityLoopsBackgroundPainter fruityLoopsBackgroundPainterSigLine;
        public ProgressODoom.FruityLoopsProgressPainter fruityLoopsProgressPainterSWRLine;
        private ProgressODoom.FruityLoopsBackgroundPainter fruityLoopsBackgroundPainterSWRLine;
        private LabelTS labelTS6;
        private LabelTS labelTS12;
        private LabelTS labelTS14;
        private LabelTS labelTS15;
        private LabelTS labelTS17;
        private LabelTS labelTS13;
        private LabelTS labelTS11;
        private ProgressODoom.ProgressBarEx progressTop;
        private ProgressODoom.ProgressBarEx progressBottom;
    }
}
