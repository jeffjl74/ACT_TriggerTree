using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ACT_TriggerTree
{
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
                    ActGlobals.oFormSpellTimers.RebuildSpellTreeView();

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
}
