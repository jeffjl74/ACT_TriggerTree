using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
// reference:System.Windows.Forms.DataVisualization.dll
// reference:System.Core.dll

[assembly: AssemblyTitle("Tree view of Custom Triggers")]
[assembly: AssemblyDescription("An alternate interface for managing Custom Triggers")]
[assembly: AssemblyCompany("Mineeme of Maj'Dul")]
[assembly: AssemblyVersion("1.2.0.0")]

namespace ACT_TriggerTree
{
    public class TriggerTree : UserControl, IActPluginV1
	{
        const int logTimeStampLength = 39;          //# of chars in the timestamp

        //trigger dictionary - list of triggers by Category name
        Dictionary<string, List<CustomTrigger>> treeDict;

        string zoneName = string.Empty;             //most recent zone name from the log file

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
        int indexAlertTab = 3;                      //child index for the Alert Tab child
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
        bool isDoubleClick = false;                 //to intercept the double click on a trigger
        Point lastEditLoc = new Point();
        Size lastEditSize = new Size();

        string keyLastFound = string.Empty;         //for Find Next trigger
        string catLastFound = string.Empty;         //for find next cat
        enum FindResult { NOT_FOUND, FOUND, FIND_FAILED};

        bool initialVisible = true;                 //save the splitter location only if it has been initialized 

        //trigger macro file stuff
        string doFileName = "triggers.txt";         //macro file name
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

        Label lblStatus;                            // The status label that appears in ACT's Plugin tab

