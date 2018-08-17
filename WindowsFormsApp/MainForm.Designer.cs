namespace WindowsFormsApp
{
    partial class MainForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.hlocChart1 = new WindowsFormsApp.HlocChart();
            this.hlocChart2 = new WindowsFormsApp.HlocChart();
            this.hlocChart3 = new WindowsFormsApp.HlocChart();
            this.hlocChart4 = new WindowsFormsApp.HlocChart();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(720, 409);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.hlocChart1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.hlocChart2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.hlocChart3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.hlocChart4, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 409);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // hlocChart1
            // 
            this.hlocChart1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hlocChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hlocChart1.LegAnalyzer = null;
            this.hlocChart1.Location = new System.Drawing.Point(1, 1);
            this.hlocChart1.Margin = new System.Windows.Forms.Padding(1);
            this.hlocChart1.Name = "hlocChart1";
            this.hlocChart1.Size = new System.Drawing.Size(292, 202);
            this.hlocChart1.TabIndex = 1;
            // 
            // hlocChart2
            // 
            this.hlocChart2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hlocChart2.LegAnalyzer = null;
            this.hlocChart2.Location = new System.Drawing.Point(296, 2);
            this.hlocChart2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.hlocChart2.Name = "hlocChart2";
            this.hlocChart2.Size = new System.Drawing.Size(291, 200);
            this.hlocChart2.TabIndex = 2;
            // 
            // hlocChart3
            // 
            this.hlocChart3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hlocChart3.LegAnalyzer = null;
            this.hlocChart3.Location = new System.Drawing.Point(2, 206);
            this.hlocChart3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.hlocChart3.Name = "hlocChart3";
            this.hlocChart3.Size = new System.Drawing.Size(290, 201);
            this.hlocChart3.TabIndex = 3;
            // 
            // hlocChart4
            // 
            this.hlocChart4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hlocChart4.LegAnalyzer = null;
            this.hlocChart4.Location = new System.Drawing.Point(296, 206);
            this.hlocChart4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.hlocChart4.Name = "hlocChart4";
            this.hlocChart4.Size = new System.Drawing.Size(291, 201);
            this.hlocChart4.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 409);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private HlocChart hlocChart1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private HlocChart hlocChart2;
        private HlocChart hlocChart3;
        private HlocChart hlocChart4;
    }
}

