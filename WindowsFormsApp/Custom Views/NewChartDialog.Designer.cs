namespace WindowsFormsApp.Custom_Views
{
    partial class NewChartDialog
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
            this.label_symbol = new System.Windows.Forms.Label();
            this.label_timeFrame = new System.Windows.Forms.Label();
            this.comboBox_timeFrame = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_timeFrame_size = new System.Windows.Forms.TextBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.dateTimePicker_from = new System.Windows.Forms.DateTimePicker();
            this.label_from = new System.Windows.Forms.Label();
            this.label_to = new System.Windows.Forms.Label();
            this.dateTimePicker_to = new System.Windows.Forms.DateTimePicker();
            this.comboBox_symbols = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label_symbol
            // 
            this.label_symbol.AutoSize = true;
            this.label_symbol.Location = new System.Drawing.Point(38, 44);
            this.label_symbol.Name = "label_symbol";
            this.label_symbol.Size = new System.Drawing.Size(65, 20);
            this.label_symbol.TabIndex = 0;
            this.label_symbol.Text = "Symbol:";
            // 
            // label_timeFrame
            // 
            this.label_timeFrame.AutoSize = true;
            this.label_timeFrame.Location = new System.Drawing.Point(38, 86);
            this.label_timeFrame.Name = "label_timeFrame";
            this.label_timeFrame.Size = new System.Drawing.Size(93, 20);
            this.label_timeFrame.TabIndex = 2;
            this.label_timeFrame.Text = "TimeFrame:";
            // 
            // comboBox_timeFrame
            // 
            this.comboBox_timeFrame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_timeFrame.FormattingEnabled = true;
            this.comboBox_timeFrame.Location = new System.Drawing.Point(148, 78);
            this.comboBox_timeFrame.Name = "comboBox_timeFrame";
            this.comboBox_timeFrame.Size = new System.Drawing.Size(121, 28);
            this.comboBox_timeFrame.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(310, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Size:";
            // 
            // textBox_timeFrame_size
            // 
            this.textBox_timeFrame_size.Location = new System.Drawing.Point(360, 80);
            this.textBox_timeFrame_size.Name = "textBox_timeFrame_size";
            this.textBox_timeFrame_size.Size = new System.Drawing.Size(100, 26);
            this.textBox_timeFrame_size.TabIndex = 6;
            this.textBox_timeFrame_size.Text = "1";
            this.textBox_timeFrame_size.TextChanged += new System.EventHandler(this.textBox_timeFrame_size_TextChanged);
            // 
            // button_ok
            // 
            this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ok.Location = new System.Drawing.Point(126, 374);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 69);
            this.button_ok.TabIndex = 7;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(279, 374);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 69);
            this.button_cancel.TabIndex = 8;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker_from
            // 
            this.dateTimePicker_from.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_from.Location = new System.Drawing.Point(94, 162);
            this.dateTimePicker_from.Name = "dateTimePicker_from";
            this.dateTimePicker_from.Size = new System.Drawing.Size(200, 26);
            this.dateTimePicker_from.TabIndex = 9;
            this.dateTimePicker_from.Value = new System.DateTime(2018, 8, 17, 0, 0, 0, 0);
            // 
            // label_from
            // 
            this.label_from.AutoSize = true;
            this.label_from.Location = new System.Drawing.Point(38, 162);
            this.label_from.Name = "label_from";
            this.label_from.Size = new System.Drawing.Size(50, 20);
            this.label_from.TabIndex = 10;
            this.label_from.Text = "From:";
            // 
            // label_to
            // 
            this.label_to.AutoSize = true;
            this.label_to.Location = new System.Drawing.Point(38, 218);
            this.label_to.Name = "label_to";
            this.label_to.Size = new System.Drawing.Size(31, 20);
            this.label_to.TabIndex = 11;
            this.label_to.Text = "To:";
            // 
            // dateTimePicker_to
            // 
            this.dateTimePicker_to.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_to.Location = new System.Drawing.Point(94, 218);
            this.dateTimePicker_to.Name = "dateTimePicker_to";
            this.dateTimePicker_to.Size = new System.Drawing.Size(200, 26);
            this.dateTimePicker_to.TabIndex = 12;
            // 
            // comboBox_symbols
            // 
            this.comboBox_symbols.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox_symbols.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_symbols.FormattingEnabled = true;
            this.comboBox_symbols.Location = new System.Drawing.Point(148, 36);
            this.comboBox_symbols.MaxDropDownItems = 30;
            this.comboBox_symbols.Name = "comboBox_symbols";
            this.comboBox_symbols.Size = new System.Drawing.Size(121, 27);
            this.comboBox_symbols.TabIndex = 13;
            this.comboBox_symbols.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_symbols_DrawItem);
            this.comboBox_symbols.SelectedIndexChanged += new System.EventHandler(this.comboBox_symbols_SelectedIndexChanged);
            // 
            // NewChartOptionsPicker
            // 
            this.AcceptButton = this.button_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(506, 552);
            this.Controls.Add(this.comboBox_symbols);
            this.Controls.Add(this.dateTimePicker_to);
            this.Controls.Add(this.label_to);
            this.Controls.Add(this.label_from);
            this.Controls.Add(this.dateTimePicker_from);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.textBox_timeFrame_size);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_timeFrame);
            this.Controls.Add(this.label_timeFrame);
            this.Controls.Add(this.label_symbol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "NewChartOptionsPicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NewChartOptionsPicker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_symbol;
        private System.Windows.Forms.Label label_timeFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        public System.Windows.Forms.ComboBox comboBox_timeFrame;
        public System.Windows.Forms.TextBox textBox_timeFrame_size;
        private System.Windows.Forms.Label label_from;
        private System.Windows.Forms.Label label_to;
        public System.Windows.Forms.DateTimePicker dateTimePicker_from;
        public System.Windows.Forms.DateTimePicker dateTimePicker_to;
        public System.Windows.Forms.ComboBox comboBox_symbols;
    }
}