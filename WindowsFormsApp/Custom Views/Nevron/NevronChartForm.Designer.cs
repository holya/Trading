
namespace WindowsFormsApp.Custom_Views
{
    partial class NevronChartForm
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
        private void InitializeComponent( )
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NevronChartForm));
            this.nevronChart = new Nevron.Chart.WinForm.NChartControl();
            this.SuspendLayout();
            // 
            // nevronChart
            // 
            this.nevronChart.AutoRefresh = false;
            this.nevronChart.BackColor = System.Drawing.SystemColors.Control;
            this.nevronChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nevronChart.InputKeys = new System.Windows.Forms.Keys[0];
            this.nevronChart.Location = new System.Drawing.Point(0, 0);
            this.nevronChart.Name = "nevronChart";
            this.nevronChart.Size = new System.Drawing.Size(800, 450);
            this.nevronChart.State = ((Nevron.Chart.WinForm.NState)(resources.GetObject("nevronChart.State")));
            this.nevronChart.TabIndex = 0;
            // 
            // NevronChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.nevronChart);
            this.Name = "NevronChartForm";
            this.Text = "NevronChartForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Nevron.Chart.WinForm.NChartControl nevronChart;
    }
}