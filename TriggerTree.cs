using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
// reference:System.Windows.Forms.DataVisualization.dll
// reference:System.Core.dll


[assembly: AssemblyTitle("Tree view of Custom Triggers")]
[assembly: AssemblyDescription("An alternate interface for managing Custom Triggers")]
[assembly: AssemblyCompany("Mineeme of Maj'Dul")]
[assembly: AssemblyVersion("1.5.0.0")]

namespace ACT_TriggerTree
{
    public class TriggerTree : UserControl, IActPluginV1
	{
        const int logTimeStampLength = 39;          //# of chars in the timestamp

        //trigger dictionary - list of triggers by Category name
        Dictionary<string, List<CustomTrigger>> treeDict;

        //zone
        string zoneName = string.Empty;             //most recent zone name from the log file
        string decoratedZoneName = string.Empty;    //includes color and instance #
        Regex reCleanLogZone = new Regex(@"\(\d{10}\)\[.{24}\] You have entered (?::.+?:)?(?:\\#[0-9A-F]{6})?(?<zone>[^.0-9]+)", RegexOptions.Compiled);
        Regex reCleanActZone = new Regex(@"(?::.+?:)?(?:\\#[0-9A-F]{6})?(?<zone>[^.0-9]+)", RegexOptions.Compiled);
        bool zoneMatchedEQII = false;
        WindowsFormsSynchronizationContext mUiContext = new WindowsFormsSynchronizationContext();

        ImageList treeImages = new ImageList();     //category folder images
        ImageList triggerImages = new ImageList();  //trigger images
        const int triggerCanMacro = 0;              //index for the image indicating that the trigger can go in a macro
        const int triggerNoMacro = 1;               //index for the image indicating that the trigger cannot go in a macro
        int triggerBlankImage;                      //index for a blank image, set at runtime

        //trigger child node labels
        string alertLabel = "Alert: ";
        string restrictLabel = "Restrict to Category / Zone";
        string timerLable = "Trigger timer";
        string tabLabel = "Add Results Tab";
        string timerNameLabel = "Timer or Tab name=";
        int indexAlertType = 0;                     //child index for the alert type child
        int indexRestricted = 1;                    //child index for the Restrict to Zone child
        int indexTimer = 2;                         //child index for the Start Timer child
        int indexResultsTab = 3;                      //child index for the Alert Tab child
        int indexTimerName = 4;                     //child index for the Timer/Tab child

        Color activeBackground = Color.LightGreen;  //background for a category that contains active triggers
        Brush activeBackgroundBrush = new SolidBrush(Color.LightGreen);
        Color inactiveBackground = Color.White;     //background for a category with only inactive triggers
        Color foundBackground = Color.Gold;         //background for a found trigger
        Color notFoundBackground = Color.White;     //background for triggers
        Color activeColor = Color.Green;            //trigger that will alert if seen in the log
        Color inactiveColor = Color.Black;          //trigger that is enabled, but we are not in the restricted zone, so will not alert
        Color disabledColor = Color.Gray;           //trigger that will never alert

        TreeNode selectedTriggerNode = null;        //node selected via mouse click
        TreeNode clickedCategoryNode = null;
        Point whereTrigMouseDown;                   //screen location for the trigger tree context menu
        Point whereCatMouseDown;
        bool isDoubleClick = false;                 //to intercept the double click on a trigger
        Point lastEditLoc = new Point();
        Size lastEditSize = new Size();

        string keyLastFound = string.Empty;         //for Find Next trigger
        string catLastFound = string.Empty;         //for find next cat
        enum FindResult { NOT_FOUND, FOUND, FIND_FAILED};

        bool neverBeenVisible = true;               //save the splitter location only if it has been initialized 

        //trigger macro file stuff
        public static string doFileName = "triggers.txt";         //macro file name
        //menu tooltips
        string invalidTriggerText = "Trigger or Category contains character(s) {0} or string {1} which are invalid in a macro file";
        string validTriggerText = "Make a triggers.txt macro file to share trigger with the ";
        string invalidCategoryText = "All triggers are disabled or contain invalid macro character(s) {0} or string {1}";
        string validCategoryText = "Make a triggers.txt macro file to share all enabled macro triggers with the ";
        string invalidTimerText = "Timer contains character(s) {0} or string {1} which are invalid in a macro file";
        string validTimerText = "Make a triggers.txt macro file to share timer with the ";
        string catMacroText = "{0} Share Macro ({1}/{2})";

        List<TimerData> categoryTimers;             //category context menu timers
        MouseButtons lastSpellMenuButton;

        // results tab mirror
        FormResultsTabs formResultsTabs;
        TabControl resultsTabCtrl = null;
        Button addEditButton = null;
        Button removeButton = null;

        Label lblStatus;                            // The status label that appears in ACT's Plugin tab

        string settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\TriggerTree.config.xml");
        XmlSerializer xmlSerializer;
        Config config;
        private ToolStripMenuItem toggleEntireCategoryToolStripMenuItem;
        private ToolStripMenuItem enableAllTriggersToolStripMenuItem;
        private ToolStripMenuItem disableAllTriggersToolStripMenuItem;
        private ToolStripMenuItem enableOnZoneinToolStripMenuItem;

