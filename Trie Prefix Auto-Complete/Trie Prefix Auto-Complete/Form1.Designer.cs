namespace Trie_Prefix_Auto_Complete
{
    partial class Form1
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
            this.entryBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // entryBox
            // 
            this.entryBox.AcceptsReturn = true;
            this.entryBox.Location = new System.Drawing.Point(15, 19);
            this.entryBox.Name = "entryBox";
            this.entryBox.Size = new System.Drawing.Size(426, 20);
            this.entryBox.TabIndex = 0;
            this.entryBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.entryBox.TextChanged += new System.EventHandler(this.entryBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.entryBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 51);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text Input";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 572);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Suggestions";
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(15, 20);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(426, 537);
            this.listBox.TabIndex = 0;
            this.listBox.Enter += new System.EventHandler(this.listBox_Enter);
            this.listBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listBox_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(480, 663);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(496, 702);
            this.MinimumSize = new System.Drawing.Size(496, 702);
            this.Name = "Form1";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VelaBorja\'s Trie Prefix Auto-Complete";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox entryBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox;
    }
}

