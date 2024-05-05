using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
// reference:System.Windows.Forms.DataVisualization.dll
// reference:System.Core.dll
// reference:System.Linq.dll


[assembly: AssemblyTitle("Tree view of Custom Triggers")]
[assembly: AssemblyDescription("An alternate interface for managing Custom Triggers")]
[assembly: AssemblyCompany("Mineeme of Maj'Dul")]
[assembly: AssemblyVersion("1.7.1.0")]

namespace ACT_TriggerTree
{
    public partial class TriggerTree : UserControl, IActPluginV1
	{

        //trigger dictionary - list of triggers by Category name
        Dictionary<string, List<CustomTrigger>> treeDict;

        //zone
        string zoneName = string.Empty;             //most recent zone name from the log file
        string decoratedZoneName = string.Empty;    //includes color and instance #
        public static Regex reCleanActZone = new Regex(@"(?::.+?:)?(?<decoration>\\#[0-9A-F]{6})?(?<zone>.+?)(?<instance> \d+)?$", RegexOptions.Compiled);
        WindowsFormsSynchronizationContext mUiContext = new WindowsFormsSynchronizationContext();
        System.Timers.Timer zoneTimer = new System.Timers.Timer(1000);

        //category groups support
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        UInt32 WM_VSCROLL = 277;
        public static string defaultGroupName = " Default Group";
        enum CatGrpEditType { NONE, ADD_NEW, RENAME_OLD }
        CatGrpEditType catGrpEditType = CatGrpEditType.NONE;
        internal class TriggerGrp { public CustomTrigger trigger; public string groupName; }

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

        static Color activeBackground = Color.LightGreen;  //background for a category that contains active triggers
        Brush activeBackgroundBrush = new SolidBrush(activeBackground);
        static Color inactiveBackground = Color.White;     //background for a category with only inactive triggers
        Brush inactiveBackgroundBrush = new SolidBrush(inactiveBackground);
        Color foundBackground = Color.Gold;         //background for a found trigger
        Color notFoundBackground = Color.White;     //background for triggers
        Color activeColor = Color.Green;            //trigger that will alert if seen in the log
        Color inactiveColor = Color.Black;          //trigger that is enabled, but we are not in the restricted zone, so will not alert
        Color disabledColor = Color.Gray;           //trigger that will never alert

        TreeNode selectedTriggerNode = null;        //node selected via mouse click
        TreeNode clickedCategoryNode = null;
        Point whereTrigMouseDown;                   //screen location for the trigger tree context menu
        Point whereCatMouseDown;
        List<XmlCopyForm> openShareDialogs = new List<XmlCopyForm>(); //track open dialogs
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

        Label lblStatus;                            // save a reference to the status label that appears in ACT's Plugin tab

        // settings
        string settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\TriggerTree.config.xml");
        XmlSerializer xmlSerializer;
        Config config;

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

            toolStripButtonAltEncoding.ToolTipText = "Enable alternate encoding so all triggers can go in a macro.\nRecipients must be using TriggerTree.";

            PopulateCatsTree(null);

            zoneTimer.Elapsed += ZoneTimer_Elapsed;
            zoneTimer.SynchronizingObject = ActGlobals.oFormActMain;
            zoneTimer.Enabled = true;
            zoneTimer.Start();

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
                        + @"\line\line{\b This release adds another level to the category tree "
                        + @"\line to help organize categories. All existing categories "
                        + @"will be moved to the {\ul Default Group} folder.}"
                        + @"\line\line If there is an update to ACT"
                        + @"\line you should click No and update ACT first."
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

