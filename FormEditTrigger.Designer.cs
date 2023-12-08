namespace ACT_TriggerTree
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
            this.contextMenuRegex = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MakeNumbered = new System.Windows.Forms.ToolStripMenuItem();
            this.MakePlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.MakeAttacker = new System.Windows.Forms.ToolStripMenuItem();
            this.MakeVictim = new System.Windows.Forms.ToolStripMenuItem();
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
            this.comboBoxCaptures = new System.Windows.Forms.ComboBox();
            this.buttonInsert = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonFindTimer = new System.Windows.Forms.Button();
            this.dataGridViewLines = new System.Windows.Forms.DataGridView();
            this.checkBoxLogLines = new System.Windows.Forms.CheckBox();
            this.treeViewEncounters = new System.Windows.Forms.TreeView();
            this.textBoxFindLine = new ACT_TriggerTree.TextBoxX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxTts = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBoxTimer = new System.Windows.Forms.PictureBox();
            this.pictureBoxCat = new System.Windows.Forms.PictureBox();
            this.pictureBoxRe = new System.Windows.Forms.PictureBox();
            this.comboBoxCatGroups = new System.Windows.Forms.ComboBox();
            this.panelTest = new System.Windows.Forms.Panel();
            this.splitContainerLog = new System.Windows.Forms.SplitContainer();
            this.panelLogLines = new System.Windows.Forms.Panel();
            this.panelLogFind = new System.Windows.Forms.Panel();
            this.checkBoxFilterRegex = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panelRegex = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelGridHelp = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.contextMenuLog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pasteInRegularExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testWithRegularExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showTimeDifferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuRegex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLines)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRe)).BeginInit();
            this.panelTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLog)).BeginInit();
            this.splitContainerLog.Panel1.SuspendLayout();
            this.splitContainerLog.Panel2.SuspendLayout();
            this.splitContainerLog.SuspendLayout();
            this.panelLogLines.SuspendLayout();
            this.panelLogFind.SuspendLayout();
            this.panelRegex.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxRegex
            // 
            this.textBoxRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegex.ContextMenuStrip = this.contextMenuRegex;
            this.helpProvider1.SetHelpString(this.textBoxRegex, "The expression to match in the EQII log file");
            this.textBoxRegex.Location = new System.Drawing.Point(129, 44);
            this.textBoxRegex.Name = "textBoxRegex";
            this.helpProvider1.SetShowHelp(this.textBoxRegex, true);
            this.textBoxRegex.Size = new System.Drawing.Size(463, 20);
            this.textBoxRegex.TabIndex = 0;
            this.textBoxRegex.TextChanged += new System.EventHandler(this.textBoxRegex_TextChanged);
            this.textBoxRegex.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBoxRegex_MouseDoubleClick);
            // 
            // contextMenuRegex
            // 
            this.contextMenuRegex.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Undo,
            this.toolStripSeparator1,
            this.Cut,
            this.Copy,
            this.Paste,
            this.Delete,
            this.toolStripSeparator2,
            this.SelectAll,
            this.toolStripSeparator3,
            this.MakeNumbered,
            this.MakePlayer,
            this.MakeAttacker,
            this.MakeVictim});
            this.contextMenuRegex.Name = "contextMenuStrip1";
            this.contextMenuRegex.Size = new System.Drawing.Size(278, 242);
            this.contextMenuRegex.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
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
            // MakeNumbered
            // 
            this.MakeNumbered.Name = "MakeNumbered";
            this.MakeNumbered.Size = new System.Drawing.Size(277, 22);
            this.MakeNumbered.Text = "Make (\\w+) numbered capture group";
            this.MakeNumbered.ToolTipText = "Numbered capture groups can be shared in a macro";
            this.MakeNumbered.Click += new System.EventHandler(this.MakewNumbered_Click);
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
            // MakeVictim
            // 
            this.MakeVictim.Name = "MakeVictim";
            this.MakeVictim.Size = new System.Drawing.Size(277, 22);
            this.MakeVictim.Text = "Make (?<victim>\\w+) capture group";
            this.MakeVictim.Click += new System.EventHandler(this.MakeVictimw_Click);
            // 
            // buttonUpdateCreate
            // 
            this.buttonUpdateCreate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.helpProvider1.SetHelpString(this.buttonUpdateCreate, "Update current trigger. Or if the Regular Expression or Category / Zone has chang" +
        "ed, create New trigger.");
            this.buttonUpdateCreate.Location = new System.Drawing.Point(193, 10);
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
            this.buttonReplace.Location = new System.Drawing.Point(282, 10);
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
            this.buttonCancel.Location = new System.Drawing.Point(371, 10);
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
            this.radioButtonNone.Location = new System.Drawing.Point(6, 19);
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
            this.radioButtonBeep.Location = new System.Drawing.Point(6, 46);
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
            this.radioButtonWav.Location = new System.Drawing.Point(57, 19);
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
            this.radioButtonTts.Location = new System.Drawing.Point(56, 46);
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
            this.textBoxSound.Location = new System.Drawing.Point(125, 15);
            this.textBoxSound.Name = "textBoxSound";
            this.helpProvider1.SetShowHelp(this.textBoxSound, true);
            this.textBoxSound.Size = new System.Drawing.Size(345, 20);
            this.textBoxSound.TabIndex = 10;
            this.textBoxSound.TextChanged += new System.EventHandler(this.textBoxSound_TextChanged);
            // 
            // buttonFileOpen
            // 
            this.buttonFileOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.buttonFileOpen, "Browse for wav files");
            this.buttonFileOpen.Location = new System.Drawing.Point(477, 15);
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
            this.buttonPlay.Location = new System.Drawing.Point(509, 15);
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
            this.textBoxCategory.Location = new System.Drawing.Point(129, 69);
            this.textBoxCategory.Name = "textBoxCategory";
            this.helpProvider1.SetShowHelp(this.textBoxCategory, true);
            this.textBoxCategory.Size = new System.Drawing.Size(338, 20);
            this.textBoxCategory.TabIndex = 3;
            this.textBoxCategory.TextChanged += new System.EventHandler(this.textBoxCategory_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 206);
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
            this.textBoxTimer.Location = new System.Drawing.Point(129, 202);
            this.textBoxTimer.Name = "textBoxTimer";
            this.helpProvider1.SetShowHelp(this.textBoxTimer, true);
            this.textBoxTimer.Size = new System.Drawing.Size(151, 20);
            this.textBoxTimer.TabIndex = 16;
            this.textBoxTimer.TextChanged += new System.EventHandler(this.textBoxTimer_TextChanged);
            // 
            // checkBoxRestrict
            // 
            this.checkBoxRestrict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxRestrict.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxRestrict, "Check to restrict matching the regular expression only when in the specified zone" +
        "");
            this.checkBoxRestrict.Location = new System.Drawing.Point(476, 72);
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
            this.checkBoxTimer.Location = new System.Drawing.Point(392, 204);
            this.checkBoxTimer.Name = "checkBoxTimer";
            this.helpProvider1.SetShowHelp(this.checkBoxTimer, true);
            this.checkBoxTimer.Size = new System.Drawing.Size(88, 17);
            this.checkBoxTimer.TabIndex = 18;
            this.checkBoxTimer.Text = "Trigger Timer";
            this.checkBoxTimer.UseVisualStyleBackColor = true;
            this.checkBoxTimer.CheckedChanged += new System.EventHandler(this.checkBoxTimer_CheckedChanged);
            // 
            // checkBoxResultsTab
            // 
            this.checkBoxResultsTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxResultsTab.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxResultsTab, "Check to add a Results Tab");
            this.checkBoxResultsTab.Location = new System.Drawing.Point(285, 204);
            this.checkBoxResultsTab.Name = "checkBoxResultsTab";
            this.helpProvider1.SetShowHelp(this.checkBoxResultsTab, true);
            this.checkBoxResultsTab.Size = new System.Drawing.Size(101, 17);
            this.checkBoxResultsTab.TabIndex = 17;
            this.checkBoxResultsTab.Text = "Add Results tab";
            this.checkBoxResultsTab.UseVisualStyleBackColor = true;
            this.checkBoxResultsTab.CheckedChanged += new System.EventHandler(this.checkBoxResultsTab_CheckedChanged);
            // 
            // buttonZone
            // 
            this.buttonZone.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this.buttonZone, "Click to copy the current zone to the Categoy / Zone setting and set Restrict to " +
        "Category / Zone");
            this.buttonZone.Location = new System.Drawing.Point(3, 67);
            this.buttonZone.Name = "buttonZone";
            this.helpProvider1.SetShowHelp(this.buttonZone, true);
            this.buttonZone.Size = new System.Drawing.Size(109, 23);
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
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(633, 15);
            this.label2.TabIndex = 22;
            this.label2.Text = "Changing the Regular Expression or the Category / Zone requires replacing the ori" +
    "ginal trigger or creating a new trigger.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(8, 46);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(101, 13);
            this.linkLabel1.TabIndex = 24;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Regular Expression:";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // comboBoxCaptureGroups
            // 
            this.comboBoxCaptures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCaptures.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.comboBoxCaptures, "Captures from the Regular Expression available for use in the TTS alert.");
            this.comboBoxCaptures.Location = new System.Drawing.Point(125, 44);
            this.comboBoxCaptures.Name = "comboBoxCaptureGroups";
            this.helpProvider1.SetShowHelp(this.comboBoxCaptures, true);
            this.comboBoxCaptures.Size = new System.Drawing.Size(152, 21);
            this.comboBoxCaptures.TabIndex = 13;
            this.toolTip1.SetToolTip(this.comboBoxCaptures, "Available capture names");
            // 
            // buttonInsert
            // 
            this.buttonInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInsert.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this.buttonInsert, "Insert selected capture name into TTS expression");
            this.buttonInsert.Location = new System.Drawing.Point(282, 43);
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
            this.buttonPaste.Location = new System.Drawing.Point(594, 41);
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
            this.buttonFindTimer.Location = new System.Drawing.Point(486, 200);
            this.buttonFindTimer.Name = "buttonFindTimer";
            this.helpProvider1.SetShowHelp(this.buttonFindTimer, true);
            this.buttonFindTimer.Size = new System.Drawing.Size(25, 23);
            this.buttonFindTimer.TabIndex = 19;
            this.buttonFindTimer.Text = "⌕";
            this.buttonFindTimer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.buttonFindTimer, "Search for spell timer. [Clear] to reset.");
            this.buttonFindTimer.UseVisualStyleBackColor = true;
            this.buttonFindTimer.Click += new System.EventHandler(this.buttonFindTimer_Click);
            // 
            // dataGridViewLines
            // 
            this.dataGridViewLines.AllowUserToAddRows = false;
            this.dataGridViewLines.AllowUserToDeleteRows = false;
            this.dataGridViewLines.AllowUserToResizeColumns = false;
            this.dataGridViewLines.AllowUserToResizeRows = false;
            this.dataGridViewLines.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLines.ColumnHeadersVisible = false;
            this.dataGridViewLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpProvider1.SetHelpString(this.dataGridViewLines, "Right click a log line for an option menu");
            this.dataGridViewLines.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewLines.Name = "dataGridViewLines";
            this.dataGridViewLines.ReadOnly = true;
            this.dataGridViewLines.RowHeadersVisible = false;
            this.dataGridViewLines.RowTemplate.Height = 18;
            this.helpProvider1.SetShowHelp(this.dataGridViewLines, true);
            this.dataGridViewLines.Size = new System.Drawing.Size(491, 156);
            this.dataGridViewLines.TabIndex = 0;
            this.dataGridViewLines.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dataGridViewLines_CellContextMenuStripNeeded);
            this.dataGridViewLines.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewLines_CellFormatting);
            // 
            // checkBoxLogLines
            // 
            this.checkBoxLogLines.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxLogLines, "Check to show the list of encounters and log lines. Useful for creating and testi" +
        "ng new triggers.");
            this.checkBoxLogLines.Location = new System.Drawing.Point(12, 243);
            this.checkBoxLogLines.Name = "checkBoxLogLines";
            this.helpProvider1.SetShowHelp(this.checkBoxLogLines, true);
            this.checkBoxLogLines.Size = new System.Drawing.Size(110, 17);
            this.checkBoxLogLines.TabIndex = 20;
            this.checkBoxLogLines.Text = "Show Encounters";
            this.toolTip1.SetToolTip(this.checkBoxLogLines, "Show / Hide encounter list");
            this.checkBoxLogLines.UseVisualStyleBackColor = true;
            this.checkBoxLogLines.CheckedChanged += new System.EventHandler(this.checkBoxLogLines_CheckedChanged);
            // 
            // treeViewEncounters
            // 
            this.treeViewEncounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewEncounters.FullRowSelect = true;
            this.helpProvider1.SetHelpString(this.treeViewEncounters, "Select an encouner to display its log lines");
            this.treeViewEncounters.Indent = 10;
            this.treeViewEncounters.Location = new System.Drawing.Point(0, 0);
            this.treeViewEncounters.Name = "treeViewEncounters";
            this.helpProvider1.SetShowHelp(this.treeViewEncounters, true);
            this.treeViewEncounters.Size = new System.Drawing.Size(144, 183);
            this.treeViewEncounters.TabIndex = 0;
            this.treeViewEncounters.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewEncounters_AfterSelect);
            // 
            // textBoxFindLine
            // 
            this.textBoxFindLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFindLine.ButtonTextClear = true;
            this.helpProvider1.SetHelpString(this.textBoxFindLine, "Filter log lines to show only those containing this text (no wildcards). For exam" +
        "ple: \'#\' to show colored lines. \'says,\' (include the comma) to show mob dialog.");
            this.textBoxFindLine.Location = new System.Drawing.Point(47, 3);
            this.textBoxFindLine.Name = "textBoxFindLine";
            this.helpProvider1.SetShowHelp(this.textBoxFindLine, true);
            this.textBoxFindLine.Size = new System.Drawing.Size(359, 20);
            this.textBoxFindLine.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBoxFindLine, "Show lines containing text. Examples: \'#\' for colored lines. \'says,\' for mob dial" +
        "og.");
            this.textBoxFindLine.TextChanged += new System.EventHandler(this.textBoxFindLine_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pictureBoxTts);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.radioButtonNone);
            this.groupBox1.Controls.Add(this.radioButtonWav);
            this.groupBox1.Controls.Add(this.radioButtonBeep);
            this.groupBox1.Controls.Add(this.buttonInsert);
            this.groupBox1.Controls.Add(this.radioButtonTts);
            this.groupBox1.Controls.Add(this.comboBoxCaptures);
            this.groupBox1.Controls.Add(this.textBoxSound);
            this.groupBox1.Controls.Add(this.buttonFileOpen);
            this.groupBox1.Controls.Add(this.buttonPlay);
            this.groupBox1.Location = new System.Drawing.Point(5, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(631, 74);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Audio Alert";
            // 
            // pictureBoxTts
            // 
            this.pictureBoxTts.ErrorImage = null;
            this.pictureBoxTts.Location = new System.Drawing.Point(107, 19);
            this.pictureBoxTts.Name = "pictureBoxTts";
            this.pictureBoxTts.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxTts.TabIndex = 30;
            this.pictureBoxTts.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxTts, "Indicates whether the string is valid in a macro");
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(313, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(274, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Shortcut: Select Reg. Expr. text and right-click to name it";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(-5, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(641, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Changing any other field simply updates the existing trigger.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxTimer
            // 
            this.pictureBoxTimer.ErrorImage = null;
            this.pictureBoxTimer.Location = new System.Drawing.Point(111, 206);
            this.pictureBoxTimer.Name = "pictureBoxTimer";
            this.pictureBoxTimer.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxTimer.TabIndex = 31;
            this.pictureBoxTimer.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxTimer, "Indicates whether the string is valid in a macro");
            // 
            // pictureBoxCat
            // 
            this.pictureBoxCat.ErrorImage = null;
            this.pictureBoxCat.Location = new System.Drawing.Point(112, 73);
            this.pictureBoxCat.Name = "pictureBoxCat";
            this.pictureBoxCat.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxCat.TabIndex = 29;
            this.pictureBoxCat.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxCat, "Indicates whether the string is valid in a macro");
            // 
            // pictureBoxRe
            // 
            this.pictureBoxRe.ErrorImage = null;
            this.pictureBoxRe.Location = new System.Drawing.Point(112, 47);
            this.pictureBoxRe.Name = "pictureBoxRe";
            this.pictureBoxRe.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxRe.TabIndex = 28;
            this.pictureBoxRe.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxRe, "Indicates whether the string is valid in a macro");
            // 
            // comboBoxCatGroups
            // 
            this.comboBoxCatGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCatGroups.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxCatGroups.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxCatGroups.FormattingEnabled = true;
            this.comboBoxCatGroups.Location = new System.Drawing.Point(129, 95);
            this.comboBoxCatGroups.Name = "comboBoxCatGroups";
            this.comboBoxCatGroups.Size = new System.Drawing.Size(334, 21);
            this.comboBoxCatGroups.TabIndex = 6;
            this.toolTip1.SetToolTip(this.comboBoxCatGroups, "Specify a category group if the Category/Zone doesn\'t already have a group");
            this.comboBoxCatGroups.TextChanged += new System.EventHandler(this.comboBoxCatGroups_TextChanged);
            // 
            // panelTest
            // 
            this.panelTest.Controls.Add(this.splitContainerLog);
            this.panelTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTest.Location = new System.Drawing.Point(0, 267);
            this.panelTest.MinimumSize = new System.Drawing.Size(630, 170);
            this.panelTest.Name = "panelTest";
            this.panelTest.Size = new System.Drawing.Size(639, 183);
            this.panelTest.TabIndex = 26;
            this.panelTest.Visible = false;
            // 
            // splitContainerLog
            // 
            this.splitContainerLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLog.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLog.Name = "splitContainerLog";
            // 
            // splitContainerLog.Panel1
            // 
            this.splitContainerLog.Panel1.Controls.Add(this.treeViewEncounters);
            // 
            // splitContainerLog.Panel2
            // 
            this.splitContainerLog.Panel2.Controls.Add(this.panelLogLines);
            this.splitContainerLog.Panel2.Controls.Add(this.panelLogFind);
            this.splitContainerLog.Size = new System.Drawing.Size(639, 183);
            this.splitContainerLog.SplitterDistance = 144;
            this.splitContainerLog.TabIndex = 1;
            // 
            // panelLogLines
            // 
            this.panelLogLines.Controls.Add(this.dataGridViewLines);
            this.panelLogLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLogLines.Location = new System.Drawing.Point(0, 27);
            this.panelLogLines.Name = "panelLogLines";
            this.panelLogLines.Size = new System.Drawing.Size(491, 156);
            this.panelLogLines.TabIndex = 4;
            // 
            // panelLogFind
            // 
            this.panelLogFind.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLogFind.Controls.Add(this.checkBoxFilterRegex);
            this.panelLogFind.Controls.Add(this.label5);
            this.panelLogFind.Controls.Add(this.textBoxFindLine);
            this.panelLogFind.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogFind.Location = new System.Drawing.Point(0, 0);
            this.panelLogFind.Name = "panelLogFind";
            this.panelLogFind.Size = new System.Drawing.Size(491, 27);
            this.panelLogFind.TabIndex = 3;
            // 
            // checkBoxFilterRegex
            // 
            this.checkBoxFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFilterRegex.AutoSize = true;
            this.checkBoxFilterRegex.Location = new System.Drawing.Point(412, 4);
            this.checkBoxFilterRegex.Name = "checkBoxFilterRegex";
            this.checkBoxFilterRegex.Size = new System.Drawing.Size(72, 17);
            this.checkBoxFilterRegex.TabIndex = 3;
            this.checkBoxFilterRegex.Text = "By Regex";
            this.checkBoxFilterRegex.UseVisualStyleBackColor = true;
            this.checkBoxFilterRegex.CheckedChanged += new System.EventHandler(this.checkBoxFilterRegex_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Filter:";
            // 
            // panelRegex
            // 
            this.panelRegex.Controls.Add(this.label7);
            this.panelRegex.Controls.Add(this.comboBoxCatGroups);
            this.panelRegex.Controls.Add(this.label6);
            this.panelRegex.Controls.Add(this.pictureBoxTimer);
            this.panelRegex.Controls.Add(this.pictureBoxCat);
            this.panelRegex.Controls.Add(this.pictureBoxRe);
            this.panelRegex.Controls.Add(this.labelGridHelp);
            this.panelRegex.Controls.Add(this.checkBoxLogLines);
            this.panelRegex.Controls.Add(this.groupBox1);
            this.panelRegex.Controls.Add(this.label3);
            this.panelRegex.Controls.Add(this.checkBoxRestrict);
            this.panelRegex.Controls.Add(this.textBoxCategory);
            this.panelRegex.Controls.Add(this.buttonPaste);
            this.panelRegex.Controls.Add(this.buttonFindTimer);
            this.panelRegex.Controls.Add(this.label1);
            this.panelRegex.Controls.Add(this.textBoxTimer);
            this.panelRegex.Controls.Add(this.label2);
            this.panelRegex.Controls.Add(this.linkLabel1);
            this.panelRegex.Controls.Add(this.checkBoxResultsTab);
            this.panelRegex.Controls.Add(this.textBoxRegex);
            this.panelRegex.Controls.Add(this.checkBoxTimer);
            this.panelRegex.Controls.Add(this.buttonZone);
            this.panelRegex.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRegex.Location = new System.Drawing.Point(0, 0);
            this.panelRegex.Name = "panelRegex";
            this.panelRegex.Size = new System.Drawing.Size(639, 267);
            this.panelRegex.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(5, 231);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(629, 2);
            this.label7.TabIndex = 34;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Category Group";
            // 
            // labelGridHelp
            // 
            this.labelGridHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGridHelp.AutoSize = true;
            this.labelGridHelp.Location = new System.Drawing.Point(161, 244);
            this.labelGridHelp.Name = "labelGridHelp";
            this.labelGridHelp.Size = new System.Drawing.Size(196, 13);
            this.labelGridHelp.TabIndex = 27;
            this.labelGridHelp.Text = "Right-click a log line for the option menu";
            this.labelGridHelp.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.buttonUpdateCreate);
            this.panel2.Controls.Add(this.buttonReplace);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 450);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(639, 36);
            this.panel2.TabIndex = 28;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(5, 4);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(629, 2);
            this.label8.TabIndex = 35;
            // 
            // contextMenuLog
            // 
            this.contextMenuLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pasteInRegularExpressionToolStripMenuItem,
            this.testWithRegularExpressionToolStripMenuItem,
            this.toolStripSeparator4,
            this.showTimeDifferencesMenuItem});
            this.contextMenuLog.Name = "contextMenuLog";
            this.contextMenuLog.Size = new System.Drawing.Size(223, 76);
            this.contextMenuLog.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuLog_Opening);
            // 
            // pasteInRegularExpressionToolStripMenuItem
            // 
            this.pasteInRegularExpressionToolStripMenuItem.Name = "pasteInRegularExpressionToolStripMenuItem";
            this.pasteInRegularExpressionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.pasteInRegularExpressionToolStripMenuItem.Text = "Paste in Regular Expression";
            this.pasteInRegularExpressionToolStripMenuItem.ToolTipText = "Paste this log line into the Regular Expression";
            this.pasteInRegularExpressionToolStripMenuItem.Click += new System.EventHandler(this.pasteInRegularExpressionToolStripMenuItem_Click);
            // 
            // testWithRegularExpressionToolStripMenuItem
            // 
            this.testWithRegularExpressionToolStripMenuItem.Name = "testWithRegularExpressionToolStripMenuItem";
            this.testWithRegularExpressionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.testWithRegularExpressionToolStripMenuItem.Text = "Test with Regular Expression";
            this.testWithRegularExpressionToolStripMenuItem.ToolTipText = "Run the Regular Expression against this log line";
            this.testWithRegularExpressionToolStripMenuItem.Click += new System.EventHandler(this.testWithRegularExpressionToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(219, 6);
            // 
            // showTimeDifferencesMenuItem
            // 
            this.showTimeDifferencesMenuItem.Name = "showTimeDifferencesMenuItem";
            this.showTimeDifferencesMenuItem.Size = new System.Drawing.Size(222, 22);
            this.showTimeDifferencesMenuItem.Text = "Show Time Differences";
            this.showTimeDifferencesMenuItem.ToolTipText = "Histogram of the time difference between filtered log lines";
            this.showTimeDifferencesMenuItem.Click += new System.EventHandler(this.showTimeDifferencesToolStripMenuItem_Click);
            // 
            // FormEditTrigger
            // 
            this.AcceptButton = this.buttonUpdateCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 486);
            this.Controls.Add(this.panelTest);
            this.Controls.Add(this.panelRegex);
            this.Controls.Add(this.panel2);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(655, 342);
            this.Name = "FormEditTrigger";
            this.ShowIcon = false;
            this.Text = "Edit Trigger";
            this.Shown += new System.EventHandler(this.FormEditTrigger_Shown);
            this.contextMenuRegex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLines)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRe)).EndInit();
            this.panelTest.ResumeLayout(false);
            this.splitContainerLog.Panel1.ResumeLayout(false);
            this.splitContainerLog.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLog)).EndInit();
            this.splitContainerLog.ResumeLayout(false);
            this.panelLogLines.ResumeLayout(false);
            this.panelLogFind.ResumeLayout(false);
            this.panelLogFind.PerformLayout();
            this.panelRegex.ResumeLayout(false);
            this.panelRegex.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.contextMenuLog.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ComboBox comboBoxCaptures;
        private System.Windows.Forms.Button buttonInsert;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip contextMenuRegex;
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
        private System.Windows.Forms.Panel panelTest;
        private System.Windows.Forms.Panel panelRegex;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainerLog;
        private System.Windows.Forms.Panel panelLogLines;
        private System.Windows.Forms.Panel panelLogFind;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dataGridViewLines;
        private System.Windows.Forms.CheckBox checkBoxLogLines;
        private System.Windows.Forms.ContextMenuStrip contextMenuLog;
        private System.Windows.Forms.ToolStripMenuItem pasteInRegularExpressionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testWithRegularExpressionToolStripMenuItem;
        private System.Windows.Forms.Label labelGridHelp;
        private System.Windows.Forms.ToolStripMenuItem MakeNumbered;
        private System.Windows.Forms.TreeView treeViewEncounters;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem showTimeDifferencesMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxRe;
        private System.Windows.Forms.PictureBox pictureBoxCat;
        private System.Windows.Forms.PictureBox pictureBoxTts;
        private System.Windows.Forms.PictureBox pictureBoxTimer;
        private TextBoxX textBoxFindLine;
        private System.Windows.Forms.CheckBox checkBoxFilterRegex;
        private System.Windows.Forms.ToolStripMenuItem MakeVictim;
        private System.Windows.Forms.ComboBox comboBoxCatGroups;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