        #region Designer Created Code (Avoid editing)

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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewCats = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCatFindNext = new System.Windows.Forms.Button();
            this.textBoxCatFind = new ACT_TriggerTree.TextBoxX();
            this.treeViewTrigs = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonResults = new System.Windows.Forms.ToolStripButton();
            this.checkBoxCurrentCategory = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.textBoxTrigFind = new ACT_TriggerTree.TextBoxX();
            this.contextMenuStripTrig = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyAsShareableXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAsDoubleEncodedXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.editTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.playAlertSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.raidsayShareMacroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupsayShareMacroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripCat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyZoneNameToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEntireCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleEntireCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.raidShareCategoryMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupShareCategoryMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shareDialogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.categorySpellTimersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.enableAllTriggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllTriggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableOnZoneinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStripTrig.SuspendLayout();
            this.contextMenuStripCat.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewCats);
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeViewTrigs);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(727, 560);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeViewCats
            // 
            this.treeViewCats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCats.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewCats.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewCats.Location = new System.Drawing.Point(0, 33);
            this.treeViewCats.Name = "treeViewCats";
            this.treeViewCats.Size = new System.Drawing.Size(240, 527);
            this.treeViewCats.TabIndex = 1;
            this.treeViewCats.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCats_DrawNode);
            this.treeViewCats.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCats_AfterSelect);
            this.treeViewCats.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewCats_MouseDown);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.buttonCatFindNext);
            this.panel3.Controls.Add(this.textBoxCatFind);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(240, 33);
            this.panel3.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Find:";
            // 
            // buttonCatFindNext
            // 
            this.buttonCatFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCatFindNext.Enabled = false;
            this.buttonCatFindNext.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonCatFindNext.Location = new System.Drawing.Point(195, 2);
            this.buttonCatFindNext.Name = "buttonCatFindNext";
            this.buttonCatFindNext.Size = new System.Drawing.Size(38, 23);
            this.buttonCatFindNext.TabIndex = 1;
            this.buttonCatFindNext.Text = "8";
            this.buttonCatFindNext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.buttonCatFindNext, "Find the next matching category");
            this.buttonCatFindNext.UseVisualStyleBackColor = true;
            this.buttonCatFindNext.Click += new System.EventHandler(this.buttonCatFindNext_Click);
            // 
            // textBoxCatFind
            // 
            this.textBoxCatFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCatFind.ButtonTextClear = true;
            this.textBoxCatFind.Location = new System.Drawing.Point(40, 4);
            this.textBoxCatFind.Name = "textBoxCatFind";
            this.textBoxCatFind.Size = new System.Drawing.Size(148, 20);
            this.textBoxCatFind.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxCatFind, "Incremental search in the category name");
            this.textBoxCatFind.TextChanged += new System.EventHandler(this.textBoxCatScroll_TextChanged);
            // 
            // treeViewTrigs
            // 
            this.treeViewTrigs.CheckBoxes = true;
            this.treeViewTrigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTrigs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewTrigs.Location = new System.Drawing.Point(0, 33);
            this.treeViewTrigs.Name = "treeViewTrigs";
            this.treeViewTrigs.ShowNodeToolTips = true;
            this.treeViewTrigs.Size = new System.Drawing.Size(483, 527);
            this.treeViewTrigs.TabIndex = 1;
            this.treeViewTrigs.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewTrigs_BeforeCheck);
            this.treeViewTrigs.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTrigs_AfterCheck);
            this.treeViewTrigs.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewTrigs_BeforeCollapse);
            this.treeViewTrigs.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewTrigs_BeforeExpand);
            this.treeViewTrigs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewTrigs_KeyDown);
            this.treeViewTrigs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewTrigs_MouseDoubleClick);
            this.treeViewTrigs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewTrigs_MouseDown);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.toolStrip1);
            this.panel2.Controls.Add(this.checkBoxCurrentCategory);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.buttonFindNext);
            this.panel2.Controls.Add(this.textBoxTrigFind);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(483, 33);
            this.panel2.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonResults});
            this.toolStrip1.Location = new System.Drawing.Point(4, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(51, 26);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(23, 23);
            this.toolStripButtonNew.Text = "+";
            this.toolStripButtonNew.ToolTipText = "Add New Trigger";
            this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNew_Click);
            // 
            // toolStripButtonResults
            // 
            this.toolStripButtonResults.CheckOnClick = true;
            this.toolStripButtonResults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonResults.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.toolStripButtonResults.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonResults.Name = "toolStripButtonResults";
            this.toolStripButtonResults.Size = new System.Drawing.Size(25, 23);
            this.toolStripButtonResults.Text = "2";
            this.toolStripButtonResults.ToolTipText = "Enable Results Tabs Popup";
            this.toolStripButtonResults.Click += new System.EventHandler(this.toolStripButtonResults_Click);
            // 
            // checkBoxCurrentCategory
            // 
            this.checkBoxCurrentCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxCurrentCategory.AutoSize = true;
            this.checkBoxCurrentCategory.Checked = true;
            this.checkBoxCurrentCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCurrentCategory.Location = new System.Drawing.Point(372, 7);
            this.checkBoxCurrentCategory.Name = "checkBoxCurrentCategory";
            this.checkBoxCurrentCategory.Size = new System.Drawing.Size(59, 17);
            this.checkBoxCurrentCategory.TabIndex = 1;
            this.checkBoxCurrentCategory.Text = "current";
            this.toolTip1.SetToolTip(this.checkBoxCurrentCategory, "Search only the current category");
            this.checkBoxCurrentCategory.UseVisualStyleBackColor = true;
            this.checkBoxCurrentCategory.CheckedChanged += new System.EventHandler(this.checkBoxCurrentCategory_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(75, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Find:";
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFindNext.Enabled = false;
            this.buttonFindNext.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonFindNext.Location = new System.Drawing.Point(437, 2);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(38, 23);
            this.buttonFindNext.TabIndex = 2;
            this.buttonFindNext.Text = "8";
            this.toolTip1.SetToolTip(this.buttonFindNext, "Find the next matching trigger");
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // textBoxTrigFind
            // 
            this.textBoxTrigFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTrigFind.ButtonTextClear = true;
            this.textBoxTrigFind.Location = new System.Drawing.Point(109, 4);
            this.textBoxTrigFind.Name = "textBoxTrigFind";
            this.textBoxTrigFind.Size = new System.Drawing.Size(257, 20);
            this.textBoxTrigFind.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxTrigFind, "Incremental search for text in the trigger\'s regular expression, alert, or timer " +
        "name");
            this.textBoxTrigFind.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
            // 
            // contextMenuStripTrig
            // 
            this.contextMenuStripTrig.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAsShareableXMLToolStripMenuItem,
            this.copyAsDoubleEncodedXMLToolStripMenuItem,
            this.toolStripSeparator5,
            this.editTriggerToolStripMenuItem,
            this.deleteTriggerToolStripMenuItem,
            this.toolStripSeparator1,
            this.playAlertSoundToolStripMenuItem,
            this.toolStripSeparator3,
            this.raidsayShareMacroToolStripMenuItem,
            this.groupsayShareMacroToolStripMenuItem,
            this.toolStripSeparator2,
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
            this.contextMenuStripTrig.Name = "contextMenuStrip1";
            this.contextMenuStripTrig.Size = new System.Drawing.Size(290, 226);
            this.contextMenuStripTrig.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTrg_Opening);
            // 
            // copyAsShareableXMLToolStripMenuItem
            // 
            this.copyAsShareableXMLToolStripMenuItem.Name = "copyAsShareableXMLToolStripMenuItem";
            this.copyAsShareableXMLToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.copyAsShareableXMLToolStripMenuItem.Text = "Copy as Shareable XML";
            this.copyAsShareableXMLToolStripMenuItem.ToolTipText = "For pasting into EQII chat";
            this.copyAsShareableXMLToolStripMenuItem.Click += new System.EventHandler(this.copyAsShareableXMLToolStripMenuItem_Click);
            // 
            // copyAsDoubleEncodedXMLToolStripMenuItem
            // 
            this.copyAsDoubleEncodedXMLToolStripMenuItem.Name = "copyAsDoubleEncodedXMLToolStripMenuItem";
            this.copyAsDoubleEncodedXMLToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.copyAsDoubleEncodedXMLToolStripMenuItem.Text = "Copy as Double-Encoded Shareable XML";
            this.copyAsDoubleEncodedXMLToolStripMenuItem.ToolTipText = "For pasting in a web forum post";
            this.copyAsDoubleEncodedXMLToolStripMenuItem.Click += new System.EventHandler(this.copyAsDoubleEncodedXMLToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(286, 6);
            // 
            // editTriggerToolStripMenuItem
            // 
            this.editTriggerToolStripMenuItem.Name = "editTriggerToolStripMenuItem";
            this.editTriggerToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.editTriggerToolStripMenuItem.Text = "Edit Trigger";
            this.editTriggerToolStripMenuItem.Click += new System.EventHandler(this.editTriggerToolStripMenuItem_Click);
            // 
            // deleteTriggerToolStripMenuItem
            // 
            this.deleteTriggerToolStripMenuItem.Name = "deleteTriggerToolStripMenuItem";
            this.deleteTriggerToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteTriggerToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.deleteTriggerToolStripMenuItem.Text = "Delete trigger";
            this.deleteTriggerToolStripMenuItem.Click += new System.EventHandler(this.deleteTriggerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(286, 6);
            // 
            // playAlertSoundToolStripMenuItem
            // 
            this.playAlertSoundToolStripMenuItem.Name = "playAlertSoundToolStripMenuItem";
            this.playAlertSoundToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.playAlertSoundToolStripMenuItem.Text = "Play Alert Sound";
            this.playAlertSoundToolStripMenuItem.Click += new System.EventHandler(this.playAlertSoundToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(286, 6);
            // 
            // raidsayShareMacroToolStripMenuItem
            // 
            this.raidsayShareMacroToolStripMenuItem.Name = "raidsayShareMacroToolStripMenuItem";
            this.raidsayShareMacroToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.raidsayShareMacroToolStripMenuItem.Text = "Raidsay Share Macro";
            this.raidsayShareMacroToolStripMenuItem.ToolTipText = "Make triggers.txt macro to share trigger with the raid";
            this.raidsayShareMacroToolStripMenuItem.Click += new System.EventHandler(this.raidsayShareMacroToolStripMenuItem_Click);
            // 
            // groupsayShareMacroToolStripMenuItem
            // 
            this.groupsayShareMacroToolStripMenuItem.Name = "groupsayShareMacroToolStripMenuItem";
            this.groupsayShareMacroToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.groupsayShareMacroToolStripMenuItem.Text = "Groupsay Share Macro";
            this.groupsayShareMacroToolStripMenuItem.ToolTipText = "Make triggers.txt macro to share trigger with the group";
            this.groupsayShareMacroToolStripMenuItem.Click += new System.EventHandler(this.groupsayShareMacroToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(286, 6);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.expandAllToolStripMenuItem.Text = "Expand all triggers";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse all triggers";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 750;
            // 
            // contextMenuStripCat
            // 
            this.contextMenuStripCat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyZoneNameToClipboardToolStripMenuItem,
            this.deleteEntireCategoryToolStripMenuItem,
            this.toggleEntireCategoryToolStripMenuItem,
            this.toolStripSeparator4,
            this.raidShareCategoryMacroMenuItem,
            this.groupShareCategoryMacroMenuItem,
            this.shareDialogMenuItem,
            this.toolStripSeparator7,
            this.categorySpellTimersMenuItem});
            this.contextMenuStripCat.Name = "contextMenuStrip2";
            this.contextMenuStripCat.Size = new System.Drawing.Size(252, 192);
            this.contextMenuStripCat.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripCat_Opening);
            // 
            // copyZoneNameToClipboardToolStripMenuItem
            // 
            this.copyZoneNameToClipboardToolStripMenuItem.Name = "copyZoneNameToClipboardToolStripMenuItem";
            this.copyZoneNameToClipboardToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.copyZoneNameToClipboardToolStripMenuItem.Text = "Copy category name to clipboard";
            this.copyZoneNameToClipboardToolStripMenuItem.ToolTipText = "To paste this Category /Zone into a trigger specification";
            this.copyZoneNameToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyZoneNameToClipboardToolStripMenuItem_Click);
            // 
            // deleteEntireCategoryToolStripMenuItem
            // 
            this.deleteEntireCategoryToolStripMenuItem.Name = "deleteEntireCategoryToolStripMenuItem";
            this.deleteEntireCategoryToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.deleteEntireCategoryToolStripMenuItem.Text = "Delete entire category";
            this.deleteEntireCategoryToolStripMenuItem.ToolTipText = "Delete the category and all its triggers";
            this.deleteEntireCategoryToolStripMenuItem.Click += new System.EventHandler(this.deleteEntireCategoryToolStripMenuItem_Click);
            // 
            // toggleEntireCategoryToolStripMenuItem
            // 
            this.toggleEntireCategoryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableAllTriggersToolStripMenuItem,
            this.disableAllTriggersToolStripMenuItem,
            this.enableOnZoneinToolStripMenuItem});
            this.toggleEntireCategoryToolStripMenuItem.Name = "toggleEntireCategoryToolStripMenuItem";
            this.toggleEntireCategoryToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.toggleEntireCategoryToolStripMenuItem.Text = "Modify entire category";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(248, 6);
            // 
            // raidShareCategoryMacroMenuItem
            // 
            this.raidShareCategoryMacroMenuItem.Name = "raidShareCategoryMacroMenuItem";
            this.raidShareCategoryMacroMenuItem.Size = new System.Drawing.Size(251, 22);
            this.raidShareCategoryMacroMenuItem.Text = "Raidsay Share Macro";
            this.raidShareCategoryMacroMenuItem.ToolTipText = "Make a triggers.txt macro to share category\'s enabled triggers with the raid";
            this.raidShareCategoryMacroMenuItem.Click += new System.EventHandler(this.raidShareCategoryMacroToolStripMenuItem_Click);
            // 
            // groupShareCategoryMacroMenuItem
            // 
            this.groupShareCategoryMacroMenuItem.Name = "groupShareCategoryMacroMenuItem";
            this.groupShareCategoryMacroMenuItem.Size = new System.Drawing.Size(251, 22);
            this.groupShareCategoryMacroMenuItem.Text = "Groupsay Share Macro";
            this.groupShareCategoryMacroMenuItem.ToolTipText = "Make a triggers.txt macro to share the category\'s triggers with the group";
            this.groupShareCategoryMacroMenuItem.Click += new System.EventHandler(this.groupShareCategoryMacroToolStripMenuItem_Click);
            // 
            // shareDialogMenuItem
            // 
            this.shareDialogMenuItem.Name = "shareDialogMenuItem";
            this.shareDialogMenuItem.Size = new System.Drawing.Size(251, 22);
            this.shareDialogMenuItem.Text = "Share Dialog...";
            this.shareDialogMenuItem.Click += new System.EventHandler(this.shareDialogMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(248, 6);
            // 
            // categorySpellTimersMenuItem
            // 
            this.categorySpellTimersMenuItem.Name = "categorySpellTimersMenuItem";
            this.categorySpellTimersMenuItem.Size = new System.Drawing.Size(251, 22);
            this.categorySpellTimersMenuItem.Text = "Category Spell Timers";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(559, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Right-click a category or trigger for menu choices.  Right-click a blank line in " +
    "the triggers pane to create a new trigger.";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(727, 38);
            this.panel1.TabIndex = 0;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(690, 3);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(29, 13);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(476, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Double-click to edit trigger fields. Expand a trigger for checkbox and right-clic" +
    "k actions on sub-items.";
            // 
            // enableAllTriggersToolStripMenuItem
            // 
            this.enableAllTriggersToolStripMenuItem.Name = "enableAllTriggersToolStripMenuItem";
            this.enableAllTriggersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.enableAllTriggersToolStripMenuItem.Text = "Enable all triggers";
            this.enableAllTriggersToolStripMenuItem.Click += new System.EventHandler(this.enableAllTriggersToolStripMenuItem_Click);
            // 
            // disableAllTriggersToolStripMenuItem
            // 
            this.disableAllTriggersToolStripMenuItem.Name = "disableAllTriggersToolStripMenuItem";
            this.disableAllTriggersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disableAllTriggersToolStripMenuItem.Text = "Disable all triggers";
            this.disableAllTriggersToolStripMenuItem.Click += new System.EventHandler(this.disableAllTriggersToolStripMenuItem_Click);
            // 
            // enableOnZoneinToolStripMenuItem
            // 
            this.enableOnZoneinToolStripMenuItem.Name = "enableOnZoneinToolStripMenuItem";
            this.enableOnZoneinToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.enableOnZoneinToolStripMenuItem.Text = "Enable on zone-in";
            this.enableOnZoneinToolStripMenuItem.ToolTipText = "Also ignores color codes and instance numbers";
            this.enableOnZoneinToolStripMenuItem.Click += new System.EventHandler(this.enableOnZoneinToolStripMenuItem_Click);
            // 
            // TriggerTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "TriggerTree";
            this.Size = new System.Drawing.Size(727, 598);
            this.VisibleChanged += new System.EventHandler(this.TriggerTree_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStripTrig.ResumeLayout(false);
            this.contextMenuStripCat.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

        #endregion

        private Label label3;
        private Label label4;
        private SplitContainer splitContainer1;
        private TreeView treeViewCats;
        private TreeView treeViewTrigs;
        private ContextMenuStrip contextMenuStripTrig;
        private ToolStripMenuItem copyAsShareableXMLToolStripMenuItem;
        private ToolStripMenuItem copyAsDoubleEncodedXMLToolStripMenuItem;
        private ToolTip toolTip1;
        private ContextMenuStrip contextMenuStripCat;
        private ToolStripMenuItem copyZoneNameToClipboardToolStripMenuItem;
        private ToolStripMenuItem deleteEntireCategoryToolStripMenuItem;
        private Label label1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem expandAllToolStripMenuItem;
        private ToolStripMenuItem collapseAllToolStripMenuItem;
        private Panel panel1;
        private Button buttonFindNext;
        private TextBoxX textBoxTrigFind;
        private Panel panel2;
        private Panel panel3;
        private TextBoxX textBoxCatFind;
        private Button buttonCatFindNext;
        private ToolStripMenuItem playAlertSoundToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private Label label2;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem raidShareCategoryMacroMenuItem;
        private ToolStripMenuItem groupShareCategoryMacroMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem raidsayShareMacroToolStripMenuItem;
        private ToolStripMenuItem groupsayShareMacroToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem deleteTriggerToolStripMenuItem;
        private ToolStripMenuItem editTriggerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem categorySpellTimersMenuItem;
        private System.Windows.Forms.CheckBox checkBoxCurrentCategory;
        private LinkLabel linkLabel1;
        private ToolStripMenuItem shareDialogMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonNew;
        private ToolStripButton toolStripButtonResults;

        #endregion

        public TriggerTree()
		{
            InitializeComponent();
		}


        #region IActPluginV1 Members

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
		{
			lblStatus = pluginStatusText;	            // Hand the status label's reference to our local var
			pluginScreenSpace.Controls.Add(this);	    // Add this UserControl to the tab ACT provides
			this.Dock = DockStyle.Fill;                 // Expand the UserControl to fill the tab's client space
            xmlSerializer = new XmlSerializer(typeof(Config));

            LoadSettings();

            treeImages.Images.Add(GetFolderBitmap());
            treeImages.Images.Add(GetOpenFolderBitmap());
            treeViewCats.ImageList = treeImages;

            //set images so that triggerCanMacro and triggerNoMacro show the appropirate image
            triggerImages.Images.Add(Macros.GetActionBitmap());
            triggerBlankImage = triggerImages.Images.Count;
            treeViewTrigs.ImageList = triggerImages;

            formResultsTabs = new FormResultsTabs(config);

            PopulateCatsTree(null);

            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;        //for zone change
            ActGlobals.oFormActMain.LogFileChanged += OFormActMain_LogFileChanged;      //for zone change
            ActGlobals.oFormActMain.XmlSnippetAdded += OFormActMain_XmlSnippetAdded;    //for incoming shared trigger

            if (ActGlobals.oFormActMain.GetAutomaticUpdatesAllowed())
            {
                // If ACT is set to automatically check for updates, check for updates to the plugin
                // If we don't put this on a separate thread, web latency will delay the plugin init phase
                new Thread(new ThreadStart(oFormActMain_UpdateCheckClicked)).Start();
            }

            lblStatus.Text = "Plugin Started";
		}

        public void DeInitPlugin()
		{
            formResultsTabs.DeInit();
            formResultsTabs.Close();
            if(addEditButton != null)
                addEditButton.Click -= AddEditButton_Click;
            if(removeButton != null)
                removeButton.Click -= RemoveButton_Click;

            // Unsubscribe from any events you listen to when exiting!
            ActGlobals.oFormActMain.OnLogLineRead -= OFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.LogFileChanged -= OFormActMain_LogFileChanged;
            ActGlobals.oFormActMain.XmlSnippetAdded -= OFormActMain_XmlSnippetAdded;

            foreach(string s in config.autoCats)
                RestrictAllTriggers(s, true);

            SaveSettings();
			lblStatus.Text = "Plugin Exited";
		}

        #endregion IActPluginV1 Members

        void oFormActMain_UpdateCheckClicked()
        {
            int pluginId = 80;

            try
            {
                Version localVersion = this.GetType().Assembly.GetName().Version;
                Version remoteVersion = new Version(ActGlobals.oFormActMain.PluginGetRemoteVersion(pluginId).TrimStart(new char[] { 'v' }));    // Strip any leading 'v' from the string before passing to the Version constructor
                if (remoteVersion.CompareTo(localVersion) > 0)
                {
                    Rectangle screen = Screen.GetWorkingArea(ActGlobals.oFormActMain);
                    DialogResult result = SimpleMessageBox.Show(new Point(screen.Width / 2 - 100, screen.Height / 2 - 100),
                          @"There is an update for TriggerTree."
                        + @"\line Update it now?"
                        + @"\line (If there is an update to ACT"
                        + @"\line you should click No and update ACT first.)"
                        + @"\line\line Release notes at project website:"
                        + @"{\line\ql https://github.com/jeffjl74/ACT_TriggerTree#overview}"
                        , "Trigger Tree New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        FileInfo updatedFile = ActGlobals.oFormActMain.PluginDownload(pluginId);
                        ActPluginData pluginData = ActGlobals.oFormActMain.PluginGetSelfData(this);
                        pluginData.pluginFile.Delete();
                        updatedFile.MoveTo(pluginData.pluginFile.FullName);
                        Application.DoEvents();
                        ThreadInvokes.CheckboxSetChecked(ActGlobals.oFormActMain, pluginData.cbEnabled, false);
                        Application.DoEvents();
                        ThreadInvokes.CheckboxSetChecked(ActGlobals.oFormActMain, pluginData.cbEnabled, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(ex, "Trigger Tree Plugin Update Download");
            }
        }

        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            // for EQII, the English parser regexArray[9] (i.e. detectedType 10) is the one that looks for a "You have entered" zone change
            // so we only need to watch detectedType == 10 for a zone change
            if (logInfo.detectedType == 10)
            {
                // pull out just the zone name (remove color and instance #)
                Match match = reCleanLogZone.Match(logInfo.logLine);
                if (match.Success)
                {
                    zoneMatchedEQII = true;
                    decoratedZoneName = logInfo.detectedZone;
                    string cleanZoneName = match.Groups["zone"].Value.Trim();
                    if (config.autoCats.Contains(cleanZoneName))
                        zoneName = cleanZoneName;
                    else
                        zoneName = decoratedZoneName;
                    mUiContext.Post(CheckAutoRestrict, null);
                    mUiContext.Post(UpdateCategoryColors, true);
                    mUiContext.Post(UpdateTriggerColors, null);
                }
            }
            // to cover other parsers without costing the time to run the EQII regex
            if(!zoneMatchedEQII)
            {
                //to reduce overhead, we only use type == 0 lines that ACT did not parse
                if (logInfo.detectedType == 0 && logInfo.logLine.Length > logTimeStampLength)
                {
                    //look for zone difference between what we've seen before
                    if (!string.IsNullOrEmpty(logInfo.detectedZone) && logInfo.detectedZone != zoneName)
                    {
                        zoneName = decoratedZoneName = logInfo.detectedZone;
                        mUiContext.Post(UpdateCategoryColors, true);
                        mUiContext.Post(UpdateTriggerColors, null);
                    }
                }
            }
        }

        private void OFormActMain_LogFileChanged(bool IsImport, string NewLogFileName)
        {
            Match match = reCleanActZone.Match(ActGlobals.oFormActMain.CurrentZone);
            if (match.Success)
            {
                zoneMatchedEQII = true;
                decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                string cleanZoneName = match.Groups["zone"].Value.Trim();
                if (config.autoCats.Contains(cleanZoneName))
                    zoneName = cleanZoneName;
                else
                    zoneName = decoratedZoneName;
            }
            else
                zoneName = decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
            mUiContext.Post(CheckAutoRestrict, null);
        }

        private void CheckAutoRestrict(object o)
        {
            foreach (string s in config.autoCats)
            {
                if (s == zoneName)
                    RestrictAllTriggers(s, false);
                else
                    RestrictAllTriggers(s, true);
            }
        }

        private void OFormActMain_XmlSnippetAdded(object sender, XmlSnippetEventArgs e)
        {
            if (e.ShareType == "Trigger")
            {
                //we need to rebuild if there is a new trigger share incoming
                mUiContext.Post(PopulateCatsTree, null);
            }
        }

        void LoadSettings()
		{
            if (File.Exists(settingsFile))
            {
                try
                {
                    using (FileStream xfs = new FileStream(settingsFile, FileMode.Open))
                    {
                        config = (Config)xmlSerializer.Deserialize(xfs);
                    }
                    toolStripButtonResults.Checked = config.ResultsPopup;
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error loading settings: " + ex.Message;
                    config = new Config();
                }
            }
            else
                config = new Config();
        }

        void SaveSettings()
		{
            //store the splitter location
            // but only save it if it was ever set
            if (!neverBeenVisible)
            {
                config.SettingsSerializer.Int32.Value = splitContainer1.SplitterDistance;
            }

            using (TextWriter writer = new StreamWriter(settingsFile))
            {
                xmlSerializer.Serialize(writer, config);
                writer.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                SimpleMessageBox.Show(ActGlobals.oFormActMain, ex.Message, "Unable to open link that was clicked.");
            }
        }

        private void VisitLink()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            linkLabel1.LinkVisited = true;
            //Call the Process.Start method to open the default browser
            //with a URL:
            System.Diagnostics.Process.Start("https://github.com/jeffjl74/ACT_TriggerTree#overview");
        }

        #region --------------- Category Tree

        void PopulateCatsTree(object o)
        {
            //try to save the current selection
            string prevCat = string.Empty;
            List<TreeNode> expnodes = new List<TreeNode>();
            if (treeDict != null)
            {
                TreeNode sel = treeViewCats.SelectedNode;
                if(sel != null)
                {
                    prevCat = sel.Text;
                    foreach(TreeNode tn in treeViewTrigs.Nodes)
                    {
                        if (tn.IsExpanded)
                            expnodes.Add(tn);
                    }
                }
            }

            treeDict = new Dictionary<string, List<CustomTrigger>>();
            treeViewCats.Nodes.Clear();

            int keyCount = ActGlobals.oFormActMain.CustomTriggers.Keys.Count;
            for(int i=0; i<keyCount; i++)
            {
                string regex = ActGlobals.oFormActMain.CustomTriggers.Keys[i];
                CustomTrigger trigger;
                if(ActGlobals.oFormActMain.CustomTriggers.TryGetValue(regex, out trigger))
                {
                    string category = trigger.Category;
                    //add this category if not already there
                    if(!treeDict.ContainsKey(category))
                    {
                        treeDict.Add(category, new List<CustomTrigger>());
                        //add the category to the tree
                        TreeNode node = treeViewCats.Nodes.Add(category);
                        node.ImageIndex = 0;    //closed folder
                        node.Name = category;   //for .Find()
                    }
                    //add this trigger to the category's list of triggers
                    List<CustomTrigger> list;
                    if(treeDict.TryGetValue(category, out list))
                    {
                        list.Add(trigger);
                    }
                    // if there is a result tab, add it to our watch list
                    if (trigger.Tabbed)
                    {
                        formResultsTabs.AddTab(trigger.ResultsTab);

                        if (resultsTabCtrl == null && trigger.ResultsTab.Parent != null)
                        {
                            // save the tab ctrl for adding back a removed tab
                            resultsTabCtrl = (TabControl)trigger.ResultsTab.Parent;

                            // to keep up with Results-Tabs enable/disable,
                            // we need to monitor ACT's Add/Edit &Remove buttons
                            TabPage trigtab = resultsTabCtrl.TabPages[0];
                            foreach (Control ctrl in trigtab.Controls)
                            {
                                if (ctrl.GetType() == typeof(Button))
                                {
                                    Button button = (Button)ctrl;
                                    if (button.Text == "Add/Edit")
                                    {
                                        addEditButton = button;
                                        addEditButton.Click += AddEditButton_Click;
                                    }
                                    else if(button.Text == "&Remove")
                                    {
                                        removeButton = button;
                                        removeButton.Click += RemoveButton_Click;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            mUiContext.Post(UpdateCategoryColors, false);

            //try to restore previous selection
            if (!string.IsNullOrEmpty(prevCat))
            {
                TreeNode[] nodes = treeViewCats.Nodes.Find(prevCat, false);
                if (nodes.Length > 0)
                {
                    treeViewCats.SelectedNode = nodes[0];
                    treeViewCats.SelectedNode.EnsureVisible();
                    foreach (TreeNode node in expnodes)
                    {
                        TreeNode[] NodeList = treeViewTrigs.Nodes.Find(node.Name, true);
                        if (NodeList.Length > 0)
                            NodeList[0].Expand();
                    }
                }
            }
            else
            {
                if(treeViewCats.Nodes.Count > 0)
                    treeViewCats.SelectedNode = treeViewCats.Nodes[0];
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            // user pressed the ACT Custom Triggers Remove button
            PopulateCatsTree(null);
        }

        private void AddEditButton_Click(object sender, EventArgs e)
        {
            // user pressed the ACT Custom Triggers Add/Edit button
            PopulateCatsTree(null);
        }

        private void UpdateCategoryColors(object o)
        {
            bool autoSelect = (bool)o;
            foreach (TreeNode category in treeViewCats.Nodes)
            {
                category.BackColor = inactiveBackground; //default
                category.Tag = false; //no active triggers
                string key = category.Text;
                List<CustomTrigger> list;
                if(treeDict.TryGetValue(key, out list))
                {
                    //look through the triggers to determine if the category should be green
                    foreach(CustomTrigger trigger in list)
                    {
                        if (trigger.Active)
                        {
                            if (trigger.RestrictToCategoryZone == false || key.Equals(zoneName) || key.Equals(decoratedZoneName))
                            {
                                category.BackColor = activeBackground;
                                category.Tag = true; //active triggers in this category, for drawing the color
                                if(key.Equals(zoneName) && autoSelect)
                                {
                                    treeViewCats.SelectedNode = category;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void treeViewCats_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;


            // if treeview's HideSelection property is "True", 
            // this will always returns "False" on unfocused treeview
            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var unfocused = !e.Node.TreeView.Focused;
            bool green = e.Node.Tag == null ? false : (bool)e.Node.Tag;

            // keep the focused highlight if selected and unfocused
            // draw green background if not selected and triggers are active
            // otherwise, default colors
            if (selected && unfocused)
            {
                var font = e.Node.NodeFont ?? e.Node.TreeView.Font;
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, SystemColors.HighlightText, TextFormatFlags.GlyphOverhangPadding);
            }
            else if (!selected && green)
            {
                var font = e.Node.NodeFont ?? e.Node.TreeView.Font;
                e.Graphics.FillRectangle(activeBackgroundBrush, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, SystemColors.ControlText, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private Bitmap GetFolderBitmap()
        {
            //could not figure out a better way to get an image into this file
            //use https://littlevgl.com/image-to-c-array to convert Visual Studio Image Library to C code
            byte[] png_data = new byte[] {
                  0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
                  0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0xf3, 0xff,
                  0x61, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e,
                  0xc4, 0x01, 0x95, 0x2b, 0x0e, 0x1b, 0x00, 0x00, 0x00, 0xa3, 0x49, 0x44, 0x41, 0x54, 0x78, 0x5e,
                  0xed, 0xd3, 0xad, 0x0a, 0x42, 0x41, 0x10, 0x05, 0xe0, 0x73, 0x75, 0x82, 0x4c, 0xf1, 0x06, 0x83,
                  0xd8, 0x6c, 0x26, 0x59, 0xa3, 0xcd, 0x27, 0xf0, 0x59, 0x6c, 0x16, 0xe1, 0x1a, 0x2c, 0x66, 0x1f,
                  0xc6, 0x6a, 0xd3, 0xe6, 0xbe, 0x80, 0xc5, 0x20, 0x08, 0x06, 0x7f, 0x60, 0x11, 0x44, 0xd6, 0x83,
                  0x20, 0x8b, 0x22, 0x0b, 0x72, 0x83, 0x20, 0x7e, 0x70, 0x18, 0xa6, 0xcc, 0xec, 0xc2, 0x6e, 0xe2,
                  0xbd, 0x47, 0x1e, 0x89, 0x73, 0x6e, 0xc9, 0x6a, 0x10, 0xb7, 0x67, 0xea, 0xaa, 0xca, 0x1a, 0x70,
                  0x39, 0xe4, 0x7a, 0x3e, 0x98, 0xed, 0x62, 0x82, 0x98, 0xb4, 0xd1, 0x4d, 0xb5, 0xda, 0xec, 0x01,
                  0x18, 0xe2, 0x85, 0x14, 0x4b, 0x65, 0x10, 0x2a, 0xed, 0x3e, 0xde, 0xb9, 0x1c, 0xd7, 0x38, 0xad,
                  0xa6, 0xe0, 0x80, 0x8c, 0xa7, 0xcd, 0x10, 0x58, 0xa6, 0x25, 0x8f, 0x6e, 0x37, 0x1f, 0x23, 0x66,
                  0x33, 0x1b, 0x3d, 0xf5, 0xb5, 0xce, 0xc0, 0x80, 0x0a, 0xc8, 0xe1, 0x67, 0x06, 0xfc, 0x07, 0x08,
                  0x63, 0xef, 0x8f, 0xe2, 0x73, 0x16, 0x14, 0x7e, 0xe3, 0xb7, 0xae, 0x70, 0x03, 0xb8, 0x17, 0x29,
                  0xc4, 0xad, 0x11, 0x0a, 0x52, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60,
                  0x82,
            };

            Bitmap bmp;
            using (var ms = new MemoryStream(png_data))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        private Bitmap GetOpenFolderBitmap()
        {
            //could not figure out a better way to get an image in this file
            byte[] png_data = new byte[] {
                   0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
                  0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0xf3, 0xff,
                  0x61, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e,
                  0xc4, 0x01, 0x95, 0x2b, 0x0e, 0x1b, 0x00, 0x00, 0x01, 0x0b, 0x49, 0x44, 0x41, 0x54, 0x78, 0x5e,
                  0xa5, 0xd2, 0xb1, 0x4a, 0xc4, 0x40, 0x14, 0x86, 0xd1, 0x2f, 0x63, 0x84, 0x10, 0x15, 0x2c, 0x84,
                  0xdd, 0xc2, 0x62, 0x3b, 0x3b, 0x4b, 0xb5, 0x10, 0xf1, 0x15, 0x7c, 0x83, 0xdd, 0xde, 0xd2, 0xca,
                  0x46, 0x04, 0x2b, 0xb1, 0xb1, 0xb6, 0xd8, 0xf8, 0x0a, 0x16, 0x96, 0xba, 0x85, 0x85, 0x82, 0x85,
                  0x5a, 0x59, 0x88, 0xae, 0x20, 0x1a, 0x50, 0x24, 0xa2, 0x64, 0x57, 0xd8, 0xdd, 0xf1, 0x26, 0x38,
                  0xc1, 0x85, 0xd1, 0x44, 0x72, 0xe0, 0x27, 0x45, 0x98, 0x3b, 0xf7, 0xce, 0x8c, 0x83, 0x88, 0xe3,
                  0xb8, 0x06, 0x48, 0x7e, 0x75, 0xe1, 0xfb, 0x7e, 0x84, 0x85, 0x2b, 0x8b, 0x9b, 0x40, 0x9d, 0xbf,
                  0x05, 0x40, 0x03, 0x0b, 0xe7, 0xfd, 0xf5, 0x49, 0x3f, 0x9f, 0xef, 0x31, 0xe8, 0x75, 0xb1, 0x19,
                  0xf1, 0x26, 0xa9, 0x2c, 0xac, 0x22, 0x56, 0x00, 0xd3, 0x45, 0x5b, 0x3a, 0x6a, 0xf3, 0xdd, 0xbe,
                  0xbe, 0x39, 0x5c, 0xd7, 0xf2, 0xb5, 0xe6, 0xee, 0x68, 0x5b, 0xbf, 0xdc, 0x9e, 0xda, 0xfe, 0x35,
                  0x11, 0x2e, 0x39, 0x26, 0x6a, 0x4b, 0x44, 0xd7, 0x07, 0x69, 0x0c, 0xe5, 0x7a, 0x54, 0x17, 0xd7,
                  0xea, 0x40, 0x23, 0xb7, 0x80, 0x5f, 0x9d, 0x4d, 0xf3, 0xd3, 0x63, 0x6b, 0x0b, 0x43, 0x51, 0x82,
                  0x8c, 0xb1, 0xac, 0x28, 0xe7, 0xd8, 0x1d, 0xaa, 0x18, 0x5e, 0x99, 0x59, 0x73, 0x85, 0x27, 0x3b,
                  0xc9, 0x39, 0x0c, 0x8f, 0xd0, 0x09, 0x2f, 0x29, 0xca, 0x9b, 0x9a, 0x41, 0x04, 0x59, 0x81, 0x7e,
                  0xf7, 0x8d, 0xcf, 0xe8, 0x9e, 0xa2, 0xc6, 0xa6, 0xe7, 0x10, 0xfb, 0x59, 0x81, 0x8f, 0x87, 0x33,
                  0x8a, 0x1a, 0x1d, 0xaf, 0x24, 0x49, 0x1e, 0x53, 0x4b, 0x99, 0x7b, 0xed, 0xc8, 0xfc, 0xc5, 0x77,
                  0x9f, 0x47, 0xec, 0x9a, 0x87, 0xb4, 0x29, 0x87, 0xb1, 0xc1, 0xff, 0x05, 0x08, 0x47, 0x6b, 0x4d,
                  0x19, 0x8a, 0x92, 0xbe, 0x00, 0xfd, 0xab, 0x7c, 0xc4, 0x09, 0xdb, 0x7d, 0x59, 0x00, 0x00, 0x00,
                  0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82,
              };

            Bitmap bmp;
            using (var ms = new MemoryStream(png_data))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        private void treeViewCats_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                treeViewCats.SelectedNode.ImageIndex = 0;
                treeViewCats.SelectedNode.SelectedImageIndex = 1;
                treeViewCats.HideSelection = false;
                string category = treeViewCats.SelectedNode.Text;

                UpdateTriggerList(category);
                treeViewCats.SelectedNode.BackColor = activeBackground;
            }
        }

        private void deleteEntireCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                string category = clickedCategoryNode.Text;
                if (SimpleMessageBox.Show(ActGlobals.oFormActMain, "Delete category '" + category + "' and all its triggers?", "Are you sure?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    List<CustomTrigger> triggers;
                    if (treeDict.TryGetValue(category, out triggers))
                    {
                        foreach (CustomTrigger trigger in triggers)
                        {
                            DeleteTrigger(trigger, true);
                        }
                    }
                }
            }
        }

        private void copyZoneNameToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                try
                {
                    Clipboard.SetText(clickedCategoryNode.Text);
                }
                catch
                {
                    TraySlider traySlider = new TraySlider();
                    traySlider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                    traySlider.ShowTraySlider("Copy to clipboard failed", "Clipboard Failed");
                }
            }
        }

        private void textBoxCatScroll_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCatFind.Text))
            {
                buttonCatFindNext.Enabled = false;
                catLastFound = string.Empty;
            }
            else
            {
                buttonCatFindNext.Enabled = true;
                FindNextCategory(false);
            }
        }

        private void FindNextCategory(bool resume)
        {
            bool found = false;
            bool foundLast = false;
            string find = textBoxCatFind.Text.ToLower();
            if (!string.IsNullOrEmpty(find))
            {
                foreach (TreeNode node in treeViewCats.Nodes)
                {
                    if (resume && !foundLast)
                    {
                        //keep looping until we get to the last find
                        if (node.Text.Equals(catLastFound))
                        {
                            foundLast = true;
                        }
                    }
                    else
                    {
                        //once past the previous find, look for the next one
                        if (node.Text.ToLower().Contains(find))
                        {
                            treeViewCats.SelectedNode = node;
                            catLastFound = node.Text;
                            node.EnsureVisible();
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    SimpleMessageBox.ShowDialog(ActGlobals.oFormActMain, String.Format(@"\b {0}\b0\line Not found", find), "Not Found");
                }
            }
        }

        private void buttonCatFindNext_Click(object sender, EventArgs e)
        {
            FindNextCategory(true);
        }

        private void treeViewCats_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                clickedCategoryNode = treeViewCats.GetNodeAt(pt);
                if (clickedCategoryNode != null)
                {
                    whereCatMouseDown = treeViewCats.PointToScreen(pt);
                    contextMenuStripCat.Show(whereCatMouseDown);
                }
            }
        }

        private void raidShareCategoryMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteCategoryMacroFile("r ");
        }

        private void groupShareCategoryMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteCategoryMacroFile("g ");
        }

        private void WriteCategoryMacroFile(string sayCmd)
        {
            if (clickedCategoryNode != null)
            {
                List<CustomTrigger> triggers;
                string category = clickedCategoryNode.Text;
                if (treeDict.TryGetValue(category, out triggers))
                {
                    Macros.WriteCategoryMacroFile(sayCmd, triggers, categoryTimers, true);
                }
            }
        }

        private void contextMenuStripCat_Opening(object sender, CancelEventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                string category = clickedCategoryNode.Text;
                int canMacroTimer = 0;
                int cannotMacroTimer = 0;

                //get tagged spell timers for the category
                categorySpellTimersMenuItem.DropDownItems.Clear();
                categoryTimers = FindCategoryTimers(category);

                //count triggers for the menu text
                List<CustomTrigger> triggers;
                int valid = 0;
                int invalid = 0;
                int active = 0;
                int inactive = 0;
                if (treeDict.TryGetValue(category, out triggers))
                {
                    foreach (CustomTrigger trigger in triggers)
                    {
                        if (trigger.Active)
                        {
                            active++;
                            if (!Macros.IsInvalidMacroTrigger(trigger))
                            {
                                valid++;
                                groupShareCategoryMacroMenuItem.Enabled = true;
                                raidShareCategoryMacroMenuItem.Enabled = true;
                                groupShareCategoryMacroMenuItem.ToolTipText = validCategoryText + "group";
                                raidShareCategoryMacroMenuItem.ToolTipText = validCategoryText + "raid";
                                //break;
                            }
                            else
                                invalid++;
                            // find timers that are activated by a trigger
                            List<TimerData> timers = TriggerTree.FindTimers(trigger);
                            foreach (TimerData timer in timers)
                            {
                                if (!categoryTimers.Contains(timer))
                                {
                                    categoryTimers.Add(timer);
                                }
                            }

                        }
                        else
                            inactive++;
                    }

                    enableOnZoneinToolStripMenuItem.Checked = config.autoCats.Contains(category);

                    if (valid == 0)
                    {
                        groupShareCategoryMacroMenuItem.Text = string.Format(catMacroText, "Groupsay", 0, valid + invalid);
                        groupShareCategoryMacroMenuItem.Enabled = false;
                        raidShareCategoryMacroMenuItem.Text = string.Format(catMacroText, "Raidsay", 0, valid + invalid);
                        raidShareCategoryMacroMenuItem.Enabled = false;
                        groupShareCategoryMacroMenuItem.ToolTipText =
                            string.Format(invalidCategoryText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                        raidShareCategoryMacroMenuItem.ToolTipText =
                            string.Format(invalidCategoryText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                    }
                    else
                    {
                        groupShareCategoryMacroMenuItem.Text = string.Format(catMacroText, "Groupsay", valid + canMacroTimer, valid + canMacroTimer + invalid + cannotMacroTimer);
                        raidShareCategoryMacroMenuItem.Text = string.Format(catMacroText, "Raidsay", valid + canMacroTimer, valid + canMacroTimer + invalid + cannotMacroTimer);
                    }
                }

                // add spell timers menus
                if (categoryTimers.Count == 0)
                {
                    categorySpellTimersMenuItem.Enabled = false;
                    categorySpellTimersMenuItem.ToolTipText = "No Spell Timers with matching Category or Custom Tooltip found";
                }
                else
                {
                    categorySpellTimersMenuItem.Enabled = true;
                    categorySpellTimersMenuItem.ToolTipText = "";
                    foreach (TimerData timer in categoryTimers)
                    {
                        ToolStripMenuItem timerMenuItem = new ToolStripMenuItem();
                        timerMenuItem.Name = timer.Name;
                        timerMenuItem.Text = timer.Name;
                        timerMenuItem.Click += TimerMenuItem_Click;
                        timerMenuItem.MouseDown += TimerMenuItem_MouseDown;
                        timerMenuItem.ToolTipText = "Left-click to search.\nRight-click to copy as shareable XML.";
                        timerMenuItem.Tag = timer;
                        if (!Macros.IsInvalidMacroTimer(timer))
                        {
                            timerMenuItem.Image = triggerImages.Images[triggerCanMacro];
                            canMacroTimer++;
                        }
                        else
                            cannotMacroTimer++;
                        categorySpellTimersMenuItem.DropDownItems.Add(timerMenuItem);
                    }
                    if (canMacroTimer > 0)
                    {
                        categorySpellTimersMenuItem.DropDownItems.Add(new ToolStripSeparator());
                        ToolStripMenuItem timerRaidMacroItem = new ToolStripMenuItem();
                        string txt = string.Format(catMacroText, "Raidsay", canMacroTimer, canMacroTimer + cannotMacroTimer);
                        timerRaidMacroItem.Name = timerRaidMacroItem.Text = txt;
                        timerRaidMacroItem.Click += TimerMacroItem_Click;
                        categorySpellTimersMenuItem.DropDownItems.Add(timerRaidMacroItem);

                        ToolStripMenuItem timerGroupMacroItem = new ToolStripMenuItem();
                        txt = string.Format(catMacroText, "Groupsay", canMacroTimer, canMacroTimer + cannotMacroTimer);
                        timerGroupMacroItem.Name = timerGroupMacroItem.Text = txt;
                        timerGroupMacroItem.Click += TimerMacroItem_Click;
                        categorySpellTimersMenuItem.DropDownItems.Add(timerGroupMacroItem);
                    }
                }
            }
        }

        private void shareDialogMenuItem_Click(object sender, EventArgs e)
        {
            string category = clickedCategoryNode.Text;
            string prefix = "/g "; 
            if (category.Contains("["))
            {
                if (category.Contains("Raid"))
                    prefix = "/r ";
            }

            List<CustomTrigger> triggers;
            if (treeDict.TryGetValue(category, out triggers))
            {
                XmlCopyForm form = new XmlCopyForm(prefix, categoryTimers, triggers);
                form.Show();
                PositionChildForm(form, whereCatMouseDown);
            }
        }

        private void TimerMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            lastSpellMenuButton = e.Button;
        }

        private void TimerMacroItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menu = sender as ToolStripMenuItem;

            //group or raid?
            string chan;
            if (menu.Text.StartsWith("R"))
                chan = "r ";
            else
                chan = "g ";

            StringBuilder sb = new StringBuilder();
            int validTimers = 0;
            int invalid = 0;
            int fileCount = 0;
            //categoryTimers was created when the context menu opened
            foreach (TimerData timer in categoryTimers)
            {
                if (!Macros.IsInvalidMacroTimer(timer))
                {
                    sb.Append(chan);
                    sb.Append(Macros.SpellTimerToMacro(timer));
                    sb.Append(Environment.NewLine);
                    validTimers++;
                    if (validTimers >= 16)
                    {
                        Macros.MacroToFile(fileCount, string.Empty, sb.ToString(), invalid, validTimers, 0);
                        fileCount++;
                        sb.Clear();
                        invalid = 0;
                        validTimers = 0;
                    }
                }
                else
                {
                    invalid++;
                }
            }

            if (validTimers > 0)
            {
                Macros.MacroToFile(fileCount, string.Empty, sb.ToString(), invalid, validTimers, 0);
            }
        }

        private void TimerMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menu = sender as ToolStripMenuItem;
            if (lastSpellMenuButton == MouseButtons.Left)
            {
                //search for the timer in the ACT spell timers
                string timerName = menu.Name.ToLower();
                ActGlobals.oFormSpellTimers.SearchSpellTreeView(timerName);
                ActGlobals.oFormSpellTimers.Visible = true;
            }
            else if(lastSpellMenuButton == MouseButtons.Right)
            {
                //copy the XML descriptoin of the timer to the clipboard
                TimerData timer = menu.Tag as TimerData;
                if(timer != null)
                {
                    string xml = Macros.SpellTimerToXML(timer);
                    try
                    {
                        Clipboard.SetText(xml);
                    }
                    catch (Exception)
                    {
                        TraySlider slider = new TraySlider();
                        slider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                        slider.ShowTraySlider("Problem with the clipboard. Try copying again.");
                    }
                }
            }
        }

        private List<TimerData> FindCategoryTimers(string category)
        {
            List<TimerData> result = new List<TimerData>();
            if (!string.IsNullOrEmpty(category))
            {
                string macroCat = category.Replace("'", ".");
                // just search through all timers for a .Category or .Tooltip that matches the passed category
                foreach (TimerData timer in ActGlobals.oFormSpellTimers.TimerDefs.Values)
                {
                    List<string> tips = timer.Tooltip.Split('|').Select(s => s.Trim()).ToList();
                    if (timer.ActiveInList &&
                        (timer.Category.Equals(category) || tips.Contains(category) || tips.Contains(macroCat)))
                    {
                        if(!result.Exists(t => t.Name.Equals(timer.Name)))
                            result.Add(timer);
                    }
                }
            }
            return result;
        }

        private void RestrictAllTriggers(string catName, bool state)
        {
            List<CustomTrigger> triggers;
            if (treeDict.TryGetValue(catName, out triggers))
            {
                foreach (CustomTrigger trigger in triggers)
                {
                    trigger.RestrictToCategoryZone = state;
                    ActGlobals.oFormActMain.AddEditCustomTrigger(trigger);
                }
                if (catName == treeViewCats.SelectedNode.Text)
                    UpdateTriggerList(catName);
            }
        }

        private void EnableAllTriggers(string catName, bool state)
        {
            List<CustomTrigger> triggers;
            if (treeDict.TryGetValue(catName, out triggers))
            {
                foreach (CustomTrigger trigger in triggers)
                {
                    trigger.Active = state;
                    ActGlobals.oFormActMain.AddEditCustomTrigger(trigger);
                }
            }
        }

        private void enableAllTriggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                EnableAllTriggers(clickedCategoryNode.Text, true);
                UpdateCategoryColors(false);
                if(clickedCategoryNode.Text == treeViewCats.SelectedNode.Text)
                    UpdateTriggerList(clickedCategoryNode.Text);
            }
        }

        private void disableAllTriggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                EnableAllTriggers(clickedCategoryNode.Text, false);
                UpdateCategoryColors(false);
                if(clickedCategoryNode.Text == treeViewCats.SelectedNode.Text)
                    UpdateTriggerList(clickedCategoryNode.Text);
            }
        }

        private void enableOnZoneinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                string category = clickedCategoryNode.Text;
                if (enableOnZoneinToolStripMenuItem.Checked)
                {
                    RestrictAllTriggers(category, true);
                    config.autoCats.Remove(category);
                    enableOnZoneinToolStripMenuItem.Checked = false;
                    if(config.autoCats.Count == 0)
                        zoneMatchedEQII = false;
                }
                else
                {
                    config.autoCats.Add(category);
                    enableOnZoneinToolStripMenuItem.Checked = true;
                    // in case we are in the zone just enabled
                    Match match = reCleanActZone.Match(ActGlobals.oFormActMain.CurrentZone);
                    if (match.Success)
                    {
                        zoneMatchedEQII = true;
                        decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                        string cleanZoneName = match.Groups["zone"].Value.Trim();
                        if (config.autoCats.Contains(cleanZoneName))
                            zoneName = cleanZoneName;
                        else
                            zoneName = decoratedZoneName;
                    }
                    CheckAutoRestrict(null);
                    UpdateCategoryColors(false);
                }
            }
        }

        #endregion Category Tree

        #region --------------- Triggers Tree

        private void UpdateTriggerColors(object o)
        {
            if (treeViewTrigs.Nodes.Count > 0)
            {
                CustomTrigger trigger = treeViewTrigs.Nodes[0].Tag as CustomTrigger;
                if (trigger != null)
                {
                    bool categoryMatch = trigger.Category.Equals(zoneName) || trigger.Category.Equals(decoratedZoneName);

                    foreach (TreeNode node in treeViewTrigs.Nodes)
                    {
                        trigger = node.Tag as CustomTrigger;

                        bool highlightFound = trigger.Key.Equals(keyLastFound);

                        node.ForeColor = disabledColor;         //default

                        if (highlightFound && node.Text.ToLower().Contains(textBoxTrigFind.Text))
                            node.BackColor = foundBackground;
                        else
                            node.BackColor = notFoundBackground;

                        bool isEffective = false;

                        if (node.Checked)
                        {
                            //trigger is enabled
                            // need restricted state from the child to see if it's actually in effect
                            if(node.Nodes.Count > indexRestricted)
                            { 
                                TreeNode child = node.Nodes[indexRestricted];
                                
                                if (child.Checked == false || categoryMatch)
                                {
                                    node.ForeColor = activeColor;   //trigger is active
                                    isEffective = true;
                                }
                                else
                                {
                                    //trigger is enabled but we are in the wrong zone
                                    node.ForeColor = inactiveColor;
                                }
                            }
                        }
                        //step through children to get their color right
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (isEffective)
                                child.ForeColor = activeColor;
                            else
                            {
                                if (node.Checked)
                                    child.ForeColor = inactiveColor;
                                else
                                    child.ForeColor = disabledColor;
                            }
                            if (highlightFound && child.Text.ToLower().Contains(textBoxTrigFind.Text))
                                child.BackColor = foundBackground;
                            else
                                child.BackColor = notFoundBackground;
                        }
                    }
                }
            }
        }

        private void UpdateTriggerList(string category)
        {
            treeViewTrigs.Nodes.Clear();

            List<CustomTrigger> triggers;
            if (treeDict.TryGetValue(category, out triggers))
            {
                foreach (CustomTrigger trigger in triggers)
                {
                    //regex parent
                    TreeNode parent = new TreeNode(trigger.RegEx.ToString());
                    parent.Checked = trigger.Active;
                    parent.Tag = trigger;
                    parent.Name = trigger.RegEx.ToString();
                    if (Macros.IsInvalidMacroTrigger(trigger))
                    {
                        parent.ImageIndex = triggerNoMacro;
                        parent.SelectedImageIndex = triggerNoMacro;
                    }
                    else
                    {
                        parent.ImageIndex = triggerCanMacro;
                        parent.SelectedImageIndex = triggerCanMacro;
                    }
                    treeViewTrigs.Nodes.Add(parent);

                    //sound type child
                    string soundType = alertLabel + trigger.SoundTypeString;
                    if (trigger.SoundType == (int)CustomTriggerSoundTypeEnum.TTS || trigger.SoundType == (int)CustomTriggerSoundTypeEnum.WAV)
                        soundType = soundType + "=" + trigger.SoundData;
                    parent.Nodes.Add(soundType).Checked = true;
                    indexAlertType = 0;
                    parent.Nodes[indexAlertType].ImageIndex = parent.Nodes[indexAlertType].SelectedImageIndex = triggerBlankImage;


                    //zone restrict child
                    parent.Nodes.Add(restrictLabel).Checked = trigger.RestrictToCategoryZone;
                    indexRestricted = 1;
                    parent.Nodes[indexRestricted].ImageIndex = parent.Nodes[indexRestricted].SelectedImageIndex = triggerBlankImage;

                    //start timer child
                    TreeNode timerNode = parent.Nodes.Add(timerLable);
                    timerNode.Checked = trigger.Timer;
                    if (!string.IsNullOrEmpty(trigger.TimerName))
                        timerNode.ToolTipText = "Double-click to search for the timer\n(Use [Clear] btn in timer window to reset search)";
                    indexTimer = 2;
                    parent.Nodes[indexTimer].ImageIndex = parent.Nodes[indexTimer].SelectedImageIndex = triggerBlankImage;

                    //add tab child
                    parent.Nodes.Add(tabLabel).Checked = trigger.Tabbed;
                    indexResultsTab = 3;
                    parent.Nodes[indexResultsTab].ImageIndex = parent.Nodes[indexResultsTab].SelectedImageIndex = triggerBlankImage;

                    //timer name child
                    parent.Nodes.Add(timerNameLabel + trigger.TimerName).Checked = trigger.Timer || trigger.Tabbed;
                    indexTimerName = 4;
                    if(CanMacroTimer(trigger))
                        parent.Nodes[indexTimerName].ImageIndex = parent.Nodes[indexTimerName].SelectedImageIndex = triggerCanMacro;
                    else
                        parent.Nodes[indexTimerName].ImageIndex = parent.Nodes[indexTimerName].SelectedImageIndex = triggerBlankImage;

                    //set colors
                    if (trigger.Active && (trigger.RestrictToCategoryZone == false || category.Equals(zoneName) || category.Equals(decoratedZoneName)))
                    {
                        parent.ForeColor = activeColor;
                        foreach (TreeNode child in parent.Nodes)
                            child.ForeColor = activeColor;
                    }
                    else
                    {
                        Color setColor = disabledColor;
                        if (trigger.Active)
                            setColor = inactiveColor;
                        parent.ForeColor = setColor;
                        foreach (TreeNode child in parent.Nodes)
                            child.ForeColor = setColor;
                    }
                }
            }
        }

        private bool CanMacroTimer(CustomTrigger trigger)
        {
            List<TimerData> timers = FindTimers(trigger);
            //look for at least one macro-able timer
            foreach (TimerData timer in timers)
            {
                if (!Macros.IsInvalidMacroTimer(timer))
                {
                    return true;
                }
            }
            return false;
        }

        private void TriggerTree_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //sync with outside changes if our tab becomes visible
                PopulateCatsTree(null);

                if (string.IsNullOrEmpty(zoneName))
                {
                    //we've never seen a zone change
                    // let's try to use wherever ACT thinks we are
                    Match match = reCleanActZone.Match(ActGlobals.oFormActMain.CurrentZone);
                    if (match.Success)
                    {
                        zoneMatchedEQII = true;
                        decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                        string cleanZoneName = match.Groups["zone"].Value.Trim();
                        if (config.autoCats.Contains(cleanZoneName))
                            zoneName = cleanZoneName;
                        else
                            zoneName = decoratedZoneName;
                        CheckAutoRestrict(null);
                    }
                    else
                        zoneName = decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                    if (!string.IsNullOrEmpty(zoneName))
                    {
                        TreeNode[] nodes = treeViewCats.Nodes.Find(zoneName, false);
                        if (nodes.Length > 0)
                        {
                            treeViewCats.SelectedNode = nodes[0];
                            treeViewCats.SelectedNode.EnsureVisible();
                        }
                    }
                }
                UpdateTriggerColors(null);

                if (neverBeenVisible)
                {
                    //set the splitter only on the first time shown
                    if (config.SettingsSerializer.Int32.Value > 0)
                        splitContainer1.SplitterDistance = config.SettingsSerializer.Int32.Value;
                    neverBeenVisible = false;
                }
            }
        }

        private void treeViewTrigs_AfterCheck(object sender, TreeViewEventArgs e)
        {
            bool updateACT = false;
            CustomTrigger trigger = null;
            
            if (e.Action != TreeViewAction.ByMouse)
                return; // ignore calls that are not user initiated

            if (e.Node.Tag != null)
            {
                //this is the regex node
                // checkbox enables or disables the trigger
                trigger = e.Node.Tag as CustomTrigger;
                if (trigger != null)
                {
                    trigger.Active = e.Node.Checked;
                    UpdateTriggerColors(null);
                    UpdateCategoryColors(false);
                    updateACT = true;
                }
            }
            else
            {
                //child node
                if(e.Node.Parent != null)
                {
                    trigger = e.Node.Parent.Tag as CustomTrigger;
                    if(trigger != null)
                    {
                        if(e.Node.Index == indexRestricted)
                        {
                            trigger.RestrictToCategoryZone = e.Node.Checked;
                            UpdateTriggerColors(null);
                            UpdateCategoryColors(false);
                            updateACT = true;
                        }
                        else if(e.Node.Index == indexTimer)
                        {
                            trigger.Timer = e.Node.Checked;
                            if(e.Node.Parent.Nodes.Count > indexTimerName)
                                e.Node.Parent.Nodes[indexTimerName].Checked = trigger.Timer || trigger.Tabbed;
                            updateACT = true;
                        }
                        else if(e.Node.Index == indexResultsTab)
                        {
                            trigger.Tabbed = e.Node.Checked;
                            if (e.Node.Parent.Nodes.Count > indexTimerName)
                                e.Node.Parent.Nodes[indexTimerName].Checked = trigger.Timer || trigger.Tabbed;
                            updateACT = true;
                        }
                    }
                }
            }

            if (updateACT && trigger != null)
            {
                //update the Custom Triggers tab, the trigger itself is already changed
                ActGlobals.oFormActMain.AddEditCustomTrigger(trigger);
                if (trigger.Tabbed)
                    UpdateResultsTab(trigger);
            }

        }

        private void UpdateResultsTab(CustomTrigger trigger)
        {
            if (trigger.ResultsTab.Text != trigger.TimerName && !string.IsNullOrEmpty(trigger.TimerName))
                trigger.ResultsTab.Text = trigger.TimerName; // make both names match

            formResultsTabs.AddTab(trigger.ResultsTab);
            if (trigger.ResultsTab != null)
            {
                // if we programatically toggle a tab off, then back on,
                // ACT does not re-add the tab to the tabcontrol
                // so we need to do it
                // (if the tab is removed then added using ACT's Custom Triggers page, it does re-add the tab)
                if (resultsTabCtrl != null && trigger.ResultsTab.Parent == null)
                {
                    resultsTabCtrl.TabPages.Add(trigger.ResultsTab);
                }
            }
        }

        private void PositionChildForm(Form form, Point loc)
        {
            //make sure it fits on screen
            if (loc.X + form.Width > SystemInformation.VirtualScreen.Right)
                loc.X = SystemInformation.VirtualScreen.Right - form.Width;
            if (loc.Y + form.Height > SystemInformation.WorkingArea.Bottom)
                loc.Y = SystemInformation.WorkingArea.Bottom - form.Height;
            form.Location = loc;
        }

        private void Sound_EditDoneEvent(object sender, EventArgs e)
        {
            //edit sound dialog callback
            //only gets called when [OK] is pressed in the dialog
            // indicating that the trigger was updated
            FormEditSound.EditEventArgs arg = e as FormEditSound.EditEventArgs;
            if(arg != null)
            {
                //update the display
                RefreshTriggerChildren(arg.editedTrigger);
            }
        }

        private void Timer_EditDoneEvent(object sender, EventArgs e)
        {
            //edit timer name dialog callback
            //only gets called when [OK] is pressed in the dialog
            // indicating that the trigger was updated
            FormEditTimer.EditEventArgs arg = e as FormEditTimer.EditEventArgs;
            if (arg != null)
            {
                //update the display
                RefreshTriggerChildren(arg.editedTrigger);
                if (arg.editedTrigger.Tabbed)
                {
                    // if the name was changed, need to re-synchronize
                    UpdateResultsTab(arg.editedTrigger);
                }
            }
        }

        private void Trigger_EditDoneEvent(object sender, EventArgs e)
        {
            //edit trigger dialog callback

            FormEditTrigger.EditEventArgs args = e as FormEditTrigger.EditEventArgs;
            lastEditLoc = args.formLocation;
            lastEditSize = args.formSize;
            if (args.result != FormEditTrigger.EventResult.CANCEL_EDIT)
            {
                bool ok = true;
                //save the original category in case we delete the original trigger
                string origCategory = args.orignalTrigger.Category;

                if (args.result == FormEditTrigger.EventResult.REPLACE_TRIGGER)
                {
                    ok = DeleteTrigger(args.orignalTrigger, true);
                }

                //new / edited trigger
                // If the regex or category was changed, this is required to update the dictionary
                ActGlobals.oFormActMain.AddEditCustomTrigger(args.editedTrigger);
                if (args.editedTrigger.Tabbed)
                    UpdateResultsTab(args.editedTrigger);

                PopulateCatsTree(null);
                if (args.result == FormEditTrigger.EventResult.CREATE_NEW)
                {
                    if (args.editedTrigger.Category != origCategory)
                    {
                        //change the selected node to the new category
                        TreeNode[] nodes = treeViewCats.Nodes.Find(args.editedTrigger.Category, false);
                        if (nodes.Length > 0)
                            treeViewCats.SelectedNode = nodes[0];
                    }
                }

                UpdateTriggerList(args.editedTrigger.Category);

                if (!ok)
                {
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, "Was not able to delete original trigger");
                }
            }
        }

        private void treeViewTrigs_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                //do not allow the alert type to be unchecked
                // i.e. can't change from currently checked to unchecked
                if (e.Node.Index == indexAlertType
                    && e.Node.Checked == true)
                {
                    e.Cancel = true;
                }
                else if (e.Node.Index == indexTimerName)
                {
                    //try to make it so the user cannot change the timer name checkbox
                    // since it doesn't really mean anything
                    // but this only mostly works due to bugs in treeview double-clicking
                    if (e.Node.Parent.Nodes[indexTimer].Checked
                        || e.Node.Parent.Nodes[indexResultsTab].Checked)
                    {
                        if (e.Node.Checked)
                            e.Cancel = true;
                    }
                    else if (!e.Node.Checked)
                        e.Cancel = true;
                }
            }
        }

        private void treeViewTrigs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if(treeViewTrigs.SelectedNode != null)
                {
                    CustomTrigger trigger;
                    if (treeViewTrigs.SelectedNode.Parent != null)
                        trigger = treeViewTrigs.SelectedNode.Parent.Tag as CustomTrigger;
                    else
                        trigger = treeViewTrigs.SelectedNode.Tag as CustomTrigger;
                    DeleteTrigger(trigger, false);
                }
            }
        }

        private FindResult FindTrigger(CustomTrigger trigger, string find)
        {
            FindResult found = FindResult.NOT_FOUND;
            //This is a trigger from enumerating the ACT dictionary.
            //If it contains the string we are searching for,
            // look for and select it in our tree structure
            if (trigger.ShortRegexString.ToLower().Contains(find)
                || trigger.TimerName.ToLower().Contains(find)
                || trigger.SoundData.ToLower().Contains(find))
            {
                //get our category set
                string category = trigger.Category;
                TreeNode[] cats = treeViewCats.Nodes.Find(category, false);
                if (cats.Length > 0)
                {
                    treeViewCats.SelectedNode = cats[0];
                    treeViewCats.SelectedNode.EnsureVisible();
                    //now find the trigger in that category
                    foreach (TreeNode trigNode in treeViewTrigs.Nodes)
                    {
                        if (trigNode.Text.Equals(trigger.ShortRegexString))
                        {
                            treeViewTrigs.SelectedNode = trigNode;
                            treeViewTrigs.SelectedNode.EnsureVisible();
                            treeViewTrigs.SelectedNode.ExpandAll();
                            buttonFindNext.Enabled = true;
                            found = FindResult.FOUND;
                            break;
                        }
                    }
                    if(found == FindResult.NOT_FOUND)
                        found = FindResult.FIND_FAILED; //should not get here, but avoid an exception in case we do
                }
            }
            return found;
        }

        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxTrigFind.Text))
            {
                buttonFindNext.Enabled = true;
                FindNextTrigger(false);
            }
            else
            {
                buttonFindNext.Enabled = false;
                keyLastFound = string.Empty;
                UpdateTriggerColors(null);
            }
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            FindNextTrigger(true);
        }

        private void FindNextTrigger(bool resume)
        {
            //when resuming, since the dictionary might have changed since the first find,
            // just start searching from the beginning until we get the previously found key
            string find = textBoxTrigFind.Text.ToLower();
            if (!string.IsNullOrEmpty(find))
            {
                bool foundPrevious = false;
                bool foundCategory = false;
                FindResult result = FindResult.NOT_FOUND;
                try
                {
                    int keyCount = ActGlobals.oFormActMain.CustomTriggers.Keys.Count;
                    string category = treeViewCats.SelectedNode.Text + "|";
                    for (int i=0; i<keyCount; i++)
                    {
                        string key = ActGlobals.oFormActMain.CustomTriggers.Keys[i];
                        if (checkBoxCurrentCategory.Checked && foundCategory == false)
                        {
                            //first, find the selected category
                            if (key.StartsWith(category))
                                foundCategory = true;
                            else
                                continue;
                        }
                        if (resume && !foundPrevious)
                        {
                            //move down the list to the previous find
                            if (key.Equals(keyLastFound))
                                foundPrevious = true;
                        }
                        else
                        {
                            //can now Find Next
                            CustomTrigger trigger;
                            if (ActGlobals.oFormActMain.CustomTriggers.TryGetValue(key, out trigger))
                            {
                                result = FindTrigger(trigger, find);
                                if (result == FindResult.FOUND)
                                {
                                    keyLastFound = key;
                                    if (checkBoxCurrentCategory.Checked && !key.StartsWith(category))
                                        result = FindResult.NOT_FOUND; //finished the category
                                    break;
                                }
                                else if (result == FindResult.NOT_FOUND && checkBoxCurrentCategory.Checked && !key.StartsWith(category))
                                    break;
                                else if (result == FindResult.FIND_FAILED)
                                    break;
                            }
                        }
                    }
                }
                catch { } //just in case there's a problem accessing ACT's dictionary, just quit this try

                UpdateTriggerColors(null);
                if (result == FindResult.NOT_FOUND)
                    SimpleMessageBox.ShowDialog(ActGlobals.oFormActMain, String.Format(@"\b {0}\b0\line  not found", find), "Not Found");
            }
        }

        private void checkBoxCurrentCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCurrentCategory.Checked)
                textBoxTrigFind.Text = string.Empty;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if we are "finding", use the <Enter> key to proceed
            if (keyData == Keys.Enter)
            {
                if (textBoxCatFind.Focused)
                {
                    buttonCatFindNext.PerformClick();
                    return true;
                }
                if (textBoxTrigFind.Focused)
                {
                    buttonFindNext.PerformClick();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void RefreshTriggerChildren(CustomTrigger trigger)
        {
            foreach(TreeNode node in treeViewTrigs.Nodes)
            {
                if(node.Text.Equals(trigger.ShortRegexString) && trigger.Category.Equals(treeViewCats.SelectedNode.Text))
                {
                    //set the sound type child
                    string soundType = alertLabel + trigger.SoundTypeString;
                    if (trigger.SoundType == (int)CustomTriggerSoundTypeEnum.TTS || trigger.SoundType == (int)CustomTriggerSoundTypeEnum.WAV)
                        soundType = soundType + "=" + trigger.SoundData;
                    node.Nodes[indexAlertType].Text = soundType;

                    //set the timer name child
                    node.Nodes[indexTimerName].Text = timerNameLabel + trigger.TimerName;

                    //set timer search tooltip
                    if (string.IsNullOrEmpty(trigger.TimerName))
                        node.Nodes[indexTimer].ToolTipText = string.Empty;
                    else
                        node.Nodes[indexTimer].ToolTipText = "Double-click to search for the timer\n(Use [Clear] btn in timer window to reset search)";

                    //set macro icons
                    if (Macros.IsInvalidMacroTrigger(trigger))
                    {
                        node.ImageIndex = triggerNoMacro;
                        node.SelectedImageIndex = triggerNoMacro;
                    }
                    else
                    {
                        node.ImageIndex = triggerCanMacro;
                        node.SelectedImageIndex = triggerCanMacro;
                    }

                    if (CanMacroTimer(trigger))
                        node.Nodes[indexTimerName].ImageIndex = node.Nodes[indexTimerName].SelectedImageIndex = triggerCanMacro;
                    else
                        node.Nodes[indexTimerName].ImageIndex = node.Nodes[indexTimerName].SelectedImageIndex = triggerBlankImage;

                    //update the Custom Triggers tab, the trigger itself is already changed
                    ActGlobals.oFormActMain.AddEditCustomTrigger(trigger);

                    break;
                }
            }
        }

        private void treeViewTrigs_MouseDown(object sender, MouseEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            selectedTriggerNode = treeViewTrigs.GetNodeAt(pt);
            whereTrigMouseDown = treeViewTrigs.PointToScreen(pt);
            if (e.Button == MouseButtons.Right)
            {
                isDoubleClick = false; //used to prevent expand/collapse on double click

                if (selectedTriggerNode != null)
                {
                    treeViewTrigs.SelectedNode = selectedTriggerNode;
                    contextMenuStripTrig.Show(whereTrigMouseDown);
                }
                else
                {
                    //clicked an empty line
                    //start a brand new trigger
                    FormEditTrigger formEditTrigger = NewTrigger();
                    if (lastEditLoc.IsEmpty && lastEditSize.IsEmpty)
                        PositionChildForm(formEditTrigger, whereTrigMouseDown);
                    else
                    {
                        formEditTrigger.Size = lastEditSize;
                        formEditTrigger.Location = lastEditLoc;
                    }
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                isDoubleClick = e.Clicks > 1; //used to prevent expand/collapse on double click
            }
        }

        private FormEditTrigger NewTrigger()
        {
            string category = " General";
            if (treeViewCats.SelectedNode != null)
                category = treeViewCats.SelectedNode.Text;
            CustomTrigger trigger = new CustomTrigger("new expression", category);
            //set restrict if it kinda looks like a zone name
            trigger.RestrictToCategoryZone = category.Contains("[") && decoratedZoneName == zoneName;
            FormEditTrigger formEditTrigger = new FormEditTrigger(trigger, zoneName);
            formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
            formEditTrigger.haveOriginal = false; //disable the replace button since there is nothing to replace
            formEditTrigger.Show(this);
            return formEditTrigger;
        }

        private void treeViewTrigs_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            //we are using double click to edit the trigger
            //rather than expanding / collapsing the tree
            if (isDoubleClick && e.Action == TreeViewAction.Collapse)
                e.Cancel = true;
        }

        private void treeViewTrigs_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //we are using double click to edit the trigger
            //rather than expanding / collapsing the tree
            if (isDoubleClick && e.Action == TreeViewAction.Expand)
                e.Cancel = true;
        }

        private void treeViewTrigs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedTriggerNode != null)
            {
                if (selectedTriggerNode.Parent == null)
                {
                    FormEditTrigger formEditTrigger = new FormEditTrigger(selectedTriggerNode.Tag as CustomTrigger, zoneName);
                    formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
                    formEditTrigger.Show(this);
                    if(lastEditLoc.IsEmpty && lastEditSize.IsEmpty)
                        PositionChildForm(formEditTrigger, whereTrigMouseDown);
                    else
                    {
                        formEditTrigger.Size = lastEditSize;
                        formEditTrigger.Location = lastEditLoc;
                    }
                }
                else
                {
                    if (selectedTriggerNode.Index == indexAlertType)
                    {
                        FormEditSound formEditSound = new FormEditSound(selectedTriggerNode.Parent.Tag as CustomTrigger, Sound_EditDoneEvent);
                        formEditSound.Show(this);
                        PositionChildForm(formEditSound, whereTrigMouseDown);
                    }
                    else if (selectedTriggerNode.Index == indexTimerName)
                    {
                        FormEditTimer formEditTimer = new FormEditTimer(selectedTriggerNode.Parent.Tag as CustomTrigger, Timer_EditDoneEvent);
                        formEditTimer.Show(this);
                        PositionChildForm(formEditTimer, whereTrigMouseDown);
                    }
                    else if (selectedTriggerNode.Index == indexTimer)
                    {
                        // search for the timer if there is a timer name
                        CustomTrigger trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                        if (trigger != null)
                        {
                            string timerName = trigger.TimerName.ToLower();
                            if (!string.IsNullOrEmpty(timerName))
                            {
                                ActGlobals.oFormSpellTimers.SearchSpellTreeView(timerName);
                                ActGlobals.oFormSpellTimers.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            FormEditTrigger formEditTrigger = NewTrigger();
            Point center = new Point(treeViewTrigs.Right/2 - formEditTrigger.Width/2, treeViewTrigs.Bottom/2 - formEditTrigger.Height/2);
            PositionChildForm(formEditTrigger, treeViewTrigs.PointToScreen(center));
        }

        private void toolStripButtonResults_Click(object sender, EventArgs e)
        {
            config.ResultsPopup = toolStripButtonResults.Checked;
        }

        #region --------------- Triggers Context Menu

        private void contextMenuStripTrg_Opening(object sender, CancelEventArgs e)
        {
            if (selectedTriggerNode != null)
            {
                CustomTrigger trigger;
                bool isParent = true; //default
                if (selectedTriggerNode.Tag != null)
                    trigger = selectedTriggerNode.Tag as CustomTrigger;
                else
                {
                    isParent = false;
                    trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                }
                if (trigger != null)
                {
                    //only enable XML copy for the trigger or the timer
                    if (isParent
                        || (!isParent && selectedTriggerNode.Index == indexTimerName && !string.IsNullOrEmpty(trigger.TimerName)))
                    {
                        copyAsShareableXMLToolStripMenuItem.Enabled = true;
                        copyAsDoubleEncodedXMLToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        copyAsShareableXMLToolStripMenuItem.Enabled = false;
                        copyAsDoubleEncodedXMLToolStripMenuItem.Enabled = false;
                    }

                    //check for macro-able trigger
                    if (isParent)
                    {
                        if (Macros.IsInvalidMacroTrigger(trigger))
                        {
                            raidsayShareMacroToolStripMenuItem.Enabled = false;
                            raidsayShareMacroToolStripMenuItem.ToolTipText =
                                string.Format(invalidTriggerText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                            groupsayShareMacroToolStripMenuItem.Enabled = false;
                            groupsayShareMacroToolStripMenuItem.ToolTipText =
                                string.Format(invalidTriggerText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                        }
                        else
                        {
                            raidsayShareMacroToolStripMenuItem.Enabled = true;
                            raidsayShareMacroToolStripMenuItem.ToolTipText = validTriggerText + "raid";
                            groupsayShareMacroToolStripMenuItem.Enabled = true;
                            groupsayShareMacroToolStripMenuItem.ToolTipText = validTriggerText + "group";
                        }
                    }
                    else if (selectedTriggerNode.Index == indexTimerName && !string.IsNullOrEmpty(trigger.TimerName))
                    {
                        //can macro the timer?
                        List<TimerData> timers = FindTimers(trigger);
                        if (timers.Count > 0)
                        {
                            if (Macros.IsInvalidMacroTimer(timers[0]))
                            {
                                raidsayShareMacroToolStripMenuItem.Enabled = false;
                                raidsayShareMacroToolStripMenuItem.ToolTipText =
                                    string.Format(invalidTimerText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                                groupsayShareMacroToolStripMenuItem.Enabled = false;
                                groupsayShareMacroToolStripMenuItem.ToolTipText =
                                    string.Format(invalidTimerText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                            }
                            else
                            {
                                raidsayShareMacroToolStripMenuItem.Enabled = true;
                                raidsayShareMacroToolStripMenuItem.ToolTipText = validTimerText + "raid";
                                groupsayShareMacroToolStripMenuItem.Enabled = true;
                                groupsayShareMacroToolStripMenuItem.ToolTipText = validTimerText + "group";
                            }
                        }
                    }
                    else
                    {
                        //not a macro-able tree node
                        raidsayShareMacroToolStripMenuItem.Enabled = false;
                        raidsayShareMacroToolStripMenuItem.ToolTipText = validTriggerText + "raid";
                        groupsayShareMacroToolStripMenuItem.Enabled = false;
                        groupsayShareMacroToolStripMenuItem.ToolTipText = validTriggerText + "group";
                    }

                    playAlertSoundToolStripMenuItem.Enabled = trigger.SoundType != (int)CustomTriggerSoundTypeEnum.None;

                    //set the Edit menu item depending on which item was clicked
                    if (isParent)
                    {
                        editTriggerToolStripMenuItem.Text = "Edit Trigger";
                        editTriggerToolStripMenuItem.Enabled = true;
                    }
                    else if (selectedTriggerNode.Index == indexTimer)
                    {
                        editTriggerToolStripMenuItem.Text = "Find Spell Timer";
                        if (string.IsNullOrEmpty(trigger.TimerName))
                            editTriggerToolStripMenuItem.Enabled = false;
                        else
                            editTriggerToolStripMenuItem.Enabled = true;
                    }
                    else if (selectedTriggerNode.Index == indexAlertType)
                    {
                        editTriggerToolStripMenuItem.Text = "Edit Alert Sound";
                        editTriggerToolStripMenuItem.Enabled = true;
                    }
                    else if (selectedTriggerNode.Index == indexTimerName)
                    {
                        editTriggerToolStripMenuItem.Text = "Edit Timer Name";
                        editTriggerToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        //any other child
                        editTriggerToolStripMenuItem.Text = "Edit Trigger";
                        editTriggerToolStripMenuItem.Enabled = true;
                    }
                }
            }
        }

        private void deleteTriggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTriggerNode != null)
            {
                CustomTrigger trigger;
                if (selectedTriggerNode.Tag != null)
                    trigger = selectedTriggerNode.Tag as CustomTrigger;
                else
                    trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                if (trigger != null)
                {
                    DeleteTrigger(trigger, false);
                }
            }
        }

        private void editTriggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTriggerNode != null)
            {
                int wholeEdit = 0;
                if (selectedTriggerNode.Parent != null)
                {
                    //it's a child item
                    if (selectedTriggerNode.Index == indexAlertType)
                    {
                        FormEditSound formEditSound = new FormEditSound(selectedTriggerNode.Parent.Tag as CustomTrigger, Sound_EditDoneEvent);
                        formEditSound.Show(this);
                        PositionChildForm(formEditSound, whereTrigMouseDown);
                    }
                    else if (selectedTriggerNode.Index == indexTimerName)
                    {
                        FormEditTimer formEditTimer = new FormEditTimer(selectedTriggerNode.Parent.Tag as CustomTrigger, Timer_EditDoneEvent);
                        formEditTimer.Show(this);
                        PositionChildForm(formEditTimer, whereTrigMouseDown);
                    }
                    else if (selectedTriggerNode.Index == indexTimer)
                    {
                        // search for the timer if there is a timer name
                        CustomTrigger trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                        if (trigger != null)
                        {
                            string timerName = trigger.TimerName.ToLower();
                            if (!string.IsNullOrEmpty(timerName))
                            {
                                ActGlobals.oFormSpellTimers.SearchSpellTreeView(timerName);
                                ActGlobals.oFormSpellTimers.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        //edit the whole trigger for any other child
                        wholeEdit = 2;
                    }
                }
                else
                {
                    //clicked a a parent node
                    wholeEdit = 1;
                }
                if (wholeEdit > 0)
                {
                    //bring up the trigger edit form
                    CustomTrigger t;
                    if (wholeEdit == 1)
                        t = selectedTriggerNode.Tag as CustomTrigger;
                    else
                        t = selectedTriggerNode.Parent.Tag as CustomTrigger;
                    FormEditTrigger formEditTrigger = new FormEditTrigger(t, zoneName);
                    formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
                    formEditTrigger.Show(this);
                    if (lastEditLoc.IsEmpty && lastEditSize.IsEmpty)
                        PositionChildForm(formEditTrigger, whereTrigMouseDown);
                    else
                    {
                        formEditTrigger.Size = lastEditSize;
                        formEditTrigger.Location = lastEditLoc;
                    }
                }
            }
        }

        private void raidsayShareMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteTriggerMacroFile("r ");
        }

        private void groupsayShareMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteTriggerMacroFile("g ");
        }

        private void WriteTriggerMacroFile(string sayCmd)
        {
            if (selectedTriggerNode != null)
            {
                CustomTrigger trigger;
                if (selectedTriggerNode.Tag != null)
                    trigger = selectedTriggerNode.Tag as CustomTrigger;
                else
                    trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                if (trigger != null)
                {
                    if (Macros.IsInvalidMacroTrigger(trigger))
                    {
                        //should not get here since the menu should be disabled
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, @"EQII does not allow certain characters in a macro.\line This trigger cannot be saved to a macro.",
                            "Unsupported Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            {
                                sb.Append(sayCmd);
                                sb.Append(Macros.TriggerToMacro(trigger));
                                sb.Append(Environment.NewLine);
                            }
                            //  if we can find the timer and it's enabled, add it to the macro file
                            List<TimerData> timers = FindTimers(trigger);
                            int timersCount = 0;
                            foreach (TimerData timer in timers)
                            {
                                if (!Macros.IsInvalidMacroTimer(timer))
                                {
                                    sb.Append(sayCmd);
                                    sb.Append(Macros.SpellTimerToMacro(timer));
                                    sb.Append(Environment.NewLine);
                                    timersCount++;
                                }
                            }
                            if (ActGlobals.oFormActMain.SendToMacroFile(doFileName, sb.ToString(), string.Empty))
                            {
                                string m1 = string.Format("Wrote trigger:\n{0}", trigger.ShortRegexString);
                                string m2 = timersCount > 0 ? string.Format("\n\nand {0} spell timer(s): '{1}'\n", timersCount, trigger.TimerName) : string.Empty;
                                string m3 = string.Format("to macro file {0}\n\nIn EQII chat enter:\n/do_file_commands {1}", doFileName, doFileName);
                                TraySlider traySlider = new TraySlider();
                                traySlider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                                traySlider.ShowTraySlider(m1 + m2 + m3, "Trigger Macro");
                            }
                        }
                        catch (Exception x)
                        {
                            SimpleMessageBox.Show(ActGlobals.oFormActMain, x.Message, "Macro file error");
                        }
                    }
                }
            }
        }

        public static List<TimerData> FindTimers(CustomTrigger trigger)
        {
            List<TimerData> result = new List<TimerData>();
            if (!string.IsNullOrEmpty(trigger.TimerName) && trigger.Timer)
            {
                string compareName = trigger.TimerName.ToLower();

                // we can't depend on the dictionay key since we have no idea what the timer's category might be
                // but we can try using the trigger's category just in case that works
                string key = string.Format("{0}|{1}", trigger.Category.ToLower(), compareName);
                TimerData timerData = null;
                bool foundKey = false;
                if (ActGlobals.oFormSpellTimers.TimerDefs.TryGetValue(key, out timerData))
                {
                    if (timerData.ActiveInList)
                    {
                        result.Add(timerData);
                        foundKey = true;
                    }
                }
                if(!foundKey)
                {
                    // just search through everything for all matching names
                    foreach (TimerData timer in ActGlobals.oFormSpellTimers.TimerDefs.Values)
                    {
                        if (timer.ActiveInList && timer.Name.ToLower().Equals(compareName))
                            result.Add(timer);
                    }
                }
            }
            return result;
        }

        private bool DeleteTrigger(CustomTrigger trigger, bool silently)
        {
            bool result = false;
            if (trigger != null)
            {
                string category = trigger.Category;
                bool doit;
                if (silently)
                    doit = true;
                else
                    doit = SimpleMessageBox.Show(ActGlobals.oFormActMain, @"\ql " + trigger.ShortRegexString, "Delete Trigger?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                if (doit)
                {
                    string key = category + "|" + trigger.ShortRegexString;
                    CustomTrigger ct;
                    ActGlobals.oFormActMain.CustomTriggers.TryGetValue(key, out ct);
                    if (ct != null)
                    {
                        if (ct.Tabbed)
                            resultsTabCtrl.TabPages.Remove(ct.ResultsTab);
                        ActGlobals.oFormActMain.CustomTriggers.Remove(key);
                        ActGlobals.oFormActMain.RebuildActiveCustomTriggers();
                        PopulateCatsTree(null);
                        UpdateTriggerList(category);
                        result = true;
                    }
                }
            }
            return result;
        }

        private void copyAsShareableXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyAsShareableXML();
        }

        private bool CopyAsShareableXML()
        {
            bool result = false;
            CustomTrigger trigger = null;
            TimerData timer = null;
            bool isTimer = false;
            //can only copy the trigger or the spell timer
            if (selectedTriggerNode.Tag != null)
                trigger = selectedTriggerNode.Tag as CustomTrigger;
            else
            {
                //clicked the spell timer?
                trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                if (selectedTriggerNode.Index == indexTimerName)
                {
                    //look for the timer name
                    List<TimerData> timers = FindTimers(trigger);
                    if(timers.Count > 0)
                    {
                        timer = timers[0]; //can only copy one
                        isTimer = true;
                    }
                    else
                    {
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, @"Could not find timer:\line\line " + trigger.TimerName, "No such timer");
                        return false;
                    }
                }
            }
            if (trigger != null)
            {
                try
                {
                    if (isTimer)
                        Clipboard.SetText(Macros.SpellTimerToXML(timer));
                    else
                        Clipboard.SetText(Macros.TriggerToXML(trigger));
                    result = true;
                }
                catch
                {
                    TraySlider traySlider = new TraySlider();
                    traySlider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                    traySlider.ShowTraySlider("Copy to trigger to clipboard failed", "Clipboard Failed");
                }
            }
            return result;
        }

        private void copyAsDoubleEncodedXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomTrigger trigger;
            string doubled = string.Empty;
            if (selectedTriggerNode.Tag != null)
            {
                trigger = selectedTriggerNode.Tag as CustomTrigger;
                string encoded = Macros.TriggerToXML(trigger);
                doubled = Macros.EncodeXml_ish(encoded, false, false, false);
            }
            else if (selectedTriggerNode.Index == indexTimerName)
            {
                trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                //look for the timer
                List<TimerData> timers = FindTimers(trigger);
                if (timers.Count > 0)
                {
                    //can only copy one, just use the first one
                    string encoded = Macros.SpellTimerToXML(timers[0]);
                    doubled = Macros.EncodeXml_ish(encoded, false, false, false);
                }
                else
                {
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, @"Could not find timer:\line\line " + trigger.TimerName, "No such timer");
                }
            }
            if (!string.IsNullOrEmpty(doubled))
            {
                try
                {
                    Clipboard.SetText(doubled);
                }
                catch
                {
                    TraySlider traySlider = new TraySlider();
                    traySlider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                    traySlider.ShowTraySlider("Copy to clipboard failed", "Clipboard Failed");
                }
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewTrigs.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewTrigs.CollapseAll();
        }

        private void playAlertSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomTrigger trigger;
            if (selectedTriggerNode.Tag != null)
                trigger = selectedTriggerNode.Tag as CustomTrigger;
            else
                trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
            if (trigger != null)
            {
                if (trigger.SoundType == (int)CustomTriggerSoundTypeEnum.TTS)
                    ActGlobals.oFormActMain.TTS(trigger.SoundData);
                else if (trigger.SoundType == (int)CustomTriggerSoundTypeEnum.WAV)
                {
                    if (File.Exists(trigger.SoundData))
                        ActGlobals.oFormActMain.PlaySoundWinApi(trigger.SoundData, 100);
                }
                else if (trigger.SoundType == (int)CustomTriggerSoundTypeEnum.Beep)
                    System.Media.SystemSounds.Beep.Play();

            }
        }

        #endregion Context Menu

        #endregion Trigger Tree
    }

}
