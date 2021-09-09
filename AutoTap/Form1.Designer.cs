using System;

namespace AutoTap
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmbWindows = new System.Windows.Forms.ComboBox();
            this.lstPoints = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblSelectedWindow = new System.Windows.Forms.Label();
            this.btnPlay = new System.Windows.Forms.Button();
            this.udTime = new System.Windows.Forms.NumericUpDown();
            this.btnClear = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.udX = new System.Windows.Forms.NumericUpDown();
            this.udY = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udTime)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udY)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbWindows
            // 
            this.cmbWindows.FormattingEnabled = true;
            this.cmbWindows.Location = new System.Drawing.Point(13, 31);
            this.cmbWindows.Name = "cmbWindows";
            this.cmbWindows.Size = new System.Drawing.Size(213, 21);
            this.cmbWindows.TabIndex = 0;
            this.cmbWindows.SelectedIndexChanged += new System.EventHandler(this.cmbWindows_SelectedIndexChanged);
            // 
            // lstPoints
            // 
            this.lstPoints.FormattingEnabled = true;
            this.lstPoints.Location = new System.Drawing.Point(13, 98);
            this.lstPoints.Name = "lstPoints";
            this.lstPoints.Size = new System.Drawing.Size(213, 173);
            this.lstPoints.TabIndex = 1;
            this.lstPoints.SelectedIndexChanged += new System.EventHandler(this.lstPoints_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(13, 65);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblSelectedWindow
            // 
            this.lblSelectedWindow.AutoSize = true;
            this.lblSelectedWindow.Location = new System.Drawing.Point(108, 70);
            this.lblSelectedWindow.Name = "lblSelectedWindow";
            this.lblSelectedWindow.Size = new System.Drawing.Size(118, 13);
            this.lblSelectedWindow.TabIndex = 3;
            this.lblSelectedWindow.Text = "Please select a window";
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(13, 306);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // udTime
            // 
            this.udTime.Location = new System.Drawing.Point(167, 277);
            this.udTime.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udTime.Name = "udTime";
            this.udTime.Size = new System.Drawing.Size(59, 20);
            this.udTime.TabIndex = 5;
            this.udTime.ValueChanged += new System.EventHandler(this.udTime_ValueChanged);
            this.udTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udTime_KeyUp);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(151, 306);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLoad,
            this.mnuSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(239, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuLoad
            // 
            this.mnuLoad.Name = "mnuLoad";
            this.mnuLoad.Size = new System.Drawing.Size(45, 20);
            this.mnuLoad.Text = "Load";
            this.mnuLoad.Click += new System.EventHandler(this.mnuLoad_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(43, 20);
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // udX
            // 
            this.udX.Location = new System.Drawing.Point(13, 277);
            this.udX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udX.Name = "udX";
            this.udX.Size = new System.Drawing.Size(71, 20);
            this.udX.TabIndex = 8;
            this.udX.ValueChanged += new System.EventHandler(this.udX_ValueChanged);
            this.udX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udX_KeyUp);
            // 
            // udY
            // 
            this.udY.Location = new System.Drawing.Point(90, 277);
            this.udY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udY.Name = "udY";
            this.udY.Size = new System.Drawing.Size(71, 20);
            this.udY.TabIndex = 9;
            this.udY.ValueChanged += new System.EventHandler(this.udY_ValueChanged);
            this.udY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udY_KeyUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 339);
            this.Controls.Add(this.udY);
            this.Controls.Add(this.udX);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.udTime);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.lblSelectedWindow);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lstPoints);
            this.Controls.Add(this.cmbWindows);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Auto Tap";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.udTime)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbWindows;
        private System.Windows.Forms.ListBox lstPoints;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblSelectedWindow;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.NumericUpDown udTime;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuLoad;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NumericUpDown udX;
        private System.Windows.Forms.NumericUpDown udY;
    }
}

