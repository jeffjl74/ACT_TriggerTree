using System.Windows.Forms;

namespace ACT_TriggerTree
{
    /// <summary>
    /// Toolstrip that activates a button press regardless of whether the form had focus immediately prior.
    /// </summary>
    public class MyToolStrip : ToolStrip
    {
        const uint WM_LBUTTONDOWN = 0x201;
        const uint WM_LBUTTONUP = 0x202;

        static private bool down = false;

        /// <summary>
        /// Takes advantage of the fact that if the form did not have focus, we did not see the WM_LBUTTONDOWN message
        /// but we still see the WM_LBUTTONUP message because by that time the form has the focus. 
        /// So we generate a down message, which will activate the button.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP && !down)
            {
                m.Msg = (int)WM_LBUTTONDOWN;
                base.WndProc(ref m);
                m.Msg = (int)WM_LBUTTONUP;
            }

            if (m.Msg == WM_LBUTTONDOWN) down = true;
            if (m.Msg == WM_LBUTTONUP) down = false;

            base.WndProc(ref m);
        }
    }
}
