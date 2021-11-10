namespace WindowsFormsApp.Custom_Views
{
    partial class OhlcChartForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.timeFrame_ComboBox = new System.Windows.Forms.ComboBox();
            this.timeFrameSize_textBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.timeFrameSize_textBox);
            this.splitContainer1.Panel1.Controls.Add(this.timeFrame_ComboBox);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 37;
            this.splitContainer1.TabIndex = 0;
            // 
            // timeFrame_ComboBox
            // 
            this.timeFrame_ComboBox.DisplayMember = "Time Frame";
            this.timeFrame_ComboBox.FormattingEnabled = true;
            this.timeFrame_ComboBox.Location = new System.Drawing.Point(12, 3);
            this.timeFrame_ComboBox.Name = "timeFrame_ComboBox";
            this.timeFrame_ComboBox.Size = new System.Drawing.Size(81, 24);
            this.timeFrame_ComboBox.TabIndex = 0;
            // 
            // timeFrameSize_textBox
            // 
            this.timeFrameSize_textBox.Location = new System.Drawing.Point(99, 3);
            this.timeFrameSize_textBox.Name = "timeFrameSize_textBox";
            this.timeFrameSize_textBox.Size = new System.Drawing.Size(40, 22);
            this.timeFrameSize_textBox.TabIndex = 3;
            // 
            // OhlcCharForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "OhlcCharForm";
            this.Text = "OhlcCharForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox timeFrame_ComboBox;
        private System.Windows.Forms.TextBox timeFrameSize_textBox;
    }
}