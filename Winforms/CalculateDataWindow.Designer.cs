namespace GeoLib.Winforms
{
    partial class CalculateDataWindow
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
            this.Cancel = new System.Windows.Forms.Button();
            this.Ok = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.LoadFile = new System.Windows.Forms.Button();
            this.textBoxFileToOpen = new System.Windows.Forms.TextBox();
            this.textBoxScaleFactor = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(490, 83);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 0;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // Ok
            // 
            this.Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Ok.Location = new System.Drawing.Point(409, 83);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 23);
            this.Ok.TabIndex = 1;
            this.Ok.Text = "Ok";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // LoadFile
            // 
            this.LoadFile.Location = new System.Drawing.Point(515, 13);
            this.LoadFile.Name = "LoadFile";
            this.LoadFile.Size = new System.Drawing.Size(29, 23);
            this.LoadFile.TabIndex = 2;
            this.LoadFile.Text = "...";
            this.LoadFile.UseVisualStyleBackColor = true;
            this.LoadFile.Click += new System.EventHandler(this.LoadFile_Click);
            // 
            // textBoxFileToOpen
            // 
            this.textBoxFileToOpen.Location = new System.Drawing.Point(12, 13);
            this.textBoxFileToOpen.Name = "textBoxFileToOpen";
            this.textBoxFileToOpen.Size = new System.Drawing.Size(497, 20);
            this.textBoxFileToOpen.TabIndex = 3;
            // 
            // textBoxScaleFactor
            // 
            this.textBoxScaleFactor.Location = new System.Drawing.Point(52, 39);
            this.textBoxScaleFactor.Name = "textBoxScaleFactor";
            this.textBoxScaleFactor.Size = new System.Drawing.Size(100, 20);
            this.textBoxScaleFactor.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Scale";
            // 
            // CalculateDataWindow
            // 
            this.AcceptButton = this.Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(577, 118);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxScaleFactor);
            this.Controls.Add(this.textBoxFileToOpen);
            this.Controls.Add(this.LoadFile);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.Cancel);
            this.MaximumSize = new System.Drawing.Size(593, 157);
            this.Name = "CalculateDataWindow";
            this.Text = "CalculateDataWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button LoadFile;
        private System.Windows.Forms.TextBox textBoxFileToOpen;
        private System.Windows.Forms.TextBox textBoxScaleFactor;
        private System.Windows.Forms.Label label1;
    }
}