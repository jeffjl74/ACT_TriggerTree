using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace FormEditTimer
{
    public partial class FormEditTimer : Form
    {
        CustomTrigger editingTrigger;
        public event EventHandler EditDoneEvent; //callback

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
            textBoxName.Text = editingTrigger.TimerName;
            textBoxName.SelectAll();
        }
    }
}