        string settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\TriggerTree.config.xml");
        SettingsSerializer xmlSettings;
        private CheckBox checkBoxCurrentCategory;

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
            this.textBoxCatFind = new System.Windows.Forms.TextBox();
            this.panelHelp = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.treeViewTrigs = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxCurrentCategory = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.textBoxTrigFind = new System.Windows.Forms.TextBox();
            this.textBoxSplitterLoc = new System.Windows.Forms.TextBox();
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
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripCat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyZoneNameToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEntireCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.raidShareCategoryMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupShareCategoryMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.categorySpellTimersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelHelp.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.panelHelp);
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
            this.treeViewCats.Location = new System.Drawing.Point(0, 30);
            this.treeViewCats.Name = "treeViewCats";
            this.treeViewCats.Size = new System.Drawing.Size(240, 530);
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
            this.panel3.Size = new System.Drawing.Size(240, 30);
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
            this.textBoxCatFind.Location = new System.Drawing.Point(40, 4);
            this.textBoxCatFind.Name = "textBoxCatFind";
            this.textBoxCatFind.Size = new System.Drawing.Size(148, 20);
            this.textBoxCatFind.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxCatFind, "Incremental search in the category name");
            this.textBoxCatFind.TextChanged += new System.EventHandler(this.textBoxCatScroll_TextChanged);
            // 
            // panelHelp
            // 
            this.panelHelp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelHelp.Controls.Add(this.webBrowser1);
            this.panelHelp.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelHelp.Location = new System.Drawing.Point(179, 30);
            this.panelHelp.Name = "panelHelp";
            this.panelHelp.Size = new System.Drawing.Size(304, 530);
            this.panelHelp.TabIndex = 2;
            this.panelHelp.Visible = false;
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(300, 526);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // treeViewTrigs
            // 
            this.treeViewTrigs.CheckBoxes = true;
            this.treeViewTrigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTrigs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewTrigs.Location = new System.Drawing.Point(0, 30);
            this.treeViewTrigs.Name = "treeViewTrigs";
            this.treeViewTrigs.ShowNodeToolTips = true;
            this.treeViewTrigs.Size = new System.Drawing.Size(483, 530);
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
            this.panel2.Controls.Add(this.checkBoxCurrentCategory);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.buttonFindNext);
            this.panel2.Controls.Add(this.textBoxTrigFind);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(483, 30);
            this.panel2.TabIndex = 0;
            // 
            // checkBoxCurrentCategory
            // 
            this.checkBoxCurrentCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxCurrentCategory.AutoSize = true;
            this.checkBoxCurrentCategory.Location = new System.Drawing.Point(372, 7);
            this.checkBoxCurrentCategory.Name = "checkBoxCurrentCategory";
            this.checkBoxCurrentCategory.Size = new System.Drawing.Size(59, 17);
            this.checkBoxCurrentCategory.TabIndex = 3;
            this.checkBoxCurrentCategory.Text = "current";
            this.toolTip1.SetToolTip(this.checkBoxCurrentCategory, "Search only the current category");
            this.checkBoxCurrentCategory.UseVisualStyleBackColor = true;
            this.checkBoxCurrentCategory.CheckedChanged += new System.EventHandler(this.checkBoxCurrentCategory_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 8);
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
            this.buttonFindNext.TabIndex = 1;
            this.buttonFindNext.Text = "8";
            this.toolTip1.SetToolTip(this.buttonFindNext, "Find the next matching trigger");
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // textBoxTrigFind
            // 
            this.textBoxTrigFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTrigFind.Location = new System.Drawing.Point(40, 4);
            this.textBoxTrigFind.Name = "textBoxTrigFind";
            this.textBoxTrigFind.Size = new System.Drawing.Size(326, 20);
            this.textBoxTrigFind.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxTrigFind, "Incremental search for text in the trigger\'s regular expression, alert, or timer " +
        "name");
            this.textBoxTrigFind.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
            // 
            // textBoxSplitterLoc
            // 
            this.textBoxSplitterLoc.Location = new System.Drawing.Point(605, 6);
            this.textBoxSplitterLoc.Name = "textBoxSplitterLoc";
            this.textBoxSplitterLoc.Size = new System.Drawing.Size(44, 20);
            this.textBoxSplitterLoc.TabIndex = 2;
            this.textBoxSplitterLoc.TabStop = false;
            this.textBoxSplitterLoc.Text = "300";
            this.textBoxSplitterLoc.Visible = false;
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
            this.collapseAllToolStripMenuItem,
            this.toolStripSeparator6,
            this.helpToolStripMenuItem});
            this.contextMenuStripTrig.Name = "contextMenuStrip1";
            this.contextMenuStripTrig.Size = new System.Drawing.Size(290, 254);
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
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(286, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(289, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
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
            this.toolStripSeparator4,
            this.raidShareCategoryMacroMenuItem,
            this.groupShareCategoryMacroMenuItem,
            this.toolStripSeparator7,
            this.categorySpellTimersMenuItem});
            this.contextMenuStripCat.Name = "contextMenuStrip2";
            this.contextMenuStripCat.Size = new System.Drawing.Size(252, 126);
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
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxSplitterLoc);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(727, 38);
            this.panel1.TabIndex = 0;
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
            this.panelHelp.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private TextBox textBoxSplitterLoc;
        private ContextMenuStrip contextMenuStripCat;
        private ToolStripMenuItem copyZoneNameToClipboardToolStripMenuItem;
        private ToolStripMenuItem deleteEntireCategoryToolStripMenuItem;
        private Label label1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem expandAllToolStripMenuItem;
        private ToolStripMenuItem collapseAllToolStripMenuItem;
        private Panel panel1;
        private Button buttonFindNext;
        private TextBox textBoxTrigFind;
        private Panel panel2;
        private Panel panel3;
        private TextBox textBoxCatFind;
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
        private ToolStripSeparator toolStripSeparator6;
        private Panel panelHelp;
        private WebBrowser webBrowser1;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem categorySpellTimersMenuItem;

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
			this.Dock = DockStyle.Fill;	                // Expand the UserControl to fill the tab's client space
			xmlSettings = new SettingsSerializer(this); // Create a new settings serializer and pass it this instance

            LoadSettings();

            treeImages.Images.Add(GetFolderBitmap());
            treeImages.Images.Add(GetOpenFolderBitmap());
            treeViewCats.ImageList = treeImages;

            //set images so that triggerCanMacro and triggerNoMacro show the appropirate image
            triggerImages.Images.Add(Macros.GetActionBitmap());
            triggerBlankImage = triggerImages.Images.Count;
            treeViewTrigs.ImageList = triggerImages;

            PopulateCatsTree();

            SetHelp();

            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;        //for zone change
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
			// Unsubscribe from any events you listen to when exiting!
            ActGlobals.oFormActMain.OnLogLineRead -= OFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.XmlSnippetAdded -= OFormActMain_XmlSnippetAdded;

            SaveSettings();
			lblStatus.Text = "Plugin Exited";
		}

        #endregion IActPluginV1 Members

        void oFormActMain_UpdateCheckClicked()
        {
            int pluginId = 80;
            bool updateAvailable = false;

            // try github
            try
            {
                Version localVersion = this.GetType().Assembly.GetName().Version;
                Version remoteVersion = new Version(ActGlobals.oFormActMain.PluginGetRemoteVersion(pluginId).TrimStart(new char[] { 'v' }));    // Strip any leading 'v' from the string before passing to the Version constructor
                if (remoteVersion > localVersion)
                    updateAvailable = true;
            }
            catch (Exception)
            {
                //ignore failure while we transition to github
            }

            if (updateAvailable)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There is an updated version of the Trigger Tree Plugin.  Update it now?\n\n(If there is an update to ACT, you should click No and update ACT first.)", "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        FileInfo updatedFile = ActGlobals.oFormActMain.PluginDownload(pluginId);
                        ActPluginData pluginData = ActGlobals.oFormActMain.PluginGetSelfData(this);
                        pluginData.pluginFile.Delete();
                        updatedFile.MoveTo(pluginData.pluginFile.FullName);
                        ThreadInvokes.CheckboxSetChecked(ActGlobals.oFormActMain, pluginData.cbEnabled, false);
                        Application.DoEvents();
                        ThreadInvokes.CheckboxSetChecked(ActGlobals.oFormActMain, pluginData.cbEnabled, true);
                    }
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.WriteExceptionLog(ex, "Plugin Update Download");
                }
            }
        }

        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            //Go ahead and process imports so we switch to the current category.

            //lines that ACT parses have a non-zero logInfo.detectedType
            //we only need type == 0 non-combat lines for a zone change
            if (logInfo.detectedType == 0 && logInfo.logLine.Length > logTimeStampLength)
            {
                //look for zone change
                if (!string.IsNullOrEmpty(logInfo.detectedZone) && logInfo.detectedZone != zoneName)
                {
                    zoneName = logInfo.detectedZone;
                    UpdateCategoryColors(ActGlobals.oFormActMain, treeViewCats, true);
                    UpdateTriggerColors(ActGlobals.oFormActMain, treeViewTrigs);
                }
            }
        }

        private void OFormActMain_XmlSnippetAdded(object sender, XmlSnippetEventArgs e)
        {
            if (e.ShareType == "Trigger")
            {
                //we need to rebuild if there is a new trigger share incoming
                PopulateCatsTree();
            }
        }

        void LoadSettings()
		{
            xmlSettings.AddControlSetting(textBoxSplitterLoc.Name, textBoxSplitterLoc);

            if (File.Exists(settingsFile))
			{
				FileStream fs = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				XmlTextReader xReader = new XmlTextReader(fs);

				try
				{
					while (xReader.Read())
					{
						if (xReader.NodeType == XmlNodeType.Element)
						{
							if (xReader.LocalName == "SettingsSerializer")
							{
								xmlSettings.ImportFromXml(xReader);
							}
						}
					}
				}
				catch (Exception ex)
				{
					lblStatus.Text = "Error loading settings: " + ex.Message;
				}
				xReader.Close();
            }
        }

        void SaveSettings()
		{
            //using a hidden textbox to store the splitter location to take advantage of xmlsettings
            // but only save it if it was ever set
            if(!initialVisible)
                textBoxSplitterLoc.Text = splitContainer1.SplitterDistance.ToString();

            FileStream fs = new FileStream(settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			XmlTextWriter xWriter = new XmlTextWriter(fs, Encoding.UTF8);
			xWriter.Formatting = Formatting.Indented;
			xWriter.Indentation = 1;
			xWriter.IndentChar = '\t';
			xWriter.WriteStartDocument(true);
			xWriter.WriteStartElement("Config");	// <Config>
			xWriter.WriteStartElement("SettingsSerializer");	// <Config><SettingsSerializer>
			xmlSettings.ExportToXml(xWriter);	// Fill the SettingsSerializer XML
			xWriter.WriteEndElement();	// </SettingsSerializer>
			xWriter.WriteEndElement();	// </Config>
			xWriter.WriteEndDocument();	// Tie up loose ends (shouldn't be any)
			xWriter.Flush();	        // Flush the file buffer to disk
			xWriter.Close();
		}

        #region Help

        private void SetHelp()
        {
            webBrowser1.DocumentText = @"
                <html>
                <body>
                <button value=""Close"" name=""Close"" style=""float: right;"" >Close</button ><br>
                <u>Trigger Tree Plugin</u><br>
<br>
The plugin provides another interface to the Custom Triggers. Triggers
can be managed from either interface.<br>
<br>
The left panel lists each Category. The right panel lists all triggers
in the selected Category.<br>
<br>
Features that are similar to the Custom Trigger Tab include:<br>
<ul>
<li>Active triggers are green text.</li>
<li>Enabled but inactive triggers (e.g. you are not currently
in the restricted zone) are black text.</li>
<li>Disabled triggers (i.e. unchecked,&nbsp;they will never
alert)
are grey text.</li>
<li>The checkbox next to the trigger will enable / disable the
trigger.</li>
<li>Right-click a Category or trigger to get a context menu,
e.g. for sharing.</li>
</ul>
Additional features over the Custom Trigger tab include:<br>
<ul>
<li>Category Panel</li>
<ul>
<li>When entering a zone that has a matching Category, the
Category is automatically selected.</li>
</ul>
<ul>
<li>Perform an incremental search for a Category by entering
text in the <i>Find:</i> box. The [Enter] key or the
double arrow button
performs a 'find next'.</li>
</ul>
<ul>
<li>To support writing a new trigger after having left the
zone, the
context menu provides a 'Copy category name to clipboard'.</li>
<li>If the Cateogry contains any active triggers it has a
green background.</li>
</ul>
<ul>
<li>If the Category name and triggers can be written to an
EQII macro, the context menu allows saving all the triggers and spell
timers in that Category to a macro for sharing in EQII. &nbsp;See
the section on <a href='#macro_share'>macro limitations</a>.</li>
<li>If there are spell timers whose Category or Custom
Tooltip&nbsp;match the Category name, the context menu allows
opening ACT's <i>Spell
Timers (Options)</i> form for the selected timer. Spell timers
that
can be written to a macro file are indicated by the 'macro play' icon
in the context menu.&nbsp;If any spell timers can be written to an
EQII macro, the
context menu allows saving&nbsp;the&nbsp;spell timer(s) to
a macro for sharing in EQII.&nbsp;See
the section on <a href='#macro_share'>macro
limitations</a>.</li>
</ul>
<li>Trigger Panel</li>
<ul>
<li>Displays the triggers for the selected category.</li>
<li>Trigger details are sub-items of the Regular Expression.</li>
<li>If the Category name and trigger can be written to an
EQII macro, the
context menu allows saving&nbsp;the trigger and its spell timer to
a macro for sharing in EQII. &nbsp;Triggers and spell timers that
can be written to a macro file are indicated by the 'macro play' icon
next to their checkbox. See the section on <a href='#macro_share'>macro
limitations</a>.</li>
<li>Double-click the <i>Alert</i> or&nbsp;<i>Timer
or Tab Name</i> sub-items
to open an edit dialog for just those settings.</li>
<li>Double-click the&nbsp;<i>Trigger Timer</i>
to open ACT's <i>Spell
Timers (Options)</i> form and search for matching spell timer
name.</li>
<li>Right-click the&nbsp;<i>Timer or Tab Name</i>
to copy the timer to
the clipboard for sharing. Note that this will copy the first spell
timer found that matches that name if there are multiple timers with
the same name but different categories.</li>
<li>Perform an incremental search in the <i>Regular
Expression</i>,
<i>Alert</i>, and <i>Timer or Tab Name</i>
by entering text in the <i>Find:</i> box. The
[Enter] key or the double arrow button performs a 'find next'.</li>
</ul>
<li>Edit Trigger Dialog (double-click a trigger regular
expression)</li>
<ul>
<li>When editing a trigger's <i>Category / Zone</i>
or<i> Regular Expression</i>,
the dialog allows for replacing the original trigger or creating a new
trigger.</li>
</ul>
<ul>
<li>If a line is copied from the<i> View Logs</i>,
it can be
properly formatted and pasted into the <i>Regular Expression</i>
using the
[Paste Clipboard] button in the trigger edit dialog.</li>
</ul>
<ul>
<li>Selecting text in the <i>Regular Expression</i>
and then
right-clicking allows replacing the selected text with a capture group.</li>
<li><i>Regular Expression</i> syntax is checked
as each character is
typed in the box. An invalid expression turns the text red. If one of
the dialog closing buttons (other than [Cancel]) is pressed when the
expression is invalid, an error popup is displayed giving more
information on the problem.</li>
</ul>
<ul>
<li>Capture groups can be easily inserted into the TTS
expression using the drop down list of capture names and the [Insert
capture name] button.</li>
</ul>
<ul>
<li>The [Search for spell timer] button will open ACT's <i>Spell
Timers (Options)</i> form and search for
matching spell timer name.<br>
</li>
</ul>
<ul>
<li>The <i>Show Encounters</i>
checkbox&nbsp;will display a list of&nbsp;encounters. Selecting
an encounter will display
the log lines for that encounter, similar to ACT's <i>View Logs</i>.
The
dialog can be resized to see more of the log lines.</li>
<li>Display only matching log lines by entering <i>Filter:</i>
text.
&nbsp;Useful filters include '#' for showing colored lines and
'says,' for showing mob dialog.</li>
<li>Right click a log line for a context menu to format and
paste the line into the <i>Regular Expression</i>.</li>
<li>Right click a log line for a context menu to test the
trigger against the log line.</li>
</ul>
</ul>
<u><a name='macro_share'></a>Sharing via an
EQII Macro</u><br>
Wouldn't it be nice to be able to share all triggers and their timers
for a zone in one
operation rather than copy-paste for each one? &nbsp;An EQII macro
can <i>almost</i> do it.<br>
<br>
Unfortunately, EQII macros cannot handle all the XML encoding used in
shared triggers. Some of the problems can be avoided by changing the
trigger. Other problems simply prevent the trigger or spell timer from
being shared via a macro and it must be shared via the clipboard.
Prohibited characters are listed in
the tool tip for the&nbsp;<i>Share Macro</i> context
menus when macro share is
disabled.<br>
<br>
You might want to use a macro to share a trigger to avoid clipboard
problems such as the occasional clipboard failure or pasting the parse
into chat rather than the trigger.<br>
<br>
If a trigger can be saved in a macro, the 'macro play' icon is
displayed next to its checkbox. &nbsp;If the trigger has a <i>Timer
or Tab name</i> and the timer can be saved in a macro, the 'macro
play' button is displayed next to&nbsp;that checkbox.<br>
<br>
When a trigger&nbsp;can be saved in a macro, selecting the
trigger's <i>Raidsay Share Macro</i> or <i>Groupsay
Share Macro</i> context menu will create a triggers.txt file to
either /raidsay or /groupsay the shared trigger, respectively. If the
trigger has a timer that can be saved in a macro, the timer is also
written to the macro. To actually share the trigger in EQII, the user
would enter in an EQII chat box: <br>
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;/do_file_commands triggers.txt<br>
<br>
If a Category has one or more triggers that can be saved in a macro,
selecting the Category's <i>Raidsay Share Macro</i> or <i>Groupsay
Share Macro</i> context menu will create a triggers.txt file to
either /raidsay or /groupsay, respectively, all of the valid triggers
and their valid timers. Macros are limited to 16 lines.<br>
<br>
Following are some macro issues and any workaround:<br>
<ul>
<li>If the Category name contains any prohibited
characters, trigger macro share is disabled. There is no workaround.</li>
<li>If the trigger <i>Regular Expression</i>
contains prohibited
characters, it can often be rewritten to remove those characters. In
many cases the offending characters can just be removed from the
beginning or end of the trigger without affecting its usefulness.
Example alternate approaches or when that's not
feasable&nbsp;include:</li>
<ul>
<li><i>Praetorian K'Tikrn gets sick</i> : replace
the apostrophe with a wild card period, becoming : <i>Praetorian
K.Tikrn gets sick</i></li>
<li><i>prepares to unleash a mighty barrage in
(?&lt;player&gt;\w+)</i> : replace the named capture
group with a numbered capture group, becoming : <i>prepares to
unleash a mighty barrage in (\w+)</i></li>
<li><i>You have infected your enemies
with&nbsp;\\#FF0000Dark Incubation!</i>&nbsp;:
replace&nbsp;the \\ with a wild card period, becoming : <i>You
have infected your enemies with .#FF0000Dark Incubation!</i></li>
<li><i>\\#FF9900You feel energized by Crystalline
Destruction</i> : just remove the slashes, becoming : <i>#FF9900You
feel energized by Crystalline Destruction</i></li>
<li><i>&nbsp;says, ''Bring on the rocks</i><i>
: </i>replace the double quote with a wild card period, becoming
:<i> &nbsp;says, .Bring on the rocks</i><i><br>
</i></li>
</ul>
<li>If the trigger <i>Alert</i> or <i>Timer
or Tab name</i> contain prohibited
characters, they can often be written differently to exclude those
characters.</li>
<li>If the timer name, sounds, or tool tip contain prohibited
&nbsp;characters, they can often be written differently to exclude
those characters.</li>
<li>Note that if the timer Category contains prohibited
characters,
changing it may break functionality if the <i>Restrict to
category zone or mob</i> checkbox is checked.</li>
<li>The
Category context menu for spell timers searches for timers with a
matching Category or Custom Tooltip. When the Custom Tooltip property
is used to provide a match, any apostrophes in the category name may be
replaced with a period and match will still succeed. For example, a
Custom Tooltip of<i> Ssraeshza's Hallowed Halls [Raid]</i>
can be replaced with <i>Ssraeshza.s Hallowed Halls [Raid]</i>
and the spell timer will still be added to the&nbsp;Category
context menu for <i>Ssraeshza's Hallowed Halls [Raid]</i>.
This can often allow the spell timer to be written to a macro.</li>
</ul>
<button value=""Close"" name=""Close"" style=""float: right;"" >Close</button ><br>
                </body>
                </html>
                ";
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Document.Body.MouseDown += Body_MouseDown;
        }

        private void Body_MouseDown(object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = webBrowser1.Document.GetElementFromPoint(e.ClientMousePosition);
                    if (element != null && "Close".Equals(element.GetAttribute("name"), StringComparison.OrdinalIgnoreCase))
                    {
                        panelHelp.Visible = false;
                        helpToolStripMenuItem.Checked = panelHelp.Visible;
                    }
                    break;
            }
        }

        #endregion Help

        #region Category Tree

        void PopulateCatsTree()
        {
            //try to save the current selection
            string prevCat = string.Empty;
            if (treeDict != null)
            {
                TreeNode sel = treeViewCats.SelectedNode;
                if(sel != null)
                {
                    prevCat = sel.Text;
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
                }
            }
            UpdateCategoryColors(ActGlobals.oFormActMain, treeViewCats, false);

            //try to restore previous selection
            if (!string.IsNullOrEmpty(prevCat))
            {
                TreeNode[] nodes = treeViewCats.Nodes.Find(prevCat, false);
                if (nodes.Length > 0)
                {
                    treeViewCats.SelectedNode = nodes[0];
                    treeViewCats.SelectedNode.EnsureVisible();
                }
            }
            else
            {
                if(treeViewCats.Nodes.Count > 0)
                    treeViewCats.SelectedNode = treeViewCats.Nodes[0];
            }
        }

        delegate void UpdateCategoryColorsCallback(Form parent, TreeView target, bool autoSelect);
        private void UpdateCategoryColors(Form parent, TreeView target, bool autoSelect)
        {
            if (target.InvokeRequired)
            {
                UpdateCategoryColorsCallback cb = new UpdateCategoryColorsCallback(UpdateCategoryColors);
                parent.Invoke(cb, new object[] { parent, target, autoSelect });
            }
            else
            {
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
                                if (trigger.RestrictToCategoryZone == false || key.Equals(zoneName))
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
        }

        private void treeViewCats_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            // if treeview's HideSelection property is "True", 
            // this will always returns "False" on unfocused treeview
            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var unfocused = !e.Node.TreeView.Focused;
            bool green = (bool)e.Node.Tag;

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
                //UpdateCategoryColors(ActGlobals.oFormActMain, treeViewCats, false);
                treeViewCats.SelectedNode.BackColor = activeBackground;
            }
        }

        private void deleteEntireCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                string category = clickedCategoryNode.Text;
                if (MessageBox.Show(ActGlobals.oFormActMain, "Delete category '" + category + "' and all its triggers?", "Are you sure?",
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
                    MessageBox.Show(ActGlobals.oFormActMain, "Not found");
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
                    Point screen = treeViewCats.PointToScreen(pt);
                    contextMenuStripCat.Show(screen);
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
                int validTrigs = 0;
                int validTimers = 0;
                int invalid = 0;
                int fileCount = 0;
                if (treeDict.TryGetValue(category, out triggers))
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        //start with timers for the category
                        foreach (TimerData timer in categoryTimers)
                        {
                            if (!Macros.IsInvalidMacroTimer(timer))
                            {
                                sb.Append(sayCmd);
                                sb.Append(SpellTimerToMacro(timer));
                                sb.Append(Environment.NewLine);
                                validTimers++;
                                if (validTimers >= 16)
                                {
                                    MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs);
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
                        //then category triggers
                        foreach (CustomTrigger trigger in triggers)
                        {
                            if (trigger.Active)
                            {
                                if (Macros.IsInvalidMacroTrigger(trigger))
                                {
                                    invalid++;
                                }
                                else
                                {
                                    sb.Append(sayCmd);
                                    sb.Append(TriggerToMacro(trigger));
                                    sb.Append(Environment.NewLine);
                                    validTrigs++;
                                }
                                if (validTrigs + validTimers >= 16)
                                {
                                    MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs);
                                    fileCount++;
                                    sb.Clear();
                                    invalid = 0;
                                    validTimers = 0;
                                    validTrigs = 0;
                                }
                                List<TimerData> timers = FindTimers(trigger);
                                foreach (TimerData timer in timers)
                                {
                                    if (!categoryTimers.Contains(timer))
                                    {
                                        if (!Macros.IsInvalidMacroTimer(timer))
                                        {
                                            sb.Append(sayCmd);
                                            sb.Append(SpellTimerToMacro(timer));
                                            sb.Append(Environment.NewLine);
                                            validTimers++;
                                            if (validTrigs + validTimers >= 16)
                                            {
                                                //tooLong = true;
                                                MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs);
                                                fileCount++;
                                                sb.Clear();
                                                invalid = 0;
                                                validTimers = 0;
                                                validTrigs = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (validTrigs > 0)
                        {
                            MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs);
                        }
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(this, "Macro file error:\n" + x.Message);
                    }
                }
            }
        }

        private void MacroToFile(int fileCount, string category, string content, int invalid, int validTimers, int validTrigs)
        {
            string fileName = doFileName;
            if (fileCount > 0)
                fileName = Path.GetFileNameWithoutExtension(doFileName) + fileCount.ToString() + Path.GetExtension(doFileName);
            if (ActGlobals.oFormActMain.SendToMacroFile(fileName, content, string.Empty))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.IsNullOrEmpty(category) ? string.Empty : string.Format("For category\n'{0}'\n", category));
                sb.Append("Wrote ");
                sb.Append(validTrigs > 0 ? string.Format("{0} trigger{1}", validTrigs, validTrigs > 1 ? "s" : string.Empty) : string.Empty);
                sb.Append(validTrigs > 0 && validTimers > 0 ? " and " : string.Empty);
                sb.Append(validTimers > 0 ? string.Format("{0} timer{1}", validTimers, validTimers > 1 ? "s" : string.Empty) : string.Empty);
                sb.Append(invalid > 0 ? string.Format("\n\nCould not write {0} item{1}.", invalid, invalid > 1 ? "s" : string.Empty) : string.Empty);
                sb.Append(string.Format("\n\nIn EQII chat, enter:\n/do_file_commands {0}", fileName));

                TraySlider traySlider = new TraySlider();
                traySlider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                traySlider.ShowTraySlider(sb.ToString(), "Wrote Category Macro");
            }
        }

        private void contextMenuStripCat_Opening(object sender, CancelEventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                string category = clickedCategoryNode.Text;
                int canMacroTimer = 0;
                int cannotMacroTimer = 0;

                //add any spell timers for the category
                categorySpellTimersMenuItem.DropDownItems.Clear();
                categoryTimers = FindCategoryTimers(category);
                if (categoryTimers.Count == 0)
                {
                    categorySpellTimersMenuItem.Enabled = false;
                    categorySpellTimersMenuItem.ToolTipText = "Found no Spell Timers with matching Category or Custom Tooltip";
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
                        txt = string.Format(catMacroText, "Groupsay", canMacroTimer, canMacroTimer+cannotMacroTimer);
                        timerGroupMacroItem.Name = timerGroupMacroItem.Text = txt;
                        timerGroupMacroItem.Click += TimerMacroItem_Click;
                        categorySpellTimersMenuItem.DropDownItems.Add(timerGroupMacroItem);
                    }
                }

                //count triggers for the menu text
                List<CustomTrigger> triggers;
                int valid = 0;
                int invalid = 0;
                if (treeDict.TryGetValue(category, out triggers))
                {
                    foreach (CustomTrigger trigger in triggers)
                    {
                        if (trigger.Active)
                        {
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
                        }
                    }
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
                    sb.Append(SpellTimerToMacro(timer));
                    sb.Append(Environment.NewLine);
                    validTimers++;
                    if (validTimers >= 16)
                    {
                        MacroToFile(fileCount, string.Empty, sb.ToString(), invalid, validTimers, 0);
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
                MacroToFile(fileCount, string.Empty, sb.ToString(), invalid, validTimers, 0);
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
                    string xml = SpellTimerToXML(timer);
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
                    if (timer.ActiveInList &&
                        (timer.Category.Equals(category) || timer.Tooltip.Equals(category) || timer.Tooltip.Equals(macroCat)))
                    {
                        if(!result.Exists(t => t.Name.Equals(timer.Name)))
                            result.Add(timer);
                    }
                }
            }
            return result;
        }

        #endregion Category Tree

        #region Triggers Tree

        delegate void UpdateTriggerColorsCallback(Form parent, TreeView target);
        private void UpdateTriggerColors(Form parent, TreeView target)
        {
            if (target.InvokeRequired)
            {
                UpdateTriggerColorsCallback cb = new UpdateTriggerColorsCallback(UpdateTriggerColors);
                parent.Invoke(cb, new object[] { parent, target });
            }
            else
            {
                if (target.Nodes.Count > 0)
                {
                    CustomTrigger trigger = target.Nodes[0].Tag as CustomTrigger;
                    if (trigger != null)
                    {
                        bool categoryMatch = trigger.Category.Equals(zoneName);

                        foreach (TreeNode node in target.Nodes)
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
                    indexAlertTab = 3;
                    parent.Nodes[indexAlertTab].ImageIndex = parent.Nodes[indexAlertTab].SelectedImageIndex = triggerBlankImage;

                    //timer name child
                    parent.Nodes.Add(timerNameLabel + trigger.TimerName).Checked = trigger.Timer || trigger.Tabbed;
                    indexTimerName = 4;
                    if(CanMacroTimer(trigger))
                        parent.Nodes[indexTimerName].ImageIndex = parent.Nodes[indexTimerName].SelectedImageIndex = triggerCanMacro;
                    else
                        parent.Nodes[indexTimerName].ImageIndex = parent.Nodes[indexTimerName].SelectedImageIndex = triggerBlankImage;

                    //set colors
                    if (trigger.Active && (trigger.RestrictToCategoryZone == false || category.Equals(zoneName)))
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
                PopulateCatsTree();

                if (string.IsNullOrEmpty(zoneName))
                {
                    //we've never seen a zone change
                    // let's try to use wherever ACT thinks we are
                    zoneName = ActGlobals.oFormActMain.CurrentZone;
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
                UpdateTriggerColors(ActGlobals.oFormActMain, treeViewTrigs);

                if (initialVisible)
                {
                    //set the splitter only on the first time shown
                    int distance = Int32.Parse(textBoxSplitterLoc.Text);
                    if (distance > 0)
                        splitContainer1.SplitterDistance = distance;
                    initialVisible = false;
                }
            }
        }

        private void treeViewTrigs_AfterCheck(object sender, TreeViewEventArgs e)
        {
            bool updateACT = false;
            CustomTrigger trigger = null;

            if (e.Node.Tag != null)
            {
                //this is the regex node
                // checkbox enables or disables the trigger
                trigger = e.Node.Tag as CustomTrigger;
                if (trigger != null)
                {
                    trigger.Active = e.Node.Checked;
                    UpdateTriggerColors(ActGlobals.oFormActMain, treeViewTrigs);
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
                            UpdateTriggerColors(ActGlobals.oFormActMain, treeViewTrigs);
                            UpdateCategoryColors(ActGlobals.oFormActMain, treeViewCats, false);
                            updateACT = true;
                        }
                        else if(e.Node.Index == indexTimer)
                        {
                            trigger.Timer = e.Node.Checked;
                            if(e.Node.Parent.Nodes.Count > indexTimerName)
                                e.Node.Parent.Nodes[indexTimerName].Checked = trigger.Timer || trigger.Tabbed;
                            updateACT = true;
                        }
                        else if(e.Node.Index == indexAlertTab)
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

                PopulateCatsTree();
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
                    MessageBox.Show(ActGlobals.oFormActMain, "Was not able to delete original trigger");
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
                        || e.Node.Parent.Nodes[indexAlertTab].Checked)
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
                UpdateTriggerColors(ActGlobals.oFormActMain, treeViewTrigs);
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

                UpdateTriggerColors(ActGlobals.oFormActMain, treeViewTrigs);
                if (result == FindResult.NOT_FOUND)
                    MessageBox.Show(this, "Not found");
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
                    string category = " General";
                    if (treeViewCats.SelectedNode != null)
                        category = treeViewCats.SelectedNode.Text;
                    CustomTrigger trigger = new CustomTrigger("new expression", category);
                    trigger.RestrictToCategoryZone = category.Contains("["); //set restrict if it kinda looks like a zone name
                    FormEditTrigger formEditTrigger = new FormEditTrigger(trigger, zoneName);
                    formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
                    formEditTrigger.haveOriginal = false; //disable the replace button since there is nothing to replace
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
            else if (e.Button == MouseButtons.Left)
            {
                isDoubleClick = e.Clicks > 1; //used to prevent expand/collapse on double click
            }
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

        #region Context Menu

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
                        MessageBox.Show(this, "EQII does not allow certain characters in a macro.\nThis trigger cannot be saved to a macro.",
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
                                sb.Append(TriggerToMacro(trigger));
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
                                    sb.Append(SpellTimerToMacro(timer));
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
                            MessageBox.Show(this, "Macro file error:\n" + x.Message);
                        }
                    }
                }
            }
        }

        private List<TimerData> FindTimers(CustomTrigger trigger)
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
                    doit = MessageBox.Show(ActGlobals.oFormActMain, "Delete trigger:\n" + trigger.ShortRegexString, "Delete?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                if (doit)
                {
                    if (ActGlobals.oFormActMain.CustomTriggers.Remove(category + "|" + trigger.ShortRegexString))
                    {
                        ActGlobals.oFormActMain.RebuildActiveCustomTriggers();
                        PopulateCatsTree();
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
                        MessageBox.Show(this, "Could not find timer:\n" + trigger.TimerName, "No such timer");
                        return false;
                    }
                }
            }
            if (trigger != null)
            {
                try
                {
                    if (isTimer)
                        Clipboard.SetText(SpellTimerToXML(timer));
                    else
                        Clipboard.SetText(TriggerToXML(trigger));
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

        private string TriggerToXML(CustomTrigger trigger)
        {
            string result = string.Empty;
            if (trigger != null)
            {
                //match the character replacement scheme used by the Custom Triggers tab
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<Trigger R=\"{0}\"", EncodeXml_ish(trigger.ShortRegexString, true, false, true)));
                sb.Append(string.Format(" SD=\"{0}\"", EncodeXml_ish(trigger.SoundData, false, true, false)));
                sb.Append(string.Format(" ST=\"{0}\"", trigger.SoundType.ToString()));
                sb.Append(string.Format(" CR=\"{0}\"", trigger.RestrictToCategoryZone ? "T" : "F"));
                sb.Append(string.Format(" C=\"{0}\"", EncodeXml_ish(trigger.Category, false, true, false)));
                sb.Append(string.Format(" T=\"{0}\"", trigger.Timer ? "T" : "F"));
                sb.Append(string.Format(" TN=\"{0}\"", EncodeXml_ish(trigger.TimerName, false, true, false)));
                sb.Append(string.Format(" Ta=\"{0}\"", trigger.Tabbed ? "T" : "F"));
                sb.Append(" />");

                result = sb.ToString();
            }
            return result;
        }

        private string SpellTimerToXML(TimerData timer)
        {
            string result = string.Empty;
            if(timer != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<Spell N=\"{0}\"", EncodeXml_ish(timer.Name, false, false, false)));
                sb.Append(string.Format(" T=\"{0}\"", timer.TimerValue));
                sb.Append(string.Format(" OM=\"{0}\"", timer.OnlyMasterTicks ? "T" : "F"));
                sb.Append(string.Format(" R=\"{0}\"", timer.RestrictToMe ? "T" : "F"));
                sb.Append(string.Format(" A=\"{0}\"",  timer.AbsoluteTiming ? "T" : "F"));
                sb.Append(string.Format(" WV=\"{0}\"", timer.WarningValue));
                sb.Append(string.Format(" RD=\"{0}\"", timer.RadialDisplay ? "T" : "F"));
                sb.Append(string.Format(" M=\"{0}\"", timer.Modable ? "T" : "F"));
                sb.Append(string.Format(" Tt=\"{0}\"", EncodeXml_ish(timer.Tooltip, false, false, false)));
                sb.Append(string.Format(" FC=\"{0}\"", timer.FillColor.ToArgb()));
                sb.Append(string.Format(" RV=\"{0}\"", timer.RemoveValue));
                sb.Append(string.Format(" C=\"{0}\"", EncodeXml_ish(timer.Category, false, false, false)));
                sb.Append(string.Format(" RC=\"{0}\"", timer.RestrictToCategory ? "T" : "F"));
                sb.Append(string.Format(" SS=\"{0}\"", timer.StartSoundData));
                sb.Append(string.Format(" WS=\"{0}\"", timer.WarningSoundData));
                sb.Append(" />");

                result = sb.ToString();
            }

            return result;
        }

        private string TriggerToMacro(CustomTrigger trigger)
        {
            string result = string.Empty;
            if (trigger != null)
            {
                //use single quotes because double quotes don't work
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<Trigger R='{0}'", trigger.ShortRegexString.Replace("\\\\", "\\\\\\\\")));
                sb.Append(string.Format(" SD='{0}'", trigger.SoundData));
                sb.Append(string.Format(" ST='{0}'", trigger.SoundType.ToString()));
                sb.Append(string.Format(" CR='{0}'", trigger.RestrictToCategoryZone ? "T" : "F"));
                sb.Append(string.Format(" C='{0}'", trigger.Category));
                sb.Append(string.Format(" T='{0}'", trigger.Timer ? "T" : "F"));
                sb.Append(string.Format(" TN='{0}'", trigger.TimerName));
                sb.Append(string.Format(" Ta='{0}'", trigger.Tabbed ? "T" : "F"));
                sb.Append(" />");

                result = sb.ToString();
            }
            return result;
        }

        private string SpellTimerToMacro(TimerData timer)
        {
            string result = string.Empty;
            if (timer != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<Spell N='{0}'", timer.Name));
                sb.Append(string.Format(" T='{0}'", timer.TimerValue));
                sb.Append(string.Format(" OM='{0}'", timer.OnlyMasterTicks ? "T" : "F"));
                sb.Append(string.Format(" R='{0}'", timer.RestrictToMe ? "T" : "F"));
                sb.Append(string.Format(" A='{0}'", timer.AbsoluteTiming ? "T" : "F"));
                sb.Append(string.Format(" WV='{0}'", timer.WarningValue));
                sb.Append(string.Format(" RD='{0}'", timer.RadialDisplay ? "T" : "F"));
                sb.Append(string.Format(" M='{0}'", timer.Modable ? "T" : "F"));
                sb.Append(string.Format(" Tt='{0}'", timer.Tooltip));
                sb.Append(string.Format(" FC='{0}'", timer.FillColor.ToArgb()));
                sb.Append(string.Format(" RV='{0}'", timer.RemoveValue));
                sb.Append(string.Format(" C='{0}'", timer.Category));
                sb.Append(string.Format(" RC='{0}'", timer.RestrictToCategory ? "T" : "F"));
                sb.Append(string.Format(" SS='{0}'", timer.StartSoundData));
                sb.Append(string.Format(" WS='{0}'", timer.WarningSoundData));
                sb.Append(" />");

                result = sb.ToString();
            }
            return result;
        }

        private string EncodeXml_ish(string text, bool encodeHash, bool encodePos, bool encodeSlashes)
        {
            if (text == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '\'':
                        if (encodePos)
                            sb.Append("&apos;");
                        else
                            sb.Append(text[i]);
                        break;
                    case '\\':
                        if (encodeSlashes)
                        {
                            if (i < len - 1)
                            {
                                //only encode double backslashes
                                if (text[i + 1] == '\\')
                                {
                                    sb.Append("&#92;&#92;");
                                    i++;
                                }
                                else
                                    sb.Append(text[i]);
                            }
                            else
                                sb.Append(text[i]);
                        }
                        else
                            sb.Append(text[i]);
                        break;
                    case '#':
                        if (encodeHash)
                            sb.Append("&#35;");
                        else //leave it alone when double encoding
                            sb.Append(text[i]);
                        break;
                    default:
                        sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        private void copyAsDoubleEncodedXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomTrigger trigger;
            string doubled = string.Empty;
            if (selectedTriggerNode.Tag != null)
            {
                trigger = selectedTriggerNode.Tag as CustomTrigger;
                string encoded = TriggerToXML(trigger);
                doubled = EncodeXml_ish(encoded, false, false, false);
            }
            else if (selectedTriggerNode.Index == indexTimerName)
            {
                trigger = selectedTriggerNode.Parent.Tag as CustomTrigger;
                //look for the timer
                List<TimerData> timers = FindTimers(trigger);
                if (timers.Count > 0)
                {
                    //can only copy one, just use the first one
                    string encoded = SpellTimerToXML(timers[0]);
                    doubled = EncodeXml_ish(encoded, false, false, false);
                }
                else
                {
                    MessageBox.Show(this, "Could not find timer:\n" + trigger.TimerName, "No such timer");
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

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelHelp.Visible = !panelHelp.Visible;
            helpToolStripMenuItem.Checked = panelHelp.Visible;
        }

        #endregion Context Menu

        #endregion Trigger Tree
    }

}
