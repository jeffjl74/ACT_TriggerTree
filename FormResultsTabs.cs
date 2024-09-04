using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ACT_TriggerTree
{
    public partial class FormResultsTabs : Form
    {
        bool disposed = false;
        bool initializing = true;
        System.Timers.Timer timer = new System.Timers.Timer();
        Config _config;

        // each results tab in ACT is represented here by an instance of this class
        internal class TabInfo
        {
            public string title;            // cosmetic ACT tab name
            public ListViewNoFlicker listACT;        // reference the ACT list
            public TabPage tabPageACT;      // reference used to make sure the tab still exists in ACT
            public HeaderListView listTT;   // our mirror of the ACT list

            public TabInfo(TabPage tab, ListViewNoFlicker list)
            {
                this.tabPageACT = tab;
                this.title = tab.Text;
                this.listACT = list;
                this.listTT = new HeaderListView(title);
                listTT.Dock = DockStyle.Fill;
            }

            // for debug
            public override string ToString()
            {
                return String.Format("{0} Visible:{1}", title, listTT.Visible);
            }
        }
        List<TabInfo> tabs = new List<TabInfo>();

        public FormResultsTabs(Config config)
        {
            InitializeComponent();

            _config = config;

            // we will check for new ACT list contents on this timer tick
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            timer.SynchronizingObject = ActGlobals.oFormActMain;
            timer.Enabled = true;
            timer.Start();
        }

        // do not take the focus when the form is shown
        // but we do want topmost
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
        private const int WS_EX_TOPMOST = 0x00000008;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_TOPMOST;
                return createParams;
            }
        }

        private void FormResultsTabs_Load(object sender, EventArgs e)
        {
            //AdjustLastColumnToFill(listView1);
        }

        private void FormResultsTabs_Shown(object sender, EventArgs e)
        {
            if(_config.ResultsSize.Height > 0 && _config.ResultsSize.Width > 0)
            {
                this.Height = _config.ResultsSize.Height;
                this.Width = _config.ResultsSize.Width;
            }
            this.Location = new Point(_config.ResultsLoc.X, _config.ResultsLoc.Y);
            initializing = false;

            foreach (TabInfo ti in tabs)
            {
                ti.listTT.AdjustColumnsToFit();
            }
        }

        private void FormResultsTabs_ResizeEnd(object sender, EventArgs e)
        {
            foreach (TabInfo ti in tabs)
            {
                ti.listTT.AdjustColumnsToFit();
            }
            ReProportionPanel();

            if (!initializing)
            {
                _config.ResultsLoc.X = this.Location.X;
                _config.ResultsLoc.Y = this.Location.Y;
                _config.ResultsSize.Width = this.Width;
                _config.ResultsSize.Height = this.Height;
            }
        }

        private void FormResultsTabs_Move(object sender, EventArgs e)
        {
            if (!initializing)
            {
                _config.ResultsLoc.X = this.Location.X;
                _config.ResultsLoc.Y = this.Location.Y;
                _config.ResultsSize.Width = this.Width;
                _config.ResultsSize.Height = this.Height;
            }
}

        public void AddTab(TabPage tab)
        {
            if(tab != null && tab.Controls != null && tab.Controls.Count > 0 && tab.Controls[0].Controls.Count > 0)
            {
                Control.ControlCollection coll = tab.Controls[0].Controls;
                ListViewNoFlicker lv = null;
                foreach (Control ctrl in coll)
                {
                    if (ctrl.GetType() == typeof(ListViewNoFlicker))
                    {
                        ListViewNoFlicker listView = (ListViewNoFlicker)ctrl;
                        if(listView != null)
                        {
                            lv = listView;
                            break;
                        }
                    }
                }

                if (lv != null)
                {
                    TabInfo ti = ContainsLV(lv.Handle);
                    if (ti == null)
                    {
                        timer.Stop();

                        ti = new TabInfo(tab, lv);
                        // hide it while it's empty
                        ti.listTT.Visible = false;
                        tabs.Add(ti);
                        AddPanel(ti);

                        timer.Start();
                    }
                    else
                    {
                        // make sure the tab name is up to date
                        ti.listTT.Header = tab.Text;
                    }
                }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!disposed)
            {
                foreach (TabInfo ti in tabs)
                {
                    if (ti.tabPageACT == null)
                        continue; //unchecking the "Add Results Tab" doesn't seem to destroy the tab page, but just in case

                    // this probably leaves corner cases where the user has cleared or disabled the tab in ACT
                    // and we miss it due to race conditions (or equality) on the item count,
                    // but this is quick and easy and covers most cases
                    if ((ti.listTT.ListView.Items.Count > ti.listACT.Items.Count) 
                        || (ti.tabPageACT.Parent == null && ti.listTT.ListView.Items.Count > 0))
                    {
                        ti.listTT.Clear();
                    }

                    // copy any "new" items from ACT's list to our list
                    if (ti.listTT.ListView.Items.Count != ti.listACT.Items.Count 
                        && ti.listACT.Items.Count > 0 
                        && ti.tabPageACT.Parent != null) // null parent = ACT hid the tab b/c user disabled it
                    {
                        foreach (ListViewItem item in ti.listACT.Items)
                        {
                            ListViewItem clone = (ListViewItem)item.Clone();
                            clone.Name = ti.title + "!" + item.Text;
                            if (!ti.listTT.ListView.Items.ContainsKey(clone.Name))
                            {
                                ti.listTT.Insert(clone);
                                if (!this.Visible && _config.ResultsPopup)
                                    this.Show();
                                if (tableLayoutPanel1.GetRow(ti.listTT) != 0)
                                    SetTopPanel(ti);
                            }
                        }
                    }

                    if (ti.listACT.Items.Count == 0 
                        || (ti.tabPageACT.Parent == null && ti.listTT.ListView.Items.Count == 0)) // need to hide it since ACT hid it?
                    {
                        if (ti.listTT.Visible)
                        {
                            // change from visible to not
                            ti.listTT.Visible = false;
                            ReProportionPanel();
                        }
                    }
                    else if(ti.listACT.Items.Count > 0)
                    {
                        if (!ti.listTT.Visible)
                        {
                            // change to visible
                            ti.listTT.Visible = true;
                            SetTopPanel(ti);
                            ReProportionPanel();
                        }
                    }
                }
            }
        }

        public void DeInit()
        {
            timer.Stop();
            timer.Enabled = false;
            disposed = true;
        }

        private TabInfo ContainsLV(IntPtr handle)
        {
            // the tab title is cosmetic and not unique, use the window handle
            foreach(TabInfo ti in tabs)
            {
                if(ti.listACT.Handle.Equals(handle))
                    return ti;
            }
            return null;
        }

        private void ReProportionPanel()
        {
            // count visible controls
            float vis = 0;
            foreach(TabInfo ti in tabs)
            {
                if(ti.listTT.Visible)
                    vis++;
            }

            if (vis > 0)
            {
                tableLayoutPanel1.RowStyles.Clear();
                // make each row the same size
                float per = 100f / vis;
                SizeType sizeType = SizeType.Percent;
                tableLayoutPanel1.AutoScroll = false;
                if (per < 20)
                {
                    // if there are more than 5 tabs, limit the min size
                    sizeType = SizeType.Absolute;
                    per = 100;
                    tableLayoutPanel1.AutoScroll = true;
                }
                float[] sizes = new float[tableLayoutPanel1.RowCount];
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    // since we re-order rows,
                    // the style for control index "i" may not be at style index "i"
                    // so make a cross-reference
                    int row = tableLayoutPanel1.GetRow(tableLayoutPanel1.Controls[i]);
                    if (tableLayoutPanel1.Controls[i].Visible)
                        sizes[row] = per;
                    else
                        sizes[row] = 0;
                }
                foreach(float size in sizes)
                {
                    if (size > 0)
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(sizeType, per));
                    else
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
                }
                tableLayoutPanel1.Refresh();
            }
        }

        private void SetTopPanel(TabInfo ti)
        {
            // put the passed control at the top of the form

            Dictionary<int, int> order = new Dictionary<int, int>();

            int currentRow = tableLayoutPanel1.GetRow(ti.listTT);
            if (currentRow != 0)
            {
                // make a map of the current row order
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    HeaderListView hlv = tableLayoutPanel1.Controls[i] as HeaderListView;
                    if (hlv != null)
                    {
                        int rownum = tableLayoutPanel1.GetRow(hlv);
                        order.Add(i, rownum);
                    }
                }
                // rearrange so the passed control is in row 0
                // and everything from the top row to
                // where the passed control used to be is shifted down
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    int rownum;
                    if (order.TryGetValue(i, out rownum))
                    {
                        if(rownum == currentRow)
                            tableLayoutPanel1.SetRow(tableLayoutPanel1.Controls[i], 0);
                        else if(rownum < currentRow)
                            tableLayoutPanel1.SetRow(tableLayoutPanel1.Controls[i], rownum + 1);
                    }
                }
                ReProportionPanel();
                tableLayoutPanel1.Refresh();
            }
        }

        private void AddPanel(TabInfo ti)
        {
            if (tableLayoutPanel1.RowCount == 1 && tableLayoutPanel1.Controls.Count == 0)
            {
                // account for the empty TableLayoutPanel we added in the Deisgner
                // we will add the listview to the Designer panel row 0 after the else {}
            }
            else
            {
                // add a new row
                tableLayoutPanel1.RowCount++;
            }
            // add our mirror listview as the bottom row
            tableLayoutPanel1.Controls.Add(ti.listTT, 0, tableLayoutPanel1.RowCount-1);
            // mirror the columns from ACT
            foreach(ColumnHeader col in ti.listACT.Columns)
            {
                ColumnHeader colTT = (ColumnHeader)col.Clone();
                // autosize
                //colTT.Width = -2;
                ti.listTT.ListView.Columns.Add(colTT);
            }
            ReProportionPanel();
        }

        private void FormResultsTabs_FormClosing(object sender, FormClosingEventArgs e)
        {
            // just hide if the user hits the X button
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
