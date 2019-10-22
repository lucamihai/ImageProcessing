namespace ComputerVision
{
    partial class MainForm
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
            this.panelSource = new System.Windows.Forms.Panel();
            this.panelDestination = new System.Windows.Forms.Panel();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonNegativare = new System.Windows.Forms.Button();
            this.buttonGrayscale = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.trackBarDelta = new System.Windows.Forms.TrackBar();
            this.trackBarIntensity = new System.Windows.Forms.TrackBar();
            this.buttonEgalizare = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarIntensity)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSource
            // 
            this.panelSource.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelSource.Location = new System.Drawing.Point(12, 12);
            this.panelSource.Name = "panelSource";
            this.panelSource.Size = new System.Drawing.Size(320, 240);
            this.panelSource.TabIndex = 0;
            // 
            // panelDestination
            // 
            this.panelDestination.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelDestination.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDestination.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelDestination.Location = new System.Drawing.Point(348, 12);
            this.panelDestination.Name = "panelDestination";
            this.panelDestination.Size = new System.Drawing.Size(320, 240);
            this.panelDestination.TabIndex = 1;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(12, 439);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.LoadClick);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.buttonEgalizare);
            this.panel1.Controls.Add(this.buttonNegativare);
            this.panel1.Controls.Add(this.buttonGrayscale);
            this.panel1.Location = new System.Drawing.Point(348, 271);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 190);
            this.panel1.TabIndex = 3;
            // 
            // buttonNegativare
            // 
            this.buttonNegativare.Location = new System.Drawing.Point(7, 126);
            this.buttonNegativare.Name = "buttonNegativare";
            this.buttonNegativare.Size = new System.Drawing.Size(75, 23);
            this.buttonNegativare.TabIndex = 14;
            this.buttonNegativare.Text = "Negativare";
            this.buttonNegativare.UseVisualStyleBackColor = true;
            this.buttonNegativare.Click += new System.EventHandler(this.NegateClick);
            // 
            // buttonGrayscale
            // 
            this.buttonGrayscale.Location = new System.Drawing.Point(7, 155);
            this.buttonGrayscale.Name = "buttonGrayscale";
            this.buttonGrayscale.Size = new System.Drawing.Size(75, 23);
            this.buttonGrayscale.TabIndex = 13;
            this.buttonGrayscale.Text = "Grayscale";
            this.buttonGrayscale.UseVisualStyleBackColor = true;
            this.buttonGrayscale.Click += new System.EventHandler(this.GrayScaleClick);
            // 
            // trackBarDelta
            // 
            this.trackBarDelta.Location = new System.Drawing.Point(12, 355);
            this.trackBarDelta.Maximum = 255;
            this.trackBarDelta.Minimum = -255;
            this.trackBarDelta.Name = "trackBarDelta";
            this.trackBarDelta.Size = new System.Drawing.Size(320, 45);
            this.trackBarDelta.TabIndex = 4;
            this.trackBarDelta.ValueChanged += new System.EventHandler(this.TrackBarDelta_ValueChanged);
            // 
            // trackBarIntensity
            // 
            this.trackBarIntensity.Location = new System.Drawing.Point(12, 304);
            this.trackBarIntensity.Maximum = 120;
            this.trackBarIntensity.Minimum = -120;
            this.trackBarIntensity.Name = "trackBarIntensity";
            this.trackBarIntensity.Size = new System.Drawing.Size(320, 45);
            this.trackBarIntensity.TabIndex = 5;
            this.trackBarIntensity.ValueChanged += new System.EventHandler(this.TrackBarIntensity_ValueChanged);
            // 
            // buttonEgalizare
            // 
            this.buttonEgalizare.Location = new System.Drawing.Point(7, 97);
            this.buttonEgalizare.Name = "buttonEgalizare";
            this.buttonEgalizare.Size = new System.Drawing.Size(75, 23);
            this.buttonEgalizare.TabIndex = 15;
            this.buttonEgalizare.Text = "Egalizare";
            this.buttonEgalizare.UseVisualStyleBackColor = true;
            this.buttonEgalizare.Click += new System.EventHandler(this.ButtonEgalizare_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 473);
            this.Controls.Add(this.trackBarIntensity);
            this.Controls.Add(this.trackBarDelta);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.panelDestination);
            this.Controls.Add(this.panelSource);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarIntensity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSource;
        private System.Windows.Forms.Panel panelDestination;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonGrayscale;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonNegativare;
        private System.Windows.Forms.TrackBar trackBarDelta;
        private System.Windows.Forms.TrackBar trackBarIntensity;
        private System.Windows.Forms.Button buttonEgalizare;
    }
}

