namespace GDI
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
            this.btn_bs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_bs
            // 
            this.btn_bs.Location = new System.Drawing.Point(389, 416);
            this.btn_bs.Name = "btn_bs";
            this.btn_bs.Size = new System.Drawing.Size(296, 72);
            this.btn_bs.TabIndex = 0;
            this.btn_bs.Text = "后台";
            this.btn_bs.UseVisualStyleBackColor = true;
            this.btn_bs.Click += new System.EventHandler(this.btn_bs_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1375, 830);
            this.Controls.Add(this.btn_bs);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_bs;
    }
}