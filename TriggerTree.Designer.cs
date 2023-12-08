using System.Windows.Forms;

namespace ACT_TriggerTree
{
    partial class TriggerTree
    {
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripCat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyZoneNameToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEntireCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableOnZoneinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleEntireCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAllTriggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllTriggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStrip2 = new ACT_TriggerTree.MyToolStrip();
            this.toolStripButtonNewGrp = new System.Windows.Forms.ToolStripButton();
            this.textBoxCatFind = new ACT_TriggerTree.TextBoxX();
            this.toolStrip1 = new ACT_TriggerTree.MyToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInfo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonResults = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAltEncoding = new System.Windows.Forms.ToolStripButton();
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
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            this.treeViewCats.AllowDrop = true;
            this.treeViewCats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCats.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewCats.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewCats.Location = new System.Drawing.Point(0, 33);
            this.treeViewCats.Name = "treeViewCats";
            this.treeViewCats.Size = new System.Drawing.Size(240, 527);
            this.treeViewCats.TabIndex = 1;
            this.treeViewCats.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewCats_AfterLabelEdit);
            this.treeViewCats.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCats_AfterCollapse);
            this.treeViewCats.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCats_AfterExpand);
            this.treeViewCats.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCats_DrawNode);
            this.treeViewCats.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewCats_ItemDrag);
            this.treeViewCats.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCats_AfterSelect);
            this.treeViewCats.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewCats_DragDrop);
            this.treeViewCats.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewCats_DragEnter);
            this.treeViewCats.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewCats_DragOver);
            this.treeViewCats.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewCats_MouseDown);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.toolStrip2);
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
            this.label3.Location = new System.Drawing.Point(35, 8);
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
            this.label4.Location = new System.Drawing.Point(109, 7);
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
            this.groupsayShareMacroToolStripMenuItem});
            this.contextMenuStripTrig.Name = "contextMenuStrip1";
            this.contextMenuStripTrig.Size = new System.Drawing.Size(290, 176);
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
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 750;
            // 
            // contextMenuStripCat
            // 
            this.contextMenuStripCat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyZoneNameToClipboardToolStripMenuItem,
            this.deleteEntireCategoryToolStripMenuItem,
            this.renameGroupToolStripMenuItem,
            this.enableOnZoneinToolStripMenuItem,
            this.toggleEntireCategoryToolStripMenuItem,
            this.toolStripSeparator4,
            this.raidShareCategoryMacroMenuItem,
            this.groupShareCategoryMacroMenuItem,
            this.shareDialogMenuItem,
            this.toolStripSeparator7,
            this.categorySpellTimersMenuItem});
            this.contextMenuStripCat.Name = "contextMenuStrip2";
            this.contextMenuStripCat.Size = new System.Drawing.Size(252, 214);
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
            // renameGroupToolStripMenuItem
            // 
            this.renameGroupToolStripMenuItem.Name = "renameGroupToolStripMenuItem";
            this.renameGroupToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.renameGroupToolStripMenuItem.Text = "Rename Group";
            this.renameGroupToolStripMenuItem.Click += new System.EventHandler(this.renameGroupToolStripMenuItem_Click);
            // 
            // enableOnZoneinToolStripMenuItem
            // 
            this.enableOnZoneinToolStripMenuItem.Name = "enableOnZoneinToolStripMenuItem";
            this.enableOnZoneinToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.enableOnZoneinToolStripMenuItem.Text = "Enable on zone-in";
            this.enableOnZoneinToolStripMenuItem.ToolTipText = "Upon zone-in, unrestrict enabled triggers";
            this.enableOnZoneinToolStripMenuItem.Click += new System.EventHandler(this.enableOnZoneinToolStripMenuItem_Click);
            // 
            // toggleEntireCategoryToolStripMenuItem
            // 
            this.toggleEntireCategoryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableAllTriggersToolStripMenuItem,
            this.disableAllTriggersToolStripMenuItem});
            this.toggleEntireCategoryToolStripMenuItem.Name = "toggleEntireCategoryToolStripMenuItem";
            this.toggleEntireCategoryToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.toggleEntireCategoryToolStripMenuItem.Text = "Modify entire category";
            // 
            // enableAllTriggersToolStripMenuItem
            // 
            this.enableAllTriggersToolStripMenuItem.Name = "enableAllTriggersToolStripMenuItem";
            this.enableAllTriggersToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.enableAllTriggersToolStripMenuItem.Text = "Enable all triggers";
            this.enableAllTriggersToolStripMenuItem.Click += new System.EventHandler(this.enableAllTriggersToolStripMenuItem_Click);
            // 
            // disableAllTriggersToolStripMenuItem
            // 
            this.disableAllTriggersToolStripMenuItem.Name = "disableAllTriggersToolStripMenuItem";
            this.disableAllTriggersToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.disableAllTriggersToolStripMenuItem.Text = "Disable all triggers";
            this.disableAllTriggersToolStripMenuItem.Click += new System.EventHandler(this.disableAllTriggersToolStripMenuItem_Click);
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
            this.label1.Size = new System.Drawing.Size(541, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Right-click a category or trigger for menu choices.  Click the [+] button in the " +
    "triggers pane to create a new trigger.";
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
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewGrp});
            this.toolStrip2.Location = new System.Drawing.Point(4, 5);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(26, 25);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonNewGrp
            // 
            this.toolStripButtonNewGrp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonNewGrp.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonNewGrp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewGrp.Name = "toolStripButtonNewGrp";
            this.toolStripButtonNewGrp.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNewGrp.Text = "+";
            this.toolStripButtonNewGrp.ToolTipText = "Add Group";
            this.toolStripButtonNewGrp.Click += new System.EventHandler(this.toolStripButtonNewGrp_Click);
            // 
            // textBoxCatFind
            // 
            this.textBoxCatFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCatFind.ButtonTextClear = true;
            this.textBoxCatFind.Location = new System.Drawing.Point(69, 4);
            this.textBoxCatFind.Name = "textBoxCatFind";
            this.textBoxCatFind.Size = new System.Drawing.Size(119, 20);
            this.textBoxCatFind.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxCatFind, "Incremental search in the category name");
            this.textBoxCatFind.TextChanged += new System.EventHandler(this.textBoxCatScroll_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonInfo,
            this.toolStripButtonResults,
            this.toolStripButtonAltEncoding});
            this.toolStrip1.Location = new System.Drawing.Point(4, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(133, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonNew.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNew.Text = "+";
            this.toolStripButtonNew.ToolTipText = "Add New Trigger";
            this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNewTrigger_Click);
            // 
            // toolStripButtonInfo
            // 
            this.toolStripButtonInfo.CheckOnClick = true;
            this.toolStripButtonInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonInfo.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.toolStripButtonInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInfo.Name = "toolStripButtonInfo";
            this.toolStripButtonInfo.Size = new System.Drawing.Size(25, 22);
            this.toolStripButtonInfo.Text = "i";
            this.toolStripButtonInfo.ToolTipText = "Toggle expand-all / collapse-all";
            this.toolStripButtonInfo.Click += new System.EventHandler(this.toolStripButtonInfo_Click);
            // 
            // toolStripButtonResults
            // 
            this.toolStripButtonResults.CheckOnClick = true;
            this.toolStripButtonResults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonResults.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.toolStripButtonResults.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonResults.Name = "toolStripButtonResults";
            this.toolStripButtonResults.Size = new System.Drawing.Size(25, 22);
            this.toolStripButtonResults.Text = "2";
            this.toolStripButtonResults.ToolTipText = "Enable Results Tabs Popup";
            this.toolStripButtonResults.Click += new System.EventHandler(this.toolStripButtonResults_Click);
            // 
            // toolStripButtonAltEncoding
            // 
            this.toolStripButtonAltEncoding.CheckOnClick = true;
            this.toolStripButtonAltEncoding.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAltEncoding.Font = new System.Drawing.Font("Wingdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.toolStripButtonAltEncoding.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAltEncoding.Name = "toolStripButtonAltEncoding";
            this.toolStripButtonAltEncoding.Size = new System.Drawing.Size(26, 22);
            this.toolStripButtonAltEncoding.Text = "+";
            this.toolStripButtonAltEncoding.Click += new System.EventHandler(this.toolStripButtonAltEncoding_Click);
            // 
            // textBoxTrigFind
            // 
            this.textBoxTrigFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTrigFind.ButtonTextClear = true;
            this.textBoxTrigFind.Location = new System.Drawing.Point(143, 4);
            this.textBoxTrigFind.Name = "textBoxTrigFind";
            this.textBoxTrigFind.Size = new System.Drawing.Size(223, 20);
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
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private Panel panel1;
        private Button buttonFindNext;
        private TextBoxX textBoxTrigFind;
        private Panel panel2;
        private Panel panel3;
        private TextBoxX textBoxCatFind;
        private Button buttonCatFindNext;
        private ToolStripMenuItem playAlertSoundToolStripMenuItem;
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
        private MyToolStrip toolStrip1;
        private ToolStripButton toolStripButtonNew;
        private ToolStripButton toolStripButtonResults;


        #endregion

        private ToolStripMenuItem toggleEntireCategoryToolStripMenuItem;
        private ToolStripMenuItem enableAllTriggersToolStripMenuItem;
        private ToolStripMenuItem disableAllTriggersToolStripMenuItem;
        private ToolStripMenuItem enableOnZoneinToolStripMenuItem;
        private MyToolStrip toolStrip2;
        private ToolStripButton toolStripButtonNewGrp;
        private ToolStripMenuItem renameGroupToolStripMenuItem;
        private ToolStripButton toolStripButtonAltEncoding;
        private ToolStripButton toolStripButtonInfo;
    }
}

