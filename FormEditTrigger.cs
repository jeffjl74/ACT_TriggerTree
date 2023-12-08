using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACT_TriggerTree
{
    public partial class FormEditTrigger : Form
    {
        static int logTimeStampLength = ActGlobals.oFormActMain.TimeStampLen;  //# of chars in the log file timestamp
        const string logTimeStampRegexStr = @"^\(\d{10}\)\[.{24}\] ";
        Regex parsePaste = new Regex(logTimeStampRegexStr + @"(?<expr>[^\r\n]*)", RegexOptions.Compiled);

        CustomTrigger editingTrigger;       //a copy of the original trigger
        CustomTrigger undoTrigger;          //a reference to the original trigger
        string decoratedCategory;           //color-coded categor
        string cleanCategory;               //category without color code or instance number
        int regexChanged = 0;               //track for replace / create new
        int zoneChanged = 0;
        int catGroupChanged = 0;            //track if category group name changed
        bool initializing = true;           //oncheck() methods do not need to do anything during shown()
        bool ignoreTextChange = false;      //don't propagate programatic find text change
        TreeNode lastSelectedNode;          //for better tree node highlighting

        //color the regex depending on restricted status / matching
        Color activeColor = Color.Green;
        Color inactiveColor = Color.Black;

        // macro validity indicators
        ImageList macroIcons = new ImageList();

        //set by owner
        public bool haveOriginal = true;    //set false by parent when creating a brand new trigger
        public bool zoneNameIsDecorated = false;
        public Dictionary<string, List<CustomTrigger>> catDict;
        Config _config;

        int logMenuRow = -1;                //context menu location in the log line grid view

        //encounter treeview scrolls inappropriately, use this to fix it
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        private const int SB_HORZ = 0x0;

        public FormEditTrigger()
        {
            InitializeComponent();
        }

        public FormEditTrigger(CustomTrigger trigger, string category, Config config)
        {
            InitializeComponent();

            zoneNameIsDecorated = IsCategoryDecorated(category);
            undoTrigger = trigger;
            _config = config;
            //make a new trigger that we can modify without changing the original trigger
            editingTrigger = new CustomTrigger(trigger.RegEx.ToString(), trigger.SoundType, trigger.SoundData, trigger.Timer, trigger.TimerName, trigger.Tabbed);
            editingTrigger.RestrictToCategoryZone = trigger.RestrictToCategoryZone;
            editingTrigger.Category = category;

            if (zoneNameIsDecorated)
            {
                bool enableOnZone = _config.autoCats.Contains(cleanCategory);
                if (enableOnZone)
                    editingTrigger.Category = cleanCategory;
                if(ActGlobals.oFormActMain.CurrentZone == decoratedCategory && enableOnZone)
                    editingTrigger.RestrictToCategoryZone = false;
            }

            macroIcons.Images.Add(Macros.GetActionBitmap());
            macroIcons.Images.Add(Macros.GetActionNotBitmap());
        }

        private bool IsCategoryDecorated(string category)
        {
            bool result = false;
            decoratedCategory = cleanCategory = category;
            Match match = TriggerTree.reCleanActZone.Match(category);
            if (match.Success)
            {
                cleanCategory = match.Groups["zone"].Value;
                if (!string.IsNullOrEmpty(match.Groups["decoration"].Value) || !string.IsNullOrEmpty(match.Groups["instance"].Value))
                    result = true;
            }
            return result;
        }

        private void FormEditTrigger_Shown(object sender, EventArgs e)
        {
            //hide encounters, initially
            this.Height = this.MinimumSize.Height;

            if (editingTrigger != null)
            {
                textBoxRegex.Text = editingTrigger.ShortRegexString;
                textBoxCategory.Text = editingTrigger.Category;

                string catGrp = _config.catGroupings.GetGroupName(editingTrigger.Category);
                if (string.IsNullOrEmpty(catGrp))
                    catGrp = TriggerTree.defaultGroupName;
                foreach (ConfigCatGroup grp in _config.catGroupings)
                    comboBoxCatGroups.Items.Add(grp.GroupName);
                comboBoxCatGroups.Text = catGrp;

                textBoxSound.Text = editingTrigger.SoundData;
                switch (editingTrigger.SoundType)
                {
                    case (int)CustomTriggerSoundTypeEnum.Beep:
                        radioButtonBeep.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonFileOpen.Enabled = false;
                        textBoxSound.Enabled = false;
                        buttonInsert.Enabled = false;
                        comboBoxCaptures.Enabled = false;
                        break;
                    case (int)CustomTriggerSoundTypeEnum.WAV:
                        radioButtonWav.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonFileOpen.Enabled = true;
                        textBoxSound.Enabled = true;
                        buttonInsert.Enabled = false;
                        comboBoxCaptures.Enabled = false;
                        break;
                    case (int)CustomTriggerSoundTypeEnum.TTS:
                        radioButtonTts.Checked = true;
                        buttonPlay.Enabled = true;
                        buttonFileOpen.Enabled = false;
                        textBoxSound.Enabled = true;
                        buttonInsert.Enabled = false;
                        comboBoxCaptures.Enabled = false;
                        break;
                    default:
                        radioButtonNone.Checked = true;
                        buttonPlay.Enabled = false;
                        buttonFileOpen.Enabled = false;
                        textBoxSound.Enabled = false;
                        buttonInsert.Enabled = false;
                        comboBoxCaptures.Enabled = false;
                        break;
                }
                textBoxTimer.Text = editingTrigger.TimerName;
                checkBoxResultsTab.Checked = editingTrigger.Tabbed;
                checkBoxTimer.Checked = editingTrigger.Timer;

                if (string.IsNullOrEmpty(editingTrigger.TimerName))
                    pictureBoxTimer.Visible = false;
                else
                {
                    pictureBoxTimer.Visible = true;
                    if (Macros.IsInvalidMacro(editingTrigger.TimerName))
                        pictureBoxTimer.Image = macroIcons.Images[1];
                    else
                        pictureBoxTimer.Image = macroIcons.Images[0];
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
                if (string.IsNullOrEmpty(editingTrigger.Category))
                    pictureBoxCat.Visible = false;
                else
                {
                    pictureBoxCat.Visible = true;
                    if (Macros.IsInvalidMacro(editingTrigger.Category))
                        pictureBoxCat.Image = macroIcons.Images[1];
                    else
                        pictureBoxCat.Image = macroIcons.Images[0];

                }
                if (string.IsNullOrEmpty(editingTrigger.ShortRegexString))
                    pictureBoxRe.Visible = false;
                else
                {
                    pictureBoxRe.Visible = true;
                    if (Macros.IsInvalidMacro(editingTrigger.ShortRegexString))
                        pictureBoxRe.Image = macroIcons.Images[1];
                    else
                        pictureBoxRe.Image = macroIcons.Images[0];
                }

                if (!haveOriginal)
                    buttonUpdateCreate.Text = "Create New";
                else
                    buttonUpdateCreate.Enabled = false; //until something changes

                PopulateCapturesList();
            }
            else // new trigger
            {
                foreach (ConfigCatGroup grp in _config.catGroupings)
                    comboBoxCatGroups.Items.Add(grp.GroupName);
                comboBoxCatGroups.Text = TriggerTree.defaultGroupName;
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
            public Point formLocation;
            public Size formSize;

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
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, "Category / Zone cannot be empty");
                    return;
                }

                if(result == EventResult.CREATE_NEW && zoneChanged > 0)
                {
                    //do we need to set "Enable on zone-in"?
                    if(zoneNameIsDecorated)
                    {
                        //is it already set?
                        if(!_config.autoCats.Contains(cleanCategory))
                        {
                            //is this a brand new category?
                            //(don't change the setting if the user has already set it)
                            if (catDict != null && !catDict.ContainsKey(cleanCategory))
                                _config.autoCats.Add(cleanCategory);
                        }
                    }
                }

                if(catGroupChanged > 0 || zoneChanged > 0)
                {
                    if (string.IsNullOrEmpty(comboBoxCatGroups.Text))
                        comboBoxCatGroups.Text = TriggerTree.defaultGroupName;

                    if(result == EventResult.CREATE_NEW || result == EventResult.REPLACE_TRIGGER)
                    {
                        _config.catGroupings.PutCatInGroup(comboBoxCatGroups.Text, editingTrigger.Category);
                    }
                }

                if (regexChanged > 0)
                {
                    if(string.IsNullOrEmpty(textBoxRegex.Text.Trim()))
                    {
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, "Regular Expression cannot be empty");
                        return;
                    }

                    try
                    {
                        //test for valid regex
                        Regex testregex = new Regex(textBoxRegex.Text);
                    }
                    catch (ArgumentException aex)
                    {
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, aex.Message, "Improper Regular Expression");
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
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, "WAV file does not exist");
                        return;
                    }
                }


                if ((editingTrigger.Timer || editingTrigger.Tabbed)
                    && string.IsNullOrEmpty(editingTrigger.TimerName))
                {
                    if (SimpleMessageBox.Show(ActGlobals.oFormActMain, @"Timer or Tab enabled without a Timer/Tab Name.\line Return to fix?", "Inconsistent Settings",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        return;
                }
            }
            EditEventArgs args = new EditEventArgs(editingTrigger, undoTrigger, result);
            args.formLocation = this.Location;
            args.formSize = this.Size;
            OnEditDoneEvent(args);
            this.Close();
        }

        #endregion Owner Callback

        #region Button Clicks

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            EditEventArgs args = new EditEventArgs(editingTrigger, undoTrigger, EventResult.CANCEL_EDIT);
            args.formLocation = this.Location;
            args.formSize = this.Size;
            OnEditDoneEvent(args);
            this.Close();
        }

        private void buttonZone_Click(object sender, EventArgs e)
        {
            zoneNameIsDecorated = IsCategoryDecorated(ActGlobals.oFormActMain.CurrentZone);
            // this will trigger the text changed event
            if (_config.autoCats.Contains(cleanCategory))
                textBoxCategory.Text = cleanCategory;
            else
                textBoxCategory.Text = ActGlobals.oFormActMain.CurrentZone;
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
            string group = comboBoxCaptures.Text;
            if (!string.IsNullOrEmpty(group))
            {
                //insert $1 if un-named, ${name} if named
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
            PasteRegEx(text);
        }

        private void PasteRegEx(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Match match = parsePaste.Match(text);
                if (match.Success)
                {
                    // strip the timestamp
                    // a \\ in the log is not an escaped \, it is two backslashes. fix it
                    // escape any parentheses
                    // escape any question marks
                    text = match.Groups["expr"].Value.Replace("\\", "\\\\")
                        .Replace("(", "\\(")
                        .Replace(")", "\\)")
                        .Replace("[", "\\[")
                        .Replace("]", "\\]")
                        .Replace("?", "\\?")
                        ;
                }
                textBoxRegex.Text = text;
                textBoxRegex.Focus();
                textBoxRegex.SelectAll();
            }
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
                SimpleMessageBox.Show(ActGlobals.oFormActMain, "Enter a spell timer name to search");
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
                comboBoxCaptures.Enabled = false;
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
                comboBoxCaptures.Enabled = false;
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
                comboBoxCaptures.Enabled = false;
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
                PopulateCapturesList();
            }
        }

        private void textBoxSound_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                if (editingTrigger != null) //avoid crash when stand-alone testing
                {
                    editingTrigger.SoundData = textBoxSound.Text;
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
                }
                buttonUpdateCreate.Enabled = true;
            }
        }

        private void textBoxCategory_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                zoneChanged++;
                editingTrigger.Category = textBoxCategory.Text;
                if(editingTrigger.Category.Contains("["))
                    checkBoxRestrict.Checked = true;
                if (zoneNameIsDecorated && _config.autoCats.Contains(cleanCategory))
                {
                    if (ActGlobals.oFormActMain.CurrentZone == decoratedCategory)
                        checkBoxRestrict.Checked = false;
                }

                if (string.IsNullOrEmpty(editingTrigger.Category))
                    pictureBoxCat.Visible = false;
                else
                {
                    pictureBoxCat.Visible = true;
                    if (Macros.IsInvalidMacro(editingTrigger.Category))
                        pictureBoxCat.Image = macroIcons.Images[1];
                    else
                        pictureBoxCat.Image = macroIcons.Images[0];
                }

                string grpName = _config.catGroupings.GetGroupName(textBoxCategory.Text);
                if(!string.IsNullOrEmpty(grpName))
                {
                    ConfigCatGroup grp = _config.catGroupings[grpName];
                    if (grp != null)
                        comboBoxCatGroups.Text = grp.GroupName;
                }

                if(zoneChanged > 1)
                {
                    // not the first change
                    // if the change has been undone, go back to unedited
                    if (editingTrigger.Category.Equals(undoTrigger.Category))
                    {
                        zoneChanged = 0;
                    }
                }

                SetButtonStates();
            }
        }

        private void SetButtonStates()
        {
            if (zoneChanged >= 1 || regexChanged >= 1)
            {
                buttonUpdateCreate.Text = "Create New";
                buttonUpdateCreate.Enabled = true;
                buttonReplace.Enabled = haveOriginal;
            }
            else if (zoneChanged == 0 && regexChanged == 0)
            {
                buttonUpdateCreate.Enabled = false;
                buttonReplace.Enabled = false;
                if (!haveOriginal)
                    buttonUpdateCreate.Text = "Create New";
                else
                {
                    buttonUpdateCreate.Text = "Update";
                    if (catGroupChanged == 0)
                        buttonUpdateCreate.Enabled = false; //until something changes
                    else
                        buttonUpdateCreate.Enabled = true;
                }
            }
        }

        private void textBoxTimer_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.TimerName = textBoxTimer.Text;
                buttonUpdateCreate.Enabled = true;

                if (string.IsNullOrEmpty(editingTrigger.TimerName))
                    pictureBoxTimer.Visible = false;
                else
                {
                    pictureBoxTimer.Visible = true;
                    if (Macros.IsInvalidMacro(editingTrigger.TimerName))
                        pictureBoxTimer.Image = macroIcons.Images[1];
                    else
                        pictureBoxTimer.Image = macroIcons.Images[0];
                }
            }
        }

        private void checkBoxRestrict_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                editingTrigger.RestrictToCategoryZone = checkBoxRestrict.Checked;
                buttonUpdateCreate.Enabled = true;
            }
            if (!editingTrigger.RestrictToCategoryZone || decoratedCategory.Equals(editingTrigger.Category))
                textBoxRegex.ForeColor =  activeColor;
            else
                textBoxRegex.ForeColor = inactiveColor;
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
                regexChanged++;
                if (textBoxRegex.Text.Equals(undoTrigger.RegEx.ToString()))
                {
                    // unchanged
                    regexChanged = 0;
                }
                SetButtonStates();
                checkBoxFilterRegex.Checked = false;
                PopulateCapturesList();

                if (string.IsNullOrEmpty(textBoxRegex.Text))
                    pictureBoxRe.Visible = false;
                else
                {
                    pictureBoxRe.Visible = true;
                    if (Macros.IsInvalidMacro(textBoxRegex.Text))
                        pictureBoxRe.Image = macroIcons.Images[1];
                    else
                        pictureBoxRe.Image = macroIcons.Images[0];
                }

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
                {
                    if (!editingTrigger.RestrictToCategoryZone || decoratedCategory.Equals(editingTrigger.Category))
                        textBoxRegex.ForeColor = activeColor;
                    else
                        textBoxRegex.ForeColor = inactiveColor;
                }
            }
        }

        private void PopulateCapturesList()
        {
            comboBoxCaptures.Items.Clear();
            comboBoxCaptures.Enabled = false;
            buttonInsert.Enabled = false;

            try
            {
                Regex re = new Regex(textBoxRegex.Text);
                string[] groups = re.GetGroupNames();
                if (groups.Length > 1)
                {
                    comboBoxCaptures.Enabled = true;
                    buttonInsert.Enabled = true;
                    for (int i = 1; i < groups.Length; i++) //skip group[0], it is the entire expression
                    {
                        comboBoxCaptures.Items.Add(groups[i]);
                    }
                }
            }
            catch { } //not a valid regex, just don't crash
        }

        private void comboBoxCatGroups_TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
            {
                string grpName = _config.catGroupings.GetGroupName(editingTrigger.Category);
                if (comboBoxCatGroups.Text.Equals(grpName) && !string.IsNullOrEmpty(grpName))
                    catGroupChanged = 0;
                else
                    catGroupChanged++;
                SetButtonStates();
            }
        }

        #endregion Changes

        #region Regular Expression Context Menu

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (textBoxRegex.CanUndo)
                contextMenuRegex.Items["Undo"].Enabled = true;
            else
                contextMenuRegex.Items["Undo"].Enabled = false;

            //can't make capture, cut, copy, paste, or delete if nothing is selected
            if (textBoxRegex.SelectedText.Length == 0)
            {
                contextMenuRegex.Items["Cut"].Enabled = false;
                contextMenuRegex.Items["Copy"].Enabled = false;
                contextMenuRegex.Items["Delete"].Enabled = false;
                contextMenuRegex.Items["MakeNumbered"].Enabled = false;
                contextMenuRegex.Items["MakePlayer"].Enabled = false;
                contextMenuRegex.Items["MakeAttacker"].Enabled = false;
                contextMenuRegex.Items["MakeVictim"].Enabled = false;
            }
            else
            {
                contextMenuRegex.Items["Cut"].Enabled = true;
                contextMenuRegex.Items["Copy"].Enabled = true;
                contextMenuRegex.Items["Delete"].Enabled = true;
                contextMenuRegex.Items["MakeNumbered"].Enabled = true;
                contextMenuRegex.Items["MakePlayer"].Enabled = true;
                contextMenuRegex.Items["MakeAttacker"].Enabled = true;
                contextMenuRegex.Items["MakeVictim"].Enabled = true;
            }

            //can't paste if there is nothing in the clipboard
            if (Clipboard.ContainsText())
                contextMenuRegex.Items["Paste"].Enabled = true;
            else
                contextMenuRegex.Items["Paste"].Enabled = false;

            //can't select all if there is no text
            if (textBoxRegex.Text.Length == 0)
                contextMenuRegex.Items["SelectAll"].Enabled = false;
            else
                contextMenuRegex.Items["SelectAll"].Enabled = true;

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

        private void MakewNumbered_Click(object sender, EventArgs e)
        {
            //use .Paste() to enable Undo
            textBoxRegex.Paste(@"(\w+)");
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

        private void MakeVictimw_Click(object sender, EventArgs e)
        {
            //use .Paste() to enable Undo
            textBoxRegex.Paste(@"(?<victim>\w+)");
        }

        private void textBoxRegex_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //This double-click-select replacement is easy, but the tradeoff is it's visually distracting.
            //By the time we get here the textbox has already made its "word" selection.
            //But it delimits strictly by spaces, and includes the trailing space.
            //We want to delimit by letters a-z, which is way more likely to be what we want to replace.
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

        #region Encounters

        private void textBoxFindLine_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxFilterRegex.Checked && ignoreTextChange == false)
            {
                ignoreTextChange = true;
                checkBoxFilterRegex.Checked = false;
                ignoreTextChange = false;
            }
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            try
            {
                DataTable dt = dataGridViewLines.DataSource as DataTable;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        string filter = textBoxFindLine.Text;
                        if (!string.IsNullOrEmpty(filter))
                        {
                            //use a simple filter, fix special chars and add LIKE syntax
                            filter = "LogLine LIKE '%" + EscapeLikeValue(filter) + "%'";
                        }
                        UseWaitCursor = true;
                        DataView view = dt.DefaultView;
                        string apply = string.IsNullOrEmpty(filter) ? string.Empty : filter;
                        if (view != null)
                        {
                            //this can take a second on a large encounter
                            //can't put it on another thread since it affects the UI
                            view.RowFilter = apply;
                        }
                        UseWaitCursor = false;
                    }
                }
            }
            catch (Exception exc)
            {
                UseWaitCursor = false;
                SimpleMessageBox.Show(ActGlobals.oFormActMain, exc.Message, "Improper filter");
            }
        }

        private static string EscapeLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static DataTable ToLineTable(List<LogLineEntry> list)
        {
            //make a DataTable of the log lines to make filtering easy
            DataTable dt = new DataTable();
            dt.Columns.Add("LogLine");
            int lineCount = list.Count;
            try
            {
                for(int i=0; i<lineCount; i++)
                {
                    dt.Rows.Add(list[i].LogLine);
                }
            }
            catch
            {
                //just in case there are any issues with accessing ACT's list,
                //just ignore it
            }
            return dt;
        }

        private static DataTable ToLineTable(List<LogLineEntry> list, string regex)
        {
            //make a DataTable of the log lines to make filtering easy
            DataTable dt = new DataTable();
            Regex re = new Regex(regex, RegexOptions.Compiled);
            dt.Columns.Add("LogLine");
            int lineCount = list.Count;
            try
            {
                for (int i = 0; i < lineCount; i++)
                {
                    if(re.Match(list[i].LogLine.Substring(logTimeStampLength)).Success)
                        dt.Rows.Add(list[i].LogLine);
                }
            }
            catch
            {
                //just in case there are any issues with accessing ACT's list,
                //just ignore it
            }
            return dt;
        }

        private async void checkBoxLogLines_CheckedChanged(object sender, EventArgs e)
        {
            //Use the minimum Sizes of the form and the panel (set in the designer)
            // to show/hide the encounters list.
            //If shown, populate it
            panelTest.Visible = checkBoxLogLines.Checked;
            if (panelTest.Visible)
            {
                this.Height = this.MinimumSize.Height + panelTest.MinimumSize.Height;
                labelGridHelp.Visible = true;

                treeViewEncounters.Nodes.Clear();
                textBoxFindLine.Clear();
                int zoneCount = ActGlobals.oFormActMain.ZoneList.Count;
                TreeNode[] zones = new TreeNode[zoneCount];
                //collect the nodes off of the UI thread
                await Task.Run(() =>
                {
                    try
                    {
                        UseWaitCursor = true;
                        for (int i = 0; i < zoneCount; i++)
                        {
                            ZoneData zonedata = ActGlobals.oFormActMain.ZoneList[i];
                            TreeNode zone = new TreeNode(zonedata.ZoneName);
                            zone.Tag = i;
                            zones[i] = zone;
                            int mobCount = zonedata.Items.Count;
                            if (mobCount > 0)
                            {
                                TreeNode[] mobs = new TreeNode[mobCount];
                                {
                                    {
                                        for (int j = 0; j < mobCount; j++)
                                        {
                                            EncounterData encounterData = zonedata.Items[j];
                                            TreeNode mob = new TreeNode(encounterData.ToString());
                                            mob.Tag = j;
                                            mobs[j] = mob;
                                        }
                                    }
                                }
                                zone.Nodes.AddRange(mobs);
                            }
                        }
                    }
                    catch { }
                    UseWaitCursor = false;
                });
                //populate the tree
                treeViewEncounters.Nodes.AddRange(zones);
                treeViewEncounters.ExpandAll();
                //scroll to the last entry
                int lastParent = treeViewEncounters.Nodes.Count - 1;
                treeViewEncounters.Nodes[lastParent].EnsureVisible();
                if (treeViewEncounters.Nodes[lastParent].IsExpanded)
                {
                    int lastChild = treeViewEncounters.Nodes[lastParent].Nodes.Count;
                    if (lastChild > 0)
                    {
                        treeViewEncounters.Nodes[lastParent].Nodes[lastChild - 1].EnsureVisible();
                        //scroll bar all the way left
                        SetScrollPos((IntPtr)treeViewEncounters.Handle, SB_HORZ, 0, true);
                    }
                }
            }
            else
            {
                //hide the panel
                this.Height = this.MinimumSize.Height;
                labelGridHelp.Visible = false;
                dataGridViewLines.DataSource = new DataTable(); //clear it, allow garbage collection
            }
        }

        private void contextMenuLog_Opening(object sender, CancelEventArgs e)
        {
            if ((string.IsNullOrEmpty(textBoxFindLine.Text) && !checkBoxFilterRegex.Checked)
                || dataGridViewLines.Rows.Count < 2)
                showTimeDifferencesMenuItem.Enabled = false;
            else
                showTimeDifferencesMenuItem.Enabled = true;

        }

        private void pasteInRegularExpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //copy the zone to the Category / Zone
            if(treeViewEncounters.SelectedNode.Parent != null)
            {
                int zoneIndex = Int32.Parse(treeViewEncounters.SelectedNode.Parent.Tag.ToString());
                ZoneData zoneData = ActGlobals.oFormActMain.ZoneList[zoneIndex];
                string zone = zoneData.ZoneName;
                if (!textBoxCategory.Text.Equals(zone) && !textBoxCategory.Text.Equals(cleanCategory))
                {
                    textBoxCategory.Text = zone;
                    checkBoxRestrict.Checked = decoratedCategory.Contains("[");
                }
            }

            //copy the log line to the regex with reformatting
            string line = dataGridViewLines.Rows[logMenuRow].Cells["LogLine"].Value.ToString();
            if (!string.IsNullOrEmpty(line))
                PasteRegEx(line);
        }

        private void testWithRegularExpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //use the regex on the log line selected by the right click
                string line = dataGridViewLines.Rows[logMenuRow].Cells["LogLine"].Value.ToString().Substring(logTimeStampLength);
                Regex re = new Regex(textBoxRegex.Text);
                Match match = re.Match(line);
                if(match.Success)
                {
                    if (radioButtonTts.Checked)
                    {
                        string alert = textBoxSound.Text;
                        string[] groups = re.GetGroupNames();
                        //group 0 is always the whole line
                        if(groups.Length > 1)
                        {
                            for(int i=1; i<groups.Length; i++)
                            {
                                int cap = 0;
                                bool result = int.TryParse(groups[i], out cap);
                                if (result)
                                    alert = alert.Replace("$" + groups[i], match.Groups[i].Value);
                                else
                                    alert = alert.Replace("${" + groups[i] + "}", match.Groups[i].Value);
                            }
                        }
                        ActGlobals.oFormActMain.TTS(alert);
                    }
                    else if (radioButtonWav.Checked)
                    {
                        if (File.Exists(textBoxSound.Text))
                            ActGlobals.oFormActMain.PlaySoundWinApi(textBoxSound.Text, 100);
                    }
                    else if (radioButtonBeep.Checked)
                        System.Media.SystemSounds.Beep.Play();

                    if(!string.IsNullOrEmpty(textBoxTimer.Text))
                    {
                        if (checkBoxTimer.Checked)
                        {
                            string victim = match.Groups["victim"].Value.ToString();
                            if(string.IsNullOrEmpty(victim))
                                victim = ActGlobals.charName;
                            string attacker = match.Groups["attacker"].Value.ToString();
                            if (string.IsNullOrEmpty(attacker))
                                attacker = victim;
                            ActGlobals.oFormSpellTimers.NotifySpell(attacker, textBoxTimer.Text, false, victim, true);
                        }
                        if(checkBoxResultsTab.Checked)
                        {
                            //not sure how to do this
                        }
                    }
                }
                else
                {
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, "Regular Expression does not match the log line", "No Match");
                }
            }
            catch (Exception rex)
            {
                SimpleMessageBox.Show(ActGlobals.oFormActMain, rex.Message, "Invalid regular expression");
            }
        }

        private void showTimeDifferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dataGridViewLines.Rows.Count > 100)
            {
                if (SimpleMessageBox.Show(ActGlobals.oFormActMain, @"There are more than 100 filtered lines.\line Are you sure the filter is correct?"
                    + @"\line (processing could take a while)"
                    , "Lots of lines", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }

            // Use the number-of-seconds time stamp at the beginning of the log line
            // to determine the time differences between displayed log lines.
            // It is enclosed with parentheses
            int paren = (dataGridViewLines.Rows[0].Cells[0].Value.ToString()).IndexOf(')');
            if (paren > 0)
            {
                SortedDictionary<Int64, uint> histogram = new SortedDictionary<Int64, uint>();
                Int64 t0 = 0;
                Int64 t1 = 0;
                for (int i=0; i<dataGridViewLines.Rows.Count; i++)
                {
                    DataGridViewRow row = dataGridViewLines.Rows[i];
                    if (Int64.TryParse(row.Cells[0].Value.ToString().Substring(1, paren-1), out t1))
                    {
                        if (i != 0)
                        {
                            Int64 diff = t1 - t0;
                            if (histogram.ContainsKey(diff))
                                histogram[diff]++;
                            else
                                histogram[diff] = 1;
                        }
                        t0 = t1;
                    }
                }
                FormHistogram chart = new FormHistogram();
                chart.SetData(histogram);
                chart.SetLocation(Location);
                string mob = treeViewEncounters.SelectedNode.Text.Substring(0, treeViewEncounters.SelectedNode.Text.IndexOf(" - "));
                chart.SetNames(textBoxFindLine.Text, mob, textBoxCategory.Text);
                chart.TimerDoneEvent += Chart_TimerDoneEvent;
                chart.Show();
            }
        }

        private void Chart_TimerDoneEvent(object sender, EventArgs e)
        {
            FormHistogram.TimerEventArgs a = e as FormHistogram.TimerEventArgs;
            if (a != null)
            {
                if (!string.IsNullOrEmpty(a.timerName))
                {
                    textBoxTimer.Text = a.timerName;
                    checkBoxTimer.Checked = true;
                    
                    // in case the user forgot to make the actual trigger,
                    // see if the log line text is represented in the regular expression
                    DataGridViewRow row = dataGridViewLines.Rows[0];
                    string line = row.Cells["LogLine"].Value.ToString();
                    Regex re = new Regex(textBoxRegex.Text);
                    Match match = re.Match(line);
                    if (!match.Success)
                    {
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, @"The regular expression does not match the text used to determine the timer value." 
                            + @"\line\line You probably want to fix the regular expression.", 
                            "Inconsistent", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void dataGridViewLines_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                e.ContextMenuStrip = contextMenuLog;
                //save where the mouse clicked
                logMenuRow = e.RowIndex;
            }
        }

        private void treeViewEncounters_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                //disconnect / clear the gridview while we update the table
                dataGridViewLines.DataSource = new DataTable();

                // better highlighting
                e.Node.BackColor = SystemColors.Highlight;
                e.Node.ForeColor = SystemColors.HighlightText;
                if (lastSelectedNode != null)
                {
                    // Deselect old node
                    lastSelectedNode.BackColor = SystemColors.Window;
                    lastSelectedNode.ForeColor = SystemColors.WindowText;
                }
                lastSelectedNode = e.Node;

                if (e.Node.Parent != null)
                {
                    FillEncounterLines(e.Node);
                }
            }
        }

        private async void FillEncounterLines(TreeNode node)
        {
            int encounterIndex = Int32.Parse(node.Tag.ToString());
            int zoneIndex = Int32.Parse(node.Parent.Tag.ToString());
            ZoneData zoneData = ActGlobals.oFormActMain.ZoneList[zoneIndex];
            if(!ignoreTextChange)
                textBoxFindLine.Clear();
            DataTable dt = null;
            try
            {
                //don't tie up the UI thread building the table (even though it's fairly quick)
                await Task.Run(() =>
                {
                    UseWaitCursor = true;
                    if (checkBoxFilterRegex.Checked)
                        dt = ToLineTable(zoneData.Items[encounterIndex].LogLines, textBoxRegex.Text);
                    else
                        dt = ToLineTable(zoneData.Items[encounterIndex].LogLines);
                    UseWaitCursor = false;
                });
                if (dt != null)
                {
                    dataGridViewLines.DataSource = dt;

                    //mode fill = can't get a horizontal scroll bar
                    //any auto size mode takes too much calculation time on large encounters
                    //so just set a pretty large width that should handle most everything we'd want to use to make a trigger
                    dataGridViewLines.Columns["LogLine"].Width = 1200;
                }
            }
            catch (Exception dtx)
            {
                SimpleMessageBox.Show(ActGlobals.oFormActMain, dtx.Message, "Problem collecting the log lines");
            }
        }

        private void dataGridViewLines_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.Value != null)
            {
                string line = e.Value.ToString();
                if(line.Length > logTimeStampLength + 9)
                {
                    // if there is a color code, like \#FF0000
                    // use it to color the text
                    if(line.Substring(logTimeStampLength,2).Equals("\\#"))
                    {
                        int red = Convert.ToInt32(line.Substring(logTimeStampLength + 2, 2),16);
                        int green = Convert.ToInt32(line.Substring(logTimeStampLength + 4, 2), 16);
                        int blue = Convert.ToInt32(line.Substring(logTimeStampLength + 6, 2), 16);
                        Color color = Color.FromArgb(red, green, blue);
                        e.CellStyle.ForeColor = color;
                        e.CellStyle.BackColor = Color.Black;
                    }
                }
            }
        }

        private void checkBoxFilterRegex_CheckedChanged(object sender, EventArgs e)
        {
            TreeNode node = treeViewEncounters.SelectedNode;
            if(node != null)
            {
                if(node.Parent != null)
                {
                    ignoreTextChange = true;
                    FillEncounterLines(node);
                    ignoreTextChange = false;
                }
            }
        }

        #endregion Encounters
    }
}
