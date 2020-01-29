namespace FormEditSound
{
    partial class FormEditSound
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
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.radioButtonBeep = new System.Windows.Forms.RadioButton();
            this.radioButtonWav = new System.Windows.Forms.RadioButton();
            this.radioButtonTts = new System.Windows.Forms.RadioButton();
            this.textBoxSound = new System.Windows.Forms.TextBox();
            this.buttonInsCapture = new System.Windows.Forms.Button();
            this.comboBoxGroups = new System.Windows.Forms.ComboBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.AutoSize = true;
            this.radioButtonNone.Location = new System.Drawing.Point(13, 13);
            this.radioButtonNone.Name = "radioButtonNone";
            this.radioButtonNone.Size = new System.Drawing.Size(51, 17);
            this.radioButtonNone.TabIndex = 3;
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.Text = "None";
            this.radioButtonNone.UseVisualStyleBackColor = true;
            this.radioButtonNone.CheckedChanged += new System.EventHandler(this.radioButtonNone_CheckedChanged);
            // 
            // radioButtonBeep
            // 
            this.radioButtonBeep.AutoSize = true;
            this.radioButtonBeep.Location = new System.Drawing.Point(13, 37);
            this.radioButtonBeep.Name = "radioButtonBeep";
            this.radioButtonBeep.Size = new System.Drawing.Size(50, 17);
            this.radioButtonBeep.TabIndex = 4;
            this.radioButtonBeep.TabStop = true;
            this.radioButtonBeep.Text = "Beep";
            this.radioButtonBeep.UseVisualStyleBackColor = true;
            this.radioButtonBeep.CheckedChanged += new System.EventHandler(this.radioButtonBeep_CheckedChanged);
            // 
            // radioButtonWav
            // 
            this.radioButtonWav.AutoSize = true;
            this.radioButtonWav.Location = new System.Drawing.Point(71, 13);
            this.radioButtonWav.Name = "radioButtonWav";
            this.radioButtonWav.Size = new System.Drawing.Size(53, 17);
            this.radioButtonWav.TabIndex = 5;
            this.radioButtonWav.TabStop = true;
            this.radioButtonWav.Text = "WAV:";
            this.radioButtonWav.UseVisualStyleBackColor = true;
            this.radioButtonWav.CheckedChanged += new System.EventHandler(this.radioButtonWav_CheckedChanged);
            // 
            // radioButtonTts
            // 
            this.radioButtonTts.AutoSize = true;
            this.radioButtonTts.Location = new System.Drawing.Point(71, 37);
            this.radioButtonTts.Name = "radioButtonTts";
            this.radioButtonTts.Size = new System.Drawing.Size(49, 17);
            this.radioButtonTts.TabIndex = 6;
            this.radioButtonTts.TabStop = true;
            this.radioButtonTts.Text = "TTS:";
            this.radioButtonTts.UseVisualStyleBackColor = true;
            this.radioButtonTts.CheckedChanged += new System.EventHandler(this.radioButtonTts_CheckedChanged);
            // 
            // textBoxSound
            // 
            this.textBoxSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.textBoxSound, "Text to speech expression or wav file name");
            this.textBoxSound.Location = new System.Drawing.Point(130, 13);
            this.textBoxSound.Name = "textBoxSound";
            this.helpProvider1.SetShowHelp(this.textBoxSound, true);
            this.textBoxSound.Size = new System.Drawing.Size(288, 20);
            this.textBoxSound.TabIndex = 0;
            // 
            // buttonInsCapture
            // 
            this.buttonInsCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInsCapture.Enabled = false;
            this.buttonInsCapture.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.helpProvider1.SetHelpString(this.buttonInsCapture, "Insert the selected capture into the alert sound");
            this.buttonInsCapture.Location = new System.Drawing.Point(293, 37);
            this.buttonInsCapture.Name = "buttonInsCapture";
            this.helpProvider1.SetShowHelp(this.buttonInsCapture, true);
            this.buttonInsCapture.Size = new System.Drawing.Size(32, 23);
            this.buttonInsCapture.TabIndex = 8;
            this.buttonInsCapture.Text = "£";
            this.buttonInsCapture.UseVisualStyleBackColor = true;
            this.buttonInsCapture.Click += new System.EventHandler(this.buttonInsCapture_Click);
            // 
            // comboBoxGroups
            // 
            this.comboBoxGroups.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.comboBoxGroups, "Available capture names from the regular expression");
            this.comboBoxGroups.Location = new System.Drawing.Point(130, 39);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.helpProvider1.SetShowHelp(this.comboBoxGroups, true);
            this.comboBoxGroups.Size = new System.Drawing.Size(153, 21);
            this.comboBoxGroups.TabIndex = 7;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(168, 71);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(70, 23);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(250, 71);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.buttonBrowse, "Browse for wav file");
            this.buttonBrowse.Location = new System.Drawing.Point(424, 11);
            this.buttonBrowse.Name = "buttonBrowse";
            this.helpProvider1.SetShowHelp(this.buttonBrowse, true);
            this.buttonBrowse.Size = new System.Drawing.Size(25, 23);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.helpProvider1.SetHelpString(this.buttonPlay, "Play the alert sound");
            this.buttonPlay.Location = new System.Drawing.Point(455, 11);
            this.buttonPlay.Name = "buttonPlay";
            this.helpProvider1.SetShowHelp(this.buttonPlay, true);
            this.buttonPlay.Size = new System.Drawing.Size(25, 23);
            this.buttonPlay.TabIndex = 2;
            this.buttonPlay.Text = "u";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "wav files|*.wav";
            // 
            // FormEditSound
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 106);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.comboBoxGroups);
            this.Controls.Add(this.buttonInsCapture);
            this.Controls.Add(this.textBoxSound);
            this.Controls.Add(this.radioButtonTts);
            this.Controls.Add(this.radioButtonWav);
            this.Controls.Add(this.radioButtonBeep);
            this.Controls.Add(this.radioButtonNone);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditSound";
            this.helpProvider1.SetShowHelp(this, true);
            this.ShowIcon = false;
            this.Text = "Edit Alert Sound";
            this.Shown += new System.EventHandler(this.FormEditSound_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonNone;
        private System.Windows.Forms.RadioButton radioButtonBeep;
        private System.Windows.Forms.RadioButton radioButtonWav;
        private System.Windows.Forms.RadioButton radioButtonTts;
        private System.Windows.Forms.TextBox textBoxSound;
        private System.Windows.Forms.Button buttonInsCapture;
        private System.Windows.Forms.ComboBox comboBoxGroups;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}