        private void ZoneTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ActGlobals.oFormActMain.CurrentZone) && ActGlobals.oFormActMain.CurrentZone != decoratedZoneName)
            {
                Match match = reCleanActZone.Match(ActGlobals.oFormActMain.CurrentZone);
                if (match.Success)
                {
                    bool isDecorated = !string.IsNullOrEmpty(match.Groups["decoration"].Value) || !string.IsNullOrEmpty(match.Groups["instance"].Value);
                    if(isDecorated)
                    {
                        decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                        string cleanZoneName = match.Groups["zone"].Value.Trim();
                        if (config.autoCats.Contains(cleanZoneName))
                            zoneName = cleanZoneName;
                        else
                            zoneName = decoratedZoneName;
                    }
                    else
                        zoneName = decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                }
                else
                    zoneName = decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                mUiContext.Post(CheckAutoRestrict, null);
                mUiContext.Post(UpdateTriggerColors, null);
                mUiContext.Post(UpdateCategoryColors, true);
            }
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
            else if(e.ShareType == "TrigTree")
            {
                // it's one of our triggers encoded to go into an EQII macro
                CustomTrigger trigger = Macros.TriggerFromMacro(e.RawXml);
                if(trigger != null)
                {
                    if(Macros.EnableOnZoneIn && !config.autoCats.Contains(trigger.Category))
                        config.autoCats.Add(trigger.Category);
                    TriggerGrp tg = new TriggerGrp { trigger = trigger, groupName = Macros.GroupName };
                    mUiContext.Post(UiPostEncodedTrigger, tg);
                    e.Handled = true;
                }
            }
            else if(e.ShareType == "SpellTT")
            {
                // it's one of our timers encoded to go into an EQII macro
                TimerData td = Macros.SpellTimerFromMacro(e.RawXml);
                if(td != null)
                {
                    ActGlobals.oFormSpellTimers.AddEditTimerDef(td);
                    ActGlobals.oFormSpellTimers.RebuildSpellTreeView();
                    e.Handled = true;
                }
            }
        }

        private void UiPostEncodedTrigger(object o)
        {
            TriggerGrp tg = o as TriggerGrp;
            if(o != null)
            {
                if (!string.IsNullOrEmpty(tg.groupName))
                    config.catGroupings.AddCatIfNotGrouped(tg.groupName, tg.trigger.Category);
                ActGlobals.oFormActMain.AddEditCustomTrigger(tg.trigger);
                if (tg.trigger.Tabbed)
                    UpdateResultsTab(tg.trigger);

                PopulateCatsTree(null);
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
                    toolStripButtonAltEncoding.Checked = config.AlternateEncoding;
                    Macros.AlternateEncoding = config.AlternateEncoding;
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

            config.catGroupings.Sort((x, y) => x.GroupName.CompareTo(y.GroupName));
            foreach (ConfigCatGroup grp in config.catGroupings)
                treeViewCats.Nodes.Add(grp.GroupName, grp.GroupName);

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
                        string group = config.catGroupings.GetGroupName(category);
                        if (string.IsNullOrEmpty(group))
                        {
                            group = defaultGroupName;
                            config.catGroupings.AddCatIfNotGrouped(group, category);
                        }
                        
                        treeDict.Add(category, new List<CustomTrigger>());

                        //add the category to the tree
                        TreeNode[] tn = treeViewCats.Nodes.Find(group, false);
                        if (tn.Length > 0)
                        {
                            TreeNode node = tn[0].Nodes.Add(category);
                            node.ImageIndex = 0;    //closed folder
                            node.Name = category;   //for .Find()
                        }
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
                                foreach(Control ctrl2 in ctrl.Controls)
                                {
                                    if (ctrl2.GetType() == typeof(Button))
                                    {
                                        Button button = (Button)ctrl2;
                                        if (button.Text == "Add/Edit")
                                        {
                                            addEditButton = button;
                                            addEditButton.Click += AddEditButton_Click;
                                        }
                                        else if (button.Text == "&Remove")
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
            }

            // use persistent setting to expand/collapse group nodes
            foreach(TreeNode ttn in treeViewCats.Nodes)
            {
                ConfigCatGroup ccg = config.catGroupings[ttn.Text];
                if(ccg != null)
                {
                    if (ccg.Collapsed)
                        ttn.Collapse();
                    else
                        ttn.Expand();
                }
            }

            // make sure all categories in the config.catGroupings list still exist
            // (e.g. cleanup if an entire category was deleted via the Custom Triggers tab)
            foreach (ConfigCatGroup grp in config.catGroupings)
            {
                int count = grp.Cats.Count;
                for(int i = count-1; i >= 0; i--)
                {
                    string cat = grp.Cats[i];
                    if(!treeDict.ContainsKey(cat))
                    {
                        grp.Cats.Remove(cat);
                    }
                }
            }

            // same cleanup for auto-zone categories
            int acount = config.autoCats.Count;
            for(int i = acount-1; i >= 0; i--)
            {
                string cat = config.autoCats[i];
                if (!treeDict.ContainsKey(cat))
                    config.autoCats.Remove(cat);
            }

            mUiContext.Post(UpdateCategoryColors, false);

            //try to restore previous selection
            if (!string.IsNullOrEmpty(prevCat))
            {
                TreeNode[] nodes = treeViewCats.Nodes.Find(prevCat, true);
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
            List<TreeNode> allnodes = EnumerateAllTreeNodes<TreeNode>(treeViewCats);
            foreach (TreeNode category in allnodes)
            {
                if (category.Level == 0)
                {
                    // the level 0 nodes will all be at the top of the list
                    // so just set them not-green for now
                    category.BackColor = inactiveBackground; //color it right now
                    category.Tag = false;                    //color it in _DrawNode()

                    continue; // green is handled at child level
                }

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
                            if (trigger.RestrictToCategoryZone == false 
                                || key.Equals(zoneName)
                                || key.Equals(decoratedZoneName))
                            {
                                category.BackColor = activeBackground;
                                category.Tag = true; //active-triggers-in-this-category flag for treeViewCats__DrawNode()
                                if (category.Parent != null)
                                {
                                    if (category.Parent.IsExpanded == false)
                                    {                                    
                                        // color a collpased group name whose group of categories contain active triggers
                                        category.Parent.BackColor = activeBackground;
                                        category.Parent.Tag = true;
                                    }
                                    else
                                    {
                                        category.Parent.BackColor = inactiveBackground;
                                        category.Parent.Tag = false;
                                    }
                                }
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

        List<T> EnumerateAllTreeNodes<T>(TreeView tree, T parentNode = null) where T : TreeNode
        {
            if (parentNode != null && parentNode.Nodes.Count == 0)
                return new List<T>() { };

            TreeNodeCollection nodes = parentNode != null ? parentNode.Nodes : tree.Nodes;
            List<T> childList = nodes.Cast<T>().ToList();

            List<T> result = new List<T>();
            result.AddRange(childList);

            //Recursion on each child node
            childList.ForEach(n => result.AddRange(EnumerateAllTreeNodes(tree, n)));

            return result;
        }

        private void treeViewCats_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            var font = e.Node.NodeFont ?? e.Node.TreeView.Font;

            // the .Bounds seems a bit too wide, measure with whatever font we ended up with
            SizeF size = e.Graphics.MeasureString(e.Node.Text, font);
            Rectangle rect = e.Bounds;
            rect.Width = (int)Math.Ceiling(size.Width);

            // if treeview's HideSelection property is "True", 
            // this will always returns "False" on unfocused treeview
            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            bool green = e.Node.Tag == null ? false : (bool)e.Node.Tag;

            // keep the focused highlight if selected regardless of focus
            // draw green background if not selected and triggers are active
            // otherwise, default colors
            if (selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, rect);
                e.Graphics.DrawString(e.Node.Text, font, Brushes.White, new Point(rect.X, rect.Y));
            }
            else if (!selected && green)
            {
                e.Graphics.FillRectangle(activeBackgroundBrush, rect);
                e.Graphics.DrawString(e.Node.Text, font, Brushes.Black, new Point(rect.X, rect.Y));
            }
            else
            {
                e.Graphics.FillRectangle(inactiveBackgroundBrush, rect);
                e.Graphics.DrawString(e.Node.Text, font, Brushes.Black, new Point(rect.X, rect.Y));
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
                toolStripButtonInfo.Checked = false;
            }
        }

        private void deleteEntireCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                if (clickedCategoryNode.Level == 0)
                {
                    // move any categories under this group to the default group
                    string categoryGroup = clickedCategoryNode.Text;
                    try
                    {
                        ConfigCatGroup defGrp = config.catGroupings[defaultGroupName];
                        ConfigCatGroup deletedGrp = config.catGroupings[categoryGroup];
                        defGrp.Cats.AddRange(deletedGrp.Cats);
                        config.catGroupings.Remove(deletedGrp);
                    }
                    catch { }
                    PopulateCatsTree(null);
                }
                else
                {
                    string category = clickedCategoryNode.Text;
                    if (SimpleMessageBox.Show(ActGlobals.oFormActMain, @"Delete category \b " + category + @"\b0  and all its triggers?", "Are you sure?",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string grpName = config.catGroupings.GetGroupName(category);
                        ConfigCatGroup catGrp = config.catGroupings[grpName];
                        if (catGrp != null)
                            catGrp.Cats.Remove(category);

                        List<CustomTrigger> triggers;
                        if (treeDict.TryGetValue(category, out triggers))
                        {
                            foreach (CustomTrigger trigger in triggers)
                            {
                                DeleteTrigger(trigger, true, false);
                            }
                            PopulateCatsTree(null);
                            TreeNode sel = treeViewCats.SelectedNode;
                            if(sel == null && treeViewCats.Nodes.Count > 0)
                                treeViewCats.SelectedNode = treeViewCats.Nodes[0];
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
                List<TreeNode> allnodes = EnumerateAllTreeNodes<TreeNode>(treeViewCats);
                foreach (TreeNode node in allnodes)
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
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, String.Format(@"\b {0}\b0\line Not found", find), "Not Found");
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
                List<string> groupedCats = new List<string>();
                string categoryGroupName;

                if (clickedCategoryNode.Level == 0)
                {
                    // it's a group name
                    categoryGroupName = clickedCategoryNode.Text;
                    foreach (TreeNode node in clickedCategoryNode.Nodes)
                    {
                        groupedCats.Add(node.Text);
                    }
                }
                else
                {
                    // single category
                    categoryGroupName = clickedCategoryNode.Parent.Text;
                    groupedCats.Add(clickedCategoryNode.Text);
                }

                List<CustomTrigger> allTriggers = new List<CustomTrigger>();
                foreach(string category in groupedCats)
                {
                    List<CustomTrigger> triggers;
                    if (treeDict.TryGetValue(category, out triggers))
                    {
                        allTriggers.AddRange(triggers);
                    }
                }
                Macros.WriteCategoryMacroFile(sayCmd, allTriggers, categoryTimers, config, categoryGroupName, true);
            }
        }

        private void contextMenuStripCat_Opening(object sender, CancelEventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                int canMacroTimer = 0;
                int cannotMacroTimer = 0;
                categorySpellTimersMenuItem.DropDownItems.Clear();
                List<string> groupedCats = new List<string>();
                categoryTimers = new List<TimerData>();
                int valid = 0;
                int invalid = 0;
                int active = 0;
                int inactive = 0;
                catGrpEditType = CatGrpEditType.NONE;
                Macros.AlternateEncoding = config.AlternateEncoding;

                if (clickedCategoryNode.Level == 0)
                {
                    // it's a group name
                    deleteEntireCategoryToolStripMenuItem.Text = "Delete Category Group";
                    deleteEntireCategoryToolStripMenuItem.ToolTipText = "Moves child categories the the Default Group";
                    renameGroupToolStripMenuItem.Visible = true;
                    enableOnZoneinToolStripMenuItem.Visible = false;
                    toggleEntireCategoryToolStripMenuItem.Visible = false;
                    foreach (TreeNode node in clickedCategoryNode.Nodes)
                    {
                        groupedCats.Add(node.Text);
                    }
                    if(groupedCats.Count == 0)
                    {
                        groupShareCategoryMacroMenuItem.Text = string.Format(catMacroText, "Groupsay", 0, 0);
                        groupShareCategoryMacroMenuItem.Enabled = false;
                        raidShareCategoryMacroMenuItem.Text = string.Format(catMacroText, "Raidsay", 0, 0);
                        raidShareCategoryMacroMenuItem.Enabled = false;
                        groupShareCategoryMacroMenuItem.ToolTipText =
                            string.Format(invalidCategoryText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                        raidShareCategoryMacroMenuItem.ToolTipText =
                            string.Format(invalidCategoryText, string.Join(" ", Macros.invalidMacroChars), string.Join(" ", Macros.invalidMacroStrings));
                    }
                }
                else
                {
                    // single category
                    deleteEntireCategoryToolStripMenuItem.Text = "Delete Entire Category";
                    deleteEntireCategoryToolStripMenuItem.ToolTipText = "Delete the category and all its triggers";
                    enableOnZoneinToolStripMenuItem.Visible = true;
                    toggleEntireCategoryToolStripMenuItem.Visible = true;
                    renameGroupToolStripMenuItem.Visible = false;
                    groupedCats.Add(clickedCategoryNode.Text);
                }

                foreach(string category in groupedCats)
                {
                    //get tagged spell timers for the category
                    categoryTimers.AddRange(FindCategoryTimers(category));

                    //count triggers for the menu text
                    List<CustomTrigger> triggers;
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
            if (clickedCategoryNode != null)
            {
                List<string> groupedCats = new List<string>();
                string groupName;
                string prefix = "/g ";

                if (clickedCategoryNode.Level == 0)
                {
                    // it's a group name
                    groupName = clickedCategoryNode.Text;
                    foreach (TreeNode node in clickedCategoryNode.Nodes)
                    {
                        groupedCats.Add(node.Text);
                    }
                }
                else
                {
                    // single category
                    groupName = clickedCategoryNode.Parent.Text;
                    groupedCats.Add(clickedCategoryNode.Text);
                    if (clickedCategoryNode.Text.Contains("["))
                    {
                        if (clickedCategoryNode.Text.Contains("Raid"))
                            prefix = "/r ";
                    }
                }

                List<CustomTrigger> allTriggers = new List<CustomTrigger>();
                foreach (string category in groupedCats)
                {
                    List<CustomTrigger> triggers;
                    if (treeDict.TryGetValue(category, out triggers))
                    {
                        allTriggers.AddRange(triggers);
                    }
                }
                XmlCopyForm form = new XmlCopyForm(prefix, categoryTimers, allTriggers, config, groupName);
                // remember open dialogs so changing the alt-encoding can be processed
                openShareDialogs.Add(form);
                form.FormClosed += (s, ev) => { openShareDialogs.Remove(form); };
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
                if (treeViewCats.SelectedNode != null)
                {
                    if (catName == treeViewCats.SelectedNode.Text)
                        UpdateTriggerList(catName);
                }
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
                }
                else
                {
                    config.autoCats.Add(category);
                    enableOnZoneinToolStripMenuItem.Checked = true;
                    // in case we are in the zone just enabled
                    Match match = reCleanActZone.Match(ActGlobals.oFormActMain.CurrentZone);
                    if (match.Success)
                    {
                        decoratedZoneName = ActGlobals.oFormActMain.CurrentZone;
                        string cleanZoneName = match.Groups["zone"].Value.TrimEnd();
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

        private void treeViewCats_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if(e.Label != null && e.Label.Length > 0)
            {
                List<TreeNode> allnodes = EnumerateAllTreeNodes<TreeNode>(treeViewCats);
                //ConfigCatGroup labelGrp = config.catGroupings.FirstOrDefault(x => x.GroupName.ToLower() == e.Label.ToLower());
                TreeNode matchingNodes = allnodes.FirstOrDefault(x => x.Text.ToLower() == e.Label.ToLower());
                if (catGrpEditType == CatGrpEditType.ADD_NEW)
                {
                    if (matchingNodes == null)
                    {
                        e.Node.EndEdit(false);
                        treeViewCats.LabelEdit = false;
                        config.catGroupings.AddGroup(e.Label);
                        mUiContext.Post(PopulateCatsTree, null);
                    }
                    else
                    {
                        e.CancelEdit = true;
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, "Duplicate folder names are not allowed", "New Node");
                        e.Node.EndEdit(true);
                        e.Node.Remove();
                    }
                }
                else if(catGrpEditType == CatGrpEditType.RENAME_OLD)
                {
                    if (matchingNodes != null && e.Node.Text != e.Label)
                    {
                        // we already have a group with this name
                        e.CancelEdit = true;
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, "Duplicate folder names are not allowed", "Node Edit");
                    }
                    else
                    {
                        ConfigCatGroup existingGrp = config.catGroupings[e.Node.Text];
                        if(existingGrp != null)
                            existingGrp.GroupName = e.Label;
                        e.Node.EndEdit(false);
                        treeViewCats.LabelEdit = false;
                        mUiContext.Post(PopulateCatsTree, null);
                    }
                }
            }
            else
            {
                // this is an abort (user probably hit ESC), just stop
                e.CancelEdit = true;
                if(catGrpEditType == CatGrpEditType.ADD_NEW)
                {
                    if(e.Node != null)
                        e.Node.Remove();
                }
            }
        }

        private void treeViewCats_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = e.Item as TreeNode;
            if (node != null)
            {
                if (node.Level == 0)
                    return; // can't drag group
               DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void treeViewCats_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void treeViewCats_DragOver(object sender, DragEventArgs e)
        {
            TreeView tv = sender as TreeView;

            // Retrieve the client coordinates of the drop location.
            Point targetPoint = tv.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = tv.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (draggedNode == null || targetNode == null || draggedNode.Equals(targetNode))
                e.Effect = DragDropEffects.None;

            else if (targetNode.Level == 0 && !draggedNode.Parent.Equals(targetNode))
                e.Effect = DragDropEffects.Move;

            else if (targetNode.Parent != null && !draggedNode.Parent.Equals(targetNode.Parent))
                e.Effect = DragDropEffects.Move;

            else
                e.Effect = DragDropEffects.None;

            if ((targetPoint.Y + 20) > tv.Height)
            {
                // scroll down
                SendMessage(tv.Handle, WM_VSCROLL, (IntPtr)1, (IntPtr)0);
            }
            else if (targetPoint.Y < 20)
            {
                // scroll up
                SendMessage(tv.Handle, WM_VSCROLL, (IntPtr)0, (IntPtr)0);
            }
        }

        private void treeViewCats_DragDrop(object sender, DragEventArgs e)
        {
            TreeView tv = sender as TreeView;

            // Retrieve the client coordinates of the drop location.
            Point targetPoint = tv.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = tv.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (draggedNode == null || targetNode == null || draggedNode.Equals(targetNode))
                return;

            // only allowing dragging between groups
            // the nodes must have different level 0 nodes
            if ((targetNode.Parent != null && draggedNode.Parent != null && !draggedNode.Parent.Equals(targetNode.Parent))
                || (targetNode.Level == 0 && !draggedNode.Parent.Equals(targetNode)))
            {
                string sourceGroup = draggedNode.Parent.Text;
                string destGroup = targetNode.Level == 0 ? targetNode.Text : targetNode.Parent.Text;
                config.catGroupings.PutCatInGroup(destGroup, draggedNode.Text);
                PopulateCatsTree(null);
                SaveSettings();
            }
        }

        private void toolStripButtonNewGrp_Click(object sender, EventArgs e)
        {
            TreeNode add = new TreeNode("New Group");
            treeViewCats.Nodes.Add(add);
            treeViewCats.SelectedNode = add;
            treeViewCats.LabelEdit = true;
            catGrpEditType = CatGrpEditType.ADD_NEW;
            add.BeginEdit();
        }

        private void renameGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedCategoryNode != null)
            {
                treeViewCats.LabelEdit = true;
                catGrpEditType = CatGrpEditType.RENAME_OLD;
                clickedCategoryNode.BeginEdit();
            }
        }

        private void treeViewCats_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Level == 0)
            {
                config.catGroupings[e.Node.Text].Collapsed = true;
                mUiContext.Post(UpdateCategoryColors, false);
            }
        }

        private void treeViewCats_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                config.catGroupings[e.Node.Text].Collapsed = false;
                mUiContext.Post(UpdateCategoryColors, false);
            }
        }

        #endregion Category Tree

        #region --------------- Triggers Panel

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
                        TreeNode[] nodes = treeViewCats.Nodes.Find(zoneName, true);
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
                    ok = DeleteTrigger(args.orignalTrigger, true, false);
                }

                //new / edited trigger
                // If the regex or category was changed, this is required to update the dictionary
                ActGlobals.oFormActMain.AddEditCustomTrigger(args.editedTrigger);
                if (args.editedTrigger.Tabbed)
                    UpdateResultsTab(args.editedTrigger);

                PopulateCatsTree(null);

                if (args.result == FormEditTrigger.EventResult.CREATE_NEW || args.result == FormEditTrigger.EventResult.REPLACE_TRIGGER)
                {
                    if (args.editedTrigger.Category != origCategory)
                    {
                        //change the selected node to the new category
                        TreeNode[] nodes = treeViewCats.Nodes.Find(args.editedTrigger.Category, true);
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
                    DeleteTrigger(trigger, false, true);
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
                TreeNode[] cats = treeViewCats.Nodes.Find(category, true);
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
                    SimpleMessageBox.Show(ActGlobals.oFormActMain, String.Format(@"\b {0}\b0\line  not found", find), "Not Found");
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
            string category = ActGlobals.oFormActMain.CurrentZone;
            if(treeViewCats.SelectedNode != null && treeViewCats.SelectedNode.Level != 0)
                category = treeViewCats.SelectedNode.Text;

            CustomTrigger trigger = new CustomTrigger("new expression", category);
            //set restrict if it kinda looks like a zone name
            trigger.RestrictToCategoryZone = category.Contains("[");
            FormEditTrigger formEditTrigger = new FormEditTrigger(trigger, category, config);
            formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
            formEditTrigger.haveOriginal = false; //disable the replace button since there is nothing to replace
            formEditTrigger.catDict = treeDict;
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
                if (selectedTriggerNode.Parent == null && treeViewCats.SelectedNode != null)
                {
                    FormEditTrigger formEditTrigger = new FormEditTrigger(selectedTriggerNode.Tag as CustomTrigger, treeViewCats.SelectedNode.Text, config);
                    formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
                    formEditTrigger.catDict = treeDict;
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

        private void toolStripButtonNewTrigger_Click(object sender, EventArgs e)
        {
            FormEditTrigger formEditTrigger = NewTrigger();
            Point center = new Point(treeViewTrigs.Right/2 - formEditTrigger.Width/2, treeViewTrigs.Bottom/2 - formEditTrigger.Height/2);
            PositionChildForm(formEditTrigger, treeViewTrigs.PointToScreen(center));
        }

        private void toolStripButtonResults_Click(object sender, EventArgs e)
        {
            config.ResultsPopup = toolStripButtonResults.Checked;
        }

        private void toolStripButtonAltEncoding_Click(object sender, EventArgs e)
        {
            config.AlternateEncoding = toolStripButtonAltEncoding.Checked;
            Macros.AlternateEncoding = config.AlternateEncoding;
            if (treeViewCats.SelectedNode != null && treeViewCats.SelectedNode.Level != 0)
            {
                string category = treeViewCats.SelectedNode.Text;
                UpdateTriggerList(category);
            }
            // update any open share dialogs
            foreach (XmlCopyForm f in openShareDialogs)
                f.AltEncodingChanged();
        }
        
        private void toolStripButtonInfo_Click(object sender, EventArgs e)
        {
            ToolStripButton tsb = sender as ToolStripButton;

            // seems necessary for ExpandAll() to work
            isDoubleClick = false;

            if (tsb != null)
            {
                if (tsb.Checked)
                    treeViewTrigs.ExpandAll();
                else
                    treeViewTrigs.CollapseAll();
            }
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
                    DeleteTrigger(trigger, false, true);
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
                    FormEditTrigger formEditTrigger = new FormEditTrigger(t, treeViewCats.SelectedNode.Text, config);
                    formEditTrigger.EditDoneEvent += Trigger_EditDoneEvent; //callback for when the edit is done
                    formEditTrigger.catDict = treeDict;
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
                string groupName = string.Empty;
                if(treeViewCats.SelectedNode != null)
                {
                    if(treeViewCats.SelectedNode.Parent != null)
                        groupName = treeViewCats.SelectedNode.Parent.Text;
                    else
                        groupName = treeViewCats.SelectedNode.Text;
                }

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
                                sb.Append(Macros.TriggerToMacro(trigger, groupName));
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
                                string m3 = string.Format(" to macro file {0}\n\nIn EQII chat enter:\n/do_file_commands {1}", doFileName, doFileName);
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

        private bool DeleteTrigger(CustomTrigger trigger, bool silently, bool populate)
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
                        if(populate)
                            PopulateCatsTree(null);
                        UpdateTriggerList(category);
                        //if the category disappeared, remove it from the "enable on zone-in" list
                        int autoIndex = -1;
                        for (int i= 0; i < config.autoCats.Count; i++)
                        {
                            string s = config.autoCats[i];
                            if (!treeDict.ContainsKey(s))
                                autoIndex = i;
                        }
                        if (autoIndex > -1)
                            config.autoCats.RemoveAt(autoIndex);

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

        #endregion Trigger Context Menu

        #endregion Triggers Panel

    }

}
