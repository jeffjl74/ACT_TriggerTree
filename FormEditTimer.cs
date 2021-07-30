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

namespace ACT_TriggerTree
{
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
}
