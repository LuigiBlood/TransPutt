namespace TransPutt.Games
{
    partial class MarvelousForm
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
            this.comboBoxLang1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownID1 = new System.Windows.Forms.NumericUpDown();
            this.textBoxDesc1 = new System.Windows.Forms.TextBox();
            this.buttonSave1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxPreview1 = new System.Windows.Forms.PictureBox();
            this.textBoxText1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownID1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxLang1
            // 
            this.comboBoxLang1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLang1.FormattingEnabled = true;
            this.comboBoxLang1.Location = new System.Drawing.Point(12, 29);
            this.comboBoxLang1.Name = "comboBoxLang1";
            this.comboBoxLang1.Size = new System.Drawing.Size(262, 21);
            this.comboBoxLang1.TabIndex = 0;
            this.comboBoxLang1.SelectedIndexChanged += new System.EventHandler(this.comboBoxLang1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Notes:";
            // 
            // numericUpDownID1
            // 
            this.numericUpDownID1.Location = new System.Drawing.Point(375, 30);
            this.numericUpDownID1.Name = "numericUpDownID1";
            this.numericUpDownID1.Size = new System.Drawing.Size(69, 20);
            this.numericUpDownID1.TabIndex = 2;
            // 
            // textBoxDesc1
            // 
            this.textBoxDesc1.Location = new System.Drawing.Point(12, 90);
            this.textBoxDesc1.Multiline = true;
            this.textBoxDesc1.Name = "textBoxDesc1";
            this.textBoxDesc1.Size = new System.Drawing.Size(262, 42);
            this.textBoxDesc1.TabIndex = 3;
            // 
            // buttonSave1
            // 
            this.buttonSave1.Location = new System.Drawing.Point(199, 355);
            this.buttonSave1.Name = "buttonSave1";
            this.buttonSave1.Size = new System.Drawing.Size(75, 23);
            this.buttonSave1.TabIndex = 4;
            this.buttonSave1.Text = "Save";
            this.buttonSave1.UseVisualStyleBackColor = true;
            this.buttonSave1.Click += new System.EventHandler(this.buttonSave1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Text:";
            // 
            // pictureBoxPreview1
            // 
            this.pictureBoxPreview1.Location = new System.Drawing.Point(12, 254);
            this.pictureBoxPreview1.Name = "pictureBoxPreview1";
            this.pictureBoxPreview1.Size = new System.Drawing.Size(262, 95);
            this.pictureBoxPreview1.TabIndex = 6;
            this.pictureBoxPreview1.TabStop = false;
            // 
            // textBoxText1
            // 
            this.textBoxText1.Location = new System.Drawing.Point(12, 163);
            this.textBoxText1.Multiline = true;
            this.textBoxText1.Name = "textBoxText1";
            this.textBoxText1.Size = new System.Drawing.Size(262, 74);
            this.textBoxText1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Language 1:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(318, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Script ID:";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(277, 254);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 95);
            this.vScrollBar1.TabIndex = 10;
            // 
            // MarvelousForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 387);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxText1);
            this.Controls.Add(this.pictureBoxPreview1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSave1);
            this.Controls.Add(this.textBoxDesc1);
            this.Controls.Add(this.numericUpDownID1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxLang1);
            this.Name = "MarvelousForm";
            this.Text = "Marvelous Text Editor";
            this.Load += new System.EventHandler(this.MarvelousForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownID1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxLang1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownID1;
        private System.Windows.Forms.TextBox textBoxDesc1;
        private System.Windows.Forms.Button buttonSave1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBoxPreview1;
        private System.Windows.Forms.TextBox textBoxText1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.VScrollBar vScrollBar1;
    }
}