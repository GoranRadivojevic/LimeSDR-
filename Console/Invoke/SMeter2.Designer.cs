using System;
using System.Windows.Forms;
using System.Drawing;

namespace PowerSDR.Invoke
{
    partial class SMeter2
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
            this.picSignalLine = new System.Windows.Forms.PictureBox();
            this.picLineUp = new System.Windows.Forms.PictureBox();
            this.picLineDown = new System.Windows.Forms.PictureBox();
            this.labelTS15 = new System.Windows.Forms.LabelTS();
            this.labelTS14 = new System.Windows.Forms.LabelTS();
            this.labelTS11 = new System.Windows.Forms.LabelTS();
            this.labelTS6 = new System.Windows.Forms.LabelTS();
            this.labelTS9 = new System.Windows.Forms.LabelTS();
            this.lblPwr_Swr = new System.Windows.Forms.LabelTS();
            this.labelTS13 = new System.Windows.Forms.LabelTS();
            this.labelTS12 = new System.Windows.Forms.LabelTS();
            this.labelTS10 = new System.Windows.Forms.LabelTS();
            this.labelTS8 = new System.Windows.Forms.LabelTS();
            this.lblSignal = new System.Windows.Forms.LabelTS();
            this.labelTS7 = new System.Windows.Forms.LabelTS();
            this.labelTS5 = new System.Windows.Forms.LabelTS();
            this.labelTS4 = new System.Windows.Forms.LabelTS();
            this.labelTS3 = new System.Windows.Forms.LabelTS();
            this.labelTS2 = new System.Windows.Forms.LabelTS();
            this.labelTS1 = new System.Windows.Forms.LabelTS();
            ((System.ComponentModel.ISupportInitialize)(this.picSignalLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLineUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLineDown)).BeginInit();
            this.SuspendLayout();
            // 
            // picSignalLine
            // 
            this.picSignalLine.Location = new System.Drawing.Point(26, 56);
            this.picSignalLine.Name = "picSignalLine";
            this.picSignalLine.Size = new System.Drawing.Size(186, 20);
            this.picSignalLine.TabIndex = 34;
            this.picSignalLine.TabStop = false;
            this.picSignalLine.Paint += new System.Windows.Forms.PaintEventHandler(this.SMeter_Paint);
            // 
            // picLineUp
            // 
            this.picLineUp.Location = new System.Drawing.Point(28, 44);
            this.picLineUp.Name = "picLineUp";
            this.picLineUp.Size = new System.Drawing.Size(184, 2);
            this.picLineUp.TabIndex = 37;
            this.picLineUp.TabStop = false;
            this.picLineUp.Paint += new System.Windows.Forms.PaintEventHandler(this.picLineUp_Paint);
            // 
            // picLineDown
            // 
            this.picLineDown.Location = new System.Drawing.Point(28, 86);
            this.picLineDown.Name = "picLineDown";
            this.picLineDown.Size = new System.Drawing.Size(184, 2);
            this.picLineDown.TabIndex = 38;
            this.picLineDown.TabStop = false;
            this.picLineDown.Paint += new System.Windows.Forms.PaintEventHandler(this.picLineDown_Paint);
            // 
            // labelTS15
            // 
            this.labelTS15.AutoSize = true;
            this.labelTS15.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS15.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS15.Image = null;
            this.labelTS15.Location = new System.Drawing.Point(213, 77);
            this.labelTS15.Name = "labelTS15";
            this.labelTS15.Size = new System.Drawing.Size(29, 23);
            this.labelTS15.TabIndex = 40;
            this.labelTS15.Text = "W";
            // 
            // labelTS14
            // 
            this.labelTS14.AutoSize = true;
            this.labelTS14.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS14.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS14.Image = null;
            this.labelTS14.Location = new System.Drawing.Point(210, 32);
            this.labelTS14.Name = "labelTS14";
            this.labelTS14.Size = new System.Drawing.Size(35, 23);
            this.labelTS14.TabIndex = 39;
            this.labelTS14.Text = "dB";
            // 
            // labelTS11
            // 
            this.labelTS11.AutoSize = true;
            this.labelTS11.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS11.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS11.Image = null;
            this.labelTS11.Location = new System.Drawing.Point(108, 96);
            this.labelTS11.Name = "labelTS11";
            this.labelTS11.Size = new System.Drawing.Size(17, 16);
            this.labelTS11.TabIndex = 36;
            this.labelTS11.Text = "7";
            // 
            // labelTS6
            // 
            this.labelTS6.AutoSize = true;
            this.labelTS6.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS6.ForeColor = System.Drawing.Color.Red;
            this.labelTS6.Image = null;
            this.labelTS6.Location = new System.Drawing.Point(168, 18);
            this.labelTS6.Name = "labelTS6";
            this.labelTS6.Size = new System.Drawing.Size(26, 16);
            this.labelTS6.TabIndex = 35;
            this.labelTS6.Text = "40";
            // 
            // labelTS9
            // 
            this.labelTS9.AutoSize = true;
            this.labelTS9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS9.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS9.Image = null;
            this.labelTS9.Location = new System.Drawing.Point(47, 96);
            this.labelTS9.Name = "labelTS9";
            this.labelTS9.Size = new System.Drawing.Size(17, 16);
            this.labelTS9.TabIndex = 33;
            this.labelTS9.Text = "3";
            // 
            // lblPwr_Swr
            // 
            this.lblPwr_Swr.AutoSize = true;
            this.lblPwr_Swr.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblPwr_Swr.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblPwr_Swr.Image = null;
            this.lblPwr_Swr.Location = new System.Drawing.Point(4, 73);
            this.lblPwr_Swr.Name = "lblPwr_Swr";
            this.lblPwr_Swr.Size = new System.Drawing.Size(24, 23);
            this.lblPwr_Swr.TabIndex = 32;
            this.lblPwr_Swr.Text = "P";
            // 
            // labelTS13
            // 
            this.labelTS13.AutoSize = true;
            this.labelTS13.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS13.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS13.Image = null;
            this.labelTS13.Location = new System.Drawing.Point(187, 96);
            this.labelTS13.Name = "labelTS13";
            this.labelTS13.Size = new System.Drawing.Size(26, 16);
            this.labelTS13.TabIndex = 31;
            this.labelTS13.Text = "20";
            // 
            // labelTS12
            // 
            this.labelTS12.AutoSize = true;
            this.labelTS12.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS12.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS12.Image = null;
            this.labelTS12.Location = new System.Drawing.Point(142, 96);
            this.labelTS12.Name = "labelTS12";
            this.labelTS12.Size = new System.Drawing.Size(26, 16);
            this.labelTS12.TabIndex = 29;
            this.labelTS12.Text = "10";
            // 
            // labelTS10
            // 
            this.labelTS10.AutoSize = true;
            this.labelTS10.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS10.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS10.Image = null;
            this.labelTS10.Location = new System.Drawing.Point(73, 96);
            this.labelTS10.Name = "labelTS10";
            this.labelTS10.Size = new System.Drawing.Size(17, 16);
            this.labelTS10.TabIndex = 27;
            this.labelTS10.Text = "5";
            // 
            // labelTS8
            // 
            this.labelTS8.AutoSize = true;
            this.labelTS8.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS8.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS8.Image = null;
            this.labelTS8.Location = new System.Drawing.Point(23, 96);
            this.labelTS8.Name = "labelTS8";
            this.labelTS8.Size = new System.Drawing.Size(17, 16);
            this.labelTS8.TabIndex = 24;
            this.labelTS8.Text = "1";
            // 
            // lblSignal
            // 
            this.lblSignal.AutoSize = true;
            this.lblSignal.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSignal.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblSignal.Image = null;
            this.lblSignal.Location = new System.Drawing.Point(3, 29);
            this.lblSignal.Name = "lblSignal";
            this.lblSignal.Size = new System.Drawing.Size(24, 23);
            this.lblSignal.TabIndex = 11;
            this.lblSignal.Text = "S";
            // 
            // labelTS7
            // 
            this.labelTS7.AutoSize = true;
            this.labelTS7.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS7.ForeColor = System.Drawing.Color.Red;
            this.labelTS7.Image = null;
            this.labelTS7.Location = new System.Drawing.Point(195, 18);
            this.labelTS7.Name = "labelTS7";
            this.labelTS7.Size = new System.Drawing.Size(26, 16);
            this.labelTS7.TabIndex = 10;
            this.labelTS7.Text = "60";
            // 
            // labelTS5
            // 
            this.labelTS5.AutoSize = true;
            this.labelTS5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS5.ForeColor = System.Drawing.Color.Red;
            this.labelTS5.Image = null;
            this.labelTS5.Location = new System.Drawing.Point(142, 18);
            this.labelTS5.Name = "labelTS5";
            this.labelTS5.Size = new System.Drawing.Size(26, 16);
            this.labelTS5.TabIndex = 8;
            this.labelTS5.Text = "20";
            // 
            // labelTS4
            // 
            this.labelTS4.AutoSize = true;
            this.labelTS4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS4.Image = null;
            this.labelTS4.Location = new System.Drawing.Point(113, 18);
            this.labelTS4.Name = "labelTS4";
            this.labelTS4.Size = new System.Drawing.Size(17, 16);
            this.labelTS4.TabIndex = 6;
            this.labelTS4.Text = "9";
            // 
            // labelTS3
            // 
            this.labelTS3.AutoSize = true;
            this.labelTS3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS3.Image = null;
            this.labelTS3.Location = new System.Drawing.Point(76, 18);
            this.labelTS3.Name = "labelTS3";
            this.labelTS3.Size = new System.Drawing.Size(17, 16);
            this.labelTS3.TabIndex = 5;
            this.labelTS3.Text = "5";
            // 
            // labelTS2
            // 
            this.labelTS2.AutoSize = true;
            this.labelTS2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS2.Image = null;
            this.labelTS2.Location = new System.Drawing.Point(45, 18);
            this.labelTS2.Name = "labelTS2";
            this.labelTS2.Size = new System.Drawing.Size(17, 16);
            this.labelTS2.TabIndex = 4;
            this.labelTS2.Text = "3";
            // 
            // labelTS1
            // 
            this.labelTS1.AutoSize = true;
            this.labelTS1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTS1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelTS1.Image = null;
            this.labelTS1.Location = new System.Drawing.Point(25, 18);
            this.labelTS1.Name = "labelTS1";
            this.labelTS1.Size = new System.Drawing.Size(17, 16);
            this.labelTS1.TabIndex = 2;
            this.labelTS1.Text = "1";
            // 
            // SMeter2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.labelTS15);
            this.Controls.Add(this.labelTS14);
            this.Controls.Add(this.picLineDown);
            this.Controls.Add(this.picLineUp);
            this.Controls.Add(this.labelTS11);
            this.Controls.Add(this.labelTS6);
            this.Controls.Add(this.picSignalLine);
            this.Controls.Add(this.labelTS9);
            this.Controls.Add(this.lblPwr_Swr);
            this.Controls.Add(this.labelTS13);
            this.Controls.Add(this.labelTS12);
            this.Controls.Add(this.labelTS10);
            this.Controls.Add(this.labelTS8);
            this.Controls.Add(this.lblSignal);
            this.Controls.Add(this.labelTS7);
            this.Controls.Add(this.labelTS5);
            this.Controls.Add(this.labelTS4);
            this.Controls.Add(this.labelTS3);
            this.Controls.Add(this.labelTS2);
            this.Controls.Add(this.labelTS1);
            this.MaximumSize = new System.Drawing.Size(240, 133);
            this.MinimumSize = new System.Drawing.Size(240, 133);
            this.Name = "SMeter2";
            this.Size = new System.Drawing.Size(240, 133);
            ((System.ComponentModel.ISupportInitialize)(this.picSignalLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLineUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLineDown)).EndInit();
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
        private System.Windows.Forms.LabelTS lblSignal;
        private LabelTS labelTS13;
        private LabelTS labelTS12;
        private LabelTS labelTS10;
        private LabelTS labelTS8;
        private LabelTS labelTS9;
        private LabelTS lblPwr_Swr;
        private PictureBox picSignalLine;
        private LabelTS labelTS6;
        private LabelTS labelTS11;
        private PictureBox picLineUp;
        private PictureBox picLineDown;
        private LabelTS labelTS14;
        private LabelTS labelTS15;
    }
}
