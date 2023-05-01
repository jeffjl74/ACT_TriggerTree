using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACT_TriggerTree
{
    public partial class HeaderListView : UserControl
    {
        public ListView ListView { get { return listView1; }  }
        public string Header { get { return labelHeader.Text; } set { labelHeader.Text = value; } }

        DateTime DisplayMinimum = DateTime.MinValue;

        public HeaderListView(string header)
        {
            InitializeComponent();

            ListView.ListViewItemSorter = new ListViewDateComparer(0);
            ListView.Sorting = SortOrder.Descending;
            ListView.View = View.Details;
            labelHeader.Text = header;
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // do not draw any items whose timestamp is less than DisplayMinimum
            try
            {
                DateTime date = Convert.ToDateTime(e.Item.SubItems[0].Text);
                if (date > DisplayMinimum)
                    e.DrawDefault = true;
                else
                    e.DrawDefault = false;
            } 
            catch
            {
                e.DrawDefault = true;
            }
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                DisplayMinimum = DateTime.Now;
            }
            else
            {
                DisplayMinimum = DateTime.MinValue;
            }
            listView1.Refresh();
        }

        public void Clear()
        {
            listView1.Items.Clear();
            checkBoxHide.Checked = false;
        }

        public void AdjustColumnsToFit()
        {
            // size for contents
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // make sure the headers fit
            ListView.ColumnHeaderCollection cc = listView1.Columns;
            for (int i = 0; i < cc.Count; i++)
            {
                int colWidth = TextRenderer.MeasureText(cc[i].Text, listView1.Font).Width + 10;
                if (colWidth > cc[i].Width)
                {
                    cc[i].Width = colWidth;
                }
            }
        }

        public void Insert(ListViewItem item)
        {
            listView1.Items.Insert(0, item);
            AdjustColumnsToFit();
        }

    }
}
