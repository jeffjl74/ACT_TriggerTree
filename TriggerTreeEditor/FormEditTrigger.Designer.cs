namespace TriggerTreeEditor
{
    partial class FormEditTrigger
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
            this.textBoxRegex = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MakePlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.MakeAttacker = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonUpdateCreate = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.radioButtonBeep = new System.Windows.Forms.RadioButton();
            this.radioButtonWav = new System.Windows.Forms.RadioButton();
            this.radioButtonTts = new System.Windows.Forms.RadioButton();
            this.textBoxSound = new System.Windows.Forms.TextBox();
            this.buttonFileOpen = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.textBoxCategory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTimer = new System.Windows.Forms.TextBox();
            this.checkBoxRestrict = new System.Windows.Forms.CheckBox();
            this.checkBoxTimer = new System.Windows.Forms.CheckBox();
            this.checkBoxResultsTab = new System.Windows.Forms.CheckBox();
            this.buttonZone = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.comboBoxGroups = new System.Windows.Forms.ComboBox();
            this.buttonInsert = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonFindTimer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxRegex
            // 
            this.textBoxRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegex.ContextMenuStrip = this.contextMenuStrip1;
            this.helpProvider1.SetHelpString(this.textBoxRegex, "The expression to match in the EQII log file");
            this.textBoxRegex.Location = new System.Drawing.Point(132, 42);
            this.textBoxRegex.Name = "textBoxRegex";
            this.helpProvider1.SetShowHelp(this.textBoxRegex, true);
            this.textBoxRegex.Size = new System.Drawing.Size(437, 20);
            this.textBoxRegex.TabIndex = 0;
            this.textBoxRegex.TextChanged += new System.EventHandler(this.textBoxRegex_TextChanged);
            this.textBoxRegex.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBoxRegex_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Undo,
            this.toolStripSeparator1,
            this.Cut,
            this.Copy,
            this.Paste,
            this.Delete,
            this.toolStripSeparator2,
            this.SelectAll,
            this.toolStripSeparator3,
            this.MakePlayer,
            this.MakeAttacker});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(278, 198);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // Undo
            // 
            this.Undo.Name = "Undo";
            this.Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.Undo.ShowShortcutKeys = false;
            this.Undo.Size = new System.Drawing.Size(277, 22);
            this.Undo.Text = "Undo";
            this.Undo.Click += new System.EventHandler(this.Undo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(274, 6);
            // 
            // Cut
            // 
            this.Cut.Name = "Cut";
            this.Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.Cut.ShowShortcutKeys = false;
            this.Cut.Size = new System.Drawing.Size(277, 22);
            this.Cut.Text = "Cut";
            this.Cut.Click += new System.EventHandler(this.Cut_Click);
            // 
            // Copy
            // 
            this.Copy.Name = "Copy";
            this.Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.Copy.ShowShortcutKeys = false;
            this.Copy.Size = new System.Drawing.Size(277, 22);
            this.Copy.Text = "Copy";
            this.Copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // Paste
            // 
            this.Paste.Name = "Paste";
            this.Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.Paste.ShowShortcutKeys = false;
            this.Paste.Size = new System.Drawing.Size(277, 22);
            this.Paste.Text = "Paste";
            this.Paste.Click += new System.EventHandler(this.Paste_Click);
            // 
            // Delete
            // 
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(277, 22);
            this.Delete.Text = "Delete";
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(274, 6);
            // 
            // SelectAll
            // 
            this.SelectAll.Name = "SelectAll";
            this.SelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.SelectAll.ShowShortcutKeys = false;
            this.SelectAll.Size = new System.Drawing.Size(277, 22);
            this.SelectAll.Text = "Select All";
            this.SelectAll.Click += new System.EventHandler(this.SelectAll_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(274, 6);
            // 
            // MakePlayer
            // 
            this.MakePlayer.Name = "MakePlayer";
            this.MakePlayer.Size = new System.Drawing.Size(277, 22);
            this.MakePlayer.Text = "Make (?<player>\\w+) capture group";
            this.MakePlayer.ToolTipText = "Optionally replace \'player\' with any valid capture name afer pasting";
            this.MakePlayer.Click += new System.EventHandler(this.MakePlayer_Click);
            // 
            // MakeAttacker
            // 
            this.MakeAttacker.Name = "MakeAttacker";
            this.MakeAttacker.Size = new System.Drawing.Size(277, 22);
            this.MakeAttacker.Text = "Make (?<attacker>\\w+) capture group";
            this.MakeAttacker.ToolTipText = "${attacker} is automatically recognized in Spell Timers";
            this.MakeAttacker.Click += new System.EventHandler(this.MakeAttacker_Click);
            // 
            // buttonUpdateCreate
            // 
            this.buttonUpdateCreate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.helpProvider1.SetHelpString(this.buttonUpdateCreate, "Update current trigger. Or if the Regular Expression or Category / Zone has chang" +
        "ed, create New trigger.");
            this.buttonUpdateCreate.Location = new System.Drawing.Point(175, 223);
            this.buttonUpdateCreate.Name = "buttonUpdateCreate";
            this.helpProvider1.SetShowHelp(this.buttonUpdateCreate, true);
            this.buttonUpdateCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonUpdateCreate.TabIndex = 19;
            this.buttonUpdateCreate.Text = "Update";
            this.toolTip1.SetToolTip(this.buttonUpdateCreate, "Update or Create trigger");
            this.buttonUpdateCreate.UseVisualStyleBackColor = true;
            this.buttonUpdateCreate.Click += new System.EventHandler(this.buttonUpdateCreate_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonReplace.Enabled = false;
            this.helpProvider1.SetHelpString(this.buttonReplace, "If the Regular Expression or Category / Zone has changed, replace the original tr" +
        "igger with this trigger.");
            this.buttonReplace.Location = new System.Drawing.Point(274, 223);
            this.buttonReplace.Name = "buttonReplace";
            this.helpProvider1.SetShowHelp(this.buttonReplace, true);
            this.buttonReplace.Size = new System.Drawing.Size(75, 23);
            this.buttonReplace.TabIndex = 20;
            this.buttonReplace.Text = "Replace";
            this.toolTip1.SetToolTip(this.buttonReplace, "Replace original trigger");
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.Location = new System.Drawing.Point(374, 223);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.AutoSize = true;
            this.helpProvider1.SetHelpString(this.radioButtonNone, "Select for no alert sound");
            this.radioButtonNone.Location = new System.Drawing.Point(17, 115);
            this.radioButtonNone.Name = "radioButtonNone";
            this.helpProvider1.SetShowHelp(this.radioButtonNone, true);
            this.radioButtonNone.Size = new System.Drawing.Size(51, 17);
            this.radioButtonNone.TabIndex = 6;
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.Text = "None";
            this.radioButtonNone.UseVisualStyleBackColor = true;
            this.radioButtonNone.CheckedChanged += new System.EventHandler(this.radioButtonNone_CheckedChanged);
            // 
            // radioButtonBeep
            // 
            this.radioButtonBeep.AutoSize = true;
            this.helpProvider1.SetHelpString(this.radioButtonBeep, "Select for a system beep alert sound");
            this.radioButtonBeep.Location = new System.Drawing.Point(18, 143);
            this.radioButtonBeep.Name = "radioButtonBeep";
            this.helpProvider1.SetShowHelp(this.radioButtonBeep, true);
            this.radioButtonBeep.Size = new System.Drawing.Size(50, 17);
            this.radioButtonBeep.TabIndex = 7;
            this.radioButtonBeep.TabStop = true;
            this.radioButtonBeep.Text = "Beep";
            this.radioButtonBeep.UseVisualStyleBackColor = true;
            this.radioButtonBeep.CheckedChanged += new System.EventHandler(this.radioButtonBeep_CheckedChanged);
            // 
            // radioButtonWav
            // 
            this.radioButtonWav.AutoSize = true;
            this.helpProvider1.SetHelpString(this.radioButtonWav, "Select and set the wav file to use as alert sound");
            this.radioButtonWav.Location = new System.Drawing.Point(74, 115);
            this.radioButtonWav.Name = "radioButtonWav";
            this.helpProvider1.SetShowHelp(this.radioButtonWav, true);
            this.radioButtonWav.Size = new System.Drawing.Size(53, 17);
            this.radioButtonWav.TabIndex = 8;
            this.radioButtonWav.TabStop = true;
            this.radioButtonWav.Text = "WAV:";
            this.radioButtonWav.UseVisualStyleBackColor = true;
            this.radioButtonWav.CheckedChanged += new System.EventHandler(this.radioButtonWav_CheckedChanged);
            // 
            // radioButtonTts
            // 
            this.radioButtonTts.AutoSize = true;
            this.helpProvider1.SetHelpString(this.radioButtonTts, "Select and enter text for speech alert");
            this.radioButtonTts.Location = new System.Drawing.Point(74, 143);
            this.radioButtonTts.Name = "radioButtonTts";
            this.helpProvider1.SetShowHelp(this.radioButtonTts, true);
            this.radioButtonTts.Size = new System.Drawing.Size(49, 17);
            this.radioButtonTts.TabIndex = 9;
            this.radioButtonTts.TabStop = true;
            this.radioButtonTts.Text = "TTS:";
            this.radioButtonTts.UseVisualStyleBackColor = true;
            this.radioButtonTts.CheckedChanged += new System.EventHandler(this.radioButtonTts_CheckedChanged);
            // 
            // textBoxSound
            // 
            this.textBoxSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.textBoxSound, "The file name for the wav file or text for speech");
            this.textBoxSound.Location = new System.Drawing.Point(133, 114);
            this.textBoxSound.Name = "textBoxSound";
            this.helpProvider1.SetShowHelp(this.textBoxSound, true);
            this.textBoxSound.Size = new System.Drawing.Size(319, 20);
            this.textBoxSound.TabIndex = 10;
            this.textBoxSound.TextChanged += new System.EventHandler(this.textBoxSound_TextChanged);
            // 
            // buttonFileOpen
            // 
            this.buttonFileOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.buttonFileOpen, "Browse for wav files");
            this.buttonFileOpen.Location = new System.Drawing.Point(458, 112);
            this.buttonFileOpen.Name = "buttonFileOpen";
            this.helpProvider1.SetShowHelp(this.buttonFileOpen, true);
            this.buttonFileOpen.Size = new System.Drawing.Size(25, 23);
            this.buttonFileOpen.TabIndex = 11;
            this.buttonFileOpen.Text = "...";
            this.toolTip1.SetToolTip(this.buttonFileOpen, "Browse for file");
            this.buttonFileOpen.UseVisualStyleBackColor = true;
            this.buttonFileOpen.Click += new System.EventHandler(this.buttonFileOpen_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.helpProvider1.SetHelpString(this.buttonPlay, "Play the WAV or TTS");
            this.buttonPlay.Location = new System.Drawing.Point(489, 112);
            this.buttonPlay.Name = "buttonPlay";
            this.helpProvider1.SetShowHelp(this.buttonPlay, true);
            this.buttonPlay.Size = new System.Drawing.Size(25, 23);
            this.buttonPlay.TabIndex = 12;
            this.buttonPlay.Text = "4";
            this.buttonPlay.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.buttonPlay, "Play alert");
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // textBoxCategory
            // 
            this.textBoxCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.textBoxCategory, "The zone or category for the trigger");
            this.textBoxCategory.Location = new System.Drawing.Point(132, 68);
            this.textBoxCategory.Name = "textBoxCategory";
            this.helpProvider1.SetShowHelp(this.textBoxCategory, true);
            this.textBoxCategory.Size = new System.Drawing.Size(319, 20);
            this.textBoxCategory.TabIndex = 3;
            this.textBoxCategory.TextChanged += new System.EventHandler(this.textBoxCategory_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Timer / Tab Name:";
            // 
            // textBoxTimer
            // 
            this.textBoxTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.textBoxTimer, "If Trigger Timer and/or Add Results tab is checked, the name of the timer and/or " +
        "tab");
            this.textBoxTimer.Location = new System.Drawing.Point(132, 189);
            this.textBoxTimer.Name = "textBoxTimer";
            this.helpProvider1.SetShowHelp(this.textBoxTimer, true);
            this.textBoxTimer.Size = new System.Drawing.Size(319, 20);
            this.textBoxTimer.TabIndex = 16;
            this.textBoxTimer.TextChanged += new System.EventHandler(this.textBoxTimer_TextChanged);
            // 
            // checkBoxRestrict
            // 
            this.checkBoxRestrict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxRestrict.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxRestrict, "Check to restrict matching the regular expression only when in the specified zone" +
        "");
            this.checkBoxRestrict.Location = new System.Drawing.Point(457, 70);
            this.checkBoxRestrict.Name = "checkBoxRestrict";
            this.helpProvider1.SetShowHelp(this.checkBoxRestrict, true);
            this.checkBoxRestrict.Size = new System.Drawing.Size(155, 17);
            this.checkBoxRestrict.TabIndex = 4;
            this.checkBoxRestrict.Text = "Restrict to Category / Zone";
            this.checkBoxRestrict.UseVisualStyleBackColor = true;
            this.checkBoxRestrict.CheckedChanged += new System.EventHandler(this.checkBoxRestrict_CheckedChanged);
            // 
            // checkBoxTimer
            // 
            this.checkBoxTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxTimer.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxTimer, "Check to trigger a spell timer");
            this.checkBoxTimer.Location = new System.Drawing.Point(457, 180);
            this.checkBoxTimer.Name = "checkBoxTimer";
            this.helpProvider1.SetShowHelp(this.checkBoxTimer, true);
            this.checkBoxTimer.Size = new System.Drawing.Size(88, 17);
            this.checkBoxTimer.TabIndex = 17;
            this.checkBoxTimer.Text = "Trigger Timer";
            this.checkBoxTimer.UseVisualStyleBackColor = true;
            this.checkBoxTimer.CheckedChanged += new System.EventHandler(this.checkBoxTimer_CheckedChanged);
            // 
            // checkBoxResultsTab
            // 
            this.checkBoxResultsTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxResultsTab.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxResultsTab, "Check to add a Results Tab");
            this.checkBoxResultsTab.Location = new System.Drawing.Point(457, 202);
            this.checkBoxResultsTab.Name = "checkBoxResultsTab";
            this.helpProvider1.SetShowHelp(this.checkBoxResultsTab, true);
            this.checkBoxResultsTab.Size = new System.Drawing.Size(101, 17);
            this.checkBoxResultsTab.TabIndex = 18;
            this.checkBoxResultsTab.Text = "Add Results tab";
            this.checkBoxResultsTab.UseVisualStyleBackColor = true;
            this.checkBoxResultsTab.CheckedChanged += new System.EventHandler(this.checkBoxResultsTab_CheckedChanged);
            // 
            // buttonZone
            // 
            this.buttonZone.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this.buttonZone, "Click to copy the current zone to the Categoy / Zone setting and set Restrict to " +
        "Category / Zone");
            this.buttonZone.Location = new System.Drawing.Point(12, 66);
            this.buttonZone.Name = "buttonZone";
            this.helpProvider1.SetShowHelp(this.buttonZone, true);
            this.buttonZone.Size = new System.Drawing.Size(114, 23);
            this.buttonZone.TabIndex = 2;
            this.buttonZone.Text = "Category / Zone ⏩";
            this.toolTip1.SetToolTip(this.buttonZone, "Paste zone");
            this.buttonZone.UseVisualStyleBackColor = true;
            this.buttonZone.Click += new System.EventHandler(this.buttonZone_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "wave files|*.wav";
            this.openFileDialog1.InitialDirectory = "%SYSTEMROOT%\\Media";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(6, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(613, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Changing the Regular Expression or the Category / Zone requires replacing the ori" +
    "ginal trigger or creating a new trigger.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(25, 45);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(101, 13);
            this.linkLabel1.TabIndex = 24;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Regular Expression:";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // comboBoxGroups
            // 
            this.comboBoxGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxGroups.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.comboBoxGroups, "Captures from the Regular Expression available for use in the TTS alert.");
            this.comboBoxGroups.Location = new System.Drawing.Point(133, 140);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.helpProvider1.SetShowHelp(this.comboBoxGroups, true);
            this.comboBoxGroups.Size = new System.Drawing.Size(124, 21);
            this.comboBoxGroups.TabIndex = 13;
            this.toolTip1.SetToolTip(this.comboBoxGroups, "Available capture names");
            // 
            // buttonInsert
            // 
            this.buttonInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInsert.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this.buttonInsert, "Insert selected capture name into TTS expression");
            this.buttonInsert.Location = new System.Drawing.Point(263, 138);
            this.buttonInsert.Name = "buttonInsert";
            this.helpProvider1.SetShowHelp(this.buttonInsert, true);
            this.buttonInsert.Size = new System.Drawing.Size(25, 23);
            this.buttonInsert.TabIndex = 14;
            this.buttonInsert.Text = "⏫";
            this.toolTip1.SetToolTip(this.buttonInsert, "Insert capture name");
            this.buttonInsert.UseVisualStyleBackColor = true;
            this.buttonInsert.Click += new System.EventHandler(this.buttonInsert_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPaste.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.helpProvider1.SetHelpString(this.buttonPaste, "Paste from clipboard. If pasting a View Logs line, escapes backslashes, removes t" +
        "imestamp and removes end-of-line");
            this.buttonPaste.Location = new System.Drawing.Point(575, 40);
            this.buttonPaste.Name = "buttonPaste";
            this.helpProvider1.SetShowHelp(this.buttonPaste, true);
            this.buttonPaste.Size = new System.Drawing.Size(42, 23);
            this.buttonPaste.TabIndex = 1;
            this.buttonPaste.Text = "7¤";
            this.buttonPaste.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.buttonPaste, "Paste clipboard");
            this.buttonPaste.UseVisualStyleBackColor = true;
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // buttonFindTimer
            // 
            this.buttonFindTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFindTimer.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this.buttonFindTimer, "Search for Timer Name in Spell Timers. Use the [Clear] button in the Spell Timers" +
        " window to reset the search.");
            this.buttonFindTimer.Location = new System.Drawing.Point(544, 176);
            this.buttonFindTimer.Name = "buttonFindTimer";
            this.helpProvider1.SetShowHelp(this.buttonFindTimer, true);
            this.buttonFindTimer.Size = new System.Drawing.Size(25, 23);
            this.buttonFindTimer.TabIndex = 25;
            this.buttonFindTimer.Text = "⌕";
            this.buttonFindTimer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.buttonFindTimer, "Search for spell timer. [Clear] to reset.");
            this.buttonFindTimer.UseVisualStyleBackColor = true;
            this.buttonFindTimer.Click += new System.EventHandler(this.buttonFindTimer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(9, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(603, 78);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Audio Alert";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(286, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(281, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "To Define: Select Reg. Expr. text and right-click to name it";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(5, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(613, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Changing any other field simply updates the existing trigger.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormEditTrigger
            // 
            this.AcceptButton = this.buttonUpdateCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 258);
            this.Controls.Add(this.buttonFindTimer);
            this.Controls.Add(this.buttonPaste);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonInsert);
            this.Controls.Add(this.comboBoxGroups);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonZone);
            this.Controls.Add(this.checkBoxResultsTab);
            this.Controls.Add(this.checkBoxTimer);
            this.Controls.Add(this.checkBoxRestrict);
            this.Controls.Add(this.textBoxTimer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxCategory);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonFileOpen);
            this.Controls.Add(this.textBoxSound);
            this.Controls.Add(this.radioButtonTts);
            this.Controls.Add(this.radioButtonWav);
            this.Controls.Add(this.radioButtonBeep);
            this.Controls.Add(this.radioButtonNone);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.buttonUpdateCreate);
            this.Controls.Add(this.textBoxRegex);
            this.Controls.Add(this.groupBox1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 297);
            this.Name = "FormEditTrigger";
            this.ShowIcon = false;
            this.Text = "Edit Trigger";
            this.Shown += new System.EventHandler(this.FormEditTrigger_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxRegex;
        private System.Windows.Forms.Button buttonUpdateCreate;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButtonNone;
        private System.Windows.Forms.RadioButton radioButtonBeep;
        private System.Windows.Forms.RadioButton radioButtonWav;
        private System.Windows.Forms.RadioButton radioButtonTts;
        private System.Windows.Forms.TextBox textBoxSound;
        private System.Windows.Forms.Button buttonFileOpen;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.TextBox textBoxCategory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTimer;
        private System.Windows.Forms.CheckBox checkBoxRestrict;
        private System.Windows.Forms.CheckBox checkBoxTimer;
        private System.Windows.Forms.CheckBox checkBoxResultsTab;
        private System.Windows.Forms.Button buttonZone;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ComboBox comboBoxGroups;
        private System.Windows.Forms.Button buttonInsert;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MakePlayer;
        private System.Windows.Forms.ToolStripMenuItem MakeAttacker;
        private System.Windows.Forms.ToolStripMenuItem Undo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Cut;
        private System.Windows.Forms.ToolStripMenuItem Copy;
        private System.Windows.Forms.ToolStripMenuItem Paste;
        private System.Windows.Forms.ToolStripMenuItem Delete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem SelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Button buttonFindTimer;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

