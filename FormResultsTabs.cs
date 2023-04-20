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
            public ListView listACT;        // reference the ACT list
            public HeaderListView listTT;   // our mirror of the ACT list

            public TabInfo(string title, ListView list)
            {
                this.title = title;
                this.listACT = list;
                this.listTT = new HeaderListView();
                listTT.Dock = DockStyle.Fill;
                listTT.ListView.ListViewItemSorter = new ListViewDateComparer(0);
                listTT.ListView.Sorting = SortOrder.Descending;
                listTT.ListView.View = View.Details;
                listTT.Header.Text = title;
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
                AdjustLastColumnToFill(ti.listTT.ListView);
            }
        }

        private void FormResultsTabs_ResizeEnd(object sender, EventArgs e)
        {
            foreach (TabInfo ti in tabs)
            {
                AdjustLastColumnToFill(ti.listTT.ListView);
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
            if(tab != null)
            {
                ListView lv = null;
                foreach (Control ctrl in tab.Controls)
                {
                    if (ctrl.GetType() == typeof(ListView))
                    {
                        ListView listView = (ListView)ctrl;
                        if(listView != null)
                        {
                            lv = listView;
                            break;
                        }
                    }
                }

                if(lv != null)
                {
                    if (!ContainsLV(lv.Handle))
                    {
                        TabInfo ti = new TabInfo(tab.Text, lv);
                        // hide it while it's empty
                        ti.listTT.Visible = false;
                        tabs.Add(ti);
                        AddPanel(ti);
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
                    // this probably leaves corner cases where the user has cleared the tab in ACT
                    // and we miss it due to race conditions (or equality) on the item count,
                    // but this is quick and easy and covers most cases
                    if (ti.listTT.ListView.Items.Count > ti.listACT.Items.Count)
                    {
                        ti.listTT.ListView.Items.Clear();
                    }

                    // copy any "new" items from ACT's list to our list
                    if (ti.listTT.ListView.Items.Count != ti.listACT.Items.Count && ti.listACT.Items.Count > 0)
                    {
                        foreach (ListViewItem item in ti.listACT.Items)
                        {
                            ListViewItem clone = (ListViewItem)item.Clone();
                            clone.Name = ti.title + "!" + item.Text;
                            if (!ti.listTT.ListView.Items.ContainsKey(clone.Name))
                            {
                                ti.listTT.ListView.Items.Insert(0, clone);
                                if (!this.Visible && _config.ResultsPopup)
                                    this.Show();
                            }
                        }
                    }

                    if (ti.listACT.Items.Count == 0)
                    {
                        if (ti.listTT.Visible)
                        {
                            // change from visible to not
                            ti.listTT.Visible = false;
                            ReProportionPanel();
                        }
                    }
                    else
                    {
                        if (!ti.listTT.Visible)
                        {
                            // change to visible
                            ti.listTT.Visible = true;
                            AdjustLastColumnToFill(ti.listTT.ListView);
                            SetTopPanel(ti);
                            ReProportionPanel();
                        }
                    }
                }
            }
        }

        private void AdjustLastColumnToFill(ListView lvw)
        {
            Int32 nWidth = lvw.ClientSize.Width; // Get width of client area.

            // Loop through all columns except the last one.
            for (Int32 i = 0; i < lvw.Columns.Count - 1; i++)
            {
                // Subtract width of the column from the width
                // of the client area.
                nWidth -= lvw.Columns[i].Width;

                // If the width goes below 1, then no need to keep going
                // because the last column can't be sized to fit due to
                // the widths of the columns before it.
                if (nWidth < 1)
                    break;
            };

            // If there is any width remaining, that will
            // be the width of the last column.
            if (nWidth > 0  && lvw.Columns.Count > 0)
                lvw.Columns[lvw.Columns.Count - 1].Width = nWidth;
        }

        public void DeInit()
        {
            timer.Stop();
            timer.Enabled = false;
            disposed = true;
        }

        public bool ContainsLV(IntPtr handle)
        {
            // the tab title is cosmetic and not unique, use the window handle
            foreach(TabInfo ti in tabs)
            {
                if(ti.listACT.Handle.Equals(handle))
                    return true;
            }
            return false;
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
                if (vis == 1)
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                else
                {
                    // make each row the same size
                    float per = 100f / vis;
                    if (per < 10) per = 10;
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
                            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, per));
                        else
                            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
                    }
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
                colTT.Width = -2; // autosize
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
