using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace ACT_TriggerTree
{

    public partial class XmlCopyForm : Form
    {
        const int maxChatLen = 240;
        bool _loading = true;
        bool _preIncremet = false;
        bool _autoIncrementing = false;
        int _validTriggers = 0;
        int _validTimers = 0;
        int _totalTriggers = 0;
        int _totalTimers = 0;
        string _prefix;
        List<TimerData> _categoryTimers;
        List<CustomTrigger> _triggers;
        bool _altEncoding;
        public event EventHandler AltEncodeCheckChanged;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        List<IntPtr> _handles = new List<IntPtr>();

        // simple class to use as the listbox item
        enum ItemType { Trigger, Timer, Command }
        class ListItem
        {
            public string description;
            public string data;
            public ItemType type;
            public override string ToString()
            {
                return description;
            }
        }

        public XmlCopyForm(string prefix, List<TimerData> categoryTimers, List<CustomTrigger> triggers, bool altEncode)
        {
            InitializeComponent();

            _prefix = prefix;
            _triggers = triggers;
            _categoryTimers = categoryTimers;
            _altEncoding = altEncode;
        }

        private void XmlCopyForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_prefix))
            {
                if (_prefix.StartsWith("/g"))
                    radioButtonG.Checked = true;
                else if (_prefix.StartsWith("/r"))
                    radioButtonR.Checked = true;
                else
                {
                    radioButtonCustom.Checked = true;
                    textBoxCustom.Text = _prefix;
                }
            }

            checkBoxAltEncode.Checked = _altEncoding;
            toolTip1.SetToolTip(checkBoxAltEncode, "Enable macro alternate encoding.\nRecipients must be using TriggerTree.");
            Macros.AlternateEncoding = _altEncoding;

            BuildList();

            // look for game instances
            Process[] processes = Process.GetProcessesByName("EverQuest2");
            if (processes.Length > 0)
            {
                _handles = new List<IntPtr>();
                foreach (Process p in processes)
                {
                    // only want the main window
                    if (p.MainWindowTitle.StartsWith("EverQuest II ("))
                    {
                        if(!_handles.Contains(p.MainWindowHandle))
                            _handles.Add(p.MainWindowHandle);
                    }
                }
                if (_handles.Count > 0)
                {
                    foreach (IntPtr intPtr in _handles)
                    {
                        comboBoxGame.Items.Add(intPtr);
                    }
                    comboBoxGame.Items.Add(""); // item to allow user to de-select game activation
                    comboBoxGame.SelectedIndex = 0;
                    // switch to macro list?
                    if(buttonMacro.Enabled)
                        buttonMacro_Click(null, null);
                }
            }

            _loading = false;
            this.TopMost = true;
        }

        private void BuildList()
        {
            listBox1.Items.Clear();
            _validTimers = _validTriggers = _totalTimers = _totalTriggers = 0;
            foreach(CustomTrigger trigger in _triggers)
            {
                if (trigger.Active)
                {
                    _totalTriggers++;
                    _validTriggers += Macros.IsInvalidMacroTrigger(trigger) ? 0 : 1;
                    ListItem item = new ListItem { description = trigger.ShortRegexString, data = Macros.TriggerToXML(trigger), type = ItemType.Trigger };
                    if (item.data.Length > maxChatLen)
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, @"\b Trigger might be too long to paste.\b0\line\line " + item.description, "Warning");
                    listBox1.Items.Add(item);

                    // find timers that are activated by a trigger
                    List<TimerData> timers = TriggerTree.FindTimers(trigger);
                    foreach (TimerData timer in timers)
                    {
                        if (!_categoryTimers.Contains(timer))
                        {
                            _categoryTimers.Add(timer);
                        }
                    }
                }
            }

            foreach(TimerData timerData in _categoryTimers)
            {
                if (timerData.ActiveInList)
                {
                    _totalTimers++;
                    _validTimers += Macros.IsInvalidMacroTimer(timerData) ? 0 : 1;
                    ListItem item = new ListItem { description = timerData.Name, data = Macros.SpellTimerToXML(timerData), type = ItemType.Timer };
                    if (item.data.Length > maxChatLen)
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, @"\b Timer might be too long to paste.\b0\line\line " + item.description, "Warning");
                    listBox1.Items.Add(item);
                }
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
                toolStripStatusLabel1.Text = "Press [Copy] to copy selection to clipboard";
            }
            else
                toolStripStatusLabel1.Text = string.Empty;

            if (_validTimers == 0 && _validTriggers == 0)
                buttonMacro.Enabled = false;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            string prefix = string.Empty;
            if (radioButtonG.Checked)
                prefix = "/g ";
            else if (radioButtonR.Checked)
                prefix = "/r ";
            else if (!string.IsNullOrEmpty(textBoxCustom.Text))
            {
                prefix = textBoxCustom.Text;
                if (!prefix.EndsWith(" "))
                    prefix = prefix + " ";
            }

            bool needLoad = true;
            if (listBox1.Items.Count > 0)
            {
                ListItem listItem = (ListItem)listBox1.Items[0];
                if (listItem.type != ItemType.Command)
                {
                    NextListItem(prefix);
                    needLoad = false;
                }
            }
            if(needLoad)
            {
                this.Text = "XML Share";
                toolTip1.SetToolTip(buttonMacro, "Press to generate and list macro files");
                toolTip1.SetToolTip(buttonCopy, "Press to copy the selected XML item to the clipboard");
                BuildList();
            }
        }

        private void NextListItem(string prefix)
        {
            try
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    if (_preIncremet)
                    {
                        // select the next item
                        _autoIncrementing = true;
                        if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                            listBox1.SelectedIndex++;
                        else
                        {
                            listBox1.SelectedIndex = -1;
                            toolStripStatusLabel1.Text = "No more items.";
                        }
                        _autoIncrementing = false;
                    }
                    else
                    {
                        // first time through, we use the selected item
                        // next time, we will go to the next item
                        _preIncremet = true;
                    }

                    if (listBox1.SelectedIndex >= 0)
                    {
                        int itemNum = listBox1.SelectedIndex + 1;
                        // copy to the clipboard
                        ListItem item = (ListItem)listBox1.Items[listBox1.SelectedIndex];
                        Clipboard.SetText(prefix + item.data);

                        bool gameActivated = false;
                        if (comboBoxGame.Items.Count > 0 && comboBoxGame.SelectedIndex >= 0)
                        {
                            // if we found an EQII game window, activate it
                            if (!string.IsNullOrEmpty(comboBoxGame.Items[comboBoxGame.SelectedIndex].ToString()))
                            {
                                toolStripStatusLabel1.Text = String.Format(@"<Enter><Ctrl-v> to paste item {0}. {1} for next.", itemNum, item.type == ItemType.Command ? "[Macro]" : "[Copy]");
                                IntPtr handle = (IntPtr)comboBoxGame.Items[comboBoxGame.SelectedIndex];
                                SetForegroundWindow(handle);
                                gameActivated = true;
                            }
                        }
                        if(!gameActivated)
                        {
                            toolStripStatusLabel1.Text = String.Format("Item {0} copied. Press {1} for next.", itemNum, item.type == ItemType.Command ? "[Macro]" : "[Copy]");
                        }
                    }
                }
                else
                {
                    SimpleMessageBox.Show(this, "Select an item to copy to the clipboard.", "Error");
                    toolStripStatusLabel1.Text = string.Empty;
                }
            }
            catch (Exception)
            {
                SimpleMessageBox.Show(this, "Clipboard copy failed. Try again.", "Failed");
                _preIncremet = false;
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButtonCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                if (radioButtonCustom.Checked)
                    _prefix = textBoxCustom.Text;
                if (listBox1.Items.Count > 0)
                {
                    ListItem listItem = (ListItem)listBox1.Items[0];
                    if (listItem.type == ItemType.Command)
                    {
                        // regenerate the macros
                        listBox1.Items.Clear();
                        buttonMacro_Click(null, null);
                    }
                }
            }
        }

        private void textBoxCustom_TextChanged(object sender, EventArgs e)
        {
            if (radioButtonCustom.Checked)
            {
                _prefix = textBoxCustom.Text;
                if (listBox1.Items.Count > 0)
                {
                    ListItem listItem = (ListItem)listBox1.Items[0];
                    if (listItem.type == ItemType.Command)
                    {
                        // regenerate the macros
                        listBox1.Items.Clear();
                        buttonMacro_Click(null, null);
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    ListItem listItem = (ListItem)listBox1.Items[listBox1.SelectedIndex];
                    toolStripStatusLabel1.Text = String.Format("Press {0} to copy selection to clipboard", listItem.type == ItemType.Command ? "[Macro]" : "[Copy]");
                    if (!_autoIncrementing)
                        _preIncremet = false;
                }
            }
        }

        private void buttonMacro_Click(object sender, EventArgs e)
        {
            bool needLoad = true;
            if(listBox1.Items.Count > 0)
            {
                ListItem listItem = (ListItem)listBox1.Items[0];
                if(listItem.type == ItemType.Command)
                {
                    // list is already do_file_commmands, select the next one
                    NextListItem("");
                    needLoad = false;
                }
            }
            if(needLoad)
            {
                string prefix = string.Empty;
                toolTip1.SetToolTip(buttonMacro, "Press to copy the selected item to the clipboard");
                toolTip1.SetToolTip(buttonCopy, "Press to generate and list XML items");
                if (radioButtonG.Checked)
                    prefix = "g ";
                else if (radioButtonR.Checked)
                    prefix = "r ";
                else if (!string.IsNullOrEmpty(textBoxCustom.Text))
                {
                    prefix = textBoxCustom.Text;
                    prefix = prefix.TrimStart('/');
                    if (!prefix.EndsWith(" "))
                        prefix = prefix + " ";
                }

                if(checkBoxAltEncode.Checked)
                    this.Text = String.Format("Share: ({0}/{1}) triggers, ({2}/{3}) timers", _totalTriggers, _totalTriggers, _totalTimers, _totalTimers);
                else if (_totalTimers > 0 && _totalTriggers > 0)
                    this.Text = String.Format("XML Share: ({0}/{1}) triggers, ({2}/{3}) timers", _validTriggers, _totalTriggers, _validTimers, _totalTimers);
                else if(_totalTriggers > 0)
                    this.Text = String.Format("XML Share: ({0}/{1}) triggers", _validTriggers, _totalTriggers);
                else if(_totalTimers > 0)
                    this.Text = String.Format("XML Share: ({2}/{3}) timers", _validTimers, _totalTimers);

                int count = Macros.WriteCategoryMacroFile(prefix, _triggers, _categoryTimers, false);
                listBox1.Items.Clear();
                for (int i = 0; i < count; i++)
                {
                    ListItem listItem = new ListItem();
                    if (i == 0)
                    {
                        listItem.data = "/do_file_commands triggers.txt";
                    }
                    else
                    {
                        listItem.data = string.Format("/do_file_commands triggers{0}.txt", i);
                    }
                    listItem.description = listItem.data;
                    listItem.type = ItemType.Command;
                    listBox1.Items.Add(listItem);
                }
                if (listBox1.Items.Count > 0)
                {
                    listBox1.SelectedIndex = 0;
                }
            }
        }

        private void radioButtonG_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                if (radioButtonG.Checked)
                {
                    if (listBox1.Items.Count > 0)
                    {
                        ListItem listItem = (ListItem)listBox1.Items[0];
                        if (listItem.type == ItemType.Command)
                        {
                            // regenerate the macros
                            listBox1.Items.Clear();
                            buttonMacro_Click(null, null);
                        }
                    }
                }
            }
        }

        private void radioButtonR_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                if (radioButtonR.Checked)
                {
                    if (listBox1.Items.Count > 0)
                    {
                        ListItem listItem = (ListItem)listBox1.Items[0];
                        if (listItem.type == ItemType.Command)
                        {
                            // regenerate the macros
                            listBox1.Items.Clear();
                            buttonMacro_Click(null, null);
                        }
                    }
                }
            }
        }

        private void checkBoxAltEncode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _altEncoding = checkBoxAltEncode.Checked;
                Macros.AlternateEncoding = _altEncoding;
                if (AltEncodeCheckChanged != null)
                {
                    // notify our parent
                    AltEncodeCheckChanged.Invoke(sender, e);
                }
                if (listBox1.Items.Count > 0)
                {
                    ListItem listItem = (ListItem)listBox1.Items[0];
                    if (listItem.type == ItemType.Command)
                    {
                        // regenerate the macros
                        BuildList();
                        buttonMacro_Click(null, null);
                    }
                }
            }

        }
    }
}
