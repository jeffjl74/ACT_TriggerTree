
namespace ACT_TriggerTree
{
    partial class FormHistogram
    {
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.textBoxTimerSec = new System.Windows.Forms.TextBox();
            this.buttonNewTimer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTimerName = new System.Windows.Forms.TextBox();
            this.textBoxCategory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonSwap = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTimerSec
            // 
            this.textBoxTimerSec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTimerSec.Location = new System.Drawing.Point(6, 112);
            this.textBoxTimerSec.Name = "textBoxTimerSec";
            this.textBoxTimerSec.Size = new System.Drawing.Size(108, 20);
            this.textBoxTimerSec.TabIndex = 0;
            // 
            // buttonNewTimer
            // 
            this.buttonNewTimer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonNewTimer.Location = new System.Drawing.Point(26, 172);
            this.buttonNewTimer.Name = "buttonNewTimer";
            this.buttonNewTimer.Size = new System.Drawing.Size(75, 23);
            this.buttonNewTimer.TabIndex = 1;
            this.buttonNewTimer.Text = "Make Timer";
            this.toolTip1.SetToolTip(this.buttonNewTimer, "Create a timer from the parameters above");
            this.buttonNewTimer.UseVisualStyleBackColor = true;
            this.buttonNewTimer.Click += new System.EventHandler(this.buttonNewTimer_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Seconds";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Timer Name";
            // 
            // textBoxTimerName
            // 
            this.textBoxTimerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTimerName.Location = new System.Drawing.Point(6, 19);
            this.textBoxTimerName.Name = "textBoxTimerName";
            this.textBoxTimerName.Size = new System.Drawing.Size(108, 20);
            this.textBoxTimerName.TabIndex = 4;
            // 
            // textBoxCategory
            // 
            this.textBoxCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCategory.Location = new System.Drawing.Point(6, 64);
            this.textBoxCategory.Name = "textBoxCategory";
            this.textBoxCategory.Size = new System.Drawing.Size(108, 20);
            this.textBoxCategory.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Category";
            // 
            // chart1
            // 
            chartArea6.AxisX.MajorGrid.Enabled = false;
            chartArea6.AxisX.Title = "Seconds Between";
            chartArea6.AxisY.MajorGrid.Enabled = false;
            chartArea6.AxisY.Title = "Occurrences";
            chartArea6.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea6);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend6.Enabled = false;
            legend6.Name = "Legend1";
            this.chart1.Legends.Add(legend6);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series6.ChartArea = "ChartArea1";
            series6.CustomProperties = "EmptyPointValue=Zero, MaxPixelPointWidth=15";
            series6.IsValueShownAsLabel = true;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chart1.Series.Add(series6);
            this.chart1.Size = new System.Drawing.Size(239, 207);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.chart1_GetToolTipText);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chart1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.buttonSwap);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxCategory);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxTimerName);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.buttonNewTimer);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxTimerSec);
            this.splitContainer1.Size = new System.Drawing.Size(369, 207);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 5;
            // 
            // buttonSwap
            // 
            this.buttonSwap.Font = new System.Drawing.Font("Wingdings 3", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonSwap.Location = new System.Drawing.Point(53, 43);
            this.buttonSwap.Name = "buttonSwap";
            this.buttonSwap.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.buttonSwap.Size = new System.Drawing.Size(25, 20);
            this.buttonSwap.TabIndex = 7;
            this.buttonSwap.Text = "B";
            this.toolTip1.SetToolTip(this.buttonSwap, "Swap between mob name and zone name");
            this.buttonSwap.UseVisualStyleBackColor = true;
            this.buttonSwap.Click += new System.EventHandler(this.buttonSwap_Click);
            // 
            // FormHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 207);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormHistogram";
            this.Text = "Histogram";
            this.Shown += new System.EventHandler(this.FormHistogram_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCategory;
        private System.Windows.Forms.TextBox textBoxTimerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonNewTimer;
        private System.Windows.Forms.TextBox textBoxTimerSec;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonSwap;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}