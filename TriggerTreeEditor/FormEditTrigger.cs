using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace TriggerTreeEditor
{
    public partial class FormEditTrigger : Form
    {
        const int logTimeStampLength = 39;  //# of chars in the log file timestamp
        const string logTimeStampRegexStr = @"^\(\d{10}\)\[.{24}\] ";
        Regex parsePaste = new Regex(logTimeStampRegexStr + @"(?<expr>[^\r\n]*)", RegexOptions.Compiled);

        CustomTrigger editingTrigger;       //a copy of the original trigger
        CustomTrigger undoTrigger;          //a reference to the original trigger
        string zoneCategory;
        bool regexChanged = false;          //track for replace / create new
        bool initializing = true;           //oncheck() methods do not need to do anything during shown()

        public bool haveOriginal = true;    //set false by parent when creating a brand new trigger

        public FormEditTrigger()
        {
            InitializeComponent();
        }

        public FormEditTrigger(CustomTrigger trigger, string category)
        {
            InitializeComponent();

            zoneCategory = category;
            undoTrigger = trigger;
            editingTrigger = new CustomTrigger(trigger.RegEx.ToString(), trigger.SoundType, trigger.SoundData, trigger.Timer, trigger.TimerName, trigger.Tabbed);
            editingTrigger.Category = trigger.Category;
            editingTrigger.RestrictToCategoryZone = trigger.RestrictToCategoryZone;
        }

        private void FormEditTrigger_Shown(object sender, EventArgs e)
        {
            if (editingTrigger != null)
            {
                textBoxRegex.Text = editingTrigger.ShortRegexString;
                textBoxCategory.Text = editingTrigger.Category;
                textBoxSound.Text = editingTrigger.SoundData;
                switch (editingTrigger.SoundType)
                {
                    case (int)CustomTriggerSoundTypeEnum.Beep:
                        radioButtonBeep.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonFileOpen.Enabled = false;
                        textBoxSound.Enabled = false;
                        buttonInsert.Enabled = false;
                        comboBoxGroups.Enabled = false;
                        break;
                    case (int)CustomTriggerSoundTypeEnum.WAV:
                        radioButtonWav.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonFileOpen.Enabled = true;
                        textBoxSound.Enabled = true;
                        buttonInsert.Enabled = false;
                        comboBoxGroups.Enabled = false;
                        break;
                    case (int)CustomTriggerSoundTypeEnum.TTS:
                        radioButtonTts.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonFileOpen.Enabled = false;
                        textBoxSound.Enabled = true;
                        buttonInsert.Enabled = false;
                        comboBoxGroups.Enabled = false;
                        break;
                    default:
                        radioButtonNone.Checked = true;
                        buttonPlay.Enabled = false;
                        buttonFileOpen.Enabled = false;
                        textBoxSound.Enabled = false;
                        buttonInsert.Enabled = false;
                        comboBoxGroups.Enabled = false;
                        break;
                }
                textBoxTimer.Text = editingTrigger.TimerName;
                checkBoxRestrict.Checked = editingTrigger.RestrictToCategoryZone;
                checkBoxResultsTab.Checked = editingTrigger.Tabbed;
                checkBoxTimer.Checked = editingTrigger.Timer;

                if (!haveOriginal)
                    buttonUpdateCreate.Text = "Create New";
                else
                    buttonUpdateCreate.Enabled = false; //until something changes

                PopulateGroupList();
            }
            initializing = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://regex101.com/r/cC7jP5/1");
        }

        #region Owner Callback support

        public event EventHandler EditDoneEvent; //callback

        //callback reasons
        public enum EventResult { CANCEL_EDIT, REPLACE_TRIGGER, CREATE_NEW };

        //callback event argument
        public class EditEventArgs : EventArgs
        {
            public EventResult result;
            public CustomTrigger editedTrigger;
            public CustomTrigger orignalTrigger;

            public EditEventArgs(CustomTrigger newTrigger, CustomTrigger origTrigger, EventResult result)
            {
                editedTrigger = newTrigger;
                orignalTrigger = origTrigger;
                this.result = result;
            }
        }

        protected void OnEditDoneEvent(EditEventArgs e)
        {
            if (EditDoneEvent != null)
            {
                EventHandler handler = EditDoneEvent;
                handler.Invoke(null, e);
            }
        }

        private void DoEventAndClose(EventResult result)
        {
            if (result != EventResult.CANCEL_EDIT)
            {
                if (string.IsNullOrEmpty(textBoxCategory.Text.Trim()))
                {
                    MessageBox.Show(this, "Category / Zone cannot be empty");
                    return;
                }

                if (regexChanged)
                {
                    if(string.IsNullOrEmpty(textBoxRegex.Text.Trim()))
                    {
                        MessageBox.Show(this, "Regular Expression cannot be empty");
                        return;
                    }

                    try
                    {
                        //test for valid regex
                        Regex testregex = new Regex(textBoxRegex.Text);
                    }
                    catch (ArgumentException aex)
                    {
                        ActGlobals.oFormActMain.NotificationAdd("Improper Custom Trigger Regular Expression", aex.Message);
                        MessageBox.Show(this, "Improper Regular Expression:\n" + aex.Message);
                        return;
                    }
                    string category = editingTrigger.Category;
                    bool restrict = editingTrigger.RestrictToCategoryZone;
                    editingTrigger = new CustomTrigger(textBoxRegex.Text,
                        editingTrigger.SoundType, editingTrigger.SoundData, editingTrigger.Timer, editingTrigger.TimerName, editingTrigger.Tabbed);
                    editingTrigger.Category = category;
                    editingTrigger.RestrictToCategoryZone = restrict;
                }

                if(radioButtonWav.Checked)
                {
                    if(!File.Exists(textBoxSound.Text))
                    {
                        MessageBox.Show(this, "WAV file does not exist");
                        return;
                    }
                }

                if((editingTrigger.Timer || editingTrigger.Tabbed)
                    && string.IsNullOrEmpty(editingTrigger.TimerName))
                {
                    if (MessageBox.Show(this, "Timer or Tab enabled without a Timer/Tab Name. Return to fix?", "Inconsistent Settings",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        return;
                }
            }
            EditEventArgs args = new EditEventArgs(editingTrigger, undoTrigger, result);
            OnEditDoneEvent(args);
            this.Close();
        }

        #endregion Owner Callback

        #region Button Clicks

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            EditEventArgs args = new EditEventArgs(editingTrigger, undoTrigger, EventResult.CANCEL_EDIT);
            OnEditDoneEvent(args);
            this.Close();
        }

        private void buttonZone_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(zoneCategory.Trim()))
            {
                textBoxCategory.Text = zoneCategory;
                //set the restricted checkbox if the string kinda looks like an EQII zone name
                checkBoxRestrict.Checked = zoneCategory.Contains("[");
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (radioButtonTts.Checked)
                ActGlobals.oFormActMain.TTS(textBoxSound.Text);
            else if (radioButtonWav.Checked)
            {
                if(File.Exists(textBoxSound.Text))
                    ActGlobals.oFormActMain.PlaySoundWinApi(textBoxSound.Text, 100);
            }
            else if (radioButtonBeep.Checked)
                System.Media.SystemSounds.Beep.Play();
        }

        private void buttonFileOpen_Click(object sender, EventArgs e)
        {
            string dir = Environment.GetEnvironmentVariable("SYSTEMROOT");
            openFileDialog1.InitialDirectory = dir + @"\Media";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxSound.Text = openFileDialog1.FileName;
            }
        }

        private void buttonUpdateCreate_Click(object sender, EventArgs e)
        {
            DoEventAndClose(EventResult.CREATE_NEW);
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            DoEventAndClose(EventResult.REPLACE_TRIGGER);
        }

        private void buttonInsert_Click(object sender, EventArgs e)
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

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            string text = Clipboard.GetText();
            Match match = parsePaste.Match(text);
            if (match.Success)
            {
                text = match.Groups["expr"].Value.Replace("\\","\\\\");
            }
            textBoxRegex.Text = text;
            textBoxRegex.SelectAll();
        }

        private void buttonFindTimer_Click(object sender, EventArgs e)
        {
            string name = textBoxTimer.Text.ToLower();
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    ActGlobals.oFormSpellTimers.SearchSpellTreeView(name);
                    ActGlobals.oFormSpellTimers.Visible = true;
                }
            }
            else
                MessageBox.Show(this, "Enter a spell timer name to search");
        }

        #endregion Button Clicks

        #region Changes

        private void radioButtonNone_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.None;
                buttonPlay.Enabled = false;
                buttonFileOpen.Enabled = false;
                textBoxSound.Enabled = false;
                buttonUpdateCreate.Enabled = true;
                buttonInsert.Enabled = false;
                comboBoxGroups.Enabled = false;
            }
        }

        private void radioButtonBeep_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.Beep;
                buttonPlay.Enabled = true;
                buttonFileOpen.Enabled = false;
                textBoxSound.Enabled = false;
                buttonUpdateCreate.Enabled = true;
                buttonInsert.Enabled = false;
                comboBoxGroups.Enabled = false;
            }
        }

        private void radioButtonWav_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.WAV;
                buttonPlay.Enabled = true;
                buttonFileOpen.Enabled = true;
                textBoxSound.Enabled = true;
                buttonUpdateCreate.Enabled = true;
                buttonInsert.Enabled = false;
                comboBoxGroups.Enabled = false;
            }
        }

        private void radioButtonTts_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.SoundType = (int)CustomTriggerSoundTypeEnum.TTS;
                buttonPlay.Enabled = true;
                buttonFileOpen.Enabled = false;
                textBoxSound.Enabled = true;
                buttonUpdateCreate.Enabled = true;
                PopulateGroupList();
            }
        }

        private void textBoxSound_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                if(editingTrigger != null) //avoid crash when stand-alone testing
                    editingTrigger.SoundData = textBoxSound.Text;
                buttonUpdateCreate.Enabled = true;
            }
        }

        private void textBoxCategory_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.Category = textBoxCategory.Text;
                buttonReplace.Enabled = true;
                buttonUpdateCreate.Text = "Create New";
                buttonUpdateCreate.Enabled = true;
                buttonReplace.Enabled = haveOriginal;
            }
        }

        private void textBoxTimer_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.TimerName = textBoxTimer.Text;
                buttonUpdateCreate.Enabled = true;
            }
        }

        private void checkBoxRestrict_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.RestrictToCategoryZone = checkBoxRestrict.Checked;
                buttonUpdateCreate.Enabled = true;
            }
        }

        private void checkBoxTimer_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.Timer = checkBoxTimer.Checked;
                buttonUpdateCreate.Enabled = true;
            }
        }

        private void checkBoxResultsTab_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.Tabbed = checkBoxResultsTab.Checked;
                buttonUpdateCreate.Enabled = true;
            }
        }

        private void textBoxRegex_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                regexChanged = true;
                buttonReplace.Enabled = haveOriginal;
                buttonUpdateCreate.Enabled = true;
                buttonUpdateCreate.Text = "Create New";
                PopulateGroupList();

                bool ok = false;
                try
                {
                    //test for valid regex
                    Regex testregex = new Regex(textBoxRegex.Text);
                    ok = true;
                }
                catch
                {
                    textBoxRegex.ForeColor = Color.Red;
                }
                if (ok)
                    textBoxRegex.ForeColor = Color.Black;
            }
        }

        private void PopulateGroupList()
        {
            comboBoxGroups.Items.Clear();
            comboBoxGroups.Enabled = false;
            buttonInsert.Enabled = false;

            try
            {
                Regex re = new Regex(textBoxRegex.Text);
                string[] groups = re.GetGroupNames();
                if (groups.Length > 1)
                {
                    comboBoxGroups.Enabled = true;
                    buttonInsert.Enabled = true;
                    for (int i = 1; i < groups.Length; i++) //skip group[0], it is the entire expression
                    {
                        comboBoxGroups.Items.Add(groups[i]);
                    }
                }
            }
            catch { } //not a valid regex, just don't crash
        }

        #endregion Changes

        #region Regular Expression Context Menu

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (textBoxRegex.CanUndo)
                contextMenuStrip1.Items["Undo"].Enabled = true;
            else
                contextMenuStrip1.Items["Undo"].Enabled = false;

            //can't cut, copy, paste, delete, make capture if nothing slected
            if(textBoxRegex.SelectedText.Length == 0)
            {
                contextMenuStrip1.Items["Cut"].Enabled = false;
                contextMenuStrip1.Items["Copy"].Enabled = false;
                contextMenuStrip1.Items["Delete"].Enabled = false;
                contextMenuStrip1.Items["MakePlayer"].Enabled = false;
                contextMenuStrip1.Items["MakeAttacker"].Enabled = false;
            }
            else
            {
                contextMenuStrip1.Items["Cut"].Enabled = true;
                contextMenuStrip1.Items["Copy"].Enabled = true;
                contextMenuStrip1.Items["Delete"].Enabled = true;
                contextMenuStrip1.Items["MakePlayer"].Enabled = true;
                contextMenuStrip1.Items["MakeAttacker"].Enabled = true;
            }

            //can't paste if there is nothing in the clipboard
            if (Clipboard.ContainsText())
                contextMenuStrip1.Items["Paste"].Enabled = true;
            else
                contextMenuStrip1.Items["Paste"].Enabled = false;

            //can't select all if there is no text
            if (textBoxRegex.Text.Length == 0)
                contextMenuStrip1.Items["SelectAll"].Enabled = false;
            else
                contextMenuStrip1.Items["SelectAll"].Enabled = true;

        }

        private void Undo_Click(object sender, EventArgs e)
        {
            textBoxRegex.Undo();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            textBoxRegex.Cut();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            textBoxRegex.Copy();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            textBoxRegex.Paste();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            //use Paste() so it can be undone
            textBoxRegex.Paste("");
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            textBoxRegex.SelectAll();
        }

        private void MakePlayer_Click(object sender, EventArgs e)
        {
            //use .Paste() to enable Undo
            textBoxRegex.Paste(@"(?<player>\w+)");
        }

        private void MakeAttacker_Click(object sender, EventArgs e)
        {
            //use .Paste() to enable Undo
            textBoxRegex.Paste(@"(?<attacker>\w+)");
        }

        private void textBoxRegex_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //This double-click-select replacement is easy, but the tradeoff is it's visually distracting.
            //By the time we get here the textbox has already made its "word" selection.
            //But it delimits strictly by spaces, and includes the trailing space.
            //We want to delimit by letters a-z, which is way more likely what we want to replace.
            //The user will see the seleciton change if we end up adjusting it.

            string text = textBoxRegex.Text;
            int starts = textBoxRegex.SelectionStart;

            //take any non-letters off the beginning
            for (; starts < text.Length; starts++)
            {
                if (Char.IsLetter(text[starts]))
                    break;
            }

            //take any non-letters off of the end
            int ends = starts;
            for (; ends < text.Length; ends++)
            {
                if (!Char.IsLetter(text[ends]))
                    break;
            }

            if (ends > starts)
            {
                textBoxRegex.SelectionStart = starts;
                textBoxRegex.SelectionLength = ends - starts;
            }
        }

        #endregion Regex Context Menu

    }
}
