﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace ACT_TriggerTree
{
    public partial class FormEditSound : Form
    {
        CustomTrigger editingTrigger;               //a reference to the original trigger

        // macro validity indicators
        ImageList macroIcons = new ImageList();

        public event EventHandler EditDoneEvent;    //callback
        //callback event argument
        public class EditEventArgs : EventArgs
        {
            public CustomTrigger editedTrigger;

            public EditEventArgs(CustomTrigger trigger)
            {
                editedTrigger = trigger;
            }
        }

        public FormEditSound()
        {
            InitializeComponent();
        }

        public FormEditSound(CustomTrigger trig, EventHandler eventHandler)
        {
            InitializeComponent();

            editingTrigger = trig;
            EditDoneEvent = eventHandler;
        }

        protected void OnEditDoneEvent(EventArgs e)
        {
            if (EditDoneEvent != null)
            {
                EventHandler handler = EditDoneEvent;
                handler.Invoke(this, e);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            editingTrigger.SoundData = textBoxSound.Text;
            if (radioButtonNone.Checked)
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.None;
            else if (radioButtonBeep.Checked)
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.Beep;
            if (radioButtonWav.Checked)
            {
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.WAV;
                if(!File.Exists(textBoxSound.Text))
                {
                    MessageBox.Show(this, "No such file");
                    return;
                }
            }
            if (radioButtonTts.Checked)
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.TTS;

            EditEventArgs arg = new EditEventArgs(editingTrigger);
            OnEditDoneEvent(arg);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormEditSound_Shown(object sender, EventArgs e)
        {
            if (editingTrigger != null) //so the form can come up for standalone testing
            {
                textBoxSound.Text = editingTrigger.SoundData;
                this.Text = "Edit Alert Sound: " + editingTrigger.ShortRegexString;

                macroIcons.Images.Add(Macros.GetActionBitmap());
                macroIcons.Images.Add(Macros.GetActionNotBitmap());

                switch (editingTrigger.SoundType)
                {
                    case (int)CustomTriggerSoundTypeEnum.TTS:
                        radioButtonTts.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonInsCapture.Enabled = true;
                        textBoxSound.Focus();
                        textBoxSound.SelectAll();
                        break;
                    case (int)CustomTriggerSoundTypeEnum.Beep:
                        radioButtonBeep.Checked = true;
                        buttonPlay.Enabled = true;
                        textBoxSound.Enabled = false;
                        break;
                    case (int)CustomTriggerSoundTypeEnum.WAV:
                        radioButtonWav.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonBrowse.Enabled = true;
                        textBoxSound.Focus();
                        textBoxSound.SelectAll();
                        break;
                    default:
                        radioButtonNone.Checked = true;
                        textBoxSound.Enabled = false;
                        break;
                }

                if (string.IsNullOrEmpty(editingTrigger.SoundData))
                    pictureBoxTts.Visible = false;
                else
                {
                    pictureBoxTts.Visible = true;
                    if (Macros.IsInvalidMacro(editingTrigger.SoundData))
                        pictureBoxTts.Image = macroIcons.Images[1];
                    else
                        pictureBoxTts.Image = macroIcons.Images[0];
                }

                string[] groups = editingTrigger.RegEx.GetGroupNames();
                for (int i = 1; i < groups.Length; i++) //skip group[0], it is the entire expression
                {
                    comboBoxGroups.Items.Add(groups[i]);
                }
            }
        }

        private void buttonInsCapture_Click(object sender, EventArgs e)
        {
            string group = comboBoxGroups.Text;
            if (!string.IsNullOrEmpty(group))
            {
                int i = 0;
                bool result = int.TryParse(group, out i);
                if (!result)
                    group = "{" + group + "}";
                string insert = "$" + group;

                textBoxSound.Text = textBoxSound.Text.Insert(textBoxSound.SelectionStart, insert);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            string dir = Environment.GetEnvironmentVariable("SYSTEMROOT");
            openFileDialog1.InitialDirectory = dir + @"\Media";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxSound.Text = openFileDialog1.FileName;
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (radioButtonTts.Checked)
                ActGlobals.oFormActMain.TTS(textBoxSound.Text);
            else if (radioButtonWav.Checked)
            {
                if (File.Exists(textBoxSound.Text))
                    ActGlobals.oFormActMain.PlaySoundWinApi(textBoxSound.Text, 100);
                else
                    MessageBox.Show(this, "No such file");
            }
            else if (radioButtonBeep.Checked)
                System.Media.SystemSounds.Beep.Play();
        }

        private void radioButtonNone_CheckedChanged(object sender, EventArgs e)
        {
            buttonBrowse.Enabled = false;
            buttonPlay.Enabled = false;
            textBoxSound.Enabled = false;
            buttonInsCapture.Enabled = false;
        }

        private void radioButtonBeep_CheckedChanged(object sender, EventArgs e)
        {
            buttonBrowse.Enabled = false;
            buttonPlay.Enabled = true;
            textBoxSound.Enabled = false;
            buttonInsCapture.Enabled = false;
        }

        private void radioButtonWav_CheckedChanged(object sender, EventArgs e)
        {
            buttonBrowse.Enabled = true;
            buttonPlay.Enabled = true;
            textBoxSound.Enabled = true;
            buttonInsCapture.Enabled = false;
        }

        private void radioButtonTts_CheckedChanged(object sender, EventArgs e)
        {
            buttonBrowse.Enabled = false;
            buttonPlay.Enabled = true;
            textBoxSound.Enabled = true;
            buttonInsCapture.Enabled = true;
        }

        private void textBoxSound_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSound.Text))
                pictureBoxTts.Visible = false;
            else
            {
                if (macroIcons.Images.Count > 0)
                {
                    pictureBoxTts.Visible = true;
                    if (Macros.IsInvalidMacro(textBoxSound.Text))
                        pictureBoxTts.Image = macroIcons.Images[1];
                    else
                        pictureBoxTts.Image = macroIcons.Images[0];
                }
            }
        }
    }
}
