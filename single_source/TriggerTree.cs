using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
// reference:System.Windows.Forms.DataVisualization.dll
// reference:System.Core.dll

[assembly: AssemblyTitle("Tree view of Custom Triggers")]
[assembly: AssemblyDescription("An alternate interface for managing Custom Triggers")]
[assembly: AssemblyCompany("Mineeme of Maj'Dul")]
[assembly: AssemblyVersion("1.3.0.0")]

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

        bool neverBeenVisible = true;               //save the splitter location only if it has been initialized 

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
        int saveSplitterLoc = -1;
        private LinkLabel linkLabel1;

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
            this.treeViewTrigs = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxCurrentCategory = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonFindNext = new System.Windows.Forms.Button();
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
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.raidShareCategoryMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupShareCategoryMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.categorySpellTimersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textBoxCatFind = new ACT_TriggerTree.TextBoxX();
            this.textBoxTrigFind = new ACT_TriggerTree.TextBoxX();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.contextMenuStripTrig.Size = new System.Drawing.Size(290, 248);
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
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
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
            // textBoxTrigFind
            // 
            this.textBoxTrigFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTrigFind.ButtonTextClear = true;
            this.textBoxTrigFind.Location = new System.Drawing.Point(40, 4);
            this.textBoxTrigFind.Name = "textBoxTrigFind";
            this.textBoxTrigFind.Size = new System.Drawing.Size(326, 20);
            this.textBoxTrigFind.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxTrigFind, "Incremental search for text in the trigger\'s regular expression, alert, or timer " +
        "name");
            this.textBoxTrigFind.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
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

            try
            {
                Version localVersion = this.GetType().Assembly.GetName().Version;
                Version remoteVersion = new Version(ActGlobals.oFormActMain.PluginGetRemoteVersion(pluginId).TrimStart(new char[] { 'v' }));    // Strip any leading 'v' from the string before passing to the Version constructor
                if (remoteVersion > localVersion)
                {
                    DialogResult result = SimpleMessageBox.Show(ActGlobals.oFormActMain,
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
            xmlSettings.AddIntSetting("saveSplitterLoc");

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
            //store the splitter location
            // but only save it if it was ever set
            if (!neverBeenVisible)
            {
                saveSplitterLoc = splitContainer1.SplitterDistance;
            }

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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                SimpleMessageBox.Show(this, ex.Message, "Unable to open link that was clicked.");
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
                //UpdateCategoryColors(ActGlobals.oFormActMain, treeViewCats, false);
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
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, "Not found");
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
                        SimpleMessageBox.Show(this, "Macro file error:\n" + x.Message);
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

                if (neverBeenVisible)
                {
                    //set the splitter only on the first time shown
                    if (saveSplitterLoc > 0)
                        splitContainer1.SplitterDistance = saveSplitterLoc;
                    neverBeenVisible = false;
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
                    SimpleMessageBox.Show(this, "Not found");
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
                        SimpleMessageBox.Show(this, @"EQII does not allow certain characters in a macro.\line This trigger cannot be saved to a macro.",
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
                            SimpleMessageBox.Show(this, x.Message, "Macro file error");
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
                    doit = SimpleMessageBox.Show(ActGlobals.oFormActMain, @"\ql" + trigger.ShortRegexString, "Delete Trigger?",
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
                        SimpleMessageBox.Show(this, @"Could not find timer:\line\line " + trigger.TimerName, "No such timer");
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
                    SimpleMessageBox.Show(this, @"Could not find timer:\line\line " + trigger.TimerName, "No such timer");
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

	#region FormEditTrigger.cs

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
        TreeNode lastSelectedNode;          //for better tree node highlighting

        //color the regex depending on restricted status / matching
        Color activeColor = Color.Green;
        Color inactiveColor = Color.Black;

        // macro validity indicators
        ImageList macroIcons = new ImageList();

        //set by owner
        public bool haveOriginal = true;    //set false by parent when creating a brand new trigger
        int logMenuRow = -1;                //context menu location in the log line grid view

        //encounter treeview scrolls inappropriately, use this to fix it
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        private const int SB_HORZ = 0x0;

        public FormEditTrigger()
        {
            InitializeComponent();
        }

        public FormEditTrigger(CustomTrigger trigger, string category)
        {
            InitializeComponent();

            zoneCategory = category;
            undoTrigger = trigger;
            //make a new trigger that we can modify without changing the original trigger
            editingTrigger = new CustomTrigger(trigger.RegEx.ToString(), trigger.SoundType, trigger.SoundData, trigger.Timer, trigger.TimerName, trigger.Tabbed);
            editingTrigger.Category = trigger.Category;
            editingTrigger.RestrictToCategoryZone = trigger.RestrictToCategoryZone;

            macroIcons.Images.Add(Macros.GetActionBitmap());
            macroIcons.Images.Add(Macros.GetActionNotBitmap());
        }

        private void FormEditTrigger_Shown(object sender, EventArgs e)
        {
            //hide encounters, initially
            this.Height = this.MinimumSize.Height;

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
                    SimpleMessageBox.Show(this, "Category / Zone cannot be empty");
                    return;
                }

                if (regexChanged)
                {
                    if(string.IsNullOrEmpty(textBoxRegex.Text.Trim()))
                    {
                        SimpleMessageBox.Show(this, "Regular Expression cannot be empty");
                        return;
                    }

                    try
                    {
                        //test for valid regex
                        Regex testregex = new Regex(textBoxRegex.Text);
                    }
                    catch (ArgumentException aex)
                    {
                        //ActGlobals.oFormActMain.NotificationAdd("Improper Custom Trigger Regular Expression", aex.Message);
                        SimpleMessageBox.Show(this, aex.Message, "Improper Regular Expression");
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
                        SimpleMessageBox.Show(this, "WAV file does not exist");
                        return;
                    }
                }

                if((editingTrigger.Timer || editingTrigger.Tabbed)
                    && string.IsNullOrEmpty(editingTrigger.TimerName))
                {
                    if (SimpleMessageBox.Show(this, @"Timer or Tab enabled without a Timer/Tab Name.\line Return to fix?", "Inconsistent Settings",
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
                    text = match.Groups["expr"].Value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)").Replace("?", "\\?");
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
                SimpleMessageBox.Show(this, "Enter a spell timer name to search");
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
                editingTrigger.Category = textBoxCategory.Text;
                buttonReplace.Enabled = true;
                buttonUpdateCreate.Text = "Create New";
                buttonUpdateCreate.Enabled = true;
                buttonReplace.Enabled = haveOriginal;

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
            if (!editingTrigger.RestrictToCategoryZone || zoneCategory.Equals(editingTrigger.Category))
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
                regexChanged = true;
                buttonReplace.Enabled = haveOriginal;
                buttonUpdateCreate.Enabled = true;
                buttonUpdateCreate.Text = "Create New";
                checkBoxFilterRegex.Checked = false;
                PopulateGroupList();

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
                    if (!editingTrigger.RestrictToCategoryZone || zoneCategory.Equals(editingTrigger.Category))
                        textBoxRegex.ForeColor = activeColor;
                    else
                        textBoxRegex.ForeColor = inactiveColor;
                }
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
            }
            else
            {
                contextMenuRegex.Items["Cut"].Enabled = true;
                contextMenuRegex.Items["Copy"].Enabled = true;
                contextMenuRegex.Items["Delete"].Enabled = true;
                contextMenuRegex.Items["MakeNumbered"].Enabled = true;
                contextMenuRegex.Items["MakePlayer"].Enabled = true;
                contextMenuRegex.Items["MakeAttacker"].Enabled = true;
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
                SimpleMessageBox.Show(this, exc.Message, "Improper filter");
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
                    if(re.Match(list[i].LogLine).Success)
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
            if (string.IsNullOrEmpty(textBoxFindLine.Text) || dataGridViewLines.Rows.Count < 2)
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
                if (!zone.Equals(textBoxCategory.Text))
                {
                    textBoxCategory.Text = zone;
                    checkBoxRestrict.Checked = zoneCategory.Contains("[");
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
                string line = dataGridViewLines.Rows[logMenuRow].Cells["LogLine"].Value.ToString();
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
                            string player = ActGlobals.charName;
                            ActGlobals.oFormSpellTimers.NotifySpell(player, textBoxTimer.Text, false, player, true);
                        }
                        if(checkBoxResultsTab.Checked)
                        {
                            //not sure how to do this
                        }
                    }
                }
                else
                {
                    SimpleMessageBox.Show(this, "Regular Expression does not match the log line", "No Match");
                }
            }
            catch (Exception rex)
            {
                SimpleMessageBox.Show(this, rex.Message, "Invalid regular expression");
            }
        }

        private void showTimeDifferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dataGridViewLines.Rows.Count > 100)
            {
                if (SimpleMessageBox.Show(this, @"There are more than 100 filtered lines.\line Are you sure the filter is correct?"
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
                        SimpleMessageBox.Show(this, @"The regular expression does not match the text used to determine the timer value." 
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
                SimpleMessageBox.Show(this, dtx.Message, "Problem collecting the log lines");
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
                    FillEncounterLines(node);
                }
            }
        }

        #endregion Encounters
    }
	#endregion FormEditTrigger.cs
	#region FormEditTrigger.Designer.cs
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
            this.dataGridViewLines = new System.Windows.Forms.DataGridView();
            this.checkBoxLogLines = new System.Windows.Forms.CheckBox();
            this.treeViewEncounters = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxTts = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBoxTimer = new System.Windows.Forms.PictureBox();
            this.pictureBoxCat = new System.Windows.Forms.PictureBox();
            this.pictureBoxRe = new System.Windows.Forms.PictureBox();
            this.panelTest = new System.Windows.Forms.Panel();
            this.splitContainerLog = new System.Windows.Forms.SplitContainer();
            this.panelLogLines = new System.Windows.Forms.Panel();
            this.panelLogFind = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panelRegex = new System.Windows.Forms.Panel();
            this.labelGridHelp = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.contextMenuLog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pasteInRegularExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testWithRegularExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showTimeDifferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxFilterRegex = new System.Windows.Forms.CheckBox();
            this.textBoxFindLine = new ACT_TriggerTree.TextBoxX();
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
            this.textBoxRegex.Size = new System.Drawing.Size(459, 20);
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
            this.MakeAttacker});
            this.contextMenuRegex.Name = "contextMenuStrip1";
            this.contextMenuRegex.Size = new System.Drawing.Size(278, 220);
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
            // buttonUpdateCreate
            // 
            this.buttonUpdateCreate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.helpProvider1.SetHelpString(this.buttonUpdateCreate, "Update current trigger. Or if the Regular Expression or Category / Zone has chang" +
        "ed, create New trigger.");
            this.buttonUpdateCreate.Location = new System.Drawing.Point(191, 11);
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
            this.buttonReplace.Location = new System.Drawing.Point(280, 11);
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
            this.buttonCancel.Location = new System.Drawing.Point(369, 11);
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
            this.textBoxSound.Size = new System.Drawing.Size(341, 20);
            this.textBoxSound.TabIndex = 10;
            this.textBoxSound.TextChanged += new System.EventHandler(this.textBoxSound_TextChanged);
            // 
            // buttonFileOpen
            // 
            this.buttonFileOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.buttonFileOpen, "Browse for wav files");
            this.buttonFileOpen.Location = new System.Drawing.Point(473, 15);
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
            this.buttonPlay.Location = new System.Drawing.Point(505, 15);
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
            this.textBoxCategory.Size = new System.Drawing.Size(334, 20);
            this.textBoxCategory.TabIndex = 3;
            this.textBoxCategory.TextChanged += new System.EventHandler(this.textBoxCategory_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 183);
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
            this.textBoxTimer.Location = new System.Drawing.Point(129, 179);
            this.textBoxTimer.Name = "textBoxTimer";
            this.helpProvider1.SetShowHelp(this.textBoxTimer, true);
            this.textBoxTimer.Size = new System.Drawing.Size(147, 20);
            this.textBoxTimer.TabIndex = 16;
            this.textBoxTimer.TextChanged += new System.EventHandler(this.textBoxTimer_TextChanged);
            // 
            // checkBoxRestrict
            // 
            this.checkBoxRestrict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxRestrict.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxRestrict, "Check to restrict matching the regular expression only when in the specified zone" +
        "");
            this.checkBoxRestrict.Location = new System.Drawing.Point(472, 72);
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
            this.checkBoxTimer.Location = new System.Drawing.Point(388, 181);
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
            this.checkBoxResultsTab.Location = new System.Drawing.Point(281, 181);
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
            this.buttonZone.Location = new System.Drawing.Point(3, 67);
            this.buttonZone.Name = "buttonZone";
            this.helpProvider1.SetShowHelp(this.buttonZone, true);
            this.buttonZone.Size = new System.Drawing.Size(109, 23);
            this.buttonZone.TabIndex = 2;
            this.buttonZone.Text = "Category / Zone ???";
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
            this.label2.Size = new System.Drawing.Size(629, 15);
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
            // comboBoxGroups
            // 
            this.comboBoxGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxGroups.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.comboBoxGroups, "Captures from the Regular Expression available for use in the TTS alert.");
            this.comboBoxGroups.Location = new System.Drawing.Point(125, 44);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.helpProvider1.SetShowHelp(this.comboBoxGroups, true);
            this.comboBoxGroups.Size = new System.Drawing.Size(148, 21);
            this.comboBoxGroups.TabIndex = 13;
            this.toolTip1.SetToolTip(this.comboBoxGroups, "Available capture names");
            // 
            // buttonInsert
            // 
            this.buttonInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInsert.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpProvider1.SetHelpString(this.buttonInsert, "Insert selected capture name into TTS expression");
            this.buttonInsert.Location = new System.Drawing.Point(278, 43);
            this.buttonInsert.Name = "buttonInsert";
            this.helpProvider1.SetShowHelp(this.buttonInsert, true);
            this.buttonInsert.Size = new System.Drawing.Size(25, 23);
            this.buttonInsert.TabIndex = 14;
            this.buttonInsert.Text = "???";
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
            this.buttonPaste.Location = new System.Drawing.Point(590, 41);
            this.buttonPaste.Name = "buttonPaste";
            this.helpProvider1.SetShowHelp(this.buttonPaste, true);
            this.buttonPaste.Size = new System.Drawing.Size(42, 23);
            this.buttonPaste.TabIndex = 1;
            this.buttonPaste.Text = "7??";
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
            this.buttonFindTimer.Location = new System.Drawing.Point(482, 177);
            this.buttonFindTimer.Name = "buttonFindTimer";
            this.helpProvider1.SetShowHelp(this.buttonFindTimer, true);
            this.buttonFindTimer.Size = new System.Drawing.Size(25, 23);
            this.buttonFindTimer.TabIndex = 25;
            this.buttonFindTimer.Text = "???";
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
            this.dataGridViewLines.Size = new System.Drawing.Size(487, 145);
            this.dataGridViewLines.TabIndex = 0;
            this.dataGridViewLines.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dataGridViewLines_CellContextMenuStripNeeded);
            this.dataGridViewLines.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewLines_CellFormatting);
            // 
            // checkBoxLogLines
            // 
            this.checkBoxLogLines.AutoSize = true;
            this.helpProvider1.SetHelpString(this.checkBoxLogLines, "Check to show the list of encounters and log lines. Useful for creating and testi" +
        "ng new triggers.");
            this.checkBoxLogLines.Location = new System.Drawing.Point(12, 208);
            this.checkBoxLogLines.Name = "checkBoxLogLines";
            this.helpProvider1.SetShowHelp(this.checkBoxLogLines, true);
            this.checkBoxLogLines.Size = new System.Drawing.Size(110, 17);
            this.checkBoxLogLines.TabIndex = 26;
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
            this.treeViewEncounters.Size = new System.Drawing.Size(144, 172);
            this.treeViewEncounters.TabIndex = 0;
            this.treeViewEncounters.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewEncounters_AfterSelect);
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
            this.groupBox1.Controls.Add(this.comboBoxGroups);
            this.groupBox1.Controls.Add(this.textBoxSound);
            this.groupBox1.Controls.Add(this.buttonFileOpen);
            this.groupBox1.Controls.Add(this.buttonPlay);
            this.groupBox1.Location = new System.Drawing.Point(5, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(627, 74);
            this.groupBox1.TabIndex = 5;
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
            this.label4.Location = new System.Drawing.Point(309, 49);
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
            this.label1.Size = new System.Drawing.Size(637, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Changing any other field simply updates the existing trigger.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxTimer
            // 
            this.pictureBoxTimer.ErrorImage = null;
            this.pictureBoxTimer.Location = new System.Drawing.Point(111, 183);
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
            // panelTest
            // 
            this.panelTest.Controls.Add(this.splitContainerLog);
            this.panelTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTest.Location = new System.Drawing.Point(0, 228);
            this.panelTest.MinimumSize = new System.Drawing.Size(630, 170);
            this.panelTest.Name = "panelTest";
            this.panelTest.Size = new System.Drawing.Size(635, 172);
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
            this.splitContainerLog.Size = new System.Drawing.Size(635, 172);
            this.splitContainerLog.SplitterDistance = 144;
            this.splitContainerLog.TabIndex = 1;
            // 
            // panelLogLines
            // 
            this.panelLogLines.Controls.Add(this.dataGridViewLines);
            this.panelLogLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLogLines.Location = new System.Drawing.Point(0, 27);
            this.panelLogLines.Name = "panelLogLines";
            this.panelLogLines.Size = new System.Drawing.Size(487, 145);
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
            this.panelLogFind.Size = new System.Drawing.Size(487, 27);
            this.panelLogFind.TabIndex = 3;
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
            this.panelRegex.Size = new System.Drawing.Size(635, 228);
            this.panelRegex.TabIndex = 27;
            // 
            // labelGridHelp
            // 
            this.labelGridHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGridHelp.AutoSize = true;
            this.labelGridHelp.Location = new System.Drawing.Point(157, 209);
            this.labelGridHelp.Name = "labelGridHelp";
            this.labelGridHelp.Size = new System.Drawing.Size(196, 13);
            this.labelGridHelp.TabIndex = 27;
            this.labelGridHelp.Text = "Right-click a log line for the option menu";
            this.labelGridHelp.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonUpdateCreate);
            this.panel2.Controls.Add(this.buttonReplace);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 400);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(635, 37);
            this.panel2.TabIndex = 28;
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
            // checkBoxFilterRegex
            // 
            this.checkBoxFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFilterRegex.AutoSize = true;
            this.checkBoxFilterRegex.Location = new System.Drawing.Point(408, 4);
            this.checkBoxFilterRegex.Name = "checkBoxFilterRegex";
            this.checkBoxFilterRegex.Size = new System.Drawing.Size(72, 17);
            this.checkBoxFilterRegex.TabIndex = 3;
            this.checkBoxFilterRegex.Text = "By Regex";
            this.checkBoxFilterRegex.UseVisualStyleBackColor = true;
            this.checkBoxFilterRegex.CheckedChanged += new System.EventHandler(this.checkBoxFilterRegex_CheckedChanged);
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
            this.textBoxFindLine.Size = new System.Drawing.Size(355, 20);
            this.textBoxFindLine.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBoxFindLine, "Show lines containing text. Examples: \'#\' for colored lines. \'says,\' for mob dial" +
        "og.");
            this.textBoxFindLine.TextChanged += new System.EventHandler(this.textBoxFindLine_TextChanged);
            // 
            // FormEditTrigger
            // 
            this.AcceptButton = this.buttonUpdateCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 437);
            this.Controls.Add(this.panelTest);
            this.Controls.Add(this.panelRegex);
            this.Controls.Add(this.panel2);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 310);
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
        private System.Windows.Forms.ComboBox comboBoxGroups;
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
    }
	#endregion FormEditTrigger.Designer.cs
	#region FormEditSound.cs

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
	#endregion FormEditSound.cs
	#region FormEditSound.Designer.cs
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
            this.pictureBoxTts = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTts)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonNone
            // 
            this.radioButtonNone.AutoSize = true;
            this.radioButtonNone.Location = new System.Drawing.Point(5, 16);
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
            this.radioButtonBeep.Location = new System.Drawing.Point(5, 40);
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
            this.radioButtonWav.Location = new System.Drawing.Point(63, 16);
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
            this.radioButtonTts.Location = new System.Drawing.Point(63, 40);
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
            this.textBoxSound.Size = new System.Drawing.Size(248, 20);
            this.textBoxSound.TabIndex = 0;
            this.textBoxSound.TextChanged += new System.EventHandler(this.textBoxSound_TextChanged);
            // 
            // buttonInsCapture
            // 
            this.buttonInsCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInsCapture.Enabled = false;
            this.buttonInsCapture.Font = new System.Drawing.Font("Wingdings 3", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.helpProvider1.SetHelpString(this.buttonInsCapture, "Insert the selected capture into the alert sound");
            this.buttonInsCapture.Location = new System.Drawing.Point(253, 37);
            this.buttonInsCapture.Name = "buttonInsCapture";
            this.helpProvider1.SetShowHelp(this.buttonInsCapture, true);
            this.buttonInsCapture.Size = new System.Drawing.Size(32, 23);
            this.buttonInsCapture.TabIndex = 8;
            this.buttonInsCapture.Text = "??";
            this.buttonInsCapture.UseVisualStyleBackColor = true;
            this.buttonInsCapture.Click += new System.EventHandler(this.buttonInsCapture_Click);
            // 
            // comboBoxGroups
            // 
            this.comboBoxGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxGroups.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.comboBoxGroups, "Available capture names from the regular expression");
            this.comboBoxGroups.Location = new System.Drawing.Point(130, 39);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.helpProvider1.SetShowHelp(this.comboBoxGroups, true);
            this.comboBoxGroups.Size = new System.Drawing.Size(117, 21);
            this.comboBoxGroups.TabIndex = 7;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(148, 71);
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
            this.buttonCancel.Location = new System.Drawing.Point(230, 71);
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
            this.buttonBrowse.Location = new System.Drawing.Point(384, 11);
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
            this.buttonPlay.Location = new System.Drawing.Point(415, 11);
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
            // pictureBoxTts
            // 
            this.pictureBoxTts.Location = new System.Drawing.Point(111, 16);
            this.pictureBoxTts.Name = "pictureBoxTts";
            this.pictureBoxTts.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxTts.TabIndex = 11;
            this.pictureBoxTts.TabStop = false;
            // 
            // FormEditSound
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 106);
            this.Controls.Add(this.pictureBoxTts);
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
            this.MinimumSize = new System.Drawing.Size(415, 145);
            this.Name = "FormEditSound";
            this.helpProvider1.SetShowHelp(this, true);
            this.ShowIcon = false;
            this.Text = "Edit Alert Sound";
            this.Shown += new System.EventHandler(this.FormEditSound_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTts)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBoxTts;
    }
	#endregion FormEditSound.Designer.cs
	#region FormEditTimer.cs

    public partial class FormEditTimer : Form
    {
        CustomTrigger editingTrigger;
        public event EventHandler EditDoneEvent; //callback

        // macro validity indicators
        ImageList macroIcons = new ImageList();

        //callback event argument
        public class EditEventArgs : EventArgs
        {
            public CustomTrigger editedTrigger;

            public EditEventArgs(CustomTrigger trigger)
            {
                editedTrigger = trigger;
            }
        }

        public FormEditTimer()
        {
            InitializeComponent();
        }

        public FormEditTimer(CustomTrigger trigger, EventHandler handler)
        {
            InitializeComponent();

            EditDoneEvent = handler;
            editingTrigger = trigger;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            editingTrigger.TimerName = textBoxName.Text;
            EditEventArgs arg = new EditEventArgs(editingTrigger);
            OnEditDoneEvent(arg);
            this.Close();
        }

        protected void OnEditDoneEvent(EditEventArgs e)
        {
            if (EditDoneEvent != null)
            {
                EventHandler handler = EditDoneEvent;
                handler.Invoke(null, e);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormEditTimer_Shown(object sender, EventArgs e)
        {
            macroIcons.Images.Add(Macros.GetActionBitmap());
            macroIcons.Images.Add(Macros.GetActionNotBitmap());

            if (editingTrigger != null)
            {
                this.Text = "Edit Timer / Tab Name: " + editingTrigger.ShortRegexString;
                textBoxName.Text = editingTrigger.TimerName;
            }

            if (string.IsNullOrEmpty(editingTrigger.TimerName))
                pictureBox1.Visible = false;
            else
            {
                pictureBox1.Visible = true;
                if (Macros.IsInvalidMacro(editingTrigger.TimerName))
                    pictureBox1.Image = macroIcons.Images[1];
                else
                    pictureBox1.Image = macroIcons.Images[0];
            }
            textBoxName.Focus();
            textBoxName.SelectAll();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text.ToLower();
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

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (macroIcons.Images.Count > 0)
            {
                if (string.IsNullOrEmpty(textBoxName.Text))
                    pictureBox1.Visible = false;
                else
                {
                    pictureBox1.Visible = true;
                    if (Macros.IsInvalidMacro(textBoxName.Text))
                        pictureBox1.Image = macroIcons.Images[1];
                    else
                        pictureBox1.Image = macroIcons.Images[0];
                }
            }
        }
    }
	#endregion FormEditTimer.cs
	#region FormEditTimer.Designer.cs
    partial class FormEditTimer
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOk.Location = new System.Drawing.Point(79, 53);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.Location = new System.Drawing.Point(161, 52);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Timer / Tab Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(125, 13);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(116, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // buttonFind
            // 
            this.buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFind.Location = new System.Drawing.Point(248, 11);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(54, 23);
            this.buttonFind.TabIndex = 4;
            this.buttonFind.Text = "Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(106, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // FormEditTimer
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 110);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.MinimumSize = new System.Drawing.Size(222, 127);
            this.Name = "FormEditTimer";
            this.Text = "Edit Timer / Tab Name";
            this.Shown += new System.EventHandler(this.FormEditTimer_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
	#endregion FormEditTimer.Designer.cs
	#region FormHistogram.cs

    public partial class FormHistogram : Form
    {
        Point loc;
        string _zoneName;
        string _mobName;
        public event EventHandler TimerDoneEvent; //callback

        //callback event argument
        public class TimerEventArgs : EventArgs
        {
            public string timerName;

            public TimerEventArgs(string name)
            {
                timerName = name;
            }
        }

        public FormHistogram()
        {
            InitializeComponent();
        }

        public void SetData(SortedDictionary<long, uint> data)
        {
            if (data.Keys.Count == 0)
                return;

            chart1.Series[0].Points.DataBindXY(data.Keys, data.Values);

            // see if we can pretty-up the x axis

            // collect ranges
            long first = data.Keys.First();
            long last = data.Keys.Last();
            uint biggest = data.Values.Max();
            var relevantKeys = data.Where(pair => biggest.Equals(pair.Value))
                .Select(pair => pair.Key);
            textBoxTimerSec.Text = relevantKeys.First().ToString();
            
            // now adjust the x-axis based on the data
            double NumTicks = 10;
            double AxisStart;
            double AxisEnd;
            double NewAxisStart;
            double NewAxisEnd;
            double NiceRange;
            double NiceTick;

            if (last - first > 1)
            {
                /** adjust x axis **/
                AxisStart = first;
                AxisEnd = last;
                /* Compute the new nice range and ticks */
                NiceRange = NiceNumber(AxisEnd - AxisStart, 0);
                NiceTick = NiceNumber(NiceRange / (NumTicks - 1), 1);
                if (NiceTick < 1)
                {
                    NumTicks /= 2;
                    NiceTick = NiceNumber(NiceRange / (NumTicks - 1), 1);
                }

                /* Compute the new nice start and end values */
                NewAxisStart = Math.Floor(AxisStart / NiceTick) * NiceTick;
                if (NewAxisStart == AxisStart)
                    NewAxisStart--;
                NewAxisEnd = Math.Ceiling(AxisEnd / NiceTick) * NiceTick;
                if (NewAxisEnd == NewAxisStart + NiceRange)
                    NewAxisEnd++;
                if (NewAxisEnd - NewAxisStart < NiceRange)
                    NewAxisEnd += NiceRange - (NewAxisEnd - NewAxisStart);

                chart1.ChartAreas[0].AxisX.Minimum = NewAxisStart;
                chart1.ChartAreas[0].AxisX.Maximum = NewAxisEnd;
                chart1.ChartAreas[0].AxisX.Interval = NiceTick;
            }

        }

        public void SetLocation(Point pt)
        {
            loc = pt;
        }

        public void SetNames(string timerName, string mobName, string zoneName)
        {
            textBoxTimerName.Text = timerName;
            textBoxCategory.Text = mobName;
            _zoneName = zoneName;
            _mobName = mobName;
        }

        private void FormHistogram_Shown(object sender, EventArgs e)
        {
            if (!loc.IsEmpty)
            {
                Point pt = new Point(loc.X + this.Width / 2, loc.Y + this.Height / 2);
                this.Location = pt;
            }
        }

        private double NiceNumber(double Value, int Round)
        {
            int Exponent;
            double Fraction;
            double NiceFraction;

            Exponent = (int)Math.Floor(Math.Log10(Value));
            Fraction = Value / Math.Pow(10, (double)Exponent);

            if (Round > 0)
            {
                if (Fraction < 1.5)
                    NiceFraction = 1.0;
                else if (Fraction < 3.0)
                    NiceFraction = 2.0;
                else if (Fraction < 7.0)
                    NiceFraction = 5.0;
                else
                    NiceFraction = 10.0;
            }
            else
            {
                if (Fraction <= 1.0)
                    NiceFraction = 1.0;
                else if (Fraction <= 2.0)
                    NiceFraction = 2.0;
                else if (Fraction <= 5.0)
                    NiceFraction = 5.0;
                else
                    NiceFraction = 10.0;
            }

            return NiceFraction * Math.Pow(10, (double)Exponent);
        }

        private void chart1_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = string.Format("{0} sec, {1} occurrence{2}", dp.XValue, dp.YValues[0], dp.YValues[0] != 1 ? "s" : string.Empty);
            }
        }

        private void buttonNewTimer_Click(object sender, EventArgs e)
        {
            int spellTime;
            if (Int32.TryParse(textBoxTimerSec.Text, out spellTime))
            {
                Color color = Color.FromName("Blue");
                int warningTime = 7;
                string warning = "tts " + textBoxTimerName.Text + " in " + (warningTime-2).ToString();
                TimerData td1 = new TimerData(textBoxTimerName.Text.ToLower(), false, spellTime, false, false, "", warning, warningTime, false, false, _zoneName, color, true, false);
                td1.Category = textBoxCategory.Text;
                td1.RemoveValue = 0;
                if (spellTime != 0)
                {
                    ActGlobals.oFormSpellTimers.AddEditTimerDef(td1);

                    ActGlobals.oFormSpellTimers.SearchSpellTreeView(textBoxTimerName.Text);
                    ActGlobals.oFormSpellTimers.Visible = true;

                    if (TimerDoneEvent != null)
                    {
                        EventHandler handler = TimerDoneEvent;
                        TimerEventArgs a = new TimerEventArgs(textBoxTimerName.Text.ToLower());
                        handler.Invoke(null, a);
                    }
                }
            }
        }

        private void buttonSwap_Click(object sender, EventArgs e)
        {
            if (textBoxCategory.Text.Equals(_mobName))
                textBoxCategory.Text = _zoneName;
            else
                textBoxCategory.Text = _mobName;
        }
    }
	#endregion FormHistogram.cs
	#region FormHistogram.Designer.cs

    partial class FormHistogram
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.textBoxTimerSec = new System.Windows.Forms.TextBox();
            this.buttonNewTimer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTimerName = new System.Windows.Forms.TextBox();
            this.textBoxCategory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonSwap = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTimerSec
            // 
            this.textBoxTimerSec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTimerSec.Location = new System.Drawing.Point(6, 112);
            this.textBoxTimerSec.Name = "textBoxTimerSec";
            this.textBoxTimerSec.Size = new System.Drawing.Size(108, 20);
            this.textBoxTimerSec.TabIndex = 0;
            // 
            // buttonNewTimer
            // 
            this.buttonNewTimer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonNewTimer.Location = new System.Drawing.Point(26, 172);
            this.buttonNewTimer.Name = "buttonNewTimer";
            this.buttonNewTimer.Size = new System.Drawing.Size(75, 23);
            this.buttonNewTimer.TabIndex = 1;
            this.buttonNewTimer.Text = "Make Timer";
            this.toolTip1.SetToolTip(this.buttonNewTimer, "Create a timer from the parameters above");
            this.buttonNewTimer.UseVisualStyleBackColor = true;
            this.buttonNewTimer.Click += new System.EventHandler(this.buttonNewTimer_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Seconds";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Timer Name";
            // 
            // textBoxTimerName
            // 
            this.textBoxTimerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTimerName.Location = new System.Drawing.Point(6, 19);
            this.textBoxTimerName.Name = "textBoxTimerName";
            this.textBoxTimerName.Size = new System.Drawing.Size(108, 20);
            this.textBoxTimerName.TabIndex = 4;
            // 
            // textBoxCategory
            // 
            this.textBoxCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCategory.Location = new System.Drawing.Point(6, 64);
            this.textBoxCategory.Name = "textBoxCategory";
            this.textBoxCategory.Size = new System.Drawing.Size(108, 20);
            this.textBoxCategory.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Category";
            // 
            // chart1
            // 
            chartArea6.AxisX.MajorGrid.Enabled = false;
            chartArea6.AxisX.Title = "Seconds Between";
            chartArea6.AxisY.MajorGrid.Enabled = false;
            chartArea6.AxisY.Title = "Occurrences";
            chartArea6.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea6);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend6.Enabled = false;
            legend6.Name = "Legend1";
            this.chart1.Legends.Add(legend6);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series6.ChartArea = "ChartArea1";
            series6.CustomProperties = "EmptyPointValue=Zero, MaxPixelPointWidth=15";
            series6.IsValueShownAsLabel = true;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chart1.Series.Add(series6);
            this.chart1.Size = new System.Drawing.Size(239, 207);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.chart1_GetToolTipText);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chart1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.buttonSwap);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxCategory);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxTimerName);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.buttonNewTimer);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxTimerSec);
            this.splitContainer1.Size = new System.Drawing.Size(369, 207);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 5;
            // 
            // buttonSwap
            // 
            this.buttonSwap.Font = new System.Drawing.Font("Wingdings 3", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonSwap.Location = new System.Drawing.Point(53, 43);
            this.buttonSwap.Name = "buttonSwap";
            this.buttonSwap.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.buttonSwap.Size = new System.Drawing.Size(25, 20);
            this.buttonSwap.TabIndex = 7;
            this.buttonSwap.Text = "B";
            this.toolTip1.SetToolTip(this.buttonSwap, "Swap between mob name and zone name");
            this.buttonSwap.UseVisualStyleBackColor = true;
            this.buttonSwap.Click += new System.EventHandler(this.buttonSwap_Click);
            // 
            // FormHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 207);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormHistogram";
            this.Text = "Histogram";
            this.Shown += new System.EventHandler(this.FormHistogram_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCategory;
        private System.Windows.Forms.TextBox textBoxTimerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonNewTimer;
        private System.Windows.Forms.TextBox textBoxTimerSec;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonSwap;
        private System.Windows.Forms.ToolTip toolTip1;
    }
	#endregion FormHistogram.Designer.cs
	#region TextBoxX.cs

    public partial class TextBoxX : TextBox
    {
        private readonly Label lblClear;

        // new event handler for the X "button"
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks X")]
        public event EventHandler ClickX;

        // required TextBox stuff
        public bool ButtonTextClear { get; set; }
        public AutoScaleMode AutoScaleMode;

        public TextBoxX()
        {
            InitializeComponent();

            ButtonTextClear = true;

            Resize += PositionX;

            lblClear = new Label()
            {
                Location = new Point(100, 0),
                AutoSize = true,
                Text = " X ",
                ForeColor = Color.Gray,
                Font = new Font("Tahoma", 8.25F),
                Cursor = Cursors.Arrow
            };

            Controls.Add(lblClear);
            lblClear.Click += LblClear_Click;
            lblClear.BringToFront();
        }

        private void LblClear_Click(object sender, EventArgs e)
        {
            Text = string.Empty; 
            ButtonX_Click(sender, e);
        }

        protected void ButtonX_Click(object sender, EventArgs e)
        {
            // report the event to the parent
            if (ClickX != null)
                ClickX(this, e);
        }

        private void PositionX(object sender, EventArgs e)
        { 
            lblClear.Location = new Point(Width - lblClear.Width, ((Height - lblClear.Height) / 2) - 1); 
        }
    }
	#endregion TextBoxX.cs
	#region TextBoxX.designer.cs

    partial class TextBoxX
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion
    }
	#endregion TextBoxX.designer.cs
	#region SimpleMessageBox.cs

    /// <summary>
    /// Class similar to MessageBox.Show() except the dialog placement is controlled by the user,
    /// text is RTF, and non-modal versions are available. 
    /// <para>In keeping with MessageBox, .Show() methods are modal and block the calling thread.
    /// New .ShowDialog() methods are non-modal.</para>
    /// <para>This is opposite the behaviour of Form.Show() and Form.ShowDialog().</para>
    /// <para>Both approaches: 1) matching MessageBox.Show() or 2) matching Form.Show(),
    /// seem to be equally confusing, so we're going with MessageBox.Show() compatibility.</para>
    /// </summary>
    public partial class SimpleMessageBox : Form
    {

        /// <summary>
        /// EventArgs passed to the button event handler
        /// </summary>
        public class OkEventArgs : EventArgs
        {
            /// <summary>
            /// Which button was pressed.
            /// </summary>
            public DialogResult result;
            /// <summary>
            /// The size of the message box when the button was pressed.
            /// </summary>
            public Size formSize;
            /// <summary>
            /// The screen location of the message box when the button was pressed.
            /// </summary>
            public Point formLocation;

            public override string ToString()
            {
                return string.Format("button:{0} size:{1} location:{2}", result.ToString(), formSize, formLocation);
            }
        }

        #region --- Internal Class Data ---

        /// <summary>
        /// Sets the maximum size of the message box by divding the screen height by this number.
        /// e.g. <see cref="maxSizeDivisor"/> = 5 divides the screen height by 5 
        /// making the max size of the message box 20% of the screen height.
        /// </summary>
        int maxSizeDivisor = 5;

        /// <summary>
        /// Positioning info
        /// </summary>
        Point desiredLocation = new Point(-1, -1); //save the location for _Load()
        Control parentControl;

        /// <summary>
        /// Regular expression to find the font table in the rtf
        /// </summary>
        Regex reFonts = new Regex(@"({\\fonttbl({.*;})+})", RegexOptions.Compiled);

        /// <summary>
        /// Debug to Console.Writeline info during the richTextBox1_ContentsResized() method 
        /// </summary>
        int sizePassDebug = 0;

        /// <summary>
        /// Button press event handler
        /// </summary>
        event EventHandler buttonEvent;

        #endregion --- Internal Class Data ---

        #region --- Private Constructors ---

        /// <summary>
        /// Instantiate with all parameters
        /// </summary>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf commands like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        /// <param name="parent">The parent control for positioning the message box. Null to disable.</param>
        /// <param name="location">Position for the top left corner of the form. Point(-1, -1) is ignored</param>
        /// <param name="eventHandler">Event hanlder to be called when the [OK] button is pressed</param>
        SimpleMessageBox(string text, string title, Control parent, Point location, EventHandler eventHandler)
        {
            InitializeComponent();
            this.Text = title;
            SetText(text);
            buttonEvent = eventHandler;
            desiredLocation = location;
            parentControl = parent;
        }

        /// <summary>
        /// Chain construtor with no location or event handler
        /// </summary>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf commands like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        /// <param name="parent">The parent control</param>
        SimpleMessageBox(string text, string title, Control parent) : this(text, title, parent, new Point(-1, -1), null) { }

        /// <summary>
        /// Chain constructor with no parent, location or event handler
        /// </summary>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf commands like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        SimpleMessageBox(string text, string title) : this(text, title, null, new Point(-1, -1), null) { }

        #endregion --- Private Contructors ---

        #region --- Public Show Methods ---

        #region -- Modal --

        /// <summary>
        /// Modal centered on the parent with choice of buttons and icon.
        /// </summary>
        /// <param name="parent">The control in which the popup will be centered</param>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf commands like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        /// <param name="buttons"><see cref="MessageBoxButtons"/> to be displayed. Default is OK.</param>
        /// <param name="icon"><see cref="MessageBoxIcon"/> to be displayed in the dialog. Default is none.</param>
        /// <param name="icon"><see cref="MessageBoxIcon"/> to be displayed in the dialog. Default is none.</param>
        /// <param name="defaultButton"><see cref="MessageBoxDefaultButton"/> with the initial focus. 
        /// <see cref="MessageBoxDefaultButton.Button1"/> will be the first (leftmost) button.
        /// <see cref="MessageBoxDefaultButton.Button2"/> will be the second button.
        /// <see cref="MessageBoxDefaultButton.Button3"/> will be the third button.</param>
        /// <returns><see cref="DialogResult"/></returns>
        public static DialogResult Show(Control parent, string text, string title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, 
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            SimpleMessageBox form = new SimpleMessageBox(text, title, parent);

            SetButtons(form, buttons, defaultButton);

            SetIcon(form, icon);

            // modal show
            return form.ShowDialog();
        }

        /// <summary>
        /// Modal shown at the given location with optional buttons and icon
        /// </summary>
        /// <param name="location">Screen coordinates for the placement of the top left corner of the form.</param>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf controls like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        /// <param name="buttons"></param>
        /// <param name="icon"><see cref="MessageBoxIcon"/> to be displayed in the dialog. Default is none.</param>
        /// <param name="defaultButton"><see cref="MessageBoxDefaultButton"/> with the initial focus. 
        /// <see cref="MessageBoxDefaultButton.Button1"/> will be the first (leftmost) button.
        /// <see cref="MessageBoxDefaultButton.Button2"/> will be the second button.
        /// <see cref="MessageBoxDefaultButton.Button3"/> will be the third button.</param>
        public static DialogResult Show(Point location, string text, string title,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            SimpleMessageBox form = new SimpleMessageBox(text, title, null, location, null);

            SetButtons(form, buttons, defaultButton);

            SetIcon(form, icon);

            // modal show
            return form.ShowDialog();
        }

        /// <summary>
        /// Modal "shortcut". Show in the middle of the screen with an OK button.
        /// </summary>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf controls like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        public static DialogResult Show(string text, string title)
        {
            // goes to the Show(Control, ...) since Point() is not nullable
            return Show(null, text, title);
        }

        #endregion -- Modal --
        
        #region -- Non-Modal --

        /// <summary>
        /// Non-modal centered on the parent with choice of buttons, icon, and event.
        /// </summary>
        /// <param name="parent">The control in which the popup will be centered</param>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf controls like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        /// <param name="buttons">Accepts <see cref="MessageBoxButtons"/>. Defaults to <see cref="MessageBoxButtons.OK"/></param>
        /// <param name="handler">Event handler to be called when a button is pressed</param>
        /// <param name="icon"><see cref="MessageBoxIcon"/> to be displayed in the dialog. Default is none.</param>
        /// <param name="defaultButton"><see cref="MessageBoxDefaultButton"/> with the initial focus. 
        /// <see cref="MessageBoxDefaultButton.Button1"/> will be the first (leftmost) button.
        /// <see cref="MessageBoxDefaultButton.Button2"/> will be the second button.
        /// <see cref="MessageBoxDefaultButton.Button3"/> will be the third button.</param>
        public static void ShowDialog(Control parent, string text, string title,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            EventHandler handler = null,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            // since we allow these to be initiated from any thread,
            // which can die at any time since we don't block,
            // and its death kills the form,
            // rather than make separate methods for on-UI-thread and off-UI-thread,
            // just always run on our own thread
            // which won't die until we are done
            var th = new Thread(() =>
            {
                SimpleMessageBox form = new SimpleMessageBox(text, title, parent, new Point(-1, -1), handler);
                form.FormClosing += (s, e) => Application.ExitThread();

                SetButtons(form, buttons, defaultButton);

                SetIcon(form, icon);

                form.Show();
                form.TopMost = true;
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        /// <summary>
        /// Non-modal shown at the given location with optional buttons, icon, and event
        /// </summary>
        /// <param name="location">Screen coordinates for the placement of the top left corner of the form.</param>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf controls like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        /// <param name="buttons"></param>
        /// <param name="handler">Event handler to be called when the [OK] button is pressed</param>
        /// <param name="icon"><see cref="MessageBoxIcon"/> to be displayed in the dialog. Default is none.</param>
        /// <param name="defaultButton"><see cref="MessageBoxDefaultButton"/> with the initial focus. 
        /// <see cref="MessageBoxDefaultButton.Button1"/> will be the first (leftmost) button.
        /// <see cref="MessageBoxDefaultButton.Button2"/> will be the second button.
        /// <see cref="MessageBoxDefaultButton.Button3"/> will be the third button.</param>
        public static void ShowDialog(Point location, string text, string title,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            EventHandler handler = null,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            var th = new Thread(() =>
            {
                SimpleMessageBox form = new SimpleMessageBox(text, title, null, location, handler);
                form.FormClosing += (s, e) => Application.ExitThread();

                SetButtons(form, buttons, defaultButton);

                SetIcon(form, icon);

                form.Show();
                form.TopMost = true;
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        /// <summary>
        /// Non-modal "shortcut" show in the middle of the screen with an OK button.
        /// </summary>
        /// <param name="text">Rich text message. Can be a simple string or or have rtf controls like "line 1\par line 2"</param>
        /// <param name="title">The title for the form.</param>
        public static void ShowDialog(string text, string title)
        {
            // goes to the ShowDialog(Control, ...) since Point() is not nullable
            ShowDialog(null, text, title);
        }

        #endregion -- Non-Modal --

        #endregion --- Public Show Methods ---

        #region --- Private Methods ---

        /// <summary>
        /// Set the text for the pop up. The text will be horizontally centered in the form unless overridden.
        /// </summary>
        /// <param name="txt">Can be a simple string, rtf group(s), or the entire rtf document</param>
        void SetText(string txt)
        {
            // if the incoming looks like the entire document, just set it
            if(txt.StartsWith(@"{\rtf"))
            {
                richTextBox1.Rtf = txt;
                return;
            }

            // otherwise, merge the incoming with the default document

            //get the empty document already in the RichTextBox
            string rtf = richTextBox1.Rtf;

            //if txt contains a font table, need to merge it with the existing one
            if (txt.Contains(@"{\fonttbl"))
            {
                // find the font table in the richTextBox1.Rtf
                Match match = reFonts.Match(rtf);
                if (match.Success)
                {
                    string existingTable = match.Groups[1].Value;
                    string existingFont = match.Groups[2].Value;
                    //now find the one in txt
                    match = reFonts.Match(txt);
                    if (match.Success)
                    {
                        //update the table in the existing rtf to include fonts from txt
                        string fonts = existingFont + match.Groups[2].Value;
                        string table = @"{\fonttbl" + fonts + "}";
                        rtf = rtf.Replace(existingTable, table);

                        //then remove the table in the incoming txt
                        txt = txt.Replace(match.Groups[1].Value, "");
                    }
                }
            }

            //center the text by default
            string add = @"\qc " + txt + @"\par";

            //need to insert it between the outer {} of the default document
            int end = rtf.LastIndexOf('}');
            string text = rtf.Insert(end, add);
            richTextBox1.Rtf = text;
        }

        /// <summary>
        /// Sets the text and <see cref="DialogResult"/> for each button, and default button for the form.
        /// </summary>
        /// <param name="form"><see cref="SimpleMessageBox"/></param>
        /// <param name="buttons"><see cref="MessageBoxButtons"/> on the form</param>
        /// <param name="defaultButton"><see cref="MessageBoxDefaultButton"/> to set as the default</param>
        static void SetButtons(SimpleMessageBox form, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    form.button3.Text = "Abort";
                    form.button3.DialogResult = DialogResult.Abort;
                    form.button3.Visible = true;
                    form.button1.Text = "Retry";
                    form.button1.DialogResult = DialogResult.Retry;
                    form.button2.Text = "Ignore";
                    form.button2.DialogResult = DialogResult.Ignore;
                    form.button2.Visible = true;
                    SetDefaultButton(form, defaultButton, 3);
                    break;
                case MessageBoxButtons.OK:
                    form.button1.Text = "OK";
                    form.button1.DialogResult = DialogResult.OK;
                    SetDefaultButton(form, defaultButton, 1);
                    break;
                case MessageBoxButtons.OKCancel:
                    form.button1.Text = "OK";
                    form.button1.DialogResult = DialogResult.OK;
                    form.button2.Text = "Cancel";
                    form.button2.DialogResult = DialogResult.Cancel;
                    form.button2.Visible = true;
                    SetDefaultButton(form, defaultButton, 2);
                    break;
                case MessageBoxButtons.RetryCancel:
                    form.button1.Text = "Retry";
                    form.button1.DialogResult = DialogResult.Retry;
                    form.button2.Text = "Cancel";
                    form.button2.DialogResult = DialogResult.Cancel;
                    form.button2.Visible = true;
                    SetDefaultButton(form, defaultButton, 2);
                    break;
                case MessageBoxButtons.YesNo:
                    form.button1.Text = "Yes";
                    form.button1.DialogResult = DialogResult.Yes;
                    form.button2.Text = "No";
                    form.button2.DialogResult = DialogResult.No;
                    form.button2.Visible = true;
                    SetDefaultButton(form, defaultButton, 2);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    form.button3.Text = "Yes";
                    form.button3.DialogResult = DialogResult.Yes;
                    form.button3.Visible = true;
                    form.button1.Text = "No";
                    form.button1.DialogResult = DialogResult.No;
                    form.button2.Text = "Cancel";
                    form.button2.DialogResult = DialogResult.Cancel;
                    form.button2.Visible = true;
                    SetDefaultButton(form, defaultButton, 3);
                    break;
                default:
                    form.button1.Text = "OK";
                    form.button1.DialogResult = DialogResult.OK;
                    SetDefaultButton(form, defaultButton, 1);
                    break;
            }
        }

        /// <summary>
        /// Set the icon for the form.
        /// </summary>
        /// <param name="form"><see cref="SimpleMessageBox"/></param>
        /// <param name="icon"><see cref="MessageBoxIcon"/> for the form</param>
        static void SetIcon(SimpleMessageBox form, MessageBoxIcon icon)
        {
            if (icon != MessageBoxIcon.None)
            {
                //going to put the icon in cell 0,2 (col,row)
                // need to move the textbox to the right by one column to cell 1,0 (col, row)
                form.tableLayoutPanel1.Controls.Add(form.richTextBox1, 1, 0);
                form.tableLayoutPanel1.SetColumnSpan(form.richTextBox1, 3);

                //add the icon
                PictureBox pic = new PictureBox();
                switch (icon)
                {
                    case MessageBoxIcon.Question:
                        pic.Image = SystemIcons.Question.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        pic.Image = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Error:
                        pic.Image = SystemIcons.Error.ToBitmap();
                        break;
                    default:
                        pic.Image = SystemIcons.Information.ToBitmap();
                        break;
                }
                pic.Anchor = AnchorStyles.None;
                form.tableLayoutPanel1.Controls.Add(pic, 0, 2);
            }
        }

        /// <summary>
        /// Sets the default button based on the number of buttons shown and the specified button
        /// </summary>
        /// <param name="form"><see cref="SimpleMessageBox"/> form containing the buttons</param>
        /// <param name="button"><see cref="MessageBoxDefaultButton"/> to set as default</param>
        /// <param name="buttonCount">Number of buttons visible on the form</param>
        static void SetDefaultButton(SimpleMessageBox form, MessageBoxDefaultButton button, int buttonCount)
        {
            if (buttonCount == 1)
                form.ActiveControl = form.button1;
            else if (buttonCount == 2)
            {
                if (button == MessageBoxDefaultButton.Button2)
                    form.ActiveControl = form.button2;
                else
                    form.ActiveControl = form.button1;
            }
            else
            {
                if (button == MessageBoxDefaultButton.Button1)
                    form.ActiveControl = form.button3;
                else if (button == MessageBoxDefaultButton.Button2)
                    form.ActiveControl = form.button1;
                else
                    form.ActiveControl = form.button2;
            }
        }

        /// <summary>
        /// Trigger the callback event, if available
        /// </summary>
        protected virtual void OnButtonPressed(EventArgs e)
        {
            if (buttonEvent != null)
            {
                buttonEvent.Invoke(this, e);
            }
        }

        /// <summary>
        /// When a button is clicked, start the event callback and close the form
        /// </summary>
        private void button_Click(object sender, EventArgs e)
        {
            DialogResult result = ((Button)sender).DialogResult;
            OkEventArgs arg = new OkEventArgs { result = result, formSize = this.Size, formLocation = this.Location };
            OnButtonPressed(arg);
            this.Close();
        }

        /// <summary>
        /// Positions the form to avoid the flicker at its default location.
        /// </summary>
        private void SimpleMessageBox_Load(object sender, EventArgs e)
        {
            bool done = false;
            if (parentControl != null)
            {
                // parent size will not be known if it has not been shown
                if (parentControl.IsHandleCreated)
                {
                    int x = parentControl.Location.X + parentControl.Width / 2 - this.Width / 2;
                    int y = parentControl.Location.Y + parentControl.Height / 2 - this.Height / 2;
                    Point screen = new Point(x, y);
                    this.Location = screen;
                    done = true;
                }
            }
            if (!done && desiredLocation.X >= 0 && desiredLocation.Y >= 0)
            {
                this.Location = desiredLocation;
                done = true;
            }
            if (!done)
            {
                //center screen by default
                Rectangle screen = Screen.FromControl(this).Bounds;
                int x = (screen.Width - this.Width) / 2;
                int y = (screen.Height - this.Height) / 2;
                Point client = new Point(x, y);
                this.Location = client;
            }

            this.TopMost = true;
        }

        /// <summary>
        /// Resize the text box to fit its contents, within limits
        /// </summary>
        /// <remarks>This gets called several times per form instantiation</remarks>
        private void richTextBox1_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            sizePassDebug++;            //debug
            //bool isAdjusted = false;    //debug

            int diff = 0;   //calculated size difference

            //since we are changing the rtf height indirectly
            // by changing the size of the form
            // which changes the tablelayout
            // which changes the richtextbox
            //add some hysteresis
            int pad = 7;

            //Divide the screen height to get a max form height.
            //e.g. divisor of 5 = 20% of the screen = height limit. The scroll bar will show up if needed.
            int maxHeight = Screen.FromControl(this).Bounds.Height / maxSizeDivisor;

            // we get serveral small, or small difference, height calls,
            // so use the pad to minimze "bouncing" between close values.
            if (e.NewRectangle.Height > richTextBox1.Height)
            {
                // change the height of the form, which will change the tablelayout cell sizes
                if (e.NewRectangle.Height < maxHeight)
                {
                    // grow if we have not reached max size
                    diff = (e.NewRectangle.Height - richTextBox1.Height) + pad;
                    if (diff > pad)
                    {
                        this.Height += diff;
                        //isAdjusted = true;  //debug
                    }
                }
                else
                {
                    // set to max size
                    diff = maxHeight - this.Height;
                    if (diff > pad)
                    {
                        this.Height = maxHeight;
                        //isAdjusted = true;  //debug
                    }
                }
            }

            // check for shrinking
            if (e.NewRectangle.Height + pad < richTextBox1.Height)
            {
                diff = richTextBox1.Height - e.NewRectangle.Height - pad;
                if (diff > pad)
                {
                    // minimum size is set in the designer
                    if (this.Height - diff > this.MinimumSize.Height)
                    {
                        this.Height -= diff;
                        //isAdjusted = true;  //debug
                    }
                }
            }

            // this method gets called a lot. Some debug to watch it.
            //Console.WriteLine(string.Format("Exit {sizePassDebug} - newRect height:{e.NewRectangle.Height}, form height:{this.Height}, rtf height:{richTextBox1.Height} Changed:{isAdjusted}, diff:{diff}"));

        }

        /// <summary>
        /// Process hyperlinks
        /// </summary>
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (Exception)
            {
                // leave it up to the user
                throw;
            }
        }

        #endregion --- Private Methods ---
    }
	#endregion SimpleMessageBox.cs
	#region SimpleMessageBox.designer.cs
    partial class SimpleMessageBox
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
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(91, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.button2, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.button3, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(253, 147);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 4);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.tableLayoutPanel1.SetRowSpan(this.richTextBox1, 4);
            this.richTextBox1.Size = new System.Drawing.Size(247, 110);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "";
            this.richTextBox1.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.richTextBox1_ContentsResized);
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.Location = new System.Drawing.Point(174, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tableLayoutPanel1.SetColumnSpan(this.button3, 2);
            this.button3.Location = new System.Drawing.Point(7, 121);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button_Click);
            // 
            // SimpleMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 147);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(16, 107);
            this.Name = "SimpleMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Simple Message Box";
            this.Load += new System.EventHandler(this.SimpleMessageBox_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
	#endregion SimpleMessageBox.designer.cs
	#region Macros.cs

    public class Macros
    {
        public static List<char> invalidMacroChars = new List<char> { '<', '>', '\'', '\"', ';' };
        public static List<string> invalidMacroStrings = new List<string> { @"\#" };

        public static Bitmap GetActionBitmap()
        {
            //use https://littlevgl.com/image-to-c-array to convert a Visual Studio Image Library png
            // to a Raw color format C array
            // so ACT can load it as part of a .cs file
            byte[] png_map = {
                0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
                0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0xf3, 0xff,
                0x61, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e,
                0xc4, 0x01, 0x95, 0x2b, 0x0e, 0x1b, 0x00, 0x00, 0x01, 0x0d, 0x49, 0x44, 0x41, 0x54, 0x78, 0x5e,
                0xed, 0x1d, 0x03, 0x60, 0x23, 0x40, 0x6c, 0xb6, 0x6d, 0xdb, 0xb6, 0x6d, 0xdb, 0xb6, 0x6d, 0x9b,
                0x6f, 0xdb, 0xb6, 0x6d, 0xdb, 0xb6, 0x6d, 0xa3, 0xb6, 0xdd, 0x21, 0x39, 0xdb, 0x06, 0xeb, 0xff,
                0xff, 0xff, 0x59, 0x28, 0x01, 0x36, 0x16, 0x0a, 0x81, 0x03, 0xc4, 0x7c, 0xff, 0xfe, 0x1d, 0x3d,
                0x18, 0x1e, 0x7c, 0x7c, 0x7c, 0x7b, 0xd1, 0x0d, 0x03, 0x43, 0x0b, 0x33, 0x8b, 0x19, 0x82, 0x80,
                0x80, 0x00, 0x30, 0x85, 0xc2, 0x9e, 0x6f, 0xdf, 0xbe, 0xcd, 0x01, 0x52, 0x71, 0x64, 0xc3, 0x68,
                0x66, 0xf1, 0x47, 0x61, 0xea, 0xce, 0x9b, 0xe9, 0x7f, 0xff, 0xfd, 0xbf, 0x09, 0x74, 0x24, 0x15,
                0x48, 0x59, 0x49, 0x4e, 0x83, 0xca, 0x25, 0x67, 0x59, 0x1c, 0x9a, 0x76, 0x88, 0x9f, 0xb9, 0xfb,
                0x76, 0x1e, 0x50, 0x7a, 0x08, 0xe8, 0x88, 0x3e, 0xf6, 0x34, 0xc0, 0x03, 0x17, 0x1f, 0xbe, 0x67,
                0x71, 0x6e, 0xdd, 0xc9, 0x92, 0xe1, 0xa6, 0xe9, 0xd0, 0x16, 0x65, 0x7c, 0x01, 0xa8, 0x34, 0x8b,
                0xe4, 0x5c, 0x00, 0xe5, 0xf2, 0xec, 0xbd, 0xb7, 0x59, 0x8c, 0x2a, 0xb7, 0x70, 0xac, 0x38, 0xf6,
                0x20, 0x8f, 0xe2, 0x6c, 0x24, 0xd9, 0x01, 0x56, 0x60, 0xf2, 0x65, 0xba, 0x6b, 0xb2, 0x5c, 0xea,
                0x0f, 0xf8, 0x13, 0x63, 0xa7, 0x32, 0x8d, 0x24, 0x07, 0x8c, 0x95, 0x45, 0x59, 0x0e, 0x36, 0x7b,
                0xb3, 0x4c, 0x4c, 0xb1, 0x3c, 0x22, 0xcc, 0xc7, 0x65, 0xc2, 0xcb, 0xcb, 0x9b, 0x4f, 0xb4, 0x03,
                0xfd, 0x09, 0xe6, 0x2c, 0x47, 0xda, 0x7c, 0xde, 0x5a, 0xa8, 0x8b, 0xa7, 0x01, 0xa5, 0x4e, 0xc0,
                0x82, 0x75, 0x95, 0xa4, 0x5c, 0xc8, 0xf7, 0xd6, 0x9e, 0x0b, 0xe4, 0xaa, 0x81, 0x16, 0xdf, 0xe2,
                0x2f, 0xca, 0x50, 0xd8, 0xb2, 0x65, 0x0b, 0x4a, 0x51, 0x06, 0xd2, 0xbd, 0x40, 0xcb, 0x68, 0xc6,
                0x31, 0xcd, 0x32, 0xbe, 0x36, 0x02, 0x00, 0x61, 0x34, 0x5a, 0xcd, 0x0b, 0xd0, 0xa5, 0xfd, 0x00,
                0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82,
            };

            Bitmap bmp;
            using (var ms = new MemoryStream(png_map))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        public static Bitmap GetActionNotBitmap()
        {
            byte[] png_map = {
                0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
                0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0xf3, 0xff,
                0x61, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47, 0x42, 0x00, 0xae, 0xce, 0x1c, 0xe9, 0x00, 0x00,
                0x00, 0x04, 0x67, 0x41, 0x4d, 0x41, 0x00, 0x00, 0xb1, 0x8f, 0x0b, 0xfc, 0x61, 0x05, 0x00, 0x00,
                0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e, 0xc4, 0x01, 0x95,
                0x2b, 0x0e, 0x1b, 0x00, 0x00, 0x02, 0x52, 0x49, 0x44, 0x41, 0x54, 0x38, 0x4f, 0x7d, 0x53, 0x5f,
                0x48, 0x53, 0x51, 0x18, 0xff, 0xee, 0xe5, 0x9a, 0xdb, 0x9d, 0x63, 0xc1, 0x84, 0x31, 0x12, 0x8c,
                0x6e, 0xa1, 0x08, 0x2d, 0x67, 0x0a, 0x32, 0x64, 0x3d, 0xf8, 0x90, 0x45, 0x2f, 0x82, 0x0f, 0xbd,
                0x18, 0xce, 0xfe, 0x40, 0xeb, 0xa9, 0x87, 0xde, 0x42, 0x23, 0x90, 0x04, 0x85, 0x30, 0x58, 0x63,
                0x20, 0xe2, 0x43, 0x90, 0xd0, 0x83, 0x04, 0x3d, 0xd8, 0x43, 0x50, 0x36, 0xca, 0x87, 0x45, 0x39,
                0x4c, 0x1b, 0xb1, 0xe1, 0x90, 0x7c, 0x18, 0x4d, 0x0b, 0xe2, 0x3a, 0xd9, 0xee, 0x3d, 0x7e, 0xdf,
                0x39, 0x67, 0xda, 0x1a, 0xf4, 0x83, 0xdf, 0x3d, 0xf7, 0x7c, 0xdf, 0xef, 0xf7, 0xdd, 0x73, 0xb8,
                0xdf, 0xa7, 0x30, 0xc6, 0xa0, 0x0e, 0x8a, 0xe2, 0xc0, 0x67, 0x2f, 0xb2, 0x13, 0x79, 0x1c, 0xf9,
                0x0b, 0xf9, 0x05, 0xb9, 0x02, 0x8c, 0x95, 0x70, 0x3d, 0x84, 0x2a, 0x57, 0x01, 0x32, 0x2a, 0xca,
                0x7d, 0x7c, 0xdb, 0x42, 0x3e, 0x42, 0xb6, 0x52, 0x58, 0xae, 0xb4, 0xdf, 0xc2, 0xfc, 0x03, 0xf9,
                0x01, 0x01, 0x3a, 0x81, 0x69, 0x9a, 0xcc, 0xcc, 0xe5, 0x98, 0xd5, 0xd5, 0xc5, 0x2a, 0x83, 0x83,
                0x6c, 0x3f, 0x1e, 0xbf, 0x26, 0xd3, 0x35, 0xc0, 0xb3, 0x5e, 0xb6, 0x1d, 0x0e, 0x66, 0x05, 0x02,
                0x5c, 0x4f, 0x3e, 0x51, 0xa0, 0x50, 0x60, 0x39, 0x97, 0x8b, 0xc5, 0x0d, 0x83, 0x07, 0x25, 0x67,
                0x91, 0x5e, 0xe9, 0x25, 0x73, 0x33, 0xf2, 0xfb, 0x7e, 0x22, 0xc1, 0x75, 0xa4, 0x27, 0x1f, 0xbf,
                0x82, 0x36, 0x31, 0x01, 0x19, 0xb7, 0x1b, 0x5e, 0xb4, 0x56, 0x4f, 0x0c, 0x10, 0x7b, 0x9d, 0xb9,
                0x6e, 0xd9, 0x2c, 0x83, 0x45, 0x22, 0x56, 0x30, 0xd8, 0x80, 0xa1, 0xe7, 0xc8, 0x25, 0x6b, 0x78,
                0x98, 0xeb, 0x48, 0x4f, 0x3e, 0x15, 0xef, 0xd3, 0xa4, 0xcd, 0xcf, 0xc3, 0xac, 0x61, 0x70, 0x63,
                0x15, 0xf7, 0x9e, 0x7d, 0x82, 0xbe, 0xb1, 0x25, 0x6f, 0x2a, 0x5b, 0x9c, 0xb3, 0xdb, 0xda, 0xf2,
                0xcc, 0xed, 0x76, 0x61, 0xf8, 0xae, 0xc8, 0x02, 0xd7, 0x93, 0x8f, 0x4e, 0x30, 0x60, 0x87, 0x42,
                0xb0, 0xdb, 0xd8, 0x28, 0x32, 0x7f, 0x61, 0x35, 0xbf, 0x0b, 0x73, 0x91, 0x31, 0xd8, 0x79, 0xf3,
                0xde, 0x5f, 0x48, 0x7d, 0xee, 0xd9, 0x33, 0xcd, 0x19, 0x99, 0xe2, 0x7a, 0xf2, 0x51, 0x81, 0x76,
                0x3b, 0x10, 0x10, 0xd1, 0x7f, 0xd0, 0x5b, 0xcc, 0xc2, 0xe3, 0xd5, 0x05, 0xe8, 0x3f, 0x77, 0x0b,
                0xce, 0x4e, 0xae, 0x68, 0x0b, 0x1f, 0x36, 0xa3, 0x32, 0xc5, 0x41, 0x3e, 0x2a, 0xa0, 0x89, 0x6d,
                0x2d, 0x5a, 0xcc, 0x1d, 0x58, 0xfc, 0x18, 0x83, 0xc8, 0xf9, 0x51, 0x58, 0xf3, 0x9c, 0x90, 0xd1,
                0x7a, 0x50, 0x81, 0x35, 0x75, 0x7d, 0x5d, 0xec, 0xaa, 0x28, 0x95, 0xb8, 0x39, 0x71, 0xea, 0x02,
                0xbc, 0x6c, 0x09, 0xc2, 0xcd, 0xfe, 0x33, 0x90, 0x9e, 0xba, 0x52, 0xb9, 0x1a, 0x3a, 0xf9, 0x54,
                0x2a, 0x38, 0xc8, 0x47, 0x05, 0xde, 0xaa, 0xcb, 0xcb, 0xe0, 0x2a, 0x97, 0x45, 0x14, 0xd1, 0x10,
                0x8d, 0xc2, 0xa6, 0xab, 0x19, 0x16, 0x2f, 0x8d, 0xc0, 0xbb, 0xf1, 0x8b, 0x30, 0x33, 0xd2, 0x93,
                0xf4, 0xe8, 0xc7, 0x3a, 0x9d, 0x4e, 0xe7, 0x1d, 0x29, 0xe1, 0x7a, 0xf2, 0xa9, 0xd8, 0x08, 0x3f,
                0xe9, 0xd7, 0xdc, 0xc8, 0x66, 0x79, 0x42, 0x9b, 0x9e, 0x06, 0x35, 0x9d, 0x86, 0x1f, 0x53, 0x4f,
                0x20, 0xf9, 0x70, 0xa0, 0xd8, 0x6d, 0x78, 0x47, 0x31, 0x1c, 0xd6, 0x75, 0xfd, 0x2b, 0x17, 0x48,
                0x90, 0x9e, 0x7c, 0xa2, 0x91, 0xb6, 0xb7, 0x59, 0x5e, 0xd7, 0xd9, 0x2b, 0xbf, 0x9f, 0xd9, 0x3e,
                0x1f, 0xdb, 0xdb, 0xd8, 0xa8, 0x6b, 0x24, 0x42, 0xb5, 0x6b, 0xa9, 0x91, 0x48, 0x4f, 0xbe, 0xa3,
                0x56, 0x46, 0x93, 0xad, 0x69, 0xac, 0x12, 0x0e, 0xf3, 0x56, 0xc6, 0x98, 0xb4, 0x1d, 0x01, 0x3b,
                0xf1, 0x74, 0x79, 0x68, 0x88, 0x59, 0x1d, 0x1d, 0x87, 0xad, 0x5c, 0x3b, 0x8d, 0x8a, 0xe2, 0xc3,
                0xe7, 0x6d, 0x24, 0xdd, 0xf5, 0x1b, 0x32, 0x85, 0xfc, 0x8d, 0xf4, 0x20, 0xbb, 0x91, 0xed, 0xc8,
                0x18, 0x72, 0x12, 0xbf, 0xcc, 0xa7, 0xf2, 0x7f, 0xe3, 0xdc, 0x87, 0xa4, 0x71, 0x6e, 0x42, 0xfe,
                0x41, 0xd2, 0x38, 0x27, 0xab, 0x46, 0x01, 0x80, 0x03, 0xc4, 0x88, 0x26, 0xec, 0x90, 0x8f, 0x67,
                0x89, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82,
            };

            Bitmap bmp;
            using (var ms = new MemoryStream(png_map))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        public static bool IsInvalidMacro(List<string> strings)
        {
            foreach (char invalid in invalidMacroChars)
            {
                foreach (string s in strings)
                {
                    if (s.IndexOf(invalid) >= 0)
                        return true;
                }
            }

            foreach (string invalid in invalidMacroStrings)
            {
                foreach (string s in strings)
                {
                    if (s.Contains(invalid))
                        return true;
                }
            }

            return false; //all strings are valid in a macro
        }

        public static bool IsInvalidMacroTrigger(CustomTrigger trigger)
        {
            List<string> strings = new List<string>();
            strings.Add(trigger.ShortRegexString);
            strings.Add(trigger.Category);
            strings.Add(trigger.SoundData);
            strings.Add(trigger.TimerName);
            return IsInvalidMacro(strings);

        }

        public static bool IsInvalidMacroTimer(TimerData timer)
        {
            List<string> strings = new List<string>();
            strings.Add(timer.Category);
            strings.Add(timer.Name);
            strings.Add(timer.StartSoundData);
            strings.Add(timer.Tooltip);
            strings.Add(timer.WarningSoundData);
            return IsInvalidMacro(strings);
        }

        public static bool IsInvalidMacro(string text)
        {
            List<string> strings = new List<string>();
            strings.Add(text);
            return IsInvalidMacro(strings);
        }
    }
	#endregion Macros.cs
}
