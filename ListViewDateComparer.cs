using System;
using System.Collections;
using System.Windows.Forms;

namespace ACT_TriggerTree
{
    class ListViewDateComparer : IComparer
    {
        private int col;
        public ListViewDateComparer()
        {
            col = 0;
        }
        public ListViewDateComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            int returnVal;
            try
            {
                DateTime dateX = Convert.ToDateTime(((ListViewItem)x).SubItems[col].Text);
                DateTime dateY = Convert.ToDateTime(((ListViewItem)y).SubItems[col].Text);
                // sort decending
                returnVal = dateY.CompareTo(dateX);
            }
            catch
            {
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, (((ListViewItem)y).SubItems[col].Text));
            }
            return returnVal;
        }
    }
}
